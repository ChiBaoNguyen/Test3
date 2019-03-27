using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public partial class ContractTariffPattern_M
	{
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public DateTime ApplyD { get; set; }
		public string DepartureC { get; set; }
		public string DestinationC { get; set; }
		public string ContainerSizeI { get; set; }
		public Nullable<decimal> UnitPrice { get; set; }
		public int DisplayLineNo { get; set; }
		public string CommodityC { get; set; }
		public string CommodityN { get; set; }
		public string CalculateByTon { get; set; }
	}
}
