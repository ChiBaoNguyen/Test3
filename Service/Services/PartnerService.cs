using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.Customer;
using Website.ViewModels.Partner;

namespace Service.Services
{
	public interface IPartnerService
	{
		IEnumerable<PartnerViewModel> GetPartners();
		IEnumerable<PartnerViewModel> GetPartnerForSuggestion(string value);
		IEnumerable<PartnerViewModel> GetPartnersByCode(string value);
		IEnumerable<PartnerViewModel> GetMainPartnersByCode(string value);
		PartnerViewModel GetByName(string value);
		PartnerViewModel GetByInvoiceName(string value);
        IEnumerable<PartnerInvoiceViewModel> GetInvoices(string value);
        PartnerInvoiceSettlementViewModel GetPartnerByMainCodeSubCode(string mainCode, string subCode);
        PartnerSettlementViewModel GetPartnerSettlement(string mainCode, string subCode, DateTime applyDate);
        PartnerDatatables GetPartnerForTable(int page, int itemsPerPage, string sortBy, bool reverse,
             string partnerSearchValue);
		PartnerInvoiceSettlementViewModel GetPartnerSettlementList(string partnerMainC, string partnerSubC);
		PartnerSettlementViewModel GetPartnerSettlementByRevenueD(string mainCode, string subCode, DateTime invoiceD);
        void CreatePartner(PartnerViewModel partner);
        PartnerStatusViewModel GetPartnerSizeByCode(string partnerCode);
		PartnerViewModel CheckExistPartner(string name, string mainc, string subc);
        void UpdatePartner(PartnerViewModel partner);
        void DeletePartner(string mainCode, string subCode);
        void SetStatusPartner(string mainCode, string subCode);
		List<PartnerViewModel> GetInvoices();
		List<PartnerViewModel> GetPartnersByInvoice(string invoiceMainC, string invoiceSubC);
		List<PartnerViewModel> GetPaymentCompanies(string value);
		string GetPartnerShortNameOrFullName(string mainCode, string subCode);
		void SavePartner();
	}
	public class PartnerService : IPartnerService
	{
        private readonly IPartnerRepository _partnerRepository;
        private readonly IPartnerSettlementRepository _partnerSettlementRepository;
        private readonly IPartnerBalanceRepository _partnerBalanceRepository;
		private readonly IUnitOfWork _unitOfWork;

        public PartnerService(IPartnerRepository partnerRepository, IPartnerSettlementRepository partnerSettlementRepository,
            IPartnerBalanceRepository partnerBalanceRepository, IUnitOfWork unitOfWork)
		{
			this._partnerRepository = partnerRepository;
            this._partnerSettlementRepository = partnerSettlementRepository;
            this._partnerBalanceRepository = partnerBalanceRepository;
			this._unitOfWork = unitOfWork;
		}

