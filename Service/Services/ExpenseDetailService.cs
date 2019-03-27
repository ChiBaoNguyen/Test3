using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.Container;
using Website.ViewModels.Expense;

namespace Service.Services
{
	public interface IExpenseDetailService
	{
		List<ExpenseDetailViewModel> GetExpenseDetail(string expenseCate, string dispatchI, DateTime orderD, string orderNo, int detailNo, int? dispatchNo);
		void SaveExpenseDetail();
	}
	public class ExpenseDetailService : IExpenseDetailService
	{
		private readonly IExpenseDetailRepository _expenseDetailRepository;
		private readonly IExpenseService _expenseService;
		private readonly IExpenseRepository _expenseRepository;
		private readonly ISupplierRepository _supplierRepository;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly IOrderRepository _orderRepository;
		private readonly IUnitOfWork _unitOfWork;

		public ExpenseDetailService(IExpenseDetailRepository expenseDetailRepository, IExpenseService expenseService,
									IExpenseRepository expenseRepository, ISupplierRepository supplierRepository,
									IEmployeeRepository empoRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
		{
			this._expenseDetailRepository = expenseDetailRepository;
			this._expenseService = expenseService;
			this._expenseRepository = expenseRepository;
			this._supplierRepository = supplierRepository;
			this._employeeRepository = empoRepository;
			this._orderRepository = orderRepository;
			this._unitOfWork = unitOfWork;
		}

		public List<ExpenseDetailViewModel> GetExpenseDetail(string expenseCate, string dispatchI, DateTime orderD, string orderNo, int detailNo, int? dispatchNo)
		{
			List<ExpenseDetailViewModel> expenseDetailList;
			List<ExpenseDetailViewModel> expenseDList = null;
			//List<ExpenseDetailViewModel> basicSettingList = null;
			if (dispatchNo != null)
			{
				//1. Load data from epenses details
				var expenseDetails = (from d in _expenseDetailRepository.GetAllQueryable()
									  join m in _expenseRepository.GetAllQueryable() on d.ExpenseC equals m.ExpenseC
									  where m.CategoryI == expenseCate &&
											  d.OrderD == orderD &&
											  d.OrderNo == orderNo &&
											  d.DetailNo == detailNo &&
											  d.DispatchNo == dispatchNo
									  select d).ToList();

				if (expenseDetails.Any())
				{
					expenseDetailList = Mapper.Map<List<Expense_D>, List<ExpenseDetailViewModel>>(expenseDetails);
					var result = (from l in expenseDetailList
								  join e in _expenseRepository.GetAllQueryable() on l.ExpenseC equals e.ExpenseC into le
								  from e in le.DefaultIfEmpty()
								  join s in _supplierRepository.GetAllQueryable() on new { l.SupplierMainC, l.SupplierSubC }
									  equals new { s.SupplierMainC, s.SupplierSubC } into les
								  from s in les.DefaultIfEmpty()
								  join em in _employeeRepository.GetAllQueryable() on l.EntryClerkC equals em.EmployeeC into lesem
								  from em in lesem.DefaultIfEmpty()
								  select new ExpenseDetailViewModel()
								  {
									  OrderD = l.OrderD,
									  OrderNo = l.OrderNo,
									  DetailNo = l.DetailNo,
									  DispatchNo = l.DispatchNo,
									  ExpenseNo = l.ExpenseNo,
									  ExpenseN = e.ExpenseN,
									  ExpenseC = e.ExpenseC,
									  PaymentMethodI = l.PaymentMethodI,
									  InvoiceD = l.InvoiceD,
									  SupplierMainC = l.SupplierMainC,
									  SupplierSubC = l.SupplierSubC,
									  SupplierN = s != null ? s.SupplierN : "",
									  Unit = l.Unit,
									  UnitPrice = l.UnitPrice,
									  Quantity = l.Quantity,
									  Amount = l.Amount,
									  TaxAmount = l.TaxAmount,
									  TaxRate = l.TaxRate,
									  IsIncluded = l.IsIncluded,
									  IsRequested = l.IsRequested,
									  IsPayable = l.IsPayable,
									  EntryClerkC = l.EntryClerkC,
									  EntryClerkFristN = em != null ? em.EmployeeFirstN : "",
									  EntryClerkLastN = em != null ? em.EmployeeLastN : "",
									  Description = l.Description,
									  TaxRoudingI = e.TaxRoundingI
								  }).ToList();
					expenseDList = result;
					//return result;
				}
			}
			if (expenseDList != null)
				return expenseDList;

			var isCollected = _orderRepository.GetAllQueryable().Where(o => o.OrderD == orderD && o.OrderNo == orderNo).FirstOrDefault().IsCollected;
			//2. Load data from basic setting 
			var isFirstTime = true;
			var bsExpenses = _expenseService.GetExpensesFromBasicSettingNew(expenseCate, dispatchI, isCollected, isFirstTime);
			if (bsExpenses != null)
			{
				//basicSettingList = Mapper.Map<List<ExpenseViewModel>, List<ExpenseDetailViewModel>>(bsExpenses);
				return Mapper.Map<List<ExpenseViewModel>, List<ExpenseDetailViewModel>>(bsExpenses);
			}

			//return MergeExpenseList(expenseDList, basicSettingList);

			return null;

			////if basic setting don't have any config, display empty
			////3. Load data from master 
			//var initExpenseDetail = _expenseService.GetExpenseByCategory(expenseCate, dispatchI);
			//expenseDetailList = Mapper.Map<List<ExpenseViewModel>, List<ExpenseDetailViewModel>>(initExpenseDetail);
			//return expenseDetailList;
		}

		private List<ExpenseDetailViewModel> MergeExpenseList(List<ExpenseDetailViewModel> expenseDetailList,
																List<ExpenseDetailViewModel> basicSettingExpenseList)
		{
			if (basicSettingExpenseList != null)
			{
				if (expenseDetailList == null)
				{
					return basicSettingExpenseList;
				}
				foreach (var t in from t in basicSettingExpenseList
								  let existExpense = expenseDetailList.FirstOrDefault(p => p.ExpenseC == t.ExpenseC)
								  where existExpense == null
								  select t)
				{
					expenseDetailList.Add(t);
				}
				return expenseDetailList;
			}
			return expenseDetailList;
		}

		public void SaveExpenseDetail()
		{
			_unitOfWork.Commit();
		}
	}
}
