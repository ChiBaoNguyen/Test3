using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Liabilities
{
	public class LiabilitiesViewModel
	{
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public DateTime? RetiredD { get; set; }
		public DateTime LiabilitiesD { get; set; }
		public string LiabilitiesI { get; set; }
		public int LiabilitiesNo { get; set; }
		public string ReceiptNo { get; set; }
		public decimal? PreviousBalance { get; set; }
		public decimal? Amount { get; set; }
		public decimal? NextBalance { get; set; }
		public decimal? AdvancePaymentLimit { get; set; }
		public string Description { get; set; }
		public int LiabilitiesIndex { get; set; }
		public List<LiabilitiesExpenseViewModel> ExpenseList { get; set; }
		public decimal? TotalExpense { get; set; }
		public int Status { get; set; }
	}
}
