using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Models;

namespace Website.ViewModels.FuelConsumption
{
	public class FuelConsumptionViewModel: FuelConsumption_M
	{
		public string ModelC { get; set; }
		public string ModelN { get; set; }
		public string LoadingPlaceN { get; set; }
		public string DischargePlaceN { get; set; }

		public decimal Con20FuelConsumption { get; set; }
		public decimal Con40FuelConsumption { get; set; }
		public decimal Con45FuelConsumption { get; set; }
		public int FuelConsumptionIndex { get; set; }
	}
}
