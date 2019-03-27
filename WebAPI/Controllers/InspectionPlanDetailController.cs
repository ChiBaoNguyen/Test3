using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.InspectionPlanDetail;

namespace WebAPI.Controllers
{
	public class InspectionPlanDetailController : ApiController
	{
		private IInspectionPlanDetailService _inspectionPlanDetailService;
		public InspectionPlanDetailController(IInspectionPlanDetailService inspectionPlanDetailService)
		{
			_inspectionPlanDetailService = inspectionPlanDetailService;
		}

		[HttpGet]
		[Filters.Authorize(Roles = "Calendar")]
		[Route("api/InspectionPlanDetail/GetInspectionMaintenanceUpdateDetail")]
		public InspectionMaintenancePlanViewModel GetInspectionMaintenancePlan(int inspectionC, string objectI, string code, DateTime implementD, int planI, int intCase, int maintenanceItemC)
		{
			return _inspectionPlanDetailService.GetInspectionMaintenanceUpdateDetail(inspectionC, objectI, code, implementD, planI, intCase, maintenanceItemC);
		}

		[HttpPost]
		[Filters.Authorize(Roles = "Calendar")]
		[Route("api/InspectionPlanDetail/UpdateInspectionMaintenanceUpdateDetail")]
		public void UpdateInspectionMaintenanceUpdateDetail(InspectionMaintenancePlanViewModel data)
		{
			_inspectionPlanDetailService.UpdateInspectionMaintenanceUpdateDetail(data);
		}

		[HttpGet]
		[Filters.Authorize(Roles = "Calendar")]
		[Route("api/InspectionPlanDetail/DeleteInspectionMaintenanceUpdateDetail")]
		public void DeleteInspectionMaintenanceUpdateDetail(int inspectionC, string objectI, string code, DateTime implementD)
		{
			_inspectionPlanDetailService.DeleteInspectionMaintenanceUpdateDetail(inspectionC, objectI, code, implementD);
		}
	}
}