using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Root.Models;
using Root.Models.Mapping;
using Website.Enum;
using Website.Utilities;
using Website.ViewModels.Container;
using Website.ViewModels.Order;
namespace CrystalReport.Service.Order
{
	public class ExportExcel
	{
		public static Stream ExportOrderListToExcel(List<ContainerViewModel> conV, List<OrderDetailListReport> data, Dictionary<string, string> dictionary, int intLanguague, string companyName, string companyAddress, string fileName, string user, DateTime? dateFrom, DateTime? dateTo)
		{
			using (ExcelPackage package = new ExcelPackage())
			{
				// add a new worksheet to the empty workbook
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("OrderGeneralReport");

				//Company name
				worksheet.Cells[1, 2].Value = companyName;
				worksheet.Cells[1, 2].Style.Font.Size = 11;
				worksheet.Cells[1, 2, 1, 3].Merge = true;
				//Company address
				worksheet.Cells[2, 2].Value = companyAddress;
				worksheet.Cells[2, 2].Style.Font.Size = 11;
				worksheet.Cells[2, 2, 2, 3].Merge = true;

				//Add the title
				worksheet.Cells[1, 5].Value = dictionary["TLTORDERREPORT"];
				worksheet.Cells[1, 5].Style.Font.Bold = true;
				worksheet.Cells[1, 5].Style.Font.Size = 15;
				worksheet.Cells[1, 5, 1, 7].Merge = true;
				worksheet.Cells[1, 5, 1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

				// write day
				if (intLanguague == 1)
				{
					worksheet.Cells[2, 5].Value = "Từ ngày " + (dateFrom != null ? Utilities.GetFormatDateReportByLanguage((DateTime)dateFrom, intLanguague) : "") + " đến ngày " + (dateTo != null ? Utilities.GetFormatDateReportByLanguage((DateTime)dateTo, intLanguague) : "");
				}
				else if (intLanguague == 2)
				{
					CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
					worksheet.Cells[2, 5].Value = "Period " + (dateFrom != null ? Utilities.GetFormatDateReportByLanguage((DateTime)dateFrom, intLanguague) : "") + " to " + (dateTo != null ? Utilities.GetFormatDateReportByLanguage((DateTime)dateTo, intLanguague) : "");
				}
				else
				{
					worksheet.Cells[2, 5].Value = (dateFrom != null ? Utilities.GetFormatDateReportByLanguage((DateTime)dateFrom, intLanguague) : "") + " から " + (dateTo != null ? Utilities.GetFormatDateReportByLanguage((DateTime)dateTo, intLanguague) : "") + "まで";
				}
				worksheet.Cells[2, 5, 2, 7].Merge = true;

				// Add the headers
				// column1
				worksheet.Cells[3, 1].Value = "#";
				worksheet.Cells[3, 1, 4, 1].Merge = true;
				// column2
				worksheet.Cells[3, 2].Value = dictionary["RPHDORDERNO"];
				worksheet.Cells[3, 2, 4, 2].Merge = true;

				worksheet.Cells[3, 3].Value = dictionary["RPHDORDERTYPE"];
				worksheet.Cells[3, 3, 4, 3].Merge = true;
				//worksheet.Column(2).Width = 30;
				// column3
				worksheet.Cells[3, 4].Value = dictionary["LBLORDERDATEDISPATCH"];
				worksheet.Cells[3, 4, 4, 4].Merge = true;
				// column4
				worksheet.Cells[3, 5].Value = dictionary["LBLCUSTOMERREPORT"];
				worksheet.Cells[3, 5, 4, 5].Merge = true;
				// column5
				worksheet.Cells[3, 6].Value = dictionary["LBLBLBKREPORT"];
				worksheet.Cells[3, 6, 4, 6].Merge = true;
				worksheet.Cells[3, 7].Value = dictionary["LBLJOBNO"];
				worksheet.Cells[3, 7, 4, 7].Merge = true;
				// column6
				worksheet.Cells[3, 8].Value = dictionary["MNUSHIPPINGCOMPANY"];
				worksheet.Cells[3, 8, 4, 8].Merge = true;
				//worksheet.Cells[3, 9].Value = dictionary["MNUVESSEL"];
				//worksheet.Cells[3, 9, 4, 9].Merge = true;
				// column7
				worksheet.Cells[3, 9].Value = dictionary["TLTROUTE"];
				worksheet.Cells[3, 9, 4, 9].Merge = true;
				// column8
				worksheet.Cells[3, 10].Value = dictionary["LBLCONTSIZE"];
				worksheet.Cells[4, 10].Value = "20'";
				worksheet.Cells[4, 11].Value = "40'";
				worksheet.Cells[4, 12].Value = "45'";
				worksheet.Cells[3, 10, 3, 12].Merge = true;
				// column10
				worksheet.Cells[3, 13].Value = dictionary["LBLNETWEIGHT"];
				worksheet.Cells[3, 13, 4, 13].Merge = true;
				// column10
				worksheet.Cells[3, 14].Value = dictionary["RPHDUNITPRICE"];
				worksheet.Cells[3, 14, 4, 14].Merge = true;
				// column11
				worksheet.Cells[3, 15].Value = dictionary["LBLTOL"];
				worksheet.Cells[3, 15, 4, 15].Merge = true;
				//set height header
				worksheet.Row(3).Height = 25;
				// set font bold
				worksheet.Cells[3, 1, 3, 15].Style.Font.Bold = true;
				worksheet.Row(4).Height = 25;
				// set font bold
				worksheet.Cells[4, 1, 4, 15].Style.Font.Bold = true;

				// set border
				worksheet.Cells[3, 1, 3, 15].Style.Border.Top.Style = ExcelBorderStyle.Thin;
				worksheet.Cells[3, 1, 3, 15].Style.Border.Left.Style = ExcelBorderStyle.Thin;
				worksheet.Cells[3, 1, 3, 15].Style.Border.Right.Style = ExcelBorderStyle.Thin;
				worksheet.Cells[3, 1, 3, 15].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

				// align center
				worksheet.Cells[3, 1, 3, 15].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Cells[3, 1, 3, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				worksheet.Cells[4, 1, 4, 15].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
				worksheet.Cells[4, 1, 4, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

				// set color
				worksheet.Cells[3, 1, 3, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
				worksheet.Cells[3, 1, 3, 15].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
				worksheet.Cells[4, 1, 4, 15].Style.Fill.PatternType = ExcelFillStyle.Solid;
				worksheet.Cells[4, 1, 4, 15].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

				//write content
				data = data.ToList();
				var indexRow = 5;
				var count = data.Count;
				double total = 0;
				double totalTax = 0;
				var startRow = 5;
				var index = 5;
				var consize1 = 0;
				var consize2 = 0;
				var consize3 = 0;
				int netw = 0;
				CultureInfo cult = CultureInfo.GetCultureInfo("vi-VN");
				if (data != null && data.Count > 0)
				{
					for (var iloop = 0; iloop < data.Count; iloop++)
					{
						
						
						worksheet.Cells[indexRow + iloop, 1].Value = iloop + 1;
						worksheet.Cells[indexRow + iloop, 2].Value = data[iloop].OrderNo;
						worksheet.Cells[indexRow + iloop, 3].Value = Utilities.GetOrderTypeName(data[iloop].OrderTypeI);
						worksheet.Cells[indexRow + iloop, 4].Value = Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].OrderD, intLanguague);
						worksheet.Cells[indexRow + iloop, 5].Value = data[iloop].CustomerShortN;
						worksheet.Cells[indexRow + iloop, 6].Value = data[iloop].BLBK;
						worksheet.Cells[indexRow + iloop, 7].Value = data[iloop].JobNo;
						worksheet.Cells[indexRow + iloop, 8].Value = data[iloop].ShippingCompanyN;
						//worksheet.Cells[indexRow + iloop, 8].Value = data[iloop].VesselN;

						DateTime oD = data[iloop].OrderD;
						string oNo = data[iloop].OrderNo;
						decimal? tolP = data[iloop].TotalPrice;
						decimal? uP = data[iloop].UnitPrice;
                        var route = "";
                        if (!String.IsNullOrWhiteSpace(data[iloop].LoadingPlaceN))
                            route += data[iloop].LoadingPlaceN + ", ";
                        if (!String.IsNullOrWhiteSpace(data[iloop].StopoverPlaceN))
                            route += data[iloop].StopoverPlaceN + ", ";
                        if (!String.IsNullOrWhiteSpace(data[iloop].DischargePlaceN))
                            route += data[iloop].DischargePlaceN;
					    if (string.IsNullOrEmpty(route))
					    {
                            var lord =
                            conV.FirstOrDefault(p =>
                                p.OrderD == oD && p.OrderNo == oNo && p.TotalPrice == tolP &&
                                p.UnitPrice == uP);
                            if (lord != null)
                            {
                                if (!String.IsNullOrWhiteSpace(lord.LocationDispatch1))
                                    route += lord.LocationDispatch1 + ", ";
                                if (!String.IsNullOrWhiteSpace(lord.LocationDispatch2))
                                    route += lord.LocationDispatch2 + ", ";
                                if (!String.IsNullOrWhiteSpace(lord.LocationDispatch3))
                                    route += lord.LocationDispatch3 ;
                            }
					    }
                        worksheet.Cells[indexRow + iloop, 9].Value = route;
						worksheet.Cells[indexRow + iloop, 10].Value = data[iloop].Quantity20HC;
						consize1 += (int)data[iloop].Quantity20HC;
						worksheet.Cells[indexRow + iloop, 11].Value = data[iloop].Quantity40HC;
						consize2 += (int)data[iloop].Quantity40HC;
						worksheet.Cells[indexRow + iloop, 12].Value = data[iloop].Quantity45HC;
						consize3 += (int)data[iloop].Quantity45HC;
						worksheet.Cells[indexRow + iloop, 13].Value = data[iloop].TotalLoads != null
							? ((decimal)data[iloop].TotalLoads).ToString("#,###.#", cult.NumberFormat)
							: "";
						if (data[iloop].TotalLoads != null)
						{
							netw += ((int)data[iloop].TotalLoads);
						}
						worksheet.Cells[indexRow + iloop, 14].Value = data[iloop].UnitPrice != null
							? ((decimal)data[iloop].UnitPrice).ToString("#,###", cult.NumberFormat)
							: "";
						worksheet.Cells[indexRow + iloop, 15].Value = data[iloop].TotalPrice !=null
							? ((decimal)data[iloop].TotalPrice).ToString("#,###", cult.NumberFormat)
							: "";
						if (data[iloop].TotalPrice != null)
						{
							total += ((double)data[iloop].TotalPrice);
						}
					}
				}
					
				worksheet.Cells[indexRow + count, 1].Value = dictionary["RPLBLTOTAL"];
				worksheet.Cells[indexRow + count, 1].Style.Font.Bold = true;
				worksheet.Cells[indexRow + count, 1].Style.Font.Size = 16;
				worksheet.Cells[indexRow + count, 1, indexRow + count, 2].Merge = true;
				//value consize
				worksheet.Cells[indexRow + count, 10].Value = consize1.ToString("#,###", cult.NumberFormat);
				worksheet.Cells[indexRow + count, 10].Style.Font.Bold = true;
				worksheet.Cells[indexRow + count, 10].Style.Font.Size = 16;

				worksheet.Cells[indexRow + count, 11].Value = consize2.ToString("#,###", cult.NumberFormat);
				worksheet.Cells[indexRow + count, 11].Style.Font.Bold = true;
				worksheet.Cells[indexRow + count, 11].Style.Font.Size = 16;

				worksheet.Cells[indexRow + count, 12].Value = consize3.ToString("#,###", cult.NumberFormat);
				worksheet.Cells[indexRow + count, 12].Style.Font.Bold = true;
				worksheet.Cells[indexRow + count, 12].Style.Font.Size = 16;

				//value net weight
				worksheet.Cells[indexRow + count, 13].Value = netw.ToString("#,###", cult.NumberFormat);
				worksheet.Cells[indexRow + count, 13].Style.Font.Bold = true;
				worksheet.Cells[indexRow + count, 13].Style.Font.Size = 16;

				//Value  sum 
				worksheet.Cells[indexRow + count, 15].Value = total.ToString("#,###", cult.NumberFormat);
				worksheet.Cells[indexRow + count, 15].Style.Font.Bold = true;
				worksheet.Cells[indexRow + count, 15].Style.Font.Size = 16;
				//Value sum Tax
				worksheet.Cells[indexRow + count, 9].Value = totalTax;
				worksheet.Cells[indexRow + count, 9].Style.Font.Bold = true;
				worksheet.Cells[indexRow + count, 9].Style.Font.Size = 16;
				//format currency
				worksheet.Cells[4, 8, indexRow + count, 9].Style.Numberformat.Format = "#,###";

				// set border
				worksheet.Cells[3, 1, indexRow + count, 15].Style.Border.Top.Style = ExcelBorderStyle.Thin;
				worksheet.Cells[3, 1, indexRow + count, 15].Style.Border.Left.Style = ExcelBorderStyle.Thin;
				worksheet.Cells[3, 1, indexRow + count, 15].Style.Border.Right.Style = ExcelBorderStyle.Thin;
				worksheet.Cells[3, 1, indexRow + count, 15].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

				//user
				// write day
				if (intLanguague == 1)
				{
					worksheet.Cells[indexRow + count + 1, 11].Value = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
																		DateTime.Now.Year;
				}
				else if (intLanguague == 2)
				{
					CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
					worksheet.Cells[indexRow + count + 1, 11].Value = cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " +
																DateTime.Now.Day + ", " + DateTime.Now.Year;
				}
				else
				{
					worksheet.Cells[indexRow + count + 1, 11].Value = DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day +
																		"日";
				}
				worksheet.Cells[indexRow + count + 1, 11, indexRow + count + 1, 15].Merge = true;
				worksheet.Cells[indexRow + count + 1, 11, indexRow + count + 1, 15].Style.HorizontalAlignment =
					ExcelHorizontalAlignment.Center;
				worksheet.Cells[indexRow + count + 2, 11].Value = dictionary["RPFTCREATOR"];
				worksheet.Cells[indexRow + count + 2, 11, indexRow + count + 2, 15].Merge = true;
				worksheet.Cells[indexRow + count + 2, 11, indexRow + count + 2, 15].Style.HorizontalAlignment =
					ExcelHorizontalAlignment.Center;

				worksheet.Cells[indexRow + count + 5, 11].Value = dictionary["RPFTPRINTBY"] + ": " + user + ", " + dictionary["RPFTPRINTTIME"] + ": " + DateTime.Now.ToString("HH:mm");
				worksheet.Cells[indexRow + count + 5, 11, indexRow + count + 5, 15].Merge = true;
				worksheet.Cells[indexRow + count + 5, 11, indexRow + count + 5, 15].Style.HorizontalAlignment =
					ExcelHorizontalAlignment.Center;
				// autoFit
				worksheet.Cells.AutoFitColumns();

				return new MemoryStream(package.GetAsByteArray());
			}
		}
	}
}
