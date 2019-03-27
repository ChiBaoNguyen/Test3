using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.CustomerPricing
{
	public class SuggestedExpenseList
	{
		public List<CustomerPricingDetailViewModel> ExpenseList { get; set; }
		public List<CustomerPricingDetailViewModel> AllowanceList { get; set; }
		public List<CustomerPricingDetailViewModel> FixedExpenseList { get; set; }
		public List<CustomerPricingDetailViewModel> OtherExpenseList { get; set; }
	}
}
