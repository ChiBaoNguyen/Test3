using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using Website.Enum;
using Website.ViewModels.Customer;
using Website.ViewModels.CustomerPricing;
using Website.ViewModels.Route;

namespace Service.Services
{
	public interface ICustomerPricingService
	{
		CustomerPricingViewModel GetCustomerPricing(string loc1C, string loc2C, string conSizeI, string conTypeC, DateTime estimateD, string customerSubC, string customerMainC);
		SuggestedRouteList GetSuggestedRoutes(string loc1C, string loc2C, string conSizeI, string conTypeC, DateTime estimateD, string customerSubC, string customerMainC);
		CustomerPricingViewModel GetCustomerPricingById(string custPricingId);
		CustomerPricingDataTable GetCustomerPricingForTable(CustomerPricingSearchParams search);
		SuggestedExpenseList GetSuggestedExpensesFromRoute(List<SuggestedRoute> suggestedRoutes);
		SuggestedExpenseList GetSuggestedExpensesFromHistory(List<SuggestedRoute> suggestedRoutes);
		void CreateCustomerPricing(CustomerPricingViewModel custPricingViewModel);
		void UpdateCustomerPricing(CustomerPricingViewModel custPricingViewModel);
		void DeleteCustomerPricing(string custPricingId);
		void SaveCustomerPricing();
	}

	class CustomerPricingService : ICustomerPricingService
	{
		private readonly ICustomerPricingRepository _customerPricingRepository;
		private readonly ICustomerPricingDetailRepository _customerPricingDetailRepository;
		private readonly ICustomerPricingLocationRepository _customerPricingLocationRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly ICustomerGrossProfitRepository _customerGrossProfitRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly IContainerTypeRepository _containerTypeRepository;
		private readonly IRouteRepository _routeRepository;
		private readonly IDispatchRepository _dispatchRepository;
		private readonly IContainerRepository _orderDRepository;
		private readonly IOrderRepository _orderHRepository;
		private readonly IFuelConsumptionDetailRepository _fuelConsumptionDetailRepository;
		private readonly IExpenseDetailRepository _expenseDetailRepository;
		private readonly IAllowanceDetailRepository _allowanceDetailRepository;
		private readonly IRouteDetailRepository _routeDetailRepository;
		private readonly IExpenseRepository _expenseRepository;
		private readonly IUnitOfWork _unitOfWork;

		public CustomerPricingService(ICustomerPricingRepository customerPricingRepository,
								ICustomerPricingDetailRepository customerPricingDetailRepository,
								ICustomerPricingLocationRepository customerPricingLocationRepository,
								ICustomerRepository customerRepository,
								ICustomerGrossProfitRepository customerGrossProfitRepository,
								ILocationRepository locationRepository,
								IContainerTypeRepository containerTypeRepository,
								IRouteRepository routeRepository,
								IDispatchRepository dispatchRepository,
								IContainerRepository orderDRepository,
								IOrderRepository orderHRepository,
								IFuelConsumptionDetailRepository fuelConsumptionDetailRepository,
								IExpenseDetailRepository expenseDetailRepository,
								IAllowanceDetailRepository allowanceDetailRepository,
								IRouteDetailRepository routeDetailRepository,
								IExpenseRepository expenseRepository,
								IUnitOfWork unitOfWork)
		{
			this._customerPricingRepository = customerPricingRepository;
			this._customerPricingDetailRepository = customerPricingDetailRepository;
			this._customerPricingLocationRepository = customerPricingLocationRepository;
			this._customerRepository = customerRepository;
			this._customerGrossProfitRepository = customerGrossProfitRepository;
			this._locationRepository = locationRepository;
			this._containerTypeRepository = containerTypeRepository;
			this._routeRepository = routeRepository;
			this._dispatchRepository = dispatchRepository;
			this._orderDRepository = orderDRepository;
			this._orderHRepository = orderHRepository;
			this._fuelConsumptionDetailRepository = fuelConsumptionDetailRepository;
			this._expenseDetailRepository = expenseDetailRepository;
			this._allowanceDetailRepository = allowanceDetailRepository;
			this._routeDetailRepository = routeDetailRepository;
			this._expenseRepository = expenseRepository;
			this._unitOfWork = unitOfWork;
		}

