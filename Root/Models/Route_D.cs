using System;

namespace Root.Models
{
	public class Route_D
	{
		public string RouteExpenseId { get; set; }
		public string RouteId { get; set; }
		public int DisplayLineNo { get; set; }
		public string CategoryI { get; set; }
		public string ExpenseC { get; set; }
		public string ExpenseN { get; set; }
		public DateTime UsedExpenseD { get; set; }
		public string Unit { get; set; }
		public decimal? UnitPrice { get; set; }
		public decimal? Quantity { get; set; }
		public decimal? Amount { get; set; }
		public bool IsUsed { get; set; }
	}
}
