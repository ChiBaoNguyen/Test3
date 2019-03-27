using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public partial class Liabilities_D
	{
		public Liabilities_D()
		{
			LiabilitiesNo = 1;
		}

		public string DriverC { get; set; }
		public DateTime LiabilitiesD { get; set; }
		public string LiabilitiesI { get; set; }
		public int LiabilitiesNo { get; set; }
		public string ReceiptNo { get; set; }
		public decimal? Amount { get; set; }
		public string Description { get; set; }

	}
}
