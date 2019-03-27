using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.Allowance;
using Website.ViewModels.Container;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Expense;
using Website.ViewModels.Order;
using Website.ViewModels.Surcharge;

namespace Service.Services
{
	public interface IContainerService
	{
		IEnumerable<ContainerViewModel> GetContainers();
		TransportConfirmContainerViewModel GetTransportConfirmContainer(DateTime date, string orderNo, int detailNo);
		TransportConfirmViewModel GetTransportConfirm(DateTime date, string orderNo, int detailNo);
		ContainerDetailDatatable GetContainersForTable(ContainerSearchParams containerSearchParams);
		object GetOrderDetailNo(DateTime date, string orderNo);
		void CreateConfirmationOrder(TransportConfirmViewModel order);
		void UpdateConfirmationOrder(TransportConfirmViewModel order);
		int DeleteContainer(DateTime orderD, string orderNo, int detailNo, bool isConfirmedDeleting);
		void DeleteContainerProcessing(DateTime orderD, string orderNo, int detailNo);
		int CheckContainerDeleting(DateTime orderD, string orderNo, int detailNo);
		int? GetDetainDay(DateTime orderD, string orderNo, int detailNo, string containerNo);
		void SaveContainer();
	}
	public class ContainerService : IContainerService
	{
		private readonly IContainerRepository _containerRepository;
		private readonly IContainerTypeRepository _containerTypeRepository;
		private readonly IOrderRepository _orderRepository;
		private readonly IDispatchRepository _dispatchRepository;
		private readonly IDriverRepository _driverRepository;
		private readonly ITruckRepository _truckRepository;
		private readonly ITrailerRepository _trailerRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IPartnerRepository _partnerRepository;
		private readonly IExpenseDetailRepository _expenseDetailRepository;
		private readonly ISurchargeDetailRepository _surchargeDetailRepository;
		private readonly IAllowanceDetailRepository _allowanceDetailRepository;
		private readonly IOrderService _orderService;
		private readonly IDispatchService _dispatchService;
		private readonly IContainerRepository _orderDRepository;
		private readonly IUnitOfWork _unitOfWork;

		public ContainerService(IContainerRepository containerRepository, IContainerTypeRepository containerTypeRepository, IOrderRepository orderRepository,
								IDispatchRepository dispatchRepository, ITrailerRepository trailerRepository, IPartnerRepository partnerRepository,
								IDriverRepository driverRepository, ICustomerRepository customerRepository, ITruckRepository truckRepository,
								ISurchargeDetailRepository surchargeDetailRepository, IAllowanceDetailRepository allowanceDetailRepository,
								IExpenseDetailRepository expenseDetailRepository, IOrderService orderService, IDispatchService dispatchService,
								IContainerRepository orderDRepository, IUnitOfWork unitOfWork)
		{
			this._containerRepository = containerRepository;
			this._containerTypeRepository = containerTypeRepository;
			this._orderRepository = orderRepository;
			this._dispatchRepository = dispatchRepository;
			this._driverRepository = driverRepository;
			this._customerRepository = customerRepository;
			this._partnerRepository = partnerRepository;
			this._trailerRepository = trailerRepository;
			this._truckRepository = truckRepository;
			this._expenseDetailRepository = expenseDetailRepository;
			this._surchargeDetailRepository = surchargeDetailRepository;
			this._allowanceDetailRepository = allowanceDetailRepository;
			this._orderService = orderService;
			this._dispatchService = dispatchService;
			this._orderDRepository = orderDRepository;
			this._unitOfWork = unitOfWork;
		}
		public IEnumerable<ContainerViewModel> GetContainers()
		{
			var orders = _containerRepository.GetAll();
			if (orders != null)
			{
				var destination = Mapper.Map<IEnumerable<Order_D>, IEnumerable<ContainerViewModel>>(orders);
				return destination;
			}
			return null;
		}

		public TransportConfirmContainerViewModel GetTransportConfirmContainer(DateTime date, string orderNo, int detailNo)
		{
			var container = _containerRepository.Query(p => p.OrderD == date && p.OrderNo == orderNo && p.DetailNo == detailNo).FirstOrDefault();

			if (container != null)
			{
				var transportConfirmContainer = Mapper.Map<Order_D, TransportConfirmContainerViewModel>(container);
				var containerType =
					_containerTypeRepository.Query(p => p.ContainerTypeC == transportConfirmContainer.ContainerTypeC).FirstOrDefault();
				if (containerType != null)
				{
					transportConfirmContainer.ContainerTypeN = containerType.ContainerTypeN;
				}

				var trailer =
					_trailerRepository.Query(p => p.TrailerC == transportConfirmContainer.TrailerC).FirstOrDefault();
				if (trailer != null)
				{
					transportConfirmContainer.TrailerNo = trailer.TrailerNo;
				}

				//Check min, max OrderNo
				transportConfirmContainer.IsMaxDetailNo = CheckDetailNoMax(date, orderNo, detailNo);
				transportConfirmContainer.IsMinDetailNo = CheckDetailNoMin(date, orderNo, detailNo);

				return transportConfirmContainer;
			}
			return null;
		}

		public TransportConfirmViewModel GetTransportConfirm(DateTime date, string orderNo, int detailNo)
		{
			var transportConfirm = new TransportConfirmViewModel();
			var transportConfirmOrder = _orderService.GetTransportConfirmOrder(date, orderNo);
			if (transportConfirmOrder == null) return null;
			var transportConfirmContainer = GetTransportConfirmContainer(date, orderNo, detailNo);
			if (transportConfirmContainer == null)
			{
				var newDetailInfo = GetOrderDetailNo(date, orderNo);
				var type = newDetailInfo.GetType();
				transportConfirm.NewOrderDetailNo = (int)type.GetProperty("newOrderDetailNo").GetValue(newDetailInfo, null);
				transportConfirm.IsMaxDetailNo = (bool)type.GetProperty("isMaxOrderDetailNo").GetValue(newDetailInfo, null);
				transportConfirm.IsMinDetailNo = (bool)type.GetProperty("isMinOrderDetailNo").GetValue(newDetailInfo, null);
			}
			var transportConfirmDispatchList = _dispatchService.GetTransConfirmDispatchList(date, orderNo, detailNo);
			transportConfirm.TransportConfirmOrder = transportConfirmOrder;
			transportConfirm.TransportConfirmContainer = transportConfirmContainer;
			transportConfirm.TransportConfirmDispatchList = transportConfirmDispatchList;
			transportConfirm.ContainerIndex = FindIndex(date, orderNo, detailNo); // we just need to find the index of the first dispatch
		    transportConfirm.ImageCount = transportConfirmDispatchList.Sum(i => i.ImageCount);
            transportConfirm.OrderImageKey = String.Join("X", transportConfirmDispatchList.Select(d => d.OrderImageKey));
			return transportConfirm;
		}

