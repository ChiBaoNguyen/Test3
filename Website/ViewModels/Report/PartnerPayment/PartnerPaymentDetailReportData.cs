using System;

namespace Website.ViewModels.Report.PartnerPayment
{
    public class PartnerPaymentDetailReportData
	{
		public DateTime? PaymentD { get; set; }
		public int PaymentY { get; set; }
		public int PaymentM { get; set; }
		public string Content { get; set; }
        public decimal? PartnerFee { get; set; }
        public decimal? PartnerExpense { get; set; }
        public decimal? PartnerSurcharge { get; set; }
        public decimal? PartnerDiscount { get; set; }
        public decimal? PartnerTaxAmount { get; set; }
		public decimal? PaymentAmount { get; set; }
	}
}
