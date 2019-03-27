using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Service.Services;
using Website.Enum;
using Website.Utilities;
using Website.ViewModels.Department;
using Website.ViewModels.InspectionDetail;
using Website.ViewModels.InspectionPlanDetail;
using Website.ViewModels.Truck;

namespace WebAPI.Controllers
{
    public class TruckController : ApiController
    {
		public ITruckService _truckService;
		public ICommonService _commonService;
	    public IInspectionDetailService _inspectionDetailService;
	    private IInspectionPlanDetailService _inspectionPlanDetailService;
		private readonly IMaintenancePlanDetailService _maintenancePlanDetailService;

		public TruckController() { }

		public TruckController(ITruckService truckService, 
			ICommonService commonService, 
			IInspectionDetailService inspectionDetailService,
			IMaintenancePlanDetailService maintenancePlanDetailService,
			IInspectionPlanDetailService inspectionPlanDetailService
			)
		{
			this._truckService = truckService;
			_commonService = commonService;
			_inspectionDetailService = inspectionDetailService;
			_inspectionPlanDetailService = inspectionPlanDetailService;
			_maintenancePlanDetailService = maintenancePlanDetailService;
		}

		// GET api/truck
		public IEnumerable<TruckViewModel> Get()
		{
			return _truckService.GetTrucks();
		}

		public IEnumerable<TruckViewModel> Get(string value)
		{
			return _truckService.GetTruckForSuggestion(value);
		}

		[Route("api/Truck/GetTrucksByCode")]
		public IEnumerable<TruckViewModel> GetTrucksByCode(string value)
		{
			return _truckService.GetTrucksByCode(value);
		}

		[HttpGet]
		[Route("api/Truck/GetByName")]
		public TruckViewModel GetByName(string value)
		{
			return _truckService.GetByName(value);
		}

		[Route("api/Truck/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "TruckC",
				  bool reverse = false,
				  string search = null,
				  string partNo = null)
		{
			var custDatatable = _truckService.GetTrucksForTable(page, itemsPerPage, sortBy, reverse, search, partNo);
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		[HttpGet]
		[Route("api/Truck/GetTruckByCode")]
		public IHttpActionResult GetTruckByCode(string truckC)
		{
			var truck = _truckService.GetTruckByCode(truckC);
			if (truck == null)
			{
				return NotFound();
			}
			return Ok(truck);
		}

		// POST api/<controller>
		[Filters.Authorize(Roles = "Truck_M_A")]
		public IHttpActionResult Post(TruckViewModel truck)
		{
			_truckService.CreateTruck(truck);
			_maintenancePlanDetailService.UpdatePlan(truck.MaintenanceItems, "0", truck.TruckC);
			_inspectionPlanDetailService.Add(truck.Inspection, "0", truck.TruckC);
			return Ok();
		}

	    // PUT api/<controller>/5
		[Filters.Authorize(Roles = "Truck_M_E")]
		public void Put(TruckViewModel truck)
		{
			_truckService.UpdateTruck(truck);
			_maintenancePlanDetailService.UpdatePlan(truck.MaintenanceItems, "0", truck.TruckC);
			_inspectionPlanDetailService.Add(truck.Inspection,"0",truck.TruckC);
		}

		// DELETE api/truck/5
		[Filters.Authorize(Roles = "Truck_M_D")]
		public void Delete(string id)
		{
			_truckService.DeleteTruck(id);
		}

		[HttpGet]
		[Route("api/Truck/CheckWhenDelete/{language}/{truckC}")]
		[Filters.Authorize(Roles = "Truck_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string truckC)
		{
			List<string> paramsList = new List<string>() { truckC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Truck_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
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
		[Route("api/Truck/SetStatusTruck/{id}")]
		[Filters.Authorize(Roles = "Truck_M_E")]
		public void SetStatusTruck(string id)
		{
			_truckService.SetStatusTruck(id);
		}

		[HttpGet]
		[Route("api/Truck/GetTruckForAutosuggestByType")]
		public IEnumerable<TruckViewModel> GetTruckForAutosuggestByType(string value, string type)
		{
			return _truckService.GetTruckForAutosuggestByType(value, type);
		}

		[HttpGet]
		[Route("api/Truck/GetTrucksByDepC")]
		public IEnumerable<TruckViewModel> GetTrucksByDepC(string depC)
		{
			return _truckService.GetTrucksByDepC(depC);
		}

		[HttpPost]
		[Route("api/Truck/UpdateOdoForTrucks")]
		[Filters.Authorize(Roles = "Odo_E")]
		public void UpdateOdoForTrucks(TruckDatatables data)
		{
			_truckService.UpdateOdoForTrucks(data.Data);
		}

	    [HttpGet]
	    [Route("api/Truck/GetMaintenanceManagementItems/{truckC}/{modelC}")]
	    public IEnumerable<TruckMaintenanceViewModel> GetMaintenanceManagementItems(string truckC, string modelC)
	    {
		    return _truckService.GetMaintenanceManagementItems(truckC, modelC);
	    }

		[HttpGet]
		[Route("api/Truck/GetTrucksForReport")]
		public IEnumerable<TruckViewModel> GetTrucksForReport()
		{
			return _truckService.GetTrucksForReport();
		}

		[HttpGet]
		[Route("api/Truck/GetCompanyTrucksForReport")]
		public IEnumerable<TruckViewModel> GetCompanyTrucksForReport()
		{
			return _truckService.GetCompanyTrucksForReport();
		}

		[HttpGet]
		[Route("api/Truck/GetTruckForSuggestionForReport")]
		public IEnumerable<TruckViewModel> GetTruckForSuggestionForReport(string value)
		{
			return _truckService.GetTruckForSuggestionForReport(value);
		}

		[HttpGet]
		[Route("api/Truck/GetInspectionData")]
		public IEnumerable<TruckInspectionViewModel> GetInspectionData(string truckC)
		{
			IEnumerable<InspectionDetailViewModel> detail = _inspectionDetailService.Get("0", truckC);
			IEnumerable<InspectionPlanDetailViewModel> planDetail =_inspectionPlanDetailService.Get("0", truckC);
			List<TruckInspectionViewModel> result = new List<TruckInspectionViewModel>();
			if (detail != null)
			{
				foreach (var item in detail)
				{
					result.Add(new TruckInspectionViewModel()
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
					result.Add(new TruckInspectionViewModel()
					{
						InspectionC = item.InspectionC,
						InspectionN = item.InspectionN,
						InspectionPlanD = item.InspectionPlanD,
					});
				}
			}
			
			return result;
		}

		[HttpGet]
		[Route("api/Truck/GetTruckDepartments")]
		public IEnumerable<DepartmentViewModel> GetTruckDepartments()
		{
			return _truckService.GetTruckDepartments();
		}


		[Route("api/Truck/GetDriverNameByTruckCode")]
		public string GetDriverNameByTruckCode(string code)
		{
			return _truckService.GetDriverNameByTruckCode(code);
		}
    }
}