using CrystalReport.Dataset.FuelConsumption;
using CrystalReport.Dataset.Liabilities;
using CrystalReport.Dataset.PartnerExpense;
using Root.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using Website.Enum;
using Website.Utilities;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Expense;
using Website.ViewModels.Report.DriverRevenue;
using Website.ViewModels.Report.FuelConsumption;
using Website.ViewModels.Report.Liabilities;
using Website.ViewModels.Report.PartnerExpense;

namespace Service.Services
{
	public partial interface IReportService
	{
		Stream ExportPdfLiabilities(DriverRevenueReportParam param, string userName);
		Stream ExportPdfLiabilitiesPayment(DriverRevenueReportParam param, string userName);
		Stream ExportPdfFuelConsumption(FuelConsumptionReportParam param);
		Stream ExportPdfPartnerDetailExpense(PartnerExpenseReportParam param, string userName);
	}

	public partial class ReportService : IReportService
	{
		public Stream ExportPdfLiabilities(DriverRevenueReportParam param, string userName)
		{
			Stream stream;
			var dicLanguage = new Dictionary<string, string>();
			//int intLanguage;
			var dt = new Liabilities.LiabilitiesDataTable();
			//var ldt = new LiabilitiesDetail.LiabilitiesDetailDataTable();
			var companyName = "";
			var companyAddress = "";
			var fileName = "";
			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				fileName = basicSetting.Logo;
			}

			//ger employeeName
			var user = GetEmployeeByUserName(userName);
			// get data
			if (param.ReportType == 0) //General
			{
				dt = ExportPdfLiabilitiesGeneral(param);
				//stream = CrystalReport.Service.Liabilities.ExportPdf.Exec(dt, param.Languague, param.DateFrom, param.DateTo, param.ReportType);
			}
			else
			{
				// get language for report
				//#region language
				//CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
				//if (param.Languague == "vi")
				//{
				//	intLanguage = 1;
				//}
				//else if (param.Languague == "jp")
				//{
				//	intLanguage = 3;
				//	cul = CultureInfo.GetCultureInfo("ja-JP");
				//}
				//else
				//{
				//	intLanguage = 2;
				//	cul = CultureInfo.GetCultureInfo("en-US");
				//}

				//var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
				//													 (con.TextKey == "TLTLIABILITIESREPORT" ||
				//													  con.TextKey == "LBLREPORTIDRIVER" ||
				//													  con.TextKey == "LBLADVANCELIABILITIES" ||
				//													  con.TextKey == "LBLEXPENSELIABILITIES" ||
				//													  con.TextKey == "RPLBLOPENINGBALANCE " ||
				//													  con.TextKey == "TLTTOTALADVANCEREPORT" ||
				//													  con.TextKey == "TLTTOTALPAYMENTREPORT" ||
				//													  con.TextKey == "TLTTOTALSUMREPORT" ||
				//													  con.TextKey == "RPHDDATE" ||
				//													  con.TextKey == "LBLAMOUNTREPORT" ||
				//													  con.TextKey == "RPHDCONTENT" ||
				//													  con.TextKey == "TLTLIABILITIESTOTALREPORT")).ToList();


				//foreach (TextResource_D item in languages)
				//{
				//	if (!dicLanguage.ContainsKey(item.TextKey))
				//	{
				//		dicLanguage.Add(item.TextKey, item.TextValue);
				//	}
				//}
				//#endregion


				//ldt = ExportPdfLiabilitiesDetailNew(param);
				//stream = CrystalReport.Service.Liabilities.ExportPdf.GeneralExec(ldt, intLanguage, param.DateFrom, param.DateTo, dicLanguage);
				dt = ExportPdfLiabilitiesDetail(param);
			}
			stream = CrystalReport.Service.Liabilities.ExportPdf.Exec(dt, param.Languague, param.DateFrom, param.DateTo, param.ReportType,
																		 companyName, companyAddress, fileName, user);
			return stream;
		}

		public Stream ExportPdfLiabilitiesPayment(DriverRevenueReportParam param, string userName)
		{
			Stream stream;
			var dicLanguage = new Dictionary<string, string>();
			int intLanguage;
			var dt = new LiabilitiesPayment.LiabilitiesPaymentDataTable();
			var companyName = "";
			var companyAddress = "";
			var fileName = "";
			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				fileName = basicSetting.Logo;
			}

