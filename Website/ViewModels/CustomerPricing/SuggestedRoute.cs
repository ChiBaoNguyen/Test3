using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.CustomerPricing
{
	public class SuggestedRoute
	{
		public string RouteId { get; set; }
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public string RouteN { get; set; }
		public string CustomerN { get; set; }
		public string Location1C { get; set; }
		public string Location1N { get; set; }
		public string Location2C { get; set; }
		public string Location2N { get; set; }
		public string ContainerSizeI { get; set; }
		public string ContainerTypeC { get; set; }
		public string ContainerTypeN { get; set; }
		public decimal? TotalExpense { get; set; }
		public string IsEmpty { get; set; }
		public string IsHeavy { get; set; }
		public string IsSingle { get; set; }
		public bool IsHistoryRoute { get; set; }
		public bool IsSelected { get; set; }
	}
}
