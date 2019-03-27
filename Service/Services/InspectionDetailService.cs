using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Root.Models.Calendar;
using Website.ViewModels.InspectionDetail;
using Website.ViewModels.Truck;

namespace Service.Services
{
	public interface IInspectionDetailService
	{
		IEnumerable<InspectionDetailViewModel> Get(string objectI, string truckC);
		List<CalendarPlanItem> GetInspectionPlan(DateTime date, string depC, bool donePlan, bool undonePlan, 
			string objectI, string objectC, DateTime? fromDate, DateTime? toDate, string contentC);
		List<CalendarPlanItemForCounting> GetInspectionPlanForCounting(string year, string depC);
	}
	public class InspectionDetailService : IInspectionDetailService
	{
		private readonly IInspectionDetailRepository _inspectionDetailRepository;
		private readonly IInspectionRepository _inspectionRepository;
		private readonly IUnitOfWork _unitOfWork;

		public InspectionDetailService(IUnitOfWork unitOfWork, IInspectionDetailRepository inspectionDetailRepository,
				IInspectionRepository inspectionRepository)
		{
			_unitOfWork = unitOfWork;
			_inspectionDetailRepository = inspectionDetailRepository;
			_inspectionRepository = inspectionRepository;
		}
		public IEnumerable<InspectionDetailViewModel> Get(string objectI, string truckC)
		{
			var data = from a in _inspectionDetailRepository.GetAllQueryable()
					   join b in _inspectionRepository.GetAllQueryable() on a.InspectionC equals b.InspectionC
					   where (a.ObjectI == objectI && a.Code == truckC)
					   select new InspectionDetailViewModel()
					   {
						   ObjectI = a.ObjectI,
						   Code = a.Code,
						   InspectionC = a.InspectionC,
						   InspectionN = b.InspectionN,
						   InspectionD = a.InspectionD,
						   InspectionPlanD = a.InspectionPlanD,
						   Odometer = a.Odometer,
						   Description = a.Description
					   };
			var dataMaintenance = from a in _inspectionDetailRepository.GetAllQueryable()
								  where (a.ObjectI == objectI && a.Code == truckC && a.InspectionC == 0)
								  select new InspectionDetailViewModel()
								  {
									  ObjectI = a.ObjectI,
									  Code = a.Code,
									  InspectionC = a.InspectionC,
									  InspectionN = "",
									  InspectionD = a.InspectionD,
									  InspectionPlanD = a.InspectionPlanD,
									  Odometer = a.Odometer,
									  Description = a.Description
								  };
			
			if (dataMaintenance.Any())
			{
				data = data.Concat(dataMaintenance);
			}

			if (data.Any())
			{
				return data.ToList().OrderBy(x=>x.InspectionPlanD).ThenBy(x=>x.InspectionD);
			}
			return null;
		}

		public List<CalendarPlanItem> GetInspectionPlan(DateTime date, string depC, bool donePlan, bool undonePlan, 
			string objectI, string objectC, DateTime? fromDate, DateTime? toDate, string contentC)
		{
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

			var calendarPlan = _inspectionDetailRepository.ExecSpToGetPlan("GetInspectionPlan @date, @depC, @donePlan, @undonePlan, @objectI, @objectC, @fromDate, @toDate, @contentC",
				dateString, depCString, bDonePlan, bUndonePlan, sObjectI, sObjectC, sFromDate, sToDate, sContentC);
			return calendarPlan.ToList();
		}

		public List<CalendarPlanItemForCounting> GetInspectionPlanForCounting(string year, string depC)
		{
			var sYear = new SqlParameter("@year", year);
			var sDepC = new SqlParameter("@depC", depC);
			var planItemForCounting = _inspectionDetailRepository.ExecSpToGetPlanForCounting("GetInspectionCountingForCalendar @year, @depC", sYear, sDepC);
			return planItemForCounting.ToList();
		}
	}
}
