using System;

namespace Website.ViewModels.Report.PartnerPayment
{
	public class PartnerPaymentReportParam
	{
		public DateTime DateFrom { get; set; }
		public DateTime DateTo { get; set; }
		public string Partner { get; set; }
		public string Languague { get; set; }
		public string ReportI { get; set; }
	}
}