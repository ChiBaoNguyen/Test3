using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Truck
{
	public class TruckMaintenanceViewModelForUpdate
	{
		public string TruckC { get; set; }
		public string ObjectI { get; set; }
		public List<TruckMaintenanceViewModel> MaintenanceItems { get; set; }
	}
}
