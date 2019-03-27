using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.Allowance;
using Website.ViewModels.Container;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Expense;
using Website.ViewModels.Surcharge;

namespace Website.ViewModels.Order
{
	public class TransportConfirmViewModel
	{
		public TransportConfirmOrderViewModel TransportConfirmOrder { get; set; }
		public TransportConfirmContainerViewModel TransportConfirmContainer { get; set; }
		public List<TransportConfirmDispatchViewModel> TransportConfirmDispatchList { get; set; }
		public List<TransportConfirmExpenseViewModel> TransportConfirmExpensesList { get; set; }
		public List<TransportConfirmSurchargeViewModel> TransportConfirmSurchargeList { get; set; }
		public List<TransportConfirmAllowanceViewModel> TransportConfirmAllowanceList { get; set; }
		public int NewOrderDetailNo { get; set; }
		public bool IsMaxDetailNo { get; set; }
		public bool IsMinDetailNo { get; set; }
		public int ContainerIndex { get; set; }
        public string OrderImageKey { get; set; }
        public int ImageCount { get; set; }

	}
}
