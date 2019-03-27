using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public class FuelConsumption_M
	{
		public string FuelConsumptionC { get; set; }
		public int FuelConsumptionId { get; set; }
		public string ContainerSizeI { get; set; }
		//public string IsEmpty { get; set; }
		//public string IsHeavy { get; set; }
		//public string IsSingle { get; set; }
		//public decimal UnitPrice { get; set; }
		//public decimal Amount { get; set; }
		public decimal? ShortRoad { get; set; }
		public decimal? LongRoad { get; set; }
		public decimal? Gradient { get; set; }
		public decimal? Empty { get; set; }
		public string ModelC { get; set; }
	}
}
