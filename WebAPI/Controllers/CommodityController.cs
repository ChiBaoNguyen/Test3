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
using Website.ViewModels.Order;
using Website.ViewModels.Ship;

namespace WebAPI.Controllers
{
	public class CommodityController : ApiController
	{
		public ICommodityService _commodityService;
		public ICommonService _commonService;
		
		public CommodityController() { }

		[System.Web.Http.Route("api/Commodity/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "CommodityC",
				  bool reverse = false,
				  string search = null)
		{
			var comDatatable = _commodityService.GetCommodityForTable(page, itemsPerPage, sortBy, reverse, search);
			if (comDatatable == null)
			{
				return NotFound();
			}
			return Ok(comDatatable);
		}

		public CommodityController(ICommodityService commodityService, ICommonService commonService)
		{
			this._commodityService = commodityService;
			_commonService = commonService;
		}

		[Filters.Authorize(Roles = "Commodity_M_A")]
		public void Post(CommodityViewModel commodity)
		{
			_commodityService.CreateCommodity(commodity);
		}

		[HttpGet]
		[Route("api/Commodity/GetCommodityByCode")]
		public IHttpActionResult GetCommodityByCode(string commodityCode)
		{
			var Commodity = _commodityService.GetCommodityByCode(commodityCode);
			if (Commodity == null)
			{
				return NotFound();
			}
			return Ok(Commodity);
		}
		[Filters.Authorize(Roles = "Commodity_M_E")]
		public void Put(CommodityViewModel commodity)
		{
			_commodityService.UpdateCommodity(commodity);
		}
		[Filters.Authorize(Roles = "Commodity_M_D")]
		public void Delete(string id)
		{
			_commodityService.DeleteCommodity(id);
		}

		[HttpGet]
		[Route("api/Commodity/CheckWhenDelete/{language}/{commodityC}")]
		[Filters.Authorize(Roles = "Commodity_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string commodityC)
		{
			List<string> paramsList = new List<string>() { commodityC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Commodity_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
			return new HttpResponseMessage()
			{
				Content = new StringContent(
					result,
					Encoding.UTF8,
					"text/html"
				)
			};
		}

		public IEnumerable<CommodityViewModel> Get(string value)
		{
			return _commodityService.GetCommodityForSuggestion(value);
		}

		[Route("api/Commodity/GetCommoditiesByCode")]
		public IEnumerable<CommodityViewModel> GetCommoditiesByCode(string value)
		{
			return _commodityService.GetCommoditiesByCode(value);
		}

		[HttpGet]
		[Route("api/Commodity/SetStatusCommodity/{id}")]
		[Filters.Authorize(Roles = "Commodity_M_E")]
		public void SetStatusCommodity(string id)
		{
			_commodityService.SetStatusCommodity(id);
		}

		[HttpGet]
		[Route("api/Commodity/GetByName")]
		public CommodityViewModel GetByName(string value)
		{
			return _commodityService.GetByName(value);
		}
	}
}