		private CustomerPricingViewModel GetCustomerPricingExpenses(CustomerPricingViewModel custPricingViewModel)
		{
			//Get route expense list
			var expenseCate = Convert.ToInt32(ExpenseCategory.Expense).ToString();
			var expensesDepartment = _customerPricingDetailRepository.Query(
										p => p.CustomerPricingId == custPricingViewModel.CustomerPricingId & p.CategoryI == expenseCate);

			var expensesDestination = Mapper.Map<IEnumerable<CustomerPricing_D>, IEnumerable<CustomerPricingDetailViewModel>>(expensesDepartment);

			//Get route allowance list
			var allowanceCate = Convert.ToInt32(ExpenseCategory.Allowance).ToString();
			var allowancesDeparment = _customerPricingDetailRepository.Query(
								p => p.CustomerPricingId == custPricingViewModel.CustomerPricingId & p.CategoryI == allowanceCate);

			var allowancesDestination = Mapper.Map<IEnumerable<CustomerPricing_D>, IEnumerable<CustomerPricingDetailViewModel>>(allowancesDeparment);

			//Get route fixed expense list
			var fixedCate = Convert.ToInt32(ExpenseCategory.Fix).ToString();
			var fixedExpensesDeparment = _customerPricingDetailRepository.Query(
								p => p.CustomerPricingId == custPricingViewModel.CustomerPricingId & p.CategoryI == fixedCate);

			var fixedExpensesDestination = Mapper.Map<IEnumerable<CustomerPricing_D>, IEnumerable<CustomerPricingDetailViewModel>>(fixedExpensesDeparment);

			//Get route other expense list
			var otherCate = Convert.ToInt32(ExpenseCategory.Other).ToString();
			var otherExpensesDeparment = _customerPricingDetailRepository.Query(
								p => p.CustomerPricingId == custPricingViewModel.CustomerPricingId & p.CategoryI == otherCate);

			var otherExpensesDestination = Mapper.Map<IEnumerable<CustomerPricing_D>, IEnumerable<CustomerPricingDetailViewModel>>(otherExpensesDeparment);

			custPricingViewModel.ExpenseList = expensesDestination == null ? null : expensesDestination.ToList();
			custPricingViewModel.AllowanceList = allowancesDestination == null ? null : allowancesDestination.ToList();
			custPricingViewModel.FixedExpenseList = fixedExpensesDestination == null ? null : fixedExpensesDestination.ToList();
			custPricingViewModel.OtherExpenseList = otherExpensesDestination == null ? null : otherExpensesDestination.ToList();
			return custPricingViewModel;
		}

		public CustomerPricingViewModel GetCustomerPricing(string loc1C, string loc2C, string conSizeI, string conTypeC,
			DateTime estimatedD, string customerSubC, string customerMainC)
		{
			var custPricing = _customerPricingRepository.Query(
								p => p.Location1C == loc1C & p.Location2C == loc2C & p.ContainerSizeI == conSizeI & p.ContainerTypeC == conTypeC & p.EstimatedD == estimatedD &
									(customerMainC == null || customerMainC == "null" ||
									 (p.CustomerMainC == customerMainC && p.CustomerSubC == customerSubC)
									)).FirstOrDefault();
			if (custPricing != null)
			{
				var custPricingViewModel = Mapper.Map<CustomerPricing_H, CustomerPricingViewModel>(custPricing);
				custPricingViewModel = GetCustomerPricingExpenses(custPricingViewModel);
				custPricingViewModel.Index = FindIndex(custPricingViewModel.CustomerPricingId);
				return custPricingViewModel;
			}
			else
			{
				var custPricingViewModel = new CustomerPricingViewModel();
				custPricingViewModel.GrossProfitRatio = 0;
				var mainCode = customerMainC;
				var profitmarkup = (from p in _customerRepository.GetAllQueryable()
									join g in _customerGrossProfitRepository.GetAllQueryable() on p.CustomerMainC equals g.CustomerMainC into pg
									from g in pg.DefaultIfEmpty()
									where p.CustomerMainC == mainCode && g.ApplyD <= estimatedD
									select g).ToList();

				if (profitmarkup.Any())
				{
					var customerGrossProfitM = profitmarkup.FirstOrDefault(p => p.ApplyD == profitmarkup.Max(b => b.ApplyD));
					if (customerGrossProfitM != null)
					{
						custPricingViewModel.GrossProfitRatio = customerGrossProfitM.GrossProfitRatio;
						return custPricingViewModel;
					}
				}
				return null;
			}
			
		}

