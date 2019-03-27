using System;

namespace Website.ViewModels.TruckExpense
{
	public class TruckExpenseViewModel
	{
		public int Id { get; set; }
		public DateTime InvoiceD { get; set; }
		public DateTime TransportD { get; set; }
        public string Code { get; set; }
		public string EntryClerkC { get; set; }
		public string EntryClerkN { get; set; }
		public DateTime? EntryClerkRetiredD { get; set; }
		public string ObjectNo { get; set; }
		public DateTime? AcquisitionD { get; set; }
		public DateTime? DisusedD { get; set; }
		public string ExpenseC { get; set; }
		public string ExpenseN { get; set; }
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public DateTime? RetiredD { get; set; }
		public string PaymentMethodI { get; set; }
		public string SupplierMainC { get; set; }
		public string SupplierSubC { get; set; }
		public string SupplierN { get; set; }
		public string SupplierShortN { get; set; }
		public decimal? Quantity { get; set; }
		public decimal? UnitPrice { get; set; }
		public decimal? Total { get; set; }
		public decimal? Tax { get; set; }
		public string Description { get; set; }
		public decimal? TaxRate { get; set; }
		public string  TaxRoundingI { get; set; }
		public int TruckExpenseIndex { get; set; }
        public string ObjectI { get; set; }
		public string ExpenseI { get; set; }
		public string IsAllocated { get; set; }

	}
}