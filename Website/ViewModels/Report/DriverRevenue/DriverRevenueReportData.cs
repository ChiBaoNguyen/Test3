using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.DriverRevenue
{
	public class DriverRevenueReportData
	{
		public DateTime? TransportD { get; set; }
		public int ContainerSize20 { get; set; }
		public int ContainerSize40 { get; set; }
		public int ContainerSize45 { get; set; }
		public decimal Load { get; set; }
		public string OrderTypeId { get; set; }
		public string Location { get; set; }
		public decimal? Amount { get; set; }
		public string DriverC { get; set; }
		public string FirstN { get; set; }
        public string LastN { get; set; }
        public string CustomerN { get; set; }
        public string ContainerNo { get; set; }
        public string ContainerSizeI { get; set; }
        public decimal? DriverAllowance { get; set; }
        
	}
}