		public SuggestedRouteList GetSuggestedRoutes(string loc1C, string loc2C, string conSizeI, string conTypeC,
													DateTime estimateD, string customerSubC, string customerMainC)
		{
			var suggestedList = new SuggestedRouteList
			{
				DefinedRoutes = GetDefinedRoutes(loc1C, loc2C, conSizeI, conTypeC),
				HistoryRoutes = GetHistoryRoutes(loc1C, loc2C, conSizeI, conTypeC)
			};

			var custPricing = _customerPricingRepository.Query(
								p => p.Location1C == loc1C & p.Location2C == loc2C & p.ContainerSizeI == conSizeI & p.ContainerTypeC == conTypeC & p.EstimatedD == estimateD & (
									string.IsNullOrEmpty(customerMainC) || string.IsNullOrEmpty(customerSubC) || (p.CustomerMainC == customerMainC && p.CustomerSubC == customerSubC))).FirstOrDefault();
			if (custPricing != null)
			{
				var expenseHistoryRoot = Convert.ToInt32(ExpenseRoot.History).ToString();
				var selectedHistoryRoutes = (from p in _customerPricingLocationRepository.GetAllQueryable()
											 where p.CustomerPricingId == custPricing.CustomerPricingId && p.ExpenseRoot == expenseHistoryRoot
											select p).ToList();

				if (selectedHistoryRoutes.Any())
				{
					foreach (var t in selectedHistoryRoutes)
					{
						foreach (var t1 in suggestedList.HistoryRoutes)
						{
							if (t.OrderD == t1.OrderD &
							    t.OrderNo == t1.OrderNo &
							    t.DetailNo == t1.DetailNo &
							    t.DispatchNo == t1.DispatchNo)
							{
								t1.IsSelected = true;
							}
						}
					}
				}

				var expenseRouteRoot = Convert.ToInt32(ExpenseRoot.Route).ToString();
				var selectedRoutes = from p in _customerPricingLocationRepository.GetAllQueryable()
									 where p.CustomerPricingId == custPricing.CustomerPricingId && p.ExpenseRoot == expenseRouteRoot
									 select p;

				if (selectedRoutes.Any())
				{
					foreach (var t in selectedRoutes)
					{
						foreach (var t1 in suggestedList.DefinedRoutes)
						{
							if (t.RouteId == t1.RouteId)
							{
								t1.IsSelected = true;
							}
						}
					}
				}
			}
			
			return suggestedList;
		}

		public CustomerPricingViewModel GetCustomerPricingById(string custPricingId)
		{
			var custPricing = (from p in _customerPricingRepository.GetAllQueryable()
							   join l1 in _locationRepository.GetAllQueryable() on p.Location1C equals l1.LocationC into pl1
							   from l1 in pl1.DefaultIfEmpty()
							   join l2 in _locationRepository.GetAllQueryable() on p.Location2C equals l2.LocationC into pl12
							   from l2 in pl12.DefaultIfEmpty()
							   join t in _containerTypeRepository.GetAllQueryable() on p.ContainerTypeC equals t.ContainerTypeC into pl12c
							   from t in pl12c.DefaultIfEmpty()
							   join c in _customerRepository.GetAllQueryable() on new { p.CustomerMainC, p.CustomerSubC } equals new { c.CustomerMainC, c.CustomerSubC } into pl12ct
							   from c in pl12ct.DefaultIfEmpty()
							   where p.CustomerPricingId == custPricingId
							   select new CustomerPricingViewModel()
							   {
								   CustomerPricingId = p.CustomerPricingId,
								   CustomerMainC = p.CustomerMainC,
								   CustomerSubC = p.CustomerSubC,
								   CustomerN = c.CustomerN,
								   Location1C = p.Location1C,
								   Location1N = l1.LocationN,
								   Location2C = p.Location2C,
								   Location2N = l2.LocationN,
								   ContainerTypeC = p.ContainerTypeC,
								   ContainerTypeN = t.ContainerTypeN,
								   ContainerSizeI = p.ContainerSizeI,
								   GrossProfitRatio = p.GrossProfitRatio,
								   EstimatedPrice = p.EstimatedPrice,
								   TotalExpense = p.TotalExpense
							   }).FirstOrDefault();
			if (custPricing != null)
			{
				custPricing = GetCustomerPricingExpenses(custPricing);
				return custPricing;
			}

			return null;
		}

