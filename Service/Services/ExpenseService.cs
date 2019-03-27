using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.Customer;
using Website.ViewModels.Expense;
using Website.ViewModels.Trailer;

namespace Service.Services
{
	public interface IExpenseService
	{
		List<ExpenseViewModel> GetExpenseByCategory(string categoryI, string dispatchI);
		List<ExpenseViewModel> GetExpenseAutoSuggestion(string value, string categoryI, string dispatchI);
		ExpenseViewModel GetExpenseByName(string name, string categoryI, string dispatchI);
		ExpenseViewModel GetExpenseByCodeAndName(string code, string name, string categoryI, string dispatchI);
		ExpenseCategoryViewModel GetFuelExpenseByName(string name);
        IEnumerable<ExpenseViewModel> GetExpense();
		IEnumerable<ExpenseViewModel> GetExpenseByCategoryIForReport(string categoryI);
        IEnumerable<ExpenseViewModel> GetExpensesByCode(string value);
		ExpenseViewModel GetExpenseByCode(string mainCode);
		int CheckExistExpenseInLocationD(string mainCode);
		IEnumerable<ExpenseViewModel> GetExpenses(string value);
        ExpenseDatatables GetExpensesForTable(int page, int itemsPerPage, string sortBy, bool reverse,
             string searchValue);
        void CreateExpense(ExpenseViewModel expense);
        void UpdateExpense(ExpenseViewModel expense);
        void DeleteExpense(string expenseC);
		IEnumerable<ExpenseViewModel> GetExpenseForSuggestion(string value);
		IEnumerable<ExpenseViewModel> GetExpenseForSuggestionOfBasicSetting(string value);
		IEnumerable<ExpenseViewModel> GetSurchargeForSuggestionOfBasicSetting(string value);
		IEnumerable<ExpenseViewModel> GetAllowanceForSuggestionOfBasicSetting(string value);
		IEnumerable<ExpenseViewModel> GetPartnerCostForSuggestionOfBasicSetting(string value);
		IEnumerable<ExpenseViewModel> GetPartnerSurchargeForSuggestionOfBasicSetting(string value);
		IEnumerable<ExpenseCategoryViewModel> GetFuelExpenseForSuggestionOfBasicSetting(string value);
		ExpenseViewModel GetByName(string value);
		IEnumerable<ExpenseViewModel> GetExpenseForAutosuggestByCategory(string value, string categoryI);
		IEnumerable<ExpenseViewModel> GetExpenseForComboboxByCategory(string categoryI);
		List<ExpenseViewModel> GetExpensesFromBasicSetting(string categoryI, string dispatchI, string isCollected, bool isFirstTime);
		List<ExpenseViewModel> GetExpensesFromBasicSettingNew(string categoryI, string dispatchI, string isCollected, bool isFirstTime);
		List<ExpenseViewModel> GetDefaultExpenseByCateAndDispatchI(string categoryI, string dispatchI);
		void SaveExpense();
	}
	public class ExpenseService : IExpenseService
	{
		private readonly ILocationDetailRepository _locationDetailRepository;
		private readonly IExpenseRepository _expenseRepository;
		private readonly IExpenseCategoryRepository _expenseCategoryRepository;
		private readonly IBasicSettingService _basicSettingService;
		private readonly IUnitOfWork _unitOfWork;

		public ExpenseService(ILocationDetailRepository localLocationDetailRepository, IExpenseRepository expenseRepository, IExpenseCategoryRepository epxnExpenseCategoryRepository, IBasicSettingService basicSettingService, IUnitOfWork unitOfWork)
		{
			this._locationDetailRepository = localLocationDetailRepository;
			this._expenseRepository = expenseRepository;
			this._basicSettingService = basicSettingService;
			this._expenseCategoryRepository = epxnExpenseCategoryRepository;
			this._unitOfWork = unitOfWork;
		}

