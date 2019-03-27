using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.Route;

namespace Service.Services
{
	public interface IRouteService
	{
		RouteViewModel GetRoute(string loc1C, string loc2C, string conSizeI, string conTypeC, string isHeavy, string isEmpty,
			string isSingle);
		RouteViewModel GetRouteById(string routeId);
		RouteDatatable GetRoutesForTable(int page, int itemsPerPage, string sortBy, bool reverse,
			 string searchValue);
		List<RouteExpenseViewModel> GetExpensesHistory(string expenseC, string categoryI, string departureC, string destinationC, string conSizeI, string conTypeC);
		void CreateRoute(RouteViewModel routeViewModel);
		void UpdateRoute(RouteViewModel routeViewModel);
		void DeleteRoute(string routeId);
		void SaveRoute();
	}

	public class RouteService : IRouteService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IRouteRepository _routeRepository;
		private readonly IRouteDetailRepository _routeDetailRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly IContainerTypeRepository _containerTypeRepository;
		private readonly IExpenseDetailRepository _expenseDetailRepository;
		private readonly IAllowanceDetailRepository _allowanceDetailRepository;
		private readonly IContainerRepository _containerRepository;
		private readonly IDispatchRepository _dispatchRepository;

		public RouteService(IUnitOfWork unitOfWork, IRouteRepository routeRepository,
							IRouteDetailRepository routeDetailRepository, ILocationRepository locationRepository,
							IExpenseDetailRepository expenseDetailRepository, IAllowanceDetailRepository allowanceDetailRepository,
							IContainerRepository containerRepository, IDispatchRepository dispatchRepository,
							IContainerTypeRepository containerTypeRepository)
		{
			_unitOfWork = unitOfWork;
			_routeRepository = routeRepository;
			_routeDetailRepository = routeDetailRepository;
			_locationRepository = locationRepository;
			_containerTypeRepository = containerTypeRepository;
			_expenseDetailRepository = expenseDetailRepository;
			_allowanceDetailRepository = allowanceDetailRepository;
			_containerRepository = containerRepository;
			_dispatchRepository = dispatchRepository;
		}

		private RouteViewModel GetRouteExpenses(RouteViewModel routeViewModel)
		{
			//Get route expense list
			var expenseCate = Convert.ToInt32(ExpenseCategory.Expense).ToString();
			var expensesDepartment = _routeDetailRepository.Query(
										p => p.RouteId == routeViewModel.RouteId & p.CategoryI == expenseCate &
											p.IsUsed).OrderBy(p => p.DisplayLineNo);

			var expensesDestination = Mapper.Map<IEnumerable<Route_D>, IEnumerable<RouteExpenseViewModel>>(expensesDepartment);

			//Get route allowance list
			var allowanceCate = Convert.ToInt32(ExpenseCategory.Allowance).ToString();
			var allowancesDeparment = _routeDetailRepository.Query(
								p => p.RouteId == routeViewModel.RouteId & p.CategoryI == allowanceCate &
									p.IsUsed).OrderBy(p => p.DisplayLineNo);

			var allowancesDestination = Mapper.Map<IEnumerable<Route_D>, IEnumerable<RouteExpenseViewModel>>(allowancesDeparment);

			//Get route fixed expense list
			var fixedCate = Convert.ToInt32(ExpenseCategory.Fix).ToString();
			var fixedExpensesDeparment = _routeDetailRepository.Query(
								p => p.RouteId == routeViewModel.RouteId & p.CategoryI == fixedCate &
									p.IsUsed).OrderBy(p => p.DisplayLineNo);

			var fixedExpensesDestination = Mapper.Map<IEnumerable<Route_D>, IEnumerable<RouteExpenseViewModel>>(fixedExpensesDeparment);

			//Get route other expense list
			var otherCate = Convert.ToInt32(ExpenseCategory.Other).ToString();
			var otherExpensesDeparment = _routeDetailRepository.Query(
								p => p.RouteId == routeViewModel.RouteId & p.CategoryI == otherCate &
									p.IsUsed).OrderBy(p => p.DisplayLineNo);

			var otherExpensesDestination = Mapper.Map<IEnumerable<Route_D>, IEnumerable<RouteExpenseViewModel>>(otherExpensesDeparment);

			routeViewModel.ExpenseList = expensesDestination == null ? null : expensesDestination.ToList();
			routeViewModel.AllowanceList = allowancesDestination == null ? null : allowancesDestination.ToList();
			routeViewModel.FixedExpenseList = fixedExpensesDestination == null ? null : fixedExpensesDestination.ToList();
			routeViewModel.OtherExpenseList = otherExpensesDestination == null ? null : otherExpensesDestination.ToList();
			return routeViewModel;
		}

