using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.Order;

namespace Website.ViewModels.Commodity
{
	public class CommodityStatusViewModel
    {
		public CommodityViewModel Commodity { get; set; }

        public string Status { get; set; }
    }
}
