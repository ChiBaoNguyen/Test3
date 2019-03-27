using System.Data.SqlClient;
using Root.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using Root.Data.Repository;
using Root.Models;
using Root.Models.Calendar;
using Website.Enum;
using Website.ViewModels.DriverLicense;
using AutoMapper;
using Website.ViewModels.DriverLicenseUpdate;

namespace Service.Services
{
	public interface IDriverLicenseService
	{
		List<CalendarPlanItem> GetDriverLicensePlan(DateTime date, string depC, bool donePlan, bool undonePlan, string driverC, DateTime? fromDate, DateTime? toDate, string contentC);
		List<CalendarPlanItemForCounting> GetDriverLicensePlanForCounting(string year, string depC);
		DriverLicenseUpdateViewModel GetDriverLicenseUpdateDetail(string driverC, string licenseC, DateTime updateD, int planI);
		void UpdateDriverLicenseDetail(DriverLicenseUpdateViewModel data);
		void DeleteDriverLicenseUpdateDetail(string driverC, string licenseC, DateTime expiryD);
		void UpdateDriverLicenses(List<DriverLicenseViewModel> listValue);
		void SaveDriverLicense();

		IEnumerable<DriverLicenseViewModel> GetByDriverC(string driverC);
	}
	public class DriverLicenseService : IDriverLicenseService
	{
		private readonly IDriverLicenseRepository _driverLicenseRepository;
		private readonly IDriverLicenseUpdateRepository _driverLicenseUpdateRepository;
		private readonly IDriverRepository _driverRepository;
		private readonly ILicenseRepository _licenseRepository;
		private readonly IUnitOfWork _unitOfWork;

		public DriverLicenseService(IDriverLicenseRepository driverLicenseRepository,
									IDriverLicenseUpdateRepository driverLicenseUpdateRepository,
									IDriverRepository driverRepository,
									ILicenseRepository licenseRepository,
									IUnitOfWork unitOfWork)
		{
			_driverLicenseRepository = driverLicenseRepository;
			_driverLicenseUpdateRepository = driverLicenseUpdateRepository;
			_driverRepository = driverRepository;
			_licenseRepository = licenseRepository;
			_unitOfWork = unitOfWork;
		}
		public void SaveDriverLicense()
		{
			_unitOfWork.Commit();
		}

		public void UpdateDriverLicenses(List<DriverLicenseViewModel> newLicenseList)
		{
			if (newLicenseList.Count == 0) return;

			var oldLicenseList = _driverLicenseRepository.GetAll().Where(x => x.DriverC == newLicenseList[0].DriverC);

			//xoa
			foreach (var item in oldLicenseList)
			{
				if (newLicenseList.Any(x => x.DriverC == item.DriverC && x.LicenseC == item.LicenseC) == false)
				{
					_driverLicenseRepository.Delete(item);
				}
			}
			//update
			foreach (var item in newLicenseList)
			{
				if (oldLicenseList.Any(x => x.DriverC == item.DriverC && x.LicenseC == item.LicenseC) == false)
				{
					var addItem = Mapper.Map<DriverLicenseViewModel, DriverLicense_M>(item);
					_driverLicenseRepository.Add(addItem);
				}
				else
				{
					var updateDriver = oldLicenseList.Where(x => x.DriverC == item.DriverC && x.LicenseC == item.LicenseC).FirstOrDefault();
					if (updateDriver != null)
					{
						updateDriver.DriverLicenseD = item.DriverLicenseD;
						updateDriver.DriverLicenseNo = item.DriverLicenseNo;
						updateDriver.ExpiryD = item.ExpiryD;
						_driverLicenseRepository.Update(updateDriver);
					}
				}
			}

			SaveDriverLicense();
		}

