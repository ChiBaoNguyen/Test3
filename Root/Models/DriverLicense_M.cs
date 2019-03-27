using System;
using System.Collections.Generic;

namespace Root.Models
{
	public partial class DriverLicense_M
	{
		public string DriverC { get; set; }
		public string LicenseC { get; set; }
		public string DriverLicenseNo { get; set; }
		public DateTime? DriverLicenseD { get; set; }
		public DateTime ExpiryD { get; set; }
	}
}