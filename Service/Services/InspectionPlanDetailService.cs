using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.InspectionPlanDetail;
using Website.ViewModels.Truck;
using Website.ViewModels.InspectionDetail;
using Website.ViewModels.Trailer;
using Website.ViewModels.MaintenanceDetail;

namespace Service.Services
{
	public interface IInspectionPlanDetailService
	{
		void Add(List<InspectionPlanDetailViewModel> data, string objectI, string code);
		IEnumerable<InspectionPlanDetailViewModel> Get(string objectI, string code);
		void Delete(string objectI, string code);
		InspectionMaintenancePlanViewModel GetInspectionMaintenanceUpdateDetail(int inspectionC, string objectI, string code, DateTime implementD, int planI, int intCase, int maintenanceItemC);
		void UpdateInspectionMaintenanceUpdateDetail(InspectionMaintenancePlanViewModel data);
		void DeleteInspectionMaintenanceUpdateDetail(int inspectionC, string objectI, string code, DateTime implementD);
	}
	public class InspectionPlanDetailService : IInspectionPlanDetailService
	{
		private readonly IInspectionPlanDetailRepository _inspectionPlanDetailRepository;
		private readonly IInspectionRepository _inspectionRepository;
		private readonly IInspectionDetailRepository _inspectionDetailRepository;
		private readonly ITruckRepository _truckRepository;
		private readonly IModelRepository _modelRepository;
		private readonly ITrailerRepository _trailerRepository;
		private readonly IMaintenanceItemRepository _maintenanceItemRepository;
		private readonly IMaintenanceItemDetailRepository _maintenanceItemDetailRepository;
		private readonly IMaintenanceDetailRepository _maintenanceDetailRepository;
		private readonly IMaintenancePlanDetailRepository _maintenancePlanDetailRepository;
		private readonly ITextResourceRepository _textResourceRepository;
		private readonly ITruckExpenseRepository _truckExpenseRepository;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly IExpenseRepository _expenseRepository;
		private readonly ISupplierRepository _supplierRepository;
		private readonly IDriverRepository _driverRepository;
		private readonly IUnitOfWork _unitOfWork;

		public InspectionPlanDetailService(IInspectionPlanDetailRepository inspectionPlanDetailRepository,
										   IInspectionRepository inspectionRepository,
										   IInspectionDetailRepository inspectionDetailRepository,
										   ITruckRepository truckRepository,
										   IModelRepository modelRepository,
										   ITrailerRepository trailerRepository,
										   IMaintenanceItemRepository maintenanceItemRepository,
										   IMaintenanceItemDetailRepository maintenanceItemDetailRepository,
										   IMaintenanceDetailRepository maintenanceDetailRepository,
										   IMaintenancePlanDetailRepository maintenancePlanDetailRepository,
										   ITextResourceRepository textResourceRepository,
											ITruckExpenseRepository truckExpenseRepository,
											IEmployeeRepository employeeRepository,
											IExpenseRepository expenseRepository,
											ISupplierRepository supplierRepository,
											IDriverRepository driverRepository,
										   IUnitOfWork unitOfWork)
		{
			_inspectionPlanDetailRepository = inspectionPlanDetailRepository;
			_inspectionDetailRepository = inspectionDetailRepository;
			_truckRepository = truckRepository;
			_modelRepository = modelRepository;
			_trailerRepository = trailerRepository;
			_maintenanceItemRepository = maintenanceItemRepository;
			_maintenanceItemDetailRepository = maintenanceItemDetailRepository;
			_maintenanceDetailRepository = maintenanceDetailRepository;
			_maintenancePlanDetailRepository = maintenancePlanDetailRepository;
			_textResourceRepository = textResourceRepository;
			_inspectionRepository = inspectionRepository;
			_truckExpenseRepository = truckExpenseRepository;
			_employeeRepository = employeeRepository;
			_expenseRepository = expenseRepository;
			_supplierRepository = supplierRepository;
			_driverRepository = driverRepository;
			_unitOfWork = unitOfWork;
		}
		public void Add(List<InspectionPlanDetailViewModel> data, string objectI, string code)
		{
			// xoa het cac plan hien tai
			var existsItems = (_inspectionPlanDetailRepository.Query(x => x.ObjectI == objectI & x.Code == code)).ToList();
			foreach (var deleteItem in existsItems)
			{
				_inspectionPlanDetailRepository.Delete(deleteItem);
			}
			// add lai plan moi

			foreach (var item in data)
			{
				if (item.InspectionD != null) continue;

				var addItems = Mapper.Map<InspectionPlanDetailViewModel, InspectionPlan_D>(item);
				addItems.Code = code;
				addItems.ObjectI = objectI;
				_inspectionPlanDetailRepository.Add(addItems);

				//var checkItem = existsItems.Where(a => a.ObjectI == objectI && a.Code == code &&
				//									   a.InspectionC == item.InspectionC &&
				//									   a.InspectionPlanD == item.InspectionPlanD);

				//if (checkItem.Any()==false)
				//{
				//	item.Code = code;
				//	item.ObjectI = objectI;
				//	_inspectionPlanDetailRepository.Add(addItems);
				//}


			}

			_unitOfWork.Commit();
		}


		public IEnumerable<InspectionPlanDetailViewModel> Get(string objectI, string code)
		{
			var data = from a in _inspectionPlanDetailRepository.GetAllQueryable()
					   join b in _inspectionRepository.GetAllQueryable()
					   on new { a.InspectionC, a.ObjectI } equals new { b.InspectionC, b.ObjectI }
					   where (a.ObjectI == objectI && a.Code == code)
					   select new InspectionPlanDetailViewModel()
					   {
						   ObjectI = a.ObjectI,
						   Code = a.Code,
						   InspectionC = a.InspectionC,
						   InspectionN = b.InspectionN,
						   InspectionPlanD = a.InspectionPlanD
					   };
			var dataMaitenance = from a in _inspectionPlanDetailRepository.GetAllQueryable()
								 where (a.ObjectI == objectI && a.Code == code && a.InspectionC == 0)
								 select new InspectionPlanDetailViewModel()
								 {
									 ObjectI = a.ObjectI,
									 Code = a.Code,
									 InspectionC = a.InspectionC,
									 InspectionN = "",
									 InspectionPlanD = a.InspectionPlanD
								 };
			if (dataMaitenance.Any())
			{
				data = data.Concat(dataMaitenance);
			}

			if (data.Any())
			{
				return data.ToList().OrderBy(x => x.InspectionPlanD);
			}

			return null;
		}

