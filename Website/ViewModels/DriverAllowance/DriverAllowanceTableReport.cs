using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.DriverAllowance
{
	public class DriverAllowanceTableReport
	{
		public DateTime OrderD { get; set; }
		public List<DriverAllowanceListReport> Items { get; set; } 
	}
}
