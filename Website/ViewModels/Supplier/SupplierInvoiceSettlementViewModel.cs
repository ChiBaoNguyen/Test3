using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Supplier
{
    public class SupplierInvoiceSettlementViewModel
	{
        public SupplierViewModel Supplier { get; set; }
        public SupplierInvoiceViewModel Invoice { get; set; }
        public SupplierSettlementViewModel Settlement { get; set; }
		public List<SupplierSettlementViewModel> SettlementList { get; set; }
	    public int Status { get; set; }
	}

    public enum SupplierStatus
	{
		Add = 1,
		Edit = 2,
		Delete = 3,
		Reset = 4,
	}
}