		public List<ExpenseViewModel> GetExpenseByCategory(string categoryI, string dispatchI)
		{
			List<Expense_M> categories = null;
			//category Expense == 1
			if (categoryI.Equals("1"))
			{
				//Owner == 0
				if (dispatchI.Equals("0"))
				{
					categories = _expenseRepository.Query(p => p.CategoryI == categoryI && p.IsPayable == "0").ToList();
				}
				//Partner == 1
				else if (dispatchI.Equals("1"))
				{
					categories = _expenseRepository.Query(p => p.CategoryI == categoryI && p.IsIncluded == "0").ToList();
				}
				else
				{
					categories = _expenseRepository.Query(p => p.CategoryI == categoryI).ToList();
				}
			}
			else
			{
				categories = _expenseRepository.Query(p => p.CategoryI == categoryI).ToList();
			}

			if (!categories.Any()) return null;
			var mCategories = Mapper.Map<List<Expense_M>, List<ExpenseViewModel>>(categories);
			return mCategories;
		}

		public List<ExpenseViewModel> GetExpenseAutoSuggestion(string value, string categoryI, string dispatchI)
		{
			List<Expense_M> categories = null;
			//category Expense == 1
			//if (categoryI.Equals("1"))
			//{
			//	//Owner == 0
			//	if (dispatchI.Equals("0"))
			//	{
			//		categories = _expenseRepository.Query(p => (p.ExpenseC.StartsWith(value) || p.ExpenseN.Contains(value)) && p.CategoryI == categoryI && p.IsPayable == "0").ToList();
			//	}
			//	//Partner == 1
			//	else if (dispatchI.Equals("1"))
			//	{
			//		categories = _expenseRepository.Query(p => (p.ExpenseC.StartsWith(value) || p.ExpenseN.Contains(value)) && p.CategoryI == categoryI && p.IsIncluded == "0").ToList();
			//	}
			//}
			//else
			//{
				categories = _expenseRepository.Query(p => (p.ExpenseC.StartsWith(value) || p.ExpenseN.Contains(value)) && p.CategoryI == categoryI).ToList();
			//}

			if (categories != null && categories.Any())
			{
				var mCategories = Mapper.Map<List<Expense_M>, List<ExpenseViewModel>>(categories);
				return mCategories;
			}
			return null;
		}

		public ExpenseViewModel GetExpenseByName(string name, string categoryI, string dispatchI)
		{
			 Expense_M expense = null;
			//category Expense == 1
			//if (categoryI.Equals("1"))
			//{
			//	//Owner == 0
			//	if (dispatchI.Equals("0"))
			//	{
			//		expense = _expenseRepository.Query(p => p.ExpenseN.Equals(name) && p.CategoryI == categoryI && p.IsPayable == "0").FirstOrDefault();
			//	}
			//	//Partner == 1
			//	else if (dispatchI.Equals("1"))
			//	{
			//		expense = _expenseRepository.Query(p => p.ExpenseN.Equals(name) && p.CategoryI == categoryI && p.IsIncluded == "0").FirstOrDefault();
			//	}
			//}
			//else
			//{
				expense = _expenseRepository.Query(p => p.ExpenseN.Equals(name) && p.CategoryI == categoryI).FirstOrDefault();
			//}

			if (expense != null)
			{
				var mCategories = Mapper.Map<Expense_M, ExpenseViewModel>(expense);
				return mCategories;
			}
			return null;
		}
		public ExpenseViewModel GetExpenseByCodeAndName(string code, string name, string categoryI, string dispatchI)
		{
			var expense = _expenseRepository.Query(p => p.ExpenseN.Equals(name) && p.CategoryI == categoryI).ToList();

			switch (expense.Count)
			{
				case 0:
					return null;
				case 1:
					return Mapper.Map<Expense_M, ExpenseViewModel>(expense[0]);
				default:
					var mExpense = expense[0];
					if (code != null && code != "undefined")
					{
						foreach (var item in expense)
						{
							if (item.ExpenseC.Equals(code))
								mExpense = item;
						}
					}
					return Mapper.Map<Expense_M, ExpenseViewModel>(mExpense);
			}
		}

		public ExpenseCategoryViewModel GetFuelExpenseByName(string name)
		{
		
			var cateExpense = _expenseCategoryRepository.Query(p => p.CategoryN.Equals(name)).FirstOrDefault();

			if (cateExpense != null)
			{
				var mCategories = Mapper.Map<ExpenseCategory_M, ExpenseCategoryViewModel>(cateExpense);
				return mCategories;
			}
			return null;
		}

		public IEnumerable<ExpenseViewModel> GetExpense()
        {
            var source = _expenseRepository.GetAll();
            var destination = Mapper.Map<IEnumerable<Expense_M>, IEnumerable<ExpenseViewModel>>(source);
            return destination;
        }

