using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.TruckExpense
{
    public class TruckExpenseDatatables
    {
        public List<TruckExpenseViewModel> Data { get; set; }
        public int Total { get; set; }
    }

	public class TruckExpenseSearchParams
	{
		public TruckExpenseSearchParams()
		{
			ParamSearch = new TruckExpenseSearchInfo();
		}

		public int Page { get; set; }
		public int ItemsPerPage { get; set; }
		public string SortBy { get; set; }
		public bool Reverse { get; set; }
		public TruckExpenseSearchInfo ParamSearch { get; set; }
	}

	public class TruckExpenseSearchInfo
	{
		public DateTime? InvoiceDStart { get; set; }
		public DateTime? InvoiceDEnd { get; set; }
		public DateTime? TransportDStart { get; set; }
		public DateTime? TransportDEnd { get; set; }
		public string Code { get; set; }
		public string ExpenseC { get; set; }
		public string SupplierMainC { get; set; }
		public string SupplierSubC { get; set; }
        public string ObjectI { get; set; }

	}
}