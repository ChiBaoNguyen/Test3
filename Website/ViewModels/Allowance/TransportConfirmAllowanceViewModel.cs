using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Allowance
{
	public class TransportConfirmAllowanceViewModel
	{
		public List<AllowanceDetailViewModel> ExpenseDetailList { get; set; }
		public int DispatchNo { get; set; }
	}
}
