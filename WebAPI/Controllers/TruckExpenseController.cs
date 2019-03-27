using System.Collections.Generic;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.TruckExpense;
using System;

namespace WebAPI.Controllers
{
	public class TruckExpenseController : ApiController
	{
		public ITruckExpenseService _truckExpenseService;
		public TruckExpenseSearchParams paramSearch = new TruckExpenseSearchParams();

		public TruckExpenseController() { }
		public TruckExpenseController(ITruckExpenseService truckExpenseService)
		{
			this._truckExpenseService = truckExpenseService;
		}

		// GET api/customer
		[Route("api/TruckExpense/Datatable")]
		public IHttpActionResult Get(int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "InvoiceD",
				  bool reverse = false,
				  DateTime? invoiceDStart = null,
				  DateTime? invoiceDEnd = null,
				  DateTime? transportDStart = null,
				  DateTime? transportDEnd = null,
                  string objectI = "",
				  string code = "",
				  string expenseC = "",
				  string supplierMainC = "",
				  string supplierSubC = ""
                  
			)
		{
			// set param search
			paramSearch.Page = page;
			paramSearch.ItemsPerPage = itemsPerPage;
			paramSearch.SortBy = sortBy;
			paramSearch.Reverse = reverse;
			paramSearch.ParamSearch.InvoiceDStart = invoiceDStart;
			paramSearch.ParamSearch.InvoiceDEnd = invoiceDEnd;
			paramSearch.ParamSearch.TransportDStart = transportDStart;
			paramSearch.ParamSearch.TransportDEnd = transportDEnd;
            paramSearch.ParamSearch.ObjectI = objectI;
			paramSearch.ParamSearch.Code = code;
			paramSearch.ParamSearch.ExpenseC = expenseC;
			paramSearch.ParamSearch.SupplierMainC = supplierMainC;
			paramSearch.ParamSearch.SupplierSubC = supplierSubC;

			var custDatatable = _truckExpenseService.GetTruckExpensesForTable(paramSearch);
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		[HttpGet]
		[Route("api/TruckExpense/GetTruckExpenseByKey")]
		public IHttpActionResult GetTruckExpenseByKey(int id)
		{
			var truckExpense = _truckExpenseService.GetTruckExpensesByKey(id);
			return Ok(truckExpense);
		}

        [HttpGet]
        [Route("api/TruckExpense/objectI")]
        public IEnumerable<TruckExpenseViewModel> Get(string objectI)
        {
            return _truckExpenseService.Get(objectI);
        }

		// POST
		[Filters.Authorize(Roles = "TruckExpense_A")]
		public void Post(TruckExpenseViewModel truckExpense)
		{
			_truckExpenseService.CreateTruckExpense(truckExpense);
		}

		// PUT
		[Filters.Authorize(Roles = "TruckExpense_E")]
		public void Put(TruckExpenseViewModel truckExpense)
		{
			_truckExpenseService.UpdateTruckExpense(truckExpense);
		}

		// DELETE
		[HttpGet]
		[Route("api/TruckExpense/DeleteTruckExpense")]
		[Filters.Authorize(Roles = "TruckExpense_D")]
		public void DeleteTruckExpense(int id)
		{
			_truckExpenseService.DeleteTruckExpense(id);
		}

		[HttpGet]
		[Route("api/TruckExpense/GetSupplierInvoiceDateMax/{supplierMainC}/{supplierSubC}")]
		public DateTime? GetSupplierInvoiceDateMax(string supplierMainC, string supplierSubC)
		{
			return _truckExpenseService.GetSupplierInvoiceDateMax(supplierMainC, supplierSubC);
		}
	}
}
