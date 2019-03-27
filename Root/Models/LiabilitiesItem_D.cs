using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public class LiabilitiesItem_D
	{
		public LiabilitiesItem_D()
		{
			LiabilitiesNo = 1;
		}
		public DateTime LiabilitiesD { get; set; }
		public int LiabilitiesNo { get; set; }
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public int ExpenseNo { get; set; }
		public string LiabilitiesStatusI { get; set; }
	}
}
