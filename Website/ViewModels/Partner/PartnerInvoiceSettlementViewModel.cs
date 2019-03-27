using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Partner
{
    public class PartnerInvoiceSettlementViewModel
	{
        public PartnerViewModel Partner { get; set; }
        public PartnerInvoiceViewModel Invoice { get; set; }
        public PartnerSettlementViewModel Settlement { get; set; }
		public List<PartnerSettlementViewModel> SettlementList { get; set; }
	    public int Status { get; set; }
	}

    public enum PartnerStatus
	{
		Add = 1,
		Edit = 2,
		Delete = 3,
		Reset = 4,
	}
}
