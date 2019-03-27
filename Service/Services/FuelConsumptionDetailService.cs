using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.Container;
using Website.ViewModels.Dispatch;
using Website.ViewModels.FuelConsumption;
using Website.ViewModels.Order;

namespace Service.Services
{
	public interface IFuelConsumptionDetailService
	{
		FuelConsumptionDetailDatatable GetFuelConsumptionDetail(FuelConsumptionDetailSearchParams searchParams);
		FuelConsumptionViewModel GetFuelConsumptionPattern(FuelConsumptionPatternParams searchParams);
		void UpdateFuelConsumptionDetail(FuelConsumptionDetailParams fuelConsumptionDetailParams);
		void SaveFuelConsumptionDetail();
	}
	public class FuelConsumptionDetailService : IFuelConsumptionDetailService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IFuelConsumptionRepository _fuelConsumptionRepository;
		private readonly IFuelConsumptionDetailRepository _fuelConsumptionDetailRepository;
		private readonly IModelRepository _modelRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly IDispatchRepository _dispatchRepository;
		private readonly IOrderRepository _orderHRepository;
		private readonly IContainerRepository _orderDRepository;
		private readonly IDriverRepository _driverRepository;
		private readonly ITruckRepository _truckRepository;
		private readonly IContainerTypeRepository _containerTypeRepository;


		public FuelConsumptionDetailService(IFuelConsumptionRepository fuelConsumptionRepository,
										IFuelConsumptionDetailRepository fuelConsumptionDetailRepository,
										IModelRepository modelRepository,
										ILocationRepository locationRepository,
										IDispatchRepository dispatchRepository,
										IDriverRepository driverRepository,
										ITruckRepository truckRepository,
										IContainerRepository orderDRepository,
										IOrderRepository orderHRepository,
										IContainerTypeRepository containerTypeRepository,
										IUnitOfWork unitOfWork)
		{
			this._unitOfWork = unitOfWork;
			this._dispatchRepository = dispatchRepository;
			this._orderHRepository = orderHRepository;
			this._orderDRepository = orderDRepository;
			this._driverRepository = driverRepository;
			this._truckRepository = truckRepository;
			this._fuelConsumptionRepository = fuelConsumptionRepository;
			this._fuelConsumptionDetailRepository = fuelConsumptionDetailRepository;
			this._modelRepository = modelRepository;
			this._locationRepository = locationRepository;
			this._containerTypeRepository = containerTypeRepository;
		}

