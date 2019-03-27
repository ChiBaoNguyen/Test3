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

namespace CrystalReport.Service.TransportExpense
{
    public class ExportPdf
    {
		public static Stream Exec(Dataset.TransportExpense.TransportExpense.TransportExpenseDataTable datatable,
								  int language,
								  string fromDate,
								  string toDate,
								  string category
								 )
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/TransportExpense/TransportExpenseListEn.rpt");
				if (language == 3)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/TransportExpense/TransportExpenseListJp.rpt");
				}
				else if (language == 1)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/TransportExpense/TransportExpenseListVi.rpt");
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
				report.SetParameterValue("category", category);

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

		public static Stream Exec(Dataset.TransportExpense.TransportExpenseList.TransportExpenseListDataTable datatable,
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

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/TransportExpense/TransportExpenseList.rpt");

				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);
				// set period time
				if (language == 1)
				{
					report.SetParameterValue("lblFromToDate", "Từ ngày " + fromDate + " đến ngày " + toDate);
				}
				else if (language == 2)
				{
					CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
					report.SetParameterValue("lblFromToDate", "Period " + fromDate + " to " + toDate);
				}
				else
				{
					report.SetParameterValue("lblFromToDate", fromDate + "から" + toDate + "まで");
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
				report.SetParameterValue("lblCompanyName", companyName ?? "");
				report.SetParameterValue("lblCompanyAddress", companyAddress ?? "");
				report.SetParameterValue("lblCompanyImageUrl", path);
				report.SetParameterValue("user", user);

				report.SetParameterValue("tltTransportExpenseList", dicLanguage["TLTTRANSPORTEXPENSEREPORT"]);
				//report.SetParameterValue("hdOrderNoType", dicLanguage["RPHDORDERNO"] + " (" + dicLanguage["RPHDORDERTYPE"] + ")");
				report.SetParameterValue("hdOrderNoType", dicLanguage["LBLJOBNO"]);
				report.SetParameterValue("hdCustomer", dicLanguage["LBLCUSTOMERREPORT"]);
				report.SetParameterValue("hdBLBK", "BL/BK");

				report.SetParameterValue("hdConNoSize", dicLanguage["RPHDCONTNO"] + " (" + dicLanguage["RPHDCONTSIZE"] + ")");
				report.SetParameterValue("hdRoute", dicLanguage["RPHDROUTE"]);
				report.SetParameterValue("hdRegisterNo", dicLanguage["LBLTREGISTEREDNO"]);
				report.SetParameterValue("hdDriverPartner", dicLanguage["LBLREPORTIDRIVER"] + " (" + dicLanguage["RPHDPARTNERN"] + ")");
				report.SetParameterValue("hdIncludedExpense", dicLanguage["RPHDISINCLUDED"]);
				report.SetParameterValue("hdAllowance", dicLanguage["RPHDDRIVINGALLOWANCE"]);
				report.SetParameterValue("hdPartnerTitle", dicLanguage["RPHDPARTNER"]);
				report.SetParameterValue("hdPartnerExpense", dicLanguage["RPHDPARTNERFEE"]);
				report.SetParameterValue("hdPartnerTax", dicLanguage["RPHDTAXAMOUNT"]);
				report.SetParameterValue("hdPartnerAdvancePayment", dicLanguage["LBLPAYONBEHALF"]);
				report.SetParameterValue("hdPartnerSurcharge", dicLanguage["RPHDTOTALSURCHARGE"]);
				report.SetParameterValue("hdPartnerExpenseTotal", dicLanguage["RPHDTOLPARTNERINCLUDETAX"]);
				report.SetParameterValue("hdPartnerPayment", dicLanguage["RPHDTOTALPARTNERAMOUNT"]);
				report.SetParameterValue("hdTransportExpense", dicLanguage["RPHDAMOUNT"]);
				report.SetParameterValue("hdSurcharge", dicLanguage["RPHDTOTALSURCHARGE"]);
				report.SetParameterValue("hdTax", dicLanguage["RPHDTAXAMOUNT"]);
				report.SetParameterValue("hdTransportExpenseTotal", dicLanguage["RPHDTOTRANSPORTFEE"]);
				report.SetParameterValue("hdAdvancePayment", dicLanguage["LBLPAYONBEHALF"]);
				report.SetParameterValue("hdExpenseTotal", dicLanguage["RPHDTOLAMOUNT"]);
				report.SetParameterValue("tltPlus", dicLanguage["RPLBLTOTAL"]);
				report.SetParameterValue("hdInternalTitle", dicLanguage["LBLINTERNALREPORT"]);
				report.SetParameterValue("hdCustomerTitle", dicLanguage["LBLCUSTOMERREPORT"]);
				report.SetParameterValue("hdTransportFee", dicLanguage["RPHDAMOUNT"]);
				report.SetParameterValue("hdTotalTransportFee", dicLanguage["RPHDTOTRANSPORTFEE"]);

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
