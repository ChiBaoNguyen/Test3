using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.Maintenance
{
	public class MaintenanceReportParam
	{
		public DateTime MaintenanceDFrom { get; set; }
		public DateTime MaintenanceDTo { get; set; }
		public string DepC { get; set; }
		public string TruckC { get; set; }
		public string TrailerC { get; set; }
		public bool Plan { get; set; }
		public bool Finished { get; set; }
		public string Languague { get; set; }
		public int Year { get; set; }

	}
}
