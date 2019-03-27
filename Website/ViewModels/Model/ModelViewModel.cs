using System.Collections.Generic;
using Website.ViewModels.MaintenanceItemDetail;

namespace Website.ViewModels.Model
{
	public class ModelViewModel
	{
		public string ObjectI { get; set; }
		public string ModelC { get; set; }
		public string ModelN { get; set; }
		public int ModelIndex { get; set; }
		public List<MaintenanceItemDetailViewModel> MaintenanceItems { get; set; }
	}
}