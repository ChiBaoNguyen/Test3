using System;
using System.Collections.Generic;
using Website.ViewModels.InspectionDetail;
using Website.ViewModels.MaintenanceDetail;
using Website.ViewModels.MaintenanceItem;
using Website.ViewModels.MaintenanceItemDetail;
using Website.ViewModels.MaintenancePlanDetail;

namespace Website.ViewModels.InspectionPlanDetail
{
	public class InspectionMaintenancePlanViewModel
	{
		public InspectionMaintenancePlanViewModel()
		{
			this.InspectionDetailList = new List<InspectionDetailViewModel>();
			this.MaintenanceDetailList = new List<MaintenanceDetailViewModel>();
		}

		public int InspectionC { get; set; }
		public string InspectionN { get; set; }
		public string ObjectI { get; set; }
		public string Code { get; set; }
		public string CodeN { get; set; }
		public int PlanI { get; set; }
		public int IntCase { get; set; }
		public string RemodelI { get; set; }
		public string ModelC { get; set; }
		public string ModelN { get; set; }
		public DateTime? InspectionPlanD { get; set; }
		public DateTime ImplementD { get; set; }
		public decimal? CurrentOdometer { get; set; }
		public decimal? Odometer { get; set; }
		public string Description { get; set; }
		public List<InspectionDetailViewModel> InspectionDetailList { get; set; }
		public List<MaintenanceDetailViewModel> MaintenanceDetailList { get; set; }

		public string EntryClerkC { get; set; }
		public string EntryClerkN { get; set; }
		public string PaymentMethodI { get; set; }
		public string ExpenseC { get; set; }
		public string ExpenseN { get; set; }
		public string SupplierMainC { get; set; }
		public string SupplierSubC { get; set; }
		public string SupplierN { get; set; }
		public decimal? Total { get; set; }
		public decimal? TaxAmount { get; set; }
		public string IsAllocated { get; set; }
	}
}