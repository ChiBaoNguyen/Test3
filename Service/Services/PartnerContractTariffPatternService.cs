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
    public interface IPartnerContractTariffPatternService
	{
        List<PartnerContractTariffPatternViewModel> GetPartnerContractTariffPatterns(string partnerMainC, string partnerSubC, string depatureC, string destinationC);
        PartnerContractTariffPatternDatatable GetPartnerContractTariffPatternsForTable(int page, int itemsPerPage, string sortBy, bool reverse,
             string searchValue);
        PartnerContractTariffViewModel GetPartnerContractTariffPattern(string partnerMainC, string partnerSubC, DateTime applyD);
        void SavePartnerContactTariffPattern();
        void CreatePattern(PartnerContractTariffViewModel pattern);
        void UpdatePattern(PartnerContractTariffViewModel pattern);
        void DeletePattern(string partnerMainC, string partnerSubC, DateTime applyD);
	}
	public class PartnerContractTariffPatternService : IPartnerContractTariffPatternService
	{
        private readonly IPartnerContractTariffPatternRepository _tariffPatternRepository;
        private readonly IPartnerRepository _partnerRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IContainerTypeRepository _containerTypeRepository;
		private readonly IUnitOfWork _unitOfWork;

        public PartnerContractTariffPatternService(IPartnerContractTariffPatternRepository tariffPatternRepository, IPartnerRepository partnerRepository,
            IContainerTypeRepository containerTypeRepository,
            ILocationRepository locationRepository, IUnitOfWork unitOfWork)
		{
            this._tariffPatternRepository = tariffPatternRepository;
            this._partnerRepository = partnerRepository;
            this._locationRepository = locationRepository;
            this._containerTypeRepository = containerTypeRepository;
			this._unitOfWork = unitOfWork;
		}

        public List<PartnerContractTariffPatternViewModel> GetPartnerContractTariffPatterns(string partnerMainC, string partnerSubC, string depatureC, string destinationC)
		{
			var contractTariffPatterns =
                _tariffPatternRepository.Query(p => p.PartnerMainC == partnerMainC &&
															p.PartnerSubC == partnerSubC &&
															p.DepartureC == depatureC &&
															p.DestinationC == destinationC &&
															p.ApplyD < DateTime.Now);

			var contractList = contractTariffPatterns.ToList();

			var groupMaxContracts = from p in contractList
									group p by new { p.PartnerMainC, p.PartnerSubC, p.DepartureC, p.DestinationC, p.ContainerSizeI, p.ContainerTypeC }
										into g
										select new
										{
											g.Key.PartnerMainC,
											g.Key.PartnerSubC,
											g.Key.DepartureC,
											g.Key.DestinationC,
											g.Key.ContainerSizeI,
											g.Key.ContainerTypeC,
											ApplyD = g.Max(p => p.ApplyD)
										};

			var contracts = from p in contractList
							join q in groupMaxContracts
							on new { p.PartnerMainC, p.PartnerSubC, p.DepartureC, p.DestinationC, p.ContainerSizeI, p.ContainerTypeC, p.ApplyD }
							equals new { q.PartnerMainC, q.PartnerSubC, q.DepartureC, q.DestinationC, q.ContainerSizeI, q.ContainerTypeC, q.ApplyD }
                            select new PartnerContractTariffPatternViewModel()
							{
								PartnerMainC = p.PartnerMainC,
								PartnerSubC = p.PartnerSubC,
								DepartureC = p.DepartureC,
								DestinationC = p.DestinationC,
								ApplyD = p.ApplyD,
								ContainerSizeI = p.ContainerSizeI,
								ContainerTypeC = p.ContainerTypeC,
								UnitPrice = p.UnitPrice
							};

			return contracts.ToList();
		}

		public void SavePartnerContactTariffPattern()
		{
			_unitOfWork.Commit();
		}

        public PartnerContractTariffPatternDatatable GetPartnerContractTariffPatternsForTable(int page, int itemsPerPage, string sortBy, bool reverse, string searchValue)
        {
            var patterns = (from p in _tariffPatternRepository.GetAllQueryable()
                                 join c in _partnerRepository.GetAllQueryable() on p.PartnerMainC equals c.PartnerMainC 
                                 //join dp in _locationRepository.GetAllQueryable() on p.DepartureC equals dp.LocationC
                                 //join ds in _locationRepository.GetAllQueryable() on p.DestinationC equals ds.LocationC
                                 where p.PartnerSubC == c.PartnerSubC
                            select new PartnerContractTariffViewModel
                                 {
                                     PartnerMainC = p.PartnerMainC,
                                     PartnerSubC = p.PartnerSubC,
                                     PartnerN = c.PartnerN,
                                     ApplyD = p.ApplyD
                                     //DepartureC = p.DepartureC,
                                     //DepartureN = dp.LocationN,
                                     //DestinationC = p.DestinationC,
                                     //DestinationN = ds.LocationN
                                 }).Distinct().AsQueryable();

            // searching
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                searchValue = searchValue.ToLower();
                patterns = patterns.Where(i => (i.PartnerN != null && i.PartnerN.ToLower().Contains(searchValue)) ||
                    (i.PartnerMainC != null && i.PartnerMainC.ToLower().Contains(searchValue)) ||
                    (i.PartnerSubC != null && i.PartnerSubC.ToLower().Contains(searchValue)));
            }

            // sorting, paging          
            List<PartnerContractTariffViewModel> result = patterns.OrderBy(sortBy + (reverse ? " descending" : ""))
                .Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            var datatable = new PartnerContractTariffPatternDatatable()
            {
                Data = result,
                Total = result.Count()
            };
            return datatable;
        }

        public void CreatePattern(PartnerContractTariffViewModel pattern)
        {
            
            if (pattern != null)
            {
                var patterns = pattern.TarrifPatterns;

                foreach (var patternM in patterns.Select(pat => new PartnerContractTariffPattern_M
                {
                    PartnerMainC = pattern.PartnerMainC,
                    PartnerSubC = pattern.PartnerSubC,
                    ApplyD = pattern.ApplyD,
                    DepartureC = pat.DepartureC,
                    DestinationC = pat.DestinationC,
                    ContainerSizeI = pat.ContainerSizeI,
                    ContainerTypeC = pat.ContainerTypeC,
                    UnitPrice = pat.UnitPrice
                }))
                {
                    _tariffPatternRepository.Add(patternM);
                }
            }

            SavePartnerContactTariffPattern();
        }

        public void UpdatePattern(PartnerContractTariffViewModel pattern)
        {
            if (pattern != null)
            {
                var removePatterns = _tariffPatternRepository.Query(p => p.PartnerMainC == pattern.PartnerMainC &&
                                                p.PartnerSubC == pattern.PartnerSubC &&
                                                p.ApplyD == pattern.ApplyD);
                if (removePatterns != null)
                {
                    foreach (var remPattern in removePatterns)
                    {
                        _tariffPatternRepository.Delete(remPattern);
                    }
                }

                var patterns = pattern.TarrifPatterns;

                foreach (var patternM in patterns.Select(pat => new PartnerContractTariffPattern_M
                {
                    PartnerMainC = pattern.PartnerMainC,
                    PartnerSubC = pattern.PartnerSubC,
                    ApplyD = pattern.ApplyD,
                    DepartureC = pat.DepartureC,
                    DestinationC = pat.DestinationC,
                    ContainerSizeI = pat.ContainerSizeI,
                    ContainerTypeC = pat.ContainerTypeC,
                    UnitPrice = pat.UnitPrice
                }))
                {
                    _tariffPatternRepository.Add(patternM);
                }

                SavePartnerContactTariffPattern();
            }
        }

        public void DeletePattern(string partnerMainC, string partnerSubC, DateTime applyD)
        {
            var patterns = _tariffPatternRepository.Query(p => p.PartnerMainC == partnerMainC &&
                                                            p.PartnerSubC == partnerSubC &&
                                                            p.ApplyD == applyD);
            if (patterns != null)
            {
                foreach (var pattern in patterns)
                {
                    _tariffPatternRepository.Delete(pattern);
                }
            }
           
            SavePartnerContactTariffPattern();
        }

        public PartnerContractTariffViewModel GetPartnerContractTariffPattern(string partnerMainC, string partnerSubC, DateTime applyD)
        {
            var tarriffs = (from p in _tariffPatternRepository.GetAllQueryable()
                            join ct in _containerTypeRepository.GetAllQueryable() on p.ContainerTypeC equals ct.ContainerTypeC
                            join dp in _locationRepository.GetAllQueryable() on p.DepartureC equals dp.LocationC
                            join ds in _locationRepository.GetAllQueryable() on p.DestinationC equals ds.LocationC
                            where p.PartnerMainC == partnerMainC 
                            && p.PartnerSubC == partnerSubC && p.ApplyD == applyD
                            select new TariffPatternViewModel
                            {
                                DepartureC = p.DepartureC,
                                DepartureN = dp.LocationN,
                                DestinationC = p.DestinationC,
                                DestinationN = ds.LocationN,
                                ContainerTypeC = p.ContainerTypeC,
                                ContainerTypeN = ct.ContainerTypeN,
                                ContainerSizeI = p.ContainerSizeI,
                                UnitPrice = p.UnitPrice
                            }).OrderBy(i => i.ContainerSizeI).ThenBy(i => i.ContainerTypeN).ToList();

            var model = new PartnerContractTariffViewModel
            {
                PartnerMainC = partnerMainC,
                PartnerSubC = partnerSubC,
                PartnerN = _partnerRepository.Query(i => i.PartnerMainC == partnerMainC && i.PartnerSubC == partnerSubC).Select(i => i.PartnerN).FirstOrDefault(),
                ApplyD = applyD,                
                TarrifPatterns = tarriffs
            };

            return model;
        }
    }
}
