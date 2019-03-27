using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public class CustomerPricingLocation_D
	{
		public int CustomerPricingLocationId { get; set; }
		public string CustomerPricingId { get; set; }
		public string RouteId { get; set; }
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public string ExpenseRoot { get; set; }
	}
}
