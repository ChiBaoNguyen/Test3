using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.TruckExpense;

namespace Service.Services
{
	public interface ITruckExpenseService
	{
		TruckExpenseDatatables GetTruckExpensesForTable(TruckExpenseSearchParams searchInfo);
		TruckExpenseViewModel GetTruckExpensesByKey(int id);
		void CreateTruckExpense(TruckExpenseViewModel truckExpense);
		void UpdateTruckExpense(TruckExpenseViewModel truckExpense);
		void DeleteTruckExpense(int id);
		void SaveTruckExpense();

		DateTime? GetSupplierInvoiceDateMax(string supplierMainC, string supplierSubC);
		IEnumerable<TruckExpenseViewModel> Get(string objectI);
	}

	public class TruckExpenseService : ITruckExpenseService
	{
		private readonly ITruckExpenseRepository _truckExpenseRepository;
		private readonly ITruckRepository _truckRepository;
		private readonly ITrailerRepository _trailerRepository;
		private readonly IExpenseRepository _expenseRepository;
		private readonly IDriverRepository _driverRepository;
		private readonly ISupplierRepository _supplierRepository;
		private readonly ISupplierSettlementRepository _supplierSettlementRepository;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly ICompanyExpenseRepository _companyExpenseRepository;
		private readonly IUnitOfWork _unitOfWork;

		public IEnumerable<TruckExpenseViewModel> Get(string objectI)
		{
			var truckExpense = _truckExpenseRepository.GetAll().Where(x => x.ObjectI.Equals(objectI)).OrderBy(x => x.Code);

			if (truckExpense.Any())
			{
				var Object = Mapper.Map<IEnumerable<TruckExpense_D>, IEnumerable<TruckExpenseViewModel>>(truckExpense);
				return Object;
			}
			return null;
		}

		public TruckExpenseService(ITruckExpenseRepository truckExpenseRepository,
								   ITruckRepository truckRepository,
								   ITrailerRepository trailerRepository,
								   IExpenseRepository expenseRepository,
								   IDriverRepository driverRepository,
								   ISupplierRepository supplierRepository,
								   ISupplierSettlementRepository supplierSettlementRepository,
								   IEmployeeRepository employeeRepository,
								   ICompanyExpenseRepository companyExpenseRepository,
								   IUnitOfWork unitOfWork)
		{
			this._supplierRepository = supplierRepository;
			this._truckRepository = truckRepository;
			this._trailerRepository = trailerRepository;
			this._expenseRepository = expenseRepository;
			this._driverRepository = driverRepository;
			this._truckExpenseRepository = truckExpenseRepository;
			this._supplierSettlementRepository = supplierSettlementRepository;
			this._employeeRepository = employeeRepository;
			this._companyExpenseRepository = companyExpenseRepository;
			this._unitOfWork = unitOfWork;
		}

