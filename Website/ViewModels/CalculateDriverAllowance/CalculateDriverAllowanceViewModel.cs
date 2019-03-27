using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.CalculateDriverAllowance
{
	public class CalculateDriverAllowanceViewModel
	{
		public string CalculateC { get; set; }
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public DateTime? ApplyD { get; set; }
		public string Description { get; set; }
		public string Content { get; set; }
		public string TakeABreak { get; set; }
		public decimal? AmountMoney { get; set; }
		public decimal? AmountMoneySubtract { get; set; }
		public int CalculateDriverAllowanceIndex { get; set; }
		public string CalculateSalary { get; set; }
	}
}
