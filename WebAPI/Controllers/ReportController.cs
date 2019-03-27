using System;
using System.Web.Http;
using Service.Services;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Order;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Net;
using Website.ViewModels.Report.CustomerExpense;
using Website.ViewModels.DriverAllowance;
using Website.ViewModels.Expense;
using Website.ViewModels.Report.CustomerPayment;
using Website.ViewModels.Report.CustomerPricing;
using Website.ViewModels.Report.Maintenance;
using Website.ViewModels.Report.PartnerCustomer;
using Website.ViewModels.Report.PartnerExpense;
using Website.ViewModels.Report.TruckRevenue;
using Website.ViewModels.Report.TruckBalance;
using Website.ViewModels.Report.DriverRevenue;
using Website.ViewModels.Report.SupplierExpense;
using Website.ViewModels.Report.TruckExpense;
using Website.ViewModels.Report.FuelConsumption;
using Website.ViewModels.Report.PartnerPayment;
using System.Web;
using Website.ViewModels.Report.CombinedRevenue;

namespace WebAPI.Controllers
{
	public partial class ReportController : ApiController
	{
		public IReportService _reportService;

		public ReportController()
		{
		}

		public ReportController(IReportService reportService)
		{
			_reportService = reportService;
		}
		#region Statistical
		[HttpGet]
		[Route("api/Report/ExportExcelDispatch/{depC}/{dispatchStatus0}/{dispatchStatus1}" +
							   "/{dispatchStatus2}/{orderTypeI}/{customer}/{laguague}" +
							   "/{transportDFrom}/{transportDTo}/{reportI}/{registeredNo}/{driver}" +
							   "/{partner}/{dispatchI}"
			)]
		[Filters.Authorize(Roles = "Dispatch_R")]
		public HttpResponseMessage ExportExcelDispatch(string depC, bool dispatchStatus0, bool dispatchStatus1,
			bool dispatchStatus2, string orderTypeI, string customer, string laguague,
			DateTime? transportDFrom, DateTime? transportDTo, string reportI, string registeredNo, string driver,
			string partner, string dispatchI
			)
		{
			DispatchReportParam param = new DispatchReportParam();
			param.DepC = depC;
			param.DispatchStatus0 = dispatchStatus0;
			param.DispatchStatus1 = dispatchStatus1;
			param.DispatchStatus2 = dispatchStatus2;
			param.OrderTypeI = orderTypeI;
			param.Customer = customer;
			param.Laguague = laguague;
			param.TransportDFrom = transportDFrom;
			param.TransportDTo = transportDTo;
			param.ReportI = reportI;
			param.TruckC = registeredNo;
			param.DriverC = driver;
			param.Partner = partner;
			param.DispatchI = dispatchI;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream;
			if (reportI == "O")
			{
				stream = _reportService.ExportExcelTransportationPlan(param, userName);
			}
			else
			{
				stream = _reportService.ExportExcel(param, userName);
			}

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
			{
				FileName = "Dispatch.xlsx"
			};
			//result.Content.Headers.ContentDisposition.FileName = "Dispatch.xlsx";
			//result.Content.Headers.Add("x-filename", "Dispatch.xlsx");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdf/{depC}/{dispatchStatus0}/{dispatchStatus1}" +
							   "/{dispatchStatus2}/{orderTypeI}/{customer}/{laguague}" +
							   "/{transportDFrom}/{transportDTo}/{reportI}/{registeredNo}/{driver}/{partner}/{dispatchI}"
			)]
		[Filters.Authorize(Roles = "Dispatch_R")]
		public HttpResponseMessage ExportPdf(string depC, bool dispatchStatus0, bool dispatchStatus1,
			bool dispatchStatus2, string orderTypeI, string customer, string laguague,
			DateTime? transportDFrom, DateTime? transportDTo, string reportI, string registeredNo, string driver,
			string partner, string dispatchI
			)
		{
			DispatchReportParam param = new DispatchReportParam();
			param.DepC = depC;
			param.DispatchStatus0 = dispatchStatus0;
			param.DispatchStatus1 = dispatchStatus1;
			param.DispatchStatus2 = dispatchStatus2;
			param.OrderTypeI = orderTypeI;
			param.Customer = customer;
			param.Laguague = laguague;
			param.TransportDFrom = transportDFrom;
			param.TransportDTo = transportDTo;
			param.ReportI = reportI;
			param.TruckC = registeredNo;
			param.DriverC = driver;
			param.Partner = partner;
			param.DispatchI = dispatchI;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdf(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/DriverDispatchReport/{depC}/{transportDFrom}/{transportDTo}" +
							   "/{truckC}/{driverC}" +
							   "/{orderTypeI}/{language}/{reportI}"
								)]
		[Filters.Authorize(Roles = "DriverDispatch_R")]
		public HttpResponseMessage ExportPdf(string depC, DateTime? transportDFrom, DateTime? transportDTo,
												 string truckC, string driverC,
												 string orderTypeI, string language,
												 string reportI
												)
		{
			DriverDispatchReportParam param = new DriverDispatchReportParam();
			param.DepC = depC;
			param.TransportDFrom = transportDFrom;
			param.TransportDTo = transportDTo;
			param.TruckC = truckC;
			//param.RegisteredNo = registeredNo;
			param.DriverC = driverC;
			//param.DriverN = driverN;
			param.OrderTypeI = orderTypeI;
			param.Laguague = language;
			param.ReportI = reportI;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdf(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfTransportExpenseList/{reportI}/{depC}/{transportDFrom}/{transportDTo}" +
																	 "/{customer}/{truckC}/{registeredNo}" +
																	 "/{driverC}/{driverN}" +
																	 "/{partner}/{orderNo}/{blbk}/{jobNo}" +
																	 "/{language}"
								)]
		[Filters.Authorize(Roles = "TransportConfirm_R")]
		public HttpResponseMessage ExportPdfTransportExpense(string reportI, string depC, DateTime? transportDFrom, DateTime? transportDTo,
															 string customer, string truckC, string registeredNo, string driverC, string driverN,
															 string partner, string orderNo, string blbk, string jobNo, string language
												)
		{
			DriverDispatchReportParam param = new DriverDispatchReportParam();
			param.DepC = depC;
			param.TransportDFrom = transportDFrom;
			param.TransportDTo = transportDTo;
			param.TruckC = truckC;
			param.RegisteredNo = registeredNo;
			param.DriverC = driverC;
			param.DriverN = driverN;
			param.Customer = customer;
			param.Partner = partner;
			param.Laguague = language;
			param.ReportI = reportI;
			param.OrderNo = orderNo;
			param.BLBK = blbk;
			param.JobNo = jobNo;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfTransportExpenseList(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportExcelTransportExpenseList/{reportI}/{depC}/{transportDFrom}/{transportDTo}" +
																	 "/{customer}/{truckC}/{registeredNo}" +
																	 "/{driverC}/{driverN}" +
																	 "/{partner}/{orderNo}/{blbk}/{jobNo}/{reportobjectI}" +
																	 "/{language}"
								)]
		[Filters.Authorize(Roles = "TransportConfirm_R")]
		public HttpResponseMessage ExportExcelTransportExpenseList(string reportI, string depC, DateTime? transportDFrom, DateTime? transportDTo,
															 string customer, string truckC, string registeredNo, string driverC, string driverN,
															 string partner, string orderNo, string blbk, string jobNo, string reportobjectI, string language
												)
		{
			DriverDispatchReportParam param = new DriverDispatchReportParam();
			param.DepC = depC;
			param.TransportDFrom = transportDFrom;
			param.TransportDTo = transportDTo;
			param.TruckC = truckC;
			param.RegisteredNo = registeredNo;
			param.DriverC = driverC;
			param.DriverN = driverN;
			param.Customer = customer;
			param.Partner = partner;
			param.Laguague = language;
			param.ReportI = reportI;
			param.OrderNo = orderNo;
			param.BLBK = blbk;
			param.JobNo = jobNo;
			param.ReportObjectI = reportobjectI;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportExcelTransportExpenseList(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
			{
				FileName = "TransportExpense.xlsx"
			};

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportExcelOrder/{entryClerkC}/{depC}/{orderTypeI}/{customer}/{jobNo}/{laguague}" +
			   "/{orderDFrom}/{orderDTo}/{sortBy}")]
		[Filters.Authorize(Roles = "Order_R")]
		public HttpResponseMessage ExportExcelOrder(string entryClerkC, string depC, string orderTypeI, string customer,
			string jobNo,
			string laguague, DateTime? orderDFrom, DateTime? orderDTo, string sortBy)
		{
			OrderReportParam param = new OrderReportParam();
			param.EntryClerkC = entryClerkC;
			param.DepC = depC;
			param.OrderTypeI = orderTypeI;
			param.Customer = customer;
			param.JobNo = jobNo;
			param.Language = laguague;
			param.OrderDFrom = orderDFrom;
			param.OrderDTo = orderDTo;
			param.SortBy = sortBy;

			//var userName = HttpContext.Current.User.Identity.Name;
			//Stream stream = _reportService.ExportPdfExpense(param, userName);

			//HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			//result.Content = new StreamContent(stream);
			//result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			//return result;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportExcelOrderGeneral(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType =
				new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
			{
				FileName = "Order.xlsx"
			};
			//result.Content.Headers.ContentDisposition.FileName = "Dispatch.xlsx";
			//result.Content.Headers.Add("x-filename", "Dispatch.xlsx");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfOrder/{entryClerkC}/{depC}/{orderTypeI}/{customer}/{jobNo}/{laguague}" +
														"/{orderDFrom}/{orderDTo}/{sortBy}"
							  )]
		[Filters.Authorize(Roles = "Order_R")]
		public HttpResponseMessage ExportPdfOrder(string entryClerkC, string depC, string orderTypeI, string customer, string jobNo,
												  string laguague, DateTime? orderDFrom, DateTime? orderDTo, string sortBy
												)
		{
			OrderReportParam param = new OrderReportParam();
			param.EntryClerkC = entryClerkC;
			param.DepC = depC;
			param.OrderTypeI = orderTypeI;
			param.Customer = customer;
			param.JobNo = jobNo;
			param.Language = laguague;
			param.OrderDFrom = orderDFrom;
			param.OrderDTo = orderDTo;
			param.SortBy = sortBy;
			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfOrderGeneral(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfMaintenance/{language}/{maintenanceDFrom}/{maintenanceDTo}/{depC}/{truckC}/{trailerC}/{plan}/{finished}")]
		[Filters.Authorize(Roles = "Maintenance_R")]
		public HttpResponseMessage ExportPdfMaintenance(string language, DateTime maintenanceDFrom, DateTime maintenanceDTo, string depC, string truckC, string trailerC, bool plan, bool finished)
		{
			var param = new MaintenanceReportParam
			{
				Languague = language,
				MaintenanceDFrom = maintenanceDFrom,
				MaintenanceDTo = maintenanceDTo,
				DepC = depC != "null" ? "," + depC + "," : depC,
				TruckC = truckC != "null" ? "," + truckC + "," : truckC,
				TrailerC = trailerC != "null" ? "," + trailerC + "," : trailerC,
				Plan = plan,
				Finished = finished
			};

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfMaintenace(param, userName);

			var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfTransportMaintenance/{language}/{year}/{truckC}")]
		[Filters.Authorize(Roles = "Maintenance_R")]
		public HttpResponseMessage ExportPdfTransportMaintenance(string language, int year, string truckC)
		{
			var param = new MaintenanceReportParam
			{
				Languague = language,
				Year = year,
				TruckC = truckC != "null" ? "," + truckC + "," : truckC,
			};

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfTransportMaintenance(param, userName);

			var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfFuelConsumptionDetailReport/{depC}/{transportDFrom}/{transportDTo}" +
																	 "/{truckC}/{registeredNo}" +
																	 //"/{driverC}/{driverN}" +
																	 "/{orderTypeI}/{customer}/{language}"
								)]
		[Filters.Authorize(Roles = "FuelConsumptionDetail_R")]
		public HttpResponseMessage ExportPdfFuelConsumptionDetailReport(string depC, DateTime transportDFrom, DateTime transportDTo,
															 string truckC, string registeredNo, /*string driverC, string driverN,*/
															 string orderTypeI, string customer, string language
												)
		{
			var param = new FuelConsumptionDetailReportParam
			{
				DepC = depC,
				DateFrom = transportDFrom,
				DateTo = transportDTo,
				TruckC = truckC,
				RegisteredNo = registeredNo,
				//DriverC = driverC,
				//DriverN = driverN,
				OrderTypeI = orderTypeI,
				Customer = customer,
				Languague = language
			};

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfFuelConsumptionDetail(param, userName);

			var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfUseFuelReport/{depC}/{transportDFrom}/{transportDTo}" +
																	 "/{truckC}/{registeredNo}" +
																	 "/{orderTypeI}/{customer}/{language}"
								)]
		[Filters.Authorize(Roles = "UseFuel_R")]
		public HttpResponseMessage ExportPdfUseFuelReport(string depC, DateTime transportDFrom, DateTime transportDTo,
															 string truckC, string registeredNo,
															 string orderTypeI, string customer, string language
												)
		{
			var param = new FuelConsumptionDetailReportParam
			{
				DepC = depC,
				DateFrom = transportDFrom,
				DateTo = transportDTo,
				TruckC = truckC,
				RegisteredNo = registeredNo,
				OrderTypeI = orderTypeI,
				Customer = customer,
				Languague = language
			};

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfUseFuelReport(param, userName);

			var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}
		#endregion

		#region Expense
		[HttpGet]
		[Route("api/Report/ExportExcelCustomerExpense/{reportI}/{depC}/{orderTypeI}/{customer}/{transportM}/{fromD}/{toD}/{invoiceStatus}/{blbk}/{reportType}/{language}/{jobNo}/{lolo}")]
		[Filters.Authorize(Roles = "PaymentRequestDetail_R")]
		public HttpResponseMessage ExportExcelCustomerExpense(string reportI, string depC, string orderTypeI, string customer,
															  DateTime? transportM, DateTime? fromD, DateTime? toD, string invoiceStatus, string blbk, int reportType, string language, string jobNo, string lolo)
		{
			CustomerExpenseReportParam param = new CustomerExpenseReportParam();
			param.ReportI = reportI;
			param.DepC = depC;
			param.OrderTypeI = orderTypeI;
			param.Customer = customer;
			param.TransportM = transportM;
			param.OrderDFrom = fromD;
			param.OrderDTo = toD;
			param.InvoiceStatus = invoiceStatus;
			param.BLBK = blbk;
			param.ReportType = reportType;
			param.Languague = language;
			param.JobNo = jobNo;
			param.LoLo = lolo;
			Stream stream;
			var userName = HttpContext.Current.User.Identity.Name;
			if (reportType == 2)
			{
				stream = _reportService.ExportExcelCustomerExpenseTransportFeeVertical(param);
			}
			else if (reportType == 3)
			{
				stream = _reportService.ExportExcelCustomerExpenseTransportFeeHorizontal(param);
			}
			else if (reportType == 4)
			{
				stream = _reportService.ExportExcelCustomerExpenseLOLOHorizontal(param);
			}
			else if (reportType == 5)
			{
				stream = _reportService.ExportExcelCustomerExpenseLOLOVertical(param);
			}
			else if (reportType == 0)
			{
				stream = _reportService.ExportExcelCustomerExpenseGeneral(param);
			}
			else
			{
				stream = _reportService.ExportExcelCustomerExpenseLoad(param, userName);
			}

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
			{
				FileName = "CustomerExpense.xlsx"
			};

			return result;
		}

		[HttpGet]
		[Route("api/Report/UpdateInvoiceStatus/{reportI}/{depC}/{orderTypeI}/{customer}/{transportM}/{fromD}/{toD}/{blbk}/{language}/{jobNo}")]
		public int UpdateInvoiceStatus(string reportI, string depC, string orderTypeI, string customer,
															  DateTime? transportM, DateTime? fromD, DateTime? toD, string blbk, string language, string jobNo)
		{
			var result = "0";
			CustomerExpenseReportParam param = new CustomerExpenseReportParam();
			param.ReportI = reportI;
			param.DepC = depC;
			param.OrderTypeI = orderTypeI;
			param.Customer = customer;
			param.TransportM = transportM;
			param.OrderDFrom = fromD;
			param.OrderDTo = toD;
			param.BLBK = blbk;
			param.Languague = language;
			param.JobNo = jobNo;
			return _reportService.UpdateInvoiceStatus(param);
		}

		[HttpGet]
		[Route("api/Report/ExportPdfCustomerExpense/{reportI}/{depC}/{orderTypeI}/{customer}/{transportM}/{fromD}/{toD}/{invoiceStatus}/{blbk}/{reportType}/{language}/{jobNo}")]
		[Filters.Authorize(Roles = "PaymentRequestDetail_R")]
		public HttpResponseMessage ExportPdfCustomerExpense(string reportI, string depC, string orderTypeI, string customer,
			DateTime? transportM, DateTime? fromD, DateTime? toD, string invoiceStatus, string blbk, int reportType, string language, string jobNo)
		{
			CustomerExpenseReportParam param = new CustomerExpenseReportParam();
			param.ReportI = reportI;
			param.DepC = depC;
			param.OrderTypeI = orderTypeI;
			param.Customer = customer;
			param.TransportM = transportM;
			param.OrderDFrom = fromD;
			param.OrderDTo = toD;
			param.InvoiceStatus = invoiceStatus;
			param.BLBK = blbk;
			param.ReportType = reportType;
			param.Languague = language;
			param.JobNo = jobNo;
			Stream stream;
			var userName = HttpContext.Current.User.Identity.Name;
			if (reportType == 0)
				stream = _reportService.ExportPdfCustomerExpense(param);
			else
				stream = _reportService.ExportPdfCustomerExpenseLoad(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfDriverAllowance/{depC}/{allowanceType}/{laguague}/{allowanceDFrom}/{allowanceDTo}")]
		[Filters.Authorize(Roles = "DriverAllowance_R")]
		public HttpResponseMessage ExportPdfDriverAllowance(string depC, string allowanceType, string laguague,
												DateTime allowanceDFrom, DateTime allowanceDTo
												)
		{
			DriverAllowanceReportParam param = new DriverAllowanceReportParam();
			param.DepC = depC;
			param.AllowanceType = allowanceType;
			param.Laguague = laguague;
			param.AllowanceDFrom = allowanceDFrom;
			param.AllowanceDTo = allowanceDTo;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfDriverAllowance(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfDriverSalary/{depC}/{allowanceType}/{laguague}/{allowanceDFrom}/{allowanceDTo}")]
		[Filters.Authorize(Roles = "DriverAllowance_R")]
		public HttpResponseMessage ExportPdfDriverSalary(string depC, string allowanceType, string laguague,
												DateTime allowanceDFrom, DateTime allowanceDTo
												)
		{
			DriverAllowanceReportParam param = new DriverAllowanceReportParam();
			param.DepC = depC;
			param.AllowanceType = allowanceType;
			param.Laguague = laguague;
			param.AllowanceDFrom = allowanceDFrom;
			param.AllowanceDTo = allowanceDTo;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfDriverSalary(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		//[HttpGet]
		//[Route("api/Report/ExportDatatableDriverAllowance/{depC}/{allowanceType}/{laguague}/{allowanceDFrom}/{allowanceDTo}")]
		//public async Task<IHttpActionResult> GetDatatableDriverAllowance(string depC, string allowanceType, string laguague,
		//										DateTime allowanceDFrom, DateTime allowanceDTo)
		//{
		//	DriverAllowanceReportParam param = new DriverAllowanceReportParam();
		//	param.DepC = depC;
		//	param.AllowanceType = allowanceType;
		//	param.Laguague = laguague;
		//	param.AllowanceDFrom = allowanceDFrom;
		//	param.AllowanceDTo = allowanceDTo;

		//	List<DriverAllowanceListReport> data = _reportService.GetDriverAllowanceList(param);
		//	List<DriverAllowanceTableReport> returnData = new List<DriverAllowanceTableReport>();
		//	decimal total = 0;

		//	foreach (var item in data)
		//	{
		//		if (returnData.Exists(m => m.OrderD == item.OrderD) == false)
		//		{
		//			returnData.Add(new DriverAllowanceTableReport() { OrderD = item.OrderD, Items = new List<DriverAllowanceListReport>() });
		//			var temp = returnData.Where(m => m.OrderD == item.OrderD).First();
		//			if (temp != null)
		//			{
		//				temp.Items.Add(item);
		//			}
		//		}
		//		else
		//		{
		//			var temp = returnData.Where(m => m.OrderD == item.OrderD).First();
		//			if (temp != null)
		//			{
		//				temp.Items.Add(item);
		//			}
		//		}
		//	}

		//	var reportDatatable = await Task.Run(() => returnData);
		//	if (reportDatatable == null)
		//	{
		//		return NotFound();
		//	}
		//	return Ok(reportDatatable);
		//}

		[HttpGet]
		[Route("api/Report/ExportExcelExpense/{supplier}/{category}/{laguague}/{invoiceDFrom}" +
										  "/{invoiceDTo}/{paymentMethod}/{reportType}/{reportI}" +
										  "/{objectI}/{truck}/{trailer}/{employee}/{driver}")]
		[Filters.Authorize(Roles = "Expense_R")]
		public HttpResponseMessage ExportExcelExpense(string supplier, string category, string laguague,
													DateTime invoiceDFrom, DateTime invoiceDTo, string paymentMethod,
													string reportType, string reportI, string objectI, string truck,
													string trailer, string employee, string driver)
		{
			ExpenseReportParam param = new ExpenseReportParam();
			param.Suppliers = supplier;
			param.ExpenseCategories = category;
			param.Language = laguague;
			param.InvoiceDFrom = invoiceDFrom;
			param.InvoiceDTo = invoiceDTo;
			param.PaymentMethod = paymentMethod;
			param.ReportType = reportType;
			param.ReportI = reportI;
			param.ObjectI = objectI;
			param.Trucks = truck;
			param.Trailers = trailer;
			param.Employees = employee;
			param.Drivers = driver;

			//var userName = HttpContext.Current.User.Identity.Name;
			//Stream stream = _reportService.ExportPdfExpense(param, userName);

			//HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			//result.Content = new StreamContent(stream);
			//result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			//return result;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportExcelExpense(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
			{
				FileName = "Expense.xlsx"
			};
			//result.Content.Headers.ContentDisposition.FileName = "Dispatch.xlsx";
			//result.Content.Headers.Add("x-filename", "Dispatch.xlsx");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfExpense/{supplier}/{category}/{laguague}/{invoiceDFrom}" +
										  "/{invoiceDTo}/{paymentMethod}/{reportType}/{reportI}" +
										  "/{objectI}/{truck}/{trailer}/{employee}/{driver}")]
		[Filters.Authorize(Roles = "Expense_R")]
		public HttpResponseMessage ExportPdfExpense(string supplier, string category, string laguague,
													DateTime invoiceDFrom, DateTime invoiceDTo, string paymentMethod,
													string reportType, string reportI, string objectI, string truck,
													string trailer, string employee, string driver)
		{
			ExpenseReportParam param = new ExpenseReportParam();
			param.Suppliers = supplier;
			param.ExpenseCategories = category;
			param.Language = laguague;
			param.InvoiceDFrom = invoiceDFrom;
			param.InvoiceDTo = invoiceDTo;
			param.PaymentMethod = paymentMethod;
			param.ReportType = reportType;
			param.ReportI = reportI;
			param.ObjectI = objectI;
			param.Trucks = truck;
			param.Trailers = trailer;
			param.Employees = employee;
			param.Drivers = driver;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfExpense(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfCustomersExpense/{depC}/{orderTypeI}/{customer}/{transportM}/{language}")]
		[Filters.Authorize(Roles = "PaymentRequestTable_R")]
		public HttpResponseMessage ExportPdfCustomersExpense(string depC, string orderTypeI, string customer,
															 DateTime transportM, string language)
		{
			CustomerExpenseReportParam param = new CustomerExpenseReportParam();
			param.DepC = depC;
			param.OrderTypeI = orderTypeI;
			param.Customer = customer;
			param.TransportM = transportM;
			param.Languague = language;

			Stream stream = _reportService.ExportPdfCustomersExpense(param);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfLiabilities/{driverList}/{laguague}/{dateFrom}/{dateTo}/{reportType}")]
		[Filters.Authorize(Roles = "Liabilities_R")]
		public HttpResponseMessage ExportPdfLiabilities(string driverList, string laguague,
												DateTime dateFrom, DateTime dateTo, int reportType)
		{
			DriverRevenueReportParam param = new DriverRevenueReportParam();
			param.DateFrom = dateFrom;
			param.DateTo = dateTo;
			param.Languague = laguague;
			param.DriverList = driverList;
			param.ReportType = reportType;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfLiabilities(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfLiabilitiesPayment/{driverList}/{laguague}/{dateFrom}/{dateTo}")]
		[Filters.Authorize(Roles = "Liabilities_R")]
		public HttpResponseMessage ExportPdfLiabilitiesPayment(string driverList, string laguague,
												DateTime dateFrom, DateTime dateTo)
		{
			DriverRevenueReportParam param = new DriverRevenueReportParam();
			param.DateFrom = dateFrom;
			param.DateTo = dateTo;
			param.Languague = laguague;
			param.DriverList = driverList;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfLiabilitiesPayment(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfFuelConsumption/{driverList}/{truckList}/{languague}/{dateFrom}/{dateTo}/{reportType}")]
		[Filters.Authorize(Roles = "FuelConsumption_R")]
		public HttpResponseMessage ExportPdfFuelConsumption(string driverList, string truckList, string languague,
												DateTime dateFrom, DateTime dateTo, int reportType)
		{
			FuelConsumptionReportParam param = new FuelConsumptionReportParam();
			param.DateFrom = dateFrom;
			param.DateTo = dateTo;
			param.Languague = languague;
			param.ReportType = reportType;
			param.DriverList = driverList;
			param.TruckList = truckList;

			Stream stream = _reportService.ExportPdfFuelConsumption(param);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfPartnerCustomerExpense/{depC}/{transportM}/{partner}/{customer}/{languague}")]
		[Filters.Authorize(Roles = "PartnerPaymentTable_R")]
		public HttpResponseMessage ExportPdfPartnerCustomerExpense(string depC, DateTime transportM,
																   string partner, string customer, string languague
			)
		{
			PartnerCustomerExpenseReportParam param = new PartnerCustomerExpenseReportParam();
			param.DepC = depC;
			param.TransportM = transportM;
			param.Partner = partner;
			param.Customer = customer;
			param.Languague = languague;

			Stream stream = _reportService.ExportPdfPartnerCustomerExpense(param);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfPartnerExpense/{reportI}/{depC}/{transportM}/{transportDFrom}/{transportDTo}/{partner}/{languague}")]
		[Filters.Authorize(Roles = "PartnerPaymentManagement_R")]
		public HttpResponseMessage ExportPdfPartnerExpense(string reportI, string depC, DateTime? transportM,
															DateTime? transportDFrom, DateTime? transportDTo, string partner, string languague
			)
		{
			PartnerExpenseReportParam param = new PartnerExpenseReportParam();
			param.ReportI = reportI;
			param.DepC = depC;
			param.TransportM = transportM;
			param.TransportDFrom = transportDFrom;
			param.TransportDTo = transportDTo;
			param.Partner = partner;
			param.Languague = languague;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfPartnerDetailExpense(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfSupplierExpense/{expenseDFrom}/{expenseDTo}/{supplier}/{expense}/{languague}")]
		[Filters.Authorize(Roles = "SupplierPaymentManagement_R")]
		public HttpResponseMessage ExportPdfSupplierExpense(DateTime? expenseDFrom, DateTime? expenseDTo,
																   string supplier, string expense, string languague
			)
		{
			SupplierExpenseReportParam param = new SupplierExpenseReportParam();
			param.ExpenseDFrom = expenseDFrom;
			param.ExpenseDTo = expenseDTo;
			param.Supplier = supplier;
			param.Expense = expense;
			param.Languague = languague;

			Stream stream = _reportService.ExportPdfSupplierExpense(param);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfTruckExpense/{depC}/{laguague}/{dateFrom}/{dateTo}")]
		[Filters.Authorize(Roles = "TruckExpense_R")]
		public HttpResponseMessage ExportPdfTruckExpense(string depC, string laguague,
												DateTime dateFrom, DateTime dateTo
												)
		{
			TruckExpenseReportParam param = new TruckExpenseReportParam();
			param.DepC = depC;
			param.Languague = laguague;
			param.DateFrom = dateFrom;
			param.DateTo = dateTo;

			Stream stream = _reportService.ExportPdfTruckExpense(param);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}
		#endregion

		#region Revenue
		[HttpGet]
		[Route("api/Report/ExportPdfCombinedRevenue/{reportTimeI}/{transportM}/{objectType}/{objectList}/{reportType}/{language}")]
		[Filters.Authorize(Roles = "CombinedRevenue_R")]
		public HttpResponseMessage ExportPdfCombinedRevenue(string reportTimeI, DateTime transportM, string objectType,
													 string objectList, string reportType, string language)
		{
			CombinedRevenueReportParam param = new CombinedRevenueReportParam();
			param.ReportTimeI = reportTimeI;
			param.TransportM = transportM;
			param.ObjectType = objectType;
			param.ObjectList = objectList;
			param.ReportType = reportType;
			param.Languague = language;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfCombinedRevenue(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfCustomerBalance/{depC}/{orderTypeI}/{customer}/{transportM}/{language}")]
		[Filters.Authorize(Roles = "CustomerBalance_R")]
		public HttpResponseMessage ExportPdfCustomerBalance(string depC, string orderTypeI, string customer,
															 DateTime transportM, string language)
		{
			CustomerExpenseReportParam param = new CustomerExpenseReportParam();
			param.DepC = depC;
			param.OrderTypeI = orderTypeI;
			param.Customer = customer;
			param.TransportM = transportM;
			param.Languague = language;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfCustomerBalance(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfCustomerPayment/{customer}/{language}/{dateFrom}/{dateTo}/{reportType}")]
		[Filters.Authorize(Roles = "CustomerPayment_R")]
		public HttpResponseMessage ExportPdfCustomerPayment(string customer, string language, DateTime dateFrom,
															 DateTime dateTo, string reportType)
		{
			var param = new CustomerPaymentReportParam
			{
				Customer = customer,
				DateFrom = dateFrom,
				DateTo = dateTo,
				Languague = language,
				ReportI = reportType
			};

			Stream stream = null;
			var userName = HttpContext.Current.User.Identity.Name;
			if (reportType == "1")
			{
				stream = _reportService.ExportPdfCustomerPaymentDetail(param, userName);
			}
			else
			{
				stream = _reportService.ExportPdfCustomerPaymentGeneral(param, userName);
			}

			var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfCustomerPaymentProgress/{customer}/{language}/{dateFrom}/{dateTo}")]
		[Filters.Authorize(Roles = "CustomerPaymentProgress_R")]
		public HttpResponseMessage ExportPdfCustomerPaymentProgress(string customer, string language, DateTime dateFrom, DateTime dateTo)
		{
			var param = new CustomerPaymentReportParam
			{
				Customer = customer,
				DateFrom = dateFrom,
				DateTo = dateTo,
				Languague = language,
			};

			Stream stream = null;
			var userName = HttpContext.Current.User.Identity.Name;
			stream = _reportService.ExportPdfCustomerPaymentProgress(param, userName);

			var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}
		[HttpGet]
		[Route("api/Report/ExportPdfTruckRevenue/{truckList}/{laguague}/{dateFrom}/{dateTo}/{reportType}")]
		[Filters.Authorize(Roles = "TruckRevenue_R")]
		public HttpResponseMessage ExportPdfTruckRevenue(string truckList, string laguague,
												DateTime dateFrom, DateTime dateTo, int reportType)
		{
			TruckRevenueReportParam param = new TruckRevenueReportParam();
			param.DateFrom = dateFrom;
			param.DateTo = dateTo;
			param.Languague = laguague;
			param.TruckList = truckList;

			Stream stream = reportType == 0 ? _reportService.ExportPdfTruckRevenueGeneral(param) : _reportService.ExportPdfTruckRevenueDetail(param);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}
		[HttpGet]
		[Route("api/Report/ExportPdfTruckBalance/{depC}/{truckList}/{laguague}/{dateM}")]
		[Filters.Authorize(Roles = "TruckBalance_R")]
		public HttpResponseMessage ExportPdfTruckBalance(string depC, string truckList, string laguague, DateTime dateM)
		{
			var dateFrom = new DateTime(dateM.Year, dateM.Month, 1);
			var dateTo = dateFrom.AddMonths(1).AddDays(-1);
			TruckBalanceReportParam param = new TruckBalanceReportParam();
			param.DepC = depC;
			param.DateFrom = dateFrom;
			param.DateTo = dateTo;
			param.Languague = laguague;
			param.TruckList = truckList;

			var userName = HttpContext.Current.User.Identity.Name;
			Stream stream = _reportService.ExportPdfTruckBalance(param, userName);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfDriverRevenue/{driverList}/{laguague}/{dateFrom}/{dateTo}/{reportType}")]
		[Filters.Authorize(Roles = "DriverRevenue_R")]
		public HttpResponseMessage ExportPdfDriverRevenue(string driverList, string laguague,
												DateTime dateFrom, DateTime dateTo, int reportType)
		{
			DriverRevenueReportParam param = new DriverRevenueReportParam();
			param.DateFrom = dateFrom;
			param.DateTo = dateTo;
			param.Languague = laguague;
			param.DriverList = driverList;

			Stream stream = reportType == 0
				? _reportService.ExportPdfDriverRevenueGeneral(param)
				: _reportService.ExportPdfDriverRevenueDetail(param);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}
		[HttpGet]
		[Route("api/Report/ExportPdfOrderBalance/{depC}/{transportDFrom}/{transportDTo}" +
																	 "/{truckC}/{registeredNo}" +
																	 "/{driverC}/{driverN}" +
																	 "/{orderTypeI}/{customer}/{language}/{reportI}"
								)]
		[Filters.Authorize(Roles = "OrderBalance_R")]
		public HttpResponseMessage ExportPdfOrderBalance(string depC, DateTime? transportDFrom, DateTime? transportDTo,
															 string truckC, string registeredNo, string driverC, string driverN,
															 string orderTypeI, string customer, string language, string reportI
												)
		{
			DriverDispatchReportParam param = new DriverDispatchReportParam();
			param.DepC = depC;
			param.TransportDFrom = transportDFrom;
			param.TransportDTo = transportDTo;
			param.TruckC = truckC;
			param.RegisteredNo = registeredNo;
			param.DriverC = driverC;
			param.DriverN = driverN;
			param.OrderTypeI = orderTypeI;
			param.Customer = customer;
			param.Laguague = language;
			param.ReportI = reportI;

			Stream stream = _reportService.ExportPdfOrderBalance(param);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfCustomerPricing/{customer}/{location1}/{location2}/{contSize}/{contType}/{dateFrom}/{dateTo}/{language}/{reportType}")]
		[Filters.Authorize(Roles = "CustomerPricing_R")]
		public HttpResponseMessage ExportPdfCustomerPricing(string customer, string location1, string location2,
			string contSize, string contType, DateTime dateFrom, DateTime dateTo, string language, string reportType)
		{
			var param = new CustomerPricingReportParam
			{
				Customer = customer,
				Location1 = location1,
				Location2 = location2,
				ContainerSize = contSize,
				ContainerType = contType,
				DateFrom = dateFrom,
				DateTo = dateTo,
				Languague = language,
				ReportType = reportType
			};

			Stream stream = null;
			stream = reportType == "1" ? _reportService.ExportPdfCustomerPricingDetail(param)
										: _reportService.ExportPdfCustomerPricingGeneral(param);

			var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}
		[HttpGet]
		[Route("api/Report/ExportPdfPartnerPayment/{partner}/{language}/{dateFrom}/{dateTo}/{reportType}")]
		[Filters.Authorize(Roles = "PartnerPayment_R")]
		public HttpResponseMessage ExportPdfPartnerPayment(string partner, string language, DateTime dateFrom,
															 DateTime dateTo, string reportType)
		{
			var param = new PartnerPaymentReportParam
			{
				Partner = partner,
				DateFrom = dateFrom,
				DateTo = dateTo,
				Languague = language,
				ReportI = reportType
			};

			Stream stream = null;
			var userName = HttpContext.Current.User.Identity.Name;
			if (reportType == "1")
			{
				stream = _reportService.ExportPdfPartnerPaymentDetail(param, userName);
			}
			else
			{
				stream = _reportService.ExportPdfPartnerPaymentGeneral(param, userName);
			}

			var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfPartnerPaymentProgress/{partner}/{language}/{dateFrom}/{dateTo}")]
		[Filters.Authorize(Roles = "PartnerPaymentProgress_R")]
		public HttpResponseMessage ExportPdfPartnerPaymentProgress(string partner, string language, DateTime dateFrom, DateTime dateTo)
		{
			var param = new PartnerPaymentReportParam
			{
				Partner = partner,
				DateFrom = dateFrom,
				DateTo = dateTo,
				Languague = language,
			};

			Stream stream = null;
			var userName = HttpContext.Current.User.Identity.Name;
			stream = _reportService.ExportPdfPartnerPaymentProgress(param, userName);

			var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfPartnerBalance/{partner}/{transportM}/{language}")]
		[Filters.Authorize(Roles = "PartnerBalance_R")]
		public HttpResponseMessage ExportPdfPartnerBalance(string partner, DateTime transportM, string language)
		{
			PartnerExpenseReportParam param = new PartnerExpenseReportParam();
			param.Partner = partner;
			param.TransportM = transportM;
			param.Languague = language;

			Stream stream = _reportService.ExportPdfPartnerBalance(param);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfTransportHandover/{orderD}/{orderNo}/{detailNo}/{dispatchNo}")]
		[Filters.Authorize(Roles = "Dispatch")]
		public HttpResponseMessage ExportPdfTransportHandover(DateTime orderD, string orderNo, int detailNo, int dispatchNo)
		{
			DispatchViewModel param = new DispatchViewModel();
			param.OrderD = orderD;
			param.OrderNo = orderNo;
			param.DetailNo = detailNo;
			param.DispatchNo = dispatchNo;

			Stream stream = _reportService.ExportPdfTransportHandover(param);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}

		[HttpGet]
		[Route("api/Report/ExportPdfTransportInstruction/{orderD}/{orderNo}/{detailNo}/{dispatchNo}")]
		[Filters.Authorize(Roles = "Dispatch")]
		public HttpResponseMessage ExportPdfTransportInstruction(DateTime orderD, string orderNo, int detailNo, int dispatchNo)
		{
			DispatchViewModel param = new DispatchViewModel();
			param.OrderD = orderD;
			param.OrderNo = orderNo;
			param.DetailNo = detailNo;
			param.DispatchNo = dispatchNo;

			Stream stream = _reportService.ExportPdfTransportInstruction(param);

			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			result.Content = new StreamContent(stream);
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

			return result;
		}
		#endregion
	}
}