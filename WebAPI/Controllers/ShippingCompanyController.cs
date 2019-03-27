using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Order;
using Website.ViewModels.Ship;

namespace WebAPI.Controllers
{
	public class ShippingCompanyController : ApiController
	{
		public IShippingCompanyService _shippingCompanyService;
		public ICommonService _commonService;

		public ShippingCompanyController() { }

		public ShippingCompanyController(IShippingCompanyService shippingCompanyService, ICommonService commonService)
		{
			this._shippingCompanyService = shippingCompanyService;
			_commonService = commonService;
		}

		[System.Web.Http.Route("api/ShippingCompany/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "ShippingCompanyC",
				  bool reverse = false,
				  string search = null)
		{
			var shippingCompanyDatatable = _shippingCompanyService.GetShippingCompanyForTable(page, itemsPerPage, sortBy, reverse, search);
			if (shippingCompanyDatatable == null)
			{
				return NotFound();
			}
			return Ok(shippingCompanyDatatable);
		}
		[Filters.Authorize(Roles = "ShippingCompany_M_A")]
		public void Post(ShippingCompanyViewModel shippingCompany)
		{
			_shippingCompanyService.CreateShippingCompany(shippingCompany);
		}

		[HttpGet]
		[Route("api/ShippingCompany/GetShippingCompanyByCode")]
		public IHttpActionResult GetShippingCompanyByCode(string shippingCompanyCode)
		{
			var shippingCompany = _shippingCompanyService.GetShippingCompanyByCode(shippingCompanyCode);
			if (shippingCompany == null)
			{
				return NotFound();
			}
			return Ok(shippingCompany);
		}
		[Filters.Authorize(Roles = "ShippingCompany_M_E")]
		public void Put(ShippingCompanyViewModel shippingCompany)
		{
			_shippingCompanyService.UpdateShippingCompany(shippingCompany);
		}

		[Filters.Authorize(Roles = "ShippingCompany_M_D")]
		public void Delete(string id)
		{
			_shippingCompanyService.DeleteShippingCompany(id);
		}

		[HttpGet]
		[Route("api/ShippingCompany/CheckWhenDelete/{language}/{shippingCompanyC}")]
		[Filters.Authorize(Roles = "ShippingCompany_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string shippingCompanyC)
		{
			List<string> paramsList = new List<string>() { shippingCompanyC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("ShippingCompany_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
			return new HttpResponseMessage()
			{
				Content = new StringContent(
					result,
					Encoding.UTF8,
					"text/html"
				)
			};
		}

		public IEnumerable<ShippingCompanyViewModel> Get(string value)
		{
			return _shippingCompanyService.GetShippingCompanyForSuggestion(value);
		}

		[Route("api/ShippingCompany/GetShippingCompaniesByCode")]
		public IEnumerable<ShippingCompanyViewModel> GetShippingCompaniesByCode(string value)
		{
			return _shippingCompanyService.GetShippingCompaniesByCode(value);
		}

		[HttpGet]
		[Route("api/ShippingCompany/GetByName")]
		public ShippingCompanyViewModel GetByName(string value)
		{
			return _shippingCompanyService.GetByName(value);
		}

		[HttpGet]
		[Route("api/ShippingCompany/SetStatusShippingCompany/{id}")]
		[Filters.Authorize(Roles = "ShippingCompany_M_E")]
		public void SetStatusShippingCompany(string id)
		{
			_shippingCompanyService.SetStatusShippingCompany(id);
		}

		[HttpGet]
		[Route("api/ShippingCompany/GetShippingCompanyCodes")]
		public IEnumerable<string> GetShippingCompanyCodes(string value, string shipComC)
		{
			return _shippingCompanyService.GetShippingCompanyCodes(value, shipComC);
		}
	}
}
