using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Expense;

namespace WebAPI.Controllers
{
	public class ExpenseCategoryController : ApiController
	{
		public IExpenseCategoryService _expenseCategoryService;
		public ICommonService _commonService;

		public ExpenseCategoryController() { }
		public ExpenseCategoryController(IExpenseCategoryService expenseService, ICommonService commonService)
		{
			this._expenseCategoryService = expenseService;
			_commonService = commonService;
		}

		// GET api/customer
		public async Task<IEnumerable<ExpenseCategoryViewModel>> Get()
		{
			return await Task.Run(() =>  _expenseCategoryService.GetExpenseCategories());
		}

		public IEnumerable<ExpenseCategoryViewModel> Get(string value)
		{
			return _expenseCategoryService.GetExpenseCategorys(value);
		}

		[Route("api/ExpenseCategory/GetExpenseCategorysByCode")]
		public IEnumerable<ExpenseCategoryViewModel> GetExpenseCategorysByCode(string value)
		{
			return _expenseCategoryService.GetExpenseCategorysByCode(value);
		}

		[HttpGet]
		[Route("api/ExpenseCategory/GetExpenseCategoryByCode")]
        public IHttpActionResult GetExpenseCategoryByCode(string expenseCategoryCode)
		{
            var category = _expenseCategoryService.GetExpenseCategoryByCode(expenseCategoryCode);
			if (category == null)
			{
				return NotFound();
			}
			return Ok(category);
		}

		[Route("api/ExpenseCategory/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "CategoryC",
				  bool reverse = false,
				  string search = null)
		{
			var custDatatable = _expenseCategoryService.GetExpenseCategorysForTable(page, itemsPerPage, sortBy, reverse, search);
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		// POST api/<controller>
		[Filters.Authorize(Roles = "ExpenseCategory_M_A")]
		public void Post(ExpenseCategoryViewModel category)
		{
			_expenseCategoryService.CreateExpenseCategory(category);
		}

		// PUT api/<controller>/5
		[Filters.Authorize(Roles = "ExpenseCategory_M_E")]
		public void Put(ExpenseCategoryViewModel category)
		{
			_expenseCategoryService.UpdateExpenseCategory(category);
		}

		// DELETE api/customer/5
		[Filters.Authorize(Roles = "ExpenseCategory_M_D")]
		public void Delete(string id)
		{
			_expenseCategoryService.DeleteExpenseCategory(id);
		}

		[HttpGet]
		[Route("api/ExpenseCategory/CheckWhenDelete/{language}/{categoryC}")]
		[Filters.Authorize(Roles = "ExpenseCategory_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string categoryC)
		{
			List<string> paramsList = new List<string>() { categoryC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("ExpenseCategory_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
			return new HttpResponseMessage()
			{
				Content = new StringContent(
					result,
					Encoding.UTF8,
					"text/html"
				)
			};
		}

		[HttpGet]
		[Route("api/ExpenseCategory/SetStatusExpenseCategory/{id}")]
		[Filters.Authorize(Roles = "ExpenseCategory_M_E")]
		public void SetStatusExpenseCategory(string id)
		{
			_expenseCategoryService.SetStatusExpenseCategory(id);
		}
	}
}