		public CustomerPricingDataTable GetCustomerPricingForTable(CustomerPricingSearchParams search)
		{
			var pricing = (from l in _customerPricingRepository.GetAllQueryable()
						   join c in _customerRepository.GetAllQueryable() on new { l.CustomerMainC, l.CustomerSubC } equals new { c.CustomerMainC, c.CustomerSubC }
						   into t1
						   from c in t1.DefaultIfEmpty()
						   join l1 in _locationRepository.GetAllQueryable() on l.Location1C equals l1.LocationC
						   join l2 in _locationRepository.GetAllQueryable() on l.Location2C equals l2.LocationC
						   join o in _containerTypeRepository.GetAllQueryable() on l.ContainerTypeC equals o.ContainerTypeC
						   where ((search.CustomerMainC == "" || l.CustomerMainC.Equals(search.CustomerMainC)) &&
								 (search.CustomerSubC == "" || l.CustomerSubC.Equals(search.CustomerSubC)) &&
								 (search.ContainerSizeI == "" || l.ContainerSizeI.Equals(search.ContainerSizeI)) &&
								 (search.ContainerTypeC == "" || l.ContainerTypeC.Equals(search.ContainerTypeC)) &&
								 (search.Location1C == "" || l.Location1C.Equals(search.Location1C)) &&
								 (search.Location2C == "" || l.Location2C.Equals(search.Location2C)))
						   select new CustomerPricingViewModel()
							{
								CustomerPricingId = l.CustomerPricingId,
								CustomerMainC = l.CustomerMainC,
								CustomerSubC = l.CustomerSubC,
								CustomerN = c.CustomerN,
								ContainerSizeI = l.ContainerSizeI,
								ContainerTypeC = l.ContainerTypeC,
								ContainerTypeN = o.ContainerTypeN,
								Location1C = l.Location1C,
								Location1N = l1.LocationN,
								Location2C = l.Location2C,
								Location2N = l2.LocationN,
								TotalExpense = l.TotalExpense,
								EstimatedPrice = l.EstimatedPrice,
								EstimatedD = l.EstimatedD,
							})
						   .OrderBy(search.SortBy + (search.Reverse ? " descending" : ""))
						   .Skip((search.Page - 1) * search.ItemsPerPage).Take(search.ItemsPerPage)
						   .ToList();

			var datatable = new CustomerPricingDataTable()
			{
				Data = pricing,
				Total = _customerPricingRepository.GetAllQueryable().Count()
			};
			return datatable;
		}

		public SuggestedExpenseList GetSuggestedExpensesFromRoute(List<SuggestedRoute> suggestedRoutes)
		{
			var suggestedExpenses = new SuggestedExpenseList();
			suggestedExpenses.ExpenseList = new List<CustomerPricingDetailViewModel>();
			suggestedExpenses.AllowanceList = new List<CustomerPricingDetailViewModel>();
			suggestedExpenses.FixedExpenseList = new List<CustomerPricingDetailViewModel>();
			suggestedExpenses.OtherExpenseList = new List<CustomerPricingDetailViewModel>();
			foreach (var suggestedRoute in suggestedRoutes)
			{
				var route = suggestedRoute;
				var expenseRoot = Convert.ToInt32(ExpenseRoot.Route).ToString();

				#region Get route expense list
				var expenseCate = Convert.ToInt32(ExpenseCategory.Expense).ToString();
				var expenses = _routeDetailRepository.Query(
												p => p.RouteId == route.RouteId & p.CategoryI == expenseCate &
												p.IsUsed).ToList();

				if (expenses.Any())
				{
					var expensesDestination = Mapper.Map<List<Route_D>, List<CustomerPricingDetailViewModel>>(expenses);
					expensesDestination.ForEach(s => s.ExpenseRoot = expenseRoot);
					suggestedExpenses.ExpenseList.AddRange(expensesDestination);
				}
				#endregion

				#region Get route allowance list
				var allowanceCate = Convert.ToInt32(ExpenseCategory.Allowance).ToString();
				var allowances = _routeDetailRepository.Query(
										p => p.RouteId == route.RouteId & p.CategoryI == allowanceCate &
										p.IsUsed).ToList();

				if (allowances.Any())
				{
					var allowancesDestination = Mapper.Map<List<Route_D>, List<CustomerPricingDetailViewModel>>(allowances);
					allowancesDestination.ForEach(s => s.ExpenseRoot = expenseRoot);
					suggestedExpenses.AllowanceList.AddRange(allowancesDestination);
				}
				#endregion

				#region Get fixed expense list
				var fixedCate = Convert.ToInt32(ExpenseCategory.Fix).ToString();
				var fixedExpenses = _routeDetailRepository.Query(
										p => p.RouteId == route.RouteId & p.CategoryI == fixedCate &
										p.IsUsed).ToList();

				if (fixedExpenses.Any())
				{
					var fixedExpensesDestination = Mapper.Map<List<Route_D>, List<CustomerPricingDetailViewModel>>(fixedExpenses);
					
					fixedExpensesDestination.ForEach(s => s.ExpenseRoot = expenseRoot);
					suggestedExpenses.FixedExpenseList.AddRange(fixedExpensesDestination);
				}
				#endregion

				#region Get other expense list
				var otherCate = Convert.ToInt32(ExpenseCategory.Other).ToString();
				var otherExpenses = _routeDetailRepository.Query(
										p => p.RouteId == route.RouteId & p.CategoryI == otherCate &
										p.IsUsed).ToList();

				if (otherExpenses.Any())
				{
					var otherExpensesDestination = Mapper.Map<List<Route_D>, List<CustomerPricingDetailViewModel>>(otherExpenses);
					otherExpensesDestination.ForEach(s => s.ExpenseRoot = expenseRoot);
					suggestedExpenses.OtherExpenseList.AddRange(otherExpensesDestination);
				}
				#endregion
			}
			return suggestedExpenses;
		}

