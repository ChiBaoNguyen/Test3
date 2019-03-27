using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using Website.Utilities;

namespace WebAPI.Controllers
{
    public class ApplicationLicenseController : ApiController
    {
		public ILicenseValidation _licenseValidation;
		public ITruckService _truckService;

		public ApplicationLicenseController() { }
		public ApplicationLicenseController(ILicenseValidation licenseValidation, ITruckService truckService)
		{
			this._licenseValidation = licenseValidation;
			this._truckService = truckService;
		}

		public bool Get()
		{
			var hashStrKey = System.Configuration.ConfigurationManager.AppSettings["CustomerKey"];
			var currTruckTotal = _truckService.GetCurrentTruckTotal();
			return _licenseValidation.CheckLicense(System.Web.Hosting.HostingEnvironment.MapPath("~/License.lic"), currTruckTotal, hashStrKey);
		}
    }
}
