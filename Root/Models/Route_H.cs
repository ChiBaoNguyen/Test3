using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public class Route_H
	{
		public string RouteId { get; set; }
		public string Location1C { get; set; }
		public string Location2C { get; set; }
		public string ContainerTypeC { get; set; }
		public string ContainerSizeI { get; set; }
		public string IsEmpty { get; set; }
		public string IsHeavy { get; set; }
		public string IsSingle { get; set; }
		public string RouteN { get; set; }
		public decimal? TotalExpense { get; set; }
	}
}
