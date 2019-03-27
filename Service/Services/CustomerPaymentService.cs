using System;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.CustomerPayment;

namespace Service.Services
{
	public interface ICustomerPaymentService
	{

		void CreateCustomerPayment(CustomerPaymentViewModel customerPayment);

		CustomerPaymentViewModel GetCustomerPayment(string customerMainC, string customerSubC, DateTime customerPaymentD, string paymentId);

		CustomerPaymentDatatable GetCustomerPaymentForTable(int page, int itemsPerPage, string sortBy, bool reverse, string searchValue);

		void UpdateCustomerPayment(CustomerPaymentViewModel customerPayment);

		void DeleteCustomerPayment(string customerMainC, string customerSubC, DateTime customerPaymentD, string paymentId);
		decimal? GetCustomerBalanceAmount(string customerMainC, string customerSubC);
		
	}
	public class CustomerPaymentService : ICustomerPaymentService
	{
		private readonly ICustomerPaymentRepository _customerPaymentRepository;
		private readonly ICustomerBalanceRepository _customerBalanceRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly IUnitOfWork _unitOfWork;

		public CustomerPaymentService(ICustomerPaymentRepository customerPaymentRepository,
									ICustomerBalanceRepository customerBalanceRepository,
									ICustomerRepository customerRepositor,
									IEmployeeRepository employeeRepository,
									IUnitOfWork unitOfWork)
		{
			this._customerPaymentRepository = customerPaymentRepository;
			this._customerBalanceRepository = customerBalanceRepository;
			this._customerRepository = customerRepositor;
			this._employeeRepository = employeeRepository;
			this._unitOfWork = unitOfWork;
		}

		public void SaveCustomerPayment()
		{
			_unitOfWork.Commit();
		}

		public void CreateCustomerPayment(CustomerPaymentViewModel customerPayment)
		{
			var customerPaymentCreate = Mapper.Map<CustomerPaymentViewModel, CustomerPayment_D>(customerPayment);
			_customerPaymentRepository.Add(customerPaymentCreate);
			SaveCustomerPayment();
		}

		public CustomerPaymentViewModel GetCustomerPayment(string customerMainC, string customerSubC, DateTime customerPaymentD, string paymentId)
		{
			//GetBalance
			//var balance = (from l in _customerBalanceRepository.GetAllQueryable()
			//			   where l.CustomerMainC == customerMainC &&
			//			   l.CustomerSubC == customerSubC
			//			   group l by new { l.CustomerMainC, l.CustomerSubC } into s
			//			   select new
			//			   {
			//				   Amount = s.Sum(i => i.TotalAmount + i.TaxAmount - i.PaymentAmount),
			//			   }).FirstOrDefault();

			//var balanceAmount = balance != null ? balance.Amount : 0;

			var customer = (from c in _customerRepository.GetAllQueryable()
							where c.CustomerMainC == customerMainC 
							&& c.CustomerSubC == customerSubC 
							select new CustomerPaymentViewModel
							{
								CustomerMainC = customerMainC,
								CustomerSubC = customerSubC,
								CustomerN = c.CustomerN,
								CustomerPaymentD = customerPaymentD,
								PaymentId = paymentId
							}).FirstOrDefault();
			if (customer == null) return null;

			var customerPayment = (from l in _customerPaymentRepository.GetAllQueryable()
								   join e in _employeeRepository.GetAllQueryable() on l.EntryClerkC equals e.EmployeeC into le
								   from e in le.DefaultIfEmpty()
								   where l.CustomerMainC == customerMainC &&
									 (l.CustomerSubC == customerSubC) &&
									 (l.CustomerPaymentD == customerPaymentD) &&
									 l.PaymentId == paymentId
							   select new CustomerPaymentViewModel
							   {
								   Amount = l.Amount,
								   Description = l.Description,
								   EntryClerkC = l.EntryClerkC,
								   EntryClerkN = e != null ? e.EmployeeLastN + " " + e.EmployeeFirstN : "",
									PaymentMethodI = l.PaymentMethodI
							   }).FirstOrDefault();

			if (customerPayment != null)
			{
				//customer.PreviousBalance = balanceAmount + customerPayment.Amount;
				customer.Amount = customerPayment.Amount;
				//customer.NextBalance = balanceAmount;
				customer.Description = customerPayment.Description;
				customer.Status = (int)FormStatus.Edit;
				//FindIndex
				customer.CustomerPaymentIndex = FindIndex(customerMainC, customerSubC, customerPaymentD, paymentId);
				customer.EntryClerkC = customerPayment.EntryClerkC;
				customer.EntryClerkN = customerPayment.EntryClerkN;
				customer.PaymentMethodI = customerPayment.PaymentMethodI;
			}
			else
			{
				//customer.PreviousBalance = balanceAmount;
				customer.Amount = 0;
				//customer.NextBalance = balanceAmount;
				customer.Description = "";
				customer.Status = (int)FormStatus.Add;
			}

			return customer;
		}

