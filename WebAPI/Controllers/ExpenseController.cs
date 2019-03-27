using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Expense;

namespace WebAPI.Controllers
{
    public class ExpenseController : ApiController
    {
		public IExpenseService _expenseService;
	    public ICommonService _commonService;

		public ExpenseController() { }
		public ExpenseController(IExpenseService expenseService, ICommonService commonService)
		{
			this._expenseService = expenseService;
			_commonService = commonService;
		}

		public IEnumerable<ExpenseViewModel> Get(string value)
		{
			return _expenseService.GetExpenseForSuggestion(value);
		}

		//Get Expense
		[HttpGet]
		[Route("api/Expense/GetExpenseForSuggestionOfBasicSetting")]
		public IEnumerable<ExpenseViewModel> GetExpenseForSuggestionOfBasicSetting(string value)
		{
			return _expenseService.GetExpenseForSuggestionOfBasicSetting(value);
		}
		//Get Surcharge
		[HttpGet]
		[Route("api/Expense/GetSurchargeForSuggestionOfBasicSetting")]
		public IEnumerable<ExpenseViewModel> GetSurchargeForSuggestionOfBasicSetting(string value)
		{
			return _expenseService.GetSurchargeForSuggestionOfBasicSetting(value);
		}
		//Get Allowance
		[HttpGet]
		[Route("api/Expense/GetAllowanceForSuggestionOfBasicSetting")]
		public IEnumerable<ExpenseViewModel> GetAllowanceForSuggestionOfBasicSetting(string value)
		{
			return _expenseService.GetAllowanceForSuggestionOfBasicSetting(value);
		}
		//Get Partner-Cost
		[HttpGet]
		[Route("api/Expense/GetPartnerCostForSuggestionOfBasicSetting")]
		public IEnumerable<ExpenseViewModel> GetPartnerCostForSuggestionOfBasicSetting(string value)
		{
			return _expenseService.GetPartnerCostForSuggestionOfBasicSetting(value);
		}
		//Get Partner-Surcharge
		[HttpGet]
		[Route("api/Expense/GetPartnerSurchargeForSuggestionOfBasicSetting")]
		public IEnumerable<ExpenseViewModel> GetPartnerSurchargeForSuggestionOfBasicSetting(string value)
		{
			return _expenseService.GetPartnerSurchargeForSuggestionOfBasicSetting(value);
		}

		//Get Fuel-Expense
		[HttpGet]
		[Route("api/Expense/GetFuelExpenseForSuggestionOfBasicSetting")]
		public IEnumerable<ExpenseCategoryViewModel> GetFuelExpenseForSuggestionOfBasicSetting(string value)
		{
			return _expenseService.GetFuelExpenseForSuggestionOfBasicSetting(value);
		}

        public async Task<IEnumerable<ExpenseViewModel>> Get()
        {
            return await Task.Run(() => _expenseService.GetExpense());
        }
		[HttpGet]
		[Route("api/Expense/GetExpenseByCategory/{categoryId}")]
		public async Task<IEnumerable<ExpenseViewModel>> GetByCategory(string categoryId)
		{
			return await Task.Run(() => _expenseService.GetExpenseByCategory(categoryId, "-1"));
		}

		[HttpGet]
		[Route("api/Expense/GetExpenseByCateAndDispatchI")]
		public async Task<IEnumerable<ExpenseViewModel>> GetExpenseByCateAndDispatchI(string categoryId, string dispatchI)
		{
			return await Task.Run(() => _expenseService.GetDefaultExpenseByCateAndDispatchI(categoryId, dispatchI));
		}

		[Route("api/Expense/GetExpensesByCode")]
		public IEnumerable<ExpenseViewModel> GetExpensesByCode(string value)
		{
			return _expenseService.GetExpensesByCode(value);
		}

        [HttpGet]
        [Route("api/Expense/GetExpenseByCode")]
		public ExpenseViewModel GetExpenseByCode(string expenseCode)
        {
            var expense = _expenseService.GetExpenseByCode(expenseCode);
			return expense;
        }

