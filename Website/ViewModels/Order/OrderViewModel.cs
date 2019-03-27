using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.Container;

namespace Website.ViewModels.Order
{
	public class OrderViewModel
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public string OrderDepC { get; set; }
		public string OrderDepN { get; set; }
		public string EntryClerkC { get; set; }
		public string EntryClerkN { get; set; }
		public string EntryClerkFirstN { get; set; }
		public DateTime? RetiredD { get; set; }
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string CustomerN { get; set; }
		public string CustomerShortN { get; set; }
		public string OrderPatternC { get; set; }
		public string OrderPatternN { get; set; }
		public string OrderTypeI { get; set; }
		public string OrderTypeN { get; set; }
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
		public List<ContainerViewModel> Containers { get; set; }
		public int OrderIndex { get; set; }
		public bool IsMaxOrderNo { get; set; }
		public bool IsMinOrderNo { get; set; }
		public bool IsLoadingDtRedColor { get; set; }
		public bool IsStopoverDtRedColor { get; set; }
		public bool IsDischargeDtRedColor { get; set; }
		public bool IsETDRedColor { get; set; }
		public string IsCollected { get; set; }
		public Nullable<DateTime> ClosingDT { get; set; }
		public bool IsClosingDTRedColor { get; set; }
		public string ContractNo { get; set; }
		public string IsCollectedByMasterCustomer { get; set; }
		public decimal TermContReturnNo { get; set; }
		public Nullable<DateTime> TermContReturnDT { get; set; }
		public string CustomerPayLiftLoweredMainC { get; set; }
		public string CustomerPayLiftLoweredSubC { get; set; }
		public string CustomerPayLiftLoweredN { get; set; }
		public string CustomerPayLiftMainC { get; set; }
		public string CustomerPayLiftSubC { get; set; }
		public string ContSize20 { get; set; }
		public string ContSize40 { get; set; }
		public string ContSize45 { get; set; }
		public string ContSizeLoad { get; set; }
	}
}
