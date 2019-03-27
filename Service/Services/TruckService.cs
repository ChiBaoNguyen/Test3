using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Utilities;
using Website.ViewModels.Department;
using Website.ViewModels.Truck;
using Website.Enum;

namespace Service.Services
{
	public interface ITruckService
	{
		IEnumerable<TruckViewModel> GetTruckForSuggestion(string value);
		IEnumerable<TruckViewModel> GetTruckForSuggestionForReport(string value);
		IEnumerable<TruckViewModel> GetTrucksByCode(string value);
		TruckViewModel GetByName(string value);
		TruckDatatables GetTrucksForTable(int page, int itemsPerPage, string sortBy, bool reverse, string truckSearchValue, string searchPartNo);
		TruckViewModel GetTruckByCode(string truckC);
		IEnumerable<DepartmentViewModel> GetTruckDepartments();
		void CreateTruck(TruckViewModel truck);
		void UpdateTruck(TruckViewModel truck);
		void DeleteTruck(string id);
		void SetStatusTruck(string id);
		IEnumerable<TruckViewModel> GetTrucks();
		IEnumerable<TruckViewModel> GetTrucksForReport();
		IEnumerable<TruckViewModel> GetCompanyTrucksForReport();
		IEnumerable<TruckViewModel> GetTruckForAutosuggestByType(string value, string type);
		IEnumerable<TruckViewModel> GetTrucksByDepC(string depC);
		void UpdateOdoForTrucks(List<TruckViewModel> data);
		void SaveTruck();
		IEnumerable<TruckMaintenanceViewModel> GetMaintenanceManagementItems(string truckC, string modelC);
		bool CheckTruckLimitation(string licensePath);
		int GetCurrentTruckTotal();
		string GetDriverNameByTruckCode(string value);
	}
	public class TruckService : ITruckService
	{
		private readonly ITruckRepository _truckRepository;
		private readonly IDriverRepository _driverRepository;
		private readonly IDepartmentRepository _departmentRepository;
		private readonly IPartnerRepository _partnerRepository;
		private readonly IModelRepository _modelRepository;
		private readonly IMaintenanceItemDetailRepository _maintenanceItemDetailRepository;
		private readonly IMaintenanceDetailRepository _maintenanceDetailRepository;
		private readonly IMaintenanceItemRepository _maintenanceItemRepository;
		private readonly IMaintenancePlanDetailRepository _maintenancePlanDetailRepository;
		private readonly ILicenseValidation _licenseValidation;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IInspectionPlanDetailService _inspectionPlanDetailService;

		public TruckService(ITruckRepository truckRepository,
							IDriverRepository driverRepository,
							IDepartmentRepository departmentRepository,
							IPartnerRepository partnerRepository,
							IModelRepository modelRepository,
							ILicenseValidation licenseValidation,
							IMaintenanceItemDetailRepository maintenanceItemDetailRepository,
							IMaintenanceDetailRepository maintenanceDetailRepository,
							IMaintenanceItemRepository maintenanceItemRepository,
							IMaintenancePlanDetailRepository maintenancePlanDetailRepository,
							IUnitOfWork unitOfWork,
							IInspectionPlanDetailService inspectionPlanDetailService)
		{
			this._truckRepository = truckRepository;
			this._driverRepository = driverRepository;
			this._departmentRepository = departmentRepository;
			this._partnerRepository = partnerRepository;
			this._modelRepository = modelRepository;
			this._licenseValidation = licenseValidation;
			this._maintenanceItemDetailRepository = maintenanceItemDetailRepository;
			this._maintenanceDetailRepository = maintenanceDetailRepository;
			this._maintenanceItemRepository = maintenanceItemRepository;
			this._maintenancePlanDetailRepository = maintenancePlanDetailRepository;
			this._unitOfWork = unitOfWork;
			this._inspectionPlanDetailService = inspectionPlanDetailService;
		}

		public IEnumerable<TruckViewModel> GetTruckForSuggestion(string value)
		{
			var truck = _truckRepository.Query(t => (t.TruckC.Contains(value) ||
													t.RegisteredNo.Contains(value)) &&
													t.IsActive == Constants.ACTIVE &&
													(t.DisusedD == null || t.DisusedD >= DateTime.Now));

			var destination = Mapper.Map<IEnumerable<Truck_M>, IEnumerable<TruckViewModel>>(truck);
			foreach (var truckM in destination)
			{

				truckM.DriverN = "";

				var driver = _driverRepository.Query(dri => dri.DriverC == truckM.DriverC).FirstOrDefault();
				if (driver != null)
				{
					truckM.DriverN = driver.LastN + " " + driver.FirstN;
				}
			}

			return destination;
		}

