using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using Website.ViewModels;
using Website.ViewModels.DriverAllowance;
using System.Threading.Tasks;
using log4net;

namespace WebAPI.Controllers
{
    public class DriverAllowanceController : ApiController
    {
		public IDriverAllowanceService _driverAllowanceService;

		protected static readonly ILog log = LogManager.GetLogger(typeof(DriverAllowanceController));
		public DriverAllowanceController() { }
		public DriverAllowanceController(IDriverAllowanceService driverAllowanceService)
		{
			this._driverAllowanceService = driverAllowanceService;
		}
		public List<DriverAllowanceViewModel> Get(string customerMainC, string customerSubC)
		{
			return _driverAllowanceService.GetDriverAllowances(customerMainC, customerSubC);
		}
		[HttpGet]
		[Route("api/DriverAllowance/GetDriverAllowancePattern")]
		public DriverAllowancePatternViewModel GetDriverAllowancePattern(string customerMainC, string customerSubC, DateTime applyD)
		{
			return _driverAllowanceService.GetDriverAllowance(customerMainC, customerSubC, applyD);
		}
		[Route("api/DriverAllowance/Datatable")]
		public async Task<IHttpActionResult> Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "CustomerMainC",
				  bool reverse = false,
				  string search = null)
		{
			log.Info("test log4net");
			var datatable = await Task.Run(() => _driverAllowanceService.GetDriverAllowancesForTable(page, itemsPerPage, sortBy, reverse, search));
			if (datatable == null)
			{
				return NotFound();
			}
			return Ok(datatable);
		}
		//insert into DB
		[Filters.Authorize(Roles = "DriverAllowance_M_A")]
		public void post(DriverAllowancePatternViewModel pattern)
		{
			_driverAllowanceService.CreatePattern(pattern);
		}
		//update into DB
		[Filters.Authorize(Roles = "DriverAllowance_M_E")]
		public void put(DriverAllowancePatternViewModel pattern)
		{
			_driverAllowanceService.UpdatePattern(pattern);
		}
		//Delete into DB
		[HttpDelete]
		[Route("api/DriverAllowance/{mainC}/{subC}/{applyD}")]
		[Filters.Authorize(Roles = "DriverAllowance_M_D")]
		public void Delete(string mainC, string subC, DateTime applyD)
		{
			_driverAllowanceService.DeletePattern(mainC, subC, applyD);
		}

		[Route("api/DriverAllowance/GetUnitPriceRate")]

		public decimal? GetUnitPriceRate(DateTime date)
		{
			return _driverAllowanceService.GetUnitPriceRate(date);
		}

		[Route("api/DriverAllowance/GetLatestDriverAllowance")]

		public decimal? GetLatestDriverAllowance(DateTime date)
		{
			return _driverAllowanceService.GetLatestDriverAllowance(date);
		}

		[Route("api/DriverAllowance/GetUnitPriceMethodI")]

		public int GetUnitPriceMethodI(DateTime date)
		{
			return _driverAllowanceService.GetUnitPriceMethodI(date);
		}
    }
}
