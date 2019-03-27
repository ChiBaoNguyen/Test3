using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Expense
{
	public class TransportConfirmExpenseViewModel
	{
		public List<ExpenseDetailViewModel> ExpenseDetailList { get; set; }
		public int DispatchNo { get; set; }
	}
}
