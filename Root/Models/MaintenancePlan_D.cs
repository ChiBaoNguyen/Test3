using System;
using System.Collections.Generic;

namespace Root.Models
{
	public partial class MaintenancePlan_D
	{
		public string ObjectI { get; set; }
		public string Code { get; set; }
		public int MaintenanceItemC { get; set; }
		public DateTime? PlanMaintenanceD { get; set; }
		public decimal? PlanMaintenanceKm { get; set; }
	}
}