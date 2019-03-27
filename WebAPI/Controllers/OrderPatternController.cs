using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.Location;
using Website.ViewModels.OrderPattern;
using log4net;
using System.Net.Http;
using Website.Utilities;
using System.Text;

namespace WebAPI.Controllers
{
	public class OrderPatternController : ApiController
	{
        public IOrderPatternService _orderPatternService;
		public ICommonService _commonService;
        protected static readonly ILog log = LogManager.GetLogger(typeof(OrderPatternController));

		public OrderPatternController() { }

		public OrderPatternController(IOrderPatternService orderPatternService, ICommonService commonService)
		{
			this._orderPatternService = orderPatternService;
			_commonService = commonService;
		}

		public IEnumerable<OrderPatternViewModel> Get(string custMainCode, string custSubCode)
		{
			return _orderPatternService.GetPatterns(custMainCode, custSubCode);
		}

		[Route("api/OrderPattern/GetPatternsByCode")]
		public IEnumerable<OrderPatternViewModel> GetPatternsByCode(string custMainCode, string custSubCode, string value)
		{
			return _orderPatternService.GetPatternsByCode(custMainCode, custSubCode, value);
		}

		[HttpGet]
		public OrderPatternViewModel GetOrderPattern(string custMainCode, string custSubCode, string orderPatternC, DateTime? date)
		{
			return _orderPatternService.GetOrderPattern(custMainCode, custSubCode, orderPatternC, date);
		}

        [Route("api/OrderPattern/Datatable")]
        public async Task<IHttpActionResult> Get(
                  int page = 1,
                  int itemsPerPage = 10,
                  string sortBy = "CustomerMainC",
                  bool reverse = false,
                  string search = null)
        {
            log.Info("test log4net");
            var custDatatable = await Task.Run(() => _orderPatternService.GetOrderPatternsForTable(page, itemsPerPage, sortBy, reverse, search));
            if (custDatatable == null)
            {
                return NotFound();
            }
            return Ok(custDatatable);
        }

		[HttpDelete]
		[Route("api/OrderPattern/{mainC}/{subC}/{patternC}")]
		[Filters.Authorize(Roles = "OrderPattern_M_D")]
		public void Delete(string mainC, string subC, string patternC)
		{
			_orderPatternService.DeletePattern(mainC, subC, patternC);
		}

		[HttpGet]
		[Route("api/OrderPattern/CheckWhenDelete/{language}/{patternC}")]
		[Filters.Authorize(Roles = "OrderPattern_M_D")]
		public HttpResponseMessage CheckWhenDelete(string language, string patternC)
		{
			List<string> paramsList = new List<string>() { patternC };
			string result = _commonService.CheckWhenDelete(language, Utilities.CreateCheckDeleteQuery("OrderPattern_M", System.Web.Hosting.HostingEnvironment.MapPath("~/DelConfig.xml")), paramsList);
			return new HttpResponseMessage()
			{
				Content = new StringContent(
					result,
					Encoding.UTF8,
					"text/html"
				)
			};
		}
		[Filters.Authorize(Roles = "OrderPattern_M_A")]
        public void Post(OrderPatternViewModel pattern)
        {
            _orderPatternService.CreatePattern(pattern);
        }

        // PUT api/<controller>/5
		[Filters.Authorize(Roles = "OrderPattern_M_E")]
        public void Put(OrderPatternViewModel pattern)
        {
            _orderPatternService.UpdatePattern(pattern);
        }

		[HttpGet]
		[Route("api/OrderPattern/GetByName")]
		public OrderPatternViewModel GetByName(string value)
		{
			return _orderPatternService.GetByName(value);
		}
	}
}
