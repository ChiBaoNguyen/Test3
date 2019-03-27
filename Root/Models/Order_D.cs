using System;
using System.Collections.Generic;

namespace Root.Models
{
	public partial class Order_D
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public string ContainerNo { get; set; }
		public string ContainerSizeI { get; set; }
		public string ContainerTypeC { get; set; }
		public string CommodityC { get; set; }
		public string CommodityN { get; set; }
		public Nullable<decimal> EstimatedWeight { get; set; }
		public Nullable<decimal> NetWeight { get; set; }
		public Nullable<decimal> GrossWeight { get; set; }
		public Nullable<decimal> UnitPrice { get; set; }
		public Nullable<decimal> TotalPrice { get; set; }
		public string SealNo { get; set; }
		public string Description { get; set; }
		public string TrailerC { get; set; }
		public Nullable<System.DateTime> ActualLoadingD { get; set; }
		public Nullable<System.DateTime> ActualDischargeD { get; set; }
		public Nullable<System.DateTime> ActualPickupReturnD { get; set; }
		public Nullable<System.DateTime> RevenueD { get; set; }
		public Nullable<decimal> Amount { get; set; }
		public Nullable<decimal> TotalExpense { get; set; }
		public Nullable<decimal> CustomerSurcharge { get; set; }
		public Nullable<decimal> CustomerDiscount { get; set; }
		public Nullable<decimal> PartnerAmount { get; set; }
		public Nullable<decimal> PartnerExpense { get; set; }
		public Nullable<decimal> PartnerSurcharge { get; set; }
		public Nullable<decimal> PartnerDiscount { get; set; }
		public Nullable<decimal> DetainAmount { get; set; }
		public Nullable<decimal> TotalAmount { get; set; }
		public Nullable<decimal> TaxAmount { get; set; }
		public Nullable<decimal> TotalPartnerAmount { get; set; }
		public Nullable<decimal> PartnerTaxAmount { get; set; }
		public Nullable<decimal> TotalCost { get; set; }
		public Nullable<decimal> TotalDriverAllowance { get; set; }
		public Nullable<int> DetainDay { get; set; }
        public Nullable<int> OrderNoDouble { get; set; }
        public Nullable<int> EnableDouble { get; set; }
		public string LocationDispatch1 { get; set; }
		public string LocationDispatch2 { get; set; }
		public string LocationDispatch3 { get; set; }
		public string CalculateByTon { get; set; }
		public string TruckCLastDispatch { get; set; }
		public string TruckCReturn { get; set; }
		public Nullable<System.DateTime> LastDDispatch { get; set; }
		public Nullable<decimal> TaxRate { get; set; }
		public string TransportConfirmDescription { get; set; }
	}
}
