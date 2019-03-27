using System;
namespace Website.ViewModels.MaintenancePlanDetail
{
	public class MaintenancePlanDetailViewModel
	{
		public string ObjectI { get; set; }
		public string Code { get; set; }
		public int MaintenanceItemC { get; set; }
		public DateTime? PlanMaintenanceD { get; set; }
		public decimal? PlanMaintenanceKm { get; set; }
	}
}