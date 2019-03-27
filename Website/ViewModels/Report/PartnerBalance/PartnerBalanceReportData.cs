using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.PartnerBalance
{
	public class PartnerBalanceReportData
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public string PartnerN { get; set; }
		public decimal? TransportFee { get; set; } // Chi phi van chuyen
		public decimal? CustomerExpense { get; set; }
		public decimal? PartnerFee { get; set; } // Tien thue xe
		public decimal? PartnerExpense { get; set; }
		public decimal? PartnerDiscount { get; set; }
		public decimal? PartnerSurcharge { get; set; }
		public List<PartnerBalanceReportExpense> CustomerExpenseList { get; set; } // Chi phi
		public List<PartnerBalanceReportExpense> PartnerExpenseList { get; set; } // Chi phi thu
		public List<PartnerBalanceReportExpense> PartnerSurchargeList { get; set; } // Chi phi thu
		
		
	}
}