		[HttpGet]
		[Route("api/Expense/CheckExistExpenseInLocationD")]
		public int CheckExistExpenseInLocationD(string expenseCode)
		{
			var expense = _expenseService.CheckExistExpenseInLocationD(expenseCode);
			return expense;
		}

		[HttpGet]
		[Route("api/Expense/GetExpenseByCodeAndName")]
		public ExpenseViewModel GetExpenseByCodeAndName(string code, string name, string categoryI, string dispatchI)
		{
			var expense = _expenseService.GetExpenseByCodeAndName(code, name, categoryI, dispatchI);
			return expense;
		}

        [Route("api/Expense/Datatable")]
        public IHttpActionResult Get(
                  int page = 1,
                  int itemsPerPage = 10,
                  string sortBy = "ExpenseC",
                  bool reverse = false,
                  string search = null)
        {
            var custDatatable = _expenseService.GetExpensesForTable(page, itemsPerPage, sortBy, reverse, search);
            if (custDatatable == null)
            {
                return NotFound();
            }
            return Ok(custDatatable);
        }

        // POST api/<controller>
		[Filters.Authorize(Roles = "Expense_M_A")]
        public void Post(ExpenseViewModel expense)
        {
            _expenseService.CreateExpense(expense);
        }

        // PUT api/<controller>/5
		[Filters.Authorize(Roles = "Expense_M_E")]
        public void Put(ExpenseViewModel expense)
        {
            _expenseService.UpdateExpense(expense);
        }

        // DELETE api/customer/5
		[Filters.Authorize(Roles = "Expense_M_D")]
		public void Delete(string id)
		{
			_expenseService.DeleteExpense(id);
		}

		[HttpGet]
		[Route("api/Expense/CheckWhenDelete/{language}/{expenseC}")]
		[Filters.Authorize(Roles = "Expense_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string expenseC)
		{
			List<string> paramsList = new List<string>() { expenseC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Expense_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
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
		[Route("api/Expense/GetByName")]
		public ExpenseViewModel GetByName(string value)
		{
			return _expenseService.GetByName(value);
		}

		[HttpGet]
		[Route("api/Expense/GetExpenseAutoSuggestion")]
		public async Task<IEnumerable<ExpenseViewModel>> GetExpenseAutoSuggestion(string value, string categoryI, string dispatchI)
		{
			return await Task.Run(() => _expenseService.GetExpenseAutoSuggestion(value, categoryI, dispatchI));
		}

		[HttpGet]
		[Route("api/Expense/GetExpenseByName")]
		public ExpenseViewModel GetExpenseByName(string name, string categoryI, string dispatchI)
		{
			return _expenseService.GetExpenseByName(name, categoryI, dispatchI);
		}

		[HttpGet]
		[Route("api/Expense/GetFuelExpenseByName")]
		public ExpenseCategoryViewModel GetFuelExpenseByName(string name)
		{
			return _expenseService.GetFuelExpenseByName(name);
		}

		[HttpGet]
		[Route("api/Expense/GetExpenseForAutosuggestByCategory")]
		public IEnumerable<ExpenseViewModel> GetExpenseForAutosuggestByCategory(string value, string categoryI)
		{
			return _expenseService.GetExpenseForAutosuggestByCategory(value, categoryI);
		}

		[HttpGet]
		[Route("api/Expense/GetExpenseForComboboxByCategory")]
		public IEnumerable<ExpenseViewModel> GetExpenseForComboboxByCategory(string categoryI)
		{
			return _expenseService.GetExpenseForComboboxByCategory(categoryI);
		}

		[HttpGet]
		[Route("api/Expense/GetExpenseByCategoryIForReport")]
		public IEnumerable<ExpenseViewModel> GetExpenseByCategoryIForReport(string categoryI)
		{
			return _expenseService.GetExpenseByCategoryIForReport(categoryI);
		}
    }
}
