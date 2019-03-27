using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Website.ViewModels.Gps;
using Website.Enum;
using Website.ViewModels.Dispatch;
using Website.Utilities;
namespace Service.Services
{
    public interface IGpsService
    {
        List<GpsViewModel> GetGpsLocationList();
        bool MUpdateLocation(string driverC, string latitude, string longitude);
    }
    public class GpsService : IGpsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGpsLocationRepository _gpsLocationRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IDispatchRepository _dispatchRepository;
        private readonly ITruckRepository _truckRepository;
        private readonly IModelRepository _modelRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public GpsService(IUnitOfWork unitOfWork, IGpsLocationRepository gpsLocationRepository, IDriverRepository driverRepository, IDispatchRepository dispatchRepository, ITruckRepository truckRepository, IModelRepository modelRepository, IDepartmentRepository departmentRepository)
        {
            this._unitOfWork = unitOfWork;
            this._gpsLocationRepository = gpsLocationRepository;
            this._driverRepository = driverRepository;
            _dispatchRepository = dispatchRepository;
            _truckRepository = truckRepository;
            _modelRepository = modelRepository;
            _departmentRepository = departmentRepository;
        }

	    public double ConvertStringToDouble(string value)
	    {
		    try
		    {
				value = value.Replace(',', '.');
				return double.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
		    }
		    catch (Exception)
		    {
			    return 0;
		    }	    
		}

	    public bool MUpdateLocation(string driverC, string latitude, string longitude)
        {
            
            var location = _gpsLocationRepository.Get(item => item.DriverC == driverC);
            if (location != null)
            {
				location.Latitude = ConvertStringToDouble(latitude);
				location.Longitude = ConvertStringToDouble(longitude);
                location.UpdateD = DateTime.Now;
                _gpsLocationRepository.Update(location);
            }
            else
            {
                var newLocation = new GpsLocation_D()
                {
                    DriverC = driverC,
					Latitude = ConvertStringToDouble(latitude),
					Longitude = ConvertStringToDouble(longitude),
                    UpdateD = DateTime.Now
                };
                _gpsLocationRepository.Add(newLocation);
            }
            
            SaveGpsLocation();
            return true;
        }
        public void SaveGpsLocation()
        {
            _unitOfWork.Commit();
        }

        private int numTransportPassed(Dispatch_D dispatch)
        {
            var numPassed = 0;
            if (dispatch.IsTransported1 && !dispatch.IsTransported2 && !dispatch.IsTransported3)
            {
                numPassed = 1;
            }
            else if (dispatch.IsTransported2 && !dispatch.IsTransported3)
            {
                numPassed = 2;
            }
            else if (dispatch.IsTransported3)
            {
                numPassed = 3;
            }
            return numPassed;
        }
        private int checkTruckEmpty(string ope1, string ope2)
        {
            //Empty
            if ((ope1 == "LR" && ope2 == "LH") ||
                (ope1 == "LR" && ope2 == "LC") ||
                (ope1 == "LR" && ope2 == "HC") ||
                (ope1 == "LR" && ope2 == "TR") ||
                (ope1 == "HC" && ope2 == "LH") ||
                (ope1 == "HC" && ope2 == "LC") ||
                (ope1 == "HC" && ope2 == "TR") ||
                (ope1 == "HC" && ope2 == "XH") ||
                (ope1 == "HC" && ope2 == "TR") ||
                (ope1 == "XH" && ope2 == "LH") ||
                (ope1 == "XH" && ope2 == "LC") ||
                (ope1 == "XH" && ope2 == "LR") ||
                (ope1 == "XH" && ope2 == "HC") ||
                (ope1 == "XH" && ope2 == "TR") ||
                (ope1 == "TR" && ope2 == "LH") ||
                (ope1 == "TR" && ope2 == "LC") ||
                (ope1 == "TR" && ope2 == "LR") ||
                (ope1 == "TR" && ope2 == "HC") ||
                (ope1 == "TR" && ope2 == "XH") ||
                (ope1 == "TR" && ope2 == "TR"))
            {
                return 1;
            }
            else if (ope1 == "LH" || ope1 == "LC")
            {
                return 0;
            }
            else
            {
                return 2;
            }
        }

