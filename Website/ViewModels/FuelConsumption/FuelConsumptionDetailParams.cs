using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.FuelConsumption
{
	public class FuelConsumptionDetailParams
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public decimal ApproximateDistance { get; set; }
		public string IsEmpty { get; set; }
		public string IsHeavy { get; set; }
		public string IsSingle { get; set; }
		public decimal FuelConsumption { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Amount { get; set; }
	}
}
