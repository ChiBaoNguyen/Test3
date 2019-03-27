using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalReport.Dataset.Dispatch;
using System;
using System.Data;
using System.IO;
using System.Web.Hosting;
using System.Globalization;


namespace CrystalReport.Service.FuelConsumption
{
	public class ExportPdf
	{
		public static Stream Exec(Dataset.FuelConsumption.FuelConsumption.FuelConsumptionDataTable datatable, string language, DateTime dateFrom, DateTime dateTo,
									string companyName, string companyAddress,string fileName)
		{
			try
			{
				string strPath = HttpContext.Current.Request.MapPath("~/FileRpt/FuelConsumption/FuelConsumptionListEn.rpt");
				if (language == "jp")
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/FuelConsumption/FuelConsumptionListJp.rpt");
				}
				else if (language == "vi")
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/FuelConsumption/FuelConsumptionListVi.rpt");
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

					report.SetParameterValue("companyName", companyName ?? "");
					report.SetParameterValue("companyAddress", companyAddress ?? "");
					var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);
					report.SetParameterValue("imageUrl", path);
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
			catch (Exception ex)
			{
				var a = ex;
				return null;
			}
		}
	}
}
