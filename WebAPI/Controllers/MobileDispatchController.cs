using Microsoft.AspNet.SignalR;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Website.ViewModels.Mobile.Dispatch;

namespace WebAPI.Controllers
{
	public class MobileDispatchController : MobileBaseController
	{
		public IDispatchService _dispatchService;
		public ITrailerService _trailerService;
		public IGpsService _gpsService;
		public IBasicSettingService _basicSettingService;

		public MobileDispatchController()
		{
		}

		public MobileDispatchController(IDispatchService dispatchService, ITrailerService trailerService, IGpsService gpsService, IBasicSettingService basicSettingService)
		{
			this._dispatchService = dispatchService;
			this._trailerService = trailerService;
			this._gpsService = gpsService;
			this._basicSettingService = basicSettingService;
		}

		[HttpGet]
		public HttpResponseMessage Get(DateTime date, string driverC)
		{
			return base.BuildSuccessResult(HttpStatusCode.OK, _dispatchService.MGetDispatchList(date, driverC));
		}

		[HttpPost]
		public HttpResponseMessage Post(MobileDispatchViewModel dispatch)
		{
			var response = _dispatchService.MUpdateDispatch(dispatch);

			//Make asynchronous when dispatch is updated from mobile device
			var context = GlobalHost.ConnectionManager.GetHubContext<DispatchMessageHub>();
			context.Clients.All.handleDispatchMessage();

			return base.BuildSuccessResult(HttpStatusCode.OK, response);
		}

		[HttpGet]
		public HttpResponseMessage GetTrailerList()
		{
			return base.BuildSuccessResult(HttpStatusCode.OK, _trailerService.MGetTrailerList());
		}

		[HttpGet]
		public HttpResponseMessage UpdateGpsLocation(string driverC, string latitude, string longitude)
		{
			return base.BuildSuccessResult(HttpStatusCode.OK, _gpsService.MUpdateLocation(driverC, latitude, longitude));
		}

		[HttpGet]
		[Route("api/MobileDispatch/GetBasicSetting")]
		public HttpResponseMessage GetBasicSetting()
		{
			return base.BuildSuccessResult(HttpStatusCode.OK, _basicSettingService.GetDispatchTransportedColor());
		}
	}
}
