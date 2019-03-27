using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using CrystalReport.Dataset.Dispatch;
using CrystalReport.Dataset.DriverAllowance;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Website.ViewModels;
using Website.ViewModels.DriverAllowance;
using Root.Models;
using AutoMapper;
using Microsoft.SqlServer.Management.Smo;

namespace Service.Services
{
	public interface IDriverAllowanceService
	{
		List<DriverAllowanceViewModel> GetDriverAllowances(string customerMainC, string customerSubC);

		DriverAllowanceDatatable GetDriverAllowancesForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string searchValue);
		DriverAllowancePatternViewModel GetDriverAllowance(string customerMainC, string customerSubC, DateTime applyD);
		void SaveDriverAllowance();

		void CreatePattern(DriverAllowancePatternViewModel pattern);
		void UpdatePattern(DriverAllowancePatternViewModel pattern);
		void DeletePattern(string customerMainC, string customerSubC, DateTime applyD);
		decimal? GetUnitPriceRate(DateTime date);
		decimal? GetLatestDriverAllowance(DateTime date);
		int GetUnitPriceMethodI(DateTime date);
	}
	public class DriverAllowanceService : IDriverAllowanceService
	{
		private readonly IDriverAllowanceRepository _driverAllowanceRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly IContainerTypeRepository _containerTypeRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IDispatchRepository _dispatchRepository;
		private readonly IAllowanceDetailRepository _allowanceDetailRepository;
		private readonly IExpenseRepository _expenseRepository;
		private readonly IOrderRepository _orderHRepository;
		private readonly IContainerRepository _orderDRepository;
		private readonly IDepartmentRepository _departmentRepository;
		private readonly ITextResourceRepository _textResourceRepository;

		public DriverAllowanceService(IDriverAllowanceRepository driverAllowanceRepository,
									  IUnitOfWork unitOfWork,
									  IDispatchRepository dispatchRepository,
										ICustomerRepository customerRepository,
									ILocationRepository locationRepository,
									IContainerTypeRepository containerTypeRepository,
									  IAllowanceDetailRepository allowanceDetailRepository,
									  IExpenseRepository _expenseRepository,
									  IOrderRepository orderHRespository,
									  IContainerRepository orderDRepository,
									  IOrderRepository orderHRepository,
									  IDepartmentRepository departmentRepository,
									  ITextResourceRepository textResourceRepository)
		{
			this._driverAllowanceRepository = driverAllowanceRepository;
			this._customerRepository = customerRepository;
            this._locationRepository = locationRepository;
            this._containerTypeRepository = containerTypeRepository;
			this._unitOfWork = unitOfWork;
			this._dispatchRepository = dispatchRepository;
			this._allowanceDetailRepository = allowanceDetailRepository;
			this._expenseRepository = _expenseRepository;
			this._orderDRepository = orderDRepository;
			this._orderHRepository = orderHRepository;
			this._departmentRepository = departmentRepository;
			this._textResourceRepository = textResourceRepository;
		}

		public List<DriverAllowanceViewModel> GetDriverAllowances(string customerMainC, string customerSubC)
		{
			var driverAllowances =
				_driverAllowanceRepository.Query(p => //p.CustomerMainC == customerMainC &&
														//p.CustomerSubC == customerSubC &&
														p.ApplyD < DateTime.Now);

			var groupMaxDriverAllowances = from p in driverAllowances
									group p by new { p.CustomerMainC, p.CustomerSubC }
										into g
										select new
										{
											g.Key.CustomerMainC,
											g.Key.CustomerSubC,
											ApplyD = g.Max(p => p.ApplyD)
										};

			var result = from p in driverAllowances
							join q in groupMaxDriverAllowances
							on new { p.CustomerMainC, p.CustomerSubC, p.ApplyD }
							equals new { q.CustomerMainC, q.CustomerSubC, q.ApplyD }
							select new DriverAllowanceViewModel()
							{
								CustomerMainC = customerMainC,
								CustomerSubC = customerSubC,
								ApplyD = (DateTime)p.ApplyD,
								UnitPriceMethodI = p.UnitPriceMethodI,
								DepartureC = p.DepartureC,
								DestinationC = p.DestinationC,
								ContainerSizeI = p.ContainerSizeI,
								EmptyGoods = p.EmptyGoods,
								UnitPrice = p.UnitPrice,
								UnitPriceRate = p.UnitPriceRate
							};

			return result.ToList();
		}
		public void CreatePattern(DriverAllowancePatternViewModel pattern)
		{
			try
			{
				var unitpricemethod = Convert.ToInt64(pattern.UnitPriceMethodI);
				if(unitpricemethod == 0)
				{
					if (pattern != null)
					{
						var iloop = 1;
						var patterns = pattern.TarrifPatterns;
						foreach (var patternM in patterns.Select(pat => new DriverAllowance_M
						{
							CustomerMainC = pattern.CustomerMainC,
							CustomerSubC = pattern.CustomerSubC,
							ApplyD = pattern.ApplyD,
							DepartureC = pat.DepartureC,
							DestinationC = pat.DestinationC,
							ContainerSizeI = pat.ContainerSizeI,
							EmptyGoods = pat.EmptyGoods,
							UnitPrice = pat.UnitPrice,
							UnitPriceMethodI = pattern.UnitPriceMethodI,
							ContainerSize = "",
							Empty = 0,
							ShortRoad = 0,
							LongRoad = 0,
							GradientRoad = 0,
							DriverAllowanceId = iloop,
							DisplayLineNo = iloop++
						}))
						{
							_driverAllowanceRepository.Add(patternM);
						}
					}
				}
				else
				{
					if(unitpricemethod == 1)
					{
						DriverAllowance_M model = new DriverAllowance_M();
						model.CustomerMainC = pattern.CustomerMainC;
						model.CustomerSubC = pattern.CustomerSubC;
						model.UnitPriceMethodI = pattern.UnitPriceMethodI;
						model.ApplyD = pattern.ApplyD;
						model.DepartureC = "";
						model.DestinationC = "";
						model.ContainerSizeI = "";
						model.EmptyGoods = 0;
						model.UnitPrice = 0;
						model.UnitPriceRate = pattern.UnitPriceRate;
						model.DriverAllowanceId = 1;
						model.DisplayLineNo = 1;
						model.ContainerSize = "";
						model.Empty = 0;
						model.ShortRoad = 0;
						model.LongRoad = 0;
						model.GradientRoad = 0;
						_driverAllowanceRepository.Add(model);
					}
					else if (unitpricemethod == 2)
					{
						if (pattern != null)
						{
							var iloop = 1;
							var patterns = pattern.TarrifPatterns;
							for (int i =0;i< patterns.Count;i++)
							{
								DriverAllowance_M daM = new DriverAllowance_M();
								daM.CustomerMainC = pattern.CustomerMainC;
								daM.CustomerSubC = pattern.CustomerSubC;
								daM.ApplyD = pattern.ApplyD;
								daM.DepartureC = "";
								daM.DestinationC = "";
								daM.ContainerSizeI = "";
								daM.EmptyGoods = 0;
								daM.UnitPrice = 0;
								daM.UnitPriceMethodI = pattern.UnitPriceMethodI;
								daM.ContainerSize = patterns[i].ContainerSize;
								daM.Empty = patterns[i].Empty;
								daM.ShortRoad = patterns[i].ShortRoad;
								daM.LongRoad = patterns[i].LongRoad;
								daM.GradientRoad = patterns[i].GradientRoad;
								daM.UnitPriceRate = pattern.UnitPriceRate;
								daM.DriverAllowanceId = iloop++;
								daM.DisplayLineNo = iloop++;
								_driverAllowanceRepository.Add(daM);
							};
						}
					}
					else
					{
						if(unitpricemethod == 3)
						{
							DriverAllowance_M model = new DriverAllowance_M();
							model.CustomerMainC = pattern.CustomerMainC;
							model.CustomerSubC = pattern.CustomerSubC;
							model.UnitPriceMethodI = pattern.UnitPriceMethodI;
							model.ApplyD = pattern.ApplyD;
							model.DepartureC = "";
							model.DestinationC = "";
							model.ContainerSizeI = "";
							model.EmptyGoods = 0;
							model.UnitPrice = 0;
							_driverAllowanceRepository.Add(model);
						}
					}
				}
				SaveDriverAllowance();
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}

		public void UpdatePattern(DriverAllowancePatternViewModel pattern)
		{
			try
			{
				var unitpricemethod = Convert.ToInt64(pattern.UnitPriceMethodI);
				var removePatterns = _driverAllowanceRepository.Query(p => p.CustomerMainC == pattern.CustomerMainC &&
												p.CustomerSubC == pattern.CustomerSubC &&
												p.ApplyD == pattern.ApplyD);
				if (removePatterns != null)
				{
					foreach (var remPattern in removePatterns)
					{
						_driverAllowanceRepository.Delete(remPattern);
					}
				}
				if(unitpricemethod == 0)
				{
					if(pattern != null)
					{
						var iloop = 1;
						var patterns = pattern.TarrifPatterns;
						foreach (var patternM in patterns.Select(pat => new DriverAllowance_M
						{
							CustomerMainC = pattern.CustomerMainC,
							CustomerSubC = pattern.CustomerSubC,
							ApplyD = pattern.ApplyD,
							DepartureC = pat.DepartureC,
							DestinationC = pat.DestinationC,
							ContainerSizeI = pat.ContainerSizeI,
							EmptyGoods = pat.EmptyGoods,
							UnitPrice = pat.UnitPrice,
							UnitPriceMethodI = pattern.UnitPriceMethodI,
							ContainerSize = "",
							Empty = 0,
							ShortRoad = 0,
							LongRoad = 0,
							GradientRoad = 0,
							DriverAllowanceId = iloop,
							DisplayLineNo = iloop++
						}))
						{
							_driverAllowanceRepository.Add(patternM);
						} 
					}
				}
				else
				{
					if(unitpricemethod == 1 || unitpricemethod == 3)
					{
						DriverAllowance_M model = new DriverAllowance_M();
						model.CustomerMainC = pattern.CustomerMainC;
						model.CustomerSubC = pattern.CustomerSubC;
						model.UnitPriceMethodI = pattern.UnitPriceMethodI;
						model.ApplyD = pattern.ApplyD;
						model.DepartureC = "";
						model.DestinationC = "";
						model.ContainerSizeI = "";
						model.EmptyGoods = 0;
						model.UnitPrice = 0;
						model.UnitPriceRate = pattern.UnitPriceRate;
						model.DriverAllowanceId = 1;
						model.DisplayLineNo = 1;
						model.ContainerSize = "";
						model.Empty = 0;
						model.ShortRoad = 0;
						model.LongRoad = 0;
						model.GradientRoad = 0;
						_driverAllowanceRepository.Add(model);
						//_driverAllowanceRepository.Update(model);
					}
					else
					{
						if (unitpricemethod == 2)
						{
							if (pattern != null)
							{
								var iloop = 1;
								var patterns = pattern.TarrifPatterns;
								for (int i = 0; i < patterns.Count; i++)
								{
									DriverAllowance_M daM = new DriverAllowance_M();
									daM.CustomerMainC = pattern.CustomerMainC;
									daM.CustomerSubC = pattern.CustomerSubC;
									daM.ApplyD = pattern.ApplyD;
									daM.UnitPriceRate = pattern.UnitPriceRate;
									daM.DepartureC = "";
									daM.DestinationC = "";
									daM.ContainerSizeI = "";
									daM.EmptyGoods = 0;
									daM.UnitPrice = 0;
									daM.UnitPriceMethodI = pattern.UnitPriceMethodI;
									daM.ContainerSize = patterns[i].ContainerSize;
									daM.Empty = patterns[i].Empty;
									daM.ShortRoad = patterns[i].ShortRoad;
									daM.LongRoad = patterns[i].LongRoad;
									daM.GradientRoad = patterns[i].GradientRoad;
									daM.DriverAllowanceId = iloop++;
									daM.DisplayLineNo = iloop++;
									_driverAllowanceRepository.Add(daM);
								};
							}
						}
					}
				}
				SaveDriverAllowance();
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}
		public void DeletePattern(string customerMainC, string customerSubC, DateTime applyD)
		{
			try
			{
				var patterns = _driverAllowanceRepository.Query(p => p.CustomerMainC == customerMainC &&
															p.CustomerSubC == customerSubC &&
															p.ApplyD == applyD);
				if(patterns != null)
				{
					foreach (var pattern in patterns)
					{
						_driverAllowanceRepository.Delete(pattern);
					}
				}
				SaveDriverAllowance();
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}
		public void SaveDriverAllowance()
		{
			_unitOfWork.Commit();
		}
		public DriverAllowanceDatatable GetDriverAllowancesForTable(int page, int itemsPerPage, string sortBy, bool reverse, string searchValue)
		{
			try
			{
				var drivers = (from p in _driverAllowanceRepository.GetAllQueryable()
							   //join c in _customerRepository.GetAllQueryable() on p.CustomerMainC equals c.CustomerMainC
							   //  where p.CustomerSubC == c.CustomerSubC
								 select new DriverAllowancePatternViewModel
								 {
									 CustomerMainC = p.CustomerMainC,
									 CustomerSubC = p.CustomerSubC,
									 CustomerN = "",//c.CustomerN,
									 ApplyD = p.ApplyD,
									 UnitPriceMethodI = p.UnitPriceMethodI,
								 }).Distinct().AsQueryable();
				//searching
				//if (!string.IsNullOrWhiteSpace(searchValue))
				//{
				//	searchValue = searchValue.ToLower();
					//drivers = drivers.Where(i => (i.CustomerN != null && i.CustomerN.ToLower().Contains(searchValue)) ||
					//	(i.CustomerMainC != null && i.CustomerMainC.ToLower().Contains(searchValue)) ||
					//	(i.CustomerSubC != null && i.CustomerSubC.ToLower().Contains(searchValue)));
				//}
				
				 // sorting, paging          
				List<DriverAllowancePatternViewModel> result = drivers.OrderBy(sortBy + (reverse ? " descending" : "")).Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();
				
				var datatable = new DriverAllowanceDatatable()
				{
					Data = result,
					Total = drivers.Count()
				};
				return datatable;
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}
		public DriverAllowancePatternViewModel GetDriverAllowance(string customerMainC, string customerSubC, DateTime applyD)
		{
			var driver = (from p in _driverAllowanceRepository.GetAllQueryable()
							  //join cs in _customerRepository.GetAllQueryable() on new {p.CustomerMainC, p.CustomerSubC} equals new{cs.CustomerMainC, cs.CustomerSubC}
							  where //p.CustomerMainC == customerMainC
							  //&& p.CustomerSubC == customerSubC && 
							  p.ApplyD == applyD
								select new DriverAllowancePatternViewModel
								{
									CustomerMainC = customerMainC,
									CustomerSubC = customerSubC,
									CustomerN = "", //cs.CustomerN,
									UnitPriceRate = p.UnitPriceRate,
									ApplyD = applyD,
									UnitPriceMethodI = p.UnitPriceMethodI,
								}).FirstOrDefault();
			if (driver != null)
			{
				driver.DriverAllowanceIndex = FindIndex("0", "0", applyD);
				var unitpriceMethodI = Convert.ToInt64(driver.UnitPriceMethodI.ToString());
				if (unitpriceMethodI == 0)
				{
					var allowances = (from p in _driverAllowanceRepository.GetAllQueryable()
									  join dp in _locationRepository.GetAllQueryable() on p.DepartureC equals dp.LocationC
									  join ds in _locationRepository.GetAllQueryable() on p.DestinationC equals ds.LocationC
									  where //p.CustomerMainC == customerMainC
									  //&& p.CustomerSubC == customerSubC && 
									  p.ApplyD == applyD
									  select new AllowanceViewModel
									  {
										  DepartureC = p.DepartureC,
										  DepartureN = dp.LocationN,
										  DestinationC = p.DestinationC,
										  DestinationN = ds.LocationN,
										  EmptyGoods = p.EmptyGoods,
										  ContainerSizeI = p.ContainerSizeI,
										  UnitPrice = p.UnitPrice,
										  DisplayLineNo = p.DisplayLineNo,
										  ContainerSize = p.ContainerSize,
										  Empty = p.Empty,
										  ShortRoad = p.ShortRoad,
										  LongRoad = p.LongRoad,
										  GradientRoad = p.GradientRoad
									  }).OrderBy("DisplayLineNo asc, ContainerSizeI asc").ToList();
					driver.TarrifPatterns = allowances;
				}
				if (unitpriceMethodI == 2)
				{
					var allowances = (from p in _driverAllowanceRepository.GetAllQueryable()
									  where //p.CustomerMainC == customerMainC
										  //&& p.CustomerSubC == customerSubC && 
									  p.ApplyD == applyD
									  select new AllowanceViewModel
									  {
										  DepartureC = p.DepartureC,
										  DepartureN = "",
										  DestinationC = p.DestinationC,
										  DestinationN = "",
										  EmptyGoods = p.EmptyGoods,
										  ContainerSizeI = p.ContainerSizeI,
										  UnitPrice = p.UnitPrice,
										  DisplayLineNo = p.DisplayLineNo,
										  ContainerSize = p.ContainerSize,
										  Empty = p.Empty,
										  ShortRoad = p.ShortRoad,
										  LongRoad = p.LongRoad,
										  GradientRoad = p.GradientRoad
									  }).OrderBy("DisplayLineNo asc, ContainerSizeI asc").ToList();
					driver.TarrifPatterns = allowances;
				}
			}
			else
			{
				driver = null;
			}
			return driver;
		}

		private int FindIndex(string customerMainC, string customerSubC, DateTime applyD)
		{
			var data = (from p in _driverAllowanceRepository.GetAllQueryable()
						select new DriverAllowancePatternViewModel
						{
							CustomerMainC = customerMainC,
							CustomerSubC = customerSubC,
							ApplyD = p.ApplyD,
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

						if (data.OrderBy("ApplyD descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.CustomerMainC == customerMainC && c.CustomerSubC == customerSubC && c.ApplyD == applyD))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("ApplyD descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (//entity.CustomerMainC == customerMainC && entity.CustomerSubC == customerSubC && 
									entity.ApplyD == applyD)
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

		public decimal? GetUnitPriceRate(DateTime date)
		{
			decimal? unp = 0;
			var flagup = _driverAllowanceRepository.Query(p => DateTime.Compare(date, p.ApplyD) >= 0 && p.UnitPriceMethodI == "1").ToList();
			if(flagup.Count>0)
			{
				var maxflagup = flagup.Max(p => p.ApplyD);
				var selectmaxflagup = _driverAllowanceRepository.Query(p => p.ApplyD == maxflagup).FirstOrDefault();
				if (selectmaxflagup != null)
				{
					unp = selectmaxflagup.UnitPriceRate ?? 0;
				}
			}
			return unp;
		}

		public decimal? GetLatestDriverAllowance(DateTime date)
		{
			decimal? unp = 0;
			var flagup = _driverAllowanceRepository.Query(p => DateTime.Compare(date, p.ApplyD) >= 0).ToList();
			if (flagup.Count > 0)
			{
				var maxflagup = flagup.Max(p => p.ApplyD);
				var selectmaxflagup = _driverAllowanceRepository.Query(p => p.ApplyD == maxflagup).FirstOrDefault();
				if (selectmaxflagup != null && selectmaxflagup.UnitPriceMethodI == "1")
				{
					unp = selectmaxflagup.UnitPriceRate ?? 0;
				}
			}
			return unp;
		}

		public int GetUnitPriceMethodI(DateTime date)
		{
			var flagup = _driverAllowanceRepository.Query(p => DateTime.Compare(date, p.ApplyD) >= 0).ToList();
			if (flagup.Count > 0)
			{
				var maxflagup = flagup.Max(p => p.ApplyD);
				var selectmaxflagup = _driverAllowanceRepository.Query(p => p.ApplyD == maxflagup).FirstOrDefault();
				return (selectmaxflagup != null ? int.Parse(selectmaxflagup.UnitPriceMethodI) : -1);
			}
			return -1;
		}
	}
}
