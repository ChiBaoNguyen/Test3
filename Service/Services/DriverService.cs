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
using Website.ViewModels.Employee;
using Website.ViewModels.Driver;
using Website.ViewModels.Department;

namespace Service.Services
{
	public interface IDriverService
	{
		DriverDatatables GetDriversForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string driverSearchValue);
		IEnumerable<DriverViewModel> GetDriversForSuggestion(string value);
		IEnumerable<DriverViewModel> GetDriversAutoSuggestionForReport(string value);
		IEnumerable<DriverViewModel> GetDriversByCode(string value);
		DriverStatusViewModel GetDriverByCode(string mainCode);
		void CreateDriver(DriverViewModel driver);
		void UpdateDriver(DriverViewModel driver);
		void DeleteDriver(string id);
		void SetStatusDriver(string id);
		DriverViewModel GetByName(string value);
		string GetDepFromDriver(string driverC);
		IEnumerable<DriverViewModel> GetDrivers();
		IEnumerable<DriverViewModel> GetDriversForReport();
		string GetDriverName(string driverC);
		IEnumerable<DepartmentViewModel> GetDriverDepartments(bool isForReport);
		void SaveDriver();
	}

	public class DriverService : IDriverService
	{
		private readonly IDriverRepository _driverRepository;
		private readonly IDepartmentRepository _departmentRepository;
		private readonly IDriverLicenseRepository _driverLicenseRepository;
		private readonly IUnitOfWork _unitOfWork;

		public DriverService(IDriverRepository driverRepository, IDepartmentRepository departmentRepository, IDriverLicenseRepository driverLicenseRepository, IUnitOfWork unitOfWork)
		{
			this._departmentRepository = departmentRepository;
			this._driverRepository = driverRepository;
			this._driverLicenseRepository = driverLicenseRepository;
			this._unitOfWork = unitOfWork;
		}

		public IEnumerable<DriverViewModel> GetDriversForSuggestion(string value)
		{
			var searchDriver =
				_driverRepository.Query(p => (p.DriverC.Contains(value) ||
											 (p.LastN + " " + p.FirstN).Contains(value)) &&
											 p.IsActive == Constants.ACTIVE);
                                             
			var destination = Mapper.Map<IEnumerable<Driver_M>, IEnumerable<DriverViewModel>>(searchDriver);
			return destination;
		}

		public IEnumerable<DriverViewModel> GetDriversAutoSuggestionForReport(string value)
		{
			var searchDriver =
				_driverRepository.Query(p => (p.DriverC.Contains(value) ||
											 (p.LastN + " " + p.FirstN).Contains(value)));

			var destination = Mapper.Map<IEnumerable<Driver_M>, IEnumerable<DriverViewModel>>(searchDriver);
			return destination;
		}

		public IEnumerable<DriverViewModel> GetDriversByCode(string value)
		{
			var searchDriver =
				_driverRepository.Query(p => p.DriverC.StartsWith(value));

			var destination = Mapper.Map<IEnumerable<Driver_M>, IEnumerable<DriverViewModel>>(searchDriver);
			return destination;
		}

		public DriverStatusViewModel GetDriverByCode(string driverCode)
		{
			var driverStatus = new DriverStatusViewModel();
			var driver = _driverRepository.Query(cus => cus.DriverC == driverCode).FirstOrDefault();
			if (driver != null)
			{
				var driverViewModel = Mapper.Map<Driver_M, DriverViewModel>(driver);
				driverViewModel.DriverIndex = FindIndex(driverCode);
				driverStatus.Driver = driverViewModel;
				driverStatus.Status = CustomerStatus.Edit.ToString();
			}
			else
			{
				driverStatus.Status = CustomerStatus.Add.ToString();
			}
			return driverStatus;
		}

		public DriverDatatables GetDriversForTable(int page, int itemsPerPage, string sortBy, bool reverse, string driverSearchValue)
		{
			var drivers = _driverRepository.GetAllQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(driverSearchValue))
			{
				driverSearchValue = driverSearchValue.ToLower();
				drivers = drivers.Where(cus => cus.FirstN.ToLower().Contains(driverSearchValue) ||
										cus.LastN.ToLower().Contains(driverSearchValue)			||
										cus.DriverC.ToLower().Contains(driverSearchValue)
										);
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var driversOrdered = drivers.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var driversPaged = driversOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var destination = Mapper.Map<List<Driver_M>, List<DriverViewModel>>(driversPaged);

			foreach (var item in destination)
			{
				var dep = _departmentRepository.Get(d => d.DepC == item.DepC);
				if (dep != null)
				{
					item.DepN = dep.DepN;
				}
			}

			var driverDatatable = new DriverDatatables()
			{
				Data = destination,
				Total = drivers.Count()
			};
			return driverDatatable;
		}

		public void CreateDriver(DriverViewModel driverViewModel)
		{
			var driver = Mapper.Map<DriverViewModel, Driver_M>(driverViewModel);
			_driverRepository.Add(driver);
			SaveDriver();
		}

		public void UpdateDriver(DriverViewModel driver)
		{
			var driverToRemove = _driverRepository.GetById(driver.DriverC);
			var updateDriver = Mapper.Map<DriverViewModel, Driver_M>(driver);
			_driverRepository.Delete(driverToRemove);
			_driverRepository.Add(updateDriver);
			SaveDriver();
		}

		//using for active and deactive user
		public void SetStatusDriver(string id)
		{
			var driverToRemove = _driverRepository.Get(c => c.DriverC == id);
			if (driverToRemove.IsActive == Constants.ACTIVE)
			{
				driverToRemove.IsActive = Constants.DEACTIVE;
			}
			else
			{
				driverToRemove.IsActive = Constants.ACTIVE;
			}
			_driverRepository.Update(driverToRemove);
			SaveDriver();
		}
		public void DeleteDriver(string id)
		{
			var driverToRemove = _driverRepository.Get(c => c.DriverC == id);
			if (driverToRemove != null)
			{
				_driverRepository.Delete(driverToRemove);
				
				var deleteItem = _driverLicenseRepository.Query(x => x.DriverC == id);
				if (deleteItem.Any())
				{
					foreach (DriverLicense_M item in deleteItem)
					{
						_driverLicenseRepository.Delete(item);
					}	
				}
				
				SaveDriver();
			}
		}

		//public EmployeeViewModel GetByName(string value)
		//{
		//	var emp = _employeeRepository.Query(e => e.EmployeeFirstN.Equals(value) || e.EmployeeLastN.Equals(value)).FirstOrDefault();
		//	if (emp != null)
		//	{
		//		var destination = Mapper.Map<Employee_M, EmployeeViewModel>(emp);
		//		return destination;
		//	}
		//	return null;
		//}

		public DriverViewModel GetByName(string value)
		{
			var driver = _driverRepository.Query(s => (s.LastN + " " + s.FirstN == value)).FirstOrDefault();
			if (driver != null)
			{
				var destination = Mapper.Map<Driver_M, DriverViewModel>(driver);
				return destination;
			}
			return null;
		}

		public IEnumerable<DriverViewModel> GetDrivers()
		{
			var source = _driverRepository.Query(i => i.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<Driver_M>, IEnumerable<DriverViewModel>>(source);
			return destination;
		}

		public IEnumerable<DriverViewModel> GetDriversForReport()
		{
			var source = _driverRepository.GetAllQueryable();
			var destination = Mapper.Map<IEnumerable<Driver_M>, IEnumerable<DriverViewModel>>(source);
			return destination;
		}

		public string GetDriverName(string driverC)
		{
			var driver = _driverRepository.Query(cus => cus.DriverC == driverC).FirstOrDefault();
			if (driver != null)
			{
				return driver.LastN + " " + driver.FirstN;
			}
			
			return "";
		}

		private int FindIndex(string code)
		{
			var data = _driverRepository.GetAllQueryable();
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

						if (data.OrderBy("DriverC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.DriverC == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("DriverC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.DriverC == code)
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
		public IEnumerable<DepartmentViewModel> GetDriverDepartments(bool isForReport)
		{
			var result = new List<DepartmentViewModel>();
			var department = (from d in _driverRepository.GetAllQueryable()
							 where (isForReport || d.IsActive == "1") &&
									d.DepC != "0"
							 group d by d.DepC into g
							 select new DepartmentViewModel{
								 DepC = g.Key
							 }).ToList();
			if (department != null && department.Count > 0)
			{
				for (int i = 0; i < department.Count; i++)
				{
					var depC = department[i].DepC;
					var dep = _departmentRepository.Get(d => d.DepC == depC);
					result.Add(Mapper.Map<Department_M, DepartmentViewModel>(dep));
				}
			}
			return result;
		}

		public string GetDepFromDriver(string driverC)
		{
			var driver = _driverRepository.Query(p => p.DriverC == driverC).FirstOrDefault();
			return (driver.DepC ?? "0");
		}

		public void SaveDriver()
		{
			_unitOfWork.Commit();
		}
	}
}
