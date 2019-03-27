using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Container
{
	public class ContainerDetailViewModel
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public string ContainerNo { get; set; }
		public Nullable<System.DateTime> TransportD { get; set; }
		public int DispatchNo { get; set; }
		public string ContainerStatus { get; set; }
		public string DispatchStatus { get; set; }
		public string DriverC { get; set; }
		public string DriverFirstN { get; set; }
		public string DriverLastN { get; set; }
		public string DriverN {
			get { return DriverLastN + " " + DriverFirstN ;}
		}
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public string PartnerN { get; set; }
		public string PartnerShortN { get; set; }
		public string TruckC { get; set; }
		public string RegisteredNo { get; set; }
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string CustomerN { get; set; }
		public string OrderTypeI { get; set; }
		public string BLBK { get; set; }
		public string JobNo { get; set; }
		public string LoadingPlaceN { get; set; }
		public string StopoverPlaceN { get; set; }
		public string DischargePlaceN { get; set; }
		public string Location1N { get; set; }
		public string Location2N { get; set; }
		public string Location3N { get; set; }
		public string SealNo { get; set; }
		public string Description { get; set; }
		public Nullable<System.DateTime> RevenueD { get; set; }
		public string LocationDispatch1 { get; set; }
		public string LocationDispatch2 { get; set; }
		public string LocationDispatch3 { get; set; }
		public string TruckCLastDispatch { get; set; }
		public string TrailerNo { get; set; }
	}
}
