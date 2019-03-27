using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using Website.ViewModels;
using System.Threading.Tasks;
using log4net;
using Website.ViewModels.Basic;
using Website.ViewModels.Liabilities;


namespace WebAPI.Controllers
{
	public class LiabilitiesController : ApiController
    {
		public ILiabilitiesService _liabilitiesService;

		protected static readonly ILog log = LogManager.GetLogger(typeof(LiabilitiesController));
		public LiabilitiesController() { }
		public LiabilitiesController(ILiabilitiesService liabilitiesService)
		{
			this._liabilitiesService = liabilitiesService;
		}

		[HttpGet]
		public LiabilitiesViewModel GetLiabilities(string liabilitiesI, DateTime liabilitiesD, string driverC, int liabilitiesNo)
		{
			return _liabilitiesService.GetLiabilities(liabilitiesI, liabilitiesD, driverC, liabilitiesNo);
		}

		[Filters.Authorize(Roles = "")]
		[Route("api/Liabilities/GetLiabilitiesNo")]
		public async Task<IHttpActionResult> GetLiabilitiesNo(string liabilitiesI, DateTime liabilitiesD, string driverC)
		{
			var liabilitiesNo = _liabilitiesService.GetNewLiabilitiesNo(liabilitiesI, liabilitiesD, driverC);

			return Ok(liabilitiesNo);
		}

		[Route("api/Liabilities/Datatable")]
		public async Task<IHttpActionResult> Get(
				  int page = 1,
				  int itemsPerPage = 20,
				  string sortBy = "LiabilitiesD",
				  bool reverse = false,
				  string search = null)
		{
			log.Info("test log4net");
			var custDatatable = await Task.Run(() => _liabilitiesService.GetLiabilitiesForTable(page, itemsPerPage, sortBy, reverse, search));
			if (custDatatable == null)
			{
				return NotFound();
			}
			return Ok(custDatatable);
		}

		// PUT api/<controller>/5
		[Filters.Authorize(Roles = "Liabilities_E")]
		public void Put(LiabilitiesViewModel liabilities)
		{
			_liabilitiesService.UpdateLiabilities(liabilities);
		}
		[Filters.Authorize(Roles = "Liabilities_A")]
		public void Post(LiabilitiesViewModel category)
		{
			_liabilitiesService.CreateLiabilities(category);
		}

		[HttpDelete]
		[Route("api/Liabilities/{liabilitiesI}/{liabilitiesD}/{driverC}/{liabilitiesNo}")]
		[Filters.Authorize(Roles = "Liabilities_D")]
		public void Delete(string liabilitiesI, DateTime liabilitiesD, string driverC, int liabilitiesNo)
		{
			_liabilitiesService.DeleteLiabilities(liabilitiesI, liabilitiesD, driverC, liabilitiesNo);
		}
	}
}