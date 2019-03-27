using System.Collections.Generic;
using System.Text;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.Partner;
using System;
using Website.Utilities;
using System.Net.Http;

namespace WebAPI.Controllers
{
    public class PartnerController : ApiController
    {
		 public IPartnerService _partnerService;
		 public ICommonService _commonService;

		public PartnerController() { }

		public PartnerController(IPartnerService partnerService, ICommonService commonService)
		{
			this._partnerService = partnerService;
			_commonService = commonService;
		}

		public IEnumerable<PartnerViewModel> Get()
		{
			return _partnerService.GetPartners();
		}

		public IEnumerable<PartnerViewModel> Get(string value)
		{
			return _partnerService.GetPartnerForSuggestion(value);
		}

		[Route("api/Partner/GetPartnersByCode")]
		public IEnumerable<PartnerViewModel> GetPartnersByCode(string value)
		{
			return _partnerService.GetPartnersByCode(value);
		}

		[Route("api/Partner/GetMainPartnersByCode")]
		public IEnumerable<PartnerViewModel> GetMainPartnersByCode(string value)
		{
			return _partnerService.GetMainPartnersByCode(value);
		}
		[HttpGet]
		[Route("api/Partner/GetByName")]
		public PartnerViewModel GetByName(string value)
		{
			return _partnerService.GetByName(value);
		}

		[HttpGet]
		[Route("api/Partner/CheckExistPartner")]
		public PartnerViewModel CheckExistPartner(string name, string mainc, string subc)
		{
			return _partnerService.CheckExistPartner(name, mainc, subc);
		}

        [System.Web.Http.Route("api/Partner/Datatable")]
        public IHttpActionResult Get(
                  int page = 1,
                  int itemsPerPage = 10,
                  string sortBy = "PartnerMainC",
                  bool reverse = false,
                  string search = null)
        {
            var partnerDatatable = _partnerService.GetPartnerForTable(page, itemsPerPage, sortBy, reverse, search);
            if (partnerDatatable == null)
            {
                return NotFound();
            }
            return Ok(partnerDatatable);
        }
		[Filters.Authorize(Roles = "Partner_M_A")]
        public void Post(PartnerViewModel partner)
        {
            _partnerService.CreatePartner(partner);
        }

        [HttpGet]
        [Route("api/Partner/GetPartnerByCode")]
        public IHttpActionResult GetPartnerByCode(string partnerCode)
        {
            var partner = _partnerService.GetPartnerSizeByCode(partnerCode);
            if (partner == null)
            {
                return NotFound();
            }
            return Ok(partner);
        }
		[Filters.Authorize(Roles = "Partner_M_E")]
        public void Put(PartnerViewModel partner)
        {
            _partnerService.UpdatePartner(partner);
        }

		[HttpDelete]
		[Route("api/Partner/{mainCode}/{subCode}")]
		[Filters.Authorize(Roles = "Partner_M_D")]
		public void Delete(string mainCode, string subCode)
		{
			_partnerService.DeletePartner(mainCode, subCode);
		}

		[HttpGet]
		[Route("api/Partner/CheckWhenDelete/{language}/{mainCode}/{subCode}")]
		[Filters.Authorize(Roles = "Partner_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string mainCode, string subCode)
		{
			List<string> paramsList = new List<string>() { mainCode, subCode };
			return new HttpResponseMessage()
			{
				Content = new StringContent(
					_commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("Partner_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList),
					Encoding.UTF8,
					"text/html"
				)
			};
		}

        [HttpGet]
        [Route("api/Partner/SetStatusPartner/{mainCode}/{subCode}")]
		[Filters.Authorize(Roles = "Partner_M_E")]
        public void SetStatusPartner(string mainCode, string subCode)
        {
            _partnerService.SetStatusPartner(mainCode, subCode);
        }

        public IHttpActionResult Get(string mainCode, string subCode)
        {
            var customer = _partnerService.GetPartnerByMainCodeSubCode(mainCode, subCode);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [Route("api/Partner/GetSettlement")]
        public IHttpActionResult GetSettlement(string mainCode, string subCode, DateTime applyDate)
        {
            var settlement = _partnerService.GetPartnerSettlement(mainCode, subCode, applyDate);
            if (settlement == null)
            {
                return NotFound();
            }
            return Ok(settlement);
        }

		[HttpGet]
		[Route("api/Partner/GetPartnerSettlementList")]
		public PartnerInvoiceSettlementViewModel GetPartnerSettlementList(string partnerMainC, string partnerSubC)
		{
			return _partnerService.GetPartnerSettlementList(partnerMainC, partnerSubC);
		}

		[HttpGet]
		[Route("api/Partner/GetPartnerSettlementByRevenueD")]
		public PartnerSettlementViewModel GetPartnerSettlementByRevenueD(string partnerMainC, string partnerSubC, DateTime invoiceD)
		{
			return _partnerService.GetPartnerSettlementByRevenueD(partnerMainC, partnerSubC, invoiceD);
		}

		[Route("api/Partner/GetInvoices")]
		public IEnumerable<PartnerViewModel> GetInvoices()
		{
			return _partnerService.GetInvoices();
		}

		[HttpGet]
		[Route("api/Partner/GetPaymentCompanies")]
		public IEnumerable<PartnerViewModel> GetPaymentCompanies(string value)
		{
			return _partnerService.GetPaymentCompanies(value);
		}

		[HttpGet]
		[Route("api/Partner/GetByInvoiceName")]
		public PartnerViewModel GetByInvoiceName(string value)
		{
			return _partnerService.GetByInvoiceName(value);
		}
    }
}