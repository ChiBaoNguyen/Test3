using System;
using System.Collections.Generic;

namespace Root.Models
{
	public partial class PartnerBalance_D
	{
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public DateTime PartnerBalanceD { get; set; }
        public decimal? PartnerFee { get; set; }
        public decimal? PartnerExpense { get; set; }
		public decimal? PartnerSurcharge { get; set; }
		public decimal? PartnerDiscount { get; set; }
		public decimal? TotalAmount { get; set; }
		public decimal? TaxAmount { get; set; }
		public decimal? PaymentAmount { get; set; }

	}
}
