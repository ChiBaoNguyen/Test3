using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.ContractPartnerPattern;
using System.Threading.Tasks;
using log4net;

namespace WebAPI.Controllers
{
	public class ContractPartnerPatternController : ApiController
	{
		public IContractPartnerPatternService _contractPartnerPatternService;
		protected static readonly ILog log = LogManager.GetLogger(typeof(OrderPatternController));

		public ContractPartnerPatternController() { }

		public ContractPartnerPatternController(IContractPartnerPatternService contractPartnerPatternService)
		{
			this._contractPartnerPatternService = contractPartnerPatternService;
		}

		public List<ContractPartnerPatternViewModel> Get(string customerMainC, string customerSubC, string depatureC, string destinationC, DateTime date)
		{
			return _contractPartnerPatternService.GetContractPartnerPatterns(customerMainC, customerSubC, depatureC, destinationC, date);
		}

		[HttpGet]
		[Route("api/ContractPartnerPattern/GetPartnerPattern")]
		public ContractPartnerViewModel GetPartnerPattern(string customerMainC, string customerSubC, DateTime applyD)
		{
			return _contractPartnerPatternService.GetContractPartnerPattern(customerMainC, customerSubC, applyD);
		}
		[Route("api/ContractPartnerPattern/Datatable")]
		public async Task<IHttpActionResult> Get(
					int page = 1,
					int itemsPerPage = 10,
					string sortBy = "CustomerMainC",
					bool reverse = false,
					string custMainC = null,
					string custSubC = null,
					string departure = null,
					string destination = null,
					string contsizeI = null)
		{
			log.Info("test log4net");
			var datatable = await Task.Run(() => _contractPartnerPatternService.GetContractPartnerPatternsForTable(page, itemsPerPage, sortBy, reverse, custMainC, custSubC, departure, destination, contsizeI));
			if (datatable == null)
			{
				return NotFound();
			}
			return Ok(datatable);
		}

		[HttpDelete]
		[Route("api/ContractPartnerPattern/{mainCode}/{subCode}/{applyD}")]
		[Filters.Authorize(Roles = "ContractPartnerPattern_M_D")]
		public void Delete(string mainCode, string subCode, DateTime applyD)
		{
			_contractPartnerPatternService.DeletePattern(mainCode, subCode, applyD);
		}

		[HttpDelete]
		[Route("api/ContractPartnerPattern/{mainCode}/{subCode}/{dep}/{des}/{consize}/{applyD}/{custMainC}/{custSubC}")]
		[Filters.Authorize(Roles = "ContractPartnerPattern_M_D")]
		public void Delete(string mainCode, string subCode, string dep, string des, string consize, DateTime applyD, string custMainC, string custSubC)
		{
			_contractPartnerPatternService.DeleteDetailPattern(mainCode, subCode, dep, des, consize, applyD, custMainC, custSubC);
		}

		[Filters.Authorize(Roles = "ContractPartnerPattern_M_A")]
		public void Post(ContractPartnerViewModel pattern)
		{
			_contractPartnerPatternService.CreatePattern(pattern);
		}
		[Filters.Authorize(Roles = "ContractPartnerPattern_M_E")]
		public void Put(ContractPartnerViewModel pattern)
		{
			_contractPartnerPatternService.UpdatePattern(pattern);
		}

		[HttpGet]
		[Route("api/ContractPartnerPattern/GetUnitPriceFromContractPartner")]
		public decimal GetUnitPriceFromContractPartner(DateTime transportD, string custMainC, string custSubC, string partMainC, string partSubC, string ordertypeI, string loca1, string loca2, string contsizeI, string calTon)
		{
			return _contractPartnerPatternService.GetUnitPriceFromContractPartner(transportD, custMainC, custSubC, partMainC, partSubC, ordertypeI, loca1, loca2, contsizeI, calTon);
		}
	}
}
