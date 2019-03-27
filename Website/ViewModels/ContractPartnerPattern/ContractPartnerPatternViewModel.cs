using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.ContractPartnerPattern
{
	public class ContractPartnerPatternViewModel
	{
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public string PartnerN { get; set; }
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string CustomerN { get; set; }
		public DateTime ApplyD { get; set; }
		public string DepartureC { get; set; }
		public string DepatureN { get; set; }
		public string DestinationC { get; set; }
		public string DestinationN { get; set; }
		public string ContainerSizeI { get; set; }
		public Nullable<decimal> UnitPrice { get; set; }
		public string CalculateByTon { get; set; }
	}
}
