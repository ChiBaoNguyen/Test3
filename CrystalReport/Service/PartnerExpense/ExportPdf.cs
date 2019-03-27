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

namespace CrystalReport.Service.PartnerExpense
{
    public class ExportPdf
    {
		public static Stream Exec(Dataset.PartnerExpense.PartnerExpenseList.PartnerExpenseListDataTable datatable,
								  int language,
								  string monthYear
								  )
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/PartnerExpense/PartnerExpenseListEn.rpt");
				if (language == 3)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/PartnerExpense/PartnerExpenseListJp.rpt");
				}
				else if (language == 1)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/PartnerExpense/PartnerExpenseListVi.rpt");
				}

				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);

				// set parameters
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
				report.SetParameterValue("monthYear", monthYear);

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
		public static Stream Exec(Dataset.PartnerExpense.PartnerExpenseDetail.PartnerExpenseDetailDataTable datatable,
								  int language, string monthYear, Dictionary<string, string> textResources,
			string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/PartnerExpense/PartnerExpenseDetail.rpt");

				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);

				// set parameters
				//var datePeriod = "";
				var hdPartnerExpenseDetail = textResources["RPHDPARTNEREXPENSEDETAIL"];
				var lblPartnerN = textResources["RPLBLPARTNERN"];
				var lblPartnerAddress = textResources["RPLBLPARTNERADDRESS"];
				var hdDate = textResources["RPHDDATE"];
				var hdCustomer = textResources["RPHDCUSTOMERN"];
				var hdContNo = textResources["RPHDCONTNO"];
				var hdContSize = textResources["RPHDCONTSIZE"];

				var hdOwe = textResources["RPHDTOTALEXPENSE"];
				var hdPartnerFee = textResources["LBLTRANSPORTFEEREPORT"];
				var hdSurcharge = textResources["RPHDTOTALSURCHARGE"];
				var hdTaxAmount = textResources["RPHDTAXAMOUNT"];
				var hdSum = textResources["LBLTOTALREPORT"];

				var hdExpense = textResources["LBLPAYONBEHALF"];
				var hdExpenseN = textResources["RPHDCONTENT"];
				var hdAmount = textResources["LBLAMOUNTREPORT"];
				var hdDescription = textResources["LBLINVOICENOREPORT"];

				var hdTotal = textResources["LBLTOTALAMOUNTREPORT"];
				var lblTotal = textResources["RPLBLTOTAL"];
				//var hdDiscount = textResources["RPHDCUSTOMERDISCOUNT"];

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
				report.SetParameterValue("datePeriod", monthYear);
				report.SetParameterValue("hdPartnerExpenseDetail", hdPartnerExpenseDetail);
				report.SetParameterValue("lblPartnerN", lblPartnerN);
				report.SetParameterValue("lblPartnerAddress", lblPartnerAddress);
				report.SetParameterValue("hdDate", hdDate);
				report.SetParameterValue("lblTotal", lblTotal);
				report.SetParameterValue("hdCustomer", hdCustomer);
				report.SetParameterValue("hdOwe", hdOwe);
				report.SetParameterValue("hdPartnerFee", hdPartnerFee);
				report.SetParameterValue("hdContNo", hdContNo);
				report.SetParameterValue("hdExpense", hdExpense);
				report.SetParameterValue("hdSurcharge", hdSurcharge);
				report.SetParameterValue("hdTaxAmount", hdTaxAmount);
				report.SetParameterValue("hdSum", hdSum);
				report.SetParameterValue("hdDescription", hdDescription);
				report.SetParameterValue("hdTotal", hdTotal);
				report.SetParameterValue("hdContSize", hdContSize);
				report.SetParameterValue("hdExpenseN", hdExpenseN);
				report.SetParameterValue("hdAmount", hdAmount);

				// set datatable images
				var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);
				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				report.SetParameterValue("imageUrl", path);
				report.SetParameterValue("user", user);

				report.SetParameterValue("ftPrintTime", textResources["RPFTPRINTTIME"]);
				report.SetParameterValue("ftPrintBy", textResources["RPFTPRINTBY"]);
				report.SetParameterValue("ftPage", textResources["RPFTPAGE"]);
				report.SetParameterValue("ftCreator", textResources["RPFTCREATOR"]);

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
