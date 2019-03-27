using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.OrderPattern
{
    public class PartnerContractTariffViewModel
	{
        public string PartnerMainC { get; set; }
        public string PartnerSubC { get; set; }
        public string PartnerN { get; set; }	    
		public DateTime ApplyD { get; set; }
	    public List<TariffPatternViewModel> TarrifPatterns { get; set; }
	}
}
