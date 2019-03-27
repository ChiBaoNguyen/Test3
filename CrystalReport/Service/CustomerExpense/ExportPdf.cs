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
using Website.Utilities;

namespace CrystalReport.Service.CustomerExpense
{
    public class ExportPdf
    {
		public static Stream Exec(Dataset.CustomerExpense.CustomerExpense.CustomerExpenseDataTable datatable,
								  int language,
								  string companyName,
								  string companyAddress,
								  string companyTaxCode,
								  string monthYear,
								  string fileName,
								  Dictionary<string, string> dicLanguage
								  )
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/CustomerExpense/CustomerExpenseListVi.rpt");

				// load report
				report = new ReportDocument();
				report.Load(strPath);

				// set datatable images
				var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);

				report.Database.Tables[0].SetDataSource((DataTable)datatable);

				// set parameters
				var tittle = dicLanguage["MNUCUSTOMEREXPENSEREPORT"];
				var transportfee = dicLanguage["LBLTRANSPORTFEEREPORT"];
				var taxamount = dicLanguage["LBLTAXAMOUNTREPORT"];
				var aftertax = dicLanguage["LBLTRANSPORTFEEAFTERTAXRP"];
				var payonbehalf = dicLanguage["LBLPAYONBEHALFRP"];
				var aftertaxamount = dicLanguage["LBLAFTERTAXAMOUNTRP"];
				var invoiceattached = dicLanguage["LBLINVOICEATTACHED"];
				var customer = dicLanguage["LBLCUSTOMER"];
				var address = dicLanguage["LBLADDRESSRP"];
				var ordertypedispatch = dicLanguage["LBLORDERTYPEDISPATCH"];
				var loadingdate = dicLanguage["LBLLOADINGDATEDISPATCHRP"];
				var dischargedate = dicLanguage["LBLDISCHARGEDATEDISPATCHRP"];
				var truckno = dicLanguage["LBLTRUCKNODISPATCHRP"];
				var contnumber = dicLanguage["LBLCONTNUMBER"];
				var contsize = dicLanguage["LBLCONTSIZERP"];
				var location = dicLanguage["MNULOCATION"];
				var amount = dicLanguage["LBLAMOUNTRP"];
				var amountshort = dicLanguage["LBLAMOUNTSHORTRP"];
				var surchargefee = dicLanguage["LBLSURCHARGEFEE"];
				var total = dicLanguage["LBLTETOTAL"];
				var isrequest = dicLanguage["LBLISREQUESTEDRP"];
				var category = dicLanguage["LBLCATEGORY"];
				var amountmoney = dicLanguage["LBLAMOUNTMONEY"];
				var voucher = dicLanguage["LBLVOUCHER"];
				var description = dicLanguage["LBLEXPLAIN"];
				var totalfinal = dicLanguage["LBLDISPATCHTOTALRP"];
				var transportD = dicLanguage["LBLTRANSPORTDATEDISPATCHRP"];
				report.SetParameterValue("tittle", tittle);
				report.SetParameterValue("transportfee", transportfee);
				report.SetParameterValue("taxamount", taxamount);
				report.SetParameterValue("aftertax", aftertax);
				report.SetParameterValue("payonbehalf", payonbehalf);
				report.SetParameterValue("aftertaxamount", aftertaxamount);
				report.SetParameterValue("invoiceattached", invoiceattached);
				report.SetParameterValue("customer", customer);
				report.SetParameterValue("address", address);
				report.SetParameterValue("ordertypedispatch", ordertypedispatch);
				report.SetParameterValue("loadingdate", loadingdate);
				report.SetParameterValue("dischargedate", dischargedate);
				report.SetParameterValue("truckno", truckno);
				report.SetParameterValue("contnumber", contnumber);
				report.SetParameterValue("contsize", contsize);
				report.SetParameterValue("location", location);
				report.SetParameterValue("amount", amount);
				report.SetParameterValue("amountshort", amountshort);
				report.SetParameterValue("surchargefee", surchargefee);
				report.SetParameterValue("total", total);
				report.SetParameterValue("isrequest", isrequest);
				report.SetParameterValue("category", category);
				report.SetParameterValue("amountmoney", amountmoney);
				report.SetParameterValue("voucher", voucher);
				report.SetParameterValue("description", description);
				report.SetParameterValue("totalfinal", totalfinal);
				report.SetParameterValue("transportD", transportD);

				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				report.SetParameterValue("companyTaxCode", companyTaxCode ?? "");
				report.SetParameterValue("monthYear", monthYear);
				report.SetParameterValue("imageUrl", path);

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

