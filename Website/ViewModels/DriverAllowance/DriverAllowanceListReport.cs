using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.Driver;
using Website.ViewModels.Liabilities;

namespace Website.ViewModels.DriverAllowance
{
	public class DriverAllowanceListReport
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public string CustomerN { get; set; }
		public string ContainerNo { get; set; }
		public DateTime TransportD { get; set; }
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public string Location1 { get; set; }
		public string Location2 { get; set; }
		public string Location3 { get; set; }
		public string DepN { get; set; }
		public decimal DriverAllowance { get; set; }
		public string ExpenseN { get; set; }
		public decimal Amount { get; set; }
		public decimal NetWeight { get; set; }
		public decimal AllowanceOfDriver { get; set; }
		public List<DriverAllowanceReport> DriverAllowanceList { get; set; }
		public List<LiabilitiesViewModel> LiabilitiesDetailList { get; set; }

	}
}
