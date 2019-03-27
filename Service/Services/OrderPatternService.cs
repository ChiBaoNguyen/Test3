using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.OrderPattern;

namespace Service.Services
{
	public interface IOrderPatternService
	{
		IEnumerable<OrderPatternViewModel> GetPatterns(string custMainCode, string custSubCode);
		IEnumerable<OrderPatternViewModel> GetPatternsByCode(string custMainCode, string custSubCode, string value);
		OrderPatternViewModel GetOrderPattern(string custMainCode, string custSubCode, string orderPatternC, DateTime? date);

        OrderPatternDatatable GetOrderPatternsForTable(int page, int itemsPerPage, string sortBy, bool reverse,
             string searchValue);
		OrderPatternViewModel GetByName(string value);
        void CreatePattern(OrderPatternViewModel pattern);
        void UpdatePattern(OrderPatternViewModel pattern);
        void DeletePattern(string mainCode, string subCode, string patternCode);
		void SavePattern();
	}

	public class OrderPatternService : IOrderPatternService
	{
		private readonly IOrderPatternRepository _orderPatternRepository;
		private readonly IContractTariffPatternRepository _contractTariffPatternRepository;
        private readonly ICustomerRepository _customerRepository;
		private readonly IShippingCompanyRepository _shippingCompanyRepository;
		private readonly IVesselRepository _vesselRepository;
		private readonly IShipperRepository _shipperRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly IContractTariffPatternService _contractTariffPatternService;
		private readonly IContainerTypeRepository _containerTypeRepository;
		private readonly IUnitOfWork _unitOfWork;

		public OrderPatternService(IOrderPatternRepository orderPatternRepository, IContractTariffPatternRepository contractTariffPatternRepository, 
                                    ICustomerRepository customerRepository, IShippingCompanyRepository shippingCompanyRepository,
									IVesselRepository vesselRepository, IShipperRepository shipperRepository, ILocationRepository locationRepository,
									IContractTariffPatternService contractTariffPatternService, IContainerTypeRepository containerTypeRepository, IUnitOfWork unitOfWork)
		{
			this._orderPatternRepository = orderPatternRepository;
			this._contractTariffPatternRepository = contractTariffPatternRepository;
			this._shippingCompanyRepository = shippingCompanyRepository;
			this._vesselRepository = vesselRepository;
			this._shipperRepository = shipperRepository;
			this._locationRepository = locationRepository;
			this._contractTariffPatternService = contractTariffPatternService;
			this._containerTypeRepository = containerTypeRepository;
			this._unitOfWork = unitOfWork;
		    this._customerRepository = customerRepository;
		}

		public IEnumerable<OrderPatternViewModel> GetPatterns(string custMainCode, string custSubCode)
		{

			var patterns = _orderPatternRepository.Query(pat => pat.CustomerMainC == custMainCode && pat.CustomerSubC == custSubCode);
			if (patterns != null)
			{
				var destination = Mapper.Map<IEnumerable<OrderPattern_M>, IEnumerable<OrderPatternViewModel>>(patterns);
				return destination;
			}
			return null;
		}

		public IEnumerable<OrderPatternViewModel> GetPatternsByCode(string custMainCode, string custSubCode, string value)
		{

			var patterns = _orderPatternRepository.Query(pat => pat.CustomerMainC == custMainCode && pat.CustomerSubC == custSubCode && pat.OrderPatternC.StartsWith(value));
			if (patterns != null)
			{
				var destination = Mapper.Map<IEnumerable<OrderPattern_M>, IEnumerable<OrderPatternViewModel>>(patterns);
				return destination;
			}
			return null;
		}

