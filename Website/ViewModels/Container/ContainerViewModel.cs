using System;

namespace Website.ViewModels.Container
{
	public class ContainerViewModel
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public string ContainerNo { get; set; }
		public string ContainerSizeI { get; set; }
		public int? ContainerSize20 { get; set; }
		public int? ContainerSize40 { get; set; }
		public int? ContainerSize45 { get; set; }
		public decimal? TotalLoads { get; set; }
		public string ContainerTypeC { get; set; }
		public string CommodityC { get; set; }
		public string CommodityN { get; set; }
		public decimal? EstimatedWeight { get; set; }
		public decimal? NetWeight { get; set; }
		public decimal? GrossWeight { get; set; }
		public decimal? TotalPrice { get; set; }
		public decimal? UnitPrice { get; set; }
		public string SealNo { get; set; }
		public string Description { get; set; }
		public string TrailerC { get; set; }
		public string TrailerNo { get; set; }
		public string ContainerSizeN { get; set; }
		public string ContainerTypeN { get; set; }
		public DateTime? ActualLoadingD { get; set; }
		public DateTime? ActualDischargeD { get; set; }
		public DateTime? ActualPickupReturnD { get; set; }
		public DateTime? RevenueD { get; set; }
		public decimal? Amount { get; set; }
		public string CustomerSortForReport { get; set; }
		public decimal? CustomerSurcharge { get; set; }
		public decimal? CustomerDiscount { get; set; }
		public int? DetainDay { get; set; }
		public decimal? TotalExpense { get; set; }
		public decimal? DetainAmount { get; set; }
		public decimal? TaxAmount { get; set; }
        public decimal? PartnerSurcharge { get; set; }		
		public string LocationDispatch1 { get; set; }
		public string LocationDispatch2 { get; set; }
		public string LocationDispatch3 { get; set; }
		public string TruckCLastDispatch { get; set; }
		public string RegisteredNo { get; set; }
		public string CalculateByTon { get; set; }
		public string TruckCReturn { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? OrderNoDouble { get; set; }
        public int? EnableDouble { get; set; }
		public string JobNo { get; set; }
		public decimal TermContReturnNo { get; set; }
		public Nullable<DateTime> TermContReturnDT { get; set; }
		public decimal? PartnerAmount { get; set; }
		public int ImageCount { get; set; }
        public string OrderImageKey { get; set; }
	}
}
