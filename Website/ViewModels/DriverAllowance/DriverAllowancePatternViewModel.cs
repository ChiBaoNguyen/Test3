using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.OrderPattern;

namespace Website.ViewModels.DriverAllowance
{
	public class DriverAllowancePatternViewModel
	{
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string CustomerN { get; set; }
		public DateTime ApplyD { get; set; }
		public string UnitPriceMethodI { get; set; }
		public decimal? UnitPriceRate { get; set; }
		public int DriverAllowanceIndex { get;set;}
		public List<AllowanceViewModel> TarrifPatterns { get; set; }
	}
}
