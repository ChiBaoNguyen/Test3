using System;

namespace Website.ViewModels.Report.CustomerPayment
{
	public class CustomerPaymentReportParam
	{
		public DateTime DateFrom { get; set; }
		public DateTime DateTo { get; set; }
		public string Customer { get; set; }
		public string Languague { get; set; }
		public string ReportI { get; set; }
	}
}