using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Supplier
{
	public class SupplierViewModel
	{
        public string SupplierMainC { get; set; }
        public string SupplierSubC { get; set; }
        public string SupplierN { get; set; }
        public string SupplierShortN { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string InvoiceMainC { get; set; }
        public string InvoiceSubC { get; set; }
        public string TaxCode { get; set; }
        public string BankAccNumber { get; set; }
        public string BankAccN { get; set; }
        public string BankN { get; set; }
        public string IsActive { get; set; }
        public int SupplierIndex { get; set; }
        public SupplierSettlementViewModel Settlement { get; set; }
		public List<SupplierSettlementViewModel> SettlementList { get; set; }
		public decimal? TaxRate { get; set; }
	}
}
