using CrystalReport.Dataset.CustomerPayment;
using CrystalReport.Dataset.TransportExpense;
using Root.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using Website.Enum;
using Website.Utilities;
using Website.ViewModels.Container;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Order;
using Website.ViewModels.Report.CustomerPayment;
using Website.ViewModels.Report.CustomerPricing;
using CrystalReport.Dataset.CustomerPricing;
using Website.ViewModels.Report.PartnerPayment;
using CrystalReport.Dataset.PartnerPayment;
using Website.ViewModels.Report.PartnerExpense;
using CrystalReport.Dataset.PartnerBalance;
using Website.ViewModels.Report.PartnerBalance;
using Website.ViewModels.Report.CombinedRevenue;
using Website.ViewModels.Report.CombinedExpense;
using CrystalReport.Dataset.CombinedRevenue;
using Website.ViewModels.Location;

namespace Service.Services
{
	public partial interface IReportService
	{
		Stream ExportPdfOrderBalance(DriverDispatchReportParam param);
		Stream ExportPdfCustomerPaymentGeneral(CustomerPaymentReportParam param, string userName);
		Stream ExportPdfCustomerPaymentDetail(CustomerPaymentReportParam param, string userName);
		Stream ExportPdfCustomerPaymentProgress(CustomerPaymentReportParam param, string userName);

		Stream ExportPdfCustomerPricingDetail(CustomerPricingReportParam param);
		Stream ExportPdfCustomerPricingGeneral(CustomerPricingReportParam param);

		Stream ExportPdfPartnerPaymentGeneral(PartnerPaymentReportParam param, string userName);
		Stream ExportPdfPartnerPaymentDetail(PartnerPaymentReportParam param, string userName);

		Stream ExportPdfPartnerPaymentProgress(PartnerPaymentReportParam param, string userName);

		Stream ExportPdfPartnerBalance(PartnerExpenseReportParam param);

		Stream ExportPdfCombinedRevenue(CombinedRevenueReportParam param, string userName);
	}

	public partial class ReportService : IReportService
	{
		public Stream ExportPdfOrderBalance(DriverDispatchReportParam param)
		{
			Stream stream;
			TransportExpense.TransportExpenseDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			int index = 1;

			// get data
			dt = new TransportExpense.TransportExpenseDataTable();
			List<DispatchDetailViewModel> data = GetOrderBalanceReportList(param);

			#region setlanguage
			// get language for report
			dicLanguage = new Dictionary<string, string>();
			CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
			if (param.Laguague == "vi")
			{
				intLanguage = 1;
			}
			else if (param.Laguague == "jp")
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
																 (con.TextKey == "LBLCONTAINERSTATUS1DISPATCH" ||
																  con.TextKey == "LBLCONTAINERSTATUS2DISPATCH" ||
																  con.TextKey == "LBLCONTAINERSTATUS3DISPATCH" ||
																  con.TextKey == "LBLDISPATCHSTATUS1" ||
																  con.TextKey == "LBLDISPATCHSTATUS2" ||
																  con.TextKey == "LBLDISPATCHSTATUS3" ||
																  con.TextKey == "LBLDISPATCHSTATUS4" ||
																  con.TextKey == "LBLREPORTI" ||
																  con.TextKey == "LBLREPORTIORDER" ||
																  con.TextKey == "LBLREPORTIDISPATCH" ||
																  con.TextKey == "LBLLOAD"
																  )).ToList();
			// add into dictionary
			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}
			#endregion