		public IEnumerable<ExpenseViewModel> GetExpenseByCategoryIForReport(string categoryI)
		{
			var source = _expenseRepository.Query(e => e.CategoryI == categoryI).OrderBy("ExpenseN asc");
			var destination = Mapper.Map<IEnumerable<Expense_M>, IEnumerable<ExpenseViewModel>>(source);
			return destination;
		}

		public IEnumerable<ExpenseViewModel> GetExpensesByCode(string value)
        {
            var expense = _expenseRepository.Query(cus => cus.ExpenseC.StartsWith(value));
            var destination = Mapper.Map<IEnumerable<Expense_M>, IEnumerable<ExpenseViewModel>>(expense);
            return destination;
        }
		public IEnumerable<ExpenseViewModel> GetExpenses(string value)
		{
			var expense = _expenseRepository.Query(cus => cus.ExpenseC.Contains(value) ||
																			cus.ExpenseN.Contains(value));
			var destination = Mapper.Map<IEnumerable<Expense_M>, IEnumerable<ExpenseViewModel>>(expense);
			return destination;
		}

		public ExpenseViewModel GetExpenseByCode(string code)
        {
            var expense = _expenseRepository.Query(cus => cus.ExpenseC == code).FirstOrDefault();
            if (expense != null)
            {
                var expenseViewModel = Mapper.Map<Expense_M, ExpenseViewModel>(expense);
	            expenseViewModel.ExpenseIndex = FindIndex(code);
	            return expenseViewModel;
            }
            return null;
        }

		public int CheckExistExpenseInLocationD(string code)
		{
			var locaD = _locationDetailRepository.Query(loca => loca.ExpenseC == code).FirstOrDefault();
			if (locaD != null)
			{
				return 1;
			}
			return 0;
		}

		public ExpenseDatatables GetExpensesForTable(int page, int itemsPerPage, string sortBy, bool reverse, string searchValue)
		{
			var expenses = (from e in _expenseRepository.GetAllQueryable()
							join c in _expenseCategoryRepository.GetAllQueryable()
								on e.CategoryC equals c.CategoryC into t
							from c in t.DefaultIfEmpty()
							where (searchValue == null || searchValue == "" ||
									e.ExpenseN.ToLower().Contains(searchValue.ToLower()) ||
									e.ExpenseC.ToLower().Contains(searchValue.ToLower()))
							select new ExpenseViewModel
							{
								ExpenseC = e.ExpenseC,
								ExpenseN = e.ExpenseN,
								CategoryI = e.CategoryI,
								CategoryN = c.CategoryN,
								Unit = e.Unit,
								UnitPrice = e.UnitPrice,
								TaxRate = e.TaxRate,
								IsIncluded = e.IsIncluded,
								Description = e.Description,
								ViewReport = e.ViewReport,
								ColumnName =e.ColumnName
							});

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			//expenses = expenses.AsQueryable().OrderBy("\"" + sortBy + (reverse ? " desc\"" : " asc\"")).AsEnumerable();
			var expensesOrdered = expenses.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var expensesPaged = expensesOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var custDatatable = new ExpenseDatatables()
			{
				Data = expensesPaged.ToList(),
				Total = expenses.Count()
			};
			return custDatatable;
		}

        public void CreateExpense(ExpenseViewModel expenseViewModel)
        {
            var expense = Mapper.Map<ExpenseViewModel, Expense_M>(expenseViewModel);
            _expenseRepository.Add(expense);
            SaveExpense();
        }

        public void UpdateExpense(ExpenseViewModel expense)
        {
            var expenseToRemove = _expenseRepository.GetById(expense.ExpenseC);
            var updateCustomer = Mapper.Map<ExpenseViewModel, Expense_M>(expense);
            _expenseRepository.Delete(expenseToRemove);
            _expenseRepository.Add(updateCustomer);
	        _locationDetailRepository.Query(p => p.ExpenseC == expense.ExpenseC).ToList().ForEach(l => l.ExpenseN = expense.ExpenseN);
            SaveExpense();
        }

