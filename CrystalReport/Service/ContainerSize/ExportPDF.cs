using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using System.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalReport.FileRpt.ContainerSize;
using System;
using System.Data;
using System.IO;
using System.Web.Hosting;

namespace CrystalReport.Service.ContainerSize
{
    public class ExportPDF
    {
		public static Stream GetContainerSizeList(_DataSet_ContainerSizeList.DataTable1DataTable datatable, int intLanguage = 1)
        {
            try
            {
                ReportDocument report;
                string strPath;

				//strPath = HttpContext.Current.Request.MapPath("~/FileRpt/ContainerSize/ContainnerSizeList.rpt");
                //deploy
				strPath = HttpContext.Current.Request.MapPath("~/FileRpt/ContainerSize/ContainnerSizeList.rpt");
				if (intLanguage == 3)
	            {
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/ContainerSize/ContainnerSizeListJP.rpt");
	            }
				else if (intLanguage == 2)
				{
					strPath = HttpContext.Current.Request.MapPath("~/FileRpt/ContainerSize/ContainnerSizeListEN.rpt");
				}

                report = new ReportDocument();
                report.Load(strPath);
                report.Database.Tables[0].SetDataSource((DataTable)datatable);

				//if (type == 1)
				//{
				//	return report.ExportToStream(ExportFormatType.Excel);
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
