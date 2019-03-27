using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using WebAPI.Filters;
using Website.ViewModels;
using System.Threading.Tasks;
using log4net;
using Website.ViewModels.Basic;


namespace WebAPI.Controllers
{
	public class BasicSettingController : ApiController
    {
		public IBasicSettingService _basicSettingService;

		protected static readonly ILog log = LogManager.GetLogger(typeof(BasicSettingController));
		public BasicSettingController() { }
		public BasicSettingController(IBasicSettingService basicSettingService)
		{
			this._basicSettingService = basicSettingService;
		}
		// GET api/basicsetting
		public IEnumerable<BasicViewModel> Get()
		{
			var bkpPath = System.Configuration.ConfigurationManager.AppSettings["BackupPath"] + "Info.txt";
			return _basicSettingService.GetBasicSetting(System.Web.Hosting.HostingEnvironment.MapPath("~/License.lic"), bkpPath);
		}

		[Filters.Authorize(Roles = "BasicSetting_E")]
		//insert into database
		public void Post(BasicViewModel basicsetting)
		{
			_basicSettingService.CreateBasicSetting(basicsetting);
		}
		public string GetContainerStatus(DateTime orderD, string orderNo, int detailNo)
		{
			return _basicSettingService.GetContainerStatus(orderD, orderNo, detailNo);
		}
	}
}