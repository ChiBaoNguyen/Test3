using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.TruckRevenue
{
	public class TruckRevenueReportParam
	{
		public DateTime DateFrom { get; set; }
		public DateTime DateTo { get; set; }
		public string TruckList { get; set; }
		public string Languague { get; set; }
	}
}
