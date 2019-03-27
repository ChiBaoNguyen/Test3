using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Models.Calendar;
using Website.Enum;

namespace Service.Services
{
	public interface ICalendarService
	{
		List<CalendarPlanItem> GetCalendarPlan(DateTime date, string depC, bool donePlan, bool undonePlan, string objectI, string objectC, DateTime? fromDate, DateTime? toDate, string contentC);
		List<CalendarPlanItemForCounting> GetCalendarPlanForCounting(string year, string depC);

	}

	public class CalendarService : ICalendarService
	{
		private readonly IDriverLicenseService _driverLicenseService;
		private readonly IMaintenanceDetailService _maintenanceDetailService;
		private readonly IInspectionDetailService _inspectionDetailService;

		public CalendarService(IDriverLicenseService driverLicenseService, IMaintenanceDetailService maintenanceDetailService, IInspectionDetailService inspectionDetailService)
		{
			this._driverLicenseService = driverLicenseService;
			this._maintenanceDetailService = maintenanceDetailService;
			this._inspectionDetailService = inspectionDetailService;
		}

		public List<CalendarPlanItem> GetCalendarPlan(DateTime date, string depC, bool donePlan, bool undonePlan, string objectI, string objectC, DateTime? fromDate, DateTime? toDate, string contentC)
		{
			var calendarPlan = new List<CalendarPlanItem>();
			if (objectI.Equals("2"))
			{
				var licensePlan = _driverLicenseService.GetDriverLicensePlan(date, depC, donePlan, undonePlan, objectC, fromDate, toDate, contentC);
				calendarPlan.AddRange(licensePlan);
			}
			else
			{
				var inspectionPlan = _inspectionDetailService.GetInspectionPlan(date, depC, donePlan, undonePlan, objectI, objectC, fromDate, toDate, contentC);
				var maintenancePlan = _maintenanceDetailService.GetMaintenancePlan(date, depC, donePlan, undonePlan, objectI, objectC, fromDate, toDate, contentC);
				calendarPlan.AddRange(inspectionPlan);
				calendarPlan.AddRange(maintenancePlan);
			}
			calendarPlan = calendarPlan.OrderBy(a => a.RemainPlanNo).ThenBy(p=>p.PlanItemStatus).ToList();
			var donePlanList = calendarPlan.Where(p => p.PlanItemStatus.Equals("1")).ToList();
			calendarPlan.RemoveAll(p => p.PlanItemStatus.Equals("1"));
			for (int i = 0; i < donePlanList.Count(); i++)
			{
				if (donePlanList[i].PlanType == Convert.ToInt32(PlanType.License).ToString())
				{
					calendarPlan.Add(donePlanList[i]);
				}
				if (donePlanList[i].PlanType == Convert.ToInt32(PlanType.Maintainence).ToString())
				{
					int j = i;
					var inspBelongTo =
						(donePlanList.Where(p => p.PlanItemC == donePlanList[j].InspectionC && 
											p.ObjectC == donePlanList[j].ObjectC && 
											p.PlanType == Convert.ToInt32(PlanType.Inspection).ToString())).ToList();
					if (!inspBelongTo.Any())
					{
						calendarPlan.Add(donePlanList[i]);
					}
				}
				if (donePlanList[i].PlanType == Convert.ToInt32(PlanType.Inspection).ToString())
				{
					var g = i;
					calendarPlan.Add(donePlanList[i]);
					var maintenanceItemBelongTo = (donePlanList.Where(p => p.InspectionC == donePlanList[g].PlanItemC &&
																	p.ObjectC == donePlanList[g].ObjectC &&
																	p.PlanType == Convert.ToInt32(PlanType.Maintainence).ToString())).ToList();
					if (maintenanceItemBelongTo.Any())
					{
						foreach (var calendarPlanItem in maintenanceItemBelongTo)
						{
							calendarPlanItem.IsChildPlan = "1";
						}
						calendarPlan.AddRange(maintenanceItemBelongTo);
					}
				}
			}
			return calendarPlan;
		}

		public List<CalendarPlanItemForCounting> GetCalendarPlanForCounting(string year, string depC)
		{
			var planForCounting = new List<CalendarPlanItemForCounting>();
			var licensePlan = _driverLicenseService.GetDriverLicensePlanForCounting(year, depC);
			var inspectionPlan = _inspectionDetailService.GetInspectionPlanForCounting(year, depC);
			var maintenancePlan = _maintenanceDetailService.GetMaintenancePlanForCounting(year, depC);
			planForCounting.AddRange(licensePlan);
			planForCounting.AddRange(inspectionPlan);
			planForCounting.AddRange(maintenancePlan);
			return planForCounting.ToList();
		}
	}
}
