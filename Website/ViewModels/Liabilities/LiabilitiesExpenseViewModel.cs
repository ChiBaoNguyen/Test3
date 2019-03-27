using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Liabilities
{
	public class LiabilitiesExpenseViewModel
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public string ContainerNo { get; set; }
		public int ExpenseNo { get; set; }
		public DateTime? TransportD { get; set; }
		public string ContainerSizeI { get; set; }
		//public string ContainerSizeN { get; set; }
		public string ExpenseC { get; set; }
		public string ExpenseN { get; set; }
		public decimal? Amount { get; set; }
		public decimal? TaxAmount { get; set; }
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string CustomerN { get; set; }
		public int LiabilitiesNo { get; set; }
		public string LiabilitiesStatusI { get; set; }
		public DateTime? LiabilitiesD { get; set; }

	}
}
