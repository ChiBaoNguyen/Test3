using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.DriverAllowance
{
	public class AllowanceViewModel
	{
		public string DepartureC { get; set; }
		public string DepartureN { get; set; }
		public string DestinationC { get; set; }
		public string DestinationN { get; set; }
		public string ContainerTypeC { get; set; }
		public string ContainerTypeN { get; set; }
		public string ContainerSizeI { get; set; }
		public string ContainerSizeN { get; set; }
		public Nullable<decimal> UnitPrice { get; set; }
		public string ContainerSize { get; set; }
		public decimal? Empty { get; set; }
		public decimal? ShortRoad { get; set; }
		public decimal? LongRoad { get; set; }
		public decimal? GradientRoad { get; set; }
		public int DisplayLineNo { get; set; }
		public decimal? EmptyGoods { get; set; }
	}
}