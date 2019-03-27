using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.Container;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Expense;
using Website.ViewModels.Order;

namespace Website.ViewModels.Report.PartnerExpense
{
	public class PartnerExpenseReportData
	{
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public string PartnerN { get; set; }
		public string PartnerShortN { get; set; }
		public decimal? PartnerFee { get; set; }
		public decimal? PartnerExpense { get; set; }
		public decimal? PartnerSurcharge { get; set; }
		public decimal? PartnerDiscount { get; set; }
		public decimal PartnerToTalTax { get; set; }
		public int ContainerAmount { get; set; }
		public string TaxMethodI { get; set; }
		public decimal TaxRate { get; set; }
		public string TaxRoundingI { get; set; }
	}

	public class PartnerExpenseDetailReportData
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public string PartnerN { get; set; }
		public string Address { get; set; }
		public decimal? PartnerFee { get; set; }
		public decimal? PartnerExpense { get; set; }
		public decimal? PartnerSurcharge { get; set; }
		public decimal? PartnerDiscount { get; set; }
		public decimal PartnerTaxAmount { get; set; }
		public DateTime? TransportD { get; set; }
		public string CustomerN { get; set; }
		public string ContainerNo { get; set; }
		public string ContainerSizeI { get; set; }
		public List<ExpenseDetailViewModel> PartnerExpenseList { get; set; }
		public decimal TaxRate { get; set; }
		public string TaxRoundingI { get; set; }
	}
}