using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.Liabilities
{
	public class LiabilitiesReportData
	{
		public DateTime? LiabilitiesD { get; set; }
		public string DriverC { get; set; }
		public string FirstN { get; set; }
		public string LastN { get; set; }
		public decimal? PreviousBalance { get; set; }
		public decimal? AdvanceAmount { get; set; }
		public decimal? ExpenseAmount { get; set; }
		public string Description { get; set; }
	}

	public class LiabilitiesData
	{
		public string DriverC { get; set; }
		public string FirstN { get; set; }
		public string LastN { get; set; }
		public DateTime? LiabilitiesD { get; set; }
		public decimal? Amount { get; set; }
		public string Description { get; set; }
	}

	public class LiabilitiesPaymentData
	{
		public string DriverC { get; set; }
		public string FirstN { get; set; }
		public string LastN { get; set; }
		public DateTime? LiabilitiesD { get; set; }
		public decimal? LiabilitiesAmount { get; set; }
		public List<LiabilitiesPaymentDetailData> PaymentDetailList { get; set; }
		public string ReceiptNo { get; set; }
		public int LiabilitiesNo { get; set; }
		public string LiabilitiesI { get; set; }
		public string LiabiltiesContent { get; set; }
	}

	public class LiabilitiesPaymentDetailData
	{
		public string TruckNo { get; set; }
		public string TrailerNo { get; set; }
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public string ContainerNo { get; set; }
		public string ContainerSizeI { get; set; }
		public string CustomerN { get; set; }
		public string CustomerShortN { get; set; }
		public string Location1N { get; set; }
		public string Location2N { get; set; }
		public string Location3N { get; set; }		
		public DateTime? Location1DT { get; set; }
		public DateTime? Location2DT { get; set; }
		public DateTime? Location3DT { get; set; }
		public string Content { get; set; }
		public decimal? ItemAmount { get; set; }
		public decimal? TotalItemAmount { get; set; }
		public DateTime? TransportD { get; set; }
	}
}