		public OrderPatternViewModel GetOrderPattern(string custMainCode, string custSubCode, string orderPatternC, DateTime? date)
		{
			var pattern = _orderPatternRepository.Query(pat => pat.CustomerMainC == custMainCode && pat.CustomerSubC == custSubCode && pat.OrderPatternC.Equals(orderPatternC)).FirstOrDefault();
			if (pattern == null) return null;
			var destination = Mapper.Map<OrderPattern_M, OrderPatternViewModel>(pattern);

		    var customer =
		        _customerRepository.Query(
		            p => p.CustomerMainC == destination.CustomerMainC && p.CustomerSubC == destination.CustomerSubC)
		            .FirstOrDefault();
            destination.CustomerN = customer != null ? customer.CustomerN : String.Empty;

			var shippingCompany = _shippingCompanyRepository.Query(p => p.ShippingCompanyC == destination.ShippingCompanyC).FirstOrDefault();
			destination.ShippingCompanyN = shippingCompany != null ? shippingCompany.ShippingCompanyN : String.Empty;

			var vessel = _vesselRepository.Query(p => p.VesselC == destination.VesselC).FirstOrDefault();
			destination.VesselN = vessel != null ? vessel.VesselN : String.Empty;

			var shipper = _shipperRepository.Query(p => p.ShipperC == destination.ShipperC).FirstOrDefault();
			destination.ShipperN = shipper != null ? shipper.ShipperN : String.Empty;

			var locLoadingPlace = _locationRepository.Query(p => p.LocationC == destination.LoadingPlaceC).FirstOrDefault();
			destination.LoadingPlaceN = locLoadingPlace != null ? locLoadingPlace.LocationN : String.Empty;

			var locStopoverPlace = _locationRepository.Query(p => p.LocationC == destination.StopoverPlaceC).FirstOrDefault();
			destination.StopoverPlaceN = locStopoverPlace != null ? locStopoverPlace.LocationN : String.Empty;

			var locDischargePlace = _locationRepository.Query(p => p.LocationC == destination.DischargePlaceC).FirstOrDefault();
			destination.DischargePlaceN = locDischargePlace != null ? locDischargePlace.LocationN : String.Empty;

			var conttainertype =
				_containerTypeRepository.Query(p => p.ContainerTypeC == destination.ContainerTypeC).FirstOrDefault();
			destination.ContainerTypeN = conttainertype != null ? conttainertype.ContainerTypeN : String.Empty;

			destination.PatternIndex = FindIndex(custMainCode, custSubCode, orderPatternC);

			//get list of contract tariff pattern
			destination.ContractTariffPatterns = _contractTariffPatternService.GetContractTariffPatterns(destination.CustomerMainC, destination.CustomerSubC, destination.LoadingPlaceC, destination.DischargePlaceC, date);
			return destination;
		}

		public void SavePattern()
		{
			_unitOfWork.Commit();
		}

