using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.CustomerBalance
{
	public class CustomerBalanceReportData
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string CustomerN { get; set; }
		public decimal? Amount { get; set; } // Chi phi van chuyen yeu cau KH thanh toan
		//public decimal? TransportFee { get; set; } // Chi phi van chuyen
		public decimal? PartnerAmount { get; set; } // Tien thue xe
		public decimal? CustomerSurcharge { get; set; } // Chi phi phu thu
		public decimal? DetainAmount { get; set; } // Chi phi luu cont
		public decimal? TotalExpense { get; set; }
		public decimal? IncludedExpense { get; set; }
		public decimal? CustomerDiscount { get; set; }
		public decimal? DriverAllowance { get; set; }
		public decimal? PartnerSurcharge { get; set; }
		public decimal? PartnerExpense { get; set; }
		public List<CustomerBalanceReportExpense> TotalExpenseList { get; set; } // Chi phi
		public List<CustomerBalanceReportExpense> IncludedExpenseList { get; set; } // Chi phi thu
		public List<CustomerBalanceReportExpense> CustomerSurchargeList { get; set; } // Chi phi thu
		
		
	}
}
