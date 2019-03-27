using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace CrystalReport.Service.CustomerPricing
{
	public class ExportPdf
	{
		public static Stream GeneralExec(Dataset.CustomerPricing.CustomerPricing.CustomerPricingGeneralDataTable dataTable, int language,
			DateTime dateFrom, DateTime dateTo, Dictionary<string, string> textResources, string companyName, string companyAddress,
			string companyTaxCode, string fileName)
		{
			try
			{
				var strPath = HttpContext.Current.Request.MapPath("~/FileRpt/CustomerPricing/CustomerPricingGeneral.rpt");

				var report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)dataTable);
				// set datatable images
				var path = HttpContext.Current.Request.MapPath("~/Images/" + fileName);

				// set parameters
				var dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				var dateToFormated = dateTo.ToString("dd/MM/yyyy");
				var datePeriod = "";
				var hdCustomerPricing = textResources["RPHDCUSTOMERPRICING"];
				var hdCustomerN = textResources["RPHDCUSTOMERN"];
				var hdLocation1 = textResources["RPHDDEPARTURECP"];
				var hdLocation2 = textResources["RPHDDESTINATIONCP"];
				var hdContanerSize = textResources["RPHDCONTSIZE"];
				var hdContainerType = textResources["RPHDCONTTYPE"];
				var hdEstimatedD = textResources["LBLESTIMATEDD"];
				var hdExpense = textResources["RPHDCPTOLEXPENSE"];
				var hdEstimatedPrice = textResources["RPHDESTIMATEDPRICE"];
				var lblTotal = textResources["RPLBLTOTAL"];

				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == 2)
				{
					var cul = CultureInfo.GetCultureInfo("en-US");
					report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);
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
				report.SetParameterValue("hdCustomerPricing", hdCustomerPricing);
				report.SetParameterValue("hdCustomerN", hdCustomerN);
				report.SetParameterValue("hdLocation1", hdLocation1);
				report.SetParameterValue("hdLocation2", hdLocation2);
				report.SetParameterValue("hdContanerSize", hdContanerSize);
				report.SetParameterValue("hdContainerType", hdContainerType);
				report.SetParameterValue("hdEstimatedD", hdEstimatedD);
				report.SetParameterValue("hdExpense", hdExpense);
				report.SetParameterValue("hdEstimatedPrice", hdEstimatedPrice);
				report.SetParameterValue("lblTotal", lblTotal);

				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				report.SetParameterValue("companyTaxCode", textResources["LBLTAXCODE"] + ": " + companyTaxCode);
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

		public static Stream DetailExec(Dataset.CustomerPricing.CustomerPricing.CustomerPricingDetailDataTable dataTable, int language,
			DateTime dateFrom, DateTime dateTo, Dictionary<string, string> textResources)
		{
			try
			{
				var strPath = HttpContext.Current.Request.MapPath("~/FileRpt/CustomerPricing/CustomerPricingDetail.rpt");

				var report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)dataTable);

				// set parameters
				var dateFromFormated = dateFrom.ToString("dd/MM/yyyy");
				var dateToFormated = dateTo.ToString("dd/MM/yyyy");
				var datePeriod = "";
				var hdCustomerPricingDetail = textResources["RPHDCUSTOMERPRICING"];
				var lblCustomerN = textResources["RPHDCUSTOMERN"] + ":";
				var lblLocation1 = textResources["RPHDDEPARTURECP"] + ":";
				var lblLocation2 = textResources["RPHDDESTINATIONCP"] + ":";
				var lblContanerSize = textResources["RPHDCONTSIZE"] + ":";
				var lblContainerType = textResources["RPHDCONTTYPE"] + ":";
				var hdEstimatedD = textResources["LBLESTIMATEDD"] + ":";
				var hdCategory = textResources["RPHDCPCATEGORY"];
				var hdExpense = textResources["RPHDEXPENSE"];
				var hdAmount = textResources["LBLAMOUNTREPORT"];
				var lblEstimatedPrice = textResources["RPHDESTIMATEDPRICE"] + ":";

				var lblTotal = textResources["RPLBLTOTAL"];

				if (language == 1)
				{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
					datePeriod = string.Format("Từ ngày {0} đến ngày {1}", dateFromFormated, dateToFormated);
				}
				else if (language == 2)
				{
					var cul = CultureInfo.GetCultureInfo("en-US");
					report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);
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
				report.SetParameterValue("hdCustomerPricingDetail", hdCustomerPricingDetail);
				report.SetParameterValue("lblCustomerN", lblCustomerN);
				report.SetParameterValue("lblLocation1", lblLocation1);
				report.SetParameterValue("lblLocation2", lblLocation2);
				report.SetParameterValue("lblContanerSize", lblContanerSize);
				report.SetParameterValue("lblContainerType", lblContainerType);
				report.SetParameterValue("hdEstimatedD", hdEstimatedD);
				report.SetParameterValue("lblEstimatedPrice", lblEstimatedPrice);
				report.SetParameterValue("hdCategory", hdCategory);
				report.SetParameterValue("hdExpense", hdExpense);
				report.SetParameterValue("hdAmount", hdAmount);
				report.SetParameterValue("lblTotal", lblTotal);

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