		public IEnumerable<TruckViewModel> GetTruckForSuggestionForReport(string value)
		{
			var truck = _truckRepository.Query(t => (t.TruckC.Contains(value) ||
													t.RegisteredNo.Contains(value)));

			var destination = Mapper.Map<IEnumerable<Truck_M>, IEnumerable<TruckViewModel>>(truck);
			foreach (var truckM in destination)
			{

				truckM.DriverN = "";

				var driver = _driverRepository.Query(dri => dri.DriverC == truckM.DriverC).FirstOrDefault();
				if (driver != null)
				{
					truckM.DriverN = driver.LastN + " " + driver.FirstN;
				}
			}

			return destination;
		}

		public IEnumerable<TruckViewModel> GetTrucksByCode(string value)
		{
			var truck = _truckRepository.Query(t => t.TruckC.StartsWith(value));

			var destination = Mapper.Map<IEnumerable<Truck_M>, IEnumerable<TruckViewModel>>(truck);
			return destination;
		}

		public IEnumerable<TruckViewModel> GetTruckForAutosuggestByType(string value, string type)
		{
			var truck = from a in _truckRepository.GetAllQueryable()
						join b in _driverRepository.GetAllQueryable() on new { a.DriverC }
							equals new { b.DriverC } into t1
						from b in t1.DefaultIfEmpty()
						join c in _partnerRepository.GetAllQueryable() on new { a.PartnerMainC, a.PartnerSubC }
							equals new { c.PartnerMainC, c.PartnerSubC } into t2
						from c in t2.DefaultIfEmpty()
						where ((a.TruckC.Contains(value) || a.RegisteredNo.Contains(value)) &&
							   a.PartnerI == type && a.IsActive == "1"
							)
						select new TruckViewModel()
						{
							TruckC = a.TruckC,
							RegisteredNo = a.RegisteredNo,
							RegisteredD = a.RegisteredD,
							DepC = a.DepC,
							DriverC = a.DriverC,
							DriverN = b != null ? b.LastN + " " + b.FirstN : "",
							DriverFirstN = b != null ? b.FirstN : "",
							PartnerMainC = a.PartnerMainC,
							PartnerSubC = a.PartnerSubC,
							PartnerN = c != null ? c.PartnerN : "",
							AssistantC = a.AssistantC,
							AssistantN = ""
						};
			truck = truck.OrderBy("RegisteredNo asc");
			var truckList = truck.ToList();
			var count = truckList.Count;
			for (var i = 0; i < count; i++)
			{
				string assC = truckList[i].AssistantC;
				var driver = _driverRepository.Query(p => p.DriverC == assC).FirstOrDefault();
				truckList[i].AssistantN = driver != null ? driver.LastN + " " + driver.FirstN : "";
			}
			return truckList;
		}

		public TruckViewModel GetByName(string value)
		{
			var truck = _truckRepository.Query(t => t.RegisteredNo.Equals(value)).FirstOrDefault();
			if (truck != null)
			{
				var destination = Mapper.Map<Truck_M, TruckViewModel>(truck);

				if (!string.IsNullOrEmpty(destination.DriverC))
				{
					var driver = _driverRepository.Query(dri => dri.DriverC == destination.DriverC).FirstOrDefault();
					if (driver != null)
					{
						destination.DriverN = driver.LastN + " " + driver.FirstN;
					}
				}

				return destination;
			}
			return null;
		}

