using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.FuelConsumption;

namespace Service.Services
{
	public interface IFuelConsumptionService
	{
		FuelConsumptionDatatable GetFuelConsumptionForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string searchValue);
		//FuelConsumptionViewModel GetFuelConsumption(string regno, string consize, string loadingC, string dischargeC);
		FuelConsumptionViewModel GetFuelConsumptionSizeByCode(string fuelconsumptionC);
		void CreateFuelConsumption(FuelConsumptionViewModel truckExpense);
		void UpdateFuelConsumption(FuelConsumptionViewModel fuelConsumption);
		void DeleteFuelConsumption(string id);
		List<FuelConsumptionViewModel> GetFuelConsumptionByCode(string value);
		int CheckExitContSize(string contSize);
		void SaveFuelConsumption();
		InfoFuel GetInfoFuel(string truckC, string containersize, string waytype);

	}
	public class InfoFuel
	{
		public decimal? LossFuelRate;
		public decimal? FuelConsumption;
		public decimal? ActualFuel;
	}

	public class FuelConsumptionService : IFuelConsumptionService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IFuelConsumptionRepository _fuelConsumptionRepository;
		private readonly IModelRepository _modelRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly ITruckRepository _truckRepository;

		public FuelConsumptionService(IFuelConsumptionRepository fuelConsumptionRepository, IModelRepository modelRepository,
										ILocationRepository locationRepository, ITruckRepository truckRepository, IUnitOfWork unitOfWork)
		{
			this._unitOfWork = unitOfWork;
			this._fuelConsumptionRepository = fuelConsumptionRepository;
			this._modelRepository = modelRepository;
			this._locationRepository = locationRepository;
			this._truckRepository = truckRepository;
		}
		public FuelConsumptionViewModel GetFuelConsumptionSizeByCode(string fuelconsumptionC)
		{
			var fuel = (from p in _fuelConsumptionRepository.GetAllQueryable()
						join m in _modelRepository.GetAllQueryable() on new { p.ModelC }
							   equals new { m.ModelC } into t
						from m in t.DefaultIfEmpty()
						where
							p.FuelConsumptionC == fuelconsumptionC

						select new FuelConsumptionViewModel
						{
							ModelC = m.ModelC,
							ModelN = m.ModelN,
							FuelConsumptionC = fuelconsumptionC,
							FuelConsumptionId = p.FuelConsumptionId,
							//ApplyD = p.ApplyD,
							ShortRoad = p.ShortRoad,
							LongRoad = p.LongRoad,
							Gradient = p.Gradient,
							Empty = p.Empty,
							ContainerSizeI = p.ContainerSizeI
						}).FirstOrDefault();
			return fuel;
		}

		public FuelConsumptionDatatable GetFuelConsumptionForTable(int page, int itemsPerPage, string sortBy, bool reverse, string searchValue)
		{
			var fuel = _fuelConsumptionRepository.GetAllQueryable();

			var destination = (from p in _fuelConsumptionRepository.GetAllQueryable()
							   join m in _modelRepository.GetAllQueryable() on new { p.ModelC }
							   equals new { m.ModelC } into t
							   from m in t.DefaultIfEmpty()
							   select new FuelConsumptionViewModel()
							   {
								   FuelConsumptionC = p.FuelConsumptionC,
								   FuelConsumptionId = p.FuelConsumptionId,
								   //ApplyD = (DateTime)p.ApplyD,
								   ShortRoad = p.ShortRoad,
								   LongRoad = p.LongRoad,
								   Gradient = p.Gradient,
								   Empty = p.Empty,
								   ContainerSizeI = p.ContainerSizeI,
								   ModelC = m.ModelC,
								   ModelN = m.ModelN
							   }
								   ).ToList();

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var allowancesOrdered = destination.OrderBy(sortBy + (reverse ? " descending" : ""));
			// paging
			var allowancesPaged = allowancesOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();
			var fuelDatatable = new FuelConsumptionDatatable()
			{
				Data = allowancesPaged,
				Total = fuel.Count()
			};
			return fuelDatatable;
		}
		public void CreateFuelConsumption(FuelConsumptionViewModel fuelConsumption)
		{
			//var model = Mapper.Map<DriverAllowanceViewModel, DriverAllowance_M>(pattern);
			FuelConsumption_M model = new FuelConsumption_M();
			model.FuelConsumptionC = fuelConsumption.FuelConsumptionC;
			model.FuelConsumptionId = fuelConsumption.FuelConsumptionId;
			//model.ApplyD = fuelConsumption.ApplyD;
			model.ModelC = fuelConsumption.ModelC;
			model.ShortRoad = fuelConsumption.ShortRoad ?? 0;
			model.LongRoad = fuelConsumption.LongRoad ?? 0;
			model.Gradient = fuelConsumption.Gradient ?? 0;
			model.ContainerSizeI = fuelConsumption.ContainerSizeI;
			model.Empty = fuelConsumption.Empty ?? 0;
			_fuelConsumptionRepository.Add(model);
			SaveFuelConsumption();
		}

		public void UpdateFuelConsumption(FuelConsumptionViewModel fuelConsumption)
		{
			var fuelconsumptionToRemove = _fuelConsumptionRepository.Query(d => d.FuelConsumptionId == fuelConsumption.FuelConsumptionId).FirstOrDefault();
			FuelConsumption_M model = new FuelConsumption_M();
			model.FuelConsumptionC = fuelConsumption.FuelConsumptionC;
			model.FuelConsumptionId = fuelConsumption.FuelConsumptionId;
			//model.ApplyD = fuelConsumption.ApplyD;
			model.ModelC = fuelConsumption.ModelC;
			model.ShortRoad = fuelConsumption.ShortRoad ?? 0;
			model.LongRoad = fuelConsumption.LongRoad ?? 0;
			model.Gradient = fuelConsumption.Gradient ?? 0;
			model.ContainerSizeI = fuelConsumption.ContainerSizeI;
			model.Empty = fuelConsumption.Empty ?? 0;
			_fuelConsumptionRepository.Add(model);
			_fuelConsumptionRepository.Delete(fuelconsumptionToRemove);
			SaveFuelConsumption();
		}

		public void DeleteFuelConsumption(string id)
		{
			var fuelConsumptionToRemove = _fuelConsumptionRepository.Get(c => c.FuelConsumptionC == id);
			if (fuelConsumptionToRemove != null)
			{
				_fuelConsumptionRepository.Delete(fuelConsumptionToRemove);
				SaveFuelConsumption();
			}
		}
		public List<FuelConsumptionViewModel> GetFuelConsumptionByCode(string value)
		{
			var driver = _fuelConsumptionRepository.Query(p => p.FuelConsumptionC.StartsWith(value));
			var result = (from p in driver
						  join m in _modelRepository.GetAllQueryable() on new { p.ModelC }
							   equals new { m.ModelC } into t
						  from m in t.DefaultIfEmpty()
						  select new FuelConsumptionViewModel
						  {
							  ModelC = m.ModelC,
							  ModelN = m.ModelN,
							  FuelConsumptionC = p.FuelConsumptionC,
							  FuelConsumptionId = p.FuelConsumptionId,
							  //ApplyD = p.ApplyD,
							  ShortRoad = p.ShortRoad,
							  LongRoad = p.LongRoad,
							  Gradient = p.Gradient,
							  Empty = p.Empty,
							  ContainerSizeI = p.ContainerSizeI
						  }).ToList();
			return result;
		}

		public int CheckExitContSize(string contSize)
		{
			var check = _fuelConsumptionRepository.Query(p => p.ContainerSizeI == contSize).FirstOrDefault();
			if (check != null)
			{
				return 1;
			}
			return 0;
		}

		public void SaveFuelConsumption()
		{
			_unitOfWork.Commit();
		}
		public InfoFuel GetInfoFuel(string truckC, string containersize, string waytype)
		{
			var obj = new InfoFuel();
			string model = null;
			var truck = _truckRepository.Query(p => p.TruckC == truckC).FirstOrDefault();
			if (truck != null)
			{
				model = truck.ModelC;
				var fuelconsumption = _fuelConsumptionRepository.Query(p => p.ContainerSizeI == containersize && p.ModelC == model).FirstOrDefault();
				if (fuelconsumption != null)
				{
					switch (waytype)
					{
						case "0":
						{
							obj = new InfoFuel()
							{
								LossFuelRate = truck.LossFuelRate ?? 0,
								FuelConsumption = fuelconsumption.Empty ?? 0,
								ActualFuel = fuelconsumption.ShortRoad ?? 0
							};
							break;
						}
						case "1":
						{
							obj = new InfoFuel()
							{
								LossFuelRate = truck.LossFuelRate ?? 0,
								FuelConsumption = fuelconsumption.Empty ?? 0,
								ActualFuel = fuelconsumption.LongRoad ?? 0
							};
							break;
						}
						case "2":
						{
							obj = new InfoFuel()
							{
								LossFuelRate = truck.LossFuelRate ?? 0,
								FuelConsumption = fuelconsumption.Empty ?? 0,
								ActualFuel = fuelconsumption.Gradient ?? 0
							};
							break;
						}
					}
				}
				else
				{
					obj = new InfoFuel()
					{
						LossFuelRate = truck.LossFuelRate ?? 0,
						FuelConsumption = 0,
						ActualFuel = 0
					};
				}
			}
			return obj;
		}
	}
}
