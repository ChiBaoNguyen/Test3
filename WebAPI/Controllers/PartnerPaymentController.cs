using System;
using System.Web.Http;
using Service.Services;
using System.Threading.Tasks;
using log4net;
using Website.ViewModels.PartnerPayment;

namespace WebAPI.Controllers
{
	public class PartnerPaymentController : ApiController
    {
		public IPartnerPaymentService _partnerPaymentService;

		protected static readonly ILog log = LogManager.GetLogger(typeof(PartnerPaymentController));
		public PartnerPaymentController() { }
		public PartnerPaymentController(IPartnerPaymentService partnerPaymentService)
		{
			this._partnerPaymentService = partnerPaymentService;
		}

		[HttpGet]
		public PartnerPaymentViewModel GetPartnerPayment(string partnerMainC, string partnerSubC, DateTime partnerPaymentD, string paymentId)
		{
			return _partnerPaymentService.GetPartnerPayment(partnerMainC, partnerSubC, partnerPaymentD, paymentId);
		}

		[HttpGet]
		[Route("api/PartnerPayment/GetSupplierPayment")]
		public PartnerPaymentViewModel GetSupplierPayment(string supplierMainC, string supplierSubC, DateTime supplierPaymentD, string paymentId)
		{
			return _partnerPaymentService.GetSupplierPayment(supplierMainC, supplierSubC, supplierPaymentD, paymentId);
		}

		[Route("api/PartnerPayment/Datatable")]
		public async Task<IHttpActionResult> Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "PartnerPaymentD",
				  bool reverse = true,
				  string search = null)
		{
			log.Info("test log4net");
			var custDatatable = await Task.Run(() => _partnerPaymentService.GetPartnerPaymentForTable(page, itemsPerPage, sortBy, reverse, search));
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		// PUT api/<controller>/5
		[Filters.Authorize(Roles = "PartnerPayment_E")]
		public void Put(PartnerPaymentViewModel partnerPayment)
		{
			_partnerPaymentService.UpdatePartnerPayment(partnerPayment);
		}
		[Filters.Authorize(Roles = "PartnerPayment_A")]
		public void Post(PartnerPaymentViewModel category)
		{
			_partnerPaymentService.CreatePartnerPayment(category);
		}

		[HttpDelete]
		[Route("api/PartnerPayment/{partnerMainC}/{partnerSubC}/{partnerPaymentD}/{paymentId}")]
		[Filters.Authorize(Roles = "PartnerPayment_D")]
		public void Delete(string partnerMainC, string partnerSubC, DateTime partnerPaymentD, string paymentId)
		{
			_partnerPaymentService.DeletePartnerPayment(partnerMainC, partnerSubC, partnerPaymentD, paymentId);
		}

		[HttpDelete]
		[Route("api/PartnerPayment/DeleteSupplier/{supplierMainC}/{supplierSubC}/{supplierPaymentD}/{paymentId}")]
		[Filters.Authorize(Roles = "PartnerPayment_D")]
		public void DeleteSupplier(string supplierMainC, string supplierSubC, DateTime supplierPaymentD, string paymentId)
		{
			_partnerPaymentService.DeleteSupplierPayment(supplierMainC, supplierSubC, supplierPaymentD, paymentId);
		}
	}
}