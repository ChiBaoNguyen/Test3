using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Authorization
{
	public class AuthorizedRoleFeature
	{
		public string FeatureParentGroupN { get; set; }
		public string FeatureGroupN { get; set; }
		public string FeatureN { get; set; }
		public string FeatureMainC { get; set; }
		public string Add { get; set; }
		public string View { get; set; }
		public string Edit { get; set; }
		public string Delete { get; set; }
		public int SortOrder { get; set; }
	}
}
