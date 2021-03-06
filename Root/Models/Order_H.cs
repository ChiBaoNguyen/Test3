using System;
using System.Collections.Generic;

namespace Root.Models
{
	public partial class Order_H
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public string OrderDepC { get; set; }
		public string EntryClerkC { get; set; }
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string OrderPatternC { get; set; }
		public string OrderTypeI { get; set; }
		public string BLBK { get; set; }
		public string ShippingCompanyC { get; set; }
		public string ShippingCompanyN { get; set; }
		public string VesselC { get; set; }
		public string VesselN { get; set; }
		public string VoyageN { get; set; }
		public string ShipperC { get; set; }
		public string ShipperN { get; set; }
		public string LoadingPlaceC { get; set; }
		public string LoadingPlaceN { get; set; }
		public Nullable<DateTime> LoadingDT { get; set; }
		public string StopoverPlaceC { get; set; }
		public string StopoverPlaceN { get; set; }
		public Nullable<DateTime> StopoverDT { get; set; }
		public string DischargePlaceC { get; set; }
		public string DischargePlaceN { get; set; }
		public Nullable<DateTime> DischargeDT { get; set; }
		public string JobNo { get; set; }
		public string Description { get; set; }
		public Nullable<decimal> TotalPrice { get; set; }
		public int? Quantity20HC { get; set; }
		public int? Quantity40HC { get; set; }
		public int? Quantity45HC { get; set; }
		public decimal? TotalLoads { get; set; }
		public Nullable<DateTime> ETD { get; set; }
		public string DischargePortC { get; set; }
		public string DischargePortN { get; set; }
		public string IsCollected { get; set; }
		public Nullable<DateTime> ClosingDT { get; set; }
		public string ContractNo { get; set; }
		public decimal TermContReturnNo { get; set; }
		public Nullable<DateTime> TermContReturnDT { get; set; }
		public string CustomerPayLiftLoweredMainC { get; set; }
		public string CustomerPayLiftLoweredSubC { get; set; }
	}
}
