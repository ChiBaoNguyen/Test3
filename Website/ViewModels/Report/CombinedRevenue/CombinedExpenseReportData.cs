using System;

namespace Website.ViewModels.Report.CombinedExpense
{
	public class CombinedExpenseReportData
	{
		public string ObjectN { get; set; }
		public int TransportCount { get; set; }
		public decimal TotalWeight { get; set; }
		public decimal Revenue { get; set; }
		public int LastYearTransportCount { get; set; }
		public decimal LastYearRevenue { get; set; }
		public int Month { get; set; }
	}
}