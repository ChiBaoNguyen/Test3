using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.Maintenance
{
	public class TransportMaintenanceReportData
	{
		public string TruckC { get; set; }
		public string RegisteredNo { get; set; }
		public int Month { get; set; }
		public int? TransportCount { get; set; }
		public decimal? TotalKm { get; set; }
		public decimal? MaintenanceAmount { get; set; }

	}
}