		public TruckDatatables GetTrucksForTable(int page, int itemsPerPage, string sortBy, bool reverse, string truckSearchValue, string searchPartNo)
		{
			var truck = from a in _truckRepository.GetAllQueryable()
						join b in _departmentRepository.GetAllQueryable() on new { a.DepC }
							equals new { b.DepC } into t1
						from b in t1.DefaultIfEmpty()
						join c in _driverRepository.GetAllQueryable() on new { a.DriverC }
							equals new { c.DriverC } into t2
						from c in t2.DefaultIfEmpty()
						join f in _driverRepository.GetAllQueryable() on a.AssistantC equals f.DriverC into t10
						from f in t10.DefaultIfEmpty()
						join d in _partnerRepository.GetAllQueryable() on new { a.PartnerMainC, a.PartnerSubC }
							equals new { d.PartnerMainC, d.PartnerSubC } into t3
						from d in t3.DefaultIfEmpty()
						join m in _modelRepository.GetAllQueryable() on new { a.ModelC }
							equals new { m.ModelC } into t4
						from m in t4.DefaultIfEmpty()
						where (truckSearchValue == null ||
							   a.TruckC.Contains(truckSearchValue) ||
							   a.RegisteredNo.Contains(truckSearchValue)
							  )
						select new TruckViewModel()
						{
							TruckC = a.TruckC,
							RegisteredNo = a.RegisteredNo,
							RegisteredD = a.RegisteredD,
							VIN = a.VIN,
							MakeN = a.MakeN,
							DepC = a.DepC,
							DepN = b != null ? b.DepN : "",
							DriverC = a.DriverC,
							DriverN = c != null ? c.LastN + " " + c.FirstN : "",
							AssistantC = a.DriverC,
							AssistantN = f != null ? f.LastN + " " + f.FirstN : "",
							AcquisitionD = a.AcquisitionD,
							PartnerI = a.PartnerI,
							GrossWeight = a.GrossWeight,
							PartnerMainC = a.PartnerMainC,
							PartnerSubC = a.PartnerSubC,
							PartnerN = (a.PartnerI == "1") ? d.PartnerN : (c != null ? c.LastN + " " + c.FirstN : ""),
							IsActive = a.IsActive,
							DisusedD = a.DisusedD,
							Odometer = a.Odometer,
							Status = a.Status,
							ModelN = m.ModelN,
							LossFuelRate = a.LossFuelRate
						};

			
			if(searchPartNo != null)
			{
				truck = from a in _truckRepository.GetAllQueryable()
						join b in _departmentRepository.GetAllQueryable() on new { a.DepC }
							equals new { b.DepC } into t1
						from b in t1.DefaultIfEmpty()
						join c in _driverRepository.GetAllQueryable() on new { a.DriverC }
							equals new { c.DriverC } into t2
						from c in t2.DefaultIfEmpty()
						join f in _driverRepository.GetAllQueryable() on a.AssistantC equals f.DriverC into t10
						from f in t10.DefaultIfEmpty()
						join d in _partnerRepository.GetAllQueryable() on new { a.PartnerMainC, a.PartnerSubC }
							equals new { d.PartnerMainC, d.PartnerSubC } into t3
						from d in t3.DefaultIfEmpty()
						join m in _maintenanceDetailRepository.GetAllQueryable() on a.TruckC  
							equals m.Code into t4
						from m in t4.DefaultIfEmpty()
						where (truckSearchValue == null ||
							   a.TruckC.Contains(truckSearchValue) ||
							   a.RegisteredNo.Contains(truckSearchValue)
							  ) &&
							  (m.ObjectI == Constants.TRUCK && 
							   m.PartNo.Contains(searchPartNo)
							  )

						select new TruckViewModel()
						{
							TruckC = a.TruckC,
							RegisteredNo = a.RegisteredNo,
							RegisteredD = a.RegisteredD,
							VIN = a.VIN,
							MakeN = a.MakeN,
							DepC = a.DepC,
							DepN = b != null ? b.DepN : "",
							DriverC = a.DriverC,
							DriverN = c != null ? c.LastN + " " + c.FirstN : "",
							AssistantC = a.DriverC,
							AssistantN = f != null ? f.LastN + " " + f.FirstN : "",
							AcquisitionD = a.AcquisitionD,
							PartnerI = a.PartnerI,
							GrossWeight = a.GrossWeight,
							PartnerMainC = a.PartnerMainC,
							PartnerSubC = a.PartnerSubC,
							PartnerN = d.PartnerN,
							IsActive = a.IsActive,
							DisusedD = a.DisusedD,
							LossFuelRate = a.LossFuelRate
						};
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var truckOrdered = truck.OrderBy(sortBy + (reverse ? " descending" : ""));
			// paging
			var truckPaged = truckOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var datatable = new TruckDatatables()
			{
				Data = truckPaged,
				Total = truckOrdered.Count()
			};

			return datatable;
		}

		public IEnumerable<DepartmentViewModel> GetTruckDepartments()
		{
			var groupData = from b in _truckRepository.GetAllQueryable()
							group b by new {b.DepC}  into c
							select new
							{
								c.Key.DepC,
							};

			var truckDepartments = from g in groupData
									join d in _departmentRepository.GetAllQueryable()  on g.DepC equals d.DepC into dg
									from d in dg.DefaultIfEmpty()
									where d.IsActive == Constants.ACTIVE
									select new DepartmentViewModel ()
									{
									   DepC =  g.DepC,
									   DepN = d.DepN,
									};

			return truckDepartments;
		}

		public void CreateTruck(TruckViewModel truckViewModel)
		{
			var truck = Mapper.Map<TruckViewModel, Truck_M>(truckViewModel);
			_truckRepository.Add(truck);

			SaveTruck();
		}

		public void UpdateTruck(TruckViewModel truck)
		{
			var truckToRemove = _truckRepository.GetById(truck.TruckC);
			var updateTruck = Mapper.Map<TruckViewModel, Truck_M>(truck);

			_truckRepository.Delete(truckToRemove);
			_truckRepository.Add(updateTruck);

			SaveTruck();
		}

		//using for active and deactive user
		public void SetStatusTruck(string id)
		{
			var truckToRemove = _truckRepository.Get(c => c.TruckC == id);
			if (truckToRemove.IsActive == Constants.ACTIVE)
			{
				truckToRemove.IsActive = Constants.DEACTIVE;
			}
			else
			{
				truckToRemove.IsActive = Constants.ACTIVE;
			}

			_truckRepository.Update(truckToRemove);

			SaveTruck();
		}
		public void DeleteTruck(string id)
		{
			var truckToRemove = _truckRepository.Get(c => c.TruckC == id);
			if (truckToRemove != null)
			{
				_truckRepository.Delete(truckToRemove);

				// delete in MaintenancePlan_D
				var maintenanceItemsPlan = _maintenancePlanDetailRepository.Query(x => x.ObjectI == "0" && x.Code == id);
				if (maintenanceItemsPlan != null)
				{
					foreach (var deleteItem in maintenanceItemsPlan)
					{
						_maintenancePlanDetailRepository.Delete((deleteItem));
					}
				}
				// delete Maintenance_D
				var maintenanceItems = _maintenanceDetailRepository.Query(x => x.ObjectI == "0" && x.Code == id);
				if(maintenanceItems !=null)
				{
					foreach (var deleteItem in maintenanceItems)
					{
						_maintenanceDetailRepository.Delete(deleteItem);
					}
				}

				// delete in inspectionPlan_D
				_inspectionPlanDetailService.Delete("0", id);

				SaveTruck();
			}
		}

		public TruckViewModel GetTruckByCode(string truckC)
		{
			TruckViewModel result = new TruckViewModel();

			var truck = from a in _truckRepository.GetAllQueryable()
						join b in _departmentRepository.GetAllQueryable() on new { a.DepC }
							equals new { b.DepC } into t1
						from b in t1.DefaultIfEmpty()
						join c in _driverRepository.GetAllQueryable() on new { a.DriverC }
							equals new { c.DriverC } into t2
						from c in t2.DefaultIfEmpty()
						join f in _driverRepository.GetAllQueryable() on a.AssistantC equals f.DriverC into t10
						from f in t10.DefaultIfEmpty()
						join d in _partnerRepository.GetAllQueryable() on new { a.PartnerMainC, a.PartnerSubC }
							equals new { d.PartnerMainC, d.PartnerSubC } into t3
						from d in t3.DefaultIfEmpty()
						join e in _modelRepository.GetAllQueryable() on a.ModelC
						equals e.ModelC into t4
						from e in t4.DefaultIfEmpty()
						where (a.TruckC == truckC && (e.ObjectI == null || (e.ObjectI != null && e.ObjectI == "0")))
						select new TruckViewModel()
						{
							TruckC = a.TruckC,
							RegisteredNo = a.RegisteredNo,
							RegisteredD = a.RegisteredD,
							VIN = a.VIN,
							MakeN = a.MakeN,
							DepC = a.DepC,
							DepN = b != null ? b.DepN : "",
							DriverC = a.DriverC,
							DriverN = c != null ? c.LastN + " " + c.FirstN : "",
							AssistantC = f.DriverC,
							AssistantN = f != null ? f.LastN + " " + f.FirstN : "",
							RetiredD = c != null ? c.RetiredD : null,
							AcquisitionD = a.AcquisitionD,
							PartnerI = a.PartnerI,
							GrossWeight = a.GrossWeight,
							PartnerMainC = a.PartnerMainC,
							PartnerSubC = a.PartnerSubC,
							PartnerN = d.PartnerN,
							IsActive = a.IsActive,
							DisusedD = a.DisusedD,
							Odometer = a.Odometer,
							ModelC = a.ModelC,
							ModelN = e.ModelN,
							RemodelI = a.RemodelI,
                            ModelYear = a.ModelYear,
							Status = a.Status,
							StatusFromD = a.StatusFromD,
							StatusToD = a.StatusToD,
							LossFuelRate = a.LossFuelRate
						};

			if (truck.Any())
			{
				var truckList = truck.ToList();
				result = truckList[0];
				result.TruckIndex = FindIndex(truckC);
				result.IsDisabledModelName = SetIsDisabledModelName(truckC);
			}

			return result;
		}

		public IEnumerable<TruckViewModel> GetTrucks()
		{
			var source = _truckRepository.Query(i => i.IsActive == Constants.ACTIVE && (i.DisusedD == null || i.DisusedD >= DateTime.Now));
			var destination = Mapper.Map<IEnumerable<Truck_M>, IEnumerable<TruckViewModel>>(source);
			return destination;
		}

		public IEnumerable<TruckViewModel> GetTrucksForReport()
		{
			var source = _truckRepository.GetAllQueryable();
			var destination = Mapper.Map<IEnumerable<Truck_M>, IEnumerable<TruckViewModel>>(source);
			return destination;
		}
		public IEnumerable<TruckViewModel> GetCompanyTrucksForReport()
		{
			var source = _truckRepository.GetAllQueryable().Where(t => t.PartnerI == "0");
			var destination = Mapper.Map<IEnumerable<Truck_M>, IEnumerable<TruckViewModel>>(source);
			return destination;
		}
		public IEnumerable<TruckViewModel> GetTrucksByDepC(string depC)
		{
			var truck = from a in _truckRepository.GetAllQueryable()
						join b in _driverRepository.GetAllQueryable() on a.DriverC
							equals b.DriverC into t1
						from b in t1.DefaultIfEmpty()
						join c in _partnerRepository.GetAllQueryable() on new { a.PartnerMainC, a.PartnerSubC }
							equals new { c.PartnerMainC, c.PartnerSubC } into t2
						from c in t2.DefaultIfEmpty()
						join d in _modelRepository.GetAllQueryable() on a.ModelC
							equals d.ModelC into t3
						from d in t3.DefaultIfEmpty()
						where (a.DepC == depC || depC == "0") && a.PartnerI == "0" && a.IsActive == Constants.ACTIVE && (d == null || d.ObjectI == "0")
						select new TruckViewModel()
						{
							TruckC = a.TruckC,
							RegisteredNo = a.RegisteredNo,
							RegisteredD = a.RegisteredD,
							DepC = a.DepC,
							DriverC = a.DriverC,
							DriverN = b != null ? b.LastN + " " + b.FirstN : "",
							PartnerMainC = a.PartnerMainC,
							PartnerSubC = a.PartnerSubC,
							PartnerN = c != null ? c.PartnerN : "",
							ModelC = a.ModelC,
							ModelN = d != null ? d.ModelN : "",
							AcquisitionD = a.AcquisitionD,
							RemodelI = a.RemodelI,
							Odometer = a.Odometer,
							DisusedD = a.DisusedD
						};
			truck = truck.OrderBy("RegisteredNo asc");

			return truck.ToList();
		}

		public void UpdateOdoForTrucks(List<TruckViewModel> data)
		{
			if (data != null && data.Count > 0)
			{
				for (var iloop = 0; iloop < data.Count; iloop++)
				{
					var truckC = data[iloop].TruckC;
					var odoMeter = data[iloop].Odometer;
					var truckToUpdate = _truckRepository.GetById(truckC);
					if (truckToUpdate != null)
					{
						truckToUpdate.Odometer = odoMeter;
						_truckRepository.Update(truckToUpdate);
					}
				}
				SaveTruck();
			}
		}

		private int FindIndex(string code)
		{
			var data = _truckRepository.GetAllQueryable();
			var index = 0;
			var totalRecords = data.Count();
			var halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
			var loopCapacity = 100;
			var recordsToSkip = 0;
			if (totalRecords > 0)
			{
				var nextIteration = true;
				while (nextIteration)
				{
					for (var counter = 0; counter < 2; counter++)
					{
						recordsToSkip = recordsToSkip + (counter * halfCount);

						if (data.OrderBy("TruckC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.TruckC == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("TruckC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.TruckC == code)
								{
									index = index + 1;
									index = recordsToSkip + index;
									break;
								}
								index = index + 1;
							}
							nextIteration = false;
							break;
						}
					}
				}
			}
			return index;
		}

