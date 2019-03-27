using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
//using InteractivePreGeneratedViews;
using log4net;
using Microsoft.Owin;
using Root.Data;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Service.Services;
using WebAPI.Controllers;
using Website.Utilities;

namespace WebAPI.Providers
{
	public class LicenseValidationMiddleware : OwinMiddleware
	{
		private readonly ILicenseValidation _licenseValidation;
		private readonly ITruckService _truckService;
		private readonly UserRepository _userRepository;
		protected static readonly ILog log = LogManager.GetLogger(typeof(LicenseValidationMiddleware));

		public LicenseValidationMiddleware(OwinMiddleware next, ILicenseValidation licenseValidation, ITruckService truckService)
			: base(next)
		{
			_licenseValidation = licenseValidation;
			_truckService = truckService;
			_userRepository = new UserRepository(new DatabaseFactory());
		}

		public override async Task Invoke(IOwinContext context)
		{
			var response = context.Response;
			var request = context.Request;
			var hashStrKey = System.Configuration.ConfigurationManager.AppSettings["CustomerKey"];
			var isApprovalRequest = request.Method.ToUpper() == "OPTIONS";

			if (!isApprovalRequest)
			{
				//Check where request from Mobile or Web
				var isFromMobile = request.Headers["IsMobile"] == "1";

				#region Check license when request from mobile device
				if (isFromMobile)
				{
					var customerTokenAuthentication = request.Headers["CustomerAuthentication"];

					if (string.IsNullOrEmpty(customerTokenAuthentication))
					{
						response.OnSendingHeaders(state =>
						{
							var resp = (IOwinResponse)state;
							var origin = request.Headers.Get("Origin");
							if (!string.IsNullOrEmpty(origin))
							{
								resp.Headers.Set("Access-Control-Allow-Origin", origin);
							}

							resp.Headers.Add("X-MyResponse-Header", new[] { "Wrong License" });
							resp.StatusCode = 403;
							resp.ReasonPhrase = "Forbidden";
						}, response);
					}

					var isLicenseValidated =
						_licenseValidation.CheckCustomerLicense(System.Web.Hosting.HostingEnvironment.MapPath("~/MobileLicense.lic"), customerTokenAuthentication,
							hashStrKey);

					if (!isLicenseValidated)
					{
						response.OnSendingHeaders(state =>
						{
							var resp = (IOwinResponse)state;
							var origin = request.Headers.Get("Origin");
							if (!string.IsNullOrEmpty(origin))
							{
								resp.Headers.Set("Access-Control-Allow-Origin", origin);
							}

							resp.Headers.Add("X-MyResponse-Header", new[] { "Wrong License" });
							resp.StatusCode = 403;
							resp.ReasonPhrase = "Forbidden";
						}, response);
					}
				}
				#endregion
				#region Check license when request from web application
				else
				{
					var currTruckTotal = _truckService.GetCurrentTruckTotal();
					var isLicenseValidated =
						_licenseValidation.CheckLicense(System.Web.Hosting.HostingEnvironment.MapPath("~/License.lic"), currTruckTotal,
							hashStrKey);

					if (!isLicenseValidated)
					{
						response.OnSendingHeaders(state =>
						{
							var resp = (IOwinResponse)state;
							var origin = request.Headers.Get("Origin");
							if (!string.IsNullOrEmpty(origin))
							{
								resp.Headers.Set("Access-Control-Allow-Origin", origin);
							}

							resp.Headers.Add("X-MyResponse-Header", new[] { "Wrong License" });
							resp.StatusCode = 403;
							resp.ReasonPhrase = "Forbidden";
						}, response);
					}
				}
				#endregion

				if ((request.Headers["Authorization"] ?? "").Trim().Length > 0)
				{
					var userSstp = request.Headers["UserSSTP"].Split('|');
					var sstp = userSstp[0];
					var userName = userSstp[1];
					if (!string.IsNullOrEmpty(sstp))
					{
						var isValidSstp = _userRepository.CheckSecurityStamp(userName, sstp);
						if (!isValidSstp)
						{
							response.OnSendingHeaders(state =>
							{
								var resp = (IOwinResponse)state;
								var origin = request.Headers.Get("Origin");
								if (!string.IsNullOrEmpty(origin))
								{
									resp.Headers.Set("Access-Control-Allow-Origin", origin);
								}

								resp.Headers.Add("X-MyResponse-Header", new[] { "Wrong License" });
								resp.StatusCode = 401;
								resp.ReasonPhrase = "Unauthorized";
							}, response);
						}
					}
				}


				//var customerTokenAuthentication = request.Headers["CustomerAuthentication"];

				//if (string.IsNullOrEmpty(customerTokenAuthentication))
				//{
				//	response.OnSendingHeaders(state =>
				//	{
				//		var resp = (IOwinResponse)state;
				//		var origin = request.Headers.Get("Origin");
				//		if (!string.IsNullOrEmpty(origin))
				//		{
				//			resp.Headers.Set("Access-Control-Allow-Origin", origin);
				//		}

				//		resp.Headers.Add("X-MyResponse-Header", new[] { "Wrong License" });
				//		resp.StatusCode = 403;
				//		resp.ReasonPhrase = "Forbidden";
				//	}, response);
				//}

				//var isLicenseValidated =
				//	_licenseValidation.CheckCustomerLicense(System.Web.Hosting.HostingEnvironment.MapPath("~/License.lic"), customerTokenAuthentication,
				//		hashStrKey);

				//if (!isLicenseValidated)
				//{
				//	response.OnSendingHeaders(state =>
				//	{
				//		var resp = (IOwinResponse)state;
				//		var origin = request.Headers.Get("Origin");
				//		if (!string.IsNullOrEmpty(origin))
				//		{
				//			resp.Headers.Set("Access-Control-Allow-Origin", origin);
				//		}

				//		resp.Headers.Add("X-MyResponse-Header", new[] { "Wrong License" });
				//		resp.StatusCode = 403;
				//		resp.ReasonPhrase = "Forbidden";
				//	}, response);
				//}

				//if ((request.Headers["Authorization"] ?? "").Trim().Length > 0)
				//{
				//	var userSstp = request.Headers["UserSSTP"].Split('|');
				//	var sstp = userSstp[0];
				//	var userName = userSstp[1];
				//	if (!string.IsNullOrEmpty(sstp))
				//	{
				//		var isValidSstp = _userRepository.CheckSecurityStamp(userName, sstp);
				//		if (!isValidSstp)
				//		{
				//			response.OnSendingHeaders(state =>
				//			{
				//				var resp = (IOwinResponse)state;
				//				var origin = request.Headers.Get("Origin");
				//				if (!string.IsNullOrEmpty(origin))
				//				{
				//					resp.Headers.Set("Access-Control-Allow-Origin", origin);
				//				}

				//				resp.Headers.Add("X-MyResponse-Header", new[] { "Wrong License" });
				//				resp.StatusCode = 401;
				//				resp.ReasonPhrase = "Unauthorized";
				//			}, response);
				//		}
				//	}
				//}
				
				await Next.Invoke(context);
			}
			else
			{
				await Next.Invoke(context);
			}
		}
	}
}