using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public partial class SupplierPayment_D
	{
		public string SupplierMainC { get; set; }
		public string SupplierSubC { get; set; }
		public string PaymentId { get; set; }
		public DateTime SupplierPaymentD { get; set; }
		public decimal? Amount { get; set; }
		public string Description { get; set; }
		public string EntryClerkC { get; set; }
		public string PaymentMethodI { get; set; }
	}
}
