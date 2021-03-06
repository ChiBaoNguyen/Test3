﻿using System;

namespace Website.ViewModels.Dispatch
{
	public class DispatchViewModel
	{
		public DateTime OrderD { get; set; }
		public string OrderNo { get; set; }
		public int DetailNo { get; set; }
		public int DispatchNo { get; set; }
		public DateTime? TransportD { get; set; }
		public string DispatchI { get; set; }
		public string TruckC { get; set; }
		public string RegisteredNo { get; set; }
		public DateTime? AcquisitionD { get; set; }
		public DateTime? DisusedD { get; set; }
		public string DriverC { get; set; }
		public string DriverN { get; set; }
		public string DriverFirstN { get; set; }
		public DateTime? RetiredD { get; set; }
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public string PartnerN { get; set; }
		public string PartnerShortN { get; set; }
		public string OrderTypeI { get; set; }
		public string OrderTypeN { get; set; }
		public int? DispatchOrder { get; set; }
		public string ContainerStatus { get; set; }
		public string ContainerContent { get; set; }
		public string DispatchStatus { get; set; }
		public string Location1C { get; set; }
		public string Location1N { get; set; }
		public string Location1A { get; set; }
		public DateTime? Location1DT { get; set; }
		public string Location1Time { get; set; }
		public string Operation1C { get; set; }
		public string Operation1N { get; set; }
		public string Location2C { get; set; }
		public string Location2N { get; set; }
		public string Location2A { get; set; }
		public DateTime? Location2DT { get; set; }
		public string Location2Time { get; set; }
		public string Operation2C { get; set; }
		public string Operation2N { get; set; }
		public string Location3C { get; set; }
		public string Location3N { get; set; }
		public string Location3A { get; set; }
		public DateTime? Location3DT { get; set; }
		public string Location3Time { get; set; }
		public string Operation3C { get; set; }
		public string Operation3N { get; set; }
		public decimal? TransportFee { get; set; }
		public decimal? PartnerFee { get; set; }
		public decimal? IncludedExpense { get; set; }
		public decimal? DriverAllowance { get; set; }
		public decimal? Expense { get; set; }
		public decimal? PartnerExpense { get; set; }
		public decimal? PartnerSurcharge { get; set; }
		public decimal? PartnerDiscount { get; set; }
        public decimal? PartnerTaxAmount { get; set; }
		public decimal? PartnerAmount { get; set; }
		public DateTime? InvoiceD { get; set; }
		public string TransportDepC { get; set; }
		public decimal? ApproximateDistance { get; set; }
		public decimal? ActualDistance { get; set; }
		public decimal? FuelConsumption { get; set; }
		public decimal? ActualFuel { get; set; }
		public DateTime? LocationDriverDispatchDT { get; set; }
		public int DetainDay { get; set; }
		public string ContainerNo { get; set; }
		public string ContainerSize { get; set; }
		public string InvoiceStatus { get; set; }
		public decimal PartnerToTalTax { get; set; }
		public string InvoiceMainC { get; set; }
		public string InvoiceSubC { get; set; }
		public string Description { get; set; }
		public string InstructionNo { get; set; }
		public bool IsTransported1 { get; set; }
		public bool IsTransported2 { get; set; }
		public bool IsTransported3 { get; set; }
		public int? CountTransport { get; set; }
		public Nullable<decimal> AllowanceOfDriver { get; set; }
		public decimal? TotalKm { get; set; }
		public decimal? TotalFuel { get; set; }
		public Nullable<decimal> VirtualDataNoGoods { get; set; }
		public Nullable<decimal> VirtualDataHaveGoods { get; set; }
		public string WayType { get; set; }
		public Nullable<decimal> TotalDriverAllowance { get; set; }
        public string OrderImageKey { get; set; }
        public string ImageCamera { get; set; }
        public int ImageCount { get; set; }
		public decimal? NetWeight { get; set; }
		public decimal? LossFuelRate { get; set; }
		public string AssistantC { get; set; }
		public string AssistantN { get; set; }
		public string TrailerC { get; set; }
		public string TrailerNo { get; set; }
	}
}