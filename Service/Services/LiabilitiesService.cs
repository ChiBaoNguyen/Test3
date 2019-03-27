using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.Liabilities;
using System.Data.Entity.Core.Objects;

namespace Service.Services
{
	public interface ILiabilitiesService
	{
		void CreateLiabilities(LiabilitiesViewModel liabilities);

		int GetNewLiabilitiesNo(string typeI, DateTime date, string driverC);

		LiabilitiesViewModel GetLiabilities(string typeI, DateTime date, string driverC, int liabilitiesNo);

		LiabilitiesDatatable GetLiabilitiesForTable(int page, int itemsPerPage, string sortBy, bool reverse, string searchValue);

		void UpdateLiabilities(LiabilitiesViewModel liabilities);

		void DeleteLiabilities(string liabilitiesI, DateTime liabilitiesD, string driverC, int liabilitiesNo);
	}
	public class LiabilitiesService : ILiabilitiesService
	{
		private readonly ILiabilitiesRepository _liabilitiesRepository;
		private readonly IDriverRepository _driverRepository;
		private readonly IContainerRepository _containerRepository;
		private readonly IDispatchRepository _dispatchRepository;
		private readonly IExpenseDetailRepository _expenseDetailRepository;
		private readonly IExpenseRepository _expenseRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IOrderRepository _orderRepository;
		private readonly ILiabilitiesItemRepository _liabilitiesItemRepository;
		private readonly IUnitOfWork _unitOfWork;

		public LiabilitiesService(ILiabilitiesRepository liabilitiesRepository, IDriverRepository driverRepositor,
									IContainerRepository containerRepository, IDispatchRepository dispatchRepository,
									IExpenseDetailRepository expenseDetailRepository, IExpenseRepository expenseRepository,
									ICustomerRepository customerRepository, IOrderRepository orderRepository,
									ILiabilitiesItemRepository liabilitiesItemRepository, IUnitOfWork unitOfWork)
		{
			this._liabilitiesRepository = liabilitiesRepository;
			this._driverRepository = driverRepositor;
			this._containerRepository = containerRepository;
			this._dispatchRepository = dispatchRepository;
			this._expenseDetailRepository = expenseDetailRepository;
			this._expenseRepository = expenseRepository;
			this._customerRepository = customerRepository;
			this._orderRepository = orderRepository;
			this._liabilitiesItemRepository = liabilitiesItemRepository;
			this._unitOfWork = unitOfWork;
		}

		public void SaveLiabilities()
		{
			_unitOfWork.Commit();
		}

		public void CreateLiabilities(LiabilitiesViewModel liabilities)
		{
			var liabilitiesCreate = Mapper.Map<LiabilitiesViewModel, Liabilities_D>(liabilities);
			_liabilitiesRepository.Add(liabilitiesCreate);

			if (liabilities.LiabilitiesI == "1")
			{
				var expenseList = liabilities.ExpenseList;
				if (expenseList.Any())
				{
					for (int i = 0; i < expenseList.Count; i++)
					{
						if (expenseList[i].LiabilitiesStatusI == "1" || expenseList[i].LiabilitiesStatusI == "2")
						{
							var item = Mapper.Map<LiabilitiesExpenseViewModel, LiabilitiesItem_D>(expenseList[i]);
							_liabilitiesItemRepository.Add(item);
						}
					}
				}
			}

			SaveLiabilities();
		}

		public int GetNewLiabilitiesNo(string typeI, DateTime date, string driverC)
		{
			var newLiabilitiesNo = 0;

			var liabilities = _liabilitiesRepository.Query(or => or.LiabilitiesI == typeI & or.LiabilitiesD == date & or.DriverC == driverC).ToList();

			if (liabilities.Count() != 0)
			{
				var oldMaxLiabilitiesNo = liabilities.Max(u => u.LiabilitiesNo);
				newLiabilitiesNo = oldMaxLiabilitiesNo + 1;
			}
			else
			{
				newLiabilitiesNo = 1;
			}

			return newLiabilitiesNo;
		}

