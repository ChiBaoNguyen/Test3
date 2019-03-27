using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models
{
	public partial class Truck_M
	{
		public string TruckC { get; set; }
		public string RegisteredNo { get; set; }
		public Nullable<DateTime> RegisteredD { get; set; }
		public string VIN { get; set; }
		public string MakeN { get; set; }
		public string DepC { get; set; }
		public string DriverC { get; set; }
		public Nullable<DateTime> AcquisitionD { get; set; }
		public string PartnerI { get; set; }
		public Nullable<decimal> GrossWeight { get; set; }
		public string PartnerMainC { get; set; }
		public string PartnerSubC { get; set; }
		public decimal? Odometer { get; set; }
		public string ModelC { get; set; }
		public string RemodelI { get; set; }
		public string IsActive { get; set; }
		public DateTime? DisusedD { get; set; }
        public string ModelYear { get; set; }
		public string Status { get; set; }
		public DateTime? StatusFromD { get; set; }
		public DateTime? StatusToD { get; set; }
		public decimal? LossFuelRate { get; set; }
		public string AssistantC { get; set; }
	}
}