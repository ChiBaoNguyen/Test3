using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.CustomerPayment
{
	public class CustomerPaymentReportData
	{
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string CustomerN { get; set; }
		public decimal? OpeningBalance { get; set; }
		public decimal? ClosingBalance { get; set; }
		public decimal? OwnExpense { get; set; }
		public decimal? PayOnBehalf { get; set; }
		public decimal? OweExpense { get; set; }
		public string CommodityN { get; set; }
		public string ContainerSizeI { get; set; }
		public decimal? ContainerCount { get; set; }
		public decimal? NetWeight { get; set; }
		public decimal? UnitPrice { get; set; }
		public int No { get; set; }
		public List<CustomerPaymentDetailReportData> OweData { get; set; }
		public List<CustomerPaymentDetailReportData> OwnData { get; set; }
	}
}
