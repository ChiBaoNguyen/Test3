using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.AuthenticationModels
{
	public class UserModel
	{
		//public string UserName { get; set; }

		//public string Password { get; set; }

		//public string ConfirmPassword { get; set; }

		[Required]
		public string UserName { get; set; }
		
		[Required]
		public string RoleName { get; set; }

		public string EmployeeC { get; set; }

		public string DriverC { get; set; }

		public string IsMobileUser { get; set; }

		public string IsActive { get; set; }

		public string IsLoggedIn { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }
        public string DeviceInfo { get; set; }
	}
}
