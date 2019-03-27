using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public partial class PartnerContractTariffPattern_M
	{
        public string PartnerMainC { get; set; }
        public string PartnerSubC { get; set; }
		public DateTime ApplyD { get; set; }
		public string DepartureC { get; set; }
		public string DestinationC { get; set; }
		public string ContainerTypeC { get; set; }
		public string ContainerSizeI { get; set; }
		public Nullable<decimal> UnitPrice { get; set; }
	}
}