		public RouteViewModel GetRoute(string loc1C, string loc2C, string conSizeI, string conTypeC, string isHeavy, string isEmpty,
			string isSingle)
		{
			isHeavy = isHeavy == "-1" ? "" : isHeavy;
			isEmpty = isEmpty == "-1" ? "" : isEmpty;
			isSingle = isSingle == "-1" ? "" : isSingle;
			var route = _routeRepository.Query(
						p => p.Location1C == loc1C & p.Location2C == loc2C & p.ContainerSizeI == conSizeI & p.ContainerTypeC == conTypeC &
							p.IsEmpty == isEmpty & p.IsHeavy == isHeavy & p.IsSingle == isSingle).FirstOrDefault();
			if (route != null)
			{
				var routeViewModel = Mapper.Map<Route_H, RouteViewModel>(route);
				routeViewModel = GetRouteExpenses(routeViewModel);
				return routeViewModel;
			}

			return null;
		}

		public RouteViewModel GetRouteById(string routeId)
		{
			var route = (from p in _routeRepository.GetAllQueryable()
						 join l1 in _locationRepository.GetAllQueryable() on p.Location1C equals l1.LocationC into pl1
						 from l1 in pl1.DefaultIfEmpty()
						 join l2 in _locationRepository.GetAllQueryable() on p.Location2C equals l2.LocationC into pl12
						 from l2 in pl12.DefaultIfEmpty()
						 join t in _containerTypeRepository.GetAllQueryable() on p.ContainerTypeC equals t.ContainerTypeC into pl12c
						 from t in pl12c.DefaultIfEmpty()
						 where p.RouteId == routeId
						 select new RouteViewModel()
						 {
							 RouteId = p.RouteId,
							 Location1C = p.Location1C,
							 Location1N = l1.LocationN,
							 Location2C = p.Location2C,
							 Location2N = l2.LocationN,
							 ContainerTypeC = p.ContainerTypeC,
							 ContainerTypeN = t.ContainerTypeN,
							 ContainerSizeI = p.ContainerSizeI,
							 IsEmpty = p.IsEmpty,
							 IsHeavy = p.IsHeavy,
							 IsSingle = p.IsSingle,
							 RouteN = p.RouteN,
							 TotalExpense = p.TotalExpense
						 }).FirstOrDefault();
			if (route != null)
			{
				route = GetRouteExpenses(route);
				return route;
			}

			return null;
		}

		public RouteDatatable GetRoutesForTable(int page, int itemsPerPage, string sortBy, bool reverse, string searchValue)
		{
			var routes = from p in _routeRepository.GetAllQueryable()
						 join l1 in _locationRepository.GetAllQueryable() on p.Location1C equals l1.LocationC into pl1
						 from l1 in pl1.DefaultIfEmpty()
						 join l2 in _locationRepository.GetAllQueryable() on p.Location2C equals l2.LocationC into pl12
						 from l2 in pl12.DefaultIfEmpty()
						 join t in _containerTypeRepository.GetAllQueryable() on p.ContainerTypeC equals t.ContainerTypeC into pl12c
						 from t in pl12c.DefaultIfEmpty()
						 select new RouteViewModel()
						 {
							 RouteId = p.RouteId,
							 Location1C = p.Location1C,
							 Location1N = l1.LocationN,
							 Location2C = p.Location2C,
							 Location2N = l2.LocationN,
							 ContainerTypeC = p.ContainerTypeC,
							 ContainerTypeN = t.ContainerTypeN,
							 ContainerSizeI = p.ContainerSizeI,
							 IsEmpty = p.IsEmpty,
							 IsHeavy = p.IsHeavy,
							 IsSingle = p.IsSingle,
							 RouteN = p.RouteN,
							 TotalExpense = p.TotalExpense
						 };
			// Searching
			if (!string.IsNullOrWhiteSpace(searchValue))
			{
				searchValue = searchValue.ToLower();
				routes = routes.Where(r => r.RouteN.ToLower().Contains(searchValue) || r.Location1N.ToLower().Contains(searchValue) ||
											r.Location2N.ToLower().Contains(searchValue) || r.ContainerTypeN.ToLower().Contains(searchValue));
			}

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var routesOrdered = routes.OrderBy(sortBy + (reverse ? " descending" : ""));

			// paging
			var routesPaged = routesOrdered.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

			var routeDatatable = new RouteDatatable()
			{
				Data = routesPaged,
				Total = routes.Count()
			};
			return routeDatatable;
		}

