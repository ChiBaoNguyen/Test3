using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.Container
{
	public class ContainerDetailDatatable
	{
		public List<ContainerDetailViewModel> Data { get; set; }
		public int Total { get; set; }
	}

	public class ContainerSearchParams
	{
		public int page { get; set; }
		public int itemsPerPage { get; set; }
		public string sortBy { get; set; }
		public bool reverse { get; set; }
		public ContainerSearchInfo ContainerInfo { get; set; }
	}

	public class ContainerSearchInfo
	{
		public string DriverC { get; set; }
		//public string TransportMonthYear { get; set; }
		//public int TransportMonth { get; set; }
		//public int TransportYear { get; set; }
		public DateTime? TransportDFrom { get; set; }
		public DateTime? TransportDTo { get; set; }
		public string TruckC { get; set; }
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public string BLBK { get; set; }
		public string TrailerNo { get; set; }
		public string OrderTypeI { get; set; }
		public string DepC { get; set; }
		public string EntryClerkC { get; set; }
		public DateTime? StartD { get; set; }
		public DateTime? EndD { get; set; }
		public string OrderNo { get; set; }
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public string ContainerNo { get; set; }
		public string SealNo { get; set; }
		public string JobNo { get; set; }
		//public string DispatchStatus { get; set; }
		public bool DispatchStatus0 { get; set; }
		public bool DispatchStatus1 { get; set; }
		public bool DispatchStatus2 { get; set; }
		public bool DispatchStatus3 { get; set; }
		public string InvoiceTypeI { get; set; }
	}
}