		public IEnumerable<DriverLicenseViewModel> GetByDriverC(string driverC)
		{
			var source = _driverLicenseRepository.Query(x => x.DriverC == driverC);
			return (from a in _driverLicenseRepository.GetAllQueryable()
					join b in _licenseRepository.GetAllQueryable() on a.LicenseC equals b.LicenseC
					where a.DriverC == driverC
					select new DriverLicenseViewModel()
					{
						DriverC = a.DriverC,
						LicenseC = a.LicenseC,
						LicenseN = b.LicenseN,
						DriverLicenseD = a.DriverLicenseD,
						DriverLicenseNo = a.DriverLicenseNo,
						ExpiryD = a.ExpiryD
					});
		}

		public DriverLicenseUpdateViewModel GetDriverLicenseUpdateDetail(string driverC, string licenseC, DateTime updateD, int planI)
		{
			var result = new DriverLicenseUpdateViewModel();

			var updateDMax = _driverLicenseUpdateRepository.Query(d => d.DriverC == driverC && d.LicenseC == licenseC).OrderBy("UpdateD desc").FirstOrDefault();
			var driverLicenseUpdate = from a in _driverLicenseUpdateRepository.GetAllQueryable()
									  join b in _driverRepository.GetAllQueryable() on a.DriverC equals b.DriverC
									  join c in _licenseRepository.GetAllQueryable() on a.LicenseC equals c.LicenseC
									  where (a.DriverC == driverC &&
											 a.LicenseC == licenseC &&
											 a.UpdateD == updateD
										  )
									  select new DriverLicenseUpdateViewModel()
									  {
										  DriverC = a.DriverC,
										  DriverN = b != null ? b.LastN + " " + b.FirstN : "",
										  LicenseC = a.LicenseC,
										  LicenseN = c != null ? c.LicenseN : "",
										  ExpiryD = a.ExpiryD,
										  UpdateD = a.UpdateD,
										  NextExpiryD = a.NextExpiryD
									  };

			if (planI == 1)
			{
				if (!driverLicenseUpdate.Any())
				{
					var driverLicenseeList = (from a in _driverLicenseRepository.GetAllQueryable()
											  join b in _driverRepository.GetAllQueryable() on a.DriverC equals b.DriverC
											  join c in _licenseRepository.GetAllQueryable() on a.LicenseC equals c.LicenseC
											  where (a.DriverC == driverC &&
													 a.LicenseC == licenseC
												  )
											  select new DriverLicenseViewModel()
											  {
												  DriverC = a.DriverC,
												  DriverN = b != null ? b.LastN + " " + b.FirstN : "",
												  LicenseC = a.LicenseC,
												  LicenseN = c != null ? c.LicenseN : "",
												  ExpiryD = a.ExpiryD,
											  }).ToList();

					result.DriverC = driverLicenseeList[0].DriverC;
					result.DriverN = driverLicenseeList[0].DriverN;
					result.LicenseC = driverLicenseeList[0].LicenseC;
					result.LicenseN = driverLicenseeList[0].LicenseN;
					result.ExpiryD = driverLicenseeList[0].ExpiryD != null
						? (DateTime)driverLicenseeList[0].ExpiryD
						: DateTime.MinValue;
					result.UpdateD = updateD;
					result.IsDisableNextExpiryD = false;
					result.IsInsert = true;
					result.IsDelete = false;
				}
				else
				{
					var driverLicenseUpdateList = driverLicenseUpdate.ToList();

					result.DriverC = driverLicenseUpdateList[0].DriverC;
					result.DriverN = driverLicenseUpdateList[0].DriverN;
					result.LicenseC = driverLicenseUpdateList[0].LicenseC;
					result.LicenseN = driverLicenseUpdateList[0].LicenseN;
					result.ExpiryD = driverLicenseUpdateList[0].NextExpiryD;
					result.UpdateD = updateD;
					result.IsDisableNextExpiryD = false;
					result.IsInsert = true;
					result.IsDelete = false;
				}
			}
			else
			{
				if (updateDMax != null)
				{
					var driverLicenseUpdateList = driverLicenseUpdate.ToList();

					result.DriverC = driverLicenseUpdateList[0].DriverC;
					result.DriverN = driverLicenseUpdateList[0].DriverN;
					result.LicenseC = driverLicenseUpdateList[0].LicenseC;
					result.LicenseN = driverLicenseUpdateList[0].LicenseN;
					result.ExpiryD = driverLicenseUpdateList[0].ExpiryD;
					result.UpdateD = updateD;
					result.NextExpiryD = driverLicenseUpdateList[0].NextExpiryD;

					if (driverLicenseUpdateList[0].UpdateD == updateDMax.UpdateD)
					{
						result.IsDisableNextExpiryD = false;
						result.IsInsert = true;
						result.IsDelete = true;
					}
					else
					{
						result.IsDisableNextExpiryD = true;
						result.IsInsert = false;
						result.IsDelete = false;
					}
				}
			}

			return result;
		}

