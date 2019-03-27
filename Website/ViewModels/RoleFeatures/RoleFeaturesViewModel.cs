using Root.Models.Authorization;
using System;
using System.Collections.Generic;
using Website.ViewModels.Role;

namespace Website.ViewModels.RoleFeatures
{
	public class RoleFeaturesViewModel
	{
		public RoleFeaturesViewModel()
		{
			this.Role = new RoleViewModel();
			this.RoleFeaturesList = new List<RoleFeaturesData>();
			this.AuthorizedRoleFeatureList = new List<AuthorizedRoleFeature>();
		}
		public RoleViewModel Role { get; set; }
		public List<RoleFeaturesData> RoleFeaturesList { get; set; }
		public List<AuthorizedRoleFeature> AuthorizedRoleFeatureList { get; set; }
		public bool IsAdd { get; set; }
	}

	public class RoleFeaturesData
	{
		public string RoleId { get; set; }
		public string UnusedFeatureC { get; set; }
	}
}