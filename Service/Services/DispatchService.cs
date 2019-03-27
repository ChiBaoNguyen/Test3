using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.Utilities;
using Website.ViewModels.Customer;
using Website.ViewModels.Department;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Container;
using Website.ViewModels.Order;
using System.IO;
using System.Data;
using System.Globalization;
using Website.ViewModels.Mobile.Dispatch;

namespace Service.Services
{
	public interface IDispatchService
	{
		DispatchDatatable GetDispatchDatatable(string departmentC, DateTime? fromDate, DateTime? toDate, bool dispatchStatus0, bool dispatchStatus1, bool dispatchStatus2, bool dispatchStatus3,
			string orderTypeI, string bkbl, string trailerNo, string containerNo, string empC, string sTruckC,
			string searchI, string customerMainC, string customerSubC, string sDriverC, string jobNo,
            string shippingCompanyC, string sealNo, string locationC, int page, int itemsPerPage, bool sortBy);
		DispatchDetailViewModel GetDispatchDetail(DateTime orderD, string orderNo, int detailNo, int dispatchNo);
		void UpdateDispatchInfo(DispatchDetailViewModel dispatchDetail);
		void UpdateContainerNo(DateTime orderD, string orderNo, int detailNo, string containerNo);
		void UpdateTrailerC(DateTime orderD, string orderNo, int detailNo, string trailerNo);
		void UpdateSealNo(DateTime orderD, string orderNo, int detailNo, string sealNo);
		void UpdateDescription(DateTime orderD, string orderNo, int detailNo, string descriptionParam);
		void UpdateTrailerCWarning(DateTime orderD, string orderNo, int detailNo, string trailerNo);
		int CheckTrailerIsUsing(DateTime orderD, string orderNo, int detailNo, string trailerNo);
		void UpdateActualLoadingD(DateTime orderD, string orderNo, int detailNo, DateTime? actualLoadingD);
		void UpdateActualDischargeD(DateTime orderD, string orderNo, int detailNo, DateTime? actualDischargeD);
		void UpdateActualLoadingDischargeD(DateTime orderD, string orderNo, int detailNo, DateTime? actualLoadingD, DateTime? actualDischargeD);
		void UpdateDispatchStatus(DateTime orderD, string orderNo, int detailNo, int dispatchNo, string status);
		DriverDispatchViewModel GetDriverDispatchDatatable(DateTime transportD, string truckC, string driverC);
		int DeleteDispatchDetail(DateTime orderD, string orderNo, int detailNo, int dispatchNo, bool isConfirmedDeleting);
		TruckListViewModel GetTruckList(DateTime transportD, string depC, string searchtruckC, bool notdispatchgoStatus, bool notdispatchbackStatus, bool dispatchgoStatus, bool dispatchbackStatus, bool otherStatus);
		TrailerListViewModel GetTrailerList(DateTime transportD, string depC, string searchtrailerC, bool notdispatchgoStatus, bool notdispatchbackStatus, bool dispatchgoStatus, bool dispatchbackStatus, bool otherStatus);

		//IEnumerable<OrderViewModel> GetAutoSuggestLocation(string value, DateTime orderDParam, string orderNoParam);
		List<TransportConfirmDispatchViewModel> GetTransConfirmDispatchList(DateTime orderD, string orderNo, int detailNo);
		int GetMaxDispatchOrder(DateTime? transportD, string driverC, string truckC);
		int CheckDispatchDeleting(DateTime orderD, string orderNo, int detailNo, int dispatchNo);
        bool SaveOrderNoDouble(int? orderNoDouble, DateTime? orderD1, string orderNo1, int? detailNo1, DateTime? orderD2, string orderNo2, int? detailNo2);
        int? GetOrderNoDoubleMax();
        bool GetStatusCont(int? detailNo, DateTime? orderD, string orderNo);

		/// <summary>
		/// Mobile Service
		/// </summary>
		MobileDispatchList MGetDispatchList(DateTime date, string driverC);
		bool MUpdateDispatch(MobileDispatchViewModel dispatch);
		void SaveDispatch();
	}

	public class DispatchService : IDispatchService
	{
		private readonly IDispatchRepository _dispatchRepository;
		private readonly IContainerRepository _orderDRepository;
		private readonly IOrderRepository _orderHRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IDepartmentRepository _departmentRepository;
		private readonly IContainerTypeRepository _containerTypeRepository;
		private readonly ITrailerRepository _trailerRepository;
		private readonly ITruckRepository _truckRepository;
		private readonly IDriverRepository _driverRepository;
		private readonly IPartnerRepository _partnerRepository;
		private readonly ITextResourceRepository _textResourceRepository;
		private readonly IExpenseDetailRepository _expenseDetailRepository;
		private readonly ISurchargeDetailRepository _surchargeDetailRepository;
		private readonly IAllowanceDetailRepository _allowanceDetailRepository;
		private readonly IShippingCompanyRepository _shippingCompanyRepository;
		private readonly ICustomerSettlementRepository _customerSettlementRepository;
		private readonly IPartnerSettlementRepository _partnerSettlementRepository;
		private readonly IBasicRepository _basicRepository;
		private readonly IInspectionPlanDetailRepository _inspectionPlanDetailRepository;
		private readonly IFuelConsumptionDetailRepository _fuelConsumptionDetailRepository;
		private readonly IOperationRepository _operationRepository;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly IUnitOfWork _unitOfWork;
        private readonly ILocationRepository _locationRepository;
		public DispatchService(IDispatchRepository dispatchRepository,
								IContainerRepository orderDRepository,
								IOrderRepository orderHRepository,
								ICustomerRepository customerRepository,
								IDepartmentRepository departmentRepository,
								IContainerTypeRepository containerTypeRepository,
								ITrailerRepository trailerRepository,
								ITruckRepository truckRepository,
								IDriverRepository driverRepository,
								IPartnerRepository partnerRepository,
								IShippingCompanyRepository shippingCompanyRepository,
								ITextResourceRepository textResourceRepository,
								IExpenseDetailRepository expenseDetailRepository,
								ISurchargeDetailRepository surchargeDetailRepository,
								IAllowanceDetailRepository allowanceDetailRepository,
								ICustomerSettlementRepository customerSettlementRepository,
								IPartnerSettlementRepository partnerSettlementRepository,
								IBasicRepository basicRepository,
								IInspectionPlanDetailRepository inspectionPlanDetailRepository,
								IFuelConsumptionDetailRepository fuelConsumptionDetailRepository,
								IOperationRepository operationRepository,
								IEmployeeRepository employeeRepository,
                                IUnitOfWork unitOfWork, ILocationRepository locationRepository)
		{
			this._dispatchRepository = dispatchRepository;
			this._orderDRepository = orderDRepository;
			this._orderHRepository = orderHRepository;
			this._customerRepository = customerRepository;
			this._departmentRepository = departmentRepository;
			this._containerTypeRepository = containerTypeRepository;
			this._trailerRepository = trailerRepository;
			this._truckRepository = truckRepository;
			this._driverRepository = driverRepository;
			this._partnerRepository = partnerRepository;
			this._shippingCompanyRepository = shippingCompanyRepository;
			this._textResourceRepository = textResourceRepository;
			this._expenseDetailRepository = expenseDetailRepository;
			this._surchargeDetailRepository = surchargeDetailRepository;
			this._allowanceDetailRepository = allowanceDetailRepository;
			this._customerSettlementRepository = customerSettlementRepository;
			this._partnerSettlementRepository = partnerSettlementRepository;
			this._basicRepository = basicRepository;
			this._inspectionPlanDetailRepository = inspectionPlanDetailRepository;
			this._fuelConsumptionDetailRepository = fuelConsumptionDetailRepository;
			this._operationRepository = operationRepository;
			this._employeeRepository = employeeRepository;
			this._unitOfWork = unitOfWork;
            this._locationRepository = locationRepository;
		}

		#region IDispatchService members
		public DispatchDatatable GetDispatchDatatable(string departmentC, DateTime? fromDate, DateTime? toDate, bool dispatchStatus0, bool dispatchStatus1, bool dispatchStatus2, bool dispatchStatus3,
														string orderTypeI, string bkbl, string trailerNo, string containerNo, string empC, string sTruckC,
														string searchI, string customerMainC, string customerSubC, string sDriverC, string jobNo,
                                                        string shippingCompanyC, string sealNo, string locationC, int page, int itemsPerPage, bool sortBy)
		{
			DateTime? nextDate = toDate;
			if (toDate != null)
			{
				DateTime tmp = ((DateTime)toDate).AddDays(1);
				nextDate = tmp;
			}
			var orderD = (from p in _orderDRepository.GetAllQueryable()
						  join q in _orderHRepository.GetAllQueryable() on new { p.OrderD, p.OrderNo } equals new { q.OrderD, q.OrderNo } into pq
						  from q in pq.DefaultIfEmpty()
                          join d in _dispatchRepository.GetAllQueryable() on new { p.OrderD, p.OrderNo, p.DetailNo } equals
							  new { d.OrderD, d.OrderNo, d.DetailNo } into dpq
						  from d in dpq.DefaultIfEmpty()
						  join r in _trailerRepository.GetAllQueryable() on p.TrailerC equals r.TrailerC into dr
						  from r in dr.DefaultIfEmpty()
						  where (toDate == null || (searchI == "O" & p.OrderD < nextDate && (!dispatchStatus3 || ((d.Operation1C != "TR" && d.Operation2C != "TR" && d.Operation3C != "TR") & (d.DispatchStatus == Constants.TRANSPORTED || d.DispatchStatus == Constants.CONFIRMED))))
                                                || (searchI == "D" & d != null && d.TransportD < nextDate)
                                                || (searchI == "C" & (q.LoadingDT < nextDate || q.StopoverDT < nextDate || q.DischargeDT < nextDate))
                                                || (searchI == "E" & q.ETD < nextDate)
                                                || (searchI == "L" & q.ClosingDT < nextDate)
                                                || (searchI == "T" & q.TermContReturnDT < nextDate && (!dispatchStatus3 || ((d.Operation1C != "TR" && d.Operation2C != "TR" && d.Operation3C != "TR")& (d.DispatchStatus == Constants.TRANSPORTED || d.DispatchStatus == Constants.CONFIRMED))))
                                                || (searchI == "A" & (p.ActualLoadingD < nextDate || p.ActualDischargeD < nextDate || p.ActualPickupReturnD < nextDate) && (!dispatchStatus3 || ((d.Operation1C != "TR" && d.Operation2C != "TR" && d.Operation3C != "TR") & (d.DispatchStatus == Constants.TRANSPORTED || d.DispatchStatus == Constants.CONFIRMED))))) &
                                (fromDate == null || (searchI == "O" & p.OrderD >= fromDate && (!dispatchStatus3 || ((d.Operation1C != "TR" && d.Operation2C != "TR" && d.Operation3C != "TR")& (d.DispatchStatus == Constants.TRANSPORTED || d.DispatchStatus == Constants.CONFIRMED))))
                                                  || (searchI == "D" & d != null && d.TransportD >= fromDate)
                                                  || (searchI == "C" & (q.LoadingDT >= fromDate || q.StopoverDT >= fromDate || q.DischargeDT >= fromDate))
                                                  || (searchI == "E" & q.ETD >= fromDate)
                                                  || (searchI == "L" & q.ClosingDT >= fromDate)
                                                  || (searchI == "T" & q.TermContReturnDT >= fromDate && (!dispatchStatus3 || ((d.Operation1C != "TR" && d.Operation2C != "TR" && d.Operation3C != "TR")& (d.DispatchStatus == Constants.TRANSPORTED || d.DispatchStatus == Constants.CONFIRMED))))
                                                  || (searchI == "A" & (p.ActualLoadingD >= fromDate || p.ActualDischargeD >= fromDate || p.ActualPickupReturnD >= fromDate) && (!dispatchStatus3 || ((d.Operation1C != "TR" && d.Operation2C != "TR" && d.Operation3C != "TR")& (d.DispatchStatus == Constants.TRANSPORTED || d.DispatchStatus == Constants.CONFIRMED))))) &
                                (containerNo == "-1" || p.ContainerNo.Contains(containerNo)) &
                                (string.IsNullOrEmpty(empC) || q.EntryClerkC == empC) &
                                (departmentC == "0" || q.OrderDepC == departmentC) &
                                (bkbl == "-1" || q.BLBK == bkbl) & (trailerNo == "-1" || r.TrailerNo.Contains(trailerNo)) &
                                (orderTypeI == "-1" || q.OrderTypeI == orderTypeI) &
                                ((dispatchStatus0 && dispatchStatus1 && dispatchStatus2 && dispatchStatus3) ||
                                (!dispatchStatus0 && !dispatchStatus1 && !dispatchStatus2 && !dispatchStatus3 && d == null) ||
                                (dispatchStatus0 && (d == null || d.DispatchStatus == Constants.NOTDISPATCH)) ||
                                (dispatchStatus1 && d.DispatchStatus == Constants.DISPATCH) ||
                                (dispatchStatus2 && (d.DispatchStatus == Constants.TRANSPORTED || d.DispatchStatus == Constants.CONFIRMED)) )&
                                (sTruckC == "-1" || d.TruckC == sTruckC) &
                                (string.IsNullOrEmpty(customerMainC) || string.IsNullOrEmpty(customerSubC)
                                    || (q.CustomerMainC.Equals(customerMainC) && q.CustomerSubC.Equals(customerSubC))) &
                                (string.IsNullOrEmpty(sDriverC) || (d != null && d.DriverC.Equals(sDriverC))) &
                                (string.IsNullOrEmpty(jobNo) || q.JobNo.Contains(jobNo)) &
                                (string.IsNullOrEmpty(shippingCompanyC) || q.ShippingCompanyC.Equals(shippingCompanyC)) &
                                (string.IsNullOrEmpty(sealNo) || p.SealNo.Contains(sealNo)) &
                                (string.IsNullOrEmpty(locationC) || (q.LoadingPlaceC.Equals(locationC) || q.StopoverPlaceC.Equals(locationC) || q.DischargePlaceC.Equals(locationC)
                                 || (d != null && d.Location1C.Equals(locationC)) || (d != null && d.Location2C.Equals(locationC)) || (d != null && d.Location3C.Equals(locationC))))
						group p by new
						{
							p.OrderD,
							p.OrderNo,
							p.DetailNo
						}
							into gp
							join w in _orderDRepository.GetAllQueryable() on new { gp.Key.OrderD, gp.Key.OrderNo, gp.Key.DetailNo }
								equals new { w.OrderD, w.OrderNo, w.DetailNo } into wgp
							from w in wgp.DefaultIfEmpty()
							select w);

			var orderDCount = orderD.Count();

			var orderDOrdered = orderD.OrderBy("OrderD desc, OrderNo asc, DetailNo asc");
            if (sortBy)
            {
                orderDOrdered = orderD.OrderBy("EnableDouble desc, OrderNoDouble desc");
            }
			// paging
			var orderDList = orderDOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var dispatchDatatable = new DispatchDatatable();
			foreach (var item in orderDList)
			{
				// set init dispatchDataRow
				var dispatchDataRow = new DispatchDataRow {OrderD = Mapper.Map<Order_D, ContainerViewModel>(item)};

				// get trailerN
				dispatchDataRow.OrderD.TrailerNo = "";
				if (item.TrailerC != null)
				{
					var trailer = _trailerRepository.Query(tra => tra.TrailerC == item.TrailerC).FirstOrDefault();
					if (trailer != null)
					{
						dispatchDataRow.OrderD.TrailerNo = trailer.TrailerNo;
					}
				}
			    var dispatchListFindImage = _dispatchRepository.GetAllQueryable()
			        .Where(od => od.OrderD.Equals(item.OrderD))
			        .Where(on => on.OrderNo.Equals(item.OrderNo))
			        .Where(dn => dn.DetailNo.Equals(item.DetailNo)).ToList();
			    dispatchDataRow.OrderD.ImageCount = dispatchListFindImage.Sum(d => d.ImageCount);
			    string orderKeyImageArray = "";
			    if (dispatchListFindImage.Count == 1)
			    {
			        orderKeyImageArray = dispatchListFindImage.FirstOrDefault().OrderImageKey;
			    }
			    if (dispatchListFindImage.Count > 1)
			    {
			        orderKeyImageArray = String.Join("X", dispatchListFindImage.Select(d=>d.OrderImageKey));
			    }
			    dispatchDataRow.OrderD.OrderImageKey = orderKeyImageArray;
				// get container size
				//if (dispatchDataRow.OrderD.ContainerSizeI == Constants.CONTAINERSIZE1)
				//{
				//	dispatchDataRow.OrderD.ContainerSizeN = Constants.CONTAINERSIZE1N;
				//}
				//else if (dispatchDataRow.OrderD.ContainerSizeI == Constants.CONTAINERSIZE2)
				//{
				//	dispatchDataRow.OrderD.ContainerSizeN = Constants.CONTAINERSIZE2N;
				//}
				//else
				//{
				//	dispatchDataRow.OrderD.ContainerSizeN = Constants.CONTAINERSIZE3N;
				//}
				dispatchDataRow.OrderD.ContainerSizeN = Utilities.GetContainerSizeName(dispatchDataRow.OrderD.ContainerSizeI);
				// get name of container type
				var containerTypeItem =
					_containerTypeRepository.Query(con => con.ContainerTypeC == item.ContainerTypeC).FirstOrDefault();

				dispatchDataRow.OrderD.ContainerTypeN = containerTypeItem != null ? containerTypeItem.ContainerTypeN : "";

				// get data from Order_H
				var orderH =
					_orderHRepository.Query(con => con.OrderD == item.OrderD &&
													con.OrderNo == item.OrderNo).FirstOrDefault();
				if (orderH != null)
				{
					dispatchDataRow.OrderH = Mapper.Map<Order_H, OrderViewModel>(orderH);

					// set false for IsLoadingDtRedColor and isDischargeDTRedColor
					dispatchDataRow.OrderH.IsLoadingDtRedColor = false;
					dispatchDataRow.OrderH.IsStopoverDtRedColor = false;
					dispatchDataRow.OrderH.IsDischargeDtRedColor = false;
					dispatchDataRow.OrderH.IsETDRedColor = false;
					dispatchDataRow.OrderH.IsClosingDTRedColor = false;
					// get short name of customer
					var customerItem =
						_customerRepository.Query(
							con => con.CustomerMainC == orderH.CustomerMainC && con.CustomerSubC == orderH.CustomerSubC)
							.FirstOrDefault();
					if (customerItem != null)
					{
						dispatchDataRow.OrderH.CustomerShortN = customerItem.CustomerN;
						if (!string.IsNullOrEmpty(customerItem.CustomerShortN))
						{
							dispatchDataRow.OrderH.CustomerShortN = customerItem.CustomerShortN;
						}
					}
					else
					{
						dispatchDataRow.OrderH.CustomerShortN = "";
					}

					#region For display Department Name
					//// get department name
					//var departmentItem = _departmentRepository.Query(con => con.DepC == orderH.OrderDepC).FirstOrDefault();
					//if (departmentItem != null)
					//{
					//	//department = Mapper.Map<Department_M, DepartmentViewModel>(departmentItem);
					//	dispatchDataRow.OrderH.OrderDepN = departmentItem.DepN;
					//}
					//else
					//{
					//	dispatchDataRow.OrderH.OrderDepN = "";
					//}
					#endregion

					// get employee name
					var employee = _employeeRepository.Query(em => em.EmployeeC == orderH.EntryClerkC).FirstOrDefault();
					if (employee != null)
					{
						dispatchDataRow.OrderH.EntryClerkN = employee.EmployeeLastN + " " + employee.EmployeeFirstN;
						dispatchDataRow.OrderH.EntryClerkFirstN = employee.EmployeeFirstN;
					}
					else
					{
						dispatchDataRow.OrderH.EntryClerkN = "";
						dispatchDataRow.OrderH.EntryClerkFirstN = "";
					}

					// get name of order type
					if (dispatchDataRow.OrderH.OrderTypeI == Constants.EXP)
					{
						dispatchDataRow.OrderH.OrderTypeN = Constants.EXPNAME;
					}
					else if (dispatchDataRow.OrderH.OrderTypeI == Constants.IMP)
					{
						dispatchDataRow.OrderH.OrderTypeN = Constants.IMPNAME;
					}
					else
					{
						dispatchDataRow.OrderH.OrderTypeN = Constants.ETCNAME;
					}

					var dispatchDList = (from d in _dispatchRepository.GetAllQueryable()
												join o1 in _operationRepository.GetAllQueryable() on d.Operation1C
													equals o1.OperationC into t3
												from o1 in t3.DefaultIfEmpty()
												join o2 in _operationRepository.GetAllQueryable() on d.Operation2C
													equals o2.OperationC into t4
												from o2 in t4.DefaultIfEmpty()
												join o3 in _operationRepository.GetAllQueryable() on d.Operation3C
													equals o3.OperationC into t5
												from o3 in t5.DefaultIfEmpty()
												where (
													d.OrderD == item.OrderD &&
													d.OrderNo == item.OrderNo &&
													d.DetailNo == item.DetailNo

												)
												select new DispatchViewModel
												{
													OrderD = d.OrderD,
													OrderNo = d.OrderNo,
													DetailNo = d.DetailNo,
													DispatchNo = d.DispatchNo,
													TransportD = d.TransportD,
													DispatchI = d.DispatchI,
													TruckC = d.TruckC,
													RegisteredNo = d.RegisteredNo,
													DriverC = d.DriverC,
													AssistantC = d.AssistantC,
													PartnerMainC = d.PartnerMainC,
													PartnerSubC = d.PartnerSubC,
													OrderTypeI = d.OrderTypeI,
													DispatchOrder = d.DispatchOrder,
													ContainerStatus = d.ContainerStatus,
													DispatchStatus = d.DispatchStatus,
													Location1C = d.Location1C,
													Location2C = d.Location2C,
													Location3C = d.Location3C,
													Location1DT = d.Location1DT,
													Location2DT = d.Location2DT,
													Location3DT = d.Location3DT,
													Location1N = (d.Operation1C != null & d.Operation1C != "" & d.Operation1C != "0") ? d.Location1N + "(" + o1.OperationN + ")" : d.Location1N,
													Location2N = (d.Operation2C != null & d.Operation2C != "" & d.Operation2C != "0") ? d.Location2N + "(" + o2.OperationN + ")" : d.Location2N,
													Location3N = (d.Operation3C != null & d.Operation3C != "" & d.Operation3C != "0") ? d.Location3N + "(" + o3.OperationN + ")" : d.Location3N,
													Operation1C = d.Operation1C,
													Operation2C = d.Operation2C,
													Operation3C = d.Operation3C,
													InvoiceD = d.InvoiceD,
													TransportDepC = d.TransportDepC,
													InvoiceStatus = d.InvoiceStatus,
													Description = d.Description,
													//Operation1N = o1.OperationN,
													//Operation2N = o2.OperationN,
													//Operation3N = o3.OperationN,
													//DetainDay = d.DetainDay,
													IsTransported1 = d.IsTransported1,
													IsTransported2 = d.IsTransported2,
													IsTransported3 = d.IsTransported3,
                                                    ImageCount = d.ImageCount,
                                                    OrderImageKey = d.OrderImageKey
												}).ToList();
					if (dispatchDList.Count > 0)
					{
						//dispatchDataRow.DispatchDList = Mapper.Map<List<Dispatch_D>, List<DispatchViewModel>>(dispatchDList);
						dispatchDataRow.DispatchDList = dispatchDList;

						if (dispatchDataRow.OrderH != null)
						{
							foreach (var dispatchD in dispatchDataRow.DispatchDList)
							{
								// get RegisteredNo
								var truckC = dispatchD.TruckC;
								var truck = _truckRepository.Query(tru => tru.TruckC == truckC).FirstOrDefault();
								if (truck != null)
								{
									dispatchD.RegisteredNo = truck.RegisteredNo;
								}

								// get driverN
								var driverC = dispatchD.DriverC;
								var driver = _driverRepository.Query(dri => dri.DriverC == driverC).FirstOrDefault();
								if (driver != null)
								{
                                    dispatchD.DriverN = driver.LastN + " " + driver.FirstN;
                                    dispatchD.DriverFirstN = driver.FirstN;
								}

								// get assistantN
								var assistantC = dispatchD.AssistantC;
								var assistant = _driverRepository.Query(dri => dri.DriverC == assistantC).FirstOrDefault();
								if (assistant != null)
								{
									//dispatchD.AssistantN = driver.FirstN;
									dispatchD.AssistantN = driver.LastN + " " + driver.FirstN;
								}

								// get partnerN
								var partnerMainC = dispatchD.PartnerMainC;
								var partnerSubC = dispatchD.PartnerSubC;
								var partner = _partnerRepository.Query(par => par.PartnerMainC == partnerMainC &&
																			  par.PartnerSubC == partnerSubC).FirstOrDefault();
								if (partner != null)
								{
									dispatchD.PartnerN = partner.PartnerN;
									dispatchD.PartnerShortN = partner.PartnerShortN;
								}

								// cal detainDay
								//if (dispatchD.DetainDay == 0)
								//{
								//	if (dispatchD.ContainerStatus == Constants.LOAD)
								//	{
								//		if (dispatchDataRow.OrderD.ActualLoadingD != null && dispatchDataRow.OrderD.ActualDischargeD != null)
								//		{
								//			var detainDay = Utilities.SubstractTwoDate((DateTime)dispatchDataRow.OrderD.ActualDischargeD,
								//				(DateTime)dispatchDataRow.OrderD.ActualLoadingD);
								//			detainDay = detainDay < 0 ? 0 : detainDay;
								//			dispatchD.DetainDay = detainDay;
								//		}
								//		else if (dispatchDataRow.OrderD.ActualLoadingD != null && dispatchDataRow.OrderD.ActualDischargeD == null)
								//		{
								//			var detainDay = Utilities.SubstractTwoDate(DateTime.Now,
								//				(DateTime)dispatchDataRow.OrderD.ActualLoadingD);
								//			detainDay = detainDay < 0 ? 0 : detainDay;
								//			dispatchD.DetainDay = detainDay;
								//		}
								//	}
								//}
							}
						}
					}

					// get warning cut-off day
					var basic = _basicRepository.GetAllQueryable().FirstOrDefault();
					var warningCutOffDay = 2;
					if (basic != null)
					{
						warningCutOffDay = basic.WarningCutOff;
					}

					// set final color for isLoadingDTRedColor
					if (dispatchDataRow.OrderH != null)
					{
						if (dispatchDataRow.DispatchDList != null && dispatchDataRow.DispatchDList.Count > 0)
						{
							int intDispatchStatus0 = 0;
							for (int iloop = 0; iloop < dispatchDataRow.DispatchDList.Count; iloop++)
							{
								if (dispatchDataRow.DispatchDList[iloop].DispatchStatus == Constants.NOTDISPATCH ||
									dispatchDataRow.DispatchDList[iloop].DispatchStatus == Constants.DISPATCH)
								{
									intDispatchStatus0++;
								}
							}

							if (intDispatchStatus0 == dispatchDataRow.DispatchDList.Count)
							{
								if (dispatchDataRow.OrderH.LoadingDT != null)
								{
									if (Utilities.SubstractTwoDate((DateTime)dispatchDataRow.OrderH.LoadingDT, DateTime.Now) <= warningCutOffDay)
									{
										dispatchDataRow.OrderH.IsLoadingDtRedColor = true;
									}
									else
									{
										dispatchDataRow.OrderH.IsLoadingDtRedColor = false;
									}
								}

								if (dispatchDataRow.OrderH.StopoverDT != null)
								{
									if (Utilities.SubstractTwoDate((DateTime)dispatchDataRow.OrderH.StopoverDT, DateTime.Now) <= warningCutOffDay)
									{
										dispatchDataRow.OrderH.IsStopoverDtRedColor = true;
									}
									else
									{
										dispatchDataRow.OrderH.IsStopoverDtRedColor = false;
									}
								}

								if (dispatchDataRow.OrderH.DischargeDT != null)
								{
									if (Utilities.SubstractTwoDate((DateTime)dispatchDataRow.OrderH.DischargeDT, DateTime.Now) <= warningCutOffDay)
									{
										dispatchDataRow.OrderH.IsDischargeDtRedColor = true;
									}
									else
									{
										dispatchDataRow.OrderH.IsDischargeDtRedColor = false;
									}
								}

								if (dispatchDataRow.OrderH.ETD != null)
								{
									if (Utilities.SubstractTwoDate((DateTime)dispatchDataRow.OrderH.ETD, DateTime.Now) <= warningCutOffDay)
									{
										dispatchDataRow.OrderH.IsETDRedColor = true;
									}
									else
									{
										dispatchDataRow.OrderH.IsETDRedColor = false;
									}
								}

								if (dispatchDataRow.OrderH.ClosingDT != null)
								{
									if (Utilities.SubstractTwoDate((DateTime)dispatchDataRow.OrderH.ClosingDT, DateTime.Now) <= warningCutOffDay)
									{
										dispatchDataRow.OrderH.IsClosingDTRedColor = true;
									}
									else
									{
										dispatchDataRow.OrderH.IsClosingDTRedColor = false;
									}
								}

							}
							else if (intDispatchStatus0 > 0)
							{
								if (dispatchDataRow.OrderH.DischargeDT != null)
								{
									if (Utilities.SubstractTwoDate((DateTime)dispatchDataRow.OrderH.DischargeDT, DateTime.Now) <= warningCutOffDay)
									{
										dispatchDataRow.OrderH.IsDischargeDtRedColor = true;
									}
									else
									{
										dispatchDataRow.OrderH.IsDischargeDtRedColor = false;
									}
								}
								if (dispatchDataRow.OrderH.ETD != null)
								{
									if (Utilities.SubstractTwoDate((DateTime)dispatchDataRow.OrderH.ETD, DateTime.Now) <= warningCutOffDay)
									{
										dispatchDataRow.OrderH.IsETDRedColor = true;
									}
									else
									{
										dispatchDataRow.OrderH.IsETDRedColor = false;
									}
								}
								if (dispatchDataRow.OrderH.ClosingDT != null)
								{
									if (Utilities.SubstractTwoDate((DateTime)dispatchDataRow.OrderH.ClosingDT, DateTime.Now) <= warningCutOffDay)
									{
										dispatchDataRow.OrderH.IsClosingDTRedColor = true;
									}
									else
									{
										dispatchDataRow.OrderH.IsClosingDTRedColor = false;
									}
								}
							}
						}
						else
						{
							if (dispatchDataRow.OrderH.LoadingDT != null)
							{
								if (Utilities.SubstractTwoDate((DateTime)dispatchDataRow.OrderH.LoadingDT, DateTime.Now) <= warningCutOffDay)
								{
									dispatchDataRow.OrderH.IsLoadingDtRedColor = true;
								}
								else
								{
									dispatchDataRow.OrderH.IsLoadingDtRedColor = false;
								}
							}

							if (dispatchDataRow.OrderH.StopoverDT != null)
							{
								if (Utilities.SubstractTwoDate((DateTime)dispatchDataRow.OrderH.StopoverDT, DateTime.Now) <= warningCutOffDay)
								{
									dispatchDataRow.OrderH.IsStopoverDtRedColor = true;
								}
								else
								{
									dispatchDataRow.OrderH.IsStopoverDtRedColor = false;
								}
							}

							if (dispatchDataRow.OrderH.DischargeDT != null)
							{
								if (Utilities.SubstractTwoDate((DateTime)dispatchDataRow.OrderH.DischargeDT, DateTime.Now) <= warningCutOffDay)
								{
									dispatchDataRow.OrderH.IsDischargeDtRedColor = true;
								}
								else
								{
									dispatchDataRow.OrderH.IsDischargeDtRedColor = false;
								}
							}
							if (dispatchDataRow.OrderH.ETD != null)
							{
								if (Utilities.SubstractTwoDate((DateTime)dispatchDataRow.OrderH.ETD, DateTime.Now) <= warningCutOffDay)
								{
									dispatchDataRow.OrderH.IsETDRedColor = true;
								}
								else
								{
									dispatchDataRow.OrderH.IsETDRedColor = false;
								}
							}
							if (dispatchDataRow.OrderH.ClosingDT != null)
							{
								if (Utilities.SubstractTwoDate((DateTime)dispatchDataRow.OrderH.ClosingDT, DateTime.Now) <= warningCutOffDay)
								{
									dispatchDataRow.OrderH.IsClosingDTRedColor = true;
								}
								else
								{
									dispatchDataRow.OrderH.IsClosingDTRedColor = false;
								}
							}
						}

					}
					dispatchDatatable.DispatchList.Add(dispatchDataRow);
				}
			}

			dispatchDatatable.Total = orderDCount;
			dispatchDatatable.DispatchList = dispatchDatatable.DispatchList.OfType<DispatchDataRow>().ToList();
			return dispatchDatatable;
		}

