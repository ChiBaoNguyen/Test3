using System;

namespace Website.ViewModels.User
{
	public class UserViewModel
	{
		public string Id { get; set; }
		public string UserName { get; set; }
		public string EmployeeC { get; set; }
		public string EmployeeN { get; set; }
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public string RoleId { get; set; }
		public string RoleN { get; set; }
		public string IsActive { get; set; }
		public int UserIndex { get; set; }
		public string SmartphonePlatform { get; set; }
		public string DeviceId { get; set; }
		public string IsLoggedIn { get; set; }
	}
}