using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using AutoMapper;
using AutoMapper.Internal;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.ViewModels.CalculateDriverAllowance;

namespace Service.Services
{
	public interface ICalculateDriverAllowanceService
	{
		CalculateDriverAllowanceDatatable GetCalculateDriverAllowancesForTable(int page, int itemsPerPage, string sortBy, bool reverse, string months, string years, string driverCs, string driverNs, string contents, bool takeabreaks);
		CalculateDriverAllowanceViewModel GetCalculateDriverAllowanceSizeByCode(string calculateC);
		void CreateCalculate(CalculateDriverAllowanceViewModel pattern);
		void UpdateCalculate(CalculateDriverAllowanceViewModel pattern);
		void DeleteCalculate(string id);
		List<CalculateDriverAllowanceViewModel> GetCalculateDriverAllowanceByCode(string value);
		string GetCodeNumber();


		void SaveCalculateDriverAllowance();
	}

	public class CalculateDriverAllowanceService : ICalculateDriverAllowanceService
	{
		private readonly ICalculateDriverAllowanceRepository _calculateDriverAllowanceRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly IContainerTypeRepository _containerTypeRepository;
		private readonly IDispatchRepository _dispatchRepository;
		private readonly IAllowanceDetailRepository _allowanceDetailRepository;
		private readonly IExpenseRepository _expenseRepository;
		private readonly IOrderRepository _orderHRepository;
		private readonly IContainerRepository _orderDRepository;
		private readonly IDepartmentRepository _departmentRepository;
		private readonly ITextResourceRepository _textResourceRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IDriverRepository _driverRepository;

