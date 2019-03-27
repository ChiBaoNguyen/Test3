using System;
using System.Collections.Generic;

namespace Root.Models
{
    public partial class OrderPattern_M
    {
        public string CustomerMainC { get; set; }
        public string CustomerSubC { get; set; }
        public string OrderPatternC { get; set; }
        public string OrderPatternN { get; set; }
        public string OrderTypeI { get; set; }
        public string ShippingCompanyC { get; set; }
        public string VesselC { get; set; }
        public string VoyageN { get; set; }
        public string ShipperC { get; set; }
		public string LoadingPlaceC { get; set; }
		public string StopoverPlaceC { get; set; }
		public string DischargePlaceC { get; set; }
		public string ContainerTypeC { get; set; }
		public string ContainerSizeI { get; set; }
		public Nullable<decimal> UnitPrice { get; set; }
		public string CommodityC { get; set; }
		public string CommodityN { get; set; }
		public string CalculateByTon { get; set; }
    }
}
