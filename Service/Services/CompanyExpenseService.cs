using System;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.CompanyExpense;
using Website.ViewModels.TruckExpense;

namespace Service.Services
{
	public interface ICompanyExpenseService
	{
		CompanyExpenseDatatable GetCompanyExpenseForTable(CompanyExpenseSearchParams searchInfo);
		TruckExpenseViewModel GetCompanyExpenseByKey(int id);
		void CreateCompanyExpense(CompanyExpenseViewModel companyExpense);
		void UpdateCompanyExpense(CompanyExpenseViewModel companyExpense);
		void DeleteCompanyExpense(int id);
		void SaveCompanyExpense();
	}

	public class CompanyExpenseService : ICompanyExpenseService
	{
		private readonly IExpenseRepository _expenseRepository;
		private readonly ISupplierRepository _supplierRepository;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly ICompanyExpenseRepository _companyExpenseRepository;
		private readonly IUnitOfWork _unitOfWork;

		public CompanyExpenseService(
								   IExpenseRepository expenseRepository,
								   ISupplierRepository supplierRepository,
								   IEmployeeRepository employeeRepository,
									ICompanyExpenseRepository companyExpenseRepository,
								   IUnitOfWork unitOfWork)
		{
			this._supplierRepository = supplierRepository;
			this._expenseRepository = expenseRepository;
			this._employeeRepository = employeeRepository;
			this._companyExpenseRepository = companyExpenseRepository;
			this._unitOfWork = unitOfWork;
		}

		public CompanyExpenseDatatable GetCompanyExpenseForTable(CompanyExpenseSearchParams searchInfo)
		{
			var companyExpense = from a in _companyExpenseRepository.GetAllQueryable()
								join c in _expenseRepository.GetAllQueryable() on new { a.ExpenseC }
								   equals new { c.ExpenseC } into t1
								from c in t1.DefaultIfEmpty()
								join e in _supplierRepository.GetAllQueryable() on new { a.SupplierMainC, a.SupplierSubC }
								   equals new { e.SupplierMainC, e.SupplierSubC } into t2
								from e in t2.DefaultIfEmpty()
								join t in _employeeRepository.GetAllQueryable() on a.EntryClerkC equals t.EmployeeC into t3
								from t in t3.DefaultIfEmpty()
								join q in _employeeRepository.GetAllQueryable() on a.EmployeeC equals q.EmployeeC into t4
								from q in t4.DefaultIfEmpty()
							   where (((searchInfo.ParamSearch.InvoiceDEnd == null || a.InvoiceD <= searchInfo.ParamSearch.InvoiceDEnd) &&
									 (searchInfo.ParamSearch.InvoiceDStart == null || a.InvoiceD >= searchInfo.ParamSearch.InvoiceDStart)) &
									 (String.IsNullOrEmpty(searchInfo.ParamSearch.EmployeeC) || a.EmployeeC == searchInfo.ParamSearch.EmployeeC) &
									 (String.IsNullOrEmpty(searchInfo.ParamSearch.ExpenseC) || a.ExpenseC == searchInfo.ParamSearch.ExpenseC) &
									 (String.IsNullOrEmpty(searchInfo.ParamSearch.SupplierMainC) || (a.SupplierMainC == searchInfo.ParamSearch.SupplierMainC && a.SupplierSubC == searchInfo.ParamSearch.SupplierSubC)))
							   select new CompanyExpenseViewModel()
							   {
								   Id = a.Id,
								   InvoiceD = a.InvoiceD,
								   ExpenseC = a.ExpenseC,
								   ExpenseN = c.ExpenseN,
								   EmployeeC = a.EmployeeC,
								   EmployeeN = q != null ? (q.EmployeeLastN + " " + q.EmployeeFirstN) : "",
								   EntryClerkC = a.EntryClerkC,
								   EntryClerkN = t != null ? (t.EmployeeLastN + " " + t.EmployeeFirstN) : "",
								   PaymentMethodI = a.PaymentMethodI,
								   SupplierMainC = a.SupplierMainC,
								   SupplierSubC = a.SupplierSubC,
								   SupplierN = e.SupplierN,
								   Quantity = a.Quantity,
								   UnitPrice = a.UnitPrice,
								   Total = a.Total,
								   Tax = a.Tax,
								   Description = a.Description,
							   };

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var companyExpenseOrdered = companyExpense.OrderBy(searchInfo.SortBy + (searchInfo.Reverse ? " descending" : ""));
			// paging
			var companyExpensePaging = companyExpenseOrdered.Skip((searchInfo.Page - 1) * searchInfo.ItemsPerPage).Take(searchInfo.ItemsPerPage).ToList();

			var datatable = new CompanyExpenseDatatable()
			{
				Data = companyExpensePaging,
				Total = companyExpenseOrdered.Count()
			};

			return datatable;
		}

