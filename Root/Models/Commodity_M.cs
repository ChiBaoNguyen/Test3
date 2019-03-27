using System;
using System.Collections.Generic;

namespace Root.Models
{
    public partial class Commodity_M
    {
        public string CommodityC { get; set; }
        public string CommodityN { get; set; }
		public string Description { get; set; }
		public Nullable<decimal> PermittedWeight { get; set; }
		public string IsActive { get; set; }
    }
}
