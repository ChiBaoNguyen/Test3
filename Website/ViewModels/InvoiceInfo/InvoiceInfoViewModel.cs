using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.InvoiceInfo
{
	public class InvoiceInfotViewModel
    {
        public string CustomerMainC { get; set; }
        public string CustomerSubC { get; set; }
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
        public DateTime ApplyD { get; set; }
        public int SettlementD { get; set; }
        public string TaxMethodI { get; set; }
        public decimal TaxRate { get; set; }
        public string TaxRoundingI { get; set; }
        public string RevenueRoundingI { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
    }
}