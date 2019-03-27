using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Website.ViewModels.ContractPartnerPattern
{
	public class PartnerPatternViewModel
	{
		public string DepartureC { get; set; }
		public string DepartureN { get; set; }
		public string DestinationC { get; set; }
		public string DestinationN { get; set; }
		public string ContainerSizeI { get; set; }
		public Nullable<decimal> UnitPrice { get; set; }
		public int DisplayLineNo { get; set; }
		public string CalculateByTon { get; set; }
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string CustomerN { get; set; }
	}
}