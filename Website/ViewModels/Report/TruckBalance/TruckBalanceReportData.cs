using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.TruckBalance
{
	public class TruckBalanceReportData
	{
		public List<TruckBalanceReportTruckList> TruckList { get; set; }
		public List<TruckBalanceReportTransportFee> TransportFees { get; set; } // Chi phi thu
		public List<TruckBalanceReportTransportFee> SurchargeFees { get; set; } // Chi phi thu ngay luu cont
		public List<TruckBalanceReportExpense> ExpensesInclude { get; set; } // Chi phi thu 
		public List<TruckBalanceReportExpense> ExpensesRequest { get; set; } // Chi phi chung
		public List<TruckBalanceReportExpense> MaintenanceExpenses { get; set; } // Chi phi bao duong, sua chua
		public List<TruckBalanceReportExpense> FixedExpenses { get; set; } // Chi phi co dinh
		public List<TruckBalanceReportExpense> BeforeFixedExpenses { get; set; } // Chi phi co dinh
		public List<TruckBalanceReportExpense> AfterFixedExpenses { get; set; } // Chi phi co dinh
	}
}
