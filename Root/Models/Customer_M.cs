using System;
using System.Collections.Generic;

namespace Root.Models
{
	public partial class Customer_M
	{
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string CustomerN { get; set; }
		public string CustomerShortN { get; set; }
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
		public string IsCollected { get; set; }
		public string IsActive { get; set; }
	}
}
