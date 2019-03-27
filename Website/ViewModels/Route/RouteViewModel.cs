using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Route
{
	public class RouteViewModel
	{
		public string RouteId { get; set; }
		public string Location1C { get; set; }
		public string Location1N { get; set; }
		public string Location2C { get; set; }
		public string Location2N { get; set; }
		public string ContainerTypeC { get; set; }
		public string ContainerTypeN { get; set; }
		public string ContainerSizeI { get; set; }
		public string IsEmpty { get; set; }
		public string IsHeavy { get; set; }
		public string IsSingle { get; set; }
		public string RouteN { get; set; }
		public decimal? TotalExpense { get; set; }
		public List<RouteExpenseViewModel> ExpenseList { get; set; }
		public List<RouteExpenseViewModel> AllowanceList { get; set; }
		public List<RouteExpenseViewModel> FixedExpenseList { get; set; }
		public List<RouteExpenseViewModel> OtherExpenseList { get; set; }
	}
}
