﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Feature
{
	public class FeatureViewModel
	{
		public string FeatureC { get; set; }
		public string FeatureN { get; set; }
		public string FeatureGroupN { get; set; }
		public string FeatureParentGroupN { get; set; }
		public int SortOrder { get; set; }
		public int FeatureIndex { get; set; }
	}
}