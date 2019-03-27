using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using Website.ViewModels;
using System.Threading.Tasks;
using log4net;
using Website.ViewModels.Basic;
using Website.ViewModels.CustomerPayment;


namespace WebAPI.Controllers
{
	public class CustomerPaymentController : ApiController
    {
		public ICustomerPaymentService _customerPaymentService;

		protected static readonly ILog log = LogManager.GetLogger(typeof(CustomerPaymentController));
		public CustomerPaymentController() { }
		public CustomerPaymentController(ICustomerPaymentService customerPaymentService)
		{
			this._customerPaymentService = customerPaymentService;
		}

		[HttpGet]
		public CustomerPaymentViewModel GetCustomerPayment(string customerMainC, string customerSubC, DateTime customerPaymentD, string paymentId)
		{
			return _customerPaymentService.GetCustomerPayment(customerMainC, customerSubC, customerPaymentD, paymentId);
		}

		[Route("api/CustomerPayment/Datatable")]
		public async Task<IHttpActionResult> Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "CustomerPaymentD",
				  bool reverse = true,
				  string search = null)
		{
			log.Info("test log4net");
			var custDatatable = await Task.Run(() => _customerPaymentService.GetCustomerPaymentForTable(page, itemsPerPage, sortBy, reverse, search));
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		// PUT api/<controller>/5
		[Filters.Authorize(Roles = "CustomerPayment_E")]
		public void Put(CustomerPaymentViewModel customerPayment)
		{
			_customerPaymentService.UpdateCustomerPayment(customerPayment);
		}
		[Filters.Authorize(Roles = "CustomerPayment_A")]
		public void Post(CustomerPaymentViewModel category)
		{
			_customerPaymentService.CreateCustomerPayment(category);
		}

		[HttpDelete]
		[Route("api/CustomerPayment/{customerMainC}/{customerSubC}/{customerPaymentD}/{paymentId}")]
		[Filters.Authorize(Roles = "CustomerPayment_D")]
		public void Delete(string customerMainC, string customerSubC, DateTime customerPaymentD, string paymentId)
		{
			_customerPaymentService.DeleteCustomerPayment(customerMainC, customerSubC, customerPaymentD, paymentId);
		}
	}
}