using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.Surcharge;

namespace WebAPI.Controllers
{
	public class SurchargeDetailController : ApiController
	{
		public ISurchargeDetailService _surchargeDetailService;

		public SurchargeDetailController() { }
		public SurchargeDetailController(ISurchargeDetailService surchargeDetailService)
		{
			this._surchargeDetailService = surchargeDetailService;
		}

		public List<SurchargeDetailViewModel> Get(string expenseCate, string dispatchI, DateTime orderD, string orderNo, int detailNo, int dispatchNo)
		{
			return _surchargeDetailService.GetExpenseDetail(expenseCate, dispatchI, orderD, orderNo, detailNo, dispatchNo);
		}
	}
}