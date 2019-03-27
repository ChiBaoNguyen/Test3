using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Models;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CrystalReport.Service.ContainerSize
{
    public class ExportExcel
    {
		public static Stream GetContainerSizeList(List<ContainerSize_M> containerSizeList, Dictionary<string, string> dicLanguage)
        {
            try
            {
                //FileInfo newFile = new FileInfo("E:\\temp.xls");
                using (ExcelPackage package = new ExcelPackage())
                {
                    int iloop;

                    // add a new worksheet to the empty workbook
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("ContainerSize");
                    //Add the title
					worksheet.Cells[1, 1].Value = dicLanguage["LBLTITLE"];
                    worksheet.Cells[1, 1, 1, 4].Merge = true;
                    //Add the headers
					worksheet.Cells[3, 1].Value = dicLanguage["LBLCONTAINERSIZECODE"];
					worksheet.Cells[3, 2].Value = dicLanguage["LBLCONTAINERSIZENAME"];
					worksheet.Cells[3, 3].Value = dicLanguage["LBLDESCRIPTION"];
					worksheet.Cells[3, 4].Value = dicLanguage["LBLSTATUS"];

                    iloop = 4;
                    foreach (var containerSizeM in containerSizeList)
                    {
                        worksheet.Cells[iloop, 1].Value = containerSizeM.ContainerSizeC;
                        worksheet.Cells[iloop, 2].Value = containerSizeM.ContainerSizeN;
                        worksheet.Cells[iloop, 3].Value = containerSizeM.Description;
                        worksheet.Cells[iloop, 4].Value = "Khả dụng";
                        if (!containerSizeM.IsActive)
                        {
                            worksheet.Cells[iloop, 4].Value = "Không khả dụng";
                        }
                        iloop++;
                    }

                    worksheet.Cells[3, 1, iloop - 1, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[3, 1, iloop - 1, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[3, 1, iloop - 1, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    worksheet.Cells[3, 1, iloop - 1, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    worksheet.Cells[3, 1, iloop - 1, 4].AutoFitColumns();

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
