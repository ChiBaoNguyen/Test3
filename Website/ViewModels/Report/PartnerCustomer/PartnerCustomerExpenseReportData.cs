using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.Container;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Expense;
using Website.ViewModels.Order;

namespace Website.ViewModels.Report.PartnerCustomer
{
	public class PartnerCustomerExpenseReportData
	{
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public string TaxMethodI { get; set; }
		public decimal TaxRate { get; set; }
		public string TaxRoundingI { get; set; }
		public ContainerViewModel OrderD { get; set; }
		public OrderViewModel OrderH { get; set; }
		public DispatchViewModel Dispatch { get; set; }
	}
}
