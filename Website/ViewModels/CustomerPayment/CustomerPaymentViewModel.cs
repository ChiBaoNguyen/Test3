using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.CustomerPayment
{
	public class CustomerPaymentViewModel
	{
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string CustomerN { get; set; }
		public DateTime? CustomerPaymentD { get; set; }
		public string PaymentId { get; set; }
		//public decimal? PreviousBalance { get; set; }
		public decimal? Amount { get; set; }
		//public decimal? NextBalance { get; set; }
		public string Description { get; set; }
		public int CustomerPaymentIndex { get; set; }
		public int Status { get; set; }
		public string EntryClerkC { get; set; }
		public string EntryClerkN { get; set; }
		public string PaymentMethodI { get; set; }
	}
}
