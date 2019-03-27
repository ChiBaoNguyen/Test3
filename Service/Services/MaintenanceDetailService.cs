using System.Data.SqlClient;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using Root.Models.Calendar;
using Website.ViewModels.MaintenanceDetail;

namespace Service.Services
{
	public interface IMaintenanceDetailService
	{
		IEnumerable<MaintenanceDetailViewModel> Get(string objectI, string code, string modelC);
		List<CalendarPlanItem> GetMaintenancePlan(DateTime date, string depC, bool donePlan, bool undonePlan, 
			string objectI, string objectC, DateTime? fromDate, DateTime? toDate, string contentC);
		List<CalendarPlanItemForCounting> GetMaintenancePlanForCounting(string year, string depC);
	}
	public class MaintenanceDetailService : IMaintenanceDetailService
	{
		private readonly IMaintenanceDetailRepository _maintenanceDetailRepository;
		private readonly IMaintenanceItemRepository _maintenanceItemRepository;
		private readonly IMaintenancePlanDetailRepository _maintenancePlanDetailRepository;
		private readonly IMaintenanceItemDetailRepository _maintenanceItemDetailRepository;
		private readonly IUnitOfWork _unitOfWork;
		public MaintenanceDetailService(IUnitOfWork unitOfWork, 
			IMaintenanceDetailRepository maintenanceDetailRepository, 
			IMaintenanceItemRepository maintenanceItemRepository, 
			IMaintenanceItemDetailRepository maintenanceItemDetailRepository,
			IMaintenancePlanDetailRepository maintenancePlanDetailRepository)
		{
			_unitOfWork = unitOfWork;
			_maintenanceDetailRepository = maintenanceDetailRepository;
			_maintenanceItemRepository = maintenanceItemRepository;
			_maintenanceItemDetailRepository = maintenanceItemDetailRepository;
			_maintenancePlanDetailRepository = maintenancePlanDetailRepository;
		}
		public IEnumerable<MaintenanceDetailViewModel> Get(string objectI, string code, string modelC)
		{
			var result = new List<MaintenanceDetailViewModel>();

			var managementItems = from a in _maintenanceItemDetailRepository.GetAllQueryable()
								  join b in _maintenanceItemRepository.GetAllQueryable() on a.MaintenanceItemC
									  equals b.MaintenanceItemC
								  where a.ModelC == modelC && a.ObjectI == objectI
								  orderby a.DisplayLineNo
								  select new MaintenanceDetailViewModel()
								  {
									  ObjectI = a.ObjectI,
									  MaintenanceItemC = a.MaintenanceItemC,
									  MaintenanceItemN = b.MaintenanceItemN,
									  NoticeI = b.NoticeI,
									  ReplacementInterval = b.ReplacementInterval,
									  NoticeNo = b.NoticeNo
								  };

			foreach (var item in managementItems)
			{
				var newItem = item;
				var maintenanceItemHistory = (from a in _maintenanceDetailRepository.GetAllQueryable()
											  where (
														a.ObjectI == objectI &&
														a.Code == code &&
														a.MaintenanceItemC == item.MaintenanceItemC)
											  orderby a.MaintenanceD descending
											  select new MaintenanceDetailViewModel()
											  {
												  TruckC = a.Code,
												  PlanMaintenanceD = a.MaintenanceD,
												  PlanMaintenanceKm = a.Odometer,
												  PartNo = a.PartNo,
												  Description = a.Description
											  }).FirstOrDefault();
				// get next plan
				var maintenancePlan = (from a in _maintenancePlanDetailRepository.GetAllQueryable()
									   where (
												 a.ObjectI == objectI &&
												 a.Code == code &&
												 a.MaintenanceItemC == item.MaintenanceItemC)
									   select new MaintenanceDetailViewModel()
									   {
										   NextMaintenanceD = a.PlanMaintenanceD,
										   NextMaintenanceKm = a.PlanMaintenanceKm
									   }).FirstOrDefault();

				if (maintenanceItemHistory != null)
				{
					newItem.TruckC = maintenanceItemHistory.TruckC;
					newItem.PlanMaintenanceD = maintenanceItemHistory.PlanMaintenanceD;
					newItem.PlanMaintenanceKm = maintenanceItemHistory.PlanMaintenanceKm;
					newItem.PartNo = maintenanceItemHistory.PartNo;
					newItem.Description = maintenanceItemHistory.Description;
				}
				if (maintenancePlan != null)
				{
					newItem.NextMaintenanceD = maintenancePlan.NextMaintenanceD;
					newItem.NextMaintenanceKm = maintenancePlan.NextMaintenanceKm;
				}

				result.Add(newItem);

			}

			return result;
		}

		public List<CalendarPlanItem> GetMaintenancePlan(DateTime date, string depC, bool donePlan, bool undonePlan, 
			string objectI, string objectC, DateTime? fromDate, DateTime? toDate, string contentC)
		{
			//var dateString = date.ToString("yyyy-MM-dd");
			var dateString = new SqlParameter("@date", date.ToString("yyyy-MM-dd"));
			var depCString = new SqlParameter("@depC", depC);
			var bDonePlan = new SqlParameter("@donePlan", donePlan);
			var bUndonePlan = new SqlParameter("@undonePlan", undonePlan);
			var sObjectI = new SqlParameter("@objectI", objectI);
			var sObjectC = new SqlParameter("@objectC", objectC);
			if (sObjectC.Value == null)
				sObjectC.Value = DBNull.Value;
			var sFromDate = new SqlParameter("@fromDate", fromDate);
			if (sFromDate.Value == null)
				sFromDate.Value = DBNull.Value;
			else
				sFromDate.Value = fromDate.Value.ToString("yyyy-MM-dd");
			var sToDate = new SqlParameter("@toDate", toDate);
			if (sToDate.Value == null)
				sToDate.Value = DBNull.Value;
			else
				sToDate.Value = toDate.Value.ToString("yyyy-MM-dd");
			var sContentC = new SqlParameter("@contentC", contentC);
			if (sContentC.Value == null)
				sContentC.Value = DBNull.Value;
			var calendarPlan = _maintenanceDetailRepository.ExecExecSpToGetPlan("GetMaintenancePlan @date, @depC, @donePlan, @undonePlan, @objectI, @objectC, @fromDate, @toDate, @contentC",
				dateString, depCString, bDonePlan, bUndonePlan, sObjectI, sObjectC, sFromDate, sToDate, sContentC);
			return calendarPlan.ToList();
		}

		public List<CalendarPlanItemForCounting> GetMaintenancePlanForCounting(string year, string depC)
		{
			var sYear = new SqlParameter("@year", year);
			var sDepC = new SqlParameter("@depC", depC);
			var planItemForCounting = _maintenanceDetailRepository.ExecSpToGetPlanForCounting("GetMaintenanceCountingForCalendar @year, @depC", sYear, sDepC);
			return planItemForCounting.ToList();
		}
	}
}
