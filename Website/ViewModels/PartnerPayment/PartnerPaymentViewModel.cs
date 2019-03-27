using System;

namespace Website.ViewModels.PartnerPayment
{
    public class PartnerPaymentViewModel
	{
	    public string ObjectI { get; set; }
	    public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public string PartnerN { get; set; }
	    public string SupplierMainC { get; set; }
	    public string SupplierSubC { get; set; }
	    public string SupplierN { get; set; }
	    public DateTime? PartnerPaymentD { get; set; }
		public DateTime? SupplierPaymentD { get; set; }
		public string PaymentId { get; set; }
		//public decimal? PreviousBalance { get; set; }
		public decimal? Amount { get; set; }
		//public decimal? NextBalance { get; set; }
		public string Description { get; set; }
		public int PartnerPaymentIndex { get; set; }
		public int Status { get; set; }
		public string EntryClerkC { get; set; }
		public string EntryClerkN { get; set; }
		public string PaymentMethodI { get; set; }
	}
}
