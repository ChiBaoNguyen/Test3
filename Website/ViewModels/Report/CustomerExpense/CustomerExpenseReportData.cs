using System;
using System.Collections.Generic;
using Website.ViewModels.Container;
using Website.ViewModels.Customer;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Expense;
using Website.ViewModels.Order;
using Website.ViewModels.Surcharge;

namespace Website.ViewModels.Report.CustomerExpense
{
	public class CustomerExpenseReportData
	{
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string CustomerN { get; set; }
		public string CustomerAddress { get; set; }
		public string CustomerPayLiftLoweredMainC { get; set; }
		public string CustomerPayLiftLoweredSubC { get; set; }
		public string CustomerPayLiftLoweredN { get; set; }
		public string CustomerPayLiftLoweredAddress { get; set; }
		public string CustomerTaxCode { get; set; }
		public DateTime ApplyD { get; set; }
		public int SettlementD { get; set; }
		public string TaxMethodI { get; set; }
		public decimal TaxRate { get; set; }
		public string TaxRoundingI { get; set; }
		public string RevenueRoundingI { get; set; }
		public string CustomerShortN { get; set; }
		public string InvoiceMainC { get; set; }
		public string InvoiceSubC { get; set; }
		public string InvoiceN { get; set; }
		public List<CustomerExpenseItem> CustomerExpenseList { get; set; }
	}

	public class CustomerExpenseItem
	{
		public ContainerViewModel OrderD { get; set; }
		public OrderViewModel OrderH { get; set; }
		public CustomerSettlementViewModel CustomerSettlement { get; set; }
		public ExpenseDetailViewModel ExpenseD { get; set; }
		public int DetainDay { get; set; }
		public decimal SurchargeAmount { get; set; }
		public string Description { get; set; }
		public decimal DetainAmount { get; set; }
		public decimal TaxAmount { get; set; }
		public string TruckNo { get; set; }
		public DateTime? TransportD { get; set; }
	}
}