		public List<RouteExpenseViewModel> GetExpensesHistory(string expenseC, string categoryI, string departureC, string destinationC, string conSizeI, string conTypeC)
		{
			if (categoryI == Convert.ToInt32(ExpenseCategory.Expense).ToString())
			{
				var historyExpenses = from e in _expenseDetailRepository.GetAllQueryable()
									  join d in _dispatchRepository.GetAllQueryable() on new { e.OrderD, e.OrderNo, e.DetailNo, e.DispatchNo } equals
										  new { d.OrderD, d.OrderNo, d.DetailNo, d.DispatchNo } into ed
									  from d in ed.DefaultIfEmpty()
									  join c in _containerRepository.GetAllQueryable() on new { d.OrderD, d.OrderNo, d.DetailNo } equals
										  new { c.OrderD, c.OrderNo, c.DetailNo } into edc
									  from c in edc.DefaultIfEmpty()
									  where e.ExpenseC == expenseC && c.ContainerSizeI == conSizeI && c.ContainerTypeC == conTypeC && d.DispatchStatus == Constants.CONFIRMED &&
											((string.IsNullOrEmpty(d.Location1C) & !string.IsNullOrEmpty(d.Location2C) &
											(d.Location2C == departureC ||
											(!string.IsNullOrEmpty(d.Location3C) && d.Location3C == destinationC))) ||
											(!string.IsNullOrEmpty(d.Location1C) & (d.Location1C == departureC ||
											(!string.IsNullOrEmpty(d.Location3C) && d.Location3C == destinationC) ||
											(string.IsNullOrEmpty(d.Location3C) && !string.IsNullOrEmpty(d.Location2C) && d.Location2C == destinationC))))
									  select new RouteExpenseViewModel()
									  {
										  UsedExpenseD = e.OrderD,
										  Quantity = e.Quantity,
										  Unit = e.Unit,
										  UnitPrice = e.UnitPrice,
										  Amount = e.Amount
									  };

				if (historyExpenses.Any())
				{
					historyExpenses = historyExpenses.OrderBy("UsedExpenseD descending");
					var limitedHistoryExpenses = historyExpenses.Skip(0).Take(10).ToList();
					return limitedHistoryExpenses;
				}
			}
			else if (categoryI == Convert.ToInt32(ExpenseCategory.Allowance).ToString())
			{
				var historyAllowances = from e in _allowanceDetailRepository.GetAllQueryable()
										join d in _dispatchRepository.GetAllQueryable() on new { e.OrderD, e.OrderNo, e.DetailNo, e.DispatchNo } equals
											new { d.OrderD, d.OrderNo, d.DetailNo, d.DispatchNo } into ed
										from d in ed.DefaultIfEmpty()
										join c in _containerRepository.GetAllQueryable() on new { d.OrderD, d.OrderNo, d.DetailNo } equals
											new { c.OrderD, c.OrderNo, c.DetailNo } into edc
										from c in edc.DefaultIfEmpty()
										where e.AllowanceC == expenseC && c.ContainerSizeI == conSizeI && c.ContainerTypeC == conTypeC && d.DispatchStatus == Constants.CONFIRMED &&
											((string.IsNullOrEmpty(d.Location1C) & !string.IsNullOrEmpty(d.Location2C) &
											(d.Location2C == departureC ||
											(!string.IsNullOrEmpty(d.Location3C) && d.Location3C == destinationC))) ||
											(!string.IsNullOrEmpty(d.Location1C) & (d.Location1C == departureC ||
											(!string.IsNullOrEmpty(d.Location3C) && d.Location3C == destinationC) ||
											(string.IsNullOrEmpty(d.Location3C) && !string.IsNullOrEmpty(d.Location2C) && d.Location2C == destinationC))))
										select new RouteExpenseViewModel()
										{
											UsedExpenseD = e.OrderD,
											Quantity = e.Quantity,
											Unit = e.Unit,
											UnitPrice = e.UnitPrice,
											Amount = e.Amount
										};

				if (historyAllowances.Any())
				{
					historyAllowances = historyAllowances.OrderBy("UsedExpenseD descending");
					var limitedhistoryAllowances = historyAllowances.Skip(0).Take(10).ToList();
					return limitedhistoryAllowances;
				}
			}

			return null;
		}

