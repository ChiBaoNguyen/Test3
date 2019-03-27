using System;
using System.Collections.Generic;

namespace Website.ViewModels.Dispatch
{
	public class TruckListViewModel
	{
		public TruckListViewModel()
		{
			TruckList = new List<TruckInfo>();
		}

		public List<TruckInfo> TruckList { get; set; }
	}

	public class TruckInfo
	{
		public string TruckC { get; set; }
		public string RegisteredNo { get; set; }
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public string DriverFirstN { get; set; }
		public string DriverC2 { get; set; }
		public string DriverN2 { get; set; }
		public string DriverFirstN2 { get; set; }
		public string CompanyN { get; set; }
		public int? DispatchOrder { get; set; }
		public string DepC { get; set; }
		public string DepN { get; set; }
		public string LocationC { get; set; }
		public string LocationN { get; set; }
		public DateTime? LocationDT { get; set; }
		public string IsActive { get; set; }
		public bool IsPlanInspection { get; set; }
		public string Status { get; set; }
		public string AssistantC { get; set; }
		public string AssistantN { get; set; }
		public string AssistantC2 { get; set; }
		public string AssistantN2 { get; set; }
		public int IsChecked { get; set; }
		public string ContainerStatus { get; set; }
		public List<DispatchInfo> DispatchInfoList { get; set; }
	}

	public class DispatchInfo
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public int? DispatchOrder { get; set; }
		public string Location1 { get; set; }
		public string Time1 { get; set; }
		public string Operation1 { get; set; }
		public string Location2 { get; set; }
		public string Time2 { get; set; }
		public string Operation2 { get; set; }
		public string Location3 { get; set; }
		public string Time3 { get; set; }
		public string Operation3 { get; set; }
		public string ContainerStatus { get; set; }
	}
}