		public void Delete(string objectI, string code)
		{
			var deleteItems = _inspectionPlanDetailRepository.Query(x => x.ObjectI == objectI && x.Code == code);
			foreach (var item in deleteItems)
			{
				_inspectionPlanDetailRepository.Delete(item);
			}

			SaveData();
		}

		public InspectionMaintenancePlanViewModel GetInspectionMaintenanceUpdateDetail(int inspectionC, string objectI, string code, DateTime implementD, int planI, int intCase, int maintenanceItemC)
		{
			var result = new InspectionMaintenancePlanViewModel();
			result.InspectionC = inspectionC;
			result.ObjectI = objectI;
			result.Code = code;
			result.ImplementD = implementD;
			result.PlanI = planI;

			// check exist InspectionC = 0
			if (intCase == 5 || intCase == 2)
			{
				var inspectionD = _inspectionDetailRepository.Query(i => i.ObjectI == objectI &&
																	 i.Code == code &&
																	 i.InspectionC == inspectionC &&
																	 i.InspectionD == implementD.Date
																	).FirstOrDefault();
				if (inspectionD != null)
				{
					intCase = 3;
				}
			}

			if (intCase == 1 || intCase == 2 || intCase == 5)
			{
				result.IntCase = intCase;
				if (objectI == "0" && code == "0")
				{
					// set inspectionN
					var textResouce = _textResourceRepository.Query(t => t.TextKey == "LBLMAINTENANCE").FirstOrDefault();
					if (textResouce != null)
					{
						result.InspectionN = textResouce.TextValue;
					}
					// set code N
					result.CodeN = "";
					// set remodelI
					result.RemodelI = "";
					// set modelN
					result.ModelN = "";
					result.InspectionPlanD = null;
				}
				else
				{
					// set inspectionN
					if (inspectionC == 0)
					{
						// set inspectionN
						var textResouce = _textResourceRepository.Query(t => t.TextKey == "LBLMAINTENANCE").FirstOrDefault();
						if (textResouce != null)
						{
							result.InspectionN = textResouce.TextValue;
						}
					}
					else
					{
						var inspection = _inspectionRepository.Query(i => i.InspectionC == inspectionC).FirstOrDefault();
						if (inspection != null)
						{
							result.InspectionN = inspection.InspectionN;
						}
					}

					// get Inspection_D
					if (intCase == 1)
					{
						var inspectionPlanD = _inspectionPlanDetailRepository.Query(i => i.ObjectI == objectI &&
																					 i.Code == code &&
																					 i.InspectionC == inspectionC
																		).FirstOrDefault();
						if (inspectionPlanD != null)
						{
							result.InspectionPlanD = inspectionPlanD.InspectionPlanD;
						}
					}

					#region get code name
					// get code name
					if (objectI == "0")
					{
						var truck = (from a in _truckRepository.GetAllQueryable()
									 join b in _modelRepository.GetAllQueryable()
											on a.ModelC equals b.ModelC into t1
									 from b in t1.DefaultIfEmpty()
									 where (a.TruckC == code)
									 select new TruckViewModel()
									 {
										 RegisteredNo = a.RegisteredNo,
										 RemodelI = a.RemodelI,
										 ModelC = a.ModelC,
										 ModelN = b != null ? b.ModelN : "",
										 Odometer = a.Odometer,
									 }).FirstOrDefault();
						if (truck != null)
						{
							result.CodeN = truck.RegisteredNo;
							result.RemodelI = truck.RemodelI;
							result.ModelC = truck.ModelC;
							result.ModelN = truck.ModelN;
							result.CurrentOdometer = truck.Odometer;
						}
					}
					else
					{
						var trailer = (from a in _trailerRepository.GetAllQueryable()
									   join b in _modelRepository.GetAllQueryable()
												on a.ModelC equals b.ModelC into t1
									   from b in t1.DefaultIfEmpty()
									   where (a.TrailerC == code)
									   select new TrailerViewModel()
									   {
										   TrailerNo = a.TrailerNo,
										   ModelC = a.ModelC,
										   ModelN = b != null ? b.ModelN : ""
									   }).FirstOrDefault();
						if (trailer != null)
						{
							result.CodeN = trailer.TrailerNo;
							result.ModelC = trailer.ModelC;
							result.ModelN = trailer.ModelN;
						}
					}
					#endregion

					#region get Inspection_D List
					// get Inspection_D List
					var inspectionDList = (from a in _inspectionDetailRepository.GetAllQueryable()
										   join b in _inspectionRepository.GetAllQueryable() on a.InspectionC equals b.InspectionC into t1
										   from b in t1.DefaultIfEmpty()
										   join e in _employeeRepository.GetAllQueryable() on a.EntryClerkC equals e.EmployeeC into t2
										   from e in t2.DefaultIfEmpty()
										   join ex in _expenseRepository.GetAllQueryable() on a.ExpenseC equals ex.ExpenseC into t3
										   from ex in t3.DefaultIfEmpty()
										   join s in _supplierRepository.GetAllQueryable() on new { a.SupplierMainC, a.SupplierSubC } equals new { s.SupplierMainC, s.SupplierSubC} into t4
										   from s in t4.DefaultIfEmpty()
										   where (a.ObjectI == objectI && a.Code == code)
										   select new InspectionDetailViewModel()
										   {
											   InspectionPlanD = a.InspectionPlanD,
											   InspectionD = a.InspectionD,
											   InspectionC = a.InspectionC,
											   InspectionN = b.InspectionN,
											   Description = a.Description,

											   EntryClerkC = a.EntryClerkC,
											   EntryClerkN = e.EmployeeLastN + " " + e.EmployeeFirstN,
											   ExpenseC = a.ExpenseC,
											   ExpenseN = ex.ExpenseN,
											   PaymentMethodI = a.PaymentMethodI,
											   SupplierMainC = a.SupplierMainC,
											   SupplierSubC = a.SupplierSubC,
											   SupplierN = s.SupplierN,
											   Total = a.Total,
											   TaxAmount = a.TaxAmount,
											   IsAllocated = a.IsAllocated,

											   IsDisabled = true,
											   IsHighLightRow = false
										   }).OrderBy("InspectionPlanD asc").ToList();
					if (inspectionDList.Count > 0)
					{
						result.InspectionDetailList.AddRange(inspectionDList);
					}

					if (intCase == 5)
					{
						// add new record
						var newRow = new InspectionDetailViewModel();
						newRow.InspectionD = implementD;
						newRow.InspectionN = result.InspectionN;
						newRow.IsDisabled = true;
						newRow.IsHighLightRow = true;
						result.InspectionDetailList.Add(newRow);
					}

					// get InspectionPlan_D List
					var inspectionPlanDList = (from a in _inspectionPlanDetailRepository.GetAllQueryable()
											   join b in _inspectionRepository.GetAllQueryable() on a.InspectionC equals b.InspectionC into t1
											   from b in t1.DefaultIfEmpty()
											   where (a.ObjectI == objectI && a.Code == code)
											   select new InspectionDetailViewModel()
											   {
												   InspectionPlanD = a.InspectionPlanD,
												   InspectionC = a.InspectionC,
												   InspectionN = b.InspectionN,
												   IsDisabled = false,
												   IsHighLightRow = false
												   //IsHighLightRow = (intCase == 1) && (a.InspectionC == inspectionC)
											   }).OrderBy("InspectionPlanD asc").ToList();
					if (inspectionPlanDList.Count > 0)
					{
						if (intCase == 1)
						{
							for (var iloop = 0; iloop < inspectionPlanDList.Count; iloop++)
							{
								if (inspectionPlanDList[iloop].InspectionC == inspectionC)
								{
									inspectionPlanDList[iloop].InspectionD = implementD;
									break;
								}
							}
						}

						result.InspectionDetailList.AddRange(inspectionPlanDList);
					}
					#endregion

					// get MaintenanceItem_D List
					var modelC = result.ModelC;
					var maintenanceMList = (from a in _maintenanceItemDetailRepository.GetAllQueryable()
											join b in _maintenanceItemRepository.GetAllQueryable() 
												on a.MaintenanceItemC equals b.MaintenanceItemC
											where a.ModelC == modelC && a.ObjectI == objectI
											select new MaintenanceDetailViewModel
											{
												MaintenanceItemC = b.MaintenanceItemC,
												MaintenanceItemN = b.MaintenanceItemN,
												ReplacementInterval = b.ReplacementInterval,
												NoticeI = b.NoticeI,
												RemarksI = "0",
												DisplayLineNo = b.DisplayLineNo,
												NoticeNo = b.NoticeNo,
												IsDisabled = true
											}
										).OrderBy("DisplayLineNo asc").ToList();

					for (var iloop = 0; iloop < maintenanceMList.Count; iloop++)
					{
						// set data
						// get and set PlanMaintenanceD and PlanMaintenanceKm
						var maintenanceItemCTemp = maintenanceMList[iloop].MaintenanceItemC;
						var maintenanceItemDetail = _maintenancePlanDetailRepository.Query(m => m.ObjectI == objectI &&
																								m.Code == code &&
																								m.MaintenanceItemC == maintenanceItemCTemp
																							).FirstOrDefault();

						if (maintenanceItemDetail != null)
						{
							maintenanceMList[iloop].PlanMaintenanceD = maintenanceItemDetail.PlanMaintenanceD;
							maintenanceMList[iloop].PlanMaintenanceKm = maintenanceItemDetail.PlanMaintenanceKm;
						}

						// get and set odometer and maintenanceD
						var nextPlanMaintenanceD = maintenanceMList[iloop].PlanMaintenanceD;
						var nextPlanMaintenanceKm = maintenanceMList[iloop].PlanMaintenanceKm;
						var maintenanceD = _maintenanceDetailRepository.Query(m => m.ObjectI == objectI &&
																				   m.Code == code &&
																				   m.MaintenanceItemC == maintenanceItemCTemp &&
																				   m.NextMaintenanceD == nextPlanMaintenanceD &&
																				   m.NextMaintenanceKm == nextPlanMaintenanceKm
																			);
						if (nextPlanMaintenanceD == null && nextPlanMaintenanceKm == null)
						{
							maintenanceMList[iloop].MaintenanceD = null;
							maintenanceMList[iloop].Odometer = null;
						}
						else if (nextPlanMaintenanceD != null)
						{
							var maintenanceDOrder = maintenanceD.OrderBy("MaintenanceD desc").FirstOrDefault();
							if (maintenanceDOrder != null)
							{
								maintenanceMList[iloop].MaintenanceD = maintenanceDOrder.MaintenanceD;
								maintenanceMList[iloop].Odometer = null;
							}
						}
						else
						{
							var maintenanceDOrder = maintenanceD.OrderBy("Odometer desc").FirstOrDefault();
							if (maintenanceDOrder != null)
							{
								maintenanceMList[iloop].MaintenanceD = null;
								maintenanceMList[iloop].Odometer = maintenanceDOrder.Odometer;
							}
						}

						result.MaintenanceDetailList.Add(maintenanceMList[iloop]);
					}
				}
			}
			// case 3 or 4
			else
			{
				result.IntCase = intCase; //3
				var inspectionDMax = _inspectionDetailRepository.Query(i => i.ObjectI == objectI &&
																		 i.Code == code &&
																		 i.InspectionC == inspectionC
																	).OrderBy("InspectionD desc").FirstOrDefault();
				if (inspectionDMax != null)
				{
					if (inspectionDMax.InspectionD.Date != implementD.Date)
					{
						result.IntCase = 4;
					}

					var inspectionD = _inspectionDetailRepository.Query(i => i.ObjectI == objectI &&
																			 i.Code == code &&
																			 i.InspectionC == inspectionC &&
																			 i.InspectionD == implementD
							).FirstOrDefault();

					if (inspectionD != null)
					{
						result.InspectionPlanD = inspectionD.InspectionPlanD;
						result.Odometer = inspectionD.Odometer;
						result.Description = inspectionD.Description;

						result.EntryClerkC = inspectionD.EntryClerkC;
						var driver = _driverRepository.Query(p => p.DriverC == inspectionD.EntryClerkC).FirstOrDefault();
						if (driver != null)
						{
							result.EntryClerkN = driver.LastN + " " + driver.FirstN;
						}
						//var employee = _employeeRepository.Query(p => p.EmployeeC == inspectionD.EntryClerkC).FirstOrDefault();
						//if (employee != null)
						//{
						//	result.EntryClerkN = employee.EmployeeLastN + " " + employee.EmployeeFirstN;
						//}

						result.ExpenseC = inspectionD.ExpenseC;
						var expense = _expenseRepository.Query(p => p.ExpenseC == inspectionD.ExpenseC).FirstOrDefault();
						if (expense != null)
						{
							result.ExpenseN = expense.ExpenseN;
						}

						result.SupplierMainC = inspectionD.SupplierMainC;
						result.SupplierSubC = inspectionD.SupplierSubC;
						var supplier =
							_supplierRepository.Query(
								p => p.SupplierMainC == inspectionD.SupplierMainC & p.SupplierSubC == inspectionD.SupplierSubC)
								.FirstOrDefault();
						if (supplier != null)
						{
							result.SupplierN = supplier.SupplierN;
						}

						result.PaymentMethodI = inspectionD.PaymentMethodI;
						result.Total = inspectionD.Total;
						result.TaxAmount = inspectionD.TaxAmount;
						result.IsAllocated = inspectionD.IsAllocated;
					}
				}
				else
				{
					//check MaintenanceDetail
					var maintenanceD = _maintenanceDetailRepository.Query(i => i.ObjectI == objectI &&
															i.Code == code &&
															i.MaintenanceItemC == maintenanceItemC
														).OrderBy("MaintenanceD desc").FirstOrDefault();
					if (maintenanceD != null )
					{
						//result.InspectionPlanD = maintenanceD.PlanMaintenanceD;
						result.Odometer = maintenanceD.Odometer;
						//result.Description = maintenanceD.Description;

						if (maintenanceD.MaintenanceD.Date != implementD.Date)
							result.IntCase = 4;
					}
				}

				// set inspectionN
				var inspection = _inspectionRepository.Query(i => i.InspectionC == inspectionC).FirstOrDefault();
				if (inspection != null)
				{
					result.InspectionN = inspection.InspectionN;
				}
				#region get code name
				// get code name
				if (objectI == "0")
				{
					var truck = (from a in _truckRepository.GetAllQueryable()
								 join b in _modelRepository.GetAllQueryable()
										on a.ModelC equals b.ModelC into t1
								 from b in t1.DefaultIfEmpty()
								 where (a.TruckC == code)
								 select new TruckViewModel()
								 {
									 RegisteredNo = a.RegisteredNo,
									 RemodelI = a.RemodelI,
									 ModelC = a.ModelC,
									 ModelN = b != null ? b.ModelN : "",
									 Odometer = a.Odometer,
								 }).FirstOrDefault();
					if (truck != null)
					{
						result.CodeN = truck.RegisteredNo;
						result.RemodelI = truck.RemodelI;
						result.ModelC = truck.ModelC;
						result.ModelN = truck.ModelN;
						result.CurrentOdometer = truck.Odometer;
					}
				}
				else
				{
					var trailer = (from a in _trailerRepository.GetAllQueryable()
								   join b in _modelRepository.GetAllQueryable()
											on a.ModelC equals b.ModelC into t1
								   from b in t1.DefaultIfEmpty()
								   where (a.TrailerC == code)
								   select new TrailerViewModel()
								   {
									   TrailerNo = a.TrailerNo,
									   ModelC = a.ModelC,
									   ModelN = b != null ? b.ModelN : ""
								   }).FirstOrDefault();
					if (trailer != null)
					{
						result.CodeN = trailer.TrailerNo;
						result.ModelC = trailer.ModelC;
						result.ModelN = trailer.ModelN;
					}
				}
				#endregion
				#region Inspection_D List
				// get Inspection_D List
				var inspectionDList = (from a in _inspectionDetailRepository.GetAllQueryable()
									   join b in _inspectionRepository.GetAllQueryable() on a.InspectionC equals b.InspectionC into t1
									   from b in t1.DefaultIfEmpty()
									   join e in _employeeRepository.GetAllQueryable() on a.EntryClerkC equals e.EmployeeC into t2
									   from e in t2.DefaultIfEmpty()
									   join ex in _expenseRepository.GetAllQueryable() on a.ExpenseC equals ex.ExpenseC into t3
									   from ex in t3.DefaultIfEmpty()
									   join s in _supplierRepository.GetAllQueryable() on new { a.SupplierMainC, a.SupplierSubC } equals new { s.SupplierMainC, s.SupplierSubC } into t4
									   from s in t4.DefaultIfEmpty()
									   where (a.ObjectI == objectI && a.Code == code)
									   select new InspectionDetailViewModel()
									   {
										   InspectionPlanD = a.InspectionPlanD,
										   InspectionD = a.InspectionD,
										   InspectionC = a.InspectionC,
										   InspectionN = b.InspectionN,
										   Description = a.Description,

										   EntryClerkC = a.EntryClerkC,
										   EntryClerkN = e.EmployeeLastN + " " + e.EmployeeFirstN,
										   ExpenseC = a.ExpenseC,
										   ExpenseN = ex.ExpenseN,
										   PaymentMethodI = a.PaymentMethodI,
										   SupplierMainC = a.SupplierMainC,
										   SupplierSubC = a.SupplierSubC,
										   SupplierN = s.SupplierN,
										   Total = a.Total,
										   TaxAmount = a.TaxAmount,
										   IsAllocated = a.IsAllocated,

										   IsDisabled = true,
										   IsHighLightRow = false
									   }).OrderBy("InspectionPlanD asc").ToList();
				if (inspectionDList.Count > 0)
				{
					result.InspectionDetailList.AddRange(inspectionDList);
				}

				// get InspectionPlan_D List
				var inspectionPlanDList = (from a in _inspectionPlanDetailRepository.GetAllQueryable()
										   join b in _inspectionRepository.GetAllQueryable() on a.InspectionC equals b.InspectionC into t1
										   from b in t1.DefaultIfEmpty()
										   where (a.ObjectI == objectI && a.Code == code)
										   select new InspectionDetailViewModel()
										   {
											   InspectionPlanD = a.InspectionPlanD,
											   InspectionC = a.InspectionC,
											   InspectionN = b.InspectionN,
											   IsDisabled = false,
											   IsHighLightRow = false
											   //IsHighLightRow = (intCase == 1) && (a.InspectionC == inspectionC)
										   }).OrderBy("InspectionPlanD asc").ToList();
				if (inspectionPlanDList.Count > 0)
				{
					for (var iloop = 0; iloop < inspectionPlanDList.Count; iloop++)
					{
						if (result.IntCase == 4)
						{
							inspectionPlanDList[iloop].IsDisabled = true;
						}
					}
					result.InspectionDetailList.AddRange(inspectionPlanDList);
				}
				#endregion
				// get MaintenanceItem_D List
				//var maintenanceItemMList = _maintenanceItemRepository.GetAllQueryable().OrderBy("DisplayLineNo asc").ToList();
				var modelC = result.ModelC;
				var maintenanceItemM = (from b in _maintenanceItemRepository.GetAllQueryable()
											join a in _maintenanceItemDetailRepository.GetAllQueryable()
												on b.MaintenanceItemC equals a.MaintenanceItemC 
											where a.ModelC == modelC && a.ObjectI == objectI
											select new MaintenanceDetailViewModel
											{
												MaintenanceItemC = b.MaintenanceItemC,
												MaintenanceItemN = b.MaintenanceItemN,
												ReplacementInterval = b.ReplacementInterval,
												NoticeI = b.NoticeI,
												RemarksI = "0",
												DisplayLineNo = b.DisplayLineNo,
												NoticeNo = b.NoticeNo,
												IsDisabled = true
											}
									);

				var maintenanceItemMList = maintenanceItemM.OrderBy("DisplayLineNo asc").ToList();
				for (var iloop = 0; iloop < maintenanceItemMList.Count; iloop++)
				{
					// set data
					var maintenanceItemCTemp = maintenanceItemMList[iloop].MaintenanceItemC;
					// get and set PlanMaintenanceD and PlanMaintenanceKm
					var maintenanceDetail = _maintenanceDetailRepository.Query(m => m.ObjectI == objectI &&
																					m.Code == code &&
																					m.MaintenanceItemC == maintenanceItemCTemp &&
																					m.MaintenanceD == implementD
																			).FirstOrDefault();
					if (maintenanceDetail != null && maintenanceDetail.InspectionC == inspectionC)
					{
						maintenanceItemMList[iloop].PlanMaintenanceD = maintenanceDetail.PlanMaintenanceD;
						maintenanceItemMList[iloop].PlanMaintenanceKm = maintenanceDetail.PlanMaintenanceKm;

						maintenanceItemMList[iloop].NextMaintenanceD = maintenanceDetail.NextMaintenanceD;
						maintenanceItemMList[iloop].NextMaintenanceKm = maintenanceDetail.NextMaintenanceKm;
						maintenanceItemMList[iloop].RemarksI = maintenanceDetail.RemarksI;
						maintenanceItemMList[iloop].MaintenanceD = maintenanceDetail.MaintenanceD;
						maintenanceItemMList[iloop].Odometer = maintenanceDetail.Odometer;
						maintenanceItemMList[iloop].PartNo = maintenanceDetail.PartNo;
						maintenanceItemMList[iloop].Quantity = maintenanceDetail.Quantity;
						maintenanceItemMList[iloop].Unit = maintenanceDetail.Unit;
						maintenanceItemMList[iloop].UnitPrice = maintenanceDetail.UnitPrice;
						maintenanceItemMList[iloop].Amount = maintenanceDetail.Amount;
						maintenanceItemMList[iloop].Description = maintenanceDetail.Description;
					}
					else
					{
						// lay du lieu tu MaintenancePlan
						var maintenancePlanD = _maintenancePlanDetailRepository.Query(m => m.ObjectI == objectI &&
																						   m.Code == code &&
																						   m.MaintenanceItemC == maintenanceItemCTemp
																						).FirstOrDefault();

						if (maintenancePlanD != null)
						{
							maintenanceItemMList[iloop].PlanMaintenanceD = maintenancePlanD.PlanMaintenanceD;
							maintenanceItemMList[iloop].PlanMaintenanceKm = maintenancePlanD.PlanMaintenanceKm;
						}
					}

					if (result.IntCase == 4)
					{
						maintenanceItemMList[iloop].IsDisabled = true;
					}

					result.MaintenanceDetailList.Add(maintenanceItemMList[iloop]);
				}
				// Get MaintenanceItem not in Model List
				var maintenanceDetailPlus = _maintenanceDetailRepository.Query(m => m.ObjectI == objectI &&
																					m.Code == code &&
																					!maintenanceItemM.Any(s => s.MaintenanceItemC == m.MaintenanceItemC) &&
																					m.MaintenanceD == implementD
																			).ToList();
				for (var jloop = 0; jloop < maintenanceDetailPlus.Count; jloop++)
				{
					var maintenanceDetailPlusItem = Mapper.Map<Maintenance_D,MaintenanceDetailViewModel>(maintenanceDetailPlus[jloop]);
					var maintenanceItem = _maintenanceItemRepository.Query(m => m.MaintenanceItemC == maintenanceDetailPlusItem.MaintenanceItemC).FirstOrDefault();
					if (maintenanceItemM != null)
					{
						maintenanceDetailPlusItem.MaintenanceItemN = maintenanceItem.MaintenanceItemN;
						maintenanceDetailPlusItem.ReplacementInterval = maintenanceItem.ReplacementInterval;
						maintenanceDetailPlusItem.ReplacementInterval = maintenanceItem.ReplacementInterval;
						maintenanceDetailPlusItem.NoticeI = maintenanceItem.NoticeI;
						maintenanceDetailPlusItem.DisplayLineNo = result.MaintenanceDetailList.Count + 1;
						maintenanceDetailPlusItem.NoticeNo = maintenanceItem.NoticeNo;
						maintenanceDetailPlusItem.IsDisabled = false;

						if (result.IntCase == 4)
						{
							maintenanceDetailPlusItem.IsDisabled = true;
						}
						result.MaintenanceDetailList.Add(maintenanceDetailPlusItem);
					}
				}
			}

			return result;
		}

