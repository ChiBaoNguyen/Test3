using System;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.PartnerPayment;

namespace Service.Services
{
	public interface IPartnerPaymentService
	{

		void CreatePartnerPayment(PartnerPaymentViewModel partnerPayment);

		PartnerPaymentViewModel GetPartnerPayment(string partnerMainC, string partnerSubC, DateTime partnerPaymentD, string paymentId);

		PartnerPaymentViewModel GetSupplierPayment(string supplierMainC, string supplierSubC, DateTime supplierPaymentD,
			string paymentId);
		PartnerPaymentDatatable GetPartnerPaymentForTable(int page, int itemsPerPage, string sortBy, bool reverse, string searchValue);

		void UpdatePartnerPayment(PartnerPaymentViewModel partnerPayment);

		void DeletePartnerPayment(string partnerMainC, string partnerSubC, DateTime partnerPaymentD, string paymentId);
		void DeleteSupplierPayment(string supplierMainC, string supplierSubC, DateTime supplierPaymentD, string paymentId);
		decimal? GetPartnerBalanceAmount(string partnerMainC, string partnerSubC);
	}
    public class PartnerPaymentService : IPartnerPaymentService
	{
		private readonly IPartnerPaymentRepository _partnerPaymentRepository;
		private readonly IPartnerBalanceRepository _partnerBalanceRepository;
		private readonly IPartnerRepository _partnerRepository;
		private readonly IEmployeeRepository _employeeRepository;
	    private readonly ISupplierPaymentRepository _supplierPaymentRepository;
		private readonly ISupplierRepository _supplierRepository;
		private readonly IUnitOfWork _unitOfWork;

		public PartnerPaymentService(IPartnerPaymentRepository partnerPaymentRepository,
									IPartnerBalanceRepository partnerBalanceRepository,
									IPartnerRepository partnerRepositor,
									IEmployeeRepository employeeRepository,
									ISupplierPaymentRepository supplierPaymentRepository,
									ISupplierRepository supplierRepository,
									IUnitOfWork unitOfWork)
		{
			this._partnerPaymentRepository = partnerPaymentRepository;
			this._partnerBalanceRepository = partnerBalanceRepository;
			this._partnerRepository = partnerRepositor;
			this._employeeRepository = employeeRepository;
			this._supplierPaymentRepository = supplierPaymentRepository;
			this._supplierRepository = supplierRepository;
			this._unitOfWork = unitOfWork;
		}

		public void SavePartnerPayment()
		{
			_unitOfWork.Commit();
		}

		public void CreatePartnerPayment(PartnerPaymentViewModel partnerPayment)
		{
			if (partnerPayment.ObjectI == "P")
			{
				var partnerPaymentCreate = Mapper.Map<PartnerPaymentViewModel, PartnerPayment_D>(partnerPayment);
				_partnerPaymentRepository.Add(partnerPaymentCreate);
			}
			else if (partnerPayment.ObjectI == "S")
			{
				var supplierPaymentCreate = Mapper.Map<PartnerPaymentViewModel, SupplierPayment_D>(partnerPayment);
				_supplierPaymentRepository.Add(supplierPaymentCreate);
			}
			SavePartnerPayment();
		}

		public PartnerPaymentViewModel GetPartnerPayment(string partnerMainC, string partnerSubC, DateTime partnerPaymentD, string paymentId)
		{
			//GetBalance
			//var balance = (from l in _partnerBalanceRepository.GetAllQueryable()
			//			   where l.PartnerMainC == partnerMainC &&
			//			   l.PartnerSubC == partnerSubC
			//			   group l by new { l.PartnerMainC, l.PartnerSubC } into s
			//			   select new
			//			   {
			//				   Amount = s.Sum(i => i.TotalAmount + i.TaxAmount - i.PaymentAmount),
			//			   }).FirstOrDefault();

			//var balanceAmount = balance != null ? balance.Amount : 0;

			var partner = (from c in _partnerRepository.GetAllQueryable()
							where c.PartnerMainC == partnerMainC 
							&& c.PartnerSubC == partnerSubC 
							select new PartnerPaymentViewModel
							{
								PartnerMainC = partnerMainC,
								PartnerSubC = partnerSubC,
								PartnerN = c.PartnerN,
								PartnerPaymentD = partnerPaymentD,
								PaymentId = paymentId
							}).FirstOrDefault();
			if (partner == null) return null;

			var partnerPayment = (from l in _partnerPaymentRepository.GetAllQueryable()
								  join e in _employeeRepository.GetAllQueryable() on l.EntryClerkC equals e.EmployeeC into le
								  from e in le.DefaultIfEmpty()
								   where l.PartnerMainC == partnerMainC &&
									 (l.PartnerSubC == partnerSubC) &&
									 (l.PartnerPaymentD == partnerPaymentD) &&
									 l.PaymentId == paymentId
							   select new PartnerPaymentViewModel
							   {
								   Amount = l.Amount,
								   Description = l.Description,
								   EntryClerkC = l.EntryClerkC,
								   EntryClerkN = e!= null ? e.EmployeeLastN + " " + e.EmployeeFirstN : "",
								   PaymentMethodI = l.PaymentMethodI
							   }).FirstOrDefault();

			if (partnerPayment != null)
			{
				partner.ObjectI = "P";
				//partner.PreviousBalance = balanceAmount + partnerPayment.Amount;
				partner.Amount = partnerPayment.Amount;
				//partner.NextBalance = balanceAmount;
				partner.Description = partnerPayment.Description;
				partner.Status = (int)FormStatus.Edit;
				//FindIndex
				//partner.PartnerPaymentIndex = FindIndex(partnerMainC, partnerSubC, partnerPaymentD, paymentId);
				partner.EntryClerkC = partnerPayment.EntryClerkC;
				partner.EntryClerkN = partnerPayment.EntryClerkN;
				partner.PaymentMethodI = partnerPayment.PaymentMethodI;
			}
			else
			{
				partner.ObjectI = "P";
				//partner.PreviousBalance = balanceAmount;
				partner.Amount = 0;
				//partner.NextBalance = balanceAmount;
				partner.Description = "";
				partner.Status = (int)FormStatus.Add;
			}

			return partner;
		}

