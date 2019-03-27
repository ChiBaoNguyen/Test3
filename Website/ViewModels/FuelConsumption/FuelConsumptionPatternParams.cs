using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.FuelConsumption
{
	public class FuelConsumptionPatternParams
	{
		public string TruckC { get; set; }
		public string Location1C { get; set; }
		public string Location2C { get; set; }
		public string Location3C { get; set; }
		public string ContainerSizeI { get; set; }
		public decimal ApproximateDistance { get; set; }
		public string IsEmpty { get; set; }
		public string IsHeavy { get; set; }
		public string IsSingle { get; set; }
	}
}
