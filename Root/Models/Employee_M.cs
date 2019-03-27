using System;

namespace Root.Models
{
    public partial class Employee_M
    {
        public string EmployeeC { get; set; }
        public string EmployeeFirstN { get; set; }
        public string EmployeeLastN { get; set; }
        public string DepC { get; set; }
        public Nullable<DateTime> BirthD { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<DateTime> JoinedD { get; set; }
        public Nullable<DateTime> RetiredD { get; set; }
        public string IsActive { get; set; }
    }
}