		public SuggestedExpenseList GetSuggestedExpensesFromHistory(List<SuggestedRoute> suggestedRoutes)
		{
			var suggestedExpenses = new SuggestedExpenseList();
			suggestedExpenses.ExpenseList = new List<CustomerPricingDetailViewModel>();
			suggestedExpenses.AllowanceList = new List<CustomerPricingDetailViewModel>();
			suggestedExpenses.FixedExpenseList = new List<CustomerPricingDetailViewModel>();
			suggestedExpenses.OtherExpenseList = new List<CustomerPricingDetailViewModel>();
			var expenseCate = Convert.ToInt32(ExpenseCategory.Expense).ToString();
			var allowanceCate = Convert.ToInt32(ExpenseCategory.Allowance).ToString();
			var expenseRoot = Convert.ToInt32(ExpenseRoot.History).ToString();
			foreach (var suggestedRoute in suggestedRoutes)
			{
				var history = suggestedRoute;
				
				#region Get expense history
				var exCate = expenseCate;
				var historyExpenses = (from p in _expenseDetailRepository.GetAllQueryable()
					join e in _expenseRepository.GetAllQueryable() on p.ExpenseC equals e.ExpenseC into pe
					from e in pe.DefaultIfEmpty()
					where p.OrderD == history.OrderD & p.OrderNo == history.OrderNo &
					      p.DetailNo == history.DetailNo & p.DispatchNo == history.DispatchNo &
					      p.IsIncluded == "1" & p.IsRequested == "0"
					select new CustomerPricingDetailViewModel()
					{
						CategoryI = exCate,
						ExpenseC = p.ExpenseC,
						ExpenseN = e.ExpenseN,
						Unit = p.Unit,
						UnitPrice = p.UnitPrice,
						Quantity = p.Quantity,
						Amount = p.Amount,
						OrderD = p.OrderD,
						OrderNo = p.OrderNo,
						DetailNo = p.DetailNo,
						DispatchNo = p.DispatchNo,
						ExpenseRoot = expenseRoot
					}).ToList();
					
					
				if (historyExpenses.Any())
				{
					suggestedExpenses.ExpenseList.AddRange(historyExpenses);
				}
				#endregion

				#region Get allowance history
				var allCate = allowanceCate;
				var historyAllowances = (from p in _allowanceDetailRepository.GetAllQueryable()
									   join e in _expenseRepository.GetAllQueryable() on p.AllowanceC equals e.ExpenseC into pe
									   from e in pe.DefaultIfEmpty()
									   where p.OrderD == history.OrderD & p.OrderNo == history.OrderNo &
											 p.DetailNo == history.DetailNo & p.DispatchNo == history.DispatchNo
									   select new CustomerPricingDetailViewModel()
									   {
										   CategoryI = allCate,
										   ExpenseC = p.AllowanceC,
										   ExpenseN = e.ExpenseN,
										   Unit = p.Unit,
										   UnitPrice = p.UnitPrice,
										   Quantity = p.Quantity,
										   Amount = p.Amount,
										   OrderD = p.OrderD,
										   OrderNo = p.OrderNo,
										   DetailNo = p.DetailNo,
										   DispatchNo = p.DispatchNo,
										   ExpenseRoot = expenseRoot
									   }).ToList();


				if (historyAllowances.Any())
				{
					suggestedExpenses.AllowanceList.AddRange(historyAllowances);
				}
				#endregion
			}
			return suggestedExpenses;
		}

