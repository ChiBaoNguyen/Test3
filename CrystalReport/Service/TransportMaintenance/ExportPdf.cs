using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalReport.Dataset.Maintenance;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CrystalReport.Service.TransportMaintenance
{
	public class ExportPdf
	{
		public static Stream Exec(MaintenanceList.TransportDataTable datatable, int language, int year, 
								  string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/TransportMaintenance/TransportMaintenanceList.rpt");
				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);
				// set parameter currentDate
				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
				}
				else if (language == 2)
				{
					report.SetParameterValue("currentDate", DateTime.Now.ToString("MMMM dd, yyyy"));
				}
				else
				{
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
				}
				report.SetParameterValue("year", year);
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