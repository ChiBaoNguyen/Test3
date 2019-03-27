using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CrystalReport.Dataset.DriverAllowance;

namespace CrystalReport.Service.DriverAllowance
{
    public class ExportPdf
    {
		public static Stream Exec(DriverAllowanceList.DriverAllowanceDetailDataTable datatable, int language, Dictionary<string, string> dicLanguage, DateTime dateFrom, DateTime dateTo,
			string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverAllowance/DriverAllowanceList.rpt");
				//if (language == 3)
				//{
				//	strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverAllowance/DriverAllowanceListJp.rpt");
				//}
				//else if (language == 1)
				//{
				//	strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverAllowance/DriverAllowanceListVi.rpt");
				//}

				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);

				var title = dicLanguage["TLTDRIVERALLOWANCE"];
				var driver = dicLanguage["LBLDRIVERREPORT"];
				var transport = dicLanguage["LBLTRANSPORTDREPORT"];
				var orderno = dicLanguage["LBLORDERENTRYNO"];
				var customer = dicLanguage["LBLCUSTOMERREPORT"];
				var contno = dicLanguage["LBLCONTAINERNO"];
				var location = dicLanguage["LBLLOCATIONREPORT"];
				var allowance = dicLanguage["LBLDRIVINGALLOWANCE"];
				var other = dicLanguage["LBLOTHALLOWANCE"];
				var weight = dicLanguage["LBLNETWEIGHTSHORT"];
				var total = dicLanguage["RPLBLTOTAL"];
				report.SetParameterValue("title", title);
				report.SetParameterValue("driver", driver);
				report.SetParameterValue("transport", transport);
				report.SetParameterValue("orderno", orderno);
				report.SetParameterValue("customer", customer);
				report.SetParameterValue("contno", contno);
				report.SetParameterValue("location", location);
				report.SetParameterValue("allowance", allowance);
				report.SetParameterValue("other", other);
				report.SetParameterValue("weight", weight);
				report.SetParameterValue("total", total);

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
				// set datatable images
				var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);
				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				report.SetParameterValue("imageUrl", path);
				report.SetParameterValue("user", user);

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

		public static Stream ExecSalary(DriverAllowanceList.DriverAllowanceDetailDataTable datatable, DriverAllowanceList.SettlementMoneyDataTable datatable2, int language, Dictionary<string, string> dicLanguage, DateTime dateFrom, DateTime dateTo,
			string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverAllowance/DriverSalaryList.rpt");
				//if (language == 3)
				//{
				//	strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverAllowance/DriverAllowanceListJp.rpt");
				//}
				//else if (language == 1)
				//{
				//	strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverAllowance/DriverAllowanceListVi.rpt");
				//}

				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables["DriverAllowanceDetail"].SetDataSource((DataTable)datatable);
				report.Database.Tables["SettlementMoney"].SetDataSource((DataTable)datatable2);

				var title = dicLanguage["LBLSALARYDRIVER"];
				var driver = dicLanguage["LBLDRIVERREPORT"];
				var transport = dicLanguage["LBLTRANSPORTDREPORT"];
				var orderno = dicLanguage["LBLORDERENTRYNO"];
				var customer = dicLanguage["LBLCUSTOMERREPORT"];
				var contno = dicLanguage["LBLCONTAINERNO"];
				var location = dicLanguage["LBLLOCATIONREPORT"];
				var allowance = dicLanguage["LBLDRIVINGALLOWANCE"];
				var other = dicLanguage["LBLOTHALLOWANCE"];
				var weight = dicLanguage["LBLNETWEIGHTSHORT"];
				var total = dicLanguage["RPLBLTOTAL"];
				var turnovercal = dicLanguage["LBLTURNOVERCALCULATEDALLOWANCE"];
				var turnoverrate = dicLanguage["LBLTURNOVERRATE"];
				var advancemoney = dicLanguage["LBLADVANCEMONEYREMAINING"];
				var advancemoney1 = dicLanguage["LBLADVANCEMONEYREMAINING"];
				var othermoney = dicLanguage["LBLOTHERMONEY"];
				var othermoney1 = dicLanguage["LBLOTHERMONEY"];
				var realmoney = dicLanguage["LBLREALMONEY"];
				var settlementmoney = dicLanguage["LBLSETTLEMENTMONEY"];
				var datefull = dicLanguage["LBLDATEFULL"];
				var advanceLia = dicLanguage["LBLADVANCELIABILITIES"];
				var exLia = dicLanguage["LBLEXPENSELIABILITIES"];
				var content = dicLanguage["LBLCONTENTCALCULATE"];
				var totalsum = dicLanguage["LBLTOTALREPORT"];
				var basicsalary = dicLanguage["LBLBASICSALARY"];
				report.SetParameterValue("title", title);
				report.SetParameterValue("driver", driver);
				report.SetParameterValue("transport", transport);
				report.SetParameterValue("orderno", orderno);
				report.SetParameterValue("customer", customer);
				report.SetParameterValue("contno", contno);
				report.SetParameterValue("location", location);
				report.SetParameterValue("allowance", allowance);
				report.SetParameterValue("other", other);
				report.SetParameterValue("weight", weight);
				report.SetParameterValue("total", total);
				report.SetParameterValue("turnovercal", turnovercal);
				report.SetParameterValue("turnoverrate", turnoverrate);
				report.SetParameterValue("advancemoney", advancemoney);
				report.SetParameterValue("advancemoney1", advancemoney1);
				report.SetParameterValue("othermoney", othermoney);
				report.SetParameterValue("othermoney1", othermoney1);
				report.SetParameterValue("realmoney", realmoney);
				report.SetParameterValue("settlementmoney", settlementmoney);
				report.SetParameterValue("datefull", datefull);
				report.SetParameterValue("advanceLia", advanceLia);
				report.SetParameterValue("exLia", exLia);
				report.SetParameterValue("content", content);
				report.SetParameterValue("totalsum", totalsum);
				report.SetParameterValue("basicsalary", basicsalary);
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
				// set datatable images
				var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);
				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				report.SetParameterValue("imageUrl", path);
				report.SetParameterValue("user", user);

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