        public OrderPatternDatatable GetOrderPatternsForTable(int page, int itemsPerPage, string sortBy, bool reverse, string searchValue)
        {
            //var orderpatterns = _orderPatternRepository.GetAllQueryable().Join();
            List<OrderPatternViewModel> result;

            var orderpatterns = (from p in _orderPatternRepository.GetAllQueryable()
                                 join sc in _shippingCompanyRepository.GetAllQueryable() on p.ShippingCompanyC equals sc.ShippingCompanyC into psc
                                 from sc in psc.DefaultIfEmpty()
                                 join c in _customerRepository.GetAllQueryable() on p.CustomerMainC equals c.CustomerMainC //and p.CustomerSubC equals c.CustomerSub
                                 join ve in _vesselRepository.GetAllQueryable() on p.VesselC equals ve.VesselC into pve
                                 from ve in pve.DefaultIfEmpty()
                                 join sh in _shipperRepository.GetAllQueryable() on p.ShipperC equals sh.ShipperC into psh
                                 from sh in psh.DefaultIfEmpty()
                                 join l1 in _locationRepository.GetAllQueryable() on p.LoadingPlaceC equals l1.LocationC into pl1
                                 from l1 in pl1.DefaultIfEmpty()
                                 join l2 in _locationRepository.GetAllQueryable() on p.StopoverPlaceC equals l2.LocationC into pl2
                                 from l2 in pl2.DefaultIfEmpty()
                                 join l3 in _locationRepository.GetAllQueryable() on p.DischargePlaceC equals l3.LocationC into pl3
                                 from l3 in pl3.DefaultIfEmpty()
								 join cont in _containerTypeRepository.GetAllQueryable() on p.ContainerTypeC equals cont.ContainerTypeC into cont1
								 from cont in cont1.DefaultIfEmpty()
                                 where p.CustomerSubC == c.CustomerSubC
                                 select new OrderPatternViewModel
                                 {
                                     CustomerMainC = p.CustomerMainC,
                                     CustomerSubC = p.CustomerSubC,
                                     CustomerN = c.CustomerN,
                                     OrderPatternC = p.OrderPatternC,
                                     OrderPatternN = p.OrderPatternN,
                                     OrderTypeI = p.OrderTypeI,
                                     ShippingCompanyC = p.ShippingCompanyC,
                                     ShippingCompanyN = sc.ShippingCompanyN,
                                     VesselC = p.VesselC,
                                     VesselN = ve.VesselN,
                                     ShipperC = p.ShipperC,
                                     ShipperN = sh.ShipperN,
                                     LoadingPlaceC = p.LoadingPlaceC,
                                     LoadingPlaceN = l1.LocationN,
                                     StopoverPlaceC = p.StopoverPlaceC,
                                     StopoverPlaceN = l2.LocationN,
                                     DischargePlaceC = p.DischargePlaceC,
                                     DischargePlaceN = l3.LocationN,
									 ContainerSizeI = p.ContainerSizeI,
									 ContainerTypeN = cont.ContainerTypeN,
									 CommodityN = p.CommodityN,
									 UnitPrice = p.UnitPrice,
									 CalculateByTon = p.CalculateByTon
                                 }).AsQueryable();

            // searching
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                searchValue = searchValue.ToLower();
                orderpatterns = orderpatterns.Where(i => i.OrderPatternC.ToLower().Contains(searchValue) ||
                                    (i.CustomerN != null && i.CustomerN.ToLower().Contains(searchValue)) ||
                                    (i.ShippingCompanyN != null && i.ShippingCompanyN.ToLower().Contains(searchValue)) ||
                                    (i.ShipperN != null && i.ShipperN.ToLower().Contains(searchValue)) ||
                                    (i.LoadingPlaceN != null && i.LoadingPlaceN.ToLower().Contains(searchValue)) ||
                                    (i.StopoverPlaceN != null && i.StopoverPlaceN.ToLower().Contains(searchValue)) ||
                                    (i.DischargePlaceN != null && i.DischargePlaceN.ToLower().Contains(searchValue)));
            }

            // sorting, paging          
            result = orderpatterns.OrderBy(sortBy + (reverse ? " descending" : ""))
                .Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            //var destination = Mapper.Map<List<OrderPattern_M>, List<OrderPatternViewModel>>(patternsPaged);
            var datatable = new OrderPatternDatatable()
            {
                Data = result,
				Total = orderpatterns.Count()
            };
            return datatable;
        }

        public void CreatePattern(OrderPatternViewModel pattern)
        {
            var createPattern = Mapper.Map<OrderPatternViewModel, OrderPattern_M>(pattern);
            _orderPatternRepository.Add(createPattern);
            SavePattern();
        }

        public void UpdatePattern(OrderPatternViewModel pattern)
        {
            var updatePattern = Mapper.Map<OrderPatternViewModel, OrderPattern_M>(pattern);
            _orderPatternRepository.Update(updatePattern);
            SavePattern();
        }

        public void DeletePattern(string mainCode, string subCode, string patternCode)
        {
            var deletePattern = _orderPatternRepository.Get(i => i.CustomerMainC == mainCode && i.CustomerSubC == subCode && i.OrderPatternC == patternCode);
            if (deletePattern != null)
            {
                _orderPatternRepository.Delete(deletePattern);
                SavePattern();
            }
        }

		public OrderPatternViewModel GetByName(string value)
		{
			var pattern = _orderPatternRepository.Query(s => s.OrderPatternN.Equals(value)).FirstOrDefault();
			if (pattern != null)
			{
				var destination = Mapper.Map<OrderPattern_M, OrderPatternViewModel>(pattern);
				return destination;
			}
			return null;
		}

		private int FindIndex(string mainC, string subC, string patternC)
		{
			var data = _orderPatternRepository.GetAllQueryable();
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

						if (data.OrderBy("OrderPatternC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.CustomerMainC == mainC && c.CustomerSubC == subC && c.OrderPatternC == patternC))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("OrderPatternC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.CustomerMainC == mainC && entity.CustomerSubC == subC && entity.OrderPatternC == patternC)
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
