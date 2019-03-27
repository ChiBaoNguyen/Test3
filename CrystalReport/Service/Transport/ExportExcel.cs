using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using Website.Enum;
using Website.Utilities;
using Website.ViewModels.Order;

namespace CrystalReport.Service.Transport
{
	public class ExportExcel
	{
		public static Stream ExportTransportationPlanToExcel(List<TransportationPlanReport> data, Dictionary<string, string> dictionary, int intLanguague, string title,
														string companyName, string companyAddress, string fileName, string user, string phoneNumber)
		{
			try
			{
				using (ExcelPackage package = new ExcelPackage())
				{

					if (data != null && data.Count > 0)
					{
						// add a new worksheet to the empty workbook
						ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("KeHoachVanChuyen");
						try
						{
							Bitmap image = new Bitmap(HttpContext.Current.Request.MapPath("~/Images/" + fileName));
							ExcelPicture excelImage = null;
							if (image != null)
							{
								excelImage = worksheet.Drawings.AddPicture("logo", image);
								excelImage.From.Column = 0;
								excelImage.From.Row = 0;
								excelImage.SetSize(80, 75);
							}
						}
						catch
						{
							// do nothing
						}
						//Add company info
						worksheet.Cells[1, 4].Value = companyName.ToUpper();
						worksheet.Cells[1, 4].Style.Font.Bold = true;
						worksheet.Cells[1, 4].Style.Font.Size = 10;
						worksheet.Cells[2, 4].Value = companyAddress.ToUpper();
						worksheet.Cells[2, 4].Style.Font.Bold = true;
						worksheet.Cells[2, 4].Style.Font.Size = 10;
						worksheet.Cells[3, 4].Value = phoneNumber;
						worksheet.Cells[3, 4].Style.Font.Bold = true;
						worksheet.Cells[3, 4].Style.Font.Size = 10;

						//Add the title
						worksheet.Cells[4, 1].Value = title.ToUpper();
						worksheet.Cells[4, 1].Style.Font.Bold = true;
						worksheet.Cells[4, 1].Style.Font.Size = 13;
						worksheet.Cells[4, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells[4, 1, 4, 20].Merge = true;

						// Add the headers
						// column1
						worksheet.Cells[6, 1].Value = "#";
						// column2
						worksheet.Cells[6, 2].Value = dictionary["LBLDATE"];
						// column3
						worksheet.Cells[6, 3].Value = dictionary["LBLORDERTYPESHORT"];
						// column4
						//worksheet.Cells[5, 3].Value = dictionary["LBLCONTAINERTYPEREPORT"];
						worksheet.Cells[6, 4].Value = dictionary["LBLCONTSIZE"];
						// column5
						worksheet.Cells[6, 5].Value = dictionary["MNUSHIPPINGCOMPANY"];
						// column6
						worksheet.Cells[6, 6].Value = dictionary["LBLBLBKREPORT"];
						//column7
						worksheet.Cells[6, 7].Value = dictionary["RPHDCOMMODITY"];
						//column8
						worksheet.Cells[6, 8].Value = dictionary["LBLLOADINGLOCATIONREPORT"];
						//column9
						worksheet.Cells[6, 9].Value = dictionary["LBLSTEVEDORAGELOCATIONREPORT"];
						//column10
						worksheet.Cells[6, 10].Value = dictionary["LBLDISCHARGELOCATIONREPORT"];
						//column11
						worksheet.Cells[6, 11].Value = dictionary["RPHDCONTNO"];
						//column12
						worksheet.Cells[6, 12].Value = dictionary["LBLJOBNO"];
						//column13
						worksheet.Cells[6, 13].Value = dictionary["LBLEXPIRATIONSHORT"];
						//column14
						worksheet.Cells[6, 14].Value = dictionary["LBLTRUCKRUNREPORT"];
						//column15
						worksheet.Cells[6, 15].Value = dictionary["LBLTRUCKRETURNREPORT"];
						//column16
						worksheet.Cells[6, 16].Value = dictionary["LBLRETURNDATEREPORT"];
						//column17
						worksheet.Cells[6, 17].Value = dictionary["LBLMOOCNO"];
						//column18
						worksheet.Cells[6, 18].Value = dictionary["LBLWEIGHT"];
						//column19
						worksheet.Cells[6, 19].Value = dictionary["LBLLIFTINGINFO"];
						//column20
						worksheet.Cells[6, 20].Value = dictionary["LBLDESCRIPTION"];

						// set font bold
						worksheet.Cells[6, 1, 6, 20].Style.Font.Bold = true;

						// set border
						worksheet.Cells[6, 1, 6, 20].Style.Border.Top.Style = ExcelBorderStyle.Thin;
						worksheet.Cells[6, 1, 6, 20].Style.Border.Left.Style = ExcelBorderStyle.Thin;
						worksheet.Cells[6, 1, 6, 20].Style.Border.Right.Style = ExcelBorderStyle.Thin;
						worksheet.Cells[6, 1, 6, 20].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

						// align center
						worksheet.Cells[6, 1, 6, 20].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
						worksheet.Cells[6, 1, 6, 20].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
						worksheet.Cells[6, 1, 6, 20].Style.WrapText = true;

						// set color
						worksheet.Cells[6, 1, 6, 20].Style.Font.Color.SetColor(Color.MediumBlue);
						worksheet.Cells[6, 1, 6, 20].Style.Font.Size = 10;
						var count = data.Count;
						// write content
						var startRow = 7;
						var rowIndex = 7;
						var totalColumn = 20;
						// set border for same cont
						worksheet.Cells[rowIndex, 1, rowIndex, 20].Style.Border.Top.Style = ExcelBorderStyle.Thick;
						worksheet.Cells[rowIndex, 1, rowIndex, 20].Style.Border.Left.Style = ExcelBorderStyle.Thin;
						worksheet.Cells[rowIndex, 1, rowIndex, 20].Style.Border.Right.Style = ExcelBorderStyle.Thin;
						DateTime oDFlag = data[0].OrderD;
						string oNoFlag = data[0].OrderNo;
						for (var iloop = 0; iloop < count; iloop++)
						{
							// set border for same cont
							if (iloop + 1 < count && ((data[iloop + 1].OrderD == oDFlag || data[iloop + 1].OrderD != oDFlag) && data[iloop + 1].OrderNo != oNoFlag))
							{
								oDFlag = data[iloop + 1].OrderD;
								oNoFlag = data[iloop + 1].OrderNo;
								worksheet.Cells[rowIndex, 1, rowIndex, 20].Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
								worksheet.Cells[rowIndex, 1, rowIndex, 20].Style.Border.Left.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[rowIndex, 1, rowIndex, 20].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							}
							else
							{
								worksheet.Cells[rowIndex, 1, rowIndex, 20].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[rowIndex, 1, rowIndex, 20].Style.Border.Left.Style = ExcelBorderStyle.Thin;
								worksheet.Cells[rowIndex, 1, rowIndex, 20].Style.Border.Right.Style = ExcelBorderStyle.Thin;
							}
							int columnIndex = 1;
							//column1
							worksheet.Cells[rowIndex, columnIndex].Value = iloop + 1;
							columnIndex++;
							//column2
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].OrderD.Day + "/" + data[iloop].OrderD.Month + "/" + data[iloop].OrderD.Year;
							if (intLanguague == 2)
							{
								worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].OrderD.Month + "/" + data[iloop].OrderD.Day + "/" + data[iloop].OrderD.Year;
							}
							//column3
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].OrderTypeI == "0"
								? dictionary["LBLEXPORTSHORT"]
								: (data[iloop].OrderTypeI == "1" ? dictionary["LBLIMPORTSHORT"] : dictionary["LBLOTHER"]);
							//column4
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].ContainerSize == "0"
								? "20'"
								: (data[iloop].ContainerSize == "1" ? "40'" : (data[iloop].ContainerSize == "2" ? "45'" : "HL"));
							//column5
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].ShippingCompanyN;
							//column6
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].Booking;
							//column7
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].CommodityN;
							//column8
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].Locaion1N;
							if (string.IsNullOrEmpty(data[iloop].Locaion1N) || data[iloop].Locaion1N == "")
							{
								if ((data[iloop].Locaion2N == data[iloop].LocaionRoot1N && data[iloop].Locaion3N == data[iloop].LocaionRoot2N)||
									(data[iloop].Locaion2N == data[iloop].LocaionRoot2N && data[iloop].Locaion3N == data[iloop].LocaionRoot1N))
								{
									worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].LocaionRoot3N;
								}
								if ((data[iloop].Locaion2N == data[iloop].LocaionRoot3N && data[iloop].Locaion3N == data[iloop].LocaionRoot2N) ||
									(data[iloop].Locaion2N == data[iloop].LocaionRoot2N && data[iloop].Locaion3N == data[iloop].LocaionRoot3N))
								{
									worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].LocaionRoot1N;
								}
								if ((data[iloop].Locaion2N == data[iloop].LocaionRoot1N && data[iloop].Locaion3N == data[iloop].LocaionRoot3N) ||
									(data[iloop].Locaion2N == data[iloop].LocaionRoot3N && data[iloop].Locaion3N == data[iloop].LocaionRoot1N))
								{
									worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].LocaionRoot2N;
								}
							}
							//column9
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].Locaion2N;
							if (string.IsNullOrEmpty(data[iloop].Locaion2N) || data[iloop].Locaion2N == "")
							{
								if ((data[iloop].Locaion1N == data[iloop].LocaionRoot1N && data[iloop].Locaion3N == data[iloop].LocaionRoot2N) ||
									(data[iloop].Locaion1N == data[iloop].LocaionRoot2N && data[iloop].Locaion3N == data[iloop].LocaionRoot1N))
								{
									worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].LocaionRoot3N;
								}
								if ((data[iloop].Locaion1N == data[iloop].LocaionRoot3N && data[iloop].Locaion3N == data[iloop].LocaionRoot2N) ||
									(data[iloop].Locaion1N == data[iloop].LocaionRoot2N && data[iloop].Locaion3N == data[iloop].LocaionRoot3N))
								{
									worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].LocaionRoot1N;
								}
								if ((data[iloop].Locaion1N == data[iloop].LocaionRoot1N && data[iloop].Locaion3N == data[iloop].LocaionRoot3N) ||
									(data[iloop].Locaion1N == data[iloop].LocaionRoot3N && data[iloop].Locaion3N == data[iloop].LocaionRoot1N))
								{
									worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].LocaionRoot2N;
								}
							}
							//column10
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].Locaion3N;
							if (string.IsNullOrEmpty(data[iloop].Locaion3N) || data[iloop].Locaion3N == "")
							{
								if ((data[iloop].Locaion1N == data[iloop].LocaionRoot1N && data[iloop].Locaion2N == data[iloop].LocaionRoot2N) ||
									(data[iloop].Locaion1N == data[iloop].LocaionRoot2N && data[iloop].Locaion2N == data[iloop].LocaionRoot1N))
								{
									worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].LocaionRoot3N;
								}
								if ((data[iloop].Locaion1N == data[iloop].LocaionRoot3N && data[iloop].Locaion2N == data[iloop].LocaionRoot2N) ||
									(data[iloop].Locaion1N == data[iloop].LocaionRoot2N && data[iloop].Locaion2N == data[iloop].LocaionRoot3N))
								{
									worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].LocaionRoot1N;
								}
								if ((data[iloop].Locaion1N == data[iloop].LocaionRoot1N && data[iloop].Locaion2N == data[iloop].LocaionRoot3N) ||
									(data[iloop].Locaion1N == data[iloop].LocaionRoot3N && data[iloop].Locaion2N == data[iloop].LocaionRoot1N))
								{
									worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].LocaionRoot2N;
								}
							}

							//column11
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].ContainerNo;
							//column12
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].JobNo;
							//column13
							columnIndex ++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].ClosingDT != null ? (data[iloop].ClosingDT.Value.Hour + "H" + (data[iloop].ClosingDT.Value.Minute.ToString() == "0" ? "" : data[iloop].ClosingDT.Value.Minute.ToString()) + "\n" +
								data[iloop].ClosingDT.Value.Day + "/" + data[iloop].ClosingDT.Value.Month) : "";
							//column14
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].TruckNoRun;
							//column15
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].TruckNoReturn;
							//column16
							columnIndex++;
							if (data[iloop].ReturnDate != null)
							{
								worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].ReturnDate.Value.Day + "/" + data[iloop].ReturnDate.Value.Month + "/" + data[iloop].ReturnDate.Value.Year;
								if (intLanguague == 2)
								{
									worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].ReturnDate.Value.Month + "/" + data[iloop].ReturnDate.Value.Day + "/" + data[iloop].ReturnDate.Value.Year;
								}
							}
							//column17
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].TrailerNo;
							//column18
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].EstimatedWeight;
							//column19
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].IsCollected;
							//column20
							columnIndex++;
							worksheet.Cells[rowIndex, columnIndex].Value = data[iloop].Description;
							rowIndex++;
						}

						// set font
						worksheet.Cells[startRow, 1, rowIndex - 1, totalColumn].Style.Font.Size = 10;

						// set border
						//worksheet.Cells[startRow, 1, rowIndex - 1, totalColumn].Style.Border.Top.Style = ExcelBorderStyle.Thin;
						//worksheet.Cells[startRow, 1, rowIndex - 1, totalColumn].Style.Border.Left.Style = ExcelBorderStyle.Thin;
						//worksheet.Cells[startRow, 1, rowIndex - 1, totalColumn].Style.Border.Right.Style = ExcelBorderStyle.Thin;
						//worksheet.Cells[startRow, 1, rowIndex - 1, totalColumn].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
						//set column
						worksheet.Column(1).Width = 4;
						worksheet.Column(2).Width = 11;
						worksheet.Column(3).Width = 8;
						worksheet.Column(4).Width = 5;
						worksheet.Column(5).Width = 10;
						worksheet.Column(6).Width = 8;
						worksheet.Column(7).Width = 11;
						worksheet.Column(8).Width = 14;
						worksheet.Column(9).Width = 14;
						worksheet.Column(10).Width = 14;
						worksheet.Column(11).Width = 11;
						worksheet.Column(12).Width = 8;
						worksheet.Column(13).Width = 10;
						worksheet.Column(14).Width = 10;
						worksheet.Column(15).Width = 10;
						worksheet.Column(16).Width = 11;
						worksheet.Column(17).Width = 12;
						worksheet.Column(18).Width = 7;
						worksheet.Column(19).Width = 20;
						worksheet.Column(20).Width = 14;

						//format text
						worksheet.Cells[startRow, 1, rowIndex - 1, totalColumn].Style.WrapText = true;
						//print printer
						worksheet.Cells[rowIndex, 13].Value = dictionary["RPFTPRINTBY"] + ": " + user + ", " + dictionary["RPFTPRINTTIME"] + ": " + DateTime.Now.ToString("HH:mm");
						worksheet.Cells[rowIndex, 13, rowIndex, totalColumn].Merge = true;
						worksheet.Cells[rowIndex, 13, rowIndex, totalColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
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
	}
}
