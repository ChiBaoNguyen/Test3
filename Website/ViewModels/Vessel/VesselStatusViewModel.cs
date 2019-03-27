using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.Order;
using Website.ViewModels.Ship;

namespace Website.ViewModels.Vessel
{
	public class VesselStatusViewModel
    {
		public VesselViewModel Vessel { get; set; }

        public string Status { get; set; }
    }
}
