using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
    public partial class SupplierSettlement_M
	{
        public string SupplierMainC { get; set; }
        public string SupplierSubC { get; set; }
		public DateTime ApplyD { get; set; }
		public int SettlementD { get; set; }
		public string TaxMethodI { get; set; }
		public decimal? TaxRate { get; set; }
		public string TaxRoundingI { get; set; }
		public string RevenueRoundingI { get; set; }
	}
}
