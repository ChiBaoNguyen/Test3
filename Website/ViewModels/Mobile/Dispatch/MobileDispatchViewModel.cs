using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Mobile.Dispatch
{
	public class MobileDispatchViewModel
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public string ContainerSizeI { get; set; }
		public string ContainerTypeN { get; set; }
		public DateTime? TransportD { get; set; }
		public string DispatchI { get; set; }
		public string RegisteredNo { get; set; }
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public string DriverFirstN { get; set; }
		public int? DispatchOrder { get; set; }
		public string ContainerStatus { get; set; }
		public string ContainerContent { get; set; }
		public string DispatchStatus { get; set; }
		public string Location1N { get; set; }
		public DateTime? Location1DT { get; set; }
		public string Location1Time { get; set; }
		public string Location2N { get; set; }
		public DateTime? Location2DT { get; set; }
		public string Location2Time { get; set; }
		public string Location3N { get; set; }
		public DateTime? Location3DT { get; set; }
		public string Location3Time { get; set; }
		public string Operation1C { get; set; }
		public string Operation2C { get; set; }
		public string Operation3C { get; set; }
		public string Operation1N { get; set; }
		public string Operation2N { get; set; }
		public string Operation3N { get; set; }
		public bool IsTransported1 { get; set; }
		public bool IsTransported2 { get; set; }
		public bool IsTransported3 { get; set; }
		public int? CountTransport { get; set; }
		public string ContainerNo { get; set; }
		public string SealNo { get; set; }
		public string Description { get; set; }
        public string TrailerNo { get; set; }
		public decimal? NetWeight { get; set; }
        public string OrderImageKey { get; set; }
        public int ImageCount { get; set; }
        public string IsCollected { get; set; } //Using for Company name
        public string CustomerPayLiftMainC { get; set; } //Using for address
        public string CustomerPayLiftSubC { get; set; } //Using for tax code
	}
}
