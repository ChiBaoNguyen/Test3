using System;
using System.Collections.Generic;
using System.Web.Http;
using Root.Models;
using Service.Services;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Employee;
using Website.ViewModels.Order;
using Website.ViewModels.Container;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Net;

namespace WebAPI.Controllers
{
	public class DispatchController : ApiController
	{
		public IDispatchService _dispatchService;

		public DispatchController()
		{
		}

		public DispatchController(IDispatchService dispatchService)
		{
			this._dispatchService = dispatchService;
		}

		[Route("api/Dispatch/Datatable")]
		public IHttpActionResult Get(
			string departmentC = "0",
			bool dispatchStatus0 = false,
			bool dispatchStatus1 = false,
			bool dispatchStatus2 = false,
			bool dispatchStatus3 = false,
			DateTime? fromDate = null,
			DateTime? toDate = null,
			string orderTypeI = "-1",
			string bkbl = "-1",
			string trailerNo= "-1",
			string containerNo = "-1",
			string empC = "",
			string truckC = "",
			string searchI = "O",
			string customerMainC = null,
			string customerSubC = null,
			string driverC = null,
			string jobNo = null,
			string shippingCompanyC = null,
			string sealNo = null,
			string locationC = null,
			int page = 1,
            int itemsPerPage = 8,
            bool sortBy = false
			)
		{
			var dispatchDatatable = _dispatchService.GetDispatchDatatable(departmentC, fromDate, toDate, dispatchStatus0,
				dispatchStatus1, dispatchStatus2, dispatchStatus3, orderTypeI, bkbl, trailerNo, containerNo, empC, truckC, 
				searchI, customerMainC, customerSubC, driverC, jobNo, shippingCompanyC, sealNo, locationC,
                page, itemsPerPage, sortBy);
			if (dispatchDatatable == null)
			{
				return NotFound();
			}
			return Ok(dispatchDatatable);
		}

		[HttpGet]
		[Route("api/Dispatch/GetDispatchDetail")]
		public IHttpActionResult GetDispatchDetail(
			DateTime orderDParam,
			string orderNoParam,
			int detailNoParam,
			int dispatchNoParam
			)
		{
			var dispatchDetail = _dispatchService.GetDispatchDetail(orderDParam, orderNoParam, detailNoParam, dispatchNoParam);
			return Ok(dispatchDetail);
		}

		[HttpGet]
		[Route("api/Dispatch/GetDriverDispatchList")]
		public IHttpActionResult GetDriverDispatchList(
			DateTime transportD,
			string truckC,
			string driverC
			)
		{
			var driverDispatchList = _dispatchService.GetDriverDispatchDatatable(transportD, truckC, driverC);
			if (driverDispatchList == null)
			{
				return NotFound();
			}
			return Ok(driverDispatchList);
		}

		[Filters.Authorize(Roles = "Dispatch_A, Dispatch_E")]
		public void Post(DispatchDetailViewModel dispatch)
		{
			_dispatchService.UpdateDispatchInfo(dispatch);
		}

		[HttpGet]
		[Route("api/Dispatch/UpdateContainerNo")]
		[Filters.Authorize(Roles = "Dispatch_A, Dispatch_E")]
		public IHttpActionResult UpdateContainerNo(
			DateTime orderDParam,
			string orderNoParam,
			int detailNoParam,
			string containerNoParam
			)
		{
			_dispatchService.UpdateContainerNo(orderDParam, orderNoParam, detailNoParam, containerNoParam);
			return Ok();
		}

		[HttpGet]
		[Route("api/Dispatch/UpdateTrailerNo")]
		[Filters.Authorize(Roles = "Dispatch_A, Dispatch_E")]
		public IHttpActionResult UpdateTrailerNo(
			DateTime orderDParam,
			string orderNoParam,
			int detailNoParam,
			string trailerC
			)
		{
			_dispatchService.UpdateTrailerC(orderDParam, orderNoParam, detailNoParam, trailerC);
			return Ok();
		}

		[HttpGet]
		[Route("api/Dispatch/UpdateSealNo")]
		[Filters.Authorize(Roles = "Dispatch_A, Dispatch_E")]
		public IHttpActionResult UpdateSealNo(DateTime orderDParam, string orderNoParam, int detailNoParam, string sealNoParam)
		{
			_dispatchService.UpdateSealNo(orderDParam, orderNoParam, detailNoParam, sealNoParam);
			return Ok();
		}

		[HttpGet]
		[Route("api/Dispatch/UpdateDescription")]
		[Filters.Authorize(Roles = "Dispatch_A, Dispatch_E")]
		public IHttpActionResult UpdateDescription(DateTime orderDParam, string orderNoParam, int detailNoParam, string descriptionParam)
		{
			_dispatchService.UpdateDescription(orderDParam, orderNoParam, detailNoParam, descriptionParam);
			return Ok();
		}

        [HttpGet]
        [Route("api/Dispatch/UpdateTrailerNoWarning")]
        [Filters.Authorize(Roles = "Dispatch_A, Dispatch_E")]
        public IHttpActionResult UpdateTrailerNoWarning(
            DateTime orderDParam,
            string orderNoParam,
            int detailNoParam,
            string trailerC
            )
        {
            _dispatchService.UpdateTrailerCWarning(orderDParam, orderNoParam, detailNoParam, trailerC);
            return Ok();
        }

		[HttpGet]
		[Route("api/Dispatch/CheckTrailerIsUsing")]
		public IHttpActionResult CheckTrailerIsUsing(
			DateTime orderD,
			string orderNo,
			int detailNo,
			string trailerC
			)
		{
			var status = _dispatchService.CheckTrailerIsUsing(orderD, orderNo, detailNo, trailerC);
			return Ok(status);
		}

