using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.Maintenance
{
	public class MaintenanceReportMaintenance
	{
		public string ObjectI { get; set; }
		public string Code { get; set; }
		public int InspectionC { get; set; }
		public int DisplayLineNo { get; set; }
		public DateTime? MaintenanceD { get; set; }
		public string MaintenanceItemN { get; set; }
		public DateTime? PlanMaintenanceDate { get; set; }
		public decimal? PlanMaintenanceKm { get; set; }
		public string NoticeI { get; set; }
		public string Remark { get; set; }
		public string MaintenanceDesc { get; set; }
		public decimal? CurrentOdometer { get; set; }
		public string IsFinished { get; set; }
	}
}
