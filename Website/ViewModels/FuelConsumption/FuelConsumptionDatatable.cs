using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.FuelConsumption
{
	public class FuelConsumptionDatatable
	{
		public List<FuelConsumptionViewModel> Data { get; set; }
		public int Total { get; set; }
	}

	public class FuelConsumptionSearchParams
	{
		public int page { get; set; }
		public int itemsPerPage { get; set; }
		public string sortBy { get; set; }
		public bool reverse { get; set; }
		public FuelConsumptionSearchInfo SearchInfo { get; set; }
	}

	public class FuelConsumptionSearchInfo
	{
		public string ModelC { get; set; }
		public string ContainerSizeI { get; set; }
		public string LoadingPlaceC { get; set; }
		public string DischargePlaceC { get; set; }
	}
}
