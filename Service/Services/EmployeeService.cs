using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.Customer;
using Website.ViewModels.Department;
using Website.ViewModels.Employee;

namespace Service.Services
{
	public interface IEmployeeService
	{
		IEnumerable<EmployeeViewModel> GetEmployees();
		IEnumerable<EmployeeViewModel> GetEmployeesByCode(string value);
		IEnumerable<EmployeeViewModel> GetEmployees(string value);
		IEnumerable<EmployeeViewModel> GetEmployeesAutosuggestForReport(string value);
		IEnumerable<EmployeeViewModel> GetEmployeesForReport();
		EmployeeStatusViewModel GetEmployeeByCode(string mainCode);
		DepartmentViewModel GetEmployeeDepartment(string employeeC);
		EmployeeDatatables GetEmployeesForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string custSearchValue);
		void CreateEmployee(EmployeeViewModel employee);
		void UpdateEmployee(EmployeeViewModel employee);
		void DeleteEmployee(string id);
		void SetStatusEmployee(string id);
		EmployeeViewModel GetByName(string value);
		void SaveEmployee();
	}

	public class EmployeeService : IEmployeeService
	{
		private readonly IEmployeeRepository _employeeRepository;
		private readonly IDepartmentRepository _departmentRepository;
		private readonly IUnitOfWork _unitOfWork;

		public EmployeeService(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
		{
			this._departmentRepository = departmentRepository;
			this._employeeRepository = employeeRepository;
			this._unitOfWork = unitOfWork;
		}

		#region IEmployeeService members
		public IEnumerable<EmployeeViewModel> GetEmployees()
		{
            var source = _employeeRepository.Query(i => i.IsActive == Constants.ACTIVE);
			var destination = Mapper.Map<IEnumerable<Employee_M>, IEnumerable<EmployeeViewModel>>(source);
			return destination;
		}

		public IEnumerable<EmployeeViewModel> GetEmployees(string value)
		{
			//var searchEmployee = _employeeRepository.Query(p => (p.EmployeeC.Contains(value) || 
			//												(p.EmployeeLastN + " " + p.EmployeeFirstN).Contains(value)) &&
			//												p.IsActive == Constants.ACTIVE);

			//var destination = Mapper.Map<IEnumerable<Employee_M>, IEnumerable<EmployeeViewModel>>(searchEmployee);
			//return destination;

			var employees = from e in _employeeRepository.GetAllQueryable()
							join d in _departmentRepository.GetAllQueryable() on e.DepC equals d.DepC into ed
							from d in ed.DefaultIfEmpty()
							where ((e.EmployeeC.Contains(value) || (e.EmployeeLastN + " " + e.EmployeeFirstN).Contains(value)) &&
								   (d.IsActive == null || d.IsActive == Constants.ACTIVE) && e.IsActive == Constants.ACTIVE)
							select new EmployeeViewModel()
							{
								EmployeeC = e.EmployeeC,
								EmployeeFirstN = e.EmployeeFirstN,
								EmployeeLastN = e.EmployeeLastN,
								BirthD = e.BirthD,
								Address = e.Address,
								PhoneNumber = e.PhoneNumber,
								JoinedD = e.JoinedD,
								RetiredD = e.RetiredD,
								DepC = e.DepC,
								DepN = d != null ? d.DepN : "",
							};

			return employees;
		}

		public IEnumerable<EmployeeViewModel> GetEmployeesAutosuggestForReport(string value)
		{
			var employees = from e in _employeeRepository.GetAllQueryable()
							join d in _departmentRepository.GetAllQueryable() on e.DepC equals d.DepC into ed
							from d in ed.DefaultIfEmpty()
							where (e.EmployeeC.Contains(value) || (e.EmployeeLastN + " " + e.EmployeeFirstN).Contains(value))
							select new EmployeeViewModel()
							{
								EmployeeC = e.EmployeeC,
								EmployeeFirstN = e.EmployeeFirstN,
								EmployeeLastN = e.EmployeeLastN,
								BirthD = e.BirthD,
								Address = e.Address,
								PhoneNumber = e.PhoneNumber,
								JoinedD = e.JoinedD,
								RetiredD = e.RetiredD,
								DepC = e.DepC,
								DepN = d != null ? d.DepN : "",
							};

			return employees;
		}
		public IEnumerable<EmployeeViewModel> GetEmployeesForReport()
		{
			var source = _employeeRepository.GetAllQueryable();
			var destination = Mapper.Map<IEnumerable<Employee_M>, IEnumerable<EmployeeViewModel>>(source);
			return destination;
		}
		public IEnumerable<EmployeeViewModel> GetEmployeesByCode(string value)
		{
			var searchEmployee = _employeeRepository.Query(p => p.EmployeeC.StartsWith(value));
			var destination = Mapper.Map<IEnumerable<Employee_M>, IEnumerable<EmployeeViewModel>>(searchEmployee);
			return destination;
		}

		public EmployeeStatusViewModel GetEmployeeByCode(string code)
		{
			var employeeStatus = new EmployeeStatusViewModel();
			var employee = _employeeRepository.Query(cus => cus.EmployeeC == code).FirstOrDefault();
			if (employee != null)
			{
				var employeeViewModel = Mapper.Map<Employee_M, EmployeeViewModel>(employee);
				employeeViewModel.EmployeeIndex = FindIndex(code);
				employeeStatus.Employee = employeeViewModel;
				employeeStatus.Status = CustomerStatus.Edit.ToString();
			}
			else
			{
				employeeStatus.Status = CustomerStatus.Add.ToString();
			}
			return employeeStatus;
		}

		public EmployeeDatatables GetEmployeesForTable(int page, int itemsPerPage, string sortBy, bool reverse, string custSearchValue)
		{
			var employees = _employeeRepository.GetAllQueryable();
			// searching
			if (!string.IsNullOrWhiteSpace(custSearchValue))
			{
				custSearchValue = custSearchValue.ToLower();
				employees = employees.Where(cus => cus.EmployeeFirstN.ToLower().Contains(custSearchValue) ||
												   cus.EmployeeLastN.ToLower().Contains(custSearchValue) ||
											       cus.EmployeeC.ToLower().Contains(custSearchValue)
											);
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var employeesOrdered = employees.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var employeesPaged = employeesOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var destination = Mapper.Map<List<Employee_M>, List<EmployeeViewModel>>(employeesPaged);

			foreach (var item in destination)
			{
				var dep = _departmentRepository.Get(d => d.DepC == item.DepC);
				if (dep != null)
				{
					item.DepN = dep.DepN;
				}
			}

			var custDatatable = new EmployeeDatatables()
			{
				Data = destination,
				Total = employees.Count()
			};
			return custDatatable;
		}

		public void CreateEmployee(EmployeeViewModel employeeViewModel)
		{
			var employee = Mapper.Map<EmployeeViewModel, Employee_M>(employeeViewModel);
			_employeeRepository.Add(employee);
			SaveEmployee();
		}

		public void UpdateEmployee(EmployeeViewModel employee)
		{
			var employeeToRemove = _employeeRepository.GetById(employee.EmployeeC);
			var updateCustomer = Mapper.Map<EmployeeViewModel, Employee_M>(employee);
			_employeeRepository.Delete(employeeToRemove);
			_employeeRepository.Add(updateCustomer);
			SaveEmployee();
		}

		//using for active and deactive user
		public void SetStatusEmployee(string id)
		{
			var employeeToRemove = _employeeRepository.Get(c => c.EmployeeC == id);
			if (employeeToRemove.IsActive == Constants.ACTIVE)
			{
				employeeToRemove.IsActive = Constants.DEACTIVE;
			}
			else
			{
				employeeToRemove.IsActive = Constants.ACTIVE;
			}
			_employeeRepository.Update(employeeToRemove);
			SaveEmployee();
		}
		public void DeleteEmployee(string id)
		{
			var employeeToRemove = _employeeRepository.Get(c => c.EmployeeC == id);
			if (employeeToRemove != null)
			{
				_employeeRepository.Delete(employeeToRemove);
				SaveEmployee();
			}
		}

		public EmployeeViewModel GetByName(string value)
		{
			var emp = _employeeRepository.Query(e => (e.EmployeeLastN + " " + e.EmployeeFirstN) == value).FirstOrDefault();
			if (emp != null)
			{
				var destination = Mapper.Map<Employee_M, EmployeeViewModel>(emp);
				var dep = _departmentRepository.Query(d => d.DepC == destination.DepC).FirstOrDefault();
				if (dep != null)
				{
					destination.DepN = dep.DepN;
				}
				return destination;
			}
			return null;
		}

		public DepartmentViewModel GetEmployeeDepartment(string employeeC)
		{
			var employee = _employeeRepository.Query(p => p.EmployeeC == employeeC).FirstOrDefault();
			if (employee != null)
			{
				var department = _departmentRepository.Query(d => d.DepC == employee.DepC).FirstOrDefault();
				var destination = Mapper.Map<Department_M, DepartmentViewModel>(department);
				return destination;
			}
			return null;
		}

		private int FindIndex(string code)
		{
			var data = _employeeRepository.GetAllQueryable();
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

						if (data.OrderBy("EmployeeC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.EmployeeC == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("EmployeeC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.EmployeeC == code)
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

		public void SaveEmployee()
		{
			_unitOfWork.Commit();
		}
		#endregion
	}
}
