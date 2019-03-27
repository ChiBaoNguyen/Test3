using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Expense
{
	public class ExpenseListReport
	{
		public DateTime OrderD { get; set; }
		public DateTime? InvoiceD { get; set; }
		public DateTime? TransportD { get; set; }
		public string OrderNo { get; set; }
		public string ExpenseN { get; set; }
		public string ExpenseC { get; set; }
		public decimal Amount { get; set; }
		public decimal TaxAmount { get; set; }
		public decimal IsIncluded { get; set; }
		public decimal IsRequested { get; set; }
		public decimal IsPayable { get; set; }
		public string PaymentMethod { get; set; }

		public decimal? DriverAllowance { get; set; }
		public decimal? PartnerExpense { get; set; }
		public decimal? PartnerSurcharge { get; set; }
		public string Description { get; set; }
	}
}
