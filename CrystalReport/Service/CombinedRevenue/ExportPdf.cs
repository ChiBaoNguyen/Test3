using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Data;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace CrystalReport.Service.CombinedRevenue
{
	public class ExportPdf
	{
		public static Stream Exec( Dataset.CombinedRevenue.CombinedRevenue.CombinedRevenueDataTable datatable,
									int language, string reportTimeI, DateTime transportM, string objectN, Dictionary<string, string> dicLanguage,
									string companyName, string companyAddress, string fileName, string user
								 )
		{
			try
			{
				ReportDocument report = new ReportDocument();
				string strPath = HttpContext.Current.Request.MapPath("~/FileRpt/CombinedRevenue/CombinedRevenueListMonth.rpt");
				report.Load(strPath);

				report.Database.Tables[0].SetDataSource((DataTable)datatable);

				report.SetParameterValue("netWeight", dicLanguage["LBLNETWEIGHT"]);
				report.SetParameterValue("yearonyear", dicLanguage["RPLLBLMONTHONMONTH"]);
				SetParameterValue(report, language, reportTimeI, transportM, objectN, dicLanguage,
													companyName, companyAddress, fileName, user);
				
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

		public static Stream ExecQuarter(Dataset.CombinedRevenue.CombinedRevenue.CombinedRevenueQuarterDataTable datatable,
							int language, string reportTimeI, DateTime transportM, string objectN, Dictionary<string, string> dicLanguage,
							string companyName, string companyAddress, string fileName, string user
						 )
		{
			try
			{
				ReportDocument report = new ReportDocument();
				string strPath = HttpContext.Current.Request.MapPath("~/FileRpt/CombinedRevenue/CombinedRevenueListQuarter.rpt");
				report.Load(strPath);

				report.Database.Tables[0].SetDataSource((DataTable)datatable);

				report.SetParameterValue("netWeight", dicLanguage["LBLNETWEIGHTSHORT"] + " (" + dicLanguage["LBLTON"] + ")");
				report.SetParameterValue("totalRevenue", dicLanguage["RPLLBLTOTALREVENUE"]);
				report.SetParameterValue("yearonyear", dicLanguage["RPLLBLYEARONYEAR"]);

				string quarter = dicLanguage["LBLQUARTER"];
				if (language == 3)
				{
					quarter = "第";
				}
				else
					quarter += " ";
				report.SetParameterValue("quarter", quarter);
				SetParameterValue(report, language, reportTimeI, transportM, objectN, dicLanguage,
														companyName, companyAddress, fileName, user);

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

		public static Stream ExecYear(Dataset.CombinedRevenue.CombinedRevenue.CombinedRevenueYearDataTable datatable,
							int language, string reportTimeI, DateTime transportM, string objectN, Dictionary<string, string> dicLanguage,
							string companyName, string companyAddress, string fileName, string user
						 )
		{
			try
			{
				ReportDocument report = new ReportDocument();
				string strPath = HttpContext.Current.Request.MapPath("~/FileRpt/CombinedRevenue/CombinedRevenueListYear.rpt");
				report.Load(strPath);

				report.Database.Tables[0].SetDataSource((DataTable)datatable);

				report.SetParameterValue("netWeight", dicLanguage["LBLNETWEIGHTSHORT"] + " (" + dicLanguage["LBLTON"] + ")");
				report.SetParameterValue("totalRevenue", dicLanguage["RPLLBLTOTALREVENUE"]);
				report.SetParameterValue("yearonyear", dicLanguage["RPLLBLYEARONYEAR"]);
				report.SetParameterValue("month1", dicLanguage["LBLMONTH1FIXEDEXPENSE"]);
				report.SetParameterValue("month2", dicLanguage["LBLMONTH2FIXEDEXPENSE"]);
				report.SetParameterValue("month3", dicLanguage["LBLMONTH3FIXEDEXPENSE"]);
				report.SetParameterValue("month4", dicLanguage["LBLMONTH4FIXEDEXPENSE"]);
				report.SetParameterValue("month5", dicLanguage["LBLMONTH5FIXEDEXPENSE"]);
				report.SetParameterValue("month6", dicLanguage["LBLMONTH6FIXEDEXPENSE"]);
				report.SetParameterValue("month7", dicLanguage["LBLMONTH7FIXEDEXPENSE"]);
				report.SetParameterValue("month8", dicLanguage["LBLMONTH8FIXEDEXPENSE"]);
				report.SetParameterValue("month9", dicLanguage["LBLMONTH9FIXEDEXPENSE"]);
				report.SetParameterValue("month10", dicLanguage["LBLMONTH10FIXEDEXPENSE"]);
				report.SetParameterValue("month11", dicLanguage["LBLMONTH11FIXEDEXPENSE"]);
				report.SetParameterValue("month12", dicLanguage["LBLMONTH12FIXEDEXPENSE"]);
				SetParameterValue(report, language, reportTimeI, transportM, objectN, dicLanguage,
														companyName, companyAddress, fileName, user);

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

		private static ReportDocument SetParameterValue(ReportDocument report, int language, string reportTimeI, DateTime transportM, string objectN, Dictionary<string, string> dicLanguage,
							string companyName, string companyAddress, string fileName, string user)
		{
			string title = dicLanguage["MNUCOMBINEDREVENUEREPORT"].ToUpper();
			string period = "";
			string unit = dicLanguage["LBLUNIT"] + ": ";
			// set parameters
			if (language == 1)
			{
				report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
				if (reportTimeI == "M")
				{
					title += " THEO THÁNG";
					period = "Tháng " + transportM.Month + "/" + transportM.Year;
					unit += " đồng";
				}
				else if (reportTimeI == "Q")
				{
					title += " THEO QUÝ";
					period = "Năm " + transportM.Year.ToString();
					//period = Math.Ceiling((double)(transportM.Month / 3))  + "/" + transportM.Year;
					unit += " đồng";
				}
				else
				{
					title += " THEO NĂM";
					period = "Năm " + transportM.Year.ToString();
					unit += "triệu đồng";
				}
			}
			else if (language == 2)
			{
				CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
				report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);

				if (reportTimeI == "M")
				{
					period = "Month " + transportM.Month + "/" + transportM.Year;
					unit += " dong";
				}
				else if (reportTimeI == "Q")
				{
					period = "Year " + transportM.Year.ToString();
					unit += " dong";
				}
				else
				{
					period = "Year " + transportM.Year.ToString();
					unit += "million dong";
				}
			}
			else
			{
				report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");

				if (reportTimeI == "M")
				{
					period = transportM.Year + "年" + transportM.Month + "月";
					unit += "ドン";
				}
				else if (reportTimeI == "Q")
				{
					period = transportM.Year.ToString() + "年";
					unit += "ドン";
				}
				else
				{

					period = transportM.Year.ToString() + "年";
					unit += "百万ドン";
				}

			}
			report.SetParameterValue("title", title);
			report.SetParameterValue("period", period);
			report.SetParameterValue("unit", unit);

			// set datatable images
			var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);
			report.SetParameterValue("companyName", companyName ?? "");
			report.SetParameterValue("companyAddress", companyAddress ?? "");
			report.SetParameterValue("imageUrl", path);
			report.SetParameterValue("user", user);

			report.SetParameterValue("objectN", objectN);
			report.SetParameterValue("transportCount", dicLanguage["RPLLBLTRANSPORTCOUNT"]);
			report.SetParameterValue("revenue", dicLanguage["RPLLBLREVENUE"]);
			//report.SetParameterValue("yearonyear", dicLanguage["RPLLBLYEARONYEAR"]);
			report.SetParameterValue("total", dicLanguage["RPLBLTOTAL"]);

			report.SetParameterValue("ftPrintTime", dicLanguage["RPFTPRINTTIME"]);
			report.SetParameterValue("ftPrintBy", dicLanguage["RPFTPRINTBY"]);
			report.SetParameterValue("ftPage", dicLanguage["RPFTPAGE"]);
			report.SetParameterValue("ftCreator", dicLanguage["RPFTCREATOR"]);

			return report;
		}

	}
}