		public void UpdateDriverLicenseDetail(DriverLicenseUpdateViewModel data)
		{
			if (data.PlanI == 1)
			{
				// insert to DriverLicenseUpdate_D table
				var driverLicenceInsert = Mapper.Map<DriverLicenseUpdateViewModel, DriverLicenseUpdate_D>(data);
				_driverLicenseUpdateRepository.Add(driverLicenceInsert);
			}
			else
			{
				// update DriverLicenseUpdate_D
				var driverLicenceUpdate = Mapper.Map<DriverLicenseUpdateViewModel, DriverLicenseUpdate_D>(data);
				var driverLicenceDelete = _driverLicenseUpdateRepository.Query(d => d.DriverC == data.DriverC && d.LicenseC == data.LicenseC && d.ExpiryD == data.ExpiryD).FirstOrDefault();
				_driverLicenseUpdateRepository.Delete(driverLicenceDelete);
				_driverLicenseUpdateRepository.Add(driverLicenceUpdate);
			}

			// update to DriverLicense_M table
			var driverC = data.DriverC;
			var licenseC = data.LicenseC;

			var driverLicense = _driverLicenseRepository.Query(d => d.DriverC == driverC && d.LicenseC == licenseC).FirstOrDefault();
			if (driverLicense != null)
			{
				driverLicense.ExpiryD = data.NextExpiryD;
				_driverLicenseRepository.Update(driverLicense);
			}

			SaveDriverLicense();
		}

		public void DeleteDriverLicenseUpdateDetail(string driverC, string licenseC, DateTime expiryD)
		{
			var driverLicenseUpdate = _driverLicenseUpdateRepository.Query(d => d.DriverC == driverC &&
																				d.LicenseC == licenseC &&
																				d.ExpiryD == expiryD
																			).FirstOrDefault();
			_driverLicenseUpdateRepository.Delete(driverLicenseUpdate);

			// update DriverLicense_M
			var driverLicense = _driverLicenseRepository.Query(d => d.DriverC == driverC && d.LicenseC == licenseC).FirstOrDefault();
			if (driverLicense != null)
			{
				driverLicense.ExpiryD = expiryD;
				_driverLicenseRepository.Update(driverLicense);
			}

			SaveDriverLicense();
		}

