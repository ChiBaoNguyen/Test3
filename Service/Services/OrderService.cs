using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Permissions;
using AutoMapper;
using AutoMapper.Internal;
using CrystalReport.Dataset.Dispatch;
using CrystalReport.Dataset.Order;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Root.Models.Mapping;
using Service.Core;
using Website.Enum;
using Website.ViewModels.Common;
using Website.ViewModels.Container;
using Website.ViewModels.Order;
using System.IO;
using System.Data;
using Website.Utilities;
using System.Globalization;

namespace Service.Services
{
	public interface IOrderService
	{
		IEnumerable<OrderViewModel> GetOrders();
		object GetOrderEntryId(DateTime date);
		OrderDatatable GetOrdersForTable(int page, int itemsPerPage, string sortBy, bool reverse,
				string orderSearchValue, string orderTypeI, DateTime? formDate, DateTime? toDate, string custMainC, string custSubC);
		OrderViewModel GetOrder(string no, DateTime date);
		TransportConfirmOrderViewModel GetTransportConfirmOrder(DateTime date, string ordedNo);
		ResponseStatus CreateOrder(OrderViewModel order);
		void UpdateOrder(OrderViewModel order);
		int DeleteOrder(DateTime orderD, string orderNo, bool isConfirmedDeleting);
		DateTime? GetOrderDateMax();
        OrderViewModel GetLocationInOrder(string no, DateTime date);
		DateTime? GetRevenueDateMax(string customerMainC, string customerSubC);
		void SaveOrder();
		DateTime? GetPartnerInvoiceDateMax(string partnerMainC, string partnerSubC);

	}
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IContainerTypeRepository _containerTypeRepository;
		private readonly IContainerRepository _containerRepository;
		private readonly IDispatchRepository _dispatchRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly IOrderPatternRepository _orderPatternRepository;
		private readonly IShippingCompanyRepository _shippingCompanyRepository;
		private readonly IExpenseDetailRepository _expenseDetailRepository;
		private readonly ISurchargeDetailRepository _surchargeDetailRepository;
		private readonly IAllowanceDetailRepository _allowanceDetailRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly IUnitOfWork _unitOfWork;

		public OrderService(IOrderRepository orderRepository, IContainerTypeRepository containerTypeRepository, IContainerRepository containerRepository,
							IOrderPatternRepository orderPatternRepository, ICustomerRepository customerRepository, IDispatchRepository dispatchRepository,
							IEmployeeRepository employeeRepository, IShippingCompanyRepository shippingCompanyRepository, 
							IExpenseDetailRepository expenseDetailRepository, ISurchargeDetailRepository surchargeDetailRepository, IAllowanceDetailRepository allowanceDetailRepository, 
							ILocationRepository locationRepository, IUnitOfWork unitOfWork)
		{
			this._orderRepository = orderRepository;
			this._containerTypeRepository = containerTypeRepository;
			this._dispatchRepository = dispatchRepository;
			this._containerRepository = containerRepository;
			this._customerRepository = customerRepository;
			this._orderPatternRepository = orderPatternRepository;
			this._employeeRepository = employeeRepository;
			this._shippingCompanyRepository = shippingCompanyRepository;
			this._expenseDetailRepository = expenseDetailRepository;
			this._surchargeDetailRepository = surchargeDetailRepository;
			this._allowanceDetailRepository = allowanceDetailRepository;
			this._locationRepository = locationRepository;
			this._unitOfWork = unitOfWork;
		}
		public IEnumerable<OrderViewModel> GetOrders()
		{
			var orders = _orderRepository.GetAll();
			if (orders != null)
			{
				var destination = Mapper.Map<IEnumerable<Order_H>, IEnumerable<OrderViewModel>>(orders);
				return destination;
			}
			return null;
		}
        public OrderViewModel GetLocationInOrder(string no, DateTime date)
        {
            var orders = _orderRepository.Query(o => o.OrderNo.Equals(no) && o.OrderD == date).ToList();
            var mOrder = new OrderViewModel();
            if (orders.Count <= 0)
            {
                return null;
            }
            else
            {
                mOrder.DischargePlaceN = orders.FirstOrDefault().DischargePlaceN ?? string.Empty;
                mOrder.DischargePlaceC = orders.FirstOrDefault().DischargePlaceC ?? string.Empty;
            }

            return mOrder;
        }
		public object GetOrderEntryId(DateTime date)
		{
			var oldMaxOrderNo = "0";
			var newOrderNo = "";

			//get orders from db
			var orders = _orderRepository.Query(or => or.OrderD == date.Date).ToList();

			//if orders != null, that means exist oldMaxOrderId
			if (orders.Count() != 0)
			{
				//oldMaxOrderId = orders.Max(u => Convert.ToInt64(u.OrderEntryID));
				oldMaxOrderNo = orders.Max(u => u.OrderNo);
				newOrderNo = OrderManagement.FormatOrderEntryId_v2(oldMaxOrderNo.ToNullSafeString(), 7);
			}
			else //else set oldMaxOrderId = 0
			{
				newOrderNo = OrderManagement.FormatOrderEntryId_v2(oldMaxOrderNo.ToNullSafeString(), 7);

			}

