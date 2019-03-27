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
//using Website.Utilities;

namespace CrystalReport.Service.CustomerBalance
{
    public class ExportPdf
    {
		public static Stream Exec(Dataset.CustomerBalance.CustomerBalance.CustomerBalanceDataTable datatable,
											int language, string monthYear, string companyName, string companyAddress, string fileName, string user, Dictionary<string, string> dicLanguage)
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/CustomerBalance/CustomerBalanceListVi.rpt");

				// load report
				report = new ReportDocument();
				report.Load(strPath);

				report.Database.Tables[0].SetDataSource((DataTable)datatable);

				var tittle = dicLanguage["MNUCUSTOMERBALANCEREPORT"];
				report.SetParameterValue("tittle", tittle);
				var customer = dicLanguage["LBLCUSTOMER"];
				report.SetParameterValue("customer", customer);
				var income = dicLanguage["LBLINCOME"];
				report.SetParameterValue("income", income);
				var expense = dicLanguage["LBLEXPENSERP"];
				report.SetParameterValue("expense", expense);
				var profit = dicLanguage["LBLPROFIT"];
				report.SetParameterValue("profit", profit);
				var item = dicLanguage["LBLITEMRP"];
				report.SetParameterValue("item", item);
				var amount = dicLanguage["LBLAMOUNTMONEY"];
				report.SetParameterValue("amount", amount);
				var entry = dicLanguage["LBLENTRYCLERKN"];
				report.SetParameterValue("entry", entry);
				var page = dicLanguage["RPFTPAGE"];
				report.SetParameterValue("page", page);
				var printby = dicLanguage["RPFTPRINTBY"];
				report.SetParameterValue("printby", printby);
				var printtime = dicLanguage["RPFTPRINTTIME"];
				report.SetParameterValue("printtime", printtime);
				var total = dicLanguage["LBLDISPATCHTOTALRP"];
				report.SetParameterValue("total", total);
				// set parameters
				
				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					report.SetParameterValue("datePeriod", "Tháng " + monthYear);

				}
				else if (language == 2)
				{
					CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
					report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);
					report.SetParameterValue("datePeriod", monthYear);
				}
				else
				{
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
					report.SetParameterValue("datePeriod", monthYear);
				}
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