using System;
namespace Website.ViewModels.InspectionDetail
{
	public class InspectionDetailViewModel
	{
		public int InspectionId { get; set; }
		public string ObjectI { get; set; }
		public string Code { get; set; }
		public int InspectionC { get; set; }
		public string InspectionN { get; set; }
		public DateTime? InspectionD { get; set; }
		public DateTime? InspectionPlanD { get; set; }
		public decimal? Odometer { get; set; }
		public string Description { get; set; }
		public bool IsDisabled { get; set; }
		public bool IsHighLightRow { get; set; }

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