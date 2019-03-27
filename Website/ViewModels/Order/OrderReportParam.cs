using System;

namespace Website.ViewModels.Order
{
	public class OrderReportParam
	{
		public string EntryClerkC { get; set; }
		public string DepC { get; set; }
		public DateTime? OrderDFrom { get; set; }
		public DateTime? OrderDTo { get; set; }
		public string OrderTypeI { get; set; }
		public DateTime? TransportDFrom { get; set; }
		public DateTime? TransportDTo { get; set; }
		public string Customer { get; set; }
		public string Language { get; set; }
		public string SortBy { get; set; }
		public string JobNo { get; set; }
	}
}