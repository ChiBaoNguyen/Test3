using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.MaintenanceDetail;

namespace WebAPI.Controllers
{
    public class MaintenanceDetailController : ApiController
    {
	    private IMaintenanceDetailService _maintenanceDetailService;

	    public MaintenanceDetailController(IMaintenanceDetailService maintenanceDetailService)
	    {
		    _maintenanceDetailService = maintenanceDetailService;
	    }

		public IEnumerable<MaintenanceDetailViewModel> Get(string objectI, string code, string modelC)
	    {
		    return _maintenanceDetailService.Get(objectI, code, modelC);
	    }
    }
}
