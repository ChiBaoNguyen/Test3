using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.Container;
using Website.ViewModels.Order;

namespace WebAPI.Controllers
{
	public class ContainerController : ApiController
	{
		public IContainerService _containerService;

		public ContainerController() { }

		public ContainerController(IContainerService containerService)
		{
			this._containerService = containerService;
		}

		//public IEnumerable<ContainerViewModel> Get()
		//{
		//	return _orderService.GetContainers();
		//}
		public TransportConfirmViewModel Get(DateTime date, string orderNo, int detailNo)
		{
			return _containerService.GetTransportConfirm(date, orderNo, detailNo);
		}

		[Route("api/Container/GetOrderDetailNo")]
		public async Task<IHttpActionResult> GetOrderDetailNo(DateTime date, string orderNo)
		{
			var orderIdInfo = await Task.Run(() => _containerService.GetOrderDetailNo(date, orderNo));

			return Ok(orderIdInfo);
		}

		[Route("api/Container/Datatable")]
		public async Task<IHttpActionResult> Post(ContainerSearchParams containerSearchParamsparams)
		{
			var containerDatatable = await Task.Run(() => _containerService.GetContainersForTable(containerSearchParamsparams));
			if (containerDatatable == null)
			{
				return NotFound();
			}
			return Ok(containerDatatable);
		}

		[HttpPost]
		[Route("api/Container/CreateConfirmationOrder")]
		[Filters.Authorize(Roles = "TransportConfirm_A")]
		public void CreateConfirmtionOrder(TransportConfirmViewModel order)
		{
			_containerService.CreateConfirmationOrder(order);
		}

		[HttpPost]
		[Route("api/Container/UpdateConfirmationOrder")]
		[Filters.Authorize(Roles = "TransportConfirm_E")]
		public void UpdateConfirmtionOrder(TransportConfirmViewModel order)
		{
			_containerService.UpdateConfirmationOrder(order);
		}

		public int Delete(DateTime orderD, string orderNo, int detailNo, bool isConfirmedDeleting)
		{
			return _containerService.DeleteContainer(orderD, orderNo, detailNo, isConfirmedDeleting);
		}

		[HttpGet]
		[Route("api/Container/CheckContainerDeleting")]
		[Filters.Authorize(Roles = "TransportConfirm_D")]
		public int CheckContainerDeleting(DateTime orderD, string orderNo, int detailNo)
		{
			return _containerService.CheckContainerDeleting(orderD, orderNo, detailNo);
		}

		[HttpGet]
		[Route("api/Container/GetDetainDay")]
		public int? GetDetainDay(DateTime orderD, string orderNo, int detailNo, string containerNo)
		{
			return _containerService.GetDetainDay(orderD, orderNo, detailNo, containerNo);
		}
	}
}