        private int checkGoBack(string status)
        {
            var result = 2;
            if (status == "2" || status == "4")
            {
                result = 0; //Di
            }
            if (status == "3" )
            {
                result = 1; //Ve
            }
            ////Di
            //if ((ope1 == "LR" && ope2 == "LH") ||
            //    (ope1 == "LC" && ope2 == "XH"))
            //{
            //    result = 0;
            //}
            //if((ope1 == "LH" && ope2 == "HC") ||
            //    (ope1 == "XH" && ope2 == "TR"))
            //{
            //    result = 1;
            //}
            return result;
        }
        public List<GpsViewModel> GetGpsLocationList()
        {
            var dt = new DateTime(2018, 08, 01);
            var today = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
            var listDriver = _driverRepository.GetAllQueryable().ToList();
            var listTruck = _truckRepository.GetAllQueryable().ToList();
            var listAllGps = _gpsLocationRepository.GetAllQueryable().ToList();
            var listGps = (from driver in listDriver
                           join gps in listAllGps on driver.DriverC equals gps.DriverC
                    into o
                from gps in o.DefaultIfEmpty()
                join dispatch in _dispatchRepository.GetAllQueryable() on driver.DriverC equals dispatch.DriverC
                where driver.IsActive == Constants.ACTIVE && dispatch.DispatchStatus == Constants.DISPATCH
                      && (dispatch.TransportD != null && dispatch.TransportD <= today)
                      && (((dispatch.Location1DT != null && dispatch.Location1DT >= today) || (dispatch.Location1DT == null && ((dispatch.Location2DT != null && dispatch.Location2DT >= today) || (dispatch.Location3DT != null && dispatch.Location3DT >= today))))
                          || ((dispatch.Location2DT != null && dispatch.Location2DT >= today) || (dispatch.Location2DT == null && ((dispatch.Location1DT != null && dispatch.Location1DT >= today) || (dispatch.Location3DT != null && dispatch.Location3DT >= today))))
                          || ((dispatch.Location3DT != null && dispatch.Location3DT >= today) || (dispatch.Location3DT == null && ((dispatch.Location2DT != null && dispatch.Location2DT >= today) || (dispatch.Location1DT != null && dispatch.Location1DT >= today))))
                          || ((dispatch.Location1DT == null && (string.IsNullOrEmpty(dispatch.Operation1C) || dispatch.Operation1C == "0")) && (dispatch.Location2DT == null && (string.IsNullOrEmpty(dispatch.Operation2C) || dispatch.Operation2C == "0")) && (dispatch.Location3DT == null && (string.IsNullOrEmpty(dispatch.Operation3C) || dispatch.Operation3C == "0"))))
                select new GpsViewModel()
                {
                    DriverC = driver.DriverC,
                    FirstN = driver.FirstN,
                    LastN = driver.LastN,
                    PhoneNumber = driver.PhoneNumber,
                    Latitude = (gps != null) ? gps.Latitude : 0,
                    Longitude = (gps != null) ? gps.Longitude : 0,
                    UpdateD = (gps != null) ? gps.UpdateD : DateTime.Now,
                    DriverDispatch = dispatch,
                    IsTruckEmpty = (dispatch != null) ? 3 : 2,
                    IsTruckGoBack = (dispatch != null) ? 3 : 2
                }).ToList();
            var joinByTruck = (from truck in listTruck
                               join gps in listGps on truck.TruckC equals gps.DriverDispatch.TruckC
                                   into o
                               from gps in o.DefaultIfEmpty()
                               where truck.IsActive == Constants.ACTIVE
                               select new GpsViewModel()
                               {
                                   DriverC = (gps != null) ? gps.DriverC : (truck.DriverC ?? ""),
                                   FirstN = (gps != null) ? gps.FirstN : "",
                                   LastN = (gps != null) ? gps.LastN : "",
                                   RegisteredNo =  truck.RegisteredNo ?? "",
                                   TruckC = truck.TruckC ?? "",
                                   DepC = truck.DepC ?? "",
                                   GrossWeight = truck.GrossWeight ?? 0,
                                   ModelC = truck.ModelC ?? "",
                                   PhoneNumber = (gps != null) ? gps.PhoneNumber : "",
                                   Latitude = (gps != null) ? gps.Latitude : 0,
                                   Longitude = (gps != null) ? gps.Longitude : 0,
                                   UpdateD = (gps != null) ? gps.UpdateD : null,
                                   IsDriverDefault = (gps != null) ? 0 : 1,
                                   DispatchStatus = (gps != null) ? 1 : 0,
                                   DriverDispatch = (gps != null) ? gps.DriverDispatch : null,
                                   IsTruckEmpty = (gps != null) ? gps.IsTruckEmpty : 3,
                                   IsTruckGoBack = (gps != null) ? gps.IsTruckGoBack : 3
                               }).ToList();
            //Group by DriverC
            var groupByTruck = (from p in joinByTruck
                           group p by new
                           {
                               p.DriverC,
                               p.FirstN,
                               p.LastN,
                               p.TruckC,
                               p.DepC,
                               p.GrossWeight,
                               p.ModelC,
                               p.RegisteredNo,
                               p.PhoneNumber,
                               p.Latitude,
                               p.Longitude,
                               p.UpdateD,
                               p.DispatchStatus,
                               p.IsDriverDefault,
                               p.IsTruckEmpty,
                               p.IsTruckGoBack
                           } into g
                           select new GpsViewModel()
                           {
                               DriverC = g.Key.DriverC,
                               FirstN = g.Key.FirstN,
                               LastN = g.Key.LastN,
                               RegisteredNo = g.Key.RegisteredNo,
                               TruckC = g.Key.TruckC,
                               DepC = g.Key.DepC,
                               GrossWeight = g.Key.GrossWeight,
                               ModelC = g.Key.ModelC,
                               PhoneNumber = g.Key.PhoneNumber,
                               Latitude = g.Key.Latitude,
                               Longitude = g.Key.Longitude,
                               UpdateD = g.Key.UpdateD,
                               IsDriverDefault = g.Key.IsDriverDefault,
                               DispatchStatus = g.Key.DispatchStatus,
                               TargetDispatch = g.ToList().Count,
                               ListDispatchViewModels = g.ToList(),
                               IsTruckEmpty = g.Key.IsTruckEmpty,
                               IsTruckGoBack = g.Key.IsTruckGoBack
                           }).ToList();
            var listModel = _modelRepository.GetAllQueryable().ToList();
            var listDep = _departmentRepository.GetAllQueryable().ToList();
            //Find target dispatch of array dispatch for display in screen
            foreach (var item in groupByTruck)
            {
                if (!string.IsNullOrEmpty(item.DepC))
                {
                    var dep = listDep.FirstOrDefault(d => d.DepC.Equals(item.DepC));
                    if (dep != null) item.DepName = dep.DepN ?? "";
                }
                if (!string.IsNullOrEmpty(item.ModelC))
                {
                    var model = listModel.FirstOrDefault(m => m.ModelC == item.ModelC);
                    if (model != null) item.ModelName = model.ModelN ?? "";
                }
                var gpsViewModel = item.ListDispatchViewModels.FirstOrDefault();
                if (gpsViewModel != null)
                {
                    var dispatch = gpsViewModel.DriverDispatch;
                    if (dispatch != null)
                    {
                        item.IsTruckGoBack = checkGoBack(dispatch.ContainerStatus);
                        switch (numTransportPassed(dispatch))
                        {
                            case 0:
                            {   
                                item.IsTruckEmpty = 0;
                            } break;
                            case 1:
                            {
                                item.IsTruckEmpty = checkTruckEmpty(dispatch.Operation1C, dispatch.Operation2C);
                                
                            } break;
                            case 2:
                            {
                                item.IsTruckEmpty = checkTruckEmpty(dispatch.Operation2C, dispatch.Operation3C);
                            } break;
                            default:
                                item.IsTruckEmpty = 2;
                                break;
                        }
                        
                    }
                }

                if (item.IsDriverDefault == 1 && item.DriverC != "")
                {
                    var driver = listDriver.FirstOrDefault(d => d.DriverC == item.DriverC);
                    if (driver != null)
                    {
                        item.FirstN = driver.FirstN;
                        item.LastN = driver.LastN;
                    }
                }
                if (item.Longitude == 0 && item.Latitude == 0)
                {
                    var findDriver = listAllGps.FirstOrDefault(d => d.DriverC.Equals(item.DriverC));
                    if (findDriver != null)
                    {
                        item.Latitude = findDriver.Latitude;
                        item.Longitude = findDriver.Longitude;
                        item.UpdateD = findDriver.UpdateD;
                    }
                }
                if (item.ListDispatchViewModels.ToArray()[0].DriverDispatch != null)
                {
                    var firstOrDefault = listTruck.FirstOrDefault(c => c.TruckC.Equals(item.ListDispatchViewModels.ToArray()[0].DriverDispatch.TruckC));
                    //1 item
                    if (item.TargetDispatch == 1)
                    {
                        if (firstOrDefault != null)
                        {
                            item.RegisteredNo = firstOrDefault.RegisteredNo;
                            item.TruckC = firstOrDefault.TruckC;
                            item.TargetDispatch = 0;
                        }
                    }
                    else
                    {
                        if (item.TargetDispatch != 0)
                        {
                            var list = item.ListDispatchViewModels.OrderBy(x => x.DriverDispatch.DispatchOrder).ToList();
                            //Utilities.Switch(list, indexOfDispatch, 0);
                            item.ListDispatchViewModels = list;
                            var custom = listTruck.FirstOrDefault(c => c.TruckC.Equals(item.ListDispatchViewModels.ToArray()[0].DriverDispatch.TruckC));
                            if (custom != null)
                            {
                                item.RegisteredNo = custom.RegisteredNo;
                                item.TruckC = custom.TruckC;
                                item.TargetDispatch = 0;
                            }
                            #region an code tam
                            ////find dispatch status = 1
                            //var isNotDefaultDispatch = false;
                            //foreach (var dispatch in item.ListDispatchViewModels)
                            //{
                            //    if (dispatch.DriverDispatch.DispatchStatus == Constants.DISPATCH)
                            //    {
                            //        var indexOfDispatch = item.ListDispatchViewModels.IndexOf(dispatch);
                            //        //move to index = 0
                            //        if (indexOfDispatch != 0)
                            //        {
                            //            var list = item.ListDispatchViewModels.OrderBy(x=>x.DriverDispatch.DispatchOrder).ToList();
                            //            //Utilities.Switch(list, indexOfDispatch, 0);
                            //            item.ListDispatchViewModels = list;
                            //            var custom = listTruck.FirstOrDefault(c => c.TruckC.Equals(item.ListDispatchViewModels.ToArray()[0].DriverDispatch.TruckC));
                            //            if (custom != null)
                            //            {
                            //                item.RegisteredNo = custom.RegisteredNo;
                            //                item.TruckC = custom.TruckC;
                            //                item.TargetDispatch = 0;
                            //            }
                            //            isNotDefaultDispatch = true;
                            //            break;
                            //        }
                            //    }

                            //}
                            //if (!isNotDefaultDispatch)
                            //{
                            //    if (firstOrDefault != null)
                            //    {
                            //        item.RegisteredNo = firstOrDefault.RegisteredNo;
                            //        item.TruckC = firstOrDefault.TruckC;
                            //        item.TargetDispatch = 0;
                            //    }
                            //}
#endregion
                        }
                        else
                        {
                            item.TargetDispatch = 0;
                        }
                    }
                }
            }

            return groupByTruck;
        }
    }

}
