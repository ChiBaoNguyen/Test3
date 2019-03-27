using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.Customer;
using Website.ViewModels.Department;
using Website.ViewModels.Employee;
using Website.ViewModels.Role;

namespace Service.Services
{
	public interface IRoleService
	{
		RoleDatatables GetRolesForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string custSearchValue);

		IEnumerable<RoleViewModel> GetRolesAutosuggest(string value);
		RoleViewModel GetByName(string value);
		void SaveRole();
	}

	public class RoleService : IRoleService
	{
		private readonly IRoleRepository _roleRepository;
		private readonly IUnitOfWork _unitOfWork;

		public RoleService(IRoleRepository roleRepository,
						   IUnitOfWork unitOfWork)
		{
			this._roleRepository = roleRepository;
			this._unitOfWork = unitOfWork;
		}

		#region IRoleService members

		public RoleDatatables GetRolesForTable(int page, int itemsPerPage, string sortBy, bool reverse, string custSearchValue)
		{
			var filtedUser = "cloud_kanri, cus_kanri, kanri, m_kanri";
			var roles = from a in _roleRepository.GetAllQueryable()
						where !filtedUser.Contains(a.Id)
						select new RoleViewModel()
						{
							Id = a.Id,
							Name = a.Name
						};
			// searching
			if (!string.IsNullOrWhiteSpace(custSearchValue))
			{
				custSearchValue = custSearchValue.ToLower();
				roles = roles.Where(r => r.Id.ToLower().Contains(custSearchValue) || r.Name.ToLower().Contains(custSearchValue));
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var rolesOrdered = roles.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var rolesPaged = rolesOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var custDatatable = new RoleDatatables()
			{
				Data = rolesPaged,
				Total = rolesPaged.Count()
			};
			return custDatatable;
		}

		public IEnumerable<RoleViewModel> GetRolesAutosuggest(string value)
		{
			var filtedUser = "cloud_kanri, cus_kanri, kanri, m_kanri";
			var roles = from a in _roleRepository.GetAllQueryable()
						where (a.Id.Contains(value) || a.Name.Contains(value)) && !filtedUser.Contains(a.Id)
						select new RoleViewModel()
						{
							Id = a.Id,
							Name = a.Name
						};

			return roles;
		}

		public RoleViewModel GetByName(string value)
		{
			var result = new RoleViewModel();
			var role = _roleRepository.Query(r => r.Name == value).FirstOrDefault();
			if (role != null)
			{
				result.Id = role.Id;
				result.Name = role.Name;
			}
			return result;
		}

		public void SaveRole()
		{
			_unitOfWork.Commit();
		}
		#endregion
	}
}
