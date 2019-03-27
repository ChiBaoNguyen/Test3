using System.Collections.Generic;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.Department;
using Website.ViewModels.Employee;
using Website.ViewModels.FixedExpense;

namespace WebAPI.Controllers
{
	public class FixedExpenseController : ApiController
	{
		public IFixedExpenseService _fixedExpenseService;

		public FixedExpenseController() { }
		public FixedExpenseController(IFixedExpenseService fixedExpenseService)
		{
			this._fixedExpenseService = fixedExpenseService;
		}

		[Route("api/FixedExpense/Datatable")]
		public IHttpActionResult Get(
				  string depC,
				  int year,
				  string expenseC)
		{
			var data = _fixedExpenseService.GetFixedExpense(depC, year, expenseC);
			return Ok(data);
		}

		 //PUT api/<controller>/5
		[Filters.Authorize(Roles = "FixExpense_E")]
		public void Put(FixedExpenseData data)
		{
			_fixedExpenseService.UpdateData(data);
		}
	}
}
