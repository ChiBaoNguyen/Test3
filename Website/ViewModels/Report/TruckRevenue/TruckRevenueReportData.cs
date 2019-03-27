using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.TruckRevenue
{
	public class TruckRevenueReportData
	{
		public DateTime? TransportD { get; set; }
		public int ContainerSize20 { get; set; }
		public int ContainerSize40 { get; set; }
		public int ContainerSize45 { get; set; }
		public decimal Load { get; set; }
		public string OrderTypeId { get; set; }
		public string Location { get; set; }
		public decimal Amount { get; set; }
		public string TruckC { get; set; }
		public string RegisteredNo { get; set; }
	}
}
