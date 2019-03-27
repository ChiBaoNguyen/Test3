using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Customer
{
    public class InvoiceViewModel
    {
        //public string CustomerId { get; set; }
        public string CustomerMainC { get; set; }
        public string CustomerSubC { get; set; }
        public string CustomerN { get; set; }
        public string CustomerShortN { get; set; }
        public string TaxCode { get; set; }
		public decimal? InitCustomerPayment { get; set; }
        public string BankAccNumber { get; set; }
        public string BankAccN { get; set; }
        public string BankN { get; set; }
    }
}
