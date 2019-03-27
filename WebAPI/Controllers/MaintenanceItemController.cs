using System.Net.Http;
using System.Text;
using System.Web.Http;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Website.Utilities;
using Website.ViewModels.MaintenanceItem;

namespace WebAPI.Controllers
{
	public class MaintenanceItemController : ApiController
	{
		public IMaintenanceItemService _maintenanceItemService;
		public ICommonService _commonService;
		public MaintenanceItemController(){}
		public MaintenanceItemController(IMaintenanceItemService licenseService, ICommonService commonService)
		{
			this._maintenanceItemService = licenseService;
			_commonService = commonService;
		}
		public IEnumerable<MaintenanceItemViewModel> Get()
		{
			return _maintenanceItemService.Get();
		}
		public IEnumerable<MaintenanceItemViewModel> Get(string value)
		{
			return _maintenanceItemService.GetForSuggestion(value);
		}
		[Route("api/MaintenanceItem/GetByName")]
		public MaintenanceItemViewModel GetByName(string name)
		{
			return _maintenanceItemService.GetByName(name);
		}
		[Filters.Authorize(Roles = "MaintenanceItem_M_E")]
		public void Put(List<MaintenanceItemViewModel> maintenanceItems)
		{
			_maintenanceItemService.Update(maintenanceItems);
		}

		[HttpGet]
		[Route("api/MaintenanceItem/CheckWhenDelete/{language}/{maintenanceItemC}")]
		[Filters.Authorize(Roles = "MaintenanceItem_M_E")]
		public HttpResponseMessage CheckWhenDelete(string language, string maintenanceItemC)
		{
			List<string> paramsList = new List<string>() { maintenanceItemC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("MaintenanceItem_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
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