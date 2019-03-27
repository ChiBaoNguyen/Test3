using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public partial class DriverAllowance_M
	{
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public DateTime ApplyD { get; set; }
		public string UnitPriceMethodI { get; set; }
		public string DepartureC { get; set; }
		public string DestinationC { get; set; }
		public string ContainerTypeC { get; set; }
		public string ContainerSizeI { get; set; }
		public decimal? UnitPrice { get; set; }
		public decimal? UnitPriceRate { get; set; }
		public string ContainerSize { get; set; }
		public decimal? Empty { get; set; }
		public decimal? ShortRoad { get; set; }
		public decimal? LongRoad { get; set; }
		public decimal? GradientRoad { get; set; }
		public int DisplayLineNo { get; set; }
		public int DriverAllowanceId { get; set; }
		public decimal? EmptyGoods { get; set; }
	}
}