using System;
using System.Collections.Generic;

namespace Root.Models
{
	public partial class Dispatch_D
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public Nullable<System.DateTime> TransportD { get; set; }
		public string DispatchI { get; set; }
		public string TruckC { get; set; }
		public string RegisteredNo { get; set; }
		public string DriverC { get; set; }
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public string OrderTypeI { get; set; }
		public Nullable<int> DispatchOrder { get; set; }
		public string ContainerStatus { get; set; }
		public string DispatchStatus { get; set; }
		public string Location1C { get; set; }
		public string Location1N { get; set; }
		public Nullable<System.DateTime> Location1DT { get; set; }
		public string Operation1C { get; set; }
		public bool IsTransported1 { get; set; }
		public string Location2C { get; set; }
		public string Location2N { get; set; }
		public Nullable<System.DateTime> Location2DT { get; set; }
		public string Operation2C { get; set; }
		public bool IsTransported2 { get; set; }
		public string Location3C { get; set; }
		public string Location3N { get; set; }
		public Nullable<System.DateTime> Location3DT { get; set; }
		public string Operation3C { get; set; }
		public bool IsTransported3 { get; set; }
		public int? CountTransport { get; set; }
		public Nullable<decimal> TransportFee { get; set; }
		public Nullable<decimal> PartnerFee { get; set; }
		public Nullable<decimal> IncludedExpense { get; set; }
		public Nullable<decimal> DriverAllowance { get; set; }
		public Nullable<decimal> Expense { get; set; }
		public Nullable<decimal> PartnerExpense { get; set; }
		public Nullable<decimal> PartnerSurcharge { get; set; }
		public Nullable<decimal> PartnerDiscount { get; set; }
        public Nullable<decimal> PartnerTaxAmount { get; set; }
		public Nullable<System.DateTime> InvoiceD { get; set; }
		public string TransportDepC { get; set; }
		public Nullable<decimal> ApproximateDistance { get; set; }
		public Nullable<decimal> ActualDistance { get; set; }
		public Nullable<decimal> FuelConsumption { get; set; }
		public Nullable<decimal> ActualFuel { get; set; }
		public int DetainDay { get; set; }
		public string InvoiceStatus { get; set; }
		public string Description { get; set; }
		public string InstructionNo { get; set; }
		public Nullable<decimal> AllowanceOfDriver { get; set; }
		public Nullable<decimal> TotalKm { get; set; }
		public Nullable<decimal> TotalFuel { get; set; }
		public Nullable<decimal> LossFuelRate { get; set; }
		public Nullable<decimal> VirtualDataNoGoods { get; set; }
		public Nullable<decimal> VirtualDataHaveGoods { get; set; }
		public string WayType { get; set; }
        public string OrderImageKey { get; set; }
        public int ImageCount { get; set; }
		public string AssistantC { get; set; }
	}
}