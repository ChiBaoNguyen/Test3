using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Models;

namespace Website.ViewModels.CompanyExpense
{
	public class CompanyExpenseViewModel: CompanyExpense_D
	{
		public string ExpenseN { get; set; }
		public string EmployeeN { get; set; }
		public string SupplierN { get; set; }
		public string SupplierShortN { get; set; }
		public string EntryClerkN { get; set; }
		public decimal? TaxRate { get; set; }
		public string TaxRoundingI { get; set; }
		public int CompanyExpenseIndex { get; set; }
		public string IsAllocated { get; set; }
		public string PaymentMethodI { get; set; }
	}
}
