using System;

namespace Website.ViewModels.Report.CustomerPricing
{
	public class CustomerPricingReportParam
	{
		public string Customer { get; set; }
		public string Location1 { get; set; }
		public string Location2 { get; set; }
		public string ContainerSize { get; set; }
		public string ContainerType { get; set; }
		public DateTime DateFrom { get; set; }
		public DateTime DateTo { get; set; }
		public string Languague { get; set; }
		public string ReportType { get; set; }
	}
}