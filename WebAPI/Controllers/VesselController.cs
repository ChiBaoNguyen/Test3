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
	public class VesselController : ApiController
	{
		public IVesselService _vesselService;
		public ICommonService _commonService;

		public VesselController() { }

		public VesselController(IVesselService vesselService, ICommonService commonService)
		{
			this._vesselService = vesselService;
			_commonService = commonService;
		}

		[System.Web.Http.Route("api/Vessel/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "VesselC",
				  bool reverse = false,
				  string search = null)
		{
			var comDatatable = _vesselService.GetVesselForTable(page, itemsPerPage, sortBy, reverse, search);
			if (comDatatable == null)
			{
				return NotFound();
			}
			return Ok(comDatatable);
		}
		[Filters.Authorize(Roles = "Vessel_M_A")]
		public void Post(VesselViewModel vessel)
		{
			_vesselService.CreateVessel(vessel);
		}

		[HttpGet]
		[Route("api/Vessel/GetVesselByCode")]
		public IHttpActionResult GetVesselByCode(string vesselCode)
		{
			var Vessel = _vesselService.GetVesselByCode(vesselCode);
			if (Vessel == null)
			{
				return NotFound();
			}
			return Ok(Vessel);
		}
		[Filters.Authorize(Roles = "Vessel_M_E")]
		public void Put(VesselViewModel vessel)
		{
			_vesselService.UpdateVessel(vessel);
		}
		[Filters.Authorize(Roles = "Vessel_M_D")]
		public void Delete(string id)
		{
			_vesselService.DeleteVessel(id);
		}
		
		[HttpGet]
		[Route("api/Vessel/CheckWhenDelete/{language}/{vesselC}")]
		[Filters.Authorize(Roles = "Vessel_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string vesselC)
		{
			List<string> paramsList = new List<string>() { vesselC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Vessel_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
			return new HttpResponseMessage()
			{
				Content = new StringContent(
					result,
					Encoding.UTF8,
					"text/html"
				)
			};
		}
		public IEnumerable<VesselViewModel> Get(string value)
		{
			return _vesselService.GetVesselForSuggestion(value);
		}

		[Route("api/Vessel/GetVesselesByCode")]
		public IEnumerable<VesselViewModel> GetVesselesByCode(string value)
		{
			return _vesselService.GetVesselesByCode(value);
		}

		[HttpGet]
		[Route("api/Vessel/GetByName")]
		public VesselViewModel GetByName(string value)
		{
			return _vesselService.GetByName(value);
		}

		[HttpGet]
		[Route("api/Vessel/SetStatusVessel/{id}")]
		[Filters.Authorize(Roles = "Vessel_M_E")]
		public void SetStatusCustomer(string id)
		{
			_vesselService.SetStatusVessel(id);
		}

		[HttpGet]
		[Route("api/Vessel/GetVesselsByShippingCompanyC")]
		public IEnumerable<VesselViewModel> GetVesselsByShippingCompanyC(string value, string shipComC)
		{
			return _vesselService.GetVesselsByShippingCompanyC(value, shipComC);
		}
	}
}
