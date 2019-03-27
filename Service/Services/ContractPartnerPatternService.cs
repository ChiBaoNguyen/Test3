using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.ContractPartnerPattern;

namespace Service.Services
{
	public interface IContractPartnerPatternService
	{
		List<ContractPartnerPatternViewModel> GetContractPartnerPatterns(string customerMainC, string customerSubC, string depatureC, string destinationC, DateTime? date);
		ContractPartnerPatternDatatable GetContractPartnerPatternsForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string custMainC, string custSubC, string departure, string destination, string contsizeI);
		ContractPartnerViewModel GetContractPartnerPattern(string customerMainC, string customerSubC, DateTime applyD);
		void SaveContractPartnerPattern();
		void CreatePattern(ContractPartnerViewModel pattern);
		void UpdatePattern(ContractPartnerViewModel pattern);
		void DeletePattern(string mainCode, string subCode, DateTime applyD);
		void DeleteDetailPattern(string mainCode, string subCode, string departureC, string destinationC, string containerSizeI, DateTime applyD, string custMainC, string custSubC);
		decimal GetUnitPriceFromContractPartner(DateTime transportD, string custMainC, string custSubC, string partMainC, string partSubC, string ordertypeI, string loca1, string loca2, string contsizeI, string calTon);
	}

	public class ContractPartnerPatternService : IContractPartnerPatternService
	{
		private readonly IContractPartnerPatternRepository _contractPartnerPatternRepository;
		private readonly IPartnerRepository _partnerRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IUnitOfWork _unitOfWork;

		public ContractPartnerPatternService(IContractPartnerPatternRepository contractPartnerPatternRepository,
			IPartnerRepository partnerRepository,
			ICustomerRepository customerRepository,
			ILocationRepository locationRepository, IUnitOfWork unitOfWork)
		{
			this._contractPartnerPatternRepository = contractPartnerPatternRepository;
			this._partnerRepository = partnerRepository;
			this._locationRepository = locationRepository;
			this._customerRepository = customerRepository;
			this._unitOfWork = unitOfWork;
		}

		public List<ContractPartnerPatternViewModel> GetContractPartnerPatterns(string customerMainC, string customerSubC,
			string depatureC, string destinationC, DateTime? date)
		{
			if (date == null) return null;
			var contractTariffPatterns =
				_contractPartnerPatternRepository.Query(p => p.PartnerMainC == customerMainC &&
															p.PartnerSubC == customerSubC &&
															p.DepartureC == depatureC &&
															p.DestinationC == destinationC &&
															p.ApplyD <= date);

			var contractList = contractTariffPatterns.ToList();

			var groupMaxContracts = from p in contractList
									group p by new { p.PartnerMainC, p.PartnerSubC, p.DepartureC, p.DestinationC, p.ContainerSizeI, p.CustomerMainC, p.CustomerSubC }
										into g
										select new
										{
											g.Key.PartnerMainC,
											g.Key.PartnerSubC,
											g.Key.DepartureC,
											g.Key.DestinationC,
											g.Key.ContainerSizeI,
											g.Key.CustomerMainC,
											g.Key.CustomerSubC,
											ApplyD = g.Max(p => p.ApplyD)
										};

			var contracts = from p in contractList
							join q in groupMaxContracts
								on
								new { p.PartnerMainC, p.PartnerSubC, p.DepartureC, p.DestinationC, p.ContainerSizeI, p.CustomerMainC, p.CustomerSubC, p.ApplyD }
								equals
								new { q.PartnerMainC, q.PartnerSubC, q.DepartureC, q.DestinationC, q.ContainerSizeI, q.CustomerMainC, q.CustomerSubC, q.ApplyD }
							select new ContractPartnerPatternViewModel()
							{
								PartnerMainC = p.PartnerMainC,
								PartnerSubC = p.PartnerSubC,
								DepartureC = p.DepartureC,
								DestinationC = p.DestinationC,
								ApplyD = p.ApplyD,
								ContainerSizeI = p.ContainerSizeI,
								CustomerMainC = p.CustomerMainC,
								CustomerSubC = p.CustomerSubC,
								UnitPrice = p.UnitPrice
							};

			return contracts.ToList();
		}

		public void SaveContractPartnerPattern()
		{
			_unitOfWork.Commit();
		}

		public ContractPartnerPatternDatatable GetContractPartnerPatternsForTable(int page, int itemsPerPage, string sortBy,
			bool reverse, string custMainC, string custSubC, string departure, string destination, string contsizeI)
		{
			var patterns = (from p in _contractPartnerPatternRepository.GetAllQueryable()
							join c in _partnerRepository.GetAllQueryable() on p.PartnerMainC equals c.PartnerMainC into pc
							from c in pc.DefaultIfEmpty()
							join d in _locationRepository.GetAllQueryable() on p.DepartureC equals d.LocationC into dp
							from d in dp.DefaultIfEmpty()
							join s in _locationRepository.GetAllQueryable() on p.DestinationC equals s.LocationC into ds
							from s in ds.DefaultIfEmpty()
							join cus in _customerRepository.GetAllQueryable() on new {p.CustomerMainC, p.CustomerSubC} equals new {cus.CustomerMainC, cus.CustomerSubC} into dt
							from cus in dt.DefaultIfEmpty()
							where p.PartnerSubC == c.PartnerSubC
							select new ContractPartnerViewModel
							{
								PartnerMainC = p.PartnerMainC,
								PartnerSubC = p.PartnerSubC,
								PartnerN = c.PartnerN,
								ContainerSizeI = p.ContainerSizeI,
								ApplyD = p.ApplyD,
								DepartureC = p.DepartureC,
								DepartureN = d.LocationN,
								DestinationC = p.DestinationC,
								DestinationN = s.LocationN,
								UnitPrice = p.UnitPrice,
								CustomerMainC = p.CustomerMainC,
								CustomerSubC = p.CustomerSubC,
								CustomerN = cus.CustomerN,
								CalculateByTon = p.CalculateByTon,
							}).Distinct().AsQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(custMainC) & !string.IsNullOrWhiteSpace(custSubC))
			{
				patterns = patterns.Where(i => (i.PartnerN != null && i.PartnerMainC.Contains(custMainC) &&
												i.PartnerSubC.Contains(custSubC)));
			}

			if (!string.IsNullOrWhiteSpace(departure))
			{
				patterns = patterns.Where(i => (i.DepartureC != null && i.DepartureC.Contains(departure)));
			}

			if (!string.IsNullOrWhiteSpace(destination))
			{
				patterns = patterns.Where(i => (i.DestinationC != null && i.DestinationC.Contains(destination)));
			}

			if (!string.IsNullOrWhiteSpace(contsizeI))
			{
				patterns = patterns.Where(i => (i.ContainerSizeI != null && i.ContainerSizeI.Contains(contsizeI)) || contsizeI == "4");
			}

			// sorting, paging          
			List<ContractPartnerViewModel> result =
				patterns.OrderBy(sortBy + (reverse ? " descending" : "")).Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var datatable = new ContractPartnerPatternDatatable()
			{
				Data = result,
				Total = patterns.Count()
			};
			return datatable;
		}

		public void CreatePattern(ContractPartnerViewModel pattern)
		{

			if (pattern != null)
			{
				var iloop = 1;
				var patterns = pattern.PartnerPatterns;

				foreach (var patternM in patterns.Select(pat => new ContractPartnerPattern_M
				{
					PartnerMainC = pattern.PartnerMainC,
					PartnerSubC = pattern.PartnerSubC,
					ApplyD = pattern.ApplyD,
					DepartureC = pat.DepartureC,
					DestinationC = pat.DestinationC,
					ContainerSizeI = pat.ContainerSizeI,
					UnitPrice = pat.UnitPrice,
					CustomerMainC = pat.CustomerMainC,
					CustomerSubC = pat.CustomerSubC,
					CalculateByTon = pat.CalculateByTon,
					DisplayLineNo = iloop++
				}))
				{
					_contractPartnerPatternRepository.Add(patternM);
				}
			}

			SaveContractPartnerPattern();
		}

		public void UpdatePattern(ContractPartnerViewModel pattern)
		{
			if (pattern != null)
			{
				var removePatterns = _contractPartnerPatternRepository.Query(p => p.PartnerMainC == pattern.PartnerMainC &&
																				 p.PartnerSubC == pattern.PartnerSubC &&
																				 p.ApplyD == pattern.ApplyD);
				if (removePatterns != null)
				{
					foreach (var remPattern in removePatterns)
					{
						_contractPartnerPatternRepository.Delete(remPattern);
					}
				}

				var iloop = 1;
				var patterns = pattern.PartnerPatterns;

				foreach (var patternM in patterns.Select(pat => new ContractPartnerPattern_M
				{
					PartnerMainC = pattern.PartnerMainC,
					PartnerSubC = pattern.PartnerSubC,
					ApplyD = pattern.ApplyD,
					DepartureC = pat.DepartureC,
					DestinationC = pat.DestinationC,
					ContainerSizeI = pat.ContainerSizeI,
					UnitPrice = pat.UnitPrice,
					CustomerMainC = pat.CustomerMainC,
					CustomerSubC = pat.CustomerSubC,
					CalculateByTon = pat.CalculateByTon,
					DisplayLineNo = iloop++
				}))
				{
					_contractPartnerPatternRepository.Add(patternM);
				}
			}
			SaveContractPartnerPattern();

		}

		public void DeletePattern(string customerMainC, string customerSubC, DateTime applyD)
		{
			var patterns = _contractPartnerPatternRepository.Query(p => p.PartnerMainC == customerMainC &&
																	   p.PartnerSubC == customerSubC &&
																	   p.ApplyD == applyD);
			if (patterns != null)
			{
				foreach (var pattern in patterns)
				{
					_contractPartnerPatternRepository.Delete(pattern);
				}
			}

			SaveContractPartnerPattern();
		}

		public void DeleteDetailPattern(string mainCode, string subCode, string departureC, string destinationC,
			string containerSizeI, DateTime applyD, string custMainC, string custSubC)
		{
			var patterns = _contractPartnerPatternRepository.Query(p => p.PartnerMainC == mainCode &&
																	   p.PartnerSubC == subCode &&
																	   p.DepartureC == departureC &&
																	   p.DestinationC == destinationC &&
																	   p.ContainerSizeI == containerSizeI &&
																	   p.ApplyD == applyD &&
																	   p.CustomerMainC == custMainC &&
																	   p.CustomerSubC == custSubC);
			if (patterns != null)
			{
				foreach (var pattern in patterns)
				{
					_contractPartnerPatternRepository.Delete(pattern);
				}
			}

			SaveContractPartnerPattern();
		}

		public ContractPartnerViewModel GetContractPartnerPattern(string customerMainC, string customerSubC, DateTime applyD)
		{
			var tarriffs = (from p in _contractPartnerPatternRepository.GetAllQueryable()
							join dp in _locationRepository.GetAllQueryable() on p.DepartureC equals dp.LocationC
							join ds in _locationRepository.GetAllQueryable() on p.DestinationC equals ds.LocationC
							join cus in _customerRepository.GetAllQueryable() on new { p.CustomerMainC, p.CustomerSubC } equals new { cus.CustomerMainC, cus.CustomerSubC } into dt
							from cus in dt.DefaultIfEmpty()
							where p.PartnerMainC == customerMainC
								  && p.PartnerSubC == customerSubC && p.ApplyD == applyD
							select new PartnerPatternViewModel
							{
								DepartureC = p.DepartureC,
								DepartureN = dp.LocationN,
								DestinationC = p.DestinationC,
								DestinationN = ds.LocationN,
								ContainerSizeI = p.ContainerSizeI,
								UnitPrice = p.UnitPrice,
								DisplayLineNo = p.DisplayLineNo,
								CustomerMainC = p.CustomerMainC,
								CustomerSubC = p.CustomerSubC,
								CustomerN = cus.CustomerN,
								CalculateByTon = p.CalculateByTon,
							}).OrderBy("DisplayLineNo asc, ContainerSizeI asc").ToList();

			var model = new ContractPartnerViewModel
			{
				PartnerMainC = customerMainC,
				PartnerSubC = customerSubC,
				PartnerN =
					_partnerRepository.Query(i => i.PartnerMainC == customerMainC && i.PartnerSubC == customerSubC)
						.Select(i => i.PartnerN)
						.FirstOrDefault(),
				ApplyD = applyD,
				PartnerPatterns = tarriffs
			};

			if (tarriffs.Count > 0)
			{
				model.PatternIndex = FindIndex(customerMainC, customerSubC, applyD);
			}

			return model;
		}

		private int FindIndex(string customerMainC, string customerSubC, DateTime applyD)
		{
			//var data = _contractTariffPatternRepository.GetAllQueryable().Distinct();
			var data = (from p in _contractPartnerPatternRepository.GetAllQueryable()
						select new ContractPartnerViewModel
						{
							PartnerMainC = p.PartnerMainC,
							PartnerSubC = p.PartnerSubC,
							ApplyD = p.ApplyD
						}).Distinct().AsQueryable();
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

						if (
							data.OrderBy("PartnerMainC descending")
								.Skip(recordsToSkip)
								.Take(halfCount)
								.Any(c => c.PartnerMainC == customerMainC && c.PartnerSubC == customerSubC && c.ApplyD == applyD))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("PartnerMainC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.PartnerMainC == customerMainC && entity.PartnerSubC == customerSubC && entity.ApplyD == applyD)
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

		public decimal GetUnitPriceFromContractPartner(DateTime transportD, string custMainC, string custSubC, string partMainC, string partSubC, string ordertypeI, string loca1, string loca2, string contsizeI, string calTon)
		{
			decimal uPrice = 0;
			var loc1 = _locationRepository.Query(l => l.LocationC == loca1).FirstOrDefault();
			loca1 = loc1 != null ? (!string.IsNullOrEmpty(loc1.AreaC) ? loc1.AreaC : loca1) : loca1;
			var loc2 = _locationRepository.Query(l => l.LocationC == loca2).FirstOrDefault();
			loca2 = loc2 != null ? (!string.IsNullOrEmpty(loc2.AreaC) ? loc2.AreaC : loca2) : loca2;

			var list =
				_contractPartnerPatternRepository.Query(
					p => DateTime.Compare(transportD, p.ApplyD) >= 0 &&
						p.CustomerMainC == custMainC && p.CustomerSubC == custSubC &&
						p.PartnerMainC == partMainC && p.PartnerSubC == partSubC &&
						p.ContainerSizeI == contsizeI && p.CalculateByTon == calTon &&
						p.DepartureC == loca1 && p.DestinationC == loca2).FirstOrDefault();
			if (list != null)
			{
				uPrice = (decimal)(list.UnitPrice ?? 0);
			}
			return uPrice;
		}
	}
}
