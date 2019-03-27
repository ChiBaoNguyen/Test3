using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Service.Services;
using Website.ViewModels.Container;
using System.Web;
using System.IO;
using System.Net.Http.Headers;

namespace WebAPI.Controllers
{
	public class ContainerSizeController : ApiController
	{
		public IContainerSizeService ContainerSizeService;

		public ContainerSizeController() { }
		public ContainerSizeController(IContainerSizeService containerSizeService)
		{
			this.ContainerSizeService = containerSizeService;
		}

		// GET api/customer
		public async Task<IEnumerable<ContainerSizeViewModel>> Get()
		{
			return await Task.Run(() =>  ContainerSizeService.GetContainerSizes());
		}

		public IEnumerable<ContainerSizeViewModel> Get(string value)
		{
			return ContainerSizeService.GetContainerSizesByCode(value);
		}

		[System.Web.Http.HttpGet]
		[System.Web.Http.Route("api/ContainerSize/GetContainerTypeByCode")]
		public IHttpActionResult GetContainerTypeByCode(string containerSizeCode)
        {
			var ContainerSize = ContainerSizeService.GetContainerSizeByCode(containerSizeCode);
            if (ContainerSize == null)
            {
                return NotFound();
            }
            return Ok(ContainerSize);
        }

        [System.Web.Http.Route("api/ContainerSize/Datatable")]
        public IHttpActionResult Get(
                  int page = 1,
                  int itemsPerPage = 10,
                  string sortBy = "ContainerSizeC",
                  bool reverse = false,
                  string search = null)
        {
            var custDatatable = ContainerSizeService.GetContainerSizesForTable(page, itemsPerPage, sortBy, reverse, search);
            if (custDatatable == null)
            {
                return NotFound();
            }
            return Ok(custDatatable);
        }

		// POST api/<controller>
		public void Post(ContainerSizeViewModel containerSize)
		{
			ContainerSizeService.CreateContainerSize(containerSize);
		}

		// PUT api/<controller>/5
		public void Put(ContainerSizeViewModel containerSize)
		{
			ContainerSizeService.UpdateContainerSize(containerSize);
		}

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/ContainerSize/SetStatusContainerSize/{id}/{name}")]
        public void SetStatusCustomer(string id, string name)
        {
            ContainerSizeService.SetStatusContainerSize(id);
        }

	    // DELETE api/customer/5
		public void Delete(string id)
		{
			ContainerSizeService.DeleteContainerSize(id);
		}

        [System.Web.Http.HttpGet]
		[System.Web.Http.Route("api/ContainerSize/GetReport/{id}")]
        public HttpResponseMessage GetReport(string id)
        {
			Stream stream = ContainerSizeService.GetReportContainerSizeList(id);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            return result;
        }

		//[System.Web.Http.HttpGet]
		//[System.Web.Http.Route("api/ContainerSize/GetExcel")]
		//public HttpResponseMessage GetExcel()
		//{
		//	Stream stream;
		//	HttpResponseMessage result;

		//	stream = ContainerSizeService.GetReportCustomerList(1);

		//	result = new HttpResponseMessage(HttpStatusCode.OK);
		//	result.Content = new StreamContent(stream);
		//	result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
		//	result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
		//	{
		//		FileName = "Danhsachcontainer.xls"
		//	};

		//	return result;
		//}

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/ContainerSize/ExportExcelContainerSizeList/{id}")]
        public HttpResponseMessage ExportExcelContainerSizeList(string id)
        {
            Stream stream = ContainerSizeService.GetContainSizeListForExcel(id);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "ContainerSizeList.xlsx"
            };

            return result;
        }
	}
}
