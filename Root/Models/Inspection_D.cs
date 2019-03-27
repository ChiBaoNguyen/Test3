using System;
using System.Collections.Generic;

namespace Root.Models
{
	public partial class Inspection_D
	{
		public string ObjectI { get; set; }
		public string Code { get; set; }
		public int InspectionC { get; set; }
		public DateTime InspectionD { get; set; }
		public DateTime? InspectionPlanD { get; set; }
		public decimal? Odometer { get; set; }
		public string Description { get; set; }

		public string EntryClerkC { get; set; }
		public string PaymentMethodI { get; set; }
		public string ExpenseC { get; set; }
		public string SupplierMainC { get; set; }
		public string SupplierSubC { get; set; }
		public decimal? Total { get; set; }
		public decimal? TaxAmount { get; set; }
		public string IsAllocated { get; set; }
	}
}