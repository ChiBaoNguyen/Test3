using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.FuelConsumption;

namespace WebAPI.Controllers
{
	public class FuelConsumptionController : ApiController
	{
		public IFuelConsumptionService _fuelConsumptionService;

		public FuelConsumptionController()
		{
		}

		public FuelConsumptionController(IFuelConsumptionService fuelConsumptionService)
		{
			this._fuelConsumptionService = fuelConsumptionService;
		}


		[HttpGet]
		[Route("api/FuelConsumption/GetFuelConsumption")]
		public FuelConsumptionViewModel GetFuelConsumption(string fuelconsumptionC)
		{
			return _fuelConsumptionService.GetFuelConsumptionSizeByCode(fuelconsumptionC);
		}

		[Route("api/FuelConsumption/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "FuelConsumptionC",
				  bool reverse = false,
				  string search = null)
		{
			var datatable = _fuelConsumptionService.GetFuelConsumptionForTable(page, itemsPerPage, sortBy, reverse, search);
			if (datatable == null)
			{
				return NotFound();
			}
			return Ok(datatable);
		}

		// POST api/<controller>
		[Filters.Authorize(Roles = "FuelConsumption_M_A")]
		public void Post(FuelConsumptionViewModel fuelConsumption)
		{
			_fuelConsumptionService.CreateFuelConsumption(fuelConsumption);
		}

		[Filters.Authorize(Roles = "FuelConsumption_M_E")]
		public void put(FuelConsumptionViewModel fuelConsumption)
		{
			_fuelConsumptionService.UpdateFuelConsumption(fuelConsumption);
		}

		[HttpDelete]
		[Route("api/FuelConsumption/{id}")]
		[Filters.Authorize(Roles = "FuelConsumption_M_D")]
		public void Delete(string id)
		{
			_fuelConsumptionService.DeleteFuelConsumption(id);
		}
		[Route("api/FuelConsumption/GetFuelConsumptionByCode")]

		public List<FuelConsumptionViewModel> GetFuelConsumptionByCode(string value)
		{
			return _fuelConsumptionService.GetFuelConsumptionByCode(value);
		}

		[HttpGet]
		[Route("api/FuelConsumption/CheckExitContSize")]
		public int CheckExitContSize(string contSize)
		{
			return _fuelConsumptionService.CheckExitContSize(contSize);
		}

		[Route("api/FuelConsumption/GetInfoFuel")]
		public InfoFuel GetInfoFuel(string truckC, string containersize, string waytype)
		{
			return _fuelConsumptionService.GetInfoFuel(truckC, containersize, waytype);
		}
	}
}
