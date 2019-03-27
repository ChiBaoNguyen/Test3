using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Customer
{
	public class CustomerInvoiceViewModel
	{
		public CustomerViewModel Customer { get; set; }
		public InvoiceViewModel Invoice { get; set; }
	    public CustomerSettlementViewModel Settlement { get; set; }
		public List<CustomerSettlementViewModel> SettlementList { get; set; }
		public List<CustomerGrossProfitViewModel> ProfitMarkupList { get; set; }
	    public int Status { get; set; }
	}

	public enum CustomerStatus
	{
		Add = 1,
		Edit = 2,
		Delete = 3,
		Reset = 4,
	}
}
