using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.FuelConsumption
{
	public class FuelConsumptionDetailSearchParams
	{
		public int page { get; set; }
		public int itemsPerPage { get; set; }
		public FuelConsumptionDetailSearchInfo SearchInfo { get; set; }
	}

	public class FuelConsumptionDetailSearchInfo
	{
		public string DepC { get; set; }
		public DateTime? TransportDFrom { get; set; }
		public DateTime? TransportDTo { get; set; }
		public bool DispatchStatus { get; set; }
		public bool TransportedStatus { get; set; }
		public string TruckCList { get; set; }
		public string DriverCList { get; set; }
	}
}