		public CalculateDriverAllowanceService(ICalculateDriverAllowanceRepository calculateDriverAllowanceRepository,
			IUnitOfWork unitOfWork,
			IDispatchRepository dispatchRepository,
			ICustomerRepository customerRepository,
			ILocationRepository locationRepository,
			IContainerTypeRepository containerTypeRepository,
			IAllowanceDetailRepository allowanceDetailRepository,
			IExpenseRepository _expenseRepository,
			IContainerRepository orderDRepository,
			IOrderRepository orderHRepository,
			IDepartmentRepository departmentRepository,
			ITextResourceRepository textResourceRepository,
			IDriverRepository driverRepository)
		{
			this._calculateDriverAllowanceRepository = calculateDriverAllowanceRepository;
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
			this._driverRepository = driverRepository;
		}
		public CalculateDriverAllowanceDatatable GetCalculateDriverAllowancesForTable(int page, int itemsPerPage, string sortBy, bool reverse, string months, string years, string driverCs, string driverNs, string contents, bool takeabreaks)
		{
			if (!string.IsNullOrEmpty(months) || !string.IsNullOrEmpty(years) || !string.IsNullOrEmpty(driverCs) ||
					 !string.IsNullOrEmpty(driverNs) || !string.IsNullOrEmpty(contents))
			{
				var driver = _calculateDriverAllowanceRepository.GetAllQueryable();

				var destination = (from p in _calculateDriverAllowanceRepository.GetAllQueryable()
								   join t in _driverRepository.GetAllQueryable() on new { p.DriverC }
								   equals new { t.DriverC } into t1
								   from t in t1.DefaultIfEmpty()
								   where (p.ApplyD.Value.Month.ToString() == months || string.IsNullOrEmpty(months)) &
								   (p.ApplyD.Value.Year.ToString() == years || string.IsNullOrEmpty(years)) &
								   (p.DriverC.Contains(driverCs) || string.IsNullOrEmpty(driverCs)) &
								   (p.Content.Contains(contents) || string.IsNullOrEmpty(contents))
								   select new CalculateDriverAllowanceViewModel()
								   {
									   CalculateC = p.CalculateC,
									   ApplyD = p.ApplyD,
									   DriverC = p.DriverC,
									   Content = p.Content,
									   AmountMoney = p.AmountMoney,
									   AmountMoneySubtract = p.AmountMoneySubtract,
									   Description = p.Description,
									   TakeABreak = p.TakeABreak,
									   DriverN = t != null ? t.LastN + " " + t.FirstN : ""
								   }).ToList();
				// sorting (done with the System.Linq.Dynamic library available on NuGet)
				var allowancesOrdered = destination.OrderBy(sortBy + (reverse ? " descending" : ""));
				// paging
				var allowancesPaged = allowancesOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();
				var driverDatatable = new CalculateDriverAllowanceDatatable()
				{
					Data = allowancesPaged,
					Total = driver.Count()
				};
				return driverDatatable;
			}
			if ((string.IsNullOrEmpty(months) && string.IsNullOrEmpty(years) && string.IsNullOrEmpty(driverCs) &&
					 string.IsNullOrEmpty(driverNs) && string.IsNullOrEmpty(contents)) && takeabreaks == false)
			{
				var driver = _calculateDriverAllowanceRepository.GetAllQueryable();

				var driverOrdered = driver.OrderBy(sortBy + (reverse ? " descending" : ""));

				// paging
				var driverPaged = driverOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

				var destination = (from p in _calculateDriverAllowanceRepository.GetAllQueryable()
								   join t in _driverRepository.GetAllQueryable() on new { p.DriverC }
								   equals new { t.DriverC } into t1
								   from t in t1.DefaultIfEmpty()
								   select new CalculateDriverAllowanceViewModel()
								   {
									   CalculateC = p.CalculateC,
									   ApplyD = p.ApplyD,
									   DriverC = p.DriverC,
									   Content = p.Content,
									   AmountMoney = p.AmountMoney,
									   AmountMoneySubtract = p.AmountMoneySubtract,
									   Description = p.Description,
									   TakeABreak = p.TakeABreak,
									   DriverN = t != null ? t.LastN + " " + t.FirstN : ""
								   }
									   ).ToList();
				var driverDatatable = new CalculateDriverAllowanceDatatable()
				{
					Data = destination,
					Total = driver.Count()
				};
				return driverDatatable;
			}
			else
			{
				var driver = _calculateDriverAllowanceRepository.GetAllQueryable();

				var driverOrdered = driver.OrderBy(sortBy + (reverse ? " descending" : ""));

				// paging
				var driverPaged = driverOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

				var destination = (from p in _calculateDriverAllowanceRepository.GetAllQueryable()
								   join t in _driverRepository.GetAllQueryable() on new { p.DriverC }
								   equals new { t.DriverC } into t1
								   from t in t1.DefaultIfEmpty()
								   where (p.TakeABreak == "True")
								   select new CalculateDriverAllowanceViewModel()
								   {
									   CalculateC = p.CalculateC,
									   ApplyD = p.ApplyD,
									   DriverC = p.DriverC,
									   Content = p.Content,
									   AmountMoney = p.AmountMoney,
									   AmountMoneySubtract = p.AmountMoneySubtract,
									   Description = p.Description,
									   TakeABreak = p.TakeABreak,
									   DriverN = t != null ? t.LastN + " " + t.FirstN : ""
								   }
									   ).ToList();
				var driverDatatable = new CalculateDriverAllowanceDatatable()
				{
					Data = destination,
					Total = driver.Count()
				};
				return driverDatatable;
			}
		}

		public CalculateDriverAllowanceViewModel GetCalculateDriverAllowanceSizeByCode(string calculateC)
		{
			var driver = (from p in _calculateDriverAllowanceRepository.GetAllQueryable()
						  join t in _driverRepository.GetAllQueryable() on new { p.DriverC }
							   equals new { t.DriverC } into t1
						  from t in t1.DefaultIfEmpty()
						  where
							  p.CalculateC == calculateC
						  select new CalculateDriverAllowanceViewModel()
						  {
							  CalculateC = calculateC,
							  ApplyD = p.ApplyD,
							  DriverC = p.DriverC,
							  Content = p.Content,
							  AmountMoney = p.AmountMoney,
							  AmountMoneySubtract = p.AmountMoneySubtract,
							  Description = p.Description,
							  TakeABreak = p.TakeABreak,
							  CalculateSalary = p.CalculateSalary,
							  DriverN = t != null ? t.LastN + " " + t.FirstN : ""
						  }).FirstOrDefault();
			return driver;
		}

