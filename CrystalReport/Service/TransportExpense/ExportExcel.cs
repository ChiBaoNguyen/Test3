using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Data;
using System.IO;
using System.Web.Hosting;
using System.Globalization;
using OfficeOpenXml;
using Website.ViewModels.Dispatch;
using System.Drawing;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using Website.Utilities;
using Website.ViewModels.Expense;
using System.Linq;

namespace CrystalReport.Service.TransportExpense
{
	public class ExportExcel
	{
		public static Stream Exec(List<ExpenseViewModel> listexpenseviewreport, List<ExpenseDetailViewModel> listexpensedetailviewreport, decimal? unitpriceFuel, List<DispatchDetailViewModel> data,
								  int language,
								  string fromDate,
								  string toDate,
								  string companyName,
								  string companyAddress,
								  string fileName,
								  string user,
								  Dictionary<string, string> dictionary
								 )
		{
			try
			{
				using (ExcelPackage package = new ExcelPackage())
				{
					// set culture
					CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
					// set period time
					var lblFromToDate = "Từ ngày " + fromDate + " đến ngày " + toDate;
					// set currentDate
					var currentDate = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
					if (language == 2)
					{
						cul = CultureInfo.GetCultureInfo("en-US");
						lblFromToDate = "Period " + fromDate + " to " + toDate;
						currentDate = cul.DateTimeFormat.GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ", " + DateTime.Now.Year;
					}
					else if (language == 3)
					{
						cul = CultureInfo.GetCultureInfo("ja-JP");
						lblFromToDate = fromDate + "から" + toDate + "まで";
						currentDate = DateTime.Now.Year + "年" + DateTime.Now.Month + "月" + DateTime.Now.Day + "日";
					}

					// add a new worksheet to the empty workbook
					ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("TransportExpenseList");
					var rowIndex = 1;
					var colIndex = 1;
					//Add the title
					#region Title
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
							excelImage.SetSize(95, 59);
						}
					}
					catch
					{
						// do nothing
					}
					var listexpense = listexpenseviewreport;
					listexpenseviewreport = listexpenseviewreport.Where(p => p.ViewReport == "True").ToList().GroupBy(p => new { p.ColumnName })
					.Select(p => new ExpenseViewModel()
					{
						ColumnName = p.Key.ColumnName
					}).ToList();
					int countview = listexpenseviewreport.Count;
					var tolCol = 16 + countview;
					//Add the title
					worksheet.Cells[rowIndex, 4].Value = companyName;
					worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
					worksheet.Cells[rowIndex, 4, rowIndex, 7].Merge = true;

					worksheet.Cells[rowIndex, 8].Value = (dictionary.ContainsKey("TLTTRANSPORTEXPENSEREPORT") ? dictionary["TLTTRANSPORTEXPENSEREPORT"] : "");
					worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;
					worksheet.Cells[rowIndex, 8].Style.Font.Size = 16;
					worksheet.Cells[rowIndex, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					worksheet.Cells[rowIndex, 8, rowIndex, 14].Merge = true;

					// next row
					rowIndex = rowIndex + 1;
					worksheet.Cells[rowIndex, 4].Value = companyAddress;
					worksheet.Cells[rowIndex, 4].Style.Font.Size = 11;
					worksheet.Cells[rowIndex, 4, rowIndex, 7].Merge = true;

					worksheet.Cells[rowIndex, 8].Value = lblFromToDate;
					worksheet.Cells[rowIndex, 8].Style.Font.Bold = true;
					worksheet.Cells[rowIndex, 8].Style.Font.Size = 11;
					worksheet.Cells[rowIndex, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					worksheet.Cells[rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					worksheet.Cells[rowIndex, 8, rowIndex, 14].Merge = true;
					#endregion

					// Add the headers
					#region Header
					// next row
					rowIndex = rowIndex + 2;
					worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Font.Size = 11;
					worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.Font.Bold = true;
					worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
					worksheet.Cells[rowIndex, 1, rowIndex + 1, tolCol].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

					colIndex = 1;
					worksheet.Cells[rowIndex, colIndex].Value = "#";
					worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
					colIndex++;
					worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLTRANSPORTDATEDISPATCHRP"];
					worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
					colIndex++;
					worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLCUSTOMERREPORT"];
					worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
					colIndex++;
					worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLCONTNUMBER"];
					worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
					colIndex++;
					worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLORDERTYPEDISPATCH"];
					worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
					colIndex++;
					worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLTYPE"];
					worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
					colIndex++;
					worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLSTOPOVERPLACESHORT"];
					worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
					colIndex++;
					worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLLOADINGPLACESHORT"];
					worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
					colIndex++;
					worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLDISCHARGEPLACESHORT"];
                    worksheet.Cells[rowIndex, colIndex, rowIndex + 1 , colIndex].Merge = true;
					colIndex++;
					worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLREVENUE"];
					worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
					colIndex++;
					worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLEXPENSESHORT"];
					worksheet.Cells[rowIndex, colIndex, rowIndex, colIndex + 3 + countview].Merge = true;
					worksheet.Cells[rowIndex + 1, colIndex].Value = dictionary["LBLFUEL"];
                    worksheet.Cells[rowIndex + 1, colIndex].Style.WrapText = true;
					colIndex++;
					worksheet.Cells[rowIndex + 1, colIndex].Value = dictionary["LBLSALARYDRIVER"];
                    worksheet.Cells[rowIndex + 1, colIndex].Style.WrapText = true;
					colIndex++;
					worksheet.Cells[rowIndex + 1, colIndex].Value = dictionary["LBLTRUCKRENTAL"];
					worksheet.Cells[rowIndex + 1, colIndex].Style.WrapText = true;
					if (countview > 0)
					{
						for (int vloop = 0; vloop < countview; vloop++)
						{
							colIndex++;
							worksheet.Cells[rowIndex + 1, colIndex].Value = !string.IsNullOrEmpty(listexpenseviewreport[vloop].ColumnName) ? listexpenseviewreport[vloop].ColumnName : "";
							worksheet.Cells[rowIndex + 1, colIndex].Style.WrapText = true;
						}
						colIndex++;
						worksheet.Cells[rowIndex + 1, colIndex].Value = dictionary["LBLOTHER"];
						worksheet.Cells[rowIndex + 1, colIndex].Style.WrapText = true;
					}
					else
					{
						colIndex++;
						worksheet.Cells[rowIndex + 1, colIndex].Value = dictionary["LBLOTHER"];
						worksheet.Cells[rowIndex + 1, colIndex].Style.WrapText = true;
					}
					
                    colIndex++;
					worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLPROFIT"];
					worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
                    worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Style.WrapText = true;
					colIndex++;
					worksheet.Cells[rowIndex, colIndex].Value = dictionary["LBLEXPLAINOTHEREXPENSE"];
					worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Merge = true;
                    worksheet.Cells[rowIndex, colIndex, rowIndex + 1, colIndex].Style.WrapText = true;
					#endregion

					// write content
					rowIndex = 7;
					var rowSumStart = rowIndex;
					var count = data.Count;
					decimal totalprofit = 0;
					decimal totalfuel = 0;
					decimal totaldriverallowance = 0;
					decimal totalpartner = 0;
					decimal totalother = 0;
					decimal totalrevenue = 0;
					if (data != null && count > 0)
					{
						decimal categoryprofit = 0;
						decimal categoryfuel = 0;
						decimal categorydriverallowance = 0;
						decimal categorypartner = 0;
						decimal categoryother = 0;
						decimal categoryrevenue = 0;
						decimal?[] arraydecimal = new decimal?[countview];

						string category = data[0].OrderH.BLBK;
						int postition = 6;
						worksheet.Cells[6, 1].Value = "BL/BK: " + (String.IsNullOrEmpty(category) ? "" : category);
						worksheet.Cells[6, 1, 6, 9].Merge = true;
						for (int iloop = 0; iloop < count; iloop++)
						{
							if (category != data[iloop].OrderH.BLBK)
							{
								if (categoryprofit != 0)
								{
									worksheet.Cells[postition, 10].Value = categoryprofit;
								}
								if (categoryfuel != 0)
								{
									worksheet.Cells[postition, 11].Value = categoryfuel;
								}
								if (categorydriverallowance != 0)
								{
									worksheet.Cells[postition, 12].Value = categorydriverallowance;
								}
								if (categorypartner != 0)
								{
									worksheet.Cells[postition, 13].Value = categorypartner;
								}
								int totaltopindex = 13;
								if (countview > 0)
								{
									for (int toploop = 0; toploop < countview; toploop++)
									{
										totaltopindex++;
										worksheet.Cells[postition, totaltopindex].Value = arraydecimal[toploop];
									}
									totaltopindex++;
									if (0 == 0)
									{
										worksheet.Cells[postition, totaltopindex].Value = "khac";
									}
									totaltopindex++;
									if (categoryrevenue != 0)
									{
										worksheet.Cells[postition, totaltopindex].Value = categoryrevenue;
									} 
								}
								else
								{
									if (0 == 0)
									{
										worksheet.Cells[postition, 14].Value = "khac";
									}
									if (categoryrevenue != 0)
									{
										worksheet.Cells[postition, 15].Value = categoryrevenue;
									}
								}
								
								worksheet.Cells[postition, 1, postition, 16 + countview].Style.Font.Bold = true;
								worksheet.Cells[postition, 1, postition, 16 + countview].Style.Font.Size = 12;
								// set color
								worksheet.Cells[postition, 1, postition, 16 + countview].Style.Fill.PatternType = ExcelFillStyle.Solid;
								worksheet.Cells[postition, 1, postition, 16 + countview].Style.Fill.BackgroundColor.SetColor(Color.SeaShell);

								categoryprofit = 0;
								categoryfuel = 0;
								categorydriverallowance = 0;
								categorypartner = 0;
								categoryother = 0;
								categoryrevenue = 0;
								arraydecimal = new decimal?[countview];

								category = data[iloop].OrderH.BLBK;
								worksheet.Cells[rowIndex + iloop, 1].Value = "BL/BK: " + (String.IsNullOrEmpty(category) ? "" : category);
								worksheet.Cells[rowIndex + iloop, 1, rowIndex + iloop, 9].Merge = true;
								postition = rowIndex + iloop;
								rowIndex += 1;
							}

							decimal rowrevenue = 0;
							decimal rowexpense = 0;
							worksheet.Cells[rowIndex + iloop, 1].Value = iloop + 1;
							if (language == 1)
							{
								worksheet.Cells[rowIndex + iloop, 2].Value = ((DateTime)data[iloop].Dispatch.TransportD).ToString("dd/MM/yyyy");
							}
							else if (language == 2)
							{
								worksheet.Cells[rowIndex + iloop, 2].Value = ((DateTime)data[iloop].Dispatch.TransportD).ToString("MM/dd/yyyy");
							}
							else if (language == 3)
							{
								worksheet.Cells[rowIndex + iloop, 2].Value = ((DateTime)data[iloop].Dispatch.TransportD).ToString("yyyy/MM/dd");
							}
							worksheet.Cells[rowIndex + iloop, 3].Value = data[iloop].OrderH.CustomerShortN != ""
								? data[iloop].OrderH.CustomerShortN
								: data[iloop].OrderH.CustomerN;
							worksheet.Cells[rowIndex + iloop, 4].Value = !string.IsNullOrEmpty(data[iloop].OrderD.ContainerNo)
								? (data[iloop].OrderD.ContainerNo)
								: "";
							worksheet.Cells[rowIndex + iloop, 5].Value = data[iloop].OrderH.OrderTypeI == "0" ? dictionary["LBLEXPORTSHORT"] : (data[iloop].OrderH.OrderTypeI == "1" ? dictionary["LBLIMPORTSHORT"] : dictionary["LBLTRIP"]);
							if (data[iloop].OrderD.ContainerSizeI == "3")
							{
								worksheet.Cells[rowIndex + iloop, 6].Style.Numberformat.Format = "#,##0";
								worksheet.Cells[rowIndex + iloop, 6].Value = data[iloop].OrderD.NetWeight;
							}
							else
							{
								worksheet.Cells[rowIndex + iloop, 6].Value = Utilities.GetContainerSizeName(data[iloop].OrderD.ContainerSizeI);
							}

							worksheet.Cells[rowIndex + iloop, 7].Value = string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch1)
								? data[iloop].Dispatch.Location1N
								: data[iloop].OrderD.LocationDispatch1;
							worksheet.Cells[rowIndex + iloop, 7].Style.WrapText = true;

							worksheet.Cells[rowIndex + iloop, 8].Value = string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch2)
								? data[iloop].Dispatch.Location2N
								: data[iloop].OrderD.LocationDispatch2;
							worksheet.Cells[rowIndex + iloop, 8].Style.WrapText = true;

							worksheet.Cells[rowIndex + iloop, 9].Value = string.IsNullOrEmpty(data[iloop].OrderD.LocationDispatch3)
								? data[iloop].Dispatch.Location3N
								: data[iloop].OrderD.LocationDispatch3;
							worksheet.Cells[rowIndex + iloop, 9].Style.WrapText = true;
							//Doanh thu
							var orderAmount = data[iloop].OrderD.Amount ?? 0;
							worksheet.Cells[rowIndex + iloop, 10].Value = "";
							if (orderAmount != 0)
							{
								categoryprofit += orderAmount;
								totalprofit += orderAmount;
								rowrevenue = orderAmount;
								worksheet.Cells[rowIndex + iloop, 10].Value = orderAmount;
							}
							//Chi phí
							var disFuelAmount = (data[iloop].Dispatch.TotalFuel ?? 0) * (unitpriceFuel ?? 0);
							var disDriverAllowance = data[iloop].Dispatch.TotalDriverAllowance ?? 0;
							var orderPartnerAmount = data[iloop].OrderD.PartnerAmount ?? 0;

							if (disFuelAmount != 0)
							{
								rowexpense += disFuelAmount;
								categoryfuel += disFuelAmount;
								totalfuel += disFuelAmount;
								worksheet.Cells[rowIndex + iloop, 11].Value = disFuelAmount;
							}
							if (disDriverAllowance != 0)
							{
								rowexpense += disDriverAllowance;
								categorydriverallowance += disDriverAllowance;
								totaldriverallowance += disDriverAllowance;
								worksheet.Cells[rowIndex + iloop, 12].Value = disDriverAllowance;
							}
							if (orderPartnerAmount != 0)
							{
								rowexpense += orderPartnerAmount;
								categorypartner += orderPartnerAmount;
								totalpartner += orderPartnerAmount;
								worksheet.Cells[rowIndex + iloop, 13].Value = orderPartnerAmount;
							}
							int columnindex = 13;
							string explainother = "";
							decimal? totalotheramountincell = 0;
							if(countview > 0)
							{
								List<ExpenseViewModel> getotherexpensebyviewreport = new List<ExpenseViewModel>(listexpense);
								for (int vloop = 0; vloop < countview; vloop++)
								{
									decimal? totalamountincell = 0;
									var getexpensebyviewreport = new List<ExpenseViewModel>();
									listexpense.ForEach(p =>
									{
										if (p.ColumnName == listexpenseviewreport[vloop].ColumnName)
										{
											getexpensebyviewreport.Add(p);
										}
									});
									for (int exloop = 0; exloop < getotherexpensebyviewreport.Count; exloop++)
									{
										getexpensebyviewreport.ForEach(p =>
										{
											if (p.ExpenseC == getotherexpensebyviewreport[exloop].ExpenseC)
											{
												getotherexpensebyviewreport.RemoveAt(exloop);
												exloop--;
											}
										});
									}
									
									for (int eloop = 0; eloop < getexpensebyviewreport.Count; eloop++)
									{
										var listResult = new List<ExpenseDetailViewModel>();
										
										
										listexpensedetailviewreport.ForEach(p =>
										{
											if (p.ExpenseC == getexpensebyviewreport[eloop].ExpenseC && p.OrderD == data[iloop].Dispatch.OrderD && p.OrderNo == data[iloop].Dispatch.OrderNo && p.DetailNo == data[iloop].Dispatch.DetailNo)
											{
												listResult.Add(p);
											}
										});
										for (int lloop = 0; lloop < listResult.Count; lloop++)
										{
											totalamountincell += listResult[lloop].Amount ?? 0;
										}
										
									}
									columnindex++;
									worksheet.Cells[rowIndex + iloop, columnindex].Value = totalamountincell;
									arraydecimal[vloop] = (arraydecimal[vloop] ?? 0) + totalamountincell;
								}
								getotherexpensebyviewreport = getotherexpensebyviewreport.GroupBy(p => new { p.ExpenseC })
								.Select(p => new ExpenseViewModel()
								{
									ExpenseC = p.Key.ExpenseC
								}).ToList();


								for (int oloop = 0; oloop < getotherexpensebyviewreport.Count; oloop++)
								{
									var listResultOther = new List<ExpenseDetailViewModel>();
									listexpensedetailviewreport.ForEach(p =>
									{
										if (p.ExpenseC == getotherexpensebyviewreport[oloop].ExpenseC && p.OrderD == data[iloop].Dispatch.OrderD && p.OrderNo == data[iloop].Dispatch.OrderNo && p.DetailNo == data[iloop].Dispatch.DetailNo)
										{
											listResultOther.Add(p);
										}
									});
									for (int lloop2 = 0; lloop2 < listResultOther.Count; lloop2++)
									{
										string exc = listResultOther[lloop2].ExpenseC;
										totalotheramountincell += listResultOther[lloop2].Amount ?? 0;
										var ex = listexpense.Where(p => p.ExpenseC == exc).FirstOrDefault();
										if (ex != null)
										{
											explainother = explainother + (!string.IsNullOrEmpty(explainother) ? "," : "") + ex.ExpenseN;
										}
									}
								}
								columnindex++;
								worksheet.Cells[rowIndex + iloop, columnindex].Value = totalotheramountincell;
								columnindex++;
								if (rowrevenue != 0 && rowexpense != 0)
								{
									categoryrevenue += rowrevenue + rowexpense;
									totalrevenue += rowrevenue + rowexpense;
									worksheet.Cells[rowIndex + iloop, columnindex].Value = rowrevenue + rowexpense;
								}
								columnindex++;
								worksheet.Cells[rowIndex + iloop, columnindex].Value = explainother;
								worksheet.Cells[rowIndex + iloop, columnindex].Style.WrapText = true;
							}
							else
							{
								for (int h = 0; h < listexpense.Count; h++)
								{
									var listResultOther = new List<ExpenseDetailViewModel>();
									listexpensedetailviewreport.ForEach(p =>
									{
										if (p.ExpenseC == listexpense[h].ExpenseC && p.OrderD == data[iloop].Dispatch.OrderD && p.OrderNo == data[iloop].Dispatch.OrderNo && p.DetailNo == data[iloop].Dispatch.DetailNo)
										{
											listResultOther.Add(p);
										}
									});
									for (int hloop = 0; hloop < listResultOther.Count; hloop++)
									{
										string exc = listResultOther[hloop].ExpenseC;
										totalotheramountincell += listResultOther[hloop].Amount ?? 0;
										var ex = listexpense.Where(p => p.ExpenseC == exc).FirstOrDefault();
										if (ex != null)
										{
											explainother = explainother + (!string.IsNullOrEmpty(explainother) ? "," : "") + ex.ExpenseN;
										}
									}
								}
								worksheet.Cells[rowIndex + iloop, 14].Value = totalotheramountincell;
								if (rowrevenue != 0 && rowexpense != 0)
								{
									categoryrevenue += rowrevenue + rowexpense;
									totalrevenue += rowrevenue + rowexpense;
									worksheet.Cells[rowIndex + iloop, 15].Value = rowrevenue + rowexpense;
								}
								worksheet.Cells[rowIndex + iloop, 16].Value = explainother;
								worksheet.Cells[rowIndex + iloop, 16].Style.WrapText = true;
							}
						}
						if (categoryprofit !=0)
						{
							worksheet.Cells[postition, 10].Value = categoryprofit;
						}
						if (categoryfuel != 0)
						{
							worksheet.Cells[postition, 11].Value = categoryfuel;
						}
						if (categorydriverallowance != 0)
						{
							worksheet.Cells[postition, 12].Value = categorydriverallowance;
						}
						if (categorypartner != 0)
						{
							worksheet.Cells[postition, 13].Value = categorypartner;
						}
						int categoryindex = 13;
						if (countview > 0)
						{
							for (int cloop = 0; cloop < countview; cloop++)
							{
								categoryindex++;
								worksheet.Cells[postition, categoryindex].Value = arraydecimal[cloop];
							}
							categoryindex++;
							if (0 == 0)
							{
								worksheet.Cells[postition, categoryindex].Value = "khac";
							}
							categoryindex++;
							if (categoryrevenue != 0)
							{
								worksheet.Cells[postition, categoryindex].Value = categoryrevenue;
							}
						}
						else
						{
							if (0 == 0)
							{
								worksheet.Cells[postition, 14].Value = "khac";
							}
							if (0 == 0)
							{
								worksheet.Cells[postition, 15].Value = categoryrevenue;
							}
						}
						worksheet.Cells[postition, 1, postition, 16 + countview].Style.Font.Bold = true;
						worksheet.Cells[postition, 1, postition, 16 + countview].Style.Font.Size = 12;
						// set color
						worksheet.Cells[postition, 1, postition, 16 + countview].Style.Fill.PatternType = ExcelFillStyle.Solid;
						worksheet.Cells[postition, 1, postition, 16 + countview].Style.Fill.BackgroundColor.SetColor(Color.SeaShell);
					}
					//write Sum
					#region Sum
					worksheet.Cells[rowIndex + count, 1].Value = dictionary["RPLBLTOTAL"];
					worksheet.Cells[rowIndex + count, 1, rowIndex + count, 9].Merge = true;
					worksheet.Cells[rowIndex + count, 1, rowIndex + count, tolCol].Style.Font.Size = 13;
					worksheet.Cells[rowIndex + count, 1, rowIndex + count, tolCol].Style.Font.Bold = true;

					if (totalprofit != 0)
					{
						worksheet.Cells[rowIndex + count, 10].Value = totalprofit;
					}
					if (totalfuel != 0)
					{
						worksheet.Cells[rowIndex + count, 11].Value = totalfuel;
					}
					if (totaldriverallowance != 0)
					{
						worksheet.Cells[rowIndex + count, 12].Value = totaldriverallowance;
					}
					if (totalpartner != 0)
					{
						worksheet.Cells[rowIndex + count, 13].Value = totalpartner;
					}
					int totalindex = 13;
					if (countview > 0)
					{
						for (int tloop = 0; tloop < countview; tloop++)
						{
							totalindex++;
						}
						totalindex++;
						if (0 == 0)
						{
							worksheet.Cells[rowIndex + count, totalindex].Value = "khac";
						}
						totalindex++;
						if (totalrevenue != 0)
						{
							worksheet.Cells[rowIndex + count, totalindex].Value = totalrevenue;
						}
					}
					else
					{
						if (0 == 0)
						{
							worksheet.Cells[rowIndex + count, 14].Value = "khac";
						}
						if (0 == 0)
						{
							worksheet.Cells[rowIndex + count, 15].Value = totalrevenue;
						}
					}
					
					#endregion
					worksheet.Cells[1, 10, rowIndex + count, tolCol].Style.Numberformat.Format = "#,##0";
					worksheet.Cells[4, 1, rowIndex + count, tolCol].Style.Border.BorderAround(ExcelBorderStyle.Thin);
					worksheet.Cells[4, 1, rowIndex + count, tolCol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[4, 1, rowIndex + count, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[4, 1, rowIndex + count, tolCol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells[4, 1, rowIndex + count, tolCol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
					
					// write footer
					#region Footer

					//note log
					worksheet.Cells[rowIndex + count + 1, 14].Value = currentDate;
					worksheet.Cells[rowIndex + count + 1, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					worksheet.Cells[rowIndex + count + 1, 14, rowIndex + count + 1, 18].Merge = true;
					worksheet.Cells[rowIndex + count + 2, 14].Value = dictionary["RPFTCREATOR"];
					worksheet.Cells[rowIndex + count + 2, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					worksheet.Cells[rowIndex + count + 2, 14, rowIndex + count + 2, 18].Merge = true;
					worksheet.Cells[rowIndex + count + 5, 14].Value = dictionary["RPFTPRINTBY"] + ": " + user + ", " + dictionary["RPFTPRINTTIME"] + ": " + DateTime.Now.ToString("HH:mm");
					worksheet.Cells[rowIndex + count + 5, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
					worksheet.Cells[rowIndex + count + 5, 14, rowIndex + count + 5, 18].Merge = true;

					#endregion

					// autoFit
					#region Column size
					worksheet.Column(1).Width = 5;
					worksheet.Column(2).Width = 11;
					worksheet.Column(3).Width = 12;
					worksheet.Column(4).Width = 13;
					worksheet.Column(5).Width = 8;
					worksheet.Column(6).Width = 5;
					worksheet.Column(7).Width = 14;
					worksheet.Column(8).Width = 14;
					worksheet.Column(9).Width = 14;
					worksheet.Column(10).Width = 14;
					worksheet.Column(11).Width = 12;
					worksheet.Column(12).Width = 12;
					worksheet.Column(13).Width = 12;
					int widthindex = 13;
					if(countview>0)
					{
						for (int widthloop = 0; widthloop < countview; widthloop++)
						{
							widthindex++;
							worksheet.Column(widthindex).Width = 12;
						}
						widthindex++;
						worksheet.Column(widthindex).Width = 12;
						widthindex++;
						worksheet.Column(widthindex).Width = 14;
						widthindex++;
						worksheet.Column(widthindex).Width = 14;
					}
					else{
						worksheet.Column(14).Width = 12;
						worksheet.Column(15).Width = 14;
						worksheet.Column(16).Width = 14;
					}
					#endregion

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
