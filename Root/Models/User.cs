using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Root.Models
{
	public class User : IdentityUser
	{
		[Column(TypeName = "VARCHAR")]
		[StringLength(5)]
		public string EmployeeC { get; set; }

		[Column(TypeName = "VARCHAR")]
		[StringLength(5)]
		public string DriverC { get; set; }

		[Column(TypeName = "VARCHAR")]
		[StringLength(255)]
		public string RegistrationId { get; set; }

		[Column(TypeName = "NVARCHAR")]
		[StringLength(50)]
		public string SmartphonePlatform { get; set; }

		[Column(TypeName = "CHAR")]
		[StringLength(1)]
		public string IsMobileUser { get; set; }

		[Column(TypeName = "CHAR")]
		[StringLength(1)]
		public string IsActive { get; set; }

		[Column(TypeName = "CHAR")]
		[StringLength(1)]
		public string IsLoggedIn { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(40)]
        public string DeviceInfo { get; set; }

	}
}
