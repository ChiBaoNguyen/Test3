using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Website.ViewModels.Partner
{
    public class PartnerSettlementViewModel
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