		public ContainerDetailDatatable GetContainersForTable(ContainerSearchParams searchInfo)
		{
			//if (!string.IsNullOrEmpty(searchInfo.ContainerInfo.TransportMonthYear))
			//{
			//	searchInfo.ContainerInfo.TransportMonth =
			//	Convert.ToInt32(searchInfo.ContainerInfo.TransportMonthYear.Split('/')[0]);
			//	searchInfo.ContainerInfo.TransportYear =
			//		Convert.ToInt32(searchInfo.ContainerInfo.TransportMonthYear.Split('/')[1]);
			//}

			var result = from c in _containerRepository.GetAllQueryable()
						 join d in _orderRepository.GetAllQueryable() on new { c.OrderD, c.OrderNo }
							 equals new { d.OrderD, d.OrderNo } into cd
						 from d in cd.DefaultIfEmpty()
						 join h in _dispatchRepository.GetAllQueryable() on new { c.OrderD, c.OrderNo, c.DetailNo }
							 equals new { h.OrderD, h.OrderNo, h.DetailNo } into cdh
						 from h in cdh.DefaultIfEmpty()
						 join cu in _customerRepository.GetAllQueryable() on new { d.CustomerMainC, d.CustomerSubC }
							equals new { cu.CustomerMainC, cu.CustomerSubC } into lcus
						 from cu in lcus.DefaultIfEmpty()
						 join dr in _driverRepository.GetAllQueryable() on h.DriverC equals dr.DriverC into ldr
						 from dr in ldr.DefaultIfEmpty()
						 join tr in _truckRepository.GetAllQueryable() on h.TruckC equals tr.TruckC into ltr
						 from tr in ltr.DefaultIfEmpty()
						 join r in _trailerRepository.GetAllQueryable() on c.TrailerC equals r.TrailerC into trai
						 from r in trai.DefaultIfEmpty()
						 join p in _partnerRepository.GetAllQueryable() on new { h.PartnerMainC, h.PartnerSubC }
							equals new { p.PartnerMainC, p.PartnerSubC } into pltr
						 from p in pltr.DefaultIfEmpty()
						 where (searchInfo.ContainerInfo.ContainerNo == "" || c.ContainerNo.Contains(searchInfo.ContainerInfo.ContainerNo)) &
							  (searchInfo.ContainerInfo.SealNo == "" || c.SealNo.Contains(searchInfo.ContainerInfo.SealNo)) &
							  (searchInfo.ContainerInfo.JobNo == "" || d.JobNo.Contains(searchInfo.ContainerInfo.JobNo)) &
							  (searchInfo.ContainerInfo.BLBK == "" || d.BLBK.Contains(searchInfo.ContainerInfo.BLBK)) &
							  (searchInfo.ContainerInfo.TrailerNo == "" || r.TrailerNo.Contains(searchInfo.ContainerInfo.TrailerNo)) &
							  (searchInfo.ContainerInfo.OrderTypeI == "-1" || d.OrderTypeI == searchInfo.ContainerInfo.OrderTypeI) &
							  (searchInfo.ContainerInfo.DepC == "" || d.OrderDepC == searchInfo.ContainerInfo.DepC) &
							  (searchInfo.ContainerInfo.EntryClerkC == "" || d.EntryClerkC == searchInfo.ContainerInfo.EntryClerkC) &
							  ((searchInfo.ContainerInfo.EndD == null || (d.OrderD <= searchInfo.ContainerInfo.EndD)) &&
							  (searchInfo.ContainerInfo.StartD == null || d.OrderD >= searchInfo.ContainerInfo.StartD)) &
							  (searchInfo.ContainerInfo.OrderNo == "" || d.OrderNo == searchInfo.ContainerInfo.OrderNo) &
							  (searchInfo.ContainerInfo.CustomerMainC == "" ||
							   (d.CustomerMainC == searchInfo.ContainerInfo.CustomerMainC && d.CustomerSubC == searchInfo.ContainerInfo.CustomerSubC)) &
							  (searchInfo.ContainerInfo.TransportDTo == null || h.TransportD <= searchInfo.ContainerInfo.TransportDTo) &
							  (searchInfo.ContainerInfo.TransportDFrom == null || h.TransportD >= searchInfo.ContainerInfo.TransportDFrom) &
							  (searchInfo.ContainerInfo.TruckC == "" || h.TruckC == searchInfo.ContainerInfo.TruckC) &
							  (searchInfo.ContainerInfo.DriverC == "" || h.DriverC == searchInfo.ContainerInfo.DriverC) &
							  (searchInfo.ContainerInfo.PartnerMainC == "" ||
							  (h.PartnerMainC == searchInfo.ContainerInfo.PartnerMainC && h.PartnerSubC == searchInfo.ContainerInfo.PartnerSubC)) &
							  (searchInfo.ContainerInfo.DispatchStatus0  && (h.DispatchStatus == null || h.DispatchStatus== Constants.NOTDISPATCH) ||
							   (searchInfo.ContainerInfo.DispatchStatus1 && h.DispatchStatus == Constants.DISPATCH) ||
							   (searchInfo.ContainerInfo.DispatchStatus2 && h.DispatchStatus == Constants.TRANSPORTED) ||
							   (searchInfo.ContainerInfo.DispatchStatus3 && h.DispatchStatus == Constants.CONFIRMED))
						 select new ContainerDetailViewModel()
						 {
							 OrderD = c.OrderD,
							 OrderNo = c.OrderNo,
							 DetailNo = c.DetailNo,
							 ContainerNo = c.ContainerNo,
							 SealNo = c.SealNo,
							 TransportD = h != null ? h.TransportD : null,
							 DispatchNo = h != null ? h.DispatchNo : 0,
							 ContainerStatus = h != null ? h.ContainerStatus : "",
							 DispatchStatus = h != null ? h.DispatchStatus : "0",
							 DriverC = h != null ? h.DriverC : "",
							 DriverFirstN = dr != null ? dr.FirstN : "",
							 DriverLastN = dr != null ? dr.LastN : "",
							 PartnerMainC = p != null ? h.PartnerMainC : "",
							 PartnerSubC = p != null ? h.PartnerSubC : "",
							 PartnerN = p != null ? p.PartnerN : "",
							 PartnerShortN = p != null ? p.PartnerShortN : "",
							 TruckC = h != null ? h.TruckC : "",
							 RegisteredNo = tr != null ? tr.RegisteredNo : h.RegisteredNo,
							 CustomerMainC = d.CustomerMainC,
							 CustomerSubC = d.CustomerSubC,
							 CustomerN = cu != null ? (cu.CustomerShortN != "" ? cu.CustomerShortN : cu.CustomerN) : "",
							 OrderTypeI = d.OrderTypeI,
							 BLBK = d.BLBK,
							 TrailerNo = r.TrailerNo ?? "",
							 JobNo = d.JobNo,
							 LoadingPlaceN = d.LoadingPlaceN,
							 StopoverPlaceN = d.StopoverPlaceN,
							 DischargePlaceN = d.DischargePlaceN,
							 Location1N = h.Location1N,
							 Location2N = h.Location2N,
							 Location3N = h.Location3N,
							 Description = c.Description,
							 RevenueD = c.RevenueD,
						 };

			// sorting
			var resultOrdered = result.OrderBy(searchInfo.sortBy + (searchInfo.reverse ? " descending" : ""));

			//paging
			var resultPaging = resultOrdered.Skip((searchInfo.page - 1) * searchInfo.itemsPerPage).Take(searchInfo.itemsPerPage).ToList();

			var datatable = new ContainerDetailDatatable()
			{
				Data = resultPaging,
				Total = resultOrdered.Count()
			};
			return datatable;
		}

