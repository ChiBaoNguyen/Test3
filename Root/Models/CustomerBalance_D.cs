using System;
using System.Collections.Generic;

namespace Root.Models
{
	public partial class CustomerBalance_D
	{
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public DateTime CustomerBalanceD { get; set; }
		public decimal? Amount { get; set; }
		public decimal? TotalExpense { get; set; }
		public decimal? CustomerSurcharge { get; set; }
		public decimal? CustomerDiscount { get; set; }
		public decimal? DetainAmount { get; set; }
		public decimal? TotalAmount { get; set; }
		public decimal? TaxAmount { get; set; }
		public decimal? PaymentAmount { get; set; }

	}
}
