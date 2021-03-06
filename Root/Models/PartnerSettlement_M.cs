﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public partial class PartnerSettlement_M
	{
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public DateTime ApplyD { get; set; }
		public int SettlementD { get; set; }
		public string TaxMethodI { get; set; }
		public decimal? TaxRate { get; set; }
		public string TaxRoundingI { get; set; }
		public string RevenueRoundingI { get; set; }
	}
}
