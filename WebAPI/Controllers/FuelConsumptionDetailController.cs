using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using System.Threading.Tasks;
using Website.ViewModels.FuelConsumption;

namespace WebAPI.Controllers
{
	public class FuelConsumptionDetailController : ApiController
	{
		public IFuelConsumptionDetailService _fuelConsumptionDetailService;

		public FuelConsumptionDetailController()
		{
		}

		public FuelConsumptionDetailController(IFuelConsumptionDetailService fuelConsumptionDetailService)
		{
			this._fuelConsumptionDetailService = fuelConsumptionDetailService;
		}

		[Route("api/FuelConsumptionDetail/Datatable")]
		public async Task<IHttpActionResult> Post(FuelConsumptionDetailSearchParams searchParams)
		{
			var fuelConsumptionDetailDatatable = await Task.Run(() => _fuelConsumptionDetailService.GetFuelConsumptionDetail(searchParams));
			if (fuelConsumptionDetailDatatable == null)
			{
				return NotFound();
			}
			return Ok(fuelConsumptionDetailDatatable);
		}

		[Route("api/FuelConsumptionDetail/GetFuelConsumptionPattern")]
		public async Task<IHttpActionResult> Post(FuelConsumptionPatternParams patternParams)
		{
			var fuelConsumptionPattern = await Task.Run(() => _fuelConsumptionDetailService.GetFuelConsumptionPattern(patternParams));
			return Ok(fuelConsumptionPattern);
		}

		[Route("api/FuelConsumptionDetail/UpdateFuelConsumptionDetail")]
		public async Task<IHttpActionResult> Post(FuelConsumptionDetailParams fuelConsumptionDetailParams)
		{
			await Task.Run(() => _fuelConsumptionDetailService.UpdateFuelConsumptionDetail(fuelConsumptionDetailParams));
			return Ok();
		}
	}
}