		public void UpdateInspectionMaintenanceUpdateDetail(InspectionMaintenancePlanViewModel data)
		{
			var objectI = data.ObjectI;
			var code = data.Code;
			var inspectionC = data.InspectionC;
			var implementD = data.ImplementD;
			var intCase = data.IntCase;
			var odometer = data.Odometer;
			if (intCase == 1 || intCase == 2 || intCase == 5)
			{
				// update Truck_M
				if (objectI == "0" && data.Odometer != null)
				{
					var truck = _truckRepository.Query(t => t.TruckC == code).FirstOrDefault();
					if (truck != null)
					{
						if (truck.Odometer == null || ((decimal)truck.Odometer < (decimal)data.Odometer))
						{
							truck.Odometer = odometer;
						}

						_truckRepository.Update(truck);
					}
				}

				// create Inspection_D
				if (intCase == 1 || intCase == 5)
				{
					var inspectionD = new Inspection_D();
					inspectionD.ObjectI = objectI;
					inspectionD.Code = code;
					inspectionD.InspectionD = data.ImplementD;
					inspectionD.InspectionC = inspectionC;
					inspectionD.Description = data.Description;
					inspectionD.Odometer = odometer;
					inspectionD.ExpenseC = data.ExpenseC;
					inspectionD.EntryClerkC = data.EntryClerkC;
					inspectionD.PaymentMethodI = data.PaymentMethodI;
					inspectionD.SupplierMainC = data.SupplierMainC;
					inspectionD.SupplierSubC = data.SupplierSubC;
					inspectionD.Total = data.Total;
					inspectionD.TaxAmount = data.TaxAmount;
					inspectionD.IsAllocated = data.IsAllocated;
					if (intCase == 1)
					{
						inspectionD.InspectionPlanD = data.InspectionPlanD;
					}
					_inspectionDetailRepository.Add(inspectionD);
				}
				else
				{
					//2015.05.20 Lan: truong hop 2 khi luu du lieu thi dong thoi them moi dong InspectionC = 0
					var inspectionD = _inspectionDetailRepository.Query(i => i.ObjectI == objectI &&
																		 i.Code == code &&
																		 i.InspectionC == inspectionC &&
																		 i.InspectionD == implementD.Date
																	).FirstOrDefault();
					if (inspectionD == null)
					{
						var insertInspectionD = new Inspection_D
						{
							ObjectI = objectI,
							Code = code,
							InspectionD = implementD,
							InspectionC = inspectionC,
							Description = data.Description,
							Odometer = odometer,
							ExpenseC = data.ExpenseC,
							EntryClerkC = data.EntryClerkC,
							PaymentMethodI = data.PaymentMethodI,
							SupplierMainC = data.SupplierMainC,
							SupplierSubC = data.SupplierSubC,
							Total = data.Total,
							TaxAmount = data.TaxAmount,
							IsAllocated = data.IsAllocated,
						};
						_inspectionDetailRepository.Add(insertInspectionD);
					}
				}

				// delete InspectionPlanD
				_inspectionPlanDetailRepository.Delete(t => t.ObjectI == objectI && t.Code == code);

				// create inspection data
				for (var iloop = 0; iloop < data.InspectionDetailList.Count; iloop++)
				{
					if (!data.InspectionDetailList[iloop].IsHighLightRow)
					{
						var inspectionPlanD = new InspectionPlan_D();
						if (data.InspectionDetailList[iloop].InspectionPlanD != null)
						{
							inspectionPlanD.InspectionPlanD = (DateTime)data.InspectionDetailList[iloop].InspectionPlanD;
						}
						inspectionPlanD.InspectionC = data.InspectionDetailList[iloop].InspectionC;
						inspectionPlanD.ObjectI = objectI;
						inspectionPlanD.Code = code;
						_inspectionPlanDetailRepository.Add(inspectionPlanD);
					}
				}

				// update MaintenancePlan_D
				for (var iloop = 0; iloop < data.MaintenanceDetailList.Count; iloop++)
				{
					var maintenanceItemC = data.MaintenanceDetailList[iloop].MaintenanceItemC;
					var maintenancePlanD =
						_maintenancePlanDetailRepository.Query(
							m => m.ObjectI == objectI && m.Code == code && m.MaintenanceItemC == maintenanceItemC).FirstOrDefault();
					if (maintenancePlanD != null)
					{
						if (data.MaintenanceDetailList[iloop].NoticeI == "1")
						{
							maintenancePlanD.PlanMaintenanceD = data.MaintenanceDetailList[iloop].NextMaintenanceD;
						}
						else
						{
							maintenancePlanD.PlanMaintenanceKm = data.MaintenanceDetailList[iloop].NextMaintenanceKm;
						}
						_maintenancePlanDetailRepository.Update(maintenancePlanD);
					}
					else
					{
						// inset new row
						var maintenancePlanDInsert = new MaintenancePlan_D();
						maintenancePlanDInsert.ObjectI = objectI;
						maintenancePlanDInsert.Code = code;
						maintenancePlanDInsert.MaintenanceItemC = maintenanceItemC;
						if (data.MaintenanceDetailList[iloop].NoticeI == "1")
						{
							maintenancePlanDInsert.PlanMaintenanceD = data.MaintenanceDetailList[iloop].NextMaintenanceD;
						}
						else
						{
							maintenancePlanDInsert.PlanMaintenanceKm = data.MaintenanceDetailList[iloop].NextMaintenanceKm;
						}

						_maintenancePlanDetailRepository.Add(maintenancePlanDInsert);
					}

					// insert Maintenance_D
					var maintenanceD = new Maintenance_D();
					maintenanceD.ObjectI = objectI;
					maintenanceD.Code = code;
					maintenanceD.InspectionC = data.InspectionC;
					maintenanceD.MaintenanceD = implementD;
					maintenanceD.Odometer = odometer;
					maintenanceD.MaintenanceItemC = data.MaintenanceDetailList[iloop].MaintenanceItemC;
					maintenanceD.PlanMaintenanceD = data.MaintenanceDetailList[iloop].PlanMaintenanceD;
					maintenanceD.PlanMaintenanceKm = data.MaintenanceDetailList[iloop].PlanMaintenanceKm;
					maintenanceD.RemarksI = data.MaintenanceDetailList[iloop].RemarksI;
					maintenanceD.NextMaintenanceD = data.MaintenanceDetailList[iloop].NextMaintenanceD;
					maintenanceD.NextMaintenanceKm = data.MaintenanceDetailList[iloop].NextMaintenanceKm;
					maintenanceD.PartNo = data.MaintenanceDetailList[iloop].PartNo;
					maintenanceD.Quantity = data.MaintenanceDetailList[iloop].Quantity;
					maintenanceD.Unit = data.MaintenanceDetailList[iloop].Unit;
					maintenanceD.UnitPrice = data.MaintenanceDetailList[iloop].UnitPrice;
					maintenanceD.Amount = data.MaintenanceDetailList[iloop].Amount;
					maintenanceD.Description = data.MaintenanceDetailList[iloop].Description;
					_maintenanceDetailRepository.Add(maintenanceD);
				}
			}
			else if (intCase == 3)
			{
				// update Truck_M
				if (objectI == "0" && odometer != null)
				{
					var truck = _truckRepository.Query(t => t.TruckC == code).FirstOrDefault();
					if (truck != null)
					{
						if (truck.Odometer == null || ((decimal)truck.Odometer < (decimal)odometer))
						{
							truck.Odometer = odometer;
						}

						_truckRepository.Update(truck);
					}
				}

				// update InspectionD
				//var implementD = data.ImplementD;
				//var inspectionCTemp = data.InspectionC;
				var inspectionD = _inspectionDetailRepository.Query(i => i.ObjectI == objectI &&
																		 i.Code == code &&
																		 i.InspectionC == inspectionC &&
																		 i.InspectionD == implementD.Date
																	).FirstOrDefault();
				if (inspectionD != null)
				{
					inspectionD.Odometer = odometer;
					inspectionD.Description = data.Description;
					inspectionD.ExpenseC = data.ExpenseC;
					inspectionD.EntryClerkC = data.EntryClerkC;
					inspectionD.PaymentMethodI = data.PaymentMethodI;
					inspectionD.SupplierMainC = data.SupplierMainC;
					inspectionD.SupplierSubC = data.SupplierSubC;
					inspectionD.Total = data.Total;
					inspectionD.TaxAmount = data.TaxAmount;
					inspectionD.IsAllocated = data.IsAllocated;
					_inspectionDetailRepository.Update(inspectionD);
				}

				// delete InspectionPlanD
				_inspectionPlanDetailRepository.Delete(t => t.ObjectI == objectI && t.Code == code);

				// create inspection data
				for (var iloop = 0; iloop < data.InspectionDetailList.Count; iloop++)
				{
					var inspectionPlanD = new InspectionPlan_D();
					if (data.InspectionDetailList[iloop].InspectionPlanD != null)
					{
						inspectionPlanD.InspectionPlanD = (DateTime)data.InspectionDetailList[iloop].InspectionPlanD;
					}
					inspectionPlanD.InspectionC = data.InspectionDetailList[iloop].InspectionC;
					inspectionPlanD.ObjectI = objectI;
					inspectionPlanD.Code = code;
					_inspectionPlanDetailRepository.Add(inspectionPlanD);
				}

				// update MaintenancePlan_D
				// delete Maintenance_D
				var maintenanceDate = data.ImplementD;
				_maintenanceDetailRepository.Delete(m => m.ObjectI == objectI &&
														 m.Code == code &&
														 m.MaintenanceD == maintenanceDate &&
														 m.InspectionC == inspectionC
													);

				for (var iloop = 0; iloop < data.MaintenanceDetailList.Count; iloop++)
				{
					var maintenanceItemC = data.MaintenanceDetailList[iloop].MaintenanceItemC;
					var maintenancePlanDetail =
						_maintenancePlanDetailRepository.Query(
							m => m.ObjectI == objectI && m.Code == code && m.MaintenanceItemC == maintenanceItemC).FirstOrDefault();
					if (maintenancePlanDetail != null)
					{
						if (data.MaintenanceDetailList[iloop].NoticeI == "1")
						{
							maintenancePlanDetail.PlanMaintenanceD = data.MaintenanceDetailList[iloop].NextMaintenanceD;
						}
						else
						{
							maintenancePlanDetail.PlanMaintenanceKm = data.MaintenanceDetailList[iloop].NextMaintenanceKm;
						}
						_maintenancePlanDetailRepository.Update(maintenancePlanDetail);
					}
					else
					{
						// insert new row
						var maintenancePlanDInsert = new MaintenancePlan_D();
						maintenancePlanDInsert.ObjectI = objectI;
						maintenancePlanDInsert.Code = code;
						maintenancePlanDInsert.MaintenanceItemC = maintenanceItemC;
						if (data.MaintenanceDetailList[iloop].NoticeI == "1")
						{
							maintenancePlanDInsert.PlanMaintenanceD = data.MaintenanceDetailList[iloop].NextMaintenanceD;
						}
						else
						{
							maintenancePlanDInsert.PlanMaintenanceKm = data.MaintenanceDetailList[iloop].NextMaintenanceKm;
						}

						_maintenancePlanDetailRepository.Add(maintenancePlanDInsert);
					}

					// insert Maintenance_D
					var maintenanceD = new Maintenance_D();
					maintenanceD.ObjectI = data.ObjectI;
					maintenanceD.Code = data.Code;
					maintenanceD.InspectionC = data.InspectionC;
					maintenanceD.MaintenanceD = data.ImplementD;
					maintenanceD.Odometer = data.Odometer;
					maintenanceD.MaintenanceItemC = data.MaintenanceDetailList[iloop].MaintenanceItemC;
					maintenanceD.PlanMaintenanceD = data.MaintenanceDetailList[iloop].PlanMaintenanceD;
					maintenanceD.PlanMaintenanceKm = data.MaintenanceDetailList[iloop].PlanMaintenanceKm;
					maintenanceD.RemarksI = data.MaintenanceDetailList[iloop].RemarksI;
					maintenanceD.NextMaintenanceD = data.MaintenanceDetailList[iloop].NextMaintenanceD;
					maintenanceD.NextMaintenanceKm = data.MaintenanceDetailList[iloop].NextMaintenanceKm;
					maintenanceD.PartNo = data.MaintenanceDetailList[iloop].PartNo;
					maintenanceD.Quantity = data.MaintenanceDetailList[iloop].Quantity;
					maintenanceD.Unit = data.MaintenanceDetailList[iloop].Unit;
					maintenanceD.UnitPrice = data.MaintenanceDetailList[iloop].UnitPrice;
					maintenanceD.Amount = data.MaintenanceDetailList[iloop].Amount;
					maintenanceD.Description = data.MaintenanceDetailList[iloop].Description;
					_maintenanceDetailRepository.Add(maintenanceD);
				}
			}

			SaveData();
		}

