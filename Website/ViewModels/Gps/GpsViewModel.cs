using System;
using System.Collections.Generic;
using Root.Models;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Mobile.Dispatch;

namespace Website.ViewModels.Gps
{
    public class GpsViewModel
	{
		public string DriverC { get; set; }
		public string DriverN
		{
			get { return LastN + " " + FirstN; }
		}
		public string FirstN { get; set; }
		public string LastN { get; set; }
		public string DepC { get; set; }
		public string DepName { get; set; }
		public string ModelName { get; set; }
		public string ModelC { get; set; }
        public decimal GrossWeight { get; set; }
		public string TruckC { get; set; }
        public string RegisteredNo { get; set; }
		public string PhoneNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Nullable<DateTime> UpdateD { get; set; }
        public Dispatch_D DriverDispatch { get; set; }
        public List<GpsViewModel> ListDispatchViewModels { get; set; }
        public int TargetDispatch { get; set; }
        public int IsDriverDefault { get; set; }
        public int DispatchStatus { get; set; }
        public int IsTruckGoBack { get; set; }
        public int IsTruckEmpty { get; set; } //0: Empty | 1: Have Commodity | 2: Undefine
        public char IsUpdated
        {
            get {
                if (UpdateD != null)
                    if (UpdateD.Value.Day == DateTime.Now.Day && UpdateD.Value.Month == DateTime.Now.Month && UpdateD.Value.Year == DateTime.Now.Year)
                        return '1';
                    else
                        return '0';
                else
                    return '2';
            } 
        }
	}
}