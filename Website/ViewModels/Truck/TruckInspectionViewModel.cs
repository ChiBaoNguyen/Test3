using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.InspectionDetail;

namespace Website.ViewModels.Truck
{
	public class TruckInspectionViewModel
	{
		public int InspectionC { get; set; }
		public string InspectionN { get; set; }
		public DateTime? InspectionD { get; set; }
		public DateTime? InspectionPlanD { get; set; }
		public string Description { get; set; }
	}
}
