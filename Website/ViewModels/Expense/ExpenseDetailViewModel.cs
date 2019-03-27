using System;
using System.Collections.Generic;

namespace Website.ViewModels.Expense
{
	public class ExpenseDetailViewModel
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public int ExpenseNo { get; set; }
		public string ExpenseN { get; set; }
		public string ExpenseC { get; set; }
		public string ExpenseI { get; set; }
		public string PaymentMethodI { get; set; }
		public DateTime? InvoiceD { get; set; }
		public string SupplierMainC { get; set; }
		public string SupplierSubC { get; set; }
		public string SupplierN { get; set; }
		public string Unit { get; set; }
		public decimal? UnitPrice { get; set; }
		public decimal? Quantity { get; set; }
		public decimal? Amount { get; set; }
		public decimal? TaxAmount { get; set; }
		public decimal? TaxRate { get; set; }
		public string IsIncluded { get; set; }
		public string IsRequested { get; set; }
		public string IsPayable { get; set; }
		public string EntryClerkC { get; set; }
		public string EntryClerkFristN { get; set; }
		public string EntryClerkLastN { get; set; }
		public string EntryClerkN
		{
			get { return EntryClerkLastN + " " + EntryClerkFristN; }
		}
		public string Description { get; set; }
		public string TaxRoudingI { get; set; }

		public List<LiftOnList> LiftOnList { get; set; }
		public List<LiftOffList> LiftOffList { get; set; }
		public List<OtherListLoLo> OtherListLoLo { get; set; }
	}
	public class OtherListLoLo
	{
		public decimal? AmountOther { get; set; }
		public string DescriptionOther { get; set; }
		public string IsIncludedOther { get; set; }
		public string IsRequestedOther { get; set; }
		public decimal? TaxAmountOther { get; set; }
	}
	public class LiftOnList
	{
		public decimal? AmountLiftOn { get; set; }
		public string DescriptionLiftOn { get; set; }
		public string IsIncludedLiftOn { get; set; }
		public string IsRequestedLiftOn { get; set; }
		public decimal? TaxAmountLiftOn { get; set; }
	}
	public class LiftOffList
	{
		public decimal? AmountLiftOff { get; set; }
		public string DescriptionLiftOff { get; set; }
		public string IsIncludedLiftOff { get; set; }
		public string IsRequestedLiftOff { get; set; }
		public decimal? TaxAmountLiftOff { get; set; }
	}
}
