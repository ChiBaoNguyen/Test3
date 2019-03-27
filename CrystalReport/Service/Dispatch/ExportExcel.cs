using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Website.Enum;
using Website.Utilities;
using Website.ViewModels.Dispatch;
using System.Globalization;

namespace CrystalReport.Service.Dispatch
{
	public class ExportExcel
	{
		//public static Stream GetContainerSizeList(List<ContainerSize_M> data, Dictionary<string, string> dicLanguage)
		public static Stream ExportDispatchListToExcel(List<DispatchListReport> data, Dictionary<string, string> dictionary, int intLanguague, string category,
														string companyName, string companyAddress, string fileName, string user)
		{
			try
			{
				using (ExcelPackage package = new ExcelPackage())
				{
					// add a new worksheet to the empty workbook
					ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("DispatchList");
					//Add the title
					worksheet.Cells[1, 1].Value = dictionary["TLTDISPATCHREPORT"];
					worksheet.Cells[1, 1].Style.Font.Bold = true;
					worksheet.Cells[1, 1].Style.Font.Size = 16;
					worksheet.Cells[1, 1, 1, 4].Merge = true;

					// write day
					if (intLanguague == 1)
					{
						worksheet.Cells[1, 13].Value = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
					}
					else if (intLanguague == 2)
					{
						CultureInfo cul = CultureInfo.GetCultureInfo("en-US");
						worksheet.Cells[1, 13].Value = cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year;
					}
					else
					{
						worksheet.Cells[1, 13].Value = DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日";
					}
					
					worksheet.Cells[1, 13, 1, 14].Merge = true;

					worksheet.Cells[2, 1].Value = category;
					worksheet.Cells[2, 1, 2, 4].Merge = true;

					// Add the headers
					// column1
					worksheet.Cells[3, 1].Value = "#";
					worksheet.Cells[3, 1, 4, 1].Merge = true;
					// column2
					worksheet.Cells[3, 2].Value = dictionary["LBLORDERDATEDISPATCH"];
					worksheet.Cells[3, 2, 4, 2].Merge = true;
					// column3
					worksheet.Cells[3, 3].Value = dictionary["LBLCUSTOMER"];
					worksheet.Cells[4, 3].Value = dictionary["LBLDISPATCHORDER"];
					// column4
					worksheet.Cells[3, 4].Value = dictionary["LBLORDERTYPEDISPATCH"];
					worksheet.Cells[4, 4].Value = dictionary["LBLTRANSPORTDATEDISPATCH"];
					// column5
					worksheet.Cells[3, 5].Value = dictionary["LBLORDERDEPARTMENTDISPATCH"];
					worksheet.Cells[4, 5].Value = dictionary["LBLTRUCKNODISPATCH"];
					// column6 + 7
					worksheet.Cells[3, 6].Value = "No";
					worksheet.Cells[3, 7].Value = dictionary["LBLBLBKDISPATCH"];
					worksheet.Cells[4, 6].Value = dictionary["LBLDRIVERDISPATCH"];
					worksheet.Cells[4, 6, 4, 7].Merge = true;
					// column8 + 9
					worksheet.Cells[3, 8].Value = dictionary["LBLSHIPPINGAGENCYDISPATCH"];
					worksheet.Cells[3, 9].Value = dictionary["LBLCONTAINERNODISPATCH"];
					worksheet.Cells[4, 8].Value = dictionary["LBLCONTAINERSTATUSDISPATCH"];
					worksheet.Cells[4, 8, 4, 9].Merge = true;
					// column10 + 11
					worksheet.Cells[3, 10].Value = dictionary["LBLLOADINGPLACEDISPATCH"];
					worksheet.Cells[3, 11].Value = dictionary["LBLLOADINGCUTOFFDISPATCH"];
					worksheet.Cells[4, 10].Value = dictionary["LBLLOCATION1DISPATCH"];
					worksheet.Cells[4, 10, 4, 11].Merge = true;
					// column12 + 13
					worksheet.Cells[3, 12].Value = dictionary["LBLDISCHARGEPLACEDISPATCH"];
					worksheet.Cells[3, 13].Value = dictionary["LBLDISCHARGECUTOFFDISPATCH"];
					worksheet.Cells[4, 12].Value = dictionary["LBLLOCATION2DISPATCH"];
					worksheet.Cells[4, 12, 4, 13].Merge = true;
					// column14 + 15
					worksheet.Cells[3, 14].Value = dictionary["LBLTRAILERNODISPATCH"];
					worksheet.Cells[3, 15].Value = dictionary["LBLLOADINGDATEDISPATCH"];
					worksheet.Cells[4, 14].Value = dictionary["LBLLOCATION3DISPATCH"];
					worksheet.Cells[4, 14, 4, 15].Merge = true;
					// column16
					worksheet.Cells[3, 16].Value = dictionary["LBLDISCHARGEDATEDISPATCH"];
					worksheet.Cells[4, 16].Value = dictionary["LBLTRANSPORT"];

					// set font bold
					worksheet.Cells[3, 1, 4, 16].Style.Font.Bold = true;

					// set border
					worksheet.Cells[3, 1, 4, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 4, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 4, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 4, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

					// align center
					worksheet.Cells[3, 1, 4, 16].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					worksheet.Cells[3, 1, 4, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					// set color
					worksheet.Cells[3, 1, 4, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells[3, 1, 4, 16].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
					

					// write content
					var startRow = 5;
					var index = 5;
					var total = 0;
					if (data != null && data.Count > 0)
					{
						var count = data.Count;
						for (var iloop = 0; iloop < count; iloop++)
						{
							// row1
							worksheet.Cells[index, 1].Value = iloop + 1;
							if (data[iloop].DispatchDList != null && data[iloop].DispatchDList.Count > 0)
							{
								// set total
								total = total + data[iloop].DispatchDList.Count;

								worksheet.Cells[index, 1, index + data[iloop].DispatchDList.Count, 1].Merge = true;
								worksheet.Cells[index, 2, index + data[iloop].DispatchDList.Count, 2].Merge = true;

								worksheet.Cells[index, 1, index + data[iloop].DispatchDList.Count, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[index, 1, index + data[iloop].DispatchDList.Count, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;

								if (iloop%2 != 0)
								{
									worksheet.Cells[index, 1, index + data[iloop].DispatchDList.Count, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
									worksheet.Cells[index, 1, index + data[iloop].DispatchDList.Count, 16].Style.Fill.BackgroundColor.SetColor(Color.BlanchedAlmond);
								}
							}
							worksheet.Cells[index, 1, index, 16].Style.Border.BorderAround(ExcelBorderStyle.Thin);
							worksheet.Cells[index, 1, index, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[index, 1, index, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[index, 1, index, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[index, 1, index, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

							if (iloop%2 != 0)
							{
								worksheet.Cells[index, 1, index, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
								worksheet.Cells[index, 1, index, 16].Style.Fill.BackgroundColor.SetColor(Color.BlanchedAlmond);
							}


							worksheet.Cells[index, 2].Value = Utilities.GetFormatDateReportByLanguage(data[iloop].OrderD, intLanguague);
							worksheet.Cells[index, 3].Value = data[iloop].CustomerShortN != "" ? data[iloop].CustomerShortN : data[iloop].CustomerN;
							worksheet.Cells[index, 4].Value = Utilities.GetOrderTypeName(data[iloop].OrderTypeI);
							worksheet.Cells[index, 5].Value = data[iloop].DepN;
							worksheet.Cells[index, 6].Value = data[iloop].OrderNo;
							worksheet.Cells[index, 7].Value = data[iloop].BLBK;
							worksheet.Cells[index, 8].Value = data[iloop].ShippingCompanyN;
							worksheet.Cells[index, 9].Value = data[iloop].ContainerNo;
							//worksheet.Cells[index, 10].Value = data[iloop].LoadingPlaceN;
							//worksheet.Cells[index, 11].Value = data[iloop].LoadingDT != null ? Utilities.GetFormatDateAndHourReportByLanguage((DateTime)data[iloop].LoadingDT, intLanguague) : "";
							//worksheet.Cells[index, 12].Value = data[iloop].DischargePlaceN;
							//worksheet.Cells[index, 13].Value = data[iloop].DischargeDT != null ? Utilities.GetFormatDateAndHourReportByLanguage((DateTime)data[iloop].DischargeDT, intLanguague) : "";
							worksheet.Cells[index, 14].Value = data[iloop].TrailerNo;
							//worksheet.Cells[index, 15].Value = data[iloop].ActualLoadingD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].ActualLoadingD, intLanguague) : "";
							//worksheet.Cells[index, 16].Value = data[iloop].ActualDischargeD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].ActualDischargeD, intLanguague) : "";

							// row2 to ...
							if (data[iloop].DispatchDList != null && data[iloop].DispatchDList.Count > 0)
							{
								var count2 = data[iloop].DispatchDList.Count;
								for (var jloop = 0; jloop < count2; jloop++)
								{
									//worksheet.Cells[index + jloop + 1, 3].Value = data[iloop].DispatchDList[jloop].DispatchNo;
									worksheet.Cells[index + jloop + 1, 3].Value = jloop + 1;
									worksheet.Cells[index + jloop + 1, 4].Value = data[iloop].DispatchDList[jloop].TransportD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].DispatchDList[jloop].TransportD, intLanguague) : "";
									worksheet.Cells[index + jloop + 1, 5].Value = data[iloop].DispatchDList[jloop].RegisteredNo;
									worksheet.Cells[index + jloop + 1, 6].Value = data[iloop].DispatchDList[jloop].DriverN;
									worksheet.Cells[index + jloop + 1, 6, index + jloop + 1, 7].Merge = true;
									//write content
									if (data[iloop].DispatchDList[jloop].ContainerStatus == Constants.NORMAL)
									{
										worksheet.Cells[index + jloop + 1, 8].Value = dictionary["LBLCONTAINERSTATUS1DISPATCH"];
									}
									else if (data[iloop].DispatchDList[jloop].ContainerStatus == Constants.LOAD)
									{
										worksheet.Cells[index + jloop + 1, 8].Value = dictionary["LBLCONTAINERSTATUS2DISPATCH"];
									}
									else
									{
										worksheet.Cells[index + jloop + 1, 8].Value = dictionary["LBLCONTAINERSTATUS3DISPATCH"];
									}
									
									worksheet.Cells[index + jloop + 1, 8, index + jloop + 1, 9].Merge = true;
									worksheet.Cells[index + jloop + 1, 10].Value = data[iloop].DispatchDList[jloop].Location1N;
									worksheet.Cells[index + jloop + 1, 10, index + jloop + 1, 11].Merge = true;
									worksheet.Cells[index + jloop + 1, 12].Value = data[iloop].DispatchDList[jloop].Location2N;
									worksheet.Cells[index + jloop + 1, 12, index + jloop + 1, 13].Merge = true;
									worksheet.Cells[index + jloop + 1, 14].Value = data[iloop].DispatchDList[jloop].Location3N;
									worksheet.Cells[index + jloop + 1, 14, index + jloop + 1, 15].Merge = true;
									//write dispatch status
									if (data[iloop].DispatchDList[jloop].DispatchStatus == Constants.NOTDISPATCH)
									{
										worksheet.Cells[index + jloop + 1, 16].Value = dictionary["LBLDISPATCHSTATUS1"];
									}
									else if (data[iloop].DispatchDList[jloop].DispatchStatus == Constants.DISPATCH)
									{
										worksheet.Cells[index + jloop + 1, 16].Value = dictionary["LBLDISPATCHSTATUS2"];
									}
									else if (data[iloop].DispatchDList[jloop].DispatchStatus == Constants.TRANSPORTED)
									{
										worksheet.Cells[index + jloop + 1, 16].Value = dictionary["LBLDISPATCHSTATUS3"];
									}
									else
									{
										worksheet.Cells[index + jloop + 1, 16].Value = dictionary["LBLDISPATCHSTATUS4"];
									}

									if (jloop + 1 < count2)
									{
										worksheet.Cells[index + jloop + 1, 3, index + jloop + 1, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
									}
								}

								index = index + count2 + 1;
							}
							else
							{
								index = index + 1;
							}
						}
					}

					// write line in final row
					worksheet.Cells[index - 1, 1, index - 1, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
					
					// write total
					worksheet.Cells[index + 1, 1].Value = dictionary["LBLDISPATCHTOTAL"];
					worksheet.Cells[index + 1, 1].Style.Font.Bold = true;
					worksheet.Cells[index + 1, 2].Value = total;
					worksheet.Cells[index + 1, 2].Style.Font.Bold = true;

					// autoFit
					worksheet.Cells.AutoFitColumns();

					//worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
					//worksheet.Cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
					//worksheet.Cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
					//worksheet.Cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

					////Add the headers
					//worksheet.Cells[3, 1].Value = dicLanguage["LBLCONTAINERSIZECODE"];
					//worksheet.Cells[3, 2].Value = dicLanguage["LBLCONTAINERSIZENAME"];
					//worksheet.Cells[3, 3].Value = dicLanguage["LBLDESCRIPTION"];
					//worksheet.Cells[3, 4].Value = dicLanguage["LBLSTATUS"];

					//iloop = 4;
					//foreach (var containerSizeM in containerSizeList)
					//{
					//	worksheet.Cells[iloop, 1].Value = containerSizeM.ContainerSizeC;
					//	worksheet.Cells[iloop, 2].Value = containerSizeM.ContainerSizeN;
					//	worksheet.Cells[iloop, 3].Value = containerSizeM.Description;
					//	worksheet.Cells[iloop, 4].Value = "Khả dụng";
					//	if (!containerSizeM.IsActive)
					//	{
					//		worksheet.Cells[iloop, 4].Value = "Không khả dụng";
					//	}
					//	iloop++;
					//}

					//worksheet.Cells[3, 1, iloop - 1, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
					//worksheet.Cells[3, 1, iloop - 1, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
					//worksheet.Cells[3, 1, iloop - 1, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
					//worksheet.Cells[3, 1, iloop - 1, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

					//worksheet.Cells[3, 1, iloop - 1, 4].AutoFitColumns();

					return new MemoryStream(package.GetAsByteArray());
				}
			}
			catch (NullReferenceException)
			{
				throw new NullReferenceException();
			}
		}
	}
}
