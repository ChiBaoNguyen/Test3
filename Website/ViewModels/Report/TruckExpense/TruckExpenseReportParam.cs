using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.TruckExpense
{
	public class TruckExpenseReportParam
	{
		public string DepC { get; set; }
		public DateTime DateFrom { get; set; }
		public DateTime DateTo { get; set; }
		public string Languague { get; set; }
	}
}
