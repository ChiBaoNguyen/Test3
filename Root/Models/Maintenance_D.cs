using System;
using System.Collections.Generic;

namespace Root.Models
{
	public partial class Maintenance_D
	{
		public string ObjectI { get; set; }
		public string Code { get; set; }
		public int InspectionC { get; set; }
		public DateTime MaintenanceD { get; set; }
		public decimal? Odometer { get; set; }
		public int MaintenanceItemC { get; set; }
		public DateTime? PlanMaintenanceD { get; set; }
		public decimal? PlanMaintenanceKm { get; set; }
		public string RemarksI { get; set; }
		public DateTime? NextMaintenanceD { get; set; }
		public decimal? NextMaintenanceKm { get; set; }
		public string PartNo { get; set; }
		public decimal? Quantity { get; set; }
		public string Unit { get; set; }
		public decimal? UnitPrice { get; set; }
		public decimal? Amount { get; set; }
		public string Description { get; set; }
	}
}