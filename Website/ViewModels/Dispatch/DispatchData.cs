using System;

namespace Website.ViewModels.Dispatch
{
	public class DispatchData
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public DateTime? TransportD { get; set; }
		public string DispatchI { get; set; }
		public string TruckC { get; set; }
		public string DriverC { get; set; }
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public string OrderTypeI { get; set; }
		public int? DispatchOrder { get; set; }
		public string ContainerStatus { get; set; }
		public string DispatchStatus { get; set; }
		public string LoadingPlaceC { get; set; }
		public string LoadingPlaceN { get; set; }
		public DateTime? LoadingDT { get; set; }
		public string StopoverPlaceC { get; set; }
		public string StopoverPlaceN { get; set; }
		public DateTime? StopoverDT { get; set; }
		public string DischargePlaceC { get; set; }
		public string DischargePlaceN { get; set; }
		public DateTime? DischargeDT { get; set; }
		public decimal? TransportFee { get; set; }
		public decimal? PartnerFee { get; set; }
		public decimal? IncludedExpense { get; set; }
		public decimal? DriverAllowance { get; set; }
		public decimal? Expense { get; set; }
		public decimal? PartnerExpense { get; set; }
		public decimal? PartnerSurcharge { get; set; }
		public decimal? PartnerDiscount { get; set; }
        public decimal? PartnerTaxAmount { get; set; }
		public DateTime? InvoiceD { get; set; }
		public string TransportDepC { get; set; }
		public decimal? ApproximateDistance { get; set; }
		public decimal? ActualDistance { get; set; }
		public decimal? TotalKm { get; set; }
		public decimal? TotalFuel { get; set; }
		public decimal? LossFuelRate { get; set; }
		public Nullable<decimal> AllowanceOfDriver { get; set; }
		public Nullable<decimal> VirtualDataNoGoods { get; set; }
		public Nullable<decimal> VirtualDataHaveGoods { get; set; }
		public string WayType { get; set; }
	}
}