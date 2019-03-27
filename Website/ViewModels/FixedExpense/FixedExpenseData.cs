using System;
using System.Collections.Generic;

namespace Website.ViewModels.FixedExpense
{
	public class FixedExpenseData
	{
		public List<FixedExpenseViewModel> FixedExpenseList { get; set; }
		public string DepC { get; set; }
		public int Year { get; set; }
		public string ExpenseC { get; set; }
		public string EntryClerkC { get; set; }
	}
}
