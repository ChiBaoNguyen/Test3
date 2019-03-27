using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public class SupplierBalance_D
	{
		public string SupplierMainC { get; set; }
		public string SupplierSubC { get; set; }
		public DateTime SupplierBalanceD { get; set; }
		public decimal? TotalAmount { get; set; }
		public decimal? TaxAmount { get; set; }
		public decimal? PaymentAmount { get; set; }
	}
}
