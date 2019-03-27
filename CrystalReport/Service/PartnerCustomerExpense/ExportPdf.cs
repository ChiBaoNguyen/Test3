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

namespace CrystalReport.Service.PartnerCustomerExpense
{
    public class ExportPdf
    {
		public static Stream Exec(Dataset.PartnerCustomer.PartnerCustomerExpenseList.PartnerCustomerExpenseListDataTable datatable, int language, string monthYear)
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/PartnerCustomerExpense/PartnerCustomerExpenseListEn.rpt");
				if (language == 3)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/PartnerCustomerExpense/PartnerCustomerExpenseListJp.rpt");
				}
				else if (language == 1)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/PartnerCustomerExpense/PartnerCustomerExpenseListVi.rpt");
				}

				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);

				// set parameter transportM
				report.SetParameterValue("transportM", monthYear);

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
