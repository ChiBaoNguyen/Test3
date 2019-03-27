using System;
using System.Collections.Generic;
using Website.ViewModels.Container;
using Website.ViewModels.Expense;
using Website.ViewModels.Order;
using Website.ViewModels.Surcharge;
using Website.ViewModels.Expense;

namespace Website.ViewModels.Dispatch
{
	public class DispatchDetailViewModel
	{
		public OrderViewModel OrderH { get; set; }
		public ContainerViewModel OrderD { get; set; }
		public DispatchViewModel Dispatch { get; set; }
        public ExpenseViewModel Expense { get; set; }
		public DriverDispatchViewModel DriverDispatchList { get; set; }
		public bool IsDelete { get; set; }
		public List<ExpenseListReport> ExpenseList { get; set; }
		public List<DispatchViewModel> DispatchList { get; set; }
		public decimal? IsIncludedAmount { get; set; }
		public decimal? IsRequestedAmount { get; set; }
		public decimal? IsPayableAmount { get; set; }
	}
}