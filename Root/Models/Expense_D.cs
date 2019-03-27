using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public partial class Expense_D
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public int ExpenseNo { get; set; }
		public string ExpenseC { get; set; }
		public string PaymentMethodI { get; set; }
		public DateTime? InvoiceD { get; set; }
		public string SupplierMainC { get; set; }
		public string SupplierSubC { get; set; }
		public string Unit { get; set; }
		public decimal? UnitPrice { get; set; }
		public decimal? Quantity { get; set; }
		public decimal? Amount { get; set; }
		public decimal? TaxAmount { get; set; }
		public decimal? TaxRate { get; set; }
		public string IsIncluded { get; set; }
		public string IsRequested { get; set; }
		public string IsPayable { get; set; }
		public string EntryClerkC { get; set; }
		public string Description { get; set; }

	}
}
