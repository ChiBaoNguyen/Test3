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
using Website.Enum;
using Website.Utilities;
using Website.ViewModels.Expense;

namespace CrystalReport.Service.Expense
{
	public class ExportExcel
	{
		public static Stream ExportExpenseListToExcel(List<ExpenseDetailListReport> data, Dictionary<string, string> dictionary, int intLanguague,
														string companyName, string companyAddress, string fileName, string user, DateTime dateFrom, DateTime dateTo)
		{
			try
			{
				using (ExcelPackage package = new ExcelPackage())
				{
					// add a new worksheet to the empty workbook
					ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ExpenseDetailList");

					//Company name
					worksheet.Cells[1, 1].Value = companyName;
					worksheet.Cells[1, 1].Style.Font.Size = 11;
					worksheet.Cells[1, 1, 1, 4].Merge = true;
					//Company address
					worksheet.Cells[2, 1].Value = companyAddress;
					worksheet.Cells[2, 1].Style.Font.Size = 11;
					worksheet.Cells[2, 1, 2, 4].Merge = true;

					//Add the title
					worksheet.Cells[1, 5].Value = dictionary["TLTEXPENSEREPORT"];
					worksheet.Cells[1, 5].Style.Font.Bold = true;
					worksheet.Cells[1, 5].Style.Font.Size = 16;
					worksheet.Cells[1, 5, 1, 10].Merge = true;
					worksheet.Cells[1, 5, 1, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

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
					worksheet.Cells[2, 5, 2, 10].Merge = true;
                    worksheet.Cells[2, 5, 2, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					// Add the headers
					// column1
					worksheet.Cells[3, 1].Value = "#";
					// column2
					worksheet.Cells[3, 2].Value = dictionary["LBLINVOICEDATE"];
					// column3
					worksheet.Cells[3, 3].Value = dictionary["LBLTRANSPORTDATEDISPATCH"];
					// column4
					worksheet.Cells[3, 4].Value = dictionary["LBLTYPE"];
					// column5
					worksheet.Cells[3, 5].Value = dictionary["LBLCONTSIZERP"];
					// column6
					worksheet.Cells[3, 6].Value = dictionary["LBLCONTNUMBER"];
					// column7
					worksheet.Cells[3, 7].Value = dictionary["TLTROUTE"];
					// column8
					worksheet.Cells[3, 8].Value = dictionary["MNUTRUCK"];
					// column9
					worksheet.Cells[3, 9].Value = dictionary["LBLEMPLOYEE"];
					// column10
					worksheet.Cells[3, 10].Value = dictionary["TLTEXPENSE"];
					// column11
					worksheet.Cells[3, 11].Value = dictionary["LBLTEQUANTITY"];
					// column12
					worksheet.Cells[3, 12].Value = dictionary["LBLUNITPRICE"];
					// column13
					worksheet.Cells[3, 13].Value = dictionary["LBLTOL"];
					// column14
					worksheet.Cells[3, 14].Value = dictionary["LBLTAXAMOUNT"];
					// column15
					worksheet.Cells[3, 15].Value = dictionary["LBLTETOTAL"];
					// column16
					worksheet.Cells[3, 16].Value = dictionary["RPHDCONTENT"];
					// column17
					worksheet.Cells[3, 17].Value = dictionary["LBLSUPPIERREPORT"];
					//set height header
					worksheet.Row(3).Height = 25;
					// set font bold
					worksheet.Cells[3, 1, 3, 17].Style.Font.Bold = true;

					// set border
					worksheet.Cells[3, 1, 3, 17].Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 17].Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 17].Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 17].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

					// align center
					worksheet.Cells[3, 1, 3, 17].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					worksheet.Cells[3, 1, 3, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					// set color
					worksheet.Cells[3, 1, 3, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells[3, 1, 3, 17].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

					//write content
					data = data.OrderBy(d => d.CategoryN).ThenBy(d => d.InvoiceD).ToList();
					var indexRow = 5;
					var count = data.Count;
					double totalQuantity = 0;
					double total = 0;
					double totalTax = 0;
				    double totalAmount = 0;
					if (data != null && data.Count > 0)
					{
						string categoryExpense = data[0].CategoryN;
						int categoryQuantity = 0;
						double categoryTax = 0;
						double categoryTotal = 0;
						int postition = 4;
					    double categoryTotalAmount = 0;
						worksheet.Cells[4, 1].Value = dictionary["LBLCATEGORYC"] + ": " + (String.IsNullOrEmpty(categoryExpense) ? dictionary["LBLOTHER"] : categoryExpense);
						worksheet.Cells[4, 1, 4, 10].Merge = true;

						for (var iloop = 0; iloop < count; iloop++)
						{
							if (categoryExpense != data[iloop].CategoryN)
							{
								worksheet.Cells[postition, 11].Value = categoryQuantity;
								worksheet.Cells[postition, 13].Value = categoryTotal;
								worksheet.Cells[postition, 14].Value = categoryTax;
                                worksheet.Cells[postition, 15].Value = categoryTotalAmount;
								worksheet.Cells[postition, 1, postition, 17].Style.Font.Bold = true;
								worksheet.Cells[postition, 1, postition, 17].Style.Font.Size = 12;
								// set color
								worksheet.Cells[postition, 1, postition, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
								worksheet.Cells[postition, 1, postition, 17].Style.Fill.BackgroundColor.SetColor(Color.SeaShell);

								categoryExpense = data[iloop].CategoryN;
								categoryQuantity = 0;
								categoryTax = 0;
								categoryTotal = 0;
								worksheet.Cells[indexRow + iloop, 1].Value = dictionary["LBLCATEGORYC"] + ": " + categoryExpense;
								worksheet.Cells[indexRow + iloop, 1, indexRow + iloop, 10].Merge = true;
								postition = indexRow + iloop;
								indexRow += 1;
							}
							categoryQuantity += data[iloop].Quantity != null ? (int)data[iloop].Quantity : 0;
							categoryTax += data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0;
							categoryTotal += data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0;
							totalQuantity += data[iloop].Quantity != null ? (double)data[iloop].Quantity : 0;
							total += data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0;
							totalTax += data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0;
						    categoryTotalAmount += (data[iloop].TotalAmount != null ? (double) data[iloop].TotalAmount : 0) +
						                           (data[iloop].TaxAmount != null ? (double) data[iloop].TaxAmount : 0);
                            totalAmount += (data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0) +
                                                   (data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0);
							String temp = data[iloop].Location1N + ", " + data[iloop].Location2N + ", " + data[iloop].Location3N;
							var getdata = temp.Split(new string[] { ", " }, StringSplitOptions.None);
							temp = "";
							if (getdata[0] != "")
							{
								temp = getdata[0];
								if (getdata[1] != "")
								{
									temp = temp + ", " + getdata[1];
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}
								}
								else
								{
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}

								}
							}
							else
							{
								if (getdata[1] != "")
								{
									temp = getdata[1];
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}
								}
								else
								{
									if (getdata[2] != "")
									{
										temp = getdata[2];
									}
								}
							}
							worksheet.Cells[indexRow + iloop, 1].Value = iloop + 1;
							worksheet.Cells[indexRow + iloop, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[indexRow + iloop, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[indexRow + iloop, 2].Value = data[iloop].InvoiceD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].InvoiceD, intLanguague) : "";
							worksheet.Cells[indexRow + iloop, 3].Value = data[iloop].TransportD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].TransportD, intLanguague) : "";
							worksheet.Cells[indexRow + iloop, 4].Value = Utilities.GetOrderTypeName(data[iloop].OrderTypeI);
							worksheet.Cells[indexRow + iloop, 5].Value = (data[iloop].ContainerSizeI == "3" ? dictionary["LBLLOAD"] + " (" + (data[iloop].NetWeight ?? 0).ToString("#,###.#") + ")" : Utilities.GetContainerSizeName(data[iloop].ContainerSizeI));
							worksheet.Cells[indexRow + iloop, 5].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 6].Value = data[iloop].ContainerNo;
							worksheet.Cells[indexRow + iloop, 6].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 7].Value = temp;
							worksheet.Cells[indexRow + iloop, 7].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 8].Value = data[iloop].RegisteredNo;
							worksheet.Cells[indexRow + iloop, 8].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 9].Value = data[iloop].DriverN;
							worksheet.Cells[indexRow + iloop, 9].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 10].Value = data[iloop].ExpenseN;
							worksheet.Cells[indexRow + iloop, 10].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 11].Value = data[iloop].Quantity;
							worksheet.Cells[indexRow + iloop, 12].Value = data[iloop].UnitPrice;
							worksheet.Cells[indexRow + iloop, 13].Value = data[iloop].TotalAmount;
							worksheet.Cells[indexRow + iloop, 14].Value = data[iloop].TaxAmount;
							worksheet.Cells[indexRow + iloop, 15].Value = (data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0) + (data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0);
							worksheet.Cells[indexRow + iloop, 16].Value = data[iloop].Description;
							worksheet.Cells[indexRow + iloop, 17].Value = data[iloop].SupplierN;
							worksheet.Cells[indexRow + iloop, 17].Style.WrapText = true;
						}
						worksheet.Cells[postition, 11].Value = categoryQuantity;
						worksheet.Cells[postition, 13].Value = categoryTotal;
						worksheet.Cells[postition, 14].Value = categoryTax;
                        worksheet.Cells[postition, 15].Value = categoryTotalAmount;
						worksheet.Cells[postition, 1, postition, 17].Style.Font.Bold = true;
						worksheet.Cells[postition, 1, postition, 17].Style.Font.Size = 12;
						// set color
						worksheet.Cells[postition, 1, postition, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells[postition, 1, postition, 17].Style.Fill.BackgroundColor.SetColor(Color.SeaShell);
					}

					worksheet.Cells[indexRow + count, 1].Value = dictionary["RPLBLTOTAL"];
					worksheet.Cells[indexRow + count, 1].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 1].Style.Font.Size = 13;
					worksheet.Cells[indexRow + count, 1, indexRow + count, 10].Merge = true;
					//Value  sum 
					worksheet.Cells[indexRow + count, 11].Value = totalQuantity;
					worksheet.Cells[indexRow + count, 11].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 11].Style.Font.Size = 13;
					//Value  sum 
					worksheet.Cells[indexRow + count, 13].Value = total;
					worksheet.Cells[indexRow + count, 13].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 13].Style.Font.Size = 13;
					//Value sum Tax
					worksheet.Cells[indexRow + count, 14].Value = totalTax;
					worksheet.Cells[indexRow + count, 14].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 14].Style.Font.Size = 13;
                    //Value sum toltal amount
                    worksheet.Cells[indexRow + count, 15].Value = totalAmount;
                    worksheet.Cells[indexRow + count, 15].Style.Font.Bold = true;
                    worksheet.Cells[indexRow + count, 15].Style.Font.Size = 13;
					//format currency
					worksheet.Cells[4, 9, indexRow + count, 17].Style.Numberformat.Format = "#,###";

					// set border
					worksheet.Cells[3, 1, indexRow + count, 17].Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 17].Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 17].Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 17].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

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
					worksheet.Cells[indexRow + count + 1, 11, indexRow + count + 1, 14].Merge = true;
					worksheet.Cells[indexRow + count + 1, 11, indexRow + count + 1, 14].Style.HorizontalAlignment =ExcelHorizontalAlignment.Center;
					worksheet.Cells[indexRow + count + 2, 11].Value = dictionary["RPFTCREATOR"];
					worksheet.Cells[indexRow + count + 2, 11, indexRow + count + 2, 14].Merge = true;
					worksheet.Cells[indexRow + count + 2, 11, indexRow + count + 2, 14].Style.HorizontalAlignment =ExcelHorizontalAlignment.Center;

					worksheet.Cells[indexRow + count + 5, 11].Value = dictionary["RPFTPRINTBY"] + ": " + user + ", " + dictionary["RPFTPRINTTIME"] + ": " + DateTime.Now.ToString("HH:mm");
					worksheet.Cells[indexRow + count + 5, 11, indexRow + count + 5, 14].Merge = true;
					worksheet.Cells[indexRow + count + 5, 11, indexRow + count + 5, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					// autoFit
					// autoFit
					worksheet.Column(1).Width = 5;
					worksheet.Column(2).Width = 10;
					worksheet.Column(3).Width = 10;
					worksheet.Column(4).Width = 5;
					worksheet.Column(5).Width = 8;
					worksheet.Column(6).Width = 12;
					worksheet.Column(7).Width = 20;
					worksheet.Column(8).Width = 11;
					worksheet.Column(9).Width = 15;
					worksheet.Column(10).Width = 15;
					worksheet.Column(11).Width = 8;
					worksheet.Column(12).Width = 9;
					worksheet.Column(13).Width = 12;
					worksheet.Column(14).Width = 12;
					worksheet.Column(15).Width = 12;
					worksheet.Column(16).Width = 14;
					worksheet.Column(17).Width = 14;

					return new MemoryStream(package.GetAsByteArray());
				}
			}
			catch (NullReferenceException)
			{
				throw new NullReferenceException();
			}
		}

		public static Stream ExportExpenseDriverListToExcel(List<ExpenseDetailListReport> data, Dictionary<string, string> dictionary, int intLanguague,
														string companyName, string companyAddress, string fileName, string user, DateTime dateFrom, DateTime dateTo)
		{
			try
			{
				using (ExcelPackage package = new ExcelPackage())
				{
					// add a new worksheet to the empty workbook
					ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ExpenseDetailList");

					//Company name
					worksheet.Cells[1, 1].Value = companyName;
					worksheet.Cells[1, 1].Style.Font.Size = 11;
					worksheet.Cells[1, 1, 1, 4].Merge = true;
					//Company address
					worksheet.Cells[2, 1].Value = companyAddress;
					worksheet.Cells[2, 1].Style.Font.Size = 11;
					worksheet.Cells[2, 1, 2, 4].Merge = true;

					//Add the title
					worksheet.Cells[1, 5].Value = dictionary["TLTEXPENSEREPORT"];
					worksheet.Cells[1, 5].Style.Font.Bold = true;
					worksheet.Cells[1, 5].Style.Font.Size = 16;
					worksheet.Cells[1, 5, 1, 10].Merge = true;
					worksheet.Cells[1, 5, 1, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

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
					worksheet.Cells[2, 5, 2, 10].Merge = true;
					worksheet.Cells[2, 5, 2, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					// Add the headers
					// column1
					worksheet.Cells[3, 1].Value = "#";
					// column2
					worksheet.Cells[3, 2].Value = dictionary["LBLINVOICEDATE"];
					// column3
					worksheet.Cells[3, 3].Value = dictionary["LBLTRANSPORTDATEDISPATCH"];
					// column4
					worksheet.Cells[3, 4].Value = dictionary["TLTROUTE"];
					// column5
					worksheet.Cells[3, 5].Value = dictionary["MNUTRUCK"] + "/" + dictionary["LBLTRAILER"];
					// column7
					worksheet.Cells[3, 6].Value = dictionary["TLTEXPENSE"];
					// column8
					worksheet.Cells[3, 7].Value = dictionary["LBLTEQUANTITY"];
					// column9
					worksheet.Cells[3, 8].Value = dictionary["LBLUNITPRICE"];
					// column10
					worksheet.Cells[3, 9].Value = dictionary["LBLTOL"];
					// column11
					worksheet.Cells[3, 10].Value = dictionary["LBLTAXAMOUNT"];
					// column12
					worksheet.Cells[3, 11].Value = dictionary["LBLTETOTAL"];
					// column13
					worksheet.Cells[3, 12].Value = dictionary["RPHDCONTENT"];
					// column14
					worksheet.Cells[3, 13].Value = dictionary["LBLSUPPIERREPORT"];
					//set height header
					worksheet.Row(3).Height = 25;
					// set font bold
					worksheet.Cells[3, 1, 3, 13].Style.Font.Bold = true;

					// set border
					worksheet.Cells[3, 1, 3, 13].Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 13].Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 13].Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 13].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

					// align center
					worksheet.Cells[3, 1, 3, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					worksheet.Cells[3, 1, 3, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					// set color
					worksheet.Cells[3, 1, 3, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells[3, 1, 3, 13].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

					//write content
					data = data.OrderBy(p => p.DriverN).ThenBy(d => d.InvoiceD).ToList();
					var indexRow = 5;
					var count = data.Count;
					double total = 0;
					double totalQuantity = 0;
					double totalTax = 0;
					double totalAmount = 0;
					if (data != null && data.Count > 0)
					{
						string categoryExpense = data[0].DriverN;
						int categoryQuantity = 0;
						double categoryTax = 0;
						double categoryTotal = 0;
						int postition = 4;
						double categoryTotalAmount = 0;
						worksheet.Cells[4, 1].Value = (String.IsNullOrEmpty(categoryExpense) ? "" : categoryExpense);
						worksheet.Cells[4, 1, 4, 6].Merge = true;

						for (var iloop = 0; iloop < count; iloop++)
						{
							if (categoryExpense != data[iloop].DriverN)
							{
								worksheet.Cells[postition, 7].Value = categoryQuantity;
								worksheet.Cells[postition, 9].Value = categoryTotal;
								worksheet.Cells[postition, 10].Value = categoryTax;
								worksheet.Cells[postition, 11].Value = categoryTotalAmount;
								worksheet.Cells[postition, 1, postition, 13].Style.Font.Bold = true;
								worksheet.Cells[postition, 1, postition, 13].Style.Font.Size = 12;
								// set color
								worksheet.Cells[postition, 1, postition, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
								worksheet.Cells[postition, 1, postition, 13].Style.Fill.BackgroundColor.SetColor(Color.SeaShell);

								categoryExpense = data[iloop].DriverN;
								categoryQuantity = 0;
								categoryTax = 0;
								categoryTotal = 0;
								worksheet.Cells[indexRow + iloop, 1].Value = categoryExpense;
								worksheet.Cells[indexRow + iloop, 1, indexRow + iloop, 6].Merge = true;
								postition = indexRow + iloop;
								indexRow += 1;
							}
							categoryQuantity += data[iloop].Quantity != null ? (int)data[iloop].Quantity : 0;
							categoryTax += data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0;
							categoryTotal += data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0;
							totalQuantity += data[iloop].Quantity != null ? (double)data[iloop].Quantity : 0;
							total += data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0;
							totalTax += data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0;
							categoryTotalAmount += (data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0) +
												   (data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0);
							totalAmount += (data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0) +
												   (data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0);
							String temp = data[iloop].Location1N + ", " + data[iloop].Location2N + ", " + data[iloop].Location3N;
							var getdata = temp.Split(new string[] { ", " }, StringSplitOptions.None);
							temp = "";
							if (getdata[0] != "")
							{
								temp = getdata[0];
								if (getdata[1] != "")
								{
									temp = temp + ", " + getdata[1];
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}
								}
								else
								{
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}

								}
							}
							else
							{
								if (getdata[1] != "")
								{
									temp = getdata[1];
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}
								}
								else
								{
									if (getdata[2] != "")
									{
										temp = getdata[2];
									}
								}
							}
							string registerednotrailer = (!String.IsNullOrEmpty(data[iloop].RegisteredNo) ? (data[iloop].RegisteredNo + (!String.IsNullOrEmpty(data[iloop].TrailerNo) ? " \n " : "")) : "") + data[iloop].TrailerNo;

							worksheet.Cells[indexRow + iloop, 1].Value = iloop + 1;
							worksheet.Cells[indexRow + iloop, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[indexRow + iloop, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[indexRow + iloop, 2].Value = data[iloop].InvoiceD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].InvoiceD, intLanguague) : "";
							worksheet.Cells[indexRow + iloop, 3].Value = data[iloop].TransportD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].TransportD, intLanguague) : "";
							worksheet.Cells[indexRow + iloop, 4].Value = temp;
							worksheet.Cells[indexRow + iloop, 4].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 5].Value = registerednotrailer;
							worksheet.Cells[indexRow + iloop, 5].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 6].Value = data[iloop].ExpenseN;
							worksheet.Cells[indexRow + iloop, 6].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 7].Value = data[iloop].Quantity;
							worksheet.Cells[indexRow + iloop, 8].Value = data[iloop].UnitPrice;
							worksheet.Cells[indexRow + iloop, 9].Value = data[iloop].TotalAmount;
							worksheet.Cells[indexRow + iloop, 10].Value = data[iloop].TaxAmount;
							worksheet.Cells[indexRow + iloop, 11].Value = (data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0) + (data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0);
							worksheet.Cells[indexRow + iloop, 12].Value = data[iloop].Description;
							worksheet.Cells[indexRow + iloop, 13].Value = data[iloop].SupplierN;
							worksheet.Cells[indexRow + iloop, 13].Style.WrapText = true;
						}
						worksheet.Cells[postition, 7].Value = categoryQuantity;
						worksheet.Cells[postition, 9].Value = categoryTotal;
						worksheet.Cells[postition, 10].Value = categoryTax;
						worksheet.Cells[postition, 11].Value = categoryTotalAmount;
						worksheet.Cells[postition, 1, postition, 13].Style.Font.Bold = true;
						worksheet.Cells[postition, 1, postition, 13].Style.Font.Size = 12;
						// set color
						worksheet.Cells[postition, 1, postition, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells[postition, 1, postition, 13].Style.Fill.BackgroundColor.SetColor(Color.SeaShell);
					}

					worksheet.Cells[indexRow + count, 1].Value = dictionary["RPLBLTOTAL"];
					worksheet.Cells[indexRow + count, 1].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 1].Style.Font.Size = 13;
					worksheet.Cells[indexRow + count, 1, indexRow + count, 6].Merge = true;
					//Value  sum 
					worksheet.Cells[indexRow + count, 7].Value = totalQuantity;
					worksheet.Cells[indexRow + count, 7].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 7].Style.Font.Size = 13;
					//Value  sum 
					worksheet.Cells[indexRow + count, 9].Value = total;
					worksheet.Cells[indexRow + count, 9].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 9].Style.Font.Size = 13;
					//Value sum Tax
					worksheet.Cells[indexRow + count, 10].Value = totalTax;
					worksheet.Cells[indexRow + count, 10].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 10].Style.Font.Size = 13;
					//Value sum toltal amount
					worksheet.Cells[indexRow + count, 11].Value = totalAmount;
					worksheet.Cells[indexRow + count, 11].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 11].Style.Font.Size = 13;
					//format currency
					worksheet.Cells[4, 8, indexRow + count, 11].Style.Numberformat.Format = "#,###";

					// set border
					worksheet.Cells[3, 1, indexRow + count, 13].Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 13].Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 13].Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 13].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

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
					worksheet.Cells[indexRow + count + 1, 11, indexRow + count + 1, 14].Merge = true;
					worksheet.Cells[indexRow + count + 1, 11, indexRow + count + 1, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					worksheet.Cells[indexRow + count + 2, 11].Value = dictionary["RPFTCREATOR"];
					worksheet.Cells[indexRow + count + 2, 11, indexRow + count + 2, 14].Merge = true;
					worksheet.Cells[indexRow + count + 2, 11, indexRow + count + 2, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					worksheet.Cells[indexRow + count + 5, 11].Value = dictionary["RPFTPRINTBY"] + ": " + user + ", " + dictionary["RPFTPRINTTIME"] + ": " + DateTime.Now.ToString("HH:mm");
					worksheet.Cells[indexRow + count + 5, 11, indexRow + count + 5, 14].Merge = true;
					worksheet.Cells[indexRow + count + 5, 11, indexRow + count + 5, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					// autoFit
					worksheet.Column(1).Width = 5;
					worksheet.Column(2).Width = 10;
					worksheet.Column(3).Width = 10;
					worksheet.Column(4).Width = 20;
					worksheet.Column(5).Width = 15;
					worksheet.Column(6).Width = 15;
					worksheet.Column(7).Width = 8;
					worksheet.Column(8).Width = 9;
					worksheet.Column(9).Width = 12;
					worksheet.Column(10).Width = 12;
					worksheet.Column(11).Width = 12;
					worksheet.Column(12).Width = 14;
					worksheet.Column(13).Width = 14;

					return new MemoryStream(package.GetAsByteArray());
				}
			}
			catch (NullReferenceException)
			{
				throw new NullReferenceException();
			}
		}

		public static Stream ExportExpenseEmployeeListToExcel(List<ExpenseDetailListReport> data, Dictionary<string, string> dictionary, int intLanguague,
														string companyName, string companyAddress, string fileName, string user, DateTime dateFrom, DateTime dateTo)
		{
			try
			{
				using (ExcelPackage package = new ExcelPackage())
				{
					// add a new worksheet to the empty workbook
					ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ExpenseDetailList");

					//Company name
					worksheet.Cells[1, 1].Value = companyName;
					worksheet.Cells[1, 1].Style.Font.Size = 11;
					worksheet.Cells[1, 1, 1, 4].Merge = true;
					//Company address
					worksheet.Cells[2, 1].Value = companyAddress;
					worksheet.Cells[2, 1].Style.Font.Size = 11;
					worksheet.Cells[2, 1, 2, 4].Merge = true;

					//Add the title
					worksheet.Cells[1, 5].Value = dictionary["TLTEXPENSEREPORT"];
					worksheet.Cells[1, 5].Style.Font.Bold = true;
					worksheet.Cells[1, 5].Style.Font.Size = 16;
					worksheet.Cells[1, 5, 1, 10].Merge = true;
					worksheet.Cells[1, 5, 1, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

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
					worksheet.Cells[2, 5, 2, 10].Merge = true;
					worksheet.Cells[2, 5, 2, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					// Add the headers
					// column1
					worksheet.Cells[3, 1].Value = "#";
					// column2
					worksheet.Cells[3, 2].Value = dictionary["LBLINVOICEDATE"];
					// column3
					worksheet.Cells[3, 3].Value = dictionary["TLTEXPENSE"];
					// column4
					worksheet.Cells[3, 4].Value = dictionary["LBLTEQUANTITY"];
					// column5
					worksheet.Cells[3, 5].Value = dictionary["LBLUNITPRICE"];
					// column6
					worksheet.Cells[3, 6].Value = dictionary["LBLTOL"];
					// column7
					worksheet.Cells[3, 7].Value = dictionary["LBLTAXAMOUNT"];
					// column8
					worksheet.Cells[3, 8].Value = dictionary["LBLTETOTAL"];
					// column9
					worksheet.Cells[3, 9].Value = dictionary["RPHDCONTENT"];
					// column10
					worksheet.Cells[3, 10].Value = dictionary["LBLSUPPIERREPORT"];
					//set height header
					worksheet.Row(3).Height = 25;
					// set font bold
					worksheet.Cells[3, 1, 3, 10].Style.Font.Bold = true;

					// set border
					worksheet.Cells[3, 1, 3, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

					// align center
					worksheet.Cells[3, 1, 3, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					worksheet.Cells[3, 1, 3, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					// set color
					worksheet.Cells[3, 1, 3, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells[3, 1, 3, 10].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

					//write content
					data = data.OrderBy(p => p.EntryClerkN).ThenBy(d => d.InvoiceD).ToList();
					var indexRow = 5;
					var count = data.Count;
					double total = 0;
					double totalQuantity = 0;
					double totalTax = 0;
					double totalAmount = 0;
					if (data != null && data.Count > 0)
					{
						string categoryExpense = data[0].EntryClerkN;
						int categoryQuantity = 0;
						double categoryTax = 0;
						double categoryTotal = 0;
						int postition = 4;
						double categoryTotalAmount = 0;
						worksheet.Cells[4, 1].Value = (String.IsNullOrEmpty(categoryExpense) ? "" : categoryExpense);
						worksheet.Cells[4, 1, 4, 3].Merge = true;

						for (var iloop = 0; iloop < count; iloop++)
						{
							if (categoryExpense != data[iloop].EntryClerkN)
							{
								worksheet.Cells[postition, 4].Value = categoryQuantity;
								worksheet.Cells[postition, 6].Value = categoryTotal;
								worksheet.Cells[postition, 7].Value = categoryTax;
								worksheet.Cells[postition, 8].Value = categoryTotalAmount;
								worksheet.Cells[postition, 1, postition, 10].Style.Font.Bold = true;
								worksheet.Cells[postition, 1, postition, 10].Style.Font.Size = 12;
								// set color
								worksheet.Cells[postition, 1, postition, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
								worksheet.Cells[postition, 1, postition, 10].Style.Fill.BackgroundColor.SetColor(Color.SeaShell);

								categoryExpense = data[iloop].EntryClerkN;
								categoryQuantity = 0;
								categoryTax = 0;
								categoryTotal = 0;
								worksheet.Cells[indexRow + iloop, 1].Value = categoryExpense;
								worksheet.Cells[indexRow + iloop, 1, indexRow + iloop, 3].Merge = true;
								postition = indexRow + iloop;
								indexRow += 1;
							}
							categoryQuantity += data[iloop].Quantity != null ? (int)data[iloop].Quantity : 0;
							categoryTax += data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0;
							categoryTotal += data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0;
							totalQuantity += data[iloop].Quantity != null ? (double)data[iloop].Quantity : 0;
							total += data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0;
							totalTax += data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0;
							categoryTotalAmount += (data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0) +
												   (data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0);
							totalAmount += (data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0) +
												   (data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0);
							String temp = data[iloop].Location1N + ", " + data[iloop].Location2N + ", " + data[iloop].Location3N;
							var getdata = temp.Split(new string[] { ", " }, StringSplitOptions.None);
							temp = "";
							if (getdata[0] != "")
							{
								temp = getdata[0];
								if (getdata[1] != "")
								{
									temp = temp + ", " + getdata[1];
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}
								}
								else
								{
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}

								}
							}
							else
							{
								if (getdata[1] != "")
								{
									temp = getdata[1];
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}
								}
								else
								{
									if (getdata[2] != "")
									{
										temp = getdata[2];
									}
								}
							}
							worksheet.Cells[indexRow + iloop, 1].Value = iloop + 1;
							worksheet.Cells[indexRow + iloop, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[indexRow + iloop, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[indexRow + iloop, 2].Value = data[iloop].InvoiceD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].InvoiceD, intLanguague) : "";
							worksheet.Cells[indexRow + iloop, 3].Value = data[iloop].ExpenseN;
							worksheet.Cells[indexRow + iloop, 3].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 4].Value = data[iloop].Quantity;
							worksheet.Cells[indexRow + iloop, 5].Value = data[iloop].UnitPrice;
							worksheet.Cells[indexRow + iloop, 6].Value = data[iloop].TotalAmount;
							worksheet.Cells[indexRow + iloop, 7].Value = data[iloop].TaxAmount;
							worksheet.Cells[indexRow + iloop, 8].Value = (data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0) + (data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0);
							worksheet.Cells[indexRow + iloop, 9].Value = data[iloop].Description;
							worksheet.Cells[indexRow + iloop, 10].Value = data[iloop].SupplierN;
							worksheet.Cells[indexRow + iloop, 10].Style.WrapText = true;
						}
						worksheet.Cells[postition, 4].Value = categoryQuantity;
						worksheet.Cells[postition, 6].Value = categoryTotal;
						worksheet.Cells[postition, 7].Value = categoryTax;
						worksheet.Cells[postition, 8].Value = categoryTotalAmount;
						worksheet.Cells[postition, 1, postition, 10].Style.Font.Bold = true;
						worksheet.Cells[postition, 1, postition, 10].Style.Font.Size = 12;
						// set color
						worksheet.Cells[postition, 1, postition, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells[postition, 1, postition, 10].Style.Fill.BackgroundColor.SetColor(Color.SeaShell);
					}

					worksheet.Cells[indexRow + count, 1].Value = dictionary["RPLBLTOTAL"];
					worksheet.Cells[indexRow + count, 1].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 1].Style.Font.Size = 13;
					worksheet.Cells[indexRow + count, 1, indexRow + count, 6].Merge = true;
					//Value  sum 
					worksheet.Cells[indexRow + count, 4].Value = totalQuantity;
					worksheet.Cells[indexRow + count, 4].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 4].Style.Font.Size = 13;
					//Value  sum 
					worksheet.Cells[indexRow + count, 6].Value = total;
					worksheet.Cells[indexRow + count, 6].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 6].Style.Font.Size = 13;
					//Value sum Tax
					worksheet.Cells[indexRow + count, 7].Value = totalTax;
					worksheet.Cells[indexRow + count, 7].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 7].Style.Font.Size = 13;
					//Value sum toltal amount
					worksheet.Cells[indexRow + count, 8].Value = totalAmount;
					worksheet.Cells[indexRow + count, 8].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 8].Style.Font.Size = 13;
					//format currency
					worksheet.Cells[4, 4, indexRow + count, 8].Style.Numberformat.Format = "#,###";

					// set border
					worksheet.Cells[3, 1, indexRow + count, 10].Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 10].Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 10].Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 10].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

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
					worksheet.Cells[indexRow + count + 1, 11, indexRow + count + 1, 14].Merge = true;
					worksheet.Cells[indexRow + count + 1, 11, indexRow + count + 1, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					worksheet.Cells[indexRow + count + 2, 11].Value = dictionary["RPFTCREATOR"];
					worksheet.Cells[indexRow + count + 2, 11, indexRow + count + 2, 14].Merge = true;
					worksheet.Cells[indexRow + count + 2, 11, indexRow + count + 2, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					worksheet.Cells[indexRow + count + 5, 11].Value = dictionary["RPFTPRINTBY"] + ": " + user + ", " + dictionary["RPFTPRINTTIME"] + ": " + DateTime.Now.ToString("HH:mm");
					worksheet.Cells[indexRow + count + 5, 11, indexRow + count + 5, 14].Merge = true;
					worksheet.Cells[indexRow + count + 5, 11, indexRow + count + 5, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					// autoFit
					worksheet.Column(1).Width = 5;
					worksheet.Column(2).Width = 10;
					worksheet.Column(3).Width = 15;
					worksheet.Column(4).Width = 8;
					worksheet.Column(5).Width = 9;
					worksheet.Column(6).Width = 12;
					worksheet.Column(7).Width = 12;
					worksheet.Column(8).Width = 12;
					worksheet.Column(9).Width = 14;
					worksheet.Column(10).Width = 14;

					return new MemoryStream(package.GetAsByteArray());
				}
			}
			catch (NullReferenceException)
			{
				throw new NullReferenceException();
			}
		}
		public static Stream ExportExpenseTrailerListToExcel(List<ExpenseDetailListReport> data, Dictionary<string, string> dictionary, int intLanguague,
														string companyName, string companyAddress, string fileName, string user, DateTime dateFrom, DateTime dateTo)
		{
			try
			{
				using (ExcelPackage package = new ExcelPackage())
				{
					// add a new worksheet to the empty workbook
					ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ExpenseDetailList");

					//Company name
					worksheet.Cells[1, 1].Value = companyName;
					worksheet.Cells[1, 1].Style.Font.Size = 11;
					worksheet.Cells[1, 1, 1, 4].Merge = true;
					//Company address
					worksheet.Cells[2, 1].Value = companyAddress;
					worksheet.Cells[2, 1].Style.Font.Size = 11;
					worksheet.Cells[2, 1, 2, 4].Merge = true;

					//Add the title
					worksheet.Cells[1, 5].Value = dictionary["TLTEXPENSEREPORT"];
					worksheet.Cells[1, 5].Style.Font.Bold = true;
					worksheet.Cells[1, 5].Style.Font.Size = 16;
					worksheet.Cells[1, 5, 1, 10].Merge = true;
					worksheet.Cells[1, 5, 1, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

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
					worksheet.Cells[2, 5, 2, 10].Merge = true;
					worksheet.Cells[2, 5, 2, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					// Add the headers
					// column1
					worksheet.Cells[3, 1].Value = "#";
					// column2
					worksheet.Cells[3, 2].Value = dictionary["LBLINVOICEDATE"];
					// column3
					worksheet.Cells[3, 3].Value = dictionary["LBLTRANSPORTDATEDISPATCH"];
					// column4
					worksheet.Cells[3, 4].Value = dictionary["LBLTYPE"];
					// column5
					worksheet.Cells[3, 5].Value = dictionary["LBLCONTSIZERP"];
					// column6
					worksheet.Cells[3, 6].Value = dictionary["LBLCONTNUMBER"];
					// column7
					worksheet.Cells[3, 7].Value = dictionary["TLTROUTE"];
					// column8
					worksheet.Cells[3, 8].Value = dictionary["MNUTRUCK"];
					// column9
					worksheet.Cells[3, 9].Value = dictionary["LBLEMPLOYEE"];
					// column10
					worksheet.Cells[3, 10].Value = dictionary["TLTEXPENSE"];

					// column11
					worksheet.Cells[3, 11].Value = dictionary["LBLTEQUANTITY"];
					// column12
					worksheet.Cells[3, 12].Value = dictionary["LBLUNITPRICE"];
					// column13
					worksheet.Cells[3, 13].Value = dictionary["LBLTOL"];
					// column14
					worksheet.Cells[3, 14].Value = dictionary["LBLTAXAMOUNT"];
					// column15
					worksheet.Cells[3, 15].Value = dictionary["LBLTETOTAL"];
					// column16
					worksheet.Cells[3, 16].Value = dictionary["RPHDCONTENT"];
					// column17
					worksheet.Cells[3, 17].Value = dictionary["LBLSUPPIERREPORT"];
					//set height header
					worksheet.Row(3).Height = 25;
					// set font bold
					worksheet.Cells[3, 1, 3, 17].Style.Font.Bold = true;

					// set border
					worksheet.Cells[3, 1, 3, 17].Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 17].Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 17].Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 17].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

					// align center
					worksheet.Cells[3, 1, 3, 17].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					worksheet.Cells[3, 1, 3, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					// set color
					worksheet.Cells[3, 1, 3, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells[3, 1, 3, 17].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

					//write content
					data = data.OrderBy(p => p.TrailerNo).ThenBy(d => d.InvoiceD).ToList();
					var indexRow = 5;
					var count = data.Count;
					double total = 0;
					double totalQuantity = 0;
					double totalTax = 0;
					double totalAmount = 0;
					if (data != null && data.Count > 0)
					{
						string categoryExpense = data[0].TrailerNo;
						int categoryQuantity = 0;
						double categoryTax = 0;
						double categoryTotal = 0;
						int postition = 4;
						double categoryTotalAmount = 0;
						worksheet.Cells[4, 1].Value = (String.IsNullOrEmpty(categoryExpense) ? "" : categoryExpense);
						worksheet.Cells[4, 1, 4, 10].Merge = true;

						for (var iloop = 0; iloop < count; iloop++)
						{
							if (categoryExpense != data[iloop].TrailerNo)
							{
								worksheet.Cells[postition, 11].Value = categoryQuantity;
								worksheet.Cells[postition, 13].Value = categoryTotal;
								worksheet.Cells[postition, 14].Value = categoryTax;
								worksheet.Cells[postition, 15].Value = categoryTotalAmount;
								worksheet.Cells[postition, 1, postition, 17].Style.Font.Bold = true;
								worksheet.Cells[postition, 1, postition, 17].Style.Font.Size = 12;
								// set color
								worksheet.Cells[postition, 1, postition, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
								worksheet.Cells[postition, 1, postition, 17].Style.Fill.BackgroundColor.SetColor(Color.SeaShell);

								categoryExpense = data[iloop].TrailerNo;
								categoryQuantity = 0;
								categoryTax = 0;
								categoryTotal = 0;
								worksheet.Cells[indexRow + iloop, 1].Value = categoryExpense;
								worksheet.Cells[indexRow + iloop, 1, indexRow + iloop, 10].Merge = true;
								postition = indexRow + iloop;
								indexRow += 1;
							}
							categoryQuantity += data[iloop].Quantity != null ? (int)data[iloop].Quantity : 0;
							categoryTax += data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0;
							categoryTotal += data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0;
							totalQuantity += data[iloop].Quantity != null ? (double)data[iloop].Quantity : 0;
							total += data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0;
							totalTax += data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0;
							categoryTotalAmount += (data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0) +
												   (data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0);
							totalAmount += (data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0) +
												   (data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0);
							String temp = data[iloop].Location1N + ", " + data[iloop].Location2N + ", " + data[iloop].Location3N;
							var getdata = temp.Split(new string[] { ", " }, StringSplitOptions.None);
							temp = "";
							if (getdata[0] != "")
							{
								temp = getdata[0];
								if (getdata[1] != "")
								{
									temp = temp + ", " + getdata[1];
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}
								}
								else
								{
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}

								}
							}
							else
							{
								if (getdata[1] != "")
								{
									temp = getdata[1];
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}
								}
								else
								{
									if (getdata[2] != "")
									{
										temp = getdata[2];
									}
								}
							}
							worksheet.Cells[indexRow + iloop, 1].Value = iloop + 1;
							worksheet.Cells[indexRow + iloop, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[indexRow + iloop, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[indexRow + iloop, 2].Value = data[iloop].InvoiceD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].InvoiceD, intLanguague) : "";
							worksheet.Cells[indexRow + iloop, 3].Value = data[iloop].TransportD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].TransportD, intLanguague) : "";
							worksheet.Cells[indexRow + iloop, 4].Value = Utilities.GetOrderTypeName(data[iloop].OrderTypeI);
							worksheet.Cells[indexRow + iloop, 5].Value = (data[iloop].ContainerSizeI == "3" ? dictionary["LBLLOAD"] + " (" + (data[iloop].NetWeight ?? 0).ToString("#,###.#") + ")" : Utilities.GetContainerSizeName(data[iloop].ContainerSizeI));
							worksheet.Cells[indexRow + iloop, 5].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 6].Value = data[iloop].ContainerNo;
							worksheet.Cells[indexRow + iloop, 6].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 7].Value = temp;
							worksheet.Cells[indexRow + iloop, 7].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 8].Value = data[iloop].RegisteredNo;
							worksheet.Cells[indexRow + iloop, 9].Value = data[iloop].DriverN;
							worksheet.Cells[indexRow + iloop, 9].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 10].Value = data[iloop].ExpenseN;
							worksheet.Cells[indexRow + iloop, 10].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 11].Value = data[iloop].Quantity;
							worksheet.Cells[indexRow + iloop, 12].Value = data[iloop].UnitPrice;
							worksheet.Cells[indexRow + iloop, 13].Value = data[iloop].TotalAmount;
							worksheet.Cells[indexRow + iloop, 14].Value = data[iloop].TaxAmount;
							worksheet.Cells[indexRow + iloop, 15].Value = (data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0) + (data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0);
							worksheet.Cells[indexRow + iloop, 16].Value = data[iloop].Description;
							worksheet.Cells[indexRow + iloop, 17].Value = data[iloop].SupplierN;
							worksheet.Cells[indexRow + iloop, 17].Style.WrapText = true;
						}
						worksheet.Cells[postition, 11].Value = categoryQuantity;
						worksheet.Cells[postition, 13].Value = categoryTotal;
						worksheet.Cells[postition, 14].Value = categoryTax;
						worksheet.Cells[postition, 15].Value = categoryTotalAmount;
						worksheet.Cells[postition, 1, postition, 17].Style.Font.Bold = true;
						worksheet.Cells[postition, 1, postition, 17].Style.Font.Size = 12;
						// set color
						worksheet.Cells[postition, 1, postition, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells[postition, 1, postition, 17].Style.Fill.BackgroundColor.SetColor(Color.SeaShell);
					}

					worksheet.Cells[indexRow + count, 1].Value = dictionary["RPLBLTOTAL"];
					worksheet.Cells[indexRow + count, 1].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 1].Style.Font.Size = 13;
					worksheet.Cells[indexRow + count, 1, indexRow + count, 10].Merge = true;
					//Value  sum 
					worksheet.Cells[indexRow + count, 11].Value = totalQuantity;
					worksheet.Cells[indexRow + count, 11].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 11].Style.Font.Size = 13;
					//Value  sum 
					worksheet.Cells[indexRow + count, 13].Value = total;
					worksheet.Cells[indexRow + count, 13].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 13].Style.Font.Size = 13;
					//Value sum Tax
					worksheet.Cells[indexRow + count, 14].Value = totalTax;
					worksheet.Cells[indexRow + count, 14].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 14].Style.Font.Size = 13;
					//Value sum toltal amount
					worksheet.Cells[indexRow + count, 15].Value = totalAmount;
					worksheet.Cells[indexRow + count, 15].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 15].Style.Font.Size = 13;
					//format currency
					worksheet.Cells[4, 11, indexRow + count, 17].Style.Numberformat.Format = "#,###";

					// set border
					worksheet.Cells[3, 1, indexRow + count, 17].Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 17].Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 17].Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 17].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

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
					worksheet.Cells[indexRow + count + 1, 11, indexRow + count + 1, 14].Merge = true;
					worksheet.Cells[indexRow + count + 1, 11, indexRow + count + 1, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					worksheet.Cells[indexRow + count + 2, 11].Value = dictionary["RPFTCREATOR"];
					worksheet.Cells[indexRow + count + 2, 11, indexRow + count + 2, 14].Merge = true;
					worksheet.Cells[indexRow + count + 2, 11, indexRow + count + 2, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					worksheet.Cells[indexRow + count + 5, 11].Value = dictionary["RPFTPRINTBY"] + ": " + user + ", " + dictionary["RPFTPRINTTIME"] + ": " + DateTime.Now.ToString("HH:mm");
					worksheet.Cells[indexRow + count + 5, 11, indexRow + count + 5, 14].Merge = true;
					worksheet.Cells[indexRow + count + 5, 11, indexRow + count + 5, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					// autoFit
					worksheet.Column(1).Width = 5;
					worksheet.Column(2).Width = 10;
					worksheet.Column(3).Width = 10;
					worksheet.Column(4).Width = 5;
					worksheet.Column(5).Width = 8;
					worksheet.Column(6).Width = 12;
					worksheet.Column(7).Width = 20;
					worksheet.Column(8).Width = 10;
					worksheet.Column(9).Width = 15;
					worksheet.Column(10).Width = 15;
					worksheet.Column(11).Width = 8;
					worksheet.Column(12).Width = 9;
					worksheet.Column(13).Width = 12;
					worksheet.Column(14).Width = 12;
					worksheet.Column(15).Width = 12;
					worksheet.Column(16).Width = 14;
					worksheet.Column(17).Width = 14;

					return new MemoryStream(package.GetAsByteArray());
				}
			}
			catch (NullReferenceException)
			{
				throw new NullReferenceException();
			}
		}
		public static Stream ExportExpenseTruckListToExcel(List<ExpenseDetailListReport> data, Dictionary<string, string> dictionary, int intLanguague,
														string companyName, string companyAddress, string fileName, string user, DateTime dateFrom, DateTime dateTo)
		{
			try
			{
				using (ExcelPackage package = new ExcelPackage())
				{
					// add a new worksheet to the empty workbook
					ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ExpenseDetailList");

					//Company name
					worksheet.Cells[1, 1].Value = companyName;
					worksheet.Cells[1, 1].Style.Font.Size = 11;
					worksheet.Cells[1, 1, 1, 4].Merge = true;
					//Company address
					worksheet.Cells[2, 1].Value = companyAddress;
					worksheet.Cells[2, 1].Style.Font.Size = 11;
					worksheet.Cells[2, 1, 2, 4].Merge = true;

					//Add the title
					worksheet.Cells[1, 5].Value = dictionary["TLTEXPENSEREPORT"];
					worksheet.Cells[1, 5].Style.Font.Bold = true;
					worksheet.Cells[1, 5].Style.Font.Size = 16;
					worksheet.Cells[1, 5, 1, 10].Merge = true;
					worksheet.Cells[1, 5, 1, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

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
					worksheet.Cells[2, 5, 2, 10].Merge = true;
					worksheet.Cells[2, 5, 2, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					// Add the headers
					// column1
					worksheet.Cells[3, 1].Value = "#";
					// column2
					worksheet.Cells[3, 2].Value = dictionary["LBLINVOICEDATE"];
					// column3
					worksheet.Cells[3, 3].Value = dictionary["LBLTRANSPORTDATEDISPATCH"];
					// column4
					worksheet.Cells[3, 4].Value = dictionary["LBLTYPE"];
					// column5
					worksheet.Cells[3, 5].Value = dictionary["LBLCONTSIZERP"];
					// column6
					worksheet.Cells[3, 6].Value = dictionary["LBLCONTNUMBER"];
					// column7
					worksheet.Cells[3, 7].Value = dictionary["TLTROUTE"];
					// column8
					worksheet.Cells[3, 8].Value = dictionary["LBLTRAILER"];
					// column9
					worksheet.Cells[3, 9].Value = dictionary["LBLEMPLOYEE"];
					// column10
					worksheet.Cells[3, 10].Value = dictionary["TLTEXPENSE"];
					
					// column11
					worksheet.Cells[3, 11].Value = dictionary["LBLTEQUANTITY"];
					// column12
					worksheet.Cells[3, 12].Value = dictionary["LBLUNITPRICE"];
					// column13
					worksheet.Cells[3, 13].Value = dictionary["LBLTOL"];
					// column14
					worksheet.Cells[3, 14].Value = dictionary["LBLTAXAMOUNT"];
					// column15
					worksheet.Cells[3, 15].Value = dictionary["LBLTETOTAL"];
					// column16
					worksheet.Cells[3, 16].Value = dictionary["RPHDCONTENT"];
					// column17
					worksheet.Cells[3, 17].Value = dictionary["LBLSUPPIERREPORT"];
					//set height header
					worksheet.Row(3).Height = 25;
					// set font bold
					worksheet.Cells[3, 1, 3, 17].Style.Font.Bold = true;

					// set border
					worksheet.Cells[3, 1, 3, 17].Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 17].Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 17].Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, 3, 17].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

					// align center
					worksheet.Cells[3, 1, 3, 17].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					worksheet.Cells[3, 1, 3, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					// set color
					worksheet.Cells[3, 1, 3, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
					worksheet.Cells[3, 1, 3, 17].Style.Fill.BackgroundColor.SetColor(Color.DarkGray);

					//write content
					data = data.OrderBy(p => p.RegisteredNo).ThenBy(d => d.InvoiceD).ToList();
					var indexRow = 5;
					var count = data.Count;
					double total = 0;
					double totalQuantity = 0;
					double totalTax = 0;
					double totalAmount = 0;
					if (data != null && data.Count > 0)
					{
						string categoryExpense = data[0].RegisteredNo;
						int categoryQuantity = 0;
						double categoryTax = 0;
						double categoryTotal = 0;
						int postition = 4;
						double categoryTotalAmount = 0;
						worksheet.Cells[4, 1].Value = (String.IsNullOrEmpty(categoryExpense) ? "" : categoryExpense);
						worksheet.Cells[4, 1, 4, 10].Merge = true;

						for (var iloop = 0; iloop < count; iloop++)
						{
							if (categoryExpense != data[iloop].RegisteredNo)
							{
								worksheet.Cells[postition, 11].Value = categoryQuantity;
								worksheet.Cells[postition, 13].Value = categoryTotal;
								worksheet.Cells[postition, 14].Value = categoryTax;
								worksheet.Cells[postition, 15].Value = categoryTotalAmount;
								worksheet.Cells[postition, 1, postition, 17].Style.Font.Bold = true;
								worksheet.Cells[postition, 1, postition, 17].Style.Font.Size = 12;
								// set color
								worksheet.Cells[postition, 1, postition, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
								worksheet.Cells[postition, 1, postition, 17].Style.Fill.BackgroundColor.SetColor(Color.SeaShell);

								categoryExpense = data[iloop].RegisteredNo;
								categoryQuantity = 0;
								categoryTax = 0;
								categoryTotal = 0;
								worksheet.Cells[indexRow + iloop, 1].Value = categoryExpense;
								worksheet.Cells[indexRow + iloop, 1, indexRow + iloop, 10].Merge = true;
								postition = indexRow + iloop;
								indexRow += 1;
							}
							categoryQuantity += data[iloop].Quantity != null ? (int)data[iloop].Quantity : 0;
							categoryTax += data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0;
							categoryTotal += data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0;
							totalQuantity += data[iloop].Quantity != null ? (double)data[iloop].Quantity : 0;
							total += data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0;
							totalTax += data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0;
							categoryTotalAmount += (data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0) +
												   (data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0);
							totalAmount += (data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0) +
												   (data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0);
							String temp = data[iloop].Location1N + ", " + data[iloop].Location2N + ", " + data[iloop].Location3N;
							var getdata = temp.Split(new string[] { ", " }, StringSplitOptions.None);
							temp = "";
							if (getdata[0] != "")
							{
								temp = getdata[0];
								if (getdata[1] != "")
								{
									temp = temp + ", " + getdata[1];
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}
								}
								else
								{
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}

								}
							}
							else
							{
								if (getdata[1] != "")
								{
									temp = getdata[1];
									if (getdata[2] != "")
									{
										temp = temp + ", " + getdata[2];
									}
								}
								else
								{
									if (getdata[2] != "")
									{
										temp = getdata[2];
									}
								}
							}
							worksheet.Cells[indexRow + iloop, 1].Value = iloop + 1;
							worksheet.Cells[indexRow + iloop, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
							worksheet.Cells[indexRow + iloop, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
							worksheet.Cells[indexRow + iloop, 2].Value = data[iloop].InvoiceD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].InvoiceD, intLanguague) : "";
							worksheet.Cells[indexRow + iloop, 3].Value = data[iloop].TransportD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].TransportD, intLanguague) : "";
							worksheet.Cells[indexRow + iloop, 4].Value = Utilities.GetOrderTypeName(data[iloop].OrderTypeI);
							worksheet.Cells[indexRow + iloop, 5].Value = (data[iloop].ContainerSizeI == "3" ? dictionary["LBLLOAD"] + " (" + (data[iloop].NetWeight ?? 0).ToString("#,###.#") + ")" : Utilities.GetContainerSizeName(data[iloop].ContainerSizeI));
							worksheet.Cells[indexRow + iloop, 5].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 6].Value = data[iloop].ContainerNo;
							worksheet.Cells[indexRow + iloop, 6].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 7].Value = temp;
							worksheet.Cells[indexRow + iloop, 7].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 8].Value = data[iloop].TrailerNo;
							worksheet.Cells[indexRow + iloop, 9].Value = data[iloop].DriverN;
							worksheet.Cells[indexRow + iloop, 9].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 10].Value = data[iloop].ExpenseN;
							worksheet.Cells[indexRow + iloop, 10].Style.WrapText = true;
							worksheet.Cells[indexRow + iloop, 11].Value = data[iloop].Quantity;
							worksheet.Cells[indexRow + iloop, 12].Value = data[iloop].UnitPrice;
							worksheet.Cells[indexRow + iloop, 13].Value = data[iloop].TotalAmount;
							worksheet.Cells[indexRow + iloop, 14].Value = data[iloop].TaxAmount;
							worksheet.Cells[indexRow + iloop, 15].Value = (data[iloop].TotalAmount != null ? (double)data[iloop].TotalAmount : 0) + (data[iloop].TaxAmount != null ? (double)data[iloop].TaxAmount : 0);
							worksheet.Cells[indexRow + iloop, 16].Value = data[iloop].Description;
							worksheet.Cells[indexRow + iloop, 17].Value = data[iloop].SupplierN;
							worksheet.Cells[indexRow + iloop, 17].Style.WrapText = true;
						}
						worksheet.Cells[postition, 11].Value = categoryQuantity;
						worksheet.Cells[postition, 13].Value = categoryTotal;
						worksheet.Cells[postition, 14].Value = categoryTax;
						worksheet.Cells[postition, 15].Value = categoryTotalAmount;
						worksheet.Cells[postition, 1, postition, 17].Style.Font.Bold = true;
						worksheet.Cells[postition, 1, postition, 17].Style.Font.Size = 12;
						// set color
						worksheet.Cells[postition, 1, postition, 17].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells[postition, 1, postition, 17].Style.Fill.BackgroundColor.SetColor(Color.SeaShell);
					}

					worksheet.Cells[indexRow + count, 1].Value = dictionary["RPLBLTOTAL"];
					worksheet.Cells[indexRow + count, 1].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 1].Style.Font.Size = 13;
					worksheet.Cells[indexRow + count, 1, indexRow + count, 10].Merge = true;
					//Value  sum 
					worksheet.Cells[indexRow + count, 11].Value = totalQuantity;
					worksheet.Cells[indexRow + count, 11].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 11].Style.Font.Size = 13;
					//Value  sum 
					worksheet.Cells[indexRow + count, 13].Value = total;
					worksheet.Cells[indexRow + count, 13].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 13].Style.Font.Size = 13;
					//Value sum Tax
					worksheet.Cells[indexRow + count, 14].Value = totalTax;
					worksheet.Cells[indexRow + count, 14].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 14].Style.Font.Size = 13;
					//Value sum toltal amount
					worksheet.Cells[indexRow + count, 15].Value = totalAmount;
					worksheet.Cells[indexRow + count, 15].Style.Font.Bold = true;
					worksheet.Cells[indexRow + count, 15].Style.Font.Size = 13;
					//format currency
					worksheet.Cells[4, 11, indexRow + count, 17].Style.Numberformat.Format = "#,###";

					// set border
					worksheet.Cells[3, 1, indexRow + count, 17].Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 17].Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 17].Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[3, 1, indexRow + count, 17].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

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
					worksheet.Cells[indexRow + count + 1, 11, indexRow + count + 1, 14].Merge = true;
					worksheet.Cells[indexRow + count + 1, 11, indexRow + count + 1, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					worksheet.Cells[indexRow + count + 2, 11].Value = dictionary["RPFTCREATOR"];
					worksheet.Cells[indexRow + count + 2, 11, indexRow + count + 2, 14].Merge = true;
					worksheet.Cells[indexRow + count + 2, 11, indexRow + count + 2, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					worksheet.Cells[indexRow + count + 5, 11].Value = dictionary["RPFTPRINTBY"] + ": " + user + ", " + dictionary["RPFTPRINTTIME"] + ": " + DateTime.Now.ToString("HH:mm");
					worksheet.Cells[indexRow + count + 5, 11, indexRow + count + 5, 14].Merge = true;
					worksheet.Cells[indexRow + count + 5, 11, indexRow + count + 5, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					// autoFit
					worksheet.Column(1).Width = 5;
					worksheet.Column(2).Width = 10;
					worksheet.Column(3).Width = 10;
					worksheet.Column(4).Width = 5;
					worksheet.Column(5).Width = 8;
					worksheet.Column(6).Width = 12;
					worksheet.Column(7).Width = 20;
					worksheet.Column(8).Width = 10;
					worksheet.Column(9).Width = 15;
					worksheet.Column(10).Width = 15;
					worksheet.Column(11).Width = 8;
					worksheet.Column(12).Width = 9;
					worksheet.Column(13).Width = 12;
					worksheet.Column(14).Width = 12;
					worksheet.Column(15).Width = 12;
					worksheet.Column(16).Width = 14;
					worksheet.Column(17).Width = 14;

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
