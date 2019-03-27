using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Order
{
    public class CommodityViewModel
    {
		public string CommodityC { get; set; }
		public string CommodityN { get; set; }
		public string Description { get; set; }
		public decimal? PermittedWeight { get; set; }
		public string IsActive { get; set; }
	    public int CommodityIndex { get; set; }
    }
}
