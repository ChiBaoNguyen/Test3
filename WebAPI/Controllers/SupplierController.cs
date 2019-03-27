using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;
using log4net;
using Service.Services;
using Website.ViewModels.Supplier;
using Website.Utilities;
using System.Net.Http;

namespace WebAPI.Controllers
{
	public class SupplierController : ApiController
	{
		public ISupplierService _supplierService;
		public ICommonService _commonService;
        protected static readonly ILog log = LogManager.GetLogger(typeof(SupplierController));

		public SupplierController() { }

		public SupplierController(ISupplierService supplierService, ICommonService commonService)
		{
			this._supplierService = supplierService;
			_commonService = commonService;
		}

		public IEnumerable<SupplierViewModel> Get()
		{
			return _supplierService.GetSuppliers();
		}

		[System.Web.Http.Route("api/Supplier/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
                  string sortBy = "SupplierMainC",
				  bool reverse = false,
				  string search = null)
		{
			var supplierDatatable = _supplierService.GetSupplierForTable(page, itemsPerPage, sortBy, reverse, search);
			if (supplierDatatable == null)
			{
				return NotFound();
			}
			return Ok(supplierDatatable);
		}
		[Filters.Authorize(Roles = "Supplier_M_A")]
		public void Post(SupplierViewModel supplier)
		{
			_supplierService.CreateSupplier(supplier);
		}

		[HttpGet]
		[Route("api/Supplier/GetSupplierByCode")]
		public IHttpActionResult GetSupplierByCode(string supplierCode)
		{
			var supplier = _supplierService.GetSupplierSizeByCode(supplierCode);
            if (supplier == null)
			{
				return NotFound();
			}
            return Ok(supplier);
		}
		[Filters.Authorize(Roles = "Supplier_M_E")]
		public void Put(SupplierViewModel supplier)
		{
			_supplierService.UpdateSupplier(supplier);
		}

		[HttpDelete]
		[Route("api/Supplier/{mainCode}/{subCode}")]
		[Filters.Authorize(Roles = "Supplier_M_D")]
		public void Delete(string mainCode, string subCode)
		{
			_supplierService.DeleteSupplier(mainCode, subCode);
		}

		[HttpGet]
		[Route("api/Supplier/CheckWhenDelete/{language}/{mainCode}/{subCode}")]
		[Filters.Authorize(Roles = "Supplier_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string mainCode, string subCode)
		{
			List<string> paramsList = new List<string>() { mainCode, subCode };
			return new HttpResponseMessage()
			{
				Content = new StringContent(
					_commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Supplier_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList),
					Encoding.UTF8,
					"text/html"
				)
			};
		}

		[HttpGet]
        [Route("api/Supplier/SetStatusSupplier/{mainCode}/{subCode}")]
		[Filters.Authorize(Roles = "Supplier_M_E")]
        public void SetStatusSupplier(string mainCode, string subCode)
		{
            _supplierService.SetStatusSupplier(mainCode, subCode);
		}

		public IEnumerable<SupplierViewModel> Get(string value)
		{
			return _supplierService.GetSupplierForSuggestion(value);
		}

		[Route("api/Supplier/GetSuppliersByCode")]
		public IEnumerable<SupplierViewModel> GetSuppliersByCode(string value)
		{
			return _supplierService.GetSuppliersByCode(value);
		}

		[Route("api/Supplier/GetMainSuppliersByCode")]
		public IEnumerable<SupplierViewModel> GetMainSuppliersByCode(string value)
		{
			return _supplierService.GetMainSuppliersByCode(value);
		}
		[HttpGet]
		[Route("api/Supplier/GetByName")]
		public SupplierViewModel GetByName(string value)
		{
			return _supplierService.GetByName(value);
		}
        public IHttpActionResult Get(string mainCode, string subCode)
        {
            var customer = _supplierService.GetSupplierByMainCodeSubCode(mainCode, subCode);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [Route("api/Supplier/GetSettlement")]
        public IHttpActionResult GetSettlement(string mainCode, string subCode, DateTime applyDate)
        {
            var settlement = _supplierService.GetSupplierSettlement(mainCode, subCode, applyDate);
            if (settlement == null)
            {
                return NotFound();
            }
            return Ok(settlement);
        }

		[HttpGet]
		[Route("api/Supplier/GetSupplierSettlementList")]
		public SupplierInvoiceSettlementViewModel GetSupplierSettlementList(string supplierMainC, string supplierSubC)
		{
			return _supplierService.GetSupplierSettlementList(supplierMainC, supplierSubC);
		}

		[HttpGet]
		[Route("api/Supplier/GetPaymentCompanies")]
		public IEnumerable<SupplierViewModel> GetPaymentCompanies(string value)
		{
			return _supplierService.GetPaymentCompanies(value);
		}

		[HttpGet]
		[Route("api/Supplier/GetSuppliersForReport")]
		public IEnumerable<SupplierViewModel> GetSuppliersForReport()
		{
			return _supplierService.GetSuppliersForReport();
		}

		[HttpGet]
		[Route("api/Supplier/GetByInvoiceName")]
		public SupplierViewModel GetByInvoiceName(string value)
		{
			return _supplierService.GetByInvoiceName(value);
		}
	}
}
