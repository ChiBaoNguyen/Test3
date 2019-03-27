using System;

namespace Root.Models
{
	public class CustomerPricing_H
	{
		public string CustomerPricingId { get; set; }
		public string Location1C { get; set; }
		public string Location2C { get; set; }
		public string ContainerTypeC { get; set; }
		public string ContainerSizeI { get; set; }
		public string CustomerMainC { get; set; }
		public string CustomerSubC { get; set; }
		public decimal? GrossProfitRatio { get; set; }
		public decimal? TotalExpense { get; set; }
		public decimal? EstimatedPrice { get; set; }
		public DateTime EstimatedD { get; set; }
	}
}