		public void CreateConfirmationOrder(TransportConfirmViewModel order)
		{
			//Insert Order
			var cOrder = order.TransportConfirmOrder;
			if (cOrder != null)
			{
				var mOrder = Mapper.Map<TransportConfirmOrderViewModel, Order_H>(cOrder);
				var existOrder = _orderRepository.Query(p => p.OrderD == cOrder.OrderD && p.OrderNo == cOrder.OrderNo).FirstOrDefault();
				if (existOrder != null)
				{
					_orderRepository.Detach(existOrder);
					_orderRepository.Update(mOrder);
				}
				else
				{
					_orderRepository.Add(mOrder);
				}
				//we need to complete insert to DB before trigger use data from Order_H to manipulate
				SaveContainer();

				//Insert Container
				var cContainer = order.TransportConfirmContainer;
				if (cContainer != null)
				{
					var mContainer = Mapper.Map<TransportConfirmContainerViewModel, Order_D>(cContainer);
					mContainer.OrderD = cOrder.OrderD;
					mContainer.OrderNo = cOrder.OrderNo;
					mContainer.UnitPrice = mContainer.UnitPrice ?? 0;
					_containerRepository.Add(mContainer);

					//Insert Dispatch
					if (order.TransportConfirmDispatchList != null && order.TransportConfirmDispatchList.Count != 0)
					{
						var dispatchNo = GetBaseDispatchNo(cOrder.OrderD, cOrder.OrderNo, cContainer.DetailNo);
						foreach (var cDispatch in order.TransportConfirmDispatchList)
						{
							var mDispatch = Mapper.Map<TransportConfirmDispatchViewModel, Dispatch_D>(cDispatch);
							mDispatch.OrderD = cOrder.OrderD;
							mDispatch.OrderNo = cOrder.OrderNo;
							mDispatch.OrderTypeI = cOrder.OrderTypeI;
							mDispatch.DetailNo = cContainer.DetailNo;
							mDispatch.DispatchNo = dispatchNo++;
							mDispatch.DispatchOrder = _dispatchService.GetMaxDispatchOrder(mDispatch.TransportD, mDispatch.DriverC,
								mDispatch.TruckC) + 1;
							// set RegisteredNo
							if (mDispatch.DispatchI == "0" ||
								(mDispatch.DispatchI == "1" && !string.IsNullOrEmpty(mDispatch.TruckC))
								)
							{
								mDispatch.RegisteredNo = null;
							}
							_dispatchRepository.Add(mDispatch);
							SaveContainer();
						}

						#region Insert Dispatch Expense
						//Insert Dispatch Expense
						if (order.TransportConfirmExpensesList != null && order.TransportConfirmExpensesList.Count != 0)
						{
							foreach (var cConfirmExpense in order.TransportConfirmExpensesList)
							{
								if (cConfirmExpense != null)
								{
									if (cConfirmExpense.ExpenseDetailList != null)
									{
										var expenseNo = GetBaseExpenseNo(cOrder.OrderD, cOrder.OrderNo, cContainer.DetailNo, cConfirmExpense.DispatchNo);
										foreach (var cExpense in cConfirmExpense.ExpenseDetailList)
										{
											var mExpense = Mapper.Map<ExpenseDetailViewModel, Expense_D>(cExpense);
											mExpense.OrderD = cOrder.OrderD;
											mExpense.OrderNo = cOrder.OrderNo;
											mExpense.DetailNo = cContainer.DetailNo;
											mExpense.DispatchNo = cConfirmExpense.DispatchNo;
											mExpense.ExpenseNo = expenseNo++;
											_expenseDetailRepository.Add(mExpense);
										}
									}
								}
							}
						}
						#endregion Insert Dispatch Expense

						#region Insert Allowance
						//Insert Allowance
						if (order.TransportConfirmAllowanceList != null && order.TransportConfirmAllowanceList.Count != 0)
						{
							foreach (var cConfirmAllowance in order.TransportConfirmAllowanceList)
							{
								if (cConfirmAllowance != null)
								{
									if (cConfirmAllowance.ExpenseDetailList != null)
									{
										var allowanceNo = GetBaseAllowanceNo(cOrder.OrderD, cOrder.OrderNo, cContainer.DetailNo, cConfirmAllowance.DispatchNo);
										foreach (var cExpense in cConfirmAllowance.ExpenseDetailList)
										{
											var mAllowance = Mapper.Map<AllowanceDetailViewModel, Allowance_D>(cExpense);
											mAllowance.OrderD = cOrder.OrderD;
											mAllowance.OrderNo = cOrder.OrderNo;
											mAllowance.DetailNo = cContainer.DetailNo;
											mAllowance.DispatchNo = cConfirmAllowance.DispatchNo;
											mAllowance.AllowanceNo = allowanceNo++;
											_allowanceDetailRepository.Add(mAllowance);
										}
									}
								}
							}
						}
						#endregion Insert Allowance

						#region Insert Dispatch Surcharge
						//Insert Dispatch Surcharge
						if (order.TransportConfirmSurchargeList != null && order.TransportConfirmSurchargeList.Count != 0)
						{
							var conSurcharges = order.TransportConfirmSurchargeList.Where(p => p.DispatchNo != 0);
							foreach (var cSurcharge in conSurcharges)
							{
								if (cSurcharge != null)
								{
									if (cSurcharge.ExpenseDetailList != null)
									{
										var surchargeNo = GetBaseSurchargeNo(cOrder.OrderD, cOrder.OrderNo, cContainer.DetailNo, cSurcharge.DispatchNo);
										foreach (var surcharge in cSurcharge.ExpenseDetailList)
										{
											var mSurcharge = Mapper.Map<SurchargeDetailViewModel, Surcharge_D>(surcharge);
											mSurcharge.OrderD = cOrder.OrderD;
											mSurcharge.OrderNo = cOrder.OrderNo;
											mSurcharge.DetailNo = cContainer.DetailNo;
											mSurcharge.DispatchNo = cSurcharge.DispatchNo;
											mSurcharge.DispatchI = "1";
											mSurcharge.SurchargeNo = surchargeNo++;
											_surchargeDetailRepository.Add(mSurcharge);
										}
									}
								}
							}
						}
						#endregion Insert Dispatch Surcharge

					}

					#region insert surcharge
					//insert Surcharge
					var cSurcharges = order.TransportConfirmSurchargeList;
					if (cSurcharges != null && cSurcharges.Count != 0)
					{
						var cSurcharge = cSurcharges.FirstOrDefault(p => p.DispatchNo == 0);
						if (cSurcharge != null)
						{
							var surchargeNo = GetBaseSurchargeNo(cOrder.OrderD, cOrder.OrderNo, cContainer.DetailNo, cSurcharge.DispatchNo);
							foreach (var surcharge in cSurcharge.ExpenseDetailList)
							{
								var mSurcharge = Mapper.Map<SurchargeDetailViewModel, Surcharge_D>(surcharge);
								mSurcharge.OrderD = cOrder.OrderD;
								mSurcharge.OrderNo = cOrder.OrderNo;
								mSurcharge.DetailNo = cContainer.DetailNo;
								mSurcharge.DispatchNo = 0;
								mSurcharge.DispatchI = "0";
								mSurcharge.SurchargeNo = surchargeNo++;
								_surchargeDetailRepository.Add(mSurcharge);
							}
						}
					}
					#endregion insert surcharge
				}
				SaveContainer();

				SetQuantityContainerAndUnitPrice(cOrder.OrderD, cOrder.OrderNo);
				// set OrderType
                //var dispatchList = _dispatchRepository.Query(d => d.OrderD == cOrder.OrderD && d.OrderNo == cOrder.OrderNo).ToList();
                //for (var iloop = 0; iloop < dispatchList.Count; iloop++)
                //{
                //    var dispatchUpdate = dispatchList[iloop];
                //    dispatchUpdate.OrderTypeI = cOrder.OrderTypeI;
                //    _dispatchRepository.Update(dispatchUpdate);
                //}
                //SaveContainer();
			}
		}

