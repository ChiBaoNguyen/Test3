using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using Website.Enum;
using Website.Utilities;
using System.Globalization;
using Website.ViewModels.Report.CustomerExpense;
using System.Web;

namespace CrystalReport.Service.CustomerExpense
{
	public class ExportExcel
	{
		public static Stream ExportCustomerExpenseListToExcel(List<CustomerExpenseReportData> dataList, Dictionary<string, string> dictionary, int intLanguague, string fileName)
		{
			try
			{
				using (ExcelPackage package = new ExcelPackage())
				{
					// set culture
					CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
					if (intLanguague == 2)
					{
						cul = CultureInfo.GetCultureInfo("en-US");
					}
					else if (intLanguague == 3)
					{
						cul = CultureInfo.GetCultureInfo("ja-JP");
					}

					for (var i = 0; i < dataList.Count; i++)
					{
						if (dataList[i].CustomerExpenseList != null && dataList[i].CustomerExpenseList.Count > 0)
						{
							var data = dataList[i].CustomerExpenseList;
							var rowIndex = 1;
							var colIndex = 1;
							var tolCol = 18;
							// add a new worksheet to the empty workbook
							ExcelWorksheet worksheet =
								package.Workbook.Worksheets.Add(dataList[i].CustomerMainC + "_" + dataList[i].CustomerSubC + "_" + i);

							// add image
							//Bitmap image = new Bitmap(HttpContext.Current.Request.MapPath("~/Images/logo.bmp"));
							try
							{
								Bitmap image = new Bitmap(HttpContext.Current.Request.MapPath("~/Images/" + fileName));
								ExcelPicture excelImage = null;
								if (image != null)
								{
									excelImage = worksheet.Drawings.AddPicture("logo", image);
									excelImage.From.Column = 0;
									excelImage.From.Row = 0;
									excelImage.SetSize(100, 75);
								}
							}
							catch
							{
								// do nothing
							}
							#region Title
							//Add the title
							worksheet.Cells[rowIndex, 4].Value = dictionary.ContainsKey("COMPANYNAME") ? dictionary["COMPANYNAME"] : "";
							worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 4, rowIndex, 9].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 4].Value = dictionary.ContainsKey("COMPANYADDRESS") ? dictionary["COMPANYADDRESS"] : "";
							worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 4, rowIndex, 9].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 4].Value = (dictionary.ContainsKey("LBLTAXCODECOMPANYREPORT")
								? dictionary["LBLTAXCODECOMPANYREPORT"]
								: "") + ": " + (dictionary.ContainsKey("COMPANYTAXCODE") ? dictionary["COMPANYTAXCODE"] : "");
							worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 4, rowIndex, 6].Merge = true;

							// next row
							rowIndex = rowIndex + 2;

