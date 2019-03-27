using System.Net.Http;
using System.Text;
using System.Web.Http;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Website.Utilities;
using Website.ViewModels.Operation;

namespace WebAPI.Controllers
{
	public class OperationController : ApiController
	{
		public IOperationService _operationService;
		public ICommonService _commonService;
		public OperationController(){}
		public OperationController(IOperationService licenseService, ICommonService commonService)
		{
			this._operationService = licenseService;
			_commonService = commonService;
		}
		[Route("api/Operation/{orderTypeI}")]
		public IEnumerable<OperationViewModel> Get(string orderTypeI)
		{
			return _operationService.Get(orderTypeI);
		}

		[HttpGet]
		[Route("api/Operation")]
		public IEnumerable<OperationViewModel> Get()
		{
			return _operationService.Get();
		}
		public IEnumerable<OperationViewModel> GetForSuggestion(string value, string orderTypeI)
		{
			return _operationService.GetForSuggestion(value, orderTypeI);
		}
		[Route("api/Operation/GetByName")]
		public OperationViewModel GetByName(string name, string orderTypeI)
		{
			return _operationService.GetByName(name, orderTypeI);
		}

		[HttpPut]
		[Route("api/Operation/UpdateOperation")]
		[Filters.Authorize(Roles = "Operation_M_E")]
		public void UpdateOperation(OperationViewModelForUpdate sentData)
		{
			List<OperationViewModel> operations = sentData.Operations;
			string orderTypeI = sentData.OrderTypeI;

			_operationService.Update(operations);
		}

		[HttpGet]
		[Route("api/Operation/CheckWhenDelete/{language}/{operationC}")]
		[Filters.Authorize(Roles = "Operation_M_E")]
		public HttpResponseMessage CheckWhenDelete(string language, string operationC)
		{
			List<string> paramsList = new List<string>() { operationC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Operation_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
			return new HttpResponseMessage()
			{
				Content = new StringContent(
					result,
					Encoding.UTF8,
					"text/html"
				)
			};
		}
	}
}