		public List<CalendarPlanItem> GetDriverLicensePlan(DateTime date, string depC, bool donePlan, bool undonePlan, string driverC, DateTime? fromDate, DateTime? toDate, string contentC)
		{
			var calendarPlan = new List<CalendarPlanItem>();

			if (undonePlan)
			{
				//check all driver license's expiry date in two month
				var checkDate = date.AddMonths(2);

				//get all driver have expriy date of license less than 2 month
				var notUpdatedLicenses = (from p in _driverLicenseRepository.GetAllQueryable()
										  join d in _driverRepository.GetAllQueryable() on p.DriverC equals d.DriverC
										  join l in _licenseRepository.GetAllQueryable() on p.LicenseC equals l.LicenseC
										  where p.ExpiryD <= checkDate && (depC == "-1" || d.DepC == depC) &&
												(string.IsNullOrEmpty(driverC) || d.DriverC ==  driverC) &&
												(fromDate == null || p.ExpiryD >= fromDate) &&
												(toDate == null || p.ExpiryD <= toDate) &&
												(contentC == null || contentC.Equals(l.LicenseC)) &&
												d.IsActive == "1"
										  select new
										  {
											  PlanItemC = l.LicenseC,
											  PlanItemN = l.LicenseN,
											  DriverFristN = d.FirstN,
											  DriverLastN = d.LastN,
											  PlanD = p.ExpiryD, 
											  p.DriverC
										  }).ToList();

				//check PlanDStatus and PlanItemStatus
				foreach (var d in notUpdatedLicenses)
				{
					var planItem = new CalendarPlanItem
					{
						PlanItemC = d.PlanItemC,
						PlanItemN = d.PlanItemN,
						ObjectC = d.DriverC,
						ObjectN = d.DriverLastN + " " + d.DriverFristN,
						PlanTime = d.PlanD.ToString("dd/MM/yyyy"),
						UnitI = Convert.ToInt32(PlanUnit.Day).ToString(),
						PlanItemStatus = Convert.ToInt32(PlanItemStatus.Undone).ToString(),
						PlanType = Convert.ToInt32(PlanType.License).ToString(),
					};

					if (date.Date.CompareTo(d.PlanD.Date) > 0)
					{
						planItem.RemainPlanNo = (int)d.PlanD.Subtract(date).TotalDays;
						planItem.PlanStatus = Convert.ToInt32(PlanStatus.LatePlan).ToString();
					}
					else if (date.Date.CompareTo(d.PlanD.Date) == 0)
					{
						planItem.RemainPlanNo = 0;
						planItem.PlanStatus = Convert.ToInt32(PlanStatus.OnPlan).ToString();
					}
					else
					{
						planItem.RemainPlanNo = (int)d.PlanD.Subtract(date.Date).TotalDays;
						planItem.PlanStatus = Convert.ToInt32(PlanStatus.EarlyPlan).ToString();
					}
					calendarPlan.Add(planItem);
				}
			}

			if (donePlan)
			{
				//get the plan item have been done on selected date
				var selectedDate = date;
				var planUnitDay = Convert.ToInt32(PlanUnit.Day).ToString();
				var planItemStatusDone = Convert.ToInt32(PlanItemStatus.Done).ToString();
				var planType = Convert.ToInt32(PlanType.License).ToString();
				var updatedLicenses = (from p in _driverLicenseUpdateRepository.GetAll()
									   join d in _driverRepository.GetAllQueryable() on p.DriverC equals d.DriverC
									   join l in _licenseRepository.GetAllQueryable() on p.LicenseC equals l.LicenseC
									   where p.UpdateD == selectedDate &&
											(string.IsNullOrEmpty(driverC) || d.DriverC == driverC) &&
											(fromDate == null || p.NextExpiryD >= fromDate) &&
											(toDate == null || p.NextExpiryD <= toDate) &&
											(contentC == null || contentC.Equals(l.LicenseC))
									   select new CalendarPlanItem()
									   {
										   PlanItemC = l.LicenseC,
										   PlanItemN = l.LicenseN,
										   ObjectC = d.DriverC,
										   ObjectN = d.LastN + " " + d.FirstN,
										   PlanTime = p.NextExpiryD.ToString("dd/MM/yyyy"),
										   UnitI = planUnitDay,
										   RemainPlanNo = (int)p.NextExpiryD.Subtract(date.Date).TotalDays,
										   PlanItemStatus = planItemStatusDone,
										   PlanType = planType
									   }).ToList();

				if (updatedLicenses.Any())
				{
					calendarPlan.AddRange(updatedLicenses);
				}
			}
			
			return calendarPlan;
		}

		public List<CalendarPlanItemForCounting> GetDriverLicensePlanForCounting(string year, string depC)
		{
			var sYear = new SqlParameter("@year", year);
			var sDepc = new SqlParameter("@depC", depC);
			var calendarPlan = _driverLicenseRepository.ExecSpToGetPlanForCounting("GetLicenseCountingForCalendar @year, @depC", sYear, sDepc);
			return calendarPlan.ToList();
		}
	}
}