        public void DeleteExpense(string expenseC)
        {
            var expenseToRemove = _expenseRepository.Get(c => c.ExpenseC == expenseC);
            if (expenseToRemove != null)
            {
                _expenseRepository.Delete(expenseToRemove);
                SaveExpense();
            }

        }
		public IEnumerable<ExpenseViewModel> GetExpenseForSuggestion(string value)
		{
			var searchExpense = _expenseRepository.Query(e => (e.CategoryI == Constants.CATEGORYI1) && 
															  (e.ExpenseC.Contains(value) || e.ExpenseN.Contains(value))
														);
			var destination = Mapper.Map<IEnumerable<Expense_M>, IEnumerable<ExpenseViewModel>>(searchExpense);
			return destination;
		}

		public IEnumerable<ExpenseViewModel> GetExpenseForSuggestionOfBasicSetting(string value)
		{
			var searchExpense = _expenseRepository.Query(e => e.CategoryI == Constants.CATEGORYI1 && // e.IsPayable == Constants.ISPAYABLE2
															  (e.ExpenseC.StartsWith(value) || e.ExpenseN.Contains(value))
														);
			var destination = Mapper.Map<IEnumerable<Expense_M>, IEnumerable<ExpenseViewModel>>(searchExpense);
			return destination;
		}
		public IEnumerable<ExpenseViewModel> GetSurchargeForSuggestionOfBasicSetting(string value)
		{
			var searchExpense = _expenseRepository.Query(e => e.CategoryI == Constants.CATEGORYI2 &&
															  (e.ExpenseC.StartsWith(value) || e.ExpenseN.Contains(value))
														);
			var destination = Mapper.Map<IEnumerable<Expense_M>, IEnumerable<ExpenseViewModel>>(searchExpense);
			return destination;
		}

		public IEnumerable<ExpenseViewModel> GetAllowanceForSuggestionOfBasicSetting(string value)
		{
			var searchExpense = _expenseRepository.Query(e => e.CategoryI == Constants.CATEGORYI3 &&
															  (e.ExpenseC.StartsWith(value) || e.ExpenseN.Contains(value))
														);
			var destination = Mapper.Map<IEnumerable<Expense_M>, IEnumerable<ExpenseViewModel>>(searchExpense);
			return destination;
		}

		public IEnumerable<ExpenseViewModel> GetPartnerCostForSuggestionOfBasicSetting(string value)
		{
			var searchExpense = _expenseRepository.Query(e => e.CategoryI == Constants.CATEGORYI1 && // e.IsIncluded == Constants.ISINCLUDED2
															  (e.ExpenseC.StartsWith(value) || e.ExpenseN.Contains(value))
														);
			var destination = Mapper.Map<IEnumerable<Expense_M>, IEnumerable<ExpenseViewModel>>(searchExpense);
			return destination;
		}
		public IEnumerable<ExpenseViewModel> GetPartnerSurchargeForSuggestionOfBasicSetting(string value)
		{
			var searchExpense = _expenseRepository.Query(e => e.CategoryI == Constants.CATEGORYI2 &&
															  (e.ExpenseC.StartsWith(value) || e.ExpenseN.Contains(value))
														);
			var destination = Mapper.Map<IEnumerable<Expense_M>, IEnumerable<ExpenseViewModel>>(searchExpense);
			return destination;
		}

		public IEnumerable<ExpenseCategoryViewModel> GetFuelExpenseForSuggestionOfBasicSetting(string value)
		{
			var searchCateExpense = _expenseCategoryRepository.Query(e => e.CategoryC.StartsWith(value) || e.CategoryN.Contains(value));
			var destination = Mapper.Map<IEnumerable<ExpenseCategory_M>, IEnumerable<ExpenseCategoryViewModel>>(searchCateExpense);
			return destination;
		}

		public ExpenseViewModel GetByName(string value)
		{
			var expense = _expenseRepository.Query(e => e.ExpenseN == value).FirstOrDefault();
			if (expense != null)
			{
				var destination = Mapper.Map<Expense_M, ExpenseViewModel>(expense);
				return destination;
			}
			return null;
		}

		public IEnumerable<ExpenseViewModel> GetExpenseForAutosuggestByCategory(string value, string categoryI)
		{
			var searchExpense = _expenseRepository.Query(e => e.CategoryI == categoryI &&
															  (e.ExpenseC.Contains(value) || e.ExpenseN.Contains(value))
														);
			var destination = Mapper.Map<IEnumerable<Expense_M>, IEnumerable<ExpenseViewModel>>(searchExpense);
			return destination;
		}

