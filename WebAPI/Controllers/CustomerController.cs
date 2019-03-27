using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Service.Services;
using Website.Utilities;
using Website.ViewModels.Customer;
using System;
using System.Text;

namespace WebAPI.Controllers
{
	public class CustomerController : ApiController
	{
		public ICustomerService _customerService;
		public ICommonService _commonService;
		protected static readonly ILog log = LogManager.GetLogger(typeof(CustomerController));

		public CustomerController() { }
		public CustomerController(ICustomerService customerService,ICommonService commonService)
		{
			this._customerService = customerService;
			_commonService = commonService;
		}

		// GET api/customer
		public IEnumerable<CustomerViewModel> Get()
		{
			return _customerService.GetCustomers();
		}

		public IEnumerable<CustomerViewModel> Get(string value)
		{
			return _customerService.GetCustomersByMainCode(value);
		}

		[Route("api/Customer/GetCustomersByCode")]
		public IEnumerable<CustomerViewModel> GetCustomersByCode(string value)
		{
			return _customerService.GetCustomersByCode(value);
		}
		[Route("api/Customer/GetMainCustomersByCode")]
		public IEnumerable<CustomerViewModel> GetMainCustomersByCode(string value)
		{
			return _customerService.GetMainCustomerByCode(value);
		}
		public IHttpActionResult Get(string mainCode, string subCode)
		{
			var customer = _customerService.GetCustomersByMainCodeSubCode(mainCode, subCode);
			if (customer == null)
			{
				return NotFound();
			}
			return Ok(customer);
		}

		[Route("api/Customer/GetCustomerName")]
		public IHttpActionResult GetCustomerName(string mainCode, string subCode)
		{
			var custName = _customerService.GetCustomerName(mainCode, subCode);
			if (custName == "")
			{
				return NotFound();
			}
			return Ok(custName);
		}

        [Route("api/Customer/GetSettlement")]
        public IHttpActionResult GetSettlement(string mainCode, string subCode, DateTime applyDate)
        {
            var settlement = _customerService.GetCustomerSettlement(mainCode, subCode, applyDate);
            if (settlement == null)
            {
                return NotFound();
            }
            return Ok(settlement);
        }

		[Route("api/Customer/Datatable")]
		public async Task<IHttpActionResult> Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "CustomerMainC",
				  bool reverse = false,
				  string search = null)
		{
			log.Info("test log4net");
			var custDatatable = await Task.Run(() => _customerService.GetCustomersForTable(page, itemsPerPage, sortBy, reverse, search));
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		[Route("api/Customer/GetInvoices/{value}")]
		public IEnumerable<InvoiceViewModel> GetInvoices(string value)
		{
			return _customerService.GetInvoices(value);
		}

		// POST api/<controller>
		[Filters.Authorize(Roles = "Customer_M_A")]
		public void Post(CustomerViewModel customer)
		{
			_customerService.CreateCustomer(customer);
		}

		// PUT api/<controller>/5
		[Filters.Authorize(Roles = "Customer_M_E")]
		public void Put(CustomerViewModel customer)
		{
			_customerService.UpdateCustomer(customer);
		}

		[HttpGet]
		[Route("api/Customer/GetByName")]
		public CustomerViewModel GetByName(string value)
		{
			return _customerService.GetByName(value);
		}

		[HttpGet]
		[Route("api/Customer/CheckExistCustomer")]
		public CustomerViewModel CheckExistCustomer(string name, string mainc, string subc)
		{
			return _customerService.CheckExistCustomer(name, mainc, subc);
		}

		// DELETE api/customer/5
		[HttpDelete]
		[Route("api/Customer/{mainCode}/{subCode}")]
		[Filters.Authorize(Roles = "Customer_M_D")]
		public void Delete(string mainCode, string subCode)
		{
			_customerService.DeleteCustomer(mainCode, subCode);
		}

		[HttpGet]
		[Route("api/Customer/CheckWhenDelete/{language}/{mainCode}/{subCode}")]
		[Filters.Authorize(Roles = "Customer_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string mainCode, string subCode)
		{
			List<string> paramsList = new List<string>() { mainCode, subCode };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Customer_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
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
		[Route("api/Customer/SetStatusCustomer/{mainCode}/{subCode}")]
		[Filters.Authorize(Roles = "Customer_M_E")]
		public void SetStatusCustomer(string mainCode, string subCode)
		{
			_customerService.SetStatusCustomer(mainCode, subCode);
		}

		[HttpGet]
		[Route("api/Customer/GetCustomerSettlementList")]
		public CustomerInvoiceViewModel GetCustomerSettlementList(string customerMainC, string customerSubC)
		{
			return _customerService.GetCustomerSettlementList(customerMainC, customerSubC);
		}

		[Route("api/Customer/GetInvoices")]
		public IEnumerable<CustomerViewModel> GetInvoices()
		{
			return _customerService.GetInvoices();
		}

		[HttpGet]
		[Route("api/Customer/GetCustomerSettlementByRevenueD")]
		public CustomerSettlementViewModel GetCustomerSettlementByRevenueD(string customerMainC, string customerSubC, DateTime revenueD)
		{
			return _customerService.GetCustomerSettlementByRevenueD(customerMainC, customerSubC, revenueD);
		}

		[HttpGet]
		[Route("api/Customer/GetPaymentCompanies")]
		public IEnumerable<CustomerViewModel> GetPaymentCompanies(string value)
		{
			return _customerService.GetPaymentCompanies(value);
		}

		[HttpGet]
		[Route("api/Customer/GetCustomersForReport")]
		public IEnumerable<CustomerViewModel> GetCustomersForReport()
		{
			return _customerService.GetCustomersForReport();
		}

		[HttpGet]
		[Route("api/Customer/GetByInvoiceName")]
		public CustomerViewModel GetByInvoiceName(string value)
		{
			return _customerService.GetByInvoiceName(value);
		}
	}
}