using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Ship
{
    public class VesselViewModel
    {
        public string VesselC { get; set; }
        public string VesselN { get; set; }
		public string ShippingCompanyC { get; set; }
		public string ShippingCompanyN { get; set; }
		public string IsActive { get; set; }
		public int VesselIndex { get; set; }
    }
}