		private bool SetIsDisabledModelName(string truckC)
		{
			bool isResult = false;

			var maintenanceD = _maintenanceDetailRepository.Query(m => m.ObjectI == "0" && m.Code == truckC);
			if (maintenanceD.Any())
			{
				isResult = true;
			}

			return isResult;
		}

		public bool CheckTruckLimitation(string licensePath)
		{
			var licenseTruckQuantity = _licenseValidation.GetTruckLimitation(licensePath);

			var trucks = _truckRepository.Query(i=>i.PartnerI == "0");
			var currTruckQuantity = trucks.Count();
			//license truck quantity must be greater than currTruckQuantity
			if (licenseTruckQuantity > currTruckQuantity)
			{
				return true;
			}

			return false;
		}

		public int GetCurrentTruckTotal()
		{
			//var trucks = _truckRepository.GetAll().Count();
			var trucks = _truckRepository.Query(i => i.PartnerI == "0" && i.IsActive == "1").Count();
			return trucks;
		}

		public void SaveTruck()
		{
			_unitOfWork.Commit();
		}


		public IEnumerable<TruckMaintenanceViewModel> GetMaintenanceManagementItems(string truckC, string modelC)
		{
			List<TruckMaintenanceViewModel> result = new List<TruckMaintenanceViewModel>();

			var managementItems = from a in _maintenanceItemDetailRepository.GetAllQueryable()
				join b in _maintenanceItemRepository.GetAllQueryable() on a.MaintenanceItemC
					equals b.MaintenanceItemC
					where (a.ModelC == modelC && a.ObjectI == "0")
				select new TruckMaintenanceViewModel()
				{
					ObjectI = a.ObjectI,
					MaintenanceItemC = a.MaintenanceItemC,
					MaintenanceItemN = b.MaintenanceItemN,
					NoticeI = b.NoticeI,
					ReplacementInterval = b.ReplacementInterval,
					NoticeNo = b.NoticeNo
				};

			foreach (TruckMaintenanceViewModel item in managementItems)
			{
				TruckMaintenanceViewModel newItem = new TruckMaintenanceViewModel();
				newItem = item;
				var maintenanceItemHistory = (from a in _maintenanceDetailRepository.GetAllQueryable()
											  where (
														a.ObjectI == "0" &&
														a.Code == truckC &&
														a.MaintenanceItemC == item.MaintenanceItemC)
														orderby a.MaintenanceD descending
														select new TruckMaintenanceViewModel()
														{
															TruckC = a.Code,
															PlanMaintenanceD = a.PlanMaintenanceD,
															PlanMaintenanceKm = a.PlanMaintenanceKm,
															Description = a.Description
														}).FirstOrDefault();
				var maintenancePlan = (from a in _maintenancePlanDetailRepository.GetAllQueryable()
									   where (
												 a.ObjectI == "0" &&
												 a.Code == truckC &&
												 a.MaintenanceItemC == item.MaintenanceItemC)
									   select new TruckMaintenanceViewModel()
									   {
										   NextMaintenanceD = a.PlanMaintenanceD,
										   NextMaintenanceKm = a.PlanMaintenanceKm
									   }).FirstOrDefault();

				if (maintenanceItemHistory != null)
				{
					newItem.TruckC = maintenanceItemHistory.TruckC;
					newItem.PlanMaintenanceD = maintenanceItemHistory.PlanMaintenanceD;
					newItem.PlanMaintenanceKm = maintenanceItemHistory.PlanMaintenanceKm;
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

		public string GetDriverNameByTruckCode(string value)
		{
			string finalName = "";
			var truck = _truckRepository.Query(t => t.TruckC == value).FirstOrDefault();
			if (truck != null)
			{
				if (!string.IsNullOrEmpty(truck.DriverC))
				{
					var driver = _driverRepository.Query(dri => dri.DriverC == truck.DriverC).FirstOrDefault();
					if (driver != null)
					{
						finalName = driver.DriverC + " - " + driver.LastN + " " + driver.FirstN;
					}
				}
			}
			return finalName;
		}
	}
}