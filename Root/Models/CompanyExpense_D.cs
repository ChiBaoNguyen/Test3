using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public class CompanyExpense_D
	{
		public int Id { get; set; }
		public DateTime InvoiceD { get; set; }
		public string ExpenseC { get; set; }
		public string EmployeeC { get; set; }
		public string PaymentMethodI { get; set; }
		public string SupplierMainC { get; set; }
		public string SupplierSubC { get; set; }
		public decimal Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Total { get; set; }
		public decimal Tax { get; set; }
		public string Description { get; set; }
		public string EntryClerkC { get; set; }
		public string IsAllocated { get; set; }
	}
}
