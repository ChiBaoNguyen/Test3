using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.FixedExpense;

namespace Service.Services
{
	public interface IFixedExpenseService
	{
		List<FixedExpenseViewModel> GetFixedExpense(string depC, int year, string expenseC);
		void UpdateData(FixedExpenseData data);
		void SaveFixedExpense();
	}
	public class FixedExpenseService : IFixedExpenseService
	{
		private readonly IFixedExpenseRepository _fixedExpenseRepository;
		private readonly IDepartmentRepository _departmentService;
		private readonly ITruckRepository _truckRepository;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly IExpenseRepository _expenseRepository;
		private readonly IUnitOfWork _unitOfWork;

		public FixedExpenseService(IFixedExpenseRepository fixedExpenseRepository,
								   IDepartmentRepository departmentService,
								   ITruckRepository truckRepository,
								   IEmployeeRepository employeeRepository,
								   IExpenseRepository expenseRepository,
								   IUnitOfWork unitOfWork)
		{
			this._fixedExpenseRepository = fixedExpenseRepository;
			this._departmentService = departmentService;
			this._truckRepository = truckRepository;
			this._employeeRepository = employeeRepository;
			this._expenseRepository = expenseRepository;
			this._unitOfWork = unitOfWork;
		}

		public List<FixedExpenseViewModel> GetFixedExpense(string depC,
															int year,
															string expenseC)
		{
			var fixedExpense = from a in _fixedExpenseRepository.GetAllQueryable()
				join b in _departmentService.GetAllQueryable() on a.DepC
					equals b.DepC into t1
				from b in t1.DefaultIfEmpty()
				join c in _truckRepository.GetAllQueryable() on a.TruckC
					equals c.TruckC into t2
				from c in t2.DefaultIfEmpty()
				join d in _employeeRepository.GetAllQueryable() on a.EntryClerkC
					equals d.EmployeeC into t3
				from d in t3.DefaultIfEmpty()
				join e in _expenseRepository.GetAllQueryable() on a.ExpenseC
					equals e.ExpenseC into t4
				from e in t4.DefaultIfEmpty()
				where (a.DepC == depC &
				       a.Year == year &
				       a.ExpenseC == expenseC
					)
				select new FixedExpenseViewModel()
				{
					DepC = a.DepC,
					DepN = b != null ? b.DepN : "",
					Year = a.Year,
					ExpenseC = a.ExpenseC,
					ExpenseN = e != null ? e.ExpenseN : "",
					TruckC = a.TruckC,
					RegisteredNo = c != null ? c.RegisteredNo : "",
					AcquisitionD = c != null ? c.AcquisitionD : null,
					RegisteredD = c != null ? c.RegisteredD : null,
					DisusedD = c != null ? c.DisusedD : null,
					EntryClerkC = a.EntryClerkC,
					EntryClerkN = d != null ? d.EmployeeLastN + " " + d.EmployeeFirstN : "",
					RetiredD = d != null ? d.RetiredD : null,
					Total = a.Total,
					Month1 = a.Month1,
					Month2 = a.Month2,
					Month3 = a.Month3,
					Month4 = a.Month4,
					Month5 = a.Month5,
					Month6 = a.Month6,
					Month7 = a.Month7,
					Month8 = a.Month8,
					Month9 = a.Month9,
					Month10 = a.Month10,
					Month11 = a.Month11,
					Month12 = a.Month12
				};
			fixedExpense = fixedExpense.OrderBy("RegisteredNo asc");
			var fixedExpenseList = fixedExpense.ToList();

			var truck = _truckRepository.Query(tru => tru.DepC == depC &&
													  tru.PartnerI == "0" &&
					  								  tru.IsActive == Constants.ACTIVE);
			var truckList = truck.OrderBy("RegisteredNo asc").ToList();
			if (truckList.Count > 0)
			{
				for (var iloop = 0; iloop < truckList.Count; iloop++)
				{
					var index = fixedExpenseList.FindIndex(f => f.TruckC == truckList[iloop].TruckC);
					if (index < 0)
					{
						var item = new FixedExpenseViewModel();
						item.TruckC = truckList[iloop].TruckC;
						item.DepC = truckList[iloop].DepC;
						item.RegisteredNo = truckList[iloop].RegisteredNo;
						item.RegisteredD = truckList[iloop].RegisteredD;
						item.AcquisitionD = truckList[iloop].AcquisitionD;
						item.DisusedD = truckList[iloop].DisusedD;
						item.Total = 0;
						item.Month1 = 0;
						item.Month2 = 0;
						item.Month3 = 0;
						item.Month4 = 0;
						item.Month5 = 0;
						item.Month6 = 0;
						item.Month7 = 0;
						item.Month8 = 0;
						item.Month9 = 0;
						item.Month10 = 0;
						item.Month11 = 0;
						item.Month12 = 0;

						fixedExpenseList.Add(item);
					}
				}
			}

			// sort fixedExpenseList by RegisteredNo
			fixedExpenseList = fixedExpenseList.OrderBy("RegisteredNo asc").ToList();

			var result = SetIsDisabledMonth(fixedExpenseList, year);

			return result;
		}

