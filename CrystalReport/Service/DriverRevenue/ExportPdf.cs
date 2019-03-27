using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CrystalReport.Service.DriverRevenue
{
	public class ExportPdf
	{
		public static Stream Exec(Dataset.DriverRevenue.DriverRevenue.DriverRevenueDataTable datatable, int language, DateTime dateFrom, DateTime dateTo)
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverRevenue/DriverRevenueListEn.rpt");
				if (language == 3)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverRevenue/DriverRevenueListJp.rpt");
				}
				else if (language == 1)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverRevenue/DriverRevenueListVi.rpt");
				}

				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);

				// set parameters
				// set parameter currentDate
				string dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				string dateToFormated = dateTo.ToString("dd/MM/yyyy");
				string datePeriod = "";
				CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == 2)
				{
					cul = CultureInfo.GetCultureInfo("en-US");
					report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);
					dateFromFormated = dateFrom.ToString("MM/dd/yyyy");
					dateToFormated = dateTo.ToString("MM/dd/yyyy");
					datePeriod = string.Format("Period {0} to {1}", dateFromFormated, dateToFormated);
				}
				else
				{
					cul = CultureInfo.GetCultureInfo("ja-JP");
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
					dateFromFormated = dateFrom.Year + "年" + dateFrom.Month + "月" + dateFrom.Day + "日";
					dateToFormated = dateTo.Year + "年" + dateTo.Month + "月" + dateTo.Day + "日";
					datePeriod = string.Format("{0}から{1}まで", dateFromFormated, dateToFormated);
				}

				report.SetParameterValue("datePeriod", datePeriod);

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
		public static Stream ExecGeneralReport(Dataset.DriverRevenue.DriverRevenue.DriverRevenueDataTable datatable, int language, DateTime dateFrom, DateTime dateTo
			,int total20HC, int total40HC, int total45HC, decimal totalAmount)
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverRevenue/DriverRevenueGeneralListEn.rpt");
				if (language == 3)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverRevenue/DriverRevenueGeneralListJp.rpt");
				}
				else if (language == 1)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverRevenue/DriverRevenueGeneralListVi.rpt");
				}

				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);

				// set parameters
				// set parameter currentDate
				string dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				string dateToFormated = dateTo.ToString("dd/MM/yyyy");
				string datePeriod = "";
				CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == 2)
				{
					cul = CultureInfo.GetCultureInfo("en-US");
					report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);
					dateFromFormated = dateFrom.ToString("MM/dd/yyyy");
					dateToFormated = dateTo.ToString("MM/dd/yyyy");
					datePeriod = string.Format("Period {0} to {1}", dateFromFormated, dateToFormated);
				}
				else
				{
					cul = CultureInfo.GetCultureInfo("ja-JP");
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
					dateFromFormated = dateFrom.Year + "年" + dateFrom.Month + "月" + dateFrom.Day + "日";
					dateToFormated = dateTo.Year + "年" + dateTo.Month + "月" + dateTo.Day + "日";
					datePeriod = string.Format("{0}から{1}まで", dateFromFormated, dateToFormated);
				}

				report.SetParameterValue("datePeriod", datePeriod);
				report.SetParameterValue("total20HC", total20HC);
				report.SetParameterValue("total40HC", total40HC);
				report.SetParameterValue("total45HC", total45HC);
				report.SetParameterValue("totalAmount", totalAmount);

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
