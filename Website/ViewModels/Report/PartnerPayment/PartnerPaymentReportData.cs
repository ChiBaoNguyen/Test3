using System.Collections.Generic;

namespace Website.ViewModels.Report.PartnerPayment
{
    public class PartnerPaymentReportData
	{
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public string PartnerN { get; set; }
		public decimal? OpeningBalance { get; set; }
		public decimal? ClosingBalance { get; set; }
		public decimal? PayExpense { get; set; }
		public decimal? OweExpense { get; set; }
		public decimal? PartnerExpense { get; set; }
		public string CommodityN { get; set; }
		public string ContainerSizeI { get; set; }
		public decimal? ContainerCount { get; set; }
		public decimal? NetWeight { get; set; }
		public decimal? UnitPrice { get; set; }
		public int No { get; set; }
		public List<PartnerPaymentDetailReportData> OweData { get; set; }
		public List<PartnerPaymentDetailReportData> PayData { get; set; }
	}
}