							worksheet.Cells[rowIndex, 5].Value = (dictionary.ContainsKey("TLTTRANSPORTFEEREPORT")
								? dictionary["TLTTRANSPORTFEEREPORT"]
								: "");
							worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 5].Style.Font.Size = 16;
							worksheet.Cells[rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 5, rowIndex, 12].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 5].Value = (dictionary.ContainsKey("LBLMONTHYEAR") ? dictionary["LBLMONTHYEAR"] : "");
							worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 5].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 5, rowIndex, 12].Merge = true;

							// next row
							rowIndex = rowIndex + 2;
							//LBLDEARREPORT
							//worksheet.Cells[rowIndex, 1].Value = (dictionary.ContainsKey("LBLCUSTCODE") ? dictionary["LBLCUSTCODE"] : "") +
							//									 ": " + dataList[i].CustomerMainC;
							//worksheet.Cells[rowIndex, 1].Style.Font.Size = 11;
							//worksheet.Cells[rowIndex, 1, rowIndex, 13].Merge = true;

							var transportRow = rowIndex;
							worksheet.Cells[rowIndex - 1, 15].Value = (dictionary.ContainsKey("LBLTRANSPORTFEEREPORT")
								? dictionary["LBLTRANSPORTFEEREPORT"]
								: "") + ": ";
							worksheet.Cells[rowIndex - 1, 15].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 15, rowIndex - 1, 16].Merge = true;

							// next row
							worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLCUSTOMER")
								? dictionary["LBLCUSTOMER"] : "")
								+ ": " + dataList[i].CustomerN;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 13].Merge = true;
							rowIndex = rowIndex + 1;
							if (dictionary.ContainsKey("COMPANYTAXRATE") && Convert.ToDecimal(dictionary["COMPANYTAXRATE"]) % 10 == 0)
							{
								worksheet.Cells[rowIndex - 1, 15].Value = (dictionary.ContainsKey("LBLTAXVAT") ? dictionary["LBLTAXVAT"] : "") + " " +
																	 Math.Round(dataList[i].TaxRate) + "%";
							}
							else
							{
								worksheet.Cells[rowIndex - 1, 15].Value = (dictionary.ContainsKey("LBLTAXVAT") ? dictionary["LBLTAXVAT"] : "") + " " +
																	 dataList[i].TaxRate + "%";
							}

							worksheet.Cells[rowIndex - 1, 15].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 15, rowIndex - 1, 16].Merge = true;

							// next row
							worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLADDRESS")
								? dictionary["LBLADDRESS"] : "")
								+ ": " + dataList[i].CustomerAddress;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 13].Merge = true;
							rowIndex = rowIndex + 1;
							var transportTaxRow = rowIndex;
							worksheet.Cells[rowIndex - 1, 15].Value = (dictionary.ContainsKey("LBLTRANSPORTFEEAFTERTAX")
								? dictionary["LBLTRANSPORTFEEAFTERTAX"]
								: "") + ":";
							worksheet.Cells[rowIndex - 1, 15].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 15, rowIndex - 1, 16].Merge = true;

							// next row
							worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLTAXCODECOMPANYREPORT")
								? dictionary["LBLTAXCODECOMPANYREPORT"] : "")
								+ ": " + dataList[i].CustomerTaxCode;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 13].Merge = true;
							rowIndex = rowIndex + 1;
							var expenseRow = rowIndex;
							worksheet.Cells[rowIndex - 1, 15].Value = (dictionary.ContainsKey("LBLEXPENSEREPORT")
								? dictionary["LBLEXPENSEREPORT"]
								: "") + ":";
							worksheet.Cells[rowIndex - 1, 15].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 15, rowIndex - 1, 16].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							var totalRow = rowIndex;
							worksheet.Cells[rowIndex - 1, 15].Value = (dictionary.ContainsKey("LBLTOTALAMOUNTREPORT")
								? dictionary["LBLTOTALAMOUNTREPORT"]
								: "") + ":";
							worksheet.Cells[rowIndex - 1, 15].Style.Font.Bold = true;
							//worksheet.Cells[rowIndex, 15].Style.Font.Size = 14;
							worksheet.Cells[rowIndex - 1, 15, rowIndex - 1, 16].Merge = true;
							#endregion

							// next row
							rowIndex = rowIndex + 1;

							#region Header
							// Add the headers
							colIndex = 1;
							// column1
							worksheet.Cells[rowIndex, colIndex].Value = "#";
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column2
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLORDERTYPEDISPATCH")
								? dictionary["LBLORDERTYPEDISPATCH"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column3
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLLOADINGDATEDISPATCH")
								? dictionary["LBLLOADINGDATEDISPATCH"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column4
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLDISCHARGEDATEDISPATCH")
								? dictionary["LBLDISCHARGEDATEDISPATCH"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							////
							//worksheet.Cells[rowIndex, 4].Value = (dictionary.ContainsKey("LBLDETAINDAYREPORT")
							//	? dictionary["LBLDETAINDAYREPORT"]
							//	: "");
							//worksheet.Cells[rowIndex, 4].Style.WrapText = true;
							//worksheet.Cells[rowIndex, 4, rowIndex + 1, 4].Merge = true;
							//column5
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLBLBKREPORT")
								? dictionary["LBLBLBKREPORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column6
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLCONTNUMBER")
								? dictionary["LBLCONTNUMBER"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column7-10
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLCONTAINERTYPEREPORT")
								? dictionary["LBLCONTAINERTYPEREPORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							// column11-13
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLLOCATIONREPORT")
								? dictionary["LBLLOCATIONREPORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex + 2].Merge = true;
							// column14
							colIndex = colIndex + 3;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLTRUCKNO")
								? dictionary["LBLTRUCKNO"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column15-17
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLTRANSPORTFEEREPORT")
								? dictionary["LBLTRANSPORTFEEREPORT"] : "");
							worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 2].Merge = true;
							worksheet.Cells[rowIndex + 1, colIndex].Value = (dictionary.ContainsKey("LBLCHARGETRANSPORT")
								? dictionary["LBLCHARGETRANSPORT"] : "");
							worksheet.Cells[rowIndex + 1, colIndex + 1].Value = (dictionary.ContainsKey("LBLSURCHARGEREPORT")
								? dictionary["LBLSURCHARGEREPORT"] : "");
							worksheet.Cells[rowIndex + 1, colIndex + 2].Value = (dictionary.ContainsKey("LBLTOTALREPORT")
								? dictionary["LBLTOTALREPORT"] : "");
							// column18-20
							colIndex = colIndex + 3;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLEXPENSEREPORT")
								? dictionary["LBLEXPENSEREPORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 2].Merge = true;
							worksheet.Cells[rowIndex + 1, colIndex].Value = (dictionary.ContainsKey("LBLEXPENSETYPEREPORT")
								? dictionary["LBLEXPENSETYPEREPORT"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 1].Value = (dictionary.ContainsKey("LBLAMOUNTREPORT")
								? dictionary["LBLAMOUNTREPORT"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 2].Value = (dictionary.ContainsKey("LBLINVOICENOREPORT")
								? dictionary["LBLINVOICENOREPORT"]
								: "");
							// column21
							colIndex = colIndex + 3;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLDESCRIPTIONREPORT")
								? dictionary["LBLDESCRIPTIONREPORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;

							// column18
							//worksheet.Cells[rowIndex, 18].Value = (dictionary.ContainsKey("LBLTAXAMOUNT")
							//	? dictionary["LBLTAXAMOUNT"]
							//	: "");
							//worksheet.Cells[rowIndex, 18, rowIndex + 1, 18].Merge = true;
							// column19
							//worksheet.Cells[rowIndex, 19].Value = (dictionary.ContainsKey("LBLTOTALREPORT")
							//	? dictionary["LBLTOTALREPORT"]
							//	: "");
							//worksheet.Cells[rowIndex, 19, rowIndex + 1, 19].Merge = true;

							// set font bold
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Font.Bold = true;
							// set border
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							// align center
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							// set color
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
							#endregion

							// next row
							rowIndex = rowIndex + 2;
							var rowSumStart = rowIndex;

							// write content
							if (data != null && data.Count > 0)
							{
								var keyNew = "";
								var keyOld = "";
								//decimal totalColumn19 = 0;
								decimal totalTransport = 0;
								decimal totalDetain = 0;
								decimal totalExpense = 0;
								decimal totalTax = 0;
								int rowOfGroup = 0;
								int index = 1;
								for (int iloop = 0; iloop < data.Count; iloop++)
								{
									#region Detail
									worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
									worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

									keyNew = data[iloop].OrderD.OrderD + data[iloop].OrderD.OrderNo + data[iloop].OrderD.DetailNo;
									if (keyNew != keyOld)
									{
										worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;

										rowOfGroup = 1;
										keyOld = keyNew;
										// cal detention day
										//var detetionDay = (data[iloop].DetainDay - beginDetetionDay + 1) > 0
										//			? (data[iloop].DetainDay - beginDetetionDay + 1)
										//			: 0;
										// column1
										colIndex = 1;
										worksheet.Cells[rowIndex, colIndex].Value = index++;
										worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
										worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										// column2
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Value = Utilities.GetOrderTypeName(data[iloop].OrderH.OrderTypeI);
										// column3
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "dd/MM/yy";
										if (intLanguague == 2)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "MM/dd/yy";
										}
										else if (intLanguague == 3)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "yy/MM/dd";
										}
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.ActualLoadingD;
										// column4
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "dd/MM/yy";
										if (intLanguague == 2)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "MM/dd/yy";
										}
										else if (intLanguague == 3)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "yy/MM/dd";
										}
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.ActualDischargeD;
										// column4
										//worksheet.Cells[rowIndex, 4].Value = "";
										//if (data[iloop].DetainDay != 0)
										//{
										//	worksheet.Cells[rowIndex, 4].Value = data[iloop].DetainDay;
										//}
										//worksheet.Cells[rowIndex, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
										//worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										// column5
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderH.BLBK;
										// column6
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.ContainerNo;
										worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
										// column7-10
										colIndex++;
										if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE1)
										{
											worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE1N;
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										else if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE2)
										{
											worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE2N;
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										else if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE3)
										{
											worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE3N;
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										else
										{
											worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLLOAD"] + (data[iloop].OrderD.NetWeight);
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										// column11-13
										colIndex++;
										if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch1))
										{
											worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.LocationDispatch1;
											worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
										}
										if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch2))
										{
											worksheet.Cells[rowIndex, colIndex + 1].Value = data[iloop].OrderD.LocationDispatch2;
											worksheet.Cells[rowIndex, colIndex + 1].Style.WrapText = true;
										}
										if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch3))
										{
											worksheet.Cells[rowIndex, colIndex + 2].Value = data[iloop].OrderD.LocationDispatch3;
											worksheet.Cells[rowIndex, colIndex + 2].Style.WrapText = true;
										}
										// column14
										colIndex = colIndex + 3;
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.RegisteredNo;
										// column 15
										colIndex++;
										if (data[iloop].OrderD.Amount != null)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
											worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.Amount;
										}
										// column 16
										colIndex++;
										if (data[iloop].DetainAmount + data[iloop].SurchargeAmount != 0)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
											worksheet.Cells[rowIndex, colIndex].Value = data[iloop].DetainAmount + data[iloop].SurchargeAmount;
											totalDetain = totalDetain + data[iloop].DetainAmount + data[iloop].SurchargeAmount;
										}
										// column 17
										colIndex++;
										if (data[iloop].OrderD.Amount != null || data[iloop].DetainAmount + data[iloop].SurchargeAmount != 0)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
											worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.Amount + data[iloop].DetainAmount + data[iloop].SurchargeAmount;
										}
										//column 18
										//if (data[iloop].TaxAmount != 0)
										//{
										//	worksheet.Cells[rowIndex, 18].Style.Numberformat.Format = "#,##0";
										//	worksheet.Cells[rowIndex, 18].Value = data[iloop].TaxAmount;
										//}

										// cal totalColumn19
										//totalColumn19 = 0;
										if (data[iloop].OrderD.Amount != null)
										{
											totalTransport = totalTransport + (decimal)data[iloop].OrderD.Amount;

											//	//if (dataList[i].TaxMethodI == "0")
											//	//{
											//	//	totalColumn19 = totalColumn19 +
											//	//					((decimal)data[iloop].OrderD.Amount + data[iloop].DetainAmount +
											//	//					 data[iloop].SurchargeAmount) * (100 + dataList[i].TaxRate) / 100;
											//	//}
											//	//else
											//	//{
											//	//	totalTax = totalTax +
											//	//			   Utilities.CalByMethodRounding(
											//	//				   ((decimal)data[iloop].OrderD.Amount + data[iloop].DetainAmount +
											//	//					data[iloop].SurchargeAmount) * (dataList[i].TaxRate) / 100, dataList[i].TaxRoundingI);
											//	//	totalColumn19 = totalColumn19 +
											//	//					Utilities.CalByMethodRounding(
											//	//						((decimal)data[iloop].OrderD.Amount + data[iloop].DetainAmount +
											//	//						 data[iloop].SurchargeAmount) * (100 + dataList[i].TaxRate) / 100, dataList[i].TaxRoundingI);
											//	//}
											totalTax += data[iloop].TaxAmount;
											//	totalColumn19 += (decimal)(data[iloop].OrderD.Amount + data[iloop].DetainAmount + data[iloop].SurchargeAmount + data[iloop].TaxAmount);
										}
										//if (data[iloop].ExpenseD.Amount != null)
										//{
										//	totalColumn19 = totalColumn19 + (decimal) data[iloop].ExpenseD.Amount;
										//}
										// column 20
										worksheet.Cells[rowIndex, tolCol].Value = data[iloop].Description;
										worksheet.Cells[rowIndex, tolCol].Style.WrapText = true;
									}
									else
									{
										//if (data[iloop].ExpenseD.Amount != null)
										//{
										//	totalColumn19 = totalColumn19 + (decimal) data[iloop].ExpenseD.Amount;
										//}

										rowOfGroup = rowOfGroup + 1;
									}

									// write total
									//if (iloop + 1 < data.Count)
									//{
									//	var keyCheck = data[iloop + 1].OrderD.OrderD + data[iloop + 1].OrderD.OrderNo +
									//				   data[iloop + 1].OrderD.DetailNo;
									//	if (keyNew != keyCheck)
									//	{
									//		if (totalColumn19 > 0)
									//		{
									//			worksheet.Cells[rowIndex - rowOfGroup + 1, 19].Style.Numberformat.Format = "#,##0";
									//			worksheet.Cells[rowIndex - rowOfGroup + 1, 19].Value = totalColumn19;
									//		}
									//	}
									//}
									//else
									//{
									//	if (totalColumn19 > 0)
									//	{
									//		worksheet.Cells[rowIndex - rowOfGroup + 1, 19].Style.Numberformat.Format = "#,##0";
									//		worksheet.Cells[rowIndex - rowOfGroup + 1, 19].Value = totalColumn19;
									//	}
									//}

									// column 18
									worksheet.Cells[rowIndex, 15].Value = data[iloop].ExpenseD.ExpenseN;
									worksheet.Cells[rowIndex, 15].Style.WrapText = true;
									// column 19
									// cal totalExpense
									if (data[iloop].ExpenseD.Amount != null)
									{
										if (data[iloop].ExpenseD.Amount != null && data[iloop].ExpenseD.Amount > 0)
										{
											worksheet.Cells[rowIndex, 16].Style.Numberformat.Format = "#,##0";
											worksheet.Cells[rowIndex, 16].Value = data[iloop].ExpenseD.Amount;
											totalExpense = totalExpense + (decimal)data[iloop].ExpenseD.Amount;
										}
									}
									// column 20
									worksheet.Cells[rowIndex, 17].Value = data[iloop].ExpenseD.Description;

									//if (iloop + 1 == data.Count)
									//{
									//	worksheet.Cells[rowIndex, 1, rowIndex, 19].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
									//}
									// next row
									rowIndex = rowIndex + 1;
									#endregion
								}
								worksheet.Cells[15, 1, rowIndex, 18].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
								#region Title Sum
								// write total
								worksheet.Cells[transportRow - 1, 17].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[transportRow - 1, 17].Value = totalTransport + totalDetain;

								// tax
								worksheet.Cells[transportRow, 17].Style.Numberformat.Format = "#,##0";
								//if (dataList[i].TaxMethodI == "0")
								//{
								//	worksheet.Cells[transportRow + 1, 6].Value =
								//		Utilities.CalByMethodRounding((totalTransport + totalDetain)*dataList[i].TaxRate/100, dataList[i].TaxRoundingI);
								//}
								//else
								//{
								worksheet.Cells[transportRow, 17].Value = totalTax;
								//}

								// after tax
								worksheet.Cells[transportTaxRow - 1, 17].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[transportTaxRow - 1, 17].Formula = "SUM(Q" + (transportRow - 1) + ":Q" + (transportRow) + ")";

								worksheet.Cells[expenseRow - 1, 17].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[expenseRow - 1, 17].Value = totalExpense;
								worksheet.Cells[totalRow - 1, 17].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[totalRow - 1, 17].Formula = "SUM(Q" + (transportTaxRow - 1) + ":Q" + (expenseRow - 1) + ")";
								worksheet.Cells[totalRow - 1, 17].Style.Font.Bold = true;
								//worksheet.Cells[totalRow - 1, 17].Style.Font.Size = 14;
								#endregion
							}

							// add total end
							worksheet.Cells[rowIndex, 1].Value = (dictionary.ContainsKey("LBLTOTALREPORT")
								? dictionary["LBLTOTALREPORT"]
								: "");
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 1, rowIndex, 6].Merge = true;

							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.BorderAround(ExcelBorderStyle.Thin);
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

							worksheet.Cells[rowIndex, 7].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[rowIndex, 8].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[rowIndex, 9].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[rowIndex, 10].Style.Numberformat.Format = "#,##0.0";
							worksheet.Cells[rowIndex, 15].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[rowIndex, 16].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[rowIndex, 17].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[rowIndex, 19].Style.Numberformat.Format = "#,##0";
							//worksheet.Cells[rowIndex, 19].Style.Numberformat.Format = "#,##0";

							worksheet.Cells[rowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

							var sumstr = "SUM(L" + rowSumStart + ":L" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 12].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

							sumstr = "SUM(M" + rowSumStart + ":M" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 13].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

							sumstr = "SUM(N" + rowSumStart + ":N" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 14].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

							sumstr = "SUM(P" + rowSumStart + ":P" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 16].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

							// total17
							//if (dataList[i].TaxMethodI == "0")
							//{
							//	if (dataList[i].TaxRoundingI == "0")
							//	{
							//		sumstr = "ROUND(SUM(S" + rowSumStart + ":S" + (rowIndex - 1) + "), 0)";
							//	}
							//	else if (dataList[i].TaxRoundingI == "1")
							//	{
							//		sumstr = "ROUNDUP(SUM(S" + rowSumStart + ":S" + (rowIndex - 1) + "), 0)";
							//	}
							//	else
							//	{
							//		sumstr = "ROUNDDOWN(SUM(S" + rowSumStart + ":S" + (rowIndex - 1) + "), 0)";
							//	}
							//	worksheet.Cells[rowIndex, 19].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";
							//}
							//else
							//{
							//	sumstr = "SUM(S" + rowSumStart + ":S" + (rowIndex - 1) + ")";
							//	worksheet.Cells[rowIndex, 19].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";
							//}

							// write day
							if (intLanguague == 1)
							{
								worksheet.Cells[rowIndex + 1, 15].Value = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
																		  DateTime.Now.Year;
							}
							else if (intLanguague == 2)
							{
								cul = CultureInfo.GetCultureInfo("en-US");
								worksheet.Cells[rowIndex + 1, 15].Value = cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " +
																		  DateTime.Now.Day + ", " + DateTime.Now.Year;
							}
							else
							{
								worksheet.Cells[rowIndex + 1, 15].Value = DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day +
																		  "日";
							}

							// autoFit
							worksheet.Column(1).Width = 3.5;
							worksheet.Column(2).Width = 5;
							worksheet.Column(3).Width = 9;
							worksheet.Column(4).Width = 9;
							worksheet.Column(5).Width = 14;
							worksheet.Column(6).Width = 10;
							worksheet.Column(7).Width = 5;
							worksheet.Column(8).Width = 11;
							worksheet.Column(9).Width = 11;
							worksheet.Column(10).Width = 11;
							worksheet.Column(11).Width = 10;
							worksheet.Column(12).AutoFit();
							worksheet.Column(13).AutoFit();
							worksheet.Column(14).AutoFit();
							worksheet.Column(15).Width = 14;
							worksheet.Column(16).AutoFit();
							worksheet.Column(17).AutoFit();
							worksheet.Column(18).AutoFit();
						}
					}

					if (package.Workbook.Worksheets.Count == 0)
					{
						package.Workbook.Worksheets.Add("NoData");
					}

					return new MemoryStream(package.GetAsByteArray());
				}
			}
			catch (NullReferenceException)
			{
				throw new NullReferenceException();
			}
		}

		public static Stream ExportExcelCustomerExpenseLOLOHorizontal(List<CustomerExpenseReportData> dataList, Dictionary<string, string> dictionary, int intLanguague, string fileName)
		{
			
				using (ExcelPackage package = new ExcelPackage())
				{
					// set culture
					CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
					if (intLanguague == 2)
					{
						cul = CultureInfo.GetCultureInfo("en-US");
					}
					else if (intLanguague == 3)
					{
						cul = CultureInfo.GetCultureInfo("ja-JP");
					}

					for (var i = 0; i < dataList.Count; i++)
					{
						if (dataList[i].CustomerExpenseList != null && dataList[i].CustomerExpenseList.Count > 0)
						{
							var data = dataList[i].CustomerExpenseList;
							var rowIndex = 1;
							var colIndex = 1;
							var tolCol = 16;
							// add a new worksheet to the empty workbook
							ExcelWorksheet worksheet =
								package.Workbook.Worksheets.Add(dataList[i].CustomerMainC + "_" + dataList[i].CustomerSubC + "_" + dataList[i].CustomerPayLiftLoweredMainC + "_" + dataList[i].CustomerPayLiftLoweredSubC + "_" + i);

							// add image
							//Bitmap image = new Bitmap(HttpContext.Current.Request.MapPath("~/Images/logo.bmp"));
							try
							{
								Bitmap image = new Bitmap(HttpContext.Current.Request.MapPath("~/Images/" + fileName));
								ExcelPicture excelImage = null;
								if (image != null)
								{
									excelImage = worksheet.Drawings.AddPicture("logo", image);
									excelImage.From.Column = 0;
									excelImage.From.Row = 0;
									excelImage.SetSize(100, 75);
								}
							}
							catch
							{
								// do nothing
							}
							#region Title
							//Add the title
							worksheet.Cells[rowIndex, 4].Value = dictionary.ContainsKey("COMPANYNAME") ? dictionary["COMPANYNAME"] : "";
							worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 4, rowIndex, 9].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 4].Value = dictionary.ContainsKey("COMPANYADDRESS") ? dictionary["COMPANYADDRESS"] : "";
							worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 4, rowIndex, 9].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 4].Value = (dictionary.ContainsKey("LBLTAXCODECOMPANYREPORT")
								? dictionary["LBLTAXCODECOMPANYREPORT"]
								: "") + ": " + (dictionary.ContainsKey("COMPANYTAXCODE") ? dictionary["COMPANYTAXCODE"] : "");
							worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 4, rowIndex, 6].Merge = true;

							// next row
							rowIndex = rowIndex + 2;

							worksheet.Cells[rowIndex, 5].Value = (dictionary.ContainsKey("TLTLOLOFEEREPORT")
								? dictionary["TLTLOLOFEEREPORT"]
								: "");
							worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 5].Style.Font.Size = 19;
							worksheet.Cells[rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 5, rowIndex, 12].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 5].Value = (dictionary.ContainsKey("LBLMONTHYEAR") ? dictionary["LBLMONTHYEAR"] : "");
							worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 5].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 5, rowIndex, 12].Merge = true;

							// next row
							//left
							rowIndex = rowIndex + 2;

							//right
							worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLDEARREPORT")
								? dictionary["LBLDEARREPORT"] : "") + ": " + dataList[i].InvoiceN;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Bold = true;
							worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 13].Merge = true;

							// next row
							//left
							rowIndex = rowIndex + 1;
							if (!string.IsNullOrEmpty(dataList[i].CustomerPayLiftLoweredN) && dataList[i].CustomerPayLiftLoweredN != "")
							{
								worksheet.Cells[rowIndex - 1, 9].Value = (dictionary.ContainsKey("LBLCUSTOMERPAYLIFTORLOWERED")
								? dictionary["LBLCUSTOMERPAYLIFTORLOWERED"] : "")
								+ ": " + dataList[i].CustomerPayLiftLoweredN;
								worksheet.Cells[rowIndex - 1, 9].Style.Font.Size = 11;
								worksheet.Cells[rowIndex - 1, 9, rowIndex - 1, 16].Merge = true;
							}

							// right
							worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLCUSTOMER")
								? dictionary["LBLCUSTOMER"] : "")
								+ ": " + dataList[i].CustomerN;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 8].Merge = true;
							//net row
							//left
							rowIndex = rowIndex + 1;
							if (!string.IsNullOrEmpty(dataList[i].CustomerPayLiftLoweredAddress) && dataList[i].CustomerPayLiftLoweredAddress != "")
							{
								worksheet.Cells[rowIndex - 1, 9].Value = (dictionary.ContainsKey("LBLADDRESS")
								? dictionary["LBLADDRESS"]
								: "") + ": " + dataList[i].CustomerPayLiftLoweredAddress;
								worksheet.Cells[rowIndex - 1, 9].Style.Font.Size = 11;
								worksheet.Cells[rowIndex - 1, 9, rowIndex - 1, 16].Merge = true;
							}

							// right
							worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLADDRESS")
								? dictionary["LBLADDRESS"] : "")
								+ ": " + dataList[i].CustomerAddress;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 8].Merge = true;
							#endregion

							// next row
							rowIndex = rowIndex + 1;

							#region Header
							// Add the headers
							colIndex = 1;
							// column1
							worksheet.Cells[rowIndex, colIndex].Value = "#";
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column2
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLORDERTYPEDISPATCH")
								? dictionary["LBLORDERTYPEDISPATCH"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column3
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLDATEREPORT")
								? dictionary["LBLDATEREPORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							//column4
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLBLBKREPORT")
								? dictionary["LBLBLBKREPORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column5
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLCONTNUMBER")
								? dictionary["LBLCONTNUMBER"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column6
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLCATEGORY")
								? dictionary["LBLCATEGORY"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							// column7-9
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLSTOPOVERPLACESHORT")
								? dictionary["LBLSTOPOVERPLACESHORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLLOADINGPLACESHORT")
								? dictionary["LBLLOADINGPLACESHORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLDISCHARGEPLACESHORT")
								? dictionary["LBLDISCHARGEPLACESHORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							// column10
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLTRUCKNO")
								? dictionary["LBLTRUCKNO"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column11-16
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLISREQUESTEDRP")
								? dictionary["LBLISREQUESTEDRP"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 5].Merge = true;
							worksheet.Cells[rowIndex + 1, colIndex].Value = (dictionary.ContainsKey("LBLLIFT")
								? dictionary["LBLLIFT"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 1].Value = (dictionary.ContainsKey("LBLBILLLIFTON")
								? dictionary["LBLBILLLIFTON"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 2].Value = (dictionary.ContainsKey("LBLLOWERED")
								? dictionary["LBLLOWERED"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 3].Value = (dictionary.ContainsKey("LBLBILLLIFTOFF")
								? dictionary["LBLBILLLIFTOFF"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 4].Value = (dictionary.ContainsKey("LBLOTHER")
								? dictionary["LBLOTHER"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 5].Value = (dictionary.ContainsKey("LBLEXPLAIN")
								? dictionary["LBLEXPLAIN"]
								: "");
							// set font bold
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Font.Bold = true;
							// set border
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							// align center
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							// set color
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
							#endregion

							// next row
							rowIndex = rowIndex + 2;
							var rowSumStart = rowIndex;

							// write content
							if (data != null && data.Count > 0)
							{
								var keyNew = "";
								var keyOld = "";
								//decimal totalColumn19 = 0;
								int rowOfGroup = 0;
								int index = 1;
								for (int iloop = 0; iloop < data.Count; iloop++)
								{
									#region Detail
									worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
									worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

									keyNew = data[iloop].OrderD.OrderD + data[iloop].OrderD.OrderNo + data[iloop].OrderD.DetailNo;
									if (keyNew != keyOld)
									{
										worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;

										rowOfGroup = 1;
										keyOld = keyNew;
										// column1
										colIndex = 1;
										worksheet.Cells[rowIndex, colIndex].Value = index++;
										worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
										worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										// column2
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Value = Utilities.GetOrderTypeName(data[iloop].OrderH.OrderTypeI);
										// column3
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "dd/MM/yy";
										if (intLanguague == 2)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "MM/dd/yy";
										}
										else if (intLanguague == 3)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "yy/MM/dd";
										}
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.ActualLoadingD;
										// column4
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderH.BLBK;
										// column5
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.ContainerNo;
										worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
										// column6
										colIndex++;
										if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE1)
										{
											worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE1N;
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										else if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE2)
										{
											worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE2N;
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										else if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE3)
										{
											worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE3N;
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										else
										{
											worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLLOAD"] + (data[iloop].OrderD.NetWeight);
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										// column7-9
										colIndex++;
										if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch1))
										{
											worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.LocationDispatch1;
											worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
										}
										if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch2))
										{
											worksheet.Cells[rowIndex, colIndex + 1].Value = data[iloop].OrderD.LocationDispatch2;
											worksheet.Cells[rowIndex, colIndex + 1].Style.WrapText = true;
										}
										if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch3))
										{
											worksheet.Cells[rowIndex, colIndex + 2].Value = data[iloop].OrderD.LocationDispatch3;
											worksheet.Cells[rowIndex, colIndex + 2].Style.WrapText = true;
										}
										// column10
										colIndex = colIndex + 3;
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.RegisteredNo;
										worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
										//chi ho
										//column11,12
										var checkcanrow = 0;
										int rowIndexRoot = rowIndex;
										if (data[iloop].ExpenseD.LiftOnList != null && data[iloop].ExpenseD.LiftOnList.Count > 0)
										{
											for (int li = 0; li < data[iloop].ExpenseD.LiftOnList.Count; li++)
											{
												if (data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn != null &&
												    data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn > 0)
												{
													worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0";
													worksheet.Cells[rowIndex, 11].Value = data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn;
												}
												worksheet.Cells[rowIndex, 12].Value = data[iloop].ExpenseD.LiftOnList[li].DescriptionLiftOn;
												worksheet.Cells[rowIndex, 12].Style.WrapText = true;
												rowIndex++;
												checkcanrow++;
												worksheet.Cells[rowIndex, 11, rowIndex, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
												worksheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
												worksheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
											}
										}
										//column13,14
										if (data[iloop].ExpenseD.LiftOffList != null && data[iloop].ExpenseD.LiftOffList.Count > 0)
										{
											rowIndex = data[iloop].ExpenseD.LiftOnList != null
												? rowIndex - data[iloop].ExpenseD.LiftOnList.Count
												: rowIndex;
											for (int lj = 0; lj < data[iloop].ExpenseD.LiftOffList.Count; lj++)
											{
												if (data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff != null &&
													data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff > 0)
												{
													worksheet.Cells[rowIndex, 13].Style.Numberformat.Format = "#,##0";
													worksheet.Cells[rowIndex, 13].Value = data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff;
												}
												worksheet.Cells[rowIndex, 14].Value = data[iloop].ExpenseD.LiftOffList[lj].DescriptionLiftOff;
												worksheet.Cells[rowIndex, 14].Style.WrapText = true;
												rowIndex++;
												checkcanrow++;
												worksheet.Cells[rowIndex, 11, rowIndex, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
												worksheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
												worksheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
											}
										}
										//column15,16
										if (data[iloop].ExpenseD.OtherListLoLo != null && data[iloop].ExpenseD.OtherListLoLo.Count > 0)
										{
											string description = "";
											decimal amount = 0;
											rowIndex = data[iloop].ExpenseD.LiftOffList != null
												? rowIndex - data[iloop].ExpenseD.LiftOffList.Count
												: rowIndex;
											for (int lk = 0; lk < data[iloop].ExpenseD.OtherListLoLo.Count; lk++)
											{
												if (data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther != null &&
													data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther > 0)
												{
													description = (data[iloop].ExpenseD.OtherListLoLo[lk].DescriptionOther != "" &&
													               !string.IsNullOrEmpty(data[iloop].ExpenseD.OtherListLoLo[lk].DescriptionOther))
														? (((description != "") ? (description + ",") : "") +
														   data[iloop].ExpenseD.OtherListLoLo[lk].DescriptionOther)
														: description;
													amount = amount + (decimal)data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther;
												}
											}
											worksheet.Cells[rowIndex, 15].Style.Numberformat.Format = "#,##0";
											worksheet.Cells[rowIndex, 15].Value = amount;
											worksheet.Cells[rowIndex, 16].Value = description;
											worksheet.Cells[rowIndex, 16].Style.WrapText = true;
											rowIndex++;
											checkcanrow++;
											worksheet.Cells[rowIndex, 11, rowIndex, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
											worksheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
											worksheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
										}
										if (checkcanrow < 1)
										{
											rowIndex = rowIndex + 1;
										}
										else
										{
											rowIndex = Math.Max(Math.Max((rowIndexRoot + (data[iloop].ExpenseD.LiftOnList != null ? data[iloop].ExpenseD.LiftOnList.Count : 0)),
												(rowIndexRoot + (data[iloop].ExpenseD.LiftOffList != null ? data[iloop].ExpenseD.LiftOffList.Count : 0))),
												(rowIndexRoot + (data[iloop].ExpenseD.OtherListLoLo != null ? data[iloop].ExpenseD.OtherListLoLo.Count : 0)));
											if (worksheet.Cells[rowIndex - 1, 11].Value == null && worksheet.Cells[rowIndex - 1, 12].Value == null &&
											    worksheet.Cells[rowIndex - 1, 13].Value == null && worksheet.Cells[rowIndex - 1, 14].Value == null &&
											    worksheet.Cells[rowIndex - 1, 15].Value == null && worksheet.Cells[rowIndex - 1, 16].Value == null)
											{
												rowIndex--;
											}
										}
									}
									else
									{
										rowOfGroup = rowOfGroup + 1;
									}
									
									#endregion
								}
								worksheet.Cells[13, 1, rowIndex, 18].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
							}

							// add total end
							worksheet.Cells[rowIndex, 1].Value = (dictionary.ContainsKey("LBLTOTALREPORT")
								? dictionary["LBLTOTALREPORT"]
								: "");
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 1, rowIndex, 6].Merge = true;

							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.BorderAround(ExcelBorderStyle.Thin);
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

							worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[rowIndex, 13].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[rowIndex, 15].Style.Numberformat.Format = "#,##0";

							worksheet.Cells[rowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

							var sumstr = "SUM(K" + rowSumStart + ":K" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 11].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

							sumstr = "SUM(M" + rowSumStart + ":M" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 13].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

							sumstr = "SUM(O" + rowSumStart + ":O" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 15].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

							// write day
							if (intLanguague == 1)
							{
								worksheet.Cells[rowIndex + 1, 15].Value = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
																		  DateTime.Now.Year;
							}
							else if (intLanguague == 2)
							{
								cul = CultureInfo.GetCultureInfo("en-US");
								worksheet.Cells[rowIndex + 1, 15].Value = cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " +
																		  DateTime.Now.Day + ", " + DateTime.Now.Year;
							}
							else
							{
								worksheet.Cells[rowIndex + 1, 15].Value = DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day +
																		  "日";
							}

							// autoFit
							worksheet.Column(1).Width = 3.5;
							worksheet.Column(2).Width = 5;
							worksheet.Column(3).Width = 9;
							worksheet.Column(4).Width = 14;
							worksheet.Column(5).Width = 13;
							worksheet.Column(6).Width = 5;
							worksheet.Column(7).Width = 14;
							worksheet.Column(8).Width = 14;
							worksheet.Column(9).Width = 14;
							worksheet.Column(10).Width = 10;
							worksheet.Column(11).Width = 11;
							worksheet.Column(12).Width = 11;
							worksheet.Column(13).Width = 11;
							worksheet.Column(14).Width = 11;
							worksheet.Column(15).Width = 11;
							worksheet.Column(16).AutoFit();
						}
					}

					if (package.Workbook.Worksheets.Count == 0)
					{
						package.Workbook.Worksheets.Add("NoData");
					}

					return new MemoryStream(package.GetAsByteArray());
				}
		}

		public static Stream ExportExcelCustomerExpenseLOLOVertical(List<CustomerExpenseReportData> dataList, Dictionary<string, string> dictionary, int intLanguague, string fileName)
		{
			try
			{
				using (ExcelPackage package = new ExcelPackage())
				{
					// set culture
					CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
					if (intLanguague == 2)
					{
						cul = CultureInfo.GetCultureInfo("en-US");
					}
					else if (intLanguague == 3)
					{
						cul = CultureInfo.GetCultureInfo("ja-JP");
					}

					for (var i = 0; i < dataList.Count; i++)
					{
						if (dataList[i].CustomerExpenseList != null && dataList[i].CustomerExpenseList.Count > 0)
						{
							var data = dataList[i].CustomerExpenseList;
							var rowIndex = 1;
							var colIndex = 1;
							var tolCol = 13;
							// add a new worksheet to the empty workbook
							ExcelWorksheet worksheet =
								package.Workbook.Worksheets.Add(dataList[i].CustomerMainC + "_" + dataList[i].CustomerSubC + "_" + dataList[i].CustomerPayLiftLoweredMainC + "_" + dataList[i].CustomerPayLiftLoweredSubC + "_" + i);

							// add image
							//Bitmap image = new Bitmap(HttpContext.Current.Request.MapPath("~/Images/logo.bmp"));
							try
							{
								Bitmap image = new Bitmap(HttpContext.Current.Request.MapPath("~/Images/" + fileName));
								ExcelPicture excelImage = null;
								if (image != null)
								{
									excelImage = worksheet.Drawings.AddPicture("logo", image);
									excelImage.From.Column = 0;
									excelImage.From.Row = 0;
									excelImage.SetSize(100, 75);
								}
							}
							catch
							{
								// do nothing
							}
							#region Title
							//Add the title
							worksheet.Cells[rowIndex, 4].Value = dictionary.ContainsKey("COMPANYNAME") ? dictionary["COMPANYNAME"] : "";
							worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 4, rowIndex, 9].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 4].Value = dictionary.ContainsKey("COMPANYADDRESS") ? dictionary["COMPANYADDRESS"] : "";
							worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 4, rowIndex, 9].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 4].Value = (dictionary.ContainsKey("LBLTAXCODECOMPANYREPORT")
								? dictionary["LBLTAXCODECOMPANYREPORT"]
								: "") + ": " + (dictionary.ContainsKey("COMPANYTAXCODE") ? dictionary["COMPANYTAXCODE"] : "");
							worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 4, rowIndex, 6].Merge = true;

							// next row
							rowIndex = rowIndex + 2;

							worksheet.Cells[rowIndex, 5].Value = (dictionary.ContainsKey("TLTLOLOFEEREPORT")
								? dictionary["TLTLOLOFEEREPORT"]
								: "");
							worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 5].Style.Font.Size = 16;
							worksheet.Cells[rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 5, rowIndex, 12].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 5].Value = (dictionary.ContainsKey("LBLMONTHYEAR") ? dictionary["LBLMONTHYEAR"] : "");
							worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 5].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 5, rowIndex, 12].Merge = true;

							// next row
							//left
							rowIndex = rowIndex + 2;

							//right
							worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLDEARREPORT")
								? dictionary["LBLDEARREPORT"] : "") + ": " + dataList[i].InvoiceN;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Bold = true;
							worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 13].Merge = true;

							// next row
							//left
							rowIndex = rowIndex + 1;
							if (!string.IsNullOrEmpty(dataList[i].CustomerPayLiftLoweredN) && dataList[i].CustomerPayLiftLoweredN != "")
							{
								worksheet.Cells[rowIndex - 1, 9].Value = (dictionary.ContainsKey("LBLCUSTOMERPAYLIFTORLOWERED")
								? dictionary["LBLCUSTOMERPAYLIFTORLOWERED"] : "")
								+ ": " + dataList[i].CustomerPayLiftLoweredN;
								worksheet.Cells[rowIndex - 1, 9].Style.Font.Size = 11;
								worksheet.Cells[rowIndex - 1, 9, rowIndex - 1, 16].Merge = true;
							}

							// right
							worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLCUSTOMER")
								? dictionary["LBLCUSTOMER"] : "")
								+ ": " + dataList[i].CustomerN;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 8].Merge = true;
							//net row
							//left
							rowIndex = rowIndex + 1;

							if (!string.IsNullOrEmpty(dataList[i].CustomerPayLiftLoweredAddress) && dataList[i].CustomerPayLiftLoweredAddress != "")
							{
								worksheet.Cells[rowIndex - 1, 9].Value = (dictionary.ContainsKey("LBLADDRESS")
								? dictionary["LBLADDRESS"] : "")
								+ ": " + dataList[i].CustomerPayLiftLoweredAddress;
								worksheet.Cells[rowIndex - 1, 9].Style.Font.Size = 11;
								worksheet.Cells[rowIndex - 1, 9, rowIndex - 1, 16].Merge = true;
							}

							// right
							worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLADDRESS")
								? dictionary["LBLADDRESS"] : "")
								+ ": " + dataList[i].CustomerAddress;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 8].Merge = true;
							#endregion
							// next row
							rowIndex = rowIndex + 1;

							#region Header
							// Add the headers
							colIndex = 1;
							// column1
							worksheet.Cells[rowIndex, colIndex].Value = "#";
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column2
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLORDERTYPEDISPATCH")
								? dictionary["LBLORDERTYPEDISPATCH"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column3
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLDATEREPORT")
								? dictionary["LBLDATEREPORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							//column5
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLBLBKREPORT")
								? dictionary["LBLBLBKREPORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column6
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLCONTNUMBER")
								? dictionary["LBLCONTNUMBER"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column7-10
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLCATEGORY")
								? dictionary["LBLCATEGORY"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							// column11-13
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLSTOPOVERPLACESHORT")
								? dictionary["LBLSTOPOVERPLACESHORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLLOADINGPLACESHORT")
								? dictionary["LBLLOADINGPLACESHORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLDISCHARGEPLACESHORT")
								? dictionary["LBLDISCHARGEPLACESHORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							// column14
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLTRUCKNO")
								? dictionary["LBLTRUCKNO"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column18-20
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLINVOICENOREPORT")
								? dictionary["LBLINVOICENOREPORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 2].Merge = true;
							worksheet.Cells[rowIndex + 1, colIndex].Value = (dictionary.ContainsKey("LBLEXPENSETYPEREPORT")
								? dictionary["LBLEXPENSETYPEREPORT"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 1].Value = (dictionary.ContainsKey("LBLAMOUNTREPORT")
								? dictionary["LBLAMOUNTREPORT"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 2].Value = (dictionary.ContainsKey("LBLINVOICENOREPORT")
								? dictionary["LBLINVOICENOREPORT"]
								: "");

							// set font bold
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Font.Bold = true;
							// set border
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							// align center
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							// set color
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
							#endregion

							// next row
							rowIndex = rowIndex + 2;
							var rowSumStart = rowIndex;

							// write content
							if (data != null && data.Count > 0)
							{
								var keyNew = "";
								var keyOld = "";
								int rowOfGroup = 0;
								int index = 1;
								for (int iloop = 0; iloop < data.Count; iloop++)
								{
									#region Detail
									worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
									worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

									keyNew = data[iloop].OrderD.OrderD + data[iloop].OrderD.OrderNo + data[iloop].OrderD.DetailNo;
									if (keyNew != keyOld)
									{
										worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;

										rowOfGroup = 1;
										keyOld = keyNew;
										// column1
										colIndex = 1;
										worksheet.Cells[rowIndex, colIndex].Value = index++;
										worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
										worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										// column2
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Value = Utilities.GetOrderTypeName(data[iloop].OrderH.OrderTypeI);
										// column3
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "dd/MM/yy";
										if (intLanguague == 2)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "MM/dd/yy";
										}
										else if (intLanguague == 3)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "yy/MM/dd";
										}
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.ActualLoadingD;
										// column5
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderH.BLBK;
										// column6
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.ContainerNo;
										worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
										// column7-10
										colIndex++;
										if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE1)
										{
											worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE1N;
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										else if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE2)
										{
											worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE2N;
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										else if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE3)
										{
											worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE3N;
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										else
										{
											worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLLOAD"] + (data[iloop].OrderD.NetWeight);
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										// column11-13
										colIndex++;
										if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch1))
										{
											worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.LocationDispatch1;
											worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
										}
										if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch2))
										{
											worksheet.Cells[rowIndex, colIndex + 1].Value = data[iloop].OrderD.LocationDispatch2;
											worksheet.Cells[rowIndex, colIndex + 1].Style.WrapText = true;
										}
										if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch3))
										{
											worksheet.Cells[rowIndex, colIndex + 2].Value = data[iloop].OrderD.LocationDispatch3;
											worksheet.Cells[rowIndex, colIndex + 2].Style.WrapText = true;
										}
										// column14
										colIndex = colIndex + 3;
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.RegisteredNo;
									}
									else
									{
										rowOfGroup = rowOfGroup + 1;
									}

									// column 18
									worksheet.Cells[rowIndex, 11].Value = data[iloop].ExpenseD.ExpenseN;
									worksheet.Cells[rowIndex, 11].Style.WrapText = true;
									// column 19
									// cal totalExpense
									if (data[iloop].ExpenseD.Amount != null && data[iloop].ExpenseD.Amount > 0)
									{
										worksheet.Cells[rowIndex, 12].Style.Numberformat.Format = "#,##0";
										worksheet.Cells[rowIndex, 12].Value = data[iloop].ExpenseD.Amount;
									}
									worksheet.Cells[rowIndex, 13].Value = data[iloop].ExpenseD.Description;
									worksheet.Cells[rowIndex, 11, rowIndex, 13].Style.Border.Top.Style = ExcelBorderStyle.Thin;
									rowIndex = rowIndex + 1;
									#endregion
								}
								worksheet.Cells[13, 1, rowIndex, 18].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
							}

							// add total end
							worksheet.Cells[rowIndex, 1].Value = (dictionary.ContainsKey("LBLTOTALREPORT")
								? dictionary["LBLTOTALREPORT"]
								: "");
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 1, rowIndex, 6].Merge = true;

							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.BorderAround(ExcelBorderStyle.Thin);
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

							worksheet.Cells[rowIndex, 12].Style.Numberformat.Format = "#,##0";

							worksheet.Cells[rowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

							var sumstr = "SUM(L" + rowSumStart + ":L" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 12].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

							// write day
							if (intLanguague == 1)
							{
								worksheet.Cells[rowIndex + 1, 15].Value = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
																		  DateTime.Now.Year;
							}
							else if (intLanguague == 2)
							{
								cul = CultureInfo.GetCultureInfo("en-US");
								worksheet.Cells[rowIndex + 1, 15].Value = cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " +
																		  DateTime.Now.Day + ", " + DateTime.Now.Year;
							}
							else
							{
								worksheet.Cells[rowIndex + 1, 15].Value = DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day +
																		  "日";
							}

							// autoFit
							worksheet.Column(1).Width = 3.5;
							worksheet.Column(2).Width = 5;
							worksheet.Column(3).Width = 9;
							worksheet.Column(4).Width = 14;
							worksheet.Column(5).Width = 13;
							worksheet.Column(6).Width = 5;
							worksheet.Column(7).Width = 14;
							worksheet.Column(8).Width = 14;
							worksheet.Column(9).Width = 14;
							worksheet.Column(10).Width = 10;
							worksheet.Column(11).AutoFit();
							worksheet.Column(12).Width = 11;
							worksheet.Column(13).AutoFit();
							
						}
					}

					if (package.Workbook.Worksheets.Count == 0)
					{
						package.Workbook.Worksheets.Add("NoData");
					}

					return new MemoryStream(package.GetAsByteArray());
				}
			}
			catch (NullReferenceException)
			{
				throw new NullReferenceException();
			}
		}

		public static Stream ExportExcelCustomerExpenseTransportFeeHorizontal(List<CustomerExpenseReportData> dataList, Dictionary<string, string> dictionary, int intLanguague, string fileName, string lolo)
		{
			using (ExcelPackage package = new ExcelPackage())
			{
				// set culture
				CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
				if (intLanguague == 2)
				{
					cul = CultureInfo.GetCultureInfo("en-US");
				}
				else if (intLanguague == 3)
				{
					cul = CultureInfo.GetCultureInfo("ja-JP");
				}

				for (var i = 0; i < dataList.Count; i++)
				{
					if (dataList[i].CustomerExpenseList != null && dataList[i].CustomerExpenseList.Count > 0)
					{
						var data = dataList[i].CustomerExpenseList;
						var rowIndex = 1;
						var colIndex = 1;
						var tolCol = lolo == "1" ? 19 : 13;
						// add a new worksheet to the empty workbook
						ExcelWorksheet worksheet =
							package.Workbook.Worksheets.Add(dataList[i].CustomerMainC + "_" + dataList[i].CustomerSubC + "_" + i);

						// add image
						//Bitmap image = new Bitmap(HttpContext.Current.Request.MapPath("~/Images/logo.bmp"));
						try
						{
							Bitmap image = new Bitmap(HttpContext.Current.Request.MapPath("~/Images/" + fileName));
							ExcelPicture excelImage = null;
							if (image != null)
							{
								excelImage = worksheet.Drawings.AddPicture("logo", image);
								excelImage.From.Column = 0;
								excelImage.From.Row = 0;
								excelImage.SetSize(100, 75);
							}
						}
						catch
						{
							// do nothing
						}
						#region Title
						//Add the title
						worksheet.Cells[rowIndex, 4].Value = dictionary.ContainsKey("COMPANYNAME") ? dictionary["COMPANYNAME"] : "";
						worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
						worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
						worksheet.Cells[rowIndex, 4, rowIndex, 9].Merge = true;

						// next row
						rowIndex = rowIndex + 1;

						worksheet.Cells[rowIndex, 4].Value = dictionary.ContainsKey("COMPANYADDRESS") ? dictionary["COMPANYADDRESS"] : "";
						worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
						worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
						worksheet.Cells[rowIndex, 4, rowIndex, 9].Merge = true;

						// next row
						rowIndex = rowIndex + 1;

						worksheet.Cells[rowIndex, 4].Value = (dictionary.ContainsKey("LBLTAXCODECOMPANYREPORT")
							? dictionary["LBLTAXCODECOMPANYREPORT"]
							: "") + ": " + (dictionary.ContainsKey("COMPANYTAXCODE") ? dictionary["COMPANYTAXCODE"] : "");
						worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
						worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
						worksheet.Cells[rowIndex, 4, rowIndex, 6].Merge = true;

						// next row
						rowIndex = rowIndex + 2;

						worksheet.Cells[rowIndex, 5].Value = (dictionary.ContainsKey("TLTTRANSPORTFEEREPORT")
							? dictionary["TLTTRANSPORTFEEREPORT"]
							: "");
						worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
						worksheet.Cells[rowIndex, 5].Style.Font.Size = lolo == "1" ? 19 : 13;
						worksheet.Cells[rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells[rowIndex, 5, rowIndex, 12].Merge = true;

						// next row
						rowIndex = rowIndex + 1;

						worksheet.Cells[rowIndex, 5].Value = (dictionary.ContainsKey("LBLMONTHYEAR") ? dictionary["LBLMONTHYEAR"] : "");
						worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
						worksheet.Cells[rowIndex, 5].Style.Font.Size = 11;
						worksheet.Cells[rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells[rowIndex, 5, rowIndex, 12].Merge = true;

						// next row
						//left
						rowIndex = rowIndex + 2;

						var transportRow = rowIndex;
						worksheet.Cells[rowIndex - 1, 10].Value = (dictionary.ContainsKey("RPHDAMOUNT")
							? dictionary["RPHDAMOUNT"]
							: "") + ": ";
						worksheet.Cells[rowIndex - 1, 10].Style.Font.Size = 11;
						worksheet.Cells[rowIndex - 1, 10, rowIndex - 1, 11].Merge = true;

						//right
						worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLDEARREPORT")
							? dictionary["LBLDEARREPORT"] : "") + ": " + dataList[i].InvoiceN;
						worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
						worksheet.Cells[rowIndex - 1, 1].Style.Font.Bold = true;
						worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 9].Merge = true;

						// next row
						//left
						rowIndex = rowIndex + 1;
						if (dictionary.ContainsKey("COMPANYTAXRATE") && Convert.ToDecimal(dictionary["COMPANYTAXRATE"]) % 10 == 0)
						{
							worksheet.Cells[rowIndex - 1, 10].Value = (dictionary.ContainsKey("LBLTAXAMOUNTREPORT") ? dictionary["LBLTAXAMOUNTREPORT"] : "") + " " +
																	Math.Round(dataList[i].TaxRate) + "%";
						}
						else
						{
							worksheet.Cells[rowIndex - 1, 10].Value = (dictionary.ContainsKey("LBLTAXAMOUNTREPORT") ? dictionary["LBLTAXAMOUNTREPORT"] : "") + " " +
																	dataList[i].TaxRate + "%";
						}

						worksheet.Cells[rowIndex - 1, 10].Style.Font.Size = 11;
						worksheet.Cells[rowIndex - 1, 10, rowIndex - 1, 11].Merge = true;

						// right
						worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLCUSTOMER")
							? dictionary["LBLCUSTOMER"] : "")
							+ ": " + dataList[i].CustomerN;
						worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
						worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 9].Merge = true;
						//net row
						//left
						rowIndex = rowIndex + 1;
						var transportTaxRow = rowIndex;
						worksheet.Cells[rowIndex - 1, 10].Value = (dictionary.ContainsKey("LBLTRANSPORTFEEAFTERTAX")
							? dictionary["LBLTRANSPORTFEEAFTERTAX"]
							: "") + ":";
						worksheet.Cells[rowIndex - 1, 10].Style.Font.Size = 11;
						worksheet.Cells[rowIndex - 1, 10, rowIndex - 1, 11].Merge = true;

						// right
						worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLADDRESS")
							? dictionary["LBLADDRESS"] : "")
							+ ": " + dataList[i].CustomerAddress;
						worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
						worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 9].Merge = true;

						//next row
						rowIndex = rowIndex + 1;
						var expenseRow = rowIndex;
						if (lolo == "1")
						{
							worksheet.Cells[rowIndex - 1, 10].Value = (dictionary.ContainsKey("LBLPAYONBEHALFRP")
							? dictionary["LBLPAYONBEHALFRP"]
							: "") + ":";
							worksheet.Cells[rowIndex - 1, 10].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 10, rowIndex - 1, 11].Merge = true;
						}

						// next row
						rowIndex = rowIndex + 1;

						var totalRow = rowIndex;
						worksheet.Cells[rowIndex - 1, 10].Value = (dictionary.ContainsKey("LBLTOTALAMOUNTREPORT")
							? dictionary["LBLTOTALAMOUNTREPORT"]
							: "") + ":";
						worksheet.Cells[rowIndex - 1, 10].Style.Font.Bold = true;
						//worksheet.Cells[rowIndex, 15].Style.Font.Size = 14;
						worksheet.Cells[rowIndex - 1, 10, rowIndex - 1, 11].Merge = true;
						#endregion

						// next row
						rowIndex = rowIndex + 1;

						#region Header
						// Add the headers
						colIndex = 1;
						// column1
						worksheet.Cells[rowIndex, colIndex].Value = "#";
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						// column2
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLORDERTYPEDISPATCH")
							? dictionary["LBLORDERTYPEDISPATCH"]
							: "");
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						// column3
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLDATEREPORT")
							? dictionary["LBLDATEREPORT"]
							: "");
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						//column5
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLBLBKREPORT")
							? dictionary["LBLBLBKREPORT"]
							: "");
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						// column6
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLCONTNUMBER")
							? dictionary["LBLCONTNUMBER"]
							: "");
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						// column7-10
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLCATEGORY")
							? dictionary["LBLCATEGORY"]
							: "");
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
						// column11-13
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLSTOPOVERPLACESHORT")
							? dictionary["LBLSTOPOVERPLACESHORT"]
							: "");
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLLOADINGPLACESHORT")
							? dictionary["LBLLOADINGPLACESHORT"]
							: "");
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLDISCHARGEPLACESHORT")
							? dictionary["LBLDISCHARGEPLACESHORT"]
							: "");
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
						// column14
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLTRUCKNO")
							? dictionary["LBLTRUCKNO"]
							: "");
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						// column15-17
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLAMOUNTSHORTRP")
							? dictionary["LBLAMOUNTSHORTRP"] : "");
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLSURCHARGEFEE")
							? dictionary["LBLSURCHARGEFEE"] : "");
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLEXPLAIN")
							? dictionary["LBLEXPLAIN"] : "");
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
						if (lolo == "1")
						{
							// column18-20
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLISREQUESTEDRP")
								? dictionary["LBLISREQUESTEDRP"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 5].Merge = true;
							worksheet.Cells[rowIndex + 1, colIndex].Value = (dictionary.ContainsKey("LBLLIFT")
								? dictionary["LBLLIFT"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 1].Value = (dictionary.ContainsKey("LBLBILLLIFTON")
								? dictionary["LBLBILLLIFTON"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 2].Value = (dictionary.ContainsKey("LBLLOWERED")
								? dictionary["LBLLOWERED"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 3].Value = (dictionary.ContainsKey("LBLBILLLIFTOFF")
								? dictionary["LBLBILLLIFTOFF"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 4].Value = (dictionary.ContainsKey("LBLOTHER")
								? dictionary["LBLOTHER"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 5].Value = (dictionary.ContainsKey("LBLEXPLAIN")
								? dictionary["LBLEXPLAIN"]
								: "");
						}
						
						// set font bold
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Font.Bold = true;
						// set border
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
						// align center
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						// set color
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
						#endregion

						// next row
						rowIndex = rowIndex + 2;
						var rowSumStart = rowIndex;

						// write content
						if (data != null && data.Count > 0)
						{
							var keyNew = "";
							var keyOld = "";
							//decimal totalColumn19 = 0;
							decimal totalTransport = 0;
							decimal totalDetain = 0;
							decimal totalExpense = 0;
							decimal totalTax = 0;
							int rowOfGroup = 0;
							int index = 1;
							for (int iloop = 0; iloop < data.Count; iloop++)
							{
								#region Detail
								worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

								keyNew = data[iloop].OrderD.OrderD + data[iloop].OrderD.OrderNo + data[iloop].OrderD.DetailNo;
								if (keyNew != keyOld)
								{
									worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;

									rowOfGroup = 1;
									keyOld = keyNew;
									// column1
									colIndex = 1;
									worksheet.Cells[rowIndex, colIndex].Value = index++;
									worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
									worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
									// column2
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Value = Utilities.GetOrderTypeName(data[iloop].OrderH.OrderTypeI);
									// column3
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "dd/MM/yy";
									if (intLanguague == 2)
									{
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "MM/dd/yy";
									}
									else if (intLanguague == 3)
									{
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "yy/MM/dd";
									}
									worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.ActualLoadingD;
									// column4
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderH.BLBK;
									// column5
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.ContainerNo;
									worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
									// column6
									colIndex++;
									if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE1)
									{
										worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE1N;
										worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
										worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
									}
									else if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE2)
									{
										worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE2N;
										worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
										worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
									}
									else if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE3)
									{
										worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE3N;
										worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
										worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
									}
									else
									{
										worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLLOAD"] + (data[iloop].OrderD.NetWeight);
										worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
										worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
									}
									// column7-9
									colIndex++;
									if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch1))
									{
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.LocationDispatch1;
										worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
									}
									if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch2))
									{
										worksheet.Cells[rowIndex, colIndex + 1].Value = data[iloop].OrderD.LocationDispatch2;
										worksheet.Cells[rowIndex, colIndex + 1].Style.WrapText = true;
									}
									if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch3))
									{
										worksheet.Cells[rowIndex, colIndex + 2].Value = data[iloop].OrderD.LocationDispatch3;
										worksheet.Cells[rowIndex, colIndex + 2].Style.WrapText = true;
									}
									// column10
									colIndex = colIndex + 3;
									worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.RegisteredNo;
									worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
									// column 11
									colIndex++;
									if (data[iloop].OrderD.Amount != null)
									{
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.Amount;
									}
									// column 12
									colIndex++;
									if (data[iloop].DetainAmount + data[iloop].SurchargeAmount != 0)
									{
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].DetainAmount + data[iloop].SurchargeAmount;
										totalDetain = totalDetain + data[iloop].DetainAmount + data[iloop].SurchargeAmount;
									}
									// column 13
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Value = data[iloop].Description;
									worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
									if (data[iloop].OrderD.Amount != null)
									{
										totalTransport = totalTransport + (decimal)data[iloop].OrderD.Amount;
										totalTax += data[iloop].TaxAmount;
									}
									if (lolo == "1")
									{
										//chi ho
										//column14,15
										var checkcanrow = 0;
										int rowIndexRoot = rowIndex;
										if (data[iloop].ExpenseD.LiftOnList != null && data[iloop].ExpenseD.LiftOnList.Count > 0)
										{
											for (int li = 0; li < data[iloop].ExpenseD.LiftOnList.Count; li++)
											{
												if (data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn != null &&
												    data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn > 0)
												{
													worksheet.Cells[rowIndex, 14].Style.Numberformat.Format = "#,##0";
													worksheet.Cells[rowIndex, 14].Value = data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn;
													totalExpense = totalExpense + (decimal) data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn;
												}
												worksheet.Cells[rowIndex, 15].Value = data[iloop].ExpenseD.LiftOnList[li].DescriptionLiftOn;
												worksheet.Cells[rowIndex, 15].Style.WrapText = true;
												rowIndex++;
												checkcanrow++;
												worksheet.Cells[rowIndex, 14, rowIndex, 19].Style.Border.Top.Style = ExcelBorderStyle.Thin;
												worksheet.Cells[rowIndex, 1, rowIndex, 19].Style.Border.Left.Style = ExcelBorderStyle.Thin;
												worksheet.Cells[rowIndex, 1, rowIndex, 19].Style.Border.Right.Style = ExcelBorderStyle.Thin;
											}
										}
										//column16,17
										if (data[iloop].ExpenseD.LiftOffList != null && data[iloop].ExpenseD.LiftOffList.Count > 0)
										{
											rowIndex = data[iloop].ExpenseD.LiftOnList != null
												? rowIndex - data[iloop].ExpenseD.LiftOnList.Count
												: rowIndex;
											for (int lj = 0; lj < data[iloop].ExpenseD.LiftOffList.Count; lj++)
											{
												if (data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff != null &&
												    data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff > 0)
												{
													worksheet.Cells[rowIndex, 16].Style.Numberformat.Format = "#,##0";
													worksheet.Cells[rowIndex, 16].Value = data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff;
													totalExpense = totalExpense + (decimal) data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff;
												}
												worksheet.Cells[rowIndex, 17].Value = data[iloop].ExpenseD.LiftOffList[lj].DescriptionLiftOff;
												worksheet.Cells[rowIndex, 17].Style.WrapText = true;
												rowIndex++;
												checkcanrow++;
												worksheet.Cells[rowIndex, 14, rowIndex, 19].Style.Border.Top.Style = ExcelBorderStyle.Thin;
												worksheet.Cells[rowIndex, 1, rowIndex, 19].Style.Border.Left.Style = ExcelBorderStyle.Thin;
												worksheet.Cells[rowIndex, 1, rowIndex, 19].Style.Border.Right.Style = ExcelBorderStyle.Thin;
											}
										}
										//column18,19
										if (data[iloop].ExpenseD.OtherListLoLo != null && data[iloop].ExpenseD.OtherListLoLo.Count > 0)
										{
											string description = "";
											decimal amount = 0;
											rowIndex = data[iloop].ExpenseD.LiftOffList != null
												? rowIndex - data[iloop].ExpenseD.LiftOffList.Count
												: rowIndex;
											for (int lk = 0; lk < data[iloop].ExpenseD.OtherListLoLo.Count; lk++)
											{
												if (data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther != null &&
												    data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther > 0)
												{
													description = (data[iloop].ExpenseD.OtherListLoLo[lk].DescriptionOther != "" &&
													               !string.IsNullOrEmpty(data[iloop].ExpenseD.OtherListLoLo[lk].DescriptionOther))
														? (((description != "") ? (description + ",") : "") +
														   data[iloop].ExpenseD.OtherListLoLo[lk].DescriptionOther)
														: description;
													amount = amount + (decimal) data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther;
													totalExpense = totalExpense + (decimal) data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther;
												}
											}
											worksheet.Cells[rowIndex, 18].Style.Numberformat.Format = "#,##0";
											worksheet.Cells[rowIndex, 18].Value = amount;
											worksheet.Cells[rowIndex, 19].Value = description;
											worksheet.Cells[rowIndex, 19].Style.WrapText = true;
											rowIndex++;
											checkcanrow++;
											worksheet.Cells[rowIndex, 14, rowIndex, 19].Style.Border.Top.Style = ExcelBorderStyle.Thin;
											worksheet.Cells[rowIndex, 1, rowIndex, 19].Style.Border.Left.Style = ExcelBorderStyle.Thin;
											worksheet.Cells[rowIndex, 1, rowIndex, 19].Style.Border.Right.Style = ExcelBorderStyle.Thin;
										}
										if (checkcanrow < 1)
										{
											rowIndex++;
										}
										else
										{
											rowIndex =
												Math.Max(
													Math.Max(
														(rowIndexRoot + (data[iloop].ExpenseD.LiftOnList != null ? data[iloop].ExpenseD.LiftOnList.Count : 0)),
														(rowIndexRoot + (data[iloop].ExpenseD.LiftOffList != null ? data[iloop].ExpenseD.LiftOffList.Count : 0))),
													(rowIndexRoot + (data[iloop].ExpenseD.OtherListLoLo != null ? data[iloop].ExpenseD.OtherListLoLo.Count : 0)));
											if (worksheet.Cells[rowIndex - 1, 14].Value == null && worksheet.Cells[rowIndex - 1, 15].Value == null &&
											    worksheet.Cells[rowIndex - 1, 16].Value == null && worksheet.Cells[rowIndex - 1, 17].Value == null &&
											    worksheet.Cells[rowIndex - 1, 18].Value == null && worksheet.Cells[rowIndex - 1, 19].Value == null)
											{
												rowIndex--;
											}
										}
									}
									else
									{
										rowIndex++;
									}
									
								}
								else
								{
									rowOfGroup = rowOfGroup + 1;
								}
									
								#endregion
							}
							worksheet.Cells[15, 1, rowIndex, 18].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
							#region Title Sum
							// write total
							worksheet.Cells[transportRow - 1, 12].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[transportRow - 1, 12].Value = totalTransport + totalDetain;
							worksheet.Cells[transportRow - 1, 12, transportRow - 1, 13].Merge = true;
							// tax
							worksheet.Cells[transportRow, 12].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[transportRow, 12].Value = totalTax;
							worksheet.Cells[transportRow, 12, transportRow, 13].Merge = true;

							// after tax
							worksheet.Cells[transportTaxRow - 1, 12].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[transportTaxRow - 1, 12, transportTaxRow - 1, 13].Merge = true;
							worksheet.Cells[transportTaxRow - 1, 12].Formula = "SUM(L" + (transportRow - 1) + ":L" + (transportRow) + ")";
							worksheet.Cells[totalRow - 1, 12].Style.Numberformat.Format = "#,##0";

							if (lolo == "1")
							{
								worksheet.Cells[expenseRow - 1, 12].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[expenseRow - 1, 12].Value = totalExpense;
								worksheet.Cells[expenseRow - 1, 12, expenseRow - 1, 13].Merge = true;
								worksheet.Cells[totalRow - 1, 12, totalRow - 1, 13].Merge = true;
								worksheet.Cells[totalRow - 1, 12].Formula = "SUM(L" + (transportTaxRow - 1) + ":L" + (expenseRow - 1) + ")";
							}
							else
							{
								worksheet.Cells[totalRow - 1, 12, totalRow - 1, 13].Merge = true;
								worksheet.Cells[totalRow - 1, 12].Formula = "SUM(L" + (transportTaxRow - 1) + ")";
							}
							worksheet.Cells[totalRow - 1, 12].Style.Font.Bold = true;
							#endregion
						}

						// add total end
						worksheet.Cells[rowIndex, 1].Value = (dictionary.ContainsKey("LBLTOTALREPORT")
							? dictionary["LBLTOTALREPORT"]
							: "");
						worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Font.Size = 11;
						worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Font.Bold = true;
						worksheet.Cells[rowIndex, 1, rowIndex, 6].Merge = true;

						worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
						worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

						worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0";
						worksheet.Cells[rowIndex, 12].Style.Numberformat.Format = "#,##0";
						worksheet.Cells[rowIndex, 14].Style.Numberformat.Format = "#,##0";
						worksheet.Cells[rowIndex, 16].Style.Numberformat.Format = "#,##0";
						worksheet.Cells[rowIndex, 18].Style.Numberformat.Format = "#,##0";

						worksheet.Cells[rowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells[rowIndex, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells[rowIndex, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells[rowIndex, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

						var sumstr = "SUM(K" + rowSumStart + ":K" + (rowIndex - 1) + ")";
						worksheet.Cells[rowIndex, 11].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

						sumstr = "SUM(L" + rowSumStart + ":L" + (rowIndex - 1) + ")";
						worksheet.Cells[rowIndex, 12].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

						if (lolo == "1")
						{
							sumstr = "SUM(N" + rowSumStart + ":N" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 14].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

							sumstr = "SUM(P" + rowSumStart + ":P" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 16].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

							sumstr = "SUM(R" + rowSumStart + ":R" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 18].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";
						}

						// write day
						if (intLanguague == 1)
						{
							worksheet.Cells[rowIndex + 1, 11].Value = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
																		DateTime.Now.Year;
						}
						else if (intLanguague == 2)
						{
							cul = CultureInfo.GetCultureInfo("en-US");
							worksheet.Cells[rowIndex + 1, 11].Value = cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " +
																		DateTime.Now.Day + ", " + DateTime.Now.Year;
						}
						else
						{
							worksheet.Cells[rowIndex + 1, 11].Value = DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day +
																		"日";
						}

						// autoFit
						worksheet.Column(1).Width = 3.5;
						worksheet.Column(2).Width = 5;
						worksheet.Column(3).Width = 9;
						worksheet.Column(4).Width = 14;
						worksheet.Column(5).Width = 13;
						worksheet.Column(6).Width = 5;
						worksheet.Column(7).Width = 14;
						worksheet.Column(8).Width = 14;
						worksheet.Column(9).Width = 14;
						worksheet.Column(10).Width = 10;
						worksheet.Column(11).Width = 11;
						worksheet.Column(12).Width = 11;
						worksheet.Column(13).AutoFit();
						if (lolo == "1")
						{
							worksheet.Column(14).Width = 11;
							worksheet.Column(15).Width = 11;
							worksheet.Column(16).Width = 11;
							worksheet.Column(17).Width = 11;
							worksheet.Column(18).Width = 11;
							worksheet.Column(19).AutoFit();
						}
					}
				}

				if (package.Workbook.Worksheets.Count == 0)
				{
					package.Workbook.Worksheets.Add("NoData");
				}

				return new MemoryStream(package.GetAsByteArray());
			}
		}

		public static Stream ExportCustomerExpenseTransportFeeVertical(List<CustomerExpenseReportData> dataList, Dictionary<string, string> dictionary, int intLanguague, string fileName, string lolo)
		{
			try
			{
				using (ExcelPackage package = new ExcelPackage())
				{
					// set culture
					CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
					if (intLanguague == 2)
					{
						cul = CultureInfo.GetCultureInfo("en-US");
					}
					else if (intLanguague == 3)
					{
						cul = CultureInfo.GetCultureInfo("ja-JP");
					}

					for (var i = 0; i < dataList.Count; i++)
					{
						if (dataList[i].CustomerExpenseList != null && dataList[i].CustomerExpenseList.Count > 0)
						{
							var data = dataList[i].CustomerExpenseList;
							var rowIndex = 1;
							var colIndex = 1;
							var tolCol = lolo == "1" ? 16 : 13;
							// add a new worksheet to the empty workbook
							ExcelWorksheet worksheet =
								package.Workbook.Worksheets.Add(dataList[i].CustomerMainC + "_" + dataList[i].CustomerSubC + "_" + i);

							// add image
							//Bitmap image = new Bitmap(HttpContext.Current.Request.MapPath("~/Images/logo.bmp"));
							try
							{
								Bitmap image = new Bitmap(HttpContext.Current.Request.MapPath("~/Images/" + fileName));
								ExcelPicture excelImage = null;
								if (image != null)
								{
									excelImage = worksheet.Drawings.AddPicture("logo", image);
									excelImage.From.Column = 0;
									excelImage.From.Row = 0;
									excelImage.SetSize(100, 75);
								}
							}
							catch
							{
								// do nothing
							}
							#region Title
							//Add the title
							worksheet.Cells[rowIndex, 4].Value = dictionary.ContainsKey("COMPANYNAME") ? dictionary["COMPANYNAME"] : "";
							worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 4, rowIndex, 9].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 4].Value = dictionary.ContainsKey("COMPANYADDRESS") ? dictionary["COMPANYADDRESS"] : "";
							worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 4, rowIndex, 9].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 4].Value = (dictionary.ContainsKey("LBLTAXCODECOMPANYREPORT")
								? dictionary["LBLTAXCODECOMPANYREPORT"]
								: "") + ": " + (dictionary.ContainsKey("COMPANYTAXCODE") ? dictionary["COMPANYTAXCODE"] : "");
							worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 4, rowIndex, 6].Merge = true;

							// next row
							rowIndex = rowIndex + 2;

							worksheet.Cells[rowIndex, 5].Value = (dictionary.ContainsKey("TLTTRANSPORTFEEREPORT")
								? dictionary["TLTTRANSPORTFEEREPORT"]
								: "");
							worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 5].Style.Font.Size = lolo == "1" ? 16 : 13;
							worksheet.Cells[rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 5, rowIndex, 12].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 5].Value = (dictionary.ContainsKey("LBLMONTHYEAR") ? dictionary["LBLMONTHYEAR"] : "");
							worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 5].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 5, rowIndex, 12].Merge = true;

							// next row
							//left
							rowIndex = rowIndex + 2;

							var transportRow = rowIndex;
							worksheet.Cells[rowIndex - 1, 10].Value = (dictionary.ContainsKey("LBLTAXEXCLUSIONAMOUNT")
								? dictionary["LBLTAXEXCLUSIONAMOUNT"]
								: "") + ": ";
							worksheet.Cells[rowIndex - 1, 10].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 10, rowIndex - 1, 11].Merge = true;

							//right
							worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLDEARREPORT")
								? dictionary["LBLDEARREPORT"] : "") + ": " + dataList[i].InvoiceN;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Bold = true;
							worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 9].Merge = true;

							// next row
							//left
							rowIndex = rowIndex + 1;
							if (dictionary.ContainsKey("COMPANYTAXRATE") && Convert.ToDecimal(dictionary["COMPANYTAXRATE"]) % 10 == 0)
							{
								worksheet.Cells[rowIndex - 1, 10].Value = (dictionary.ContainsKey("LBLTAXAMOUNTREPORT") ? dictionary["LBLTAXAMOUNTREPORT"] : "") + " " +
																	 Math.Round(dataList[i].TaxRate) + "%";
							}
							else
							{
								worksheet.Cells[rowIndex - 1, 10].Value = (dictionary.ContainsKey("LBLTAXAMOUNTREPORT") ? dictionary["LBLTAXAMOUNTREPORT"] : "") + " " +
																	 dataList[i].TaxRate + "%";
							}

							worksheet.Cells[rowIndex - 1, 10].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 10, rowIndex - 1, 11].Merge = true;

							// right
							worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLCUSTOMER")
								? dictionary["LBLCUSTOMER"] : "")
								+ ": " + dataList[i].CustomerN;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 9].Merge = true;
							//net row
							//left
							rowIndex = rowIndex + 1;
							var transportTaxRow = rowIndex;
							worksheet.Cells[rowIndex - 1, 10].Value = (dictionary.ContainsKey("LBLTRANSPORTFEEAFTERTAX")
								? dictionary["LBLTRANSPORTFEEAFTERTAX"]
								: "") + ":";
							worksheet.Cells[rowIndex - 1, 10].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 10, rowIndex - 1, 11].Merge = true;

							// right
							worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLADDRESS")
								? dictionary["LBLADDRESS"] : "")
								+ ": " + dataList[i].CustomerAddress;
							worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 9].Merge = true;

							//next row
							rowIndex = rowIndex + 1;
							var expenseRow = rowIndex;
							if (lolo == "1")
							{
								worksheet.Cells[rowIndex - 1, 10].Value = (dictionary.ContainsKey("LBLPAYONBEHALFRP")
									? dictionary["LBLPAYONBEHALFRP"]
									: "") + ":";
								worksheet.Cells[rowIndex - 1, 10].Style.Font.Size = 11;
								worksheet.Cells[rowIndex - 1, 10, rowIndex - 1, 11].Merge = true;
							}

							// next row
							rowIndex = rowIndex + 1;

							var totalRow = rowIndex;
							worksheet.Cells[rowIndex - 1, 10].Value = (dictionary.ContainsKey("LBLTOTALAMOUNTREPORT")
								? dictionary["LBLTOTALAMOUNTREPORT"]
								: "") + ":";
							worksheet.Cells[rowIndex - 1, 10].Style.Font.Bold = true;
							//worksheet.Cells[rowIndex, 15].Style.Font.Size = 14;
							worksheet.Cells[rowIndex - 1, 10, rowIndex - 1, 11].Merge = true;
							#endregion

							// next row
							rowIndex = rowIndex + 1;

							#region Header
							// Add the headers
							colIndex = 1;
							// column1
							worksheet.Cells[rowIndex, colIndex].Value = "#";
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column2
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLORDERTYPEDISPATCH")
								? dictionary["LBLORDERTYPEDISPATCH"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column3
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLDATEREPORT")
								? dictionary["LBLDATEREPORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							//column5
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLBLBKREPORT")
								? dictionary["LBLBLBKREPORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column6
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLCONTNUMBER")
								? dictionary["LBLCONTNUMBER"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column7-10
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLCATEGORY")
								? dictionary["LBLCATEGORY"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							// column11-13
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLSTOPOVERPLACESHORT")
								? dictionary["LBLSTOPOVERPLACESHORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLLOADINGPLACESHORT")
								? dictionary["LBLLOADINGPLACESHORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLDISCHARGEPLACESHORT")
								? dictionary["LBLDISCHARGEPLACESHORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							// column14
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLTRUCKNO")
								? dictionary["LBLTRUCKNO"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							// column15-17
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLAMOUNTSHORTRP")
								? dictionary["LBLAMOUNTSHORTRP"] : "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLSURCHARGEFEE")
								? dictionary["LBLSURCHARGEFEE"] : "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLEXPLAIN")
								? dictionary["LBLEXPLAIN"] : "");
							worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							// column18-20
							if (lolo == "1")
							{
								colIndex++;
								worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLISREQUESTEDRP")
									? dictionary["LBLISREQUESTEDRP"]
									: "");
								worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 2].Merge = true;
								worksheet.Cells[rowIndex + 1, colIndex].Value = (dictionary.ContainsKey("LBLEXPENSETYPEREPORT")
									? dictionary["LBLEXPENSETYPEREPORT"]
									: "");
								worksheet.Cells[rowIndex + 1, colIndex + 1].Value = (dictionary.ContainsKey("LBLAMOUNTREPORT")
									? dictionary["LBLAMOUNTREPORT"]
									: "");
								worksheet.Cells[rowIndex + 1, colIndex + 2].Value = (dictionary.ContainsKey("LBLINVOICENOREPORT")
									? dictionary["LBLINVOICENOREPORT"]
									: "");
							}
							

							// set font bold
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Font.Bold = true;
							// set border
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							// align center
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							// set color
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
							#endregion

							// next row
							rowIndex = rowIndex + 2;
							var rowSumStart = rowIndex;

							// write content
							if (data != null && data.Count > 0)
							{
								var keyNew = "";
								var keyOld = "";
								//decimal totalColumn19 = 0;
								decimal totalTransport = 0;
								decimal totalDetain = 0;
								decimal totalExpense = 0;
								decimal totalTax = 0;
								int rowOfGroup = 0;
								int index = 1;
								for (int iloop = 0; iloop < data.Count; iloop++)
								{
									#region Detail
									worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
									worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

									keyNew = data[iloop].OrderD.OrderD + data[iloop].OrderD.OrderNo + data[iloop].OrderD.DetailNo;
									if (keyNew != keyOld)
									{
										worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;

										rowOfGroup = 1;
										keyOld = keyNew;
										// column1
										colIndex = 1;
										worksheet.Cells[rowIndex, colIndex].Value = index++;
										worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
										worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										// column2
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Value = Utilities.GetOrderTypeName(data[iloop].OrderH.OrderTypeI);
										// column3
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "dd/MM/yy";
										if (intLanguague == 2)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "MM/dd/yy";
										}
										else if (intLanguague == 3)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "yy/MM/dd";
										}
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.ActualLoadingD;
										// column5
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderH.BLBK;
										// column6
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.ContainerNo;
										worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
										// column7-10
										colIndex++;
										if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE1)
										{
											worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE1N;
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										else if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE2)
										{
											worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE2N;
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										else if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE3)
										{
											worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE3N;
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										else
										{
											worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLLOAD"] + (data[iloop].OrderD.NetWeight);
											worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
											worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										}
										// column11-13
										colIndex++;
										if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch1))
										{
											worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.LocationDispatch1;
											worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
										}
										if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch2))
										{
											worksheet.Cells[rowIndex, colIndex + 1].Value = data[iloop].OrderD.LocationDispatch2;
											worksheet.Cells[rowIndex, colIndex + 1].Style.WrapText = true;
										}
										if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch3))
										{
											worksheet.Cells[rowIndex, colIndex + 2].Value = data[iloop].OrderD.LocationDispatch3;
											worksheet.Cells[rowIndex, colIndex + 2].Style.WrapText = true;
										}
										// column14
										colIndex = colIndex + 3;
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.RegisteredNo;
										// column 15
										colIndex++;
										if (data[iloop].OrderD.Amount != null)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
											worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.Amount;
										}
										// column 16
										colIndex++;
										if (data[iloop].DetainAmount + data[iloop].SurchargeAmount != 0)
										{
											worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
											worksheet.Cells[rowIndex, colIndex].Value = data[iloop].DetainAmount + data[iloop].SurchargeAmount;
											totalDetain = totalDetain + data[iloop].DetainAmount + data[iloop].SurchargeAmount;
										}
										// column 17
										colIndex++;
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].Description;

										// cal totalColumn19
										//totalColumn19 = 0;
										if (data[iloop].OrderD.Amount != null)
										{
											totalTransport = totalTransport + (decimal)data[iloop].OrderD.Amount;
											totalTax += data[iloop].TaxAmount;
										}
									}
									else
									{
										rowOfGroup = rowOfGroup + 1;
									}

									if (lolo == "1")
									{
										// column 18
										worksheet.Cells[rowIndex, 14].Value = data[iloop].ExpenseD.ExpenseN;
										worksheet.Cells[rowIndex, 14].Style.WrapText = true;
										// column 19
										// cal totalExpense
										if (data[iloop].ExpenseD.Amount != null && data[iloop].ExpenseD.Amount > 0)
										{
											worksheet.Cells[rowIndex, 15].Style.Numberformat.Format = "#,##0";
											worksheet.Cells[rowIndex, 15].Value = data[iloop].ExpenseD.Amount;
											totalExpense = totalExpense + (decimal)data[iloop].ExpenseD.Amount;
										}
										worksheet.Cells[rowIndex, 16].Value = data[iloop].ExpenseD.Description;
										worksheet.Cells[rowIndex, 14, rowIndex, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
									}
									rowIndex = rowIndex + 1;
									#endregion
								}
								worksheet.Cells[15, 1, rowIndex, 18].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
								#region Title Sum
								// write total
								worksheet.Cells[transportRow - 1, 12].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[transportRow - 1, 12].Value = totalTransport + totalDetain;
								worksheet.Cells[transportRow - 1, 12, transportRow - 1, 13].Merge = true;

								// tax
								worksheet.Cells[transportRow, 12].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[transportRow, 12].Value = totalTax;
								worksheet.Cells[transportRow, 12, transportRow, 13].Merge = true;

								// after tax
								worksheet.Cells[transportTaxRow - 1, 12, transportTaxRow - 1, 13].Merge = true;
								worksheet.Cells[transportTaxRow - 1, 12].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[transportTaxRow - 1, 12].Formula = "SUM(L" + (transportRow - 1) + ":L" + (transportRow) + ")";
								if (lolo == "1")
								{
									worksheet.Cells[expenseRow - 1, 12].Style.Numberformat.Format = "#,##0";
									worksheet.Cells[expenseRow - 1, 12].Value = totalExpense;
									worksheet.Cells[expenseRow - 1, 12, expenseRow - 1, 13].Merge = true;
									worksheet.Cells[totalRow - 1, 12, totalRow - 1, 13].Merge = true;
									worksheet.Cells[totalRow - 1, 12].Style.Numberformat.Format = "#,##0";
									worksheet.Cells[totalRow - 1, 12].Formula = "SUM(L" + (transportTaxRow - 1) + ":L" + (expenseRow - 1) + ")";
									worksheet.Cells[totalRow - 1, 12].Style.Font.Bold = true;
								}
								else
								{
									worksheet.Cells[totalRow - 1, 12, totalRow - 1, 13].Merge = true;
									worksheet.Cells[totalRow - 1, 12].Style.Numberformat.Format = "#,##0";
									worksheet.Cells[totalRow - 1, 12].Formula = "SUM(L" + (transportTaxRow - 1) + ")";
									worksheet.Cells[totalRow - 1, 12].Style.Font.Bold = true;
								}
								#endregion
							}

							// add total end
							worksheet.Cells[rowIndex, 1].Value = (dictionary.ContainsKey("LBLTOTALREPORT")
								? dictionary["LBLTOTALREPORT"]
								: "");
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 1, rowIndex, 6].Merge = true;

							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.BorderAround(ExcelBorderStyle.Thin);
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

							worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[rowIndex, 12].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[rowIndex, 15].Style.Numberformat.Format = "#,##0";

							worksheet.Cells[rowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

							var sumstr = "SUM(K" + rowSumStart + ":K" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 11].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

							sumstr = "SUM(L" + rowSumStart + ":L" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 12].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";
							if (lolo == "1")
							{
								sumstr = "SUM(O" + rowSumStart + ":O" + (rowIndex - 1) + ")";
								worksheet.Cells[rowIndex, 15].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";
							}

							// write day
							if (intLanguague == 1)
							{
								worksheet.Cells[rowIndex + 1, 11].Value = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
																		  DateTime.Now.Year;
							}
							else if (intLanguague == 2)
							{
								cul = CultureInfo.GetCultureInfo("en-US");
								worksheet.Cells[rowIndex + 1, 11].Value = cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " +
																		  DateTime.Now.Day + ", " + DateTime.Now.Year;
							}
							else
							{
								worksheet.Cells[rowIndex + 1, 11].Value = DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day +
																		  "日";
							}

							// autoFit
							worksheet.Column(1).Width = 3.5;
							worksheet.Column(2).Width = 5;
							worksheet.Column(3).Width = 9;
							worksheet.Column(4).Width = 14;
							worksheet.Column(5).Width = 13;
							worksheet.Column(6).Width = 5;
							worksheet.Column(7).Width = 14;
							worksheet.Column(8).Width = 14;
							worksheet.Column(9).Width = 14;
							worksheet.Column(10).Width = 10;
							worksheet.Column(11).Width = 11;
							worksheet.Column(12).Width = 11;
							worksheet.Column(13).AutoFit();
							worksheet.Column(14).AutoFit();
							worksheet.Column(15).Width = 11;
							if (lolo == "1")
							{
								worksheet.Column(16).AutoFit();
							}
						}
					}

					if (package.Workbook.Worksheets.Count == 0)
					{
						package.Workbook.Worksheets.Add("NoData");
					}

					return new MemoryStream(package.GetAsByteArray());
				}
			}
			catch (NullReferenceException)
			{
				throw new NullReferenceException();
			}
		}

		public static Stream ExecLoadToExcel(List<CustomerExpenseReportData> dataList, Dictionary<string, string> dictionary,
						  int language, string companyName, string companyAddress,
						  string companyTaxCode, string monthYear, string fileName, string user)
		{
			try
			{
				using (ExcelPackage package = new ExcelPackage())
				{
					// set culture
					CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
					if (language == 2)
					{
						cul = CultureInfo.GetCultureInfo("en-US");
					}
					else if (language == 3)
					{
						cul = CultureInfo.GetCultureInfo("ja-JP");
					}

					for (var i = 0; i < dataList.Count; i++)
					{
						if (dataList[i].CustomerExpenseList != null && dataList[i].CustomerExpenseList.Count > 0)
						{
							var cusExpenseList = dataList[i].CustomerExpenseList;
							var rowIndex = 1;
							var colIndex = 1;
							var tolCol = 11;
							// add a new worksheet to the empty workbook
							ExcelWorksheet worksheet =
								package.Workbook.Worksheets.Add(dataList[i].CustomerMainC + "_" + dataList[i].CustomerSubC + "_" + i);

							// add image
							try
							{
								Bitmap image = new Bitmap(HttpContext.Current.Request.MapPath("~/Images/" + fileName));
								ExcelPicture excelImage = null;
								if (image != null)
								{
									excelImage = worksheet.Drawings.AddPicture("logo", image);
									excelImage.From.Column = 0;
									excelImage.From.Row = 0;
									excelImage.SetSize(80, 60);
								}
							}
							catch
							{
								// do nothing
							}
							#region Title
							//Add the title
							worksheet.Cells[rowIndex, 3].Value = companyName;
							worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 3].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 3, rowIndex, 6].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 3].Value = companyAddress;
							worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 3].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 3, rowIndex, 7].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 3].Value = (dictionary.ContainsKey("LBLTAXCODECOMPANYREPORT")
								? dictionary["LBLTAXCODECOMPANYREPORT"]
								: "") + ": " + companyTaxCode;
							worksheet.Cells[rowIndex, 3].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 3].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 3, rowIndex, 6].Merge = true;

							// next row
							rowIndex = rowIndex + 2;

							worksheet.Cells[rowIndex, 4].Value = (dictionary.ContainsKey("LBLCUSTOMEREXPENSELOADTITTLE")
								? dictionary["LBLCUSTOMEREXPENSELOADTITTLE"] : "");
							worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 4].Style.Font.Size = 16;
							worksheet.Cells[rowIndex, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 4, rowIndex, 8].Merge = true;

							// next row
							rowIndex = rowIndex + 1;

							worksheet.Cells[rowIndex, 4].Value = monthYear;
							worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
							worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[rowIndex, 4, rowIndex, 8].Merge = true;

							// next row - cus name
							rowIndex = rowIndex + 2;
							//LBLDEARREPORT
							worksheet.Cells[rowIndex, 1].Value = (dictionary.ContainsKey("LBLCUSTCODE") ? dictionary["LBLCUSTCODE"] : "") +
																			 ": " + dataList[i].CustomerMainC;
							worksheet.Cells[rowIndex, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 1, rowIndex, 5].Merge = true;

							// next row
							rowIndex = rowIndex + 1;
							worksheet.Cells[rowIndex, 1].Value = (dictionary.ContainsKey("LBLCUSTOMER")
								? dictionary["LBLCUSTOMER"] : "")
								+ ": " + dataList[i].CustomerN;
							worksheet.Cells[rowIndex, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 1, rowIndex, 5].Merge = true;

							// next row - address
							rowIndex = rowIndex + 1;
							worksheet.Cells[rowIndex, 1].Value = (dictionary.ContainsKey("LBLADDRESS")
								? dictionary["LBLADDRESS"] : "")
								+ ": " + dataList[i].CustomerAddress;
							worksheet.Cells[rowIndex, 1].Style.Font.Size = 11;
							worksheet.Cells[rowIndex, 1, rowIndex, 5].Merge = true;

							// next row
							//rowIndex = rowIndex + 1;
							//worksheet.Cells[rowIndex, 1].Value = (dictionary.ContainsKey("LBLTAXCODECOMPANYREPORT")
							//	? dictionary["LBLTAXCODECOMPANYREPORT"] : "")
							//	+ ": " + dataList[i].CustomerTaxCode;
							//worksheet.Cells[rowIndex, 1].Style.Font.Size = 11;
							//worksheet.Cells[rowIndex, 1, rowIndex, 5].Merge = true;
							#endregion

							// next row
							rowIndex = rowIndex + 2;

							#region Header
							// Add the headers
							colIndex = 1;
							// column1
							worksheet.Cells[rowIndex, colIndex].Value = "#";
							// column2
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLTRANSPORTDREPORT")
								? dictionary["LBLTRANSPORTDREPORT"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							// column3
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLTRUCKNO")
								? dictionary["LBLTRUCKNO"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							// column4
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLLOADINGLOC")
								? dictionary["LBLLOADINGLOC"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

							//column5
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLDISCHARGELOC")
								? dictionary["LBLDISCHARGELOC"]
								: "");
							worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
							// column6
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLADDRESS")
								? dictionary["LBLADDRESS"]
								: "");
							// column7
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("RPHDQUANTITYNETWEIGHT")
								? dictionary["RPHDQUANTITYNETWEIGHT"]
								: "");
							// column8
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("RPHDUNITPRICE")
								? dictionary["RPHDUNITPRICE"]
								: "");
							// column9
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("RPHDAMOUNT")
								? dictionary["RPHDAMOUNT"]
								: "");
							// column10
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("RPHDTOTALSURCHARGE")
								? dictionary["RPHDTOTALSURCHARGE"]
								: "");
							// column11
							colIndex++;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLDESCRIPTION")
								? dictionary["LBLDESCRIPTION"]
								: "");

							// set font bold
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Font.Bold = true;
							// set border
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
							// align center
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							// set color
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
							worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
							#endregion

							// next row
							rowIndex++;
							var rowSumStart = rowIndex;

							// write content
							if (cusExpenseList != null && cusExpenseList.Count > 0)
							{
								var newKey = "";
								var oldKey = "";
								decimal totalDetainAmount = 0;
								decimal detainAmount = 0;
								decimal totalTransport = 0;
								decimal totalTax = 0;
								decimal taxRate = 0;
								int index = 1;
								for (int jloop = 0; jloop < cusExpenseList.Count; jloop++)
								{
									#region Detail
									decimal amount = 0;
									decimal taxAmount = 0;
									if (cusExpenseList[jloop].OrderD.Amount != null)
									{
										amount = (cusExpenseList[jloop].OrderD.Amount ?? 0) + (cusExpenseList[jloop].OrderD.DetainAmount ?? 0);
										taxAmount = cusExpenseList[jloop].TaxAmount;
									}

									newKey = ((amount == 0) ? 0 : Math.Round(taxAmount * 100 / (amount))).ToString();

									if (newKey != oldKey)
									{
										if ((int)((cusExpenseList[jloop].OrderD.Amount ?? 0) + (cusExpenseList[jloop].OrderD.DetainAmount ?? 0)) == 0)
										{
											newKey = ((int)cusExpenseList[jloop].CustomerSettlement.TaxRate).ToString();
										}
										if (newKey != oldKey)
										{
											oldKey = newKey;
											index = 1;

											if (rowIndex > 13)
											{
												#region Title Sum
												// write total
												//rowIndex++;
												worksheet.Cells[rowIndex, 9].Value = (dictionary.ContainsKey("LBLTRANSPORTFEEREPORT")
													? dictionary["LBLTRANSPORTFEEREPORT"]
													: "") + ": ";
												worksheet.Cells[rowIndex, 9].Style.Font.Size = 11;
												worksheet.Cells[rowIndex, 9, rowIndex, 10].Merge = true;
												worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0";
												worksheet.Cells[rowIndex, 11].Value = totalTransport + totalDetainAmount;
												worksheet.Cells[rowIndex, 9, rowIndex, tolCol].Style.Font.Bold = true;
												// tax
												rowIndex++;
												worksheet.Cells[rowIndex, 9].Value = (dictionary.ContainsKey("LBLTAXVAT") ? dictionary["LBLTAXVAT"] : "") + " " +
																									 taxRate + "%";
												worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0";
												worksheet.Cells[rowIndex, 11].Value = totalTax;

												worksheet.Cells[rowIndex, 9].Style.Font.Size = 11;
												worksheet.Cells[rowIndex, 9, rowIndex, 10].Merge = true;
												worksheet.Cells[rowIndex, 9, rowIndex, tolCol].Style.Font.Bold = true;
												// after tax
												rowIndex++;
												worksheet.Cells[rowIndex, 9].Value = (dictionary.ContainsKey("LBLTRANSPORTFEEAFTERTAX")
													? dictionary["LBLTRANSPORTFEEAFTERTAX"]
													: "") + ":";
												worksheet.Cells[rowIndex, 9].Style.Font.Size = 11;
												worksheet.Cells[rowIndex, 9, rowIndex, 10].Merge = true;
												worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0";
												worksheet.Cells[rowIndex, 11].Value = totalTransport + totalDetainAmount + totalTax;
												worksheet.Cells[rowIndex, 9, rowIndex, tolCol].Style.Font.Bold = true;

												//reset
												totalTransport = 0;
												totalTax = 0;
												rowIndex = rowIndex + 2;
												worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;

												#endregion
											}
										}
									}

									worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
									worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

									// column1
									colIndex = 1;
									worksheet.Cells[rowIndex, colIndex].Value = index++;
									worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
									worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
									// column2
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Value = (DateTime)cusExpenseList[jloop].OrderD.ActualLoadingD;
									worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "dd/MM/yy";
									if (language == 2)
									{
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "MM/dd/yy";
									}
									else if (language == 3)
									{
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "yy/MM/dd";
									}
									// column3
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Value = cusExpenseList[jloop].TruckNo;
									// column4
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Value = cusExpenseList[jloop].OrderH.LoadingPlaceN;
									worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
									// column5
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Value = cusExpenseList[jloop].OrderH.StopoverPlaceN;
									worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
									// column6
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Value = cusExpenseList[jloop].OrderH.DischargePlaceN;
									worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
									// column7
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Value = cusExpenseList[jloop].OrderD.NetWeight > 0
										? cusExpenseList[jloop].OrderD.NetWeight : 0;
									worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
									// column8
									colIndex++;
									if (cusExpenseList[jloop].OrderD.UnitPrice != null)
									{
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
										worksheet.Cells[rowIndex, colIndex].Value = cusExpenseList[jloop].OrderD.UnitPrice;
									}
									// column9
									colIndex++;
									if (cusExpenseList[jloop].OrderD.Amount != null)
									{
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
										amount = cusExpenseList[jloop].OrderD.Amount ?? 0;
										worksheet.Cells[rowIndex, colIndex].Value = (decimal)cusExpenseList[jloop].OrderD.Amount;
										totalTransport += amount;
									}
									// column10
									colIndex++;
									if (cusExpenseList[jloop].OrderD.DetainAmount != null)
									{
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
										detainAmount = (decimal)cusExpenseList[jloop].OrderD.DetainAmount;
										worksheet.Cells[rowIndex, colIndex].Value = detainAmount;
										totalDetainAmount += detainAmount;

									}
									// column11
									colIndex++;
									var surchargeDescription = cusExpenseList[jloop].Description;
									if (String.IsNullOrEmpty(surchargeDescription) && String.IsNullOrWhiteSpace(surchargeDescription))
									{
										worksheet.Cells[rowIndex, colIndex].Value = cusExpenseList[jloop].OrderD.Description;
									}
									else
									{
										worksheet.Cells[rowIndex, colIndex].Value = surchargeDescription + ", " + cusExpenseList[jloop].OrderD.Description;
									}

									worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

									//tax rate
									taxAmount = cusExpenseList[jloop].TaxAmount;
									//taxRate = ((amount + detainAmount) == 0) ? 0 : Math.Round(taxAmount * 100 / (amount + detainAmount));
									if (amount + detainAmount == 0)
									{
										taxRate = (int)(cusExpenseList[jloop].CustomerSettlement.TaxRate ?? 0);
									}
									else
									{
										taxRate = Math.Round(taxAmount * 100 / (amount + detainAmount));
									}

									totalTax += taxAmount;

									// next row
									rowIndex++;
									worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;

									#endregion
								}
								// add total end
								#region Title Sum
								//rowIndex++;
								worksheet.Cells[rowIndex, 9].Value = (dictionary.ContainsKey("LBLTRANSPORTFEEREPORT")
									? dictionary["LBLTRANSPORTFEEREPORT"]
									: "") + ": ";
								worksheet.Cells[rowIndex, 9].Style.Font.Size = 11;
								worksheet.Cells[rowIndex, 9, rowIndex, 10].Merge = true;
								worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[rowIndex, 11].Value = totalTransport + totalDetainAmount;
								worksheet.Cells[rowIndex, 9, rowIndex, tolCol].Style.Font.Bold = true;
								// tax
								rowIndex++;
								if (dictionary.ContainsKey("COMPANYTAXRATE") && Convert.ToDecimal(dictionary["COMPANYTAXRATE"]) % 10 == 0)
								{
									worksheet.Cells[rowIndex, 9].Value = (dictionary.ContainsKey("LBLTAXVAT") ? dictionary["LBLTAXVAT"] : "") + " " +
																					 Math.Round(taxRate) + "%";
								}
								else
								{
									worksheet.Cells[rowIndex, 9].Value = (dictionary.ContainsKey("LBLTAXVAT") ? dictionary["LBLTAXVAT"] : "") + " " +
																					 taxRate + "%";
								}
								worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[rowIndex, 11].Value = totalTax;

								worksheet.Cells[rowIndex, 9].Style.Font.Size = 11;
								worksheet.Cells[rowIndex, 9, rowIndex, 10].Merge = true;
								worksheet.Cells[rowIndex, 9, rowIndex, tolCol].Style.Font.Bold = true;
								// after tax
								rowIndex++;
								worksheet.Cells[rowIndex, 9].Value = (dictionary.ContainsKey("LBLTRANSPORTFEEAFTERTAX")
									? dictionary["LBLTRANSPORTFEEAFTERTAX"]
									: "") + ":";
								worksheet.Cells[rowIndex, 9].Style.Font.Size = 11;
								worksheet.Cells[rowIndex, 9, rowIndex, 10].Merge = true;
								worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[rowIndex, 11].Value = totalTransport + totalTax + totalDetainAmount;
								worksheet.Cells[rowIndex, 9, rowIndex, tolCol].Style.Font.Bold = true;
								//reset
								totalTransport = 0;
								totalTax = 0;
								#endregion
							}
							// write day
							rowIndex = rowIndex + 2;
							if (language == 1)
							{
								worksheet.Cells[rowIndex, 6].Value = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
																		  DateTime.Now.Year;
							}
							else if (language == 2)
							{
								cul = CultureInfo.GetCultureInfo("en-US");
								worksheet.Cells[rowIndex, 6].Value = cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " +
																		  DateTime.Now.Day + ", " + DateTime.Now.Year;
							}
							else
							{
								worksheet.Cells[rowIndex, 6].Value = DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day +
																		  "日";
							}

							// autoFit
							worksheet.Column(1).Width = 3.5;
							worksheet.Column(2).Width = 9;
							worksheet.Column(3).Width = 10;
							worksheet.Column(4).Width = 25;
							worksheet.Column(5).Width = 30;
							worksheet.Column(6).Width = 30;
							worksheet.Column(7).Width = 10;
							worksheet.Column(8).Width = 12;
							worksheet.Column(9).Width = 12;
							worksheet.Column(10).Width = 12;
							worksheet.Column(11).Width = 25;
						}
					}

					if (package.Workbook.Worksheets.Count == 0)
					{
						package.Workbook.Worksheets.Add("NoData");
					}

					return new MemoryStream(package.GetAsByteArray());
				}
			}
			catch (NullReferenceException)
			{
				throw new NullReferenceException();
			}
		}
		public static Stream ExportCustomerExpenseListToExcelGeneral(List<CustomerExpenseReportData> dataList, Dictionary<string, string> dictionary, int intLanguague, string fileName, string lolo)
		{
			using (ExcelPackage package = new ExcelPackage())
			{
				// set culture
				CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
				if (intLanguague == 2)
				{
					cul = CultureInfo.GetCultureInfo("en-US");
				}
				else if (intLanguague == 3)
				{
					cul = CultureInfo.GetCultureInfo("ja-JP");
				}

				for (var i = 0; i < dataList.Count; i++)
				{
					if (dataList[i].CustomerExpenseList != null && dataList[i].CustomerExpenseList.Count > 0)
					{
						var data = dataList[i].CustomerExpenseList;
						var rowIndex = 1;
						var colIndex = 1;
						var tolCol = lolo == "1" ? 22 : 16;
						// add a new worksheet to the empty workbook
						ExcelWorksheet worksheet =
							package.Workbook.Worksheets.Add((!string.IsNullOrEmpty(dataList[i].CustomerShortN) ? dataList[i].CustomerShortN : (dataList[i].CustomerMainC + "_" + dataList[i].CustomerSubC)) + "_" + i);
						// add image
						//Bitmap image = new Bitmap(HttpContext.Current.Request.MapPath("~/Images/logo.bmp"));
						try
						{
							Bitmap image = new Bitmap(HttpContext.Current.Request.MapPath("~/Images/" + fileName));
							ExcelPicture excelImage = null;
							if (image != null)
							{
								excelImage = worksheet.Drawings.AddPicture("logo", image);
								excelImage.From.Column = 0;
								excelImage.From.Row = 0;
								excelImage.SetSize(100, 75);
							}
						}
						catch
						{
							// do nothing
						}
						#region Title
						//Add the title
						worksheet.Cells[rowIndex, 4].Value = dictionary.ContainsKey("COMPANYNAME") ? dictionary["COMPANYNAME"] : "";
						worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
						worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
						worksheet.Cells[rowIndex, 4, rowIndex, 9].Merge = true;

						// next row
						rowIndex = rowIndex + 1;

						worksheet.Cells[rowIndex, 4].Value = dictionary.ContainsKey("COMPANYADDRESS") ? dictionary["COMPANYADDRESS"] : "";
						worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
						worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
						worksheet.Cells[rowIndex, 4, rowIndex, 9].Merge = true;

						// next row
						rowIndex = rowIndex + 1;

						worksheet.Cells[rowIndex, 4].Value = (dictionary.ContainsKey("LBLTAXCODECOMPANYREPORT")
							? dictionary["LBLTAXCODECOMPANYREPORT"]
							: "") + ": " + (dictionary.ContainsKey("COMPANYTAXCODE") ? dictionary["COMPANYTAXCODE"] : "");
						worksheet.Cells[rowIndex, 4].Style.Font.Bold = true;
						worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
						worksheet.Cells[rowIndex, 4, rowIndex, 6].Merge = true;

						// next row
						rowIndex = rowIndex + 2;

						worksheet.Cells[rowIndex, 5].Value = (dictionary.ContainsKey("TLTTRANSPORTFEEREPORT")
							? dictionary["TLTTRANSPORTFEEREPORT"]
							: "");
						worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
						worksheet.Cells[rowIndex, 5].Style.Font.Size = 19;
						worksheet.Cells[rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells[rowIndex, 5, rowIndex, 12].Merge = true;

						// next row
						rowIndex = rowIndex + 1;

						worksheet.Cells[rowIndex, 5].Value = (dictionary.ContainsKey("LBLMONTHYEAR") ? dictionary["LBLMONTHYEAR"] : "");
						worksheet.Cells[rowIndex, 5].Style.Font.Bold = true;
						worksheet.Cells[rowIndex, 5].Style.Font.Size = 11;
						worksheet.Cells[rowIndex, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells[rowIndex, 5, rowIndex, 12].Merge = true;

						// next row
						//left
						rowIndex = rowIndex + 2;

						var transportRow = rowIndex;
						worksheet.Cells[rowIndex - 1, 14].Value = (dictionary.ContainsKey("LBLBEFORETAX")
							? dictionary["LBLBEFORETAX"]
							: "") + ": ";
						worksheet.Cells[rowIndex - 1, 14].Style.Font.Size = 11;
						worksheet.Cells[rowIndex - 1, 14, rowIndex - 1, 15].Merge = true;

						//right
						worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLDEARREPORT")
							? dictionary["LBLDEARREPORT"] : "") + ": " + dataList[i].InvoiceN;
						worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
						worksheet.Cells[rowIndex - 1, 1].Style.Font.Bold = true;
						worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 13].Merge = true;

						// next row
						//left
						rowIndex = rowIndex + 1;
						if (dictionary.ContainsKey("COMPANYTAXRATE") && Convert.ToDecimal(dictionary["COMPANYTAXRATE"]) % 10 == 0)
						{
							worksheet.Cells[rowIndex - 1, 14].Value = (dictionary.ContainsKey("LBLTAXAMOUNTREPORT") ? dictionary["LBLTAXAMOUNTREPORT"] : "") + " " +
																 Math.Round(dataList[i].TaxRate) + "%";
						}
						else
						{
							worksheet.Cells[rowIndex - 1, 14].Value = (dictionary.ContainsKey("LBLTAXAMOUNTREPORT") ? dictionary["LBLTAXAMOUNTREPORT"] : "") + " " +
																 dataList[i].TaxRate + "%";
						}

						worksheet.Cells[rowIndex - 1, 14].Style.Font.Size = 11;
						worksheet.Cells[rowIndex - 1, 14, rowIndex - 1, 15].Merge = true;

						// right
						worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLCUSTOMER")
							? dictionary["LBLCUSTOMER"] : "")
							+ ": " + dataList[i].CustomerN;
						worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
						worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 13].Merge = true;
						//net row
						//left
						rowIndex = rowIndex + 1;
						var transportTaxRow = rowIndex;
						worksheet.Cells[rowIndex - 1, 14].Value = (dictionary.ContainsKey("LBLAFTERTAXAMOUNTRP")
							? dictionary["LBLAFTERTAXAMOUNTRP"]
							: "") + ":";
						worksheet.Cells[rowIndex - 1, 14].Style.Font.Size = 11;
						worksheet.Cells[rowIndex - 1, 14, rowIndex - 1, 15].Merge = true;

						// right
						worksheet.Cells[rowIndex - 1, 1].Value = (dictionary.ContainsKey("LBLADDRESS")
							? dictionary["LBLADDRESS"] : "")
							+ ": " + dataList[i].CustomerAddress;
						worksheet.Cells[rowIndex - 1, 1].Style.Font.Size = 11;
						worksheet.Cells[rowIndex - 1, 1, rowIndex - 1, 13].Merge = true;

						//next row
						rowIndex = rowIndex + 1;
						var expenseRow = rowIndex;
						if (lolo == "1")
						{
							worksheet.Cells[rowIndex - 1, 14].Value = (dictionary.ContainsKey("LBLPAYONBEHALFRP")
							? dictionary["LBLPAYONBEHALFRP"]
							: "") + ":";
							worksheet.Cells[rowIndex - 1, 14].Style.Font.Size = 11;
							worksheet.Cells[rowIndex - 1, 14, rowIndex - 1, 15].Merge = true;
						}

						// next row
						rowIndex = rowIndex + 1;

						var totalRow = rowIndex;
						worksheet.Cells[rowIndex - 1, 14].Value = (dictionary.ContainsKey("LBLTOTALAMOUNTREPORT")
							? dictionary["LBLTOTALAMOUNTREPORT"]
							: "") + ":";
						worksheet.Cells[rowIndex - 1, 14].Style.Font.Bold = true;
						//worksheet.Cells[rowIndex, 15].Style.Font.Size = 14;
						worksheet.Cells[rowIndex - 1, 14, rowIndex - 1, 15].Merge = true;
						#endregion


						#region Header
						// Add the headers
						colIndex = 1;
						// column1
						worksheet.Cells[rowIndex, colIndex].Value = "#";
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						// column2
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLORDERTYPEDISPATCH")
							? dictionary["LBLORDERTYPEDISPATCH"]
							: "");
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						// column3
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLDATEREPORT")
							? dictionary["LBLDATEREPORT"]
							: "");
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						//column4
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLBLBKREPORT")
							? dictionary["LBLBLBKREPORT"]
							: "");
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						// column5
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLCONTNUMBER")
							? dictionary["LBLCONTNUMBER"]
							: "");
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						// column6
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLCATEGORY")
							? dictionary["LBLCATEGORY"]
							: "");
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
						// column7-9
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLSTOPOVERPLACESHORT")
							? dictionary["LBLSTOPOVERPLACESHORT"]
							: "");
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLLOADINGPLACESHORT")
							? dictionary["LBLLOADINGPLACESHORT"]
							: "");
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;

						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLDISCHARGEPLACESHORT")
							? dictionary["LBLDISCHARGEPLACESHORT"]
							: "");
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
						// column10
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLTRUCKNO")
							? dictionary["LBLTRUCKNO"]
							: "");
						worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
						worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
						// column11-13
						colIndex++;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLPOSTAGE")
							? dictionary["LBLPOSTAGE"]
							: "");
						worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 2].Merge = true;
						worksheet.Cells[rowIndex + 1, colIndex].Value = (dictionary.ContainsKey("LBLPOSTAGE")
							? dictionary["LBLPOSTAGE"]
							: "");
						worksheet.Cells[rowIndex + 1, colIndex + 1].Value = (dictionary.ContainsKey("LBLSURCHARGEFEE")
							? dictionary["LBLSURCHARGEFEE"]
							: "");
						worksheet.Cells[rowIndex + 1, colIndex + 2].Value = (dictionary.ContainsKey("LBLEXPLAIN")
							? dictionary["LBLEXPLAIN"]
							: "");
						// column14-19
						colIndex = colIndex + 3;
						worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLLOLOFEEINCLUDEVAT")
							? dictionary["LBLLOLOFEEINCLUDEVAT"]
							: "");
						worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 2].Merge = true;
						worksheet.Cells[rowIndex + 1, colIndex].Value = (dictionary.ContainsKey("LBLLIFT")
							? dictionary["LBLLIFT"]
							: "");
						worksheet.Cells[rowIndex + 1, colIndex + 1].Value = (dictionary.ContainsKey("LBLLOWERED")
							? dictionary["LBLLOWERED"]
							: "");
						worksheet.Cells[rowIndex + 1, colIndex + 2].Value = (dictionary.ContainsKey("LBLOTHER")
							? dictionary["LBLOTHER"]
							: "");
						if (lolo == "1")
						{
							// column20-25
							colIndex = colIndex + 3;
							worksheet.Cells[rowIndex, colIndex].Value = (dictionary.ContainsKey("LBLISREQUESTEDRP")
								? dictionary["LBLISREQUESTEDRP"]
								: "");
							worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 5].Merge = true;
							worksheet.Cells[rowIndex + 1, colIndex].Value = (dictionary.ContainsKey("LBLLIFT")
								? dictionary["LBLLIFT"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 1].Value = (dictionary.ContainsKey("LBLBILLLIFTON")
								? dictionary["LBLBILLLIFTON"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 2].Value = (dictionary.ContainsKey("LBLLOWERED")
								? dictionary["LBLLOWERED"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 3].Value = (dictionary.ContainsKey("LBLBILLLIFTOFF")
								? dictionary["LBLBILLLIFTOFF"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 4].Value = (dictionary.ContainsKey("LBLOTHER")
								? dictionary["LBLOTHER"]
								: "");
							worksheet.Cells[rowIndex + 1, colIndex + 5].Value = (dictionary.ContainsKey("LBLEXPLAIN")
								? dictionary["LBLEXPLAIN"]
								: "");
						}
						// set font bold
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Font.Bold = true;
						// set border
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
						// align center
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						// set color
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);
						#endregion

						// next row
						rowIndex = rowIndex + 2;
						var rowSumStart = rowIndex;

						// write content
						if (data != null && data.Count > 0)
						{
							var keyNew = "";
							var keyOld = "";
							//decimal totalColumn19 = 0;
							decimal totalTransport = 0;
							decimal totalDetain = 0;
							decimal totalExpense = 0;
							decimal totalLoLo = 0;
							decimal totalTax = 0;
							int rowOfGroup = 0;
							int index = 1;
							for (int iloop = 0; iloop < data.Count; iloop++)
							{
								int totalcol = lolo == "1" ? 22 : 16;
								int countcellnull = 0;
								for (int checkcolnull = 1; checkcolnull <= totalcol; checkcolnull++)
								{
									countcellnull = worksheet.Cells[rowIndex - 1, checkcolnull].Value == null ? countcellnull + 1 : countcellnull;
								}
								if (countcellnull == 22 || countcellnull == 16)
								{
									rowIndex--;
								}

								#region Detail
								worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

								keyNew = data[iloop].OrderD.OrderD + data[iloop].OrderD.OrderNo + data[iloop].OrderD.DetailNo;
								if (keyNew != keyOld)
								{
									worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;

									rowOfGroup = 1;
									keyOld = keyNew;
									// column1
									colIndex = 1;
									worksheet.Cells[rowIndex, colIndex].Value = index++;
									worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
									worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
									// column2
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Value = Utilities.GetOrderTypeName(data[iloop].OrderH.OrderTypeI);
									// column3
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "dd/MM/yy";
									if (intLanguague == 2)
									{
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "MM/dd/yy";
									}
									else if (intLanguague == 3)
									{
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "yy/MM/dd";
									}
									worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.ActualLoadingD;
									// column4
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderH.BLBK;
									// column5
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.ContainerNo;
									worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
									// column6
									colIndex++;
									if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE1)
									{
										worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE1N;
										worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
										worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
									}
									else if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE2)
									{
										worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE2N;
										worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
										worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
									}
									else if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE3)
									{
										worksheet.Cells[rowIndex, colIndex].Value = Constants.CONTAINERSIZE3N;
										worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
										worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
									}
									else
									{
										worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLLOAD"] + (data[iloop].OrderD.NetWeight);
										worksheet.Cells[rowIndex, colIndex].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
										worksheet.Cells[rowIndex, colIndex].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
										worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
									}
									// column7-9
									colIndex++;
									if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch1))
									{
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.LocationDispatch1;
										worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
									}
									if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch2))
									{
										worksheet.Cells[rowIndex, colIndex + 1].Value = data[iloop].OrderD.LocationDispatch2;
										worksheet.Cells[rowIndex, colIndex + 1].Style.WrapText = true;
									}
									if (!string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch3))
									{
										worksheet.Cells[rowIndex, colIndex + 2].Value = data[iloop].OrderD.LocationDispatch3;
										worksheet.Cells[rowIndex, colIndex + 2].Style.WrapText = true;
									}
									// column10
									colIndex = colIndex + 3;
									worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.RegisteredNo;
									worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
									// column 11
									colIndex++;
									if (data[iloop].OrderD.Amount != null)
									{
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].OrderD.Amount;
									}
									// column 12
									colIndex++;
									if (data[iloop].DetainAmount + data[iloop].SurchargeAmount != 0)
									{
										worksheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#,##0";
										worksheet.Cells[rowIndex, colIndex].Value = data[iloop].DetainAmount + data[iloop].SurchargeAmount;
										totalDetain = totalDetain + data[iloop].DetainAmount + data[iloop].SurchargeAmount;
									}
									// column 13
									colIndex++;
									worksheet.Cells[rowIndex, colIndex].Value = data[iloop].Description;
									worksheet.Cells[rowIndex, colIndex].Style.WrapText = true;
									if (data[iloop].OrderD.Amount != null)
									{
										totalTransport = totalTransport + (decimal)data[iloop].OrderD.Amount;
										totalTax += data[iloop].TaxAmount;
									}

									//LOLO
									var checkcanrow = 0;
									int rowIndexRoot = rowIndex;
									if (data[iloop].ExpenseD.LiftOnList != null && data[iloop].ExpenseD.LiftOnList.Count > 0)
									{
										for (int li = 0; li < data[iloop].ExpenseD.LiftOnList.Count; li++)
										{
											if (data[iloop].ExpenseD.LiftOnList[li].IsIncludedLiftOn == "1" &&
											    data[iloop].ExpenseD.LiftOnList[li].IsRequestedLiftOn == "1" &&
											    data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn != null &&
											    data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn > 0)
											{
												worksheet.Cells[rowIndex, 14].Style.Numberformat.Format = "#,##0";
												worksheet.Cells[rowIndex, 14].Value = data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn + data[iloop].ExpenseD.LiftOnList[li].TaxAmountLiftOn;
												totalLoLo = totalLoLo + (decimal)data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn + (decimal)data[iloop].ExpenseD.LiftOnList[li].TaxAmountLiftOn;
											}
											rowIndex++;
											checkcanrow++;
											if (li < data[iloop].ExpenseD.LiftOnList.Count - 1)
											{
												worksheet.Cells[rowIndex, 14, rowIndex, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
												worksheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
												worksheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
											}
										}
									}
									//column16,17
									if (data[iloop].ExpenseD.LiftOffList != null && data[iloop].ExpenseD.LiftOffList.Count > 0)
									{
										rowIndex = data[iloop].ExpenseD.LiftOnList != null
											? rowIndex - data[iloop].ExpenseD.LiftOnList.Count
											: rowIndex;
										for (int lj = 0; lj < data[iloop].ExpenseD.LiftOffList.Count; lj++)
										{
											if (data[iloop].ExpenseD.LiftOffList[lj].IsIncludedLiftOff == "1" &&
												data[iloop].ExpenseD.LiftOffList[lj].IsRequestedLiftOff == "1" &&
												data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff != null &&
												data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff > 0)
											{
												worksheet.Cells[rowIndex, 15].Style.Numberformat.Format = "#,##0";
												worksheet.Cells[rowIndex, 15].Value = data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff + data[iloop].ExpenseD.LiftOffList[lj].TaxAmountLiftOff;
												totalLoLo = totalLoLo + (decimal)data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff + (decimal)data[iloop].ExpenseD.LiftOffList[lj].TaxAmountLiftOff;
											}
											
											rowIndex++;
											checkcanrow++;
											if (lj < data[iloop].ExpenseD.LiftOffList.Count - 1)
											{
												worksheet.Cells[rowIndex, 14, rowIndex, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
												worksheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
												worksheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
											}
										}
									}
									//column18,19
									if (data[iloop].ExpenseD.OtherListLoLo != null && data[iloop].ExpenseD.OtherListLoLo.Count > 0)
									{
										string description = null;
										decimal? amount = 0;
										rowIndex = data[iloop].ExpenseD.LiftOffList != null
											? rowIndex - data[iloop].ExpenseD.LiftOffList.Count
											: rowIndex;
										for (int lk = 0; lk < data[iloop].ExpenseD.OtherListLoLo.Count; lk++)
										{
											if (data[iloop].ExpenseD.OtherListLoLo[lk].IsIncludedOther == "1" &&
												data[iloop].ExpenseD.OtherListLoLo[lk].IsRequestedOther == "1" &&
												data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther != null &&
												data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther > 0)
											{
												description = (data[iloop].ExpenseD.OtherListLoLo[lk].DescriptionOther != "" &&
															   !string.IsNullOrEmpty(data[iloop].ExpenseD.OtherListLoLo[lk].DescriptionOther))
													? (((description != "") ? (description + ",") : "") +
													   data[iloop].ExpenseD.OtherListLoLo[lk].DescriptionOther)
													: description;
												amount = amount + data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther + data[iloop].ExpenseD.OtherListLoLo[lk].TaxAmountOther;
												totalLoLo = totalLoLo + (decimal)data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther + (decimal)data[iloop].ExpenseD.OtherListLoLo[lk].TaxAmountOther;
											}
										}
										worksheet.Cells[rowIndex, 16].Style.Numberformat.Format = "#,##0";
										worksheet.Cells[rowIndex, 16].Value = amount;
										rowIndex++;
										checkcanrow++;
									}
									if (lolo == "1")
									{
										if (checkcanrow < 1)
										{
											rowIndex = rowIndex + 1;
										}
										else
										{
											rowIndex = Math.Max(Math.Max((rowIndexRoot + (data[iloop].ExpenseD.LiftOnList != null ? data[iloop].ExpenseD.LiftOnList.Count(p => p.IsIncludedLiftOn == "1" && p.IsRequestedLiftOn == "1") : 0)),
												(rowIndexRoot + (data[iloop].ExpenseD.LiftOffList != null ? data[iloop].ExpenseD.LiftOffList.Count(p => p.IsIncludedLiftOff == "1" && p.IsRequestedLiftOff == "1") : 0))),
												(rowIndexRoot + (data[iloop].ExpenseD.OtherListLoLo != null ? data[iloop].ExpenseD.OtherListLoLo.Count(p => p.IsIncludedOther == "1" && p.IsRequestedOther == "1") : 0)));
										}
										//chi ho
										//column20,21
										var comparerowIndex = rowIndex;
										rowIndex = rowIndexRoot;
										var checkcanrow2 = 0;
										int rowIndexRoot2 = rowIndexRoot;
										if (data[iloop].ExpenseD.LiftOnList != null && data[iloop].ExpenseD.LiftOnList.Count > 0)
										{
											for (int li = 0; li < data[iloop].ExpenseD.LiftOnList.Count; li++)
											{
												if (data[iloop].ExpenseD.LiftOnList[li].IsRequestedLiftOn == "1" &&
													data[iloop].ExpenseD.LiftOnList[li].IsIncludedLiftOn == "0" &&
													data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn != null &&
													data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn > 0)
												{
													worksheet.Cells[rowIndex, 17].Style.Numberformat.Format = "#,##0";
													worksheet.Cells[rowIndex, 17].Value = data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn;
													totalExpense = totalExpense + (decimal)data[iloop].ExpenseD.LiftOnList[li].AmountLiftOn;
													worksheet.Cells[rowIndex, 18].Value = data[iloop].ExpenseD.LiftOnList[li].DescriptionLiftOn;
													worksheet.Cells[rowIndex, 18].Style.WrapText = true;
												}

												rowIndex++;
												checkcanrow2++;
												if (li < data[iloop].ExpenseD.LiftOnList.Count - 1)
												{
													worksheet.Cells[rowIndex, 17, rowIndex, 22].Style.Border.Top.Style = ExcelBorderStyle.Thin;
													worksheet.Cells[rowIndex, 1, rowIndex, 22].Style.Border.Left.Style = ExcelBorderStyle.Thin;
													worksheet.Cells[rowIndex, 1, rowIndex, 22].Style.Border.Right.Style = ExcelBorderStyle.Thin;
												}
											}
										}
										//column22,23
										if (data[iloop].ExpenseD.LiftOffList != null && data[iloop].ExpenseD.LiftOffList.Count > 0)
										{
											rowIndex = data[iloop].ExpenseD.LiftOnList != null
												? rowIndex - data[iloop].ExpenseD.LiftOnList.Count
												: rowIndex;
											for (int lj = 0; lj < data[iloop].ExpenseD.LiftOffList.Count; lj++)
											{
												if (data[iloop].ExpenseD.LiftOffList[lj].IsRequestedLiftOff == "1" &&
													data[iloop].ExpenseD.LiftOffList[lj].IsIncludedLiftOff == "0" &&
													data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff != null &&
													data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff > 0)
												{
													worksheet.Cells[rowIndex, 19].Style.Numberformat.Format = "#,##0";
													worksheet.Cells[rowIndex, 19].Value = data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff;
													totalExpense = totalExpense + (decimal)data[iloop].ExpenseD.LiftOffList[lj].AmountLiftOff;
													worksheet.Cells[rowIndex, 20].Value = data[iloop].ExpenseD.LiftOffList[lj].DescriptionLiftOff;
													worksheet.Cells[rowIndex, 20].Style.WrapText = true;
												}
												rowIndex++;
												checkcanrow2++;
												if (lj < data[iloop].ExpenseD.LiftOffList.Count - 1)
												{
													worksheet.Cells[rowIndex, 17, rowIndex, 22].Style.Border.Top.Style = ExcelBorderStyle.Thin;
													worksheet.Cells[rowIndex, 1, rowIndex, 22].Style.Border.Left.Style = ExcelBorderStyle.Thin;
													worksheet.Cells[rowIndex, 1, rowIndex, 22].Style.Border.Right.Style = ExcelBorderStyle.Thin;
												}
											}
										}
										//column24,25
										if (data[iloop].ExpenseD.OtherListLoLo != null && data[iloop].ExpenseD.OtherListLoLo.Count > 0)
										{
											string description = null;
											decimal? amount = 0;
											rowIndex = data[iloop].ExpenseD.LiftOffList != null
												? rowIndex - data[iloop].ExpenseD.LiftOffList.Count
												: rowIndex;
											for (int lk = 0; lk < data[iloop].ExpenseD.OtherListLoLo.Count; lk++)
											{
												if (data[iloop].ExpenseD.OtherListLoLo[lk].IsRequestedOther == "1" &&
													data[iloop].ExpenseD.OtherListLoLo[lk].IsIncludedOther == "0" &&
													data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther != null &&
													data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther > 0)
												{
													description = (data[iloop].ExpenseD.OtherListLoLo[lk].DescriptionOther != "" &&
																   !string.IsNullOrEmpty(data[iloop].ExpenseD.OtherListLoLo[lk].DescriptionOther))
														? (((description != "") ? (description + ",") : "") +
														   data[iloop].ExpenseD.OtherListLoLo[lk].DescriptionOther)
														: description;
													amount = amount + data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther;
													totalExpense = totalExpense + (decimal)data[iloop].ExpenseD.OtherListLoLo[lk].AmountOther;
												}
											}
											worksheet.Cells[rowIndex, 21].Style.Numberformat.Format = "#,##0";
											worksheet.Cells[rowIndex, 21].Value = amount;
											worksheet.Cells[rowIndex, 22].Value = description;
											worksheet.Cells[rowIndex, 22].Style.WrapText = true;
											rowIndex++;
											checkcanrow2++;
										}
										if (checkcanrow2 < 1)
										{
											rowIndex = rowIndex + 1;
										}
										else
										{
											rowIndex = Math.Max(Math.Max(Math.Max((rowIndexRoot2 + (data[iloop].ExpenseD.LiftOnList != null ? data[iloop].ExpenseD.LiftOnList.Count(p => p.IsRequestedLiftOn == "1" && p.IsIncludedLiftOn == "0") : 0)),
												(rowIndexRoot2 + (data[iloop].ExpenseD.LiftOffList != null ? data[iloop].ExpenseD.LiftOffList.Count(p => p.IsRequestedLiftOff == "1" && p.IsIncludedLiftOff == "0") : 0))),
												(rowIndexRoot2 + (data[iloop].ExpenseD.OtherListLoLo != null ? data[iloop].ExpenseD.OtherListLoLo.Count(p => p.IsRequestedOther == "1" && p.IsIncludedOther == "0") : 0))), comparerowIndex);
											if (worksheet.Cells[rowIndex - 1, 17].Value == null && worksheet.Cells[rowIndex - 1, 18].Value == null &&
												worksheet.Cells[rowIndex - 1, 19].Value == null && worksheet.Cells[rowIndex - 1, 20].Value == null &&
												(worksheet.Cells[rowIndex - 1, 21].Value == "0" || worksheet.Cells[rowIndex - 1, 21].Value == null) && worksheet.Cells[rowIndex - 1, 22].Value == null)
											{
												rowIndex--;
											}
										}
										if (rowIndex <= rowIndexRoot)
										{
											rowIndex++;
										}
									}
									else
									{
										rowIndex = Math.Max(Math.Max((rowIndexRoot + (data[iloop].ExpenseD.LiftOnList != null ? data[iloop].ExpenseD.LiftOnList.Count(p => p.IsIncludedLiftOn == "1" && p.IsRequestedLiftOn == "1") : 0)),
											(rowIndexRoot + (data[iloop].ExpenseD.LiftOffList != null ? data[iloop].ExpenseD.LiftOffList.Count(p => p.IsIncludedLiftOff == "1" && p.IsRequestedLiftOff == "1") : 0))),
											(rowIndexRoot + (data[iloop].ExpenseD.OtherListLoLo != null ? data[iloop].ExpenseD.OtherListLoLo.Count(p => p.IsIncludedOther == "1" && p.IsRequestedOther == "1") : 0)));
										rowIndex++;
									}
								}
								else
								{
									rowOfGroup = rowOfGroup + 1;
								}
								
								#endregion
							}
							if (lolo == "1")
							{
								worksheet.Cells[12, 1, rowIndex, 22].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
							}
							else
							{
								worksheet.Cells[12, 1, rowIndex, 16].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
							}
							#region Title Sum
							// write total
							worksheet.Cells[transportRow - 1, 16].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[transportRow - 1, 16].Value = totalTransport + totalDetain + (totalLoLo / (1 + (dataList[i].TaxRate / 100)));

							// tax
							worksheet.Cells[transportRow, 16].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[transportRow, 16].Value = (totalTransport + totalDetain + (totalLoLo / (1 + (dataList[i].TaxRate / 100)))) * dataList[i].TaxRate / 100;

							// after tax
							worksheet.Cells[transportTaxRow - 1, 16].Style.Numberformat.Format = "#,##0";
							worksheet.Cells[transportTaxRow - 1, 16].Formula = "SUM(P" + (transportRow - 1) + ":P" + (transportRow) + ")";
							if (lolo == "1")
							{
								worksheet.Cells[expenseRow - 1, 16].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[expenseRow - 1, 16].Value = totalExpense;
								worksheet.Cells[totalRow - 1, 16].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[totalRow - 1, 16].Formula = "SUM(P" + (transportTaxRow - 1) + ":P" + (expenseRow - 1) + ")";
								worksheet.Cells[totalRow - 1, 16].Style.Font.Bold = true;
							}
							else
							{
								worksheet.Cells[totalRow - 1, 16].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[totalRow - 1, 16].Formula = "SUM(P" + (transportTaxRow - 1) + ")";
								worksheet.Cells[totalRow - 1, 16].Style.Font.Bold = true;
							}
							#endregion
						}
						int totalcol1 = lolo == "1" ? 22 : 16;
						int countcellnull1 = 0;
						for (int checkcolnull = 1; checkcolnull <= totalcol1; checkcolnull++)
						{
							countcellnull1 = worksheet.Cells[rowIndex - 1, checkcolnull].Value == null ? countcellnull1 + 1 : countcellnull1;
						}
						if (countcellnull1 == 22 || countcellnull1 == 16)
						{
							rowIndex--;
						}
						// add total end
						worksheet.Cells[rowIndex, 1].Value = (dictionary.ContainsKey("LBLTOTALREPORT")
							? dictionary["LBLTOTALREPORT"]
							: "");
						worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Font.Size = 11;
						worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Font.Bold = true;
						worksheet.Cells[rowIndex, 1, rowIndex, 6].Merge = true;

						worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.BorderAround(ExcelBorderStyle.Thin);
						worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
						worksheet.Cells[rowIndex, 1, rowIndex, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;

						worksheet.Cells[rowIndex, 11].Style.Numberformat.Format = "#,##0";
						worksheet.Cells[rowIndex, 12].Style.Numberformat.Format = "#,##0";
						worksheet.Cells[rowIndex, 14].Style.Numberformat.Format = "#,##0";
						worksheet.Cells[rowIndex, 15].Style.Numberformat.Format = "#,##0";
						worksheet.Cells[rowIndex, 16].Style.Numberformat.Format = "#,##0";
						worksheet.Cells[rowIndex, 17].Style.Numberformat.Format = "#,##0";
						worksheet.Cells[rowIndex, 19].Style.Numberformat.Format = "#,##0";
						worksheet.Cells[rowIndex, 21].Style.Numberformat.Format = "#,##0";

						worksheet.Cells[rowIndex, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[rowIndex, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells[rowIndex, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells[rowIndex, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[rowIndex, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells[rowIndex, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[rowIndex, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

						var sumstr = "SUM(K" + rowSumStart + ":K" + (rowIndex - 1) + ")";
						worksheet.Cells[rowIndex, 11].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

						sumstr = "SUM(L" + rowSumStart + ":L" + (rowIndex - 1) + ")";
						worksheet.Cells[rowIndex, 12].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

						sumstr = "SUM(N" + rowSumStart + ":N" + (rowIndex - 1) + ")";
						worksheet.Cells[rowIndex, 14].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

						sumstr = "SUM(O" + rowSumStart + ":O" + (rowIndex - 1) + ")";
						worksheet.Cells[rowIndex, 15].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

						sumstr = "SUM(P" + rowSumStart + ":P" + (rowIndex - 1) + ")";
						worksheet.Cells[rowIndex, 16].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

						if (lolo == "1")
						{
							sumstr = "SUM(Q" + rowSumStart + ":Q" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 17].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

							sumstr = "SUM(S" + rowSumStart + ":S" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 19].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";

							sumstr = "SUM(U" + rowSumStart + ":U" + (rowIndex - 1) + ")";
							worksheet.Cells[rowIndex, 21].Formula = "IF(" + sumstr + "> 0," + sumstr + ", \"\")";
						}

						// write day
						if (intLanguague == 1)
						{
							worksheet.Cells[rowIndex + 1, 15].Value = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " +
																	  DateTime.Now.Year;
						}
						else if (intLanguague == 2)
						{
							cul = CultureInfo.GetCultureInfo("en-US");
							worksheet.Cells[rowIndex + 1, 15].Value = cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " +
																	  DateTime.Now.Day + ", " + DateTime.Now.Year;
						}
						else
						{
							worksheet.Cells[rowIndex + 1, 15].Value = DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day +
																	  "日";
						}

						// autoFit
						worksheet.Column(1).Width = 3.5;
						worksheet.Column(2).Width = 5;
						worksheet.Column(3).Width = 9;
						worksheet.Column(4).Width = 14;
						worksheet.Column(5).Width = 16;
						worksheet.Column(6).Width = 5;
						worksheet.Column(7).Width = 14;
						worksheet.Column(8).Width = 14;
						worksheet.Column(9).Width = 14;
						worksheet.Column(10).Width = 10;
						worksheet.Column(11).Width = 11;
						worksheet.Column(12).Width = 11;
						worksheet.Column(13).AutoFit();
						worksheet.Column(14).Width = 11;
						worksheet.Column(15).Width = 11;
						worksheet.Column(16).Width = 11;
						if (lolo == "1")
						{
							worksheet.Column(17).Width = 11;
							worksheet.Column(18).Width = 11;
							worksheet.Column(19).Width = 11;
							worksheet.Column(20).Width = 11;
							worksheet.Column(21).Width = 11;
							worksheet.Column(22).AutoFit();
						}
					}
				}

				if (package.Workbook.Worksheets.Count == 0)
				{
					package.Workbook.Worksheets.Add("NoData");
				}

				return new MemoryStream(package.GetAsByteArray());
			}
		}


	}
}