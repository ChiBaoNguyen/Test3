using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Container;
using System.Net.Http;

namespace WebAPI.Controllers
{
	public class ContainerTypeController : ApiController
	{
		public IContainerTypeService _containertypeService;
		public ICommonService _commonService;

		public ContainerTypeController() { }
		public ContainerTypeController(IContainerTypeService containertypeService, ICommonService commonService)
		{
			this._containertypeService = containertypeService;
			_commonService = commonService;
		}

		// GET api/customer
		public async Task<IEnumerable<ContainerTypeViewModel>> Get()
		{
			return await Task.Run(() =>  _containertypeService.GetContainerTypes());
		}

		public IEnumerable<ContainerTypeViewModel> Get(string value)
		{
			return _containertypeService.GetContainerTypes(value);
		}

		[Route("api/ContainerType/GetContainerTypesByCode")]
		public IEnumerable<ContainerTypeViewModel> GetContainerTypesByCode(string value)
		{
			return _containertypeService.GetContainerTypesByCode(value);
		}

		[HttpGet]
		[Route("api/ContainerType/GetContainerTypeByCode")]
		public IHttpActionResult GetContainerTypeByCode(string containerTypeCode)
		{
			var containertype = _containertypeService.GetContainerTypeByCode(containerTypeCode);
			if (containertype == null)
			{
				return NotFound();
			}
			return Ok(containertype);
		}

		[Route("api/ContainerType/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "ContainerTypeC",
				  bool reverse = false,
				  string search = null)
		{
			var custDatatable = _containertypeService.GetContainerTypesForTable(page, itemsPerPage, sortBy, reverse, search);
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		// POST api/<controller>
		[Filters.Authorize(Roles = "ContainerType_M_A")]
		public void Post(ContainerTypeViewModel containertype)
		{
			_containertypeService.CreateContainerType(containertype);
		}

		// PUT api/<controller>/5
		[Filters.Authorize(Roles = "ContainerType_M_E")]
		public void Put(ContainerTypeViewModel containertype)
		{
			_containertypeService.UpdateContainerType(containertype);
		}

		// DELETE api/customer/5
		[Filters.Authorize(Roles = "ContainerType_M_D")]
		public void Delete(string id)
		{
			_containertypeService.DeleteContainerType(id);
		}

		[HttpGet]
		[Route("api/ContainerType/CheckWhenDelete/{language}/{ContainerTypeC}")]
		[Filters.Authorize(Roles = "ContainerType_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string ContainerTypeC)
		{
			List<string> paramsList = new List<string>() { ContainerTypeC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("ContainerType_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
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
		[Route("api/ContainerType/SetStatusContainerType/{id}")]
		[Filters.Authorize(Roles = "ContainerType_M_E")]
		public void SetStatusCustomer(string id)
		{
			_containertypeService.SetStatusContainerType(id);
		}

		[HttpGet]
		[Route("api/ContainerType/GetByName")]
		public ContainerTypeViewModel GetByName(string value)
		{
			return _containertypeService.GetByName(value);
		}
	}
}