			//ger employeeName
			var user = GetEmployeeByUserName(userName);
			// get language for report
			#region language
			CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
			if (param.Languague == "vi")
			{
				intLanguage = 1;
			}
			else if (param.Languague == "jp")
			{
				intLanguage = 3;
				cul = CultureInfo.GetCultureInfo("ja-JP");
			}
			else
			{
				intLanguage = 2;
				cul = CultureInfo.GetCultureInfo("en-US");
			}

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																	(con.TextKey == "TLTLIABILITIESPAYMENTREPORT" ||
																	con.TextKey == "LBLREPORTIDRIVER" ||
																	con.TextKey == "RPHDDATE" ||
																	con.TextKey == "TLTTRUCK" ||
																	con.TextKey == "TLTTRAILER" ||
																	con.TextKey == "LBLCONTAINERNO" ||
																	con.TextKey == "LBLCONTAINERSIZE" ||
																	con.TextKey == "LBLCUSTOMER" ||
																	con.TextKey == "TLTLOCATION" ||
																	con.TextKey == "RPHDCONTENT" ||
																	con.TextKey == "LBLAMOUNTREPORT" ||
																	con.TextKey == "TLTLIABILITIESTOTALREPORT" ||
																	con.TextKey == "LBLRECEIPTNO" ||
																	con.TextKey == "LBLDETAIL" ||
																	con.TextKey == "LBLITEM" ||
																	con.TextKey == "LBLLOAD" ||
																	con.TextKey == "RPFTPRINTTIME" ||
																	con.TextKey == "RPFTPRINTBY" ||
																	con.TextKey == "RPFTPAGE" ||
																	con.TextKey == "RPFTCREATOR"
																	)).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}
			#endregion

			dt = ExportPdfLiabilitiesPaymentData(param, dicLanguage, intLanguage);
			stream = CrystalReport.Service.Liabilities.ExportPdf.LiabilitiesPaymentExec(dt, intLanguage, param.DateFrom, param.DateTo, dicLanguage,
																		 companyName, companyAddress, fileName, user);
			return stream;
		}

		private Liabilities.LiabilitiesDataTable ExportPdfLiabilitiesGeneral(DriverRevenueReportParam param)
		{
			var dt = new Liabilities.LiabilitiesDataTable();
			DataRow row;
			List<LiabilitiesReportData> data = GetLiabilitiesDetailList(param);
			List<LiabilitiesReportData> balance = GetLiabilitiesBalanceList(param);

			var joinData = data.Concat(balance); // full outer join

			var groupData = (from d in joinData.AsQueryable()
							 group d by new { d.DriverC, d.FirstN, d.LastN } into c
							 select new LiabilitiesReportData()
							 {
								 DriverC = c.Key.DriverC,
								 FirstN = c.Key.FirstN,
								 LastN = c.Key.LastN,
								 PreviousBalance = c.Sum(d => d.PreviousBalance),
								 AdvanceAmount = c.Sum(d => d.AdvanceAmount),
								 ExpenseAmount = c.Sum(d => d.ExpenseAmount),
							 }).ToList();

			if (groupData.Count > 0)
			{
				foreach (var item in groupData)
				{
					row = dt.NewRow();
					row["LiabilitiesD"] = DBNull.Value;
					row["DriverC"] = item.DriverC;
					row["DriverN"] = param.Languague == "vi" ? string.Format("{0} {1}", item.LastN, item.FirstN) : string.Format("{0} {1}", item.FirstN, item.LastN);
					row["PreviousBalance"] = item.PreviousBalance;
					row["AdvanceAmount"] = item.AdvanceAmount;
					row["ExpenseAmount"] = item.ExpenseAmount;
					dt.Rows.Add(row);
				}
			}

			return dt;
		}

		private Liabilities.LiabilitiesDataTable ExportPdfLiabilitiesDetail(DriverRevenueReportParam param)
		{
			var dt = new Liabilities.LiabilitiesDataTable();
			DataRow row;
			List<LiabilitiesReportData> data = GetLiabilitiesDetailList(param);
			List<LiabilitiesReportData> balance = GetLiabilitiesBalanceList(param);

			if (data != null && data.Count > 0)
			{
				foreach (var item in data)
				{
					var preBalance = balance.FirstOrDefault(p => p.DriverC == item.DriverC);
					decimal? totalAdvanceAmount = data.Where(x => x.DriverC == item.DriverC).Select(c => c.AdvanceAmount).Sum();
					decimal? totalExpenseAmount = data.Where(x => x.DriverC == item.DriverC).Select(c => c.ExpenseAmount).Sum();
					int countDriverC = data.Count(p => p.DriverC == item.DriverC);

					row = dt.NewRow();
					row["LiabilitiesD"] = item.LiabilitiesD;
					row["DriverC"] = item.DriverC;
					row["DriverN"] = param.Languague == "vi" ? string.Format("{0} {1}", item.LastN, item.FirstN) : string.Format("{0} {1}", item.FirstN, item.LastN);
					row["AdvanceAmount"] = item.AdvanceAmount;
					row["ExpenseAmount"] = item.ExpenseAmount;
					row["PreviousBalance"] = preBalance != null ? preBalance.PreviousBalance : 0;
					row["Description"] = item.Description;
					row["TotalAdvanceAmount"] = totalAdvanceAmount;
					row["TotalExpenseAmount"] = totalExpenseAmount;
					row["CountDriverC"] = countDriverC;
					dt.Rows.Add(row);
				}
			}

			////get PriviousBalance

			//if (balance != null && balance.Count > 0)
			//{
			//	foreach (var item in balance)
			//	{
			//		row = dt.NewRow();
			//		row["LiabilitiesD"] = DBNull.Value;
			//		row["DriverC"] = item.DriverC;
			//		row["DriverN"] = param.Languague == "vi" ? string.Format("{0} {1}", item.LastN, item.FirstN) : string.Format("{0} {1}", item.FirstN, item.LastN);
			//		row["AdvanceAmount"] = item.PreviousBalance;
			//		row["ExpenseAmount"] = 0;
			//		row["Description"] = "";
			//		dt.Rows.Add(row);
			//	}
			//}
			return dt;
		}

		private LiabilitiesDetail.LiabilitiesDetailDataTable ExportPdfLiabilitiesDetailNew(DriverRevenueReportParam param)
		{
			var dt = new LiabilitiesDetail.LiabilitiesDetailDataTable();
			var balanceList = GetLiabilitiesBalanceList(param);

			if (param.DriverList != null)
			{
				var driverList = (param.DriverList).Split(new string[] { "," }, StringSplitOptions.None);

				for (var d = 0; d < driverList.Length; d++)
				{

					var driverN = "";
					var driverC = driverList[d];
					var driver = _driverRepository.Query(p => p.DriverC == driverC).FirstOrDefault();
					if (driver != null)
					{
						driverN = param.Languague == "vi" ? string.Format("{0} {1}", driver.LastN, driver.FirstN) : string.Format("{0} {1}", driver.FirstN, driver.LastN);
					}

					var advanceData = GetAdvanceLiabilitiesDetailList(param, driverC);
					var paymentData = GetPayementLiabilitiesDetailList(param, driverC);

					var totalPreviousBalance = 0;
					var previousBalance = balanceList.FirstOrDefault(p => p.DriverC == driverC);
					if (previousBalance != null)
					{
						if (previousBalance.PreviousBalance != null)
						{
							totalPreviousBalance = (int)previousBalance.PreviousBalance;
						}
					}

					var dtLength = advanceData.Count > paymentData.Count ? advanceData.Count : paymentData.Count;
					var totalAdvanceAmount = advanceData.Sum(p => p.Amount);
					var totalPaymentAmount = paymentData.Sum(p => p.Amount);

					for (var i = 0; i < dtLength; i++)
					{
						var row = dt.NewRow();
						row["DriverC"] = driverC;
						row["DriverN"] = driverN;
						row["TotalPreviousBalance"] = totalPreviousBalance;
						row["TotalAdvanceAmount"] = totalAdvanceAmount;
						row["TotalPaymentAmount"] = totalPaymentAmount;

						if (i < advanceData.Count)
						{
							row["AdvanceLiabilitiesD"] = advanceData[i].LiabilitiesD != null ? advanceData[i].LiabilitiesD.Value.ToString("dd/MM/yyyy") : "";
							row["AdvanceAmount"] = advanceData[i].Amount;
							row["AdvanceContent"] = advanceData[i].Description;
						}

						if (i < paymentData.Count)
						{
							row["PaymentLiabilitiesD"] = paymentData[i].LiabilitiesD != null ? paymentData[i].LiabilitiesD.Value.ToString("dd/MM/yyyy") : "";
							row["PaymentAmount"] = paymentData[i].Amount;
							row["PaymentContent"] = paymentData[i].Description;
						}

						dt.Rows.Add(row);
					}
				}
			}

			return dt;
		}

		private string[] GetAllLiabilitiesDriver()
		{
			return _liabilitiesRepository.Query(p => p.LiabilitiesI == "1").Select(q => q.DriverC).Distinct().ToArray();
		}

		private LiabilitiesPayment.LiabilitiesPaymentDataTable ExportPdfLiabilitiesPaymentData(DriverRevenueReportParam param, Dictionary<string, string> dicLanguage, int intLanguage)
		{
			var dt = new LiabilitiesPayment.LiabilitiesPaymentDataTable();

			string[] driverList = null;
			if (param.DriverList == null || param.DriverList == "null")
			{
				driverList = GetAllLiabilitiesDriver();
			}
			else
			{
				driverList = (param.DriverList).Split(new string[] { "," }, StringSplitOptions.None);
			}

			if (!driverList.Any())
			{
				driverList = GetAllLiabilitiesDriver();
			}

			for (var d = 0; d < driverList.Length; d++)
			{
				var driverN = "";
				var driverC = driverList[d];
				var driver = _driverRepository.Query(p => p.DriverC == driverC).FirstOrDefault();
				if (driver != null)
				{
					driverN = param.Languague == "vi" ? string.Format("{0} {1}", driver.LastN, driver.FirstN) : string.Format("{0} {1}", driver.FirstN, driver.LastN);
				}

				var paymentData = GetLiabilitiesPayementDataList(param, driverC);
				var dtLength = paymentData.Count;
				var currContainerNo = "";

				for (var i = 0; i < dtLength; i++)
				{
					var paymentList = paymentData[i].PaymentDetailList;
					if (paymentList != null && paymentList.Count > 0)
					{
						string truckN = paymentData[i].PaymentDetailList[0].TruckNo;
						string contN = paymentData[i].PaymentDetailList[0].ContainerNo;
						string loca1N = paymentData[i].PaymentDetailList[0].Location1N;
						string loca2N = paymentData[i].PaymentDetailList[0].Location2N;
						string loca3N = paymentData[i].PaymentDetailList[0].Location3N;
						string cusN = paymentData[i].PaymentDetailList[0].CustomerN;
						string cusshortN = paymentData[i].PaymentDetailList[0].CustomerShortN;
						string contsizeI = paymentData[i].PaymentDetailList[0].ContainerSizeI;
						for (var p = 1; p < paymentList.Count; p++)
						{
							if (truckN == paymentData[i].PaymentDetailList[p].TruckNo &&
								contN == paymentData[i].PaymentDetailList[p].ContainerNo &&
								loca1N == paymentData[i].PaymentDetailList[p].Location1N &&
								loca2N == paymentData[i].PaymentDetailList[p].Location2N &&
								loca3N == paymentData[i].PaymentDetailList[p].Location3N &&
								cusN == paymentData[i].PaymentDetailList[p].CustomerN &&
								cusshortN == paymentData[i].PaymentDetailList[p].CustomerShortN &&
								contsizeI == paymentData[i].PaymentDetailList[p].ContainerSizeI)
							{
								paymentData[i].PaymentDetailList[p].TruckNo = "";
								paymentData[i].PaymentDetailList[p].ContainerNo = "";
								paymentData[i].PaymentDetailList[p].Location1N = "";
								paymentData[i].PaymentDetailList[p].Location2N = "";
								paymentData[i].PaymentDetailList[p].Location3N = "";
								paymentData[i].PaymentDetailList[p].CustomerN = "";
								paymentData[i].PaymentDetailList[p].CustomerShortN = "";
								paymentData[i].PaymentDetailList[p].ContainerSizeI = "";
							}
							else
							{
								truckN = paymentData[i].PaymentDetailList[p].TruckNo;
								contN = paymentData[i].PaymentDetailList[p].ContainerNo;
								loca1N = paymentData[i].PaymentDetailList[p].Location1N;
								loca2N = paymentData[i].PaymentDetailList[p].Location2N;
								loca3N = paymentData[i].PaymentDetailList[p].Location3N;
								cusN = paymentData[i].PaymentDetailList[p].CustomerN;
								cusshortN = paymentData[i].PaymentDetailList[p].CustomerShortN;
								contsizeI = paymentData[i].PaymentDetailList[p].ContainerSizeI;
							}
						}
						for (int j = 0; j < paymentList.Count; j++)
						{
							var row = dt.NewRow();
							row["DriverC"] = driverC;
							row["DriverN"] = driverN;
							row["LiabilitiesD"] = paymentData[i].LiabilitiesD != null ? paymentData[i].LiabilitiesD.Value.ToString("dd/MM/yyyy") : "";
							row["LiabilitiesI"] = paymentData[i].LiabilitiesI;
							if (j == 0)
							{
								row["LiabilitiesNo"] = paymentData[i].LiabilitiesNo;
								row["ReceiptNo"] = paymentData[i].ReceiptNo ?? "";
								row["LiabilitiesContent"] = paymentData[i].LiabiltiesContent ?? "";
								row["LiabilitiesAmount"] = paymentData[i].LiabilitiesAmount ?? null;
							}
							row["TruckNo"] = paymentList[j].TruckNo ?? "";
							row["TrailerNo"] = paymentList[j].TrailerNo;
							currContainerNo = paymentList[j].ContainerNo;
							row["ContainerNo"] = paymentList[j].ContainerNo + "<br>" +
								(paymentList[j].ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] : Utilities.GetContainerSizeName(paymentList[j].ContainerSizeI));

							row["CustomerN"] = string.IsNullOrEmpty(paymentList[j].CustomerShortN) ? paymentList[j].CustomerN : paymentList[j].CustomerShortN;
							var location = "";
							if (!string.IsNullOrEmpty(paymentList[j].Location1N))
							{
								location += paymentList[j].Location1N;
								location += (paymentList[j].Location1DT != null ? " " + Utilities.GetFormatDateAndHourReportByLanguage((DateTime)paymentList[j].Location1DT, intLanguage) : "");
								location += ", ";
							}
							if (!string.IsNullOrEmpty(paymentList[j].Location2N))
							{
								location += paymentList[j].Location2N;
								location += (paymentList[j].Location2DT != null ? " " + Utilities.GetFormatDateAndHourReportByLanguage((DateTime)paymentList[j].Location2DT, intLanguage) : "");
								location += ", ";
							}
							if (!string.IsNullOrEmpty(paymentList[j].Location3N))
							{
								location += paymentList[j].Location3N;
								location += (paymentList[j].Location3DT != null ? " " + Utilities.GetFormatDateAndHourReportByLanguage((DateTime)paymentList[j].Location3DT, intLanguage) : "");
								location += ", ";
							}
							if (location.Length > 1)
								row["LocationN"] = location.Remove(location.Length - 2);

							row["Content"] = paymentList[j].Content;
							row["Amount"] = paymentList[j].ItemAmount ?? 0;
							row["TotalAmount"] = paymentList[j].TotalItemAmount ?? 0;
							dt.Rows.Add(row);
						}
					}
					else
					{
						var row = dt.NewRow();
						row["DriverC"] = driverC;
						row["DriverN"] = driverN;
						row["LiabilitiesD"] = paymentData[i].LiabilitiesD != null ? paymentData[i].LiabilitiesD.Value.ToString("dd/MM/yyyy") : "";
						row["LiabilitiesI"] = paymentData[i].LiabilitiesI;
						row["LiabilitiesNo"] = paymentData[i].LiabilitiesNo;
						row["ReceiptNo"] = paymentData[i].ReceiptNo ?? "";
						row["LiabilitiesContent"] = paymentData[i].LiabiltiesContent ?? "";
						row["LiabilitiesAmount"] = paymentData[i].LiabilitiesAmount ?? null;
						dt.Rows.Add(row);
					}
				}
			}
			//}

			return dt;
		}
		private List<LiabilitiesReportData> GetLiabilitiesBalanceList(DriverRevenueReportParam param)
		{
			var data = from a in _liabilitiesRepository.GetAllQueryable()
					   join c in _driverRepository.GetAllQueryable() on a.DriverC equals c.DriverC into t2
					   from c in t2.DefaultIfEmpty()
					   where ((param.DriverList == "null" || (param.DriverList == c.DriverC)) &
							  (param.DateFrom == null || a.LiabilitiesD < param.DateFrom) &
							  a.Amount != null & a.Amount > 0
						   )
					   select new LiabilitiesReportData()
					   {
						   DriverC = a.DriverC,
						   FirstN = c.FirstN,
						   LastN = c.LastN,
						   PreviousBalance = a.LiabilitiesI == "0" ? a.Amount : a.Amount * (-1),
					   };

			var groupData = from b in data
							group b by new { b.DriverC, b.FirstN, b.LastN } into c
							select new LiabilitiesReportData()
							{
								DriverC = c.Key.DriverC,
								FirstN = c.Key.FirstN,
								LastN = c.Key.LastN,
								PreviousBalance = c.Sum(b => b.PreviousBalance)
							};

			return groupData.ToList();
		}

		private List<LiabilitiesReportData> GetLiabilitiesDetailList(DriverRevenueReportParam param)
		{
			var data = from a in _liabilitiesRepository.GetAllQueryable()
					   join c in _driverRepository.GetAllQueryable() on a.DriverC equals c.DriverC into t2
					   from c in t2.DefaultIfEmpty()
					   where ((param.DriverList == "null" || (param.DriverList == c.DriverC)) &
							  (param.DateFrom == null || a.LiabilitiesD >= param.DateFrom) &
							  (param.DateTo == null || a.LiabilitiesD <= param.DateTo) &
							  a.Amount != null & a.Amount > 0
						   )
					   select new LiabilitiesReportData()
					   {
						   LiabilitiesD = a.LiabilitiesD,
						   DriverC = a.DriverC,
						   FirstN = c.FirstN,
						   LastN = c.LastN,
						   AdvanceAmount = a.LiabilitiesI == "0" ? a.Amount : 0,
						   ExpenseAmount = a.LiabilitiesI == "1" ? a.Amount : 0,
						   Description = a.Description
					   };

			return data.ToList();

		}

		private List<LiabilitiesData> GetAdvanceLiabilitiesDetailList(DriverRevenueReportParam param, string driverC)
		{
			var data = from a in _liabilitiesRepository.GetAllQueryable()
					   join c in _driverRepository.GetAllQueryable() on a.DriverC equals c.DriverC into t2
					   from c in t2.DefaultIfEmpty()
					   where (c.DriverC == driverC &
							  (param.DateFrom == null || a.LiabilitiesD >= param.DateFrom) &
							  (param.DateTo == null || a.LiabilitiesD <= param.DateTo) &
							  a.Amount != null & a.Amount > 0 & a.LiabilitiesI == "0"
						   )
					   select new LiabilitiesData()
					   {
						   DriverC = a.DriverC,
						   FirstN = c.FirstN,
						   LastN = c.LastN,
						   LiabilitiesD = a.LiabilitiesD,
						   Amount = a.LiabilitiesI == "0" ? a.Amount : 0,
						   Description = a.Description,
					   };

			return data.ToList();
		}

		private List<LiabilitiesData> GetPayementLiabilitiesDetailList(DriverRevenueReportParam param, string driverC)
		{
			var data = from l in _liabilitiesItemRepository.GetAllQueryable()
					   join e in _expenseDetailRepository.GetAllQueryable()
					   on new { l.OrderD, l.OrderNo, l.DetailNo, l.DispatchNo, l.ExpenseNo } equals new { e.OrderD, e.OrderNo, e.DetailNo, e.DispatchNo, e.ExpenseNo } into le
					   from e in le.DefaultIfEmpty()
					   join d in _dispatchRepository.GetAllQueryable()
					   on new { l.OrderD, l.OrderNo, l.DetailNo, l.DispatchNo } equals new { d.OrderD, d.OrderNo, d.DetailNo, d.DispatchNo } into led
					   from d in led.DefaultIfEmpty()
					   join c in _driverRepository.GetAllQueryable() on d.DriverC equals c.DriverC into ledc
					   from c in ledc.DefaultIfEmpty()
					   join m in _expenseRepository.GetAllQueryable() on e.ExpenseC equals m.ExpenseC into ledcm
					   from m in ledcm.DefaultIfEmpty()
					   where (d.DriverC == driverC &
							  (param.DateFrom == null || l.LiabilitiesD >= param.DateFrom) &
							  (param.DateTo == null || l.LiabilitiesD <= param.DateTo))
					   select new LiabilitiesData()
					   {
						   DriverC = d.DriverC,
						   FirstN = c.FirstN,
						   LastN = c.LastN,
						   LiabilitiesD = l.LiabilitiesD,
						   Amount = e != null ? e.Amount : 0,
						   Description = m.ExpenseN,
					   }; ;

			return data.ToList();
		}

		private List<LiabilitiesPaymentData> GetLiabilitiesPayementDataList(DriverRevenueReportParam param, string driverC)
		{
			var data = (from p in _liabilitiesRepository.GetAllQueryable()
						join c in _driverRepository.GetAllQueryable() on p.DriverC equals c.DriverC into ledc
						from c in ledc.DefaultIfEmpty()
						where (p.DriverC == driverC & p.LiabilitiesI == "1" &
							   (param.DateFrom == null || p.LiabilitiesD >= param.DateFrom) &
							   (param.DateTo == null || p.LiabilitiesD <= param.DateTo))
						select new LiabilitiesPaymentData()
						{
							DriverC = p.DriverC,
							FirstN = c.FirstN,
							LastN = c.LastN,
							LiabilitiesD = p.LiabilitiesD,
							LiabilitiesI = p.LiabilitiesI,
							LiabilitiesNo = p.LiabilitiesNo,
							LiabilitiesAmount = p.Amount,
							LiabiltiesContent = p.Description,
							ReceiptNo = p.ReceiptNo
						}).OrderBy(x => x.DriverC).OrderBy(x => x.LiabilitiesD)
						.OrderBy(x => x.LiabilitiesNo).ToList();

			for (int i = 0; i < data.Count; i++)
			{
				var liabilitiesD = data[i].LiabilitiesD;
				var liabilitiesNo = data[i].LiabilitiesNo;
				var iDriverC = data[i].DriverC;

				var detail = from l in _liabilitiesItemRepository.GetAllQueryable()
							 join e in _expenseDetailRepository.GetAllQueryable()
							 on new { l.OrderD, l.OrderNo, l.DetailNo, l.DispatchNo, l.ExpenseNo } equals new { e.OrderD, e.OrderNo, e.DetailNo, e.DispatchNo, e.ExpenseNo } into le
							 from e in le.DefaultIfEmpty()
							 join d in _dispatchRepository.GetAllQueryable()
							 on new { l.OrderD, l.OrderNo, l.DetailNo, l.DispatchNo } equals new { d.OrderD, d.OrderNo, d.DetailNo, d.DispatchNo } into led
							 from d in led.DefaultIfEmpty()
							 join m in _expenseRepository.GetAllQueryable() on e.ExpenseC equals m.ExpenseC into ledcm
							 from m in ledcm.DefaultIfEmpty()
							 join tr in _truckRepository.GetAllQueryable()
							 on d.TruckC equals tr.TruckC into tro
							 from tr in tro.DefaultIfEmpty()
							 join od in _orderDRepository.GetAllQueryable()
							 on new { l.OrderD, l.OrderNo, l.DetailNo } equals new { od.OrderD, od.OrderNo, od.DetailNo } into trod
							 from od in trod.DefaultIfEmpty()
							 join ta in _trailerRepository.GetAllQueryable()
							 on od.TrailerC equals ta.TrailerC into odta
							 from ta in odta.DefaultIfEmpty()
							 join oh in _orderHRepository.GetAllQueryable()
							 on new { l.OrderD, l.OrderNo } equals new { oh.OrderD, oh.OrderNo } into ohta
							 from oh in ohta.DefaultIfEmpty()
							 join cu in _customerRepository.GetAllQueryable()
							 on new { oh.CustomerMainC, oh.CustomerSubC } equals new { cu.CustomerMainC, cu.CustomerSubC } into ohcu
							 from cu in ohcu.DefaultIfEmpty()
							 where (l.LiabilitiesD == liabilitiesD &
									 l.LiabilitiesNo == liabilitiesNo &
									 d.DriverC == iDriverC &
									 l.LiabilitiesStatusI == "1")
							 select new LiabilitiesPaymentDetailData()
							 {
								 OrderD = od.OrderD,
								 OrderNo = od.OrderNo,
								 ItemAmount = l != null ? (e.Amount + e.TaxAmount) : 0,
								 Content = l != null ? m.ExpenseN : "",
								 TruckNo = l != null ? tr.RegisteredNo : "",
								 TrailerNo = l != null ? ta.TrailerNo : "",
								 ContainerNo = l != null ? od.ContainerNo : "",
								 CustomerN = l != null ? cu.CustomerN : "",
								 CustomerShortN = l != null ? cu.CustomerShortN : "",
								 ContainerSizeI = l != null ? od.ContainerSizeI : "",
								 Location1N = l != null ? d.Location1N : "",
								 Location2N = l != null ? d.Location2N : "",
								 Location3N = l != null ? d.Location3N : "",
								 Location1DT = l != null ? d.Location1DT : null,
								 Location2DT = l != null ? d.Location2DT : null,
								 Location3DT = l != null ? d.Location3DT : null,
								 TransportD = d.TransportD
							 };
				if (detail.Any())
				{
					var group = (from d in detail
								 group d by new { d.OrderD, d.OrderNo } into g
								 select new
								 {
									 OrderD = g.Key.OrderD,
									 OrderNo = g.Key.OrderNo,
									 TotalItemAmount = g.Sum(d => d.ItemAmount)
								 }).ToList();

					var list = detail.OrderBy(x => x.OrderD).OrderBy(x => x.OrderNo).OrderBy(x => x.TransportD).ToList();

					for (int j = 0; j < list.Count; j++)
					{
						list[j].TotalItemAmount = group.Where(x => x.OrderD == list[j].OrderD & x.OrderNo == list[j].OrderNo)
														.Select(x => x.TotalItemAmount).First();
					}
					data[i].PaymentDetailList = list;
				}
			}

			return data;
		}

		public Stream ExportPdfFuelConsumption(FuelConsumptionReportParam param)
		{
			// get basic information
			var companyName = "";
			var companyAddress = "";
			var fileName = "";
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				fileName = basicSetting.Logo;
			}

			// get data
			var dt = new FuelConsumption.FuelConsumptionDataTable();
			DataRow row;
			List<FuelConsumptionReportData> data = GetFuelConsumptionList(param);

			if (data != null && data.Count > 0)
			{
				foreach (var item in data)
				{
					row = dt.NewRow();
					if (param.ReportType == 0)
					{
						row["Name"] = param.Languague == "vi"
							? string.Format("{0} {1}", item.LastN, item.FirstN)
							: string.Format("{0} {1}", item.FirstN, item.LastN);
					}
					else
					{
						row["Name"] = item.RegisteredNo;
					}
					row["ContainerNumber"] = item.ContainerNumber ?? 0;
					row["EstimatedDistance"] = item.EstimatedDistance ?? 0;
					row["EstimatedFuel"] = item.EstimatedFuel ?? 0;
					row["ActualDistance"] = item.ActualDistance ?? 0;
					row["ActualFuel"] = item.ActualFuel ?? 0;
					dt.Rows.Add(row);
				}
			}

			Stream stream = CrystalReport.Service.FuelConsumption.ExportPdf.Exec(dt, param.Languague, param.DateFrom, param.DateTo, companyName, companyAddress, fileName);
			return stream;
		}

		private List<FuelConsumptionReportData> GetFuelConsumptionList(FuelConsumptionReportParam param)
		{
			var data = new List<FuelConsumptionReportData>();

			//var basic = _basicRepository.GetAllQueryable().FirstOrDefault();
			//string expenseCategoryList ="";
			//if (basic != null)
			//{
			//	if (basic.FuelExpense1 != null) expenseCategoryList += "," + basic.FuelExpense1;
			//	if (basic.FuelExpense2 != null) expenseCategoryList += "," + basic.FuelExpense2;
			//	if (basic.FuelExpense3 != null) expenseCategoryList += "," + basic.FuelExpense3;
			//	if (basic.FuelExpense4 != null) expenseCategoryList += "," + basic.FuelExpense4;
			//	if (basic.FuelExpense5 != null) expenseCategoryList += "," + basic.FuelExpense5;
			//	if (basic.FuelExpense6 != null) expenseCategoryList += "," + basic.FuelExpense6;
			//	if (basic.FuelExpense7 != null) expenseCategoryList += "," + basic.FuelExpense7;
			//	if (basic.FuelExpense8 != null) expenseCategoryList += "," + basic.FuelExpense8;
			//	if (basic.FuelExpense9 != null) expenseCategoryList += "," + basic.FuelExpense9;
			//	if (basic.FuelExpense10 != null) expenseCategoryList += "," + basic.FuelExpense10;
			//}
			//var expenseList = (from e in _expenseRepository.GetAllQueryable()
			//					where expenseCategoryList.Contains(e.CategoryC)
			//					select new { e.ExpenseC }
			//					).ToList();
			//var expense = String.Join("," , expenseList);

			if (param.ReportType == 0)
			{
				var detail = from d in _dispatchRepository.GetAllQueryable()
							 //from f in _fuelConsumptionDetailRepository.GetAllQueryable()
							 //join d in _dispatchRepository.GetAllQueryable() on new { f.OrderD, f.OrderNo, f.DispatchNo, f.DetailNo }
							 //	equals new { d.OrderD, d.OrderNo, d.DispatchNo, d.DetailNo }

							 where ((param.DriverList == "null" || (param.DriverList).Contains(d.DriverC)) &
									(d.TransportD >= param.DateFrom) &
									(d.TransportD <= param.DateTo)
								 )
							 //group new { f, d } by new { d.DriverC, d.OrderD, d.OrderNo, d.DetailNo } into g
							 group new { d } by new { d.DriverC, d.OrderD, d.OrderNo, d.DetailNo } into g
							 select new
							 {
								 Code = g.Key.DriverC,
								 ContainerNumber = 1,
								 EstimatedDistance = g.Sum(i => i.d.ApproximateDistance),
								 EstimatedFuel = g.Sum(i => i.d.FuelConsumption),
								 ActualDistance = g.Sum(i => i.d.ActualDistance),
								 ActualFuel = g.Sum(i => i.d.ActualFuel)
							 };

				var detail2 = from d in detail.AsQueryable()
							  group d by d.Code into g
							  select new
							  {
								  Code = g.Key,
								  ContainerNumber = g.Sum(i => i.ContainerNumber),
								  EstimatedDistance = g.Sum(i => i.EstimatedDistance),
								  EstimatedFuel = g.Sum(i => i.EstimatedFuel),
								  ActualDistance = g.Sum(i => i.ActualDistance),
								  ActualFuel = g.Sum(i => i.ActualFuel)
							  };

				//var actual = from e in _truckExpenseRepository.GetAllQueryable()
				//			 where ((param.DriverList == "null" || (param.DriverList).Contains(e.DriverC)) &
				//					 (e.TransportD >= param.DateFrom) &
				//					 (e.TransportD <= param.DateTo) &
				//					 (expense.Contains(e.ExpenseC)))
				//			 group e by new { e.DriverC } into g
				//			 select new
				//			 {
				//				 Code = g.Key.DriverC,
				//				 ActualFuel = g.Sum(i => i.Quantity)
				//			 };

				data = (from d in detail2.AsQueryable()
						//join a in actual.AsQueryable() on d.Code equals a.Code into t1
						//from a in t1.DefaultIfEmpty()
						join v in _driverRepository.GetAllQueryable() on d.Code equals v.DriverC
						//group new { d, a, v } by new { d.Code, v.LastN, v.FirstN } into c
						group new { d, v } by new { d.Code, v.LastN, v.FirstN } into c
						select new FuelConsumptionReportData()
						{
							Code = c.Key.Code,
							LastN = c.Key.LastN,
							FirstN = c.Key.FirstN,
							ContainerNumber = c.Sum(i => i.d.ContainerNumber),
							EstimatedDistance = c.Sum(i => i.d.EstimatedDistance),
							EstimatedFuel = c.Sum(i => i.d.EstimatedFuel),
							ActualDistance = c.Sum(i => i.d.ActualDistance),
							ActualFuel = c.Sum(i => i.d.ActualFuel),
						}).ToList();
			}
			else
			{
				var detail = from d in _dispatchRepository.GetAllQueryable()
							 //from f in _fuelConsumptionDetailRepository.GetAllQueryable()
							 //join d in _dispatchRepository.GetAllQueryable() on new {f.OrderD, f.OrderNo, f.DispatchNo, f.DetailNo}
							 //	equals new {d.OrderD, d.OrderNo, d.DispatchNo, d.DetailNo}
							 where ((param.TruckList == "null" || (param.TruckList).Contains(d.TruckC)) &
									(d.TransportD >= param.DateFrom) &
									(d.TransportD <= param.DateTo)
								 )
							 group new { d } by new { d.TruckC, d.OrderD, d.OrderNo, d.DetailNo } into g
							 select new
							 {
								 Code = g.Key.TruckC,
								 ContainerNumber = 1, //g.Count(),
								 EstimatedDistance = g.Sum(i => i.d.ApproximateDistance),
								 EstimatedFuel = g.Sum(i => i.d.FuelConsumption),
								 ActualDistance = g.Sum(i => i.d.ActualDistance),
								 ActualFuel = g.Sum(i => i.d.ActualFuel)
							 };

				var detail2 = from d in detail.AsQueryable()
							  group d by d.Code into g
							  select new
							  {
								  Code = g.Key,
								  ContainerNumber = g.Sum(i => i.ContainerNumber),
								  EstimatedDistance = g.Sum(i => i.EstimatedDistance),
								  EstimatedFuel = g.Sum(i => i.EstimatedFuel),
								  ActualDistance = g.Sum(i => i.ActualDistance),
								  ActualFuel = g.Sum(i => i.ActualFuel)
							  };

				//var actual = from e in _truckExpenseRepository.GetAllQueryable() 
				//			where ((e.ObjectI == "0") && (param.TruckList == "null" || (param.TruckList).Contains(e.Code)) &
				//					(e.TransportD >= param.DateFrom) &
				//					(e.TransportD <= param.DateTo) &
				//					(expense.Contains(e.ExpenseC)))
				//			group e by new { e.Code } into g
				//			select new
				//			{
				//				Code = g.Key.Code,
				//				ActualFuel = g.Sum(i => i.Quantity)
				//			};

				data = (from d in detail2.AsQueryable()
						//	 join a in actual.AsQueryable() on d.Code equals a.Code into t1
						//from a in t1.DefaultIfEmpty()
						join t in _truckRepository.GetAllQueryable() on d.Code equals t.TruckC
						group new { d, t } by new { d.Code, t.RegisteredNo } into c
						select new FuelConsumptionReportData()
						{
							Code = c.Key.Code,
							RegisteredNo = c.Key.RegisteredNo,
							ContainerNumber = c.Sum(i => i.d.ContainerNumber),
							EstimatedDistance = c.Sum(i => i.d.EstimatedDistance),
							EstimatedFuel = c.Sum(i => i.d.EstimatedFuel),
							ActualDistance = c.Sum(i => i.d.ActualDistance),
							ActualFuel = c.Sum(i => i.d.ActualFuel),
						}).ToList();
			}

			return data;
		}

		public Stream ExportPdfPartnerDetailExpense(PartnerExpenseReportParam param, string userName)
		{
			Stream stream;
			DataRow row;
			PartnerExpenseDetail.PartnerExpenseDetailDataTable dt;
			int intLanguage;
			//decimal taxRate = 0;
			// get basic information
			var companyName = "";
			var companyAddress = "";
			var fileName = "";
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				fileName = basicSetting.Logo;
			}
			var user = GetEmployeeByUserName(userName);
			// get data
			dt = new PartnerExpenseDetail.PartnerExpenseDetailDataTable();
			List<PartnerExpenseDetailReportData> data = GetPartnerExpenseDetail(param);

			// get language for report
			var dicLanguage = new Dictionary<string, string>();
			CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
			if (param.Languague == "vi")
			{
				intLanguage = 1;
			}
			else if (param.Languague == "jp")
			{
				intLanguage = 3;
				cul = CultureInfo.GetCultureInfo("ja-JP");
			}
			else
			{
				intLanguage = 2;
				cul = CultureInfo.GetCultureInfo("en-US");
			}
			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
													  (con.TextKey == "RPHDPARTNEREXPENSEDETAIL" ||
													  con.TextKey == "RPLBLPARTNERN" ||
													  con.TextKey == "RPLBLPARTNERADDRESS" ||
													  con.TextKey == "RPHDDATE" ||
													  con.TextKey == "RPHDCUSTOMERN" ||
													  con.TextKey == "RPHDCONTNO" ||
													  con.TextKey == "RPHDCONTSIZE" ||

													  con.TextKey == "RPHDTOTALEXPENSE" ||
													  con.TextKey == "LBLTRANSPORTFEEREPORT" ||
													  con.TextKey == "RPHDTOTALSURCHARGE" ||
													  con.TextKey == "RPHDTAXAMOUNT" ||
													  con.TextKey == "LBLTOTALREPORT" ||

													  con.TextKey == "LBLPAYONBEHALF" ||
													  con.TextKey == "RPHDCONTENT" ||
													  con.TextKey == "LBLAMOUNTREPORT" ||
													  con.TextKey == "LBLINVOICENOREPORT" ||

													  con.TextKey == "LBLTOTALAMOUNTREPORT" ||
													  con.TextKey == "RPLBLTOTAL" ||
													  con.TextKey == "RPHDCUSTOMERDISCOUNT" ||

													  con.TextKey == "RPFTPRINTTIME" ||
													  con.TextKey == "RPFTPRINTBY" ||
													  con.TextKey == "RPFTPAGE" ||
													  con.TextKey == "RPFTCREATOR" ||

													  con.TextKey == "LBLLOAD"
													 )).ToList();
			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			if (data != null && data.Count > 0)
			{
				for (int iloop = 0; iloop < data.Count; iloop++)
				{
					row = dt.NewRow();
					row["No"] = 1;
					row["PartnerMainC"] = data[iloop].PartnerMainC;
					row["PartnerSubC"] = data[iloop].PartnerSubC;
					row["PartnerN"] = data[iloop].PartnerN;
					row["Address"] = data[iloop].Address;

					row["TransportD"] = ((DateTime)data[iloop].TransportD).ToString(cul.DateTimeFormat.ShortDatePattern);
					row["CustomerN"] = data[iloop].CustomerN;
					row["ContainerNo"] = data[iloop].ContainerNo;
					row["ContainerSize"] = data[iloop].ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] : Utilities.GetContainerSizeName(data[iloop].ContainerSizeI);
					// set money
					var partnerFee = data[iloop].PartnerFee == null ? 0 : (decimal)data[iloop].PartnerFee;
					var partnerExpense = data[iloop].PartnerExpense == null ? 0 : (decimal)data[iloop].PartnerExpense;
					var partnerSurcharge = data[iloop].PartnerSurcharge == null ? 0 : (decimal)data[iloop].PartnerSurcharge;
					var partnerDiscount = data[iloop].PartnerDiscount == null ? 0 : (decimal)data[iloop].PartnerDiscount;
					var taxRate = (partnerFee + partnerSurcharge) == 0 ? 0 : data[iloop].PartnerTaxAmount / (partnerFee + partnerSurcharge);
					var partnerTaxAmount = data[iloop].PartnerTaxAmount;
					row["PartnerFee"] = partnerFee;
					row["PartnerExpense"] = partnerExpense;
					row["PartnerSurcharge"] = partnerSurcharge;
					row["PartnerDiscount"] = partnerDiscount;
					row["PartnerTaxAmount"] = partnerTaxAmount;

					// set total
					row["PartnerSum"] = partnerFee + partnerSurcharge + partnerTaxAmount;
					row["PartnerTotal"] = partnerFee + partnerSurcharge + partnerTaxAmount + partnerExpense;

					if (data[iloop].PartnerExpenseList != null && data[iloop].PartnerExpenseList.Count() > 0)
					{
						for (int j = 0; j < data[iloop].PartnerExpenseList.Count(); j++)
						{
							if (j == 0)
							{
								row["ExpenseN"] = data[iloop].PartnerExpenseList[j].ExpenseN;
								row["Amount"] = data[iloop].PartnerExpenseList[j].Amount;
								row["Description"] = data[iloop].PartnerExpenseList[j].Description;
								dt.Rows.Add(row);
							}
							else
							{
								var rowDetail = dt.NewRow();
								rowDetail["No"] = 0;
								rowDetail["PartnerMainC"] = data[iloop].PartnerMainC;
								rowDetail["PartnerSubC"] = data[iloop].PartnerSubC;

								rowDetail["ExpenseN"] = data[iloop].PartnerExpenseList[j].ExpenseN;
								rowDetail["Amount"] = data[iloop].PartnerExpenseList[j].Amount;
								rowDetail["Description"] = data[iloop].PartnerExpenseList[j].Description;
								dt.Rows.Add(rowDetail);
							}
						}
					}
					else
					{
						row["ExpenseN"] = "";
						row["Amount"] = 0;
						row["Description"] = "";
						dt.Rows.Add(row);
					}
				}
			}

			// set month and year
			var monthYear = "";
			if (param.ReportI.Equals("A"))
			{
				var transportM = (DateTime)param.TransportM;
				monthYear = transportM.Month + "/" + transportM.Year;
				if (intLanguage == 1)
				{
					monthYear = "tháng " + transportM.Month + "/" + transportM.Year;
				}
				if (intLanguage == 3)
				{
					monthYear = transportM.Year + "年" + transportM.Month + "月";
				}
			}
			else
			{
				var fromDate = (DateTime)param.TransportDFrom;
				var toDate = (DateTime)param.TransportDTo;

				monthYear = "from " + fromDate.ToString("MM/dd/yyyy") + " to " + toDate.ToString("MM/dd/yyyy");
				if (intLanguage == 1)
				{
					monthYear = "từ ngày " + fromDate.ToString("dd/MM/yyyy") + " đến ngày " + toDate.ToString("dd/MM/yyyy");
				}
				if (intLanguage == 3)
				{
					monthYear = fromDate.Year + "年" + fromDate.Month + "月" + fromDate.Day + "日" + "から" +
								toDate.Year + "年" + toDate.Month + "月" + toDate.Day + "日" + "まで";
				}
			}
			stream = CrystalReport.Service.PartnerExpense.ExportPdf.Exec(dt, intLanguage, monthYear, dicLanguage,
																		companyName, companyAddress, fileName, user);
			return stream;
		}

		public List<PartnerExpenseDetailReportData> GetPartnerExpenseDetail(PartnerExpenseReportParam param)
		{
			var result = new List<PartnerExpenseDetailReportData>();

			// get all partner if partner param equals null
			if (param.Partner == "null")
			{
				var partnerList = _partnerService.GetInvoices();

				if (partnerList != null && partnerList.Count > 0)
				{
					param.Partner = "";
					for (var iloop = 0; iloop < partnerList.Count; iloop++)
					{
						if (iloop == 0)
						{
							param.Partner = partnerList[iloop].PartnerMainC + "_" + partnerList[iloop].PartnerSubC;
						}
						else
						{
							param.Partner = param.Partner + "," + partnerList[iloop].PartnerMainC + "_" + partnerList[iloop].PartnerSubC;
						}
					}
				}
			}

			// get data
			if (param.Partner != "null")
			{
				var partrnerArr = (param.Partner).Split(new string[] { "," }, StringSplitOptions.None);
				for (var iloop = 0; iloop < partrnerArr.Length; iloop++)
				{
					var arr = (partrnerArr[iloop]).Split(new string[] { "_" }, StringSplitOptions.None);
					var partnerMainC = arr[0];
					var partnerSubC = arr[1];
					DateTime startDate;
					DateTime endDate;
					if (param.ReportI.Equals("A"))
					{
						// get month and year transport
						var transportM = (DateTime)param.TransportM;
						var month = transportM.Month;
						var year = transportM.Year;
						var invoiceInfo = _invoiceInfoService.GetLimitStartAndEndInvoiceDPartner(partnerMainC, partnerSubC, month, year);
						startDate = invoiceInfo.StartDate.Date;
						endDate = invoiceInfo.EndDate.Date;
					}
					else
					{
						startDate = (DateTime)param.TransportDFrom;
						endDate = (DateTime)param.TransportDTo;
					}
					// get partner who shared a invoice company
					var partnerStr = "";
					var partnerList = _partnerService.GetPartnersByInvoice(partnerMainC, partnerSubC);
					for (var aloop = 0; aloop < partnerList.Count; aloop++)
					{
						partnerStr = partnerStr + "," + partnerList[aloop].PartnerMainC + "_" + partnerList[aloop].PartnerSubC;
					}

					if (partnerStr != "")
					{
						var dispatch = from a in _dispatchRepository.GetAllQueryable()
									   join o in _orderDRepository.GetAllQueryable()
									   on new { a.OrderD, a.OrderNo, a.DetailNo } equals new { o.OrderD, o.OrderNo, o.DetailNo }
									   join h in _orderHRepository.GetAllQueryable()
									   on new { a.OrderD, a.OrderNo } equals new { h.OrderD, h.OrderNo }
									   join p in _partnerRepository.GetAllQueryable()
									   on new { a.PartnerMainC, a.PartnerSubC } equals new { p.PartnerMainC, p.PartnerSubC }
									   join c in _customerRepository.GetAllQueryable()
									   on new { h.CustomerMainC, h.CustomerSubC } equals new { c.CustomerMainC, c.CustomerSubC }
									   where ((o.RevenueD >= startDate & o.RevenueD <= endDate) &
											  (param.DepC == "0" || a.TransportDepC == param.DepC) &
											  (a.PartnerMainC != null & a.PartnerMainC != "" & a.PartnerSubC != null & a.PartnerSubC != "" &
											   partnerStr.Contains(a.PartnerMainC + "_" + a.PartnerSubC)
											  )
											 )
									   select new PartnerExpenseDetailReportData()
									   {
										   OrderD = a.OrderD,
										   OrderNo = a.OrderNo,
										   DetailNo = a.DetailNo,
										   DispatchNo = a.DispatchNo,
										   TransportD = a.TransportD,
										   CustomerN = c.CustomerN,
										   ContainerNo = o.ContainerNo,
										   ContainerSizeI = o.ContainerSizeI,
										   PartnerMainC = partnerMainC,
										   PartnerSubC = partnerSubC,
										   PartnerN = p.PartnerN,
										   Address = p.Address1,
										   PartnerFee = a.PartnerFee,
										   PartnerExpense = a.PartnerExpense,
										   PartnerSurcharge = a.PartnerSurcharge,
										   PartnerDiscount = a.PartnerDiscount,
										   PartnerTaxAmount = a.PartnerTaxAmount ?? 0
										   //TaxRate = invoiceInfo.TaxRate,
										   //TaxRoundingI = invoiceInfo.TaxRoundingI
									   };
						var dataList = dispatch.ToList();
						if (dataList.Count > 0)
						{
							for (int i = 0; i < dataList.Count; i++)
							{
								var orderD = dataList[i].OrderD;
								var orderNo = dataList[i].OrderNo;
								var detailNo = dataList[i].DetailNo;
								var dispatchNo = dataList[i].DispatchNo;
								var expense = from e in _expenseDetailRepository.GetAllQueryable()
											  join m in _expenseRepository.GetAllQueryable()
											  on e.ExpenseC equals m.ExpenseC
											  where e.OrderD == orderD &&
													e.OrderNo == orderNo &&
													e.DetailNo == detailNo &&
													e.DispatchNo == dispatchNo &&
													e.IsPayable == "1"
											  select new ExpenseDetailViewModel()
											  {
												  ExpenseN = m.ExpenseN,
												  Amount = e.Amount,
												  Description = e.Description
											  };
								dataList[i].PartnerExpenseList = expense.ToList();
							}
							result.AddRange(dataList);
						}
					}
				}
			}

			return result;
		}


	}
}
