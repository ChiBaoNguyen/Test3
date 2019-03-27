using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.OrderPattern;
using System.Threading.Tasks;
using log4net;

namespace WebAPI.Controllers
{
    public class ContractTariffPatternController : ApiController
    {
        public IContractTariffPatternService _contractTariffPatternService;
        protected static readonly ILog log = LogManager.GetLogger(typeof(OrderPatternController));

		public ContractTariffPatternController() { }

		public ContractTariffPatternController(IContractTariffPatternService contractTariffPatternService)
		{
			this._contractTariffPatternService = contractTariffPatternService;
		}

		public List<ContractTariffPatternViewModel> Get(string customerMainC, string customerSubC, string depatureC, string destinationC, DateTime date)
		{
			return _contractTariffPatternService.GetContractTariffPatterns(customerMainC, customerSubC, depatureC, destinationC, date);
		}

		[HttpGet]
		[Route("api/ContractTariffPattern/GetTariffPattern")]
        public ContractTariffViewModel GetTariffPattern(string customerMainC, string customerSubC, DateTime applyD)
        {
            return _contractTariffPatternService.GetContractTariffPattern(customerMainC, customerSubC, applyD);
        }
        [Route("api/ContractTariffPattern/Datatable")]
        public async Task<IHttpActionResult> Get(
					int page = 1,
					int itemsPerPage = 10,
					string sortBy = "CustomerMainC",
					bool reverse = false,
					string custMainC = null,
					string custSubC = null,
					string departure = null,
					string destination = null,
					string commo = null,
					string contsizeI = null)
        {
            log.Info("test log4net");
			var datatable = await Task.Run(() => _contractTariffPatternService.GetContractTariffPatternsForTable(page, itemsPerPage, sortBy, reverse, custMainC, custSubC, departure, destination, commo, contsizeI));
            if (datatable == null)
            {
                return NotFound();
            }
            return Ok(datatable);
        }
        
        [HttpDelete]
        [Route("api/ContractTariffPattern/{mainC}/{subC}/{applyD}")]
		[Filters.Authorize(Roles = "ContractTariffPattern_M_D")]
        public void Delete(string mainC, string subC, DateTime applyD)
        {
            _contractTariffPatternService.DeletePattern(mainC, subC, applyD);
        }

		[HttpDelete]
		[Route("api/ContractTariffPattern/{mainC}/{subC}/{dep}/{des}/{consize}/{applyD}")]
		[Filters.Authorize(Roles = "ContractTariffPattern_M_D")]
		public void Delete(string mainC, string subC, string dep, string des, string consize, DateTime applyD)
		{
			_contractTariffPatternService.DeleteDetailPattern(mainC, subC, dep, des, consize, applyD);
		}

		[Filters.Authorize(Roles = "ContractTariffPattern_M_A")]
        public void Post(ContractTariffViewModel pattern)
        {
            _contractTariffPatternService.CreatePattern(pattern);
        }
		[Filters.Authorize(Roles = "ContractTariffPattern_M_E")]
        public void Put(ContractTariffViewModel pattern)
        {
            _contractTariffPatternService.UpdatePattern(pattern);
        }

        //[Route("api/ContractTariffPattern/DefaultTariff")]
        //public async Task<IHttpActionResult> Get()
        //{
        //    log.Info("test log4net");
        //    var datatable = await Task.Run(() => _contractTariffPatternService.GetTariffPattern());
        //    if (datatable == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(datatable);
        //}
		[HttpGet]
		[Route("api/ContractTariffPattern/GetUnitPriceFromContractTariff")]

		public decimal GetUnitPriceFromContractTariff(DateTime orderD, string custMainC, string custSubC, string ordertypeI, string loca1, string loca2, string contsizeI, string calTon, string comC)
		{
			return _contractTariffPatternService.GetUnitPriceFromContractTariff(orderD, custMainC, custSubC, ordertypeI, loca1, loca2, contsizeI, calTon, comC);
		}
    }
}