		private int FindIndex(string customerMainC, string customerSubC, DateTime customerPaymentD, string paymentId)
		{
			var customerPayment = _customerPaymentRepository.GetAllQueryable();
			var index = 0;
			var totalRecords = customerPayment.Count();
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

						if (customerPayment.OrderBy("CustomerPaymentD descending").Skip(recordsToSkip).Take(halfCount)
							.Any(c => c.CustomerMainC == customerMainC && c.CustomerSubC == customerSubC &&
								c.CustomerPaymentD == customerPaymentD && c.PaymentId == paymentId))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var order in customerPayment.OrderBy("CustomerPaymentD descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (order.CustomerMainC == customerMainC && order.CustomerSubC == customerSubC &&
									order.CustomerPaymentD == customerPaymentD && order.PaymentId == paymentId)
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

		public CustomerPaymentDatatable GetCustomerPaymentForTable(int page, int itemsPerPage, string sortBy, bool reverse, string searchValue)
		{
			var customerPayment = (from l in _customerPaymentRepository.GetAllQueryable()
								   join d in _customerRepository.GetAllQueryable()
									   on new { l.CustomerMainC, l.CustomerSubC } equals new { d.CustomerMainC, d.CustomerSubC }
								   select new CustomerPaymentViewModel
								   {
									   CustomerMainC = d.CustomerMainC,
									   CustomerSubC = d.CustomerSubC,
									   CustomerN = d.CustomerN,
									   CustomerPaymentD = l.CustomerPaymentD,
									   PaymentId = l.PaymentId,
									   //PreviousBalance = 0,
									   Amount = l.Amount,
									   //NextBalance = 0,
									   Description = l.Description
								   }).AsQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(searchValue)  && !searchValue.Equals("null"))
			{
				searchValue = searchValue.ToLower();
				customerPayment = customerPayment.Where(i => i.CustomerN.ToLower().Contains(searchValue) ||
															i.PaymentId.ToLower().Contains(searchValue) ||
															i.Description.ToLower().Contains(searchValue));
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var customerPaymentOrderBy = customerPayment.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var customerPaymentPaged = customerPaymentOrderBy.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			//var destination = Mapper.Map<List<OrderPattern_M>, List<OrderPatternViewModel>>(patternsPaged);
			var datatable = new CustomerPaymentDatatable()
			{
				Data = customerPaymentPaged,
				Total = customerPaymentPaged.Count()
			};

			return datatable;
		}
		public void UpdateCustomerPayment(CustomerPaymentViewModel customerPayment)
		{
			var updateCustomerPayment = Mapper.Map<CustomerPaymentViewModel, CustomerPayment_D>(customerPayment);
			_customerPaymentRepository.Update(updateCustomerPayment);
			SaveCustomerPayment();
		}
		public void DeleteCustomerPayment(string customerMainC, string customerSubC, DateTime customerPaymentD, string paymentId)
		{
			var deleteCustomerPayment = _customerPaymentRepository.Get( i => i.CustomerMainC == customerMainC &&
																		i.CustomerSubC == customerSubC &&
																		i.CustomerPaymentD == customerPaymentD &&
																		i.PaymentId == paymentId);
			if (deleteCustomerPayment != null)
			{
				_customerPaymentRepository.Delete(deleteCustomerPayment);
				SaveCustomerPayment();
			}
		}
		public decimal? GetCustomerBalanceAmount(string customerMainC, string customerSubC)
		{
			var customerBalance = (from l in _customerBalanceRepository.GetAllQueryable()
								   where l.CustomerMainC == customerMainC &&
								   l.CustomerSubC == customerSubC
								   group l by new { l.CustomerMainC, l.CustomerSubC } into s
								   select new
								   {
									   Amount = s.Sum(i => i.TotalAmount + i.TaxAmount - i.PaymentAmount),
								   }).FirstOrDefault();
			var customerBalanceAmount = customerBalance != null ? customerBalance.Amount : 0;
			return customerBalanceAmount;
		}


		
	}
}
