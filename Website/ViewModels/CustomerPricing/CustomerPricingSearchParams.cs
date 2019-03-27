using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.CustomerPricing
{
	public class CustomerPricingSearchParams
	{
		public int Page { get; set; }
		public int ItemsPerPage { get; set; }
		public string SortBy { get; set; }
		public bool Reverse { get; set; }
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string ContainerSizeI { get; set; }
		public string ContainerTypeC { get; set; }
		public string Location1C { get; set; }
		public string Location2C { get; set; }
	}
}
