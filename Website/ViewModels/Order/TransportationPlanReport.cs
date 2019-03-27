using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.Expense;

namespace Website.ViewModels.Order
{
	public class TransportationPlanReport
	{
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int Quantity { get; set; }
		public string ContainerNo { get; set; }
		public string ContainerSize { get; set; }
		public string ShippingCompanyN { get; set; }
		public string Booking { get; set; }
		public string CommodityN { get; set; }
		public string SealNo { get; set; }
		public string TrailerNo { get; set; }
		public string TruckNoRun { get; set; }
		public string TruckNoReturn { get; set; }
		public DateTime? ReturnDate { get; set; }
		public DateTime? ActualDischargeD { get; set; }
		public DateTime? ActualPickupReturnD { get; set; }
		public string Locaion1N { get; set; }
		public string Locaion2N { get; set; }
		public string Locaion3N { get; set; }
		public DateTime? ClosingDT { get; set; }
		public decimal? EstimatedWeight { get; set; }
		public string ContractNo { get; set; }
		public string CustomerPayLift { get; set; }
		public string CustomerN { get; set; }
		public decimal? KmDispatch { get; set; }
		public string Description { get; set; }
		public string OrderTypeI { get; set; }
		public string CustomerPayLiftMainC { get; set; }
		public string CustomerPayLiftSubC { get; set; }
		public int? OrderNoDouble { get; set; }
		public string JobNo { get; set; }
		public string LocaionRoot1N { get; set; }
		public string LocaionRoot2N { get; set; }
		public string LocaionRoot3N { get; set; }
		public string IsCollected { get; set; }
		public int DetailNo { get; set; }
		public List<ExpenseDetailViewModel> ExpenseList { get; set; }
	}
}