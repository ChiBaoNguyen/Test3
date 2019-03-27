using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.OrderPattern
{
    public class PartnerContractTariffPatternViewModel
	{
        public string PartnerMainC { get; set; }
        public string PartnerSubC { get; set; }
        public string PartnerN { get; set; }	    
		public DateTime ApplyD { get; set; }
		public string DepartureC { get; set; }
        public string DepatureN { get; set; }
		public string DestinationC { get; set; }
        public string DestinationN { get; set; }
		public string ContainerTypeC { get; set; }
        public string ContainerTypeN { get; set; }
		public string ContainerSizeI { get; set; }
		public Nullable<decimal> UnitPrice { get; set; }
	}
}
