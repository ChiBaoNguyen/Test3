using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.TransportDistance;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Container;
using Website.ViewModels.FuelConsumption;
using Website.ViewModels.DriverAllowance;
using Website.ViewModels.Order;

namespace Service.Services
{
	public interface ITransportDistanceService
	{
		TransportDistanceDatatable GetTransportDistanceForTable(int page, int itemsPerPage, string sortBy, bool reverse, string departure, string destination);
		//FuelConsumptionViewModel GetFuelConsumption(string regno, string consize, string loadingC, string dischargeC);
		TransportDistanceViewModel GetTransportDistanceSizeByCode(string tansportdistanceC);
		void CreateTransportDistance(TransportDistanceViewModel transport);
		void UpdateTransportDistance(TransportDistanceViewModel transport);
		void DeleteTransportDistance(string id);
		List<TransportDistanceViewModel> GetTransportDistanceByCode(string value);
		void SaveTransportDistance();

		Result GetInfoFromTransportDistance(DateTime orderD, string orderNo, int detailNo,
			string location1C, string location2C, string operation1C, string operation2C, DateTime transportD, string waytype);
	}
	public class Result
	{
		public decimal? Km;
		public decimal? EmptyCommodity;
		public decimal? UnitPrice;
		public string WayType;
		public string IsDistance;
	}
	public class TransportDistanceService : ITransportDistanceService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ITransportDistanceRepository _transportDistanceRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly IContainerRepository _orderDRepository;
		private readonly IFuelConsumptionRepository _fuelConsumptionRepository;
		private readonly IDriverAllowanceRepository _driverAllowanceRepository;
		private readonly IOrderRepository _orderRepository;

		public TransportDistanceService(IOrderRepository orderRepository, IDriverAllowanceRepository driverAllowanceRepository, IFuelConsumptionRepository fuelConsumptionRepository, IContainerRepository orderDRepository, ITransportDistanceRepository transportDistanceRepository, ILocationRepository locationRepository, IUnitOfWork unitOfWork)
		{
			this._unitOfWork = unitOfWork;
			this._transportDistanceRepository = transportDistanceRepository;
			this._locationRepository = locationRepository;
			this._orderDRepository = orderDRepository;
			this._fuelConsumptionRepository = fuelConsumptionRepository;
			this._driverAllowanceRepository = driverAllowanceRepository;
			this._orderRepository = orderRepository;
		}
		public TransportDistanceViewModel GetTransportDistanceSizeByCode(string tansportdistanceC)
		{
			var fuel = (from p in _transportDistanceRepository.GetAllQueryable()
						 join l1 in _locationRepository.GetAllQueryable() on p.FromAreaC equals l1.LocationC into pl1
                         from l1 in pl1.DefaultIfEmpty()
						 join l2 in _locationRepository.GetAllQueryable() on p.ToAreaC equals l2.LocationC into pl2
                         from l2 in pl2.DefaultIfEmpty()
						where
							p.TransportDistanceC == tansportdistanceC

						select new TransportDistanceViewModel()
						{
							TransportDistanceC = tansportdistanceC,
							ToAreaC = p.ToAreaC,
							FromAreaC = p.FromAreaC,
							Km = p.Km,
							WayType = p.WayType,
							ToAreaN = l2.LocationN,
							FromAreaN = l1.LocationN
						}).FirstOrDefault();
			return fuel;
		}

