using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.FuelConsumption
{
	public class FuelConsumptionDetailViewModel
	{

		public DateTime? TransportD { get; set; }
		public string DriverN { get; set; }
		public string RegisteredNo { get; set; }
		public string Location1N { get; set; }
		public string Location2N { get; set; }
		public string Location3N { get; set; }
		public decimal? EstimatedDistance { get; set; }
		public decimal? EstimatedFuel { get; set; }
		public decimal? ActualDistance { get; set; }
		public decimal? ActualFuel { get; set; }
		public string ContainerNo { get; set; }
		public string InstructionNo { get; set; }
		//Service
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public string ContainerStatus { get; set; }
		public string TruckC { get; set; }
		public string ContainerTypeN { get; set; }
		public string ContainerSizeI { get; set; }
		public decimal? GrossWeight { get; set; }
		public string Location1C { get; set; }
		public string Location2C { get; set; }
		public string Location3C { get; set; }
		public decimal? ApproximateDistance { get; set; }
		public string IsEmpty { get; set; }
		public string IsHeavy { get; set; }
		public string IsSingle { get; set; }
		public decimal? FuelConsumption { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal Amount { get; set; }
		public string CustomerN { get; set; }
		public string CustomerShortN { get; set; }
		public string BLBK { get; set; }
		public string OrderTypeI { get; set; }
		public string DepartmentN { get; set; }
		public decimal? LossFuelRate { get; set; }
		public string ModelC { get; set; }
		public decimal? TotalFuel { get; set; }
		public decimal? OpeningPeriod { get; set; }
		public decimal? MidPeriod { get; set; }
		public decimal? ClosingPeriod { get; set; }
	}
}
