using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.CustomerPricing
{
	public class CustomerPricingViewModel
	{
		public string CustomerPricingId { get; set; }
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string CustomerN { get; set; }
		public string ContainerSizeI { get; set; }
		public string ContainerTypeC { get; set; }
		public string ContainerTypeN { get; set; }
		public string Location1C { get; set; }
		public string Location1N { get; set; }
		public string Location2C { get; set; }
		public string Location2N { get; set; }
		public decimal? GrossProfitRatio { get; set; }
		public decimal? TotalExpense { get; set; }
		public decimal? EstimatedPrice { get; set; }
		public DateTime EstimatedD { get; set; }
		public int Index { get; set; }
		public List<CustomerPricingDetailViewModel> ExpenseList { get; set; }
		public List<CustomerPricingDetailViewModel> AllowanceList { get; set; }
		public List<CustomerPricingDetailViewModel> FixedExpenseList { get; set; }
		public List<CustomerPricingDetailViewModel> OtherExpenseList { get; set; }
	}
}
