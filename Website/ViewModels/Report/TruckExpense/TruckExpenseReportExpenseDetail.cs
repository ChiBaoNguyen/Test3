using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.TruckExpense
{
	public class TruckExpenseReportExpenseDetail
	{
		public string TruckC { get; set; }
		public string RegisteredNo { get; set; }
		public string CategoryC { get; set; }
		public string CategoryN { get; set; }
		public decimal? Amount { get; set; }
		public DateTime? OrderD { get; set; }
		public string OrderNo { get; set; }
		public int? DetailNo { get; set; }
		public int? DispatchNo { get; set; }
	}
}
