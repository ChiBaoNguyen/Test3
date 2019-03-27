using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.Container;
namespace Website.ViewModels.Order
{
	public class OrderDetailListReport
	{
		public OrderViewModel OrderH { get; set; }
		public List<ContainerViewModel> OrderDate { get; set; }
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public string CustomerShortN { get; set; }
		public string BLBK { get; set; }
		public string OrderTypeI { get; set; }
		public string ShippingCompanyN { get; set; }
		public string VesselN { get; set; }
		public string LoadingPlaceN { get; set; }
		public string StopoverPlaceN { get; set; }
		public string DischargePlaceN { get; set; }
		public string JobNo { get; set; }
		public int? Quantity20HC { get; set; }
		public int? Quantity40HC { get; set; }
		public int? Quantity45HC { get; set; }
		public decimal? TotalLoads { get; set; }
		public string ContainerSizeI { get; set; }
		public decimal? NetWeight { get; set; }
		public decimal? TotalPrice { get; set; }
		public decimal? UnitPrice { get; set; }
	}
}
