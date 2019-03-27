using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.FuelConsumption
{
	public class FuelConsumptionReportParam
	{
		public DateTime DateFrom { get; set; }
		public DateTime DateTo { get; set; }
		public int ReportType { get; set; }
		public string DriverList { get; set; }
		public string TruckList { get; set; }
		public string Languague { get; set; }
		
	}
}
