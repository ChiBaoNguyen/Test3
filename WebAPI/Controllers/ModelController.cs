using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.MaintenanceItemDetail;
using Website.ViewModels.Model;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
	public class ModelController:ApiController
	{
		private IModelService _modelService;
		private IMaintenanceItemDetailService _maintenanceItemDetailService;
		private ICommonService _commonService;
		public ModelController() { }

		public ModelController(IModelService modelService,IMaintenanceItemDetailService maintenanceItemDetailService, ICommonService commonService)
		{
			_modelService = modelService;
			_maintenanceItemDetailService = maintenanceItemDetailService;
			_commonService = commonService;
		}

		public IEnumerable<ModelViewModel> Get(string value, string objectI)
		{
			return _modelService.GetModelForSuggestion(value, objectI);
		}

		[Route("api/Model/GetSuggestionTruckModel")]
		public IEnumerable<ModelViewModel> GetSuggestionTruckModel(string value)
		{
			return _modelService.GetModelForSuggestion(value, "0");
		}

		[Route("api/Model/UpdateModel")]
		[Filters.Authorize(Roles = "Model_M_E")]
		public void Update(ModelViewModel model)
		{
			_modelService.Update(model);
		}
		[Filters.Authorize(Roles = "Model_M_A")]
		public void Post(ModelViewModel model)
		{
			_modelService.Create(model);
		}

		[Route("api/Model/GetMaintenanceItemDetail/{objectI}/{modelC}")]
		public IEnumerable<MaintenanceItemDetailViewModel> GetMaintenanceItemDetail(string objectI, string modelC)
		{
			return _maintenanceItemDetailService.GetByModelC(objectI, modelC);
		}

		[Route("api/Model/GetModel/{objectI}/{modelC}")]
		public ModelViewModel GetModel(string objectI, string modelC)
		{
			return _modelService.Get(objectI, modelC);
		}

		[Route("api/Model/GetModelByName")]
		public ModelViewModel GetModelByName(string objectI, string modelN)
		{
			return _modelService.GetByName(objectI, modelN);
		}

		[Route("api/Model/GetTruckModelByName")]
		public ModelViewModel GetTruckModelByName(string value)
		{
			return _modelService.GetByName("0", value);
		}

		[Route("api/Model/DeleteModel/{objectI}/{modelC}")]
		[Filters.Authorize(Roles = "Model_M_D")]
		public void Delete(string objectI, string modelC)
		{
			_modelService.Delete(objectI, modelC);
		}

		[HttpGet]
		[Route("api/Model/CheckWhenDelete/{language}/{modelC}/{objectI}")]
		[Filters.Authorize(Roles = "Model_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string modelC, string objectI)
		{
			List<string> paramsList = new List<string>() { modelC, objectI };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Model_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
			return new HttpResponseMessage()
			{
				Content = new StringContent(
					result,
					Encoding.UTF8,
					"text/html"
				)
			};
		}

		[Route("api/Model/Datatable")]
		public async Task<IHttpActionResult> Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "ModelC",
				  bool reverse = false,
				  string search = null)
		{
			var modelDatatable = await Task.Run(() => _modelService.GetModelsForTable(page, itemsPerPage, sortBy, reverse, search));
			if (modelDatatable == null)
			{
				return NotFound();
			}
			return Ok(modelDatatable);
		}
	}
}