		public IEnumerable<ExpenseViewModel> GetExpenseForComboboxByCategory(string categoryI)
		{
			var searchExpense = _expenseRepository.Query(e => e.CategoryI == categoryI);
			var destination = Mapper.Map<IEnumerable<Expense_M>, IEnumerable<ExpenseViewModel>>(searchExpense);
			return destination;
		}

		public List<ExpenseViewModel> GetExpensesFromBasicSetting(string categoryI, string dispatchI, string isCollected, bool isFirstTime)
		{
			var expenseCodes = _basicSettingService.GetExpenseCodesFromBasicSetting(categoryI, dispatchI);
			if (expenseCodes == null) return null;
			var expenses = (from p in expenseCodes
				join m in _expenseRepository.GetAllQueryable() on p.ExpenseC equals m.ExpenseC into pm
				from m in pm.DefaultIfEmpty()
				select new ExpenseViewModel()
				{
					ExpenseN = m.ExpenseN,
					ExpenseC = p.ExpenseC,
					PaymentMethodI = m.PaymentMethodI,
					Unit = m.Unit,
					UnitPrice = m.UnitPrice,
					TaxRate = m.TaxRate,
					IsIncluded = dispatchI.Equals(Convert.ToInt16(DispatchType.PartnerTruck).ToString()) ? "0" : m.IsIncluded,
					IsRequested = String.IsNullOrEmpty(isCollected) ? m.IsRequested : isCollected,
					IsPayable = dispatchI.Equals(Convert.ToInt16(DispatchType.Truck).ToString()) ? "0" : m.IsPayable,
					Description = m.Description,
					Quantity = isFirstTime ? 1 : 0,
					Amount = isFirstTime ? m.UnitPrice : 0
				}).ToList();

			return expenses;
		}

		public List<ExpenseViewModel> GetExpensesFromBasicSettingNew(string categoryI, string dispatchI, string isCollected, bool isFirstTime)
		{
			var expenseCodes = _basicSettingService.GetExpenseCodesFromBasicSetting(categoryI, dispatchI);
			if (expenseCodes == null) return null;
			var expenses = (from p in expenseCodes
							join m in _expenseRepository.GetAllQueryable() on p.ExpenseC equals m.ExpenseC into pm
							from m in pm.DefaultIfEmpty()
							select new ExpenseViewModel()
							{
								ExpenseN = m.ExpenseN,
								ExpenseC = p.ExpenseC,
								PaymentMethodI = m.PaymentMethodI,
								Unit = m.Unit,
								UnitPrice = m.UnitPrice,
								TaxRate = m.TaxRate,
								IsIncluded = dispatchI.Equals(Convert.ToInt16(DispatchType.PartnerTruck).ToString()) ? "0" : m.IsIncluded,
								//IsRequested = String.IsNullOrEmpty(isCollected) ? m.IsRequested : isCollected,
								IsRequested = m.IsRequested,
								IsPayable = dispatchI.Equals(Convert.ToInt16(DispatchType.Truck).ToString()) ? "0" : m.IsPayable,
								Description = m.Description,
								Quantity = isFirstTime ? 1 : 0,
								Amount = isFirstTime ? m.UnitPrice : 0
							}).ToList();

			return expenses;
		}

		public List<ExpenseViewModel> GetDefaultExpenseByCateAndDispatchI(string categoryI, string dispatchI)
		{
			//1. Load form basic setting
			var isFirstTime = true;
			var bsExpenses = GetExpensesFromBasicSetting(categoryI, dispatchI, null, isFirstTime);
			if (bsExpenses != null)
			{
				return bsExpenses;
			}

			return null;

			//if basic setting dont have any config, display empty
			//2.If basic setting is null, load from master
			//var msExpense = GetExpenseByCategory(categoryI, dispatchI);
			//return msExpense;
		}

		private int FindIndex(string code)
		{
			var data = _expenseRepository.GetAllQueryable();
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

						if (data.OrderBy("ExpenseC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.ExpenseC == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("ExpenseC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.ExpenseC == code)
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

		public void SaveExpense()
		{
			_unitOfWork.Commit();
		}
	}
}
