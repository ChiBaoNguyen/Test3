using System;

namespace Website.ViewModels.Customer
{
    public class CustomerListViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerMainCode { get; set; }
        public string CustomerSubCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerShortName { get; set; }
        public string ContactPerson { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public Nullable<int> InvoiceId { get; set; }
        public Root.Models.Customer_M InvoiceCompany { get; set; }
    }
}