		public TransportDistanceDatatable GetTransportDistanceForTable(int page, int itemsPerPage, string sortBy, bool reverse, string departure, string destination)
		{
			var fuel = _transportDistanceRepository.GetAllQueryable();

			var data = (from p in _transportDistanceRepository.GetAllQueryable()
							   join l1 in _locationRepository.GetAllQueryable() on p.FromAreaC equals l1.LocationC into pl1
							   from l1 in pl1.DefaultIfEmpty()
							   join l2 in _locationRepository.GetAllQueryable() on p.ToAreaC equals l2.LocationC into pl2
							   from l2 in pl2.DefaultIfEmpty()
							   select new TransportDistanceViewModel()
							   {
								   TransportDistanceC = p.TransportDistanceC,
								   ToAreaC = p.ToAreaC,
								   FromAreaC = p.FromAreaC,
								   Km = p.Km,
								   WayType = p.WayType,
								   ToAreaN = l2.LocationN,
								   FromAreaN = l1.LocationN
							   }
								   ).Distinct().AsQueryable();
			if (!string.IsNullOrWhiteSpace(departure))
			{
				data = data.Where(i => (i.FromAreaC != null && i.FromAreaC.Contains(departure)));
			}

			if (!string.IsNullOrWhiteSpace(destination))
			{
				data = data.Where(i => (i.ToAreaC != null && i.ToAreaC.Contains(destination)));
			}
			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var allowancesOrdered = data.OrderBy(sortBy + (reverse ? " descending" : ""));
			// paging
			var allowancesPaged = allowancesOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();
			var fuelDatatable = new TransportDistanceDatatable()
			{
				Data = allowancesPaged,
				Total = fuel.Count()
			};
			return fuelDatatable;
		}
		public void CreateTransportDistance(TransportDistanceViewModel transport)
		{
			//var model = Mapper.Map<DriverAllowanceViewModel, DriverAllowance_M>(pattern);
			TransportDistance_M model = new TransportDistance_M();
			model.TransportDistanceC = transport.TransportDistanceC;
			model.ToAreaC = transport.ToAreaC;
			model.FromAreaC = transport.FromAreaC;
			model.WayType = transport.WayType;
			model.Km = transport.Km ?? 0;
			_transportDistanceRepository.Add(model);
			SaveTransportDistance();
		}

		public void UpdateTransportDistance(TransportDistanceViewModel transport)
		{
			var transportdistanceToRemove = _transportDistanceRepository.Query(d => d.TransportDistanceC == transport.TransportDistanceC).FirstOrDefault();
			TransportDistance_M model = new TransportDistance_M();
			model.TransportDistanceC = transport.TransportDistanceC;
			model.ToAreaC = transport.ToAreaC;
			model.FromAreaC = transport.FromAreaC;
			model.WayType = transport.WayType;
			model.Km = transport.Km ?? 0;
			_transportDistanceRepository.Add(model);
			_transportDistanceRepository.Delete(transportdistanceToRemove);
			SaveTransportDistance();
		}

		public void DeleteTransportDistance(string id)
		{
			var transportdistanceToRemove = _transportDistanceRepository.Get(c => c.TransportDistanceC == id);
			if (transportdistanceToRemove != null)
			{
				_transportDistanceRepository.Delete(transportdistanceToRemove);
				SaveTransportDistance();
			}
		}
		public List<TransportDistanceViewModel> GetTransportDistanceByCode(string value)
		{
			var driver = _transportDistanceRepository.Query(p => p.TransportDistanceC.StartsWith(value));
			var result = (from p in driver
						  join l1 in _locationRepository.GetAllQueryable() on p.FromAreaC equals l1.LocationC into pl1
						  from l1 in pl1.DefaultIfEmpty()
						  join l2 in _locationRepository.GetAllQueryable() on p.ToAreaC equals l2.LocationC into pl2
						  from l2 in pl2.DefaultIfEmpty()
						  select new TransportDistanceViewModel
						  {
							  TransportDistanceC = p.TransportDistanceC,
							  ToAreaC = p.ToAreaC,
							  FromAreaC = p.FromAreaC,
							  Km = p.Km,
							  WayType = p.WayType,
							  ToAreaN = l2.LocationN,
							  FromAreaN = l1.LocationN
						  }).ToList();
			return result;
		}

		public void SaveTransportDistance()
		{
			_unitOfWork.Commit();
		}

