using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Data;
using System.IO;
using System.Web.Hosting;
using System.Globalization;

namespace CrystalReport.Service.DriverDispatch
{
    public class ExportPdf
    {
		public static Stream Exec(Dataset.DriverDispatch.DriverDispatch.DriverDispatchDataTable datatable,
								  int language,
								  string total,
								  string fromDate,
								  string toDate,
								  string category,
								  string companyName,
								  string companyAddress,
								  string fileName,
								  string user
								 )
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverDispatch/DriverDispatchListEn.rpt");
				if (language == 3)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverDispatch/DriverDispatchListJp.rpt");
				}
				else if (language == 1)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverDispatch/DriverDispatchListVi.rpt");
				}

				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);
				// set period time
				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					if (fromDate.Equals(toDate))
						report.SetParameterValue("period", "Ngày " + fromDate);
					else
						report.SetParameterValue("period", "Từ ngày " + fromDate + " đến ngày " + toDate);
				}
				else if (language == 2)
				{
					CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
					report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);
					report.SetParameterValue("period", "Period " + fromDate + " to " + toDate);
				}
				else
				{
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
					report.SetParameterValue("period", fromDate + "から" + toDate + "まで");
				}
				// set parameter currentDate
				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
				}
				else if (language == 2)
				{
					CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
					report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);
				}
				else
				{
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
				}

				report.SetParameterValue("total", total);
				report.SetParameterValue("category", category);
				//report.SetParameterValue("driverName", driverN);
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
