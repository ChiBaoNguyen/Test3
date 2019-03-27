using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Data;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace CrystalReport.Service.FuelConsumptionDetail
{
	public class ExportPdf
	{
		public static Stream Exec(CrystalReport.Dataset.FuelConsumption.FuelConsumptionDetail.FuelConsumptionDetailDataTable datatable, string language, DateTime dateFrom, DateTime dateTo,
									string companyName, string companyAddress, string fileName, string user, Dictionary<string, string> dicLanguage)
		{
			string strPath = HttpContext.Current.Request.MapPath("~/FileRpt/FuelConsumption/FuelConsumptionDetailListVi.rpt");

			ReportDocument report = new ReportDocument();
			report.Load(strPath);
			report.Database.Tables[0].SetDataSource((DataTable)datatable);

			// set parameters
			var tittle = dicLanguage["MNUFUELCONSUMPTIONDETAILREPORT"];
			report.SetParameterValue("tittle", tittle);
			var intructionno = dicLanguage["LBLINSTRUCTIONORP"];
			report.SetParameterValue("intructionno", intructionno);
			var transportdate = dicLanguage["LBLTRANSPORTDATEDISPATCHRP"];
			report.SetParameterValue("transportdate", transportdate);
			var truckno = dicLanguage["LBLTRUCKNODISPATCHRP"];
			report.SetParameterValue("truckno", truckno);
			var driver = dicLanguage["LBLDRIVER"];
			report.SetParameterValue("driver", driver);
			var router = dicLanguage["TLTROUTERP"];
			report.SetParameterValue("router", router);
			var contnumber = dicLanguage["LBLCONTNUMBER"];
			report.SetParameterValue("contnumber", contnumber);
			var replacement = dicLanguage["LBLREPLACEMENTINTERVALRP"];
			report.SetParameterValue("replacement", replacement);
			var actual = dicLanguage["HDACTUAL1"];
			report.SetParameterValue("actual", actual);
			var difference = dicLanguage["LBLDIFFERENCE"];
			report.SetParameterValue("difference", difference);
			var km = dicLanguage["LBLKMS"];
			report.SetParameterValue("km", km);
			var lit = dicLanguage["LBLLITS"];
			report.SetParameterValue("lit", lit);
			var total = dicLanguage["LBLDISPATCHTOTALRP"];
			report.SetParameterValue("total", total);
			var entryclerkn = dicLanguage["LBLENTRYCLERKN"];
			report.SetParameterValue("entryclerkn", entryclerkn);
			var page = dicLanguage["RPFTPAGE"];
			report.SetParameterValue("page", page);
			var printby = dicLanguage["RPFTPRINTBY"];
			report.SetParameterValue("printby", printby);
			var printtime = dicLanguage["RPFTPRINTTIME"];
			report.SetParameterValue("printtime", printtime);
			var dispatchi = dicLanguage["LBLDISPATCHI"];
			report.SetParameterValue("dispatchi", dispatchi);
			var loss = dicLanguage["LBLLOSS"];
			report.SetParameterValue("loss", loss);
			
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
