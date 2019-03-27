using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.DriverLicense;
using Website.ViewModels.DriverLicenseUpdate;

namespace WebAPI.Controllers
{
    public class DriverLicenseController : ApiController
    {
	    private IDriverLicenseService _driverLicenseService;

	    public DriverLicenseController(IDriverLicenseService driverLicenseService)
	    {
		    _driverLicenseService = driverLicenseService;
	    }
		[Route("api/DriverLicense/GetByDriverC/{driverC}")]
		public IEnumerable<DriverLicenseViewModel> GetByDriverC(string driverC)
	    {
		    return _driverLicenseService.GetByDriverC(driverC);
	    }
		public void Put(List<DriverLicenseViewModel> licenseList)
		{
			_driverLicenseService.UpdateDriverLicenses(licenseList);
		}

		[HttpGet]
		[Route("api/DriverLicense/GetDriverLicenseUpdateDetail")]
		public DriverLicenseUpdateViewModel GetDriverLicenseUpdateDetail(string driverC, string licenseC, DateTime updateD, int planI)
		{
			return _driverLicenseService.GetDriverLicenseUpdateDetail(driverC, licenseC, updateD, planI);
		}

		[HttpPost]
		[Route("api/DriverLicense/UpdateDriverLicenseDetail")]
		public void UpdateDriverLicenseDetail(DriverLicenseUpdateViewModel data)
		{
			_driverLicenseService.UpdateDriverLicenseDetail(data);
		}

		[HttpGet]
		[Route("api/DriverLicense/DeleteDriverLicenseUpdateDetail")]
		public void DeleteDriverLicenseUpdateDetail(string driverC, string licenseC, DateTime expiryD)
		{
			_driverLicenseService.DeleteDriverLicenseUpdateDetail(driverC, licenseC, expiryD);
		}
    }
}