		public LiabilitiesViewModel GetLiabilities(string typeI, DateTime date, string driverC, int liabilitiesNo)
		{
			//GetBalance
			var previousBalance = (from l in _liabilitiesRepository.GetAllQueryable()
								   where l.DriverC == driverC
								   group l by l.DriverC into s
								   select new
								   {
									   Amount = s.Sum(i => i.LiabilitiesI == "0" ? i.Amount : i.Amount * (-1)),
								   }).FirstOrDefault();

			var balance = previousBalance != null ? previousBalance.Amount : 0;

			//GetExpenseList
			var expenseList = GetExpenseList(date, driverC, liabilitiesNo);
			var expense = expenseList.Where(p => p.LiabilitiesStatusI == "1").Sum(i => i.Amount);

			var driver = (from d in _driverRepository.GetAllQueryable()
						  where d.DriverC == driverC
						  select new LiabilitiesViewModel
						  {
							  DriverC = driverC,
							  DriverN = d.LastN + " " + d.FirstN,
							  RetiredD = d.RetiredD,
							  AdvancePaymentLimit = d.AdvancePaymentLimit,
							  LiabilitiesD = date,
							  LiabilitiesI = typeI,
							  LiabilitiesNo = liabilitiesNo,
							  TotalExpense = expense
						  }).FirstOrDefault();

			if (driver == null) return null;

			var liabilities = (from l in _liabilitiesRepository.GetAllQueryable()
							   where l.DriverC == driverC &&
									 (l.LiabilitiesI == null || l.LiabilitiesI == typeI) &&
									 (l.LiabilitiesD == null || l.LiabilitiesD == date) &&
									 (l.LiabilitiesNo == liabilitiesNo)
							   select new LiabilitiesViewModel
							   {
								   ReceiptNo = l.ReceiptNo,
								   Amount = l.Amount,
								   Description = l.Description,
							   }).FirstOrDefault();

			if (typeI != "0")
			{
				driver.ExpenseList = expenseList;
			}

			if (liabilities != null)
			{
				driver.PreviousBalance = typeI == "0" ? balance - liabilities.Amount : balance + liabilities.Amount;
				driver.Amount = liabilities.Amount;
				driver.NextBalance = balance;
				driver.Description = liabilities.Description;
				driver.ReceiptNo = liabilities.ReceiptNo;
				driver.Status = (int)FormStatus.Edit;
				//FindIndex
				driver.LiabilitiesIndex = FindIndex(typeI);

			}
			else
			{
				driver.PreviousBalance = balance;
				driver.Amount = typeI == "0" ? 0 : expense;
				driver.NextBalance = typeI == "0" ? balance : balance - expense;
				driver.Description = "";
				driver.ReceiptNo = "";
				driver.Status = (int)FormStatus.Add;
				driver.LiabilitiesNo = GetNewLiabilitiesNo(typeI, date, driverC);
			}

			return driver;
		}

