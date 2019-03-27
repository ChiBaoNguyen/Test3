using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Website.ViewModels.OrderPattern
{
    public class TariffPatternViewModel
    {
        public string DepartureC { get; set; }
        public string DepartureN { get; set; }
        public string DestinationC { get; set; }
        public string DestinationN { get; set; }
        public string ContainerTypeC { get; set; }
        public string ContainerTypeN { get; set; }
        public string ContainerSizeI { get; set; }
        public string ContainerSizeN { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
		public int DisplayLineNo { get; set; }
		public string CommodityC { get; set; }
		public string CommodityN { get; set; }
		public string CalculateByTon { get; set; }
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string OrderPatternC { get; set; }
    }
}