		public PartnerPaymentViewModel GetSupplierPayment(string supplierMainC, string supplierSubC, DateTime supplierPaymentD, string paymentId)
		{

			var supplier = (from c in _supplierRepository.GetAllQueryable()
						   where c.SupplierMainC == supplierMainC
						   && c.SupplierSubC == supplierSubC
						   select new PartnerPaymentViewModel
						   {
							   SupplierMainC = supplierMainC,
							   SupplierSubC = supplierSubC,
							   SupplierN = c.SupplierN,
							   SupplierPaymentD = supplierPaymentD,
							   PaymentId = paymentId
						   }).FirstOrDefault();
			if (supplier == null) return null;

			var supplierPayment = (from l in _supplierPaymentRepository.GetAllQueryable()
								  join e in _employeeRepository.GetAllQueryable() on l.EntryClerkC equals e.EmployeeC into le
								  from e in le.DefaultIfEmpty()
								  where l.SupplierMainC == supplierMainC &&
									(l.SupplierSubC == supplierSubC) &&
									(l.SupplierPaymentD == supplierPaymentD) &&
									l.PaymentId == paymentId
								  select new PartnerPaymentViewModel
								  {
									  Amount = l.Amount,
									  Description = l.Description,
									  EntryClerkC = l.EntryClerkC,
									  EntryClerkN = e != null ? e.EmployeeLastN + " " + e.EmployeeFirstN : "",
									  PaymentMethodI = l.PaymentMethodI
								  }).FirstOrDefault();

			if (supplierPayment != null)
			{
				supplier.ObjectI = "S";
				supplier.Amount = supplierPayment.Amount;
				supplier.Description = supplierPayment.Description;
				supplier.Status = (int)FormStatus.Edit;
				//FindIndex
				//supplier.PartnerPaymentIndex = FindIndex(supplierMainC, supplierSubC, supplierPaymentD, paymentId);
				supplier.EntryClerkC = supplierPayment.EntryClerkC;
				supplier.EntryClerkN = supplierPayment.EntryClerkN;
				supplier.PaymentMethodI = supplierPayment.PaymentMethodI;
			}
			else
			{
				supplier.ObjectI = "S";
				supplier.Amount = 0;
				supplier.Description = "";
				supplier.Status = (int)FormStatus.Add;
			}

			return supplier;
		}

