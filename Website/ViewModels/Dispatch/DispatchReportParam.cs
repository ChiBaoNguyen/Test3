using System;

namespace Website.ViewModels.Dispatch
{
	public class DispatchReportParam
	{
		public string DepC { get; set; }
		public bool DispatchStatus0 { get; set; }
		public bool DispatchStatus1 { get; set; }
		public bool DispatchStatus2 { get; set; }
		public string OrderTypeI { get; set; }
		public DateTime? TransportDFrom { get; set; }
		public DateTime? TransportDTo { get; set; }
		public string Customer { get; set; }
		public string Laguague { get; set; }
		public string ReportI { get; set; }
		public string TruckC { get; set; }
		public string DriverC { get; set; }
		public string Partner { get; set; }
		public string DispatchI { get; set; }
	}
}