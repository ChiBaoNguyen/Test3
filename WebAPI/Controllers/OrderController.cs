using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.Common;
using Website.ViewModels.Container;
using Website.ViewModels.Order;
using System.IO;
using System.Net.Http.Headers;

namespace WebAPI.Controllers
{
	public class OrderController : ApiController
	{
		public IOrderService _orderService;

		public OrderController() { }

		public OrderController(IOrderService orderService)
		{
			this._orderService = orderService;
		}

		public IEnumerable<OrderViewModel> Get()
		{
			return _orderService.GetOrders();
		}

		public OrderViewModel Get(string no, DateTime date)
		{
			return _orderService.GetOrder(no, date);
		}
        [Route("api/Order/GetLocation")]
        public OrderViewModel GetLocation(string no, DateTime date)
        {
            return _orderService.GetLocationInOrder(no, date);
        }
		[Filters.Authorize(Roles = "Order, TransportConfirm")]
		[Route("api/Order/GetOrderNumberId")]
		public async Task<IHttpActionResult> GetOrderEntryId(DateTime date)
		{
			var orderIdInfo = await Task.Run(() => _orderService.GetOrderEntryId(date));

			return Ok(orderIdInfo);
		}

		[Filters.Authorize(Roles = "Order")]
		[Route("api/Order/Datatable")]
		public async Task<IHttpActionResult> Get(
					int page = 1,
					int itemsPerPage = 10,
					string sortBy = "OrderD",
					bool reverse = false,
					string search = null,
					string orderTypeI = null,
					DateTime ? fromDate = null,
					DateTime? toDate = null, 
					string custMainC = "", 
					string custSubC = "")
		{
			var orderDatatable = await Task.Run(() => _orderService.GetOrdersForTable(page, itemsPerPage, sortBy, reverse, search, orderTypeI, fromDate, toDate, custMainC, custSubC));
			if (orderDatatable == null)
			{
				return NotFound();
			}
			return Ok(orderDatatable);
		}

		[Filters.Authorize(Roles = "Order_A")]
		public ResponseStatus Post(OrderViewModel order)
		{
			return _orderService.CreateOrder(order);
		}

		[Filters.Authorize(Roles = "Order_E")]
		public void Put(OrderViewModel order)
		{
			_orderService.UpdateOrder(order);
		}

		[Filters.Authorize(Roles = "Order_D")]
		public int Delete(DateTime orderD, string orderNo, bool isConfirmedDeleting)
		{
			return _orderService.DeleteOrder(orderD, orderNo, isConfirmedDeleting);
		}

		//[System.Web.Http.HttpGet]
		//[System.Web.Http.Route("api/Order/ExportPdf/{depC}/{orderTypeI}/{customer}/{laguague}" +
		//												"/{orderDFrom}/{orderDTo}"
		//					  )]
		//public HttpResponseMessage ExportPdf(string depC, string orderTypeI, string customer, string laguague,
		//										DateTime? orderDFrom, DateTime? orderDTo
		//										)
		//{
		//	OrderReportParam param = new OrderReportParam();
		//	param.DepC = depC;
		//	param.OrderTypeI = orderTypeI;
		//	param.Customer = customer;
		//	param.Laguague = laguague;
		//	param.OrderDFrom = orderDFrom;
		//	param.OrderDTo = orderDTo;

		//	Stream stream = _orderService.ExportPdf(param);

		//	HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
		//	result.Content = new StreamContent(stream);
		//	result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

		//	return result;
		//}

		//[HttpGet]
		//[Route("api/Order/GetOrderDateMax")]
		//public DateTime? GetMaxOrderD()
		//{
		//	return _orderService.GetOrderDateMax();
		//}

		[HttpGet]
		[Route("api/Order/GetRevenueDateMax/{customerMainC}/{customerSubC}")]
		public DateTime? GetRevenueDateMax(string customerMainC, string customerSubC)
		{
			return _orderService.GetRevenueDateMax(customerMainC, customerSubC);
		}

		[HttpGet]
		[Route("api/Order/GetPartnerInvoiceDateMax/{partnerMainC}/{partnerSubC}")]
		public DateTime? GetPartnerInvoiceDateMax(string partnerMainC, string partnerSubC)
		{
			return _orderService.GetPartnerInvoiceDateMax(partnerMainC, partnerSubC);
		}
	}
}