		private List<SuggestedRoute> GetDefinedRoutes(string location1C, string location2C, string containerSizeI, string containerTypeC)
		{
			var routes =
				_routeRepository.Query(
					p =>
						(p.Location1C == location1C || p.Location2C == location2C) &
						p.ContainerSizeI == containerSizeI &
						p.ContainerTypeC == containerTypeC);

			if (routes.Any())
			{
				var routesList = routes.ToList();
				var mappedRoutes = Mapper.Map<List<Route_H>, List<SuggestedRoute>>(routesList);
				var suggestedRoutes = (from p in mappedRoutes
									   join l1 in _locationRepository.GetAllQueryable() on p.Location1C equals l1.LocationC into pl1
									   from l1 in pl1.DefaultIfEmpty()
									   join l2 in _locationRepository.GetAllQueryable() on p.Location2C equals l2.LocationC into pl12
									   from l2 in pl12.DefaultIfEmpty()
									   //join t in _containerTypeRepository.GetAllQueryable() on p.ContainerTypeC equals t.ContainerTypeC into pl12c
									   //from t in pl12c.DefaultIfEmpty()
									   select new SuggestedRoute()
									   {
										   RouteId = p.RouteId,
										   Location1C = p.Location1C,
										   Location1N = l1.LocationN,
										   Location2C = p.Location2C,
										   Location2N = l2.LocationN,
										   ContainerTypeC = p.ContainerTypeC,
										   ContainerSizeI = p.ContainerSizeI,
										   IsEmpty = p.IsEmpty,
										   IsHeavy = p.IsHeavy,
										   IsSingle = p.IsSingle,
										   RouteN = p.RouteN,
										   TotalExpense = p.TotalExpense,
									   }).ToList();
				return suggestedRoutes;
			}
			return null;
		}

		private List<SuggestedRoute> GetHistoryRoutes(string location1C, string location2C, string containerSizeI, string containerTypeC)
		{
			var expenses = from p in _expenseDetailRepository.GetAllQueryable()
						   where p.IsIncluded == "1" && p.IsRequested == "0"
						   group p by new { p.OrderD, p.OrderNo, p.DetailNo, p.DispatchNo }
							   into g
							   select new
							   {
								   g.Key.OrderD,
								   g.Key.OrderNo,
								   g.Key.DetailNo,
								   g.Key.DispatchNo,
								   TotalExpense = g.Sum(p => p.Amount)
							   };

			var driverAllowances = from p in _allowanceDetailRepository.GetAllQueryable()
								   group p by new { p.OrderD, p.OrderNo, p.DetailNo, p.DispatchNo }
									   into g
									   select new
									   {
										   g.Key.OrderD,
										   g.Key.OrderNo,
										   g.Key.DetailNo,
										   g.Key.DispatchNo,
										   TotalExpense = g.Sum(p => p.Amount)
									   };

			var dispatches = from a in _dispatchRepository.GetAllQueryable()
							 join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo }
								 equals new { b.OrderD, b.OrderNo, b.DetailNo } into t1
							 from b in t1.DefaultIfEmpty()
							 join c in _orderHRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
								 equals new { c.OrderD, c.OrderNo } into t2
							 from c in t2.DefaultIfEmpty()
							 join d in _customerRepository.GetAllQueryable() on new { c.CustomerMainC, c.CustomerSubC }
								 equals new { d.CustomerMainC, d.CustomerSubC } into t3
							 from d in t3.DefaultIfEmpty()
							 join f in _fuelConsumptionDetailRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo, a.DispatchNo }
								 equals new { f.OrderD, f.OrderNo, f.DetailNo, f.DispatchNo } into t4
							 from f in t4.DefaultIfEmpty()
							 join e in expenses on new { a.OrderD, a.OrderNo, a.DetailNo, a.DispatchNo }
								 equals new { e.OrderD, e.OrderNo, e.DetailNo, e.DispatchNo } into t5
							 from e in t5.DefaultIfEmpty()
							 join w in driverAllowances on new { a.OrderD, a.OrderNo, a.DetailNo, a.DispatchNo }
								 equals new { w.OrderD, w.OrderNo, w.DetailNo, w.DispatchNo } into t6
							 from w in t6.DefaultIfEmpty()
							 where ((string.IsNullOrEmpty(a.Location1C) & !string.IsNullOrEmpty(a.Location2C) & 
										( a.Location2C == location1C || 
										(!string.IsNullOrEmpty(a.Location3C) && a.Location3C == location2C))) ||
									(!string.IsNullOrEmpty(a.Location1C) & ( a.Location1C == location1C || 
																			(!string.IsNullOrEmpty(a.Location3C) && a.Location3C == location2C) ||
																			(string.IsNullOrEmpty(a.Location3C) && !string.IsNullOrEmpty(a.Location2C) && a.Location2C == location2C)))) &
									b.ContainerSizeI == containerSizeI &
									b.ContainerTypeC == containerTypeC & a.DispatchStatus == Constants.CONFIRMED
							 select new SuggestedRoute()
							 {
								 OrderD = a.OrderD,
								 OrderNo = a.OrderNo,
								 DetailNo = a.DetailNo,
								 DispatchNo = a.DispatchNo,
								 CustomerN = d.CustomerN,
								 Location1C = !string.IsNullOrEmpty(a.Location1C) ? a.Location1C : a.Location2C,
								 Location1N = !string.IsNullOrEmpty(a.Location1N) ? a.Location1N : a.Location2N,
								 Location2C = !string.IsNullOrEmpty(a.Location3C) ? a.Location3C : a.Location2C,
								 Location2N = !string.IsNullOrEmpty(a.Location3N) ? a.Location3N : a.Location2N,
								 ContainerSizeI = b.ContainerSizeI,
								 ContainerTypeC = b.ContainerTypeC,
								 IsEmpty = f != null ? f.IsEmpty : "",
								 IsHeavy = f != null ? f.IsHeavy : "",
								 IsSingle = f != null ? f.IsSingle : "",
								 IsHistoryRoute = true,
								 TotalExpense = (e != null ? e.TotalExpense : 0) + (w != null ? w.TotalExpense : 0)
							 };

