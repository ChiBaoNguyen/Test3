using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Trailer
{
	public class TrailerInspectionViewModel
	{
		public int InspectionC { get; set; }
		public string InspectionN { get; set; }
		public DateTime? InspectionD { get; set; }
		public DateTime? InspectionPlanD { get; set; }
		public string Description { get; set; }
	}
}
