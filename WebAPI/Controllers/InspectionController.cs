using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Inspection;

namespace WebAPI.Controllers
{
	public class InspectionController:ApiController
	{
		private IInspectionService _inspectionService;
		private ICommonService _commonService;
		public InspectionController() { }
		public InspectionController(IInspectionService inspectionService, ICommonService commonService)
		{
			this._inspectionService = inspectionService;
			_commonService = commonService;
		}
		[Route("api/Inspection/{objectI}")]
		public IEnumerable<InspectionViewModel> Get(string objectI)
		{
			return _inspectionService.Get(objectI);
		}
		[Filters.Authorize(Roles = "Inspection_M_E")]
		public void Put(List<InspectionViewModel> inspections)
		{
			_inspectionService.Update(inspections);
		}

		[HttpGet]
		[Route("api/Inspection/GetForSuggestion")]
		public IEnumerable<InspectionViewModel> GetForSuggestion(string textSearch, string objectI)
		{
			return _inspectionService.GetForSuggestion(textSearch, objectI);
		}

		[HttpGet]
		[Route("api/Inspection/GetByName")]
		public InspectionViewModel GetByName(string inspectionN, string objectI)
		{
			return _inspectionService.GetByName(inspectionN, objectI);
		}
		[HttpGet]
		[Route("api/Inspection/CheckWhenDelete/{language}/{inspectionC}")]
		[Filters.Authorize(Roles = "Inspection_M_E")]
		public HttpResponseMessage CheckWhenDelete(string language, string inspectionC)
		{
			List<string> paramsList = new List<string>() { inspectionC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Inspection_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
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