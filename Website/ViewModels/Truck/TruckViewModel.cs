using System;
using System.Collections.Generic;
using Website.ViewModels.InspectionPlanDetail;
using Website.ViewModels.MaintenanceDetail;

namespace Website.ViewModels.Truck
{
	public class TruckViewModel
	{
		public string TruckC { get; set; }
		public string RegisteredNo { get; set; }
		public Nullable<DateTime> RegisteredD { get; set; }
		public string VIN { get; set; }
		public string MakeN { get; set; }
		public string DepC { get; set; }
		public string DepN { get; set; }
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public string DriverFirstN { get; set; }
		public DateTime? RetiredD { get; set; }
		public Nullable<DateTime> AcquisitionD { get; set; }
		public string PartnerI { get; set; }
		public Nullable<decimal> GrossWeight { get; set; }
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public string PartnerN { get; set; }
		public decimal? Odometer { get; set; }
		public string ModelC { get; set; }
		public string ModelN { get; set; }
		public string RemodelI { get; set; }
		public string IsActive { get; set; }
		public DateTime? DisusedD { get; set; }
		public int TruckIndex { get; set; }
		public bool IsDisabledModelName { get; set; }
		public List<MaintenanceDetailViewModel> MaintenanceItems { get; set; }
		public List<InspectionPlanDetailViewModel> Inspection { get; set; }
        public string ModelYear { get; set; }
		public string Status { get; set; }
		public DateTime? StatusFromD { get; set; }
		public DateTime? StatusToD { get; set; }
		public decimal? LossFuelRate { get; set; }
		public string AssistantC { get; set; }
		public string AssistantN { get; set; }
	}
}