		public void SetQuantityContainerAndUnitPrice(DateTime orderD, string orderNo)
		{
			var data = from a in _containerRepository.GetAllQueryable()
						where (a.OrderD == orderD && a.OrderNo == orderNo)
						select new ContainerViewModel()
						{
							OrderD = a.OrderD,
							OrderNo = a.OrderNo,
							ContainerSize20 = a.ContainerSizeI == "0" ? 1 : 0,
							ContainerSize40 = a.ContainerSizeI == "1" ? 1 : 0,
							ContainerSize45 = a.ContainerSizeI == "2" ? 1 : 0,
							TotalLoads = a.NetWeight,
							Amount = a.Amount
						};

			if (data.Any())
			{
				var groupData = from b in data
								group b by new { b.OrderD, b.OrderNo }
									into c
									select new
									{
										c.Key.OrderD,
										c.Key.OrderNo,
										Quantity20HC = c.Sum(b => b.ContainerSize20),
										Quantity40HC = c.Sum(b => b.ContainerSize40),
										Quantity45HC = c.Sum(b => b.ContainerSize45),
										TotalLoads = c.Sum(b => b.TotalLoads),
										Amount = c.Sum(b => b.Amount),
									};

				var groupDataList = groupData.ToList();
				if (groupDataList.Count > 0)
				{
					// update order_H
					var quantity20 = groupDataList[0].Quantity20HC;
					var quantity40 = groupDataList[0].Quantity40HC;
					var quantity45 = groupDataList[0].Quantity45HC;
					var totalLoads = groupDataList[0].TotalLoads;
					var amount = groupDataList[0].Amount;

					var orderH = _orderRepository.Query(ord => ord.OrderD == orderD && ord.OrderNo == orderNo).FirstOrDefault();
					if (orderH != null)
					{
						orderH.Quantity20HC = quantity20;
						orderH.Quantity40HC = quantity40;
						orderH.Quantity45HC = quantity45;
						orderH.TotalLoads = totalLoads;
						orderH.TotalPrice = amount;

						_orderRepository.Update(orderH);
						SaveContainer();
					}
				}
			}
			else
			{
				var orderH = _orderRepository.Query(ord => ord.OrderD == orderD && ord.OrderNo == orderNo).FirstOrDefault();
				if (orderH != null)
				{
					orderH.Quantity20HC = 0;
					orderH.Quantity40HC = 0;
					orderH.Quantity45HC = 0;
					orderH.TotalLoads = 0;
					orderH.TotalPrice = 0;

					_orderRepository.Update(orderH);
					SaveContainer();
				}
			}
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
						else
						{
							if (dispatchItem.OrderTypeI == "1" || dispatchItem.OrderTypeI == "2")
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
						}
					}
				}
				_orderDRepository.Update(orderDetail);
				SaveContainer();
			}
		}

		public void UpdateConfirmationOrder(TransportConfirmViewModel order)
		{
			var cOrder = order.TransportConfirmOrder;
			//Update Order
			if (cOrder != null)
			{
				var orderH = _orderRepository.Query(p => p.OrderD == cOrder.OrderD && p.OrderNo == cOrder.OrderNo).FirstOrDefault();
				
				var mOrder = Mapper.Map<TransportConfirmOrderViewModel, Order_H>(cOrder);
				if (orderH != null)
				{
					mOrder.Quantity20HC = orderH.Quantity20HC;
					mOrder.Quantity40HC = orderH.Quantity40HC;
					mOrder.Quantity45HC = orderH.Quantity45HC;
					mOrder.TotalPrice = orderH.TotalPrice;
				}
				_orderRepository.Detach(orderH);
				_orderRepository.Update(mOrder);

				var cContainer = order.TransportConfirmContainer;

				//Update Container
				if (cContainer != null)
				{
					var mContainer = Mapper.Map<TransportConfirmContainerViewModel, Order_D>(cContainer);
					mContainer.OrderD = cOrder.OrderD;
					mContainer.OrderNo = cOrder.OrderNo;
					mContainer.UnitPrice = mContainer.UnitPrice ?? 0;
					_containerRepository.Update(mContainer);

					#region update dispatch
					//Update Dispatch
					if (order.TransportConfirmDispatchList != null)
					{
						if (order.TransportConfirmDispatchList.Count > 0)
						{
							var dispatchNo = GetBaseDispatchNo(cOrder.OrderD, cOrder.OrderNo, cContainer.DetailNo);
							foreach (var cDispatch in order.TransportConfirmDispatchList)
							{
								TransportConfirmDispatchViewModel dispatch = cDispatch;
								var existDispatch =
									_dispatchRepository.Query(c => c.OrderD == cOrder.OrderD && c.OrderNo == cOrder.OrderNo &&
																	c.DetailNo == cContainer.DetailNo && c.DispatchNo == dispatch.DispatchNo)
										.FirstOrDefault();
								int countTransport = 0;
								if (!String.IsNullOrEmpty(cDispatch.Location1N))
								{
									countTransport++;
								}
								if (!String.IsNullOrEmpty(cDispatch.Location2N))
								{
									countTransport++;
								}
								if (!String.IsNullOrEmpty(cDispatch.Location3N))
								{
									countTransport++;
								}
								cDispatch.CountTransport = countTransport;
								var mDispatch = Mapper.Map<TransportConfirmDispatchViewModel, Dispatch_D>(cDispatch);
								mDispatch.OrderD = cOrder.OrderD;
								mDispatch.OrderNo = cOrder.OrderNo;
								mDispatch.OrderTypeI = cOrder.OrderTypeI;
								mDispatch.DetailNo = cContainer.DetailNo;
								if (cDispatch.DispatchStatus == "3" || cDispatch.DispatchStatus == "2" || cDispatch.DispatchStatus == "1")
								{
									//update Location for export report
									UpdateLocationOrderDetail(cOrder.OrderD, cOrder.OrderNo, cContainer.DetailNo);
								}
								// set RegisteredNo
								if (mDispatch.DispatchI == "0" ||
									(mDispatch.DispatchI == "1" && !string.IsNullOrEmpty(mDispatch.TruckC))
									)
								{
									mDispatch.RegisteredNo = null;
								}

								if (existDispatch != null)
								{
									mDispatch.DispatchNo = existDispatch.DispatchNo;

									//fix when transport confirm doesn't have field to display Time in the grid
									if (Convert.ToDateTime(existDispatch.Location1DT).Date == Convert.ToDateTime(mDispatch.Location1DT).Date)
									{
										mDispatch.Location1DT = existDispatch.Location1DT;
									}
									if (Convert.ToDateTime(existDispatch.Location2DT).Date == Convert.ToDateTime(mDispatch.Location2DT).Date)
									{
										mDispatch.Location2DT = existDispatch.Location2DT;
									}
									if (Convert.ToDateTime(existDispatch.Location3DT).Date == Convert.ToDateTime(mDispatch.Location3DT).Date)
									{
										mDispatch.Location3DT = existDispatch.Location3DT;
									}

									//fix reload detain date
									//if ((mDispatch.DispatchStatus == Convert.ToInt32(DispatchStatus.Transported).ToString() ||
									//	mDispatch.DispatchStatus == Convert.ToInt32(DispatchStatus.Confirmed).ToString()) &&
									//	mDispatch.ContainerStatus == Convert.ToInt32(ContainerStatus.Load).ToString())
									//{
									//	if (cContainer.ActualDischargeD != null && cContainer.ActualLoadingD != null)
									//	{
									//		mDispatch.DetainDay = (int) Convert.ToDateTime(cContainer.ActualDischargeD).Date
									//			.Subtract(Convert.ToDateTime(cContainer.ActualLoadingD).Date)
									//			.TotalDays;
									//	}
									//	else if (cContainer.ActualDischargeD == null && cContainer.ActualLoadingD != null)
									//	{
									//		if (DateTime.Now.Date > Convert.ToDateTime(cContainer.ActualLoadingD))
									//		{
									//			mDispatch.DetainDay = (int) DateTime.Now.Date
									//				.Subtract(Convert.ToDateTime(cContainer.ActualLoadingD).Date)
									//				.TotalDays;
									//		}
									//		else
									//		{
									//			mDispatch.DetainDay = 0;
									//		}
									//	}
									//	else if (cContainer.ActualLoadingD == null)
									//	{
									//		mDispatch.DetainDay = 0;
									//	}
									//}
									//else
									//{
									//	mDispatch.DetainDay = 0;
									//}

                                    //update OrderType
                                    mDispatch.OrderTypeI = cOrder.OrderTypeI;

									_dispatchRepository.Detach(existDispatch);
									_dispatchRepository.Update(mDispatch);
								}
								else
								{
									mDispatch.DispatchNo = dispatchNo++;
									_dispatchRepository.Add(mDispatch);
								}
							}
						}

						#region delete dispatch
						var currDispatches = _dispatchRepository.Query(c => c.OrderD == cOrder.OrderD && c.OrderNo == cOrder.OrderNo && c.DetailNo == cContainer.DetailNo).ToList();
						//do for delete dispatch
						foreach (var currDispatch in currDispatches)
						{
							Dispatch_D dispatch = currDispatch;
							var delDispatch = order.TransportConfirmDispatchList.FirstOrDefault(p => p.DetailNo == dispatch.DetailNo && p.OrderD == dispatch.OrderD &&
																								p.OrderNo == dispatch.OrderNo && p.DispatchNo == dispatch.DispatchNo);
							if (delDispatch == null)
							{
								_dispatchRepository.Delete(currDispatch);

								//Delete expense after delete dispatch
								var currRelatedExpenses =
									_expenseDetailRepository.Query(p => p.DetailNo == dispatch.DetailNo && p.OrderD == dispatch.OrderD &&
																		p.OrderNo == dispatch.OrderNo && p.DispatchNo == dispatch.DispatchNo);
								foreach (var currExpense in currRelatedExpenses)
								{
									_expenseDetailRepository.Delete(currExpense);
								}

								//Delete expense after delete dispatch
								var currRelatedSurcharges =
									_surchargeDetailRepository.Query(p => p.DetailNo == dispatch.DetailNo && p.OrderD == dispatch.OrderD &&
																		p.OrderNo == dispatch.OrderNo && p.DispatchNo == dispatch.DispatchNo);
								foreach (var currRelatedSurcharge in currRelatedSurcharges)
								{
									_surchargeDetailRepository.Delete(currRelatedSurcharge);
								}

								//Delete expense after delete dispatch
								var currRelatedAllowances =
									_allowanceDetailRepository.Query(p => p.DetailNo == dispatch.DetailNo && p.OrderD == dispatch.OrderD &&
																		p.OrderNo == dispatch.OrderNo && p.DispatchNo == dispatch.DispatchNo);
								foreach (var currRelatedAllowance in currRelatedAllowances)
								{
									_allowanceDetailRepository.Delete(currRelatedAllowance);
								}
							}
						}
						#endregion delete dispatch

						#region update dispatch expense
						//Update Dispatch Expense
						if (order.TransportConfirmExpensesList != null)
						{
							if (order.TransportConfirmExpensesList.Count > 0)
							{
								foreach (var cConfirmExpense in order.TransportConfirmExpensesList)
								{
									if (cConfirmExpense != null)
									{
										if (cConfirmExpense.ExpenseDetailList != null)
										{
											if (cConfirmExpense.ExpenseDetailList.Count > 0)
											{
												var expenseNo = GetBaseExpenseNo(cOrder.OrderD, cOrder.OrderNo, cContainer.DetailNo, cConfirmExpense.DispatchNo);
												foreach (var cExpense in cConfirmExpense.ExpenseDetailList)
												{
													ExpenseDetailViewModel expense = cExpense;
													TransportConfirmExpenseViewModel confirmExpense = cConfirmExpense;
													var existExpense =
															_expenseDetailRepository.Query(c => c.OrderD == cOrder.OrderD && c.OrderNo == cOrder.OrderNo &&
																							c.DetailNo == cContainer.DetailNo && c.DispatchNo == confirmExpense.DispatchNo && 
																							c.ExpenseC == expense.ExpenseC && c.ExpenseNo == expense.ExpenseNo)
																.FirstOrDefault();
													var mExpense = Mapper.Map<ExpenseDetailViewModel, Expense_D>(cExpense);
													mExpense.OrderD = cOrder.OrderD;
													mExpense.OrderNo = cOrder.OrderNo;
													mExpense.DetailNo = cContainer.DetailNo;
													mExpense.DispatchNo = confirmExpense.DispatchNo;
													if (existExpense != null)
													{
														mExpense.ExpenseNo = existExpense.ExpenseNo;
														_expenseDetailRepository.Detach(existExpense);
														_expenseDetailRepository.Update(mExpense);
													}
													else
													{
														mExpense.ExpenseNo = expenseNo++;
														_expenseDetailRepository.Add(mExpense);
													}
												}
											}
										}
										var currExpenses = _expenseDetailRepository.Query(c => c.OrderD == cOrder.OrderD && c.OrderNo == cOrder.OrderNo &&
																									c.DetailNo == cContainer.DetailNo && c.DispatchNo == cConfirmExpense.DispatchNo);
										//do for delete dispatch
										foreach (var currExpense in currExpenses)
										{
											Expense_D expense = currExpense;
											if (cConfirmExpense.ExpenseDetailList != null)
											{
												var delExpense = cConfirmExpense.ExpenseDetailList.FirstOrDefault(p => p.ExpenseNo == expense.ExpenseNo);
												if (delExpense == null)
												{
													_expenseDetailRepository.Delete(currExpense);
												}
											}
											else
											{
												_expenseDetailRepository.Delete(currExpense);
											}
										}
									}
								}
							}
						}
						#endregion update dispatch expense

						#region update allowance
						//Update Allowance
						if (order.TransportConfirmAllowanceList != null)
						{
							if (order.TransportConfirmAllowanceList.Count > 0)
							{
								foreach (var cConfirmAllowance in order.TransportConfirmAllowanceList)
								{
									if (cConfirmAllowance != null)
									{
										if (cConfirmAllowance.ExpenseDetailList != null)
										{
											if (cConfirmAllowance.ExpenseDetailList.Count > 0)
											{
												var allowanceNo = GetBaseAllowanceNo(cOrder.OrderD, cOrder.OrderNo, cContainer.DetailNo, cConfirmAllowance.DispatchNo);
												foreach (var cExpense in cConfirmAllowance.ExpenseDetailList)
												{
													AllowanceDetailViewModel expense = cExpense;
													TransportConfirmAllowanceViewModel confirmExpense = cConfirmAllowance;
													var existExpense =
															_allowanceDetailRepository.Query(c => c.OrderD == cOrder.OrderD && c.OrderNo == cOrder.OrderNo &&
																							c.DetailNo == cContainer.DetailNo && c.DispatchNo == confirmExpense.DispatchNo && 
																							c.AllowanceC == expense.ExpenseC && c.AllowanceNo == expense.AllowanceNo)
																.FirstOrDefault();
													var mExpense = Mapper.Map<AllowanceDetailViewModel, Allowance_D>(cExpense);
													mExpense.OrderD = cOrder.OrderD;
													mExpense.OrderNo = cOrder.OrderNo;
													mExpense.DetailNo = cContainer.DetailNo;
													mExpense.DispatchNo = confirmExpense.DispatchNo;
													if (existExpense != null)
													{
														mExpense.AllowanceNo = existExpense.AllowanceNo;
														_allowanceDetailRepository.Detach(existExpense);
														_allowanceDetailRepository.Update(mExpense);
													}
													else
													{
														mExpense.AllowanceNo = allowanceNo++;
														_allowanceDetailRepository.Add(mExpense);
													}
												}
											}
										}
										var currExpenses = _allowanceDetailRepository.Query(c => c.OrderD == cOrder.OrderD && c.OrderNo == cOrder.OrderNo &&
																									c.DetailNo == cContainer.DetailNo && c.DispatchNo == cConfirmAllowance.DispatchNo);
										//do for delete allowance
										foreach (var currExpense in currExpenses)
										{
											Allowance_D expense = currExpense;
											if (cConfirmAllowance.ExpenseDetailList != null)
											{
												var delExpense = cConfirmAllowance.ExpenseDetailList.FirstOrDefault(p => p.AllowanceNo == expense.AllowanceNo);
												if (delExpense == null)
												{
													_allowanceDetailRepository.Delete(currExpense);
												}
											}
											else
											{
												_allowanceDetailRepository.Delete(currExpense);
											}
											
										}
									}
								}
							}
						}
						#endregion update allowance

						#region update surcharge
						//Update Surcharge
						if (order.TransportConfirmSurchargeList != null)
						{
							if (order.TransportConfirmSurchargeList.Count > 0)
							{
								var conSurcharges = order.TransportConfirmSurchargeList.Where(p => p.DispatchNo != 0);
								foreach (var cConfirmSurcharge in conSurcharges)
								{
									if (cConfirmSurcharge != null)
									{
										if (cConfirmSurcharge.ExpenseDetailList != null)
										{
											if (cConfirmSurcharge.ExpenseDetailList.Count > 0)
											{
												var expenseNo = GetBaseSurchargeNo(cOrder.OrderD, cOrder.OrderNo, cContainer.DetailNo, cConfirmSurcharge.DispatchNo);
												foreach (var cExpense in cConfirmSurcharge.ExpenseDetailList)
												{
													SurchargeDetailViewModel expense = cExpense;
													TransportConfirmSurchargeViewModel confirmExpense = cConfirmSurcharge;
													var existExpense =
															_surchargeDetailRepository.Query(c => c.OrderD == cOrder.OrderD && c.OrderNo == cOrder.OrderNo &&
																							c.DetailNo == cContainer.DetailNo && c.DispatchNo == confirmExpense.DispatchNo 
																							&& c.SurchargeC == expense.ExpenseC && c.SurchargeNo == expense.SurchargeNo)
																.FirstOrDefault();
													var mExpense = Mapper.Map<SurchargeDetailViewModel, Surcharge_D>(cExpense);
													mExpense.OrderD = cOrder.OrderD;
													mExpense.OrderNo = cOrder.OrderNo;
													mExpense.DetailNo = cContainer.DetailNo;
													mExpense.DispatchNo = confirmExpense.DispatchNo;
													mExpense.DispatchI = "1";
													if (existExpense != null)
													{
														mExpense.SurchargeNo = existExpense.SurchargeNo;
														_surchargeDetailRepository.Detach(existExpense);
														_surchargeDetailRepository.Update(mExpense);
													}
													else
													{
														mExpense.SurchargeNo = expenseNo++;
														_surchargeDetailRepository.Add(mExpense);
													}
												}
											}
										}
										var currExpenses = _surchargeDetailRepository.Query(c => c.OrderD == cOrder.OrderD && c.OrderNo == cOrder.OrderNo &&
																									c.DetailNo == cContainer.DetailNo && c.DispatchNo == cConfirmSurcharge.DispatchNo);
										//do for delete surcharge
										foreach (var currExpense in currExpenses)
										{
											Surcharge_D expense = currExpense;
											if (cConfirmSurcharge.ExpenseDetailList != null)
											{
												var delExpense = cConfirmSurcharge.ExpenseDetailList.FirstOrDefault(p => p.SurchargeNo == expense.SurchargeNo);
												if (delExpense == null)
												{
													_surchargeDetailRepository.Delete(currExpense);
												}
											}
											else
											{
												_surchargeDetailRepository.Delete(currExpense);
											}
										}
									}
								}
							}
						}
						#endregion update surcharge
					}
					#endregion update dispatch

					#region update container surcharge
					//Update Container Surcharge
					if (order.TransportConfirmSurchargeList != null)
					{
						if (order.TransportConfirmSurchargeList.Count > 0)
						{
							var cSurcharge = order.TransportConfirmSurchargeList.FirstOrDefault(p => p.DispatchNo == 0);
							if (cSurcharge != null)
							{
								if (cSurcharge.ExpenseDetailList.Count > 0)
								{
									var expenseNo = GetBaseSurchargeNo(cOrder.OrderD, cOrder.OrderNo, cContainer.DetailNo, cSurcharge.DispatchNo);
									foreach (var cExpense in cSurcharge.ExpenseDetailList)
									{
										SurchargeDetailViewModel expense = cExpense;
										TransportConfirmSurchargeViewModel confirmExpense = cSurcharge;
										var existExpense =
												_surchargeDetailRepository.Query(c => c.OrderD == cOrder.OrderD && c.OrderNo == cOrder.OrderNo &&
																				c.DetailNo == cContainer.DetailNo && c.DispatchNo == confirmExpense.DispatchNo && c.SurchargeC == expense.ExpenseC)
													.FirstOrDefault();
										var mExpense = Mapper.Map<SurchargeDetailViewModel, Surcharge_D>(cExpense);
										mExpense.OrderD = cOrder.OrderD;
										mExpense.OrderNo = cOrder.OrderNo;
										mExpense.DetailNo = cContainer.DetailNo;
										mExpense.DispatchNo = confirmExpense.DispatchNo;
										mExpense.DispatchI = "0";
										if (existExpense != null)
										{
											mExpense.SurchargeNo = existExpense.SurchargeNo;
											_surchargeDetailRepository.Detach(existExpense);
											_surchargeDetailRepository.Update(mExpense);
										}
										else
										{
											mExpense.SurchargeNo = expenseNo++;
											_surchargeDetailRepository.Add(mExpense);
										}
									}
								}

								var currExpenses = _surchargeDetailRepository.Query(c => c.OrderD == cOrder.OrderD && c.OrderNo == cOrder.OrderNo &&
																								c.DetailNo == cContainer.DetailNo && c.DispatchNo == 0);
								//do for delete dispatch
								foreach (var currExpense in currExpenses)
								{
									Surcharge_D expense = currExpense;
									var delExpense = cSurcharge.ExpenseDetailList.FirstOrDefault(p => p.SurchargeNo == expense.SurchargeNo);
									if (delExpense == null)
									{
										_surchargeDetailRepository.Delete(currExpense);
									}
								}
							}
						}
					}
					#endregion update container surcharge
				}
				SaveContainer();

				SetQuantityContainerAndUnitPrice(cOrder.OrderD, cOrder.OrderNo);

				// set OrderType
                //var dispatchList = _dispatchRepository.Query(d => d.OrderD == cOrder.OrderD && d.OrderNo == cOrder.OrderNo).ToList();
                //for (var iloop = 0; iloop < dispatchList.Count; iloop++)
                //{
                //    var dispatchUpdate = dispatchList[iloop];
                //    dispatchUpdate.OrderTypeI = cOrder.OrderTypeI;
                //    _dispatchRepository.Update(dispatchUpdate);
                //}
                //SaveContainer();
			}
		}

		private int GetBaseDispatchNo(DateTime orderD, string orderNo, int detailNo)
		{
			var baseNo = 0;
			var dispatches = _dispatchRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo).ToList();
			baseNo = dispatches.Count == 0 ? 1 : (dispatches.Max(u => u.DispatchNo) + 1);
			return baseNo;
		}

		private int GetBaseExpenseNo(DateTime orderD, string orderNo, int detailNo, int dispatchNo)
		{
			var baseNo = 0;
			var expenses = _expenseDetailRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo && p.DispatchNo == dispatchNo).ToList();
			baseNo = expenses.Count == 0 ? 1 : (expenses.Max(u => u.ExpenseNo) + 1);
			return baseNo;
		}

		private int GetBaseAllowanceNo(DateTime orderD, string orderNo, int detailNo, int dispatchNo)
		{
			var baseNo = 0;
			var expenses = _allowanceDetailRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo && p.DispatchNo == dispatchNo).ToList();
			baseNo = expenses.Count == 0 ? 1 : (expenses.Max(u => u.AllowanceNo) + 1);
			return baseNo;
		}

		private int GetBaseSurchargeNo(DateTime orderD, string orderNo, int detailNo, int dispatchNo)
		{
			var baseNo = 0;
			var surcharges = _surchargeDetailRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo && p.DispatchNo == dispatchNo).ToList();
			baseNo = surcharges.Count == 0 ? 1 : (surcharges.Max(u => u.SurchargeNo) + 1);
			return baseNo;
		}

		private int FindIndex(DateTime orderD, string orderNo, int detailNo)
		{
			//var dispatches = _dispatchRepository.GetAllQueryable();
			var dispatches = from c in _containerRepository.GetAllQueryable()
							 join d in _orderRepository.GetAllQueryable() on new { c.OrderD, c.OrderNo }
								 equals new { d.OrderD, d.OrderNo } into cd
							 from d in cd.DefaultIfEmpty()
							 join h in _dispatchRepository.GetAllQueryable() on new { c.OrderD, c.OrderNo, c.DetailNo }
								 equals new { h.OrderD, h.OrderNo, h.DetailNo } into cdh
							 from h in cdh.DefaultIfEmpty()
							 select new ContainerDetailViewModel()
							 {
								 OrderD = c.OrderD,
								 OrderNo = c.OrderNo,
								 DetailNo = c.DetailNo,
							 };
			var index = 0;
			var totalRecords = dispatches.Count();
			var halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
			var loopCapacity = 100;
			var recordsToSkip = 0;
			if (totalRecords > 0)
			{
				//var nextIteration = true;
				//while (nextIteration)
				//{
				for (var counter = 0; counter < 2; counter++)
				{
					recordsToSkip = recordsToSkip + (counter * halfCount);

					if (dispatches.OrderBy("OrderD descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.OrderNo == orderNo && c.OrderD == orderD && c.DetailNo == detailNo))
					{
						if (halfCount > loopCapacity)
						{
							totalRecords = totalRecords - (halfCount * 1);
							halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
							break;
						}
						foreach (var dispatch in dispatches.OrderBy("OrderD descending").Skip(recordsToSkip).Take(halfCount))
						{
							if (dispatch.OrderNo == orderNo && dispatch.OrderD == orderD && dispatch.DetailNo == detailNo)
							{
								index = index + 1;
								index = recordsToSkip + index;
								break;
							}
							index = index + 1;
						}
						//nextIteration = false;
						break;
					}
				}
				//}
			}
			return index;
		}

		private bool CheckDetailNoMax(DateTime orderD, string orderNo, int detailNo)
		{
			var detail = _containerRepository.Query(o => o.OrderD == orderD && o.OrderNo == orderNo);
			if (detail.Any())
			{
				var detailNoMax = detail.Max(p => p.DetailNo);
				return detailNo >= (detailNoMax + 1);
			}
			return true;
		}

		private bool CheckDetailNoMin(DateTime orderD, string orderNo, int detailNo)
		{
			var detail = _containerRepository.Query(o => o.OrderD == orderD && o.OrderNo == orderNo);
			if (detail.Any())
			{
				var detailNoMin = detail.Min(p => p.DetailNo);
				return detailNo <= detailNoMin;
			}
			return true;
		}

		public object GetOrderDetailNo(DateTime date, string orderNo)
		{
			var newOrderDetailNo = 0;

			//get orders from db
			var containers = _containerRepository.Query(c => c.OrderD == date.Date && c.OrderNo == orderNo).ToList();

			//if orders != null, that means exist oldMaxOrderId
			if (containers.Count() != 0)
			{
				var oldMaxOrderDetailNo = containers.Max(u => u.DetailNo);
				newOrderDetailNo = oldMaxOrderDetailNo + 1;
			}
			else //else set oldMaxOrderId = 0
			{
				newOrderDetailNo += 1;
			}

			//Check min, max OrderNo
			var isMaxOrderDetailNo = CheckDetailNoMax(date, orderNo, newOrderDetailNo);
			var isMinOrderDetailNo = CheckDetailNoMin(date, orderNo, newOrderDetailNo);

			return new { newOrderDetailNo, isMaxOrderDetailNo, isMinOrderDetailNo };
		}

		public int DeleteContainer(DateTime orderD, string orderNo, int detailNo, bool isConfirmedDeleting)
		{
			var dispatchToRemoves = _dispatchRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo);

			var isDeletedContainer = IsDeleteContainerForTransportConfirm(dispatchToRemoves);

			if (isDeletedContainer == Convert.ToInt32(DeleteLevel.NotDeleted) ||
				(isDeletedContainer == Convert.ToInt32(DeleteLevel.NotDeletedAndWarning) && !isConfirmedDeleting) ||
				(isDeletedContainer == Convert.ToInt32(DeleteLevel.Deleted) && !isConfirmedDeleting))
			{
				return isDeletedContainer;
			}

			DeleteContainerProcessing(orderD, orderNo, detailNo);
			SaveContainer();
			SetQuantityContainerAndUnitPrice(orderD, orderNo);
			return isDeletedContainer;
		}

		public int CheckContainerDeleting(DateTime orderD, string orderNo, int detailNo)
		{
			var containerDispatches = _dispatchRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo);

			return IsDeleteContainer(containerDispatches);
		}

		private int IsDeleteContainer(IEnumerable<Dispatch_D> dispatches)
		{
			//toBeDeleted will have 3 levels:
			// "0": can't deleted
			// "1": can deleted but warning
			// "2": can deleted and not warning
			var toBeDeleted = Convert.ToInt32(DeleteLevel.NotDeleted);
			dispatches = dispatches.ToList();

			if (dispatches.Any())
			{
				foreach (var dispatchD in dispatches)
				{
					if (dispatchD.DispatchStatus == Convert.ToInt32(DispatchStatus.Transported).ToString() ||
						dispatchD.DispatchStatus == Convert.ToInt32(DispatchStatus.Confirmed).ToString())
					{
						toBeDeleted = Convert.ToInt32(DeleteLevel.NotDeleted);
						break;
					}
					if (dispatchD.DispatchStatus == Convert.ToInt32(DispatchStatus.Dispatch).ToString())
					{
						toBeDeleted = Convert.ToInt32(DeleteLevel.NotDeletedAndWarning);
					}
					else if (dispatchD.DispatchStatus == Convert.ToInt32(DispatchStatus.NotDispatch).ToString())
					{
						if (toBeDeleted != Convert.ToInt32(DeleteLevel.NotDeletedAndWarning))
						{
							toBeDeleted = Convert.ToInt32(DeleteLevel.Deleted);
						}
					}
					//if DispatchI is Transported and Confirmed, user can't deleted
				}
				return toBeDeleted;
			}

			//if dispatches doesn't have any value, can deleted and not warning
			return Convert.ToInt32(DeleteLevel.Deleted); ;
		}

		private int IsDeleteContainerForTransportConfirm(IEnumerable<Dispatch_D> dispatches)
		{
			//toBeDeleted will have 2 levels:
			// "0": can't deleted
			// "2": can deleted and not warning
			var toBeDeleted = Convert.ToInt32(DeleteLevel.Deleted);
			dispatches = dispatches.ToList();

			if (dispatches.Any())
			{
				foreach (var dispatchD in dispatches)
				{
					if (dispatchD.DispatchStatus == Convert.ToInt32(DispatchStatus.Confirmed).ToString())
					{
						if (dispatchD.InvoiceStatus == Convert.ToInt32(InvoiceStatus.Issued).ToString())
						{
							toBeDeleted = Convert.ToInt32(DeleteLevel.NotDeleted);
							break;
						}
					}
					else
					{
						toBeDeleted = Convert.ToInt32(DeleteLevel.Deleted);
					}
				}
				return toBeDeleted;
			}

			//if dispatches doesn't have any value, can deleted and not warning
			return Convert.ToInt32(DeleteLevel.Deleted); ;
		}

		public void DeleteContainerProcessing(DateTime orderD, string orderNo, int detailNo)
		{
			var containerToRemove = _containerRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo).FirstOrDefault();
			var dispatchToRemoves = _dispatchRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo);
			
			var expenseDs = _expenseDetailRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo);
			if (expenseDs != null)
			{
				foreach (var expenseD in expenseDs)
				{
					_expenseDetailRepository.Delete(expenseD);
				}
			}

			var surchargeDs = _surchargeDetailRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo);
			if (surchargeDs != null)
			{
				foreach (var surchargeD in surchargeDs)
				{
					_surchargeDetailRepository.Delete(surchargeD);
				}
			}

			var allowanceDs = _allowanceDetailRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo);
			if (allowanceDs != null)
			{
				foreach (var allowanceD in allowanceDs)
				{
					_allowanceDetailRepository.Delete(allowanceD);
				}
			}

			if (dispatchToRemoves != null)
			{
				foreach (var dispatchToRemove in dispatchToRemoves)
				{
					_dispatchRepository.Delete(dispatchToRemove);
				}
			}
			if (containerToRemove != null)
			{
				_containerRepository.Delete(containerToRemove);
			}
		}

		public int? GetDetainDay(DateTime orderD, string orderNo, int detailNo, string containerNo)
		{
			var order =
				_containerRepository.Query(
					p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo && p.ContainerNo == containerNo)
					.FirstOrDefault();
			return (order != null ? (order.DetainDay ?? 0) : 0);
		}
		public void SaveContainer()
		{
			_unitOfWork.Commit();
		}
	}
}