		public Result GetInfoFromTransportDistance(DateTime orderD, string orderNo, int detailNo,
			string location1C, string location2C, string operation1C, string operation2C, DateTime transportD, string waytype)
		{
			var loc1 = _locationRepository.Query(l => l.LocationC == location1C).FirstOrDefault();
			location1C = loc1 != null ? (!string.IsNullOrEmpty(loc1.AreaC) ? loc1.AreaC : location1C) : location1C;
			var loc2 = _locationRepository.Query(l => l.LocationC == location2C).FirstOrDefault();
			location2C = loc2 != null ? (!string.IsNullOrEmpty(loc2.AreaC) ? loc2.AreaC : location2C) : location2C;
			if (waytype == "4")
			{
				var obj = new Result();
				var checkUnitPriceMethod =
					_driverAllowanceRepository.Query(p => DateTime.Compare(transportD, p.ApplyD) >= 0).ToList();
				var maxApplyD = checkUnitPriceMethod.Max(p => p.ApplyD);
				var selectResult = _driverAllowanceRepository.Query(p => p.ApplyD == maxApplyD).FirstOrDefault();
				if (selectResult != null)
				{
					var oD =
						_orderDRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo)
							.FirstOrDefault();
					var transport =
						_transportDistanceRepository.Query(p => p.FromAreaC == location1C && p.ToAreaC == location2C).FirstOrDefault();
					if (selectResult.UnitPriceMethodI == "2")
					{
						if (transport != null)
						{
							if (oD != null)
							{
								var flagup =
									_driverAllowanceRepository.Query(p => DateTime.Compare(transportD, p.ApplyD) >= 0 && p.UnitPriceMethodI == "2")
										.ToList();
								var maxflagup = flagup.Max(p => p.ApplyD);
								var selectmaxflagup =
									_driverAllowanceRepository.Query(p => p.ApplyD == maxflagup && p.ContainerSize == oD.ContainerSizeI)
										.FirstOrDefault();
								if (selectmaxflagup != null)
								{
									switch (transport.WayType)
									{
										case "0":
										{
											obj = new Result()
											{
												Km = transport.Km ?? 0,
												EmptyCommodity = selectmaxflagup.Empty ?? 0,
												UnitPrice = selectmaxflagup.ShortRoad ?? 0,
												WayType = transport.WayType,
												IsDistance = "0"
											};
											break;
										}
										case "1":
										{
											obj = new Result()
											{
												Km = transport.Km ?? 0,
												EmptyCommodity = selectmaxflagup.Empty ?? 0,
												UnitPrice = selectmaxflagup.LongRoad ?? 0,
												WayType = transport.WayType,
												IsDistance = "0"
											};
											break;
										}
										case "2":
										{
											obj = new Result()
											{
												Km = transport.Km ?? 0,
												EmptyCommodity = selectmaxflagup.Empty ?? 0,
												UnitPrice = selectmaxflagup.GradientRoad ?? 0,
												WayType = transport.WayType,
												IsDistance = "0"
											};
											break;
										}
									}
								}
								else
								{
									obj = new Result()
									{
										Km = transport.Km ?? 0,
										EmptyCommodity = 0,
										UnitPrice = 0,
										WayType = transport.WayType,
										IsDistance = "0"
									};
								}
							}
						}
					}
					else if (selectResult.UnitPriceMethodI == "0")
					{
						var oH =
							_orderRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo).FirstOrDefault();

						if (oH != null && oD != null)
						{
							decimal? empty = 0;
							decimal? commodity = 0;

							selectResult =
								_driverAllowanceRepository.Query(
									p =>
										p.ApplyD == maxApplyD && p.DepartureC == location1C && p.DestinationC == location2C &&
										p.ContainerSizeI == oD.ContainerSizeI).FirstOrDefault();
							if (selectResult != null)
							{
								empty = (operation1C == "LR" || operation1C == "TR" || operation2C == "LR" || operation2C == "TR")
									? selectResult.EmptyGoods
									: 0;
								commodity = (operation1C != "LR" && operation1C != "TR" && operation2C != "LR" && operation2C != "TR")
									? selectResult.UnitPrice
									: 0;
								obj = new Result()
								{
									Km = transport != null ? (transport.Km ?? 0) : 0,
									EmptyCommodity = empty ?? 0,
									UnitPrice = commodity ?? 0,
									WayType = transport != null ? (transport.WayType ?? waytype) : waytype,
									IsDistance = "1"
								};
							}
						}
					}
					if (obj.Km == null && obj.EmptyCommodity == null && obj.UnitPrice == null && obj.WayType == null)
					{
						obj = new Result()
						{
							Km = transport != null ? (transport.Km ?? 0) : 0,
							EmptyCommodity = 0,
							UnitPrice = 0,
							WayType = transport != null ? (transport.WayType ?? waytype) : waytype,
							IsDistance = "0"
						};
					}
				}
				return obj;
			}
			else
			{
				var obj = new Result();
				var checkUnitPriceMethod =
					_driverAllowanceRepository.Query(p => DateTime.Compare(transportD, p.ApplyD) >= 0).ToList();
				var maxApplyD = checkUnitPriceMethod.Max(p => p.ApplyD);
				var selectResult = _driverAllowanceRepository.Query(p => p.ApplyD == maxApplyD).FirstOrDefault();
				if (selectResult != null)
				{
					var oD =
						_orderDRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo)
							.FirstOrDefault();
					var transport =
						_transportDistanceRepository.Query(p => p.FromAreaC == location1C && p.ToAreaC == location2C).FirstOrDefault();
					if (selectResult.UnitPriceMethodI == "2")
					{
						if (transport != null)
						{
							if (oD != null)
							{
								var flagup =
									_driverAllowanceRepository.Query(p => DateTime.Compare(transportD, p.ApplyD) >= 0 && p.UnitPriceMethodI == "2")
										.ToList();
								var maxflagup = flagup.Max(p => p.ApplyD);
								var selectmaxflagup =
									_driverAllowanceRepository.Query(p => p.ApplyD == maxflagup && p.ContainerSize == oD.ContainerSizeI)
										.FirstOrDefault();
								if (selectmaxflagup != null)
								{
									switch (waytype)
									{
										case "0":
										{
											obj = new Result()
											{
												Km = transport.Km ?? 0,
												EmptyCommodity = selectmaxflagup.Empty ?? 0,
												UnitPrice = selectmaxflagup.ShortRoad ?? 0,
												WayType = waytype,
												IsDistance = "0"
											};
											break;
										}
										case "1":
										{
											obj = new Result()
											{
												Km = transport.Km ?? 0,
												EmptyCommodity = selectmaxflagup.Empty ?? 0,
												UnitPrice = selectmaxflagup.LongRoad ?? 0,
												WayType = waytype,
												IsDistance = "0"
											};
											break;
										}
										case "2":
										{
											obj = new Result()
											{
												Km = transport.Km ?? 0,
												EmptyCommodity = selectmaxflagup.Empty ?? 0,
												UnitPrice = selectmaxflagup.GradientRoad ?? 0,
												WayType = waytype,
												IsDistance = "0"
											};
											break;
										}
									}
								}
								else
								{
									obj = new Result()
									{
										Km = transport.Km ?? 0,
										EmptyCommodity = 0,
										UnitPrice = 0,
										WayType = waytype,
										IsDistance = "0"
									};
								}

							}
						}
					}
					else if (selectResult.UnitPriceMethodI == "0")
					{
						var oH =
							_orderRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo).FirstOrDefault();

						if (oH != null && oD != null)
						{
							decimal? empty = 0;
							decimal? commodity = 0;

							selectResult =
								_driverAllowanceRepository.Query(
									p =>
										p.ApplyD == maxApplyD && p.DepartureC == location1C && p.DestinationC == location2C &&
										p.ContainerSizeI == oD.ContainerSizeI).FirstOrDefault();
							if (selectResult != null)
							{
								empty = (operation1C == "LR" || operation1C == "TR" || operation2C == "LR" || operation2C == "TR")
									? selectResult.EmptyGoods
									: 0;
								commodity = (operation1C != "LR" && operation1C != "TR" && operation2C != "LR" && operation2C != "TR")
									? selectResult.UnitPrice
									: 0;
								obj = new Result()
								{
									Km = transport != null ? (transport.Km ?? 0) : 0,
									EmptyCommodity = empty ?? 0,
									UnitPrice = commodity ?? 0,
									WayType = waytype,
									IsDistance = "1"
								};
							}
						}
					}
					if (obj.Km == null && obj.EmptyCommodity == null && obj.UnitPrice == null && obj.WayType == null)
					{
						selectResult =
							_driverAllowanceRepository.Query(p => p.ApplyD == maxApplyD && oD.ContainerSizeI == p.ContainerSize)
								.FirstOrDefault();
						if (selectResult != null)
						{
							switch (waytype)
							{
								case "0":
								{
									obj = new Result()
									{
										Km = transport != null ? (transport.Km ?? 0) : 0,
										EmptyCommodity = selectResult.Empty ?? 0,
										UnitPrice = selectResult.ShortRoad ?? 0,
										WayType = waytype,
										IsDistance = "0"
									};
									break;
								}
								case "1":
								{
									obj = new Result()
									{
										Km = transport != null ? (transport.Km ?? 0) : 0,
										EmptyCommodity = selectResult.Empty ?? 0,
										UnitPrice = selectResult.LongRoad ?? 0,
										WayType = waytype,
										IsDistance = "0"
									};
									break;
								}
								case "2":
								{
									obj = new Result()
									{
										Km = transport != null ? (transport.Km ?? 0) : 0,
										EmptyCommodity = selectResult.Empty ?? 0,
										UnitPrice = selectResult.GradientRoad ?? 0,
										WayType = waytype,
										IsDistance = "0"
									};
									break;
								}
							}
						}
						else
						{
							obj = new Result()
							{
								Km = transport != null ? (transport.Km ?? 0) : 0,
								EmptyCommodity = 0,
								UnitPrice = 0,
								WayType = waytype,
								IsDistance = "0"
							};
						}
					}
				}
				return obj;
			}
		}
	}
}
