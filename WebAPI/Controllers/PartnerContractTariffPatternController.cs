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
    public class PartnerContractTariffPatternController : ApiController
    {
        public IPartnerContractTariffPatternService _contractTariffPatternService;
        protected static readonly ILog log = LogManager.GetLogger(typeof(OrderPatternController));

		public PartnerContractTariffPatternController() { }

		public PartnerContractTariffPatternController(IPartnerContractTariffPatternService contractTariffPatternService)
		{
			this._contractTariffPatternService = contractTariffPatternService;
		}

        public List<PartnerContractTariffPatternViewModel> Get(string partnerMainC, string partnerSubC, string depatureC, string destinationC)
		{
			return _contractTariffPatternService.GetPartnerContractTariffPatterns(partnerMainC, partnerSubC, depatureC, destinationC);
		}
        public PartnerContractTariffViewModel GetTariffPattern(string partnerMainC, string partnerSubC, DateTime applyD)
        {
            return _contractTariffPatternService.GetPartnerContractTariffPattern(partnerMainC, partnerSubC, applyD);
        }

        [Route("api/PartnerContractTariffPattern/Datatable")]
        public async Task<IHttpActionResult> Get(
                  int page = 1,
                  int itemsPerPage = 10,
                  string sortBy = "PartnerMainC",
                  bool reverse = false,
                  string search = null)
        {
            log.Info("test log4net");
            var datatable = await Task.Run(() => _contractTariffPatternService.GetPartnerContractTariffPatternsForTable(page, itemsPerPage, sortBy, reverse, search));
            if (datatable == null)
            {
                return NotFound();
            }
            return Ok(datatable);
        }
        
        [HttpDelete]
        [Route("api/PartnerContractTariffPattern/{mainC}/{subC}/{applyD}")]
        public void Delete(string mainC, string subC, DateTime applyD)
        {
            _contractTariffPatternService.DeletePattern(mainC, subC, applyD);
        }

        public void Post(PartnerContractTariffViewModel pattern)
        {
            _contractTariffPatternService.CreatePattern(pattern);
        }

        public void Put(PartnerContractTariffViewModel pattern)
        {
            _contractTariffPatternService.UpdatePattern(pattern);
        }

    }
}
