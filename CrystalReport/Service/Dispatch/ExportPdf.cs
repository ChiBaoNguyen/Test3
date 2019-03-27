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
using Website.ViewModels.Order;

namespace CrystalReport.Service.Dispatch
{
    public class ExportPdf
    {
		public static Stream Exec(DispatchList.DispatchListDataTable datatable, int language, string total, string fromDate, string toDate, string category,
									string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Dispatch/DispatchListEn.rpt");
				if (language == 3)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Dispatch/DispatchListJp.rpt");
				}
				else if (language == 1)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Dispatch/DispatchListVi.rpt");
				}

				report = new ReportDocument();
				report.Load(strPath);
				report.Database.Tables[0].SetDataSource((DataTable)datatable);
				report.SetParameterValue("total", total);
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

		public static Stream ExecTransportHandover(TransportConfirmViewModel datatable,
																	string companyName, string companyAddress, string companyTaxCode, string userContactName, string phoneNumber1, string phoneNumber2)
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Dispatch/Handover.rpt");

				report = new ReportDocument();
				report.Load(strPath);
				var isCollected = datatable.TransportConfirmOrder.IsCollected;
				var collectedName = "";
				var collectedTax = "";
				var collectedAddress = "";
				if (isCollected == "1")
				{
					collectedName = companyName;
					collectedTax = companyTaxCode;
					collectedAddress = companyAddress;
				}
				else if (isCollected == "0")
				{
					collectedName = datatable.TransportConfirmOrder.CustomerN ?? "";
					collectedTax = datatable.TransportConfirmOrder.CustomerTaxCode ?? "";
					collectedAddress = datatable.TransportConfirmOrder.CustomerAddress ?? "";
					
				}
				// set date
				var customerCode = datatable.TransportConfirmOrder.CustomerN;
				var orderDate = (datatable.TransportConfirmOrder.OrderD).ToString("dd/MM/yyyy");
				var location1 = datatable.TransportConfirmOrder.LoadingPlaceN ?? "";
				var location2 = datatable.TransportConfirmOrder.StopoverPlaceN ?? "";
				var location3 = datatable.TransportConfirmOrder.DischargePlaceN ?? "";
				var containerNo = datatable.TransportConfirmContainer.ContainerNo ?? "";
				var sealNo = datatable.TransportConfirmContainer.SealNo ?? "";
				var commodity = datatable.TransportConfirmContainer.CommodityN ?? "";
				var pickupReturnDate = datatable.TransportConfirmOrder.StopoverDT != null ? (datatable.TransportConfirmOrder.StopoverDT.Value).ToString("dd/MM/yyyy HH:mm") : "";
				var contactAddress = datatable.TransportConfirmOrder.ShipperN ?? "";
				//var contactAddress = datatable.TransportConfirmOrder.DeliveryContact ?? "";

				report.SetParameterValue("collectedName", collectedName);
				report.SetParameterValue("collectedTax", collectedTax);
				report.SetParameterValue("collectedAddress", collectedAddress);
				report.SetParameterValue("customerCode", customerCode);
				report.SetParameterValue("orderDate", orderDate);
				report.SetParameterValue("location1", location1);
				report.SetParameterValue("location2", location2);
				report.SetParameterValue("location3", location3);
				report.SetParameterValue("containerNo", containerNo);
				report.SetParameterValue("sealNo", sealNo);
				report.SetParameterValue("commodity", commodity);
				report.SetParameterValue("userContactName", userContactName);
				report.SetParameterValue("phoneNumber1", phoneNumber1);
				report.SetParameterValue("phoneNumber2", phoneNumber2);
				report.SetParameterValue("pickupReturnDate", pickupReturnDate);
				report.SetParameterValue("contactAddress", contactAddress ?? "");
				// set datatable images
				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				
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

		public static Stream ExecTransportInstruction(string companyName, string companyAddress, string companyTel, string customerCode,
			  string driverName, string licenseNo, string driverTel, string route, string commodity, string trailerNo, string truckNo, string location2export, string location2import, string instructionNo)
		{
			try
			{
				ReportDocument report;
				string strPath;

				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/Dispatch/TransportIntruction.rpt");

				report = new ReportDocument();
				report.Load(strPath);

				// set date
				report.SetParameterValue("customerCode", customerCode);
				report.SetParameterValue("driverName", driverName);
				report.SetParameterValue("licenseNo", licenseNo);
				report.SetParameterValue("driverTel", driverTel);
				report.SetParameterValue("route", route);
				report.SetParameterValue("commodity", commodity);
				report.SetParameterValue("trailerNo", trailerNo);
				report.SetParameterValue("truckNo", truckNo);
				report.SetParameterValue("location2export", location2export);
				report.SetParameterValue("location2import", location2import ?? "");
				report.SetParameterValue("instructionNo", instructionNo ?? "");
				// set datatable images
				report.SetParameterValue("companyName", companyName ?? "");
				report.SetParameterValue("companyAddress", companyAddress ?? "");
				report.SetParameterValue("companyTel", companyTel ?? "");
				// set parameter currentDate
				//if (language == 1)
				//{
					report.SetParameterValue("currentDate", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
				//}
				//else if (language == 2)
				//{
				//	CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
				//	report.SetParameterValue("currentDate", cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year);
				//}
				//else
				//{
				//	report.SetParameterValue("currentDate", DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日");
				//}
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
