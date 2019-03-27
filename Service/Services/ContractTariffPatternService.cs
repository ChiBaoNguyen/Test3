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
	public interface IContractTariffPatternService
	{
		List<ContractTariffPatternViewModel> GetContractTariffPatterns(string customerMainC, string customerSubC, string depatureC, string destinationC, DateTime? date);
		ContractTariffPatternDatatable GetContractTariffPatternsForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string custMainC, string custSubC, string departure, string destination, string commo, string contsizeI);
		ContractTariffViewModel GetContractTariffPattern(string customerMainC, string customerSubC, DateTime applyD);
		void SaveContactTariffPattern();
		void CreatePattern(ContractTariffViewModel pattern);
		void UpdatePattern(ContractTariffViewModel pattern);
		void DeletePattern(string customerMainC, string customerSubC, DateTime applyD);
		void DeleteDetailPattern(string customerMainC, string customerSubC, string departureC, string destinationC, string containerSizeI, DateTime applyD);
		decimal GetUnitPriceFromContractTariff(DateTime orderD, string custMainC, string custSubC, string ordertypeI, string loca1, string loca2, string contsizeI, string calTon, string comC);

		//List<TariffPatternViewModel> GetTariffPattern();
	}

	public class ContractTariffPatternService : IContractTariffPatternService
	{
		private readonly IContractTariffPatternRepository _contractTariffPatternRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly IUnitOfWork _unitOfWork;

		public ContractTariffPatternService(IContractTariffPatternRepository contractTariffPatternRepository,
			ICustomerRepository customerRepository,
			ILocationRepository locationRepository, IUnitOfWork unitOfWork)
		{
			this._contractTariffPatternRepository = contractTariffPatternRepository;
			this._customerRepository = customerRepository;
			this._locationRepository = locationRepository;
			this._unitOfWork = unitOfWork;
		}

		public List<ContractTariffPatternViewModel> GetContractTariffPatterns(string customerMainC, string customerSubC,
			string depatureC, string destinationC, DateTime? date)
		{
			if (date == null) return null;
			var contractTariffPatterns =
				_contractTariffPatternRepository.Query(p => p.CustomerMainC == customerMainC &&
															p.CustomerSubC == customerSubC &&
															p.DepartureC == depatureC &&
															p.DestinationC == destinationC &&
															p.ApplyD <= date);

			var contractList = contractTariffPatterns.ToList();

			var groupMaxContracts = from p in contractList
									group p by new { p.CustomerMainC, p.CustomerSubC, p.DepartureC, p.DestinationC, p.ContainerSizeI }
										into g
										select new
										{
											g.Key.CustomerMainC,
											g.Key.CustomerSubC,
											g.Key.DepartureC,
											g.Key.DestinationC,
											g.Key.ContainerSizeI,
											ApplyD = g.Max(p => p.ApplyD)
										};

			var contracts = from p in contractList
							join q in groupMaxContracts
								on
								new { p.CustomerMainC, p.CustomerSubC, p.DepartureC, p.DestinationC, p.ContainerSizeI, p.ApplyD }
								equals
								new { q.CustomerMainC, q.CustomerSubC, q.DepartureC, q.DestinationC, q.ContainerSizeI, q.ApplyD }
							select new ContractTariffPatternViewModel()
							{
								CustomerMainC = p.CustomerMainC,
								CustomerSubC = p.CustomerSubC,
								DepartureC = p.DepartureC,
								DestinationC = p.DestinationC,
								ApplyD = p.ApplyD,
								ContainerSizeI = p.ContainerSizeI,
								UnitPrice = p.UnitPrice
							};

			return contracts.ToList();
		}

		public void SaveContactTariffPattern()
		{
			_unitOfWork.Commit();
		}

		public ContractTariffPatternDatatable GetContractTariffPatternsForTable(int page, int itemsPerPage, string sortBy,
			bool reverse, string custMainC, string custSubC, string departure, string destination, string commo, string contsizeI)
		{
			var patterns = (from p in _contractTariffPatternRepository.GetAllQueryable()
							join c in _customerRepository.GetAllQueryable() on p.CustomerMainC equals c.CustomerMainC into pc
							from c in pc.DefaultIfEmpty()
							join d in _locationRepository.GetAllQueryable() on p.DepartureC equals d.LocationC into dp
							from d in dp.DefaultIfEmpty()
							join s in _locationRepository.GetAllQueryable() on p.DestinationC equals s.LocationC into ds
							from s in ds.DefaultIfEmpty()
							where p.CustomerSubC == c.CustomerSubC
							select new ContractTariffViewModel
							{
								CustomerMainC = p.CustomerMainC,
								CustomerSubC = p.CustomerSubC,
								CustomerN = c.CustomerN,
								ContainerSizeI = p.ContainerSizeI,
								ApplyD = p.ApplyD,
								DepartureC = p.DepartureC,
								DepartureN = d.LocationN,
								DestinationC = p.DestinationC,
								DestinationN = s.LocationN,
								UnitPrice = p.UnitPrice,
								CommodityC = p.CommodityC,
								CommodityN = p.CommodityN,
								CalculateByTon = p.CalculateByTon,
							}).Distinct().AsQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(custMainC) & !string.IsNullOrWhiteSpace(custSubC))
			{
				patterns = patterns.Where(i => (i.CustomerN != null && i.CustomerMainC.Contains(custMainC) &&
												i.CustomerSubC.Contains(custSubC)));
			}

			if (!string.IsNullOrWhiteSpace(departure))
			{
				patterns = patterns.Where(i => (i.DepartureC != null && i.DepartureC.Contains(departure)));
			}

			if (!string.IsNullOrWhiteSpace(destination))
			{
				patterns = patterns.Where(i => (i.DestinationC != null && i.DestinationC.Contains(destination)));
			}

			if (!string.IsNullOrWhiteSpace(commo))
			{
				patterns = patterns.Where(i => (i.CommodityC != null && i.CommodityC.Contains(commo)));
			}

			if (!string.IsNullOrWhiteSpace(contsizeI))
			{
				patterns = patterns.Where(i => (i.ContainerSizeI != null && i.ContainerSizeI.Contains(contsizeI)) || contsizeI == "4");
			}

			// sorting, paging          
			List<ContractTariffViewModel> result =
				patterns.OrderBy(sortBy + (reverse ? " descending" : "")).Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var datatable = new ContractTariffPatternDatatable()
			{
				Data = result,
				Total = patterns.Count()
			};
			return datatable;
		}

		public void CreatePattern(ContractTariffViewModel pattern)
		{

			if (pattern != null)
			{
				var iloop = 1;
				var patterns = pattern.TarrifPatterns;

				foreach (var patternM in patterns.Select(pat => new ContractTariffPattern_M
				{
					CustomerMainC = pattern.CustomerMainC,
					CustomerSubC = pattern.CustomerSubC,
					ApplyD = pattern.ApplyD,
					DepartureC = pat.DepartureC,
					DestinationC = pat.DestinationC,
					ContainerSizeI = pat.ContainerSizeI,
					UnitPrice = pat.UnitPrice,
					CommodityC = pat.CommodityC,
					CommodityN = pat.CommodityN,
					CalculateByTon = pat.CalculateByTon,
					DisplayLineNo = iloop++
				}))
				{
					_contractTariffPatternRepository.Add(patternM);
				}
			}

			SaveContactTariffPattern();
		}

		public void UpdatePattern(ContractTariffViewModel pattern)
		{
			if (pattern != null)
			{
				var removePatterns = _contractTariffPatternRepository.Query(p => p.CustomerMainC == pattern.CustomerMainC &&
																				 p.CustomerSubC == pattern.CustomerSubC &&
																				 p.ApplyD == pattern.ApplyD);
				if (removePatterns != null)
				{
					foreach (var remPattern in removePatterns)
					{
						_contractTariffPatternRepository.Delete(remPattern);
					}
				}

				var iloop = 1;
				var patterns = pattern.TarrifPatterns;

				foreach (var patternM in patterns.Select(pat => new ContractTariffPattern_M
				{
					CustomerMainC = pattern.CustomerMainC,
					CustomerSubC = pattern.CustomerSubC,
					ApplyD = pattern.ApplyD,
					DepartureC = pat.DepartureC,
					DestinationC = pat.DestinationC,
					ContainerSizeI = pat.ContainerSizeI,
					UnitPrice = pat.UnitPrice,
					CommodityC = pat.CommodityC,
					CommodityN = pat.CommodityN,
					CalculateByTon = pat.CalculateByTon,
					DisplayLineNo = iloop++
				}))
				{
					_contractTariffPatternRepository.Add(patternM);
				}
			}
			SaveContactTariffPattern();

		}

		public void DeletePattern(string customerMainC, string customerSubC, DateTime applyD)
		{
			var patterns = _contractTariffPatternRepository.Query(p => p.CustomerMainC == customerMainC &&
																	   p.CustomerSubC == customerSubC &&
																	   p.ApplyD == applyD);
			if (patterns != null)
			{
				foreach (var pattern in patterns)
				{
					_contractTariffPatternRepository.Delete(pattern);
				}
			}

			SaveContactTariffPattern();
		}

		public void DeleteDetailPattern(string customerMainC, string customerSubC, string departureC, string destinationC,
			string containerSizeI, DateTime applyD)
		{
			var patterns = _contractTariffPatternRepository.Query(p => p.CustomerMainC == customerMainC &&
																	   p.CustomerSubC == customerSubC &&
																	   p.DepartureC == departureC &&
																	   p.DestinationC == destinationC &&
																	   p.ContainerSizeI == containerSizeI &&
																	   p.ApplyD == applyD);
			if (patterns != null)
			{
				foreach (var pattern in patterns)
				{
					_contractTariffPatternRepository.Delete(pattern);
				}
			}

			SaveContactTariffPattern();
		}

		public ContractTariffViewModel GetContractTariffPattern(string customerMainC, string customerSubC, DateTime applyD)
		{
			var tarriffs = (from p in _contractTariffPatternRepository.GetAllQueryable()
							join dp in _locationRepository.GetAllQueryable() on p.DepartureC equals dp.LocationC
							join ds in _locationRepository.GetAllQueryable() on p.DestinationC equals ds.LocationC
							where p.CustomerMainC == customerMainC
								  && p.CustomerSubC == customerSubC && p.ApplyD == applyD
							select new TariffPatternViewModel
							{
								DepartureC = p.DepartureC,
								DepartureN = dp.LocationN,
								DestinationC = p.DestinationC,
								DestinationN = ds.LocationN,
								ContainerSizeI = p.ContainerSizeI,
								UnitPrice = p.UnitPrice,
								DisplayLineNo = p.DisplayLineNo,
								CommodityC = p.CommodityC,
								CommodityN = p.CommodityN,
								CalculateByTon = p.CalculateByTon,
							}).OrderBy("DisplayLineNo asc, ContainerSizeI asc").ToList();

			var model = new ContractTariffViewModel
			{
				CustomerMainC = customerMainC,
				CustomerSubC = customerSubC,
				CustomerN =
					_customerRepository.Query(i => i.CustomerMainC == customerMainC && i.CustomerSubC == customerSubC)
						.Select(i => i.CustomerN)
						.FirstOrDefault(),
				ApplyD = applyD,
				TarrifPatterns = tarriffs
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
			var data = (from p in _contractTariffPatternRepository.GetAllQueryable()
						select new ContractTariffViewModel
						{
							CustomerMainC = p.CustomerMainC,
							CustomerSubC = p.CustomerSubC,
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
							data.OrderBy("CustomerMainC descending")
								.Skip(recordsToSkip)
								.Take(halfCount)
								.Any(c => c.CustomerMainC == customerMainC && c.CustomerSubC == customerSubC && c.ApplyD == applyD))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("CustomerMainC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.CustomerMainC == customerMainC && entity.CustomerSubC == customerSubC && entity.ApplyD == applyD)
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

		public decimal GetUnitPriceFromContractTariff(DateTime orderD, string custMainC, string custSubC, string ordertypeI, string loca1, string loca2, string contsizeI, string calTon, string comC)
		{
			decimal uPrice = 0;
			var loc1 = _locationRepository.Query(l => l.LocationC == loca1).FirstOrDefault();
			loca1 = loc1 != null ? (!string.IsNullOrEmpty(loc1.AreaC) ? loc1.AreaC : loca1) : loca1;
			var loc2 = _locationRepository.Query(l => l.LocationC == loca2).FirstOrDefault();
			loca2 = loc2 != null ? (!string.IsNullOrEmpty(loc2.AreaC) ? loc2.AreaC : loca2) : loca2;
			
			var list =
				_contractTariffPatternRepository.Query(
					p => DateTime.Compare(orderD, p.ApplyD) >= 0 &&
						p.CustomerMainC == custMainC && p.CustomerSubC == custSubC &&
						p.ContainerSizeI == contsizeI && p.CalculateByTon == calTon &&
						p.CommodityC == comC && !string.IsNullOrEmpty(p.CommodityC) && p.DepartureC == loca1 && p.DestinationC == loca2).FirstOrDefault();
			if (list == null)
			{
				list =
				_contractTariffPatternRepository.Query(
					p => DateTime.Compare(orderD, p.ApplyD) >= 0 &&
						p.CustomerMainC == custMainC && p.CustomerSubC == custSubC &&
						p.ContainerSizeI == contsizeI && p.CalculateByTon == calTon &&
						p.DepartureC == loca1 && p.DestinationC == loca2 && string.IsNullOrEmpty(p.CommodityC)).FirstOrDefault();
			}
			if (list != null)
			{
				uPrice = (decimal)(list.UnitPrice ?? 0);
			}
			return uPrice;
		}
	}
}
