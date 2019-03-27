using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Ship;

namespace WebAPI.Controllers
{
    public class ShipperController : ApiController
    {
		 public IShipperService _shipperService;
		 public ICommonService _commonService;
		public ShipperController() { }

		public ShipperController(IShipperService shipperService, ICommonService commonService)
		{
			this._shipperService = shipperService;
			_commonService = commonService;
		}

		[System.Web.Http.Route("api/Shipper/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "ContainerSizeC",
				  bool reverse = false,
				  string search = null)
		{
			var shipperDatatable = _shipperService.GetShipperForTable(page, itemsPerPage, sortBy, reverse, search);
			if (shipperDatatable == null)
			{
				return NotFound();
			}
			return Ok(shipperDatatable);
		}
		[Filters.Authorize(Roles = "Shipper_M_A")]
		public void Post(ShipperViewModel shipper)
		{
			_shipperService.CreateShipper(shipper);
		}

		[HttpGet]
		[Route("api/Shipper/GetShipperByCode")]
		public IHttpActionResult GetShipperByCode(string shipperC)
		{
			var Shipper = _shipperService.GetShipperSizeByCode(shipperC);
			if (Shipper == null)
			{
				return NotFound();
			}
			return Ok(Shipper);
		}
		[Filters.Authorize(Roles = "Shipper_M_E")]
		public void Put(ShipperViewModel shipper)
		{
			_shipperService.UpdateShipper(shipper);
		}

		[Filters.Authorize(Roles = "Shipper_M_D")]
		public void Delete(string id)
		{
			_shipperService.DeleteShipper(id);
		}

		[HttpGet]
		[Route("api/Shipper/CheckWhenDelete/{language}/{shipperC}")]
		[Filters.Authorize(Roles = "Shipper_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string shipperC)
		{
			List<string> paramsList = new List<string>() { shipperC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Shipper_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
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
		[Route("api/Shipper/SetStatusShipper/{id}")]
		[Filters.Authorize(Roles = "Shipper_M_E")]
		public void SetStatusCustomer(string id)
		{
			_shipperService.SetStatusShipper(id);
		}

		public IEnumerable<ShipperViewModel> Get(string value)
		{
			return _shipperService.GetShipperForSuggestion(value);
		}

		[Route("api/Shipper/GetShippersByCode")]
		public IEnumerable<ShipperViewModel> GetShippersByCode(string value)
		{
			return _shipperService.GetShippersByCode(value);
		}

		[HttpGet]
		[Route("api/Shipper/GetByName")]
		public ShipperViewModel GetByName(string value)
		{
			return _shipperService.GetByName(value);
		}
    }
}
