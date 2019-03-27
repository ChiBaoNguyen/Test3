
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Allowance
{
	public class AllowanceDetailViewModel
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public int AllowanceNo { get; set; }
		public string AllowanceC
		{
			get { return ExpenseC; }
			set { ExpenseC = value; }
		}
		public string AllowanceN
		{
			get { return ExpenseN; }
			set { ExpenseN = value; }
		}
		public string ExpenseC { get; set; }
		public string ExpenseN { get; set; }
		public string Unit { get; set; }
		public decimal? UnitPrice { get; set; }
		public decimal? Quantity { get; set; }
		public decimal? Amount { get; set; }
		public string Description { get; set; }
	}
}