			//Check min, max OrderNo
			var isMaxOrderNo = CheckOrderNoMax(date, newOrderNo);
			var isMinOrderNo = CheckOrderNoMin(date, newOrderNo);

			return new { newOrderNo, isMaxOrderNo, isMinOrderNo };
		}

		public OrderDatatable GetOrdersForTable(int page, int itemsPerPage, string sortBy, bool reverse, string orderSearchValue, string orderTypeI, DateTime? fromDate, DateTime? toDate, string custMainC, string custSubC)
		{
			var orders =
				_orderRepository.Query(
					p =>
						(orderTypeI == "-1" || p.OrderTypeI.Equals(orderTypeI)) 
						&& (string.IsNullOrEmpty(custMainC) || (p.CustomerMainC == custMainC & p.CustomerSubC == custSubC))
						);

			// searching for value
			if (!string.IsNullOrWhiteSpace(orderSearchValue))
			{
				orderSearchValue = orderSearchValue.ToLower();
				orders = orders.Where(or => or.OrderNo.ToLower().Contains(orderSearchValue) ||
											or.ShippingCompanyN != null && or.ShippingCompanyN.ToLower().Contains(orderSearchValue) ||
											or.BLBK != null && or.BLBK.ToLower().Contains(orderSearchValue) ||
											or.LoadingPlaceN != null && or.LoadingPlaceN.ToLower().Contains(orderSearchValue) ||
											or.StopoverPlaceN != null && or.StopoverPlaceN.ToLower().Contains(orderSearchValue) ||
											or.DischargePlaceN != null && or.DischargePlaceN.ToLower().Contains(orderSearchValue) ||
											or.Description.ToLower().Contains(orderSearchValue) ||
											or.JobNo != null && or.JobNo.ToLower().Contains(orderSearchValue) ||
											or.ContractNo != null && or.ContractNo.ToLower().Contains(orderSearchValue));
			}
			//fromDate searching
			if (fromDate != null)
			{
				orders = orders.Where(or => or.OrderD >= fromDate);
			}
			if (toDate != null)
			{
				orders = orders.Where(or => or.OrderD <= toDate);
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			//customers = customers.AsQueryable().OrderBy("\"" + sortBy + (reverse ? " desc\"" : " asc\"")).AsEnumerable();
			var customersOrdered = orders.OrderBy(sortBy + (reverse ? " descending" : ""));
			// paging
			var ordersPaged = customersOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var destination = Mapper.Map<List<Order_H>, List<OrderViewModel>>(ordersPaged);

			if (destination.Any())
			{
				foreach (var order in destination)
				{
					var customer =
						_customerRepository.Query(p => p.CustomerMainC == order.CustomerMainC && p.CustomerSubC == order.CustomerSubC)
							.FirstOrDefault();
					if (customer != null)
					{
						order.CustomerN = customer.CustomerN;
						order.CustomerShortN = customer.CustomerShortN;
					}
					var customerPay =
						_customerRepository.Query(p => p.CustomerMainC == order.CustomerPayLiftLoweredMainC && p.CustomerSubC == order.CustomerPayLiftLoweredSubC)
							.FirstOrDefault();
					if (customerPay != null)
					{
						order.CustomerPayLiftLoweredN = customerPay.CustomerShortN;
					}
					DateTime oD = order.OrderD;
					string oNo = order.OrderNo;
					int cont20 = (_containerRepository.Query(p => p.OrderD == oD && p.OrderNo == oNo && p.ContainerSizeI == "0").ToList().Count());
					order.ContSize20 = cont20 > 0 ? cont20.ToString() : "";
					int cont40 = _containerRepository.Query(p => p.OrderD == oD && p.OrderNo == oNo && p.ContainerSizeI == "1").ToList().Count();
					order.ContSize40 = cont40 > 0 ? cont40.ToString() : "";
					int cont45 = _containerRepository.Query(p => p.OrderD == oD && p.OrderNo == oNo && p.ContainerSizeI == "2").ToList().Count();
					order.ContSize45 = cont45 > 0 ? cont45.ToString() : "";
					var oDLoad = _containerRepository.Query(p => p.OrderD == oD && p.OrderNo == oNo && p.ContainerSizeI == "3" && p.GrossWeight != null && p.GrossWeight > 0).ToList();
					int contload = oDLoad.Count();
					decimal? weight = oDLoad.Sum(p => p.GrossWeight);
					order.ContSizeLoad = contload > 0 ? (contload.ToString() + (weight > 0 ? ("(" + weight.Value.ToString("#,###.#") + ")") : "")) : "";
				}
			}

			var orderDatatable = new OrderDatatable()
			{
				Data = destination,
				Total = orders.Count()
			};
			return orderDatatable;
		}

		public OrderViewModel GetOrder(string no, DateTime date)
		{
			var orders = _orderRepository.Query(o => o.OrderNo.Equals(no) && o.OrderD == date).ToList();
			if (orders.Count <= 0) return null;
			var mOrder = Mapper.Map<Order_H, OrderViewModel>(orders.FirstOrDefault());

			var employee =
				_employeeRepository.Query(p => p.EmployeeC == mOrder.EntryClerkC)
					.Select(p => new { p.EmployeeFirstN, p.EmployeeLastN, p.RetiredD }).FirstOrDefault();
			if (employee != null)
			{
				mOrder.EntryClerkN = employee.EmployeeLastN + " " + employee.EmployeeFirstN;
				mOrder.RetiredD = employee.RetiredD;
			}

			var customer =
				_customerRepository.Query(p => p.CustomerMainC == mOrder.CustomerMainC && p.CustomerSubC == mOrder.CustomerSubC)
					.Select(p => new { p.CustomerN, p.IsCollected}).FirstOrDefault();
			if (customer != null)
			{
				mOrder.CustomerN = customer.CustomerN;
				mOrder.IsCollectedByMasterCustomer = customer.IsCollected;
			}

			var customerPay =
				_customerRepository.Query(p => p.CustomerMainC == mOrder.CustomerPayLiftLoweredMainC && p.CustomerSubC == mOrder.CustomerPayLiftLoweredSubC)
					.Select(p => new { p.CustomerN }).FirstOrDefault();
			if (customerPay != null)
			{
				mOrder.CustomerPayLiftLoweredN = customerPay.CustomerN;
			}

			var pattern =
				_orderPatternRepository.Query(
					p =>
						p.CustomerMainC == mOrder.CustomerMainC && p.CustomerSubC == mOrder.CustomerSubC &&
						p.OrderPatternC == mOrder.OrderPatternC)
					.FirstOrDefault();
			if (pattern != null)
			{
				mOrder.OrderPatternN = pattern.OrderPatternN;
			}

			mOrder.OrderIndex = FindIndex(mOrder.OrderD, mOrder.OrderNo);

			//get containers
			var containers = _containerRepository.Query(c => c.OrderNo.Equals(mOrder.OrderNo) && c.OrderD == mOrder.OrderD).ToList();
			//if (containers.Count > 0)
			//{
			//	foreach (var container in containers)
			//	{
			//		var mContainer = Mapper.Map<Order_D, ContainerViewModel>(container);
			//		mCOntainers.Add(mContainer);
			//	}
			//	mOrder.Containers = mCOntainers;
			//}
			if (containers.Any())
			{
				var mContainers = (from c in containers
								   join t in _containerTypeRepository.GetAllQueryable() on c.ContainerTypeC equals t.ContainerTypeC into ct
								   from t in ct.DefaultIfEmpty()
								   select new ContainerViewModel()
								   {
									   OrderD = c.OrderD,
									   OrderNo = c.OrderNo,
									   DetailNo = c.DetailNo,
									   ContainerNo = c.ContainerNo,
									   ContainerSizeI = c.ContainerSizeI,
									   ContainerTypeC = c.ContainerTypeC,
									   ContainerTypeN = t != null ? t.ContainerTypeN : "",
									   CommodityC = c.CommodityC,
									   CommodityN = c.CommodityN,
									   NetWeight = c.NetWeight,
									   GrossWeight = c.GrossWeight,
									   TotalPrice = c.TotalPrice,
									   UnitPrice = c.UnitPrice,
									   SealNo = c.SealNo,
									   Description = c.Description,
									   EstimatedWeight = c.EstimatedWeight,
									   CalculateByTon = c.CalculateByTon,
								   }).ToList();

				mOrder.Containers = mContainers;
			}

			//Check min, max OrderNo
			mOrder.IsMaxOrderNo = CheckOrderNoMax(date, no);
			mOrder.IsMinOrderNo = CheckOrderNoMin(date, no);

			return mOrder;
		}

		private int GetContainerDetailNo(DateTime orderD, string orderNo)
		{
			var baseConNo = 0;
			var cons = _containerRepository.Query(con => con.OrderD == orderD && con.OrderNo == orderNo).ToList();
			baseConNo = cons.Count == 0 ? 1 : (cons.Max(u => u.DetailNo) + 1);
			return baseConNo;
		}

		public ResponseStatus CreateOrder(OrderViewModel order)
		{
			var isDuplicatedOrder = false;
			var mOrder = Mapper.Map<OrderViewModel, Order_H>(order);

			var newestOrderObj = GetOrderEntryId(order.OrderD);
			var newestOrderNo = Convert.ToString(newestOrderObj.GetType().GetProperty("newOrderNo").GetValue(newestOrderObj, null));

			if (newestOrderNo != order.OrderNo)
			{
				isDuplicatedOrder = true;
				mOrder.OrderNo = newestOrderNo;
			}

			_orderRepository.Add(mOrder);

			if (order.Containers != null && order.Containers.Count != 0)
			{
				var baseConNo = GetContainerDetailNo(mOrder.OrderD, mOrder.OrderNo);

				foreach (var container in order.Containers)
				{
					var con = Mapper.Map<ContainerViewModel, Order_D>(container);
					if(String.IsNullOrEmpty(con.ContainerTypeC))
					{
						return new ResponseStatus()
						{
							Successful = false,
							Message = ""
						};
					}
					else
					{
						con.OrderD = order.OrderD;
						con.OrderNo = mOrder.OrderNo;
						con.DetailNo = baseConNo++;
						con.LocationDispatch1 = mOrder.LoadingPlaceN;
						con.LocationDispatch2 = mOrder.StopoverPlaceN;
						con.LocationDispatch3 = mOrder.DischargePlaceN;
						con.ContainerNo = con.ContainerNo.ToUpper();
						con.ContainerSizeI = String.IsNullOrEmpty(con.ContainerSizeI) ? "" : con.ContainerSizeI;
						con.ContainerTypeC = String.IsNullOrEmpty(con.ContainerTypeC) ? "" : con.ContainerTypeC;
						con.UnitPrice = con.UnitPrice ?? 0;
						_containerRepository.Add(con);
					}
					
				}
			}
			SaveOrder();

			if (isDuplicatedOrder)
			{
				return new ResponseStatus()
				{
					Successful = true,
					Message = "duplicated",
					NewestOrderNo = newestOrderNo
				};
			}
			return new ResponseStatus()
			{
				Successful = true,
				Message = ""
			};
		}

		public void UpdateOrder(OrderViewModel order)
		{
			var upOrder = Mapper.Map<OrderViewModel, Order_H>(order);
			_orderRepository.Update(upOrder);

			var currContainers = _containerRepository.Query(c => c.OrderD == order.OrderD && c.OrderNo == order.OrderNo).ToList();
			//get baseConNo in case have new containers
			var baseConNo = GetContainerDetailNo(order.OrderD, order.OrderNo);
			if (order.Containers != null)
			{
				if (order.Containers.Count > 0)
				{
					//do for update and insert if have new container
					foreach (var container in order.Containers)
					{
						ContainerViewModel con = container;
						var existContainer =
							_containerRepository.Query(c => c.OrderD == order.OrderD && c.OrderNo == order.OrderNo && c.DetailNo == con.DetailNo)
								.FirstOrDefault();
						var mCon = Mapper.Map<ContainerViewModel, Order_D>(container);
						//for update containers
						if (existContainer != null)
						{
							//_containerRepository.Detach(existContainer);
							existContainer.ContainerNo = container.ContainerNo.ToUpper();
							existContainer.ContainerSizeI = container.ContainerSizeI;
							existContainer.CalculateByTon = container.CalculateByTon;
							existContainer.ContainerTypeC = container.ContainerTypeC;
							existContainer.CommodityC = container.CommodityC;
							existContainer.CommodityN = container.CommodityN;
							existContainer.NetWeight = container.NetWeight;
							existContainer.GrossWeight = container.GrossWeight;
							existContainer.TotalPrice = container.TotalPrice;
							existContainer.UnitPrice = container.UnitPrice ?? 0;
							existContainer.SealNo = container.SealNo;
							existContainer.Description = container.Description;
							existContainer.EstimatedWeight = container.EstimatedWeight;
							existContainer.LocationDispatch1 = upOrder.LoadingPlaceN;
							existContainer.LocationDispatch2 = upOrder.StopoverPlaceN;
							existContainer.LocationDispatch3 = upOrder.DischargePlaceN;
							//_containerRepository.Update(existContainer);
						}
						//for insert new containers
						else
						{
							mCon.OrderD = order.OrderD;
							mCon.OrderNo = order.OrderNo;
							mCon.DetailNo = baseConNo++;
							mCon.ContainerNo = mCon.ContainerNo.ToUpper();
							mCon.ContainerSizeI = String.IsNullOrEmpty(mCon.ContainerSizeI) ? "" : mCon.ContainerSizeI;
							mCon.ContainerTypeC = String.IsNullOrEmpty(mCon.ContainerTypeC) ? "" : mCon.ContainerTypeC;
							mCon.CalculateByTon = String.IsNullOrEmpty(mCon.CalculateByTon) ? "" : mCon.CalculateByTon;
							mCon.UnitPrice = mCon.UnitPrice ?? 0;
                            mCon.LocationDispatch1 = upOrder.LoadingPlaceN;
                            mCon.LocationDispatch2 = upOrder.StopoverPlaceN;
                            mCon.LocationDispatch3 = upOrder.DischargePlaceN;
							_containerRepository.Add(mCon);
						}
					}
				}

				//do for delete container
				foreach (var currContainer in currContainers)
				{
					Order_D container = currContainer;
					var delContainer = order.Containers.FirstOrDefault(p => p.DetailNo == container.DetailNo && p.OrderD == container.OrderD && p.OrderNo == container.OrderNo);
					if (delContainer == null)
					{
						DeleteContainerProcessing(container.OrderD, container.OrderNo, container.DetailNo);
					}
				}
			}

			SaveOrder();

			UpdateDispatchOrderType(order.OrderD, order.OrderNo, order.OrderTypeI);
		}

		public void UpdateDispatchOrderType(DateTime orderD, string orderNo, string orderTypeI) {
			var dispatchList = _dispatchRepository.Query(d => d.OrderD == orderD && d.OrderNo == orderNo).ToList();
			for (var iloop = 0; iloop < dispatchList.Count; iloop++)
			{
				var dispatchUpdate = dispatchList[iloop];
				dispatchUpdate.OrderTypeI = orderTypeI;
				_dispatchRepository.Update(dispatchUpdate);
			}

			SaveOrder();
		}

		public int DeleteOrder(DateTime orderD, string orderNo, bool isConfirmedDeleting)
		{
			var orderToRemove = _orderRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo).FirstOrDefault();
			var containerToRemoves = _containerRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo);
			var dispatchToRemoves = _dispatchRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo);

			//checking Order's Dispatches Status before delete
			var isDeleteOder = IsDeleteOrder(containerToRemoves, dispatchToRemoves);

			//The order will not be deleted with Level.NotDeleted and Level.NotDeleteAndWarning but isDeletedWithoutWarning is false
			if (isDeleteOder == Convert.ToInt32(DeleteLevel.NotDeleted) ||
				(isDeleteOder == Convert.ToInt32(DeleteLevel.NotDeletedAndWarning) && !isConfirmedDeleting) ||
				(isDeleteOder == Convert.ToInt32(DeleteLevel.Deleted) && !isConfirmedDeleting))
			{
				return isDeleteOder;
			}

			//Process deleting Order
			var expenseDs = _expenseDetailRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo);
			if (expenseDs != null)
			{
				foreach (var expenseD in expenseDs)
				{
					_expenseDetailRepository.Delete(expenseD);
				}
			}

			var surchargeDs = _surchargeDetailRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo);
			if (surchargeDs != null)
			{
				foreach (var surchargeD in surchargeDs)
				{
					_surchargeDetailRepository.Delete(surchargeD);
				}
			}

			var allowanceDs = _allowanceDetailRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo);
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
			if (containerToRemoves != null)
			{
				foreach (var containerToRemove in containerToRemoves)
				{
					_containerRepository.Delete(containerToRemove);
				}
			}
			if (orderToRemove != null)
			{
				_orderRepository.Delete(orderToRemove);
			}
			SaveOrder();
			return isDeleteOder;
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
		private int FindIndex(DateTime orderD, string orderNo)
		{
			var orders = _orderRepository.GetAllQueryable();
			var index = 0;
			var totalRecords = orders.Count();
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

						if (orders.OrderBy("OrderD descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.OrderNo == orderNo && c.OrderD == orderD))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var order in orders.OrderBy("OrderD descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (order.OrderNo == orderNo && order.OrderD == orderD)
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

		private bool CheckOrderNoMax(DateTime orderD, string orderNo)
		{
			var orderNoMax = _orderRepository.Query(o => o.OrderD == orderD).Max(u => u.OrderNo);
			if (orderNoMax != null)
			{
				return Convert.ToInt32(orderNo) >= (Convert.ToInt32(orderNoMax) + 1);
			}
			return true;
		}

		private bool CheckOrderNoMin(DateTime orderD, string orderNo)
		{
			var orderNoMin = _orderRepository.Query(o => o.OrderD == orderD).Min(u => u.OrderNo);
			if (orderNoMin != null)
			{
				return Convert.ToInt32(orderNo) <= Convert.ToInt32(orderNoMin);
			}
			return true;
		}

		public TransportConfirmOrderViewModel GetTransportConfirmOrder(DateTime date, string orderNo)
		{
			var order = _orderRepository.Query(p => p.OrderD == date && p.OrderNo == orderNo).FirstOrDefault();
			if (order != null)
			{
				var transportConfirmOrder = Mapper.Map<Order_H, TransportConfirmOrderViewModel>(order);

				var employee =
					_employeeRepository.Query(p => p.EmployeeC == transportConfirmOrder.EntryClerkC)
					.Select(p => new { p.EmployeeFirstN, p.EmployeeLastN, p.RetiredD }).FirstOrDefault();
				if (employee != null)
				{
					transportConfirmOrder.EntryClerkN = employee.EmployeeLastN + " " + employee.EmployeeFirstN;
					transportConfirmOrder.RetiredD = employee.RetiredD;
				}

				var customer =
					_customerRepository.Query(p => p.CustomerMainC == transportConfirmOrder.CustomerMainC && p.CustomerSubC == transportConfirmOrder.CustomerSubC)
					.Select(p => new { p.CustomerN }).FirstOrDefault();
				if (customer != null)
				{
					transportConfirmOrder.CustomerN = customer.CustomerN;
				}

				var customerPayLiffLoweredN = _customerRepository.Query(p => p.CustomerMainC == transportConfirmOrder.CustomerPayLiftLoweredMainC && p.CustomerSubC == transportConfirmOrder.CustomerPayLiftLoweredSubC)
					.Select(p => new { p.CustomerN }).FirstOrDefault();
				if (customerPayLiffLoweredN != null)
				{
					transportConfirmOrder.CustomerPayLiftLoweredN = customerPayLiffLoweredN.CustomerN;
				}

				var pattern =
				_orderPatternRepository.Query(p => p.OrderPatternC == transportConfirmOrder.OrderPatternC)
					.FirstOrDefault();
				if (pattern != null)
				{
					transportConfirmOrder.OrderPatternN = pattern.OrderPatternN;
				}

				var loadingPlaceAreaC = _locationRepository.Query(l => l.LocationC == transportConfirmOrder.LoadingPlaceC).FirstOrDefault();
				if (loadingPlaceAreaC != null)
				{
					transportConfirmOrder.LoadingPlaceAreaC = loadingPlaceAreaC.AreaC;
				}
				var stopoverPlaceAreaC= _locationRepository.Query(l => l.LocationC == transportConfirmOrder.StopoverPlaceC).FirstOrDefault();
				if (stopoverPlaceAreaC != null)
				{
					transportConfirmOrder.StopoverPlaceAreaC = stopoverPlaceAreaC.AreaC;
				}
				var dischargePlaceAreaC= _locationRepository.Query(l => l.LocationC == transportConfirmOrder.DischargePlaceC).FirstOrDefault();
				if (dischargePlaceAreaC != null)
				{
					transportConfirmOrder.DischargePlaceAreaC = dischargePlaceAreaC.AreaC;
				}

				//Check min, max OrderNo
				transportConfirmOrder.IsMaxOrderNo = CheckOrderNoMax(date, orderNo);
				transportConfirmOrder.IsMinOrderNo = CheckOrderNoMin(date, orderNo);

				return transportConfirmOrder;
			}
			return null;
		}

		#region comment
		//public Stream ExportPdf(OrderReportParam param)
		//{
		//	Stream stream;
		//	DataRow row;
		//	OrderList.OrderListDataTable dt;
		//	int intLanguage;
		//	int index;
		//	int totalContainer1; // 20HC
		//	int totalContainer2; // 40HC
		//	int totalContainer3; // 45HC
		//	decimal totalUnirPrice;

		//	// get data
		//	dt = new OrderList.OrderListDataTable();
		//	List<OrderRow> data = GetOrderList(param);
		//	CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
		//	if (param.Laguague == "vi")
		//	{
		//		intLanguage = 1;
		//	}
		//	else if (param.Laguague == "jp")
		//	{
		//		intLanguage = 3;
		//		cul = CultureInfo.GetCultureInfo("ja-JP");
		//	}
		//	else
		//	{
		//		intLanguage = 2;
		//		cul = CultureInfo.GetCultureInfo("en-US");
		//	}

		//	index = 1;
		//	totalContainer1 = 0;
		//	totalContainer2 = 0;
		//	totalContainer3 = 0;
		//	totalUnirPrice = 0;
			
		//	if (data != null && data.Count > 0)
		//	{
		//		for (int iloop = 0; iloop < data.Count; iloop++)
		//		{
		//			if (data[iloop].OrderD != null && data[iloop].OrderD.Count > 0)
		//			{
		//				for (int jloop = 0; jloop < data[iloop].OrderD.Count; jloop++)
		//				{
		//					row = dt.NewRow();
		//					row["No"] = index;
		//					row["OrderD"] = data[iloop].OrderH.OrderD.ToString("dd/MM/yyyy");
		//					row["OrderNo"] = data[iloop].OrderH.OrderNo;
		//					row["CustomerShortN"] = data[iloop].OrderH.CustomerShortN;
		//					row["OrderTypeI"] = Utilities.GetOrderTypeName(data[iloop].OrderH.OrderTypeI);
		//					row["BLBK"] = data[iloop].OrderH.BLBK;
		//					row["ShippingCompanyN"] = data[iloop].OrderH.ShippingCompanyN;
		//					row["LoadingPlaceN"] = data[iloop].OrderH.LoadingPlaceN;
		//					row["StopoverPlaceN"] = data[iloop].OrderH.StopoverPlaceN;
		//					row["DischargePlaceN"] = data[iloop].OrderH.DischargePlaceN;
		//					row["DetailNo"] = data[iloop].OrderD[jloop].DetailNo;
		//					row["ContainerNo"] = data[iloop].OrderD[jloop].ContainerNo;
		//					row["ContainerSizeI"] = Utilities.GetContainerSizeName(data[iloop].OrderD[jloop].ContainerSizeI);
		//					row["ContainerTypeN"] = data[iloop].OrderD[jloop].ContainerTypeN;
		//					row["CommodityN"] = data[iloop].OrderD[jloop].CommodityN;
		//					if (data[iloop].OrderD[jloop].NetWeight != null)
		//					{
		//						row["NetWeight"] = ((decimal)data[iloop].OrderD[jloop].NetWeight).ToString("#,###.#", cul.NumberFormat);
		//					}
		//					else
		//					{
		//						row["NetWeight"] = "";
		//					}
		//					if (data[iloop].OrderD[jloop].GrossWeight != null)
		//					{
		//						row["GrossWeight"] = ((decimal)data[iloop].OrderD[jloop].GrossWeight).ToString("#,###.#", cul.NumberFormat);
		//					}
		//					else
		//					{
		//						row["GrossWeight"] = "";
		//					}
		//					if (data[iloop].OrderD[jloop].UnitPrice != null)
		//					{
		//						row["UnitPrice"] = ((decimal)data[iloop].OrderD[jloop].UnitPrice).ToString("#,###", cul.NumberFormat);
		//					}
		//					else
		//					{
		//						row["UnitPrice"] = "";
		//					}
		//					row["Key"] = row["OrderD"] + row["OrderNo"].ToString();

		//					// set total container
		//					if (data[iloop].OrderD[jloop].ContainerSizeI == Constants.CONTAINERSIZE1)
		//					{
		//						totalContainer1 = totalContainer1 + 1;
		//					}
		//					else if (data[iloop].OrderD[jloop].ContainerSizeI == Constants.CONTAINERSIZE2)
		//					{
		//						totalContainer2 = totalContainer2 + 1;
		//					}
		//					else if (data[iloop].OrderD[jloop].ContainerSizeI == Constants.CONTAINERSIZE3)
		//					{
		//						totalContainer3 = totalContainer3 + 1;
		//					}

		//					// set total unitPrice
		//					if (data[iloop].OrderD[jloop].UnitPrice != null)
		//					{
		//						totalUnirPrice = totalUnirPrice + (decimal)data[iloop].OrderD[jloop].UnitPrice;
		//					}

		//					dt.Rows.Add(row);
		//				}
		//			}
		//			else
		//			{
		//				row = dt.NewRow();
		//				row["No"] = index;
		//				row["OrderD"] = data[iloop].OrderH.OrderD.ToString("dd/MM/yyyy");
		//				row["OrderNo"] = data[iloop].OrderH.OrderNo;
		//				row["CustomerShortN"] = data[iloop].OrderH.CustomerShortN;
		//				row["OrderTypeI"] = Utilities.GetOrderTypeName(data[iloop].OrderH.OrderTypeI);
		//				row["BLBK"] = data[iloop].OrderH.BLBK;
		//				row["ShippingCompanyN"] = data[iloop].OrderH.ShippingCompanyN;
		//				row["LoadingPlaceN"] = data[iloop].OrderH.LoadingPlaceN;
		//				row["StopoverPlaceN"] = data[iloop].OrderH.StopoverPlaceN;
		//				row["DischargePlaceN"] = data[iloop].OrderH.DischargePlaceN;
		//				row["Key"] = row["OrderD"] + row["OrderNo"].ToString();

		//				dt.Rows.Add(row);
		//			}

		//			index++;
		//		}
		//	}

		//	string totalContainer = totalContainer1 + " X " + Constants.CONTAINERSIZE1N + ", " +
		//							totalContainer2 + " X " + Constants.CONTAINERSIZE2N + ", " +
		//							totalContainer3 + " X " + Constants.CONTAINERSIZE3N;
		//	stream = CrystalReport.Service.Order.ExportPdf.Exec(dt,
		//														intLanguage,
		//														(index - 1).ToString(),
		//														totalContainer,
		//														totalUnirPrice.ToString("#,###", cul.NumberFormat)
		//														);
		//	return stream;
		//}

		//public List<OrderRow> GetOrderList(OrderReportParam param)
		//{
		//	List<OrderRow> result = new List<OrderRow>();
		//	OrderRow row;
		//	// get data from Order_D and Order_H
		//	var orderH = from a in _orderRepository.GetAllQueryable()
		//				 join b in _customerRepository.GetAllQueryable() on new { a.CustomerMainC, a.CustomerSubC }
		//					  equals new { b.CustomerMainC, b.CustomerSubC } into t2
		//				 from b in t2.DefaultIfEmpty()
		//				 where ((param.OrderDFrom == null || a.OrderD >= param.OrderDFrom) &
		//						(param.OrderDTo == null || a.OrderD <= param.OrderDTo) &
		//						(param.Customer == "null" || (param.Customer).Contains(a.CustomerMainC + "_" + a.CustomerSubC)) &
		//						(string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || a.OrderDepC == param.DepC) &
		//						(string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || a.OrderTypeI == param.OrderTypeI)
		//					   )
		//				 select new OrderViewModel()
		//				 {
		//					 OrderD = a.OrderD,
		//					 OrderNo = a.OrderNo,
		//					 CustomerShortN = b != null ? b.CustomerShortN : "",
		//					 OrderTypeI = a.OrderTypeI,
		//					 BLBK = a.BLBK,
		//					 ShippingCompanyN = a.ShippingCompanyN,
		//					 LoadingPlaceN = a.LoadingPlaceN,
		//					 StopoverPlaceN = a.StopoverPlaceN,
		//					 DischargePlaceN = a.DischargePlaceN,

		//				 };

		//	orderH = orderH.OrderBy("OrderD asc, OrderNo asc");
		//	var orderHList = orderH.ToList();

		//	if (orderHList.Count > 0)
		//	{
		//		for (int iloop = 0; iloop < orderHList.Count; iloop++)
		//		{
		//			// new row
		//			row = new OrderRow();
		//			// set orderH
		//			row.OrderH = orderHList[iloop];
		//			// set orderD
		//			var orderDParam = orderHList[iloop].OrderD;
		//			var orderNoParam = orderHList[iloop].OrderNo;
		//			var orderD = from a in _containerRepository.GetAllQueryable()
		//						 join b in _containerTypeRepository.GetAllQueryable() on new { a.ContainerTypeC }
		//							  equals new { b.ContainerTypeC } into t2
		//						 from b in t2.DefaultIfEmpty()
		//						 where (a.OrderD == orderDParam & a.OrderNo == orderNoParam)
		//						 select new ContainerViewModel()
		//						 {
		//							 OrderD = a.OrderD,
		//							 OrderNo = a.OrderNo,
		//							 DetailNo = a.DetailNo,
		//							 ContainerNo = a.ContainerNo,
		//							 ContainerSizeI = a.ContainerSizeI,
		//							 ContainerTypeN = b != null ? b.ContainerTypeN : "",
		//							 CommodityN = a.CommodityN,
		//							 NetWeight = a.NetWeight,
		//							 GrossWeight = a.GrossWeight,
		//							 UnitPrice = a.UnitPrice,
		//						 };

		//			orderD = orderD.OrderBy("DetailNo asc");
		//			if (orderD.Any())
		//			{
		//				row.OrderD = orderD.ToList();
		//			}

		//			result.Add(row);
		//		}
		//	}

		//	return result;
		//}
		#endregion
		public DateTime? GetOrderDateMax()
		{
			var orderH = _orderRepository.GetAllQueryable().OrderByDescending(o => o.OrderD).FirstOrDefault();
			if (orderH != null)
			{
				return orderH.OrderD;
			}
			return null;
		}

		public DateTime? GetRevenueDateMax(string customerMainC, string customerSubC)
		{
			var orderD = from h in _orderRepository.GetAllQueryable()
						 join d in _containerRepository.GetAllQueryable() on new { h.OrderD, h.OrderNo }
							 equals new { d.OrderD, d.OrderNo } into t2
						 from d in t2.DefaultIfEmpty()
						 where (h.CustomerMainC == customerMainC & h.CustomerSubC == customerSubC)
						 select new { Date = d.RevenueD };
			var revenueD = orderD.OrderByDescending(i => i.Date).FirstOrDefault();
			return revenueD != null ? revenueD.Date : null;
		}

		public DateTime? GetPartnerInvoiceDateMax(string partnerMainC, string partnerSubC)
		{
			var orderD = from d in _dispatchRepository.GetAllQueryable()
						 where (d.PartnerMainC == partnerMainC & d.PartnerSubC == partnerSubC)
						 select new { Date = d.InvoiceD };
			var invoiceD = orderD.OrderByDescending(i => i.Date).FirstOrDefault();
			return invoiceD != null ? invoiceD.Date : null;
		}

		private int IsDeleteOrder(IEnumerable<Order_D> containers, IEnumerable<Dispatch_D> dispatches)
		{
			//toBeDeleted will have 3 levels:
			// "0": can't deleted
			// "1": can deleted but warning
			// "2": can deleted and not warning
			var toBeDeleted = Convert.ToInt32(DeleteLevel.NotDeleted);
			dispatches = dispatches.ToList();
			containers = containers.ToList();

			if (dispatches.Any() && containers.Any())
			{
				foreach (var dispatchD in dispatches)
				{
					if(dispatchD.DispatchStatus == Convert.ToInt32(DispatchStatus.Transported).ToString() ||
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
					//if DispatchI is Transported and Confirmed, user can't deleted the order
				}
				return toBeDeleted;
			}

			//if containers and dispatches doesn't have any value, can deleted and not warning
			return Convert.ToInt32(DeleteLevel.Deleted); ;
		}

		public void SaveOrder()
		{
			_unitOfWork.Commit();
		}
	}
}
