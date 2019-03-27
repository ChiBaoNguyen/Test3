using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalReport.Dataset.Order;
using System;
using System.Data;
using System.IO;
using System.Web.Hosting;
using System.Globalization;

namespace CrystalReport.Service.Order
{
	public class ExportPdf
	{
		public static Stream Exec(OrderList.OrderListDataTable datatable,
								  int language,
								  string totalOrderNo,
								  string totalSize,
								  string totalContainerSize,
								  string totalUnitPrice,
								  string fromDate,
								  string toDate
								 )
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Order/OrderListEn.rpt");
				if (language == 3)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Order/OrderListJp.rpt");
				}
				else if (language == 1)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Order/OrderListVi.rpt");
				}

				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);
				// set period time
				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
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

				report.SetParameterValue("totalOrderNo", totalOrderNo);
				report.SetParameterValue("totalSize", totalSize);
				report.SetParameterValue("totalContainerSize", totalContainerSize);
				report.SetParameterValue("totalUnitPrice", totalUnitPrice);

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

		public static Stream Exec(OrderList.OrderGeneralDataTable datatable,
								  int language,
								  string fromDate,
								  string toDate,
								  string companyName,
								  string companyAddress,
								  string fileName,
								  string user, 
								  Dictionary<string, string> dicLanguage
								 )
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Order/OrderGeneralList.rpt");

				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);
				// set period time
				if (language == 1)
				{
					//report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					report.SetParameterValue("period", "Từ ngày " + fromDate + " đến ngày " + toDate);
				}
				else if (language == 2)
				{
					CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
					//report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);
					report.SetParameterValue("period", "Period " + fromDate + " to " + toDate);
				}
				else
				{
					//report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
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
				// set datatable images
				var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);
				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				report.SetParameterValue("imageUrl", path);
				report.SetParameterValue("user", user);

				report.SetParameterValue("hdReport", dicLanguage["TLTORDERREPORT"]);
				report.SetParameterValue("hdOrderNo", dicLanguage["LBLORDERNODISPATCH"]);
				report.SetParameterValue("hdOrderType",  dicLanguage["LBLORDERTYPEREPORT"]);
				report.SetParameterValue("hdOrderD", dicLanguage["LBLORDERDATEDISPATCH"]);
				report.SetParameterValue("hdCustomer", dicLanguage["LBLCUSTOMERREPORT"]);
				report.SetParameterValue("hdBKBL", dicLanguage["LBLBLBKREPORT"]);
				report.SetParameterValue("hdJobNo",  dicLanguage["LBLJOBNO"]);
				report.SetParameterValue("hdShippingCompany", dicLanguage["LBLSHIPPINGAGENCY"]);
				report.SetParameterValue("hdVessel", dicLanguage["MNUVESSEL"]);
				report.SetParameterValue("hdRoute", dicLanguage["TLTROUTE"]);
				report.SetParameterValue("hdContainer", dicLanguage["RPHDCONTSIZE"]);
				report.SetParameterValue("hdNetWeight", dicLanguage["LBLNETWEIGHT"]);
				report.SetParameterValue("hdTon",  dicLanguage["LBLTON"]);
				report.SetParameterValue("hdUnitPrice", dicLanguage["LBLTEUNITPRICE"]);
				report.SetParameterValue("hdTotalPrice", dicLanguage["LBLTOL"]);
				report.SetParameterValue("lbSum", dicLanguage["RPLBLTOTAL"]);

				report.SetParameterValue("ftPrintTime", dicLanguage["RPFTPRINTTIME"]);
				report.SetParameterValue("ftPrintBy", dicLanguage["RPFTPRINTBY"]);
				report.SetParameterValue("ftPage", dicLanguage["RPFTPAGE"]);
				report.SetParameterValue("ftCreator", dicLanguage["RPFTCREATOR"]);

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
