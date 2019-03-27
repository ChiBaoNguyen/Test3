using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Expense
{
	public class ExpenseDetailListReport
	{
		//public DateTime OrderD { get; set; }
		//public string OrderNo { get; set; }
		public DateTime? InvoiceD { get; set; }
		public string CategoryN { get; set; }
		public string ExpenseN { get; set; }
		public string PaymentMethodI { get; set; }
		public DateTime? TransportD { get; set; }
		public string RegisteredNo { get; set; }
		public string TrailerNo { get; set; }
		public string DriverN { get; set; }
		public string SupplierShortN { get; set; }
		public string SupplierN { get; set; }
		public string EntryClerkN { get; set; }
		public decimal? UnitPrice { get; set; }
		public decimal? Quantity { get; set; }
		public decimal? TotalAmount { get; set; }
		public decimal? TaxAmount { get; set; }
		public string Description { get; set; }
		public string Location1N { get; set; }
		public string Location2N { get; set; }
		public string Location3N { get; set; }
		public string OrderTypeI { get; set; }
		public string ContainerSizeI { get; set; }
		public string ContainerNo { get; set; }
		public decimal? NetWeight { get; set; }
	}
}