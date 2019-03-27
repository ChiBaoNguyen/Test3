using System;

namespace Website.ViewModels.DriverLicense
{
	public class DriverLicenseViewModel
	{
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public string LicenseC { get; set; }
		public string LicenseN { get; set; }
		public string DriverLicenseNo { get; set; }
		public DateTime? DriverLicenseD { get; set; }
		public DateTime ExpiryD { get; set; }
	}
}