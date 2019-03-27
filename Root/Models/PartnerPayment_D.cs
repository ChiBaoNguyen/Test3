using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public partial class PartnerPayment_D
	{
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public string PaymentId { get; set; }
		public DateTime PartnerPaymentD { get; set; }
		public decimal? Amount { get; set; }
		public string Description { get; set; }
		public string EntryClerkC { get; set; }
		public string PaymentMethodI { get; set; }
	}
}
