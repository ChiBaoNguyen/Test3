using System;
using System.Collections.Generic;

namespace Root.Models
{
	public partial class Driver_M
	{
		public string DriverC { get; set; }
		public string FirstN { get; set; }
		public string LastN { get; set; }
		public string DepC	 { get; set; }
		public Nullable<System.DateTime> BirthD { get; set; }
		public string Address { get; set; }
		public string PhoneNumber { get; set; }
		public Nullable<System.DateTime> JoinedD { get; set; }
		public Nullable<System.DateTime> RetiredD { get; set; }
		public decimal? AdvancePaymentLimit { get; set; }
		public string IsActive { get; set; }
		public decimal? BasicSalary { get; set; }
	}
}
