using System;

namespace Website.ViewModels.Dispatch
{
	public class DriverDispatchReportParam
	{
		public string DepC { get; set; }
		public DateTime? TransportDFrom { get; set; }
		public DateTime? TransportDTo { get; set; }
		public string TruckC { get; set; }
		public string RegisteredNo { get; set; }
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public string OrderTypeI { get; set; }
		public string Customer { get; set; }
		public string Partner { get; set; }
		public string Laguague { get; set; }
		public string ReportI { get; set; }
		public string OrderNo { get; set; }
		public string BLBK { get; set; }
		public string JobNo { get; set; }
		public string SortBy { get; set; }
		public string ReportType { get; set; }
		public string ReportObjectI { get; set; }
	}
}