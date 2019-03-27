using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Expense
{
	public class ExpenseReportParam
	{
		public string Suppliers { get; set; }
		public DateTime InvoiceDFrom { get; set; }
		public DateTime InvoiceDTo { get; set; }
		public string ExpenseCategories { get; set; }
		public string Language { get; set; }
		public string PaymentMethod { get; set; }
		public string Customers { get; set; }
		public string ReportType { get; set; }
		public string ReportI { get; set; }
		public string ObjectI { get; set; }
		public string Trucks { get; set; }
		public string Trailers { get; set; }
		public string Employees { get; set; }
		public string Drivers { get; set; }
	}
}