		public FuelConsumptionDetailDatatable GetFuelConsumptionDetail(FuelConsumptionDetailSearchParams searchParams)
		{
			var searchInfo = searchParams.SearchInfo;
			var newTruckCList =  "," + searchInfo.TruckCList + ",";
			var newDriverCList = "," + searchInfo.DriverCList + ",";

			var dispatches = from a in _dispatchRepository.GetAllQueryable()
							 join b in _orderHRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
								 equals new { b.OrderD, b.OrderNo } into ab
							 from b in ab.DefaultIfEmpty()
							 where ((searchInfo.TransportDFrom == null || a.TransportD >= searchInfo.TransportDFrom) &&
								   (searchInfo.TransportDTo == null || a.TransportD <= searchInfo.TransportDTo)) &
								   (string.IsNullOrEmpty(searchInfo.TruckCList) || newTruckCList.Contains("," + a.TruckC + ",")) &
								   (string.IsNullOrEmpty(searchInfo.DriverCList) || newDriverCList.Contains("," + a.DriverC + ",")) &
								   (string.IsNullOrEmpty(searchInfo.DepC) || searchInfo.DepC == "0" || b.OrderDepC == searchInfo.DepC) &
								   ((searchInfo.DispatchStatus && a.DispatchStatus == Constants.DISPATCH) ||
								   (searchInfo.TransportedStatus && (a.DispatchStatus == Constants.TRANSPORTED || a.DispatchStatus == Constants.CONFIRMED))) &
								   (a.DispatchI == "0")
							 select new
							 {
								 a.OrderD,
								 a.OrderNo,
								 a.DispatchNo,
								 a.DetailNo,
								 a.TransportD,
								 a.DriverC,
								 a.TruckC,
								 a.ContainerStatus,
								 a.Location1C,
								 a.Location1N,
								 a.Location2C,
								 a.Location2N,
								 a.Location3C,
								 a.Location3N
							 };

			var dispatchOrdered = dispatches.OrderBy("TransportD desc");

			var dispatchPaged = dispatchOrdered.Skip((searchParams.page - 1) * searchParams.itemsPerPage).Take(searchParams.itemsPerPage).ToList();

			var fuelConsumptionDetail = from a in dispatchPaged
										join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo }
											equals new { b.OrderD, b.OrderNo, b.DetailNo } into ab
										from b in ab.DefaultIfEmpty()
										join c in _driverRepository.GetAllQueryable() on a.DriverC equals c.DriverC into abc
										from c in abc.DefaultIfEmpty()
										join d in _containerTypeRepository.GetAllQueryable() on b.ContainerTypeC equals d.ContainerTypeC into abcd
										from d in abcd.DefaultIfEmpty()
										join e in _truckRepository.GetAllQueryable() on a.TruckC equals e.TruckC into abcde
										from e in abcde.DefaultIfEmpty()
										join f in _fuelConsumptionDetailRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo, a.DispatchNo }
											equals new { f.OrderD, f.OrderNo, f.DetailNo, f.DispatchNo } into abcdef
										from f in abcdef.DefaultIfEmpty()
										select new FuelConsumptionDetailViewModel()
										{
											OrderD = a.OrderD,
											OrderNo = a.OrderNo,
											DispatchNo = a.DispatchNo,
											DetailNo = a.DetailNo,
											TransportD = a.TransportD,
											ContainerStatus = a.ContainerStatus,
											DriverN = c != null ? c.LastN + " " + c.FirstN : "",
											TruckC = a.TruckC,
											RegisteredNo = e != null ? e.RegisteredNo : "",
											ContainerSizeI = b.ContainerSizeI,
											ContainerTypeN = d != null ? d.ContainerTypeN : "",
											GrossWeight = b != null ? b.GrossWeight : 0,
											Location1C = a.Location1C,
											Location1N = a.Location1N,
											Location2C = a.Location2C,
											Location2N = a.Location2N,
											Location3C = a.Location3C,
											Location3N = a.Location3N,
											ApproximateDistance = f != null ? f.Distance : 0,
											IsEmpty = f != null ? f.IsEmpty : "",
											IsHeavy = f != null ? f.IsHeavy : "",
											IsSingle = f != null ? f.IsSingle : "",
											FuelConsumption = f != null ? f.FuelConsumption : 0,
											UnitPrice = f != null ? f.UnitPrice : 0,
											Amount = f != null ? f.Amount : 0,
										};
			return new FuelConsumptionDetailDatatable()
			{
				Data = fuelConsumptionDetail.ToList(),
				Total = dispatches.Count()
			};
		}

		public FuelConsumptionViewModel GetFuelConsumptionPattern(FuelConsumptionPatternParams patternParams)
		{
			var truckModel = _truckRepository.Query(p => p.TruckC == patternParams.TruckC).FirstOrDefault();
			var modelC = "";
			if (truckModel != null)
			{
				modelC = truckModel.ModelC;
			}

			var loadingPlaceC = "";
			var dischargePlace = "";

			if (!string.IsNullOrEmpty(patternParams.Location1C))
			{
				loadingPlaceC = patternParams.Location1C;
				if (!string.IsNullOrEmpty(patternParams.Location3C))
				{
					dischargePlace = patternParams.Location3C;
				}
				else if (!string.IsNullOrEmpty(patternParams.Location2C))
				{
					dischargePlace = patternParams.Location2C;
				}
			}
			else if (!string.IsNullOrEmpty(patternParams.Location2C))
			{
				loadingPlaceC = patternParams.Location2C;
				if (!string.IsNullOrEmpty(patternParams.Location3C))
				{
					dischargePlace = patternParams.Location3C;
				}
			}

			//var fuelComsumption = _fuelConsumptionRepository.Query(p => p.ModelC == modelC && p.ContainerSizeI == patternParams.ContainerSizeI &&
			//														//p.IsEmpty == patternParams.IsEmpty && p.IsHeavy == patternParams.IsHeavy &&
			//														//p.IsSingle == patternParams.IsSingle && 
			//														p.Distance == patternParams.ApproximateDistance &&
			//														p.LoadingPlaceC == loadingPlaceC && p.DischargePlaceC == dischargePlace).FirstOrDefault();
			var fuelComsumption = _fuelConsumptionRepository.Query(p => p.ContainerSizeI == patternParams.ContainerSizeI).FirstOrDefault();

			if (fuelComsumption != null)
			{
				var fConsumption = Mapper.Map<FuelConsumption_M, FuelConsumptionViewModel>(fuelComsumption);
				return fConsumption;
			}
			return null;
		}

		public void UpdateFuelConsumptionDetail(FuelConsumptionDetailParams fuelConsumptionDetailParams)
		{
			//check exit fuel consumption detail
			var existFuelConsumptionD = _fuelConsumptionDetailRepository.Query(p => p.OrderD == fuelConsumptionDetailParams.OrderD &
																					p.OrderNo == fuelConsumptionDetailParams.OrderNo &
																					p.DetailNo == fuelConsumptionDetailParams.DetailNo &
																					p.DispatchNo ==
																					fuelConsumptionDetailParams.DispatchNo).FirstOrDefault();
			if (existFuelConsumptionD != null)
			{
				if (string.IsNullOrEmpty(fuelConsumptionDetailParams.IsEmpty) &
				    string.IsNullOrEmpty(fuelConsumptionDetailParams.IsHeavy) &
				    string.IsNullOrEmpty(fuelConsumptionDetailParams.IsSingle) & fuelConsumptionDetailParams.FuelConsumption == 0 &
				    fuelConsumptionDetailParams.UnitPrice == 0 & fuelConsumptionDetailParams.Amount == 0)
				{
					_fuelConsumptionDetailRepository.Delete(existFuelConsumptionD);
					SaveFuelConsumptionDetail();
				}
				else
				{
					existFuelConsumptionD.IsEmpty = fuelConsumptionDetailParams.IsEmpty;
					existFuelConsumptionD.IsHeavy = fuelConsumptionDetailParams.IsHeavy;
					existFuelConsumptionD.IsSingle = fuelConsumptionDetailParams.IsSingle;
					existFuelConsumptionD.Distance = fuelConsumptionDetailParams.ApproximateDistance;
					existFuelConsumptionD.FuelConsumption = fuelConsumptionDetailParams.FuelConsumption;
					existFuelConsumptionD.UnitPrice = fuelConsumptionDetailParams.UnitPrice;
					existFuelConsumptionD.Amount = fuelConsumptionDetailParams.Amount;
					SaveFuelConsumptionDetail();
				}
			}
			else
			{
				var fuelConsumptionMapped = Mapper.Map<FuelConsumptionDetailParams, FuelConsumption_D>(fuelConsumptionDetailParams);
				fuelConsumptionMapped.Distance = fuelConsumptionDetailParams.ApproximateDistance;
				_fuelConsumptionDetailRepository.Add(fuelConsumptionMapped);
				SaveFuelConsumptionDetail();
			}
		}


		public void SaveFuelConsumptionDetail()
		{
			_unitOfWork.Commit();
		}
	}
}