		public void CreateRoute(RouteViewModel routeViewModel)
		{
			var route = Mapper.Map<RouteViewModel, Route_H>(routeViewModel);
			route.RouteId = Guid.NewGuid().ToString();
			route.IsHeavy = route.IsHeavy == "-1" ? "" : route.IsHeavy;
			route.IsEmpty = route.IsEmpty == "-1" ? "" : route.IsEmpty;
			route.IsSingle = route.IsSingle == "-1" ? "" : route.IsSingle;
			_routeRepository.Add(route);

			InsertRouteExpenses(route.RouteId, routeViewModel);

			SaveRoute();
		}

		public void UpdateRoute(RouteViewModel routeViewModel)
		{
			var route = _routeRepository.Query(p => p.RouteId == routeViewModel.RouteId).FirstOrDefault();
			if (route != null)
			{
				route.RouteN = routeViewModel.RouteN;
				route.TotalExpense = routeViewModel.TotalExpense;
				_routeRepository.Update(route);

				//UPDATE ROUTE EXPENSES
				_routeDetailRepository.Delete(p => p.RouteId == routeViewModel.RouteId);
				InsertRouteExpenses(route.RouteId, routeViewModel);
				SaveRoute();
			}
		}

		private void InsertRouteExpenses(string routeId, RouteViewModel routeViewModel)
		{
			if (routeViewModel.ExpenseList != null && routeViewModel.ExpenseList.Count > 0)
			{
				for (var i = 0; i < routeViewModel.ExpenseList.Count; i++)
				{
					var expense = Mapper.Map<RouteExpenseViewModel, Route_D>(routeViewModel.ExpenseList[i]);
					expense.RouteExpenseId = Guid.NewGuid().ToString();
					expense.RouteId = routeId;
					expense.CategoryI = Convert.ToInt32(ExpenseCategory.Expense).ToString();
					expense.UsedExpenseD = DateTime.Now;
					expense.IsUsed = true;
					_routeDetailRepository.Add(expense);
				}
			}

			if (routeViewModel.AllowanceList != null && routeViewModel.AllowanceList.Count > 0)
			{
				for (var i = 0; i < routeViewModel.AllowanceList.Count; i++)
				{
					var expense = Mapper.Map<RouteExpenseViewModel, Route_D>(routeViewModel.AllowanceList[i]);
					expense.RouteExpenseId = Guid.NewGuid().ToString();
					expense.RouteId = routeId;
					expense.CategoryI = Convert.ToInt32(ExpenseCategory.Allowance).ToString();
					expense.UsedExpenseD = DateTime.Now;
					expense.IsUsed = true;
					_routeDetailRepository.Add(expense);
				}
			}

			if (routeViewModel.FixedExpenseList != null && routeViewModel.FixedExpenseList.Count > 0)
			{
				for (var i = 0; i < routeViewModel.FixedExpenseList.Count; i++)
				{
					var expense = Mapper.Map<RouteExpenseViewModel, Route_D>(routeViewModel.FixedExpenseList[i]);
					expense.RouteExpenseId = Guid.NewGuid().ToString();
					expense.RouteId = routeId;
					expense.CategoryI = Convert.ToInt32(ExpenseCategory.Fix).ToString();
					expense.UsedExpenseD = DateTime.Now;
					expense.IsUsed = true;
					_routeDetailRepository.Add(expense);
				}
			}

			if (routeViewModel.OtherExpenseList != null && routeViewModel.OtherExpenseList.Count > 0)
			{
				for (var i = 0; i < routeViewModel.OtherExpenseList.Count; i++)
				{
					var expense = Mapper.Map<RouteExpenseViewModel, Route_D>(routeViewModel.OtherExpenseList[i]);
					expense.RouteExpenseId = Guid.NewGuid().ToString();
					expense.RouteId = routeId;
					expense.CategoryI = Convert.ToInt32(ExpenseCategory.Other).ToString();
					expense.UsedExpenseD = DateTime.Now;
					expense.IsUsed = true;
					_routeDetailRepository.Add(expense);
				}
			}
		}

		public void DeleteRoute(string routeId)
		{
			_routeRepository.Delete(p => p.RouteId == routeId);
			_routeDetailRepository.Delete(p => p.RouteId == routeId);

			SaveRoute();
		}

		public void SaveRoute()
		{
			_unitOfWork.Commit();
		}
	}
}
