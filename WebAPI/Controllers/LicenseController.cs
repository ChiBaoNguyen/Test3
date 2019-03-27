using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.License;

namespace WebAPI.Controllers
{
	public class LicenseController:ApiController
	{
		public ILicenseService _licenseService;
		public ICommonService _commonService;

		public LicenseController(){}
		public LicenseController(ILicenseService licenseService, ICommonService commonService)
		{
			this._licenseService = licenseService;
			_commonService = commonService;
		}
		public IEnumerable<LicenseViewModel> Get()
		{
			return _licenseService.Get();
		}
		[Route("api/License/GetForSuggestion")]
		public IEnumerable<LicenseViewModel> GetForSuggestion(string value)
		{
			return _licenseService.GetForSuggestion(value);
		}
		[Filters.Authorize(Roles = "License_M_E")]
		public void Put(List<LicenseViewModel> licenses)
		{
			_licenseService.Update(licenses);
		}

		[HttpGet]
		[Route("api/License/CheckWhenDelete/{language}/{licenseC}")]
		[Filters.Authorize(Roles = "License_M_E")]
		public HttpResponseMessage CheckWhenDelete(string language, string licenseC)
		{
			List<string> paramsList = new List<string>() { licenseC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("License_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
			return new HttpResponseMessage()
			{
				Content = new StringContent(
					result,
					Encoding.UTF8,
					"text/html"
				)
			};
		}

		[HttpGet]
		[Route("api/License/GetByName")]
		public LicenseViewModel GetByName(string licenseN)
		{
			return _licenseService.GetByName(licenseN);
		}
	}
}