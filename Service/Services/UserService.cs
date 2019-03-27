using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Smo;
using Root.AuthenticationModels;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Website.ViewModels.User;

namespace Service.Services
{
	public interface IUserService
	{
		UserDatatables GetUsersForTable(int page, int itemsPerPage, string sortBy, bool reverse, string userLogin,
			 string custSearchValue);

		UserDatatables GetMobileUsersForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string custSearchValue);

		UserViewModel GetUserByName(string userName);

		IEnumerable<UserViewModel> GetUsers(string value);

		IEnumerable<UserViewModel> GetMobileUsers(string value);

		bool CheckUserLoggedIn(string userName, string password);

		void SetUserLogOut(string userName);

		bool RegisterDeviceRegistrationId(string driverC, string registrationId, string platform);

		void SaveUser();

        string GetDeviceInfo(string userName);

        bool SetDeviceInfo(string userName, string deviceInfo);
    }
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IUserRolesRepository _userRolesRepository;
		private readonly IRoleRepository _roleRepository;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly IDriverRepository _driverRepository;
		private readonly IUnitOfWork _unitOfWork;

		public UserService(IUserRepository userRepository,
						   IUserRolesRepository userRolesRepository,
						   IRoleRepository roleRepository,
						   IEmployeeRepository employeeRepository,
						   IDriverRepository driverRepository,
						   IUnitOfWork unitOfWork)
		{
			this._userRepository = userRepository;
			this._userRolesRepository = userRolesRepository;
			this._roleRepository = roleRepository;
			this._employeeRepository = employeeRepository;
			this._driverRepository = driverRepository;
			this._unitOfWork = unitOfWork;
		}

		public UserDatatables GetUsersForTable(int page, int itemsPerPage, string sortBy, bool reverse, string userLogin, string custSearchValue)
		{
			var filtedUser = "";
			if (userLogin.Equals("cus_kanri"))
			{
				filtedUser = "cloud_kanri, cus_kanri";
			}

			var users = from a in _userRepository.GetAllQueryable()
						join b in _userRolesRepository.GetAllQueryable() on a.Id equals b.UserId into t1
						from b in t1.DefaultIfEmpty()
						join c in _employeeRepository.GetAllQueryable() on a.EmployeeC equals c.EmployeeC into t2
						from c in t2.DefaultIfEmpty()
						join d in _roleRepository.GetAllQueryable() on b.RoleId equals d.Id into t3
						from d in t3.DefaultIfEmpty()
						join t in _driverRepository.GetAllQueryable() on a.DriverC equals t.DriverC into t4
						from t in t4.DefaultIfEmpty()
						where t == null && a.IsMobileUser == "0" && (custSearchValue == null || custSearchValue == "" || (a.UserName.Contains(custSearchValue) ||
							    c == null || (c.EmployeeLastN + " " + c.EmployeeFirstN).Contains(custSearchValue))
							  ) && (string.IsNullOrEmpty(filtedUser) || !filtedUser.Contains(a.UserName))
						select new UserViewModel()
						{
							UserName = a.UserName,
							EmployeeC = a.EmployeeC,
							EmployeeN = c != null ? c.EmployeeLastN + " " + c.EmployeeFirstN : "",
							RoleId = b.RoleId,
							RoleN = d != null ? d.Name : "",
							IsActive = a.IsActive
						};
			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var usersOrdered = users.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var usersPaged = usersOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var data = new UserDatatables()
			{
				Data = usersPaged,
				Total = usersPaged.Count()
			};
			return data;
		}

		public UserDatatables GetMobileUsersForTable(int page, int itemsPerPage, string sortBy, bool reverse, string custSearchValue)
		{
			var users = from a in _userRepository.GetAllQueryable()
							join b in _userRolesRepository.GetAllQueryable() on a.Id equals b.UserId into t1
							from b in t1.DefaultIfEmpty()
							join d in _roleRepository.GetAllQueryable() on b.RoleId equals d.Id into t3
							from d in t3.DefaultIfEmpty()
							join t in _driverRepository.GetAllQueryable() on a.DriverC equals t.DriverC into t4
							from t in t4.DefaultIfEmpty()
							where t != null && a.IsMobileUser == "1" && (string.IsNullOrEmpty(custSearchValue) || (a.UserName.Contains(custSearchValue) ||
									 (t.LastN + " " + t.FirstN).Contains(custSearchValue))
								  )
							select new UserViewModel()
							{
								UserName = a.UserName,
								DriverC = a.DriverC,
								DriverN = t != null ? t.LastN + " " + t.FirstN : "",
								RoleId = b.RoleId,
								RoleN = d != null ? d.Name : "",
								IsActive = a.IsActive
							};
			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var usersOrdered = users.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var usersPaged = usersOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var data = new UserDatatables()
			{
				Data = usersPaged,
				Total = usersPaged.Count()
			};
			return data;
		}

		public UserViewModel GetUserByName(string userName)
		{
			var user = (from a in _userRepository.GetAllQueryable()
						join b in _userRolesRepository.GetAllQueryable() on a.Id equals b.UserId into t1
						from b in t1.DefaultIfEmpty()
						join c in _employeeRepository.GetAllQueryable() on a.EmployeeC equals c.EmployeeC into t2
						from c in t2.DefaultIfEmpty()
						join d in _roleRepository.GetAllQueryable() on b.RoleId equals d.Id into t3
						from d in t3.DefaultIfEmpty()
						join t in _driverRepository.GetAllQueryable() on a.DriverC equals t.DriverC into t4
						from t in t4.DefaultIfEmpty()
						where (a.UserName == userName)
						select new UserViewModel()
						{
							UserName = a.UserName,
							EmployeeC = a.EmployeeC,
							EmployeeN = c != null ? c.EmployeeLastN + " " + c.EmployeeFirstN : "",
							DriverC = a.DriverC,
							DriverN = t != null ? t.LastN + " " + t.FirstN : "",
							RoleId = b.RoleId,
							RoleN = d != null ? d.Name : "",
							IsActive = a.IsActive
						}).ToList();

			if (user.Count > 0)
			{
				user[0].UserIndex = FindIndex(userName);
				return user[0];
			}

			return null;
		}

		public IEnumerable<UserViewModel> GetUsers(string value)
		{
			var users = (from a in _userRepository.GetAllQueryable()
						join b in _userRolesRepository.GetAllQueryable() on a.Id equals b.UserId into t1
						from b in t1.DefaultIfEmpty()
						join c in _employeeRepository.GetAllQueryable() on a.EmployeeC equals c.EmployeeC into t2
						from c in t2.DefaultIfEmpty()
						join d in _roleRepository.GetAllQueryable() on b.RoleId equals d.Id into t3
						from d in t3.DefaultIfEmpty()
						 join t in _driverRepository.GetAllQueryable() on a.DriverC equals t.DriverC into t4
						 from t in t4.DefaultIfEmpty()
						 where a.IsMobileUser == "0" && (a.UserName.Contains(value) || (c != null && (c.EmployeeLastN + " " + c.EmployeeFirstN).Contains(value)) ||
								(t != null && (t.LastN + " " + t.FirstN).Contains(value)))
						select new UserViewModel()
						{
							UserName = a.UserName,
							EmployeeC = a.EmployeeC,
							EmployeeN = c != null ? c.EmployeeLastN + " " + c.EmployeeFirstN : "",
							DriverC = a.DriverC,
							DriverN = t != null ? t.LastN + " " + t.FirstN : "",
							RoleId = b.RoleId,
							RoleN = d != null ? d.Name : "",
							IsActive = a.IsActive
						}).ToList();

			return users;
		}

		public IEnumerable<UserViewModel> GetMobileUsers(string value)
		{
			var users = (from a in _userRepository.GetAllQueryable()
						 join b in _userRolesRepository.GetAllQueryable() on a.Id equals b.UserId into t1
						 from b in t1.DefaultIfEmpty()
						 join c in _employeeRepository.GetAllQueryable() on a.EmployeeC equals c.EmployeeC into t2
						 from c in t2.DefaultIfEmpty()
						 join d in _roleRepository.GetAllQueryable() on b.RoleId equals d.Id into t3
						 from d in t3.DefaultIfEmpty()
						 join t in _driverRepository.GetAllQueryable() on a.DriverC equals t.DriverC into t4
						 from t in t4.DefaultIfEmpty()
						 where a.IsMobileUser == "1" && (a.UserName.Contains(value) || (c != null && (c.EmployeeLastN + " " + c.EmployeeFirstN).Contains(value)) ||
								(t != null && (t.LastN + " " + t.FirstN).Contains(value)))
						 select new UserViewModel()
						 {
							 UserName = a.UserName,
							 EmployeeC = a.EmployeeC,
							 EmployeeN = c != null ? c.EmployeeLastN + " " + c.EmployeeFirstN : "",
							 DriverC = a.DriverC,
							 DriverN = t != null ? t.LastN + " " + t.FirstN : "",
							 RoleId = b.RoleId,
							 RoleN = d != null ? d.Name : "",
							 IsActive = a.IsActive
						 }).ToList();

			return users;
		}

		public bool CheckUserLoggedIn(string userName, string password)
		{
			var user = _userRepository.GetUser(userName, password);
			if (user != null && user.IsActive == "1" && user.IsLoggedIn == "1")
			{
				return true;
			}
			return false;
		}

		public void SetUserLogOut(string userName)
		{
			var user = _userRepository.Query(p => p.UserName.Equals(userName)).FirstOrDefault();
			if (user != null && user.IsLoggedIn == "1")
			{
				user.IsLoggedIn = "0";
				_userRepository.Update(user);
				SaveUser();
			}
		}

		private int FindIndex(string userName)
		{
			var data = _userRepository.GetAllQueryable();
			var index = 0;
			var totalRecords = data.Count();
			var halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
			var loopCapacity = 100;
			var recordsToSkip = 0;
			if (totalRecords > 0)
			{
				var nextIteration = true;
				while (nextIteration)
				{
					for (var counter = 0; counter < 2; counter++)
					{
						recordsToSkip = recordsToSkip + (counter * halfCount);

						if (data.OrderBy("UserName descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.UserName == userName))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("UserName descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.UserName == userName)
								{
									index = index + 1;
									index = recordsToSkip + index;
									break;
								}
								index = index + 1;
							}
							nextIteration = false;
							break;
						}
					}
				}
			}
			return index;
		}

		public void SaveUser()
		{
			_unitOfWork.Commit();
		}

		public bool RegisterDeviceRegistrationId(string driverC, string registrationId, string platform)
		{
			var user = _userRepository.Query(p => p.DriverC == driverC).FirstOrDefault();
			if (user != null)
			{
				if (string.IsNullOrEmpty(user.RegistrationId) || !user.RegistrationId.Equals(registrationId))
				{
					user.RegistrationId = registrationId;
					user.SmartphonePlatform = platform;
					_userRepository.Update(user);
					SaveUser();
				}
				return true;
			}
			return false;
		}
        public string GetDeviceInfo(string userName)
        {
            return _userRepository.Query(u => u.UserName == userName).FirstOrDefault().DeviceInfo ?? ""; 
        }

        public bool SetDeviceInfo(string userName, string deviceInfo){
            var user = _userRepository.Query(u => u.UserName == userName).FirstOrDefault();
            if(user !=null)
            {
                user.DeviceInfo = deviceInfo;
                _userRepository.Update(user);
                SaveUser();
                return true;
            }
            return false;
        }
	}
}
