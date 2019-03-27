using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Data;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace CrystalReport.Service.UseFuel
{
	public class ExportPdf
	{
		public static Stream Exec(CrystalReport.Dataset.FuelConsumption.FuelConsumptionDetail.UseFuelDetailDataTable datatable, string language, DateTime dateFrom, DateTime dateTo,
									string companyName, string companyAddress, string fileName, string user, Dictionary<string, string> dicLanguage)
		{
			string strPath = HttpContext.Current.Request.MapPath("~/FileRpt/UseFuel/UseFuelList.rpt");

			ReportDocument report = new ReportDocument();
			report.Load(strPath);
			report.Database.Tables[0].SetDataSource((DataTable)datatable);

			// set parameters
			var tittle = dicLanguage["FTRUSEFUELMASTER"];
			report.SetParameterValue("tittle", tittle);
			var total = dicLanguage["RPLBLTOTAL"];
			report.SetParameterValue("total", total);
			var registeredno = dicLanguage["LBLTRUCKNODISPATCHRP"];
			report.SetParameterValue("registeredno", registeredno);
			var openingperiod = dicLanguage["LBLFUELOPENINGPERRIOD"];
			report.SetParameterValue("openingperiod", openingperiod);
			var midperiod = dicLanguage["LBLUSEFUEL"];
			report.SetParameterValue("midperiod", midperiod);
			var totalfuel = dicLanguage["LBLESTIMATED"];
			report.SetParameterValue("totalfuel", totalfuel);
			var closingperiod = dicLanguage["LBLFUELCLOSINGPERRIOD"];
			report.SetParameterValue("closingperiod", closingperiod);
			var page = dicLanguage["RPFTPAGE"];
			report.SetParameterValue("page", page);
			var printby = dicLanguage["RPFTPRINTBY"];
			report.SetParameterValue("printby", printby);
			var printtime = dicLanguage["RPFTPRINTTIME"];
			report.SetParameterValue("printtime", printtime);
			var drivern = dicLanguage["LBLDRIVERREPORT"];
			report.SetParameterValue("drivern", drivern);

			// set parameter currentDate
			string dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
			string dateToFormated = dateTo.ToString("dd/MM/yyyy");
			CultureInfo cul;
			string datePeriod = "";
			report.SetParameterValue("companyName", companyName ?? "");
			report.SetParameterValue("companyAddress", companyAddress ?? "");
			var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);
			report.SetParameterValue("imageUrl", path);
			report.SetParameterValue("user", user);
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

			Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
			report.Close();
			report.Dispose();
			GC.Collect();
			return stream;
		}
	}
}
