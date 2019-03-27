using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.InspectionPlanDetail;
using Website.ViewModels.MaintenanceDetail;
using Website.ViewModels.Truck;

namespace Website.ViewModels.Trailer
{
	public class TrailerViewModel
	{
		public string TrailerC { get; set; }
		public string TrailerNo { get; set; }
		public Nullable<DateTime> RegisteredD { get; set; }
		public string VIN { get; set; }
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public DateTime? RetiredD { get; set; }
		public string ModelC { get; set; }
		public string ModelN { get; set; }
		public string IsActive { get; set; }
		public int TrailerIndex { get; set; }
		public string UsingDriverC { get; set; }
		public string IsUsing { get; set; }
		public Nullable<decimal> GrossWeight { get; set; }
		public List<MaintenanceDetailViewModel> MaintenanceItems { get; set; }
		public List<InspectionPlanDetailViewModel> Inspection { get; set; }
		public string Situation { get; set; }
		public Nullable<DateTime> FromDate { get; set; }
		public Nullable<DateTime> ToDate { get; set; }
		
	}
}