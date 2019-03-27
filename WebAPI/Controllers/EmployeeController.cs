using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Department;
using Website.ViewModels.Employee;

namespace WebAPI.Controllers
{
	//[Authorize(Roles = "Employee_M")]
	public class EmployeeController : ApiController
	{
		public IEmployeeService _employeeService;
		public ICommonService _commonService;
		public EmployeeController() { }
		public EmployeeController(IEmployeeService employeeService,ICommonService commonService)
		{
			this._employeeService = employeeService;
			_commonService = commonService;
		}

		// GET api/customer
		public IEnumerable<EmployeeViewModel> Get()
		{
			return _employeeService.GetEmployees();
		}

		public IEnumerable<EmployeeViewModel> Get(string value)
		{
			return _employeeService.GetEmployees(value);
		}

		[Route("api/Employee/GetEmployeesByCode")]
		public IEnumerable<EmployeeViewModel> GetEmployeesByCode(string value)
		{
			return _employeeService.GetEmployeesByCode(value);
		}

		[Route("api/Employee/GetEmployeeDepartment")]
		public DepartmentViewModel GetEmployeeDepartment(string employeeC)
		{
			return _employeeService.GetEmployeeDepartment(employeeC);
		}

		[HttpGet]
		[Route("api/Employee/GetEmployeeByCode")]
		public IHttpActionResult GetEmployeeByCode(string employeeCode)
		{
			var employee = _employeeService.GetEmployeeByCode(employeeCode);
			if (employee == null)
			{
				return NotFound();
			}
			return Ok(employee);
		}

		//[Authorize(Roles = "Employee_M")]
		[Route("api/Employee/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "EmployeeC",
				  bool reverse = false,
				  string search = null)
		{
			var custDatatable = _employeeService.GetEmployeesForTable(page, itemsPerPage, sortBy, reverse, search);
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		// POST api/<controller>
		[Filters.Authorize(Roles = "Employee_M_A")]
		public void Post(EmployeeViewModel employee)
		{
			_employeeService.CreateEmployee(employee);
		}

		// PUT api/<controller>/5
		[Filters.Authorize(Roles = "Employee_M_E")]
		public void Put(EmployeeViewModel employee)
		{
			_employeeService.UpdateEmployee(employee);
		}

		// DELETE api/customer/5
		[Filters.Authorize(Roles = "Employee_M_D")]
		public void Delete(string id)
		{
			_employeeService.DeleteEmployee(id);
		}

		[HttpGet]
		[Route("api/Employee/CheckWhenDelete/{language}/{entryClerkC}")]
		[Filters.Authorize(Roles = "Employee_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string entryClerkC)
		{
			List<string> paramsList = new List<string>() { entryClerkC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Employee_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
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
		[Route("api/Employee/SetStatusEmployee/{id}")]
		[Filters.Authorize(Roles = "Employee_M_E")]
		public void SetStatusEmployee(string id)
		{
			_employeeService.SetStatusEmployee(id);
		}

		[HttpGet]
		[Route("api/Employee/GetByName")]
		public EmployeeViewModel GetByName(string value)
		{
			return _employeeService.GetByName(value);
		}

		[HttpGet]
		[Route("api/Employee/GetEmployeesAutosuggestForReport")]
		public IEnumerable<EmployeeViewModel> GetEmployeesAutosuggestForReport(string value)
		{
			return _employeeService.GetEmployeesAutosuggestForReport(value);
		}

		[HttpGet]
		[Route("api/Employee/GetEmployeesForReport")]
		public IEnumerable<EmployeeViewModel> GetEmployeesForReport()
		{
			return _employeeService.GetEmployeesForReport();
		}
	}
}
