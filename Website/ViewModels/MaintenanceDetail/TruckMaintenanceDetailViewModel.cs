using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.MaintenanceDetail
{
	public class TruckMaintenanceDetailViewModel
	{
		public string ObjectI { get; set; }
		public string Code { get; set; }
		public int InspectionC { get; set; }
		public DateTime MaintenanceD { get; set; }
		public int MaintenanceItemC { get; set; }
		public DateTime? PlanMaintenanceD { get; set; }
		public decimal? PlanMaintenanceKm { get; set; }
		public string RemarksI { get; set; }
		public DateTime? NextMaintenanceD { get; set; }
		public decimal? NextMaintenanceKm { get; set; }
		public string PartNo { get; set; }
		public string Description { get; set; }
	}
}
