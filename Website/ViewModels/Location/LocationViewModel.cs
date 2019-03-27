using System.Collections.Generic;
using Website.ViewModels.Expense;

namespace Website.ViewModels.Location
{
    public class LocationViewModel
    {
        public string LocationC { get; set; }
        public string LocationN { get; set; }
        public string Address { get; set; }
        public string LocationI { get; set; }
		public string Description { get; set; }
		public string AreaC { get; set; }
		public string AreaN { get; set; }
        public string IsActive { get; set; }
		public string SupplierMainC { get; set; }
		public string SupplierSubC { get; set; }
		public string SupplierN { get; set; }
	    public int LocationIndex { get; set; }
		public List<ExpenseViewModel> ExpenseData { get; set; }
    }
}
