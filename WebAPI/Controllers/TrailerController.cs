using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.InspectionDetail;
using Website.ViewModels.InspectionPlanDetail;
using Website.ViewModels.Ship;
using Website.ViewModels.Trailer;

namespace WebAPI.Controllers
{
	public class TrailerController : ApiController
	{
		public ITrailerService _trailerService;
		public ICommonService _commonService;
		public IMaintenancePlanDetailService _maintenancePlanDetailService;
		public IInspectionDetailService _inspectionDetailService;
		private IInspectionPlanDetailService _inspectionPlanDetailService;
		public TrailerController() { }

		public TrailerController(ITrailerService trailerService,
									ICommonService commonService,
									IInspectionDetailService inspectionDetailService,
									IInspectionPlanDetailService inspectionPlanDetailService,
									IMaintenancePlanDetailService maintenancePlanDetailService
			)
		{
			this._trailerService = trailerService;
			this._maintenancePlanDetailService = maintenancePlanDetailService;
			_inspectionDetailService = inspectionDetailService;
			_inspectionPlanDetailService = inspectionPlanDetailService;
			this._commonService = commonService;
		}

		public IEnumerable<TrailerViewModel> Get()
		{
			return _trailerService.GetAll();
		}
        public IEnumerable<TrailerViewModel> Get(string value)
        {
            return _trailerService.GetTrailerForSuggestion(value);
        }

        [Route("api/Trailer/GetTrailersAndDriverForSuggestion")]
        public IEnumerable<TrailerViewModel> GetTrailersAndDriverForSuggestion(string value)
        {
            return _trailerService.GetTrailersAndDriverForSuggestion(value);
        }

        [Route("api/Trailer/GetTrailerForSuggestionWithWarning")]
        public IEnumerable<SuggestedWarningTrailer> GetTrailerForSuggestionWithWarning(string value)
        {
            return _trailerService.GetTrailerForSuggestionWithWarning(value);
        }

		[Route("api/Trailer/GetTrailersByCode")]
		public IEnumerable<TrailerViewModel> GetTrailersByCode(string value)
		{
			return _trailerService.GetTrailersByCode(value);
		}

		[HttpGet]
		[Route("api/Trailer/GetByName")]
		public TrailerViewModel GetByName(string value)
		{
			return _trailerService.GetByName(value);
		}

        [HttpGet]
        [Route("api/Trailer/GetByNameWarning")]
		public TrailerViewModel GetByNameWarning(string value)
        {
            return _trailerService.GetByNameWarning(value);
        }

		[Route("api/Trailer/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "TrailerC",
				  bool reverse = false,
				  string search = null,
				  string partNo = null)
		{
			var custDatatable = _trailerService.GetTrailersForTable(page, itemsPerPage, sortBy, reverse, search, partNo);
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		[HttpGet]
		[Route("api/Trailer/GetTrailerByCode")]
		public IHttpActionResult GetTrailerByCode(string trailerC)
		{
			var trailer = _trailerService.GetTrailerByCode(trailerC);
			if (trailer == null)
			{
				return NotFound();
			}
			return Ok(trailer);
		}

		// POST api/<controller>
		[Filters.Authorize(Roles = "Trailer_M_A")]
		public void Post(TrailerViewModel trailer)
		{
			_trailerService.CreateTrailer(trailer);
			_maintenancePlanDetailService.UpdatePlan(trailer.MaintenanceItems, "1", trailer.TrailerC);
			_inspectionPlanDetailService.Add(trailer.Inspection, "1", trailer.TrailerC);
		}

		// PUT api/<controller>/5
		[Filters.Authorize(Roles = "Trailer_M_E")]
		public void Put(TrailerViewModel trailer)
		{
			_trailerService.UpdateTrailer(trailer);
			_maintenancePlanDetailService.UpdatePlan(trailer.MaintenanceItems, "1", trailer.TrailerC);
			_inspectionPlanDetailService.Add(trailer.Inspection, "1", trailer.TrailerC);
		}

		// DELETE api/trailer/5
		[Filters.Authorize(Roles = "Trailer_M_D")]
		public void Delete(string id)
		{
			_trailerService.DeleteTrailer(id);
		}

		[HttpGet]
		[Route("api/Trailer/CheckWhenDelete/{language}/{trailerC}")]
		[Filters.Authorize(Roles = "Trailer_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string trailerC)
		{
			List<string> paramsList = new List<string>() { trailerC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Trailer_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
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
		[Route("api/Trailer/SetStatusTrailer/{id}")]
		[Filters.Authorize(Roles = "Trailer_M_E")]
		public void SetStatusTrailer(string id)
		{
			_trailerService.SetStatusTrailer(id);
		}

		[HttpGet]
		[Route("api/Trailer/GetInspectionData")]
		public IEnumerable<TrailerInspectionViewModel> GetInspectionData(string trailerC)
		{
			IEnumerable<InspectionDetailViewModel> detail = _inspectionDetailService.Get("1", trailerC);
			IEnumerable<InspectionPlanDetailViewModel> planDetail = _inspectionPlanDetailService.Get("1", trailerC);
			List<TrailerInspectionViewModel> result = new List<TrailerInspectionViewModel>();
			if (detail != null)
			{
				foreach (var item in detail)
				{
					result.Add(new TrailerInspectionViewModel()
					{
						InspectionC = item.InspectionC,
						InspectionN = item.InspectionN,
						InspectionPlanD = item.InspectionPlanD,
						Description = item.Description,
						InspectionD = item.InspectionD
					});
				}
			}
			if (planDetail != null)
			{
				foreach (var item in planDetail)
				{
					result.Add(new TrailerInspectionViewModel()
					{
						InspectionC = item.InspectionC,
						InspectionN = item.InspectionN,
						InspectionPlanD = item.InspectionPlanD,
					});
				}
			}

			return result;
		}
		[Route("api/Trailer/GetDriverNameByTrailerCode")]
		public string GetDriverNameByTrailerCode(string code)
		{
			return _trailerService.GetDriverNameByTrailerCode(code);
		}
	}
}