		private int FindIndex(string partnerMainC, string partnerSubC, DateTime partnerPaymentD, string paymentId)
		{
			var partnerPayment = _partnerPaymentRepository.GetAllQueryable();
			var index = 0;
			var totalRecords = partnerPayment.Count();
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

						if (partnerPayment.OrderBy("PartnerPaymentD descending").Skip(recordsToSkip).Take(halfCount)
							.Any(c => c.PartnerMainC == partnerMainC && c.PartnerSubC == partnerSubC &&
								c.PartnerPaymentD == partnerPaymentD && c.PaymentId == paymentId))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var order in partnerPayment.OrderBy("PartnerPaymentD descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (order.PartnerMainC == partnerMainC && order.PartnerSubC == partnerSubC &&
									order.PartnerPaymentD == partnerPaymentD && order.PaymentId == paymentId)
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

		public PartnerPaymentDatatable GetPartnerPaymentForTable(int page, int itemsPerPage, string sortBy, bool reverse, string searchValue)
		{
			var partnerPayment = (from l in _partnerPaymentRepository.GetAllQueryable()
								   join d in _partnerRepository.GetAllQueryable()
									   on new { l.PartnerMainC, l.PartnerSubC } equals new { d.PartnerMainC, d.PartnerSubC }
								   select new PartnerPaymentViewModel
								   {
									   ObjectI = "P",
									   PartnerMainC = d.PartnerMainC,
									   PartnerSubC = d.PartnerSubC,
									   PartnerN = d.PartnerN,
									   PartnerPaymentD = l.PartnerPaymentD,
									   PaymentId = l.PaymentId,
									   //PreviousBalance = 0,
									   Amount = l.Amount,
									   //NextBalance = 0,
									   Description = l.Description
								   }).ToList();

			var supplierPayment = (from l in _supplierPaymentRepository.GetAllQueryable()
								  join d in _supplierRepository.GetAllQueryable()
									  on new { l.SupplierMainC, l.SupplierSubC } equals new { d.SupplierMainC, d.SupplierSubC }
								  select new PartnerPaymentViewModel
								  {
									  ObjectI = "S",
									  SupplierMainC = d.SupplierMainC,
									  SupplierSubC = d.SupplierSubC,
									  SupplierN = d.SupplierN,
									  SupplierPaymentD = l.SupplierPaymentD,
									  PaymentId = l.PaymentId,
									  Amount = l.Amount,
									  Description = l.Description
								  }).ToList();

			var payment = partnerPayment.Union(supplierPayment);
						  

			// searching
			if (!string.IsNullOrWhiteSpace(searchValue) && !searchValue.Equals("null"))
			{
				searchValue = searchValue.ToLower();
				payment = payment.Where(i => i.PartnerN.ToLower().Contains(searchValue) ||
												i.SupplierN.ToLower().Contains(searchValue) ||
												i.PaymentId.ToLower().Contains(searchValue) ||
												i.Description.ToLower().Contains(searchValue));
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var partnerPaymentOrderBy = payment.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			//var partnerPaymentPaged = partnerPaymentOrderBy.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			//var destination = Mapper.Map<List<OrderPattern_M>, List<OrderPatternViewModel>>(patternsPaged);
			var datatable = new PartnerPaymentDatatable()
			{
				//Data = partnerPaymentPaged,
				Data = partnerPaymentOrderBy.ToList(),
				Total = payment.Count()
			};

			return datatable;
		}
		public void UpdatePartnerPayment(PartnerPaymentViewModel partnerPayment)
		{
			if (partnerPayment.ObjectI == "P")
			{
				var updatePartnerPayment = Mapper.Map<PartnerPaymentViewModel, PartnerPayment_D>(partnerPayment);
				_partnerPaymentRepository.Update(updatePartnerPayment);
			}
			else if (partnerPayment.ObjectI == "S")
			{
				var updateSupplierPayment = Mapper.Map<PartnerPaymentViewModel, SupplierPayment_D>(partnerPayment);
				_supplierPaymentRepository.Update(updateSupplierPayment);
			}
			SavePartnerPayment();
		}
		public void DeletePartnerPayment(string partnerMainC, string partnerSubC, DateTime partnerPaymentD, string paymentId)
		{
			var deletePartnerPayment = _partnerPaymentRepository.Get( i => i.PartnerMainC == partnerMainC &&
																		i.PartnerSubC == partnerSubC &&
																		i.PartnerPaymentD == partnerPaymentD &&
																		i.PaymentId == paymentId);
			if (deletePartnerPayment != null)
			{
				_partnerPaymentRepository.Delete(deletePartnerPayment);
				SavePartnerPayment();
			}
		}

		public void DeleteSupplierPayment(string supplierMainC, string supplierSubC, DateTime supplierPaymentD, string paymentId)
	    {
			var deleteSupplierPayment = _supplierPaymentRepository.Get(i => i.SupplierMainC == supplierMainC &&
																		i.SupplierSubC == supplierSubC &&
																		i.SupplierPaymentD == supplierPaymentD &&
																		i.PaymentId == paymentId);
			if (deleteSupplierPayment != null)
			{
				_supplierPaymentRepository.Delete(deleteSupplierPayment);
				SavePartnerPayment();
			}
	    }

	    public decimal? GetPartnerBalanceAmount(string partnerMainC, string partnerSubC)
		{
			var partnerBalance = (from l in _partnerBalanceRepository.GetAllQueryable()
								  where l.PartnerMainC == partnerMainC &&
								  l.PartnerSubC == partnerSubC
								  group l by new { l.PartnerMainC, l.PartnerSubC } into s
								  select new
								  {
									  Amount = s.Sum(i => i.TotalAmount + i.TaxAmount - i.PaymentAmount),
								  }).FirstOrDefault();
			var partnerBalanceAmount = partnerBalance != null ? partnerBalance.Amount : 0;
			return partnerBalanceAmount;
		}

	}
}
