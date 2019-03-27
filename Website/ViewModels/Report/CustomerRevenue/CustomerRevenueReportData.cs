using System;
using Website.ViewModels.Container;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Expense;
using Website.ViewModels.Order;

namespace Website.ViewModels.Report.CustomerRevenue
{
	public class CustomerRevenueReportData
	{
		public OrderViewModel OrderH { get; set; }
		public decimal Amount { get; set; }
		public decimal TotalExpense { get; set; }
		public decimal CustomerSurcharge { get; set; }
		public decimal CustomerDiscount { get; set; }
		public decimal TotalAmount { get; set; }
		public decimal DetainAmount { get; set; }
		public decimal TaxAmount { get; set; }
	}
}