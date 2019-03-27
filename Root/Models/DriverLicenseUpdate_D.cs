using System;
using System.Collections.Generic;

namespace Root.Models
{
	public partial class DriverLicenseUpdate_D
	{
		public string DriverC { get; set; }
		public string LicenseC { get; set; }
		public DateTime ExpiryD { get; set; }
		public DateTime? UpdateD { get; set; }
		public DateTime NextExpiryD { get; set; }
	}
}