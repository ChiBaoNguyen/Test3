using System;
namespace Website.ViewModels.MaintenanceDetail
{
	public class MaintenanceDetailViewModel
	{
		public string ObjectI { get; set; }
		public string TruckC { get; set; }
		public int MaintenanceItemC { get; set; }
		public string MaintenanceItemN { get; set; }
		public DateTime? PlanMaintenanceD { get; set; }
		public decimal? PlanMaintenanceKm { get; set; }
		public string PlanMaintenance { get; set; }
		public string RemarksI { get; set; }
		public DateTime? NextMaintenanceD { get; set; }
		public decimal? NextMaintenanceKm { get; set; }
		public string PartNo { get; set; }
		public decimal? Quantity { get; set; }
		public string Unit { get; set; }
		public decimal? UnitPrice { get; set; }
		public decimal? Amount { get; set; }
		public string Description { get; set; }
		public string NoticeI { get; set; }
		public string NoticeI2 { get; set; }
		public string Remaining { get; set; }
		public string NextRemaining { get; set; }
		public int? ReplacementInterval { get; set; }
		public int NoticeNo { get; set; }
		public decimal? Odometer { get; set; }
		public int DisplayLineNo { get; set; }
		public bool IsWarning { get; set; }
		public bool IsNextWarning { get; set; }
		public bool IsDisabled { get; set; }
		public bool IsHighLightRow { get; set; }
		public DateTime? MaintenanceD { get; set; }
	}
}