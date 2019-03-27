using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Cryptography;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.Customer;

namespace Service.Services
{
	public interface ICustomerService
	{
		IEnumerable<CustomerViewModel> GetCustomers();
		IEnumerable<CustomerViewModel> GetCustomersForReport();
		List<CustomerViewModel> GetInvoices();
		List<CustomerViewModel> GetCustomersByInvoice(string invoiceMainC, string invoiceSubC);
		CustomerSettlementViewModel GetCustomerSettlementByMainCodeSubCode(string mainCode, string subCode);
		List<CustomerViewModel> GetPaymentCompanies(string value);
		IEnumerable<CustomerViewModel> GetCustomersByMainCode(string value);
		IEnumerable<CustomerViewModel> GetCustomersByCode(string value);
		IEnumerable<CustomerViewModel> GetMainCustomerByCode(string value);
		IEnumerable<InvoiceViewModel> GetInvoices(string value);
		string GetCustomerName(string mainCode, string subCode);
		string GetCustomerShortName(string mainCode, string subCode);
		CustomerViewModel GetByName(string value);
		CustomerViewModel GetByInvoiceName(string value);
		CustomerViewModel CheckExistCustomer(string name, string mainc, string subc);
		CustomerInvoiceViewModel GetCustomersByMainCodeSubCode(string mainCode, string subCode);
		CustomerSettlementViewModel GetCustomerSettlement(string mainCode, string subCode, DateTime applyDate);
		CustomerDatatables GetCustomersForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string custSearchValue);
		CustomerInvoiceViewModel GetCustomerSettlementList(string customerMainC, string customerSubC);
		CustomerSettlementViewModel GetCustomerSettlementByRevenueD(string mainCode, string subCode, DateTime revenueD);
		void CreateCustomer(CustomerViewModel customer);
		void UpdateCustomer(CustomerViewModel customer);
		void SetStatusCustomer(string mainCode, string subCode);
		void DeleteCustomer(string mainCode, string subCode);
		void SaveCustomer();
	}

	public class CustomerService : ICustomerService
	{
		private readonly ICustomerRepository _customerRepository;
		private readonly ICustomerSettlementRepository _customerSettlementRepository;
		private readonly ICustomerGrossProfitRepository _customerGrossProfitRepository;
		private readonly ICustomerBalanceRepository _customerBalanceRepository;
		private readonly IUnitOfWork _unitOfWork;

		public CustomerService(ICustomerRepository customerRepository,
								ICustomerSettlementRepository customerSettlementRepository,
								ICustomerGrossProfitRepository customerGrossProfitRepository,
								ICustomerBalanceRepository customerBalanceRepository,
								IUnitOfWork unitOfWork)
		{
			this._customerRepository = customerRepository;
			this._customerSettlementRepository = customerSettlementRepository;
			this._customerGrossProfitRepository = customerGrossProfitRepository;
			this._customerBalanceRepository = customerBalanceRepository;
			this._unitOfWork = unitOfWork;
		}

		#region ICustomerService members
		public IEnumerable<CustomerViewModel> GetCustomers()
		{
			var source = _customerRepository.Query(i => i.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<Customer_M>, IEnumerable<CustomerViewModel>>(source);
			return destination;
		}

		public IEnumerable<CustomerViewModel> GetCustomersForReport()
		{
			var source = _customerRepository.GetAllQueryable();
			var destination = Mapper.Map<IEnumerable<Customer_M>, IEnumerable<CustomerViewModel>>(source);
			return destination;
		}

		public List<CustomerViewModel> GetInvoices()
		{
			var result = new List<CustomerViewModel>();
			var invoiceList = _customerRepository.GetAllQueryable().Select(c => new { CustomerMainC = c.InvoiceMainC, CustomerSubC = c.InvoiceSubC }).Distinct().AsQueryable();

			var invoiceInfo = from a in _customerRepository.GetAllQueryable()
							  join b in invoiceList on new { a.CustomerMainC, a.CustomerSubC }
								equals new { b.CustomerMainC, b.CustomerSubC }
							  select new CustomerViewModel()
							  {
								  CustomerMainC = a.CustomerMainC,
								  CustomerSubC = a.CustomerSubC,
								  CustomerN = a.CustomerN,
								  CustomerShortN = a.CustomerShortN,
							  };

			return invoiceInfo.ToList();
		}

		public List<CustomerViewModel> GetCustomersByInvoice(string invoiceMainC, string invoiceSubC)
		{
			var customerList = _customerRepository.Query(cus => cus.InvoiceMainC == invoiceMainC & cus.InvoiceSubC == invoiceSubC).OrderBy("CustomerMainC asc, CustomerSubC asc").ToList();

			var destination = Mapper.Map<List<Customer_M>, List<CustomerViewModel>>(customerList);

			return destination;
		}

		public List<CustomerViewModel> GetPaymentCompanies(string value)
		{
			var paymentCompanies = (from a in _customerSettlementRepository.GetAllQueryable()
									join b in _customerRepository.GetAllQueryable() on new { a.CustomerMainC, a.CustomerSubC }
										equals new { b.CustomerMainC, b.CustomerSubC }
									where (b.IsActive == Constants.ACTIVE &&
										   b.CustomerSubC == "000" &&
										   (b.CustomerMainC.Contains(value) ||
											b.CustomerN.Contains(value) ||
											b.CustomerShortN.Contains(value))
										  )
									select new CustomerViewModel()
									{
										CustomerMainC = b.CustomerMainC,
										CustomerSubC = b.CustomerSubC,
										CustomerShortN = b.CustomerShortN,
										CustomerN = b.CustomerN
									}).Distinct().AsQueryable().ToList();
			return paymentCompanies;
		}

		public IEnumerable<CustomerViewModel> GetCustomersByMainCode(string value)
		{
			var customer = _customerRepository.Query(cus => (cus.CustomerMainC.Contains(value) ||
																			cus.CustomerN.Contains(value) ||
																			cus.CustomerShortN.Contains(value)) &&
																			cus.IsActive == Constants.ACTIVE
																			);
			var destination = Mapper.Map<IEnumerable<Customer_M>, IEnumerable<CustomerViewModel>>(customer);
			return destination;
		}

		public IEnumerable<CustomerViewModel> GetCustomersByCode(string value)
		{
			var customer = _customerRepository.Query(cus => (cus.CustomerMainC + cus.CustomerSubC).StartsWith(value) && cus.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<Customer_M>, IEnumerable<CustomerViewModel>>(customer);
			return destination;
		}


		public IEnumerable<InvoiceViewModel> GetInvoices(string value)
		{
			var customer = _customerRepository.Query(cus => cus.CustomerN.Contains(value) || cus.CustomerShortN.Contains(value));
			var destination = Mapper.Map<IEnumerable<Customer_M>, IEnumerable<InvoiceViewModel>>(customer);
			return destination;
		}

		public string GetCustomerName(string mainCode, string subCode)
		{
			var cust =
				_customerRepository.Query(cus => cus.CustomerMainC == mainCode && cus.CustomerSubC == subCode)
					.FirstOrDefault();
			if (cust != null)
			{
				return cust.CustomerN;
			}
			return "";
		}

		public string GetCustomerShortName(string mainCode, string subCode)
		{
			var cust =
				_customerRepository.Query(cus => cus.CustomerMainC == mainCode && cus.CustomerSubC == subCode)
					.FirstOrDefault();
			if (cust != null)
			{
				return cust.CustomerShortN;
			}
			return "";
		}

		public CustomerInvoiceViewModel GetCustomersByMainCodeSubCode(string mainCode, string subCode)
		{
			var custInvoice = new CustomerInvoiceViewModel();
			var customer = _customerRepository.Query(cus => cus.CustomerMainC == mainCode && cus.CustomerSubC == subCode).FirstOrDefault();
			if (customer != null)
			{
				var custViewModel = Mapper.Map<Customer_M, CustomerViewModel>(customer);
				var custIndex = FindIndex(customer.CustomerMainC, customer.CustomerSubC);
				custViewModel.CustomerIndex = custIndex;
				custInvoice.Customer = custViewModel;

				if (!string.IsNullOrEmpty(custViewModel.InvoiceMainC) && !string.IsNullOrEmpty(custViewModel.InvoiceSubC))
				{
					var invoice =
						 _customerRepository.Query(cus => cus.CustomerMainC == custViewModel.InvoiceMainC && cus.CustomerSubC == custViewModel.InvoiceSubC).FirstOrDefault();
					var invoiceViewModel = Mapper.Map<Customer_M, InvoiceViewModel>(invoice);
					custInvoice.Invoice = invoiceViewModel;

					// get init customer payment
					var initCustomerPayment =
						_customerBalanceRepository.Get(
							p => p.CustomerMainC == mainCode & p.CustomerSubC == "000" & p.CustomerBalanceD == new DateTime(1900, 1, 1));
					if (initCustomerPayment != null)
					{
						custInvoice.Invoice.InitCustomerPayment = initCustomerPayment.TotalAmount;
					}

					// get settlement list
					var settlementList = _customerSettlementRepository.Query(set => set.CustomerMainC == custViewModel.InvoiceMainC && set.CustomerSubC == custViewModel.InvoiceSubC).OrderBy(i => i.ApplyD).ToList();
					custInvoice.SettlementList = Mapper.Map<List<CustomerSettlement_M>, List<CustomerSettlementViewModel>>(settlementList);

					// get profit markup list
					var profitMarkupList = _customerGrossProfitRepository.Query(p => p.CustomerMainC == custViewModel.InvoiceMainC && p.CustomerSubC == custViewModel.InvoiceSubC).OrderBy(i => i.ApplyD).ToList();
					custInvoice.ProfitMarkupList = Mapper.Map<List<CustomerGrossProfit_M>, List<CustomerGrossProfitViewModel>>(profitMarkupList);
				}

				custInvoice.Status = Convert.ToInt32(CustomerStatus.Edit);
			}
			else
			{
				customer = _customerRepository.Query(cus => cus.CustomerMainC == mainCode && cus.CustomerSubC == "000").FirstOrDefault();
				if (customer != null)
				{
					var custViewModel = Mapper.Map<Customer_M, CustomerViewModel>(customer);
					custInvoice.Customer = custViewModel;
					custInvoice.Customer.CustomerSubC = subCode;

					if (!string.IsNullOrEmpty(custViewModel.InvoiceMainC) && !string.IsNullOrEmpty(custViewModel.InvoiceSubC))
					{
						var invoice =
							 _customerRepository.Query(cus => cus.CustomerMainC == custViewModel.InvoiceMainC && cus.CustomerSubC == custViewModel.InvoiceSubC).FirstOrDefault();
						var invoiceViewModel = Mapper.Map<Customer_M, InvoiceViewModel>(invoice);
						custInvoice.Invoice = invoiceViewModel;

						// get init customer payment
						var initCustomerPayment =
							_customerBalanceRepository.Get(
								p => p.CustomerMainC == mainCode & p.CustomerSubC == "000" & p.CustomerBalanceD == new DateTime(1900, 1, 1));
						if (initCustomerPayment != null)
						{
							custInvoice.Invoice.InitCustomerPayment = initCustomerPayment.TotalAmount;
						}

						// get settlement list
						var settlementList = _customerSettlementRepository.Query(set => set.CustomerMainC == custViewModel.InvoiceMainC && set.CustomerSubC == custViewModel.InvoiceSubC).OrderBy(i => i.ApplyD).ToList();
						custInvoice.SettlementList = Mapper.Map<List<CustomerSettlement_M>, List<CustomerSettlementViewModel>>(settlementList);

						// get profit markup list
						var profitMarkupList = _customerGrossProfitRepository.Query(p => p.CustomerMainC == custViewModel.InvoiceMainC && p.CustomerSubC == custViewModel.InvoiceSubC).OrderBy(i => i.ApplyD).ToList();
						custInvoice.ProfitMarkupList = Mapper.Map<List<CustomerGrossProfit_M>, List<CustomerGrossProfitViewModel>>(profitMarkupList);
					}
					custInvoice.Status = Convert.ToInt32(CustomerStatus.Add);
				}
				else
				{
					custInvoice.Status = Convert.ToInt32(CustomerStatus.Add);
				}
			}
			return custInvoice;
		}

		public CustomerInvoiceViewModel GetCustomerSettlementList(string customerMainC, string customerSubC)
		{
			CustomerInvoiceViewModel result = new CustomerInvoiceViewModel();

			// get invoice info
			var invoice = _customerRepository.Query(cus => cus.CustomerMainC == customerMainC && cus.CustomerSubC == customerSubC).FirstOrDefault();
			var invoiceViewModel = Mapper.Map<Customer_M, InvoiceViewModel>(invoice);
			result.Invoice = invoiceViewModel;

			var settlementList = _customerSettlementRepository.Query(set => set.CustomerMainC == customerMainC && set.CustomerSubC == customerSubC).OrderBy(i => i.ApplyD).ToList();
			var settlementListViewModel = Mapper.Map<List<CustomerSettlement_M>, List<CustomerSettlementViewModel>>(settlementList);
			result.SettlementList = settlementListViewModel;
			// get profit markup list
			var profitMarkupList = _customerGrossProfitRepository.Query(p => p.CustomerMainC == customerMainC && p.CustomerSubC == customerSubC).OrderBy(i => i.ApplyD).ToList();
			result.ProfitMarkupList = Mapper.Map<List<CustomerGrossProfit_M>, List<CustomerGrossProfitViewModel>>(profitMarkupList);

			return result;
		}

		public CustomerDatatables GetCustomersForTable(int page, int itemsPerPage, string sortBy, bool reverse, string custSearchValue)
		{
			var customers = _customerRepository.GetAllQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(custSearchValue))
			{
				custSearchValue = custSearchValue.ToLower();
				customers = customers.Where(cus => cus.CustomerN.ToLower().Contains(custSearchValue) ||
																(cus.CustomerShortN != null && cus.CustomerShortN.ToLower().Contains(custSearchValue)) ||
																(cus.ContactPerson != null && cus.ContactPerson.ToLower().Contains(custSearchValue)) ||
																(cus.CustomerMainC != null && cus.CustomerMainC.ToLower().Contains(custSearchValue)) ||
																(cus.CustomerSubC != null && cus.CustomerSubC.ToLower().Contains(custSearchValue)) ||
																(cus.Email != null && cus.Email.ToLower().Contains(custSearchValue)) ||
																(cus.PhoneNumber1 != null && cus.PhoneNumber1.ToLower().Contains(custSearchValue)));
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			//customers = customers.AsQueryable().OrderBy("\"" + sortBy + (reverse ? " desc\"" : " asc\"")).AsEnumerable();
			var customersOrdered = customers.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var customersPaged = customersOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var destination = Mapper.Map<List<Customer_M>, List<CustomerViewModel>>(customersPaged);
			for (int i = 0; i < destination.Count; i++)
			{
				string invoiceM = destination[i].InvoiceMainC;
				string invoiceS = destination[i].InvoiceSubC;
				var getcustomer = _customerRepository.Query(p => p.InvoiceMainC == invoiceM && p.InvoiceSubC == invoiceS).FirstOrDefault();
				destination[i].InvoiceN = getcustomer != null ? getcustomer.CustomerN : "";
			}
			var custDatatable = new CustomerDatatables()
			{
				Data = destination,
				Total = customers.Count()
			};
			return custDatatable;
		}

		public void CreateCustomer(CustomerViewModel customerViewModel)
		{
			var customer = Mapper.Map<CustomerViewModel, Customer_M>(customerViewModel);
			_customerRepository.Add(customer);

			if (customerViewModel.SettlementList != null && customerViewModel.SettlementList.Count > 0)
			{
				_customerSettlementRepository.Delete(set => set.CustomerMainC == customer.InvoiceMainC && set.CustomerSubC == customer.InvoiceSubC);
				SaveCustomer();
				for (var iloop = 0; iloop < customerViewModel.SettlementList.Count; iloop++)
				{
					var settlement = Mapper.Map<CustomerSettlementViewModel, CustomerSettlement_M>(customerViewModel.SettlementList[iloop]);
					_customerSettlementRepository.Add(settlement);
				}
			}

			if (customerViewModel.ProfitMarkupList != null && customerViewModel.ProfitMarkupList.Count > 0)
			{
				_customerGrossProfitRepository.Delete(p => p.CustomerMainC == customer.InvoiceMainC && p.CustomerSubC == customer.InvoiceSubC);
				SaveCustomer();
				for (var i = 0; i < customerViewModel.ProfitMarkupList.Count; i++)
				{
					var profitMarkup = Mapper.Map<CustomerGrossProfitViewModel, CustomerGrossProfit_M>(customerViewModel.ProfitMarkupList[i]);
					_customerGrossProfitRepository.Add(profitMarkup);
				}
			}

			//insert init customer payment
			var initCustomerPayment = new CustomerBalance_D()
			{
				CustomerMainC = customerViewModel.CustomerMainC,
				CustomerSubC = customerViewModel.CustomerSubC,
				CustomerBalanceD = new DateTime(1900, 1, 1),
				Amount = 0,
				TotalExpense = 0,
				CustomerSurcharge = 0,
				CustomerDiscount = 0,
				DetainAmount = 0,
				TotalAmount = customerViewModel.InitCustomerPayment ?? 0,
				TaxAmount = 0,
				PaymentAmount = 0
			};
			_customerBalanceRepository.Add(initCustomerPayment);

			SaveCustomer();
		}

		public void UpdateCustomer(CustomerViewModel customerViewModel)
		{
			var updateCustomer = Mapper.Map<CustomerViewModel, Customer_M>(customerViewModel);
			_customerRepository.Update(updateCustomer);

			//update customer settlement
			if (customerViewModel.SettlementList != null && customerViewModel.SettlementList.Count > 0)
			{
				_customerSettlementRepository.Delete(set => set.CustomerMainC == updateCustomer.InvoiceMainC && set.CustomerSubC == updateCustomer.InvoiceSubC);
				SaveCustomer();
				for (var iloop = 0; iloop < customerViewModel.SettlementList.Count; iloop++)
				{
					var settlement = Mapper.Map<CustomerSettlementViewModel, CustomerSettlement_M>(customerViewModel.SettlementList[iloop]);
					_customerSettlementRepository.Add(settlement);
				}
			}

			//update customer profit markup
			if (customerViewModel.ProfitMarkupList != null && customerViewModel.ProfitMarkupList.Count > 0)
			{
				_customerGrossProfitRepository.Delete(p => p.CustomerMainC == updateCustomer.InvoiceMainC && p.CustomerSubC == updateCustomer.InvoiceSubC);
				SaveCustomer();
				for (var i = 0; i < customerViewModel.ProfitMarkupList.Count; i++)
				{
					var profitMarkup = Mapper.Map<CustomerGrossProfitViewModel, CustomerGrossProfit_M>(customerViewModel.ProfitMarkupList[i]);
					_customerGrossProfitRepository.Add(profitMarkup);
				}
			}

			//Update init customer payment
			var currInitCustomerPayment =
				_customerBalanceRepository.Get(
					p =>
						p.CustomerMainC == customerViewModel.CustomerMainC & p.CustomerSubC == customerViewModel.CustomerSubC &
						p.CustomerBalanceD == new DateTime(1900, 1, 1));
			if (currInitCustomerPayment != null)
			{
				currInitCustomerPayment.TotalAmount = customerViewModel.InitCustomerPayment ?? 0;
				_customerBalanceRepository.Update(currInitCustomerPayment);
			}
			else
			{
				var initCustomerPayment = new CustomerBalance_D()
				{
					CustomerMainC = customerViewModel.CustomerMainC,
					CustomerSubC = customerViewModel.CustomerSubC,
					CustomerBalanceD = new DateTime(1900, 1, 1),
					Amount = 0,
					TotalExpense = 0,
					CustomerSurcharge = 0,
					CustomerDiscount = 0,
					DetainAmount = 0,
					TotalAmount = customerViewModel.InitCustomerPayment ?? 0,
					TaxAmount = 0,
					PaymentAmount = 0
				};
				_customerBalanceRepository.Add(initCustomerPayment);
			}

			SaveCustomer();
		}

		//using for active and deactive user
		public void SetStatusCustomer(string mainCode, string subCode)
		{
			var customerToRemove = _customerRepository.Get(c => c.CustomerMainC == mainCode && c.CustomerSubC == subCode); ;
			customerToRemove.IsActive = customerToRemove.IsActive == Constants.ACTIVE ? Constants.DEACTIVE : Constants.ACTIVE;
			_customerRepository.Update(customerToRemove);
			SaveCustomer();
		}

		public void DeleteCustomer(string mainCode, string subCode)
		{
			var customerToRemove = _customerRepository.Get(c => c.CustomerMainC == mainCode && c.CustomerSubC == subCode);
			if (customerToRemove != null)
			{
				_customerRepository.Delete(customerToRemove);

				var settlements = _customerSettlementRepository.Query(c => c.CustomerMainC == mainCode && c.CustomerSubC == subCode);
				if (settlements != null)
				{
					foreach (var settlement in settlements)
					{
						_customerSettlementRepository.Delete(settlement);

					}
				}

				var profitMarkups = _customerGrossProfitRepository.Query(c => c.CustomerMainC == mainCode && c.CustomerSubC == subCode);
				if (profitMarkups != null)
				{
					foreach (var profitMarkup in profitMarkups)
					{
						_customerGrossProfitRepository.Delete(profitMarkup);

					}
				}

				//Delete init customer payment
				var initCustomerPayment =
					_customerBalanceRepository.Get(p => p.CustomerMainC == mainCode && 
														p.CustomerSubC == subCode && 
														p.CustomerBalanceD == new DateTime(1900, 1, 1));
				if (initCustomerPayment != null)
				{
					_customerBalanceRepository.Delete(initCustomerPayment);
				}

				SaveCustomer();
			}
		}

		private int FindIndex(string custMainC, string custSubC)
		{
			var customers = _customerRepository.GetAllQueryable();
			var index = 0;
			var totalRecords = customers.Count();
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

						if (customers.OrderBy("CustomerMainC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.CustomerMainC == custMainC && c.CustomerSubC == custSubC))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var cust in customers.OrderBy("CustomerMainC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (cust.CustomerMainC == custMainC && cust.CustomerSubC == custSubC)
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

		public CustomerViewModel GetByName(string value)
		{
			var customer = _customerRepository.Query(s => s.CustomerN.Equals(value) && s.IsActive == Constants.ACTIVE).FirstOrDefault();
			if (customer != null)
			{
				var destination = Mapper.Map<Customer_M, CustomerViewModel>(customer);
				return destination;
			}
			return null;
		}

		public CustomerViewModel GetByInvoiceName(string value)
		{
			var customer = (from a in _customerSettlementRepository.GetAllQueryable()
							join b in _customerRepository.GetAllQueryable() on new { a.CustomerMainC, a.CustomerSubC }
								equals new { b.CustomerMainC, b.CustomerSubC }
							where (b.CustomerN.Equals(value))
							select new CustomerViewModel()
							{
								CustomerMainC = a.CustomerMainC,
								CustomerSubC = a.CustomerSubC,
							}).FirstOrDefault();
			return customer;
		}

		public CustomerViewModel CheckExistCustomer(string name, string mainc, string subc)
		{
			var customer = _customerRepository.Query(s => s.CustomerN.Equals(name) &&
															s.CustomerMainC.Equals(mainc) &&
															s.CustomerSubC.Equals(subc) &&
															s.IsActive == Constants.ACTIVE).FirstOrDefault();
			if (customer != null)
			{
				var destination = Mapper.Map<Customer_M, CustomerViewModel>(customer);
				return destination;
			}
			return null;
		}

		public void SaveCustomer()
		{
			_unitOfWork.Commit();
		}
		#endregion

		public CustomerSettlementViewModel GetCustomerSettlement(string mainCode, string subCode, DateTime applyDate)
		{
			var set = _customerSettlementRepository.Query(i => i.ApplyD == applyDate.Date && i.CustomerMainC == mainCode && i.CustomerSubC == subCode).FirstOrDefault();

			if (set != null)
			{
				var settlement = Mapper.Map<CustomerSettlement_M, CustomerSettlementViewModel>(set);
				return settlement;
			}

			return null;
		}

		public CustomerSettlementViewModel GetCustomerSettlementByRevenueD(string mainCode, string subCode, DateTime revenueD)
		{

			var customer = _customerRepository.Query(p => p.CustomerMainC == mainCode && p.CustomerSubC == subCode).FirstOrDefault();

			var settlements = _customerSettlementRepository.GetAllQueryable()
								.Where(p => p.CustomerMainC == customer.InvoiceMainC &&
										p.CustomerSubC == customer.InvoiceSubC &&
										p.ApplyD <= revenueD).ToList();

			var settlement = settlements.FirstOrDefault(p => p.ApplyD == settlements.Max(b => b.ApplyD));

			var mSettlement = Mapper.Map<CustomerSettlement_M, CustomerSettlementViewModel>(settlement);

			return mSettlement;
		}
		public CustomerSettlementViewModel GetCustomerSettlementByMainCodeSubCode(string mainCode, string subCode)
		{

			var customerSettlement = _customerSettlementRepository.Query(p => p.CustomerMainC == mainCode && p.CustomerSubC == subCode).ToList();
			var settlement = customerSettlement.FirstOrDefault(p => p.ApplyD == customerSettlement.Max(b => b.ApplyD));
			var cSettlement = Mapper.Map<CustomerSettlement_M, CustomerSettlementViewModel>(settlement);
			return cSettlement;
		}

		public IEnumerable<CustomerViewModel> GetMainCustomerByCode(string value)
		{
			var customer = _customerRepository.Query(cus => (cus.CustomerMainC.Contains(value) ||
																			cus.CustomerN.Contains(value) ||
																			cus.CustomerShortN.Contains(value)) &&
																			cus.CustomerSubC == "000" &&
																			cus.IsActive == Constants.ACTIVE
																			);
			var destination = Mapper.Map<IEnumerable<Customer_M>, IEnumerable<CustomerViewModel>>(customer);
			return destination;
		}
	}
}
