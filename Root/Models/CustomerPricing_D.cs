using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public class CustomerPricing_D
	{
		public string CustomerPricingExpenseId { get; set; }
		public string CustomerPricingId { get; set; }
		public string RouteId { get; set; }
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public string CategoryI { get; set; }
		public string ExpenseC { get; set; }
		public string ExpenseN { get; set; }
		public string Unit { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Quantity { get; set; }
		public decimal Amount { get; set; }
		public string ExpenseRoot { get; set; }
		public string Description { get; set; }
	}
}
