using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public partial class Trailer_M
	{
		public string TrailerC { get; set; }
		public string TrailerNo { get; set; }
		public Nullable<DateTime> RegisteredD { get; set; }
		public string VIN { get; set; }
		public string DriverC { get; set; }
		public string ModelC { get; set; }
		public string IsUsing { get; set; }
		public string UsingDriverC { get; set; }
		public string IsActive { get; set; }
		public Nullable<decimal> GrossWeight { get; set; }
		public string Situation { get; set; }
		public Nullable<DateTime> FromDate { get; set; }
		public Nullable<DateTime> ToDate { get; set; }
	}
}
