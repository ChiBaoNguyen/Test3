using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.Order;

namespace Website.ViewModels.Ship
{
	public class ShipperStatusViewModel
    {
		public ShipperViewModel Shipper { get; set; }
        public string Status { get; set; }
    }
}
