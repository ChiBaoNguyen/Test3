using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Website.ViewModels.OrderPattern
{
    public class OrderPatternViewModel
    {
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
        public string CustomerN { get; set; }
		public string OrderPatternC { get; set; }
		public string OrderPatternN { get; set; }
		public string OrderTypeI { get; set; }
		public string ShippingCompanyC { get; set; }
		public string ShippingCompanyN { get; set; }
		public string VesselC { get; set; }
		public string VesselN { get; set; }
		public string VoyageN { get; set; }
		public string ShipperC { get; set; }
		public string ShipperN { get; set; }
		public string LoadingPlaceC { get; set; }
		public string LoadingPlaceN { get; set; }
		public string StopoverPlaceC { get; set; }
		public string StopoverPlaceN { get; set; }
		public string DischargePlaceC { get; set; }
		public string DischargePlaceN { get; set; }
		public int PatternIndex { get; set; }
		public string ContainerTypeC { get; set; }
		public string ContainerTypeN { get; set; }
		public string ContainerSizeI { get; set; }
		public string ContainerSizeN { get; set; }
		public Nullable<decimal> UnitPrice { get; set; }
		public string CommodityC { get; set; }
		public string CommodityN { get; set; }
		public string CalculateByTon { get; set; }
		public List<ContractTariffPatternViewModel> ContractTariffPatterns { get; set; }
    }
}
