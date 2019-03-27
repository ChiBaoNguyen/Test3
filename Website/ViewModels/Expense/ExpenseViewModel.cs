using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Expense
{
	public class ExpenseViewModel
	{
		public string ExpenseC { get; set; }
		public string ExpenseN { get; set; }
		public string CategoryI { get; set; }
		public string CategoryC { get; set; }
		public string CategoryN { get; set; }
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
		public int ExpenseIndex { get; set; }
		public decimal? Quantity { get; set; }
		public decimal? Amount { get; set; }
		public string ContainerSizeI { get; set; }
		public decimal? AmountMoney { get; set; }
		public string Category { get; set; }
		public string LocationC { get; set; }
		public string Display { get; set; }
		public string CategoryExpense { get; set; }
		public string ViewReport { get; set; }
		public string ColumnName { get; set; }
	}
}