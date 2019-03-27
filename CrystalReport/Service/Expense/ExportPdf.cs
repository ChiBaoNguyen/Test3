using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalReport.Dataset.Expense;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CrystalReport.Service.Expense
{
	public class ExportPdf
	{
		public static Stream Exec(ExpenseList.ExpenseListDataTable datatable, int language, string amountTotal, string taxAmountTotal, string includedTotal, string requestedTotal, string payableTotal, DateTime dateFrom, DateTime dateTo, string category, string columnDate)
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Expense/ExpenseListEn.rpt");
				if (language == 3)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Expense/ExpenseListJp.rpt");
				}
				else if (language == 1)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Expense/ExpenseListVi.rpt");
				}

				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);
				report.SetParameterValue("total", amountTotal);
				report.SetParameterValue("taxAmountTotal", taxAmountTotal);
				report.SetParameterValue("requestedTotal", requestedTotal);
				report.SetParameterValue("includedTotal", includedTotal);
				report.SetParameterValue("payableTotal", payableTotal);
				// set parameter currentDate
				string dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				string dateToFormated = dateTo.ToString("dd/MM/yyyy");
				string datePeriod = "";
				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == 2)
				{
					report.SetParameterValue("currentDate", DateTime.Now.ToString("MMMM dd, yyyy"));
					dateFromFormated = dateFrom.ToString("MM/dd/yyyy");
					dateToFormated = dateTo.ToString("MM/dd/yyyy");
					datePeriod = string.Format("Period {0} to {1}", dateFromFormated, dateToFormated);
				}
				else
				{
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
					dateFromFormated = dateFrom.Year + "年" + dateFrom.Month + "月" + dateFrom.Day + "日";
					dateToFormated = dateTo.Year + "年" + dateTo.Month + "月" + dateTo.Day + "日";
					datePeriod = string.Format("{0}から{1}まで", dateFromFormated, dateToFormated);
				}
				report.SetParameterValue("datePeriod", datePeriod);
				report.SetParameterValue("category", category);
				report.SetParameterValue("columnDate", columnDate);

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

		public static Stream ExecCategory(ExpenseList.ExpenseListDataTable datatable, int language, DateTime dateFrom, DateTime dateTo, Dictionary<string, string> textResources,
											string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				ReportDocument report;
				string strPath =  HttpContext.Current.Request.MapPath("~/FileRpt/Expense/ExpenseDetailListVi.rpt");
				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);
				// set parameters
				var tittle = textResources["MNUEXPENSELISTREPORT"];
				report.SetParameterValue("tittle", tittle);
				var expense = textResources["MNUSTATISTICSEXPENSE"];
				report.SetParameterValue("expense", expense);
				var invoicedate = textResources["LBLINVOICEDATERP"];
				report.SetParameterValue("invoicedate", invoicedate);
				var transportdate = textResources["LBLTRANSPORTDATEDISPATCHRP"];
				report.SetParameterValue("transportdate", transportdate);
				var truck = textResources["MNUTRUCK"];
				report.SetParameterValue("truck", truck);
				var trailer = textResources["MNUTRAILER"];
				report.SetParameterValue("trailer", trailer);
				var employee = textResources["MNUEMPLOYEE"];
				report.SetParameterValue("employee", employee);
				var router = textResources["TLTROUTERP"];
				report.SetParameterValue("router", router);
				var quantity = textResources["LBLQUANTITYSHORTRP"];
				report.SetParameterValue("quantity", quantity);
				var unitprice = textResources["LBLUNITPRICERP"];
				report.SetParameterValue("unitprice", unitprice);
				var amount = textResources["LBLTOL"];
				report.SetParameterValue("amount", amount);
				var taxamount = textResources["LBLTAXAMOUNTREPORT"];
				report.SetParameterValue("taxamount", taxamount);
				var totalamount = textResources["LBLTETOTAL"];
				report.SetParameterValue("totalamount", totalamount);
				var content = textResources["LBLCONTENTCALCULATE"];
				report.SetParameterValue("content", content);
				var supplier = textResources["LBLSUPPLIERSHORTRP"];
				report.SetParameterValue("supplier", supplier);
				var type = textResources["LBLTYPE"];
				report.SetParameterValue("type", type);
				var contsize = textResources["LBLCONTSIZERP"];
				report.SetParameterValue("contsize", contsize);
				var contno = textResources["LBLCONTNUMBER"];
				report.SetParameterValue("contno", contno);

				// set parameter currentDate
				string dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				string dateToFormated = dateTo.ToString("dd/MM/yyyy");
				string datePeriod = "";
				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == 2)
				{
					report.SetParameterValue("currentDate", DateTime.Now.ToString("MMMM dd, yyyy"));
					dateFromFormated = dateFrom.ToString("MM/dd/yyyy");
					dateToFormated = dateTo.ToString("MM/dd/yyyy");
					datePeriod = string.Format("Period {0} to {1}", dateFromFormated, dateToFormated);
				}
				else
				{
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
					dateFromFormated = dateFrom.Year + "年" + dateFrom.Month + "月" + dateFrom.Day + "日";
					dateToFormated = dateTo.Year + "年" + dateTo.Month + "月" + dateTo.Day + "日";
					datePeriod = string.Format("{0}から{1}まで", dateFromFormated, dateToFormated);
				}
				report.SetParameterValue("datePeriod", datePeriod);
				//report.SetParameterValue("category", category);

				report.SetParameterValue("total", textResources["RPLBLTOTAL"]);
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
		public static Stream ExecCategoryDriver(ExpenseList.ExpenseListDataTable datatable, int language, DateTime dateFrom, DateTime dateTo, Dictionary<string, string> textResources,
											string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				ReportDocument report;
				string strPath =  HttpContext.Current.Request.MapPath("~/FileRpt/Expense/ExpenseDetailDriverListVi.rpt");
				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);
				// set parameters
				var tittle = textResources["MNUEXPENSELISTREPORT"];
				report.SetParameterValue("tittle", tittle);
				var expense = textResources["MNUSTATISTICSEXPENSE"];
				report.SetParameterValue("expense", expense);
				var invoicedate = textResources["LBLINVOICEDATERP"];
				report.SetParameterValue("invoicedate", invoicedate);
				var transportdate = textResources["LBLTRANSPORTDATEDISPATCHRP"];
				report.SetParameterValue("transportdate", transportdate);
				var truck = textResources["MNUTRUCK"];
				report.SetParameterValue("truck", truck);
				var trailer = textResources["MNUTRAILER"];
				report.SetParameterValue("trailer", trailer);
				var employee = textResources["MNUEMPLOYEE"];
				report.SetParameterValue("employee", employee);
				var router = textResources["TLTROUTERP"];
				report.SetParameterValue("router", router);
				var quantity = textResources["LBLQUANTITYSHORTRP"];
				report.SetParameterValue("quantity", quantity);
				var unitprice = textResources["LBLUNITPRICERP"];
				report.SetParameterValue("unitprice", unitprice);
				var amount = textResources["LBLTOL"];
				report.SetParameterValue("amount", amount);
				var taxamount = textResources["LBLTAXAMOUNTREPORT"];
				report.SetParameterValue("taxamount", taxamount);
				var totalamount = textResources["LBLTETOTAL"];
				report.SetParameterValue("totalamount", totalamount);
				var content = textResources["LBLCONTENTCALCULATE"];
				report.SetParameterValue("content", content);
				var supplier = textResources["LBLSUPPLIERSHORTRP"];
				report.SetParameterValue("supplier", supplier);
				// set parameter currentDate
				string dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				string dateToFormated = dateTo.ToString("dd/MM/yyyy");
				string datePeriod = "";
				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == 2)
				{
					report.SetParameterValue("currentDate", DateTime.Now.ToString("MMMM dd, yyyy"));
					dateFromFormated = dateFrom.ToString("MM/dd/yyyy");
					dateToFormated = dateTo.ToString("MM/dd/yyyy");
					datePeriod = string.Format("Period {0} to {1}", dateFromFormated, dateToFormated);
				}
				else
				{
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
					dateFromFormated = dateFrom.Year + "年" + dateFrom.Month + "月" + dateFrom.Day + "日";
					dateToFormated = dateTo.Year + "年" + dateTo.Month + "月" + dateTo.Day + "日";
					datePeriod = string.Format("{0}から{1}まで", dateFromFormated, dateToFormated);
				}
				report.SetParameterValue("datePeriod", datePeriod);
				//report.SetParameterValue("category", category);

				report.SetParameterValue("total", textResources["RPLBLTOTAL"]);
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

		public static Stream ExecCategoryEmployee(ExpenseList.ExpenseListDataTable datatable, int language, DateTime dateFrom, DateTime dateTo, Dictionary<string, string> textResources,
											string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				ReportDocument report;
				string strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Expense/ExpenseDetailEmployeeListVi.rpt");
				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);
				// set parameters
				var tittle = textResources["MNUEXPENSELISTREPORT"];
				report.SetParameterValue("tittle", tittle);
				var expense = textResources["MNUSTATISTICSEXPENSE"];
				report.SetParameterValue("expense", expense);
				var invoicedate = textResources["LBLINVOICEDATERP"];
				report.SetParameterValue("invoicedate", invoicedate);
				var transportdate = textResources["LBLTRANSPORTDATEDISPATCHRP"];
				report.SetParameterValue("transportdate", transportdate);
				var truck = textResources["MNUTRUCK"];
				report.SetParameterValue("truck", truck);
				var trailer = textResources["MNUTRAILER"];
				report.SetParameterValue("trailer", trailer);
				var employee = textResources["MNUEMPLOYEE"];
				report.SetParameterValue("employee", employee);
				var router = textResources["TLTROUTERP"];
				report.SetParameterValue("router", router);
				var quantity = textResources["LBLQUANTITYSHORTRP"];
				report.SetParameterValue("quantity", quantity);
				var unitprice = textResources["LBLUNITPRICERP"];
				report.SetParameterValue("unitprice", unitprice);
				var amount = textResources["LBLTOL"];
				report.SetParameterValue("amount", amount);
				var taxamount = textResources["LBLTAXAMOUNTREPORT"];
				report.SetParameterValue("taxamount", taxamount);
				var totalamount = textResources["LBLTETOTAL"];
				report.SetParameterValue("totalamount", totalamount);
				var content = textResources["LBLCONTENTCALCULATE"];
				report.SetParameterValue("content", content);
				var supplier = textResources["LBLSUPPLIERSHORTRP"];
				report.SetParameterValue("supplier", supplier);
				// set parameter currentDate
				string dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				string dateToFormated = dateTo.ToString("dd/MM/yyyy");
				string datePeriod = "";
				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == 2)
				{
					report.SetParameterValue("currentDate", DateTime.Now.ToString("MMMM dd, yyyy"));
					dateFromFormated = dateFrom.ToString("MM/dd/yyyy");
					dateToFormated = dateTo.ToString("MM/dd/yyyy");
					datePeriod = string.Format("Period {0} to {1}", dateFromFormated, dateToFormated);
				}
				else
				{
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
					dateFromFormated = dateFrom.Year + "年" + dateFrom.Month + "月" + dateFrom.Day + "日";
					dateToFormated = dateTo.Year + "年" + dateTo.Month + "月" + dateTo.Day + "日";
					datePeriod = string.Format("{0}から{1}まで", dateFromFormated, dateToFormated);
				}
				report.SetParameterValue("datePeriod", datePeriod);
				//report.SetParameterValue("category", category);

				report.SetParameterValue("total", textResources["RPLBLTOTAL"]);
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

		public static Stream ExecCategoryTrailer(ExpenseList.ExpenseListDataTable datatable, int language, DateTime dateFrom, DateTime dateTo, Dictionary<string, string> textResources,
											string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				ReportDocument report;
				string strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Expense/ExpenseDetailTrailerListVi.rpt");
				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);
				// set parameters
				var tittle = textResources["MNUEXPENSELISTREPORT"];
				report.SetParameterValue("tittle", tittle);
				var expense = textResources["MNUSTATISTICSEXPENSE"];
				report.SetParameterValue("expense", expense);
				var invoicedate = textResources["LBLINVOICEDATERP"];
				report.SetParameterValue("invoicedate", invoicedate);
				var transportdate = textResources["LBLTRANSPORTDATEDISPATCHRP"];
				report.SetParameterValue("transportdate", transportdate);
				var truck = textResources["MNUTRUCK"];
				report.SetParameterValue("truck", truck);
				var trailer = textResources["MNUTRAILER"];
				report.SetParameterValue("trailer", trailer);
				var employee = textResources["MNUEMPLOYEE"];
				report.SetParameterValue("employee", employee);
				var router = textResources["TLTROUTERP"];
				report.SetParameterValue("router", router);
				var quantity = textResources["LBLQUANTITYSHORTRP"];
				report.SetParameterValue("quantity", quantity);
				var unitprice = textResources["LBLUNITPRICERP"];
				report.SetParameterValue("unitprice", unitprice);
				var amount = textResources["LBLTOL"];
				report.SetParameterValue("amount", amount);
				var taxamount = textResources["LBLTAXAMOUNTREPORT"];
				report.SetParameterValue("taxamount", taxamount);
				var totalamount = textResources["LBLTETOTAL"];
				report.SetParameterValue("totalamount", totalamount);
				var content = textResources["LBLCONTENTCALCULATE"];
				report.SetParameterValue("content", content);
				var supplier = textResources["LBLSUPPLIERSHORTRP"];
				report.SetParameterValue("supplier", supplier);
				var type = textResources["LBLTYPE"];
				report.SetParameterValue("type", type);
				var contsize = textResources["LBLCONTSIZERP"];
				report.SetParameterValue("contsize", contsize);
				var contno = textResources["LBLCONTNUMBER"];
				report.SetParameterValue("contno", contno);
				// set parameter currentDate
				string dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				string dateToFormated = dateTo.ToString("dd/MM/yyyy");
				string datePeriod = "";
				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == 2)
				{
					report.SetParameterValue("currentDate", DateTime.Now.ToString("MMMM dd, yyyy"));
					dateFromFormated = dateFrom.ToString("MM/dd/yyyy");
					dateToFormated = dateTo.ToString("MM/dd/yyyy");
					datePeriod = string.Format("Period {0} to {1}", dateFromFormated, dateToFormated);
				}
				else
				{
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
					dateFromFormated = dateFrom.Year + "年" + dateFrom.Month + "月" + dateFrom.Day + "日";
					dateToFormated = dateTo.Year + "年" + dateTo.Month + "月" + dateTo.Day + "日";
					datePeriod = string.Format("{0}から{1}まで", dateFromFormated, dateToFormated);
				}
				report.SetParameterValue("datePeriod", datePeriod);
				//report.SetParameterValue("category", category);

				report.SetParameterValue("total", textResources["RPLBLTOTAL"]);
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

		public static Stream ExecCategoryTruck(ExpenseList.ExpenseListDataTable datatable, int language, DateTime dateFrom, DateTime dateTo, Dictionary<string, string> textResources,
											string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				ReportDocument report;
				string strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Expense/ExpenseDetailTruckListVi.rpt");
				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);
				// set parameters
				var tittle = textResources["MNUEXPENSELISTREPORT"];
				report.SetParameterValue("tittle", tittle);
				var expense = textResources["MNUSTATISTICSEXPENSE"];
				report.SetParameterValue("expense", expense);
				var invoicedate = textResources["LBLINVOICEDATERP"];
				report.SetParameterValue("invoicedate", invoicedate);
				var transportdate = textResources["LBLTRANSPORTDATEDISPATCHRP"];
				report.SetParameterValue("transportdate", transportdate);
				var truck = textResources["MNUTRUCK"];
				report.SetParameterValue("truck", truck);
				var trailer = textResources["MNUTRAILER"];
				report.SetParameterValue("trailer", trailer);
				var employee = textResources["MNUEMPLOYEE"];
				report.SetParameterValue("employee", employee);
				var router = textResources["TLTROUTERP"];
				report.SetParameterValue("router", router);
				var quantity = textResources["LBLQUANTITYSHORTRP"];
				report.SetParameterValue("quantity", quantity);
				var unitprice = textResources["LBLUNITPRICERP"];
				report.SetParameterValue("unitprice", unitprice);
				var amount = textResources["LBLTOL"];
				report.SetParameterValue("amount", amount);
				var taxamount = textResources["LBLTAXAMOUNTREPORT"];
				report.SetParameterValue("taxamount", taxamount);
				var totalamount = textResources["LBLTETOTAL"];
				report.SetParameterValue("totalamount", totalamount);
				var content = textResources["LBLCONTENTCALCULATE"];
				report.SetParameterValue("content", content);
				var supplier = textResources["LBLSUPPLIERSHORTRP"];
				report.SetParameterValue("supplier", supplier);
				var type = textResources["LBLTYPE"];
				report.SetParameterValue("type", type);
				var contsize = textResources["LBLCONTSIZERP"];
				report.SetParameterValue("contsize", contsize);
				var contno = textResources["LBLCONTNUMBER"];
				report.SetParameterValue("contno", contno);

				// set parameter currentDate
				string dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				string dateToFormated = dateTo.ToString("dd/MM/yyyy");
				string datePeriod = "";
				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == 2)
				{
					report.SetParameterValue("currentDate", DateTime.Now.ToString("MMMM dd, yyyy"));
					dateFromFormated = dateFrom.ToString("MM/dd/yyyy");
					dateToFormated = dateTo.ToString("MM/dd/yyyy");
					datePeriod = string.Format("Period {0} to {1}", dateFromFormated, dateToFormated);
				}
				else
				{
					report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
					dateFromFormated = dateFrom.Year + "年" + dateFrom.Month + "月" + dateFrom.Day + "日";
					dateToFormated = dateTo.Year + "年" + dateTo.Month + "月" + dateTo.Day + "日";
					datePeriod = string.Format("{0}から{1}まで", dateFromFormated, dateToFormated);
				}
				report.SetParameterValue("datePeriod", datePeriod);
				//report.SetParameterValue("category", category);

				report.SetParameterValue("total", textResources["RPLBLTOTAL"]);
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