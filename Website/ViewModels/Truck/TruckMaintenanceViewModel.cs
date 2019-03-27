using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Truck
{
	public class TruckMaintenanceViewModel
	{
		public string ObjectI { get; set; }
		public string TruckC { get; set; }
		public int MaintenanceItemC { get; set; }
		public string MaintenanceItemN { get; set; }
		public DateTime? PlanMaintenanceD { get; set; }
		public decimal? PlanMaintenanceKm { get; set; }
		public DateTime? NextMaintenanceD { get; set; }
		public decimal? NextMaintenanceKm { get; set; }
		public string Description { get; set; }
		public string NoticeI { get; set; }
		public decimal? Remaining { get; set; }
		public int? ReplacementInterval { get; set; }
		public int? NoticeNo { get; set; }
		public decimal? Odometer { get; set; }
	}
}
