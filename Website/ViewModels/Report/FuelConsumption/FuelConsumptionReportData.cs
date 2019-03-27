using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.FuelConsumption
{
	public class FuelConsumptionReportData
	{
		public string Code { get; set; }
		public string LastN { get; set; }
		public string FirstN { get; set; }
		public string RegisteredNo { get; set; }
		public decimal? ContainerNumber { get; set; }
		public decimal? EstimatedDistance { get; set; }
		public decimal? EstimatedFuel { get; set; }
		public decimal? ActualDistance { get; set; }
		public decimal? ActualFuel { get; set; }
	}
}
