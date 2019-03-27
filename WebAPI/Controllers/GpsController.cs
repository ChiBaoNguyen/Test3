using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Employee;
using Website.ViewModels.Driver;
using Website.ViewModels.Gps;

namespace WebAPI.Controllers
{
	public class GpsController : ApiController
	{
		public IGpsService _gpsService;
		public ICommonService _commonService;

		public GpsController() { }
        public GpsController(IGpsService gpsService, ICommonService commonService)
		{
            this._gpsService = gpsService;
            this._commonService = commonService;
		}

		public List<GpsViewModel> Get()
		{
            return _gpsService.GetGpsLocationList();
		}

	}
}