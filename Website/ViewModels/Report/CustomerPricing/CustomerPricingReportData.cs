using System;
using System.Collections.Generic;

namespace Website.ViewModels.Report.CustomerPricing
{
	public class CustomerPricingReportData
	{
		public string CustomerPricingId { get; set; }
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string CustomerN { get; set; }
		public string Location1N { get; set; }
		public string Location2N { get; set; }
		public string ContainerSizeI { get; set; }
		public string ContainerTypeN { get; set; }
		public DateTime EstimatedD { get; set; }
		public decimal? Expense { get; set; }
		public decimal? EstimatedPrice { get; set; }
		public List<CustomerPricingExpenseDetailReportData> ExpenseDetail { get; set; }
	
	}
}
