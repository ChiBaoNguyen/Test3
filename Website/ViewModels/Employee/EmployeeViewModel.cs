using System;

namespace Website.ViewModels.Employee
{
	public class EmployeeViewModel
	{
		public string EmployeeC { get; set; }
		public string EmployeeFirstN { get; set; }
		public string EmployeeLastN { get; set; }
		public string EmployeeN
		{
			get { return EmployeeLastN + " " + EmployeeFirstN; }
		}
		public DateTime? BirthD { get; set; }
		public string Address { get; set; }
		public string PhoneNumber { get; set; }
		public DateTime? JoinedD { get; set; }
		public DateTime? RetiredD { get; set; }
		public string DepC { get; set; }
		public string DepN { get; set; }
		public string IsActive { get; set; }
		public int EmployeeIndex { get; set; }
	}
}