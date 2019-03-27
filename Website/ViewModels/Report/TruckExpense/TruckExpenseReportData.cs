using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.TruckExpense
{
	public class TruckExpenseReportData
	{
		public List<TruckExpenseReportTruckList> TruckList { get; set; }
		public List<TruckExpenseReportContainerCount> CountainerCount { get; set; } 
		public List<TruckExpenseReportTransportFee> TransportFees { get; set; } // Chi phi thu
		public List<TruckExpenseReportExpense> Expenses { get; set; } // Chi phi chung
		public List<TruckExpenseReportExpense> MaintenanceExpenses { get; set; } // Chi phi bao duong, sua chua
		public List<TruckExpenseReportExpense> FixedExpenses { get; set; } // Chi phi co dinh
		public List<TruckExpenseReportExpenseDetail> ExpenseDetail { get; set; } // Chi tiết chi phí nội bộ
	}
}