		#region ITruckExpenseService members
		public TruckExpenseDatatables GetTruckExpensesForTable(TruckExpenseSearchParams searchInfo)
		{
			var truckExpense = (from a in _truckExpenseRepository.GetAllQueryable()
								join b in _truckRepository.GetAllQueryable() on a.Code
								   equals b.TruckC into t1
								join f in _trailerRepository.GetAllQueryable() on a.Code
									equals f.TrailerC into t6
								from f in t6.DefaultIfEmpty()
								from b in t1.DefaultIfEmpty()
								join c in _expenseRepository.GetAllQueryable() on new { a.ExpenseC }
									equals new { c.ExpenseC } into t2
								from c in t2.DefaultIfEmpty()
								join d in _driverRepository.GetAllQueryable() on new { a.DriverC }
									equals new { d.DriverC } into t3
								from d in t3.DefaultIfEmpty()
								join e in _supplierRepository.GetAllQueryable() on new { a.SupplierMainC, a.SupplierSubC }
									equals new { e.SupplierMainC, e.SupplierSubC } into t4
								from e in t4.DefaultIfEmpty()
								join t in _employeeRepository.GetAllQueryable() on a.EntryClerkC equals t.EmployeeC into t5
								from t in t5.DefaultIfEmpty()
								where (((searchInfo.ParamSearch.InvoiceDEnd == null || a.InvoiceD <= searchInfo.ParamSearch.InvoiceDEnd) &&
									  (searchInfo.ParamSearch.InvoiceDStart == null || a.InvoiceD >= searchInfo.ParamSearch.InvoiceDStart)) &
									  ((searchInfo.ParamSearch.TransportDEnd == null || a.TransportD <= searchInfo.ParamSearch.TransportDEnd) &&
									  (searchInfo.ParamSearch.TransportDStart == null || a.TransportD >= searchInfo.ParamSearch.TransportDStart)) &
									  (String.IsNullOrEmpty(searchInfo.ParamSearch.ExpenseC) || a.ExpenseC == searchInfo.ParamSearch.ExpenseC) &
									  (String.IsNullOrEmpty(searchInfo.ParamSearch.SupplierMainC) || (a.SupplierMainC == searchInfo.ParamSearch.SupplierMainC && a.SupplierSubC == searchInfo.ParamSearch.SupplierSubC)) &
									  (//searchInfo.ParamSearch.ObjectI != "C" &&
										( //((String.IsNullOrEmpty(searchInfo.ParamSearch.Code) || a.Code == searchInfo.ParamSearch.Code)) || 
											(searchInfo.ParamSearch.ObjectI == "1" && searchInfo.ParamSearch.ObjectI == a.ObjectI && (String.IsNullOrEmpty(searchInfo.ParamSearch.Code) || searchInfo.ParamSearch.Code == f.TrailerC))
											|| (searchInfo.ParamSearch.ObjectI == "0" && searchInfo.ParamSearch.ObjectI == a.ObjectI && (String.IsNullOrEmpty(searchInfo.ParamSearch.Code) || searchInfo.ParamSearch.Code == b.TruckC)))))
								select new TruckExpenseViewModel()
								{
									Id = a.Id,
									InvoiceD = a.InvoiceD,
									TransportD = a.TransportD,
									ExpenseC = a.ExpenseC,
									ExpenseN = c.ExpenseN,
									EntryClerkC = a.EntryClerkC,
									EntryClerkN = t != null ? (t.EmployeeLastN + " " + t.EmployeeFirstN) : "",
									DriverC = a.DriverC,
									DriverN = d != null ? d.LastN + " " + d.FirstN : "",
									PaymentMethodI = a.PaymentMethodI,
									SupplierMainC = a.SupplierMainC,
									SupplierSubC = a.SupplierSubC,
									SupplierN = e.SupplierN,
									Quantity = a.Quantity,
									UnitPrice = a.UnitPrice,
									Total = a.Total + a.Tax,
									Tax = a.Tax,
									Description = a.Description,
									ObjectNo = a.ObjectI == "0" ? b.RegisteredNo : f.TrailerNo,
									ObjectI = a.ObjectI,
									ExpenseI = "T",
								}).ToList();
			var companyExpense = (from a in _companyExpenseRepository.GetAllQueryable()
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
									   //(String.IsNullOrEmpty(searchInfo.ParamSearch.EmployeeC) || a.EmployeeC == searchInfo.ParamSearch.EmployeeC) &
									   (String.IsNullOrEmpty(searchInfo.ParamSearch.ExpenseC) || a.ExpenseC == searchInfo.ParamSearch.ExpenseC) &
										(searchInfo.ParamSearch.ObjectI == "C") &
									   (String.IsNullOrEmpty(searchInfo.ParamSearch.SupplierMainC) || (a.SupplierMainC == searchInfo.ParamSearch.SupplierMainC && a.SupplierSubC == searchInfo.ParamSearch.SupplierSubC)))
								 select new TruckExpenseViewModel()
								 {
									 Id = a.Id,
									 InvoiceD = a.InvoiceD,
									 ExpenseC = a.ExpenseC,
									 ExpenseN = c.ExpenseN,
									 //DriverC = a.EmployeeC,
									 //DriverN = q != null ? (q.EmployeeLastN + " " + q.EmployeeFirstN) : "",
									 EntryClerkC = a.EntryClerkC,
									 EntryClerkN = t != null ? (t.EmployeeLastN + " " + t.EmployeeFirstN) : "",
									 PaymentMethodI = a.PaymentMethodI,
									 SupplierMainC = a.SupplierMainC,
									 SupplierSubC = a.SupplierSubC,
									 SupplierN = e.SupplierN,
									 Quantity = a.Quantity,
									 UnitPrice = a.UnitPrice,
									 Total = a.Total + a.Tax,
									 Tax = a.Tax,
									 Description = a.Description,
									 ExpenseI = "C",
								 }).ToList();
			var combined = truckExpense.Concat(companyExpense);
			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var truckExpenseOrdered = combined.OrderBy(searchInfo.SortBy + (searchInfo.Reverse ? " descending" : ""));
			// paging
			var truckExpensePaging = truckExpenseOrdered.Skip((searchInfo.Page - 1) * searchInfo.ItemsPerPage).Take(searchInfo.ItemsPerPage).ToList();

			var datatable = new TruckExpenseDatatables()
			{
				Data = truckExpensePaging,
				Total = truckExpenseOrdered.Count()
			};

			return datatable;
		}

