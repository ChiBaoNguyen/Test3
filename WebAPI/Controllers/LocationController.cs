using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Location;
using Website.ViewModels.TextResource;
using Website.ViewModels.Expense;


namespace WebAPI.Controllers
{
	public class LocationController : ApiController
	{
		public ILocationService _locationService;
		public ICommonService _commonService;

		public LocationController() { }

		public LocationController(ILocationService locationService, ICommonService commonService)
		{
			this._locationService = locationService;
			_commonService = commonService;
		}

		public IEnumerable<LocationViewModel> Get(string value)
		{
			return _locationService.GetLocations(value);
		}
		[HttpGet]
		[Route("api/Location/GetLocationsForOrder")]
		public IEnumerable<LocationViewModel> GetLocationsForOrder(string value)
		{
			return _locationService.GetLocationsForOrder(value);
		}
        [HttpGet]
        [Route("api/Location/GetLocationByCodeForDispatch")]
        public IHttpActionResult GetLocationByCodeForDispatch(string code)
        {
            var location = _locationService.GetLocationByCodeForDispatch(code);
            if (location == null)
            {
                return NotFound();
            }
            return Ok(location);
        }

		[Route("api/Location/GetLocationsByCode")]
		public IEnumerable<LocationViewModel> GetLocationsByCode(string value)
		{
			return _locationService.GetLocationsByCode(value);
		}

        [HttpGet]
        [Route("api/Location/GetLocationByCode")]
        public IHttpActionResult GetLocationByCode(string code)
        {
            var location = _locationService.GetLocationByCode(code);
            if (location == null)
            {
                return NotFound();
            }
            return Ok(location);
        }
		[HttpGet]
		[Route("api/Location/GetByName")]
		public LocationViewModel GetByName(string value)
		{
			return _locationService.GetByName(value);
		}
        [Route("api/Location/Datatable")]
        public IHttpActionResult Get(
                  int page = 1,
                  int itemsPerPage = 10,
                  string sortBy = "LocationC",
                  bool reverse = false,
                  string search = null)
        {
            var custDatatable = _locationService.GetLocationsForTable(page, itemsPerPage, sortBy, reverse, search);
            if (custDatatable == null)
            {
                return NotFound();
            }
            return Ok(custDatatable);
        }

        // POST api/<controller>
		[Filters.Authorize(Roles = "Location_M_A")]
        public void Post(LocationViewModel location)
        {
            _locationService.CreateLocation(location);
        }

        // PUT api/<controller>/5
		[Filters.Authorize(Roles = "Location_M_E")]
        public void Put(LocationViewModel location)
        {
            _locationService.UpdateLocation(location);
        }

        // DELETE api/customer/5
		[Filters.Authorize(Roles = "Location_M_D")]
		public void Delete(string id)
		{
			_locationService.DeleteLocation(id);
		}

		[HttpGet]
		[Route("api/Location/CheckWhenDelete/{language}/{locationC}")]
		[Filters.Authorize(Roles = "Location_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string locationC)
		{
			List<string> paramsList = new List<string>() { locationC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Location_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
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
        [Route("api/Location/SetStatusLocation/{id}")]
		[Filters.Authorize(Roles = "Location_M_E")]
        public void SetStatusLocation(string id)
        {
            _locationService.SetStatusLocation(id);
        }

		[Route("api/Location/GetLocations")]
		public IEnumerable<LocationViewModel> GetLocations()
		{
			return _locationService.GetLocations();
		}

		[Route("api/Location/GetAreas")]
		public IEnumerable<LocationViewModel> GetAreas()
		{
			return _locationService.GetAreas();
		}
		[HttpGet]
		[Route("api/Location/GetAreaByName")]
		public LocationViewModel GetAreaByName(string value)
		{
			return _locationService.GetAreaByName(value);
		}
		[HttpGet]
		[Route("api/Location/GetAreasByValue")]
		public IEnumerable<LocationViewModel> GetAreasByValue(string value)
		{
			return _locationService.GetAreas(value);
		}

		[HttpGet]
		[Route("api/Location/GetListExpenseData")]
		public List<ExpenseDetailViewModel> GetListExpenseData(string expenseCate, string liftuplowerParam,
			string internalParam, int detailNo,
			int dispatchNo,
			string location1, string location2, string location3, string ordertypeI, string containersizeI, string operation1,
			string operation2,
			string operation3, DateTime orderD, string orderNo)
		{
			return _locationService.GetListExpenseData(expenseCate, liftuplowerParam, internalParam, detailNo, dispatchNo,
				location1,
				location2, location3,
				ordertypeI, containersizeI, operation1, operation2,
				operation3, orderD, orderNo);
		}
	}
}
