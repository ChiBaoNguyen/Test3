using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Department;

namespace WebAPI.Controllers
{
	public class DepartmentController : ApiController
	{
		public IDepartmentService _departmenttypeService;
		public ICommonService _commonService;
		public DepartmentController() { }
		public DepartmentController(IDepartmentService departmenttypeService, ICommonService commonService)
		{
			this._departmenttypeService = departmenttypeService;
			_commonService = commonService;
		}

		// GET api/customer
		public async Task<IEnumerable<DepartmentViewModel>> Get()
		{
			return await Task.Run(() =>  _departmenttypeService.GetDepartments());
		}

		public IEnumerable<DepartmentViewModel> Get(string value)
		{
			return _departmenttypeService.GetDepartments(value);
		}

		[Route("api/Department/GetDepartmentsByCode")]
		public IEnumerable<DepartmentViewModel> GetDepartmentsByCode(string value)
		{
			return _departmenttypeService.GetDepartmentsByCode(value);
		}

		[HttpGet]
		[Route("api/Department/GetDepartmentByCode")]
		public IHttpActionResult GetDepartmentByCode(string departmentCode)
		{
			var departmenttype = _departmenttypeService.GetDepartmentByCode(departmentCode);
			if (departmenttype == null)
			{
				return NotFound();
			}
			return Ok(departmenttype);
		}

		[HttpGet]
		[Route("api/Department/GetDepartmentByEntryClerkC")]
		public IHttpActionResult GetDepartmentByEntryClerkC(string departmentCode)
		{
			var departmenttype = _departmenttypeService.GetDepartmentByCode(departmentCode);
			if (departmenttype == null)
			{
				return NotFound();
			}
			return Ok(departmenttype);
		}

		[Route("api/Department/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "DepC",
				  bool reverse = false,
				  string search = null)
		{
			var custDatatable = _departmenttypeService.GetDepartmentsForTable(page, itemsPerPage, sortBy, reverse, search);
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		// POST api/<controller>
		[Filters.Authorize(Roles = "Department_M_A")]
		public void Post(DepartmentViewModel departmenttype)
		{
			_departmenttypeService.CreateDepartment(departmenttype);
		}

		// PUT api/<controller>/5
		[Filters.Authorize(Roles = "Department_M_E")]
		public void Put(DepartmentViewModel departmenttype)
		{
			_departmenttypeService.UpdateDepartment(departmenttype);
		}

		// DELETE api/customer/5
		[Filters.Authorize(Roles = "Department_M_D")]
		public void Delete(string id)
		{
			_departmenttypeService.DeleteDepartment(id);
		}

		[HttpGet]
		[Route("api/Department/CheckWhenDelete/{language}/{depC}")]
		[Filters.Authorize(Roles = "Department_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string depC)
		{
			List<string> paramsList = new List<string>() { depC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Department_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
			return new HttpResponseMessage()
			{
				Content = new StringContent(
					result,
					Encoding.UTF8,
					"text/html"
				)
			};
		}

		[HttpGet]
		[Route("api/Department/SetStatusDepartment/{id}")]
		[Filters.Authorize(Roles = "Department_M_E")]
		public void SetStatusDepartment(string id)
		{
			_departmenttypeService.SetStatusDepartment(id);
		}

		[HttpGet]
		[Route("api/Department/GetByName")]
		public DepartmentViewModel GetByName(string value)
		{
			return _departmenttypeService.GetByName(value);
		}

		[HttpGet]
		[Route("api/Department/GetDepartmentsForReport")]
		public IEnumerable<DepartmentViewModel> GetDepartmentsForReport()
		{
			return _departmenttypeService.GetDepartmentsForReport();
		}
	}
}
