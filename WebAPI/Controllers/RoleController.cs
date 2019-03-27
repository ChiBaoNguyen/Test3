using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Department;
using Website.ViewModels.Employee;
using Website.ViewModels.Role;

namespace WebAPI.Controllers
{
	public class RoleController : ApiController
	{
		public IRoleService _roleService;

		public RoleController() { }
		public RoleController(IRoleService roleService)
		{
			this._roleService = roleService;
		}

		[HttpGet]
		[Route("api/Role/GetByName")]
		public RoleViewModel GetByName(string value)
		{
			return _roleService.GetByName(value);
		}

		[HttpGet]
		[Route("api/Role/GetRolesAutosuggest")]
		public IEnumerable<RoleViewModel> GetRolesAutosuggest(string value)
		{
			return _roleService.GetRolesAutosuggest(value);
		}

		[Route("api/Role/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "Id",
				  bool reverse = false,
				  string search = null)
		{
			var custDatatable = _roleService.GetRolesForTable(page, itemsPerPage, sortBy, reverse, search);
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}
	}
}