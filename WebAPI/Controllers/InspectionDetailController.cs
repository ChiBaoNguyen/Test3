using System.Collections.Generic;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.InspectionDetail;
using Website.ViewModels.Truck;

namespace WebAPI.Controllers
{
    public class InspectionDetailController : ApiController
    {
	    private IInspectionDetailService _inspectionDetailService;

		public InspectionDetailController(IInspectionDetailService inspectionDetailService)
	    {
		    _inspectionDetailService = inspectionDetailService;
	    }
    }
}
