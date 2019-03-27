using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Service.Services;
using Website.ViewModels;
using Website.ViewModels.CalculateDriverAllowance;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
	public class CalculateDriverAllowanceController : ApiController
	{
		public ICalculateDriverAllowanceService _calculatedriverAllowanceService;
		public CalculateDriverAllowanceController() { }

		public CalculateDriverAllowanceController(ICalculateDriverAllowanceService calculateDriverAllowanceService)
		{
			this._calculatedriverAllowanceService = calculateDriverAllowanceService;
		}

		[Route("api/CalculateDriverAllowance/Datatable")]
		public IHttpActionResult Get(
				  int page = 1,
				  int itemsPerPage = 10,
				  string sortBy = "CalculateC",
				  bool reverse = false,
				  string months = "",
				  string years = "",
				  string driverCs = "",
				  string driverNs = "",
				  string contents = "",
				  bool takeabreaks = false
				  )
		{
			var datatable = _calculatedriverAllowanceService.GetCalculateDriverAllowancesForTable(page, itemsPerPage, sortBy, reverse, months, years, driverCs, driverNs, contents, takeabreaks);
			if (datatable == null)
			{
				return NotFound();
			}
			return Ok(datatable);
		}
		[HttpGet]
		[Route("api/CalculateDriverAllowance/GetDriverAllowanceCalculate")]

		public CalculateDriverAllowanceViewModel GetDriverAllowanceCalculate(string calculateC)
		{
			return _calculatedriverAllowanceService.GetCalculateDriverAllowanceSizeByCode(calculateC);
		}

		//insert into DB
		[Filters.Authorize(Roles = "CalculateDriverAllowance_M_A")]
		public void post(CalculateDriverAllowanceViewModel pattern)
		{
			_calculatedriverAllowanceService.CreateCalculate(pattern);
		}
		////update into DB
		[Filters.Authorize(Roles = "CalculateDriverAllowance_M_E")]
		public void put(CalculateDriverAllowanceViewModel pattern)
		{
			_calculatedriverAllowanceService.UpdateCalculate(pattern);
		}
		//Delete into DB
		[HttpDelete]
		[Route("api/CalculateDriverAllowance/{id}")]
		[Filters.Authorize(Roles = "CalculateDriverAllowance_M_D")]
		public void Delete(string id)
		{
			_calculatedriverAllowanceService.DeleteCalculate(id);
		}
		[Route("api/CalculateDriverAllowance/GetCalculateDriverAllowanceByCode")]

		public List<CalculateDriverAllowanceViewModel> GetCalculateDriverAllowanceByCode(string value)
		{
			return _calculatedriverAllowanceService.GetCalculateDriverAllowanceByCode(value);
		}
		[HttpGet]
		[Route("api/CalculateDriverAllowance/GetCodeNumber")]

		public string GetCodeNumber()
		{
			return _calculatedriverAllowanceService.GetCodeNumber();
		}
	}
}