		public static Stream ExecSalaryNoFreight(DriverAllowanceList.DriverAllowanceDetailDataTable datatable, DriverAllowanceList.SettlementMoneyDataTable datatable2, int language, Dictionary<string, string> dicLanguage, DateTime dateFrom, DateTime dateTo,
			string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverAllowance/DriverSalaryListNoFreight.rpt");
				//if (language == 3)
				//{
				//	strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverAllowance/DriverAllowanceListJp.rpt");
				//}
				//else if (language == 1)
				//{
				//	strPath = HttpContext.Current.Request.MapPath("~/FileRpt/DriverAllowance/DriverAllowanceListVi.rpt");
				//}

				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables["DriverAllowanceDetail"].SetDataSource((DataTable)datatable);
				report.Database.Tables["SettlementMoney"].SetDataSource((DataTable)datatable2);

				var title = dicLanguage["LBLSALARYDRIVER"];
				var driver = dicLanguage["LBLDRIVERREPORT"];
				var transport = dicLanguage["LBLTRANSPORTDREPORT"];
				var orderno = dicLanguage["LBLORDERENTRYNO"];
				var customer = dicLanguage["LBLCUSTOMERREPORT"];
				var contno = dicLanguage["LBLCONTAINERNO"];
				var location = dicLanguage["LBLLOCATIONREPORT"];
				var allowance = dicLanguage["LBLDRIVINGALLOWANCE"];
				var other = dicLanguage["LBLOTHALLOWANCE"];
				var weight = dicLanguage["LBLNETWEIGHTSHORT"];
				var total = dicLanguage["RPLBLTOTAL"];
				var turnovercal = dicLanguage["LBLTURNOVERCALCULATEDALLOWANCE"];
				var turnoverrate = dicLanguage["LBLTURNOVERRATE"];
				var advancemoney = dicLanguage["LBLADVANCEMONEYREMAINING"];
				var advancemoney1 = dicLanguage["LBLADVANCEMONEYREMAINING"];
				var othermoney = dicLanguage["LBLOTHERMONEY"];
				var othermoney1 = dicLanguage["LBLOTHERMONEY"];
				var realmoney = dicLanguage["LBLREALMONEY"];
				var settlementmoney = dicLanguage["LBLSETTLEMENTMONEY"];
				var datefull = dicLanguage["LBLDATEFULL"];
				var advanceLia = dicLanguage["LBLADVANCELIABILITIES"];
				var exLia = dicLanguage["LBLEXPENSELIABILITIES"];
				var content = dicLanguage["LBLCONTENTCALCULATE"];
				var totalsum = dicLanguage["LBLTOTALREPORT"];
				var basicsalary = dicLanguage["LBLBASICSALARY"];
				report.SetParameterValue("title", title);
				report.SetParameterValue("driver", driver);
				report.SetParameterValue("transport", transport);
				report.SetParameterValue("orderno", orderno);
				report.SetParameterValue("customer", customer);
				report.SetParameterValue("contno", contno);
				report.SetParameterValue("location", location);
				report.SetParameterValue("allowance", allowance);
				report.SetParameterValue("other", other);
				report.SetParameterValue("weight", weight);
				report.SetParameterValue("total", total);
				report.SetParameterValue("turnovercal", turnovercal);
				report.SetParameterValue("turnoverrate", turnoverrate);
				report.SetParameterValue("advancemoney", advancemoney);
				report.SetParameterValue("advancemoney1", advancemoney1);
				report.SetParameterValue("othermoney", othermoney);
				report.SetParameterValue("othermoney1", othermoney1);
				report.SetParameterValue("realmoney", realmoney);
				report.SetParameterValue("settlementmoney", settlementmoney);
				report.SetParameterValue("datefull", datefull);
				report.SetParameterValue("advanceLia", advanceLia);
				report.SetParameterValue("exLia", exLia);
				report.SetParameterValue("content", content);
				report.SetParameterValue("totalsum", totalsum);
				report.SetParameterValue("basicsalary", basicsalary);
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
				// set datatable images
				var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);
				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				report.SetParameterValue("imageUrl", path);
				report.SetParameterValue("user", user);

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
