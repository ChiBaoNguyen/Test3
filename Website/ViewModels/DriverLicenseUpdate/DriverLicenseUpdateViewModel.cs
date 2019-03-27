using System;

namespace Website.ViewModels.DriverLicenseUpdate
{
	public class DriverLicenseUpdateViewModel
	{
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public string LicenseC { get; set; }
		public string LicenseN { get; set; }
		public DateTime ExpiryD { get; set; }
		public DateTime? UpdateD { get; set; }
		public DateTime NextExpiryD { get; set; }
		public bool IsDisableNextExpiryD { get; set; }
		public bool IsInsert { get; set; }
		public bool IsDelete { get; set; }
		public int PlanI { get; set; }
	}
}