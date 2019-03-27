using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Partner
{
    public class PartnerInvoiceViewModel
    {
        public string PartnerMainC { get; set; }
        public string PartnerSubC { get; set; }
        public string PartnerN { get; set; }
        public string PartnerShortN { get; set; }
        public string TaxCode { get; set; }
        public decimal? InitPartnerPayment { get; set; }
        public string BankAccNumber { get; set; }
        public string BankAccN { get; set; }
        public string BankN { get; set; }
    }
}
