using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace CrystalReport.Service.CustomerPayment
{
	public class ExportPdf
	{
		public static Stream GeneralExec(Dataset.CustomerPayment.CustomerPaymentGeneral.CustomerPaymentGeneralDataTable dataTable, int language,
			DateTime dateFrom, DateTime dateTo, Dictionary<string, string> textResources,
			string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				var strPath = HttpContext.Current.Request.MapPath("~/FileRpt/CustomerPayment/CustomerPaymentGeneral.rpt");

				var report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)dataTable);

				// set parameters
				var dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				var dateToFormated = dateTo.ToString("dd/MM/yyyy");
				var datePeriod = "";
				var hdCustomerPayment = textResources["RPHDCUSTOMERPAYMENT"];
				var hdCustomerN = textResources["RPHDCUSTOMERN"];
				var hdOpeningBalance = textResources["RPHDOPENINGBALANCE"];
				var hdOwnExpense = textResources["RPHDOWNEXPENSE"];
				var hdOweExpense = textResources["RPHDOWEEXPENSE"];
				var hdClosingBalance = textResources["RPHDCLOSINGBALANCE"];
				var lblTotal = textResources["RPLBLTOTAL"];
				var hdTransportFee = textResources["RPHDAMOUNT"];
				var hdExpense = textResources["LBLEXPENSEREPORT"];

				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == 2)
				{
					var cul = CultureInfo.GetCultureInfo("en-US");
					report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);
					dateFromFormated = dateFrom.ToString("MM/dd/yyyy");
					dateToFormated = dateTo.ToString("MM/dd/yyyy");
					datePeriod = string.Format("Period {0} to {1}", dateFromFormated, dateToFormated);
				}
				else
				{
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
					dateFromFormated = dateFrom.Year + "年" + dateFrom.Month + "月" + dateFrom.Day + "日";
					dateToFormated = dateTo.Year + "年" + dateTo.Month + "月" + dateTo.Day + "日";
					datePeriod = string.Format("{0}から{1}まで", dateFromFormated, dateToFormated);
				}

				report.SetParameterValue("datePeriod", datePeriod);
				report.SetParameterValue("hdCustomerPayment", hdCustomerPayment);
				report.SetParameterValue("hdCustomerN", hdCustomerN);
				report.SetParameterValue("hdOpeningBalance", hdOpeningBalance);
				report.SetParameterValue("hdOweExpense", hdOweExpense);
				report.SetParameterValue("hdOwnExpense", hdOwnExpense);
				report.SetParameterValue("hdClosingBalance", hdClosingBalance);
				report.SetParameterValue("lblTotal", lblTotal);
				report.SetParameterValue("hdTransportFee", hdTransportFee);
				report.SetParameterValue("hdExpense", hdExpense);
				// set datatable images
				var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);
				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				report.SetParameterValue("imageUrl", path);
				report.SetParameterValue("user", user);

				report.SetParameterValue("ftPrintTime", textResources["RPFTPRINTTIME"]);
				report.SetParameterValue("ftPrintBy", textResources["RPFTPRINTBY"]);
				report.SetParameterValue("ftPage", textResources["RPFTPAGE"]);
				report.SetParameterValue("ftCreator", textResources["RPFTCREATOR"]);

				Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
				report.Close();
				report.Dispose();
				GC.Collect();
				return stream;
			}
			catch (NullReferenceException)
			{
				throw new NullReferenceException();
			}
		}

		public static Stream Exec(Dataset.CustomerPayment.CustomerPayment.CustomerPaymentDataTable dataTable, int language,
			DateTime dateFrom, DateTime dateTo, Dictionary<string, string> textResources,
			string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				var strPath = HttpContext.Current.Request.MapPath("~/FileRpt/CustomerPayment/CustomerPayment.rpt");

				var report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)dataTable);

				// set parameters
				var dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				var dateToFormated = dateTo.ToString("dd/MM/yyyy");
				var datePeriod = "";
				var hdCustomerPaymentDetail = textResources["RPHDCUSTOMERPAYMENTDETAIL"];
				var lblCustomerN = textResources["RPLBLCUSTOMERN"];
				var hdDate = textResources["LBLMONTH"];
				var hdContent = textResources["RPHDCONTENT"];
				var hdTransportFee = textResources["RPHDAMOUNT"];
				var hdExpense = textResources["LBLEXPENSEREPORT"];
				var hdSurcharge = textResources["RPHDTOTALSURCHARGE"];
				var hdDetainAmount = textResources["RPHDDETAINAMOUNT"];
				var hdDiscount = textResources["RPHDCUSTOMERDISCOUNT"];
				var hdTaxAmount = textResources["RPHDTAXAMOUNT"];
				var hdOwe = textResources["RPHDOWEEXPENSE"];
				var hdOwn = textResources["RPHDOWNEXPENSE"];
				var lblOpeningBalance = textResources["RPLBLOPENINGBALANCE"];
				var lblClosingBalance = textResources["RPLBLCLOSINGBALANCE"];
				var lblTotal = textResources["RPLBLTOTAL"];
				var hdAmount = textResources["LBLAMOUNTREPORT"];

				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == 2)
				{
					var cul = CultureInfo.GetCultureInfo("en-US");
					report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);
					dateFromFormated = dateFrom.ToString("MM/dd/yyyy");
					dateToFormated = dateTo.ToString("MM/dd/yyyy");
					datePeriod = string.Format("Period {0} to {1}", dateFromFormated, dateToFormated);
				}
				else
				{
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
					dateFromFormated = dateFrom.Year + "年" + dateFrom.Month + "月" + dateFrom.Day + "日";
					dateToFormated = dateTo.Year + "年" + dateTo.Month + "月" + dateTo.Day + "日";
					datePeriod = string.Format("{0}から{1}まで", dateFromFormated, dateToFormated);
				}

				report.SetParameterValue("datePeriod", datePeriod);
				report.SetParameterValue("hdCustomerPaymentDetail", hdCustomerPaymentDetail);
				report.SetParameterValue("lblCustomerN", lblCustomerN);
				report.SetParameterValue("lblOpeningBalance", lblOpeningBalance);
				report.SetParameterValue("lblClosingBalance", lblClosingBalance);
				report.SetParameterValue("hdDate", hdDate);
				report.SetParameterValue("hdContent", hdContent);
				report.SetParameterValue("hdOwe", hdOwe);
				report.SetParameterValue("hdOwn", hdOwn);
				report.SetParameterValue("hdTransportFee", hdTransportFee);
				report.SetParameterValue("hdExpense", hdExpense);
				report.SetParameterValue("hdSurcharge", hdSurcharge);
				report.SetParameterValue("hdDetainAmount", hdDetainAmount);
				report.SetParameterValue("hdDiscount", hdDiscount);
				report.SetParameterValue("hdTaxAmount", hdTaxAmount);
				report.SetParameterValue("lblTotal", lblTotal);
				report.SetParameterValue("hdAmount", hdAmount);

				// set datatable images
				var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);
				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				report.SetParameterValue("imageUrl", path);
				report.SetParameterValue("user", user);

				Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
				report.Close();
				report.Dispose();
				GC.Collect();
				return stream;
			}
			catch (NullReferenceException)
			{
				throw new NullReferenceException();
			}
		}

		public static Stream DetailExec(Dataset.CustomerPayment.CustomerPaymentGeneral.CustomerPaymentDetailDataTable dataTable, int language,
			DateTime dateFrom, DateTime dateTo, Dictionary<string, string> textResources,
			string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				var strPath = HttpContext.Current.Request.MapPath("~/FileRpt/CustomerPayment/CustomerPaymentDetail.rpt");

				var report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)dataTable);

				// set parameters
				var dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				var dateToFormated = dateTo.ToString("dd/MM/yyyy");
				var datePeriod = "";
				var hdCustomerPayment = textResources["RPHDCUSTOMERPAYMENT"];
				var hdCustomerN = textResources["RPHDCUSTOMERN"];
				var hdOpeningBalance = textResources["RPHDOPENINGBALANCE"];
				var hdOwnExpense = textResources["RPHDOWNEXPENSE"];
				var hdOweExpense = textResources["RPHDOWEEXPENSE"];
				var hdClosingBalance = textResources["RPHDCLOSINGBALANCE"];
				var lblTotal = textResources["RPLBLTOTAL"];
				var hdTransportFee = textResources["RPHDAMOUNT"];
				var hdExpense = textResources["LBLEXPENSEREPORT"];

				var hdCommodity = textResources["RPHDCOMMODITY"];
				var hdContType = textResources["LBLEXPENSETYPEREPORT"];
				var hdQuantity = textResources["RPHDQUANTITYNETWEIGHT"];
				var hdUnitPrice = textResources["RPHDUNITPRICE"];
				var hdTotal = textResources["RPHDTOLAMOUNT"];

				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == 2)
				{
					var cul = CultureInfo.GetCultureInfo("en-US");
					report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);
					dateFromFormated = dateFrom.ToString("MM/dd/yyyy");
					dateToFormated = dateTo.ToString("MM/dd/yyyy");
					datePeriod = string.Format("Period {0} to {1}", dateFromFormated, dateToFormated);
				}
				else
				{
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
					dateFromFormated = dateFrom.Year + "年" + dateFrom.Month + "月" + dateFrom.Day + "日";
					dateToFormated = dateTo.Year + "年" + dateTo.Month + "月" + dateTo.Day + "日";
					datePeriod = string.Format("{0}から{1}まで", dateFromFormated, dateToFormated);
				}

				report.SetParameterValue("datePeriod", datePeriod);
				report.SetParameterValue("hdCustomerPayment", hdCustomerPayment);
				report.SetParameterValue("hdCustomerN", hdCustomerN);
				report.SetParameterValue("hdOpeningBalance", hdOpeningBalance);
				report.SetParameterValue("hdOweExpense", hdOweExpense);
				report.SetParameterValue("hdOwnExpense", hdOwnExpense);
				report.SetParameterValue("hdClosingBalance", hdClosingBalance);
				report.SetParameterValue("lblTotal", lblTotal);
				report.SetParameterValue("hdTransportFee", hdTransportFee);
				report.SetParameterValue("hdExpense", hdExpense);

				report.SetParameterValue("hdCommodity", hdCommodity);
				report.SetParameterValue("hdContType", hdContType);
				report.SetParameterValue("hdQuantity", hdQuantity);
				report.SetParameterValue("hdUnitPrice", hdUnitPrice);
				report.SetParameterValue("hdTotal", hdTotal);
				// set datatable images
				var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);
				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				report.SetParameterValue("imageUrl", path);
				report.SetParameterValue("user", user);

				report.SetParameterValue("ftPrintTime", textResources["RPFTPRINTTIME"]);
				report.SetParameterValue("ftPrintBy", textResources["RPFTPRINTBY"]);
				report.SetParameterValue("ftPage", textResources["RPFTPAGE"]);
				report.SetParameterValue("ftCreator", textResources["RPFTCREATOR"]);

				Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
				report.Close();
				report.Dispose();
				GC.Collect();
				return stream;
			}
			catch (NullReferenceException)
			{
				throw new NullReferenceException();
			}
		}
	}
}
