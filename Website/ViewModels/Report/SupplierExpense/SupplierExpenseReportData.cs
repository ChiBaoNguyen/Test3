using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.Container;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Expense;
using Website.ViewModels.Order;

namespace Website.ViewModels.Report.SupplierExpense
{
	public class SupplierExpenseReportData
	{
		public string SupplierMainC { get; set; }
		public string SupplierSubC { get; set; }
		public string SupplierN { get; set; }
		public string SupplierShortN { get; set; }
		public string ExpenseC { get; set; }
		public string ExpenseN { get; set; }
		public decimal? Quantity { get; set; }
		public decimal? UnitPrice { get; set; }
		public decimal? Total { get; set; }
		public decimal? TaxAmount { get; set; }
	}
}