		private List<LiabilitiesExpenseViewModel> GetExpenseList(DateTime date, string driverC, int liabilitiesNo)
		{
			//var result = (from o in _containerRepository .GetAllQueryable()
			//				join d in _dispatchRepository.GetAllQueryable()
			//				on new { o.OrderD, o.OrderNo, o.DetailNo } equals new { d.OrderD, d.OrderNo, d.DetailNo }
			//				join e in _expenseDetailRepository.GetAllQueryable()
			//				on new { d.OrderD, d.OrderNo, d.DetailNo, d.DispatchNo } equals new { e.OrderD, e.OrderNo, e.DetailNo, e.DispatchNo }
			//				join m in _expenseRepository.GetAllQueryable()
			//				on e.ExpenseC equals m.ExpenseC
			//				where (d.DriverC == driverC && 
			//				d.TransportD < EntityFunctions.AddDays(date,1) &&
			//				e.PaymentMethodI == Constants.CASH)
			//				select new LiabilitiesExpenseViewModel()
			//				{
			//					ContainerNo = o.ContainerNo,
			//					TransportD =d.TransportD,
			//					ContainerSizeN = o.ContainerSizeI == Constants.CONTAINERSIZE1 ? Constants.CONTAINERSIZE1N : 
			//									 o.ContainerSizeI == Constants.CONTAINERSIZE2 ? Constants.CONTAINERSIZE2N : 
			//									 Constants.CONTAINERSIZE3N,
			//					ExpenseC = m.ExpenseC,
			//					ExpenseN =m.ExpenseN,
			//					Amount = e.Amount
			//				}
			//			);

			var result = (from e in _expenseDetailRepository.GetAllQueryable()
						  join d in _dispatchRepository.GetAllQueryable()
						  on new { e.OrderD, e.OrderNo, e.DetailNo, e.DispatchNo } equals new { d.OrderD, d.OrderNo, d.DetailNo, d.DispatchNo } into ed
						  from d in ed.DefaultIfEmpty()
						  join o in _containerRepository.GetAllQueryable()
						  on new { d.OrderD, d.OrderNo, d.DetailNo } equals new { o.OrderD, o.OrderNo, o.DetailNo } into edo
						  from o in edo.DefaultIfEmpty()
						  join or in _orderRepository.GetAllQueryable()
						  on new { e.OrderD, e.OrderNo } equals new { or.OrderD, or.OrderNo } into edor
						  from or in edor.DefaultIfEmpty()
						  join c in _customerRepository.GetAllQueryable()
						  on new { or.CustomerMainC, or.CustomerSubC } equals new { c.CustomerMainC, c.CustomerSubC } into cedor
						  from c in cedor.DefaultIfEmpty()
						  join m in _expenseRepository.GetAllQueryable()
						  on e.ExpenseC equals m.ExpenseC into mcedor
						  from m in mcedor.DefaultIfEmpty()
						  join f in _liabilitiesItemRepository.GetAllQueryable()
						  on new { e.OrderD, e.OrderNo, e.DetailNo, e.DispatchNo, e.ExpenseNo } equals new { f.OrderD, f.OrderNo, f.DetailNo, f.DispatchNo, f.ExpenseNo } into fmcedor
						  from f in fmcedor.DefaultIfEmpty()
						  where (d.DriverC == driverC &&
						  d.TransportD < EntityFunctions.AddDays(date, 1) &&
						  e.PaymentMethodI == Constants.CASH) &&
						  (f == null || (f.LiabilitiesD == date && f.LiabilitiesNo == liabilitiesNo))
						  select new LiabilitiesExpenseViewModel()
						  {
							  OrderD = e.OrderD,
							  OrderNo = e.OrderNo,
							  DetailNo = e.DetailNo,
							  DispatchNo = e.DispatchNo,
							  ExpenseNo = e.ExpenseNo,
							  ContainerNo = o.ContainerNo,
							  TransportD = d.TransportD,
							  ContainerSizeI = o.ContainerSizeI,
							  //ContainerSizeN = o.ContainerSizeI == Constants.CONTAINERSIZE1 ? Constants.CONTAINERSIZE1N :
							  //				 o.ContainerSizeI == Constants.CONTAINERSIZE2 ? Constants.CONTAINERSIZE2N :
							  //				 Constants.CONTAINERSIZE3N,
							  ExpenseC = m.ExpenseC,
							  ExpenseN = m.ExpenseN,
							  //Amount = e.Amount,
							  Amount = (e.Amount ?? 0) + (e.TaxAmount ?? 0),
							  TaxAmount = e.TaxAmount,
							  CustomerN = c != null ? c.CustomerN : "",
							  LiabilitiesStatusI = f == null ? "0" : f.LiabilitiesStatusI,
							  LiabilitiesD = f == null ? (DateTime?)null : f.LiabilitiesD,
							  LiabilitiesNo = f == null ? 0 : f.LiabilitiesNo
						  }
						);

			//var previousDate = _liabilitiesRepository.Query(i => i.DriverC == driverC && i.LiabilitiesI == "1" && i.LiabilitiesD < date).OrderBy("LiabilitiesD desc").FirstOrDefault();

			//if (previousDate != null)
			//{
			//	return result.AsQueryable().Where(i => i.TransportD >= previousDate.LiabilitiesD).ToList();
			//}
			//else
			//{
				return result.ToList();
			//}
		}
		private int FindIndex(string typeI)
		{
			var liabilities = _liabilitiesRepository.GetAllQueryable();
			var index = 0;
			var totalRecords = liabilities.Count();
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

						if (liabilities.OrderBy("LiabilitiesI descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.LiabilitiesI == typeI))// && c.OrderD == orderD))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var order in liabilities.OrderBy("LiabilitiesI descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (order.LiabilitiesI == typeI)// && order.OrderD == orderD)
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

		public LiabilitiesDatatable GetLiabilitiesForTable(int page, int itemsPerPage, string sortBy, bool reverse, string searchValue)
		{
			var liabilities = (from l in _liabilitiesRepository.GetAllQueryable()
							   join d in _driverRepository.GetAllQueryable() on l.DriverC equals d.DriverC
							   select new LiabilitiesViewModel
							   {
								   DriverC = l.DriverC,
								   DriverN = d.LastN + " " + d.FirstN,
								   RetiredD = d.RetiredD,
								   LiabilitiesD = l.LiabilitiesD,
								   LiabilitiesI = l.LiabilitiesI,
								   LiabilitiesNo = l.LiabilitiesNo,
								   PreviousBalance = 0,
								   Amount = l.Amount,
								   NextBalance = 0,
								   Description = l.Description
							   }).AsQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(searchValue))
			{
				searchValue = searchValue.ToLower();
				liabilities = liabilities.Where(i => i.DriverN.ToLower().Contains(searchValue) ||
													i.Description.ToLower().Contains(searchValue));
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var liabilitiesOrderBy = liabilities.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var liabilitiesPaged = liabilitiesOrderBy.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			//var destination = Mapper.Map<List<OrderPattern_M>, List<OrderPatternViewModel>>(patternsPaged);
			var datatable = new LiabilitiesDatatable()
			{
				Data = liabilitiesPaged,
				Total = liabilitiesOrderBy.Count()
			};

			return datatable;
		}
		public void UpdateLiabilities(LiabilitiesViewModel liabilities)
		{
			var updateLiabilities = Mapper.Map<LiabilitiesViewModel, Liabilities_D>(liabilities);
			_liabilitiesRepository.Update(updateLiabilities);

			if (liabilities.LiabilitiesI == "1")
			{
				//var updatedItems = new List<LiabilitiesExpenseViewModel>();
				var expenseList = liabilities.ExpenseList;
				if (expenseList.Any())
				{
					for (int i = 0; i < expenseList.Count; i++)
					{
						if (expenseList[i].LiabilitiesStatusI == "1" || expenseList[i].LiabilitiesStatusI == "2")
						{
							//updatedItems.Add(expenseList[i]);
							var item = Mapper.Map<LiabilitiesExpenseViewModel, LiabilitiesItem_D>(expenseList[i]);
							_liabilitiesItemRepository.AddOrUpdate(item);
						}
						else
						{
							var item = Mapper.Map<LiabilitiesExpenseViewModel, LiabilitiesItem_D>(expenseList[i]);
							var existedItem = _liabilitiesItemRepository.Get(p => p.OrderD == item.OrderD & p.OrderNo == item.OrderNo & p.DetailNo == item.DetailNo &
																									p.DispatchNo == item.DispatchNo & p.ExpenseNo == item.ExpenseNo & p.LiabilitiesD == item.LiabilitiesD & p.LiabilitiesNo == item.LiabilitiesNo);
							if (existedItem != null)
								_liabilitiesItemRepository.Delete(existedItem);
						}
					}
				}

				//var liabilitiesItem = _liabilitiesItemRepository.Query(p => p.LiabilitiesD == liabilities.LiabilitiesD & 
				//																				p.LiabilitiesNo == liabilities.LiabilitiesNo);
				//if (liabilitiesItem.Any())
				//{
				//	foreach (var i in liabilitiesItem)
				//	{
				//		var item = i;
				//		var existedItem = updatedItems.FirstOrDefault(p => p.OrderD == item.OrderD & p.OrderNo == item.OrderNo & p.DetailNo == item.DetailNo &
				//															p.DispatchNo == item.DispatchNo & p.ExpenseNo == item.ExpenseNo);

				//		if (existedItem == null)
				//		{
				//			_liabilitiesItemRepository.Delete(i);
				//		}
				//	}
				//}
			}

			SaveLiabilities();
		}
		public void DeleteLiabilities(string liabilitiesI, DateTime liabilitiesD, string driverC, int liabilitiesNo)
		{
			var deleteLiabilities = _liabilitiesRepository.Get(i => i.LiabilitiesI == liabilitiesI && i.LiabilitiesD == liabilitiesD && i.DriverC == driverC && i.LiabilitiesNo == liabilitiesNo);
			if (deleteLiabilities != null)
			{
				_liabilitiesRepository.Delete(deleteLiabilities);

				var deleteLiabilitiesItemList = _liabilitiesItemRepository.Query(e => e.LiabilitiesD == liabilitiesD && e.LiabilitiesNo == liabilitiesNo).ToList();
				if (deleteLiabilitiesItemList != null)
				{
					foreach (var item in deleteLiabilitiesItemList)
					{
						var dispatch = _dispatchRepository.Get(p => p.OrderD == item.OrderD & p.OrderNo == item.OrderNo & p.DetailNo == item.DetailNo &
																									p.DispatchNo == item.DispatchNo & p.DriverC == driverC);
						if (dispatch != null)
						{
							_liabilitiesItemRepository.Delete(item);
						}
					}
				}
				SaveLiabilities();
			}
		}
	}
}
