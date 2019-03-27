using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public partial class TruckExpense_D
	{
		public int Id { get; set; }
		public DateTime InvoiceD { get; set; }
		public DateTime TransportD { get; set; }
		public string Code { get; set; }
		public string ExpenseC { get; set; }
		public string EntryClerkC { get; set; }
		public string DriverC { get; set; }
		public string PaymentMethodI { get; set; }
		public string SupplierMainC { get; set; }
		public string SupplierSubC { get; set; }
		public Nullable<decimal> Quantity { get; set; }
		public Nullable<decimal> UnitPrice { get; set; }
		public Nullable<decimal> Total { get; set; }
		public Nullable<decimal> Tax { get; set; }
		public string Description { get; set; }
        public string ObjectI { get; set; }
		public string IsAllocated { get; set; }

	}
}
