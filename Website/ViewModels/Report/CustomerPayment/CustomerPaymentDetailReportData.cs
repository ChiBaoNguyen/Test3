using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.CustomerPayment
{
	public class CustomerPaymentDetailReportData
	{
		public DateTime? PaymentD { get; set; }
		public string Content { get; set; }
		public decimal? Amount { get; set; }
		public decimal? TotalExpense { get; set; }
		public decimal? CustomerSurcharge { get; set; }
		public decimal? DetainAmount { get; set; }
		public decimal? CustomerDiscount { get; set; }
		public decimal? TaxAmount { get; set; }
		public decimal? TotalAmount { get; set; }
		public decimal? PaymentAmount { get; set; }
		public int PaymentY { get; set; }
		public int PaymentM { get; set; }
	}
}
