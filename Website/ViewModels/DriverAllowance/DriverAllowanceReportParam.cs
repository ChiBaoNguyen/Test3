using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.DriverAllowance
{
	public class DriverAllowanceReportParam
	{
		public string DepC { get; set; }
		public DateTime AllowanceDFrom { get; set; }
		public DateTime AllowanceDTo { get; set; }
		public string AllowanceType { get; set; }
		public string Laguague { get; set; }
	}
}
