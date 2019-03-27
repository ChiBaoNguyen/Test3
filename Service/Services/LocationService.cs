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
using Website.ViewModels.Location;
using Website.ViewModels.Expense;
using Website.ViewModels.Order;

namespace Service.Services
{
	public interface ILocationService
	{
		IEnumerable<LocationViewModel> GetLocations(string value);
		IEnumerable<LocationViewModel> GetLocationsForOrder(string value);
		IEnumerable<LocationViewModel> GetLocationsByCode(string value);
		LocationViewModel GetByName(string value);
        List<LocationViewModel> GetLocationByCodeForDispatch(string code);
		LocationStatusViewModel GetLocationByCode(string code);
		LocationDatatables GetLocationsForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string searchValue);
		void CreateLocation(LocationViewModel location);
		void UpdateLocation(LocationViewModel location);
		void DeleteLocation(string id);
		void SetStatusLocation(string id);
		void SaveLocation();
		List<ExpenseDetailViewModel> GetListExpenseData(string expenseCate, string liftuplowerParam, string internalParam,
			int detailNo,
			int dispatchNo,
			string location1, string location2, string location3, string ordertypeI, string containersizeI, string operation1,
			string operation2,
			string operation3, DateTime orderD, string orderNo);

		IEnumerable<LocationViewModel> GetLocations();
		IEnumerable<LocationViewModel> GetAreas();
		IEnumerable<LocationViewModel> GetAreas(string value);
		LocationViewModel GetAreaByName(string value);
		List<LocationViewModel> GetLocationsByArea(string value);
	}

	public class LocationService : ILocationService
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IExpenseDetailRepository _expenseDetailRepository;
		private readonly IExpenseRepository _expenseRepository;
		private readonly ISupplierRepository _supplierRepository;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly ILocationDetailRepository _locationDetailRepository;
		private readonly IUnitOfWork _unitOfWork;

		public LocationService(IExpenseDetailRepository expenseDetailRepository, IExpenseRepository expenseRepository,
			ISupplierRepository supplierRepository,
			IEmployeeRepository empoRepository, ILocationRepository locationRepository,
			ILocationDetailRepository locationDetailRepository,
			IUnitOfWork unitOfWork, IOrderRepository orderRepository)
		{
			this._expenseDetailRepository = expenseDetailRepository;
			this._expenseRepository = expenseRepository;
			this._supplierRepository = supplierRepository;
			this._employeeRepository = empoRepository;
			this._locationRepository = locationRepository;
			this._locationDetailRepository = locationDetailRepository;
			this._unitOfWork = unitOfWork;
		    _orderRepository = orderRepository;
		}

		public IEnumerable<LocationViewModel> GetLocations(string value)
		{
			var locations = _locationRepository.Query(i => (i.LocationC.Contains(value) ||
			                                                i.LocationN.Contains(value) ||
			                                                i.Address.Contains(value)) &&
			                                               i.IsActive == Constants.ACTIVE);
			if (locations != null)
			{
				var destination = Mapper.Map<IEnumerable<Location_M>, IEnumerable<LocationViewModel>>(locations);
				return destination;
			}
			return null;
		}

		public IEnumerable<LocationViewModel> GetLocationsForOrder(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				var param = value.Split('/');
				if (param.Length > 1)
				{
					var param0 = param[0];
					var param1 = param[1];
					if (param1 == "FromOrder")
					{
						var location = _locationRepository.Query(i => (i.LocationC.Contains(param0) ||
																i.LocationN.Contains(param0) ||
																i.Address.Contains(param0)) &&
															   i.IsActive == Constants.ACTIVE && i.LocationI != "3");
						if (location != null)
						{
							var destination = Mapper.Map<IEnumerable<Location_M>, IEnumerable<LocationViewModel>>(location);
							return destination;
						}
					}
				}
				var locations = _locationRepository.Query(i => (i.LocationC.Contains(value) ||
																i.LocationN.Contains(value) ||
																i.Address.Contains(value)) &&
															   i.IsActive == Constants.ACTIVE);
				if (locations != null)
				{
					var destination = Mapper.Map<IEnumerable<Location_M>, IEnumerable<LocationViewModel>>(locations);
					return destination;
				}
			}
			return null;
		}

		public IEnumerable<LocationViewModel> GetLocationsByCode(string value)
		{
			var locations = _locationRepository.Query(i => i.LocationC.StartsWith(value));
			if (locations != null)
			{
				var destination = Mapper.Map<IEnumerable<Location_M>, IEnumerable<LocationViewModel>>(locations);
				return destination;
			}
			return null;
		}
        public List<LocationViewModel> GetLocationByCodeForDispatch(string code)
        {
            var locations = _locationRepository.Query(l => ("," + code + ",").Contains(l.LocationC)).ToList();
            if (locations.Any())
            {
                var destination = Mapper.Map<List<Location_M>, List<LocationViewModel>>(locations);
                return destination;
            }
            return null;
        }
		public LocationStatusViewModel GetLocationByCode(string code)
		{
			var locationStatus = new LocationStatusViewModel();
			var location = _locationRepository.Query(i => i.LocationC == code).FirstOrDefault();
			if (location != null)
			{
				var locationViewModel = Mapper.Map<Location_M, LocationViewModel>(location);
				locationViewModel.LocationIndex = FindIndex(code);
				var listDetail = (from p in _locationDetailRepository.GetAllQueryable()
					where p.LocationC == code
					select new ExpenseViewModel()
					{
						ExpenseC = p.ExpenseC,
						ExpenseN = p.ExpenseN,
						ContainerSizeI = p.ContainerSizeI,
						LocationC = p.LocationC,
						AmountMoney = p.AmountMoney,
						Category = p.Category,
						Display = p.Display,
						CategoryExpense = p.CategoryExpense,
						Description = p.Description
					}).OrderBy(p => p.ContainerSizeI).ToList();
				locationViewModel.ExpenseData = listDetail;
				locationStatus.Location = locationViewModel;
				locationStatus.Status = CustomerStatus.Edit.ToString();
			}
			else
			{
				locationStatus.Status = CustomerStatus.Add.ToString();
			}
			return locationStatus;
		}

		public LocationViewModel GetByName(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				var location = _locationRepository.Query(loc => loc.LocationN == value).FirstOrDefault();
				if (location != null)
				{
					var destination = Mapper.Map<Location_M, LocationViewModel>(location);
					return destination;
				}
			}
			return null;
		}

		public LocationDatatables GetLocationsForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			string searchValue)
		{
			var location = (from l in _locationRepository.GetAllQueryable()
				join a in _locationRepository.GetAllQueryable()
					on l.AreaC equals a.LocationC into t
				from a in t.DefaultIfEmpty()
				//where a == null 
				//|| a.LocationI == "3"
				select new LocationViewModel()
				{
					LocationC = l.LocationC,
					LocationN = l.LocationN,
					LocationI = l.LocationI,
					Address = l.Address,
					Description = l.Description,
					AreaN = a.LocationN,
					IsActive = l.IsActive,
				}).AsQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(searchValue))
			{
				searchValue = searchValue.ToLower();
				location = location.Where(l => l.LocationN.ToLower().Contains(searchValue)
				                               || l.LocationC.ToLower().Contains(searchValue));
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var locationsOrdered = location.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var locationsPaged = locationsOrdered.Skip((page - 1)*itemsPerPage).Take(itemsPerPage).ToList();

			//var destination = Mapper.Map<List<Location_M>, List<LocationViewModel>>(locationsPaged);
			var datatable = new LocationDatatables()
			{
				Data = locationsPaged,
				Total = location.Count()
			};
			return datatable;
		}

		public void CreateLocation(LocationViewModel locationViewModel)
		{
			for (int l = 0; l < locationViewModel.ExpenseData.Count; l++)
			{
				if (locationViewModel.ExpenseData[l].Display == "True")
				{
					locationViewModel.ExpenseData[l].Display = "1";
				}
				if (locationViewModel.ExpenseData[l].Display == "False")
				{
					locationViewModel.ExpenseData[l].Display = "0";
				}
			}
			var location = Mapper.Map<LocationViewModel, Location_M>(locationViewModel);
			_locationRepository.Add(location);
			var expenseData = locationViewModel.ExpenseData;
			if (expenseData != null)
			{
				if (expenseData.Any())
				{
					for (int i = 0; i < expenseData.Count; i++)
					{
						LocationDetail_M locaDetailM = new LocationDetail_M();
						locaDetailM.LocationDetailId = i;
						locaDetailM.LocationC = locationViewModel.LocationC;
						locaDetailM.ExpenseC = expenseData[i].ExpenseC;
						locaDetailM.ExpenseN = expenseData[i].ExpenseN ?? "";
						locaDetailM.ContainerSizeI = expenseData[i].ContainerSizeI;
						locaDetailM.Display = expenseData[i].Display;
						locaDetailM.AmountMoney = expenseData[i].AmountMoney ?? 0;
						locaDetailM.Category = expenseData[i].Category ?? "";
						locaDetailM.CategoryExpense = expenseData[i].CategoryExpense ?? "";
						locaDetailM.Description = expenseData[i].Description ?? "";
						_locationDetailRepository.Add(locaDetailM);
					}
				}
			}
			SaveLocation();
		}

		public void UpdateLocation(LocationViewModel location)
		{
			if (location != null)
			{
				for (int l = 0; l < location.ExpenseData.Count; l++)
				{
					if (location.ExpenseData[l].Display == "True")
					{
						location.ExpenseData[l].Display = "1";
					}
					if (location.ExpenseData[l].Display == "False")
					{
						location.ExpenseData[l].Display = "0";
					}
				}
				var locationToRemove = _locationRepository.GetById(location.LocationC);
				var updateLocation = Mapper.Map<LocationViewModel, Location_M>(location);
				_locationRepository.Delete(locationToRemove);
				var removeDetail = _locationDetailRepository.Query(p => p.LocationC == location.LocationC).ToList();
				if (removeDetail.Count > 0)
				{
					for (int i = 0; i < removeDetail.Count; i++)
					{
						_locationDetailRepository.Delete(removeDetail[i]);
					}
				}
				_locationRepository.Add(updateLocation);
				var expenseData = location.ExpenseData;
				if (expenseData != null)
				{
					if (expenseData.Any())
					{
						for (int i = 0; i < expenseData.Count; i++)
						{
							LocationDetail_M locaDetailM = new LocationDetail_M();
							locaDetailM.LocationDetailId = i;
							locaDetailM.LocationC = location.LocationC;
							locaDetailM.ExpenseC = expenseData[i].ExpenseC;
							locaDetailM.ExpenseN = expenseData[i].ExpenseN ?? "";
							locaDetailM.ContainerSizeI = expenseData[i].ContainerSizeI;
							locaDetailM.Display = expenseData[i].Display;
							locaDetailM.AmountMoney = expenseData[i].AmountMoney ?? 0;
							locaDetailM.Category = expenseData[i].Category ?? "";
							locaDetailM.CategoryExpense = expenseData[i].CategoryExpense ?? "";
							locaDetailM.Description = expenseData[i].Description ?? "";
							_locationDetailRepository.Add(locaDetailM);
						}
					}
				}
				SaveLocation();
			}
		}

		//using for active and deactive location
		public void SetStatusLocation(string id)
		{
			var locationToRemove = _locationRepository.Get(c => c.LocationC == id);
			if (locationToRemove.IsActive == Constants.ACTIVE)
			{
				locationToRemove.IsActive = Constants.DEACTIVE;
			}
			else
			{
				locationToRemove.IsActive = Constants.ACTIVE;
			}
			_locationRepository.Update(locationToRemove);
			SaveLocation();
		}

		public void DeleteLocation(string id)
		{
			//var locationToRemove = _locationRepository.Get(c => c.LocationC == id);
			//if (locationToRemove != null)
			//{
			//	_locationRepository.Delete(locationToRemove);
			//	SaveLocation();
			//}
			var locationToRemove = _locationRepository.Get(c => c.LocationC == id);
			var locationDetailToRemove =
				_locationDetailRepository.Query(
					i => i.LocationC == id).ToList();
			if (locationToRemove != null)
			{
				_locationRepository.Delete(locationToRemove);
			}
			if (locationDetailToRemove.Count > 0)
			{
				foreach (var locationdetail in locationDetailToRemove)
				{
					_locationDetailRepository.Delete(locationdetail);
				}
			}
			SaveLocation();
		}

		private int FindIndex(string code)
		{
			var data = _locationRepository.GetAllQueryable();
			var index = 0;
			var totalRecords = data.Count();
			var halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords/2))) + 1;
			var loopCapacity = 100;
			var recordsToSkip = 0;
			if (totalRecords > 0)
			{
				var nextIteration = true;
				while (nextIteration)
				{
					for (var counter = 0; counter < 2; counter++)
					{
						recordsToSkip = recordsToSkip + (counter*halfCount);

						if (data.OrderBy("LocationC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.LocationC == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount*1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords/2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("LocationC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.LocationC == code)
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

		public void SaveLocation()
		{
			_unitOfWork.Commit();
		}

		public IEnumerable<LocationViewModel> GetLocations()
		{
			var info = from a in _locationRepository.GetAllQueryable()
				select new LocationViewModel()
				{
					LocationC = a.LocationC,
					LocationN = a.LocationN,
				};
			return info.ToList();
		}

		public IEnumerable<LocationViewModel> GetAreas()
		{
			var info = from a in _locationRepository.GetAllQueryable()
				where a.LocationI == "3"
				select new LocationViewModel()
				{
					LocationC = a.LocationC,
					LocationN = a.LocationN,
				};
			return info.ToList();
		}

		public IEnumerable<LocationViewModel> GetAreas(string value)
		{
			var locations = _locationRepository.Query(i => (i.LocationC.Contains(value) ||
			                                                i.LocationN.Contains(value) ||
			                                                i.Address.Contains(value)) &&
														   //i.LocationI == "3" &&
			                                               i.IsActive == Constants.ACTIVE);
			if (locations != null)
			{
				var destination = Mapper.Map<IEnumerable<Location_M>, IEnumerable<LocationViewModel>>(locations);
				return destination;
			}
			return null;
		}

		public LocationViewModel GetAreaByName(string value)
		{
			var location =
				_locationRepository.Query(loc => loc.LocationN.Equals(value)/* && loc.LocationI.Equals("3")*/).FirstOrDefault();
			if (location != null)
			{
				var destination = Mapper.Map<Location_M, LocationViewModel>(location);
				return destination;
			}
			return null;
		}

		public List<LocationViewModel> GetLocationsByArea(string value)
		{
			var locations = from a in _locationRepository.GetAllQueryable()
				where a.AreaC.Equals(value)
				select new LocationViewModel()
				{
					LocationC = a.LocationC,
					LocationN = a.LocationN,
				};
			return locations.ToList();
		}

		public List<ExpenseDetailViewModel> GetListExpenseData(string expenseCate, string liftuplowerParam,
			string internalParam, int detailNo,
			int dispatchNo,
			string location1, string location2, string location3, string ordertypeI, string containersizeI, string operation1,
			string operation2,
			string operation3, DateTime orderD, string orderNo)
		{
			List<ExpenseDetailViewModel> listExpense = new List<ExpenseDetailViewModel>();
			List<ExpenseDetailViewModel> expenseDetailList;
			//hang xuat
            var orderH = _orderRepository.Query(p => p.OrderD.Equals(orderD) && p.OrderNo.Equals(orderNo)).FirstOrDefault();
			
			if (ordertypeI == "0")
			{
				if ((operation1 == "LR" || operation1 == "HC" || operation1 == "XH" || operation1 == "LH") &&
				    (operation2 == "LR" || operation2 == "HC" || operation2 == "XH" || operation2 == "LH") &&
				    (string.IsNullOrEmpty(operation3) || operation3 == "0"))
				{
					if (liftuplowerParam == "1" && internalParam == "0")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location2) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "5" || expense.CategoryExpense == "0")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = operation1 == "LR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location1 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "1")
								.FirstOrDefault()
							: (operation1 == "HC"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location1 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "4")
									.FirstOrDefault()
								: null);

						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense1.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense1.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
								SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
								SupplierN = loca1 != null ? loca1.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense1.ExpenseC,
								ExpenseN = expense1.ExpenseN
							});
						}
						var expense2 = operation2 == "HC"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location2 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "4")
								.FirstOrDefault()
							: (operation2 == "LR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location2 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "1")
									.FirstOrDefault()
								: ((operation2 == "LH" || operation2 == "XH") && internalParam == "1"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location2 && p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
											p.CategoryExpense == "5")
										.FirstOrDefault()
									: null));
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense2.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense2.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
								SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
								SupplierN = loca2 != null ? loca2.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense2.ExpenseC,
								ExpenseN = expense2.ExpenseN
							});
						}
					}
					else if (liftuplowerParam == "0" && internalParam == "1")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location2) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "1" || expense.CategoryExpense == "2" || expense.CategoryExpense == "3" ||
									    expense.CategoryExpense == "4")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();

						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}
						var expense2 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location2 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int u = 0; u < expense2.Count; u++)
								{
									string expC = expense2[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[u].ExpenseC,
										ExpenseN = expense2[u].ExpenseN
									});
								}
							}
						}
					}
					else if (liftuplowerParam == "1" && internalParam == "1")
					{
						var expense1 = operation1 == "LR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location1 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation1 == "HC"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location1 && ((p.CategoryExpense == "4") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation1 == "LH" || operation1 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location1 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));

						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}
						var expense2 = operation2 == "HC"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location2 && ((p.CategoryExpense == "4") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation2 == "LR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location2 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation2 == "LH" || operation2 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location2 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int u = 0; u < expense2.Count; u++)
								{
									string expC = expense2[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[u].ExpenseC,
										ExpenseN = expense2[u].ExpenseN
									});
								}
							}
						}
					}
					else
					{
						listExpense = null;
					}
				}
				if ((operation1 == "LR" || operation1 == "HC" || operation1 == "XH" || operation1 == "LH") &&
				    (operation3 == "HC" || operation3 == "LR" || operation3 == "XH" || operation3 == "LH") &&
				    (string.IsNullOrEmpty(operation2) || operation2 == "0"))
				{
					if (liftuplowerParam == "1" && internalParam == "0")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location3) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "5" || expense.CategoryExpense == "0")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = operation1 == "LR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location1 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "1")
								.FirstOrDefault()
							: (operation1 == "HC"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location1 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "4")
									.FirstOrDefault()
								: null);
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense1.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense1.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
								SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
								SupplierN = loca1 != null ? loca1.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense1.ExpenseC,
								ExpenseN = expense1.ExpenseN
							});
						}
						var expense3 = operation3 == "HC"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location3 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "4")
								.FirstOrDefault()
							: (operation3 == "LR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location3 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "1")
									.FirstOrDefault()
								: ((operation3 == "LH" || operation3 == "XH") && internalParam == "1"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location3 && p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
											p.CategoryExpense == "5")
										.FirstOrDefault()
									: null));
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense3.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense3.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
								SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
								SupplierN = loca3 != null ? loca3.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense3.ExpenseC,
								ExpenseN = expense3.ExpenseN
							});
						}
					}
					else if (liftuplowerParam == "0" && internalParam == "1")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location3) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "1" || expense.CategoryExpense == "2" || expense.CategoryExpense == "3" ||
									    expense.CategoryExpense == "4")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}
						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}
					}
					else if (liftuplowerParam == "1" && internalParam == "1")
					{
						var expense1 = operation1 == "LR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location1 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation1 == "HC"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location1 &&
										((p.CategoryExpense == "4") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation1 == "LH" || operation1 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location1 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}

						var expense3 = operation3 == "HC"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location3 && ((p.CategoryExpense == "4") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation3 == "LR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location3 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation3 == "LH" || operation3 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location3 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}
					}
					else
					{
						listExpense = null;
					}
				}
				if ((operation2 == "LR" || operation2 == "HC" || operation2 == "XH" || operation2 == "LH") &&
				    (operation3 == "HC" || operation3 == "LR" || operation3 == "XH" || operation3 == "LH") &&
				    (string.IsNullOrEmpty(operation1) || operation1 == "0"))
				{
					if (liftuplowerParam == "1" && internalParam == "0")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location3 || p.LocationC == location2) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "5" || expense.CategoryExpense == "0")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense2 = operation2 == "LR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location2 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "1")
								.FirstOrDefault()
							: (operation2 == "HC"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location2 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "4")
									.FirstOrDefault()
								: null);
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense2.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense2.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
								SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
								SupplierN = loca2 != null ? loca2.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense2.ExpenseC,
								ExpenseN = expense2.ExpenseN
							});
						}
						var expense3 = operation3 == "HC"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location3 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "4")
								.FirstOrDefault()
							: (operation3 == "LR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location3 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "1")
									.FirstOrDefault()
								: ((operation3 == "LH" || operation3 == "XH") && internalParam == "1"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location3 && p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
											p.CategoryExpense == "5")
										.FirstOrDefault()
									: null));
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense3.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense3.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
								SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
								SupplierN = loca3 != null ? loca3.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense3.ExpenseC,
								ExpenseN = expense3.ExpenseN
							});
						}
					}
					else if (liftuplowerParam == "0" && internalParam == "1")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location3 || p.LocationC == location2) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "1" || expense.CategoryExpense == "2" || expense.CategoryExpense == "3" ||
									    expense.CategoryExpense == "4")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense2 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location2 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int i = 0; i < expense2.Count; i++)
								{
									string expC = expense2[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[i].ExpenseC,
										ExpenseN = expense2[i].ExpenseN
									});
								}
							}
						}
						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}
					}
					else if (liftuplowerParam == "1" && internalParam == "1")
					{
						var expense2 = operation2 == "LR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location2 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation2 == "HC"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location2 && ((p.CategoryExpense == "4") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation2 == "LH" || operation2 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location2 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int i = 0; i < expense2.Count; i++)
								{
									string expC = expense2[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[i].ExpenseC,
										ExpenseN = expense2[i].ExpenseN
									});
								}
							}
						}

						var expense3 = operation3 == "HC"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location3 && ((p.CategoryExpense == "4") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation3 == "LR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location3 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation3 == "LH" || operation3 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location3 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}

					}
					else
					{
						listExpense = null;
					}
				}

				if (operation1 == "LR" && (operation2 == "LH" || operation2 == "XH") && operation3 == "HC")
				{
					if (liftuplowerParam == "1" && internalParam == "0")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location2 || p.LocationC == location3) && p.ExpenseC == exC)
										.FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "5" || expense.CategoryExpense == "0")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
								p.CategoryExpense == "1")
							.FirstOrDefault();
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense1.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense1.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
								SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
								SupplierN = loca1 != null ? loca1.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense1.ExpenseC,
								ExpenseN = expense1.ExpenseN
							});
						}
						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
								p.CategoryExpense == "4")
							.FirstOrDefault();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense3.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense3.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
								SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
								SupplierN = loca3 != null ? loca3.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense3.ExpenseC,
								ExpenseN = expense3.ExpenseN
							});
						}
					}
					else if (liftuplowerParam == "0" && internalParam == "1")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location2 || p.LocationC == location3) && p.ExpenseC == exC)
										.FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "1" || expense.CategoryExpense == "2" || expense.CategoryExpense == "3" ||
									    expense.CategoryExpense == "4")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}
						var expense2 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location2 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int i = 0; i < expense2.Count; i++)
								{
									string expC = expense2[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[i].ExpenseC,
										ExpenseN = expense2[i].ExpenseN
									});
								}
							}
						}
						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int i = 0; i < expense3.Count; i++)
								{
									var exM = _expenseRepository.Query(p => p.ExpenseC == expense3[i].ExpenseC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[i].ExpenseC,
										ExpenseN = expense3[i].ExpenseN
									});
								}
							}
						}
					}
					else if (liftuplowerParam == "1" && internalParam == "1")
					{
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
								p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
							.ToList();
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}

						var expense2 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location2 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int i = 0; i < expense2.Count; i++)
								{
									string expC = expense2[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[i].ExpenseC,
										ExpenseN = expense2[i].ExpenseN
									});
								}
							}
						}

						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && ((p.CategoryExpense == "4") || (p.CategoryExpense == "5")) &&
								p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
							.ToList();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}

					}
					else
					{
						listExpense = null;
					}
				}
			}
			//hang nhap
			if (ordertypeI == "1")
			{
				if ((operation1 == "LC" || operation1 == "TR" || operation1 == "XH" || operation1 == "LH") &&
				    (operation2 == "LC" || operation2 == "TR" || operation2 == "XH" || operation2 == "LH") &&
				    (string.IsNullOrEmpty(operation3) || operation3 == "0"))
				{
					if (liftuplowerParam == "1" && internalParam == "0")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location2) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "5" || expense.CategoryExpense == "0")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = operation1 == "LC"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location1 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "3")
								.FirstOrDefault()
							: (operation1 == "TR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location1 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "2")
									.FirstOrDefault()
								: null);

						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense1.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense1.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
								SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
								SupplierN = loca1 != null ? loca1.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense1.ExpenseC,
								ExpenseN = expense1.ExpenseN
							});
						}
						var expense2 = operation2 == "TR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location2 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "2")
								.FirstOrDefault()
							: (operation2 == "LC"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location2 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "3")
									.FirstOrDefault()
								: ((operation2 == "LH" || operation2 == "XH") && internalParam == "1"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location2 && p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
											p.CategoryExpense == "5")
										.FirstOrDefault()
									: null));
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense2.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense2.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
								SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
								SupplierN = loca2 != null ? loca2.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense2.ExpenseC,
								ExpenseN = expense2.ExpenseN
							});
						}
					}
					else if (liftuplowerParam == "0" && internalParam == "1")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location2) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "1" || expense.CategoryExpense == "2" || expense.CategoryExpense == "3" ||
									    expense.CategoryExpense == "4")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();

						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}
						var expense2 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location2 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int u = 0; u < expense2.Count; u++)
								{
									string expC = expense2[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[u].ExpenseC,
										ExpenseN = expense2[u].ExpenseN
									});
								}
							}
						}
					}
					else if (liftuplowerParam == "1" && internalParam == "1")
					{
						var expense1 = operation1 == "LC"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location1 && ((p.CategoryExpense == "3") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation1 == "TR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location1 && ((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation1 == "LH" || operation1 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location1 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));

						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}

						var expense2 = operation2 == "TR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location2 && ((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation2 == "LC"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location2 && ((p.CategoryExpense == "3") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation2 == "LH" || operation2 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location2 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int u = 0; u < expense2.Count; u++)
								{
									string expC = expense2[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[u].ExpenseC,
										ExpenseN = expense2[u].ExpenseN
									});
								}
							}
						}

					}
					else
					{
						listExpense = null;
					}
				}
				if ((operation1 == "LC" || operation1 == "TR" || operation1 == "XH" || operation1 == "LH") &&
				    (operation3 == "TR" || operation3 == "LC" || operation3 == "XH" || operation3 == "LH") &&
				    (string.IsNullOrEmpty(operation2) || operation2 == "0"))
				{
					if (liftuplowerParam == "1" && internalParam == "0")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location3) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "5" || expense.CategoryExpense == "0")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = operation1 == "LC"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location1 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "3")
								.FirstOrDefault()
							: (operation1 == "TR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location1 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "2")
									.FirstOrDefault()
								: null);
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense1.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense1.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
								SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
								SupplierN = loca1 != null ? loca1.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense1.ExpenseC,
								ExpenseN = expense1.ExpenseN
							});
						}
						var expense3 = operation3 == "TR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location3 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "2")
								.FirstOrDefault()
							: (operation3 == "LC"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location3 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "3")
									.FirstOrDefault()
								: ((operation3 == "LH" || operation3 == "XH") && internalParam == "1"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location3 && p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
											p.CategoryExpense == "5")
										.FirstOrDefault()
									: null));
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense3.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense3.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
								SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
								SupplierN = loca3 != null ? loca3.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense3.ExpenseC,
								ExpenseN = expense3.ExpenseN
							});
						}
					}
					else if (liftuplowerParam == "0" && internalParam == "1")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location3) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "1" || expense.CategoryExpense == "2" || expense.CategoryExpense == "3" ||
									    expense.CategoryExpense == "4")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}
						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}
					}
					else if (liftuplowerParam == "1" && internalParam == "1")
					{
						var expense1 = operation1 == "LC"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location1 && ((p.CategoryExpense == "3") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation1 == "TR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location1 && ((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation1 == "LH" || operation1 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location1 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}

						var expense3 = operation3 == "TR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location3 && ((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation3 == "LC"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location3 && ((p.CategoryExpense == "3") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation3 == "LH" || operation3 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location3 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}

					}
					else
					{
						listExpense = null;
					}
				}
				if ((operation2 == "LC" || operation2 == "TR" || operation2 == "XH" || operation2 == "LH") &&
				    (operation3 == "TR" || operation3 == "LC" || operation3 == "XH" || operation3 == "LH") &&
				    (string.IsNullOrEmpty(operation1) || operation1 == "0"))
				{
					if (liftuplowerParam == "1" && internalParam == "0")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location3 || p.LocationC == location2) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "5" || expense.CategoryExpense == "0")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense2 = operation2 == "LC"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location2 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "3")
								.FirstOrDefault()
							: (operation2 == "TR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location2 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "2")
									.FirstOrDefault()
								: null);
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense2.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense2.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
								SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
								SupplierN = loca2 != null ? loca2.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense2.ExpenseC,
								ExpenseN = expense2.ExpenseN
							});
						}
						var expense3 = operation3 == "TR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location3 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "2")
								.FirstOrDefault()
							: (operation3 == "LC"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location3 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "3")
									.FirstOrDefault()
								: ((operation3 == "LH" || operation3 == "XH") && internalParam == "1"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location3 && p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
											p.CategoryExpense == "5")
										.FirstOrDefault()
									: null));
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense3.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense3.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
								SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
								SupplierN = loca3 != null ? loca3.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense3.ExpenseC,
								ExpenseN = expense3.ExpenseN
							});
						}
					}
					else if (liftuplowerParam == "0" && internalParam == "1")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location3 || p.LocationC == location2) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "1" || expense.CategoryExpense == "2" || expense.CategoryExpense == "3" ||
									    expense.CategoryExpense == "4")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense2 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location2 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int i = 0; i < expense2.Count; i++)
								{
									string expC = expense2[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[i].ExpenseC,
										ExpenseN = expense2[i].ExpenseN
									});
								}
							}
						}
						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}
					}
					else if (liftuplowerParam == "1" && internalParam == "1")
					{
						var expense2 = operation2 == "LC"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location2 && ((p.CategoryExpense == "3") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation2 == "TR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location2 && ((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation2 == "LH" || operation2 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location2 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int i = 0; i < expense2.Count; i++)
								{
									string expC = expense2[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[i].ExpenseC,
										ExpenseN = expense2[i].ExpenseN
									});
								}
							}
						}

						var expense3 = operation3 == "TR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location3 && ((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation3 == "LC"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location3 && ((p.CategoryExpense == "3") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation3 == "LH" || operation3 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location3 && ((p.CategoryExpense == "3") || (p.CategoryExpense == "5")) &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}

					}
					else
					{
						listExpense = null;
					}
				}

				if (operation1 == "LC" && (operation2 == "LH" || operation2 == "XH") && operation3 == "TR")
				{
					if (liftuplowerParam == "1" && internalParam == "0")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location2 || p.LocationC == location3) && p.ExpenseC == exC)
										.FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "5" || expense.CategoryExpense == "0")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
								p.CategoryExpense == "3")
							.FirstOrDefault();
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense1.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense1.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
								SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
								SupplierN = loca1 != null ? loca1.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense1.ExpenseC,
								ExpenseN = expense1.ExpenseN
							});
						}
						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
								p.CategoryExpense == "2")
							.FirstOrDefault();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense3.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense3.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
								SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
								SupplierN = loca3 != null ? loca3.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense3.ExpenseC,
								ExpenseN = expense3.ExpenseN
							});
						}
					}
					else if (liftuplowerParam == "0" && internalParam == "1")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location2 || p.LocationC == location3) && p.ExpenseC == exC)
										.FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "1" || expense.CategoryExpense == "2" || expense.CategoryExpense == "3" ||
									    expense.CategoryExpense == "4")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) &&
								p.CategoryExpense == "5")
							.ToList();
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}

						var expense2 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location2 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int i = 0; i < expense2.Count; i++)
								{
									string expC = expense2[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[i].ExpenseC,
										ExpenseN = expense2[i].ExpenseN
									});
								}
							}
						}

						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int i = 0; i < expense3.Count; i++)
								{
									string expC = expense3[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[i].ExpenseC,
										ExpenseN = expense3[i].ExpenseN
									});
								}
							}
						}
					}
					else if (liftuplowerParam == "1" && internalParam == "1")
					{
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && ((p.CategoryExpense == "3") || (p.CategoryExpense == "5")) &&
								p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
							.ToList();
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}

						var expense2 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location2 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int i = 0; i < expense2.Count; i++)
								{
									string expC = expense2[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[i].ExpenseC,
										ExpenseN = expense2[i].ExpenseN
									});
								}
							}
						}

						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && ((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
								p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
							.ToList();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}

					}
					else
					{
						listExpense = null;
					}
				}
			}

			//hang khac
			if (ordertypeI == "2")
			{
				if ((operation1 == "LR" || operation1 == "TR" || operation1 == "XH" || operation1 == "LH") &&
				    (operation2 == "LR" || operation2 == "TR" || operation2 == "XH" || operation2 == "LH") &&
				    (string.IsNullOrEmpty(operation3) || operation3 == "0"))
				{
					if (liftuplowerParam == "1" && internalParam == "0")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location2) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "5" || expense.CategoryExpense == "0")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = operation1 == "LR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location1 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "1")
								.FirstOrDefault()
							: (operation1 == "TR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location1 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "2")
									.FirstOrDefault()
								: null);

						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense1.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense1.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
								SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
								SupplierN = loca1 != null ? loca1.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense1.ExpenseC,
								ExpenseN = expense1.ExpenseN
							});
						}
						var expense2 = operation2 == "TR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location2 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "2")
								.FirstOrDefault()
							: (operation2 == "LR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location2 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "1")
									.FirstOrDefault()
								: ((operation2 == "LH" || operation2 == "XH") && internalParam == "1"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location2 && p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
											p.CategoryExpense == "5")
										.FirstOrDefault()
									: null));
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense2.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense2.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
								SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
								SupplierN = loca2 != null ? loca2.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense2.ExpenseC,
								ExpenseN = expense2.ExpenseN
							});
						}
					}
					else if (liftuplowerParam == "0" && internalParam == "1")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location2) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "1" || expense.CategoryExpense == "2" || expense.CategoryExpense == "3" ||
									    expense.CategoryExpense == "4")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();

						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}
						var expense2 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location2 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int u = 0; u < expense2.Count; u++)
								{
									string expC = expense2[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[u].ExpenseC,
										ExpenseN = expense2[u].ExpenseN
									});
								}
							}
						}
					}
					else if (liftuplowerParam == "1" && internalParam == "1")
					{
						var expense1 = operation1 == "LR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location1 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation1 == "TR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location1 && ((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation1 == "LH" || operation1 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location1 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));

						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}

						var expense2 = operation2 == "TR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location2 && ((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation2 == "LR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location2 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation2 == "LH" || operation2 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location2 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense2.Count > 0)
							{
								for (int u = 0; u < expense2.Count; u++)
								{
									string expC = expense2[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[u].ExpenseC,
										ExpenseN = expense2[u].ExpenseN
									});
								}
							}
						}

					}
					else
					{
						listExpense = null;
					}
				}
				if ((operation1 == "LR" || operation1 == "TR" || operation1 == "XH" || operation1 == "LH") &&
				    (operation3 == "TR" || operation3 == "LR" || operation3 == "XH" || operation3 == "LH") &&
				    (string.IsNullOrEmpty(operation2) || operation2 == "0"))
				{
					if (liftuplowerParam == "1" && internalParam == "0")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location3) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "5" || expense.CategoryExpense == "0")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = operation1 == "LR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location1 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "1")
								.FirstOrDefault()
							: (operation1 == "TR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location1 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "2")
									.FirstOrDefault()
								: null);
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense1.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense1.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
								SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
								SupplierN = loca1 != null ? loca1.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense1.ExpenseC,
								ExpenseN = expense1.ExpenseN
							});
						}
						var expense3 = operation3 == "TR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location3 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "2")
								.FirstOrDefault()
							: (operation3 == "LR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location3 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "1")
									.FirstOrDefault()
								: ((operation3 == "LH" || operation3 == "XH") && internalParam == "1"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location3 && p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
											p.CategoryExpense == "5")
										.FirstOrDefault()
									: null));
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense3.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense3.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
								SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
								SupplierN = loca3 != null ? loca3.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense3.ExpenseC,
								ExpenseN = expense3.ExpenseN
							});
						}
					}
					else if (liftuplowerParam == "0" && internalParam == "1")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location3) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "1" || expense.CategoryExpense == "2" || expense.CategoryExpense == "3" ||
									    expense.CategoryExpense == "4")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}
						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}
					}
					else if (liftuplowerParam == "1" && internalParam == "1")
					{
						var expense1 = operation1 == "LR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location1 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation1 == "TR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location1 && ((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation1 == "LH" || operation1 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location1 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}

						var expense3 = operation3 == "TR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location3 && ((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation3 == "LR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location3 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation3 == "LH" || operation3 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location3 && p.CategoryExpense == "5" &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}

					}
					else
					{
						listExpense = null;
					}
				}
				if ((operation2 == "LR" || operation2 == "TR" || operation2 == "XH" || operation2 == "LH") &&
				    (operation3 == "TR" || operation3 == "LR" || operation3 == "XH" || operation3 == "LH") &&
				    (string.IsNullOrEmpty(operation1) || operation1 == "0"))
				{
					if (liftuplowerParam == "1" && internalParam == "0")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location3 || p.LocationC == location2) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "5" || expense.CategoryExpense == "0")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense2 = operation2 == "LR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location2 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "1")
								.FirstOrDefault()
							: (operation2 == "TR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location2 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "2")
									.FirstOrDefault()
								: null);
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense2.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense2.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
								SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
								SupplierN = loca2 != null ? loca2.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense2.ExpenseC,
								ExpenseN = expense2.ExpenseN
							});
						}
						var expense3 = operation3 == "TR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location3 && p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
									p.CategoryExpense == "2")
								.FirstOrDefault()
							: (operation3 == "LR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location3 && p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
										p.CategoryExpense == "1")
									.FirstOrDefault()
								: ((operation3 == "LH" || operation3 == "XH") && internalParam == "1"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location3 && p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
											p.CategoryExpense == "5")
										.FirstOrDefault()
									: null));
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense3.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense3.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
								SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
								SupplierN = loca3 != null ? loca3.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense3.ExpenseC,
								ExpenseN = expense3.ExpenseN
							});
						}
					}
					else if (liftuplowerParam == "0" && internalParam == "1")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location3 || p.LocationC == location2) && p.ExpenseC == exC).FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "1" || expense.CategoryExpense == "2" || expense.CategoryExpense == "3" ||
									    expense.CategoryExpense == "4")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense2 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location2 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int i = 0; i < expense2.Count; i++)
								{
									string expC = expense2[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[i].ExpenseC,
										ExpenseN = expense2[i].ExpenseN
									});
								}
							}
						}
						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}
					}
					else if (liftuplowerParam == "1" && internalParam == "1")
					{
						var expense2 = operation2 == "LR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location2 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation2 == "TR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location2 && ((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation2 == "LH" || operation2 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location2 &&
											((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int i = 0; i < expense2.Count; i++)
								{
									string expC = expense2[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[i].ExpenseC,
										ExpenseN = expense2[i].ExpenseN
									});
								}
							}
						}

						var expense3 = operation3 == "TR"
							? _locationDetailRepository.Query(
								p =>
									p.LocationC == location3 && ((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
									p.Display == "1" &&
									(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
									 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
									  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
									 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
									  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
								.ToList()
							: (operation3 == "LR"
								? _locationDetailRepository.Query(
									p =>
										p.LocationC == location3 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
										p.Display == "1" &&
										(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
										 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
										  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
										 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
										  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
									.ToList()
								: (operation3 == "LH" || operation3 == "XH"
									? _locationDetailRepository.Query(
										p =>
											p.LocationC == location3 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
											p.Display == "1" &&
											(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
											 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
											  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
											 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
											  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
										.ToList()
									: null));
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}

					}
					else
					{
						listExpense = null;
					}
				}

				if (operation1 == "LR" && (operation2 == "LH" || operation2 == "XH") && operation3 == "TR")
				{
					if (liftuplowerParam == "1" && internalParam == "0")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location2 || p.LocationC == location3) && p.ExpenseC == exC)
										.FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "5" || expense.CategoryExpense == "0")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
								p.CategoryExpense == "1")
							.FirstOrDefault();
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense1.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense1.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
								SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
								SupplierN = loca1 != null ? loca1.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense1.ExpenseC,
								ExpenseN = expense1.ExpenseN
							});
						}
						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 string.IsNullOrEmpty(containersizeI) || containersizeI == "") &&
								p.CategoryExpense == "2")
							.FirstOrDefault();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							var exM = _expenseRepository.Query(p => p.ExpenseC == expense3.ExpenseC).FirstOrDefault();
							listExpense.Add(new ExpenseDetailViewModel()
							{
								OrderD = orderD,
								OrderNo = orderNo,
								DetailNo = detailNo,
								DispatchNo = dispatchNo,
								UnitPrice = expense3.AmountMoney,
								Quantity = 1,
								SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
								SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
								SupplierN = loca3 != null ? loca3.SupplierN : "",
								IsIncluded = orderH.IsCollected,
								IsRequested = exM != null ? exM.IsRequested : "",
								IsPayable = exM != null ? exM.IsPayable : "",
								ExpenseC = expense3.ExpenseC,
								ExpenseN = expense3.ExpenseN
							});
						}
					}
					else if (liftuplowerParam == "0" && internalParam == "1")
					{
						if (listExpense != null)
						{
							for (int le = 0; le < listExpense.Count; le++)
							{
								string exC = listExpense[le].ExpenseC;
								var expense =
									_locationDetailRepository.Query(
										p => (p.LocationC == location1 || p.LocationC == location2 || p.LocationC == location3) && p.ExpenseC == exC)
										.FirstOrDefault();
								if (expense != null)
								{
									if (expense.CategoryExpense == "1" || expense.CategoryExpense == "2" || expense.CategoryExpense == "3" ||
									    expense.CategoryExpense == "4")
									{
										listExpense.RemoveAt(le);
										le--;
									}
								}
							}
						}
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}

						var expense2 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location2 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int i = 0; i < expense2.Count; i++)
								{
									string expC = expense2[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[i].ExpenseC,
										ExpenseN = expense2[i].ExpenseN
									});
								}
							}
						}

						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int i = 0; i < expense3.Count; i++)
								{
									string expC = expense3[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[i].ExpenseC,
										ExpenseN = expense3[i].ExpenseN
									});
								}
							}
						}
					}
					else if (liftuplowerParam == "1" && internalParam == "1")
					{
						var expense1 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location1 && ((p.CategoryExpense == "1") || (p.CategoryExpense == "5")) &&
								p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
							.ToList();
						var loca1 = _locationRepository.Query(p => p.LocationC == location1).FirstOrDefault();
						if (expense1 != null)
						{
							if (expense1.Count > 0)
							{
								for (int i = 0; i < expense1.Count; i++)
								{
									string expC = expense1[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense1[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca1 != null ? loca1.SupplierMainC : "",
										SupplierSubC = loca1 != null ? loca1.SupplierSubC : "",
										SupplierN = loca1 != null ? loca1.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense1[i].ExpenseC,
										ExpenseN = expense1[i].ExpenseN
									});
								}
							}
						}
						var expense2 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location2 && p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")) && p.CategoryExpense == "5")
							.ToList();
						var loca2 = _locationRepository.Query(p => p.LocationC == location2).FirstOrDefault();
						if (expense2 != null)
						{
							if (expense2.Count > 0)
							{
								for (int i = 0; i < expense2.Count; i++)
								{
									string expC = expense2[i].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense2[i].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca2 != null ? loca2.SupplierMainC : "",
										SupplierSubC = loca2 != null ? loca2.SupplierSubC : "",
										SupplierN = loca2 != null ? loca2.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense2[i].ExpenseC,
										ExpenseN = expense2[i].ExpenseN
									});
								}
							}
						}
						var expense3 = _locationDetailRepository.Query(
							p =>
								p.LocationC == location3 && ((p.CategoryExpense == "2") || (p.CategoryExpense == "5")) &&
								p.Display == "1" &&
								(((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") && p.ContainerSizeI == containersizeI) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (string.IsNullOrEmpty(containersizeI) || containersizeI == "")) ||
								 ((string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI == "") &&
								  (!string.IsNullOrEmpty(containersizeI) || containersizeI != "")) ||
								 ((!string.IsNullOrEmpty(p.ContainerSizeI) || p.ContainerSizeI != "") &&
								  string.IsNullOrEmpty(containersizeI) || containersizeI == "")))
							.ToList();
						var loca3 = _locationRepository.Query(p => p.LocationC == location3).FirstOrDefault();
						if (expense3 != null)
						{
							if (expense3.Count > 0)
							{
								for (int u = 0; u < expense3.Count; u++)
								{
									string expC = expense3[u].ExpenseC;
									var exM = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									listExpense.Add(new ExpenseDetailViewModel()
									{
										OrderD = orderD,
										OrderNo = orderNo,
										DetailNo = detailNo,
										DispatchNo = dispatchNo,
										UnitPrice = expense3[u].AmountMoney,
										Quantity = 1,
										SupplierMainC = loca3 != null ? loca3.SupplierMainC : "",
										SupplierSubC = loca3 != null ? loca3.SupplierSubC : "",
										SupplierN = loca3 != null ? loca3.SupplierN : "",
										IsIncluded = orderH.IsCollected,
										IsRequested = exM != null ? exM.IsRequested : "",
										IsPayable = exM != null ? exM.IsPayable : "",
										ExpenseC = expense3[u].ExpenseC,
										ExpenseN = expense3[u].ExpenseN
									});
								}
							}
						}

					}
					else
					{
						listExpense = null;
					}
				}
			}
			if (listExpense != null && listExpense.Count >= 1)
			{
				List<ExpenseDetailViewModel> result = listExpense.OrderByDescending(p => p.ExpenseC).ToList();
				string exp = result[0].ExpenseC;
				int flag = 0;
				for (int i = 0; i < result.Count; i++)
				{
					if ((i + 1) < result.Count)
					{
						if (exp == result[i].ExpenseC)
						{
							if (result[i + 1].ExpenseC == exp && flag == 0)
							{
								//result[i].Quantity++;
								result[i].UnitPrice = result[i + 1].UnitPrice;
								//result[i].Amount = result[i].UnitPrice != null
								//	? (result[i].UnitPrice*result[i].Quantity)
								//	: result[i + 1].UnitPrice;
								flag++;
							}
							else if (result[i + 1].ExpenseC == exp && flag > 0)
							{
								//result[i - 1].Quantity++;
								//result[i - 1].Amount = result[i - 1].UnitPrice*result[i - 1].Quantity;
								result.RemoveAt(i);
								i--;
								flag++;
							}
							else if (result[i + 1].ExpenseC != exp && flag > 1)
							{
								exp = result[i + 1].ExpenseC;
								result.RemoveAt(i);
								i--;
								flag = 0;
							}
							else if (result[i + 1].ExpenseC != exp && flag > 0)
							{
								exp = result[i + 1].ExpenseC;
								result.RemoveAt(i);
								i--;
								flag = 0;
							}
							else
							{
								exp = result[i + 1].ExpenseC;
							}
						}
					}
					else
					{
						if (exp == result[i].ExpenseC)
						{
							if (flag > 0)
							{
								result.RemoveAt(i);
							}
						}
					}
				}
				return result;
			}
			else return null;
		}
	}
}
