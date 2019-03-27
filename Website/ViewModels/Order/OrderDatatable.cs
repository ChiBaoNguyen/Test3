using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Order
{
	public class OrderDatatable
	{
		public List<OrderViewModel> Data { get; set; }
		public int Total { get; set; }
	}
}
