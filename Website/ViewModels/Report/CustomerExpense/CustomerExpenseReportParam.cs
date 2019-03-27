using System;

namespace Website.ViewModels.Report.CustomerExpense
{
	public class CustomerExpenseReportParam
	{
		public string DepC { get; set; }
		public DateTime? TransportM { get; set; }
		public DateTime? OrderDFrom { get; set; }
		public DateTime? OrderDTo { get; set; }
		public string OrderTypeI { get; set; }
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string Customer { get; set; }
		public string InvoiceStatus { get; set; }
		public string BLBK { get; set; }
		public string Languague { get; set; }
		public int ReportType { get; set; }
		public string ReportI { get; set; }
		public string JobNo { get; set; }
		public string LoLo { get; set; }
	}
}