		public DispatchDetailViewModel GetDispatchDetail(DateTime orderD, string orderNo, int detailNo, int dispatchNo)
		{
			try
			{
				var dispatchDetailViewModel = new DispatchDetailViewModel();
				var dispatchViewModel = new DispatchViewModel();

				// get orderH info
				var orderH = _orderHRepository.Query(ord => ord.OrderD == orderD.Date &&
				                                            ord.OrderNo == orderNo
					).FirstOrDefault();
				if (orderH != null)
				{
					dispatchDetailViewModel.OrderH = Mapper.Map<Order_H, OrderViewModel>(orderH);
				}

				// get orderD info
				var orderDetail = _orderDRepository.Query(ord => ord.OrderD == orderD.Date &&
				                                                 ord.OrderNo == orderNo &&
				                                                 ord.DetailNo == detailNo
					).FirstOrDefault();
				if (orderDetail != null)
				{
					dispatchDetailViewModel.OrderD = Mapper.Map<Order_D, ContainerViewModel>(orderDetail);

					// get trailerN
					dispatchDetailViewModel.OrderD.TrailerNo = "";
					if (orderDetail.TrailerC != null)
					{
						var trailer = _trailerRepository.Query(tra => tra.TrailerC == orderDetail.TrailerC).FirstOrDefault();
						if (trailer != null)
						{
							dispatchDetailViewModel.OrderD.TrailerNo = trailer.TrailerNo;
						}
					}

					// get ContainerTypeN
					dispatchDetailViewModel.OrderD.ContainerTypeN = "";
					if (orderDetail.ContainerTypeC != null)
					{
						var conType = _containerTypeRepository.Query(p => p.ContainerTypeC == orderDetail.ContainerTypeC).FirstOrDefault();
						if (conType != null)
						{
							dispatchDetailViewModel.OrderD.ContainerTypeN = conType.ContainerTypeN;
						}
					}
				}

				// get dispatch info
				var dispatch = _dispatchRepository.Query(disp => disp.OrderD == orderD.Date &&
				                                                 disp.OrderNo == orderNo &&
				                                                 disp.DetailNo == detailNo &&
				                                                 disp.DispatchNo == dispatchNo
					).FirstOrDefault();
				if (dispatch != null)
				{
					dispatchDetailViewModel.Dispatch = Mapper.Map<Dispatch_D, DispatchViewModel>(dispatch);

					// get RegisteredNo
					if (!(dispatch.DispatchI == "1" && string.IsNullOrEmpty(dispatch.TruckC)))
					{
						var truckC = dispatch.TruckC;
						var truck = _truckRepository.Query(tru => tru.TruckC == truckC).FirstOrDefault();
						if (truck != null)
						{
							dispatchDetailViewModel.Dispatch.RegisteredNo = truck.RegisteredNo;
							dispatchDetailViewModel.Dispatch.AcquisitionD = truck.AcquisitionD;
							dispatchDetailViewModel.Dispatch.DisusedD = truck.DisusedD;
						}
					}

					// get driverN
					var driverC = dispatch.DriverC;
					var driver = _driverRepository.Query(dri => dri.DriverC == driverC).FirstOrDefault();
					if (driver != null)
					{
						dispatchDetailViewModel.Dispatch.DriverFirstN = driver.FirstN;
						dispatchDetailViewModel.Dispatch.DriverN = driver.LastN + " " + driver.FirstN;
						dispatchDetailViewModel.Dispatch.RetiredD = driver.RetiredD;
					}

					//get assistantN
					var assistantC = dispatch.AssistantC;
					var driver1 = _driverRepository.Query(dri => dri.DriverC == assistantC).FirstOrDefault();
					if (driver1 != null)
					{
						dispatchDetailViewModel.Dispatch.AssistantN = driver1.LastN + " " + driver1.FirstN;
						dispatchDetailViewModel.Dispatch.RetiredD = driver1.RetiredD;
					}

					// get partnerN
					var partnerMainC = dispatch.PartnerMainC;
					var partnerSubC = dispatch.PartnerSubC;
					var partner = _partnerRepository.Query(par => par.PartnerMainC == partnerMainC &&
					                                              par.PartnerSubC == partnerSubC).FirstOrDefault();
					if (partner != null)
					{
						dispatchDetailViewModel.Dispatch.PartnerN = partner.PartnerN;
						dispatchDetailViewModel.Dispatch.PartnerShortN = partner.PartnerShortN;
					}
				}
				else
				{
					dispatchDetailViewModel.Dispatch = new DispatchViewModel();
					var dispatch2 = _dispatchRepository.Query(disp => disp.OrderD == orderD.Date &&
					                                                  disp.OrderNo == orderNo &&
					                                                  disp.DetailNo == detailNo
						).OrderBy("DispatchNo Desc").FirstOrDefault();

					var orh = _orderHRepository.Query(o => o.OrderD == orderD.Date && o.OrderNo == orderNo).FirstOrDefault();
					if (dispatch2 != null)
					{
						if (!String.IsNullOrEmpty(dispatch2.Location3C) || !String.IsNullOrEmpty(dispatch2.Location2C) ||
						    !String.IsNullOrEmpty(dispatch2.Location1C))
						{
							if (orh != null)
							{
								if (orh.OrderTypeI == "1")
								{
									dispatchDetailViewModel.Dispatch.ContainerStatus = "3";
									if (dispatch2.Location2N != dispatchDetailViewModel.OrderH.StopoverPlaceN)
									{
										dispatchDetailViewModel.Dispatch.Operation1C = "XH";
										dispatchDetailViewModel.Dispatch.Location1C = dispatch2.Location2C;
										dispatchDetailViewModel.Dispatch.Location1N = dispatch2.Location2N;
										dispatchDetailViewModel.Dispatch.Location1DT = dispatch2.Location2DT;

										dispatchDetailViewModel.Dispatch.Operation2C = "TR";
										dispatchDetailViewModel.Dispatch.Location2C = dispatchDetailViewModel.OrderH.DischargePlaceC;
										dispatchDetailViewModel.Dispatch.Location2N = dispatchDetailViewModel.OrderH.DischargePlaceN;
										dispatchDetailViewModel.Dispatch.Location2DT = dispatchDetailViewModel.OrderH.DischargeDT;
									}
									else
									{
										dispatchDetailViewModel.Dispatch.Operation1C = "XH";
										dispatchDetailViewModel.Dispatch.Location1C = dispatchDetailViewModel.OrderH.StopoverPlaceC;
										dispatchDetailViewModel.Dispatch.Location1N = dispatchDetailViewModel.OrderH.StopoverPlaceN;
										dispatchDetailViewModel.Dispatch.Location1DT = dispatchDetailViewModel.OrderH.StopoverDT;

										dispatchDetailViewModel.Dispatch.Operation2C = "TR";
										dispatchDetailViewModel.Dispatch.Location2C = dispatchDetailViewModel.OrderH.DischargePlaceC;
										dispatchDetailViewModel.Dispatch.Location2N = dispatchDetailViewModel.OrderH.DischargePlaceN;
										dispatchDetailViewModel.Dispatch.Location2DT = dispatchDetailViewModel.OrderH.DischargeDT;
									}
								}
								else if (orderH.OrderTypeI == "2")
								{
									dispatchDetailViewModel.Dispatch.ContainerStatus = "3";
									if (dispatch2.Location2N != dispatchDetailViewModel.OrderH.DischargePlaceN)
									{
										dispatchDetailViewModel.Dispatch.Operation1C = "XH";
										dispatchDetailViewModel.Dispatch.Location1C = dispatch2.Location2C;
										dispatchDetailViewModel.Dispatch.Location1N = dispatch2.Location2N;
										dispatchDetailViewModel.Dispatch.Location1DT = dispatch2.Location2DT;

										dispatchDetailViewModel.Dispatch.Operation2C = "TR";
										dispatchDetailViewModel.Dispatch.Location2C = dispatchDetailViewModel.OrderH.LoadingPlaceC;
										dispatchDetailViewModel.Dispatch.Location2N = dispatchDetailViewModel.OrderH.LoadingPlaceN;
										dispatchDetailViewModel.Dispatch.Location2DT = dispatchDetailViewModel.OrderH.LoadingDT;
									}
									else
									{
										dispatchDetailViewModel.Dispatch.Operation1C = "XH";
										dispatchDetailViewModel.Dispatch.Location1C = dispatchDetailViewModel.OrderH.DischargePlaceC;
										dispatchDetailViewModel.Dispatch.Location1N = dispatchDetailViewModel.OrderH.DischargePlaceN;
										dispatchDetailViewModel.Dispatch.Location1DT = dispatchDetailViewModel.OrderH.DischargeDT;

										dispatchDetailViewModel.Dispatch.Operation2C = "TR";
										dispatchDetailViewModel.Dispatch.Location2C = dispatchDetailViewModel.OrderH.LoadingPlaceC;
										dispatchDetailViewModel.Dispatch.Location2N = dispatchDetailViewModel.OrderH.LoadingPlaceN;
										dispatchDetailViewModel.Dispatch.Location2DT = dispatchDetailViewModel.OrderH.LoadingDT;
									}
								}
								else
								{
									dispatchDetailViewModel.Dispatch.ContainerStatus = "3";
									if (dispatch2.Location2N != dispatchDetailViewModel.OrderH.StopoverPlaceN)
									{
										dispatchDetailViewModel.Dispatch.Operation1C = orderDetail.ContainerSizeI == "3" ? "XH" : "LH";
										dispatchDetailViewModel.Dispatch.Location1C = dispatch2.Location2C;
										dispatchDetailViewModel.Dispatch.Location1N = dispatch2.Location2N;
										dispatchDetailViewModel.Dispatch.Location1DT = dispatch2.Location2DT;

										dispatchDetailViewModel.Dispatch.Operation2C = orderDetail.ContainerSizeI == "3" ? "TR" : "HC";
										dispatchDetailViewModel.Dispatch.Location2C = dispatchDetailViewModel.OrderH.DischargePlaceC;
										dispatchDetailViewModel.Dispatch.Location2N = dispatchDetailViewModel.OrderH.DischargePlaceN;
										dispatchDetailViewModel.Dispatch.Location2DT = dispatchDetailViewModel.OrderH.DischargeDT;
									}
									else
									{
										dispatchDetailViewModel.Dispatch.Operation1C = orderDetail.ContainerSizeI == "3" ? "XH" : "LH";
										dispatchDetailViewModel.Dispatch.Location1C = dispatchDetailViewModel.OrderH.StopoverPlaceC;
										dispatchDetailViewModel.Dispatch.Location1N = dispatchDetailViewModel.OrderH.StopoverPlaceN;
										dispatchDetailViewModel.Dispatch.Location1DT = dispatchDetailViewModel.OrderH.StopoverDT;

										dispatchDetailViewModel.Dispatch.Operation2C = orderDetail.ContainerSizeI == "3" ? "TR" : "HC";
										dispatchDetailViewModel.Dispatch.Location2C = dispatchDetailViewModel.OrderH.DischargePlaceC;
										dispatchDetailViewModel.Dispatch.Location2N = dispatchDetailViewModel.OrderH.DischargePlaceN;
										dispatchDetailViewModel.Dispatch.Location2DT = dispatchDetailViewModel.OrderH.DischargeDT;
									}
								}
							}
						}
					}
					else
					{
						dispatchDetailViewModel.Dispatch.ContainerStatus = "2";
						if (orderH.OrderTypeI == "1")
						{
							dispatchDetailViewModel.Dispatch.Location1C = dispatchDetailViewModel.OrderH.LoadingPlaceC;
							dispatchDetailViewModel.Dispatch.Location1N = dispatchDetailViewModel.OrderH.LoadingPlaceN;
							dispatchDetailViewModel.Dispatch.Location1DT = dispatchDetailViewModel.OrderH.LoadingDT;
							dispatchDetailViewModel.Dispatch.Operation1C = orderDetail.ContainerSizeI == "3" ? "LH" : "LC";

							dispatchDetailViewModel.Dispatch.Location2C = dispatchDetailViewModel.OrderH.StopoverPlaceC;
							dispatchDetailViewModel.Dispatch.Location2N = dispatchDetailViewModel.OrderH.StopoverPlaceN;
							dispatchDetailViewModel.Dispatch.Location2DT = dispatchDetailViewModel.OrderH.StopoverDT;
							dispatchDetailViewModel.Dispatch.Operation2C = "XH";

							dispatchDetailViewModel.Dispatch.Location3C = dispatchDetailViewModel.OrderH.DischargePlaceC;
							dispatchDetailViewModel.Dispatch.Location3N = dispatchDetailViewModel.OrderH.DischargePlaceN;
							dispatchDetailViewModel.Dispatch.Location3DT = dispatchDetailViewModel.OrderH.DischargeDT;
							dispatchDetailViewModel.Dispatch.Operation3C = "TR";
						}
						else if (orderH.OrderTypeI == "2")
						{
							dispatchDetailViewModel.Dispatch.Location1C = dispatchDetailViewModel.OrderH.StopoverPlaceC;
							dispatchDetailViewModel.Dispatch.Location1N = dispatchDetailViewModel.OrderH.StopoverPlaceN;
							dispatchDetailViewModel.Dispatch.Location1DT = dispatchDetailViewModel.OrderH.StopoverDT;
							dispatchDetailViewModel.Dispatch.Operation1C = "LH";

							dispatchDetailViewModel.Dispatch.Location2C = dispatchDetailViewModel.OrderH.DischargePlaceC;
							dispatchDetailViewModel.Dispatch.Location2N = dispatchDetailViewModel.OrderH.DischargePlaceN;
							dispatchDetailViewModel.Dispatch.Location2DT = dispatchDetailViewModel.OrderH.DischargeDT;
							dispatchDetailViewModel.Dispatch.Operation2C = "XH";

							dispatchDetailViewModel.Dispatch.Location3C = dispatchDetailViewModel.OrderH.LoadingPlaceC;
							dispatchDetailViewModel.Dispatch.Location3N = dispatchDetailViewModel.OrderH.LoadingPlaceN;
							dispatchDetailViewModel.Dispatch.Location3DT = dispatchDetailViewModel.OrderH.LoadingDT;
							dispatchDetailViewModel.Dispatch.Operation3C = "TR";
						}
						else
						{
							dispatchDetailViewModel.Dispatch.Location1C = dispatchDetailViewModel.OrderH.LoadingPlaceC;
							dispatchDetailViewModel.Dispatch.Location1N = dispatchDetailViewModel.OrderH.LoadingPlaceN;
							dispatchDetailViewModel.Dispatch.Location1DT = dispatchDetailViewModel.OrderH.LoadingDT;
							dispatchDetailViewModel.Dispatch.Operation1C = orderDetail.ContainerSizeI == "3" ? "LH" : "LR";

							dispatchDetailViewModel.Dispatch.Location2C = dispatchDetailViewModel.OrderH.StopoverPlaceC;
							dispatchDetailViewModel.Dispatch.Location2N = dispatchDetailViewModel.OrderH.StopoverPlaceN;
							dispatchDetailViewModel.Dispatch.Location2DT = dispatchDetailViewModel.OrderH.StopoverDT;
							dispatchDetailViewModel.Dispatch.Operation2C = orderDetail.ContainerSizeI == "3" ? "XH" : "LH";

							dispatchDetailViewModel.Dispatch.Location3C = dispatchDetailViewModel.OrderH.DischargePlaceC;
							dispatchDetailViewModel.Dispatch.Location3N = dispatchDetailViewModel.OrderH.DischargePlaceN;
							dispatchDetailViewModel.Dispatch.Location3DT = dispatchDetailViewModel.OrderH.DischargeDT;
							dispatchDetailViewModel.Dispatch.Operation3C = orderDetail.ContainerSizeI == "3" ? "TR" : "HC";
						}
					}
				}

				return dispatchDetailViewModel;
			}
			catch (Exception)
			{
				throw;
			}

		}

