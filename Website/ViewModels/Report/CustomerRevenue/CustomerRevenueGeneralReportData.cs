using System;
using Website.ViewModels.Container;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Expense;
using Website.ViewModels.Order;

namespace Website.ViewModels.Report.CustomerRevenue
{
	public class CustomerRevenueGeneralReportData
	{
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string CustomerN { get; set; }
		public string CustomerShortN { get; set; }
		public string InvoiceMainC { get; set; }
		public string InvoiceSubC { get; set; }
		public int? ContainerSize20 { get; set; }
		public int? ContainerSize40 { get; set; }
		public int? ContainerSize45 { get; set; }
		public decimal? NetWeight { get; set; }
		public int? Quantity20HC { get; set; }
		public int? Quantity40HC { get; set; }
		public int? Quantity45HC { get; set; }
		public decimal? TotalLoads { get; set; }
		public decimal? Amount { get; set; }
		public decimal? TotalExpense { get; set; }
		public decimal? CustomerSurcharge { get; set; }
		public decimal? CustomerDiscount { get; set; }
		public decimal? TotalAmount { get; set; }
		public decimal? TaxAmount { get; set; }
		public decimal? DetainAmount { get; set; }
		public string TaxMethodI { get; set; }
		public decimal TaxRate { get; set; }
		public string TaxRoundingI { get; set; }
	}
}