		public List<FixedExpenseViewModel> SetIsDisabledMonth(List<FixedExpenseViewModel> data, int currentYear)
		{
			if (data != null && data.Count > 0)
			{
				for (var iloop = 0; iloop < data.Count; iloop++)
				{
					if (data[iloop].AcquisitionD == null)
					{
						data[iloop].IsDisableMonth1 = false;
						data[iloop].IsDisableMonth2 = false;
						data[iloop].IsDisableMonth3 = false;
						data[iloop].IsDisableMonth4 = false;
						data[iloop].IsDisableMonth5 = false;
						data[iloop].IsDisableMonth6 = false;
						data[iloop].IsDisableMonth7 = false;
						data[iloop].IsDisableMonth8 = false;
						data[iloop].IsDisableMonth9 = false;
						data[iloop].IsDisableMonth10 = false;
						data[iloop].IsDisableMonth11 = false;
						data[iloop].IsDisableMonth12 = false;
					}
					else
					{
						var registeredYear = ((DateTime)data[iloop].AcquisitionD).Year;
						var registeredMonth = ((DateTime)data[iloop].AcquisitionD).Month;
						if (currentYear > registeredYear)
						{
							data[iloop].IsDisableMonth1 = true;
							data[iloop].IsDisableMonth2 = true;
							data[iloop].IsDisableMonth3 = true;
							data[iloop].IsDisableMonth4 = true;
							data[iloop].IsDisableMonth5 = true;
							data[iloop].IsDisableMonth6 = true;
							data[iloop].IsDisableMonth7 = true;
							data[iloop].IsDisableMonth8 = true;
							data[iloop].IsDisableMonth9 = true;
							data[iloop].IsDisableMonth10 = true;
							data[iloop].IsDisableMonth11 = true;
							data[iloop].IsDisableMonth12 = true;
						}
						else if (currentYear == registeredYear)
						{
							data[iloop].IsDisableMonth1 = true;
							data[iloop].IsDisableMonth2 = true;
							data[iloop].IsDisableMonth3 = true;
							data[iloop].IsDisableMonth4 = true;
							data[iloop].IsDisableMonth5 = true;
							data[iloop].IsDisableMonth6 = true;
							data[iloop].IsDisableMonth7 = true;
							data[iloop].IsDisableMonth8 = true;
							data[iloop].IsDisableMonth9 = true;
							data[iloop].IsDisableMonth10 = true;
							data[iloop].IsDisableMonth11 = true;
							data[iloop].IsDisableMonth12 = true;

							if (registeredMonth > 1)
							{
								data[iloop].IsDisableMonth1 = false;
							}
							if (registeredMonth > 2)
							{
								data[iloop].IsDisableMonth2 = false;
							}
							if (registeredMonth > 3)
							{
								data[iloop].IsDisableMonth3 = false;
							}
							if (registeredMonth > 4)
							{
								data[iloop].IsDisableMonth4 = false;
							}
							if (registeredMonth > 5)
							{
								data[iloop].IsDisableMonth5 = false;
							}
							if (registeredMonth > 6)
							{
								data[iloop].IsDisableMonth6 = false;
							}
							if (registeredMonth > 7)
							{
								data[iloop].IsDisableMonth7 = false;
							}
							if (registeredMonth > 8)
							{
								data[iloop].IsDisableMonth8 = false;
							}
							if (registeredMonth > 9)
							{
								data[iloop].IsDisableMonth9 = false;
							}
							if (registeredMonth > 10)
							{
								data[iloop].IsDisableMonth10 = false;
							}
							if (registeredMonth > 11)
							{
								data[iloop].IsDisableMonth11 = false;
							}
							if (registeredMonth > 12)
							{
								data[iloop].IsDisableMonth12 = false;
							}
						}
						else
						{
							data[iloop].IsDisableMonth1 = false;
							data[iloop].IsDisableMonth2 = false;
							data[iloop].IsDisableMonth3 = false;
							data[iloop].IsDisableMonth4 = false;
							data[iloop].IsDisableMonth5 = false;
							data[iloop].IsDisableMonth6 = false;
							data[iloop].IsDisableMonth7 = false;
							data[iloop].IsDisableMonth8 = false;
							data[iloop].IsDisableMonth9 = false;
							data[iloop].IsDisableMonth10 = false;
							data[iloop].IsDisableMonth11 = false;
							data[iloop].IsDisableMonth12 = false;
						}
					}
				}
			}

			return data;
		}

		public void UpdateData(FixedExpenseData data)
		{
			var depC = data.DepC;
			var year = data.Year;
			var expenseC = data.ExpenseC;
			var entryClerkC = data.EntryClerkC;

			var fixedExpenseToDelete = _fixedExpenseRepository.Query(f => f.DepC == depC &&
																		f.Year == year &&
																		f.ExpenseC == expenseC
																	);
			if (fixedExpenseToDelete.Any())
			{
				var fixedExpenseToDeleteList = fixedExpenseToDelete.ToList();
				for (var iloop = 0; iloop < fixedExpenseToDeleteList.Count; iloop++)
				{
					_fixedExpenseRepository.Delete(fixedExpenseToDeleteList[iloop]);
				}
				
				SaveFixedExpense();
			}

			if (data.FixedExpenseList != null && data.FixedExpenseList.Count > 0)
			{
				for (var iloop = 0; iloop < data.FixedExpenseList.Count; iloop++)
				{
					data.FixedExpenseList[iloop].DepC = depC;
					data.FixedExpenseList[iloop].Year = year;
					data.FixedExpenseList[iloop].ExpenseC = expenseC;
					data.FixedExpenseList[iloop].EntryClerkC = entryClerkC;

					var updateFixedExpense = Mapper.Map<FixedExpenseViewModel, FixedExpense_D>(data.FixedExpenseList[iloop]);
					_fixedExpenseRepository.Add(updateFixedExpense);
				}
				SaveFixedExpense();
			}
		}

		public void SaveFixedExpense()
		{
			_unitOfWork.Commit();
		}
	}
}