		public void CreateCalculate(CalculateDriverAllowanceViewModel pattern)
		{
			//var model = Mapper.Map<DriverAllowanceViewModel, DriverAllowance_M>(pattern);
			CalculateDriverAllowance_M model = new CalculateDriverAllowance_M();
			model.CalculateC = pattern.CalculateC;
			model.ApplyD = pattern.ApplyD;
			model.DriverC = pattern.DriverC;
			model.Content = pattern.Content;
			model.AmountMoney = pattern.AmountMoney;
			model.AmountMoneySubtract = pattern.AmountMoneySubtract;
			model.Description = pattern.Description;
			model.TakeABreak = pattern.TakeABreak;
			if (pattern.CalculateSalary == "True" || pattern.CalculateSalary == "1")
			{
				model.CalculateSalary = "1";
			}
			else
			{
				model.CalculateSalary = "0";
			}
			_calculateDriverAllowanceRepository.Add(model);
			SaveCalculateDriverAllowance();
		}
		public void UpdateCalculate(CalculateDriverAllowanceViewModel pattern)
		{
			var driverAllowanceToRemove = _calculateDriverAllowanceRepository.Query(d => d.CalculateC == pattern.CalculateC).FirstOrDefault();
			CalculateDriverAllowance_M model = new CalculateDriverAllowance_M();
			model.CalculateC = pattern.CalculateC;
			model.ApplyD = pattern.ApplyD;
			model.DriverC = pattern.DriverC;
			model.Content = pattern.Content;
			model.AmountMoney = pattern.AmountMoney;
			model.AmountMoneySubtract = pattern.AmountMoneySubtract;
			model.Description = pattern.Description;
			model.TakeABreak = pattern.TakeABreak;
			if (pattern.CalculateSalary == "True" || pattern.CalculateSalary == "1")
			{
				model.CalculateSalary = "1";
			}
			else
			{
				model.CalculateSalary = "0";
			}
			_calculateDriverAllowanceRepository.Add(model);
			_calculateDriverAllowanceRepository.Delete(driverAllowanceToRemove);
			SaveCalculateDriverAllowance();
		}
		public void DeleteCalculate(string id)
		{
			var patterns = _calculateDriverAllowanceRepository.Get(c => c.CalculateC == id);
			if (patterns != null)
			{
				_calculateDriverAllowanceRepository.Delete(patterns);
				SaveCalculateDriverAllowance();
			}
		}
		public List<CalculateDriverAllowanceViewModel> GetCalculateDriverAllowanceByCode(string value)
		{
			var driver = _calculateDriverAllowanceRepository.Query(p => p.CalculateC.StartsWith(value));
			var result = (from p in driver
						  join t in _driverRepository.GetAllQueryable() on new { p.DriverC }
							   equals new { t.DriverC } into t1
						  from t in t1.DefaultIfEmpty()
						  select new CalculateDriverAllowanceViewModel
						  {
							  CalculateC = p.CalculateC,
							  ApplyD = p.ApplyD,
							  DriverC = p.DriverC,
							  Content = p.Content,
							  AmountMoney = p.AmountMoney,
							  AmountMoneySubtract = p.AmountMoneySubtract,
							  Description = p.Description,
							  TakeABreak = p.TakeABreak,
							  DriverN = t != null ? t.LastN + " " + t.FirstN : ""
						  }).ToList();
			return result;
		}

		public string GetCodeNumber()
		{
			var codeMax = "0";
			var newCode = "";
			var list = _calculateDriverAllowanceRepository.GetAllQueryable().ToList();
			if (list.Count() != 0)
			{
				codeMax = list.Max(u => u.CalculateC);
				newCode = FormatCodeNumber(codeMax.ToNullSafeString(), 2);
			}
			else
			{
				newCode = FormatCodeNumber(codeMax.ToNullSafeString(), 2);
			}
			return newCode;
		}
		public static string FormatCodeNumber(string oldVal, int noLength)
		{
			return (Convert.ToInt64(oldVal) + 1).ToString("D" + noLength);
		}
		public void SaveCalculateDriverAllowance()
		{
			_unitOfWork.Commit();
		}
	}
}
