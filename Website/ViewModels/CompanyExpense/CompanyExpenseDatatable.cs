using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.ViewModels.CompanyExpense
{
	public class CompanyExpenseDatatable
	{
		public List<CompanyExpenseViewModel> Data { get; set; }
		public int Total { get; set; }
	}

	public class CompanyExpenseSearchParams
	{
		public CompanyExpenseSearchParams()
		{
			ParamSearch = new CompanySearchSearchInfo();
		}

		public int Page { get; set; }
		public int ItemsPerPage { get; set; }
		public string SortBy { get; set; }
		public bool Reverse { get; set; }
		public CompanySearchSearchInfo ParamSearch { get; set; }
	}

	public class CompanySearchSearchInfo
	{
		public DateTime? InvoiceDStart { get; set; }
		public DateTime? InvoiceDEnd { get; set; }
		public string ExpenseC { get; set; }
		public string SupplierMainC { get; set; }
		public string SupplierSubC { get; set; }
		public string EmployeeC { get; set; }

	}
}
