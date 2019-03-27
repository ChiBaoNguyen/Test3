using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Route;

namespace WebAPI.Controllers
{
	public class RouteController: ApiController
	{
		public IRouteService _routeService;
		public ICommonService _commonService;
		public RouteController() { }
		public RouteController(IRouteService routeService, ICommonService commonService)
		{
			_routeService = routeService;
			_commonService = commonService;
		}

		public IHttpActionResult Get(string loc1C, string loc2C, string conSizeI, string conTypeC, string isHeavy,
			string isEmpty, string isSingle)
		{
			var route = _routeService.GetRoute(loc1C, loc2C, conSizeI, conTypeC, isHeavy, isEmpty, isSingle);
			return Ok(route);
		}

		public IHttpActionResult Get(string routeId)
		{
			var route = _routeService.GetRouteById(routeId);
			return Ok(route);
		}

		public IHttpActionResult Get(string expenseC, string categoryI, string departureC, string destinationC, string conSizeI, string conTypeC)
		{
			var route = _routeService.GetExpensesHistory(expenseC, categoryI, departureC, destinationC, conSizeI, conTypeC);
			return Ok(route);
		}

		[Route("api/Route/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "RouteN",
				  bool reverse = false,
				  string search = null)
		{
			var routeDatatable = _routeService.GetRoutesForTable(page, itemsPerPage, sortBy, reverse, search);
			if (routeDatatable == null)
			{
				return NotFound();
			}
			return Ok(routeDatatable);
		}

		// POST api/<controller>
		[Filters.Authorize(Roles = "Route_M_A")]
		public void Post(RouteViewModel routeViewModel)
		{
			_routeService.CreateRoute(routeViewModel);
		}

		// PUT api/<controller>/5
		[Filters.Authorize(Roles = "Route_M_E")]
		public void Put(RouteViewModel routeViewModel)
		{
			_routeService.UpdateRoute(routeViewModel);
		}

		// DELETE api/customer/5
		[Filters.Authorize(Roles = "Route_M_D")]
		public void Delete(string id)
		{
			_routeService.DeleteRoute(id);
		}

		[HttpGet]
		[Route("api/Route/CheckWhenDelete/{language}/{routeId}")]
		//[Authorize(Roles = "Department_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string routeId)
		{
			List<string> paramsList = new List<string>() { routeId };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Route_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
			return new HttpResponseMessage()
			{
				Content = new StringContent(
					result,
					Encoding.UTF8,
					"text/html"
				)
			};
		}
	}
}