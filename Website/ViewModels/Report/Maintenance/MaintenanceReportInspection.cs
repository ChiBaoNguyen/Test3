using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.Maintenance
{
	public class MaintenanceReportInspection
	{
		public string ObjectI { get; set; }
		public string Code { get; set; }
		public string InspectionN { get; set; }
		public int InspectionC { get; set; }
		public DateTime? InspectionPlanD { get; set; }
		public DateTime InspectionD { get; set; }
		public decimal? ImplementOdometer { get; set; }
		public string InspectionDescription { get; set; }
		public string IsFinished { get; set; }

		public string SupplierN { get; set; }
		public decimal? Total { get; set; }
	}
}
