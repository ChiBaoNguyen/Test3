using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Data;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace CrystalReport.Service.PartnerBalance
{
	public class ExportPdf
	{
		public static Stream Exec(Dataset.PartnerBalance.PartnerBalance.PartnerBalanceDataTable datatable,
								  int language, string monthYear, Dictionary<string, string> textResources)
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/PartnerBalance/PartnerBalanceList.rpt");

				// load report
				report = new ReportDocument();
				report.Load(strPath);

				report.Database.Tables[0].SetDataSource((DataTable)datatable);

				// set parameters
				var hdPartnerBalance = textResources["TLTPARTNERBALANCEREPORT"];
				var hdPartnerN = textResources["RPHDPARTNERN"];
				var hdRevenue = textResources["RPHDREVENUE"];
				var hdExpense = textResources["RPHDEXPENSE"];
				var hdProfit = textResources["RPHDPROFIT"];
				var hdName = textResources["LBLEXPENSEITEM"];
				var hdAmount = textResources["LBLAMOUNTREPORT"];
				var lblTotal = textResources["RPLBLTOTAL"];

				report.SetParameterValue("datePeriod", monthYear);
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
				report.SetParameterValue("hdPartnerBalance", hdPartnerBalance.ToUpper());
				report.SetParameterValue("hdPartnerN", hdPartnerN);
				report.SetParameterValue("hdRevenue", hdRevenue);
				report.SetParameterValue("hdExpense", hdExpense);
				report.SetParameterValue("hdProfit", hdProfit);
				report.SetParameterValue("hdName", hdName);
				report.SetParameterValue("hdAmount", hdAmount);
				report.SetParameterValue("lblTotal", lblTotal.ToUpper());

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