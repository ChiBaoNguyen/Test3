using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Employee;
using Website.ViewModels.Driver;
using Website.ViewModels.Department;

namespace WebAPI.Controllers
{
	public class DriverController : ApiController
	{
		public IDriverService _driverService;
		public ICommonService _commonService;

		public DriverController() { }
		public DriverController(IDriverService driverService, ICommonService commonService)
		{
			this._driverService = driverService;
			_commonService = commonService;
		}

		// GET api/driver
		public IEnumerable<DriverViewModel> Get()
		{
			return _driverService.GetDrivers();
		}

		public IEnumerable<DriverViewModel> Get(string value)
		{
			return _driverService.GetDriversForSuggestion(value);
		}

		[Route("api/Driver/GetDriversByCode")]
		public IEnumerable<DriverViewModel> GetDriversByCode(string value)
		{
			return _driverService.GetDriversByCode(value);
		}

		[HttpGet]
		[Route("api/Driver/GetDriverByCode")]
		public IHttpActionResult GetDriverByCode(string driverCode)
		{
			var driver = _driverService.GetDriverByCode(driverCode);
			if (driver == null)
			{
				return NotFound();
			}
			return Ok(driver);
		}

		[Route("api/Driver/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "DriverC",
				  bool reverse = false,
				  string search = null)
		{
			var driverDatatable = _driverService.GetDriversForTable(page, itemsPerPage, sortBy, reverse, search);
			if (driverDatatable == null)
			{
				return NotFound();
			}
			return Ok(driverDatatable);
		}

		// POST api/<controller>
		[Filters.Authorize(Roles = "Driver_M_A")]
		public void Post(DriverViewModel driver)
		{
			_driverService.CreateDriver(driver);
		}

		// PUT api/<controller>/5
		[Filters.Authorize(Roles = "Driver_M_E")]
		public void Put(DriverViewModel driver)
		{
			_driverService.UpdateDriver(driver);
		}

		// DELETE api/customer/5
		[Filters.Authorize(Roles = "Driver_M_D")]
		public void Delete(string id)
		{
			_driverService.DeleteDriver(id);
		}

		[HttpGet]
		[Route("api/Driver/CheckWhenDelete/{language}/{driverC}")]
		[Filters.Authorize(Roles = "Driver_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string driverC)
		{
			List<string> paramsList = new List<string>() { driverC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Driver_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
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
		[Route("api/Driver/SetStatusDriver/{id}")]
		[Filters.Authorize(Roles = "Driver_M_E")]
		public void SetStatusDriver(string id)
		{
			_driverService.SetStatusDriver(id);
		}

		[HttpGet]
		[Route("api/Driver/GetByName")]
		public DriverViewModel GetByName(string value)
		{
			return _driverService.GetByName(value);
		}

		[HttpGet]
		[Route("api/Driver/GetDepFromDriver")]
		public string GetDepFromDriver(string driverC)
		{
			return _driverService.GetDepFromDriver(driverC);
		}

		[HttpGet]
		[Route("api/Driver/GetDriversAutoSuggestionForReport")]
		public IEnumerable<DriverViewModel> GetDriversAutoSuggestionForReport(string value)
		{
			return _driverService.GetDriversAutoSuggestionForReport(value);
		}

		[HttpGet]
		[Route("api/Driver/GetDriversForReport")]
		public IEnumerable<DriverViewModel> GetDriversForReport()
		{
			return _driverService.GetDriversForReport();
		}

		[HttpGet]
		[Route("api/Driver/GetDriverDepartments/{isForReport}")]
		public IEnumerable<DepartmentViewModel> GetDriverDepartments(bool isForReport)
		{
			return _driverService.GetDriverDepartments(isForReport);
		}
	}
}