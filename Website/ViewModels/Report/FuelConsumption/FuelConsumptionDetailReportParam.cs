using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.FuelConsumption
{
	public class FuelConsumptionDetailReportParam
	{
		public DateTime DateFrom { get; set; }
		public DateTime DateTo { get; set; }
		public string TruckC { get; set; }
		public string RegisteredNo { get; set; }
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public string Languague { get; set; }
		public string DepC { get; set; }
		public string OrderTypeI { get; set; }
		public string Customer { get; set; }
	}
}