		public IEnumerable<PartnerViewModel> GetPartnerForSuggestion(string value)
		{
			var partner = _partnerRepository.Query(par => (par.PartnerMainC.Contains(value) ||
                                                            par.PartnerN.Contains(value)) &&
                                                            par.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<Partner_M>, IEnumerable<PartnerViewModel>>(partner);
			return destination;
		}

		public IEnumerable<PartnerViewModel> GetPartnersByCode(string value)
		{
			var partner = _partnerRepository.Query(par => (par.PartnerMainC + par.PartnerSubC).StartsWith(value));
			var destination = Mapper.Map<IEnumerable<Partner_M>, IEnumerable<PartnerViewModel>>(partner);
			return destination;
		}

		public PartnerViewModel GetByName(string value)
		{
			var partner = _partnerRepository.Query(s => s.PartnerN.Equals(value)).FirstOrDefault();
			if (partner != null)
			{
				var destination = Mapper.Map<Partner_M, PartnerViewModel>(partner);
				return destination;
			}
			return null;
		}

		public PartnerViewModel GetByInvoiceName(string value)
		{
			var partner = (from a in _partnerSettlementRepository.GetAllQueryable()
							join b in _partnerRepository.GetAllQueryable() on new { a.PartnerMainC, a.PartnerSubC }
								equals new { b.PartnerMainC, b.PartnerSubC }
							where (b.PartnerN.Equals(value))
						   select new PartnerViewModel()
							{
								PartnerMainC = a.PartnerMainC,
								PartnerSubC = a.PartnerSubC,
							}).FirstOrDefault();
			return partner;
		}

		public void SavePartner()
		{
			_unitOfWork.Commit();
		}

        public IEnumerable<PartnerInvoiceViewModel> GetInvoices(string value)
        {
            var partner = _partnerRepository.Query(cus => cus.PartnerN.Contains(value) || cus.PartnerShortN.Contains(value));
            var destination = Mapper.Map<IEnumerable<Partner_M>, IEnumerable<PartnerInvoiceViewModel>>(partner);
            return destination;
        }

        public PartnerInvoiceSettlementViewModel GetPartnerByMainCodeSubCode(string mainCode, string subCode)
        {
            var result = new PartnerInvoiceSettlementViewModel();
            var partner = _partnerRepository.Query(i => i.PartnerMainC == mainCode && i.PartnerSubC == subCode).FirstOrDefault();
            if (partner != null)
            {
                var viewModel = Mapper.Map<Partner_M, PartnerViewModel>(partner);
                var index = FindIndex(partner.PartnerMainC, partner.PartnerSubC);
                viewModel.PartnerIndex = index;
                result.Partner = viewModel;

                if (!string.IsNullOrEmpty(viewModel.InvoiceMainC) && !string.IsNullOrEmpty(viewModel.InvoiceSubC))
                {
                    var invoice = _partnerRepository.Query(cus => cus.PartnerMainC == viewModel.InvoiceMainC && cus.PartnerSubC == viewModel.InvoiceSubC).FirstOrDefault();
                    var invoiceViewModel = Mapper.Map<Partner_M, PartnerInvoiceViewModel>(invoice);
                    result.Invoice = invoiceViewModel;
                    
                    // get init partner payment
                    var initPartnerPayment = _partnerBalanceRepository.Get(
                            p => p.PartnerMainC == mainCode & p.PartnerSubC == "000" & p.PartnerBalanceD == new DateTime(1900, 1, 1));
                    if (initPartnerPayment != null)
                    {
                        result.Invoice.InitPartnerPayment = initPartnerPayment.TotalAmount;
                    }
					
                    // get settlement list
					var settlementList = _partnerSettlementRepository.Query(set => set.PartnerMainC == viewModel.InvoiceMainC && set.PartnerSubC == viewModel.InvoiceSubC).OrderBy(i => i.ApplyD).ToList();
					result.SettlementList = Mapper.Map<List<PartnerSettlement_M>, List<PartnerSettlementViewModel>>(settlementList);
                }
               
                result.Status = Convert.ToInt32(PartnerStatus.Edit);
            }
            else
            {
                partner = _partnerRepository.Query(i => i.PartnerMainC == mainCode && i.PartnerSubC == "000").FirstOrDefault();
                if (partner != null)
                {
                    var viewModel = Mapper.Map<Partner_M, PartnerViewModel>(partner);
                    result.Partner = viewModel;
                    result.Partner.PartnerSubC = subCode;

					if (!string.IsNullOrEmpty(viewModel.InvoiceMainC) && !string.IsNullOrEmpty(viewModel.InvoiceSubC))
                    {
                        var invoice = _partnerRepository.Query(cus => cus.PartnerMainC == viewModel.InvoiceMainC && cus.PartnerSubC == viewModel.InvoiceSubC).FirstOrDefault();
                        var invoiceViewModel = Mapper.Map<Partner_M, PartnerInvoiceViewModel>(invoice);
                        result.Invoice = invoiceViewModel;

                        // get init partner payment
                        var initPartnerPayment = _partnerBalanceRepository.Get(
                                p => p.PartnerMainC == mainCode & p.PartnerSubC == "000" & p.PartnerBalanceD == new DateTime(1900, 1, 1));
                        if (initPartnerPayment != null)
                        {
                            result.Invoice.InitPartnerPayment = initPartnerPayment.TotalAmount;
                        }

						// get settlement list
						var settlementList = _partnerSettlementRepository.Query(set => set.PartnerMainC == viewModel.InvoiceMainC && set.PartnerSubC == viewModel.InvoiceSubC).OrderBy(i => i.ApplyD).ToList();
						result.SettlementList = Mapper.Map<List<PartnerSettlement_M>, List<PartnerSettlementViewModel>>(settlementList);
                    }
                    result.Status = Convert.ToInt32(PartnerStatus.Add);
                }
                else
                {
                    result.Status = Convert.ToInt32(PartnerStatus.Add);
                }
            }
            return result;
        }

		public PartnerInvoiceSettlementViewModel GetPartnerSettlementList(string partnerMainC, string partnerSubC)
		{
			PartnerInvoiceSettlementViewModel result = new PartnerInvoiceSettlementViewModel();

			// get invoice info
			var invoice = _partnerRepository.Query(cus => cus.PartnerMainC == partnerMainC && cus.PartnerSubC == partnerSubC).FirstOrDefault();
			var invoiceViewModel = Mapper.Map<Partner_M, PartnerInvoiceViewModel>(invoice);
			result.Invoice = invoiceViewModel;

			var settlementList = _partnerSettlementRepository.Query(set => set.PartnerMainC == partnerMainC && set.PartnerSubC == partnerSubC).OrderBy(i => i.ApplyD).ToList();
			var settlementListViewModel = Mapper.Map<List<PartnerSettlement_M>, List<PartnerSettlementViewModel>>(settlementList);
			result.SettlementList = settlementListViewModel;

			return result;
		}

        public PartnerSettlementViewModel GetPartnerSettlement(string mainCode, string subCode, DateTime applyDate)
        {
            var set = _partnerSettlementRepository.Query(i => i.ApplyD == applyDate.Date && i.PartnerMainC == mainCode && i.PartnerSubC == subCode).FirstOrDefault();

            if (set != null)
            {
                var settlement = Mapper.Map<PartnerSettlement_M, PartnerSettlementViewModel>(set);
                return settlement;
            }

            return null;
        }

        public PartnerDatatables GetPartnerForTable(int page, int itemsPerPage, string sortBy, bool reverse, string partnerSearchValue)
        {
            var partner = _partnerRepository.GetAllQueryable();
            // searching
            if (!string.IsNullOrWhiteSpace(partnerSearchValue))
            {
                partnerSearchValue = partnerSearchValue.ToLower();
                partner = partner.Where(cus => cus.PartnerN.ToLower().Contains(partnerSearchValue) ||
                                        (cus.PartnerShortN != null && cus.PartnerShortN.ToLower().Contains(partnerSearchValue)) ||
                                        (cus.ContactPerson != null && cus.ContactPerson.ToLower().Contains(partnerSearchValue)) ||
                                        (cus.PartnerMainC != null && cus.PartnerMainC.ToLower().Contains(partnerSearchValue)) ||
                                        (cus.PartnerSubC != null && cus.PartnerSubC.ToLower().Contains(partnerSearchValue)) ||
                                        (cus.Email != null && cus.Email.ToLower().Contains(partnerSearchValue)) ||
                                        (cus.PhoneNumber1 != null && cus.PhoneNumber1.ToLower().Contains(partnerSearchValue)));
            }

            var partnerOrdered = partner.OrderBy(sortBy + (reverse ? " descending" : ""));

            // paging
            var partnerPaged = partnerOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            var destination = Mapper.Map<List<Partner_M>, List<PartnerViewModel>>(partnerPaged);
            var partnerDatatable = new PartnerDatatables()
            {
                Data = destination,
                Total = partner.Count()
            };
            return partnerDatatable;
        }

        public void CreatePartner(PartnerViewModel partner)
        {
            var partnerInsert = Mapper.Map<PartnerViewModel, Partner_M>(partner);
            _partnerRepository.Add(partnerInsert);

			//if (partner.Settlement != null)
			//{
			//	var settlement = Mapper.Map<PartnerSettlementViewModel, PartnerSettlement_M>(partner.Settlement);                
			//	_partnerSettlementRepository.Add(settlement);
			//}
			if (partner.SettlementList != null && partner.SettlementList.Count > 0)
			{
				_partnerSettlementRepository.Delete(set => set.PartnerMainC == partner.InvoiceMainC && set.PartnerSubC == partner.InvoiceSubC);
				SavePartner();
				for (var iloop = 0; iloop < partner.SettlementList.Count; iloop++)
				{
					var settlement = Mapper.Map<PartnerSettlementViewModel, PartnerSettlement_M>(partner.SettlementList[iloop]);
					_partnerSettlementRepository.Add(settlement);
				}
			}

            //insert init partner payment
            var initPartnerPayment = new PartnerBalance_D()
            {
                PartnerMainC = partner.PartnerMainC,
                PartnerSubC = partner.PartnerSubC,
                PartnerBalanceD = new DateTime(1900, 1, 1),
                PartnerFee = 0,
                PartnerExpense = 0,
				PartnerSurcharge = 0,
                PartnerDiscount = 0,
				TotalAmount =  partner.InitPartnerPayment ?? 0,
				TaxAmount = 0,
				PaymentAmount = 0
            };
            _partnerBalanceRepository.Add(initPartnerPayment);

            SavePartner();
        }

        public PartnerStatusViewModel GetPartnerSizeByCode(string partnerCode)
        {
            var PartnerStatus = new PartnerStatusViewModel();
            var partner = _partnerRepository.Query(cus => cus.PartnerMainC == partnerCode).FirstOrDefault();
            if (partner != null)
            {
                var partnerViewModel = Mapper.Map<Partner_M, PartnerViewModel>(partner);
                PartnerStatus.Partner = partnerViewModel;
                PartnerStatus.Status = CustomerStatus.Edit.ToString();
            }
            else
            {
                PartnerStatus.Status = CustomerStatus.Add.ToString();
            }
            return PartnerStatus;
        }

        public void UpdatePartner(PartnerViewModel partner)
        {
            var updatePartner = Mapper.Map<PartnerViewModel, Partner_M>(partner);
            _partnerRepository.Update(updatePartner);

			//if (partner.Settlement != null)
			//{
			//	var settlement = Mapper.Map<PartnerSettlementViewModel, PartnerSettlement_M>(partner.Settlement);
                
			//	var set = _partnerSettlementRepository.Query( i =>
			//				i.ApplyD == settlement.ApplyD && i.PartnerMainC == updatePartner.PartnerMainC &&
			//				i.PartnerSubC == updatePartner.PartnerSubC).FirstOrDefault();
			//	if (set != null)
			//	{
			//		_partnerSettlementRepository.Delete(set);
			//		_partnerSettlementRepository.Add(settlement);
			//	}
			//	else
			//	{
			//		_partnerSettlementRepository.Add(settlement);
			//	}
			//}

			if (partner.SettlementList != null && partner.SettlementList.Count > 0)
			{
				_partnerSettlementRepository.Delete(set => set.PartnerMainC == updatePartner.InvoiceMainC && set.PartnerSubC == updatePartner.InvoiceSubC);
				SavePartner();
				for (var iloop = 0; iloop < partner.SettlementList.Count; iloop++)
				{
					var settlement = Mapper.Map<PartnerSettlementViewModel, PartnerSettlement_M>(partner.SettlementList[iloop]);
					_partnerSettlementRepository.Add(settlement);
				}
			}

            //Update init partner payment
            var currInitPartnerPayment = _partnerBalanceRepository.Get( p =>
                        p.PartnerMainC == partner.PartnerMainC & p.PartnerSubC == partner.PartnerSubC &
                        p.PartnerBalanceD == new DateTime(1900, 1, 1));
            if (currInitPartnerPayment != null)
            {
                currInitPartnerPayment.TotalAmount = partner.InitPartnerPayment ?? 0;
                _partnerBalanceRepository.Update(currInitPartnerPayment);
            }
            else
            {
                var initPartnerPayment = new PartnerBalance_D()
                {
                    PartnerMainC = partner.PartnerMainC,
                    PartnerSubC = partner.PartnerSubC,
                    PartnerBalanceD = new DateTime(1900, 1, 1),
                    PartnerFee = 0,
                    PartnerExpense = 0,
                    PartnerSurcharge = 0,
                    PartnerDiscount = 0,
                    TotalAmount =  partner.InitPartnerPayment ?? 0,
                    TaxAmount = 0,
                    PaymentAmount = 0
                };
                _partnerBalanceRepository.Add(initPartnerPayment);
            }

            SavePartner();
        }

        public void DeletePartner(string mainCode, string subCode)
        {
            var partnerToRemove = _partnerRepository.Get(c => c.PartnerMainC == mainCode && c.PartnerSubC == subCode);
            if (partnerToRemove != null)
            {
                _partnerRepository.Delete(partnerToRemove);
                var settlements = _partnerSettlementRepository.Query(c => c.PartnerMainC == mainCode && c.PartnerSubC == subCode);
                if (settlements != null)
                {
                    foreach (var settlement in settlements)
                    {
                        _partnerSettlementRepository.Delete(settlement);
                    }
                }                
                SavePartner();
            }

            //Delete init customer payment
            var initPartnerPayment = _partnerBalanceRepository.Get(p => p.PartnerMainC == mainCode &&
                                                    p.PartnerSubC == subCode &&
                                                    p.PartnerBalanceD == new DateTime(1900, 1, 1));
            if (initPartnerPayment != null)
            {
                _partnerBalanceRepository.Delete(initPartnerPayment);
            }
        }

        public void SetStatusPartner(string mainCode, string subCode)
        {
            var partnerToRemove = _partnerRepository.Get(c => c.PartnerMainC == mainCode && c.PartnerSubC == subCode); ;
            partnerToRemove.IsActive = partnerToRemove.IsActive == Constants.ACTIVE ? Constants.DEACTIVE : Constants.ACTIVE;
            _partnerRepository.Update(partnerToRemove);
            SavePartner();
        }

        private int FindIndex(string mainC, string subC)
        {
            var partners = _partnerRepository.GetAllQueryable();
            var index = 0;
            var totalRecords = partners.Count();
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

						if (partners.OrderBy("PartnerMainC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.PartnerMainC == mainC && c.PartnerSubC == subC))
                        {
                            if (halfCount > loopCapacity)
                            {
                                totalRecords = totalRecords - (halfCount * 1);
                                halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
                                break;
                            }
							foreach (var sup in partners.OrderBy("PartnerMainC descending").Skip(recordsToSkip).Take(halfCount))
                            {
                                if (sup.PartnerMainC == mainC && sup.PartnerSubC == subC)
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

		public IEnumerable<PartnerViewModel> GetPartners()
		{
			var partner = _partnerRepository.GetAllQueryable();
			// searching
			partner = partner.Where(p => p.IsActive == Constants.ACTIVE);

			var partnerOrdered = partner.OrderBy("PartnerN asc");

			var partnerList = partnerOrdered.ToList();

			var destination = Mapper.Map<List<Partner_M>, List<PartnerViewModel>>(partnerList);

			return destination;
		}

		public PartnerSettlementViewModel GetPartnerSettlementByRevenueD(string mainCode, string subCode, DateTime invoiceD)
		{
			var partner = _partnerRepository.Query(p => p.PartnerMainC == mainCode && p.PartnerSubC == subCode).FirstOrDefault();

			var settlements = _partnerSettlementRepository.GetAllQueryable()
								.Where(p => p.PartnerMainC == partner.InvoiceMainC &&
										p.PartnerSubC == partner.InvoiceSubC &&
										p.ApplyD <= invoiceD).ToList();

			var settlement = settlements.FirstOrDefault(p => p.ApplyD == settlements.Max(b => b.ApplyD));

			var mSettlement = Mapper.Map<PartnerSettlement_M, PartnerSettlementViewModel>(settlement);

			return mSettlement;
		}

		public PartnerViewModel CheckExistPartner(string name, string mainc, string subc)
		{
			var partner = _partnerRepository.Query(s => s.PartnerN.Equals(name) &&
															s.PartnerMainC.Equals(mainc) &&
															s.PartnerSubC.Equals(subc) &&
															s.IsActive == Constants.ACTIVE).FirstOrDefault();
			if (partner != null)
			{
				var destination = Mapper.Map<Partner_M, PartnerViewModel>(partner);
				return destination;
			}
			return null;
		}


		public IEnumerable<PartnerViewModel> GetMainPartnersByCode(string value)
		{
			var partner = _partnerRepository.Query(par => (par.PartnerMainC.Contains(value) ||
												par.PartnerN.Contains(value) ||
												par.PartnerShortN.Contains(value)) &&
												par.PartnerSubC == "000" &&
												par.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<Partner_M>, IEnumerable<PartnerViewModel>>(partner);
			return destination;
		}

		public List<PartnerViewModel> GetInvoices()
		{
			var result = new List<PartnerViewModel>();
			var invoiceList = _partnerRepository.GetAllQueryable().Select(c => new { PartnerMainC = c.InvoiceMainC, PartnerSubC = c.InvoiceSubC }).Distinct().AsQueryable();

			var invoiceInfo = from a in _partnerRepository.GetAllQueryable()
							  join b in invoiceList on new { a.PartnerMainC, a.PartnerSubC }
								equals new { b.PartnerMainC, b.PartnerSubC }
							  select new PartnerViewModel()
							  {
								  PartnerMainC = a.PartnerMainC,
								  PartnerSubC = a.PartnerSubC,
								  PartnerN = a.PartnerN,
								  PartnerShortN = a.PartnerShortN,
							  };

			return invoiceInfo.ToList();
		}

		public List<PartnerViewModel> GetPartnersByInvoice(string invoiceMainC, string invoiceSubC)
		{
			var partnerList = _partnerRepository.Query(par => par.InvoiceMainC == invoiceMainC & par.InvoiceSubC == invoiceSubC).OrderBy("PartnerMainC asc, PartnerSubC asc").ToList();

			var destination = Mapper.Map<List<Partner_M>, List<PartnerViewModel>>(partnerList);

			return destination;
		}

		public List<PartnerViewModel> GetPaymentCompanies(string value)
		{
			var paymentCompanies = (from a in _partnerSettlementRepository.GetAllQueryable()
									   join b in _partnerRepository.GetAllQueryable() on new { a.PartnerMainC, a.PartnerSubC }
										   equals new { b.PartnerMainC, b.PartnerSubC }
									   where (b.IsActive == Constants.ACTIVE &&
											  b.PartnerSubC == "000" &&
											  (b.PartnerMainC.Contains(value) ||
											   b.PartnerN.Contains(value) ||
											   b.PartnerShortN.Contains(value))
											 )
									   select new PartnerViewModel()
									   {
										   PartnerMainC = b.PartnerMainC,
										   PartnerSubC = b.PartnerSubC,
										   PartnerN = b.PartnerN,
										   PartnerShortN = b.PartnerShortN
									   }).Distinct().AsQueryable().ToList();
			return paymentCompanies;
		}

		public string GetPartnerShortNameOrFullName(string mainCode, string subCode)
		{
			var part = _partnerRepository.Query(p => p.PartnerMainC == mainCode && p.PartnerSubC == subCode)
					.FirstOrDefault();
			if (part != null)
			{
				return part.PartnerShortN ?? part.PartnerN;
			}
			return "";
		}
	}
}