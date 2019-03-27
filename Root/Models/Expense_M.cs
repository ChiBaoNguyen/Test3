using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public partial class Expense_M
	{
		public string ExpenseC { get; set; }
		public string ExpenseN { get; set; }
		public string CategoryI { get; set; }
		public string CategoryC { get; set; }
		public string ExpenseI { get; set; }
		public string Unit { get; set; }
		public decimal? UnitPrice { get; set; }
		public decimal? TaxRate { get; set; }
		public string PaymentMethodI { get; set; }
		public string IsIncluded { get; set; }
		public string IsRequested { get; set; }
		public string IsPayable { get; set; }
		public string Description { get; set; }
		public string TaxRoundingI { get; set; }
		public string ViewReport { get; set; }
		public string ColumnName { get; set; }
	}
}
