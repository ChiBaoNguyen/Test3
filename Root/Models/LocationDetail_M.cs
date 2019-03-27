using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public partial class LocationDetail_M
	{
		public string LocationC { get; set; }
		public string ExpenseC { get; set; }
		public string ExpenseN { get; set; }
		public string ContainerSizeI { get; set; }
		public decimal? AmountMoney { get; set; }
		public string Category { get; set; }
		public string Display { get; set; }
		public string Description { get; set; }
		public string CategoryExpense { get; set; }
		public int LocationDetailId { get; set; }

	}
}
