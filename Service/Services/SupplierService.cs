using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.Customer;
using Website.ViewModels.Supplier;
using Website.Enum;

namespace Service.Services
{
	public interface ISupplierService
	{
		IEnumerable<SupplierViewModel> GetSuppliers();
		IEnumerable<SupplierViewModel> GetSuppliersForReport();
		IEnumerable<SupplierInvoiceViewModel> GetInvoices(string value);
		SupplierInvoiceSettlementViewModel GetSupplierByMainCodeSubCode(string mainCode, string subCode);
		SupplierSettlementViewModel GetSupplierSettlement(string mainCode, string subCode, DateTime applyDate);
		SupplierDatatables GetSupplierForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string supplierSearchValue);
		SupplierInvoiceSettlementViewModel GetSupplierSettlementList(string supplierMainC, string supplierSubC);
		void CreateSupplier(SupplierViewModel supplier);
		SupplierStatusViewModel GetSupplierSizeByCode(string supplierCode);
		void UpdateSupplier(SupplierViewModel supplier);
		void DeleteSupplier(string mainCode, string subCode);
		void SetStatusSupplier(string mainCode, string subCode);
		IEnumerable<SupplierViewModel> GetSupplierForSuggestion(string value);
		IEnumerable<SupplierViewModel> GetSuppliersByCode(string value);
		IEnumerable<SupplierViewModel> GetMainSuppliersByCode(string value);
		SupplierViewModel GetByName(string value);
		SupplierViewModel GetByInvoiceName(string value);
		List<SupplierViewModel> GetPaymentCompanies(string value);
		void SaveSupplier();
	}
	public class SupplierService : ISupplierService
	{
		private readonly ISupplierRepository _supplierRepository;
		private readonly ISupplierSettlementRepository _supplierSettlementRepository;
		private readonly IUnitOfWork _unitOfWork;

		public SupplierService(ISupplierRepository supplierRepository, ISupplierSettlementRepository supplierSettlementRepository, IUnitOfWork unitOfWork)
		{
			this._supplierRepository = supplierRepository;
			this._supplierSettlementRepository = supplierSettlementRepository;
			this._unitOfWork = unitOfWork;
		}
		public IEnumerable<SupplierViewModel> GetSuppliers(string value)
		{
			var suppliers = _supplierRepository.Query(i => (i.SupplierMainC.Contains(value) || i.SupplierN.Contains(value)) &&
                                                    i.IsActive == Constants.ACTIVE);
			if (suppliers != null)
			{
				var destination = Mapper.Map<IEnumerable<Supplier_M>, IEnumerable<SupplierViewModel>>(suppliers);
				return destination;
			}
			return null;
		}

		public IEnumerable<SupplierInvoiceViewModel> GetInvoices(string value)
		{
			var supplier = _supplierRepository.Query(cus => cus.SupplierN.Contains(value) || cus.SupplierN.Contains(value));
			var destination = Mapper.Map<IEnumerable<Supplier_M>, IEnumerable<SupplierInvoiceViewModel>>(supplier);
			return destination;
		}

		public SupplierDatatables GetSupplierForTable(int page, int itemsPerPage, string sortBy, bool reverse, string supplierSearchValue)
		{
			var supplier = _supplierRepository.GetAllQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(supplierSearchValue))
			{
				supplierSearchValue = supplierSearchValue.ToLower();
				supplier = supplier.Where(cus => cus.SupplierN.ToLower().Contains(supplierSearchValue) ||
										(cus.SupplierShortN != null && cus.SupplierShortN.ToLower().Contains(supplierSearchValue)) ||
										(cus.ContactPerson != null && cus.ContactPerson.ToLower().Contains(supplierSearchValue)) ||
										(cus.SupplierMainC != null && cus.SupplierMainC.ToLower().Contains(supplierSearchValue)) ||
										(cus.SupplierSubC != null && cus.SupplierSubC.ToLower().Contains(supplierSearchValue)) ||
										(cus.Email != null && cus.Email.ToLower().Contains(supplierSearchValue)) ||
										(cus.PhoneNumber1 != null && cus.PhoneNumber1.ToLower().Contains(supplierSearchValue)));
			}

			var supplierOrdered = supplier.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var supplierPaged = supplierOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var destination = Mapper.Map<List<Supplier_M>, List<SupplierViewModel>>(supplierPaged);
			var supplierDatatable = new SupplierDatatables()
			{
				Data = destination,
				Total = supplier.Count()
			};
			return supplierDatatable;
		}

		public void CreateSupplier(SupplierViewModel supplier)
		{
			var supplierInsert = Mapper.Map<SupplierViewModel, Supplier_M>(supplier);
            _supplierRepository.Add(supplierInsert);

			if (supplier.SettlementList != null && supplier.SettlementList.Count > 0)
			{
				_supplierSettlementRepository.Delete(set => set.SupplierMainC == supplier.InvoiceMainC && set.SupplierSubC == supplier.InvoiceSubC);
				SaveSupplier();
				for (var iloop = 0; iloop < supplier.SettlementList.Count; iloop++)
				{
					var settlement = Mapper.Map<SupplierSettlementViewModel, SupplierSettlement_M>(supplier.SettlementList[iloop]);
					_supplierSettlementRepository.Add(settlement);
				}
			}

		    SaveSupplier();
		}

		public SupplierStatusViewModel GetSupplierSizeByCode(string supplierCode)
		{
			var SupplierStatus = new SupplierStatusViewModel();
			var supplier = _supplierRepository.Query(cus => cus.SupplierMainC == supplierCode).FirstOrDefault();
			if (supplier != null)
			{
				var supplierViewModel = Mapper.Map<Supplier_M, SupplierViewModel>(supplier);
				SupplierStatus.Supplier = supplierViewModel;
				SupplierStatus.Status = CustomerStatus.Edit.ToString();
			}
			else
			{
				SupplierStatus.Status = CustomerStatus.Add.ToString();
			}
			return SupplierStatus;
		}

		public void UpdateSupplier(SupplierViewModel supplier)
		{
			var updateSupplier = Mapper.Map<SupplierViewModel, Supplier_M>(supplier);
            _supplierRepository.Update(updateSupplier);

			//if (supplier.Settlement != null)
			//{
			//	var settlement = Mapper.Map<SupplierSettlementViewModel, SupplierSettlement_M>(supplier.Settlement);
		        
			//	var set = _supplierSettlementRepository.Query( i =>
			//				i.ApplyD == settlement.ApplyD && i.SupplierMainC == updateSupplier.SupplierMainC &&
			//				i.SupplierSubC == updateSupplier.SupplierSubC).FirstOrDefault();
			//	if (set != null)
			//	{
			//		_supplierSettlementRepository.Delete(set);
			//		_supplierSettlementRepository.Add(settlement);
			//	}
			//	else
			//	{
			//		_supplierSettlementRepository.Add(settlement);
			//	}
			//}
			if (supplier.SettlementList != null && supplier.SettlementList.Count > 0)
			{
				_supplierSettlementRepository.Delete(set => set.SupplierMainC == updateSupplier.InvoiceMainC && set.SupplierSubC == updateSupplier.InvoiceSubC);
				SaveSupplier();
				for (var iloop = 0; iloop < supplier.SettlementList.Count; iloop++)
				{
					var settlement = Mapper.Map<SupplierSettlementViewModel, SupplierSettlement_M>(supplier.SettlementList[iloop]);
					_supplierSettlementRepository.Add(settlement);
				}
			}

		    SaveSupplier();
		}

		public void DeleteSupplier(string mainCode, string subCode)
		{
			var supplierToRemove = _supplierRepository.Get(c => c.SupplierMainC == mainCode && c.SupplierSubC == subCode);
			if (supplierToRemove != null)
			{
				_supplierRepository.Delete(supplierToRemove);

                var settlements = _supplierSettlementRepository.Query(c => c.SupplierMainC == mainCode && c.SupplierSubC == subCode);
                if (settlements != null)
                {
                    foreach (var settlement in settlements)
                    {
                        _supplierSettlementRepository.Delete(settlement);

                    }
                }   
				SaveSupplier();
			}
		}

		public void SetStatusSupplier(string mainCode, string subCode)
		{
			var supplierToRemove = _supplierRepository.Get(c => c.SupplierMainC == mainCode && c.SupplierSubC == subCode); ;
			supplierToRemove.IsActive = supplierToRemove.IsActive == Constants.ACTIVE ? Constants.DEACTIVE : Constants.ACTIVE;
			_supplierRepository.Update(supplierToRemove);
			SaveSupplier();
		}

		public IEnumerable<SupplierViewModel> GetSupplierForSuggestion(string value)
		{
			var supplier = _supplierRepository.Query(i => (i.SupplierMainC.Contains(value) ||
															i.SupplierSubC.Contains(value) ||
                                                            i.SupplierN.Contains(value)) &&
                                                            i.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<Supplier_M>, IEnumerable<SupplierViewModel>>(supplier);
			return destination;
		}

		public IEnumerable<SupplierViewModel> GetSuppliersByCode(string value)
		{
			var supplier = _supplierRepository.Query(i => (i.SupplierMainC + i.SupplierSubC).StartsWith(value));
			var destination = Mapper.Map<IEnumerable<Supplier_M>, IEnumerable<SupplierViewModel>>(supplier);
			return destination;
		}

		public SupplierViewModel GetByName(string value)
		{
			var supplier = _supplierRepository.Query(s => s.SupplierN.Equals(value)).FirstOrDefault();
			if (supplier != null)
			{
				var dateNow = DateTime.Now.Date;
				var supplierSettlement = _supplierSettlementRepository.Query(s => s.SupplierMainC == supplier.SupplierMainC &&
																				  s.SupplierSubC == supplier.SupplierSubC &&
																				  s.ApplyD <= dateNow
																			);
				var supplierSettlementOrder = supplierSettlement.OrderBy("ApplyD desc").FirstOrDefault();

				var destination = Mapper.Map<Supplier_M, SupplierViewModel>(supplier);

				destination.TaxRate = 0;
				if (supplierSettlementOrder != null && supplierSettlementOrder.TaxRate != null)
				{
					destination.TaxRate = supplierSettlementOrder.TaxRate;
				}

				return destination;
			}
			return null;
		}

		public SupplierViewModel GetByInvoiceName(string value)
		{
			var supplier = (from a in _supplierSettlementRepository.GetAllQueryable()
							join b in _supplierRepository.GetAllQueryable() on new { a.SupplierMainC, a.SupplierSubC }
								equals new { b.SupplierMainC, b.SupplierSubC }
							where (b.SupplierN.Equals(value))
							select new SupplierViewModel()
							{
								SupplierMainC = a.SupplierMainC,
								SupplierSubC = a.SupplierSubC,
							}).FirstOrDefault();
			return supplier;
		}

		public void SaveSupplier()
		{
			_unitOfWork.Commit();
		}

		public SupplierSettlementViewModel GetSupplierSettlement(string mainCode, string subCode, DateTime applyDate)
		{
			var set = _supplierSettlementRepository.Query(i => i.ApplyD == applyDate.Date && i.SupplierMainC == mainCode && i.SupplierSubC == subCode).FirstOrDefault();

			if (set != null)
			{
				var settlement = Mapper.Map<SupplierSettlement_M, SupplierSettlementViewModel>(set);
				return settlement;
			}

			return null;
		}

		private int FindIndex(string mainC, string subC)
		{
			var suppliers = _supplierRepository.GetAllQueryable();
			var index = 0;
			var totalRecords = suppliers.Count();
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

						if (suppliers.OrderBy("SupplierMainC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.SupplierMainC == mainC && c.SupplierSubC == subC))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var sup in suppliers.OrderBy("SupplierMainC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (sup.SupplierMainC == mainC && sup.SupplierSubC == subC)
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

		public SupplierInvoiceSettlementViewModel GetSupplierByMainCodeSubCode(string mainCode, string subCode)
		{
			var result = new SupplierInvoiceSettlementViewModel();
			var supplier = _supplierRepository.Query(i => i.SupplierMainC == mainCode && i.SupplierSubC == subCode).FirstOrDefault();
			if (supplier != null)
			{
				var viewModel = Mapper.Map<Supplier_M, SupplierViewModel>(supplier);
				var index = FindIndex(supplier.SupplierMainC, supplier.SupplierSubC);
				viewModel.SupplierIndex = index;
				result.Supplier = viewModel;

				if (!string.IsNullOrEmpty(viewModel.InvoiceMainC) && !string.IsNullOrEmpty(viewModel.InvoiceSubC))
				{
					var invoice = _supplierRepository.Query(cus => cus.SupplierMainC == viewModel.InvoiceMainC && cus.SupplierSubC == viewModel.InvoiceSubC).FirstOrDefault();
					var invoiceViewModel = Mapper.Map<Supplier_M, SupplierInvoiceViewModel>(invoice);
					result.Invoice = invoiceViewModel;

					// get settlement list
					var settlementList = _supplierSettlementRepository.Query(set => set.SupplierMainC == viewModel.InvoiceMainC && set.SupplierSubC == viewModel.InvoiceSubC).OrderBy(i => i.ApplyD).ToList();
					result.SettlementList = Mapper.Map<List<SupplierSettlement_M>, List<SupplierSettlementViewModel>>(settlementList);
				}
				
				result.Status = Convert.ToInt32(SupplierStatus.Edit);
			}
			else
			{
				supplier = _supplierRepository.Query(i => i.SupplierMainC == mainCode && i.SupplierSubC == "000").FirstOrDefault();
				if (supplier != null)
				{
					var viewModel = Mapper.Map<Supplier_M, SupplierViewModel>(supplier);
					result.Supplier = viewModel;
					result.Supplier.SupplierSubC = subCode;

					if (!string.IsNullOrEmpty(viewModel.InvoiceMainC) && !string.IsNullOrEmpty(viewModel.InvoiceSubC))
					{
						var invoice = _supplierRepository.Query(cus => cus.SupplierMainC == viewModel.InvoiceMainC && cus.SupplierSubC == viewModel.InvoiceSubC).FirstOrDefault();
						var invoiceViewModel = Mapper.Map<Supplier_M, SupplierInvoiceViewModel>(invoice);
						result.Invoice = invoiceViewModel;

						// get settlement list
						var settlementList = _supplierSettlementRepository.Query(set => set.SupplierMainC == viewModel.InvoiceMainC && set.SupplierSubC == viewModel.InvoiceSubC).OrderBy(i => i.ApplyD).ToList();
						result.SettlementList = Mapper.Map<List<SupplierSettlement_M>, List<SupplierSettlementViewModel>>(settlementList);
					}
					result.Status = Convert.ToInt32(SupplierStatus.Add);
				}
				else
				{
					result.Status = Convert.ToInt32(SupplierStatus.Add);
				}
			}
			return result;
		}

		public SupplierInvoiceSettlementViewModel GetSupplierSettlementList(string supplierMainC, string supplierSubC)
		{
			SupplierInvoiceSettlementViewModel result = new SupplierInvoiceSettlementViewModel();

			// get invoice info
			var invoice = _supplierRepository.Query(cus => cus.SupplierMainC == supplierMainC && cus.SupplierSubC == supplierSubC).FirstOrDefault();
			var invoiceViewModel = Mapper.Map<Supplier_M, SupplierInvoiceViewModel>(invoice);
			result.Invoice = invoiceViewModel;

			var settlementList = _supplierSettlementRepository.Query(set => set.SupplierMainC == supplierMainC && set.SupplierSubC == supplierSubC).OrderBy(i => i.ApplyD).ToList();
			var settlementListViewModel = Mapper.Map<List<SupplierSettlement_M>, List<SupplierSettlementViewModel>>(settlementList);
			result.SettlementList = settlementListViewModel;

			return result;
		}

		public IEnumerable<SupplierViewModel> GetSuppliers()
		{
			var supplier = _supplierRepository.GetAllQueryable();
			// searching
			supplier = supplier.Where(p => p.IsActive == Constants.ACTIVE);

			var supplierOrdered = supplier.OrderBy("SupplierN asc");

			var supplierList = supplierOrdered.ToList();

			var destination = Mapper.Map<List<Supplier_M>, List<SupplierViewModel>>(supplierList);

			return destination;
		}
		public IEnumerable<SupplierViewModel> GetSuppliersForReport()
		{
			var supplier = _supplierRepository.GetAllQueryable().OrderBy("SupplierN asc");
			var destination = Mapper.Map<IEnumerable<Supplier_M>, IEnumerable<SupplierViewModel>>(supplier);

			return destination;
		}

		public IEnumerable<SupplierViewModel> GetMainSuppliersByCode(string value)
		{
			var supplier = _supplierRepository.Query(i => (i.SupplierMainC.Contains(value) ||
												i.SupplierShortN.Contains(value) ||
												i.SupplierN.Contains(value)) &&
												i.SupplierSubC == "000" &&
												i.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<Supplier_M>, IEnumerable<SupplierViewModel>>(supplier);
			return destination;
		}

		public List<SupplierViewModel> GetPaymentCompanies(string value)
		{
			var paymentCompanies = (from a in _supplierSettlementRepository.GetAllQueryable()
									join b in _supplierRepository.GetAllQueryable() on new { a.SupplierMainC, a.SupplierSubC }
										equals new { b.SupplierMainC, b.SupplierSubC }
									where (b.IsActive == Constants.ACTIVE &&
										   b.SupplierSubC == "000" &&
										   (b.SupplierMainC.Contains(value) ||
											b.SupplierN.Contains(value) ||
											b.SupplierShortN.Contains(value))
										  )
									select new SupplierViewModel()
									{
										SupplierMainC = b.SupplierMainC,
										SupplierSubC = b.SupplierSubC,
										SupplierN = b.SupplierN,
										SupplierShortN = b.SupplierShortN
									}).Distinct().AsQueryable().ToList();
			return paymentCompanies;
		}
	}
}
