using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.PartnerExpense
{
	public class PartnerExpenseReportParam
	{
		public string ReportI { get; set; }
		public string DepC { get; set; }
		public DateTime? TransportDFrom { get; set; }
		public DateTime? TransportDTo { get; set; }
		public DateTime? TransportM { get; set; }
		public string Partner { get; set; }
		public string Languague { get; set; }
	}
}
