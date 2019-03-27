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
using Website.ViewModels.Surcharge;

namespace Service.Services
{
	public interface ISurchargeDetailService
	{
		List<SurchargeDetailViewModel> GetExpenseDetail(string expenseCate, string dispatchI, DateTime orderD, string orderNo, int detailNo, int dispatchNo);
		void SaveSurchargeDetail();
	}
	public class SurchargeDetailService : ISurchargeDetailService
	{
		private readonly ISurchargeDetailRepository _surchargeDetailRepository;
		private readonly IExpenseService _expenseService;
		private readonly IExpenseRepository _expenseRepository;
		private readonly IUnitOfWork _unitOfWork;

		public SurchargeDetailService(ISurchargeDetailRepository surchargeDetailRepository, IExpenseService expenseService,
									IExpenseRepository expenseRepository, IUnitOfWork unitOfWork)
		{
			this._surchargeDetailRepository = surchargeDetailRepository;
			this._expenseService = expenseService;
			this._expenseRepository = expenseRepository;
			this._unitOfWork = unitOfWork;
		}

		public List<SurchargeDetailViewModel> GetExpenseDetail(string expenseCate, string dispatchI, DateTime orderD, string orderNo, int detailNo, int dispatchNo)
		{
			List<SurchargeDetailViewModel> expenseDetailList;
			List<SurchargeDetailViewModel> expenseDList = null;
			List<SurchargeDetailViewModel> basicSettingList = null;
			//1. Load data from epenses details
			var expenseDetails = (from d in _surchargeDetailRepository.GetAllQueryable()
								  join m in _expenseRepository.GetAllQueryable() on d.SurchargeC equals m.ExpenseC
								  where m.CategoryI == expenseCate &&
										  d.OrderD == orderD &&
										  d.OrderNo == orderNo &&
										  d.DetailNo == detailNo &&
										  d.DispatchNo == dispatchNo
								  select d).ToList();

			if (expenseDetails.Any())
			{
				expenseDetailList = Mapper.Map<List<Surcharge_D>, List<SurchargeDetailViewModel>>(expenseDetails);
				var result = (from l in expenseDetailList
							  join e in _expenseRepository.GetAllQueryable() on l.SurchargeC equals e.ExpenseC into le
							  from e in le.DefaultIfEmpty()
							  select new SurchargeDetailViewModel()
							  {
								  OrderD = l.OrderD,
								  OrderNo = l.OrderNo,
								  DetailNo = l.DetailNo,
								  DispatchNo = l.DispatchNo,
								  SurchargeNo = l.SurchargeNo,
								  SurchargeN = e.ExpenseN,
								  SurchargeC = e.ExpenseC,
								  Unit = l.Unit,
								  UnitPrice = l.UnitPrice,
								  Quantity = l.Quantity,
								  Amount = l.Amount,
								  Description = l.Description
							  }).ToList();
				expenseDList = result;
				//return result;
			}

			//2. Load data from basic setting 
			var isFirstTime = false;
			var bsExpenses = _expenseService.GetExpensesFromBasicSetting(expenseCate, dispatchI, null, isFirstTime);
			if (bsExpenses != null)
			{
				basicSettingList = Mapper.Map<List<ExpenseViewModel>, List<SurchargeDetailViewModel>>(bsExpenses);
				//return Mapper.Map<List<ExpenseViewModel>, List<SurchargeDetailViewModel>>(bsExpenses);
			}

			return MergeExpenseList(expenseDList, basicSettingList);
			//return null;

			////if basic setting don't have any config, display empty
			////3. Load data from master 
			//var initExpenseDetail = _expenseService.GetExpenseByCategory(expenseCate, "-1");
			//expenseDetailList = Mapper.Map<List<ExpenseViewModel>, List<SurchargeDetailViewModel>>(initExpenseDetail);
			//return expenseDetailList;
		}

		private List<SurchargeDetailViewModel> MergeExpenseList(List<SurchargeDetailViewModel> expenseDetailList,
																List<SurchargeDetailViewModel> basicSettingExpenseList)
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

		public void SaveSurchargeDetail()
		{
			_unitOfWork.Commit();
		}
	}
}
