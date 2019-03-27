using System;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Web;
using System.Globalization;
using System.Data;
using CrystalDecisions.Shared;

namespace CrystalReport.Service.Liabilities
{
	public class ExportPdf
	{
		public static Stream Exec(Dataset.Liabilities.Liabilities.LiabilitiesDataTable datatable, string language, DateTime dateFrom, DateTime dateTo, int reportType,
									string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				var type = reportType == 0 ? "General" : "";
				string strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Liabilities/Liabilities" + type + "ListEn.rpt");
				if (language == "jp")
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Liabilities/Liabilities" + type + "ListJp.rpt");
				}
				else if (language == "vi")
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Liabilities/Liabilities" + type + "ListVi.rpt");
				}
				
				ReportDocument report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);

				// set parameters
				// set parameter currentDate
				string dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				string dateToFormated = dateTo.ToString("dd/MM/yyyy");
				CultureInfo cul;
				string datePeriod = "";
				if (language == "vi")
				{
					cul = CultureInfo.GetCultureInfo("vi-VN");
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == "jp")
				{
					cul = CultureInfo.GetCultureInfo("ja-JP");
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
					dateFromFormated = dateFrom.Year + "年" + dateFrom.Month + "月" + dateFrom.Day + "日";
					dateToFormated = dateTo.Year + "年" + dateTo.Month + "月" + dateTo.Day + "日";
					datePeriod = string.Format("{0}から{1}まで", dateFromFormated, dateToFormated);
				}
				else 
				{
					cul = CultureInfo.GetCultureInfo("en-US");
					report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);
					dateFromFormated = dateFrom.ToString("MM/dd/yyyy");
					dateToFormated = dateTo.ToString("MM/dd/yyyy");
					datePeriod = string.Format("Period {0} to {1}", dateFromFormated, dateToFormated);
				}

				report.SetParameterValue("datePeriod", datePeriod);

				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);
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
		//public static Stream GeneralExec(Dataset.Liabilities.LiabilitiesDetail.LiabilitiesDetailDataTable dataTable, int language,
		//	DateTime dateFrom, DateTime dateTo, Dictionary<string, string> textResources)
		//{
		//	try
		//	{
		//		var strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Liabilities/LiabilitiesDetail.rpt");

		//		var report = new ReportDocument();
		//		report.Load(strPath);
		//		report.Database.Tables[0].SetDataSource((DataTable)dataTable);

		//		// set parameters
		//		var dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
		//		var dateToFormated = dateTo.ToString("dd/MM/yyyy");
		//		var datePeriod = "";
		//		var hdLiabilitiesDetail = textResources["TLTLIABILITIESREPORT"].ToUpper();
		//		var hdDriverName = textResources["LBLREPORTIDRIVER"];
		//		var hdTotalPreviousBalance = textResources["RPLBLOPENINGBALANCE"];
		//		var hdTotalAdvanceAmount = textResources["TLTTOTALADVANCEREPORT"];
		//		var hdTotalPaymentAmount = textResources["TLTTOTALPAYMENTREPORT"];
		//		var hdSumAmount = textResources["TLTTOTALSUMREPORT"];
		//		var hdAdvanceCol = textResources["LBLADVANCELIABILITIES"];
		//		var hdPaymentCol = textResources["LBLEXPENSELIABILITIES"];
		//		var hdLiabilitiesD = textResources["RPHDDATE"];
		//		var hdAmount = textResources["LBLAMOUNTREPORT"];
		//		var hdLiabilitiesContent = textResources["RPHDCONTENT"];
		//		var hdTotal = textResources["TLTLIABILITIESTOTALREPORT"];

		//		if (language == 1)
		//		{
		//			report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
		//			datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
		//		}
		//		else if (language == 2)
		//		{
		//			var cul = CultureInfo.GetCultureInfo("en-US");
		//			report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);
		//			dateFromFormated = dateFrom.ToString("MM/dd/yyyy");
		//			dateToFormated = dateTo.ToString("MM/dd/yyyy");
		//			datePeriod = string.Format("Period {0} to {1}", dateFromFormated, dateToFormated);
		//		}
		//		else
		//		{
		//			report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
		//			dateFromFormated = dateFrom.Year + "年" + dateFrom.Month + "月" + dateFrom.Day + "日";
		//			dateToFormated = dateTo.Year + "年" + dateTo.Month + "月" + dateTo.Day + "日";
		//			datePeriod = string.Format("{0}から{1}まで", dateFromFormated, dateToFormated);
		//		}

		//		report.SetParameterValue("datePeriod", datePeriod);
		//		report.SetParameterValue("hdLiabilitiesDetail", hdLiabilitiesDetail);
		//		report.SetParameterValue("hdDriverName", hdDriverName);
		//		report.SetParameterValue("hdTotalPreviousBalance", hdTotalPreviousBalance);
		//		report.SetParameterValue("hdTotalAdvanceAmount", hdTotalAdvanceAmount);
		//		report.SetParameterValue("hdTotalPaymentAmount", hdTotalPaymentAmount);
		//		report.SetParameterValue("hdSumAmount", hdSumAmount);
		//		report.SetParameterValue("hdAdvanceCol", hdAdvanceCol);
		//		report.SetParameterValue("hdPaymentCol", hdPaymentCol);
		//		report.SetParameterValue("hdLiabilitiesD", hdLiabilitiesD);
		//		report.SetParameterValue("hdAmount", hdAmount);
		//		report.SetParameterValue("hdLiabilitiesContent", hdLiabilitiesContent);
		//		report.SetParameterValue("hdTotal", hdTotal);


		//		return report.ExportToStream(ExportFormatType.PortableDocFormat);
		//	}
		//	catch (NullReferenceException)
		//	{
		//		throw new NullReferenceException();
		//	}
		//}

		public static Stream LiabilitiesPaymentExec(Dataset.Liabilities.LiabilitiesPayment.LiabilitiesPaymentDataTable dataTable, int language,
			DateTime dateFrom, DateTime dateTo, Dictionary<string, string> textResources,
			string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				var strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Liabilities/LiabilitiesPayment.rpt");

				var report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)dataTable);

				// set parameters
				var dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				var dateToFormated = dateTo.ToString("dd/MM/yyyy");
				var datePeriod = "";
				var hdLiabilitiesPayment = textResources["TLTLIABILITIESPAYMENTREPORT"].ToUpper();
				var hdDriverName = textResources["LBLREPORTIDRIVER"];
				var hdTotal = textResources["TLTLIABILITIESTOTALREPORT"];
				var hdDate = textResources["RPHDDATE"];
				var hdTruckNo = textResources["TLTTRUCK"];
				var hdRoomoc = textResources["TLTTRAILER"];
				var hdConNo = textResources["LBLCONTAINERNO"];
				var hdCustomer = textResources["LBLCUSTOMER"];
				var hdLocation = textResources["TLTLOCATION"];
				var hdContent = textResources["LBLITEM"];
				var hdAmount = textResources["LBLAMOUNTREPORT"];
				var hdConSize = textResources["LBLCONTAINERSIZE"];
				var hdDetail = textResources["LBLDETAIL"]; ;
				var hdReceipt = textResources["LBLRECEIPTNO"];
				var hdLiabilitiesContent = textResources["RPHDCONTENT"];

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
				report.SetParameterValue("hdLiabilitiesPayment", hdLiabilitiesPayment);
				report.SetParameterValue("hdDriverName", hdDriverName);
				report.SetParameterValue("hdDate", hdDate);
				report.SetParameterValue("hdTruckNo", hdTruckNo);
				report.SetParameterValue("hdRomooc", hdRoomoc);
				report.SetParameterValue("hdConNo", hdConNo);
				report.SetParameterValue("hdCustomer", hdCustomer);
				report.SetParameterValue("hdLocation", hdLocation);
				report.SetParameterValue("hdContent", hdContent);
				report.SetParameterValue("hdAmount", hdAmount);
				report.SetParameterValue("hdConSize", hdConSize);
				report.SetParameterValue("hdTotal", hdTotal);
				report.SetParameterValue("hdDetail", hdDetail);
				report.SetParameterValue("hdReceipt", hdReceipt);
				report.SetParameterValue("hdLiabilitiesContent", hdLiabilitiesContent);
				report.SetParameterValue("hdLiabilitiesAmount", hdAmount);

				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);
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
