using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalReport.Dataset.Maintenance;
using CrystalReport.Dataset.Order;
using System;
using System.Data;
using System.IO;
using System.Globalization;

namespace CrystalReport.Service.Maintenance
{
	public class ExportPdf
	{
		public static Stream Exec(MaintenanceList.MaintenanceDataTable datatable,
								  int language,
								  DateTime fromDate,
								  DateTime toDate, string depList, string truckList, string trailerList,
								  string companyName, string companyAddress, string fileName, string user
								 )
		{
			try
			{
				
				string strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Maintenance/MaintenanceListEn.rpt");
				if (language == 3)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Maintenance/MaintenanceListJp.rpt");
				}
				else if (language == 1)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Maintenance/MaintenanceListVi.rpt");
				}

				ReportDocument report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables["Maintenance"].SetDataSource((DataTable)datatable);
				// set parameters
				// set parameter currentDate
				string dateFromFormated = fromDate.ToString("dd/MM/yyyy");
				string dateToFormated = toDate.ToString("dd/MM/yyyy");
				string datePeriod = "";
				CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
				if (language == 1)
				{
					string curdate = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
					string curdateMessage = curdate;
					report.SetParameterValue("currentDate", curdateMessage);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == 2)
				{
					cul = CultureInfo.GetCultureInfo("en-US");
					string curdate = cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year;
					string curdateMessage = curdate;
					report.SetParameterValue("currentDate", curdateMessage);
					dateFromFormated = fromDate.ToString("MM/dd/yyyy");
					dateToFormated = toDate.ToString("MM/dd/yyyy");
					datePeriod = string.Format("Period {0} to {1}", dateFromFormated, dateToFormated);
				}
				else
				{
					cul = CultureInfo.GetCultureInfo("ja-JP");
					string curdate = DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日";
					string curdateMessage = curdate;
					report.SetParameterValue("currentDate", curdateMessage);
					dateFromFormated = fromDate.Year + "年" + fromDate.Month + "月" + fromDate.Day + "日";
					dateToFormated = toDate.Year + "年" + toDate.Month + "月" + toDate.Day + "日";
					datePeriod = string.Format("{0}から{1}まで", dateFromFormated, dateToFormated);
				}

				report.SetParameterValue("depList", depList);
				report.SetParameterValue("truckList", truckList);
				report.SetParameterValue("trailerList", trailerList);
				report.SetParameterValue("datePeriod", datePeriod);
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
	}
}