		public TruckExpenseViewModel GetTruckExpensesByKey(int id)
		{
			var searchTruckExpense = _truckExpenseRepository.Query(t => t.Id == id).FirstOrDefault();
			var truckExpense = Mapper.Map<TruckExpense_D, TruckExpenseViewModel>(searchTruckExpense);

			if (truckExpense != null)
			{
				// get entryClerk
				var employee = _employeeRepository.Query(e => e.EmployeeC == truckExpense.EntryClerkC).FirstOrDefault();
				if (employee != null)
				{
					truckExpense.EntryClerkN = employee.EmployeeLastN + " " + employee.EmployeeFirstN;
					truckExpense.EntryClerkRetiredD = employee.RetiredD;
				}

				// get Ob
				if (truckExpense.ObjectI == "0")
				{
					var truck = _truckRepository.Query(tr => tr.TruckC == truckExpense.Code).FirstOrDefault();
					truckExpense.ObjectNo = truck.RegisteredNo;
					truckExpense.AcquisitionD = truck.AcquisitionD;
					truckExpense.DisusedD = truck.DisusedD;
				}
				else if (truckExpense.ObjectI == "1")
				{
					var trailer = _trailerRepository.Query(tr => tr.TrailerC == truckExpense.Code).FirstOrDefault();
					truckExpense.ObjectNo = trailer.TrailerNo;
				}

				// get expenseN
				var expense = _expenseRepository.Query(ex => ex.ExpenseC == truckExpense.ExpenseC).FirstOrDefault();
				if (expense != null)
				{
					truckExpense.ExpenseN = expense.ExpenseN;
					truckExpense.TaxRate = expense.TaxRate;
					truckExpense.TaxRoundingI = expense.TaxRoundingI;
				}

				// get driverN
				if (truckExpense.DriverC != null)
				{
					var driver = _driverRepository.Query(dr => dr.DriverC == truckExpense.DriverC).FirstOrDefault();
					if (driver != null)
					{
						truckExpense.DriverN = driver.LastN + " " + driver.FirstN;
						truckExpense.RetiredD = driver.RetiredD;
					}
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
				truckExpense.ExpenseI = "T";
				truckExpense.TruckExpenseIndex = FindIndex(id);
			}

			return truckExpense;
		}

		public void CreateTruckExpense(TruckExpenseViewModel truckExpenseViewModel)
		{
			var truckExpense = Mapper.Map<TruckExpenseViewModel, TruckExpense_D>(truckExpenseViewModel);
			_truckExpenseRepository.Add(truckExpense);
			SaveTruckExpense();
		}

		public void UpdateTruckExpense(TruckExpenseViewModel truckExpense)
		{
			var id = truckExpense.Id;

			var truckExpenseCheckExist = _truckExpenseRepository.Get(t => t.Id == id);

			if (truckExpenseCheckExist != null)
			{
				truckExpenseCheckExist.InvoiceD = truckExpense.InvoiceD;
				truckExpenseCheckExist.TransportD = truckExpense.TransportD;
				truckExpenseCheckExist.Code = truckExpense.Code;
				truckExpenseCheckExist.ExpenseC = truckExpense.ExpenseC;
				truckExpenseCheckExist.EntryClerkC = truckExpense.EntryClerkC;
				truckExpenseCheckExist.DriverC = truckExpense.DriverC;
				truckExpenseCheckExist.PaymentMethodI = truckExpense.PaymentMethodI;
				truckExpenseCheckExist.SupplierMainC = truckExpense.SupplierMainC;
				truckExpenseCheckExist.SupplierSubC = truckExpense.SupplierSubC;
				truckExpenseCheckExist.Quantity = truckExpense.Quantity;
				truckExpenseCheckExist.UnitPrice = truckExpense.UnitPrice;
				truckExpenseCheckExist.Total = truckExpense.Total;
				truckExpenseCheckExist.Tax = truckExpense.Tax;
				truckExpenseCheckExist.Description = truckExpense.Description;
				truckExpenseCheckExist.ObjectI = truckExpense.ObjectI;
				truckExpenseCheckExist.IsAllocated = truckExpense.IsAllocated;

				_truckExpenseRepository.Update(truckExpenseCheckExist);
				SaveTruckExpense();
			}
		}

		public void DeleteTruckExpense(int id)
		{
			var truckExpenseToRemove = _truckExpenseRepository.Get(t => t.Id == id);
			if (truckExpenseToRemove != null)
			{
				_truckExpenseRepository.Delete(truckExpenseToRemove);
				SaveTruckExpense();
			}
		}

		public void SaveTruckExpense()
		{
			_unitOfWork.Commit();
		}
		#endregion

		public DateTime? GetSupplierInvoiceDateMax(string supplierMainC, string supplierSubC)
		{
			var orderD = from e in _truckExpenseRepository.GetAllQueryable()
						 where (e.SupplierMainC == supplierMainC & e.SupplierSubC == supplierSubC)
						 select new { Date = e.InvoiceD };
			var invoiceD = orderD.OrderByDescending(i => i.Date).FirstOrDefault();
			if (invoiceD != null)
				return invoiceD.Date;
			return null;
		}

		private int FindIndex(int id)
		{
			var data = _truckExpenseRepository.GetAllQueryable();
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
