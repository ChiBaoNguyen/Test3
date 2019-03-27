using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Feature;

namespace WebAPI.Controllers
{
	public class FeatureController : ApiController
	{
		public IFeatureService _featuretypeService;
		public FeatureController() { }
		public FeatureController(IFeatureService featuretypeService)
		{
			this._featuretypeService = featuretypeService;
		}

		public IEnumerable<FeatureViewModel> Get(string value)
		{
			return _featuretypeService.GetFeatures(value);
		}

		[Route("api/Feature/GetFeaturesAutosuggest")]
		public IEnumerable<FeatureViewModel> GetFeaturesAutosuggest(string value)
		{
			return _featuretypeService.GetFeaturesAutosuggest(value);
		}

		[HttpGet]
		[Route("api/Feature/GetFeatureByCode")]
		public IHttpActionResult GetFeatureByCode(string featureC)
		{
			var feature = _featuretypeService.GetFeatureByCode(featureC);
			if (feature == null)
			{
				return NotFound();
			}
			return Ok(feature);
		}

		[Route("api/Feature/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "FeatureC",
				  bool reverse = false,
				  string search = null)
		{
			var custDatatable = _featuretypeService.GetFeaturesForTable(page, itemsPerPage, sortBy, reverse, search);
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		// POST api/<controller>
		public void Post(FeatureViewModel featuretype)
		{
			_featuretypeService.CreateFeature(featuretype);
		}

		// PUT api/<controller>/5
		public void Put(FeatureViewModel featuretype)
		{
			_featuretypeService.UpdateFeature(featuretype);
		}

		// DELETE api/customer/5
		public void Delete(string id)
		{
			_featuretypeService.DeleteFeature(id);
		}

		[HttpGet]
		[Route("api/Feature/GetByName")]
		public FeatureViewModel GetByName(string value)
		{
			return _featuretypeService.GetByName(value);
		}
	}
}