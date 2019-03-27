using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.Allowance;

namespace WebAPI.Controllers
{
    public class AllowanceDetailController : ApiController
    {
		public IAllowanceDetailService _allowanceDetailService;

		public AllowanceDetailController() { }
		public AllowanceDetailController(IAllowanceDetailService allowanceDetailService)
		{
			this._allowanceDetailService = allowanceDetailService;
		}

		public List<AllowanceDetailViewModel> Get(string expenseCate, DateTime orderD, string orderNo, int detailNo, int dispatchNo)
		{
			return _allowanceDetailService.GetExpenseDetail(expenseCate, orderD, orderNo, detailNo, dispatchNo);
		}
    }
}
