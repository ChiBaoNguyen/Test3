using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Surcharge
{
	public class TransportConfirmSurchargeViewModel
	{
		public List<SurchargeDetailViewModel> ExpenseDetailList { get; set; }
		public int DispatchNo { get; set; }
		public string DispatchI { get; set; }
	}
}