			if (dispatches.Any())
			{
				var suggestedRoutes = dispatches.ToList();
				return suggestedRoutes;
			}
			return null;
		}

		//CRUD
		private void InsertCustomerPricingExpenses(string custPricingId, CustomerPricingViewModel custPricingViewModel)
		{
			if (custPricingViewModel.ExpenseList != null && custPricingViewModel.ExpenseList.Count > 0)
			{
				for (var i = 0; i < custPricingViewModel.ExpenseList.Count; i++)
				{
					var expense = Mapper.Map<CustomerPricingDetailViewModel, CustomerPricing_D>(custPricingViewModel.ExpenseList[i]);
					expense.CustomerPricingExpenseId = Guid.NewGuid().ToString();
					expense.CustomerPricingId = custPricingId;
					expense.CategoryI = Convert.ToInt32(ExpenseCategory.Expense).ToString();
					_customerPricingDetailRepository.Add(expense);
				}
			}

			if (custPricingViewModel.AllowanceList != null && custPricingViewModel.AllowanceList.Count > 0)
			{
				for (var i = 0; i < custPricingViewModel.AllowanceList.Count; i++)
				{
					var expense = Mapper.Map<CustomerPricingDetailViewModel, CustomerPricing_D>(custPricingViewModel.AllowanceList[i]);
					expense.CustomerPricingExpenseId = Guid.NewGuid().ToString();
					expense.CustomerPricingId = custPricingId;
					expense.CategoryI = Convert.ToInt32(ExpenseCategory.Allowance).ToString();
					_customerPricingDetailRepository.Add(expense);
				}
			}

			if (custPricingViewModel.FixedExpenseList != null && custPricingViewModel.FixedExpenseList.Count > 0)
			{
				for (var i = 0; i < custPricingViewModel.FixedExpenseList.Count; i++)
				{
					var expense = Mapper.Map<CustomerPricingDetailViewModel, CustomerPricing_D>(custPricingViewModel.FixedExpenseList[i]);
					expense.CustomerPricingExpenseId = Guid.NewGuid().ToString();
					expense.CustomerPricingId = custPricingId;
					expense.CategoryI = Convert.ToInt32(ExpenseCategory.Fix).ToString();
					_customerPricingDetailRepository.Add(expense);
				}
			}

			if (custPricingViewModel.OtherExpenseList != null && custPricingViewModel.OtherExpenseList.Count > 0)
			{
				for (var i = 0; i < custPricingViewModel.OtherExpenseList.Count; i++)
				{
					var expense = Mapper.Map<CustomerPricingDetailViewModel, CustomerPricing_D>(custPricingViewModel.OtherExpenseList[i]);
					expense.CustomerPricingExpenseId = Guid.NewGuid().ToString();
					expense.CustomerPricingId = custPricingId;
					expense.CategoryI = Convert.ToInt32(ExpenseCategory.Other).ToString();
					_customerPricingDetailRepository.Add(expense);
				}
			}
		}

		private void InsertSelectedReferences(string custPricingId, CustomerPricingViewModel custPricingViewModel)
		{
			var listExpense = new List<CustomerPricingDetailViewModel>();
			listExpense.AddRange(custPricingViewModel.ExpenseList);
			listExpense.AddRange(custPricingViewModel.AllowanceList);
			listExpense.AddRange(custPricingViewModel.FixedExpenseList);
			listExpense.AddRange(custPricingViewModel.OtherExpenseList);

			var expenseRouteRoot = Convert.ToInt32(ExpenseRoot.Route).ToString();
			var referenceRouteData = (from p in listExpense
									where p.ExpenseRoot == expenseRouteRoot
									group p by new { p.RouteId, p.ExpenseRoot }
									into g
									select new CustomerPricingLocation_D()
									{
										CustomerPricingId = custPricingId,
										RouteId = g.Key.RouteId,
										ExpenseRoot = g.Key.ExpenseRoot
									}).ToList();
			if (referenceRouteData.Any())
			{
				foreach (var reference in referenceRouteData)
				{
					_customerPricingLocationRepository.Add(reference);
				}
			}

			var expenseHistoryRoot = Convert.ToInt32(ExpenseRoot.History).ToString();
			var referenceHistoryData = (from p in listExpense
									   where p.ExpenseRoot == expenseHistoryRoot
									 group p by new { p.OrderD, p.OrderNo, p.DetailNo, p.DispatchNo, p.ExpenseRoot }
										 into g
										 select new CustomerPricingLocation_D()
										 {
											 CustomerPricingId = custPricingId,
											 OrderD = g.Key.OrderD,
											 OrderNo = g.Key.OrderNo,
											 DetailNo = g.Key.DetailNo,
											 DispatchNo = g.Key.DispatchNo,
											 ExpenseRoot = g.Key.ExpenseRoot,
										 }).ToList();

			if (referenceHistoryData.Any())
			{
				foreach (var reference in referenceHistoryData)
				{
					_customerPricingLocationRepository.Add(reference);
				}
			}
		}

		public void CreateCustomerPricing(CustomerPricingViewModel custPricingViewModel)
		{
			var custPricing = Mapper.Map<CustomerPricingViewModel, CustomerPricing_H>(custPricingViewModel);
			custPricing.CustomerPricingId = Guid.NewGuid().ToString();
			_customerPricingRepository.Add(custPricing);

			InsertCustomerPricingExpenses(custPricing.CustomerPricingId, custPricingViewModel);
			InsertSelectedReferences(custPricing.CustomerPricingId, custPricingViewModel);

			SaveCustomerPricing();
		}

		public void UpdateCustomerPricing(CustomerPricingViewModel custPricingViewModel)
		{
			var custPricing = _customerPricingRepository.Query(p => p.CustomerPricingId == custPricingViewModel.CustomerPricingId).FirstOrDefault();
			if (custPricing != null)
			{
				custPricing.EstimatedPrice = custPricingViewModel.EstimatedPrice;
				custPricing.TotalExpense = custPricingViewModel.TotalExpense;
				custPricing.GrossProfitRatio = custPricingViewModel.GrossProfitRatio;
				_customerPricingRepository.Update(custPricing);

				//UPDATE ROUTE EXPENSES
				_customerPricingDetailRepository.Delete(p => p.CustomerPricingId == custPricingViewModel.CustomerPricingId);
				_customerPricingLocationRepository.Delete(p => p.CustomerPricingId == custPricingViewModel.CustomerPricingId);
				InsertCustomerPricingExpenses(custPricing.CustomerPricingId, custPricingViewModel);
				InsertSelectedReferences(custPricing.CustomerPricingId, custPricingViewModel);
				SaveCustomerPricing();
			}
		}

		public void DeleteCustomerPricing(string custPricingId)
		{
			_customerPricingRepository.Delete(p => p.CustomerPricingId == custPricingId);
			_customerPricingDetailRepository.Delete(p => p.CustomerPricingId == custPricingId);
			_customerPricingLocationRepository.Delete(p => p.CustomerPricingId == custPricingId);

			SaveCustomerPricing();
		}

		private int FindIndex(string code)
		{
			var data = _customerPricingRepository.GetAllQueryable();
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

						if (data.OrderBy("EstimatedD descending").Skip(recordsToSkip).Take(halfCount).Any(c => c.CustomerPricingId == code))
						{
							if (halfCount > loopCapacity)
							{
								totalRecords = totalRecords - (halfCount * 1);
								halfCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalRecords / 2))) + 1;
								break;
							}
							foreach (var entity in data.OrderBy("EstimatedD descending").Skip(recordsToSkip).Take(halfCount))
							{
								if (entity.CustomerPricingId == code)
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

		public void SaveCustomerPricing()
		{
			_unitOfWork.Commit();
		}
	}

}
