using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.CustomerPricing;

namespace WebAPI.Controllers
{
	public class CustomerPricingController : ApiController
	{
		public ICustomerPricingService _customerPricingService;

		public CustomerPricingController(ICustomerPricingService customerPricingService)
		{
			this._customerPricingService = customerPricingService;
		}

		public IHttpActionResult Get(string loc1C, string loc2C, string conSizeI, string conTypeC, DateTime estimateD, string customerSubC = "", string customerMainC = "")
		{
			var customerPricing = _customerPricingService.GetCustomerPricing(loc1C, loc2C, conSizeI, conTypeC, estimateD, customerSubC, customerMainC);
			return Ok(customerPricing);
		}

		[Route("api/CustomerPricing/GetSuggestedRoutes")]
		public IHttpActionResult GetSuggestedRoutes(string loc1C, string loc2C, string conSizeI, string conTypeC, DateTime estimateD, string customerSubC = "", string customerMainC = "")
		{
			var customerPricing = _customerPricingService.GetSuggestedRoutes(loc1C, loc2C, conSizeI, conTypeC, estimateD, customerSubC, customerMainC);
			return Ok(customerPricing);
		}

		public IHttpActionResult Get(string custPricingId)
		{
			var route = _customerPricingService.GetCustomerPricingById(custPricingId);
			return Ok(route);
		}

		[Route("api/CustomerPricing/Datatable")]
		public async Task<IHttpActionResult> Post(CustomerPricingSearchParams search)
		{
			var datatable = await Task.Run(() => _customerPricingService.GetCustomerPricingForTable(search));
			if (datatable == null)
			{
				return NotFound();
			}
			return Ok(datatable);
		}

		[HttpPost]
		[Route("api/CustomerPricing/GetSuggestedExpensesFromRoute")]
		public async Task<IHttpActionResult> GetSuggestedExpensesFromRoute(List<SuggestedRoute> suggestedRoutes)
		{
			var suggestedExpenses =  await Task.Run(() =>_customerPricingService.GetSuggestedExpensesFromRoute(suggestedRoutes));
			return Ok(suggestedExpenses);
		}

		[HttpPost]
		[Route("api/CustomerPricing/GetSuggestedExpensesFromHistory")]
		public async Task<IHttpActionResult> GetSuggestedExpensesFromHistory(List<SuggestedRoute> suggestedRoutes)
		{
			var suggestedExpenses = await Task.Run(() => _customerPricingService.GetSuggestedExpensesFromHistory(suggestedRoutes));
			return Ok(suggestedExpenses);
		}

		// POST api/<controller>
		//[Authorize(Roles = "CustomerPricing_A")]
		public void Post(CustomerPricingViewModel custPricingViewModel)
		{
			_customerPricingService.CreateCustomerPricing(custPricingViewModel);
		}

		// PUT api/<controller>/5
		//[Authorize(Roles = "CustomerPricing_E")]
		public void Put(CustomerPricingViewModel routeViewModel)
		{
			_customerPricingService.UpdateCustomerPricing(routeViewModel);
		}

		// DELETE api/customer/5
		//[Authorize(Roles = "CustomerPricing_D")]
		public void Delete(string id)
		{
			_customerPricingService.DeleteCustomerPricing(id);
		}
	}
}