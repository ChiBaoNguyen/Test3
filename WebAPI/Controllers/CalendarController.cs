using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Root.Models.Calendar;
using Service.Services;

namespace WebAPI.Controllers
{
    public class CalendarController : ApiController
    {
	    public ICalendarService _calendarService;
		public CalendarController() { }
		public CalendarController(ICalendarService calendarService)
		{
			this._calendarService = calendarService;
		}

		[Filters.Authorize(Roles = "Calendar")]
		public List<CalendarPlanItem> Get(DateTime date, string depC, bool donePlan, bool undonePlan, string objectI, string objectC = null, DateTime? fromDate = null, DateTime? toDate = null, string contentC = null)
		{
			return _calendarService.GetCalendarPlan(date, depC, donePlan, undonePlan, objectI, objectC, fromDate, toDate, contentC);
		}

		[Filters.Authorize(Roles = "Calendar")]
		[Route("api/Calendar/GetCalendarPlanForCounting")]
		public List<CalendarPlanItemForCounting> Get(string year, string depC)
		{
			return _calendarService.GetCalendarPlanForCounting(year, depC);
		}

    }
}
