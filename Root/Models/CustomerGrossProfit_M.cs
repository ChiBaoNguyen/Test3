using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public partial class CustomerGrossProfit_M
	{
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public DateTime ApplyD { get; set; }
		public decimal? GrossProfitRatio { get; set; }
		public string Description { get; set; }
	}
}