		public static Stream ExecLoad(Dataset.CustomerExpense.CustomerExpense.CustomerExpenseDataTable datatable,
						  int language, string companyName,string companyAddress,
						  string companyTaxCode, string monthYear, string fileName, string user, Dictionary<string, string> dicLanguage)
		{
			try
			{
				ReportDocument report;
				string strPath;
				CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
				string currentDate = cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year;
				if (language == 3)
				{
					cul = CultureInfo.GetCultureInfo("ja-JP");
					currentDate = DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日";
				}
				else if (language == 1)
				{
					cul = CultureInfo.GetCultureInfo("vi-VN");
					currentDate = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
				}
				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/CustomerExpense/CustomerExpenseLoad.rpt");
				// load report
				report = new ReportDocument();
				report.Load(strPath);

				// set datatable images
				var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);

				report.Database.Tables[0].SetDataSource((DataTable)datatable);
				var tittle = dicLanguage["LBLCUSTOMEREXPENSELOADTITTLE"];
				report.SetParameterValue("tittle", tittle);
				var invoiceattach = dicLanguage["LBLINVOICEATTACHED"];
				report.SetParameterValue("invoiceattach", invoiceattach);
				var customer = dicLanguage["LBLCUSTOMER"];
				report.SetParameterValue("customer", customer);
				var address = dicLanguage["LBLADDRESSRP"];
				report.SetParameterValue("address", address);
				var transportdate = dicLanguage["LBLTRANSPORTDATEDISPATCHRP"];
				report.SetParameterValue("transportdate", transportdate);
				var truckno = dicLanguage["LBLTRUCKNODISPATCHRP"];
				report.SetParameterValue("truckno", truckno);
				var loadingloc = dicLanguage["LBLLOADINGLOC"];
				report.SetParameterValue("loadingloc", loadingloc);
				var dischargeloc = dicLanguage["LBLDISCHARGELOC"];
				report.SetParameterValue("dischargeloc", dischargeloc);
				var slkl = dicLanguage["RPHDQUANTITYNETWEIGHT"];
				report.SetParameterValue("slkl", slkl);
				var unitprice = dicLanguage["LBLUNITPRICERP"];
				report.SetParameterValue("unitprice", unitprice);
				var amounttrans = dicLanguage["LBLAMOUNTSHORTRP"];
				report.SetParameterValue("amounttrans", amounttrans);
				var surchargefee = dicLanguage["LBLSURCHARGEFEE"];
				report.SetParameterValue("surchargefee", surchargefee);
				var description = dicLanguage["LBLDESCRIPTIONREPORT"];
				report.SetParameterValue("description", description);
				var amount = dicLanguage["LBLAMOUNTRP"];
				report.SetParameterValue("amount", amount);
				var taxamount = dicLanguage["LBLTAXAMOUNTREPORT"];
				report.SetParameterValue("taxamount", taxamount);
				var amountaftertax = dicLanguage["LBLTRANSPORTFEEAFTERTAXRP"];
				report.SetParameterValue("amountaftertax", amountaftertax);
				var entry = dicLanguage["LBLENTRYCLERKN"];
				report.SetParameterValue("entry", entry);
				var page = dicLanguage["RPFTPAGE"];
				report.SetParameterValue("page", page);
				var printby = dicLanguage["RPFTPRINTBY"];
				report.SetParameterValue("printby", printby);
				var printtime = dicLanguage["RPFTPRINTTIME"];
				report.SetParameterValue("printtime", printtime);
				// set parameters
				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				report.SetParameterValue("companyTaxCode", companyTaxCode ?? "");
				report.SetParameterValue("monthYear", monthYear);
				report.SetParameterValue("imageUrl", path);
				report.SetParameterValue("currentDate", currentDate);
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