		public void UpdateDispatchInfo(DispatchDetailViewModel dispatchDetail)
		{
			var orderD = dispatchDetail.Dispatch.OrderD.Date;
			var orderNo = dispatchDetail.Dispatch.OrderNo;
			var detailNo = dispatchDetail.Dispatch.DetailNo;
			var dispatchNo = dispatchDetail.Dispatch.DispatchNo;

			var orderDUpdate = _orderDRepository.Query(ord => ord.OrderD == orderD &&
															  ord.OrderNo == orderNo &&
															  ord.DetailNo == detailNo
													  ).FirstOrDefault();
			if (orderDUpdate != null)
			{
				// update to Order_D
				orderDUpdate.ContainerNo = dispatchDetail.OrderD.ContainerNo;
				orderDUpdate.TrailerC = dispatchDetail.OrderD.TrailerC;
				orderDUpdate.ActualLoadingD = dispatchDetail.OrderD.ActualLoadingD;
				orderDUpdate.ActualDischargeD = dispatchDetail.OrderD.ActualDischargeD;
				orderDUpdate.SealNo = dispatchDetail.OrderD.SealNo;
				orderDUpdate.Description = dispatchDetail.OrderD.Description;
				if (dispatchDetail.OrderH.OrderTypeI == "0")
				{
					if (dispatchDetail.Dispatch.Operation1C == "LR")
					{
						orderDUpdate.LocationDispatch1 = dispatchDetail.Dispatch.Location1N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location1DT;
					}
					if (dispatchDetail.Dispatch.Operation2C == "LR")
					{
						orderDUpdate.LocationDispatch1 = dispatchDetail.Dispatch.Location2N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location2DT;
					}
					if (dispatchDetail.Dispatch.Operation3C == "LR")
					{
						orderDUpdate.LocationDispatch1 = dispatchDetail.Dispatch.Location3N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location3DT;
					}

					if (dispatchDetail.Dispatch.Operation1C == "LH")
					{
						orderDUpdate.LocationDispatch2 = dispatchDetail.Dispatch.Location1N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location1DT;
					}
					if (dispatchDetail.Dispatch.Operation2C == "LH")
					{
						orderDUpdate.LocationDispatch2 = dispatchDetail.Dispatch.Location2N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location2DT;
					}
					if (dispatchDetail.Dispatch.Operation3C == "LH")
					{
						orderDUpdate.LocationDispatch2 = dispatchDetail.Dispatch.Location3N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location3DT;
					}

					if (dispatchDetail.Dispatch.Operation1C == "HC")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location1N;
						orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location1DT;
					}
					if (dispatchDetail.Dispatch.Operation2C == "HC")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location2N;
						orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location2DT;
					}
					if (dispatchDetail.Dispatch.Operation3C == "HC")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location3N;
						orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location3DT;
					}
					//exceptions, cannot get goods -> return empty
					if (dispatchDetail.Dispatch.Operation1C == "TR")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location1N;
						orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location1DT;
					}
					if (dispatchDetail.Dispatch.Operation2C == "TR")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location2N;
						orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location2DT;
					}
					if (dispatchDetail.Dispatch.Operation3C == "TR")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location3N;
						orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location3DT;
					}
					//end exceptions
				}
				else if (dispatchDetail.OrderH.OrderTypeI == "1")
				{
					if (dispatchDetail.Dispatch.Operation1C == "LC")
					{
						orderDUpdate.LocationDispatch1 = dispatchDetail.Dispatch.Location1N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location1DT;
					}
					if (dispatchDetail.Dispatch.Operation2C == "LC")
					{
						orderDUpdate.LocationDispatch1 = dispatchDetail.Dispatch.Location2N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location2DT;
					}
					if (dispatchDetail.Dispatch.Operation3C == "LC")
					{
						orderDUpdate.LocationDispatch1 = dispatchDetail.Dispatch.Location3N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location3DT;
					}

					if (dispatchDetail.Dispatch.Operation1C == "XH")
					{
						orderDUpdate.LocationDispatch2 = dispatchDetail.Dispatch.Location1N;
						orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location1DT;
					}
					if (dispatchDetail.Dispatch.Operation2C == "XH")
					{
						orderDUpdate.LocationDispatch2 = dispatchDetail.Dispatch.Location2N;
						orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location2DT;
					}
					if (dispatchDetail.Dispatch.Operation3C == "XH")
					{
						orderDUpdate.LocationDispatch2 = dispatchDetail.Dispatch.Location3N;
						orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location3DT;
					}

					if (dispatchDetail.Dispatch.Operation1C == "TR")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location1N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location1DT;
					}
					if (dispatchDetail.Dispatch.Operation2C == "TR")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location2N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location2DT;
					}
					if (dispatchDetail.Dispatch.Operation3C == "TR")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location3N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location3DT;
					}
				}
				else
				{
					if (dispatchDetail.Dispatch.Operation1C == "LR")
					{
						orderDUpdate.LocationDispatch1 = dispatchDetail.Dispatch.Location1N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location1DT;
					}
					if (dispatchDetail.Dispatch.Operation2C == "LR")
					{
						orderDUpdate.LocationDispatch1 = dispatchDetail.Dispatch.Location2N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location2DT;
					}
					if (dispatchDetail.Dispatch.Operation3C == "LR")
					{
						orderDUpdate.LocationDispatch1 = dispatchDetail.Dispatch.Location3N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location3DT;
					}

					if (dispatchDetail.Dispatch.Operation1C == "LH")
					{
						orderDUpdate.LocationDispatch2 = dispatchDetail.Dispatch.Location1N;
						orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location1DT;
					}
					if (dispatchDetail.Dispatch.Operation2C == "LH")
					{
						orderDUpdate.LocationDispatch2 = dispatchDetail.Dispatch.Location2N;
						orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location2DT;
					}
					if (dispatchDetail.Dispatch.Operation3C == "LH")
					{
						orderDUpdate.LocationDispatch2 = dispatchDetail.Dispatch.Location3N;
						orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location3DT;
					}

					if (dispatchDetail.Dispatch.Operation1C == "XH")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location1N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location1DT;
					}
					if (dispatchDetail.Dispatch.Operation2C == "XH")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location2N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location2DT;
					}
					if (dispatchDetail.Dispatch.Operation3C == "XH")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location3N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location3DT;
					}

					if (dispatchDetail.Dispatch.Operation1C == "TR")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location1N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location1DT;
					}
					if (dispatchDetail.Dispatch.Operation2C == "TR")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location2N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location2DT;
					}
					if (dispatchDetail.Dispatch.Operation3C == "TR")
					{
						orderDUpdate.LocationDispatch3 = dispatchDetail.Dispatch.Location3N;
						if (String.IsNullOrEmpty(orderDUpdate.TruckCLastDispatch))
						{
							orderDUpdate.TruckCLastDispatch = dispatchDetail.Dispatch.TruckC;
						}
						orderDUpdate.LastDDispatch = dispatchDetail.Dispatch.Location3DT;
					}
				}
				orderDUpdate.DetainDay = dispatchDetail.OrderD.DetainDay;
				//if (dispatchDetail.OrderD.ActualDischargeD != null)
				//{
				//	orderDUpdate.RevenueD = dispatchDetail.OrderD.ActualDischargeD;
				//}
				_orderDRepository.Update(orderDUpdate);

				// update to Dispatch_D
				if (dispatchNo != -1)
				{
					var dispatchDToRemove = _dispatchRepository.Query(dis => dis.OrderD == orderD &&
																			 dis.OrderNo == orderNo &&
																			 dis.DetailNo == detailNo &&
																			 dis.DispatchNo == dispatchNo
																	).FirstOrDefault();
					var dispatchDUpdate = Mapper.Map<DispatchViewModel, Dispatch_D>(dispatchDetail.Dispatch);

					// set RegisteredNo
					if (dispatchDUpdate.DispatchI == "0" ||
						(dispatchDUpdate.DispatchI == "1" && !string.IsNullOrEmpty(dispatchDUpdate.TruckC))
						)
					{
						dispatchDUpdate.RegisteredNo = null;
					}

					// calculate detain day
					//if (dispatchDUpdate.ContainerStatus == Constants.LOAD && dispatchDUpdate.DispatchStatus == Constants.TRANSPORTED)
					//{
					//	dispatchDUpdate.DetainDay = 0;
					//	var detainDay = 0;
					//	if (orderDUpdate.ActualLoadingD != null && orderDUpdate.ActualDischargeD != null)
					//	{
					//		detainDay = Utilities.SubstractTwoDate((DateTime)orderDUpdate.ActualDischargeD, (DateTime)orderDUpdate.ActualLoadingD);
					//		detainDay = detainDay < 0 ? 0 : detainDay;
					//		dispatchDUpdate.DetainDay = detainDay;
					//	}
					//	else if (orderDUpdate.ActualLoadingD != null && orderDUpdate.ActualDischargeD == null)
					//	{
					//		detainDay = Utilities.SubstractTwoDate(DateTime.Now, (DateTime)orderDUpdate.ActualLoadingD);
					//		detainDay = detainDay < 0 ? 0 : detainDay;
					//		dispatchDUpdate.DetainDay = detainDay;
					//	}
					//	else
					//	{
					//		dispatchDUpdate.DetainDay = 0;
					//	}
					//}

					_dispatchRepository.Delete(dispatchDToRemove);
					_dispatchRepository.Add(dispatchDUpdate);
					//if (dispatchDUpdate.ContainerStatus == Constants.LOAD)
					//{
					//	var dispatchDCheck = _dispatchRepository.Query(dis => dis.OrderD == orderD &&
					//													  dis.OrderNo == orderNo &&
					//													  dis.DetailNo == detailNo &&
					//													  dis.DispatchNo == dispatchNo + 1
					//												).FirstOrDefault();

					//	if (dispatchDCheck == null)
					//	{
					//		var dispatch2 = dispatchDetail.Dispatch;
					//		dispatch2.DispatchNo = dispatchNo + 1;
					//		dispatch2.TruckC = null;
					//		dispatch2.DriverC = null;
					//		dispatch2.TransportD = null;
					//		dispatch2.DispatchI = "0";
					//		dispatch2.DispatchStatus = Constants.NOTDISPATCH;
					//		dispatch2.ContainerStatus = Constants.DISCHARGE;
					//		dispatch2.PartnerMainC = null;
					//		dispatch2.PartnerSubC = null;

					//		dispatch2.Location1C = null;
					//		dispatch2.Location1N = null;
					//		dispatch2.Location1DT = null;
					//		dispatch2.Location2C = null;
					//		dispatch2.Location2N = null;
					//		dispatch2.Location2DT = null;
					//		dispatch2.Location3C = null;
					//		dispatch2.Location3N = null;
					//		dispatch2.Location3DT = null;

					//		var orderH = _orderHRepository.Query(o => o.OrderD == orderD && o.OrderNo == orderNo).FirstOrDefault();
					//		if (orderH != null)
					//		{
					//			dispatch2.Location1C = orderH.StopoverPlaceC;
					//			dispatch2.Location1N = orderH.StopoverPlaceN;
					//			dispatch2.Location2C = orderH.DischargePlaceC;
					//			dispatch2.Location2N = orderH.DischargePlaceN;
					//		}

					//		var dispatch2Insert = Mapper.Map<DispatchViewModel, Dispatch_D>(dispatch2);
					//		_dispatchRepository.Add(dispatch2Insert);
					//	}
					//}
				}
				else
				{
					var dispatchCheck = _dispatchRepository.Query(dis => dis.OrderD == orderD &&
																			dis.OrderNo == orderNo &&
																			dis.DetailNo == detailNo
																	);

					var dispatchNoMax = 0;
					dispatchDetail.Dispatch.DispatchNo = dispatchNoMax + 1;
					if (dispatchCheck.Any())
					{
						dispatchNoMax = dispatchCheck.Max(i => i.DispatchNo);
						dispatchDetail.Dispatch.DispatchNo = dispatchNoMax + 1;
					}
					var dispatchD = Mapper.Map<DispatchViewModel, Dispatch_D>(dispatchDetail.Dispatch);

					// set RegisteredNo
					if (dispatchD.DispatchI == "0" ||
						(dispatchD.DispatchI == "1" && !string.IsNullOrEmpty(dispatchD.TruckC))
						)
					{
						dispatchD.RegisteredNo = null;
					}

					// cal detain day
					if (dispatchD.ContainerStatus == Constants.LOAD && dispatchD.DispatchStatus == Constants.TRANSPORTED)
					{
						var detainDay = 0;
						if (dispatchDetail.OrderD.ActualLoadingD != null && dispatchDetail.OrderD.ActualDischargeD != null)
						{
							detainDay = Utilities.SubstractTwoDate((DateTime)dispatchDetail.OrderD.ActualDischargeD, (DateTime)dispatchDetail.OrderD.ActualLoadingD);
							detainDay = detainDay < 0 ? 0 : detainDay;
							dispatchD.DetainDay = detainDay;
						}
						else if (dispatchDetail.OrderD.ActualLoadingD != null && dispatchDetail.OrderD.ActualDischargeD == null)
						{
							detainDay = Utilities.SubstractTwoDate(DateTime.Now, (DateTime)dispatchDetail.OrderD.ActualLoadingD);
							detainDay = detainDay < 0 ? 0 : detainDay;
							dispatchD.DetainDay = detainDay;
						}
						else
						{
							dispatchD.DetainDay = 0;
						}
					}

					_dispatchRepository.Add(dispatchD);
					// insert dispatch 2
					//if (dispatchDetail.Dispatch.ContainerStatus == Constants.LOAD)
					//{
					//	var dispatch2 = dispatchDetail.Dispatch;
					//	dispatch2.DispatchNo = dispatchNoMax + 2;
					//	dispatch2.TruckC = null;
					//	dispatch2.DriverC = null;
					//	dispatch2.AssistantC = null;
					//	dispatch2.TransportD = null;
					//	dispatch2.DispatchI = "0";
					//	dispatch2.DispatchStatus = Constants.NOTDISPATCH;
					//	dispatch2.ContainerStatus = Constants.DISCHARGE;
					//	dispatch2.PartnerMainC = null;
					//	dispatch2.PartnerSubC = null;

					//	dispatch2.Location1C = null;
					//	dispatch2.Location1N = null;
					//	dispatch2.Location1DT = null;
					//	dispatch2.Location2C = null;
					//	dispatch2.Location2N = null;
					//	dispatch2.Location2DT = null;
					//	dispatch2.Location3C = null;
					//	dispatch2.Location3N = null;
					//	dispatch2.Location3DT = null;

					//	var orderH = _orderHRepository.Query(o => o.OrderD == orderD && o.OrderNo == orderNo).FirstOrDefault();
					//	if (orderH != null)
					//	{
					//		dispatch2.Location1C = orderH.StopoverPlaceC;
					//		dispatch2.Location1N = orderH.StopoverPlaceN;
					//		dispatch2.Location2C = orderH.DischargePlaceC;
					//		dispatch2.Location2N = orderH.DischargePlaceN;
					//	}

					//	var dispatch2Insert = Mapper.Map<DispatchViewModel, Dispatch_D>(dispatch2);
					//	_dispatchRepository.Add(dispatch2Insert);
					//}
				}

				// commit insert
				SaveDispatch();

				// update DispatchOrder
				if (dispatchDetail.DriverDispatchList != null &&
					dispatchDetail.DriverDispatchList.DriverDispatchList != null &&
					dispatchDetail.DriverDispatchList.DriverDispatchList.Count > 0
					)
				{
					foreach (DispatchViewModel driverDispatch in dispatchDetail.DriverDispatchList.DriverDispatchList)
					{
						var orderDDriver = driverDispatch.OrderD.Date;
						var orderNoDriver = driverDispatch.OrderNo;
						var detailNoDriver = driverDispatch.DetailNo;
						var dispatchNoDriver = driverDispatch.DispatchNo;

						var dispatch = _dispatchRepository.Query(dis => dis.OrderD == orderDDriver &&
																 dis.OrderNo == orderNoDriver &&
																 dis.DetailNo == detailNoDriver &&
																 dis.DispatchNo == dispatchNoDriver
																).FirstOrDefault();

						if (dispatch != null)
						{
							dispatch.DispatchOrder = driverDispatch.DispatchOrder;
							_dispatchRepository.Update(dispatch);
						}
					}
				}

				SaveDispatch();

				//Update container actual dispatch
				//UpdateActualDispatchDate(orderD, orderNo, detailNo);
				UpdateOrderDInfo(orderD, orderNo, detailNo);
				//Task.Run(() => UpdateSalaryDriver(dispatchDetail, salaryDriverType));

				////insert when double cont
				//if (currentDispatchNoMax != -100)
				//{
				//	var orderDouble = _orderDRepository.Query(or => or.OrderNoDouble == dispatchDetail.OrderD.OrderNoDouble &&
				//	(or.OrderD != dispatchDetail.OrderD.OrderD ||
				//	or.OrderNo != dispatchDetail.OrderD.OrderNo ||
				//	or.DetailNo != dispatchDetail.OrderD.DetailNo))
				//	.FirstOrDefault();
				//	if (orderDouble != null)
				//	{
				//		var dispatchDouble = _dispatchRepository.Query(
				//			d => d.OrderD == orderDouble.OrderD && d.OrderNo == orderDouble.OrderNo && d.DetailNo == orderDouble.DetailNo &&
				//			d.DispatchNo == currentDispatchNoMax).FirstOrDefault();
				//		if (dispatchDouble == null)
				//		{
				//			var dataDispatchDouble = GetDispatchDetail(orderDouble.OrderD, orderDouble.OrderNo, orderDouble.DetailNo, -1);
				//			dataDispatchDouble.Dispatch.OrderD = orderDouble.OrderD;
				//			dataDispatchDouble.Dispatch.OrderNo = orderDouble.OrderNo;
				//			dataDispatchDouble.Dispatch.DetailNo = orderDouble.DetailNo;
				//			dataDispatchDouble.Dispatch.DispatchNo = -1;
				//			dataDispatchDouble.Dispatch.TransportD = dispatchDetail.Dispatch.TransportD;
				//			dataDispatchDouble.Dispatch.ContainerStatus = dispatchDetail.Dispatch.ContainerStatus;
				//			dataDispatchDouble.Dispatch.TruckC = dispatchDetail.Dispatch.TruckC;
				//			dataDispatchDouble.Dispatch.RegisteredNo = dispatchDetail.Dispatch.RegisteredNo;
				//			dataDispatchDouble.Dispatch.DispatchI = dispatchDetail.Dispatch.DispatchI;
				//			dataDispatchDouble.Dispatch.DriverC = dispatchDetail.Dispatch.DriverC;
				//			dataDispatchDouble.Dispatch.DriverN = dispatchDetail.Dispatch.DriverN;
				//			dataDispatchDouble.Dispatch.DriverFirstN = dispatchDetail.Dispatch.DriverFirstN;
				//			dataDispatchDouble.Dispatch.AssistantC = dispatchDetail.Dispatch.AssistantC;
				//			dataDispatchDouble.Dispatch.AssistantN = dispatchDetail.Dispatch.AssistantN;
				//			dataDispatchDouble.Dispatch.DispatchOrder = dispatchDetail.Dispatch.DispatchOrder + 1;

				//			//check ContainerStatus task : QT20180302_02: Dispatch
				//			dataDispatchDouble.Dispatch.Location1C = dispatchDetail.Dispatch.Location1C;
				//			dataDispatchDouble.Dispatch.Location1N = dispatchDetail.Dispatch.Location1N;
				//			dataDispatchDouble.Dispatch.Location1A = dispatchDetail.Dispatch.Location1A;
				//			dataDispatchDouble.Dispatch.Location1DT = dispatchDetail.Dispatch.Location1DT;
				//			dataDispatchDouble.Dispatch.Location1Time = dispatchDetail.Dispatch.Location1Time;
				//			dataDispatchDouble.Dispatch.Operation1C = dispatchDetail.Dispatch.Operation1C;
				//			dataDispatchDouble.Dispatch.Operation1N = dispatchDetail.Dispatch.Operation1N;

				//			dataDispatchDouble.Dispatch.Location2C = dispatchDetail.Dispatch.Location2C;
				//			dataDispatchDouble.Dispatch.Location2N = dispatchDetail.Dispatch.Location2N;
				//			dataDispatchDouble.Dispatch.Location2A = dispatchDetail.Dispatch.Location2A;
				//			dataDispatchDouble.Dispatch.Location2DT = dispatchDetail.Dispatch.Location2DT;
				//			dataDispatchDouble.Dispatch.Location2Time = dispatchDetail.Dispatch.Location2Time;
				//			dataDispatchDouble.Dispatch.Operation2C = dispatchDetail.Dispatch.Operation2C;
				//			dataDispatchDouble.Dispatch.Operation2N = dispatchDetail.Dispatch.Operation2N;

				//			dataDispatchDouble.Dispatch.Location3C = dispatchDetail.Dispatch.Location3C;
				//			dataDispatchDouble.Dispatch.Location3N = dispatchDetail.Dispatch.Location3N;
				//			dataDispatchDouble.Dispatch.Location3A = dispatchDetail.Dispatch.Location3A;
				//			dataDispatchDouble.Dispatch.Location3DT = dispatchDetail.Dispatch.Location3DT;
				//			dataDispatchDouble.Dispatch.Location3Time = dispatchDetail.Dispatch.Location3Time;
				//			dataDispatchDouble.Dispatch.Operation3C = dispatchDetail.Dispatch.Operation3C;
				//			dataDispatchDouble.Dispatch.Operation3N = dispatchDetail.Dispatch.Operation3N;

				//			dataDispatchDouble.Dispatch.DispatchStatus = dispatchDetail.Dispatch.DispatchStatus;
				//			//dataDispatchDouble.Dispatch.Relocatecont = dispatchDetail.Dispatch.Relocatecont;
				//			//dataDispatchDouble.Dispatch.Km = dispatchDetail.Dispatch.Km;
				//			//dataDispatchDouble.Dispatch.WayTypeI = dispatchDetail.Dispatch.WayTypeI;
				//			dataDispatchDouble.Dispatch.OrderTypeI = dispatchDetail.Dispatch.OrderTypeI;
				//			dataDispatchDouble.OrderD.DetainDay = dispatchDetail.OrderD.DetainDay;

				//			UpdateDispatchInfo(dataDispatchDouble);
				//		}
				//	}
				//}

			}
		}

		public void UpdateOrderDInfo(DateTime orderD, string orderNo, int detailNo)
		{
			var dispatches = _dispatchRepository.Query(ord => ord.OrderD == orderD.Date &&
																ord.OrderNo == orderNo &&
																ord.DetailNo == detailNo
														).OrderBy(p => p.DispatchNo);
			var orderDToUpdate = _orderDRepository.Query(ord => ord.OrderD == orderD.Date &&
																ord.OrderNo == orderNo &&
																ord.DetailNo == detailNo
														).FirstOrDefault();

			orderDToUpdate.ActualLoadingD = null;
			orderDToUpdate.ActualDischargeD = null;
			orderDToUpdate.ActualPickupReturnD = null;

			orderDToUpdate.TruckCReturn = null;
			orderDToUpdate.TruckCLastDispatch = null;
			//orderDToUpdate.LocationDispatch1 = null;
			//orderDToUpdate.LocationDispatch2 = null;
			//orderDToUpdate.LocationDispatch3 = null;

			var orderH = _orderHRepository.Query(ord => ord.OrderD == orderD.Date &&
																ord.OrderNo == orderNo
														).FirstOrDefault();
			if (orderH != null)
			{
				if (dispatches.Count() == 0)
				{
					orderDToUpdate.LocationDispatch1 = orderH.LoadingPlaceN;
					orderDToUpdate.LocationDispatch2 = orderH.StopoverPlaceN;
					orderDToUpdate.LocationDispatch3 = orderH.DischargePlaceN;
				}
				foreach (var c in dispatches)
				{
					if (orderH.OrderTypeI == "1")
					{
						//if (dispatchItem.Operation1C == "LC")
						//{
						//	orderDetail.LocationDispatch1 = dispatchItem.Location1N;
						//	orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
						//}
						//if (dispatchItem.Operation2C == "LC")
						//{
						//	orderDetail.LocationDispatch1 = dispatchItem.Location2N;
						//	orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
						//}
						//if (dispatchItem.Operation3C == "LC")
						//{
						//	orderDetail.LocationDispatch1 = dispatchItem.Location3N;
						//	orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
						//}

						//if (dispatchItem.Operation1C == "XH")
						//{
						//	orderDetail.LocationDispatch2 = dispatchItem.Location1N;
						//	orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
						//}
						//if (dispatchItem.Operation2C == "XH")
						//{
						//	orderDetail.LocationDispatch2 = dispatchItem.Location2N;
						//	orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
						//}
						//if (dispatchItem.Operation3C == "XH")
						//{
						//	orderDetail.LocationDispatch2 = dispatchItem.Location3N;
						//	orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
						//}

						//if (dispatchItem.Operation1C == "TR")
						//{
						//	orderDetail.LocationDispatch3 = dispatchItem.Location1N;
						//	orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
						//}
						//if (dispatchItem.Operation2C == "TR")
						//{
						//	orderDetail.LocationDispatch3 = dispatchItem.Location2N;
						//	orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
						//}
						//if (dispatchItem.Operation3C == "TR")
						//{
						//	orderDetail.LocationDispatch3 = dispatchItem.Location3N;
						//	orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
						//}
						//Check for Location1
						if (c.Operation1C == "LC")
						{
							if (orderDToUpdate.ActualLoadingD == null)
							{
								orderDToUpdate.ActualLoadingD = c.Location1DT;
							}
							orderDToUpdate.LocationDispatch1 = c.Location1N;
							orderDToUpdate.TruckCLastDispatch = c.TruckC;
						}
						else if (c.Operation1C == "XH")
						{
							if (orderDToUpdate.ActualDischargeD == null)
							{
								orderDToUpdate.ActualDischargeD = c.Location1DT;
							}
							orderDToUpdate.LocationDispatch2 = c.Location1N;
							//orderDToUpdate.TruckCReturn = c.TruckC;
						}
						else if (c.Operation1C == "TR")
						{
							if (orderDToUpdate.ActualPickupReturnD == null)
							{
								orderDToUpdate.ActualPickupReturnD = c.Location1DT;
							}
							orderDToUpdate.LocationDispatch3 = c.Location1N;
							orderDToUpdate.TruckCReturn = c.TruckC;
						}

						//Check for Location2
						if (c.Operation2C == "LC")
						{
							if (orderDToUpdate.ActualLoadingD == null)
							{
								orderDToUpdate.ActualLoadingD = c.Location2DT;
							}
							orderDToUpdate.LocationDispatch1 = c.Location2N;
							orderDToUpdate.TruckCLastDispatch = c.TruckC;
						}
						else if (c.Operation2C == "XH")
						{
							if (orderDToUpdate.ActualDischargeD == null)
							{
								orderDToUpdate.ActualDischargeD = c.Location2DT;
							}
							orderDToUpdate.LocationDispatch2 = c.Location2N;
							//orderDToUpdate.TruckCReturn = c.TruckC;
						}
						else if (c.Operation2C == "TR")
						{
							if (orderDToUpdate.ActualPickupReturnD == null)
							{
								orderDToUpdate.ActualPickupReturnD = c.Location2DT;
							}
							orderDToUpdate.LocationDispatch3 = c.Location2N;
							orderDToUpdate.TruckCReturn = c.TruckC;
						}

						//Check for Location3
						if (c.Operation3C == "LC")
						{
							if (orderDToUpdate.ActualLoadingD == null)
							{
								orderDToUpdate.ActualLoadingD = c.Location3DT;
							}
							orderDToUpdate.LocationDispatch1 = c.Location3N;
							orderDToUpdate.TruckCLastDispatch = c.TruckC;
						}
						else if (c.Operation3C == "XH")
						{
							if (orderDToUpdate.ActualDischargeD == null)
							{
								orderDToUpdate.ActualDischargeD = c.Location3DT;
							}
							orderDToUpdate.LocationDispatch2 = c.Location3N;
							//orderDToUpdate.TruckCReturn = c.TruckC;
						}
						else if (c.Operation3C == "TR")
						{
							if (orderDToUpdate.ActualPickupReturnD == null)
							{
								orderDToUpdate.ActualPickupReturnD = c.Location3DT;
							}
							orderDToUpdate.LocationDispatch3 = c.Location3N;
							orderDToUpdate.TruckCReturn = c.TruckC;
						}
					}
					else if (orderH.OrderTypeI == "0")
					{
						//Check for Location1
						if (c.Operation1C == "LR")
						{
							if (orderDToUpdate.ActualPickupReturnD == null)
							{
								orderDToUpdate.ActualPickupReturnD = c.Location1DT;
							}
							orderDToUpdate.LocationDispatch1 = c.Location1N;
						}
						else if (c.Operation1C == "LH")
						{
							if (orderDToUpdate.ActualLoadingD == null)
							{
								orderDToUpdate.ActualLoadingD = c.Location1DT;
							}
							if (orderDToUpdate.TruckCLastDispatch == null)
							{
								orderDToUpdate.TruckCLastDispatch = c.TruckC;
							}
							orderDToUpdate.LocationDispatch2 = c.Location1N;
						}
						else if (c.Operation1C == "HC")
						{
							if (orderDToUpdate.ActualDischargeD == null)
							{
								orderDToUpdate.ActualDischargeD = c.Location1DT;
							}
							orderDToUpdate.TruckCReturn = c.TruckC;
							orderDToUpdate.LocationDispatch3 = c.Location1N;
						}
						else if (c.Operation1C == "TR")
						{
							if (orderDToUpdate.ActualDischargeD == null)
							{
								orderDToUpdate.ActualDischargeD = c.Location1DT;
							}
							orderDToUpdate.TruckCReturn = c.TruckC;
							orderDToUpdate.LocationDispatch3 = c.Location1N;
						}
						//if (c.Operation1C == "LR")
						//{
						//	if (orderDToUpdate.ActualLoadingD == null)
						//	{
						//		orderDToUpdate.ActualLoadingD = c.Location1DT;
						//	}
						//}
						//else if (c.Operation1C == "LH")
						//{
						//	if (orderDToUpdate.ActualDischargeD == null)
						//	{
						//		orderDToUpdate.ActualDischargeD = c.Location1DT;
						//	}
						//}
						//else if (c.Operation1C == "HC")
						//{
						//	if (orderDToUpdate.ActualPickupReturnD == null)
						//	{
						//		orderDToUpdate.ActualPickupReturnD = c.Location1DT;
						//	}
						//}
						//else if (c.Operation1C == "TR")
						//{
						//	if (orderDToUpdate.ActualPickupReturnD == null)
						//	{
						//		orderDToUpdate.ActualPickupReturnD = c.Location1DT;
						//	}
						//}

						//Check for Location2
						if (c.Operation2C == "LR")
						{
							if (orderDToUpdate.ActualPickupReturnD == null)
							{
								orderDToUpdate.ActualPickupReturnD = c.Location2DT;
							}
							orderDToUpdate.LocationDispatch1 = c.Location2N;
						}
						else if (c.Operation2C == "LH")
						{
							if (orderDToUpdate.ActualLoadingD == null)
							{
								orderDToUpdate.ActualLoadingD = c.Location2DT;
							}
							if (orderDToUpdate.TruckCLastDispatch == null)
							{
								orderDToUpdate.TruckCLastDispatch = c.TruckC;
							}
							orderDToUpdate.LocationDispatch2 = c.Location2N;
						}
						else if (c.Operation2C == "HC")
						{
							if (orderDToUpdate.ActualDischargeD == null)
							{
								orderDToUpdate.ActualDischargeD = c.Location2DT;
							}
							orderDToUpdate.TruckCReturn = c.TruckC;
							orderDToUpdate.LocationDispatch3 = c.Location2N;
						}
						else if (c.Operation2C == "TR")
						{
							if (orderDToUpdate.ActualDischargeD == null)
							{
								orderDToUpdate.ActualDischargeD = c.Location2DT;
							}
							orderDToUpdate.TruckCReturn = c.TruckC;
							orderDToUpdate.LocationDispatch3 = c.Location2N;
						}
						//if (c.Operation2C == "LR")
						//{
						//	if (orderDToUpdate.ActualLoadingD == null)
						//	{
						//		orderDToUpdate.ActualLoadingD = c.Location2DT;
						//	}
						//}
						//else if (c.Operation2C == "LH")
						//{
						//	if (orderDToUpdate.ActualDischargeD == null)
						//	{
						//		orderDToUpdate.ActualDischargeD = c.Location2DT;
						//	}
						//}
						//else if (c.Operation2C == "HC")
						//{
						//	if (orderDToUpdate.ActualPickupReturnD == null)
						//	{
						//		orderDToUpdate.ActualPickupReturnD = c.Location2DT;
						//	}
						//}
						//else if (c.Operation2C == "TR")
						//{
						//	if (orderDToUpdate.ActualPickupReturnD == null)
						//	{
						//		orderDToUpdate.ActualPickupReturnD = c.Location2DT;
						//	}
						//}

						//Check for Location3
						if (c.Operation3C == "LR")
						{
							if (orderDToUpdate.ActualPickupReturnD == null)
							{
								orderDToUpdate.ActualPickupReturnD = c.Location3DT;
							}
							orderDToUpdate.LocationDispatch1 = c.Location3N;
						}
						else if (c.Operation3C == "LH")
						{
							if (orderDToUpdate.ActualLoadingD == null)
							{
								orderDToUpdate.ActualLoadingD = c.Location3DT;
							}
							if (orderDToUpdate.TruckCLastDispatch == null)
							{
								orderDToUpdate.TruckCLastDispatch = c.TruckC;
							}
							orderDToUpdate.LocationDispatch2 = c.Location3N;
						}
						else if (c.Operation3C == "HC")
						{
							if (orderDToUpdate.ActualDischargeD == null)
							{
								orderDToUpdate.ActualDischargeD = c.Location3DT;
							}
							orderDToUpdate.TruckCReturn = c.TruckC;
							orderDToUpdate.LocationDispatch3 = c.Location3N;
						}
						else if (c.Operation3C == "TR")
						{
							if (orderDToUpdate.ActualDischargeD == null)
							{
								orderDToUpdate.ActualDischargeD = c.Location3DT;
							}
							orderDToUpdate.TruckCReturn = c.TruckC;
							orderDToUpdate.LocationDispatch3 = c.Location3N;
						}
						//if (c.Operation3C == "LR")
						//{
						//	if (orderDToUpdate.ActualLoadingD == null)
						//	{
						//		orderDToUpdate.ActualLoadingD = c.Location3DT;
						//	}
						//}
						//else if (c.Operation3C == "LH")
						//{
						//	if (orderDToUpdate.ActualDischargeD == null)
						//	{
						//		orderDToUpdate.ActualDischargeD = c.Location3DT;
						//	}
						//}
						//else if (c.Operation3C == "HC")
						//{
						//	if (orderDToUpdate.ActualPickupReturnD == null)
						//	{
						//		orderDToUpdate.ActualPickupReturnD = c.Location3DT;
						//	}
						//}
						//else if (c.Operation3C == "TR")
						//{
						//	if (orderDToUpdate.ActualPickupReturnD == null)
						//	{
						//		orderDToUpdate.ActualPickupReturnD = c.Location3DT;
						//	}
						//}
					}
					else
					{
						//Check for Location1
						if (c.Operation1C == "LR")
						{
							if (orderDToUpdate.ActualPickupReturnD == null)
							{
								orderDToUpdate.ActualPickupReturnD = c.Location1DT;
							}
							orderDToUpdate.LocationDispatch1 = c.Location1N;
						}
						else if (c.Operation1C == "LH")
						{
							if (orderDToUpdate.ActualLoadingD == null)
							{
								orderDToUpdate.ActualLoadingD = c.Location1DT;
							}
							if (orderDToUpdate.TruckCLastDispatch == null)
							{
								orderDToUpdate.TruckCLastDispatch = c.TruckC;
							}
							orderDToUpdate.LocationDispatch2 = c.Location1N;
						}
						else if (c.Operation1C == "XH")
						{
							if (orderDToUpdate.ActualDischargeD == null)
							{
								orderDToUpdate.ActualDischargeD = c.Location1DT;
							}
							orderDToUpdate.TruckCReturn = c.TruckC;
							orderDToUpdate.LocationDispatch3 = c.Location1N;
						}
						else if (c.Operation1C == "TR")
						{
							if (orderDToUpdate.ActualPickupReturnD == null)
							{
								orderDToUpdate.ActualPickupReturnD = c.Location1DT;
							}
							orderDToUpdate.TruckCReturn = c.TruckC;
							orderDToUpdate.LocationDispatch3 = c.Location1N;
						}

						//Check for Location2
						if (c.Operation2C == "LR")
						{
							if (orderDToUpdate.ActualPickupReturnD == null)
							{
								orderDToUpdate.ActualPickupReturnD = c.Location2DT;
							}
							orderDToUpdate.LocationDispatch1 = c.Location2N;
						}
						else if (c.Operation2C == "LH")
						{
							if (orderDToUpdate.ActualLoadingD == null)
							{
								orderDToUpdate.ActualLoadingD = c.Location2DT;
							}
							if (orderDToUpdate.TruckCLastDispatch == null)
							{
								orderDToUpdate.TruckCLastDispatch = c.TruckC;
							}
							orderDToUpdate.LocationDispatch2 = c.Location2N;
						}
						else if (c.Operation2C == "XH")
						{
							if (orderDToUpdate.ActualDischargeD == null)
							{
								orderDToUpdate.ActualDischargeD = c.Location2DT;
							}
							orderDToUpdate.TruckCReturn = c.TruckC;
							orderDToUpdate.LocationDispatch3 = c.Location2N;
						}
						else if (c.Operation2C == "TR")
						{
							if (orderDToUpdate.ActualPickupReturnD == null)
							{
								orderDToUpdate.ActualPickupReturnD = c.Location2DT;
							}
							orderDToUpdate.TruckCReturn = c.TruckC;
							orderDToUpdate.LocationDispatch3 = c.Location2N;
						}

						//Check for Location3
						if (c.Operation3C == "LR")
						{
							if (orderDToUpdate.ActualPickupReturnD == null)
							{
								orderDToUpdate.ActualPickupReturnD = c.Location3DT;
							}
							orderDToUpdate.LocationDispatch1 = c.Location3N;
						}
						else if (c.Operation3C == "LH")
						{
							if (orderDToUpdate.ActualLoadingD == null)
							{
								orderDToUpdate.ActualLoadingD = c.Location3DT;
							}
							if (orderDToUpdate.TruckCLastDispatch == null)
							{
								orderDToUpdate.TruckCLastDispatch = c.TruckC;
							}
							orderDToUpdate.LocationDispatch2 = c.Location3N;
						}
						else if (c.Operation3C == "XH")
						{
							if (orderDToUpdate.ActualDischargeD == null)
							{
								orderDToUpdate.ActualDischargeD = c.Location3DT;
							}
							orderDToUpdate.TruckCReturn = c.TruckC;
							orderDToUpdate.LocationDispatch3 = c.Location3N;
						}
						else if (c.Operation3C == "TR")
						{
							if (orderDToUpdate.ActualPickupReturnD == null)
							{
								orderDToUpdate.ActualPickupReturnD = c.Location3DT;
							}
							orderDToUpdate.TruckCReturn = c.TruckC;
							orderDToUpdate.LocationDispatch3 = c.Location3N;
						}
					}

				}

				_orderDRepository.Update(orderDToUpdate);
				SaveDispatch();
			}
		}

		public void UpdateActualDispatchDate(DateTime orderD, string orderNo, int detailNo)
		{
			var dispatches = _dispatchRepository.Query(ord => ord.OrderD == orderD.Date &&
																ord.OrderNo == orderNo &&
																ord.DetailNo == detailNo
														).OrderBy(p => p.DispatchNo);

			var orderDToUpdate = _orderDRepository.Query(ord => ord.OrderD == orderD.Date &&
																ord.OrderNo == orderNo &&
																ord.DetailNo == detailNo
														).FirstOrDefault();

			orderDToUpdate.ActualLoadingD = null;
			orderDToUpdate.ActualDischargeD = null;
			orderDToUpdate.ActualPickupReturnD = null;

			var orderH = _orderHRepository.Query(ord => ord.OrderD == orderD.Date &&
																ord.OrderNo == orderNo
														).FirstOrDefault();

			foreach (var c in dispatches)
			{
				if (orderH.OrderTypeI == "1")
				{
					//Check for Location1
					if (c.Operation1C == "LC")
					{
						if (orderDToUpdate.ActualLoadingD == null)
						{
							orderDToUpdate.ActualLoadingD = c.Location1DT;
						}
					}
					else if (c.Operation1C == "XH")
					{
						if (orderDToUpdate.ActualDischargeD == null)
						{
							orderDToUpdate.ActualDischargeD = c.Location1DT;
						}
					}
					else if (c.Operation1C == "TR")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location1DT;
						}
					}

					//Check for Location2
					if (c.Operation2C == "LC")
					{
						if (orderDToUpdate.ActualLoadingD == null)
						{
							orderDToUpdate.ActualLoadingD = c.Location2DT;
						}
					}
					else if (c.Operation2C == "XH")
					{
						if (orderDToUpdate.ActualDischargeD == null)
						{
							orderDToUpdate.ActualDischargeD = c.Location2DT;
						}
					}
					else if (c.Operation2C == "TR")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location2DT;
						}
					}

					//Check for Location3
					if (c.Operation3C == "LC")
					{
						if (orderDToUpdate.ActualLoadingD == null)
						{
							orderDToUpdate.ActualLoadingD = c.Location3DT;
						}
					}
					else if (c.Operation3C == "XH")
					{
						if (orderDToUpdate.ActualDischargeD == null)
						{
							orderDToUpdate.ActualDischargeD = c.Location3DT;
						}
					}
					else if (c.Operation3C == "TR")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location3DT;
						}
					}
				}
				else if (orderH.OrderTypeI == "0")
				{
					//Check for Location1
					if (c.Operation1C == "LR")
					{
						if (orderDToUpdate.ActualLoadingD == null)
						{
							orderDToUpdate.ActualLoadingD = c.Location1DT;
						}
					}
					else if (c.Operation1C == "LH")
					{
						if (orderDToUpdate.ActualDischargeD == null)
						{
							orderDToUpdate.ActualDischargeD = c.Location1DT;
						}
					}
					else if (c.Operation1C == "HC")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location1DT;
						}
					}
					else if (c.Operation1C == "TR")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location1DT;
						}
					}

					//Check for Location2
					if (c.Operation2C == "LR")
					{
						if (orderDToUpdate.ActualLoadingD == null)
						{
							orderDToUpdate.ActualLoadingD = c.Location2DT;
						}
					}
					else if (c.Operation2C == "LH")
					{
						if (orderDToUpdate.ActualDischargeD == null)
						{
							orderDToUpdate.ActualDischargeD = c.Location2DT;
						}
					}
					else if (c.Operation2C == "HC")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location2DT;
						}
					}
					else if (c.Operation2C == "TR")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location2DT;
						}
					}

					//Check for Location3
					if (c.Operation3C == "LR")
					{
						if (orderDToUpdate.ActualLoadingD == null)
						{
							orderDToUpdate.ActualLoadingD = c.Location3DT;
						}
					}
					else if (c.Operation3C == "LH")
					{
						if (orderDToUpdate.ActualDischargeD == null)
						{
							orderDToUpdate.ActualDischargeD = c.Location3DT;
						}
					}
					else if (c.Operation3C == "HC")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location3DT;
						}
					}
					else if (c.Operation3C == "TR")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location3DT;
						}
					}
				}
				else
				{
					//Check for Location1
					if (c.Operation1C == "LR")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location1DT;
						}
					}
					else if (c.Operation1C == "LH")
					{
						if (orderDToUpdate.ActualLoadingD == null)
						{
							orderDToUpdate.ActualLoadingD = c.Location1DT;
						}
					}
					else if (c.Operation1C == "XH")
					{
						if (orderDToUpdate.ActualDischargeD == null)
						{
							orderDToUpdate.ActualDischargeD = c.Location1DT;
						}
					}
					else if (c.Operation1C == "TR")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location1DT;
						}
					}

					//Check for Location2
					if (c.Operation2C == "LR")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location2DT;
						}
					}
					else if (c.Operation2C == "LH")
					{
						if (orderDToUpdate.ActualLoadingD == null)
						{
							orderDToUpdate.ActualLoadingD = c.Location2DT;
						}
					}
					else if (c.Operation2C == "XH")
					{
						if (orderDToUpdate.ActualDischargeD == null)
						{
							orderDToUpdate.ActualDischargeD = c.Location2DT;
						}
					}
					else if (c.Operation2C == "TR")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location2DT;
						}
					}

					//Check for Location3
					if (c.Operation3C == "LR")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location3DT;
						}
					}
					else if (c.Operation3C == "LH")
					{
						if (orderDToUpdate.ActualLoadingD == null)
						{
							orderDToUpdate.ActualLoadingD = c.Location3DT;
						}
					}
					else if (c.Operation3C == "XH")
					{
						if (orderDToUpdate.ActualDischargeD == null)
						{
							orderDToUpdate.ActualDischargeD = c.Location3DT;
						}
					}
					else if (c.Operation3C == "TR")
					{
						if (orderDToUpdate.ActualPickupReturnD == null)
						{
							orderDToUpdate.ActualPickupReturnD = c.Location3DT;
						}
					}
				}

			}

			_orderDRepository.Update(orderDToUpdate);
			SaveDispatch();
		}

		public void UpdateContainerNo(DateTime orderD, string orderNo, int detailNo, string containerNo)
		{
			var orderDToUpdate = _orderDRepository.Query(ord => ord.OrderD == orderD.Date &&
																ord.OrderNo == orderNo &&
																ord.DetailNo == detailNo
														).FirstOrDefault();
			if (orderDToUpdate != null)
			{
				orderDToUpdate.ContainerNo = containerNo != null ? containerNo.ToUpper() : "";
				_orderDRepository.Update(orderDToUpdate);
				SaveDispatch();
			}
		}

		public void UpdateTrailerC(DateTime orderD, string orderNo, int detailNo, string trailerC)
		{
			var orderDToUpdate = _orderDRepository.Query(ord => ord.OrderD == orderD.Date &&
																ord.OrderNo == orderNo &&
																ord.DetailNo == detailNo
														).FirstOrDefault();
			if (orderDToUpdate != null)
			{
				orderDToUpdate.TrailerC = trailerC;
				_orderDRepository.Update(orderDToUpdate);
				SaveDispatch();
			}
		}
		public void UpdateSealNo(DateTime orderD, string orderNo, int detailNo, string sealNo)
		{
			var orderDToUpdate = _orderDRepository.Query(ord => ord.OrderD == orderD.Date &&
																ord.OrderNo == orderNo &&
																ord.DetailNo == detailNo
														).FirstOrDefault();
			if (orderDToUpdate != null)
			{
				orderDToUpdate.SealNo = sealNo;
				_orderDRepository.Update(orderDToUpdate);
				SaveDispatch();
			}
		}
		public void UpdateDescription(DateTime orderD, string orderNo, int detailNo, string description)
		{
			var orderDToUpdate = _orderDRepository.Query(ord => ord.OrderD == orderD.Date &&
																ord.OrderNo == orderNo &&
																ord.DetailNo == detailNo
														).FirstOrDefault();
			if (orderDToUpdate != null)
			{
				orderDToUpdate.Description = description;
				_orderDRepository.Update(orderDToUpdate);
				SaveDispatch();
			}
		}

		public void UpdateTrailerCWarning(DateTime orderD, string orderNo, int detailNo, string trailerC)
		{
			var orderDToUpdate = _orderDRepository.Query(ord => ord.OrderD == orderD.Date &&
																ord.OrderNo == orderNo &&
																ord.DetailNo == detailNo
														).FirstOrDefault();
			if (orderDToUpdate != null)
			{
				orderDToUpdate.TrailerC = trailerC;
				_orderDRepository.Update(orderDToUpdate);
				SaveDispatch();
			}
		}

		public int CheckTrailerIsUsing(DateTime orderD, string orderNo, int detailNo, string trailerC)
		{
			var container = _orderDRepository.Query(ord => ord.OrderD == orderD.Date &&
																ord.OrderNo == orderNo &&
																ord.DetailNo == detailNo
														).FirstOrDefault();
			var trailer = _trailerRepository.Query(p => p.TrailerC == trailerC).FirstOrDefault();
			var isTrailerUsing = trailer != null && trailer.IsUsing == "1" ? 1 : 0;
			if (container != null)
			{
				if (container.TrailerC == trailerC & isTrailerUsing == 1)
				{
					//is used by this container
					return 2;
				}
				else if (container.TrailerC != trailerC & isTrailerUsing == 1)
				{
					//is used by other contriner
					return 1;
				}
				return 0;
			}
			else
			{
				//is used by other containers
				if (isTrailerUsing == 1)
				{
					return 1;
				}
				return 0;
			}
		}

		public void UpdateActualLoadingD(DateTime orderD, string orderNo, int detailNo, DateTime? actualLoadingD)
		{
			//var orderDToUpdate = _orderDRepository.Query(ord => ord.OrderD == orderD.Date &&
			//													ord.OrderNo == orderNo &&
			//													ord.DetailNo == detailNo
			//											).FirstOrDefault();
			//if (orderDToUpdate != null)
			//{
			//	orderDToUpdate.ActualLoadingD = actualLoadingD;
			//	_orderDRepository.Update(orderDToUpdate);

			//	// cal detain day
			//	var dispatch = _dispatchRepository.GetAllQueryable();
			//	var dispatchList = dispatch.Where(dis => dis.OrderD == orderD.Date &&
			//											 dis.OrderNo == orderNo &&
			//											 dis.DetailNo == detailNo
			//									 ).ToList();

			//	if (dispatchList.Count > 0)
			//	{
			//		var detainDay = 0;
			//		if (actualLoadingD != null)
			//		{
			//			if (orderDToUpdate.ActualDischargeD != null)
			//			{
			//				for (int iloop = 0; iloop < dispatchList.Count; iloop++)
			//				{
			//					if (dispatchList[iloop].ContainerStatus == Constants.LOAD &&
			//						dispatchList[iloop].DispatchStatus == Constants.TRANSPORTED
			//					   )
			//					{
			//						detainDay = Utilities.SubstractTwoDate((DateTime)orderDToUpdate.ActualDischargeD, (DateTime)actualLoadingD);
			//						detainDay = detainDay < 0 ? 0 : detainDay;
			//						dispatchList[iloop].DetainDay = detainDay;
			//						_dispatchRepository.Update(dispatchList[iloop]);
			//						break;
			//					}
			//				}
			//			}
			//			else
			//			{
			//				for (int iloop = 0; iloop < dispatchList.Count; iloop++)
			//				{
			//					if (dispatchList[iloop].ContainerStatus == Constants.LOAD &&
			//						dispatchList[iloop].DispatchStatus == Constants.TRANSPORTED
			//					   )
			//					{
			//						detainDay = Utilities.SubstractTwoDate(DateTime.Now, (DateTime)actualLoadingD);
			//						detainDay = detainDay < 0 ? 0 : detainDay;
			//						dispatchList[iloop].DetainDay = detainDay;
			//						_dispatchRepository.Update(dispatchList[iloop]);
			//						break;
			//					}
			//				}
			//			}
			//		}
			//		else
			//		{
			//			for (int iloop = 0; iloop < dispatchList.Count; iloop++)
			//			{
			//				if (dispatchList[iloop].ContainerStatus == Constants.LOAD &&
			//					dispatchList[iloop].DispatchStatus == Constants.TRANSPORTED
			//				   )
			//				{
			//					dispatchList[iloop].DetainDay = 0;
			//					_dispatchRepository.Update(dispatchList[iloop]);
			//					break;
			//				}
			//			}
			//		}
			//	}

			//	SaveDispatch();
			//}
		}

		public void UpdateActualDischargeD(DateTime orderD, string orderNo, int detailNo, DateTime? actualDischargeD)
		{
			var orderDToUpdate = _orderDRepository.Query(ord => ord.OrderD == orderD.Date &&
																ord.OrderNo == orderNo &&
																ord.DetailNo == detailNo
														).FirstOrDefault();
			if (orderDToUpdate != null)
			{
				orderDToUpdate.ActualDischargeD = actualDischargeD;
				_orderDRepository.Update(orderDToUpdate);

				// update detain of dispatch_d
				var dispatch = _dispatchRepository.GetAllQueryable();
				var dispatchList = dispatch.Where(dis => dis.OrderD == orderD.Date &&
														 dis.OrderNo == orderNo &&
														 dis.DetailNo == detailNo
												 ).ToList();
				var detainDay = 0;
				if (orderDToUpdate.ActualLoadingD != null && orderDToUpdate.ActualDischargeD != null)
				{
					if (dispatchList.Count > 0)
					{
						for (int iloop = 0; iloop < dispatchList.Count; iloop++)
						{
							if (dispatchList[iloop].ContainerStatus == Constants.LOAD)
							{
								detainDay = Utilities.SubstractTwoDate((DateTime)orderDToUpdate.ActualDischargeD, (DateTime)orderDToUpdate.ActualLoadingD);
								detainDay = detainDay < 0 ? 0 : detainDay;
								dispatchList[iloop].DetainDay = detainDay;
								_dispatchRepository.Update(dispatchList[iloop]);
								break;
							}
						}
					}
				}
				else if (orderDToUpdate.ActualLoadingD != null && orderDToUpdate.ActualDischargeD == null)
				{
					if (dispatchList.Count > 0)
					{
						for (int iloop = 0; iloop < dispatchList.Count; iloop++)
						{
							if (dispatchList[iloop].ContainerStatus == Constants.LOAD &&
								dispatchList[iloop].DispatchStatus == Constants.TRANSPORTED
							   )
							{
								detainDay = Utilities.SubstractTwoDate(DateTime.Now, (DateTime)orderDToUpdate.ActualLoadingD);
								detainDay = detainDay < 0 ? 0 : detainDay;
								dispatchList[iloop].DetainDay = detainDay;
								_dispatchRepository.Update(dispatchList[iloop]);
								break;
							}
						}
					}
				}
				else
				{
					if (dispatchList.Count > 0)
					{
						for (int iloop = 0; iloop < dispatchList.Count; iloop++)
						{
							if (dispatchList[iloop].ContainerStatus == Constants.LOAD
							   )
							{
								dispatchList[iloop].DetainDay = 0;
								_dispatchRepository.Update(dispatchList[iloop]);
								break;
							}
						}
					}
				}

				SaveDispatch();
			}
		}

		public void UpdateActualLoadingDischargeD(DateTime orderD, string orderNo, int detailNo, DateTime? actualLoadingD, DateTime? actualDischargeD)
		{
			//var orderDToUpdate = _orderDRepository.Query(ord => ord.OrderD == orderD.Date &&
			//													ord.OrderNo == orderNo &&
			//													ord.DetailNo == detailNo
			//											).FirstOrDefault();
			//if (orderDToUpdate != null)
			//{
			//	// update actual loading date
			//	orderDToUpdate.ActualLoadingD = actualLoadingD;
			//	_orderDRepository.Update(orderDToUpdate);
			//	// update actual discharge date
			//	orderDToUpdate.ActualDischargeD = actualDischargeD;
			//	if (actualDischargeD != null)
			//	{
			//		orderDToUpdate.RevenueD = actualDischargeD;
			//	}
			//	_orderDRepository.Update(orderDToUpdate);

			//	// cal detain day
			//	var dispatch = _dispatchRepository.GetAllQueryable();
			//	var dispatchList = dispatch.Where(dis => dis.OrderD == orderD.Date &&
			//											 dis.OrderNo == orderNo &&
			//											 dis.DetailNo == detailNo
			//									 ).ToList();

			//	if (dispatchList.Count > 0)
			//	{
			//		if (actualLoadingD != null)
			//		{
			//			var detainDay = 0;
			//			if (orderDToUpdate.ActualDischargeD != null)
			//			{
			//				for (int iloop = 0; iloop < dispatchList.Count; iloop++)
			//				{
			//					if (dispatchList[iloop].ContainerStatus == Constants.LOAD)
			//					{
			//						if (dispatchList[iloop].DispatchStatus == Constants.TRANSPORTED ||
			//							dispatchList[iloop].DispatchStatus == Constants.CONFIRMED
			//							)
			//						{
			//							detainDay = Utilities.SubstractTwoDate((DateTime)orderDToUpdate.ActualDischargeD, (DateTime)actualLoadingD);
			//							detainDay = detainDay < 0 ? 0 : detainDay;
			//							dispatchList[iloop].DetainDay = detainDay;
			//							_dispatchRepository.Update(dispatchList[iloop]);
			//						}
			//						else
			//						{
			//							dispatchList[iloop].DetainDay = 0;
			//							_dispatchRepository.Update(dispatchList[iloop]);
			//						}
			//						break;
			//					}
			//				}
			//			}
			//			else
			//			{
			//				for (int iloop = 0; iloop < dispatchList.Count; iloop++)
			//				{
			//					if (dispatchList[iloop].ContainerStatus == Constants.LOAD)
			//					{
			//						if (dispatchList[iloop].DispatchStatus == Constants.TRANSPORTED ||
			//							dispatchList[iloop].DispatchStatus == Constants.CONFIRMED)
			//						{
			//							detainDay = Utilities.SubstractTwoDate(DateTime.Now, (DateTime)actualLoadingD);
			//							detainDay = detainDay < 0 ? 0 : detainDay;
			//							dispatchList[iloop].DetainDay = detainDay;
			//							_dispatchRepository.Update(dispatchList[iloop]);
			//						}
			//						else
			//						{
			//							dispatchList[iloop].DetainDay = 0;
			//							_dispatchRepository.Update(dispatchList[iloop]);
			//						}
			//						break;
			//					}
			//				}
			//			}
			//		}
			//		else
			//		{
			//			for (int iloop = 0; iloop < dispatchList.Count; iloop++)
			//			{
			//				if (dispatchList[iloop].ContainerStatus == Constants.LOAD &&
			//					dispatchList[iloop].DispatchStatus == Constants.TRANSPORTED
			//				   )
			//				{
			//					dispatchList[iloop].DetainDay = 0;
			//					_dispatchRepository.Update(dispatchList[iloop]);
			//					break;
			//				}
			//			}
			//		}
			//	}

			//	SaveDispatch();
			//}
		}

		public void UpdateDispatchStatus(DateTime orderD, string orderNo, int detailNo, int dispatchNo, string status)
		{
			var orderDObject = _orderDRepository.Query(dis => dis.OrderD == orderD.Date &&
															  dis.OrderNo == orderNo &&
															  dis.DetailNo == detailNo
												).FirstOrDefault();

			var dispatchToUpdate = _dispatchRepository.Query(dis => dis.OrderD == orderD.Date &&
																	dis.OrderNo == orderNo &&
																	dis.DetailNo == detailNo &&
																	dis.DispatchNo == dispatchNo
															).FirstOrDefault();
			if (dispatchToUpdate != null)
			{
				dispatchToUpdate.DispatchStatus = status;
				if (status == "0" || status == "1")
				{
					dispatchToUpdate.IsTransported1 = false;
					dispatchToUpdate.IsTransported2 = false;
					dispatchToUpdate.IsTransported3 = false;
				}
				else if (status == "2" || status == "3")
				{
					dispatchToUpdate.IsTransported1 = true;
					dispatchToUpdate.IsTransported2 = true;
					dispatchToUpdate.IsTransported3 = true;
				}
				// cal detain day
				//if (orderDObject != null &&
				//	dispatchToUpdate.ContainerStatus == Constants.LOAD &&
				//	(status == Constants.TRANSPORTED || status == Constants.CONFIRMED)
				//	)
				//{
				//	// update detain of dispatch_d
				//	var detainDay = 0;
				//	if (orderDObject.ActualLoadingD != null && orderDObject.ActualDischargeD != null)
				//	{
				//		detainDay = Utilities.SubstractTwoDate((DateTime)orderDObject.ActualDischargeD, (DateTime)orderDObject.ActualLoadingD);
				//		detainDay = detainDay < 0 ? 0 : detainDay;
				//		dispatchToUpdate.DetainDay = detainDay;
				//	}
				//	else if (orderDObject.ActualLoadingD != null && orderDObject.ActualDischargeD == null)
				//	{
				//		detainDay = Utilities.SubstractTwoDate(DateTime.Now, (DateTime)orderDObject.ActualLoadingD);
				//		detainDay = detainDay < 0 ? 0 : detainDay;
				//		dispatchToUpdate.DetainDay = Utilities.SubstractTwoDate(DateTime.Now, (DateTime)orderDObject.ActualLoadingD);
				//	}
				//	else
				//	{
				//		dispatchToUpdate.DetainDay = 0;
				//	}
				//}
				//else if (orderDObject != null && dispatchToUpdate.ContainerStatus == Constants.LOAD &&
				//		 (status != Constants.TRANSPORTED && status != Constants.CONFIRMED))
				//{
				//	dispatchToUpdate.DetainDay = 0;
				//}

				_dispatchRepository.Update(dispatchToUpdate);
				SaveDispatch();
			}
		}

		public DriverDispatchViewModel GetDriverDispatchDatatable(DateTime transportD, string truckC, string driverC)
		{
			DriverDispatchViewModel driverDispatchViewModel = new DriverDispatchViewModel();
			var dispatch = _dispatchRepository.GetAllQueryable();
			// searching
			dispatch = dispatch.Where(dis => dis.TransportD == transportD.Date &&
												 dis.DriverC == driverC &&
												 dis.TruckC == truckC
											);

			var dispatchList = dispatch.OrderBy("DispatchOrder asc").ToList();
			driverDispatchViewModel.DriverDispatchList = Mapper.Map<List<Dispatch_D>, List<DispatchViewModel>>(dispatchList);
			for (var i = 0; i < driverDispatchViewModel.DriverDispatchList.Count; i++)
			{
				DateTime oD = driverDispatchViewModel.DriverDispatchList[i].OrderD;
				string oNo = driverDispatchViewModel.DriverDispatchList[i].OrderNo;
				int dNo = driverDispatchViewModel.DriverDispatchList[i].DetailNo;
				var orderD =
					_orderDRepository.Query(o => o.OrderD == oD && o.OrderNo == oNo && o.DetailNo == dNo)
						.FirstOrDefault();
				driverDispatchViewModel.DriverDispatchList[i].NetWeight = orderD != null ? orderD.NetWeight : null;
			}
			if (driverDispatchViewModel.DriverDispatchList != null && driverDispatchViewModel.DriverDispatchList.Count > 0)
			{
				foreach (var dispatchItem in driverDispatchViewModel.DriverDispatchList)
				{
					if (dispatchItem.OrderTypeI == Constants.EXP)
					{
						dispatchItem.OrderTypeN = Constants.EXPNAME;
					}
					else if (dispatchItem.OrderTypeI == Constants.IMP)
					{
						dispatchItem.OrderTypeN = Constants.IMPNAME;
					}
					else
					{
						dispatchItem.OrderTypeN = Constants.ETCNAME;
					}

					// set date display on grid
					var orderD = dispatchItem.OrderD;
					var orderNo = dispatchItem.OrderNo;
					var orderH = _orderHRepository.Query(o => o.OrderD == orderD && o.OrderNo == orderNo).FirstOrDefault();
					dispatchItem.LocationDriverDispatchDT = null;
					if (orderH != null)
					{
						if (orderH.DischargeDT != null)
						{
							dispatchItem.LocationDriverDispatchDT = orderH.DischargeDT;
						}
						else if (orderH.StopoverDT != null)
						{
							dispatchItem.LocationDriverDispatchDT = orderH.StopoverDT;
						}
						else if (orderH.LoadingDT != null)
						{
							dispatchItem.LocationDriverDispatchDT = orderH.LoadingDT;
						}
					}

					// set containerNo
					var detailNo = dispatchItem.DetailNo;
					var orderDObject = _orderDRepository.Query(ord => ord.OrderD == orderD.Date &&
															   ord.OrderNo == orderNo &&
															   ord.DetailNo == detailNo
															  ).FirstOrDefault();
					dispatchItem.ContainerNo = "";
					if (orderDObject != null)
					{
						dispatchItem.ContainerNo = orderDObject.ContainerNo;
					}
				}
			}

			return driverDispatchViewModel;
		}

		public List<TransportConfirmDispatchViewModel> GetTransConfirmDispatchList(DateTime orderD, string orderNo,
			int detailNo)
		{
			var dispatches = _dispatchRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo);
			if (dispatches.Any())
			{
				var dispatchsOrdered = dispatches.OrderBy("DispatchNo ascending").ToList();
				var transportConfirmDispatchList = (from p in dispatchsOrdered
													join t in _truckRepository.GetAllQueryable() on p.TruckC equals t.TruckC into pt
													from t in pt.DefaultIfEmpty()
													join d in _driverRepository.GetAllQueryable() on p.DriverC equals d.DriverC into ptd
													from d in ptd.DefaultIfEmpty()
													join f in _driverRepository.GetAllQueryable() on p.AssistantC equals f.DriverC into t10
													from f in t10.DefaultIfEmpty()
													join n in _partnerRepository.GetAllQueryable() on new { p.PartnerMainC, p.PartnerSubC } equals new { n.PartnerMainC, n.PartnerSubC } into ptdn
													from n in ptdn.DefaultIfEmpty()
													join m in _departmentRepository.GetAllQueryable() on p.TransportDepC equals m.DepC into ptdnm
													from m in ptdnm.DefaultIfEmpty()
													select new TransportConfirmDispatchViewModel()
													{
														OrderD = p.OrderD,
														OrderNo = p.OrderNo,
														DetailNo = p.DetailNo,
														DispatchNo = p.DispatchNo,
														TransportD = p.TransportD,
														DispatchI = p.DispatchI,
														TruckC = p.TruckC,
														RegisteredNo = !string.IsNullOrEmpty(p.RegisteredNo) ? p.RegisteredNo : (t != null ? t.RegisteredNo : ""),
														AcquisitionD = t != null ? t.AcquisitionD : null,
														DisusedD = t != null ? t.DisusedD : null,
														DriverC = p.DriverC,
														FirstN = d != null ? d.FirstN : "",
														LastN = d != null ? d.LastN : "",
														AssistantC = p.AssistantC,
														RetiredD = d != null ? d.RetiredD : null,
														PartnerMainC = p.PartnerMainC,
														PartnerSubC = p.PartnerSubC,
														PartnerN = n != null ? n.PartnerN : "",
														OrderTypeI = p.OrderTypeI,
														DispatchOrder = p.DispatchOrder,
														ContainerStatus = p.ContainerStatus,
														DispatchStatus = p.DispatchStatus,
														Location1C = p.Location1C,
														Location1N = p.Location1N,
														Location1DT = p.Location1DT,
														Location2C = p.Location2C,
														Location2N = p.Location2N,
														Location2DT = p.Location2DT,
														Location3C = p.Location3C,
														Location3N = p.Location3N,
														Location3DT = p.Location3DT,
														TransportFee = p.TransportFee,
														PartnerFee = p.PartnerFee,
														IncludedExpense = p.IncludedExpense,
														DriverAllowance = p.DriverAllowance,
														Expense = p.Expense,
														PartnerExpense = p.PartnerExpense,
														PartnerSurcharge = p.PartnerSurcharge,
														PartnerDiscount = p.PartnerDiscount,
														PartnerTaxAmount = p.PartnerTaxAmount,
														InvoiceD = (p.DispatchI == "1" && p.InvoiceD == null) ? p.TransportD : p.InvoiceD,
														TransportDepC = p.TransportDepC,
														TransportDepN = m != null ? m.DepN : "",
														ApproximateDistance = p.ApproximateDistance ?? 0,
														ActualDistance = p.ActualDistance,
														FuelConsumption = p.FuelConsumption ?? 0,
														ActualFuel = p.ActualFuel ?? 0,
														InstructionNo = p.InstructionNo,
														Description = p.Description,
														Operation1C = p.Operation1C,
														Operation2C = p.Operation2C,
														Operation3C = p.Operation3C,
														AllowanceOfDriver = p.AllowanceOfDriver ?? 0,
														TotalKm = p.TotalKm ?? 0,
														TotalFuel = p.TotalFuel ?? 0,
														LossFuelRate = p.LossFuelRate ?? 0,
														VirtualDataNoGoods = p.VirtualDataNoGoods ?? 0,
														VirtualDataHaveGoods = p.VirtualDataHaveGoods ?? 0,
                                                        WayType = p.WayType,
                                                        OrderImageKey =  p.OrderImageKey,
                                                        ImageCount =  p.ImageCount
													}).ToList();

				return transportConfirmDispatchList;
			}
			return null;
		}

		private void UpdateLocationOrderDetail(DateTime orderD, string orderNo, int detailNo)
		{
			var orderDetail = _orderDRepository.Query(or => or.OrderD == orderD && or.OrderNo == orderNo && or.DetailNo == detailNo).FirstOrDefault();
			if (orderDetail != null)
			{
				var dispatchList = _dispatchRepository.Query(d => d.OrderD == orderD.Date &&
															 d.OrderNo == orderNo &&
															 d.DetailNo == detailNo
													   ).ToList();
				//update 3 location and TruckNo null
				orderDetail.LocationDispatch1 = string.Empty;
				orderDetail.LocationDispatch2 = string.Empty;
				orderDetail.LocationDispatch3 = string.Empty;
				orderDetail.TruckCLastDispatch = string.Empty;
				orderDetail.LastDDispatch = null;
				if (dispatchList.Any())
				{
					foreach (var dispatchItem in dispatchList)
					{
						if (dispatchItem.OrderTypeI == "0")
						{
							if (dispatchItem.Operation1C == "LR")
							{
								orderDetail.LocationDispatch1 = dispatchItem.Location1N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location1DT;
							}
							if (dispatchItem.Operation2C == "LR")
							{
								orderDetail.LocationDispatch1 = dispatchItem.Location2N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location2DT;
							}
							if (dispatchItem.Operation3C == "LR")
							{
								orderDetail.LocationDispatch1 = dispatchItem.Location3N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location3DT;
							}

							if (dispatchItem.Operation1C == "LH")
							{
								orderDetail.LocationDispatch2 = dispatchItem.Location1N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location1DT;
							}
							if (dispatchItem.Operation2C == "LH")
							{
								orderDetail.LocationDispatch2 = dispatchItem.Location2N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location2DT;
							}
							if (dispatchItem.Operation3C == "LH")
							{
								orderDetail.LocationDispatch2 = dispatchItem.Location3N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location3DT;
							}

							if (dispatchItem.Operation1C == "HC")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location1N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location1DT;
							}
							if (dispatchItem.Operation2C == "HC")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location2N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location2DT;
							}
							if (dispatchItem.Operation3C == "HC")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location3N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location3DT;
							}
							//exceptions, cannot get goods -> return empty
							if (dispatchItem.Operation1C == "TR")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location1N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location1DT;
							}
							if (dispatchItem.Operation2C == "TR")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location2N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location2DT;
							}
							if (dispatchItem.Operation3C == "TR")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location3N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location3DT;
							}
							//end exceoptions
						}
						else if (dispatchItem.OrderTypeI == "1")
						{
							if (dispatchItem.Operation1C == "LC")
							{
								orderDetail.LocationDispatch1 = dispatchItem.Location1N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location1DT;
							}
							if (dispatchItem.Operation2C == "LC")
							{
								orderDetail.LocationDispatch1 = dispatchItem.Location2N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location2DT;
							}
							if (dispatchItem.Operation3C == "LC")
							{
								orderDetail.LocationDispatch1 = dispatchItem.Location3N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location3DT;
							}

							if (dispatchItem.Operation1C == "XH")
							{
								orderDetail.LocationDispatch2 = dispatchItem.Location1N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location1DT;
							}
							if (dispatchItem.Operation2C == "XH")
							{
								orderDetail.LocationDispatch2 = dispatchItem.Location2N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location2DT;
							}
							if (dispatchItem.Operation3C == "XH")
							{
								orderDetail.LocationDispatch2 = dispatchItem.Location3N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location3DT;
							}

							if (dispatchItem.Operation1C == "TR")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location1N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location1DT;
							}
							if (dispatchItem.Operation2C == "TR")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location2N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location2DT;
							}
							if (dispatchItem.Operation3C == "TR")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location3N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location3DT;
							}
						}
						else
						{
							if (dispatchItem.Operation1C == "LR")
							{
								orderDetail.LocationDispatch1 = dispatchItem.Location1N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location1DT;
							}
							if (dispatchItem.Operation2C == "LR")
							{
								orderDetail.LocationDispatch1 = dispatchItem.Location2N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location2DT;
							}
							if (dispatchItem.Operation3C == "LR")
							{
								orderDetail.LocationDispatch1 = dispatchItem.Location3N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location3DT;
							}

							if (dispatchItem.Operation1C == "LH")
							{
								orderDetail.LocationDispatch2 = dispatchItem.Location1N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location1DT;
							}
							if (dispatchItem.Operation2C == "LH")
							{
								orderDetail.LocationDispatch2 = dispatchItem.Location2N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location2DT;
							}
							if (dispatchItem.Operation3C == "LH")
							{
								orderDetail.LocationDispatch2 = dispatchItem.Location3N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location3DT;
							}

							if (dispatchItem.Operation1C == "XH")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location1N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location1DT;
							}
							if (dispatchItem.Operation2C == "XH")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location2N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location2DT;
							}
							if (dispatchItem.Operation3C == "XH")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location3N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location3DT;
							}

							if (dispatchItem.Operation1C == "TR")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location1N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location1DT;
							}
							if (dispatchItem.Operation2C == "TR")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location2N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location2DT;
							}
							if (dispatchItem.Operation3C == "TR")
							{
								orderDetail.LocationDispatch3 = dispatchItem.Location3N;
								orderDetail.TruckCLastDispatch = dispatchItem.TruckC;
								orderDetail.LastDDispatch = dispatchItem.Location3DT;
							}
						}
					}
				}
				_orderDRepository.Update(orderDetail);
				SaveDispatch();
			}
		}

		public int DeleteDispatchDetail(DateTime orderD, string orderNo, int detailNo, int dispatchNo, bool isConfirmedDeleting)
		{
			var dispatchDel = _dispatchRepository.Query(d => d.OrderD == orderD.Date &&
															 d.OrderNo == orderNo &&
															 d.DetailNo == detailNo &&
															 d.DispatchNo == dispatchNo
													   ).FirstOrDefault();

			//if (dispatchDel == null)
			//{
			//	return;
			//}

			var isDeletedDispatch = IsDeleteDispatch(dispatchDel);

			if (isDeletedDispatch == Convert.ToInt32(DeleteLevel.NotDeleted) ||
				(isDeletedDispatch == Convert.ToInt32(DeleteLevel.NotDeletedAndWarning) && !isConfirmedDeleting) ||
				(isDeletedDispatch == Convert.ToInt32(DeleteLevel.Deleted) && !isConfirmedDeleting))
			{
				return isDeletedDispatch;
			}

			// delete Expense_D
			var expenseDDel = _expenseDetailRepository.Query(e => e.OrderD == orderD.Date &&
																	 e.OrderNo == orderNo &&
																	 e.DetailNo == detailNo &&
																	 e.DispatchNo == dispatchNo
																).FirstOrDefault();
			if (expenseDDel != null)
			{
				_expenseDetailRepository.Delete(expenseDDel);
			}

			// delete Surcharge_D
			var surchargeDDel = _surchargeDetailRepository.Query(s => s.OrderD == orderD.Date &&
																	  s.OrderNo == orderNo &&
																	  s.DetailNo == detailNo &&
																	  s.DispatchNo == dispatchNo
																).FirstOrDefault();
			if (surchargeDDel != null)
			{
				_surchargeDetailRepository.Delete(surchargeDDel);
			}

			// delete Allowance_D
			var allowanceDDel = _allowanceDetailRepository.Query(a => a.OrderD == orderD.Date &&
																	  a.OrderNo == orderNo &&
																	  a.DetailNo == detailNo &&
																	  a.DispatchNo == dispatchNo
																).FirstOrDefault();
			if (allowanceDDel != null)
			{
				_allowanceDetailRepository.Delete(allowanceDDel);
			}


			// update Order_D
			var orderDetail = _orderDRepository.Query(o => o.OrderD == orderD.Date &&
													  o.OrderNo == orderNo &&
													  o.DetailNo == detailNo
												).FirstOrDefault();

			if (orderDetail != null)
			{
				// Lan 26.02.2015 Fix Bug No 213
				//Không tính lại số tiền Amount khi xóa Dispatch
				// update Amount
				//if (dispatchDel.TransportFee != null)
				//{
				//	orderDetail.Amount = orderDetail.Amount - dispatchDel.TransportFee;
				//}

				//Lan 2017.08.22
				//update ActualLoadingD, ActualDischargeD, ActualPickupReturnD
				if (dispatchNo == 1)
				{
					orderDetail.ActualLoadingD = null;
					orderDetail.ActualDischargeD = null;
					orderDetail.ActualPickupReturnD = null;
				}
				// update TotalExpense
				if (dispatchDel.Expense != null)
				{
					orderDetail.TotalExpense -= dispatchDel.Expense;
				}

				// update TotalAmount
				//orderDetail.TotalAmount = orderDetail.Amount + orderDetail.TotalExpense + orderDetail.CustomerSurcharge - orderDetail.CustomerDiscount;
				orderDetail.TotalAmount = orderDetail.Amount + orderDetail.TotalExpense + orderDetail.CustomerSurcharge;

				decimal? totalPartnerAmount = 0;
				// update PartnerAmount
				if (dispatchDel.PartnerFee != null)
				{
					orderDetail.PartnerAmount = orderDetail.PartnerAmount - dispatchDel.PartnerFee;
					totalPartnerAmount += dispatchDel.PartnerFee;
				}

				// update PartnerExpense
				if (dispatchDel.PartnerExpense != null)
				{
					orderDetail.PartnerExpense = orderDetail.PartnerExpense - dispatchDel.PartnerExpense;
				}

				// update PartnerSurcharge
				if (dispatchDel.PartnerSurcharge != null)
				{
					orderDetail.PartnerSurcharge = orderDetail.PartnerSurcharge - dispatchDel.PartnerSurcharge;
					totalPartnerAmount += dispatchDel.PartnerSurcharge;
				}

				// update PartnerDiscount
				if (dispatchDel.PartnerDiscount != null)
				{
					orderDetail.PartnerDiscount = orderDetail.PartnerDiscount - dispatchDel.PartnerDiscount;
				}

				// update TotalPartnerAmount
				orderDetail.TotalPartnerAmount = orderDetail.PartnerAmount +
												 orderDetail.PartnerExpense +
												 orderDetail.PartnerSurcharge +
												 orderDetail.PartnerDiscount;

				// update TotalCost
				if (dispatchDel.IncludedExpense != null)
				{
					orderDetail.TotalCost = orderDetail.TotalCost - dispatchDel.IncludedExpense;
				}

				// update TotalDriverAllowance
				if (dispatchDel.DriverAllowance != null)
				{
					orderDetail.TotalDriverAllowance = orderDetail.TotalDriverAllowance - dispatchDel.DriverAllowance;
				}

				// update TaxAmount
				// Nếu điều chỉnh có Surcharge cho từng Dispatch thì uncomment
				//orderDetail.TaxAmount = 0;
				//var orderH = _orderHRepository.Query(o => o.OrderD == orderD.Date &&
				//										  o.OrderNo == orderNo
				//									).FirstOrDefault();

				//if (orderH != null)
				//{
				//	var customerMainC = orderH.CustomerMainC;
				//	var customerSubC = orderH.CustomerSubC;
				//	if (!string.IsNullOrEmpty(customerMainC) && !string.IsNullOrEmpty(orderH.CustomerSubC))
				//	{
				//		var customerSettlementD = _customerSettlementRepository.Query(c => c.CustomerMainC == customerMainC &&
				//																		   c.CustomerSubC == customerSubC &&
				//																		   c.ApplyD <= orderDetail.RevenueD
				//																	);
				//		customerSettlementD = customerSettlementD.OrderBy("ApplyD Desc");
				//		var customerSettlementDList = customerSettlementD.ToList();

				//		if (customerSettlementDList != null && customerSettlementDList.Count > 0)
				//		{
				//			if (customerSettlementDList[0].TaxRate != null)
				//			{
				//				orderDetail.TaxAmount = Utilities.CalByMethodRounding((decimal)(orderDetail.Amount + orderDetail.CustomerSurcharge) * customerSettlementDList[0].TaxRate) / 100, customerSettlementDList[0].TaxRoundingI);
				//			}

				//		}
				//	}
				//}

				// update PartnerTaxAmount
				var partnerMainC = dispatchDel.PartnerMainC;
				var partnerSubC = dispatchDel.PartnerSubC;
				var partnerInvoiceD = dispatchDel.InvoiceD;
				if (!string.IsNullOrEmpty(partnerMainC) && !string.IsNullOrEmpty(partnerSubC))
				{
					var partnerSettlementD = _partnerSettlementRepository.Query(p => p.PartnerMainC == partnerMainC &&
																					 p.PartnerSubC == partnerSubC &&
																					 p.ApplyD <= partnerInvoiceD
																					);
					partnerSettlementD = partnerSettlementD.OrderBy("ApplyD desc");
					var partnerSettlementDList = partnerSettlementD.ToList();

					if (partnerSettlementDList != null && partnerSettlementDList.Count > 0)
					{
						if (partnerSettlementDList[0].TaxRate != null)
						{
							orderDetail.PartnerTaxAmount -= Utilities.CalByMethodRounding((decimal)(totalPartnerAmount * partnerSettlementDList[0].TaxRate) / 100, partnerSettlementDList[0].TaxRoundingI);
						}
					}
				}

				_orderDRepository.Update(orderDetail);
			}

			_dispatchRepository.Delete(dispatchDel);

			var fuelConsumptionD =
				_fuelConsumptionDetailRepository.Query(
					p => p.OrderD == orderD & p.OrderNo == orderNo & p.DetailNo == detailNo & p.DispatchNo == dispatchNo).FirstOrDefault();
			if (fuelConsumptionD != null)
			{
				_fuelConsumptionDetailRepository.Delete(fuelConsumptionD);
			}

			SaveDispatch();
			//Update container actual dispatch
			UpdateOrderDInfo(orderD, orderNo, detailNo);
			UpdateActualDispatchDate(orderD, orderNo, detailNo);
			//UpdateLocationOrderDetail(orderD, orderNo, detailNo);
			return isDeletedDispatch;
		}

		public int CheckDispatchDeleting(DateTime orderD, string orderNo, int detailNo, int dispatchNo)
		{
			var containerDispatche = _dispatchRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo && p.DispatchNo == dispatchNo).FirstOrDefault();

			return IsDeleteDispatch(containerDispatche);
		}

		private int IsDeleteDispatch(Dispatch_D dispatch)
		{
			//toBeDeleted will have 3 levels:
			// "0": can't deleted
			// "1": can deleted but warning
			// "2": can deleted and not warning
			var toBeDeleted = Convert.ToInt32(DeleteLevel.NotDeleted);

			if (dispatch != null)
			{
				toBeDeleted = Convert.ToInt32(DeleteLevel.Deleted);
				if (dispatch.DispatchStatus == Convert.ToInt32(DispatchStatus.Transported).ToString() ||
					dispatch.DispatchStatus == Convert.ToInt32(DispatchStatus.Confirmed).ToString())
				{
					toBeDeleted = Convert.ToInt32(DeleteLevel.NotDeleted);
				}

				return toBeDeleted;
			}

			//if dispatche doesn't have any value, can deleted and not warning
			return Convert.ToInt32(DeleteLevel.Deleted); ;
		}

		public TruckListViewModel GetTruckList(DateTime transportD, string depC, string searchtruckC, bool notdispatchgoStatus, bool notdispatchbackStatus,
			bool dispatchgoStatus, bool dispatchbackStatus, bool otherStatus)
		{
			TruckListViewModel truckListViewModel = new TruckListViewModel();
			// get Truck_M
			var truck = from a in _truckRepository.GetAllQueryable()
				join b in _driverRepository.GetAllQueryable() on new {a.DriverC}
					equals new {b.DriverC} into t1
				from b in t1.DefaultIfEmpty()
				where ((a.DisusedD == null || (a.DisusedD != null && a.DisusedD >= transportD.Date)) &&
				       (a.AcquisitionD == null || (a.AcquisitionD != null && a.AcquisitionD <= transportD.Date)) &&
				       (depC == null || depC == "" || depC == "0" || a.DepC == depC) &&
				       a.PartnerI == "0" && a.IsActive == Constants.ACTIVE)
				select new TruckInfo()
				{
					TruckC = a.TruckC,
					RegisteredNo = a.RegisteredNo,
					DriverC = "",
					DriverN = "",
					AssistantC = "",
					AssistantN = "",
					DriverFirstN = "",
					DriverC2 = (a.DriverC ?? ""),
					DriverN2 = b != null ? b.LastN + " " + b.FirstN : "",
					DriverFirstN2 = b.FirstN,
					AssistantC2 = (a.AssistantC ?? ""),
					AssistantN2 = "",
					DispatchOrder = null,
					Status =
						a.StatusFromD != null && a.StatusToD != null
							? (a.StatusFromD <= transportD.Date && transportD.Date <= a.StatusToD ? a.Status : "")
							: (a.StatusFromD != null && a.StatusToD == null
								? (a.StatusFromD <= transportD.Date ? a.Status : "")
								: (a.StatusFromD == null && a.StatusToD != null ? (transportD.Date <= a.StatusToD ? a.Status : "") : ""))
				};

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var truckList = truck.OrderBy("TruckC asc, DriverC2 asc").ToList();

			// get Dispatch
			var dispatch = from a in _dispatchRepository.GetAllQueryable()
				join b in _truckRepository.GetAllQueryable() on new {a.TruckC}
					equals new {b.TruckC} into t1
				from b in t1.DefaultIfEmpty()
				join c in _driverRepository.GetAllQueryable() on new {a.DriverC}
					equals new {c.DriverC} into t2
				from c in t2.DefaultIfEmpty()
				join f in _driverRepository.GetAllQueryable() on a.AssistantC equals f.DriverC into t10
				from f in t10.DefaultIfEmpty()
				join d in _orderHRepository.GetAllQueryable() on new {a.OrderD, a.OrderNo}
					equals new {d.OrderD, d.OrderNo} into t3
				from d in t3.DefaultIfEmpty()
				join o1 in _operationRepository.GetAllQueryable() on a.Operation1C equals o1.OperationC into op1
				from o1 in op1.DefaultIfEmpty()
				join o2 in _operationRepository.GetAllQueryable() on a.Operation2C equals o2.OperationC into op2
				from o2 in op2.DefaultIfEmpty()
				join o3 in _operationRepository.GetAllQueryable() on a.Operation3C equals o3.OperationC into op3
				from o3 in op3.DefaultIfEmpty()
				where
					((a.TransportD == transportD ||
					  (a.TransportD != transportD &&
					   (a.Location1DT <= transportD &&
					    ((a.Location3DT != null && a.Location3DT >= transportD) ||
					     (a.Location3DT == null && (a.Location2DT != null && a.Location2DT >= transportD)))))) &&
					 !string.IsNullOrEmpty(a.TruckC) &&
					  (depC == null || depC == "" || depC == "0" || d.OrderDepC == depC) &&
					  a.TruckC != null && a.TruckC != "" && b.PartnerI == "0")
				select new DispatchViewModel()
				{
					OrderD = a.OrderD,
					OrderNo = a.OrderNo,
					DetailNo = a.DetailNo,
					DispatchNo = a.DispatchNo,
					TruckC = a.TruckC,
					RegisteredNo = b.RegisteredNo,
					DriverC = a.DriverC,
					DriverN = c.LastN + " " + c.FirstN,
					AssistantC = a.AssistantC,
					AssistantN = f.LastN + " " + f.FirstN,
					DriverFirstN = c.FirstN,
					DispatchOrder = a.DispatchOrder,
					Location1C = a.Location1C,
					Location1N = a.Location1N,
					Location1DT = a.Location1DT,
					Operation1N = o1.OperationN,
					Location2C = a.Location2C,
					Location2N = a.Location2N,
					Location2DT = a.Location2DT,
					Operation2N = o2.OperationN,
					Location3C = a.Location3C,
					Location3N = a.Location3N,
					Location3DT = a.Location3DT,
					Operation3N = o3.OperationN,
					ContainerStatus = a.ContainerStatus
				};
			var dispatchList = dispatch.OrderBy("TruckC asc, DriverC asc, DispatchOrder asc").ToList();

			#region get dispatch info

			// set info from dispatch
			if (dispatchList.Count > 0)
			{
				var oldKey = "";
				for (var iloop = 0; iloop < dispatchList.Count; iloop++)
				{
					var newKey = dispatchList[iloop].DriverC + "_" + dispatchList[iloop].TruckC;
					if (oldKey != newKey)
					{
						oldKey = newKey;
						var truckInfo = new TruckInfo();

						truckInfo.TruckC = dispatchList[iloop].TruckC;
						truckInfo.RegisteredNo = dispatchList[iloop].RegisteredNo;
						truckInfo.DriverC = dispatchList[iloop].DriverC;
						truckInfo.DriverN = dispatchList[iloop].DriverN;
						truckInfo.AssistantC = dispatchList[iloop].AssistantC;
						truckInfo.AssistantN = dispatchList[iloop].AssistantN;
						truckInfo.DriverFirstN = dispatchList[iloop].DriverFirstN;

						// set dispatch info
						truckInfo.DispatchInfoList = new List<DispatchInfo>();
						var dispatchInfo = new DispatchInfo();

						dispatchInfo.OrderD = dispatchList[iloop].OrderD;
						dispatchInfo.OrderNo = dispatchList[iloop].OrderNo;
						dispatchInfo.DetailNo = dispatchList[iloop].DetailNo;
						dispatchInfo.DispatchNo = dispatchList[iloop].DispatchNo;
						// set dispatch order
						dispatchInfo.DispatchOrder = dispatchList[iloop].DispatchOrder;
						// set Location1 and Time1
						dispatchInfo.Location1 = "";
						dispatchInfo.Time1 = "";
						dispatchInfo.Operation1 = "";
						if (!string.IsNullOrEmpty(dispatchList[iloop].Location1N))
						{
							dispatchInfo.Location1 = dispatchList[iloop].Location1N;
							if (dispatchList[iloop].Location1DT != null)
							{
								dispatchInfo.Time1 = Utilities.GetFormatDateAndHourReportByLanguage((DateTime) dispatchList[iloop].Location1DT,
									1);
							}
							dispatchInfo.Operation1 = dispatchList[iloop].Operation1N;
						}

						//set Location2 and Time2
						dispatchInfo.Location2 = "";
						dispatchInfo.Time2 = "";
						dispatchInfo.Operation2 = "";
						if (!string.IsNullOrEmpty(dispatchList[iloop].Location2N))
						{
							dispatchInfo.Location2 = dispatchList[iloop].Location2N;
							if (dispatchList[iloop].Location2DT != null)
							{
								dispatchInfo.Time2 = Utilities.GetFormatDateAndHourReportByLanguage((DateTime) dispatchList[iloop].Location2DT,
									1);
							}
							dispatchInfo.Operation2 = dispatchList[iloop].Operation2N;
						}

						//set Location3 and Time3
						dispatchInfo.Location3 = "";
						dispatchInfo.Time3 = "";
						dispatchInfo.Operation3 = "";
						if (!string.IsNullOrEmpty(dispatchList[iloop].Location3N))
						{
							dispatchInfo.Location3 = dispatchList[iloop].Location3N;
							if (dispatchList[iloop].Location3DT != null)
							{
								dispatchInfo.Time3 = Utilities.GetFormatDateAndHourReportByLanguage((DateTime) dispatchList[iloop].Location3DT,
									1);
							}
							dispatchInfo.Operation3 = dispatchList[iloop].Operation3N;
						}
						dispatchInfo.ContainerStatus = dispatchList[iloop].ContainerStatus;
						truckInfo.DispatchInfoList.Add(dispatchInfo);
						truckListViewModel.TruckList.Add(truckInfo);
					}
					else
					{
						var dispatchInfo = new DispatchInfo();

						dispatchInfo.OrderD = dispatchList[iloop].OrderD;
						dispatchInfo.OrderNo = dispatchList[iloop].OrderNo;
						dispatchInfo.DetailNo = dispatchList[iloop].DetailNo;
						dispatchInfo.DispatchNo = dispatchList[iloop].DispatchNo;
						// set dispatch order
						dispatchInfo.DispatchOrder = dispatchList[iloop].DispatchOrder;
						// set Location1 and Time1
						dispatchInfo.Location1 = "";
						dispatchInfo.Time1 = "";
						dispatchInfo.Operation1 = "";
						if (!string.IsNullOrEmpty(dispatchList[iloop].Location1N))
						{
							dispatchInfo.Location1 = dispatchList[iloop].Location1N;
							if (dispatchList[iloop].Location1DT != null)
							{
								dispatchInfo.Time1 = Utilities.GetFormatDateAndHourReportByLanguage((DateTime) dispatchList[iloop].Location1DT,
									1);
							}
							dispatchInfo.Operation1 = dispatchList[iloop].Operation1N;
						}

						//set Location2 and Time2
						dispatchInfo.Location2 = "";
						dispatchInfo.Time2 = "";
						dispatchInfo.Operation2 = "";
						if (!string.IsNullOrEmpty(dispatchList[iloop].Location2N))
						{
							dispatchInfo.Location2 = dispatchList[iloop].Location2N;
							if (dispatchList[iloop].Location2DT != null)
							{
								dispatchInfo.Time2 = Utilities.GetFormatDateAndHourReportByLanguage((DateTime) dispatchList[iloop].Location2DT,
									1);
							}
							dispatchInfo.Operation2 = dispatchList[iloop].Operation2N;
						}

						//set Location3 and Time3
						dispatchInfo.Location3 = "";
						dispatchInfo.Time3 = "";
						dispatchInfo.Operation3 = "";
						if (!string.IsNullOrEmpty(dispatchList[iloop].Location3N))
						{
							dispatchInfo.Location3 = dispatchList[iloop].Location3N;
							if (dispatchList[iloop].Location3DT != null)
							{
								dispatchInfo.Time3 = Utilities.GetFormatDateAndHourReportByLanguage((DateTime) dispatchList[iloop].Location3DT,
									1);
							}
							dispatchInfo.Operation3 = dispatchList[iloop].Operation3N;
						}
						dispatchInfo.ContainerStatus = dispatchList[iloop].ContainerStatus;
						truckListViewModel.TruckList[truckListViewModel.TruckList.Count - 1].DispatchInfoList.Add(dispatchInfo);
					}
				}
			}

			#endregion

			// set info from Truck_M
			if (truckList.Count > 0)
			{
				for (var iloop = 0; iloop < truckList.Count; iloop++)
				{
					var index = dispatchList.FindIndex(f => f.TruckC == truckList[iloop].TruckC);
					if (index < 0)
					{
						var driver = _driverRepository.GetAllQueryable().ToList();
						var assistantName = driver.FirstOrDefault(p => p.DriverC == truckList[iloop].AssistantC2);
						var truckInfo = new TruckInfo();
						var truckC = truckList[iloop].TruckC;
						truckInfo.TruckC = truckList[iloop].TruckC;
						truckInfo.RegisteredNo = truckList[iloop].RegisteredNo;
						truckInfo.DriverC = "";
						truckInfo.DriverN = "";
						truckInfo.AssistantC = "";
						truckInfo.AssistantN = "";
						truckInfo.DriverC2 = truckList[iloop].DriverC2;
						truckInfo.DriverN2 = truckList[iloop].DriverN2;
						truckInfo.AssistantC2 = truckList[iloop].AssistantC2;
						truckInfo.AssistantN2 = assistantName != null ? assistantName.LastN + " " + assistantName.FirstN : "";
						truckInfo.DriverFirstN2 = truckList[iloop].DriverFirstN2;
						truckInfo.CompanyN = "";
						truckInfo.DispatchOrder = null;
						truckInfo.Status = truckList[iloop].Status;
						truckInfo.IsChecked = 0;
						//check planInspection
						var plan = _inspectionPlanDetailRepository.GetAllQueryable()
							.Where(i => i.ObjectI == "0" && i.Code == truckC &&
							            i.InspectionPlanD == transportD).ToList();

						truckInfo.IsPlanInspection = plan.Count > 0;
						truckListViewModel.TruckList.Add(truckInfo);
					}
					else
					{
						int count = truckListViewModel.TruckList.Count;
						if (count > 0)
						{
							for (var tloop = 0; tloop < truckListViewModel.TruckList.Count; tloop++)
							{
								if (truckListViewModel.TruckList[tloop].TruckC == truckList[iloop].TruckC)
									truckListViewModel.TruckList[tloop].Status = truckList[iloop].Status;
							}
						}
					}
				}
			}

			if (truckListViewModel.TruckList != null && truckListViewModel.TruckList.Count > 0)
			{
				for (var tloop = 0; tloop < truckListViewModel.TruckList.Count; tloop++)
				{
					var notcalculatecurrent = 0;
					//start lay tinh trang tiep theo
					var listDPSort = new List<DispatchInfo>();
					if (truckListViewModel.TruckList[tloop].DispatchInfoList != null)
					{
						var listdispatch = new List<DispatchInfo>();
						for (int loop = 0; loop < truckListViewModel.TruckList[tloop].DispatchInfoList.Count; loop++)
						{
							
							DateTime orderD = truckListViewModel.TruckList[tloop].DispatchInfoList[loop].OrderD;
							string orderN = truckListViewModel.TruckList[tloop].DispatchInfoList[loop].OrderNo;
							int detailN = truckListViewModel.TruckList[tloop].DispatchInfoList[loop].DetailNo;
							int dispatchN = truckListViewModel.TruckList[tloop].DispatchInfoList[loop].DispatchNo;
							var dis = _dispatchRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderN && p.DetailNo == detailN && p.DispatchNo == dispatchN).FirstOrDefault();
							if (dis.Location1DT <= transportD &&
							    ((dis.Location3DT != null && dis.Location3DT > transportD) ||
							     (dis.Location2DT != null && dis.Location2DT > transportD)))
							{
								truckListViewModel.TruckList[tloop].Status = "/";
								truckListViewModel.TruckList[tloop].ContainerStatus = "dispatching";
								notcalculatecurrent++;
							}
							else
							{
								listdispatch.Add(truckListViewModel.TruckList[tloop].DispatchInfoList[loop]);
							}
						}
						if (listdispatch.Count > 0)
						{
							int? maxdispatchOrder = listdispatch.Max(p => p.DispatchOrder);
							var selectmaxflagup =
								truckListViewModel.TruckList[tloop].DispatchInfoList.FirstOrDefault(p => p.DispatchOrder == maxdispatchOrder);
							//neu noi dung la ha thi status la chua dieu di, neu noi dung la nang thi la chua dieu ve, neu noi dung la nang ha thi la da dieu di ve
							truckListViewModel.TruckList[tloop].Status = selectmaxflagup.ContainerStatus == "2"
								? (truckListViewModel.TruckList[tloop].Status + "/3")
								: (selectmaxflagup.ContainerStatus == "3"
									? (truckListViewModel.TruckList[tloop].Status + "/2")
									: (selectmaxflagup.ContainerStatus == "4"
										? truckListViewModel.TruckList[tloop].Status + "/4"
										: (truckListViewModel.TruckList[tloop].Status + "/2")));
						}
						listDPSort = truckListViewModel.TruckList[tloop].DispatchInfoList.OrderBy("OrderD asc, DispatchNo asc, DispatchOrder asc").ToList();
					}
					else
					{
						truckListViewModel.TruckList[tloop].Status = (truckListViewModel.TruckList[tloop].Status + "/0");
					}
					//end lay tinh trang tiep theo
					if (notcalculatecurrent < 1)
					{
						//start lay tinh trang hien tai
						string dc = truckListViewModel.TruckList[tloop].DriverC;
						string truckC = truckListViewModel.TruckList[tloop].TruckC;
						var dispatchtransD =
							_dispatchRepository.Query(p => p.TransportD == transportD && p.DriverC == dc)
								.OrderByDescending(p => p.DispatchOrder)
								.FirstOrDefault();
						if (dispatchtransD != null)
						{
							truckListViewModel.TruckList[tloop].ContainerStatus = dispatchtransD.ContainerStatus;
						}
						else
						{
							var selectprevdispatch =
								_dispatchRepository.Query(p => (p.Location1DT <= transportD &&
								((p.Location3DT != null && p.Location3DT >= transportD) ||
								 (p.Location2DT != null && p.Location2DT >= transportD))) && p.TruckC == truckC)
									.OrderByDescending(p => p.DispatchNo)
									.ThenByDescending(p => p.DispatchOrder)
									.FirstOrDefault();

							if (selectprevdispatch != null)
							{
								if (selectprevdispatch.TransportD != transportD)
								{
									truckListViewModel.TruckList[tloop].ContainerStatus = selectprevdispatch.ContainerStatus;
								}
								else
								{
									truckListViewModel.TruckList[tloop].ContainerStatus = (selectprevdispatch.ContainerStatus == "2" ? "prev3" : (selectprevdispatch.ContainerStatus == "3" ? "prev4" : "0"));
								}
							}
							else
							{
								truckListViewModel.TruckList[tloop].ContainerStatus = "0";
							}
						}
						//end lay tinh trang hien tai
					}
					truckListViewModel.TruckList[tloop].DispatchInfoList = listDPSort;
				}
				truckListViewModel.TruckList =
					truckListViewModel.TruckList.Where(a => ( //chua dieu di
						(notdispatchgoStatus &
						 (a.Status.Substring(a.Status.Length - 1) == "2" || a.Status.Substring(a.Status.Length - 1) == "4" ||
						  a.ContainerStatus == "0" || a.ContainerStatus == "prev4")) ||
						//chua dieu ve
						(notdispatchbackStatus & (a.Status.Substring(a.Status.Length - 1) == "3" || a.ContainerStatus == "prev3")) ||
						//da dieu di
						(dispatchgoStatus & (a.ContainerStatus == "2")) ||
						//da dieu ve
						(dispatchbackStatus & (a.ContainerStatus == "3")) ||
						//chua dieu di + chua dieu ve
						(notdispatchgoStatus & notdispatchbackStatus &
						 (a.Status.Substring(a.Status.Length - 1) == "2" || a.Status.Substring(a.Status.Length - 1) == "3" ||
						  a.Status.Substring(a.Status.Length - 1) == "4" || a.ContainerStatus == "0" || a.ContainerStatus == "prev4" ||
						  a.ContainerStatus == "prev3")) ||
						//chua dieu di + da dieu di
						(notdispatchgoStatus & dispatchgoStatus &
						 (a.Status.Substring(a.Status.Length - 1) == "2" || a.Status.Substring(a.Status.Length - 1) == "4" ||
						  a.ContainerStatus == "2" || a.ContainerStatus == "0" || a.ContainerStatus == "prev4")) ||
						// chua dieu di + da dieu ve
						(notdispatchgoStatus & dispatchbackStatus &
						 (a.Status.Substring(a.Status.Length - 1) == "2" || a.Status.Substring(a.Status.Length - 1) == "4" ||
						  a.ContainerStatus == "3" || a.ContainerStatus == "0" || a.ContainerStatus == "prev4")) ||
						//chua dieu ve + da dieu di
						(notdispatchbackStatus & dispatchgoStatus &
						 (a.Status.Substring(a.Status.Length - 1) == "3" || a.ContainerStatus == "2" || a.ContainerStatus == "prev3")) ||
						//chua dieu ve + da dieu ve
						(notdispatchbackStatus & dispatchbackStatus &
						 (a.Status.Substring(a.Status.Length - 1) == "3" || a.ContainerStatus == "3" || a.ContainerStatus == "prev3")) ||
						//da dieu di + da dieu ve
						(dispatchgoStatus & dispatchbackStatus & (a.ContainerStatus == "2" || a.ContainerStatus == "3")) ||
						//khac
						(otherStatus & a.Status.Length > 4) ||
						a.ContainerStatus == "dispatching") &&
					                                        (string.IsNullOrEmpty(searchtruckC) || searchtruckC == "" ||
					                                         a.TruckC.Equals(searchtruckC))).ToList();
				truckListViewModel.TruckList = truckListViewModel.TruckList.OrderBy("RegisteredNo asc, DriverN2 asc").ToList();
			}

			return truckListViewModel;
		}

		public TrailerListViewModel GetTrailerList(DateTime transportD, string depC, string searchtrailerC, bool notdispatchgoStatus, bool notdispatchbackStatus,
			bool dispatchgoStatus, bool dispatchbackStatus, bool otherStatus)
		{
			TrailerListViewModel trailerListViewModel = new TrailerListViewModel();
			// get Trailer_M
			var trailer = from a in _trailerRepository.GetAllQueryable()
				join b in _driverRepository.GetAllQueryable() on new {a.DriverC}
					equals new {b.DriverC} into t1
				from b in t1.DefaultIfEmpty()
				where (a.IsActive == Constants.ACTIVE)
				select new TrailerInfo()
				{
					TrailerC = a.TrailerC,
					TrailerNo = a.TrailerNo,
					DriverC = "",
					DriverN = "",
					DriverFirstN = "",
					DriverC2 = (a.DriverC ?? ""),
					DriverN2 = b != null ? b.LastN + " " + b.FirstN : "",
					DriverFirstN2 = b.FirstN,
					DispatchOrder = null,
					Status =
						a.FromDate != null && a.ToDate != null
							? (a.FromDate <= transportD.Date && transportD.Date <= a.ToDate ? a.Situation : "")
							: (a.FromDate != null && a.ToDate == null
								? (a.FromDate <= transportD.Date ? a.Situation : "")
								: (a.FromDate == null && a.ToDate != null ? (transportD.Date <= a.ToDate ? a.Situation : "") : ""))
				};

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var trailerList = trailer.OrderBy("TrailerC asc, DriverC2 asc").ToList();

			// get Dispatch
			var dispatch = from a in _dispatchRepository.GetAllQueryable()

				join oD in _orderDRepository.GetAllQueryable() on new {a.OrderD, a.OrderNo, a.DetailNo}
					equals new {oD.OrderD, oD.OrderNo, oD.DetailNo} into t
				from oD in t.DefaultIfEmpty()

				join b in _trailerRepository.GetAllQueryable() on new {oD.TrailerC}
					equals new {b.TrailerC} into t1
				from b in t1.DefaultIfEmpty()
				join c in _driverRepository.GetAllQueryable() on new {a.DriverC}
					equals new {c.DriverC} into t2
				from c in t2.DefaultIfEmpty()
				join d in _orderHRepository.GetAllQueryable() on new {a.OrderD, a.OrderNo}
					equals new {d.OrderD, d.OrderNo} into t3
				from d in t3.DefaultIfEmpty()
				join o1 in _operationRepository.GetAllQueryable() on a.Operation1C equals o1.OperationC into op1
				from o1 in op1.DefaultIfEmpty()
				join o2 in _operationRepository.GetAllQueryable() on a.Operation2C equals o2.OperationC into op2
				from o2 in op2.DefaultIfEmpty()
				join o3 in _operationRepository.GetAllQueryable() on a.Operation3C equals o3.OperationC into op3
				from o3 in op3.DefaultIfEmpty()
				where
					((a.TransportD == transportD ||
					  (a.TransportD != transportD &&
					   (a.Location1DT <= transportD &&
						((a.Location3DT != null && a.Location3DT >= transportD) ||
						 (a.Location3DT == null && (a.Location2DT != null && a.Location2DT >= transportD)))))) &&
					  (string.IsNullOrEmpty(depC) || depC == "0" || d.OrderDepC == depC) && !string.IsNullOrEmpty(oD.TrailerC))
				//oD.TrailerC != null && oD.TrailerC != "")
				select new DispatchViewModel()
				{
					OrderD = a.OrderD,
					OrderNo = a.OrderNo,
					DetailNo = a.DetailNo,
					DispatchNo = a.DispatchNo,
					TrailerC = oD.TrailerC,
					TrailerNo = b.TrailerNo,
					DriverC = a.DriverC,
					DriverN = c.LastN + " " + c.FirstN,
					DriverFirstN = c.FirstN,
					DispatchOrder = a.DispatchOrder,
					Location1C = a.Location1C,
					Location1N = a.Location1N,
					Location1DT = a.Location1DT,
					Operation1N = o1.OperationN,
					Location2C = a.Location2C,
					Location2N = a.Location2N,
					Location2DT = a.Location2DT,
					Operation2N = o2.OperationN,
					Location3C = a.Location3C,
					Location3N = a.Location3N,
					Location3DT = a.Location3DT,
					Operation3N = o3.OperationN
				};
			var dispatchList = dispatch.OrderBy("TrailerC asc, DriverC asc, DispatchOrder asc").ToList();

			#region get dispatch info

			// set info from dispatch
			if (dispatchList.Count > 0)
			{
				var oldKey = "";
				for (var iloop = 0; iloop < dispatchList.Count; iloop++)
				{
					var newKey = dispatchList[iloop].TrailerC;
					if (oldKey != newKey)
					{
						oldKey = newKey;
						var trailerInfo = new TrailerInfo();

						trailerInfo.TrailerC = dispatchList[iloop].TrailerC;
						trailerInfo.TrailerNo = dispatchList[iloop].TrailerNo;
						trailerInfo.DriverC = dispatchList[iloop].DriverC;
						trailerInfo.DriverN = dispatchList[iloop].DriverN;
						trailerInfo.DriverFirstN = dispatchList[iloop].DriverFirstN;

						// set dispatch info
						trailerInfo.DispatchInfoList = new List<TrailerDispatchInfo>();
						var dispatchInfo = new TrailerDispatchInfo();

						dispatchInfo.OrderD = dispatchList[iloop].OrderD;
						dispatchInfo.OrderNo = dispatchList[iloop].OrderNo;
						dispatchInfo.DetailNo = dispatchList[iloop].DetailNo;
						dispatchInfo.DispatchNo = dispatchList[iloop].DispatchNo;
						// set dispatch order
						dispatchInfo.DispatchOrder = dispatchList[iloop].DispatchOrder;
						// set Location1 and Time1
						dispatchInfo.Location1 = "";
						dispatchInfo.Time1 = "";
						dispatchInfo.Operation1 = "";
						if (!string.IsNullOrEmpty(dispatchList[iloop].Location1N))
						{
							dispatchInfo.Location1 = dispatchList[iloop].Location1N;
							if (dispatchList[iloop].Location1DT != null)
							{
								dispatchInfo.Time1 = Utilities.GetFormatDateAndHourReportByLanguage((DateTime) dispatchList[iloop].Location1DT,
									1);
							}
							dispatchInfo.Operation1 = dispatchList[iloop].Operation1N;
						}

						//set Location2 and Time2
						dispatchInfo.Location2 = "";
						dispatchInfo.Time2 = "";
						dispatchInfo.Operation2 = "";
						if (!string.IsNullOrEmpty(dispatchList[iloop].Location2N))
						{
							dispatchInfo.Location2 = dispatchList[iloop].Location2N;
							if (dispatchList[iloop].Location2DT != null)
							{
								dispatchInfo.Time2 = Utilities.GetFormatDateAndHourReportByLanguage((DateTime) dispatchList[iloop].Location2DT,
									1);
							}
							dispatchInfo.Operation2 = dispatchList[iloop].Operation2N;
						}

						//set Location3 and Time3
						dispatchInfo.Location3 = "";
						dispatchInfo.Time3 = "";
						dispatchInfo.Operation3 = "";
						if (!string.IsNullOrEmpty(dispatchList[iloop].Location3N))
						{
							dispatchInfo.Location3 = dispatchList[iloop].Location3N;
							if (dispatchList[iloop].Location3DT != null)
							{
								dispatchInfo.Time3 = Utilities.GetFormatDateAndHourReportByLanguage((DateTime) dispatchList[iloop].Location3DT,
									1);
							}
							dispatchInfo.Operation3 = dispatchList[iloop].Operation3N;
						}

						trailerInfo.DispatchInfoList.Add(dispatchInfo);
						trailerListViewModel.TrailerList.Add(trailerInfo);
					}
					else
					{
						var dispatchInfo = new TrailerDispatchInfo();

						dispatchInfo.OrderD = dispatchList[iloop].OrderD;
						dispatchInfo.OrderNo = dispatchList[iloop].OrderNo;
						dispatchInfo.DetailNo = dispatchList[iloop].DetailNo;
						dispatchInfo.DispatchNo = dispatchList[iloop].DispatchNo;
						// set dispatch order
						dispatchInfo.DispatchOrder = dispatchList[iloop].DispatchOrder;
						// set Location1 and Time1
						dispatchInfo.Location1 = "";
						dispatchInfo.Time1 = "";
						dispatchInfo.Operation1 = "";
						if (!string.IsNullOrEmpty(dispatchList[iloop].Location1N))
						{
							dispatchInfo.Location1 = dispatchList[iloop].Location1N;
							if (dispatchList[iloop].Location1DT != null)
							{
								dispatchInfo.Time1 = Utilities.GetFormatDateAndHourReportByLanguage((DateTime) dispatchList[iloop].Location1DT,
									1);
							}
							dispatchInfo.Operation1 = dispatchList[iloop].Operation1N;
						}

						//set Location2 and Time2
						dispatchInfo.Location2 = "";
						dispatchInfo.Time2 = "";
						dispatchInfo.Operation2 = "";
						if (!string.IsNullOrEmpty(dispatchList[iloop].Location2N))
						{
							dispatchInfo.Location2 = dispatchList[iloop].Location2N;
							if (dispatchList[iloop].Location2DT != null)
							{
								dispatchInfo.Time2 = Utilities.GetFormatDateAndHourReportByLanguage((DateTime) dispatchList[iloop].Location2DT,
									1);
							}
							dispatchInfo.Operation2 = dispatchList[iloop].Operation2N;
						}

						//set Location3 and Time3
						dispatchInfo.Location3 = "";
						dispatchInfo.Time3 = "";
						dispatchInfo.Operation3 = "";
						if (!string.IsNullOrEmpty(dispatchList[iloop].Location3N))
						{
							dispatchInfo.Location3 = dispatchList[iloop].Location3N;
							if (dispatchList[iloop].Location3DT != null)
							{
								dispatchInfo.Time3 = Utilities.GetFormatDateAndHourReportByLanguage((DateTime) dispatchList[iloop].Location3DT,
									1);
							}
							dispatchInfo.Operation3 = dispatchList[iloop].Operation3N;
						}

						trailerListViewModel.TrailerList[trailerListViewModel.TrailerList.Count - 1].DispatchInfoList.Add(dispatchInfo);
					}
				}
			}

			#endregion

			// set info from Truck_M
			if (trailerList.Count > 0)
			{
				for (var iloop = 0; iloop < trailerList.Count; iloop++)
				{
					var index = dispatchList.FindIndex(f => f.TrailerC == trailerList[iloop].TrailerC);
					if (index < 0)
					{
						var driver = _driverRepository.GetAllQueryable().ToList();
						var trailerInfo = new TrailerInfo();
						var trailerC = trailerList[iloop].TrailerC;
						trailerInfo.TrailerC = trailerList[iloop].TrailerC;
						trailerInfo.TrailerNo = trailerList[iloop].TrailerNo;
						trailerInfo.DriverC = "";
						trailerInfo.DriverN = "";
						trailerInfo.DriverC2 = trailerList[iloop].DriverC2;
						trailerInfo.DriverN2 = trailerList[iloop].DriverN2;
						trailerInfo.DriverFirstN2 = trailerList[iloop].DriverFirstN2;
						trailerInfo.CompanyN = "";
						trailerInfo.DispatchOrder = null;
						trailerInfo.Status = trailerList[iloop].Status;
						trailerInfo.IsChecked = 0;
						trailerListViewModel.TrailerList.Add(trailerInfo);
					}
					else
					{
						var selectforupdate =
							trailerListViewModel.TrailerList.SingleOrDefault(x => x.TrailerC == trailerList[iloop].TrailerC);
						if (selectforupdate != null)
							selectforupdate.Status = trailerList[iloop].Status;
					}
				}
			}

			if (trailerListViewModel.TrailerList != null && trailerListViewModel.TrailerList.Count > 0)
			{
				trailerListViewModel.TrailerList =
					trailerListViewModel.TrailerList.Where(
						a => (string.IsNullOrEmpty(searchtrailerC) || searchtrailerC == "" || a.TrailerC.Equals(searchtrailerC))).ToList();
				//|| (!dispatchStatus1 & !dispatchStatus0 & !otherStatus & !inspectionStatus)).ToList();

				trailerListViewModel.TrailerList = trailerListViewModel.TrailerList.OrderBy("TrailerNo asc, DriverN2 asc").ToList();
			}

			return trailerListViewModel;
		}

		public int GetMaxDispatchOrder(DateTime? transportD, string driverC, string truckC)
		{
			if (transportD == null || driverC == null || truckC == null) return 0;
			var maxDipatchOrder =
				_dispatchRepository.Query(p => p.TransportD == transportD && p.DriverC == driverC && p.TruckC == truckC);
			var maxNo = maxDipatchOrder.Max(p => p.DispatchOrder);
			return maxNo ?? 0;
		}

		public List<DispatchListReport> GetDispatchList(DispatchReportParam param)
		{
			//List<DispatchListReport> result = new List<DispatchListReport>();
			//// get data from Order_D and Order_H
			//var order = from a in _orderDRepository.GetAllQueryable()
			//			   join b in _orderHRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
			//					   equals new { b.OrderD, b.OrderNo} into t1
			//			   from b in t1.DefaultIfEmpty()
			//			   join c in _customerRepository.GetAllQueryable() on new { b.CustomerMainC, b.CustomerSubC }
			//					equals new { c.CustomerMainC, c.CustomerSubC } into t2
			//			   from c in t2.DefaultIfEmpty()
			//			   join d in _departmentRepository.GetAllQueryable() on b.OrderDepC 
			//					equals  d.DepC  into t3
			//			   from d in t3.DefaultIfEmpty()
			//			   join e in _shippingCompanyRepository.GetAllQueryable() on b.ShippingCompanyC
			//					equals e.ShippingCompanyC into t4
			//			   from e in t4.DefaultIfEmpty()
			//			   join f in _trailerRepository.GetAllQueryable() on a.TrailerC
			//					equals f.TrailerC into t5
			//			   from f in t5.DefaultIfEmpty()
			//			   where ((param.OrderDFrom == null || a.OrderD >= param.OrderDFrom) &
			//					  (param.OrderDTo == null || a.OrderD <= param.OrderDTo) &
			//					  (param.Customer == "null" || (param.Customer).Contains(b.CustomerMainC + "_" + b.CustomerSubC)) &
			//					  (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || b.OrderDepC == param.DepC) &
			//					  (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || b.OrderTypeI == param.OrderTypeI)
			//					 )

			//			   select new DispatchListReport()
			//			   {
			//				   OrderD = a.OrderD,
			//				   OrderNo = a.OrderNo,
			//				   DetailNo = a.DetailNo,
			//				   CustomerShortN = c != null ? c.CustomerShortN : "",
			//				   OrderTypeI = b.OrderTypeI,
			//				   DepN = d!= null ? d.DepN : "",
			//				   BLBK = b.BLBK,
			//				   ShippingCompanyN = e != null ? e.ShippingCompanyN : "",
			//				   ContainerNo = a.ContainerNo,
			//				   TrailerNo = f != null ? f.TrailerNo : "",
			//				   LoadingPlaceN = b.LoadingPlaceN,
			//				   LoadingDT = b.LoadingDT,
			//				   ActualLoadingD = a.ActualLoadingD,
			//				   DischargeDT = b.DischargeDT,
			//				   DischargePlaceN = b.DischargePlaceN,
			//				   ActualDischargeD = a.ActualDischargeD
			//			   };

			//order = order.OrderBy("OrderD asc, OrderNo asc, DetailNo asc");
			//var orderList = order.ToList();

			//for (int iloop = 0; iloop < orderList.Count; iloop++)
			//{
			//	var orderD = orderList[iloop].OrderD;
			//	var orderNo = orderList[iloop].OrderNo;
			//	var detailNo = orderList[iloop].DetailNo;
			//	var dispatch = from a in _dispatchRepository.GetAllQueryable()
			//				   join b in _driverRepository.GetAllQueryable() on a.DriverC
			//					   equals b.DriverC into t1
			//				   from b in t1.DefaultIfEmpty()
			//				   join c in _truckRepository.GetAllQueryable() on a.TruckC
			//					   equals c.TruckC into t2
			//				   from c in t2.DefaultIfEmpty()
			//				   where (a.OrderD == orderD &
			//					   a.OrderNo == orderNo &
			//					   a.DetailNo == detailNo &
			//					   (param.TransportDFrom == null || a.TransportD >= param.TransportDFrom) &
			//					   (param.TransportDTo == null || a.TransportD <= param.TransportDTo) &
			//					   ((param.DispatchStatus0 && param.DispatchStatus1 && param.DispatchStatus2) ||
			//						(param.DispatchStatus0 && a.DispatchStatus == "0") ||
			//						(param.DispatchStatus1 && a.DispatchStatus == "1") ||
			//						(param.DispatchStatus2 && a.DispatchStatus == "2")
			//					   )
			//					 )
			//				   select new DispatchViewModel()
			//				   {
			//					   DispatchNo = a.DispatchNo,
			//					   TransportD = a.TransportD,
			//					   RegisteredNo = c != null ? c.RegisteredNo : "",
			//					   DriverN = b != null ? b.LastN + " " + b.FirstN : "",
			//					   ContainerStatus = a.ContainerStatus,
			//					   Location1N = a.Location1N,
			//					   Location2N = a.Location2N,
			//					   Location3N = a.Location3N,
			//					   DispatchStatus = a.DispatchStatus
			//				   };

			//	dispatch = dispatch.OrderBy("DispatchNo asc");


				//if (param.DispatchStatus0 && param.DispatchStatus1 && param.DispatchStatus2)
				//{
				//	if (dispatch.Any())
				//	{
				//		orderList[iloop].DispatchDList = dispatch.ToList();
				//	}
				//	result.Add(orderList[iloop]);
				//}
				//else
				//{
				//	if (dispatch.Any())
				//	{
				//		orderList[iloop].DispatchDList = dispatch.ToList();
				//		result.Add(orderList[iloop]);
				//	}
				//}
			//	if (dispatch.Any())
			//	{
			//		orderList[iloop].DispatchDList = dispatch.ToList();
			//		result.Add(orderList[iloop]);
			//	}
			//}

			//return result;
			return new List<DispatchListReport>();
		}

		public List<DispatchDetailViewModel> GetDriverDispatchReportList(DriverDispatchReportParam param)
		{
			var driver = from a in _dispatchRepository.GetAllQueryable()
						join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo }
								equals new { b.OrderD, b.OrderNo, b.DetailNo } into t1
						from b in t1.DefaultIfEmpty()
						join c in _orderHRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
							 equals new { c.OrderD, c.OrderNo } into t2
						from c in t2.DefaultIfEmpty()
						join d in _customerRepository.GetAllQueryable() on new { c.CustomerMainC, c.CustomerSubC }
							 equals new { d.CustomerMainC, d.CustomerSubC } into t3
						from d in t3.DefaultIfEmpty()
						where ((param.TransportDFrom == null || a.TransportD >= param.TransportDFrom) &
							   (param.TransportDTo == null || a.TransportD <= param.TransportDTo) &
							   (param.TruckC == null || a.TruckC == param.TruckC) &
							   (param.DriverC == null || a.TruckC == param.DriverC) &
							   (param.Customer == "null" || (param.Customer).Contains(c.CustomerMainC + "_" + c.CustomerSubC)) &
							   (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || c.OrderDepC == param.DepC) &
							   (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || c.OrderTypeI == param.OrderTypeI)
							   )
						select new DispatchDetailViewModel()
						{
							Dispatch = new DispatchViewModel
							{
								TransportD = a.TransportD,
								DriverC = a.DriverC,
								DispatchOrder = a.DispatchOrder,
								ContainerStatus = a.ContainerStatus,
								Location1N = a.Location1N,
								Location2N = a.Location2N,
								Location3N = a.Location3N,
								Location1DT = a.Location1DT,
								Location2DT = a.Location2DT,
								Location3DT = a.Location3DT,
							},
							OrderD = new ContainerViewModel()
							{
								ContainerNo = b.ContainerNo,
								ContainerSizeI = b.ContainerSizeI,
							},
							OrderH = new OrderViewModel()
							{
								OrderTypeI = c.OrderTypeI,
								CustomerShortN = d != null ? d.CustomerShortN : "",
							}
						};

			driver = driver.OrderBy("Dispatch.TransportD asc, Dispatch.DriverC asc, Dispatch.DispatchOrder asc");
			var driverList = driver.ToList();

			return driverList;
		}

		public List<DispatchDetailViewModel> GetTransportExpenseReportList(DriverDispatchReportParam param)
		{
			var transportExpense = from a in _dispatchRepository.GetAllQueryable()
						 join b in _truckRepository.GetAllQueryable() on a.TruckC
								 equals b.TruckC into t1
						 from b in t1.DefaultIfEmpty()
						 join c in _driverRepository.GetAllQueryable() on a.DriverC
								 equals c.DriverC into t2
						 from c in t2.DefaultIfEmpty()
						 join d in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo }
								 equals new { d.OrderD, d.OrderNo, d.DetailNo } into t3
						 from d in t3.DefaultIfEmpty()
						 join e in _orderHRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
							  equals new { e.OrderD, e.OrderNo } into t4
						 from e in t4.DefaultIfEmpty()
						 join f in _customerRepository.GetAllQueryable() on new { e.CustomerMainC, e.CustomerSubC }
							  equals new { f.CustomerMainC, f.CustomerSubC } into t5
						 from f in t5.DefaultIfEmpty()
						 where ((param.TransportDFrom == null || a.TransportD >= param.TransportDFrom) &
								(param.TransportDTo == null || a.TransportD <= param.TransportDTo) &
								(string.IsNullOrEmpty(param.TruckC) || param.TruckC == "undefined" || a.TruckC == param.TruckC) &
								(string.IsNullOrEmpty(param.DriverC) || param.DriverC == "undefined" || a.TruckC == param.DriverC) &
								(param.Customer == "null" || (param.Customer).Contains(e.CustomerMainC + "_" + e.CustomerSubC)) &
								(string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || e.OrderDepC == param.DepC) &
								(string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || e.OrderTypeI == param.OrderTypeI)
							   )
						 select new DispatchDetailViewModel()
						 {
							 Dispatch = new DispatchViewModel
							 {
								 OrderD = a.OrderD,
								 OrderNo = a.OrderNo,
								 DetailNo = a.DetailNo,
								 DispatchNo = a.DispatchNo,
								 TransportD = a.TransportD,
								 DispatchOrder = a.DispatchOrder,
								 ContainerStatus = a.ContainerStatus,
								 TruckC = a.TruckC,
								 RegisteredNo = b != null ? b.RegisteredNo : "",
								 DriverC = a.DriverC,
								 DriverN = c != null ? c.LastN + " " + c.FirstN : "",
								 Expense = a.Expense,
								 TransportFee = a.TransportFee,
							 },
							 OrderD = new ContainerViewModel()
							 {
								 ContainerNo = d.ContainerNo,
								 ActualLoadingD = d.ActualLoadingD,
								 ActualDischargeD = d.ActualDischargeD,
							 },
							 OrderH = new OrderViewModel()
							 {
								 OrderTypeI = e.OrderTypeI,
								 BLBK = e.BLBK,
								 CustomerShortN = f != null ? f.CustomerShortN : "",
							 }
						 };

			transportExpense = transportExpense.OrderBy("Dispatch.OrderD asc, Dispatch.OrderNo asc, Dispatch.DetailNo asc, Dispatch.DetailNo asc, Dispatch.TransportD asc, Dispatch.DispatchOrder asc");
			var transportExpenseList = transportExpense.ToList();

			return transportExpenseList;
		}

		public void SaveDispatch()
		{
			_unitOfWork.Commit();
		}


		/// <summary>
		/// Mobile API Get Dispatch List
		/// </summary>
		/// <param name="date"></param>
		/// <param name="driverC"></param>
		/// <returns></returns>
		public MobileDispatchList MGetDispatchList(DateTime date, string driverC)
		{
			var dispatchData = new MobileDispatchList();
			var dispatchList = (from p in _dispatchRepository.GetAllQueryable()
									  join d in _orderDRepository.GetAllQueryable() on new { p.OrderD, p.OrderNo, p.DetailNo } equals new { d.OrderD, d.OrderNo, d.DetailNo } into pd
									  from d in pd.DefaultIfEmpty()
									  join t in _containerTypeRepository.GetAllQueryable() on new { d.ContainerTypeC } equals new { t.ContainerTypeC } into td
									  from t in td.DefaultIfEmpty()
									  join o1 in _operationRepository.GetAllQueryable() on p.Operation1C equals o1.OperationC into to1
									  from o1 in to1.DefaultIfEmpty()
									  join o2 in _operationRepository.GetAllQueryable() on p.Operation2C equals o2.OperationC into to2
									  from o2 in to2.DefaultIfEmpty()
									  join o3 in _operationRepository.GetAllQueryable() on p.Operation3C equals o3.OperationC into to3
									  from o3 in to3.DefaultIfEmpty()
									  join r in _trailerRepository.GetAllQueryable() on d.TrailerC equals r.TrailerC into dr
									  from r in dr.DefaultIfEmpty()
									  join tr in _truckRepository.GetAllQueryable() on p.TruckC equals tr.TruckC into ptr
									  from tr in ptr.DefaultIfEmpty()
                                      join h in _orderHRepository.GetAllQueryable() on new { d.OrderD, d.OrderNo }
							                        equals new { h.OrderD, h.OrderNo } into t2
                                        from h in t2.DefaultIfEmpty()
									  where p.TransportD == date && p.DriverC == driverC
									  select new MobileDispatchViewModel()
									  {
										  OrderD = p.OrderD,
										  OrderNo = p.OrderNo,
										  DetailNo = p.DetailNo,
										  ContainerSizeI = d.ContainerSizeI,
										  ContainerTypeN = t.ContainerTypeN,
										  ContainerNo = d.ContainerNo,
										  SealNo = d.SealNo,
										  DispatchNo = p.DispatchNo,
										  TransportD = p.TransportD,
										  DispatchI = p.DispatchI,
										  //RegisteredNo = p.RegisteredNo,
										  RegisteredNo = tr.RegisteredNo,
										  DriverC = p.DriverC,
										  DispatchOrder = p.DispatchOrder,
										  ContainerStatus = p.ContainerStatus,
										  DispatchStatus = p.DispatchStatus,
										  Location1N = p.Location1N,
										  Location2N = p.Location2N,
										  Location3N = p.Location3N,
										  Location1DT = p.Location1DT,
										  Location2DT = p.Location2DT,
										  Location3DT = p.Location3DT,
										  Operation1C = p.Operation1C,
										  Operation2C = p.Operation2C,
										  Operation3C = p.Operation3C,
										  Operation1N = o1.OperationN,
										  Operation2N = o2.OperationN,
										  Operation3N = o3.OperationN,
										  IsTransported1 = p.IsTransported1,
										  IsTransported2 = p.IsTransported2,
										  IsTransported3 = p.IsTransported3,
										  CountTransport = p.CountTransport,
										  Description = p.Description,
										  TrailerNo = r.TrailerNo,
										  NetWeight = d.NetWeight,
                                          OrderImageKey = p.OrderImageKey ?? "",
                                          ImageCount = p.ImageCount,
                                          IsCollected = h.IsCollected,
                                          CustomerPayLiftSubC = h.CustomerPayLiftLoweredSubC,
                                          CustomerPayLiftMainC = h.CustomerPayLiftLoweredMainC
									  }).OrderBy(p => p.DispatchOrder).ToList();
			//var truckC = dispatchD.TruckC;
			//var truck = _truckRepository.Query(tru => tru.TruckC == truckC).FirstOrDefault();
			//if (truck != null)
			//{
			//	dispatchD.RegisteredNo = truck.RegisteredNo;
			//}
			if (dispatchList != null && dispatchList.Count > 0)
			{
				var basicSet = _basicRepository.GetAllQueryable().FirstOrDefault();
				for (int i = 0; i < dispatchList.Count; i++)
				{
					string custMainC = dispatchList[i].CustomerPayLiftMainC;
					string custSuc = dispatchList[i].CustomerPayLiftSubC;
					var cust =
						_customerRepository.Query(p => p.CustomerMainC == custMainC && p.CustomerSubC == custSuc).FirstOrDefault();
					if (dispatchList[i].IsCollected == "1")
					{
						if (basicSet != null)
						{
							dispatchList[i].IsCollected = basicSet.CompanyShortN;
							dispatchList[i].CustomerPayLiftMainC = basicSet.Address1;
							dispatchList[i].CustomerPayLiftSubC = basicSet.TaxCode;
						}
					}
					else if (dispatchList[i].IsCollected == "0")
					{
						if (cust != null)
						{
							dispatchList[i].IsCollected = cust.CustomerN;
							dispatchList[i].CustomerPayLiftMainC = cust.Address1;
							dispatchList[i].CustomerPayLiftSubC = cust.TaxCode;
						}

					}
					else
					{
						dispatchList[i].IsCollected = "";
						dispatchList[i].CustomerPayLiftMainC = "";
						dispatchList[i].CustomerPayLiftSubC = "";
					}
				}
				dispatchData.Dispatchs = dispatchList;
			}
			return dispatchData;
		}


		public bool MUpdateDispatch(MobileDispatchViewModel dispatch)
		{
			var orderD = dispatch.OrderD.Date;
			var orderNo = dispatch.OrderNo;
			var detailNo = dispatch.DetailNo;
			var dispatchNo = dispatch.DispatchNo;

			var orderDUpdate = _orderDRepository.Query(ord => ord.OrderD == orderD &&
															  ord.OrderNo == orderNo &&
															  ord.DetailNo == detailNo
													  ).FirstOrDefault();
			if (orderDUpdate != null)
			{
				// update to Order_D
				orderDUpdate.ContainerNo = dispatch.ContainerNo;
				orderDUpdate.SealNo = dispatch.SealNo;
				var trailerNo = dispatch.TrailerNo;
				var trailerMaster = _trailerRepository.Query(tr => tr.TrailerNo == trailerNo).FirstOrDefault();
				if (trailerMaster != null)
				{
					orderDUpdate.TrailerC = trailerMaster.TrailerC;
				}
				else
				{
					orderDUpdate.TrailerC = null;
				}
				_orderDRepository.Update(orderDUpdate);

				// update to Dispatch_D
				var dispatchDUpdate = _dispatchRepository.Query(dis => dis.OrderD == orderD &&
																		dis.OrderNo == orderNo &&
																		dis.DetailNo == detailNo &&
																		dis.DispatchNo == dispatchNo
																).FirstOrDefault();
				dispatchDUpdate.IsTransported1 = dispatch.IsTransported1;
				dispatchDUpdate.IsTransported2 = dispatch.IsTransported2;
				dispatchDUpdate.IsTransported3 = dispatch.IsTransported3;

				//set dispatch status when all operations done
				if ((!string.IsNullOrEmpty(dispatchDUpdate.Location1C) || !string.IsNullOrEmpty(dispatchDUpdate.Location1N)) &
					(!string.IsNullOrEmpty(dispatchDUpdate.Location2C) || !string.IsNullOrEmpty(dispatchDUpdate.Location2N)) &
					(!string.IsNullOrEmpty(dispatchDUpdate.Location3C) || !string.IsNullOrEmpty(dispatchDUpdate.Location3N)))
				{
					if (dispatch.IsTransported1 & dispatch.IsTransported2 & dispatch.IsTransported3)
					{
						dispatchDUpdate.DispatchStatus = Convert.ToInt32(DispatchStatus.Transported).ToString();
					}
					else
					{
						dispatchDUpdate.DispatchStatus = Convert.ToInt32(DispatchStatus.Dispatch).ToString();
					}
				}
				else if ((!string.IsNullOrEmpty(dispatchDUpdate.Location1C) || !string.IsNullOrEmpty(dispatchDUpdate.Location1N)) &
						(!string.IsNullOrEmpty(dispatchDUpdate.Location2C) || !string.IsNullOrEmpty(dispatchDUpdate.Location2N)) &
						(string.IsNullOrEmpty(dispatchDUpdate.Location3C) & string.IsNullOrEmpty(dispatchDUpdate.Location3N)))
				{
					if (dispatch.IsTransported1 & dispatch.IsTransported2)
					{
						dispatchDUpdate.DispatchStatus = Convert.ToInt32(DispatchStatus.Transported).ToString();
					}
					else
					{
						dispatchDUpdate.DispatchStatus = Convert.ToInt32(DispatchStatus.Dispatch).ToString();
					}
				}
				else if ((!string.IsNullOrEmpty(dispatchDUpdate.Location1C) || !string.IsNullOrEmpty(dispatchDUpdate.Location1N)) &
					(string.IsNullOrEmpty(dispatchDUpdate.Location2C) & string.IsNullOrEmpty(dispatchDUpdate.Location2N)) &
					(!string.IsNullOrEmpty(dispatchDUpdate.Location3C) || !string.IsNullOrEmpty(dispatchDUpdate.Location3N)))
				{
					if (dispatch.IsTransported1 & dispatch.IsTransported3)
					{
						dispatchDUpdate.DispatchStatus = Convert.ToInt32(DispatchStatus.Transported).ToString();
					}
					else
					{
						dispatchDUpdate.DispatchStatus = Convert.ToInt32(DispatchStatus.Dispatch).ToString();
					}
				}
				else if ((string.IsNullOrEmpty(dispatchDUpdate.Location1C) & string.IsNullOrEmpty(dispatchDUpdate.Location1N)) &
				(!string.IsNullOrEmpty(dispatchDUpdate.Location2C) || !string.IsNullOrEmpty(dispatchDUpdate.Location2N)) &
				(!string.IsNullOrEmpty(dispatchDUpdate.Location3C) || !string.IsNullOrEmpty(dispatchDUpdate.Location3N)))
				{
					if (dispatch.IsTransported2 & dispatch.IsTransported3)
					{
						dispatchDUpdate.DispatchStatus = Convert.ToInt32(DispatchStatus.Transported).ToString();
					}
					else
					{
						dispatchDUpdate.DispatchStatus = Convert.ToInt32(DispatchStatus.Dispatch).ToString();
					}
				}
				else if ((!string.IsNullOrEmpty(dispatchDUpdate.Location1C) || !string.IsNullOrEmpty(dispatchDUpdate.Location1N)) &
							(string.IsNullOrEmpty(dispatchDUpdate.Location2C) & string.IsNullOrEmpty(dispatchDUpdate.Location2N)) &
							(string.IsNullOrEmpty(dispatchDUpdate.Location3C) & string.IsNullOrEmpty(dispatchDUpdate.Location3N)))
				{
					if (dispatch.IsTransported1)
					{
						dispatchDUpdate.DispatchStatus = Convert.ToInt32(DispatchStatus.Transported).ToString();
					}
					else
					{
						dispatchDUpdate.DispatchStatus = Convert.ToInt32(DispatchStatus.Dispatch).ToString();
					}
				}
				else if ((string.IsNullOrEmpty(dispatchDUpdate.Location1C) & string.IsNullOrEmpty(dispatchDUpdate.Location1N)) &
							(!string.IsNullOrEmpty(dispatchDUpdate.Location2C) || !string.IsNullOrEmpty(dispatchDUpdate.Location2N)) &
							(string.IsNullOrEmpty(dispatchDUpdate.Location3C) & string.IsNullOrEmpty(dispatchDUpdate.Location3N)))
				{
					if (dispatch.IsTransported2)
					{
						dispatchDUpdate.DispatchStatus = Convert.ToInt32(DispatchStatus.Transported).ToString();
					}
					else
					{
						dispatchDUpdate.DispatchStatus = Convert.ToInt32(DispatchStatus.Dispatch).ToString();
					}
				}
				else if ((string.IsNullOrEmpty(dispatchDUpdate.Location1C) & string.IsNullOrEmpty(dispatchDUpdate.Location1N)) &
							(string.IsNullOrEmpty(dispatchDUpdate.Location2C) & string.IsNullOrEmpty(dispatchDUpdate.Location2N)) &
							(!string.IsNullOrEmpty(dispatchDUpdate.Location3C) || !string.IsNullOrEmpty(dispatchDUpdate.Location3N)))
				{
					if (dispatch.IsTransported3)
					{
						dispatchDUpdate.DispatchStatus = Convert.ToInt32(DispatchStatus.Transported).ToString();
					}
					else
					{
						dispatchDUpdate.DispatchStatus = Convert.ToInt32(DispatchStatus.Dispatch).ToString();
					}
				}

				// calculate detain day
				//if (dispatchDUpdate.ContainerStatus == Constants.LOAD && dispatchDUpdate.DispatchStatus == Constants.TRANSPORTED)
				//{
				//    dispatchDUpdate.DetainDay = 0;
				//    var detainDay = 0;
				//    if (orderDUpdate.ActualLoadingD != null && orderDUpdate.ActualDischargeD != null)
				//    {
				//        detainDay = Utilities.SubstractTwoDate((DateTime)orderDUpdate.ActualDischargeD, (DateTime)orderDUpdate.ActualLoadingD);
				//        detainDay = detainDay < 0 ? 0 : detainDay;
				//        dispatchDUpdate.DetainDay = detainDay;
				//    }
				//    else if (orderDUpdate.ActualLoadingD != null && orderDUpdate.ActualDischargeD == null)
				//    {
				//        detainDay = Utilities.SubstractTwoDate(DateTime.Now, (DateTime)orderDUpdate.ActualLoadingD);
				//        detainDay = detainDay < 0 ? 0 : detainDay;
				//        dispatchDUpdate.DetainDay = detainDay;
				//    }
				//    else
				//    {
				//        dispatchDUpdate.DetainDay = 0;
				//    }
				//}
				_dispatchRepository.Update(dispatchDUpdate);

				// commit update
				SaveDispatch();
			}
			return true;
		}

        public int? GetOrderNoDoubleMax()
        {
            var maxOrderDouble = _orderDRepository.Query(o => o != null).Max(o => o.OrderNoDouble);
            return maxOrderDouble;
        }
		#endregion
        public bool GetStatusCont(int? detailNo, DateTime? orderD, string orderNo)
        {
            var statusCont =
                _orderDRepository.Query(o => o.OrderD == orderD && o.OrderNo == orderNo && o.DetailNo == detailNo).FirstOrDefault();
            if (statusCont == null)
            {
                return false;
            }
            else
            {
                if (statusCont.EnableDouble == 1 && statusCont.OrderNoDouble != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool SaveOrderNoDouble(int? orderNoDouble, DateTime? orderD1, string orderNo1, int? detailNo1, DateTime? orderD2, string orderNo2, int? detailNo2)
        {
            if (orderD2 == null)
            {
                var orderDList = _orderDRepository.Query(o => o.OrderNoDouble == orderNoDouble).ToList();
                foreach (var item in orderDList)
                {
                    item.OrderNoDouble = null;
                    item.EnableDouble = null;
                }
                SaveDispatch();

            }
            else
            {
                var order1 =
                _orderDRepository.Query(o => o.OrderD == orderD1 && o.OrderNo == orderNo1 && o.DetailNo == detailNo1)
                .FirstOrDefault();
                var order2 =
                    _orderDRepository.Query(o => o.OrderD == orderD2 && o.OrderNo == orderNo2 && o.DetailNo == detailNo2)
                    .FirstOrDefault();
                if (order1 == null || order2 == null)
                {
                    return false;
                }
                order1.OrderNoDouble = orderNoDouble;
                order1.EnableDouble = 1;

                order2.OrderNoDouble = orderNoDouble;
                order2.EnableDouble = 1;
                SaveDispatch();//save to get OrderNoDouble
            }

            return true;
        }
	}
}
