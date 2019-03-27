using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.SupplierExpense
{
	public class SupplierExpenseReportParam
	{
		public string DepC { get; set; }
		public DateTime? ExpenseDFrom { get; set; }
		public DateTime? ExpenseDTo { get; set; }
		public string Supplier { get; set; }
		public string Expense { get; set; }
		public string Languague { get; set; }
	}
}
