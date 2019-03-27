using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Report.Maintenance
{
	public class MaintenanceReportData
	{
		public List<string> DepList { get; set; }
		public List<string> TruckList { get; set; }
		public List<string> TrailerList { get; set; }
		public List<MaintenanceReportDevice> Devices { get; set; }
		public List<MaintenanceReportInspection> InspectionsPlan { get; set; }
		public List<MaintenanceReportMaintenance> MaintenancesPlan { get; set; }
		public List<MaintenanceReportInspection> InspectionsFinished { get; set; }
		public List<MaintenanceReportMaintenance> MaintenancesFinished { get; set; }
		public List<MaintenanceReportInspection> ExpensesFinished { get; set; } 

	}
}