		[HttpGet]
		[Route("api/Dispatch/UpdateActualLoadingD")]
		[Filters.Authorize(Roles = "Dispatch_A, Dispatch_E")]
		public IHttpActionResult UpdateActualLoadingD(
			DateTime orderDParam,
			string orderNoParam,
			int detailNoParam,
			DateTime? actualLoadingD
			)
		{
			_dispatchService.UpdateActualLoadingD(orderDParam, orderNoParam, detailNoParam, actualLoadingD);
			return Ok();
		}

		[HttpGet]
		[Route("api/Dispatch/UpdateActualDischargeD")]
		[Filters.Authorize(Roles = "Dispatch_A, Dispatch_E")]
		public IHttpActionResult UpdateActualDischargeD(
			DateTime orderDParam,
			string orderNoParam,
			int detailNoParam,
			DateTime? actualDischargeD
			)
		{
			_dispatchService.UpdateActualDischargeD(orderDParam, orderNoParam, detailNoParam, actualDischargeD);
			return Ok();
		}

		[HttpGet]
		[Route("api/Dispatch/UpdateActualLoadingDischargeD")]
		[Filters.Authorize(Roles = "Dispatch_A, Dispatch_E")]
		public IHttpActionResult UpdateActualLoadingDischargeD(
			DateTime orderDParam,
			string orderNoParam,
			int detailNoParam,
			DateTime? actualLoadingD,
			DateTime? actualDischargeD
			)
		{
			_dispatchService.UpdateActualLoadingDischargeD(orderDParam, orderNoParam, detailNoParam, actualLoadingD, actualDischargeD);
			return Ok();
		}

		[HttpGet]
		[Route("api/Dispatch/UpdateDispatchStatus")]
		[Filters.Authorize(Roles = "Dispatch_E")]
		public IHttpActionResult UpdateDispatchStatus(
			DateTime orderDParam,
			string orderNoParam,
			int detailNoParam,
			int dispatchNo,
			string dispatchStatus
			)
		{
			_dispatchService.UpdateDispatchStatus(orderDParam, orderNoParam, detailNoParam, dispatchNo, dispatchStatus);
			return Ok();
		}

		[HttpGet]
		[Route("api/Dispatch/DeleteDispatchDetail")]
		[Filters.Authorize(Roles = "Dispatch_D")]
		public int DeleteDispatchDetail(
			DateTime orderDParam,
			string orderNoParam,
			int detailNoParam,
			int dispatchNoParam,
			bool isConfirmedDeleting
			)
		{
			return _dispatchService.DeleteDispatchDetail(orderDParam, orderNoParam, detailNoParam, dispatchNoParam, isConfirmedDeleting);
		}

		[HttpGet]
		[Route("api/Dispatch/GetTruckList")]
		public IHttpActionResult GetTruckList(
			DateTime transportD,
			string depC,
			string searchtruckC,
			bool notdispatchgoStatus = false,
			bool notdispatchbackStatus = false,
			bool dispatchgoStatus = false,
			bool dispatchbackStatus = false,
			bool otherStatus = false
			)
		{
			var truckList = _dispatchService.GetTruckList(transportD, depC, searchtruckC, notdispatchgoStatus, notdispatchbackStatus, dispatchgoStatus, dispatchbackStatus, otherStatus);
			return Ok(truckList);
		}

		[HttpGet]
		[Route("api/Dispatch/GetTrailerList")]
		public IHttpActionResult GetTrailerList(
			DateTime transportD,
			string depC,
			string searchtrailerC,
			bool notdispatchgoStatus = false,
			bool notdispatchbackStatus = false,
			bool dispatchgoStatus = false,
			bool dispatchbackStatus = false,
			bool otherStatus = false
			)
		{
			var truckList = _dispatchService.GetTrailerList(transportD, depC, searchtrailerC, notdispatchgoStatus, notdispatchbackStatus, dispatchgoStatus, dispatchbackStatus, otherStatus);
			return Ok(truckList);
		}

		[HttpGet]
		[Route("api/Dispatch/CheckDispatchDeleting")]
		public int CheckDispatchDeleting(DateTime orderD, string orderNo, int detailNo, int dispatchNo)
		{
			return _dispatchService.CheckDispatchDeleting(orderD, orderNo, detailNo, dispatchNo);
		}
        [HttpGet]
        [Route("api/Dispatch/GetOrderNoDoubleMax")]
        public int? GetOrderNoDoubleMax()
        {
            return _dispatchService.GetOrderNoDoubleMax();
        }

        [HttpGet]
        [Route("api/Dispatch/SaveOrderNoDouble")]
        [Filters.Authorize(Roles = "Dispatch_A, Dispatch_E")]
        public bool SaveOrderNoDouble(
            int? detailNo1,
            int? detailNo2,
            DateTime? orderD1,
            DateTime? orderD2,
            string orderNo1,
            string orderNo2,
            int? orderNoDouble
            )
        {
            return _dispatchService.SaveOrderNoDouble(orderNoDouble, orderD1, orderNo1, detailNo1, orderD2, orderNo2, detailNo2);
        }

        [HttpGet]
        [Route("api/Dispatch/GetStatusCont")]
        //[Filters.Authorize(Roles = "Dispatch_A, Dispatch_E")]
        public bool GetStatusCont(
            int? detailNo,
            DateTime? orderD,
            string orderNo
            )
        {
            return _dispatchService.GetStatusCont(detailNo, orderD, orderNo);
        }
	}
}