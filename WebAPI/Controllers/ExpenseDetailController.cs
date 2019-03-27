using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.Expense;

namespace WebAPI.Controllers
{
	public class ExpenseDetailController : ApiController
	{
		public IExpenseDetailService _expenseDetailService;

		public ExpenseDetailController() { }
		public ExpenseDetailController(IExpenseDetailService expenseDetailService)
		{
			this._expenseDetailService = expenseDetailService;
		}

		public List<ExpenseDetailViewModel> Get(string expenseCate, string dispatchI, DateTime orderD, string orderNo, int detailNo, int? dispatchNo)
		{
			return _expenseDetailService.GetExpenseDetail(expenseCate, dispatchI, orderD, orderNo, detailNo, dispatchNo);
		}
	}
}
