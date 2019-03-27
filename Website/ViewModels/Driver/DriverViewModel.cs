using System;

namespace Website.ViewModels.Driver
{
	public class DriverViewModel
	{
		public string DriverC { get; set; }
		public string DriverN
		{
			get { return LastN + " " + FirstN; }
		}
		public string FirstN { get; set; }
		public string LastN { get; set; }
		public string DepC { get; set; }
		public string DepN { get; set; }
		public Nullable<System.DateTime> BirthD { get; set; }
		public string Address { get; set; }
		public string PhoneNumber { get; set; }
		public Nullable<System.DateTime> JoinedD { get; set; }
		public Nullable<System.DateTime> RetiredD { get; set; }
		public decimal? AdvancePaymentLimit { get; set; }
		public string IsActive { get; set; }
		public int DriverIndex { get; set; }
		public decimal? BasicSalary { get; set; }
		public string AssistantC { get; set; }
		public string AssistantN
		{
			get { return LastN + " " + FirstN; }
		}
	}
}