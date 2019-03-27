using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.Customer;
using Website.ViewModels.Expense;
using Website.Enum;

namespace Service.Services
{
	public interface IExpenseCategoryService
	{
		IEnumerable<ExpenseCategoryViewModel> GetExpenseCategories();
		IEnumerable<ExpenseCategoryViewModel> GetExpenseCategorys(string value);
		IEnumerable<ExpenseCategoryViewModel> GetExpenseCategorysByCode(string value);
		ExpenseCategoryStatusViewModel GetExpenseCategoryByCode(string mainCode);

		ExpenseCategoryDatatables GetExpenseCategorysForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string searchValue);
		void CreateExpenseCategory(ExpenseCategoryViewModel expenseCategory);
		void UpdateExpenseCategory(ExpenseCategoryViewModel expenseCategory);
		void DeleteExpenseCategory(string id);
		void SetStatusExpenseCategory(string id);
		void SaveExpenseCategory();
	}

	public class ExpenseCategoryService : IExpenseCategoryService
	{
		private readonly IExpenseCategoryRepository _expenseCategoryRepository;
		private readonly IUnitOfWork _unitOfWork;

		public ExpenseCategoryService(IExpenseCategoryRepository expenseCategoryRepository, IUnitOfWork unitOfWork)
		{
			this._expenseCategoryRepository = expenseCategoryRepository;
			this._unitOfWork = unitOfWork;
		}

		#region IExpenseCategoryService members
		public IEnumerable<ExpenseCategoryViewModel> GetExpenseCategories()
		{
            var source = _expenseCategoryRepository.Query(i => i.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<ExpenseCategory_M>, IEnumerable<ExpenseCategoryViewModel>>(source);
			return destination;
		}

		public IEnumerable<ExpenseCategoryViewModel> GetExpenseCategorys(string value)
		{
			var expenseCategory = _expenseCategoryRepository.Query(cus => (cus.CategoryC.Contains(value) ||
																			cus.CategoryN.Contains(value)) && cus.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<ExpenseCategory_M>, IEnumerable<ExpenseCategoryViewModel>>(expenseCategory);
			return destination;
		}

		public IEnumerable<ExpenseCategoryViewModel> GetExpenseCategorysByCode(string value)
		{
			var expenseCategory = _expenseCategoryRepository.Query(cus => cus.CategoryC.StartsWith(value));
			var destination = Mapper.Map<IEnumerable<ExpenseCategory_M>, IEnumerable<ExpenseCategoryViewModel>>(expenseCategory);
			return destination;
		}

		public ExpenseCategoryStatusViewModel GetExpenseCategoryByCode(string code)
		{
			var expenseCategoryStatus = new ExpenseCategoryStatusViewModel();
			var expenseCategory = _expenseCategoryRepository.Query(cus => cus.CategoryC == code).FirstOrDefault();
			if (expenseCategory != null)
			{
				var expenseCategoryViewModel = Mapper.Map<ExpenseCategory_M, ExpenseCategoryViewModel>(expenseCategory);
				expenseCategoryViewModel.CategoryIndex = FindIndex(code);
				expenseCategoryStatus.ExpenseCategory = expenseCategoryViewModel;
				expenseCategoryStatus.Status = CustomerStatus.Edit.ToString();
			}
			else
			{
				expenseCategoryStatus.Status = CustomerStatus.Add.ToString();
			}
			return expenseCategoryStatus;
		}

		public ExpenseCategoryDatatables GetExpenseCategorysForTable(int page, int itemsPerPage, string sortBy, bool reverse, string searchValue)
		{
			var expenseCategorys = _expenseCategoryRepository.GetAllQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(searchValue))
			{
				searchValue = searchValue.ToLower();
				expenseCategorys = expenseCategorys.Where(cus => cus.CategoryN.ToLower().Contains(searchValue)
																|| cus.CategoryC.ToLower().Contains(searchValue)
																);
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			//expenseCategorys = expenseCategorys.AsQueryable().OrderBy("\"" + sortBy + (reverse ? " desc\"" : " asc\"")).AsEnumerable();
			var expenseCategorysOrdered = expenseCategorys.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var expenseCategorysPaged = expenseCategorysOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var destination = Mapper.Map<List<ExpenseCategory_M>, List<ExpenseCategoryViewModel>>(expenseCategorysPaged);
			var custDatatable = new ExpenseCategoryDatatables()
			{
				Data = destination,
				Total = expenseCategorys.Count()
			};
			return custDatatable;
		}

		public void CreateExpenseCategory(ExpenseCategoryViewModel expenseCategoryViewModel)
		{
			var expenseCategory = Mapper.Map<ExpenseCategoryViewModel, ExpenseCategory_M>(expenseCategoryViewModel);
			_expenseCategoryRepository.Add(expenseCategory);
			SaveExpenseCategory();
		}

		public void UpdateExpenseCategory(ExpenseCategoryViewModel expenseCategory)
		{
			var expenseCategoryToRemove = _expenseCategoryRepository.GetById(expenseCategory.CategoryC);
			var updateCustomer = Mapper.Map<ExpenseCategoryViewModel, ExpenseCategory_M>(expenseCategory);
			_expenseCategoryRepository.Delete(expenseCategoryToRemove);
			_expenseCategoryRepository.Add(updateCustomer);
			SaveExpenseCategory();
		}

		//using for active and deactive expenseCategory
		public void SetStatusExpenseCategory(string id)
		{
			var expenseCategoryToRemove = _expenseCategoryRepository.Get(c => c.CategoryC == id);
			if (expenseCategoryToRemove.IsActive == Constants.ACTIVE)
			{
				expenseCategoryToRemove.IsActive = Constants.DEACTIVE;
			}
			else
			{
				expenseCategoryToRemove.IsActive = Constants.ACTIVE;
			}
			_expenseCategoryRepository.Update(expenseCategoryToRemove);
			SaveExpenseCategory();
		}
		public void DeleteExpenseCategory(string id)
		{
			//var expenseCategoryToRemove = _expenseCategoryRepository.GetById(id);
			var expenseCategoryToRemove = _expenseCategoryRepository.Get(c => c.CategoryC == id);
			if (expenseCategoryToRemove != null)
			{
				_expenseCategoryRepository.Delete(expenseCategoryToRemove);
				SaveExpenseCategory();
			}

		}

		private int FindIndex(string code)
		{
			var data = _expenseCategoryRepository.GetAllQueryable();
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

						if (data.OrderBy("CategoryC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.CategoryC == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("CategoryC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.CategoryC == code)
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

		public void SaveExpenseCategory()
		{
			_unitOfWork.Commit();
		}
		#endregion
	}
}
