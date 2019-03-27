using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Root.AuthenticationModels;
using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class UserRepository : RepositoryBase<User>, IUserRepository, IDisposable
	{
		private readonly IDbSet<User> _dbset;
		private SGTSVNDBContext _ctx;
		private UserManager<User> _userManager;
		private UserStore<User> _userStore;
		private RoleManager<IdentityRole> _roleManager;
		public UserRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{
			_dbset = DataContext.Set<User>();
			_ctx = DataContext;
			//_userManager = _userManager ?? new UserManager<User>(new UserStore<User>(_ctx));
			_userStore = _userStore ?? new UserStore<User>(_ctx);
			_userManager = _userManager ?? new UserManager<User>(_userStore);
			_roleManager = _roleManager ?? new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_ctx));
		}
		public IList<string> GetRoles(string id)
		{
			return _userManager.GetRoles(id);
		}

		public Task<IdentityRole> GetRoleByName(string name)
		{
			return _roleManager.FindByIdAsync(name);
		}

		public async Task<IdentityResult> RegisterUser(UserModel userModel)
		{
			User user = new User
			{
				UserName = userModel.UserName,
				EmployeeC = userModel.EmployeeC,
				DriverC = userModel.DriverC,
				IsMobileUser = userModel.IsMobileUser,
				IsActive = userModel.IsActive
			};

			var result = await _userManager.CreateAsync(user, userModel.Password);

			// get user
			var userInsert = await FindUser(userModel.UserName, userModel.Password);
			await _userManager.AddToRoleAsync(userInsert.Id, userModel.RoleName);

			return result;
		}

		public async Task<IdentityResult> UpdateUser(UserModel userModel)
		{
			IdentityResult result = null;
			// update Users table
			var userList = _ctx.Users.Where(u => u.UserName == userModel.UserName);
			var userUpdate = userList.FirstOrDefault();
			if (userUpdate != null)
			{
				userUpdate.EmployeeC = userModel.EmployeeC;
				userUpdate.DriverC = userModel.DriverC;
				if (userUpdate.IsActive != userModel.IsActive)
				{
					_userManager.UpdateSecurityStamp(userUpdate.Id);
				}
				userUpdate.IsActive = userModel.IsActive;
				
				result = await _userManager.UpdateAsync(userUpdate);

				var userRoles = await _userManager.GetRolesAsync(userUpdate.Id);
				if (userRoles.Count > 0)
				{
					await _userManager.RemoveFromRoleAsync(userUpdate.Id, userRoles[0]);
					await _userManager.AddToRoleAsync(userUpdate.Id, userModel.RoleName);
				}
				else
				{
					await _userManager.AddToRoleAsync(userUpdate.Id, userModel.RoleName);
				}
			}

			return result;
		}

		public async Task<IdentityResult> DeleteUser(UserModel userModel)
		{
			IdentityResult result = null;
			// update Users table
			var userList = _ctx.Users.Where(u => u.UserName == userModel.UserName);
			var userDelete = userList.FirstOrDefault();
			if (userDelete != null)
			{
				var id = userDelete.Id;
				await _userManager.RemoveFromRoleAsync(id, userModel.RoleName);
				result = await _userManager.DeleteAsync(userDelete);
			}

			return result;
		}

		public async Task<IdentityResult> SetActiveStatus(UserModel userModel)
		{
			IdentityResult result = null;
			// update Users table
			//var userList = _ctx.Users.Where(u => u.UserName == userModel.UserName);

			var userUpdate = await _userManager.FindByNameAsync(userModel.UserName);
			//var userUpdate = userList.FirstOrDefault();
			if (userUpdate != null)
			{
				userUpdate.IsActive = userModel.IsActive;
				result = await _userManager.UpdateAsync(userUpdate);
				_userStore.Context.SaveChanges();
				_userManager.UpdateSecurityStamp(userUpdate.Id);
			}

			return result;
		}

		public async Task<IdentityResult> ResetPassword(string userName, string defaultPassword)
		{
			IdentityResult result = null;
			// update Users table
			var userList = _ctx.Users.Where(u => u.UserName == userName);
			var userUpdate = userList.FirstOrDefault();
			if (userUpdate != null)
			{
				var hashedNewPassword = _userManager.PasswordHasher.HashPassword(defaultPassword);

				userUpdate.PasswordHash = hashedNewPassword;
				result = await _userManager.UpdateAsync(userUpdate);
				_userManager.UpdateSecurityStamp(userUpdate.Id);
			}

			return result;
		}

		public async Task<IdentityResult> ChangePassword(string userName, string password, string newPassword)
		{
			var userId = _userManager.FindByName(userName).Id;
			var result = await _userManager.ChangePasswordAsync(userId, password, newPassword);
			_userManager.UpdateSecurityStamp(userId);
			return result;
		}

		public async Task<User> FindUser(string userName, string password)
		{
			//Phat Nguyen - 14/05/2015
			//Every connection is using singleton design pattern, so when we find one user, the user is cached in this 
			//dbcontext. When we change this user from another connection, and get back to previous connection, the user with data changed
			//is not apply
			//Solutio: create new DBcontext everytime we need to get all user information.
			var context = new SGTSVNDBContext();
			using (var userManager = new UserManager<User>(new UserStore<User>(context)))
			{
				var user = await userManager.FindAsync(userName, password);
				//var userId = _userManager.FindByName(userName).Id;
				if (user != null && user.IsActive == "1")
				{
					if (user.IsLoggedIn != "1")
					{
						user.IsLoggedIn = "1";
						userManager.Update(user);
					}

					userManager.UpdateSecurityStamp(user.Id);
				}
				
				return user;
			}
		}
		public User GetUser(string userName, string password)
		{
			var context = new SGTSVNDBContext();
			using (var userManager = new UserManager<User>(new UserStore<User>(context)))
			{
				var user = userManager.Find(userName, password);
				return user;
			}
		}
		public Client FindClient(string clientId)
		{
			var client = _ctx.Clients.Find(clientId);

			return client;
		}
		public async Task<bool> AddRefreshToken(RefreshToken token)
		{

			var existingToken = _ctx.RefreshTokens.SingleOrDefault(r => r.Subject == token.Subject && r.ClientId == token.ClientId);

			if (existingToken != null)
			{
				var result = await RemoveRefreshToken(existingToken);
			}

			_ctx.RefreshTokens.Add(token);

			return await _ctx.SaveChangesAsync() > 0;
		}
		public async Task<bool> RemoveRefreshToken(string refreshTokenId)
		{
			var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

			if (refreshToken != null)
			{
				_ctx.RefreshTokens.Remove(refreshToken);
				return await _ctx.SaveChangesAsync() > 0;
			}

			return false;
		}
		public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
		{
			_ctx.RefreshTokens.Remove(refreshToken);
			return await _ctx.SaveChangesAsync() > 0;
		}
		public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
		{
			var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

			return refreshToken;
		}
		public List<RefreshToken> GetAllRefreshTokens()
		{
			return _ctx.RefreshTokens.ToList();
		}
		public async Task<User> FindAsync(UserLoginInfo loginInfo)
		{
			User user = await _userManager.FindAsync(loginInfo);

			return user;
		}
		public async Task<IdentityResult> CreateAsync(User user)
		{
			var result = await _userManager.CreateAsync(user);

			return result;
		}

		public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
		{
			var result = await _userManager.AddLoginAsync(userId, login);

			return result;
		}
		public bool CheckSecurityStamp(string userName, string clientSstp)
		{
			var context = new SGTSVNDBContext();
			using (new UserManager<User>(new UserStore<User>(context)))
			{
				var sstp = _userManager.FindByName(userName).SecurityStamp;
				if (clientSstp.Equals(sstp))
					return true;
			}
			return false;
		}
		public void Dispose()
		{
			_ctx.Dispose();
			_userManager.Dispose();
		}
	}
	public interface IUserRepository : IRepository<User>
	{
		IList<string> GetRoles(string id);
		Task<IdentityRole> GetRoleByName(string id);
		Task<IdentityResult> RegisterUser(UserModel userModel);
		Task<IdentityResult> UpdateUser(UserModel userModel);
		Task<IdentityResult> ResetPassword(string userName, string defaultPassword);
		Task<IdentityResult> ChangePassword(string userName, string password, string newPassword);
		Task<IdentityResult> DeleteUser(UserModel userModel);
		Task<IdentityResult> SetActiveStatus(UserModel userModel);
		Task<User> FindUser(string userName, string password);
		Client FindClient(string clientId);
		Task<bool> AddRefreshToken(RefreshToken token);
		Task<bool> RemoveRefreshToken(string refreshTokenId);
		Task<bool> RemoveRefreshToken(RefreshToken refreshToken);
		Task<RefreshToken> FindRefreshToken(string refreshTokenId);
		List<RefreshToken> GetAllRefreshTokens();
		Task<User> FindAsync(UserLoginInfo loginInfo);
		Task<IdentityResult> CreateAsync(User user);
		Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);
		bool CheckSecurityStamp(string userName, string clientSstp);
		User GetUser(string userName, string password);
	}
}