		public void DeleteInspectionMaintenanceUpdateDetail(int inspectionC, string objectI, string code, DateTime implementD)
		{
			//delete Inspection
			var inspectionD = _inspectionDetailRepository.Query(i => i.InspectionC == inspectionC &&
																	 i.ObjectI == objectI &&
																	 i.Code == code &&
																	 i.InspectionD == implementD.Date
																).FirstOrDefault();

			if (inspectionD != null)
			{
				// update InspectionPlanD
				if (inspectionD.InspectionPlanD != null)
				{
					var inspectionPlan = _inspectionPlanDetailRepository.Query(i => i.ObjectI == objectI &&
																				i.Code == code &&
																				i.InspectionC == inspectionC
																			).FirstOrDefault();
					if (inspectionPlan != null)
					{
						//inspectionPlan.InspectionPlanD = (DateTime)inspectionD.InspectionPlanD;
					}
					else
					{
						var inspection = new InspectionPlan_D()
						{
							Code = code,
							ObjectI = objectI,
							InspectionC = inspectionC,
							InspectionPlanD = (DateTime)inspectionD.InspectionPlanD
						};
						_inspectionPlanDetailRepository.Add(inspection);
					}

				}


				// delete InspectionD
				_inspectionDetailRepository.Delete(i => i.InspectionC == inspectionC &&
																		 i.ObjectI == objectI &&
																		 i.Code == code &&
																		 i.InspectionD == implementD.Date);
			}

			// Lan 13.04.2015
			//delete MaintenanceDetail
			var maintenanceListD = _maintenanceDetailRepository.Query(i => i.ObjectI == objectI &&
																		   i.Code == code &&
																		   i.MaintenanceD == implementD.Date &&
																		   i.InspectionC == inspectionC
																).ToList();
			if (maintenanceListD.Count > 0 )
			{
				for (int iloop = 0; iloop < maintenanceListD.Count; iloop++)
				{
					// update MaintenancePlanD
					var maintenanceItemC = maintenanceListD[iloop].MaintenanceItemC;
					if (maintenanceListD[iloop].PlanMaintenanceD != null || maintenanceListD[iloop].PlanMaintenanceKm != null)
					{
						var maintenancePlan = _maintenancePlanDetailRepository.Query(i => i.ObjectI == objectI &&
																						  i.Code == code &&
																						  i.MaintenanceItemC == maintenanceItemC
							).FirstOrDefault();
						if (maintenancePlan != null)
						{
							maintenancePlan.PlanMaintenanceD = maintenanceListD[iloop].PlanMaintenanceD;
							maintenancePlan.PlanMaintenanceKm = maintenanceListD[iloop].PlanMaintenanceKm;
							_maintenancePlanDetailRepository.Update(maintenancePlan);
						}
					}
					else
					{
						//delete MaintenancePlan
						_maintenancePlanDetailRepository.Delete(i => i.ObjectI == objectI &&
																	 i.Code == code &&
																	 i.MaintenanceItemC == maintenanceItemC);
					}
				}
				// delete MaintenanceD
				_maintenanceDetailRepository.Delete(i => i.ObjectI == objectI &&
														 i.Code == code &&
														 i.MaintenanceD == implementD.Date &&
														 i.InspectionC == inspectionC);
			}

			SaveData();
		}

		public void SaveData()
		{
			_unitOfWork.Commit();
		}
	}
}