		public TruckExpenseViewModel GetCompanyExpenseByKey(int id)
		{
			var truckExpense = (from a in _companyExpenseRepository.GetAllQueryable()
							   where a.Id == id
							   select new TruckExpenseViewModel()
							   {
								   Id = a.Id,
								   InvoiceD = a.InvoiceD,
								   ExpenseC = a.ExpenseC,
								   //ExpenseN = c.ExpenseN,
								   //EmployeeC = a.EmployeeC,
								   //EmployeeN = q != null ? (q.EmployeeLastN + " " + q.EmployeeFirstN) : "",
								   EntryClerkC = a.EntryClerkC,
								   //EntryClerkN = t != null ? (t.EmployeeLastN + " " + t.EmployeeFirstN) : "",
								   PaymentMethodI = a.PaymentMethodI,
								   SupplierMainC = a.SupplierMainC,
								   SupplierSubC = a.SupplierSubC,
								   //SupplierN = e.SupplierN,
								   Quantity = a.Quantity,
								   UnitPrice = a.UnitPrice,
								   Total = a.Total,
								   Tax = a.Tax,
								   Description = a.Description,
								   IsAllocated = a.IsAllocated
							   }).FirstOrDefault();

			if (truckExpense != null)
			{
				// get entryClerk
				var entryClerk = _employeeRepository.Query(e => e.EmployeeC == truckExpense.EntryClerkC).FirstOrDefault();
				if (entryClerk != null)
				{
					truckExpense.EntryClerkN = entryClerk.EmployeeLastN + " " + entryClerk.EmployeeFirstN;
				}

				// get employee
				//var employee = _employeeRepository.Query(e => e.EmployeeC == truckExpense.EmployeeC).FirstOrDefault();
				//if (employee != null)
				//{
				//	truckExpense.EmployeeN = employee.EmployeeLastN + " " + employee.EmployeeFirstN;
				//}

				// get expenseN
				var expense = _expenseRepository.Query(ex => ex.ExpenseC == truckExpense.ExpenseC).FirstOrDefault();
				if (expense != null)
				{
					truckExpense.ExpenseN = expense.ExpenseN;
					truckExpense.TaxRate = expense.TaxRate;
					truckExpense.TaxRoundingI = expense.TaxRoundingI;
				}

				// get supplierN
				if (truckExpense.SupplierMainC != null && truckExpense.SupplierSubC != null)
				{
					var supplier = _supplierRepository.Query(su => su.SupplierMainC == truckExpense.SupplierMainC &&
																   su.SupplierSubC == truckExpense.SupplierSubC
															).FirstOrDefault();
					if (supplier != null)
					{
						truckExpense.SupplierN = supplier.SupplierN;
					}
				}
				truckExpense.ExpenseI = "C";
				truckExpense.TruckExpenseIndex = FindIndex(id);
			}

			return truckExpense;
		}

		public void CreateCompanyExpense(CompanyExpenseViewModel companyExpenseViewModel)
		{
			companyExpenseViewModel.EmployeeC = companyExpenseViewModel.EntryClerkC;
			var companyExpense = Mapper.Map<CompanyExpenseViewModel, CompanyExpense_D>(companyExpenseViewModel);
			_companyExpenseRepository.Add(companyExpense);
			SaveCompanyExpense();
		}

		public void UpdateCompanyExpense(CompanyExpenseViewModel companyExpense)
		{
			var id = companyExpense.Id;

			var companyExpenseCheckExist = _companyExpenseRepository.Get(t => t.Id == id);

			if (companyExpenseCheckExist != null)
			{
				companyExpenseCheckExist.InvoiceD = companyExpense.InvoiceD;
				companyExpenseCheckExist.ExpenseC = companyExpense.ExpenseC;
				companyExpenseCheckExist.EntryClerkC = companyExpense.EntryClerkC;
				companyExpenseCheckExist.EmployeeC = companyExpense.EntryClerkC;
				companyExpenseCheckExist.PaymentMethodI = companyExpense.PaymentMethodI;
				companyExpenseCheckExist.SupplierMainC = companyExpense.SupplierMainC;
				companyExpenseCheckExist.SupplierSubC = companyExpense.SupplierSubC;
				companyExpenseCheckExist.Quantity = companyExpense.Quantity;
				companyExpenseCheckExist.UnitPrice = companyExpense.UnitPrice;
				companyExpenseCheckExist.Total = companyExpense.Total;
				companyExpenseCheckExist.Tax = companyExpense.Tax;
				companyExpenseCheckExist.Description = companyExpense.Description;
				companyExpenseCheckExist.IsAllocated = companyExpense.IsAllocated;

				_companyExpenseRepository.Update(companyExpenseCheckExist);
				SaveCompanyExpense();
			}
		}

		public void DeleteCompanyExpense(int id)
		{
			var companyExpenseToRemove = _companyExpenseRepository.Get(t => t.Id == id);
			if (companyExpenseToRemove != null)
			{
				_companyExpenseRepository.Delete(companyExpenseToRemove);
				SaveCompanyExpense();
			}
		}

		public void SaveCompanyExpense()
		{
			_unitOfWork.Commit();
		}

		private int FindIndex(int id)
		{
			var data = _companyExpenseRepository.GetAllQueryable();
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

						if (data.OrderBy("InvoiceD descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.Id == id))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("InvoiceD descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.Id == id)
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
	}
}
