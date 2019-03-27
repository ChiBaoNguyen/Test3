using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.TransportDistance;
using Website.ViewModels.Dispatch;

namespace WebAPI.Controllers
{
	public class TransportDistanceController : ApiController
	{
		public ITransportDistanceService _transportdistanceService;

		public TransportDistanceController()
		{
		}

		public TransportDistanceController(ITransportDistanceService transportdistanceService)
		{
			this._transportdistanceService = transportdistanceService;
		}


		[HttpGet]
		[Route("api/TransportDistance/GetTransportDistance")]

		public TransportDistanceViewModel GetTransportDistance(string transportdistanceC)
		{
			return _transportdistanceService.GetTransportDistanceSizeByCode(transportdistanceC);
		}

		[Route("api/TransportDistance/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "TransportDistanceC",
				  bool reverse = false,
				  string departure = null,
				  string destination = null)
		{
			var datatable = _transportdistanceService.GetTransportDistanceForTable(page, itemsPerPage, sortBy, reverse, departure, destination);
			if (datatable == null)
			{
				return NotFound();
			}
			return Ok(datatable);
		}

		// POST api/<controller>
		[Filters.Authorize(Roles = "TransportDistance_M_A")]
		public void Post(TransportDistanceViewModel transport)
		{
			_transportdistanceService.CreateTransportDistance(transport);
		}

		[Filters.Authorize(Roles = "TransportDistance_M_E")]
		public void put(TransportDistanceViewModel transport)
		{
			_transportdistanceService.UpdateTransportDistance(transport);
		}

		[HttpDelete]
		[Route("api/TransportDistance/{id}")]
		[Filters.Authorize(Roles = "TransportDistance_M_D")]
		public void Delete(string id)
		{
			_transportdistanceService.DeleteTransportDistance(id);
		}
		[Route("api/TransportDistance/GetTransportDistanceByCode")]

		public List<TransportDistanceViewModel> GetTransportDistanceByCode(string value)
		{
			return _transportdistanceService.GetTransportDistanceByCode(value);
		}

		[HttpGet]
		[Route("api/TransportDistance/GetInfoFromTransportDistance")]

		public Result GetInfoFromTransportDistance(DateTime orderD, string orderNo, int detailNo,
			string location1C, string location2C, string operation1C, string operation2C, DateTime transportD, string waytype)
		{
			return _transportdistanceService.GetInfoFromTransportDistance(orderD, orderNo, detailNo, location1C, location2C, operation1C, operation2C, transportD, waytype);
		}
	}
}
