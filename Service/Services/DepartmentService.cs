using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.Customer;
using Website.ViewModels.Department;
using Website.Enum;

namespace Service.Services
{
    public interface IDepartmentService
    {
        IEnumerable<DepartmentViewModel> GetDepartments();
        IEnumerable<DepartmentViewModel> GetDepartmentsByCode(string value);
	    IEnumerable<DepartmentViewModel> GetDepartments(string value);
		IEnumerable<DepartmentViewModel> GetDepartmentsForReport();
        DepartmentStatusViewModel GetDepartmentByCode(string mainCode);
	    DepartmentViewModel GetByName(string depN);
        DepartmentDatatables GetDepartmentsForTable(int page, int itemsPerPage, string sortBy, bool reverse,
             string custSearchValue);
        void CreateDepartment(DepartmentViewModel department);
        void UpdateDepartment(DepartmentViewModel department);
        void DeleteDepartment(string id);
        void SetStatusDepartment(string id);
        void SaveDepartment();
    }

    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
        {
            this._departmentRepository = departmentRepository;
            this._unitOfWork = unitOfWork;
        }

        #region IDepartmentService members
        public IEnumerable<DepartmentViewModel> GetDepartments()
        {
	        var source = _departmentRepository.Query(i => i.IsActive == Constants.ACTIVE);
            var destination = Mapper.Map<IEnumerable<Department_M>, IEnumerable<DepartmentViewModel>>(source);
            return destination;
        }

        public IEnumerable<DepartmentViewModel> GetDepartments(string value)
        {
            var department = _departmentRepository.Query(i => (i.DepC.Contains(value) || i.DepN.Contains(value)) &&
                                                                i.IsActive == Constants.ACTIVE);
            var destination = Mapper.Map<IEnumerable<Department_M>, IEnumerable<DepartmentViewModel>>(department);
            return destination;
        }

		public IEnumerable<DepartmentViewModel> GetDepartmentsForReport()
		{
			var department = _departmentRepository.GetAllQueryable();
			var destination = Mapper.Map<IEnumerable<Department_M>, IEnumerable<DepartmentViewModel>>(department);
			return destination;
		}

		public IEnumerable<DepartmentViewModel> GetDepartmentsByCode(string value)
		{
			var department = _departmentRepository.Query(i => i.DepC.StartsWith(value));
			var destination = Mapper.Map<IEnumerable<Department_M>, IEnumerable<DepartmentViewModel>>(department);
			return destination;
		}

        public DepartmentStatusViewModel GetDepartmentByCode(string code)
        {
            var departmentStatus = new DepartmentStatusViewModel();
            var department = _departmentRepository.Query(cus => cus.DepC == code).FirstOrDefault();
            if (department != null)
            {
                var departmentViewModel = Mapper.Map<Department_M, DepartmentViewModel>(department);
	            departmentViewModel.DepIndex = FindIndex(code);
                departmentStatus.Department = departmentViewModel;
                departmentStatus.Status = CustomerStatus.Edit.ToString();
            }
            else
            {
                departmentStatus.Status = CustomerStatus.Add.ToString();
            }
            return departmentStatus;
        }

        public DepartmentDatatables GetDepartmentsForTable(int page, int itemsPerPage, string sortBy, bool reverse, string custSearchValue)
        {
            var departments = _departmentRepository.GetAllQueryable();
            // searching
            if (!string.IsNullOrWhiteSpace(custSearchValue))
            {
                custSearchValue = custSearchValue.ToLower();
                departments = departments.Where(cus => cus.DepN.ToLower().Contains(custSearchValue)
                                                                || cus.DepC.ToLower().Contains(custSearchValue)
                                                                );
            }

            // sorting (done with the System.Linq.Dynamic library available on NuGet)
            //departments = departments.AsQueryable().OrderBy("\"" + sortBy + (reverse ? " desc\"" : " asc\"")).AsEnumerable();
            var departmentsOrdered = departments.OrderBy(sortBy + (reverse ? " descending" : ""));

            // paging
            var departmentsPaged = departmentsOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            var destination = Mapper.Map<List<Department_M>, List<DepartmentViewModel>>(departmentsPaged);
            var custDatatable = new DepartmentDatatables()
            {
                Data = destination,
                Total = departments.Count()
            };
            return custDatatable;
        }

        public void CreateDepartment(DepartmentViewModel departmentViewModel)
        {
            var department = Mapper.Map<DepartmentViewModel, Department_M>(departmentViewModel);
            _departmentRepository.Add(department);
            SaveDepartment();
        }

        public void UpdateDepartment(DepartmentViewModel department)
        {
            var departmentToRemove = _departmentRepository.GetById(department.DepC);
            var updateCustomer = Mapper.Map<DepartmentViewModel, Department_M>(department);
            _departmentRepository.Delete(departmentToRemove);
            _departmentRepository.Add(updateCustomer);
            SaveDepartment();
        }

        //using for active and deactive department
        public void SetStatusDepartment(string id)
        {
            var departmentToRemove = _departmentRepository.Get(c => c.DepC == id);
            if (departmentToRemove.IsActive == Constants.ACTIVE)
            {
                departmentToRemove.IsActive = Constants.DEACTIVE;
            }
            else
            {
                departmentToRemove.IsActive = Constants.ACTIVE;
            }
            _departmentRepository.Update(departmentToRemove);
            SaveDepartment();
        }
        public void DeleteDepartment(string id)
        {
            //var departmentToRemove = _departmentRepository.GetById(id);
            var departmentToRemove = _departmentRepository.Get(c => c.DepC == id);
            if (departmentToRemove != null)
            {
                _departmentRepository.Delete(departmentToRemove);
                SaveDepartment();
            }

        }

	    public DepartmentViewModel GetByName(string depN)
	    {
			var dep = _departmentRepository.Query(e => e.DepN == depN).FirstOrDefault();
			if (dep != null)
			{
				var destination = Mapper.Map<Department_M, DepartmentViewModel>(dep);
				return destination;
			}
			return null;
	    }

		private int FindIndex(string code)
		{
			var data = _departmentRepository.GetAllQueryable();
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

						if (data.OrderBy("DepC descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.DepC == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("DepC descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.DepC == code)
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

	    public void SaveDepartment()
        {
            _unitOfWork.Commit();
        }
        #endregion
    }
}
