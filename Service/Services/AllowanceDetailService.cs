using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.Allowance;
using Website.ViewModels.Expense;

namespace Service.Services
{
	public interface IAllowanceDetailService
	{
		List<AllowanceDetailViewModel> GetExpenseDetail(string expenseCate, DateTime orderD, string orderNo, int detailNo, int dispatchNo);
		void SaveAllowanceDetail();
	}
	public class AllowanceDetailService : IAllowanceDetailService
	{
		private readonly IAllowanceDetailRepository _allowanceDetailRepository;
		private readonly IExpenseService _expenseService;
		private readonly IExpenseRepository _expenseRepository;
		private readonly IUnitOfWork _unitOfWork;

		public AllowanceDetailService(IAllowanceDetailRepository allowanceDetailRepository, IExpenseService expenseService,
									IExpenseRepository expenseRepository, IUnitOfWork unitOfWork)
		{
			this._allowanceDetailRepository = allowanceDetailRepository;
			this._expenseService = expenseService;
			this._expenseRepository = expenseRepository;
			this._unitOfWork = unitOfWork;
		}

		public List<AllowanceDetailViewModel> GetExpenseDetail(string expenseCate, DateTime orderD, string orderNo, int detailNo, int dispatchNo)
		{
			List<AllowanceDetailViewModel> expenseDetailList;
			List<AllowanceDetailViewModel> expenseDList = null;
			List<AllowanceDetailViewModel> basicSettingList = null;
			//1. Load data from epenses details
			var expenseDetails = (from d in _allowanceDetailRepository.GetAllQueryable()
								  where d.OrderD == orderD &&
										  d.OrderNo == orderNo &&
										  d.DetailNo == detailNo &&
										  d.DispatchNo == dispatchNo
								  select d).ToList();

			if (expenseDetails.Any())
			{
				expenseDetailList = Mapper.Map<List<Allowance_D>, List<AllowanceDetailViewModel>>(expenseDetails);
				var result = (from l in expenseDetailList
							  join e in _expenseRepository.GetAllQueryable() on l.AllowanceC equals e.ExpenseC into le
							  from e in le.DefaultIfEmpty()
							  select new AllowanceDetailViewModel()
							  {
								  OrderD = l.OrderD,
								  OrderNo = l.OrderNo,
								  DetailNo = l.DetailNo,
								  DispatchNo = l.DispatchNo,
								  AllowanceNo = l.AllowanceNo,
								  AllowanceC = e.ExpenseC,
								  AllowanceN = e.ExpenseN,
								  Quantity = l.Quantity,
								  Unit = l.Unit,
								  UnitPrice = l.UnitPrice,
								  Amount = l.Amount,
								  Description = l.Description
							  }).ToList();
				expenseDList = result;
				//return result;
			}

			//2. Load data from basic setting 
			var isFirstTime = false;
			var bsExpenses = _expenseService.GetExpensesFromBasicSetting(expenseCate, "-1", null, isFirstTime);
			if (bsExpenses != null)
			{
				basicSettingList = Mapper.Map<List<ExpenseViewModel>, List<AllowanceDetailViewModel>>(bsExpenses);
				//return Mapper.Map<List<ExpenseViewModel>, List<AllowanceDetailViewModel>>(bsExpenses);
			}

			return MergeExpenseList(expenseDList, basicSettingList);
			//return null;

			////if basic setting don't have any config, display empty
			////3. Load data from master 
			//var initExpenseDetail = _expenseService.GetExpenseByCategory(expenseCate, "-1");
			//expenseDetailList = Mapper.Map<List<ExpenseViewModel>, List<AllowanceDetailViewModel>>(initExpenseDetail);
			//return expenseDetailList;
		}

		private List<AllowanceDetailViewModel> MergeExpenseList(List<AllowanceDetailViewModel> expenseDetailList,
																List<AllowanceDetailViewModel> basicSettingExpenseList)
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

		public void SaveAllowanceDetail()
		{
			_unitOfWork.Commit();
		}
	}
}
