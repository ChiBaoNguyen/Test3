using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.CompanyExpense;

namespace WebAPI.Controllers
{
    public class CompanyExpenseController : ApiController
    {
		public ICompanyExpenseService _companyExpenseService;
		public CompanyExpenseSearchParams paramSearch = new CompanyExpenseSearchParams();

		public CompanyExpenseController() { }
		public CompanyExpenseController(ICompanyExpenseService companyExpenseService)
		{
			this._companyExpenseService = companyExpenseService;
		}

		// GET api/customer
		[Route("api/CompanyExpense/Datatable")]
		public IHttpActionResult Get(int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "InvoiceD",
				  bool reverse = false,
				  DateTime? invoiceDStart = null,
				  DateTime? invoiceDEnd = null,
				  string expenseC = "",
				  string supplierMainC = "",
				  string supplierSubC = "",
				  string employeeC = ""
			)
		{
			// set param search
			paramSearch.Page = page;
			paramSearch.ItemsPerPage = itemsPerPage;
			paramSearch.SortBy = sortBy;
			paramSearch.Reverse = reverse;
			paramSearch.ParamSearch.InvoiceDStart = invoiceDStart;
			paramSearch.ParamSearch.InvoiceDEnd = invoiceDEnd;
			paramSearch.ParamSearch.ExpenseC = expenseC;
			paramSearch.ParamSearch.SupplierMainC = supplierMainC;
			paramSearch.ParamSearch.SupplierSubC = supplierSubC;
			paramSearch.ParamSearch.EmployeeC = employeeC;

			var custDatatable = _companyExpenseService.GetCompanyExpenseForTable(paramSearch);
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		[HttpGet]
		[Route("api/CompanyExpense/GetCompanyExpenseByKey")]
		public IHttpActionResult GetCompanyExpenseByKey(int id)
		{
			var companyExpense = _companyExpenseService.GetCompanyExpenseByKey(id);
			return Ok(companyExpense);
		}

		// POST
		[Filters.Authorize(Roles = "TruckExpense_A")]
		public void Post(CompanyExpenseViewModel companyExpense)
		{
			_companyExpenseService.CreateCompanyExpense(companyExpense);
		}

		// PUT
		[Filters.Authorize(Roles = "TruckExpense_E")]
		public void Put(CompanyExpenseViewModel companyExpense)
		{
			_companyExpenseService.UpdateCompanyExpense(companyExpense);
		}

		// DELETE
		[HttpGet]
		[Route("api/CompanyExpense/DeleteCompanyExpense")]
		[Filters.Authorize(Roles = "TruckExpense_D")]
		public void DeleteCompanyExpense(int id)
		{
			_companyExpenseService.DeleteCompanyExpense(id);
		}
    }
}
