using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models.Authorization;
using AutoMapper;
using Microsoft.AspNet.Identity.EntityFramework;
using Root.Models;

namespace Service.Services
{
	public interface IUserRolesService
	{
		void CreateUserRoles(string userId, string roleId);
	}
	public class UserRolesService : IUserRolesService
	{
		private readonly IUserRolesRepository _userRolesRepository;
		private readonly IUnitOfWork _unitOfWork;

		public UserRolesService(IUserRolesRepository userRolesRepository,
								IUnitOfWork unitOfWork)
		{
			this._userRolesRepository = userRolesRepository;
			this._unitOfWork = unitOfWork;
		}
		public void SaveUserRoles()
		{
			_unitOfWork.Commit();
		}

		public void CreateUserRoles(string userId, string roleId)
		{
			var userRoles = new IdentityUserRole();
			userRoles.UserId = userId;
			userRoles.RoleId = roleId;

			_userRolesRepository.Add(userRoles);

			SaveUserRoles();
		}
	}
}