			if (data != null && data.Count > 0)
			{
				for (int iloop = 0; iloop < data.Count; iloop++)
				{
					decimal? totalExpense = 0;
					var amount = data[iloop].OrderD.Amount;
					var customerDiscount = data[iloop].OrderD.CustomerDiscount;

					for (int j = 0; j < data[iloop].DispatchList.Count(); j++)
					{
						var dispatchViewModel = data[iloop].DispatchList.GroupBy(d => d.DetailNo)
							.Select(cl => new DispatchViewModel
							{
								Expense = cl.Sum(
									c =>
										c.PartnerFee + c.IncludedExpense + c.DriverAllowance  //+ c.Expense
										+ c.PartnerExpense + c.PartnerSurcharge - c.PartnerDiscount),
							}).ToList().FirstOrDefault();
						if (dispatchViewModel != null)
							totalExpense = dispatchViewModel.Expense;

						var row = dt.NewRow();
						if (j > 0)
						{
							row["No"] = 0;
							row["OrderDate"] = "";
							row["OrderNo"] = "";
							row["OrderType"] = "";
							row["Customer"] = "";
							//row["DepartmentN"] = data[iloop].OrderH.OrderDepN;
							row["BKBL"] = "";
							row["ContainerNo"] = "";
							row["ContainerSize"] = "";
							row["LoadingDate"] = "";
							row["DischargeDate"] = "";
							row["Amount"] = 0;
							row["Surcharge"] = 0;
							row["Discount"] = 0;
							row["Profit"] = 0;
						}
						else
						{
							row["No"] = index;
							row["OrderDate"] = Utilities.GetFormatDateReportByLanguage(data[iloop].OrderH.OrderD, intLanguage);
							row["OrderNo"] = data[iloop].OrderH.OrderNo + "-" + data[iloop].OrderD.DetailNo;
							row["OrderType"] = Utilities.GetOrderTypeName(data[iloop].OrderH.OrderTypeI);
							row["Customer"] = data[iloop].OrderH.CustomerShortN != "" ? ("&nbsp;" + data[iloop].OrderH.CustomerShortN) : ("&nbsp;" + data[iloop].OrderH.CustomerN);
							//row["DepartmentN"] = data[iloop].OrderH.OrderDepN;
							row["BKBL"] = data[iloop].OrderH.BLBK;
							row["ContainerNo"] = "&nbsp;" + data[iloop].OrderD.ContainerNo;
							row["ContainerSize"] = data[iloop].OrderD.ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] : Utilities.GetContainerSizeName(data[iloop].OrderD.ContainerSizeI);
							row["LoadingDate"] = data[iloop].OrderD.ActualLoadingD != null ? 
								Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].OrderD.ActualLoadingD, intLanguage) : "";
							row["DischargeDate"] = data[iloop].OrderD.ActualDischargeD != null ? 
								Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].OrderD.ActualDischargeD, intLanguage) : "";
							row["Amount"] = amount;
							row["Surcharge"] = data[iloop].OrderD.CustomerSurcharge ?? 0;
							row["Discount"] = customerDiscount;
							row["Profit"] = amount - customerDiscount - totalExpense;
						}
						row["TransportD"] = data[iloop].DispatchList[j].TransportD != null ?
							Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].DispatchList[j].TransportD, intLanguage) : "";
						row["RegisteredNo"] = data[iloop].DispatchList[j].RegisteredNo;
						row["Driver"] = data[iloop].DispatchList[j].DriverN;
						// set content(containerStatus)
						if (data[iloop].DispatchList[j].ContainerStatus == Constants.NORMAL)
						{
							row["Content"] = dicLanguage["LBLCONTAINERSTATUS1DISPATCH"];
						}
						else if (data[iloop].DispatchList[j].ContainerStatus == Constants.LOAD)
						{
							row["Content"] = dicLanguage["LBLCONTAINERSTATUS2DISPATCH"];
						}
						else
						{
							row["Content"] = dicLanguage["LBLCONTAINERSTATUS3DISPATCH"];
						}
						// set dispatch status
						if (data[iloop].DispatchList[j].DispatchStatus == Constants.NOTDISPATCH)
						{
							row["DispatchStatus"] = dicLanguage["LBLDISPATCHSTATUS1"];
						}
						else if (data[iloop].DispatchList[j].DispatchStatus == Constants.DISPATCH)
						{
							row["DispatchStatus"] = dicLanguage["LBLDISPATCHSTATUS2"];
						}
						else if (data[iloop].DispatchList[j].DispatchStatus == Constants.TRANSPORTED)
						{
							row["DispatchStatus"] = dicLanguage["LBLDISPATCHSTATUS3"];
						}
						else
						{
							row["DispatchStatus"] = dicLanguage["LBLDISPATCHSTATUS4"];
						}

						row["TransportFee"] = data[iloop].DispatchList[j].TransportFee;
						row["PartnerFee"] = data[iloop].DispatchList[j].PartnerFee;
						row["IncludedExpense"] = data[iloop].DispatchList[j].IncludedExpense;
						row["DriverAllowance"] = data[iloop].DispatchList[j].DriverAllowance;
						row["Expense"] = data[iloop].DispatchList[j].Expense;
						row["PartnerExpense"] = data[iloop].DispatchList[j].PartnerExpense;
						row["PartnerSurcharge"] = data[iloop].DispatchList[j].PartnerSurcharge;
						row["PartnerDiscount"] = data[iloop].DispatchList[j].PartnerDiscount;

						dt.Rows.Add(row);
					}
					index++;
				}
			}

			#region setparam
			// set fromDate and toDate
			var fromDate = "";
			if (param.TransportDFrom != null)
			{
				if (intLanguage == 1)
				{
					fromDate = ((DateTime)param.TransportDFrom).ToString("dd/MM/yyyy");
				}
				else if (intLanguage == 2)
				{
					fromDate = ((DateTime)param.TransportDFrom).ToString("MM/dd/yyyy");
				}
				else if (intLanguage == 3)
				{
					var date = (DateTime)param.TransportDFrom;
					fromDate = date.Year + "年" + date.Month + "月" + date.Day + "日";
				}
			}
			var toDate = "";
			if (param.TransportDTo != null)
			{
				if (intLanguage == 1)
				{
					toDate = ((DateTime)param.TransportDTo).ToString("dd/MM/yyyy");
				}
				else if (intLanguage == 2)
				{
					toDate = ((DateTime)param.TransportDTo).ToString("MM/dd/yyyy");
				}
				else if (intLanguage == 3)
				{
					var date = (DateTime)param.TransportDTo;
					toDate = date.Year + "年" + ("0" + date.Month).Substring(("0" + date.Month).Length - 2) + "月" + date.Day + "日";
				}
			}

			// set category
			var category = dicLanguage["LBLREPORTI"] + ": ";
			if (param.ReportI == "D")
			{
				category = category + dicLanguage["LBLREPORTIDISPATCH"];
			}
			else
			{
				category = category + dicLanguage["LBLREPORTIORDER"];
			}
			#endregion

			stream = CrystalReport.Service.OrderBalance.ExportPdf.Exec(dt, intLanguage, fromDate, toDate, category);
			return stream;
		}
		public List<DispatchDetailViewModel> GetOrderBalanceReportList(DriverDispatchReportParam param)
		{
			var transportExpense = from a in _dispatchRepository.GetAllQueryable()
								   join b in _truckRepository.GetAllQueryable() on a.TruckC equals b.TruckC into t1
								   from b in t1.DefaultIfEmpty()
								   join c in _driverRepository.GetAllQueryable() on a.DriverC equals c.DriverC into t2
								   from c in t2.DefaultIfEmpty()
								   join d in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo }
										   equals new { d.OrderD, d.OrderNo, d.DetailNo } into t3
								   from d in t3.DefaultIfEmpty()
								   join e in _orderHRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
										equals new { e.OrderD, e.OrderNo } into t4
								   from e in t4.DefaultIfEmpty()
								   join f in _customerRepository.GetAllQueryable() on new { e.CustomerMainC, e.CustomerSubC }
										equals new { f.CustomerMainC, f.CustomerSubC } into t5
								   from f in t5.DefaultIfEmpty()
								   join k in _departmentRepository.GetAllQueryable() on e.OrderDepC equals k.DepC into t6
								   from k in t6.DefaultIfEmpty()
								   where ((param.TransportDFrom == null || (param.ReportI == "D" && a.TransportD >= param.TransportDFrom) || (param.ReportI == "O" && e.OrderD >= param.TransportDFrom)) &
										(param.TransportDTo == null || (param.ReportI == "D" && a.TransportD <= param.TransportDTo) || (param.ReportI == "O" && e.OrderD <= param.TransportDTo)) &
										(string.IsNullOrEmpty(param.TruckC) || param.TruckC == "undefined" || a.TruckC == param.TruckC) &
										(string.IsNullOrEmpty(param.DriverC) || param.DriverC == "undefined" || a.DriverC == param.DriverC) &
										(param.Customer == "null" || (param.Customer).Contains(e.CustomerMainC + "_" + e.CustomerSubC)) &
										(string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || (param.ReportI == "D" && b.DepC == param.DepC) || (param.ReportI == "O" && e.OrderDepC == param.DepC))
										)
								   select new DispatchDetailViewModel()
								   {
									   OrderD = new ContainerViewModel()
									   {
										   DetailNo = d.DetailNo,
										   ContainerNo = d.ContainerNo,
										   ContainerSizeI = d.ContainerSizeI,
										   RevenueD = d.RevenueD,
										   Amount = d.Amount ?? 0,
										   CustomerSurcharge = d.CustomerSurcharge ?? 0,
										   CustomerDiscount = d.CustomerDiscount ?? 0,
										   ActualLoadingD = d.ActualLoadingD,
										   ActualDischargeD = d.ActualDischargeD
									   },
									   OrderH = new OrderViewModel()
									   {
										   OrderD = e.OrderD,
										   OrderNo = e.OrderNo,
										   OrderTypeI = e.OrderTypeI,
										   OrderDepC = e.OrderDepC,
										   OrderDepN = k != null ? k.DepN : "",
										   BLBK = e.BLBK,
										   CustomerShortN = f != null ? f.CustomerShortN : "",
										   CustomerN = f != null ? f.CustomerN : "",
									   }
								   };
			transportExpense = transportExpense.Distinct().OrderBy("OrderH.OrderD asc, OrderH.OrderNo asc, OrderD.DetailNo asc");
			var transportExpenseList = transportExpense.ToList();

			#region detailData
			for (int i = 0; i < transportExpenseList.Count(); i++)
			{
				var orderD = transportExpenseList[i].OrderH.OrderD;
				var orderNo = transportExpenseList[i].OrderH.OrderNo;
				var detailNo = transportExpenseList[i].OrderD.DetailNo;

				var dispatchList = (
						from a in _dispatchRepository.GetAllQueryable()
						join b in _truckRepository.GetAllQueryable() on a.TruckC equals b.TruckC into t1
						from b in t1.DefaultIfEmpty()
						join c in _driverRepository.GetAllQueryable() on a.DriverC equals c.DriverC into t2
						from c in t2.DefaultIfEmpty()
						where (a.OrderD == orderD &&
							a.OrderNo == orderNo &&
							a.DetailNo == detailNo &&
							(param.TransportDFrom == null || (param.ReportI == "D" && a.TransportD >= param.TransportDFrom) || (param.ReportI == "O")) &&
							(param.TransportDTo == null || (param.ReportI == "D" && a.TransportD <= param.TransportDTo) || (param.ReportI == "O")) &&
							(string.IsNullOrEmpty(param.TruckC) || param.TruckC == "undefined" || a.TruckC == param.TruckC) &&
							(string.IsNullOrEmpty(param.DriverC) || param.DriverC == "undefined" || a.DriverC == param.DriverC) &&
							(string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || (param.ReportI == "D" && b.DepC == param.DepC) || (param.ReportI == "O"))
							)
						select new DispatchViewModel()
						{
							DetailNo = a.DetailNo,
							DispatchNo = a.DispatchNo,
							TransportD = a.TransportD,
							DispatchOrder = a.DispatchOrder,
							ContainerStatus = a.ContainerStatus,
							DispatchStatus = a.DispatchStatus,
							TruckC = a.TruckC,
							RegisteredNo = (a.RegisteredNo != null && a.RegisteredNo != "") ? a.RegisteredNo : (b != null ? b.RegisteredNo : ""),
							DriverC = a.DriverC,
							DriverN = c != null ? c.LastN + " " + c.FirstN : "",
							TransportFee = a.TransportFee ?? 0,
							PartnerFee = a.PartnerFee ?? 0,
							IncludedExpense = a.IncludedExpense ?? 0,
							DriverAllowance = a.DriverAllowance ?? 0,
							Expense = a.Expense ?? 0,
							PartnerExpense = a.PartnerExpense ?? 0,
							PartnerSurcharge = a.PartnerSurcharge ?? 0,
							PartnerDiscount = a.PartnerDiscount ?? 0,
							DispatchI = a.DispatchI,
						}
					);


				transportExpenseList[i].DispatchList = dispatchList.OrderBy("DispatchNo asc").ToList();
			}
			#endregion

			return transportExpenseList;
		}

		public Stream ExportPdfCustomerPaymentGeneral(CustomerPaymentReportParam param, string userName)
		{
			var companyName = "";
			var companyAddress = "";
			var fileName = "";
			//Get CustomerPayment Data
			var dt = new CustomerPaymentGeneral.CustomerPaymentGeneralDataTable();
			var customerData = GetCustomerPaymentReportList(param);
			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				//companyTaxCode = basicSetting.TaxCode;
				fileName = basicSetting.Logo;
			}
			//ger employeeName
			var user = GetEmployeeByUserName(userName);

			#region Get text resource for translating
			// get language for report
			var dicLanguage = new Dictionary<string, string>();
			int intLanguage = Utilities.ConvertLanguageToInt(param.Languague);

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																  (con.TextKey == "RPHDCUSTOMERPAYMENT" ||
																  con.TextKey == "RPHDCUSTOMERN" ||
																  con.TextKey == "RPHDOPENINGBALANCE" ||
																  con.TextKey == "RPHDOWNEXPENSE" ||
																  con.TextKey == "RPHDOWEEXPENSE" ||
																  con.TextKey == "RPHDCLOSINGBALANCE" ||
																  con.TextKey == "RPLBLTOTAL" ||
																  con.TextKey == "RPHDAMOUNT" ||
																  con.TextKey == "LBLEXPENSEREPORT" ||

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

			if (customerData != null && customerData.Count > 0)
			{
				decimal? totalOpeningBalance = 0;
				decimal? totalOweExpense = 0;
				decimal? totalOwnExpense = 0;
				decimal? totalClosingBalance = 0;
				decimal? totalPayOnBehalf = 0;
				for (var iloop = 0; iloop < customerData.Count; iloop++)
				{
					totalOpeningBalance = totalOpeningBalance + customerData[iloop].OpeningBalance;
					totalOweExpense = totalOweExpense + customerData[iloop].OweExpense;
					totalOwnExpense = totalOwnExpense + customerData[iloop].OwnExpense;
					totalClosingBalance = totalClosingBalance + customerData[iloop].ClosingBalance;
					totalPayOnBehalf = totalPayOnBehalf + customerData[iloop].PayOnBehalf;
				}
				foreach (var customer in customerData)
				{
					var row = dt.NewRow();
					row["CustomerName"] = customer.CustomerN;
					row["OpeningBalance"] = customer.OpeningBalance != 0 ? ((decimal)customer.OpeningBalance.Value).ToString("#,##0") : null;
					row["OweExpense"] = customer.OweExpense != 0 ? ((decimal)customer.OweExpense.Value).ToString("#,##0") : null;
					row["OwnExpense"] = customer.OwnExpense != 0 ? ((decimal)customer.OwnExpense.Value).ToString("#,##0") : null;
					row["ClosingBalance"] = customer.ClosingBalance != 0 ? ((decimal)customer.ClosingBalance.Value).ToString("#,##0") : null;
					row["PayOnBehalf"] = customer.PayOnBehalf != 0 ? ((decimal)customer.PayOnBehalf.Value).ToString("#,##0") : null;

					row["TotalOpeningBalance"] = totalOpeningBalance != 0 ? ((decimal)totalOpeningBalance.Value).ToString("#,##0") : null;
					row["TotalOweExpense"] = totalOweExpense != 0 ? ((decimal)totalOweExpense.Value).ToString("#,##0") : null;
					row["TotalOwnExpense"] = totalOwnExpense != 0 ? ((decimal)totalOwnExpense.Value).ToString("#,##0") : null;
					row["TotalClosingBalance"] = totalClosingBalance != 0 ? ((decimal)totalClosingBalance.Value).ToString("#,##0") : null;
					row["TotalPayOnBehalf"] = totalPayOnBehalf != 0 ? ((decimal)totalPayOnBehalf.Value).ToString("#,##0") : null;
					dt.Rows.Add(row);
				}
			}

			var stream = CrystalReport.Service.CustomerPayment.ExportPdf.GeneralExec(dt, intLanguage, param.DateFrom, param.DateTo, dicLanguage,
																					companyName, companyAddress, fileName, user);
			return stream;
		}

		private List<CustomerPaymentReportData> GetCustomerPaymentReportList(CustomerPaymentReportParam param)
		{
			var customerDataList = new List<CustomerPaymentReportData>();
			if (param.Customer == "null")
			{
				var invoiceList = _customerService.GetInvoices();

				if (invoiceList != null && invoiceList.Count > 0)
				{
					param.Customer = "";
					for (var iloop = 0; iloop < invoiceList.Count; iloop++)
					{
						if (iloop == 0)
						{
							param.Customer = invoiceList[iloop].CustomerMainC + "_" + invoiceList[iloop].CustomerSubC;
						}
						else
						{
							param.Customer = param.Customer + "," + invoiceList[iloop].CustomerMainC + "_" + invoiceList[iloop].CustomerSubC;
						}
					}
				}
			}

			if (param.Customer != "null")
			{
				var invoiceArr = param.Customer.Split(',');
				int number = 1;
				#region Add invoice data to list
				for (var i = 0; i < invoiceArr.Length; i++)
				{
					var invoiceDetailArr = invoiceArr[i].Split('_');
					var invoiceMainC = invoiceDetailArr[0];
					var invoiceSubC = invoiceDetailArr[1];
					var invoiceData = new CustomerPaymentReportData();
					// get customers who shared a invoice company
					var customerStr = "";
					var customerList = _customerService.GetCustomersByInvoice(invoiceMainC, invoiceSubC);
					for (var aloop = 0; aloop < customerList.Count; aloop++)
					{
						customerStr = customerStr + "," + customerList[aloop].CustomerMainC + "_" + customerList[aloop].CustomerSubC;
					}

					//Get Customer Name
					var customerN = (from p in _customerRepository.GetAllQueryable()
									where p.CustomerMainC == invoiceMainC && p.CustomerSubC == invoiceSubC
									select p.CustomerN).FirstOrDefault();
					if (customerN != null)
					{
						invoiceData.CustomerN = customerN;
					}

					//Get expense before the DateFrom
					var beforeDateFromExpense = (from p in _customerBalanceRepository.GetAllQueryable()
												where customerStr.Contains("," + p.CustomerMainC + "_" + p.CustomerSubC) &
													  p.CustomerBalanceD < param.DateFrom
												group p by new {p.CustomerMainC, p.CustomerSubC} into g
												select new
												{
													//SumAmount = g.Sum(p=>p.Amount) ?? 0,
													//SumTotalExpense = g.Sum(p => p.TotalExpense) ?? 0,
													//SumCustomerSurcharge = g.Sum(p => p.CustomerSurcharge) ?? 0,
													//SumCustomerDiscount = g.Sum(p => p.CustomerDiscount) ?? 0,
													//SumDetainAmount = g.Sum(p => p.DetainAmount) ?? 0,
													SumTotalAmount = g.Sum(p => p.TotalAmount) ?? 0,
													SumTaxAmount = g.Sum(p => p.TaxAmount) ?? 0,
													SumPaymentAmount = g.Sum(p => p.PaymentAmount) ?? 0
												}).ToList();
					decimal openingBalance = 0;
					if (beforeDateFromExpense != null)
					{
						openingBalance = (decimal)(beforeDateFromExpense.Sum(s => s.SumTotalAmount) + beforeDateFromExpense.Sum(s => s.SumTaxAmount) - beforeDateFromExpense.Sum(s => s.SumPaymentAmount));
					}

					var betweenDatefromAndDateto = (from p in _customerBalanceRepository.GetAllQueryable()
													where customerStr.Contains("," + p.CustomerMainC + "_" + p.CustomerSubC) &
														  (p.CustomerBalanceD >= param.DateFrom && p.CustomerBalanceD <= param.DateTo)
													group p by new { p.CustomerMainC, p.CustomerSubC }
														into g
														select new
														{
															//SumAmount = g.Sum(p => p.Amount) ?? 0,
															SumTotalExpense = g.Sum(p => p.TotalExpense) ?? 0,
															//SumCustomerSurcharge = g.Sum(p => p.CustomerSurcharge) ?? 0,
															//SumCustomerDiscount = g.Sum(p => p.CustomerDiscount) ?? 0,
															//SumDetainAmount = g.Sum(p => p.DetainAmount) ?? 0,
															SumTotalAmount = g.Sum(p => p.TotalAmount) ?? 0,
															SumTaxAmount = g.Sum(p => p.TaxAmount) ?? 0,
															SumPaymentAmount = g.Sum(p => p.PaymentAmount) ?? 0
														}).ToList();

					decimal closingBalance = 0;
					decimal ownExpense = 0;
					decimal oweExpense = 0;
					decimal payOnBehalf = 0;
					if (betweenDatefromAndDateto != null)
					{
						oweExpense = (decimal)(betweenDatefromAndDateto.Sum(s => s.SumTotalAmount) + betweenDatefromAndDateto.Sum(s => s.SumTaxAmount) - betweenDatefromAndDateto.Sum(s => s.SumTotalExpense));
						payOnBehalf = (decimal)betweenDatefromAndDateto.Sum(s => s.SumTotalExpense);
						ownExpense = (decimal)betweenDatefromAndDateto.Sum(s => s.SumPaymentAmount);
					} 
					closingBalance = openingBalance + (oweExpense + payOnBehalf - ownExpense);

					invoiceData.OpeningBalance = openingBalance;
					invoiceData.OwnExpense = ownExpense;
					invoiceData.ClosingBalance = closingBalance;
					invoiceData.PayOnBehalf = payOnBehalf;
					invoiceData.OweExpense = oweExpense;
					if (openingBalance > 0 || closingBalance > 0 || ownExpense > 0 ||
						payOnBehalf > 0 || oweExpense > 0)
					{
						invoiceData.No = number;
						number++;
						customerDataList.Add(invoiceData);

						if (param.ReportI == "1")
						{
							var betweenDatefromAndDatetoDetail = (from h in _orderHRepository.GetAllQueryable()
																  join o in _orderDRepository.GetAllQueryable()
																  on new { h.OrderD, h.OrderNo } equals new { o.OrderD, o.OrderNo }
																  where customerStr.Contains("," + h.CustomerMainC + "_" + h.CustomerSubC) &
																		(o.RevenueD >= param.DateFrom && o.RevenueD <= param.DateTo)
																  group new { h, o } by new { h.CustomerMainC, h.CustomerSubC, o.CommodityN, o.ContainerSizeI, o.UnitPrice }
																	  into g
																	  select new
																	  {
																		  CommodityN = g.Key.CommodityN,
																		  ContainerSizeI = g.Key.ContainerSizeI,
																		  UnitPrice = g.Key.UnitPrice,
																		  SumQuantity = g.Count(),
																		  SumNetWeight = g.Sum(p => p.o.NetWeight) ?? 0,
																		  SumTotalExpense = g.Sum(p => p.o.TotalExpense) ?? 0,
																		  SumTotalAmount = g.Sum(p => p.o.TotalAmount) ?? 0,
																		  SumTaxAmount = g.Sum(p => p.o.TaxAmount) ?? 0,
																	  }).ToList();

							if (betweenDatefromAndDatetoDetail != null && betweenDatefromAndDatetoDetail.Count > 0)
							{
								for (int j = 0; j < betweenDatefromAndDatetoDetail.Count; j++)
								{
									oweExpense = (decimal)(betweenDatefromAndDatetoDetail[j].SumTotalAmount + betweenDatefromAndDatetoDetail[j].SumTaxAmount - betweenDatefromAndDatetoDetail[j].SumTotalExpense);
									payOnBehalf = (decimal)betweenDatefromAndDatetoDetail[j].SumTotalExpense;

									var invoiceDataDetail = new CustomerPaymentReportData();
									invoiceDataDetail.CustomerN = customerN;
									invoiceDataDetail.OpeningBalance = 0;
									invoiceDataDetail.OwnExpense = 0;
									invoiceDataDetail.ClosingBalance = 0;
									invoiceDataDetail.No = 0;
									invoiceDataDetail.PayOnBehalf = payOnBehalf;
									invoiceDataDetail.OweExpense = oweExpense;
									invoiceDataDetail.CommodityN = betweenDatefromAndDatetoDetail[j].CommodityN;
									invoiceDataDetail.ContainerSizeI = betweenDatefromAndDatetoDetail[j].ContainerSizeI;
									invoiceDataDetail.ContainerCount = betweenDatefromAndDatetoDetail[j].SumQuantity;
									invoiceDataDetail.UnitPrice = betweenDatefromAndDatetoDetail[j].UnitPrice;
									invoiceDataDetail.NetWeight = betweenDatefromAndDatetoDetail[j].SumNetWeight;

									if ( payOnBehalf > 0 || oweExpense > 0)
										customerDataList.Add(invoiceDataDetail);
								}
							}
						}
					}
					
				}
				#endregion
			}

			return customerDataList;
		}

		//private string GetAllCustomerFormatString()
		//{
		//	var customerListString = "";
		//	var customerList = _customerService.GetInvoices();

		//	if (customerList != null && customerList.Count > 0)
		//	{
				
		//		for (var iloop = 0; iloop < customerList.Count; iloop++)
		//		{
		//			if (iloop == 0)
		//			{
		//				customerListString = customerList[iloop].CustomerMainC + "_" + customerList[iloop].CustomerSubC;
		//			}
		//			else
		//			{
		//				customerListString = customerListString + "," + customerList[iloop].CustomerMainC + "_" + customerList[iloop].CustomerSubC;
		//			}
		//		}

		//		return customerListString;
		//	}
		//	return "";
		//}

		private CustomerPaymentReportData GetCustomerPaymentGeneralData(string invoiceMainC, string invoiceSubC, DateTime dateFrom, DateTime dateTo, string customerStr)
		{
			
			var customerData = new CustomerPaymentReportData();
			customerData.CustomerMainC = invoiceMainC;
			customerData.CustomerSubC = invoiceSubC;
			//Get Customer Name
			var customerN = (from p in _customerRepository.GetAllQueryable()
							 where p.CustomerMainC == invoiceMainC && p.CustomerSubC == invoiceSubC
							 select p.CustomerN).FirstOrDefault();
			if (customerN != null)
			{
				customerData.CustomerN = customerN;
			}

			//Get expense before the DateFrom
			var beforeDateFromExpense = (from p in _customerBalanceRepository.GetAllQueryable()
										 where customerStr.Contains("," + p.CustomerMainC + "_" + p.CustomerSubC) &
											   p.CustomerBalanceD < dateFrom
										 group p by new { p.CustomerMainC, p.CustomerSubC } into g
										 select new
										 {
											 SumTotalAmount = g.Sum(p => p.TotalAmount) ?? 0,
											 SumTaxAmount = g.Sum(p => p.TaxAmount) ?? 0,
											 SumPaymentAmount = g.Sum(p => p.PaymentAmount) ?? 0
										 }).ToList();
			decimal openingBalance = 0;
			if (beforeDateFromExpense != null)
			{
				openingBalance = (decimal)(beforeDateFromExpense.Sum(i => i.SumTotalAmount) + beforeDateFromExpense.Sum(i => i.SumTaxAmount) - beforeDateFromExpense.Sum(i => i.SumPaymentAmount));
			}

			//var betweenDatefromAndDateto = (from p in _customerBalanceRepository.GetAllQueryable()
			//								where p.CustomerMainC == customerMainC & p.CustomerSubC == customerSubC &
			//									  (p.CustomerBalanceD >= dateFrom && p.CustomerBalanceD <= dateTo)
			//								group p by new { p.CustomerMainC, p.CustomerSubC }
			//									into g
			//									select new
			//									{
			//										SumTotalAmount = g.Sum(p => p.TotalAmount) ?? 0,
			//										SumTaxAmount = g.Sum(p => p.TaxAmount) ?? 0,
			//										SumPaymentAmount = g.Sum(p => p.PaymentAmount) ?? 0
			//									}).FirstOrDefault();
			//decimal closingBalance = 0;
			//decimal ownExpense = 0;
			//decimal oweExpense = 0;
			//if (betweenDatefromAndDateto != null)
			//{
			//	oweExpense = (decimal)(betweenDatefromAndDateto.SumTotalAmount + betweenDatefromAndDateto.SumTaxAmount);
			//	ownExpense = (decimal)betweenDatefromAndDateto.SumPaymentAmount;
			//}

			//closingBalance = openingBalance + (oweExpense - ownExpense);

			customerData.OpeningBalance = openingBalance;
			//customerData.ClosingBalance = closingBalance;
			//customerData.OwnExpense = ownExpense;
			//customerData.OweExpense = oweExpense;
			return customerData;
		}

		private List<CustomerPaymentReportData> GetCustomerPaymentProgressReportList(CustomerPaymentReportParam param)
		{
			var customerDataList = new List<CustomerPaymentReportData>();
			if (param.Customer == "null")
			{
				var invoiceList = _customerService.GetInvoices();

				if (invoiceList != null && invoiceList.Count > 0)
				{
					param.Customer = "";
					for (var iloop = 0; iloop < invoiceList.Count; iloop++)
					{
						if (iloop == 0)
						{
							param.Customer = invoiceList[iloop].CustomerMainC + "_" + invoiceList[iloop].CustomerSubC;
						}
						else
						{
							param.Customer = param.Customer + "," + invoiceList[iloop].CustomerMainC + "_" + invoiceList[iloop].CustomerSubC;
						}
					}
				}

			}

			if (param.Customer != "null")
			{
				var invoiceArr = param.Customer.Split(',');

				#region Add invoice data to list
				for (var i = 0; i < invoiceArr.Length; i++)
				{
					var invoiceDetailArr = invoiceArr[i].Split('_');
					var invoiceMainC = invoiceDetailArr[0];
					var invoiceSubC = invoiceDetailArr[1];
					// get customers who shared a invoice company
					var customerStr = "";
					var customerList = _customerService.GetCustomersByInvoice(invoiceMainC, invoiceSubC);
					for (var aloop = 0; aloop < customerList.Count; aloop++)
					{
						customerStr = customerStr + "," + customerList[aloop].CustomerMainC + "_" + customerList[aloop].CustomerSubC;
					}

					var customerData = GetCustomerPaymentGeneralData(invoiceMainC, invoiceSubC, param.DateFrom, param.DateTo, customerStr);
					customerData.OweData = GetCustomerPaymentOweData(invoiceMainC, invoiceSubC, param.DateFrom, param.DateTo, customerStr);
					customerData.OwnData = GetCustomerPaymentOwnData(invoiceMainC, invoiceSubC, param.DateFrom, param.DateTo, customerStr);
					customerDataList.Add(customerData);
				}
				#endregion
			}

			return customerDataList;
		}

		private List<CustomerPaymentDetailReportData> GetCustomerPaymentOweData(string customerMainC, string customerSubC,
			DateTime dateFrom, DateTime dateTo, string customerStr)
		{
			var ownData = (from p in _customerPaymentRepository.GetAllQueryable()
							where customerStr.Contains("," + p.CustomerMainC + "_" + p.CustomerSubC) &
								  (p.CustomerPaymentD >= dateFrom && p.CustomerPaymentD <= dateTo)
							select new CustomerPaymentDetailReportData()
							{
								PaymentY = p.CustomerPaymentD.Year,
								PaymentM = p.CustomerPaymentD.Month,
								Content = p.Description,
								PaymentAmount = p.Amount
							}).ToList();

			if (ownData.Count > 0)
			{
				for (int i = 0; i < ownData.Count; i++)
				{
					ownData[i].PaymentD = new DateTime(ownData[i].PaymentY, ownData[i].PaymentM, 1);
				}
				return ownData;
			}
			return null;
		}

		private List<CustomerPaymentDetailReportData> GetCustomerPaymentOwnData(string customerMainC, string customerSubC,
			DateTime dateFrom, DateTime dateTo, string customerStr)
		{
			//var oweData = (from p in _orderDRepository.GetAllQueryable()
			//				join q in _orderHRepository.GetAllQueryable() on new {p.OrderD, p.OrderNo} equals new {q.OrderD, q.OrderNo} into pq
			//				from q in pq.DefaultIfEmpty()
			//				where q.CustomerMainC == customerMainC & q.CustomerSubC == customerSubC &
			//					  (p.RevenueD >= dateFrom && p.RevenueD <= dateTo)
			//				select new CustomerPaymentDetailReportData()
			//				{
			//					PaymentD = p.RevenueD,
			//					Content = p.ContainerNo,
			//					Amount = p.Amount,
			//					TotalExpense = p.TotalExpense,
			//					CustomerSurcharge = p.CustomerSurcharge,
			//					DetainAmount = p.DetainAmount,
			//					CustomerDiscount = p.CustomerDiscount,
			//					TaxAmount = p.TaxAmount
			//				}).ToList();
			var oweData = (from p in _customerBalanceRepository.GetAllQueryable()
						   where customerStr.Contains("," + p.CustomerMainC + "_" + p.CustomerSubC) &
							   p.CustomerBalanceD >= dateFrom && p.CustomerBalanceD <= dateTo &
							   p.TotalAmount > 0
						   group p by new { p.CustomerBalanceD.Year, p.CustomerBalanceD.Month } into g
						   select new CustomerPaymentDetailReportData()
							{
								PaymentY = g.Key.Year,
								PaymentM = g.Key.Month,
								//Amount = g.Sum(p=> p.Amount),
								TotalAmount = g.Sum(p => p.Amount + p.DetainAmount + p.CustomerSurcharge + p.TaxAmount),
								TotalExpense = g.Sum(p => p.TotalExpense),
								//CustomerSurcharge = g.Sum(p=> p.CustomerSurcharge),
								//DetainAmount = g.Sum(p=> p.DetainAmount),
								CustomerDiscount = g.Sum(p=> p.CustomerDiscount),
								//TaxAmount = g.Sum(p => p.TaxAmount)
							}).ToList();
			if (oweData.Count > 0)
			{
				for (int i = 0; i < oweData.Count; i++)
				{
					oweData[i].PaymentD = new DateTime(oweData[i].PaymentY, oweData[i].PaymentM, 1);
				}
				return oweData;
			}
			
			return null;
		}

		public Stream ExportPdfCustomerPaymentProgress(CustomerPaymentReportParam param, string userName)
		{
			//Get CustomerPayment Data
			var dt = new CustomerPayment.CustomerPaymentDataTable();
			var customerData = GetCustomerPaymentProgressReportList(param);
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
			#region Get text resource for translating
			// get language for report
			var dicLanguage = new Dictionary<string, string>();
			int intLanguage = Utilities.ConvertLanguageToInt(param.Languague);

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																  (con.TextKey == "RPHDCUSTOMERPAYMENTDETAIL" ||
																  con.TextKey == "RPLBLCUSTOMERN" ||
																  con.TextKey == "LBLMONTH" ||
																  con.TextKey == "RPHDCONTENT" ||
																  con.TextKey == "RPHDAMOUNT" ||
																  con.TextKey == "LBLEXPENSEREPORT" ||
																  con.TextKey == "RPHDTOTALSURCHARGE" ||
																  con.TextKey == "RPHDDETAINAMOUNT" ||
																  con.TextKey == "RPHDCUSTOMERDISCOUNT" ||
																  con.TextKey == "RPHDTAXAMOUNT" ||
																  con.TextKey == "RPHDOWEEXPENSE" ||
																  con.TextKey == "RPHDOWNEXPENSE" ||
																  con.TextKey == "RPLBLOPENINGBALANCE" ||
																  con.TextKey == "RPLBLCLOSINGBALANCE" ||
																  con.TextKey == "RPLBLTOTAL"||
																  con.TextKey == "LBLAMOUNTREPORT"
																 )).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}
			#endregion

			if (customerData != null && customerData.Count > 0)
			{
				foreach (var customer in customerData)
				{
					var totalOweData = customer.OweData != null ? customer.OweData.Count : 0;
					var totalOwnData = customer.OwnData != null ? customer.OwnData.Count : 0;
					var totalRow = totalOweData + totalOwnData;
					for (var i = 0; i < totalRow; i++)
					{
						var row = dt.NewRow();
						row["CustomerN"] = customer.CustomerN;
						row["CustomerMainC"] = customer.CustomerMainC;
						row["CustomerSubC"] = customer.CustomerSubC;
						row["OpeningBalance"] = customer.OpeningBalance;
						row["ClosingBalance"] = 0;//customer.ClosingBalance;
						if (i < totalOwnData)
						{
							if (customer.OwnData != null)
							{
								var ownIndex = i;
								row["PaymentStringD"] = Utilities.GetFormatMonthDateReportByLanguage(customer.OwnData[ownIndex].PaymentD, intLanguage);
								row["PaymentD"] = customer.OwnData[ownIndex].PaymentD;
								row["Description"] = customer.OwnData[ownIndex].Content;
								row["Amount"] = customer.OwnData[ownIndex].TotalAmount ?? 0;
								row["TotalExpense"] = customer.OwnData[ownIndex].TotalExpense ?? 0;
								//row["TotalSurcharge"] = customer.OwnData[ownIndex].CustomerSurcharge ?? 0;
								//row["DetainAmount"] = customer.OwnData[ownIndex].DetainAmount ?? 0;
								row["CustomerDiscount"] = customer.OwnData[ownIndex].CustomerDiscount ?? 0;
								//row["TaxAmount"] = customer.OwnData[ownIndex].TaxAmount ?? 0;
								row["PaymentAmount"] = 0;
							}
						}
						else
						{
							if (customer.OweData != null)
							{
								var oweIndex = i - totalOwnData;
								row["PaymentStringD"] = Utilities.GetFormatMonthDateReportByLanguage(customer.OweData[oweIndex].PaymentD, intLanguage);
								row["PaymentD"] = customer.OweData[oweIndex].PaymentD;
								row["Description"] = customer.OweData[oweIndex].Content;
								row["PaymentAmount"] = customer.OweData[oweIndex].PaymentAmount;
							}
						}

						dt.Rows.Add(row);
					}
				}
			}

			var stream = CrystalReport.Service.CustomerPayment.ExportPdf.Exec(dt, intLanguage, param.DateFrom, param.DateTo, dicLanguage,
																				companyName, companyAddress, fileName, user);
			return stream;
		}
		public Stream ExportPdfCustomerPaymentDetail(CustomerPaymentReportParam param, string userName)
		{
			var companyName = "";
			var companyAddress = "";
			var fileName = "";
			//Get CustomerPayment Data
			var dt = new CustomerPaymentGeneral.CustomerPaymentDetailDataTable();
			var customerData = GetCustomerPaymentReportList(param);
			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				//companyTaxCode = basicSetting.TaxCode;
				fileName = basicSetting.Logo;
			}
			//ger employeeName
			var user = GetEmployeeByUserName(userName);

			#region Get text resource for translating
			// get language for report
			var dicLanguage = new Dictionary<string, string>();
			int intLanguage = Utilities.ConvertLanguageToInt(param.Languague);

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																  (con.TextKey == "RPHDCUSTOMERPAYMENT" ||
																  con.TextKey == "RPHDCUSTOMERN" ||
																  con.TextKey == "RPHDOPENINGBALANCE" ||
																  con.TextKey == "RPHDOWNEXPENSE" ||
																  con.TextKey == "RPHDOWEEXPENSE" ||
																  con.TextKey == "RPHDCLOSINGBALANCE" ||
																  con.TextKey == "RPLBLTOTAL" ||
																  con.TextKey == "RPHDAMOUNT" ||
																  con.TextKey == "LBLEXPENSEREPORT" ||

																  con.TextKey == "RPHDCOMMODITY" ||
																  con.TextKey == "LBLEXPENSETYPEREPORT" ||
																  con.TextKey == "RPHDQUANTITYNETWEIGHT" ||
																  con.TextKey == "RPHDUNITPRICE" ||
																  con.TextKey == "RPHDTOLAMOUNT" ||

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
			#endregion

			if (customerData != null && customerData.Count > 0)
			{
				foreach (var customer in customerData)
				{
					var row = dt.NewRow();
					row["No"] = customer.No;
					row["CustomerName"] = customer.CustomerN;
					row["OpeningBalance"] = customer.OpeningBalance;
					row["OweExpense"] = customer.OweExpense;
					row["OwnExpense"] = customer.OwnExpense;
					row["ClosingBalance"] = customer.ClosingBalance;
					row["PayOnBehalf"] = customer.PayOnBehalf;

					row["Commodity"] = customer.CommodityN;
					row["ContainerType"] = customer.ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] : Utilities.GetContainerSizeName(customer.ContainerSizeI);
					row["Quantity"] = customer.ContainerSizeI == "3" ? customer.NetWeight??0: customer.ContainerCount??0;
					row["UnitPrice"] = customer.UnitPrice??0;
					dt.Rows.Add(row);
				}
			}

			var stream = CrystalReport.Service.CustomerPayment.ExportPdf.DetailExec(dt, intLanguage, param.DateFrom, param.DateTo, dicLanguage,
																					companyName, companyAddress, fileName, user);
			return stream;
		}

		public Stream ExportPdfCustomerPricingDetail(CustomerPricingReportParam param)
		{
			int intLanguage;
			//Get CustomerPayment Data
			var dt = new CustomerPricing.CustomerPricingDetailDataTable();
			var customerData = GetCustomerPricingDetailReportList(param);

			#region Get text resource for translating
			// get language for report
			var dicLanguage = new Dictionary<string, string>();
			switch (param.Languague)
			{
				case "vi":
					intLanguage = 1;
					break;
				case "jp":
					intLanguage = 3;
					break;
				default:
					intLanguage = 2;
					break;
			}

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																 (con.TextKey == "RPHDCUSTOMERPRICING" ||
																  con.TextKey == "RPHDCUSTOMERN" ||
																  con.TextKey == "RPHDDEPARTURECP" ||
																  con.TextKey == "RPHDDESTINATIONCP" ||
																  con.TextKey == "RPHDCONTSIZE" ||
																  con.TextKey == "RPHDCONTTYPE" ||
																  con.TextKey == "RPHDCPCATEGORY" ||
																  con.TextKey == "LBLESTIMATEDD" ||
																  con.TextKey == "RPHDEXPENSE" ||
																  con.TextKey == "LBLAMOUNTREPORT" ||
																  con.TextKey == "RPHDCPTOLEXPENSE" ||
																  con.TextKey == "RPHDESTIMATEDPRICE" ||
																  con.TextKey == "RPLBLTOTAL" ||
																  con.TextKey == "CATEGORYI1" ||
																  con.TextKey == "CATEGORYI2" ||
																  con.TextKey == "CATEGORYI3" ||
																  con.TextKey == "CATEGORYI4" ||
																  con.TextKey == "LBLOTHER" ||
																  con.TextKey == "LBLLOAD"
																 )).ToList();

			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}
			#endregion

			if (customerData != null && customerData.Count > 0)
			{
				foreach (var customer in customerData)
				{
					if (customer.ExpenseDetail != null && customer.ExpenseDetail.Any())
					{
						var index = 1;
						foreach (var expense in customer.ExpenseDetail)
						{
							var row = dt.NewRow();
							row["CustomerMainC"] = customer.CustomerMainC;
							row["CustomerSubC"] = customer.CustomerSubC;
							row["CustomerN"] = customer.CustomerN;
							row["Location1N"] = customer.Location1N;
							row["Location2N"] = customer.Location2N;
							row["ContainerSizeN"] = customer.ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] : Utilities.GetContainerSizeName(customer.ContainerSizeI);
							row["ContainerTypeN"] = customer.ContainerTypeN;
							row["EstimatedD"] = Utilities.GetFormatDateReportByLanguage(customer.EstimatedD, intLanguage); 
							row["Expense"] = customer.Expense;
							row["EstimatedPrice"] = customer.EstimatedPrice;
							//row detail
							row["No"] = index;
							row["ExpenseC"] = expense.ExpenseC;
							row["ExpenseN"] = expense.ExpenseN;
							//row["CategoryI"] = expense.CategoryI;
							if (expense.CategoryI == Constants.CATEGORYI1)
							{
								row["CategoryI"] = dicLanguage["CATEGORYI1"];
							} else if (expense.CategoryI == Constants.CATEGORYI2)
							{
								row["CategoryI"] = dicLanguage["CATEGORYI2"];
							} else if (expense.CategoryI == Constants.CATEGORYI3)
							{
								row["CategoryI"] = dicLanguage["CATEGORYI3"];
							} else if (expense.CategoryI == Constants.CATEGORYI4)
							{
								row["CategoryI"] = dicLanguage["CATEGORYI4"];
							} else 
							{
								row["CategoryI"] = dicLanguage["LBLOTHER"];
							}
							row["Amount"] = expense.Amount ?? 0;
							dt.Rows.Add(row);
							index++;
						}
					}
					else
					{
						var row = dt.NewRow();
						row["CustomerMainC"] = customer.CustomerMainC;
						row["CustomerSubC"] = customer.CustomerSubC;
						row["CustomerN"] = customer.CustomerN;
						row["Location1N"] = customer.Location1N;
						row["Location2N"] = customer.Location2N;
						row["ContainerSizeN"] = customer.ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] : Utilities.GetContainerSizeName(customer.ContainerSizeI);
						row["ContainerTypeN"] = customer.ContainerTypeN;
						row["EstimatedD"] = Utilities.GetFormatDateReportByLanguage(customer.EstimatedD, intLanguage); 
						row["Expense"] = customer.Expense;
						row["EstimatedPrice"] = customer.EstimatedPrice;
						dt.Rows.Add(row);
					}
					
				}
			}

			var stream = CrystalReport.Service.CustomerPricing.ExportPdf.DetailExec(dt, intLanguage, param.DateFrom, param.DateTo, dicLanguage);
			return stream;
		}

		private List<CustomerPricingReportData> GetCustomerPricingDetailReportList(CustomerPricingReportParam param)
		{
			var location1List = ConvertStringToList(param.Location1);
			var location2List = ConvertStringToList(param.Location2);
			var contSizeList = ConvertStringToList(param.ContainerSize);
			var contTypeList = ConvertStringToList(param.ContainerType);
			var customerList = "," + param.Customer;
			var customerDataList = (from p in _customerPricingRepository.GetAllQueryable()
									join c in _customerRepository.GetAllQueryable() on new {p.CustomerMainC, p.CustomerSubC} equals new {c.CustomerMainC, c.CustomerSubC}
									into t1 from c in t1.DefaultIfEmpty()
									join l1 in _locationRepository.GetAllQueryable() on p.Location1C equals l1.LocationC
									join l2 in _locationRepository.GetAllQueryable() on p.Location2C equals l2.LocationC
									join t in _containerTypeRepository.GetAllQueryable() on p.ContainerTypeC equals t.ContainerTypeC
									where ( p.EstimatedD >= param.DateFrom && p.EstimatedD <= param.DateTo &&
											(param.Customer == "null" || customerList.Contains("," + p.CustomerMainC + "_" + p.CustomerSubC)) &&
											(param.Location1 == "null" || location1List.Contains(p.Location1C)) &&
											(param.Location2 == "null" || location2List.Contains(p.Location2C)) &&
											(param.ContainerSize == "null" || contSizeList.Contains(p.ContainerSizeI)) &&
											(param.ContainerType == "null" || contTypeList.Contains(p.ContainerTypeC))
											)
									select new CustomerPricingReportData()
									{
										CustomerPricingId = p.CustomerPricingId,
										CustomerMainC = p.CustomerMainC,
										CustomerSubC = p.CustomerSubC,
										CustomerN = c.CustomerN,
										Location1N = l1.LocationN,
										Location2N = l2.LocationN,
										ContainerSizeI = p.ContainerSizeI,
										ContainerTypeN = t.ContainerTypeN,
										EstimatedD = p.EstimatedD,
										Expense = p.TotalExpense,
										EstimatedPrice = p.EstimatedPrice
									}).ToList();

			if (customerDataList.Count > 0 && param.ReportType == "1")
			{
				foreach (var customerData in customerDataList)
				{
					var expenseList = ( from p in _customerPricingDetailRepository.GetAllQueryable()
										where p.CustomerPricingId.Equals(customerData.CustomerPricingId)
										select new CustomerPricingExpenseDetailReportData()
										{
											ExpenseC = p.ExpenseC,
											ExpenseN = p.ExpenseN,
											CategoryI = p.CategoryI,
											Amount = p.Amount
										}
									).OrderBy(i => i.CategoryI).ToList();
					customerData.ExpenseDetail = expenseList;
				}
			}
			return customerDataList;
		}
		public Stream ExportPdfCustomerPricingGeneral(CustomerPricingReportParam param)
		{
			int intLanguage;
			var companyName = "";
			var companyAddress = "";
			var companyTaxCode = "";
			var fileName = "";
			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				companyTaxCode = basicSetting.TaxCode;
				fileName = basicSetting.Logo;
			}

			//Get CustomerPayment Data
			var dt = new CustomerPricing.CustomerPricingGeneralDataTable();
			var customerData = GetCustomerPricingDetailReportList(param);

			#region Get text resource for translating
			// get language for report
			var dicLanguage = new Dictionary<string, string>();
			switch (param.Languague)
			{
				case "vi":
					intLanguage = 1;
					break;
				case "jp":
					intLanguage = 3;
					break;
				default:
					intLanguage = 2;
					break;
			}

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																 (con.TextKey == "RPHDCUSTOMERPRICING" ||
																  con.TextKey == "RPHDCUSTOMERN" ||
																  con.TextKey == "RPHDDEPARTURECP" ||
																  con.TextKey == "RPHDDESTINATIONCP" ||
																  con.TextKey == "RPHDCONTSIZE" ||
																  con.TextKey == "RPHDCONTTYPE" ||
																  con.TextKey == "LBLESTIMATEDD" ||
																  con.TextKey == "RPHDCPTOLEXPENSE" ||
																  con.TextKey == "RPHDESTIMATEDPRICE" ||
																  con.TextKey == "RPLBLTOTAL" ||
																  con.TextKey == "LBLTAXCODE" ||
																  con.TextKey == "LBLLOAD"
																 )).ToList();

			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}
			#endregion

			if (customerData != null && customerData.Count > 0)
			{
				foreach (var customer in customerData)
				{
					var row = dt.NewRow();
					row["CustomerMainC"] = customer.CustomerMainC;
					row["CustomerSubC"] = customer.CustomerSubC;
					row["CustomerN"] = customer.CustomerN;
					row["Location1N"] = customer.Location1N;
					row["Location2N"] = customer.Location2N;
					row["ContainerSizeN"] = customer.ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] : Utilities.GetContainerSizeName(customer.ContainerSizeI);
					row["ContainerTypeN"] = customer.ContainerTypeN;
					row["EstimatedD"] = Utilities.GetFormatDateReportByLanguage(customer.EstimatedD, intLanguage); 
					row["Expense"] = customer.Expense;
					row["EstimatedPrice"] = customer.EstimatedPrice;
					dt.Rows.Add(row);
				}
			}

			var stream = CrystalReport.Service.CustomerPricing.ExportPdf.GeneralExec(dt, intLanguage, param.DateFrom, param.DateTo, dicLanguage, companyName, companyAddress, companyTaxCode, fileName);
			return stream;
		}

		public Stream ExportPdfPartnerPaymentGeneral(PartnerPaymentReportParam param, string userName)
		{
			int intLanguage;
			var companyName = "";
			var companyAddress = "";
			var fileName = "";
			//Get PartnerPayment Data
			var dt = new PartnerPaymentGeneral.PartnerPaymentGeneralDataTable();
			var partnerData = GetPartnerPaymentReportList(param);
			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				//companyTaxCode = basicSetting.TaxCode;
				fileName = basicSetting.Logo;
			}
			//ger employeeName
			var user = GetEmployeeByUserName(userName);

			#region Get text resource for translating
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
																  (con.TextKey == "RPHDPARTNERPAYMENT" ||
																  con.TextKey == "RPHDPARTNERN" ||
																  con.TextKey == "RPHDOPENINGBALANCE" ||
																  con.TextKey == "RPHDOWNEXPENSE" ||
																  con.TextKey == "RPHDOWEEXPENSE" ||
																  con.TextKey == "RPHDCLOSINGBALANCE" ||
																  con.TextKey == "RPLBLTOTAL" ||
																  con.TextKey == "RPHDAMOUNT" ||
																  con.TextKey == "LBLEXPENSEREPORT" ||

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

			if (partnerData != null && partnerData.Count > 0)
			{
				foreach (var partner in partnerData)
				{
					var row = dt.NewRow();
					row["PartnerName"] = partner.PartnerN;
					row["OpeningBalance"] = partner.OpeningBalance;
					row["OweExpense"] = partner.OweExpense;
					row["OwnExpense"] = partner.PayExpense;
					row["ClosingBalance"] = partner.ClosingBalance;
					row["PartnerExpense"] = partner.PartnerExpense;
					dt.Rows.Add(row);
				}
			}

			var stream = CrystalReport.Service.PartnerPayment.ExportPdf.GeneralExec(dt, intLanguage, param.DateFrom, param.DateTo, dicLanguage,
																					companyName, companyAddress, fileName, user);
			return stream;
		}
		private List<PartnerPaymentReportData> GetPartnerPaymentReportList(PartnerPaymentReportParam param)
		{
			var partnerDataList = new List<PartnerPaymentReportData>();
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

			if (param.Partner != "null")
			{
				var partnerArr = param.Partner.Split(',');
				int number = 1;
				#region Add partner data to list
				for (var i = 0; i < partnerArr.Length; i++)
				{
					var partnerDetailArr = partnerArr[i].Split('_');
					var partnerMainC = partnerDetailArr[0];
					var partnerSubC = partnerDetailArr[1];
					var partnerData = new PartnerPaymentReportData();
					// get partners who shared a invoice company
					var partnerStr = "";
					var partnerList = _partnerService.GetPartnersByInvoice(partnerMainC, partnerSubC);
					for (var aloop = 0; aloop < partnerList.Count; aloop++)
					{
						partnerStr = partnerStr + "," + partnerList[aloop].PartnerMainC + "_" + partnerList[aloop].PartnerSubC;
					}
					//Get Partner Name
					var partnerN = (from p in _partnerRepository.GetAllQueryable()
										 where p.PartnerMainC == partnerMainC && p.PartnerSubC == partnerSubC
										 select p.PartnerN).FirstOrDefault();
					if (partnerN != null)
					{
						partnerData.PartnerN = partnerN;
					}

					//Get expense before the DateFrom
					var beforeDateFromExpense = (from p in _partnerBalanceRepository.GetAllQueryable()
												 where partnerStr.Contains("," + p.PartnerMainC + "_" + p.PartnerSubC) &
													   p.PartnerBalanceD < param.DateFrom
												 group p by new { p.PartnerMainC, p.PartnerSubC } into g
												 select new
												 {
													 SumTotalAmount = g.Sum(p => p.TotalAmount) ?? 0,
													 SumTaxAmount = g.Sum(p => p.TaxAmount) ?? 0,
													 SumPaymentAmount = g.Sum(p => p.PaymentAmount) ?? 0
												 }).ToList();
					decimal openingBalance = 0;
					if (beforeDateFromExpense != null)
					{
						openingBalance = (decimal)(beforeDateFromExpense.Sum(s => s.SumTotalAmount) + beforeDateFromExpense.Sum(s => s.SumTaxAmount) - beforeDateFromExpense.Sum(s => s.SumPaymentAmount));
					}

					var betweenDatefromAndDateto = (from p in _partnerBalanceRepository.GetAllQueryable()
													where partnerStr.Contains("," + p.PartnerMainC + "_" + p.PartnerSubC) &
														  (p.PartnerBalanceD >= param.DateFrom && p.PartnerBalanceD <= param.DateTo)
													group p by new { p.PartnerMainC, p.PartnerSubC }
														into g
														select new
														{
															SumTotalAmount = g.Sum(p => p.TotalAmount) ?? 0,
															SumTaxAmount = g.Sum(p => p.TaxAmount) ?? 0,
															SumPaymentAmount = g.Sum(p => p.PaymentAmount) ?? 0,
															SumPartnerExpense = g.Sum(p => p.PartnerExpense) ?? 0
														}).ToList();
					decimal closingBalance = 0;
					decimal ownExpense = 0;
					decimal oweExpense = 0;
					decimal partnerExpense = 0;
					if (betweenDatefromAndDateto != null)
					{
						partnerExpense = (decimal)betweenDatefromAndDateto.Sum(s => s.SumPartnerExpense);
						oweExpense = (decimal)(betweenDatefromAndDateto.Sum(s => s.SumTotalAmount) + betweenDatefromAndDateto.Sum(s => s.SumTaxAmount)) - partnerExpense;
						ownExpense = (decimal)betweenDatefromAndDateto.Sum(s => s.SumPaymentAmount);
					}

					closingBalance = openingBalance + (oweExpense + partnerExpense - ownExpense);

					partnerData.OpeningBalance = openingBalance;
					partnerData.ClosingBalance = closingBalance;
					partnerData.PayExpense = ownExpense;
					partnerData.OweExpense = oweExpense;
					partnerData.PartnerExpense = partnerExpense;
					if (openingBalance > 0 || closingBalance > 0 || ownExpense > 0 ||
						partnerExpense > 0 || oweExpense > 0)
					{
						partnerData.No = number;
						number++;
						partnerDataList.Add(partnerData);

						if (param.ReportI == "1")
						{
							var betweenDatefromAndDatetoDetail = (from d in _dispatchRepository.GetAllQueryable()
																			  join o in _orderDRepository.GetAllQueryable()
																			  on new { d.OrderD, d.OrderNo, d.DetailNo } equals new { o.OrderD, o.OrderNo, o.DetailNo }
																			  where partnerStr.Contains("," + d.PartnerMainC + "_" + d.PartnerSubC) &
																					(o.RevenueD >= param.DateFrom && o.RevenueD <= param.DateTo)
																			  group new { d, o } by new { d.PartnerMainC, d.PartnerSubC, o.CommodityN, o.ContainerSizeI, d.PartnerFee }
																				  into g
																				  select new
																				  {
																					  CommodityN = g.Key.CommodityN,
																					  ContainerSizeI = g.Key.ContainerSizeI,
																					  UnitPrice = g.Key.PartnerFee,
																					  SumQuantity = g.Count(),
																					  SumNetWeight = g.Sum(p => p.o.NetWeight) ?? 0,
																					  SumPartnerExpense = g.Sum(p => p.d.PartnerExpense) ?? 0,
																					  SumTotalAmount = g.Sum(p => p.d.PartnerFee) ?? 0 + g.Sum(p => p.d.PartnerSurcharge) ?? 0,
																					  SumTaxAmount = g.Sum(p => p.d.PartnerTaxAmount) ?? 0,
																					  PartnerSurcharge = g.Sum(p => p.o.PartnerSurcharge) ?? 0
																				  }).ToList();

							if (betweenDatefromAndDatetoDetail != null && betweenDatefromAndDatetoDetail.Count > 0)
							{
								for (int j = 0; j < betweenDatefromAndDatetoDetail.Count; j++)
								{
									partnerExpense = (decimal)betweenDatefromAndDatetoDetail[j].SumPartnerExpense;
									oweExpense = (decimal)(betweenDatefromAndDatetoDetail[j].SumTotalAmount) - partnerExpense;

									var invoiceDataDetail = new PartnerPaymentReportData();
									invoiceDataDetail.PartnerN = partnerN;
									invoiceDataDetail.OpeningBalance = 0;
									invoiceDataDetail.PayExpense = 0;
									invoiceDataDetail.ClosingBalance = 0;
									invoiceDataDetail.No = 0;
									invoiceDataDetail.PartnerExpense = partnerExpense;
									invoiceDataDetail.OweExpense = oweExpense + partnerExpense + betweenDatefromAndDatetoDetail[j].SumTaxAmount + betweenDatefromAndDatetoDetail[j].PartnerSurcharge;
									invoiceDataDetail.CommodityN = betweenDatefromAndDatetoDetail[j].CommodityN;
									invoiceDataDetail.ContainerSizeI = betweenDatefromAndDatetoDetail[j].ContainerSizeI;
									invoiceDataDetail.ContainerCount = betweenDatefromAndDatetoDetail[j].SumQuantity;
									invoiceDataDetail.UnitPrice = betweenDatefromAndDatetoDetail[j].UnitPrice;
									invoiceDataDetail.NetWeight = betweenDatefromAndDatetoDetail[j].SumNetWeight;
									if (partnerExpense > 0 || oweExpense > 0)
										partnerDataList.Add(invoiceDataDetail);
								}
							}
						}
					}
				}
				#endregion
			}

			return partnerDataList;
		}
		public Stream ExportPdfPartnerPaymentProgress(PartnerPaymentReportParam param, string userName)
		{
			int intLanguage;
			//Get PartnerPayment Data
			var dt = new PartnerPaymentDetail.PartnerPaymentDetailDataTable();
			var partnerData = GetPartnerPaymentProgressReportList(param);
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
			#region Get text resource for translating
			// get language for report
			var dicLanguage = new Dictionary<string, string>();
			//CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
			if (param.Languague == "vi")
			{
				intLanguage = 1;
			}
			else if (param.Languague == "jp")
			{
				intLanguage = 3;
				//cul = CultureInfo.GetCultureInfo("ja-JP");
			}
			else
			{
				intLanguage = 2;
				//cul = CultureInfo.GetCultureInfo("en-US");
			}

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																  (con.TextKey == "RPHDPARTNERPAYMENTDETAIL" ||
																  con.TextKey == "RPLBLPARTNERN" ||
																  con.TextKey == "LBLMONTH" ||
																  con.TextKey == "RPHDCONTENT" ||
																  con.TextKey == "RPHDAMOUNT" ||
																  con.TextKey == "RPHDTOTALEXPENSE" ||
																  con.TextKey == "RPHDTOTALSURCHARGE" ||
																  con.TextKey == "RPHDCUSTOMERDISCOUNT" ||
																  con.TextKey == "RPHDTAXAMOUNT" ||
																  con.TextKey == "RPHDOWEEXPENSE" ||
																  con.TextKey == "RPHDOWNEXPENSE" ||
																  con.TextKey == "RPLBLOPENINGBALANCE" ||
																  con.TextKey == "RPLBLCLOSINGBALANCE" ||
																  con.TextKey == "RPLBLTOTAL" ||
																  con.TextKey == "LBLAMOUNTREPORT" ||

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

			if (partnerData != null && partnerData.Count > 0)
			{
				foreach (var partner in partnerData)
				{
					var totalOweData = partner.OweData != null ? partner.OweData.Count : 0;
					var totalOwnData = partner.PayData != null ? partner.PayData.Count : 0;
					var totalRow = totalOweData + totalOwnData;
					for (var i = 0; i < totalRow; i++)
					{
						var row = dt.NewRow();
						row["PartnerN"] = partner.PartnerN;
						row["PartnerMainC"] = partner.PartnerMainC;
						row["PartnerSubC"] = partner.PartnerSubC;
						row["OpeningBalance"] = partner.OpeningBalance;
						row["ClosingBalance"] = 0;//partner.ClosingBalance;
						if (i < totalOwnData)
						{
							if (partner.PayData != null)
							{
								var ownIndex = i;
								row["PaymentD"] = Utilities.GetFormatMonthDateReportByLanguage(partner.PayData[ownIndex].PaymentD, intLanguage);
								row["Description"] = partner.PayData[ownIndex].Content;
								row["PartnerFee"] = partner.PayData[ownIndex].PartnerFee ?? 0;
								row["PartnerExpense"] = partner.PayData[ownIndex].PartnerExpense ?? 0;
								//row["PartnerSurcharge"] = partner.PayData[ownIndex].PartnerSurcharge ?? 0;
								//row["PartnerDiscount"] = partner.PayData[ownIndex].PartnerDiscount ?? 0;
								//row["PartnerTaxAmount"] = partner.PayData[ownIndex].PartnerTaxAmount ?? 0;
								row["PaymentAmount"] = 0;
							}
						}
						else
						{
							if (partner.OweData != null)
							{
								var oweIndex = i - totalOwnData;
								row["PaymentD"] = Utilities.GetFormatMonthDateReportByLanguage(partner.OweData[oweIndex].PaymentD, intLanguage);
								row["Description"] = partner.OweData[oweIndex].Content;
								row["PaymentAmount"] = partner.OweData[oweIndex].PaymentAmount;
							}
						}
						dt.Rows.Add(row);
					}
				}
			}

			var stream = CrystalReport.Service.PartnerPayment.ExportPdf.Exec(dt, intLanguage, param.DateFrom, param.DateTo, dicLanguage,
																				companyName, companyAddress, fileName, user);
			return stream;
		}
		private List<PartnerPaymentReportData> GetPartnerPaymentProgressReportList(PartnerPaymentReportParam param)
		{
			var partnerDataList = new List<PartnerPaymentReportData>();
			if (param.Partner == "null")
			{
				param.Partner = GetAllPartnerFormatString();
			}

			if (param.Partner != "null")
			{
				var partnerArr = param.Partner.Split(',');

				#region Add partner data to list
				for (var i = 0; i < partnerArr.Length; i++)
				{
					var partnerDetailArr = partnerArr[i].Split('_');
					var partnerMainC = partnerDetailArr[0];
					var partnerSubC = partnerDetailArr[1];
					// get partners who shared a invoice company
					var partnerStr = "";
					var partnerList = _partnerService.GetPartnersByInvoice(partnerMainC, partnerSubC);
					for (var aloop = 0; aloop < partnerList.Count; aloop++)
					{
						partnerStr = partnerStr + "," + partnerList[aloop].PartnerMainC + "_" + partnerList[aloop].PartnerSubC;
					}

					var partnerData = GetPartnerPaymentGeneralData(partnerMainC, partnerSubC, param.DateFrom, param.DateTo, partnerStr);
					partnerData.OweData = GetPartnerPaymentOweData(partnerMainC, partnerSubC, param.DateFrom, param.DateTo, partnerStr);
					partnerData.PayData = GetPartnerPaymentPayData(partnerMainC, partnerSubC, param.DateFrom, param.DateTo, partnerStr);
					partnerDataList.Add(partnerData);
				}
				#endregion
			}

			return partnerDataList;
		}
		private string GetAllPartnerFormatString()
        {
            var partnerListString = "";
            var partnerList = _partnerService.GetInvoices();

            if (partnerList != null && partnerList.Count > 0)
            {

                for (var iloop = 0; iloop < partnerList.Count; iloop++)
                {
                    if (iloop == 0)
                    {
                        partnerListString = partnerList[iloop].PartnerMainC + "_" + partnerList[iloop].PartnerSubC;
                    }
                    else
                    {
                        partnerListString = partnerListString + "," + partnerList[iloop].PartnerMainC + "_" + partnerList[iloop].PartnerSubC;
                    }
                }

                return partnerListString;
            }
            return "";
        }
		private PartnerPaymentReportData GetPartnerPaymentGeneralData(string partnerMainC, string partnerSubC, DateTime dateFrom, DateTime dateTo, string partnerStr)
		{

			var partnerData = new PartnerPaymentReportData();
			partnerData.PartnerMainC = partnerMainC;
			partnerData.PartnerSubC = partnerSubC;
			//Get Partner Name
			var partnerN = (from p in _partnerRepository.GetAllQueryable()
							where p.PartnerMainC == partnerMainC && p.PartnerSubC == partnerSubC
							select p.PartnerN).FirstOrDefault();
			if (partnerN != null)
			{
				partnerData.PartnerN = partnerN;
			}

			//Get expense before the DateFrom
			var beforeDateFromExpense = (from p in _partnerBalanceRepository.GetAllQueryable()
										 where partnerStr.Contains("," + p.PartnerMainC + "_" + p.PartnerSubC) &
											   p.PartnerBalanceD < dateFrom
										 group p by new { p.PartnerMainC, p.PartnerSubC } into g
										 select new
										 {
											 SumTotalAmount = g.Sum(p => p.TotalAmount) ?? 0,
											 SumTaxAmount = g.Sum(p => p.TaxAmount) ?? 0,
											 SumPaymentAmount = g.Sum(p => p.PaymentAmount) ?? 0
										 }).ToList();
			decimal openingBalance = 0;
			if (beforeDateFromExpense != null)
			{
				openingBalance = (decimal)(beforeDateFromExpense.Sum(s=>s.SumTotalAmount) + beforeDateFromExpense.Sum(s=>s.SumTaxAmount) - beforeDateFromExpense.Sum(s=>s.SumPaymentAmount));
			}

			//var betweenDatefromAndDateto = (from p in _partnerBalanceRepository.GetAllQueryable()
			//								where p.PartnerMainC == partnerMainC & p.PartnerSubC == partnerSubC &
			//									  (p.PartnerBalanceD >= dateFrom && p.PartnerBalanceD <= dateTo)
			//								group p by new { p.PartnerMainC, p.PartnerSubC }
			//									into g
			//									select new
			//									{
			//										SumTotalAmount = g.Sum(p => p.TotalAmount) ?? 0,
			//										SumTaxAmount = g.Sum(p => p.TaxAmount) ?? 0,
			//										SumPaymentAmount = g.Sum(p => p.PaymentAmount) ?? 0
			//									}).FirstOrDefault();
			//decimal closingBalance = 0;
			//decimal payExpense = 0;
			//decimal oweExpense = 0;
			//if (betweenDatefromAndDateto != null)
			//{
			//	oweExpense = (decimal)(betweenDatefromAndDateto.SumTotalAmount + betweenDatefromAndDateto.SumTaxAmount);
			//	payExpense = (decimal)betweenDatefromAndDateto.SumPaymentAmount;
			//}

			//closingBalance = openingBalance + (oweExpense - payExpense);

			partnerData.OpeningBalance = openingBalance;
			//partnerData.ClosingBalance = closingBalance;
			//partnerData.PayExpense = payExpense;
			//partnerData.OweExpense = oweExpense;
			return partnerData;
		}
		private List<PartnerPaymentDetailReportData> GetPartnerPaymentOweData(string partnerMainC, string partnerSubC,
			DateTime dateFrom, DateTime dateTo, string partnerStr)
		{
			var ownData = (from p in _partnerPaymentRepository.GetAllQueryable()
						   where partnerStr.Contains("," + p.PartnerMainC + "_" + p.PartnerSubC) &
								 (p.PartnerPaymentD >= dateFrom && p.PartnerPaymentD <= dateTo)
						   select new PartnerPaymentDetailReportData()
						   {
							   PaymentY = p.PartnerPaymentD.Year,
							   PaymentM = p.PartnerPaymentD.Month,
							   Content = p.Description,
							   PaymentAmount = p.Amount
						   }).ToList();

			if (ownData.Count > 0)
			{
				for (int i = 0; i < ownData.Count; i++)
				{
					ownData[i].PaymentD = new DateTime(ownData[i].PaymentY, ownData[i].PaymentM, 1);
				}
				return ownData;
			}
			return null;
		}
		private List<PartnerPaymentDetailReportData> GetPartnerPaymentPayData(string partnerMainC, string partnerSubC,
			DateTime dateFrom, DateTime dateTo, string partnerStr)
		{
			//var oweData = (from d in _dispatchRepository.GetAllQueryable()
			//			   join o in _orderDRepository.GetAllQueryable() on new { d.OrderD, d.OrderNo, d.DetailNo } equals new { o.OrderD, o.OrderNo, o.DetailNo }
			//			   where d.PartnerMainC == partnerMainC & d.PartnerSubC == partnerSubC &
			//					 (d.InvoiceD >= dateFrom && d.InvoiceD <= dateTo)
			//			   select new PartnerPaymentDetailReportData()
			//			   {
			//				   PaymentD = d.InvoiceD,
			//				   Content = o.ContainerNo,
			//				   PartnerFee = d.PartnerFee,
			//				   PartnerExpense = d.PartnerExpense,
			//				   PartnerSurcharge = d.PartnerSurcharge,
			//				   PartnerDiscount = d.PartnerDiscount,
			//				   PartnerTaxAmount = d.PartnerTaxAmount
			//			   }).ToList();

			var oweData = (from p in _partnerBalanceRepository.GetAllQueryable()
						   where partnerStr.Contains("," + p.PartnerMainC + "_" + p.PartnerSubC) &
							   p.PartnerBalanceD >= dateFrom && p.PartnerBalanceD <= dateTo &
							   p.TotalAmount > 0
						   group p by new { p.PartnerBalanceD.Year, p.PartnerBalanceD.Month } into g
						   select new PartnerPaymentDetailReportData()
						   {
							   PaymentY = g.Key.Year,
							   PaymentM = g.Key.Month,
							   PartnerFee = g.Sum(p => p.PartnerFee + p.PartnerSurcharge + p.TaxAmount),
							   PartnerExpense = g.Sum(p => p.PartnerExpense),
							   //PartnerSurcharge = g.Sum(p => p.PartnerSurcharge),
							   //PartnerDiscount = g.Sum(p => p.PartnerDiscount),
							   //PartnerTaxAmount = g.Sum(p => p.TaxAmount)
						   }).ToList();

			if (oweData.Count > 0)
			{
				for (int i = 0; i < oweData.Count; i++)
				{
					oweData[i].PaymentD = new DateTime(oweData[i].PaymentY, oweData[i].PaymentM, 1);
				}
				return oweData;
			}

			return null;
		}
		public Stream ExportPdfPartnerPaymentDetail(PartnerPaymentReportParam param, string userName)
		{
			var companyName = "";
			var companyAddress = "";
			var fileName = "";
			//Get PartnerPayment Data
			var dt = new PartnerPaymentGeneral.PartnerPaymentDetailDataTable();
			var partnerData = GetPartnerPaymentReportList(param);
			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				//companyTaxCode = basicSetting.TaxCode;
				fileName = basicSetting.Logo;
			}
			//ger employeeName
			var user = GetEmployeeByUserName(userName);

			#region Get text resource for translating
			// get language for report
			var dicLanguage = new Dictionary<string, string>();
			int intLanguage = Utilities.ConvertLanguageToInt(param.Languague);

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																  (con.TextKey == "RPHDPARTNERPAYMENT" ||
																  con.TextKey == "RPHDPARTNERN" ||
																  con.TextKey == "RPHDOPENINGBALANCE" ||
																  con.TextKey == "RPHDOWNEXPENSE" ||
																  con.TextKey == "RPHDOWEEXPENSE" ||
																  con.TextKey == "RPHDCLOSINGBALANCE" ||
																  con.TextKey == "RPLBLTOTAL" ||
																  con.TextKey == "RPHDAMOUNT" ||
																  con.TextKey == "LBLEXPENSEREPORT" ||

																  con.TextKey == "RPHDCOMMODITY" ||
																  con.TextKey == "LBLEXPENSETYPEREPORT" ||
																  con.TextKey == "RPHDQUANTITYNETWEIGHT" ||
																  con.TextKey == "RPHDUNITPRICE" ||
																  con.TextKey == "RPHDTOLAMOUNT" ||

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
			#endregion

			if (partnerData != null && partnerData.Count > 0)
			{
				foreach (var partner in partnerData)
				{
					var row = dt.NewRow();
					row["No"] = partner.No;
					row["PartnerName"] = partner.PartnerN;
					row["OpeningBalance"] = partner.OpeningBalance;
					row["OweExpense"] = partner.OweExpense;
					row["OwnExpense"] = partner.PayExpense;
					row["ClosingBalance"] = partner.ClosingBalance;
					row["PartnerExpense"] = partner.PartnerExpense;

					row["Commodity"] = partner.CommodityN;
					row["ContainerType"] = partner.ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] : Utilities.GetContainerSizeName(partner.ContainerSizeI);
					row["Quantity"] = partner.ContainerSizeI == "3" ? partner.NetWeight ?? 0 : partner.ContainerCount ?? 0;
					row["UnitPrice"] = partner.UnitPrice ?? 0;
					dt.Rows.Add(row);
				}
			}

			var stream = CrystalReport.Service.PartnerPayment.ExportPdf.DetailExec(dt, intLanguage, param.DateFrom, param.DateTo, dicLanguage,
																					companyName, companyAddress, fileName, user);
			return stream;
		}

		public Stream ExportPdfPartnerBalance(PartnerExpenseReportParam param)
		{
			Stream stream;
			DataRow row;
			PartnerBalance.PartnerBalanceDataTable dt = new PartnerBalance.PartnerBalanceDataTable();
			Dictionary<string, string> dicLanguage = new Dictionary<string, string>();
			int intLanguage;

			// get data
			List<PartnerBalanceReportData> data = GetPartnerBalanceList(param);

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
																 (con.ScreenID == 14 || con.ScreenID == 99) &&
																 (con.TextKey == "LBLTRANSPORTFEE" ||
																  con.TextKey == "LBLPARTNERFEE" ||
																  con.TextKey == "LBLPROFIT" ||
																  con.TextKey == "LBLEXPENSE" ||
																  con.TextKey == "LBLPARTNEREXPENSE" ||
																  con.TextKey == "LBLPROFIT/REVENUE" ||
																  con.TextKey == "LBLPARTNERSURCHARGE" ||
																  con.TextKey == "LBLPARTNERDISCOUNT" ||
																  con.TextKey == "RPLBLTOTAL" ||
																  con.TextKey == "RPHDREVENUE" ||
																  con.TextKey == "RPHDPROFIT" ||
																  con.TextKey == "RPHDPARTNERN" ||
																  con.TextKey == "TLTPARTNERBALANCEREPORT" ||
																  con.TextKey == "LBLEXPENSEITEM" ||
																  con.TextKey == "LBLAMOUNTREPORT" ||
																  con.TextKey == "RPHDEXPENSE"
																 )).ToList();
			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}
			#endregion

			#region convertdata
			if (data != null && data.Count > 0)
			{
				for (int i = 0; i < data.Count; i++)
				{
					var partnerMainC = data[i].PartnerMainC;
					var partnerSubC = data[i].PartnerSubC;
					var customerExpenseList = data[0].CustomerExpenseList.Where(x => x.PartnerMainC == partnerMainC &&
																					 x.PartnerSubC == partnerSubC &&
																					 x.Value > 0)
																		 .OrderBy("Name").ToList();
					var partnerExpenseList = data[0].PartnerExpenseList.Where(x => x.PartnerMainC == partnerMainC &&
																					x.PartnerSubC == partnerSubC &&
																					x.Value > 0)
																		.OrderBy("Name").ToList();
					var partnerSurchargeList = data[0].PartnerSurchargeList.Where(x => x.PartnerMainC == partnerMainC &&
																						x.PartnerSubC == partnerSubC &&
																						x.Value > 0)
																			.OrderBy("Name").ToList();

					int customerExpenseCount = customerExpenseList.Count();
					int partnerExpenseCount = partnerExpenseList.Count();
					int partnerSurchargeCount = partnerSurchargeList.Count();

					int totalRow = Math.Max(customerExpenseCount + 2, partnerExpenseCount + partnerSurchargeCount + 4);

					for (int rowindex = 0; rowindex < totalRow; rowindex++)
					{
						decimal transportFee = 0;
						decimal customerExpense = 0;
						decimal partnerFee = 0;
						decimal partnerExpense = 0;
						decimal partnerSurcharge = 0;
						decimal partnerDiscount = 0;
						decimal profit;

						row = dt.NewRow();
						row["Key"] = partnerMainC + "_" + partnerSubC;
						row["PartnerN"] = data[i].PartnerN;

						// set money
						transportFee = data[i].TransportFee != null ? (decimal) data[i].TransportFee : 0;
						customerExpense = data[i].CustomerExpense != null ? (decimal)data[i].CustomerExpense : 0;
						partnerFee = data[i].PartnerFee != null ? (decimal)data[i].PartnerFee : 0;
						partnerExpense = data[i].PartnerExpense != null ? (decimal)data[i].PartnerExpense : 0;
						partnerSurcharge = data[i].PartnerSurcharge != null ? (decimal)data[i].PartnerSurcharge : 0;
						partnerDiscount = data[i].PartnerDiscount != null ? (decimal)data[i].PartnerDiscount : 0;

						row["TotalIncome"] = (transportFee + customerExpense).ToString("#,###", cul.NumberFormat);
						row["TotalExpense"] = (partnerFee + partnerExpense + partnerSurcharge - partnerDiscount).ToString("#,###", cul.NumberFormat);
						profit = transportFee + customerExpense - partnerFee - partnerExpense - partnerSurcharge + partnerDiscount;

						if (rowindex == 0)
						{
							row["IncomeN"] = "<b>" + dicLanguage["LBLTRANSPORTFEE"] + "</b>";
							row["IncomeAmount"] = transportFee.ToString("#,###", cul.NumberFormat);

							row["ExpenseN"] = "<b>" + dicLanguage["LBLPARTNERFEE"] + "</b>";
							row["ExpenseAmount"] = partnerFee.ToString("#,###", cul.NumberFormat);

							row["ProfitN"] = "<b>" + dicLanguage["LBLPROFIT"] + "</b>";
							row["ProfitAmount"] = profit.ToString("#,###", cul.NumberFormat); ;
						}
						else if (rowindex == 1)
						{
							row["IncomeN"] = "<b>" + dicLanguage["LBLEXPENSE"] + "</b>";
							row["IncomeAmount"] = customerExpense.ToString("#,###", cul.NumberFormat);

							row["ExpenseN"] = "<b>" + dicLanguage["LBLPARTNEREXPENSE"] + "</b>";
							row["ExpenseAmount"] = partnerExpense.ToString("#,###", cul.NumberFormat);

							row["ProfitN"] = "<b>" + dicLanguage["LBLPROFIT/REVENUE"] + "</b>";
							if (transportFee + customerExpense != 0)
							{
								row["ProfitAmount"] = Utilities.CalByMethodRounding(100 * profit / (transportFee + customerExpense), "0") + "%";
							}
						}
						else
						{
							int index = rowindex - 2;
							if (index < customerExpenseCount)
							{
								row["IncomeN"] = customerExpenseList[index].Name;
								row["IncomeAmount"] = customerExpenseList[index].Value != null ? ((decimal)customerExpenseList[index].Value).ToString("#,###", cul.NumberFormat) : null;
							}

							if (index < partnerExpenseCount)
							{
								row["ExpenseN"] = partnerExpenseList[index].Name;
								row["ExpenseAmount"] = partnerExpenseList[index].Value != null ? ((decimal)partnerExpenseList[index].Value).ToString("#,###", cul.NumberFormat) : null;
							}
							else if (index == partnerExpenseCount)
							{
								row["ExpenseN"] = "<b>" + dicLanguage["LBLPARTNERSURCHARGE"] + "</b>";
								row["ExpenseAmount"] = data[i].PartnerSurcharge != null ? ((decimal)data[i].PartnerSurcharge).ToString("#,###", cul.NumberFormat) : null;
							}
							else if ((index - partnerExpenseCount - 1) < partnerSurchargeCount)
							{
								int index2 = index - partnerExpenseCount - 1;
								row["ExpenseN"] = partnerSurchargeList[index2].Name;
								row["ExpenseAmount"] = partnerSurchargeList[index2].Value != null ? ((decimal)partnerSurchargeList[index2].Value).ToString("#,###", cul.NumberFormat) : null;
							}
							else if ((index - partnerExpenseCount - 1) == partnerSurchargeCount)
							{
								row["ExpenseN"] = "<b>" + dicLanguage["LBLPARTNERDISCOUNT"] + "</b>";
								row["ExpenseAmount"] = data[i].PartnerDiscount != null ? ((decimal)data[i].PartnerDiscount).ToString("#,###", cul.NumberFormat) : null;
							}
						}
						dt.Rows.Add(row);
					}
				}
			}
			#endregion

			// set month and year
			var monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
			if (intLanguage == 3)
			{
				monthYear = param.TransportM.Value.Year + "年" + param.TransportM.Value.Month + "月";
			}

			stream = CrystalReport.Service.PartnerBalance.ExportPdf.Exec(dt, intLanguage, monthYear, dicLanguage);
			return stream;
		}
		private List<PartnerBalanceReportData> GetPartnerBalanceList(PartnerExpenseReportParam param)
		{
			var result = new List<PartnerBalanceReportData>();

			// Lấy danh sách partner
			var month = param.TransportM.Value.Month;
			var year = param.TransportM.Value.Year;
			// get settlement info
			var invoiceInfo = _invoiceInfoService.GetLimitStartAndEndInvoiceDSelf(month, year);
			var startDate = invoiceInfo.StartDate.Date;
			var endDate = invoiceInfo.EndDate.Date;

			if (param.Partner == "null")
			{
				var invoiceList = _partnerService.GetInvoices();

				if (invoiceList != null && invoiceList.Count > 0)
				{
					param.Partner = "";
					for (var iloop = 0; iloop < invoiceList.Count; iloop++)
					{
						if (iloop == 0)
						{
							param.Partner = invoiceList[iloop].PartnerMainC + "_" + invoiceList[iloop].PartnerSubC;
						}
						else
						{
							param.Partner = param.Partner + "," + invoiceList[iloop].PartnerMainC + "_" + invoiceList[iloop].PartnerSubC;
						}
					}
				}
			}

			if (param.Partner != "null")
			{
				var partnerArr = (param.Partner).Split(new string[] { "," }, StringSplitOptions.None);

				var customerExpenseList = new List<PartnerBalanceReportExpense>();
				var partnerExpenseList = new List<PartnerBalanceReportExpense>();
				var partnerSurchargeList = new List<PartnerBalanceReportExpense>();

				// xu ly tung ben thue xe
				for (var iloop = 0; iloop < partnerArr.Length; iloop++)
				{
					var arr = (partnerArr[iloop]).Split(new string[] { "_" }, StringSplitOptions.None);
					var invoiceMainC = arr[0];
					var invoiceSubC = arr[1];

					// get invoice name
					var invoiceName = _partnerService.GetPartnerShortNameOrFullName(invoiceMainC, invoiceSubC);

					// get partners who shared a invoice company
					var partnerStr = "";
					var partnerList = _partnerService.GetPartnersByInvoice(invoiceMainC, invoiceSubC);
					for (var aloop = 0; aloop < partnerList.Count; aloop++)
					{
						partnerStr = partnerStr + "," + partnerList[aloop].PartnerMainC + "_" + partnerList[aloop].PartnerSubC;
					}

					// get data for 1 invoice
					#region getdata
					var data = (from a in _dispatchRepository.GetAllQueryable()
								join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo }
									equals new { b.OrderD, b.OrderNo, b.DetailNo } into t1
								from b in t1.DefaultIfEmpty()
								where (partnerStr.Contains("," + a.PartnerMainC + "_" + a.PartnerSubC) &
									   (a.InvoiceD >= startDate & a.InvoiceD <= endDate)
									)
								select new PartnerBalanceReportData()
								{
									OrderD = a.OrderD,
									OrderNo = a.OrderNo,
									DetailNo = a.DetailNo,
									DispatchNo = a.DispatchNo,
									PartnerMainC = invoiceMainC,
									PartnerSubC = invoiceSubC,
									PartnerN = invoiceName,
									TransportFee = a.TransportFee,
									CustomerExpense = a.Expense,
									PartnerFee = a.PartnerFee,
									PartnerExpense = a.PartnerExpense,
									PartnerSurcharge = a.PartnerSurcharge,
									PartnerDiscount = a.PartnerDiscount,
								}).AsQueryable();

					var dataList = data.ToList();

					// lay danh sach chi tiet
					for (var jloop = 0; jloop < dataList.Count(); jloop++)
					{
						var orderD = dataList[jloop].OrderD;
						var orderNo = dataList[jloop].OrderNo;
						var detailNo = dataList[jloop].DetailNo;
						var dispatchNo = dataList[jloop].DispatchNo;

						#region CÁC KHOẢN THU
						customerExpenseList.AddRange(
							(from a in _expenseRepository.GetAllQueryable()
							 join b in _expenseDetailRepository.GetAllQueryable() on a.ExpenseC equals b.ExpenseC into t1
							 from b in t1.DefaultIfEmpty()
							 where (b.OrderD == orderD &&
									b.OrderNo == orderNo &&
									b.DetailNo == detailNo &&
									b.DispatchNo == dispatchNo &&
									(b.IsRequested == "1")
								 )
							 select new PartnerBalanceReportExpense()
								{
									PartnerMainC = invoiceMainC,
									PartnerSubC = invoiceSubC,
									Name = a.ExpenseN,
									Value = b.Amount
								}).ToList()
							);
						#endregion

						#region CAC KHOAN CHI
						// Lấy chi phí chi
						partnerExpenseList.AddRange(
							(from a in _expenseRepository.GetAllQueryable()
							 join b in _expenseDetailRepository.GetAllQueryable() on a.ExpenseC equals b.ExpenseC into t1
							 from b in t1.DefaultIfEmpty()
							 where (b.OrderD == orderD &&
									b.OrderNo == orderNo &&
									b.DetailNo == detailNo &&
									b.DispatchNo == dispatchNo &&
									b.IsPayable == "1"
								 )
							 select
								 new PartnerBalanceReportExpense()
								 {
									 PartnerMainC = invoiceMainC,
									 PartnerSubC = invoiceSubC,
									 Name = a.ExpenseN,
									 Value = b.Amount
								 }).ToList()
						);

						partnerSurchargeList.AddRange(
							(from a in _expenseRepository.GetAllQueryable()
							 join b in _surchargeDetailRepository.GetAllQueryable() on a.ExpenseC equals b.SurchargeC into t1
							 from b in t1.DefaultIfEmpty()
							 where (b.OrderD == orderD &&
									b.OrderNo == orderNo &&
									b.DetailNo == detailNo &&
									b.DispatchNo == dispatchNo
								 )
							 select
								 new PartnerBalanceReportExpense()
								 {
									 PartnerMainC = invoiceMainC,
									 PartnerSubC = invoiceSubC,
									 Name = a.ExpenseN,
									 Value = b.Amount
								 }).ToList()
						);

						#endregion
					}

					if (dataList.Count > 0)
					{
						result.AddRange(dataList);
					}
					#endregion
				}

				//sum data
				#region sumdata
				if (result.Any())
				{
					result = (from b in result
							  group b by new { b.PartnerMainC, b.PartnerSubC, b.PartnerN } into c
							  select new PartnerBalanceReportData()
							  {
								PartnerMainC = c.Key.PartnerMainC,
								PartnerSubC = c.Key.PartnerSubC,
								PartnerN = c.Key.PartnerN,
								TransportFee = c.Sum(b => b.TransportFee),
								CustomerExpense = c.Sum(b => b.CustomerExpense),
								PartnerFee = c.Sum(b => b.PartnerFee),
								PartnerExpense = c.Sum(b => b.PartnerExpense),
								PartnerSurcharge = c.Sum(b => b.PartnerSurcharge),
								PartnerDiscount = c.Sum(b => b.PartnerDiscount),
							  }).ToList();

					result[0].CustomerExpenseList = (from b in customerExpenseList
													group b by new { b.PartnerMainC, b.PartnerSubC, b.Name }
													into c
													select new PartnerBalanceReportExpense()
													{
														PartnerMainC = c.Key.PartnerMainC,
														PartnerSubC = c.Key.PartnerSubC,
														Name = c.Key.Name,
														Value = c.Sum(b => b.Value),
													}).ToList();
					result[0].PartnerExpenseList = (from b in partnerExpenseList
													group b by new { b.PartnerMainC, b.PartnerSubC, b.Name }
													into c
													select new PartnerBalanceReportExpense()
													{
														PartnerMainC = c.Key.PartnerMainC,
														PartnerSubC = c.Key.PartnerSubC,
														Name = c.Key.Name,
														Value = c.Sum(b => b.Value),
													}).ToList();
					result[0].PartnerSurchargeList = (from b in partnerSurchargeList
													  group b by new { b.PartnerMainC, b.PartnerSubC, b.Name }
													  into c
													  select new PartnerBalanceReportExpense()
													  {
														PartnerMainC = c.Key.PartnerMainC,
														PartnerSubC = c.Key.PartnerSubC,
														Name = c.Key.Name,
														Value = c.Sum(b => b.Value),
													  }).ToList();
				}

				#endregion
			}
			return result;
		}

		public Stream ExportPdfCombinedRevenue(CombinedRevenueReportParam param, string userName)
		{
			//Get CustomerPayment Data
			var reportData = new List<CombinedExpenseReportData>();
			var month = param.TransportM.Month;
			var year = param.TransportM.Year;

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
			#region Get text resource for translating
			// get language for report
			var dicLanguage = new Dictionary<string, string>();
			int intLanguage = Utilities.ConvertLanguageToInt(param.Languague);

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																  (con.TextKey == "MNUCOMBINEDREVENUEREPORT" ||
																  con.TextKey == "LBLMONTH" ||
																  con.TextKey == "LBLQUARTER" ||
																  con.TextKey == "LBLYEAR" ||
																  con.TextKey == "LBLCUSTOMERREPORT" ||
																  con.TextKey == "LBLREPORTITRUCK" ||
																  con.TextKey == "LBLREPORTIAREA" ||
																  con.TextKey == "LBLDEPARTMENTREPORT" ||
																  con.TextKey == "LBLREPORTIEMPLOYEE" ||
																  con.TextKey == "RPLLBLTRANSPORTCOUNT" ||
																  con.TextKey == "LBLNETWEIGHT" ||
																  con.TextKey == "LBLNETWEIGHTSHORT" ||
																  con.TextKey == "LBLTON" ||
																  con.TextKey == "RPLLBLREVENUE" ||
																  con.TextKey == "RPLLBLTOTALREVENUE" ||
																  con.TextKey == "RPLLBLYEARONYEAR" ||
																  con.TextKey == "RPLLBLMONTHONMONTH" ||
																  con.TextKey == "RPLBLTOTAL" ||
																  con.TextKey == "LBLUNIT" ||

																  con.TextKey == "RPFTPRINTTIME" ||
																  con.TextKey == "RPFTPRINTBY" ||
																  con.TextKey == "RPFTPAGE" ||
																  con.TextKey == "RPFTCREATOR" ||

																  con.TextKey == "LBLMONTH1FIXEDEXPENSE" ||
																  con.TextKey == "LBLMONTH2FIXEDEXPENSE" ||
																  con.TextKey == "LBLMONTH3FIXEDEXPENSE" ||
																  con.TextKey == "LBLMONTH4FIXEDEXPENSE" ||
																  con.TextKey == "LBLMONTH5FIXEDEXPENSE" ||
																  con.TextKey == "LBLMONTH6FIXEDEXPENSE" ||
																  con.TextKey == "LBLMONTH7FIXEDEXPENSE" ||
																  con.TextKey == "LBLMONTH8FIXEDEXPENSE" ||
																  con.TextKey == "LBLMONTH9FIXEDEXPENSE" ||
																  con.TextKey == "LBLMONTH10FIXEDEXPENSE" ||
																  con.TextKey == "LBLMONTH11FIXEDEXPENSE" ||
																  con.TextKey == "LBLMONTH12FIXEDEXPENSE"


																 )).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}
			#endregion

			var objectN = "";
			switch (param.ObjectType)
			{
				case "C":
					reportData = GetCompanyRevenueReportList(param, month, year);
					objectN = dicLanguage["LBLCUSTOMERREPORT"];
					break;
				case "T":
					reportData = GetTruckRevenueReportList(param, month, year);
					objectN = dicLanguage["LBLREPORTITRUCK"];
					break;
				case "A":
					reportData = GetAreaRevenueReportList(param, month, year);
					objectN = dicLanguage["LBLREPORTIAREA"];
					break;
				case "D":
					reportData = GetDepartmentRevenueReportList(param, month, year);
					objectN = dicLanguage["LBLDEPARTMENTREPORT"];
					break;
				case "E":
					reportData = GetEmployeeRevenueReportList(param, month, year);
					objectN = dicLanguage["LBLREPORTIEMPLOYEE"];
					break;

			}

			if (param.ReportTimeI == "M")
			{
				var dt = new CombinedRevenue.CombinedRevenueDataTable();
				if (reportData != null && reportData.Count > 0)
				{
					foreach (var data in reportData)
					{
                        if (data.Revenue > 0)
					    {
					        var row = dt.NewRow();
						    row["ObjectN"] = data.ObjectN;
						    row["Month"] = data.Month;
						    row["TransportCount"] = data.TransportCount;
						    row["TotalWeight"] = data.TotalWeight;
						    row["Revenue"] = data.Revenue; //Math.Round(data.Revenue / 1000000, 3);
						    row["LastYearTransportCount"] = data.LastYearTransportCount;
						    row["LastYearRevenue"] = data.LastYearRevenue; //Math.Round(data.LastYearRevenue / 1000000, 3);

						    dt.Rows.Add(row);
					    }
					}
				}
				return CrystalReport.Service.CombinedRevenue.ExportPdf.Exec(dt, intLanguage, param.ReportTimeI, param.TransportM, objectN, dicLanguage,
																			companyName, companyAddress, fileName, user);
			} else if (param.ReportTimeI == "Q")
			{
				var dt = new CombinedRevenue.CombinedRevenueQuarterDataTable();
				if (reportData != null && reportData.Count > 0)
				{
					var quarter = (from d in reportData
								   group d by new { d.ObjectN } into g
								   select new
								   {
									   ObjectN = g.Key.ObjectN,
									   Q1TransportCount = g.Where(s => s.Month == 1 || s.Month == 2 || s.Month == 3).Sum(s => s.TransportCount),
									   Q1TotalWeight = g.Where(s => s.Month == 1 || s.Month == 2 || s.Month == 3).Sum(s => s.TotalWeight),
									   Q1Revenue = g.Where(s => s.Month == 1 || s.Month == 2 || s.Month == 3).Sum(s => s.Revenue),
									   Q1LastYearRevenue = g.Where(s => s.Month == 1 || s.Month == 2 || s.Month == 3).Sum(s => s.LastYearRevenue),

									   Q2TransportCount = g.Where(s => s.Month == 4 || s.Month == 5 || s.Month == 6).Sum(s => s.TransportCount),
									   Q2TotalWeight = g.Where(s => s.Month == 4 || s.Month == 5 || s.Month == 6).Sum(s => s.TotalWeight),
									   Q2Revenue = g.Where(s => s.Month == 4 || s.Month == 5 || s.Month == 6).Sum(s => s.Revenue),
									   Q2LastYearRevenue = g.Where(s => s.Month == 4 || s.Month == 5 || s.Month == 6).Sum(s => s.LastYearRevenue),

									   Q3TransportCount = g.Where(s => s.Month == 7 || s.Month == 8 || s.Month == 9).Sum(s => s.TransportCount),
									   Q3TotalWeight = g.Where(s => s.Month == 7 || s.Month == 8 || s.Month == 9).Sum(s => s.TotalWeight),
									   Q3Revenue = g.Where(s => s.Month == 7 || s.Month == 8 || s.Month == 9).Sum(s => s.Revenue),
									   Q3LastYearRevenue = g.Where(s => s.Month == 7 || s.Month == 8 || s.Month == 9).Sum(s => s.LastYearRevenue),

									   Q4TransportCount = g.Where(s => s.Month == 10 || s.Month == 11 || s.Month == 12).Sum(s => s.TransportCount),
									   Q4TotalWeight = g.Where(s => s.Month == 10 || s.Month == 11 || s.Month == 12).Sum(s => s.TotalWeight),
									   Q4Revenue = g.Where(s => s.Month == 10 || s.Month == 11 || s.Month == 12).Sum(s => s.Revenue),
									   Q4LastYearRevenue = g.Where(s => s.Month == 10 || s.Month == 11 || s.Month == 12).Sum(s => s.LastYearRevenue),
								   }).ToList();

					foreach (var data in quarter)
					{
						var row = dt.NewRow();
						row["ObjectN"] = data.ObjectN;
						row["Q1TransportCount"] = data.Q1TransportCount;
						row["Q1TotalWeight"] = data.Q1TotalWeight;
						row["Q1Revenue"] = data.Q1Revenue;// Math.Round(data.Q1Revenue / 1000000, 3);
						row["Q1LastYearRevenue"] = data.Q1LastYearRevenue;// Math.Round(data.Q1LastYearRevenue / 1000000, 3);

						row["Q2TransportCount"] = data.Q2TransportCount;
						row["Q2TotalWeight"] = data.Q2TotalWeight;
						row["Q2Revenue"] = data.Q2Revenue; // Math.Round(data.Q2Revenue / 1000000, 3);
						row["Q2LastYearRevenue"] = data.Q2LastYearRevenue; // Math.Round(data.Q2LastYearRevenue / 1000000, 3);

						row["Q3TransportCount"] = data.Q3TransportCount;
						row["Q3TotalWeight"] = data.Q3TotalWeight;
						row["Q3Revenue"] = data.Q3Revenue; // Math.Round(data.Q3Revenue / 1000000, 3);
						row["Q3LastYearRevenue"] = data.Q3LastYearRevenue; // Math.Round(data.Q3LastYearRevenue / 1000000, 3);

						row["Q4TransportCount"] = data.Q4TransportCount;
						row["Q4TotalWeight"] = data.Q4TotalWeight;
						row["Q4Revenue"] = data.Q4Revenue; // Math.Round(data.Q4Revenue / 1000000, 3);
						row["Q4LastYearRevenue"] = data.Q4LastYearRevenue; // Math.Round(data.Q4LastYearRevenue / 1000000, 3);
						dt.Rows.Add(row);
					}
				}
				return CrystalReport.Service.CombinedRevenue.ExportPdf.ExecQuarter(dt, intLanguage, param.ReportTimeI, param.TransportM, objectN, dicLanguage,
																					companyName, companyAddress, fileName, user);
			}
			else
			{
				var dt = new CombinedRevenue.CombinedRevenueYearDataTable();
				if (reportData != null && reportData.Count > 0)
				{
					var yearData = (from d in reportData
									group d by new { d.ObjectN } into g
									select new
									{
										ObjectN = g.Key.ObjectN,
										M1Revenue = g.Where(s => s.Month == 1).Sum(s => s.Revenue),
										M2Revenue = g.Where(s => s.Month == 2).Sum(s => s.Revenue),
										M3Revenue = g.Where(s => s.Month == 3).Sum(s => s.Revenue),
										M4Revenue = g.Where(s => s.Month == 4).Sum(s => s.Revenue),
										M5Revenue = g.Where(s => s.Month == 5).Sum(s => s.Revenue),
										M6Revenue = g.Where(s => s.Month == 6).Sum(s => s.Revenue),
										M7Revenue = g.Where(s => s.Month == 7).Sum(s => s.Revenue),
										M8Revenue = g.Where(s => s.Month == 8).Sum(s => s.Revenue),
										M9Revenue = g.Where(s => s.Month == 9).Sum(s => s.Revenue),
										M10Revenue = g.Where(s => s.Month == 10).Sum(s => s.Revenue),
										M11Revenue = g.Where(s => s.Month == 11).Sum(s => s.Revenue),
										M12Revenue = g.Where(s => s.Month == 12).Sum(s => s.Revenue),
										LastYearRevenue = g.Sum(s => s.LastYearRevenue),
									}).ToList();

					foreach (var data in yearData)
					{
						var row = dt.NewRow();
						row["ObjectN"] = data.ObjectN;
						row["M1Revenue"] = Math.Round(data.M1Revenue / 1000000, 3);
						row["M2Revenue"] = Math.Round(data.M2Revenue / 1000000, 3);
						row["M3Revenue"] = Math.Round(data.M3Revenue / 1000000, 3);
						row["M4Revenue"] = Math.Round(data.M4Revenue / 1000000, 3);
						row["M5Revenue"] = Math.Round(data.M5Revenue / 1000000, 3);
						row["M6Revenue"] = Math.Round(data.M6Revenue / 1000000, 3);
						row["M7Revenue"] = Math.Round(data.M7Revenue / 1000000, 3);
						row["M8Revenue"] = Math.Round(data.M8Revenue / 1000000, 3);
						row["M9Revenue"] = Math.Round(data.M9Revenue / 1000000, 3);
						row["M10Revenue"] = Math.Round(data.M10Revenue / 1000000, 3);
						row["M11Revenue"] = Math.Round(data.M11Revenue / 1000000, 3);
						row["M12Revenue"] = Math.Round(data.M12Revenue / 1000000, 3);
						row["LastYearRevenue"] = Math.Round(data.LastYearRevenue / 1000000, 3);
						dt.Rows.Add(row);
					}
				}
				return CrystalReport.Service.CombinedRevenue.ExportPdf.ExecYear(dt, intLanguage, param.ReportTimeI, param.TransportM, objectN, dicLanguage,
																					companyName, companyAddress, fileName, user);
			}
		}

		private List<CombinedExpenseReportData> GetCompanyRevenueReportList(CombinedRevenueReportParam param, int month, int year)
		{
			var result = new List<CombinedExpenseReportData>();

			if (param.ReportType.Equals("I")) {
				if (param.ObjectList == "null")
				{
					var customerList = _customerService.GetInvoices();

					if (customerList != null && customerList.Count > 0)
					{
						param.ObjectList = "";
						for (var iloop = 0; iloop < customerList.Count; iloop++)
						{
							if (iloop == 0)
							{
								param.ObjectList = customerList[iloop].CustomerMainC + "_" + customerList[iloop].CustomerSubC;
							}
							else
							{
								param.ObjectList = param.ObjectList + "," + customerList[iloop].CustomerMainC + "_" + customerList[iloop].CustomerSubC;
							}
						}
					}
				}

				if (param.ObjectList != "null")
				{
					var customerArr = (param.ObjectList).Split(new string[] { "," }, StringSplitOptions.None);
					for (var iloop = 0; iloop < customerArr.Length; iloop++)
					{
						var arr = (customerArr[iloop]).Split(new string[] { "_" }, StringSplitOptions.None);
						var invoiceMainC = arr[0];
						var invoiceSubC = arr[1];
						// get customers who shared a invoice company
						var customerList = _customerService.GetCustomersByInvoice(invoiceMainC, invoiceSubC);
						var customerStr = "";
						for (var aloop = 0; aloop < customerList.Count; aloop++)
						{
							customerStr = customerStr + "," + customerList[aloop].CustomerMainC + "_" + customerList[aloop].CustomerSubC;
						}
						var invoiceN = _customerService.GetCustomersByMainCodeSubCode(invoiceMainC, invoiceSubC).Customer.CustomerShortN;

						if (param.ReportTimeI == "M")
						{
							var res = GetCompanyRevenueReportListByMonth(invoiceMainC, invoiceSubC, invoiceN, customerStr, month, year, param.ReportTimeI);
							result.AddRange(res);
						}
						else
						{
							for (int mloop = 1; mloop <= 12; mloop++)
							{
								var res = GetCompanyRevenueReportListByMonth(invoiceMainC, invoiceSubC, invoiceN, customerStr, mloop, year, param.ReportTimeI);
								result.AddRange(res);
							}
						}
					}
				}
			} else { //(param.ReportType.Equals("C")
				if (param.ObjectList == "null")
				{
					var customerList = _customerService.GetCustomersForReport();

					if (customerList != null && customerList.Count() > 0)
					{
						param.ObjectList = "";
						for (var iloop = 0; iloop < customerList.Count(); iloop++)
						{
							if (iloop == 0)
							{
								param.ObjectList = customerList.ElementAt(iloop).CustomerMainC + "_" + customerList.ElementAt(iloop).CustomerSubC;
							}
							else
							{
								param.ObjectList = param.ObjectList + "," + customerList.ElementAt(iloop).CustomerMainC + "_" + customerList.ElementAt(iloop).CustomerSubC;
							}
						}
					}
				}

				if (param.ObjectList != "null")
				{
					var customerArr = (param.ObjectList).Split(new string[] { "," }, StringSplitOptions.None);
					for (var iloop = 0; iloop < customerArr.Length; iloop++)
					{
						var arr = (customerArr[iloop]).Split(new string[] { "_" }, StringSplitOptions.None);
						var customerMainC = arr[0];
						var customerSubC = arr[1];
						var customerStr = "," + customerArr[iloop];

						var customerN = _customerService.GetCustomersByMainCodeSubCode(customerMainC, customerSubC).Customer.CustomerShortN;

						if (param.ReportTimeI == "M")
						{
							var res = GetCompanyRevenueReportListByMonth(customerMainC, customerSubC, customerN, customerStr, month, year, param.ReportTimeI);
							result.AddRange(res);
						}
						else
						{
							for (int mloop = 1; mloop <= 12; mloop++)
							{
								var res = GetCompanyRevenueReportListByMonth(customerMainC, customerSubC, customerN, customerStr, mloop, year, param.ReportTimeI);
								result.AddRange(res);
							}
						}
					}
				}
			}
			return result;
		}

		private List<CombinedExpenseReportData> GetCompanyRevenueReportListByMonth(string invoiceMainC, string invoiceSubC, string invoiceN, string customerStr, int month, int year, string reportTimeI)
		{
			var result = new List<CombinedExpenseReportData>();

			var invoiceInfo = _invoiceInfoService.GetLimitStartAndEndInvoiceD(invoiceMainC, invoiceSubC, month, year);
			var startDate = invoiceInfo.StartDate.Date;
			var endDate = invoiceInfo.EndDate.Date;
			var lastYearStartDate = startDate.AddYears(-1);
			var lastYearEndDate = endDate.AddYears(-1);
			if (reportTimeI == "M")
			{
				lastYearStartDate = startDate.AddMonths(-1);
				lastYearEndDate = endDate.AddMonths(-1);
			}

			// get data
			var data = (from a in _orderHRepository.GetAllQueryable()
						join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
						 equals new { b.OrderD, b.OrderNo } into t1
						from b in t1.DefaultIfEmpty()
						where (customerStr.Contains("," + a.CustomerMainC + "_" + a.CustomerSubC) &
							   (b.RevenueD >= startDate & b.RevenueD <= endDate))
						group new { a, b } by new { a.CustomerMainC, b.ContainerSizeI } into g
						select new CombinedExpenseReportData()
						{
							TransportCount = g.Count(),
							TotalWeight = g.Key.ContainerSizeI == "3" ? g.Sum(f => f.b.NetWeight ?? 0) : 0,
							//Revenue = g.Sum(f => f.b.TotalAmount ?? 0) + g.Sum(f => f.b.TaxAmount ?? 0)
							Revenue = g.Sum(f => f.b.Amount ?? 0) + g.Sum(f => f.b.DetainAmount ?? 0)
						}).ToList();

			var lastYearData = (from a in _orderHRepository.GetAllQueryable()
								join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
								 equals new { b.OrderD, b.OrderNo } into t1
								from b in t1.DefaultIfEmpty()
								where (customerStr.Contains("," + a.CustomerMainC + "_" + a.CustomerSubC) &
									   (b.RevenueD >= lastYearStartDate & b.RevenueD <= lastYearEndDate)
									 )
								group new { a, b } by new { a.CustomerMainC } into g
								select new CombinedExpenseReportData()
								{
									LastYearTransportCount = g.Count(),
									LastYearRevenue = g.Sum(f => f.b.Amount ?? 0) + g.Sum(f => f.b.CustomerSurcharge ?? 0)
								}).ToList();
			var row = new CombinedExpenseReportData()
			{
				ObjectN = invoiceN,
				Month = month,
				TransportCount = data.Sum(c => c.TransportCount),
				TotalWeight = data.Sum(c => c.TotalWeight),
				Revenue = data.Sum(c => c.Revenue),
				LastYearTransportCount = lastYearData.Sum(c => c.LastYearTransportCount),
				LastYearRevenue = lastYearData.Sum(c => c.LastYearRevenue)
			};
			if(row.TransportCount > 0 || row.TotalWeight > 0 || row.Revenue > 0 || row.LastYearRevenue > 0)
				result.Add(row);
			var orderH =
				_orderHRepository.Query(p => customerStr.Contains("," + p.CustomerMainC + "_" + p.CustomerSubC)).ToList();
			decimal? totalsurcharge = 0;
			int flagcountSurcharge = 0;
			if (orderH.Count > 0)
			{
				for (var o = 0; o < orderH.Count; o++)
				{
					DateTime oDH = orderH[o].OrderD;
					string oNH = orderH[o].OrderNo;
					var orderD = _orderDRepository.Query(p => p.OrderD == oDH && p.OrderNo == oNH).ToList();
					if (orderD.Count > 0)
					{
						for (var d = 0; d < orderD.Count; d++)
						{
							DateTime oDD = orderD[d].OrderD;
							string oND = orderD[d].OrderNo;
							int dND = orderD[d].DetailNo;
							var surcharge =
								_surchargeDetailRepository.Query(p => p.OrderD == oDD && p.OrderNo == oND && p.DetailNo == dND).ToList();
							if (surcharge.Count > 0)
							{
								for (var s = 0; s < surcharge.Count; s++)
								{
									totalsurcharge = totalsurcharge + surcharge[s].Amount;
									flagcountSurcharge++;
								}
							}
						}
					}
				}
			}
			if (result.Count > 0 && totalsurcharge > 0)
			{
				result[0].Revenue = result[0].Revenue + (decimal) totalsurcharge;
			}
			return result;
		}

		private List<CombinedExpenseReportData> GetTruckRevenueReportList(CombinedRevenueReportParam param, int month, int year)
		{
			var truckList = "," + param.ObjectList + ",";
			var data = (from d in _dispatchRepository.GetAllQueryable()
						join b in _orderDRepository.GetAllQueryable() on new { d.OrderD, d.OrderNo, d.DetailNo }
						 equals new { b.OrderD, b.OrderNo, b.DetailNo } into t1
						from b in t1.DefaultIfEmpty()
						where (param.ObjectList == "null" || truckList.Contains("," + d.TruckC + ",")) &&
								(d.DispatchI == "0") &&
							   (param.ReportTimeI != "M" || d.TransportD.Value.Month == month) &&
							   (d.TransportD.Value.Year == year)
						group new { b, d } by new { d.TruckC, d.TransportD.Value.Month } into g
						select new CombinedExpenseReportData()
						{
							ObjectN = g.Key.TruckC,
							Month = g.Key.Month,
							TotalWeight = g.Where(f => f.b.ContainerSizeI == "3").DefaultIfEmpty().Sum(f => f.b.NetWeight ?? 0),
							TransportCount = g.Count(),
							Revenue = g.Sum(f => f.d.TransportFee ?? 0)
						}).ToList();
			DateTime lastYear = param.ReportTimeI == "M" ? lastYear = param.TransportM.AddMonths(-1) : param.TransportM.AddYears(-1);
			var lastYearData = (from d in _dispatchRepository.GetAllQueryable()
									join b in _orderDRepository.GetAllQueryable() on new { d.OrderD, d.OrderNo, d.DetailNo }
									equals new { b.OrderD, b.OrderNo, b.DetailNo } into t1
									from b in t1.DefaultIfEmpty()
									where (param.ObjectList == "null" || param.ObjectList.Contains("," + d.TruckC)) &&
											(d.DispatchI == "0") &&
											(param.ReportTimeI != "M" || d.TransportD.Value.Month == lastYear.Month) &&
											(d.TransportD.Value.Year == lastYear.Year)
									group d by new { d.TruckC, d.TransportD.Value.Month } into g
									select new CombinedExpenseReportData()
									{
										ObjectN = g.Key.TruckC,
										Month = g.Key.Month,
										LastYearTransportCount = g.Count(),
										LastYearRevenue = g.Sum(f => f.TransportFee ?? 0)
									}).ToList();

			var concat = data.Concat(lastYearData).ToList();

			var result = (from c in concat
					  join t in _truckRepository.GetAllQueryable()
					  on c.ObjectN equals t.TruckC
					  group new { c, t } by new { t.RegisteredNo, c.Month } into g
					  select new CombinedExpenseReportData()
					  {
						  ObjectN = g.Key.RegisteredNo,
						  Month = g.Key.Month,
						  TransportCount = g.Sum(s => s.c.TransportCount),
						  TotalWeight = g.Sum(s => s.c.TotalWeight),
						  Revenue = g.Sum(s => s.c.Revenue),
						  LastYearTransportCount = g.Sum(s => s.c.LastYearTransportCount),
						  LastYearRevenue = g.Sum(s => s.c.LastYearRevenue),
					  }).ToList();
			var finalresult = result.Where(p => p.TransportCount > 0 && p.Revenue > 0).ToList();
			return finalresult;
		}

		private List<CombinedExpenseReportData> GetAreaRevenueReportList(CombinedRevenueReportParam param, int month, int year)
		{
			var result = new List<CombinedExpenseReportData>();
			if (param.ObjectList == "null")
			{
				var areaList = _locationService.GetAreas();

				if (areaList != null && areaList.Count() > 0)
				{
					param.ObjectList = "";
					for (var iloop = 0; iloop < areaList.Count(); iloop++)
					{
						if (iloop == 0)
						{
							param.ObjectList = areaList.ElementAt(iloop).LocationC;
						}
						else
						{
							param.ObjectList = param.ObjectList + "," + areaList.ElementAt(iloop).LocationC;
						}
					}
				}
			}

			var locationStr = "";
			if (param.ObjectList != "null")
			{
				var areaArr = (param.ObjectList).Split(new string[] { "," }, StringSplitOptions.None);
				for (var iloop = 0; iloop < areaArr.Length; iloop++)
				{
					var areaN = _locationService.GetLocationByCode(areaArr[iloop]).Location.LocationN;
					var locationList = _locationService.GetLocationsByArea(areaArr[iloop]);
					for (var aloop = 0; aloop < locationList.Count; aloop++)
					{
						locationStr = locationStr + "," + locationList[aloop].LocationC;
					}
					locationStr += ",";
					// get data
					var data = (from a in _orderHRepository.GetAllQueryable()
								join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
								 equals new { b.OrderD, b.OrderNo } into t1
								from b in t1.DefaultIfEmpty()
								where locationStr.Contains("," + a.StopoverPlaceC) &&
									  ((a.OrderTypeI == "" && locationStr.Contains("," + a.LoadingPlaceC)) ||
									   (a.OrderTypeI == "" && locationStr.Contains("," + a.DischargePlaceC))) &&
									  (param.ReportTimeI != "M" || b.RevenueD.Value.Month == month) &&
									  (b.RevenueD.Value.Year == year)
								group b by new { b.RevenueD.Value.Month, b.ContainerSizeI } into g
								select new CombinedExpenseReportData()
								{
									Month = g.Key.Month,
									TransportCount = g.Count(),
									TotalWeight = g.Key.ContainerSizeI == "3" ? g.Sum(f => f.NetWeight ?? 0) : 0,
									Revenue = g.Sum(f => f.Amount ?? 0) + g.Sum(f => f.CustomerSurcharge ?? 0)
								}).ToList();

					DateTime lastYear = param.ReportTimeI == "M" ? param.TransportM.AddMonths(-1) : param.TransportM.AddYears(-1);
					var lastYearData = (from a in _orderHRepository.GetAllQueryable()
										join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
										 equals new { b.OrderD, b.OrderNo } into t1
										from b in t1.DefaultIfEmpty()
										where locationStr.Contains("," + a.StopoverPlaceC) &&
											  ((a.OrderTypeI == "" && locationStr.Contains("," + a.LoadingPlaceC)) ||
											   (a.OrderTypeI == "" && locationStr.Contains("," + a.DischargePlaceC))) &&
											  (param.ReportTimeI != "M" || b.RevenueD.Value.Month == lastYear.Month) &&
											  (b.RevenueD.Value.Year == lastYear.Year)
										group b by new { b.RevenueD.Value.Month, b.ContainerSizeI } into g
										select new CombinedExpenseReportData()
										{
											Month = g.Key.Month,
											LastYearTransportCount = g.Count(),
											LastYearRevenue = g.Sum(f => f.Amount ?? 0) + g.Sum(f => f.CustomerSurcharge ?? 0)
										}).ToList();
					if (param.ReportTimeI == "M")
					{
						var row = new CombinedExpenseReportData()
						{
							ObjectN = areaN,
							Month = month,
							TransportCount = data.Sum(c => c.TransportCount),
							TotalWeight = data.Sum(c => c.TotalWeight),
							Revenue = data.Sum(c => c.Revenue),
							LastYearTransportCount = lastYearData.Sum(c => c.LastYearTransportCount),
							LastYearRevenue = lastYearData.Sum(c => c.LastYearRevenue)
						};
						if (row.TransportCount > 0 || row.TotalWeight > 0 || row.Revenue > 0 || row.LastYearRevenue > 0)
							result.Add(row);
					}
					else
					{
						for (int m = 1; m <= 12; m++)
						{
							var row = new CombinedExpenseReportData()
							{
								ObjectN = areaN,
								Month = m,
								TransportCount = data.Where(c => c.Month == m).Sum(c => c.TransportCount),
								TotalWeight = data.Where(c => c.Month == m).Sum(c => c.TotalWeight),
								Revenue = data.Where(c => c.Month == m).Sum(c => c.Revenue),
								LastYearTransportCount = lastYearData.Where(c => c.Month == m).Sum(c => c.LastYearTransportCount),
								LastYearRevenue = lastYearData.Where(c => c.Month == m).Sum(c => c.LastYearRevenue)
							};
							if (row.TransportCount > 0 || row.TotalWeight > 0 || row.Revenue > 0 || row.LastYearRevenue > 0)
								result.Add(row);
						}
					}
				}
			}
			return result;
		}

		private List<CombinedExpenseReportData> GetDepartmentRevenueReportList(CombinedRevenueReportParam param, int month, int year)
		{
			var data = (from a in _orderHRepository.GetAllQueryable()
						join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
						 equals new { b.OrderD, b.OrderNo } into t1
						from b in t1.DefaultIfEmpty()
						where (param.ObjectList == "0" || param.ObjectList.Equals(a.OrderDepC)) &&
							   (param.ReportTimeI != "M" || b.RevenueD.Value.Month == month) &&
							   (b.RevenueD.Value.Year == year)
						group new { a, b } by new { a.OrderDepC, b.RevenueD.Value.Month } into g
						select new CombinedExpenseReportData()
						{
							ObjectN = g.Key.OrderDepC,
							Month = g.Key.Month,
							TransportCount = g.Count(),
							TotalWeight = g.Where(f => f.b.ContainerSizeI == "3").DefaultIfEmpty().Sum(f => f.b.NetWeight ?? 0),
							Revenue = g.Sum(f => f.b.Amount ?? 0) + g.Sum(f => f.b.CustomerSurcharge ?? 0)
						}).ToList();

			DateTime lastYear = param.ReportTimeI == "M" ? param.TransportM.AddMonths(-1) : param.TransportM.AddYears(-1);
			var lastYearData = (from a in _orderHRepository.GetAllQueryable()
								join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
								 equals new { b.OrderD, b.OrderNo } into t1
								from b in t1.DefaultIfEmpty()
								where (param.ObjectList == "0" || param.ObjectList.Equals(a.OrderDepC)) &&
										(param.ReportTimeI != "M" || b.RevenueD.Value.Month == lastYear.Month) &&
									   (b.RevenueD.Value.Year == lastYear.Year)
								group new { a, b } by new { a.OrderDepC, b.RevenueD.Value.Month } into g
								select new CombinedExpenseReportData()
								{
									ObjectN = g.Key.OrderDepC,
									Month = g.Key.Month,
									LastYearTransportCount = g.Count(),
									LastYearRevenue = g.Sum(f => f.b.Amount ?? 0) + g.Sum(f => f.b.CustomerSurcharge ?? 0)
								}).ToList();
			//var dataWeight = (from a in _orderHRepository.GetAllQueryable()
			//				  join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
			//				   equals new { b.OrderD, b.OrderNo } into t1
			//				  from b in t1.DefaultIfEmpty()
			//				  where (param.ObjectList == "0" || param.ObjectList.Equals(a.OrderDepC)) &&
			//						 (param.ReportTimeI != "M" || b.RevenueD.Value.Month == month) &&
			//						 (b.RevenueD.Value.Year == year) &&
			//						 b.ContainerSizeI == "3"
			//				  group new { a, b } by new { a.OrderDepC, b.RevenueD.Value.Month } into g
			//				  select new CombinedExpenseReportData()
			//				  {
			//					  ObjectN = g.Key.OrderDepC,
			//					  Month = g.Key.Month,
			//					  TotalWeight = g.Sum(f => f.b.NetWeight ?? 0),
			//				  }).ToList();

			var concat = data.Concat(lastYearData).ToList(); //.Concat(dataWeight)

			var result = (from c in concat
						  join d in _departmentRepository.GetAllQueryable()
						  on c.ObjectN equals d.DepC
						  group new { c, d } by new { d.DepN, c.Month } into g
						  select new CombinedExpenseReportData()
						  {
							  ObjectN = g.Key.DepN,
							  Month = g.Key.Month,
							  TransportCount = g.Sum(s => s.c.TransportCount),
							  TotalWeight = g.Sum(s => s.c.TotalWeight),
							  Revenue = g.Sum(s => s.c.Revenue),
							  LastYearTransportCount = g.Sum(s => s.c.LastYearTransportCount),
							  LastYearRevenue = g.Sum(s => s.c.LastYearRevenue),
						  }).ToList();

			return result;
		}

		private List<CombinedExpenseReportData> GetEmployeeRevenueReportList(CombinedRevenueReportParam param, int month, int year)
		{
			var data = (from a in _orderHRepository.GetAllQueryable()
						join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
						 equals new { b.OrderD, b.OrderNo } //into t1
						//from b in t1.DefaultIfEmpty()
						where (param.ObjectList == "null" || param.ObjectList.Contains("," + a.EntryClerkC)) &&
							   (param.ReportTimeI != "M" || b.RevenueD.Value.Month == month) &&
							   (b.RevenueD.Value.Year == year)
						group new { a, b } by new { a.EntryClerkC, b.RevenueD.Value.Month } into g
						select new CombinedExpenseReportData()
						{
							ObjectN = g.Key.EntryClerkC,
							Month = g.Key.Month,
							TransportCount = g.Count(),
							TotalWeight = g.Where(f => f.b.ContainerSizeI == "3").DefaultIfEmpty().Sum(f => f.b.NetWeight ?? 0),
							Revenue = g.Sum(f => f.b.Amount ?? 0) + g.Sum(f => f.b.CustomerSurcharge ?? 0)
						}).ToList();

			DateTime lastYear = param.ReportTimeI == "M" ? param.TransportM.AddMonths(-1) : param.TransportM.AddYears(-1);
			var lastYearData = (from a in _orderHRepository.GetAllQueryable()
								join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
								 equals new { b.OrderD, b.OrderNo } into t1
								from b in t1.DefaultIfEmpty()
								where (param.ObjectList == "null" || param.ObjectList.Contains("," + a.EntryClerkC)) &&
										(param.ReportTimeI != "M" || b.RevenueD.Value.Month == lastYear.Month) &&
										(b.RevenueD.Value.Year == lastYear.Year)
								group new { a, b } by new { a.EntryClerkC, b.RevenueD.Value.Month } into g
								select new CombinedExpenseReportData()
								{
									ObjectN = g.Key.EntryClerkC,
									Month = g.Key.Month,
									LastYearTransportCount = g.Count(),
									LastYearRevenue = g.Sum(f => f.b.Amount ?? 0) + g.Sum(f => f.b.CustomerSurcharge ?? 0)
								}).ToList();

			//var dataWeight = (from a in _orderHRepository.GetAllQueryable()
			//			join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
			//			 equals new { b.OrderD, b.OrderNo } into t1
			//			from b in t1.DefaultIfEmpty()
			//			where (param.ObjectList == "null" || param.ObjectList.Contains("," + a.EntryClerkC)) &&
			//				   (param.ReportTimeI != "M" || b.RevenueD.Value.Month == month) &&
			//				   (b.RevenueD.Value.Year == year) &&
			//				   b.ContainerSizeI == "3"
			//			group new { a, b } by new { a.EntryClerkC, b.RevenueD.Value.Month } into g
			//			select new CombinedExpenseReportData()
			//			{
			//				ObjectN = g.Key.EntryClerkC,
			//				Month = g.Key.Month,
			//				TotalWeight = g.Sum(f => f.b.NetWeight ?? 0),
			//			}).ToList();

			var concat = data.Concat(lastYearData).ToList();//.Concat(dataWeight)

			var result = (from c in concat
						  join e in _employeeRepository.GetAllQueryable()
						  on c.ObjectN equals e.EmployeeC
						  group new { c, e } by new { e.EmployeeFirstN, e.EmployeeLastN , c.Month } into g
						  select new CombinedExpenseReportData()
						  {
							  ObjectN = g.Key.EmployeeLastN + " " + g.Key.EmployeeFirstN,
							  Month = g.Key.Month,
							  TransportCount = g.Sum(s => s.c.TransportCount),
							  TotalWeight = g.Sum(s => s.c.TotalWeight),
							  Revenue = g.Sum(s => s.c.Revenue),
							  LastYearTransportCount = g.Sum(s => s.c.LastYearTransportCount),
							  LastYearRevenue = g.Sum(s => s.c.LastYearRevenue),
						  }).ToList();

			return result;
		}
	}
}
