using System;
using System.Collections.Generic;

namespace Website.ViewModels.Dispatch
{
	public class DispatchListReport
	{
		public List<DispatchViewModel> DispatchDList { get; set; }
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public string CustomerShortN { get; set; }
		public string CustomerN { get; set; }
		public string OrderTypeI { get; set; }
		public string DepN { get; set; }
		public string BLBK { get; set; }
		public string ShippingCompanyN { get; set; }
		public string VesselN { get; set; }
		public string ContainerNo { get; set; }
		public string ContainerSizeI { get; set; }
		public string TrailerNo { get; set; }
		//public string LoadingPlaceN { get; set; }
		//public DateTime? LoadingDT { get; set; }
		//public DateTime? ActualLoadingD { get; set; }
		//public DateTime? DischargeDT { get; set; }
		//public string DischargePlaceN { get; set; }
		//public DateTime? ActualDischargeD { get; set; }
		public string CommodityN { get; set; }
		public string JobNo { get; set; }
		public string SealNo { get; set; }
		public string IsCollected { get; set; }
		public decimal NetWeight { get; set; }
		

	}
}