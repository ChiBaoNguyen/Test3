using System.Text;
using CrystalReport.Dataset.FuelConsumption;
using CrystalReport.Dataset.Maintenance;
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
using Website.ViewModels.Expense;
using Website.ViewModels.FuelConsumption;
using Website.ViewModels.Order;
using Website.ViewModels.Report.FuelConsumption;
using Website.ViewModels.Report.Maintenance;
using Website.ViewModels.DriverLicense;
using Website.ViewModels.Surcharge;
using Website.ViewModels.Expense;
using AutoMapper;

namespace Service.Services
{
	public partial interface IReportService
	{
		//Stream ExportPdfTransportExpense(DriverDispatchReportParam param);
		Stream ExportPdfTransportExpenseList(DriverDispatchReportParam param, string userName);
		Stream ExportExcelTransportExpenseList(DriverDispatchReportParam param, string userName);
		Stream ExportPdfMaintenace(MaintenanceReportParam param, string userName);
		Stream ExportPdfFuelConsumptionDetail(FuelConsumptionDetailReportParam param, string userName);
		Stream ExportPdfTransportMaintenance(MaintenanceReportParam param, string userName);
		Stream ExportPdfTransportHandover(DispatchViewModel param);
		Stream ExportPdfTransportInstruction(DispatchViewModel param);
		Stream ExportPdfUseFuelReport(FuelConsumptionDetailReportParam param, string userName);
	}

	public partial class ReportService : IReportService
	{
		private string GetEmployeeByUserName(string userName)
		{
			var user = (from a in _userRepository.GetAllQueryable()
						join c in _employeeRepository.GetAllQueryable() on a.EmployeeC equals c.EmployeeC into t2
						from c in t2.DefaultIfEmpty()
						where (a.UserName == userName)
						select new
						{
							EmployeeN = c != null ? c.EmployeeLastN + " " + c.EmployeeFirstN : ""
						}).ToList().FirstOrDefault().EmployeeN;
			return user;
		}
		public Stream ExportPdfTransportExpenseList(DriverDispatchReportParam param, string userName)
		{
			Stream stream;
			DataRow row;
			TransportExpenseList.TransportExpenseListDataTable dt;
			Dictionary<string, string> dicLanguage;
			int index;

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
			dt = new TransportExpenseList.TransportExpenseListDataTable();
			List<DispatchDetailViewModel> data = GetTransportExpenseReportList(param);

			#region setlanguage
			// get language for report
			dicLanguage = new Dictionary<string, string>();
			int intLanguage = Utilities.ConvertLanguageToInt(param.Laguague);

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																 (con.TextKey == "TLTTRANSPORTEXPENSEREPORT" ||
																	con.TextKey == "LBLCUSTOMERREPORT" ||
																	con.TextKey == "RPHDCONTNO" ||
																	con.TextKey == "RPHDCONTSIZE" ||
																	con.TextKey == "LBLTREGISTEREDNO" ||
																	con.TextKey == "LBLREPORTIDRIVER" ||
																	con.TextKey == "RPHDPARTNERN" ||
																	con.TextKey == "RPHDTAXAMOUNT" ||
																	con.TextKey == "LBLPAYONBEHALF" ||
																	con.TextKey == "RPHDTOTALSURCHARGE" ||
																	con.TextKey == "RPHDAMOUNT" ||
																	con.TextKey == "RPLBLTOTAL" ||
																	con.TextKey == "LBLINTERNALREPORT" ||
																	con.TextKey == "LBLTREGISTEREDNO" ||
																	con.TextKey == "RPHDORDERNO" ||
																	con.TextKey == "RPHDORDERTYPE" ||
																	con.TextKey == "RPHDROUTE" ||
																	con.TextKey == "RPHDISINCLUDED" ||
																	con.TextKey == "RPHDDRIVINGALLOWANCE" ||
																	con.TextKey == "RPHDPARTNER" ||
																	con.TextKey == "RPHDPARTNERFEE" ||
																	con.TextKey == "RPHDTOTRANSPORTFEE" ||
																	con.TextKey == "RPHDTOLAMOUNT" ||
																	con.TextKey == "RPHDTOLPARTNERINCLUDETAX" ||
																	con.TextKey == "RPHDTOTALPARTNERAMOUNT" ||
																	con.TextKey == "RPHDAMOUNT" ||
																	con.TextKey == "RPHDTOTRANSPORTFEE" ||
																	con.TextKey == "LBLJOBNO" ||
																	con.TextKey == "LBLLOAD" ||


																	con.TextKey == "RPFTPRINTTIME" ||
																	con.TextKey == "RPFTPRINTBY" ||
																	con.TextKey == "RPFTPAGE" ||
																	con.TextKey == "RPFTCREATOR"
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

			index = 1;
			if (data != null && data.Count > 0)
			{
				for (int iloop = 0; iloop < data.Count; iloop++)
				{
					row = dt.NewRow();

					// group row
					row["No"] = index;
					//row["OrderNo"] = data[iloop].Dispatch.OrderNo + "-" + data[iloop].Dispatch.DetailNo;
					row["OrderNo"] = data[iloop].OrderH.JobNo;
					row["OrderType"] = Utilities.GetOrderTypeName(data[iloop].OrderH.OrderTypeI);
					row["CustomerName"] = data[iloop].OrderH.CustomerShortN != "" ? data[iloop].OrderH.CustomerShortN : data[iloop].OrderH.CustomerN;
					row["BLBK"] = !string.IsNullOrEmpty(data[iloop].OrderH.BLBK) ? data[iloop].OrderH.BLBK : "";
					row["ConNo"] = !string.IsNullOrEmpty(data[iloop].OrderD.ContainerNo) ? (data[iloop].OrderD.ContainerNo) : "";
					row["ConSize"] = data[iloop].OrderD.ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] + ((data[iloop].OrderD.NetWeight != null && data[iloop].OrderD.NetWeight > 0) ? " " + (data[iloop].OrderD.NetWeight ?? 0).ToString("#,###.0") : "")
						: Utilities.GetContainerSizeName(data[iloop].OrderD.ContainerSizeI);
					row["RegisterNo"] = !string.IsNullOrEmpty(data[iloop].Dispatch.RegisteredNo) ? data[iloop].Dispatch.RegisteredNo : "";
					row["DriverName"] = !string.IsNullOrEmpty(data[iloop].Dispatch.DriverN) ? data[iloop].Dispatch.DriverN : "";

					var loc1 = "";
					var loc2 = "";
					var loc3 = "";
					if (!string.IsNullOrEmpty(data[iloop].Dispatch.Location1N))
					{
						loc1 += data[iloop].Dispatch.Location1N;
						if (!string.IsNullOrEmpty(data[iloop].Dispatch.Operation1C) && data[iloop].Dispatch.Operation1C != "0")
						{
							loc1 += " (" + data[iloop].Dispatch.Operation1C + ")";
						}
						if (data[iloop].Dispatch.Location1DT != null)
						{
							loc1 += " " + Utilities.GetFormatDateAndHourReportByLanguage(data[iloop].Dispatch.Location1DT.Value, intLanguage);
						}
						loc1 += ", ";
					}
					if (!string.IsNullOrEmpty(data[iloop].Dispatch.Location2N))
					{
						loc2 += data[iloop].Dispatch.Location2N;
						if (!string.IsNullOrEmpty(data[iloop].Dispatch.Operation2C) && data[iloop].Dispatch.Operation2C != "0")
						{
							loc2 += " (" + data[iloop].Dispatch.Operation2C + ")";
						}
						if (data[iloop].Dispatch.Location2DT != null)
						{
							loc2 += " " + Utilities.GetFormatDateAndHourReportByLanguage(data[iloop].Dispatch.Location2DT.Value, intLanguage);
						}
						loc2 += ", ";
					}
					if (!string.IsNullOrEmpty(data[iloop].Dispatch.Location3N))
					{
						loc3 += data[iloop].Dispatch.Location3N;
						if (!string.IsNullOrEmpty(data[iloop].Dispatch.Operation3C) && data[iloop].Dispatch.Operation3C != "0")
						{
							loc3 += " (" + data[iloop].Dispatch.Operation3C + ")";
						}
						if (data[iloop].Dispatch.Location3DT != null)
						{
							loc3 += " " + Utilities.GetFormatDateAndHourReportByLanguage(data[iloop].Dispatch.Location3DT.Value, intLanguage);
						}
						loc3 += ", ";
					}
					string loc = loc1 + loc2 + loc3;
					if (loc.Length > 1)
						row["Route"] = loc.Remove(loc.Length - 2);
					row["IncludedExpense"] = data[iloop].Dispatch.IncludedExpense ?? 0;
					row["TransportFee"] = data[iloop].Dispatch.TransportFee ?? 0;
					row["Allowance"] = data[iloop].Dispatch.DriverAllowance ?? 0;

					var partnerFee = data[iloop].Dispatch.PartnerFee ?? 0;
					row["PartnerFee"] = partnerFee;
					var partnerTaxAmount = data[iloop].Dispatch.PartnerTaxAmount ?? 0;
					row["PartnerTaxAmount"] = partnerTaxAmount;
					var partnerExpense = data[iloop].Dispatch.PartnerExpense ?? 0;
					row["PartnerExpense"] = partnerExpense;
					var partnerSurcharge = data[iloop].Dispatch.PartnerSurcharge ?? 0;
					row["PartnerSurcharge"] = partnerSurcharge;
					var partnerExpenseTotal = partnerFee + partnerTaxAmount + partnerSurcharge;
					row["PartnerExpenseTotal"] = partnerExpenseTotal;
					row["PartnerPayment"] = partnerExpenseTotal + partnerExpense;

					if ((iloop == 0)
						|| (data[iloop].OrderD.OrderD != data[iloop - 1].OrderD.OrderD
						|| data[iloop].OrderD.OrderNo != data[iloop - 1].OrderD.OrderNo
						|| data[iloop].OrderD.DetailNo != data[iloop - 1].OrderD.DetailNo)
						)
					{
						var amount = data[iloop].OrderD.Amount ?? 0;
						row["Amount"] = amount;
						var surcharge = data[iloop].OrderD.CustomerSurcharge ?? 0;
						row["Surcharge"] = surcharge;
						var taxAmount = data[iloop].OrderD.TaxAmount ?? 0;
						row["TaxAmount"] = taxAmount;
						var totalExpense = data[iloop].OrderD.TotalExpense ?? 0;
						row["TotalExpense"] = totalExpense;
						var transportExpenseTotal = amount + surcharge + taxAmount;
						row["TransportExpenseTotal"] = transportExpenseTotal;
						row["Payment"] = transportExpenseTotal + totalExpense;
					}

					dt.Rows.Add(row);
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

			#endregion

			stream = CrystalReport.Service.TransportExpense.ExportPdf.Exec(dt,
																		 intLanguage,
																		 fromDate,
																		 toDate,
																		 companyName,
																		 companyAddress,
																		 fileName,
																		 user,
																		 dicLanguage
																		 );
			return stream;
		}

		public Stream ExportExcelTransportExpenseList(DriverDispatchReportParam param, string userName)
		{
			Stream stream;
			DataRow row;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			int index;

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
			List<DispatchDetailViewModel> data = GetTransportExpenseReportList(param);

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
																 (con.TextKey == "TLTTRANSPORTEXPENSEREPORT" ||
																	con.TextKey == "LBLCUSTOMERREPORT" ||
																	con.TextKey == "LBLCONTNUMBER" ||
																	con.TextKey == "RPHDCONTSIZE" ||
																	con.TextKey == "LBLTREGISTEREDNO" ||
																	con.TextKey == "LBLREPORTIDRIVER" ||
																	con.TextKey == "RPHDPARTNERN" ||
																	con.TextKey == "RPHDTAXAMOUNT" ||
																	con.TextKey == "LBLPAYONBEHALF" ||
																	con.TextKey == "RPHDTOTALSURCHARGE" ||
																	con.TextKey == "RPHDAMOUNT" ||
																	con.TextKey == "RPLBLTOTAL" ||
																	con.TextKey == "LBLINTERNALREPORT" ||
																	con.TextKey == "LBLTREGISTEREDNO" ||
																	con.TextKey == "RPHDORDERNO" ||
																	con.TextKey == "RPHDORDERTYPE" ||
																	con.TextKey == "RPHDROUTE" ||
																	con.TextKey == "LBLLOCATIONREPORT" ||
																	con.TextKey == "RPHDISINCLUDED" ||
																	con.TextKey == "RPHDDRIVINGALLOWANCE" ||
																	con.TextKey == "RPHDPARTNER" ||
																	con.TextKey == "RPHDPARTNERFEE" ||
																	con.TextKey == "RPHDTOTRANSPORTFEE" ||
																	con.TextKey == "RPHDTOLAMOUNT" ||
																	con.TextKey == "RPHDTOLPARTNERINCLUDETAX" ||
																	con.TextKey == "RPHDTOTALPARTNERAMOUNT" ||
																	con.TextKey == "RPFTPRINTTIME" ||
																	con.TextKey == "RPFTPRINTBY" ||
																	con.TextKey == "RPFTPAGE" ||
																	con.TextKey == "RPFTCREATOR" ||
																	con.TextKey == "RPHDAMOUNT" ||
																	con.TextKey == "RPHDTOTRANSPORTFEE" ||
																	con.TextKey == "LBLJOBNO" ||
																	con.TextKey == "LBLTYPE" ||
																	con.TextKey == "LBLREVENUE" ||
																	con.TextKey == "LBLEXPENSESHORT" ||
																	con.TextKey == "LBLTRUCKRENTAL" ||
																	con.TextKey == "LBLEXPENSEINTERNALREPORT" ||
																	con.TextKey == "LBLSALARYDRIVER" ||
                                                                    con.TextKey == "LBLPROVISIONALPROFIT" ||
																	con.TextKey == "LBLFUEL" ||
																	con.TextKey == "LBLTAXAMOUNT" ||
																	con.TextKey == "LBLCUSTOMER" ||
																	con.TextKey == "LBLINTERNAL" ||
																	con.TextKey == "LBLTOTALTRANSPORTRP" ||
																	con.TextKey == "LBLGROSSPROCEEDS" ||
																	con.TextKey == "LBLTOTALRENTAL" ||
																	con.TextKey == "LBLMONEYTOPAY" ||
																	con.TextKey == "LBLTOTALEXPENSE" ||
																	con.TextKey == "LBLPROFIT" ||
																	con.TextKey == "LBLMONEYRENTAL" ||
                                                                    con.TextKey ==  "RPHDTOTALSURCHARGE" ||
																	con.TextKey == "LBLTRANSPORTDATEDISPATCHRP" ||
																	con.TextKey == "LBLORDERTYPEDISPATCH" ||
																	con.TextKey == "LBLSTOPOVERPLACESHORT" ||
																	con.TextKey == "LBLLOADINGPLACESHORT" ||
																	con.TextKey == "LBLDISCHARGEPLACESHORT" ||
																	con.TextKey == "LBLEXPLAINOTHEREXPENSE" ||
																	con.TextKey == "LBLOTHER" ||
																	con.TextKey == "LBLEXPORTSHORT" ||
																	con.TextKey == "LBLIMPORTSHORT" ||
																	con.TextKey == "LBLTRIP"
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
			#endregion
			var expense = _expenseRepository.Query(p => p.ExpenseI == "F").FirstOrDefault();
			decimal? unitpriceFuel = expense != null ? expense.UnitPrice : 0;
			var maplist = _expenseRepository.GetAllQueryable().ToList();
			var listexpenseviewreport = Mapper.Map<List<Expense_M>, List<ExpenseViewModel>>(maplist);

			var maplist2 = _expenseDetailRepository.GetAllQueryable().ToList();
			var listexpensedetailviewreport = Mapper.Map<List<Expense_D>, List<ExpenseDetailViewModel>>(maplist2);

			stream = CrystalReport.Service.TransportExpense.ExportExcel.Exec(listexpenseviewreport, listexpensedetailviewreport, unitpriceFuel, data, intLanguage,
																				fromDate, toDate,
																				companyName, companyAddress,
																				fileName, user,
																				dicLanguage
																				);
			return stream;
		}
        public List<DispatchDetailViewModel> GetTransportExpenseReportList(DriverDispatchReportParam param)
        {
            #region Querry
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
                                   // bổ sung thêm Partner
                                   join p in _partnerRepository.GetAllQueryable() on new { a.PartnerMainC, a.PartnerSubC }
                                   equals new { p.PartnerMainC, p.PartnerSubC } into t7
                                   from p in t7.DefaultIfEmpty()

                                   // end
                                   where ((param.TransportDFrom == null || (param.ReportI == "D" && a.TransportD >= param.TransportDFrom) || (param.ReportI == "O" && e.OrderD >= param.TransportDFrom)) &
                                        (param.TransportDTo == null || (param.ReportI == "D" && a.TransportD <= param.TransportDTo) || (param.ReportI == "O" && e.OrderD <= param.TransportDTo)) &
                                        (string.IsNullOrEmpty(param.TruckC) || param.TruckC == "undefined" || a.TruckC == param.TruckC) &
                                        (string.IsNullOrEmpty(param.DriverC) || param.DriverC == "undefined" || a.DriverC == param.DriverC) &
                                        (param.Customer == "null" || (param.Customer).Contains(e.CustomerMainC + "_" + e.CustomerSubC)) &
                                        (param.Partner == "null" || (p != null && (param.Partner).Contains(p.PartnerMainC + "_" + p.PartnerSubC))) &
                                        (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || (param.ReportI == "D" && b.DepC == param.DepC) || (param.ReportI == "O" && e.OrderDepC == param.DepC)) &
                                       //(string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || e.OrderTypeI == param.OrderTypeI)
                                        (string.IsNullOrEmpty(param.OrderNo) || param.OrderNo == "undefined" || a.OrderNo == param.OrderNo) &
                                        (string.IsNullOrEmpty(param.BLBK) || param.BLBK == "undefined" || e.BLBK == param.BLBK) &
                                        (string.IsNullOrEmpty(param.BLBK) || param.JobNo == "undefined" || e.JobNo == param.JobNo) &//Convert.ToInt32(DispatchStatus.Confirmed).ToString()
										(d.TaxAmount != null && d.TotalAmount != null && d.Amount != null)
                                        )
                                   select new DispatchDetailViewModel()
                                   {
                                       Dispatch = new DispatchViewModel
                                       {
                                           OrderD = a.OrderD,
                                           OrderNo = a.OrderNo,
                                           DetailNo = a.DetailNo,
                                           DispatchNo = a.DispatchNo,
                                           TransportD = a.TransportD,
                                           DispatchOrder = a.DispatchOrder,
                                           ContainerStatus = a.ContainerStatus,
                                           DispatchStatus = a.DispatchStatus,
                                           TruckC = a.TruckC,
                                           RegisteredNo = (a.RegisteredNo != null && a.RegisteredNo != "") ? a.RegisteredNo : (b != null ? b.RegisteredNo : ""),
                                           DriverC = a.DriverC,
                                           DriverN = c != null ? c.LastN + " " + c.FirstN : (p != null ? ((p.PartnerShortN != null && p.PartnerShortN != "") ? p.PartnerShortN : p.PartnerN) : ""),
                                           TransportFee = a.TransportFee,
                                           IncludedExpense = a.IncludedExpense,
                                           DriverAllowance = a.DriverAllowance,
                                           Expense = a.Expense,
                                           PartnerFee = a.PartnerFee,
                                           PartnerExpense = a.PartnerExpense,
                                           PartnerSurcharge = a.PartnerSurcharge,
                                           PartnerDiscount = a.PartnerDiscount,
                                           PartnerTaxAmount = a.PartnerTaxAmount,
                                           ApproximateDistance = a.ApproximateDistance,
                                           ActualDistance = a.ActualDistance,
                                           DispatchI = a.DispatchI,
                                           InvoiceD = a.DispatchI == "1" ? a.InvoiceD : null,
                                           Location1N = a.Location1N,
                                           Operation1C = a.Operation1C,
                                           Location1DT = a.Location1DT,
                                           Location2N = a.Location2N,
                                           Operation2C = a.Operation2C,
                                           Location2DT = a.Location2DT,
                                           Location3N = a.Location3N,
                                           Operation3C = a.Operation3C,
                                           Location3DT = a.Location3DT,
                                           TotalFuel = a.TotalFuel,
                                           TotalDriverAllowance = d.TotalDriverAllowance,
										   LossFuelRate = a.LossFuelRate
                                       },
                                       OrderD = new ContainerViewModel()
                                       {
                                           OrderD = d.OrderD,
                                           OrderNo = d.OrderNo,
                                           DetailNo = d.DetailNo,
                                           Amount = d.Amount,
                                           TotalExpense = d.TotalExpense,
                                           DetainAmount = d.DetainAmount,
                                           CustomerSurcharge = d.CustomerSurcharge,
                                           TaxAmount = d.TaxAmount,
                                           PartnerSurcharge = d.PartnerSurcharge,
                                           ContainerNo = d.ContainerNo,
                                           ContainerSizeI = d.ContainerSizeI,
                                           ActualLoadingD = d.ActualLoadingD,
                                           ActualDischargeD = d.ActualDischargeD,
                                           RevenueD = d.RevenueD,
                                           NetWeight = d.NetWeight,
                                           PartnerAmount = d.PartnerAmount,
                                           LocationDispatch1 = d.LocationDispatch1,
                                           LocationDispatch2 = d.LocationDispatch2,
                                           LocationDispatch3 = d.LocationDispatch3
                                       },
                                       OrderH = new OrderViewModel()
                                       {
                                           OrderD = e.OrderD,
                                           OrderNo = e.OrderNo,
                                           OrderTypeI = e.OrderTypeI,
                                           OrderDepC = e.OrderDepC,
                                           OrderDepN = k != null ? k.DepN : "",
                                           BLBK = e.BLBK ?? "",
                                           CustomerShortN = f != null ? (f.CustomerShortN ?? "") : "",
                                           CustomerN = f != null ? f.CustomerN : "",
                                           JobNo = e.JobNo,
                                       },
                                   };

            #endregion
			transportExpense = transportExpense.OrderBy("Dispatch.OrderD asc, Dispatch.OrderNo asc, Dispatch.DetailNo asc, Dispatch.TransportD asc, OrderH.BLBK asc");
           
            var transportExpenseList = transportExpense.ToList();
            var listResult = new List<DispatchDetailViewModel>();
	        var sumData =
		        transportExpenseList.Select(p => p.Dispatch).ToList()
			        .GroupBy(p => new {p.OrderNo, p.OrderD, p.DetailNo, p.TransportD})
			        .Select(p => new DispatchViewModel()
			        {
				        OrderNo = p.Key.OrderNo,
				        OrderD = p.Key.OrderD,
				        DetailNo = p.Key.DetailNo,
						TransportD = p.Key.TransportD,
				        TransportFee = p.Sum(x => x.TransportFee),
				        IncludedExpense = p.Sum(x => x.IncludedExpense),
				        DriverAllowance = p.Sum(x => x.DriverAllowance),
				        Expense = p.Sum(x => x.Expense),
				        PartnerTaxAmount = p.Sum(x => x.PartnerTaxAmount),
				        ApproximateDistance = p.Sum(x => x.ApproximateDistance),
				        ActualDistance = p.Sum(x => x.ActualDistance),
				        TotalFuel = p.Sum(x => x.TotalFuel),
				        TotalKm = p.Sum(x => x.TotalKm),
				        TotalDriverAllowance = p.Sum(x => x.TotalDriverAllowance),
				        LossFuelRate = p.Sum(x => x.LossFuelRate)
			        }).ToList();
            sumData.ForEach(p =>
            {
                var item =
                    transportExpenseList.FirstOrDefault(
                        t =>
                            t.Dispatch.OrderNo == p.OrderNo && t.Dispatch.OrderD == p.OrderD &&
                            t.OrderD.DetailNo == p.DetailNo);
                var expenseD =
                    _expenseDetailRepository.Query(
                        e => e.OrderD == p.OrderD && e.OrderNo == p.OrderNo && e.DetailNo == p.DetailNo).ToList();
                decimal iinclude = 0;
                decimal ipayable = 0;
                decimal irequest = 0;
                if (expenseD != null)
                {
					var sum = expenseD.Where(i => i.IsIncluded.Equals("1") && i.IsRequested.Equals("1")).Sum(i => i.Amount);
                    if (sum != null)
                        iinclude = (decimal)sum;
                    var sum1 = expenseD.Where(i => i.IsPayable.Equals("1")).Sum(i => i.Amount);
                    if (sum1 != null)
                        ipayable = (decimal)sum1;
					var sum2 = expenseD.Where(i => i.IsRequested.Equals("1") && i.IsIncluded.Equals("0")).Sum(i => i.Amount);
                    if (sum2 != null)
                        irequest = (decimal)sum2;
                }
                listResult.Add(new DispatchDetailViewModel()
                {
                    OrderD = item.OrderD,
                    OrderH = item.OrderH,
                    Dispatch = new DispatchViewModel()
                    {
                        OrderNo = p.OrderNo,
                        OrderD = p.OrderD,
                        DetailNo = p.DetailNo,
						TransportD = p.TransportD,
                        TransportFee = p.TransportFee,
                        IncludedExpense = p.IncludedExpense,
                        DriverAllowance = p.DriverAllowance,
                        Expense = p.Expense,
                        PartnerTaxAmount = p.PartnerTaxAmount,
                        ApproximateDistance = p.ApproximateDistance,
                        ActualDistance = p.ActualDistance,
                        Location1N = string.IsNullOrEmpty(p.Location1N) ? item.Dispatch.Location1N : p.Location1N,
                        Location2N = string.IsNullOrEmpty(p.Location2N) ? item.Dispatch.Location2N : p.Location2N,
                        Location3N = string.IsNullOrEmpty(p.Location3N) ? item.Dispatch.Location2N : p.Location3N,
                        TotalFuel = p.TotalFuel,
                        TotalDriverAllowance = p.TotalDriverAllowance,
						LossFuelRate = p.LossFuelRate
                    },
                    IsIncludedAmount = iinclude,
                    IsPayableAmount = ipayable,
                    IsRequestedAmount = irequest
                });
            });
            return listResult;
        }

		#region Maintenance
		public Stream ExportPdfMaintenace(MaintenanceReportParam param, string userName)
		{
			Stream stream;
			MaintenanceList.MaintenanceDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			var companyName = "";
			var companyAddress = "";
			var fileName = "";

			// get data
			dt = new MaintenanceList.MaintenanceDataTable();
			MaintenanceReportData data = GetMaintenanceList(param);
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

			// get language for report
			dicLanguage = new Dictionary<string, string>();
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
																 (con.TextKey == "LBLMAINTENANCEPLANREPORT" ||
																  con.TextKey == "LBLMAINTENANCEFINISHEDREPORT" ||
																  con.TextKey == "LBLREMODELNO" ||
																  con.TextKey == "LBLREMODELYES" ||
																  con.TextKey == "LBLMAINTENANCEPLANREPORT" ||
																  con.TextKey == "LBLMAINTENANCEFINISHEDREPORT" ||
																  con.TextKey == "LBLCHECKREPORT" ||
																  con.TextKey == "LBLREPLACEREPORT" ||
																  con.TextKey == "LBLALLREPORT" ||
																  con.TextKey == "LBLDATE" ||
																  con.TextKey == "LBLMAINTENANCE" ||
																  con.TextKey == "LBLREPAIR"
																  )).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			#region +Insert Implement data to datatable

			if (param.Finished)
			{
				foreach (var device in data.Devices)
				{
					int index = 0;
					var code = device.Code;
					var objectI = device.ObjectI;
					//var maintenanceList = data.MaintenancesFinished
					//	.Where(x => x.Code == device.Code && x.ObjectI == device.ObjectI)
					//	.ToList();
					var inspectionList = data.InspectionsFinished
						.Where(x => x.Code == device.Code && x.ObjectI == device.ObjectI)
						.OrderBy("InspectionD, InspectionC")
						.ToList();
					//int maintenanceCount = maintenanceList.Count();
					int inspectionCount = inspectionList.Count();
					//int maxLenght = maintenanceCount > inspectionCount ? maintenanceCount : inspectionCount;
					while (index < inspectionCount) //maxLenght
					{
						DataRow row = dt.NewRow();
						////row["DriverN"] = device.DriverN;
						row["Remodel"] = device.RemodelI == "1" ? dicLanguage["LBLREMODELYES"] : (device.RemodelI == "0" ? dicLanguage["LBLREMODELNO"] : "");
						row["ModelN"] = device.ModelN;
						row["ObjectI"] = objectI;
						row["IsFinished"] = dicLanguage["LBLMAINTENANCEFINISHEDREPORT"];

						var inspectionC = inspectionList[index].InspectionC;
						var inspectionD = inspectionList[index].InspectionD;
						if (index < inspectionCount)
						{
							row["Code"] = device.RegisterNo;
							row["InspectionN"] = inspectionC != 0 ? inspectionList[index].InspectionN : dicLanguage["LBLMAINTENANCE"];
							row["InspectionPlanD"] = inspectionList[index].InspectionPlanD != null ? Utilities.GetFormatShortDateReportByLanguage((DateTime)inspectionList[index].InspectionPlanD, intLanguage) : "&nbsp;&nbsp;&nbsp;";
							row["InspectionD"] = inspectionList[index].InspectionD != DateTime.MinValue ? Utilities.GetFormatShortDateReportByLanguage(inspectionList[index].InspectionD, intLanguage) : "&nbsp;&nbsp;&nbsp;";
							row["ImplementOdometer"] = inspectionList[index].ImplementOdometer;// != null
							//? ((decimal)inspectionList[index].ImplementOdometer).ToString("#,###.#", cul.NumberFormat)
							//: "";
							row["InspectionDesc"] = "&nbsp;" + inspectionList[index].InspectionDescription;
							row["Total"] = inspectionList[index].Total ?? 0;
							row["SupplierN"] = "&nbsp;" + inspectionList[index].SupplierN;
							row["MaintenanceItemN"] = "";
							row["Remark"] = "";
							row["MaintenanceDesc"] = "";
						}

						var maintenanceList = data.MaintenancesFinished
							.Where(x => x.Code == code && x.ObjectI == objectI &&
								x.InspectionC == inspectionC && x.MaintenanceD == inspectionD)
							.OrderBy("DisplayLineNo")
							.ToList();
						int maintenanceCount = maintenanceList.Count();
						if (maintenanceCount > 0)
						{
							for (int i = 0; i < maintenanceCount; i++)
							{
								DataRow row2 = dt.NewRow();
								if (i != 0)
								{
									row["Code"] = "";
									row["InspectionN"] = "";
									row["InspectionPlanD"] = "";
									row["InspectionD"] = "";
									row["ImplementOdometer"] = "";
									row["InspectionDesc"] = "";
									row["Total"] = DBNull.Value;
									row["SupplierN"] = "";
								}
								row2.ItemArray = row.ItemArray;
								row2["MaintenanceItemN"] = maintenanceList[i].MaintenanceItemN;

								//if (index < maintenanceCount)
								//{
								//row["MaintenanceItemN"] = maintenanceList[i].MaintenanceItemN;

								if (maintenanceList[i].NoticeI == "2" && maintenanceList[i].PlanMaintenanceKm != null)
								{
									row2["Remain"] = device.ObjectI == "0"
										? ((decimal)maintenanceList[i].PlanMaintenanceKm - maintenanceList[i].CurrentOdometer ?? 0).ToString()
										: "";
									row2["RemainUnit"] = device.ObjectI == "0" ? " Km" : "";
									row2["NoticeI"] = "2";
									row2["PlanMaintenance"] = ((decimal)maintenanceList[i].PlanMaintenanceKm).ToString("#,###.#",
										cul.NumberFormat);
								}
								else if (maintenanceList[i].NoticeI == "1" && maintenanceList[i].PlanMaintenanceDate != null)
								{
									row2["Remain"] = Math.Ceiling((double)((DateTime)maintenanceList[i].PlanMaintenanceDate - (DateTime)maintenanceList[i].MaintenanceD).TotalHours / 24);
									row2["RemainUnit"] = " " + dicLanguage["LBLDATE"];
									row2["NoticeI"] = "1";
									row2["PlanMaintenance"] = Utilities.GetFormatDateReportByLanguage((DateTime)maintenanceList[i].PlanMaintenanceDate, intLanguage);
								}

								//row2["Remark"] = maintenanceList[i].Remark;
								row2["MaintenanceDesc"] = maintenanceList[i].MaintenanceDesc;
								row2["CurrentOdometer"] = maintenanceList[i].CurrentOdometer;
								//}
								row2["Remark"] = maintenanceList[i].Remark == "1" ? dicLanguage["LBLCHECKREPORT"]
									: maintenanceList[i].Remark == "2" ? dicLanguage["LBLREPLACEREPORT"]
									: maintenanceList[i].Remark == "3" ? dicLanguage["LBLREPAIR"] : "";
								//row2["MaintenanceDesc"] = maintenanceList[i].MaintenanceDesc;
								//row2["CurrentOdometer"] = maintenanceList[i].CurrentOdometer;

								dt.Rows.Add(row2);
							}
						}
						else
						{
							dt.Rows.Add(row);
						}
						index++;
					}
				}
			}


			#endregion

			#region +Insert Plan to datatable

			//if (param.Plan)
			//{
			//	foreach (var device in data.Devices)
			//	{

			//		int index = 0;
			//		var maintenanceList = data.MaintenancesPlan.Where(x => x.Code == device.Code && x.ObjectI == device.ObjectI).ToList();
			//		var inspectionList = data.InspectionsPlan.Where(x => x.Code == device.Code && x.ObjectI == device.ObjectI).ToList();
			//		int maintenanceCount = maintenanceList.Count();
			//		int inspectionCount = inspectionList.Count();
			//		int maxLenght = maintenanceCount > inspectionCount ? maintenanceCount : inspectionCount;
			//		while (index < maxLenght)
			//		{
			//			DataRow row = dt.NewRow();
			//			row["Code"] = device.RegisterNo;
			//			////row["DriverN"] = device.DriverN;
			//			row["Remodel"] = device.RemodelI == "1" ? dicLanguage["LBLREMODELYES"] : (device.RemodelI == "0" ? dicLanguage["LBLREMODELNO"] : "");
			//			row["ModelN"] = device.ModelN;
			//			row["ObjectI"] = device.ObjectI;
			//			row["IsFinished"] = dicLanguage["LBLMAINTENANCEPLANREPORT"];
			//			if (index < maintenanceCount)
			//			{
			//				row["MaintenanceItemN"] = maintenanceList[index].MaintenanceItemN;

			//				if (maintenanceList[index].NoticeI == "2" && maintenanceList[index].PlanMaintenanceKm != null)
			//				{
			//					row["Remain"] = device.ObjectI == "0"
			//						? ((decimal)maintenanceList[index].PlanMaintenanceKm - maintenanceList[index].CurrentOdometer ?? 0).ToString()
			//						: "";
			//					row["RemainUnit"] = device.ObjectI == "0" ? " Km" : "";
			//					row["NoticeI"] = "2";
			//					row["PlanMaintenance"] = ((decimal)maintenanceList[index].PlanMaintenanceKm).ToString("#,###.#", cul.NumberFormat);
			//					//maintenanceList[index].PlanMaintenanceKm;
			//				}
			//				else if (maintenanceList[index].NoticeI == "1" && maintenanceList[index].PlanMaintenanceDate != null)
			//				{
			//					row["Remain"] = Math.Ceiling((double)((DateTime)maintenanceList[index].PlanMaintenanceDate - DateTime.Now).TotalHours / 24);
			//					row["RemainUnit"] = " " + dicLanguage["LBLDATE"];
			//					row["NoticeI"] = "1";
			//					row["PlanMaintenance"] = Utilities.GetFormatDateReportByLanguage((DateTime)maintenanceList[index].PlanMaintenanceDate, intLanguage);
			//				}

			//				row["Remark"] = maintenanceList[index].Remark == "1"
			//					? dicLanguage["LBLCHECKREPORT"]
			//					: maintenanceList[index].Remark == "2" ? dicLanguage["LBLREPLACEREPORT"] : "";
			//				row["MaintenanceDesc"] = maintenanceList[index].MaintenanceDesc;
			//				row["CurrentOdometer"] = maintenanceList[index].CurrentOdometer;
			//			}

			//			if (index < inspectionCount)
			//			{
			//				row["InspectionN"] = inspectionList[index].InspectionC != 0 ? inspectionList[index].InspectionN : dicLanguage["LBLMAINTENANCE"];
			//				row["InspectionPlanD"] = inspectionList[index].InspectionPlanD != null ? Utilities.GetFormatShortDateReportByLanguage((DateTime)inspectionList[index].InspectionPlanD, intLanguage) : "";
			//				row["InspectionD"] = inspectionList[index].InspectionD != DateTime.MinValue ? Utilities.GetFormatShortDateReportByLanguage(inspectionList[index].InspectionD, intLanguage) : "";
			//				row["ImplementOdometer"] = inspectionList[index].ImplementOdometer;
			//				row["InspectionDesc"] = inspectionList[index].InspectionDescription;
			//			}

			//			dt.Rows.Add(row);
			//			index++;
			//		}

			//	}
			//}


			#endregion

			string depList = param.DepC == "null" ? dicLanguage["LBLALLREPORT"] :
				string.Join(",", data.DepList.ToArray());
			string truckList = param.TruckC == "null" ? dicLanguage["LBLALLREPORT"] :
			string.Join(",", data.TruckList.ToArray());
			string trailerList = param.TrailerC == "null" ? dicLanguage["LBLALLREPORT"] :
			string.Join(",", data.TrailerList.ToArray());

			stream = CrystalReport.Service.Maintenance.ExportPdf.Exec(dt, intLanguage, param.MaintenanceDFrom, param.MaintenanceDTo, depList, truckList, trailerList,
																		companyName, companyAddress, fileName, user);
			return stream;
		}

		private MaintenanceReportData GetMaintenanceList(MaintenanceReportParam param)
		{
			MaintenanceReportData result = new MaintenanceReportData();

			result.DepList = (from a in _departmentRepository.GetAllQueryable()
							  where param.DepC == "null" || param.DepC.Contains("," + a.DepC + ",")
							  select a.DepN).ToList();
			result.TruckList = (from a in _truckRepository.GetAllQueryable()
								where param.TruckC == "null" || param.TruckC.Contains("," + a.TruckC + ",")
								select a.RegisteredNo).ToList();

			result.TrailerList = (from a in _trailerRepository.GetAllQueryable()
								  where param.TrailerC == "null" || param.TrailerC.Contains("," + a.TrailerC + ",")
								  select a.TrailerNo).ToList();

			var truckList = (from a in _truckRepository.GetAllQueryable()
							 join b in _modelRepository.GetAllQueryable() on a.ModelC equals b.ModelC into t1
							 from b in t1.DefaultIfEmpty()
							 ////join d in _driverRepository.GetAllQueryable() on a.DriverC equals d.DriverC into t2
							 ////from d in t2.DefaultIfEmpty()
							 orderby a.RegisteredNo
							 where ((param.DepC == "null" || param.DepC.Contains("," + a.DepC + ",")) &&
									 (param.TruckC == "null" || param.TruckC.Contains("," + a.TruckC + ",")) &&
									 (a.ModelC == null || a.ModelC == "" || b.ObjectI == "0") &&
									 (a.PartnerI == "0")
							 )
							 select new MaintenanceReportDevice()
							 {
								 ObjectI = "0",
								 Code = a.TruckC,
								 RegisterNo = a.RegisteredNo,
								 ////DriverN = d.FirstN,
								 RemodelI = a.RemodelI,
								 ModelN = b != null ? b.ModelN : ""
							 }).ToList();

			var trailerList = (from a in _trailerRepository.GetAllQueryable()
							   join b in _modelRepository.GetAllQueryable() on a.ModelC equals b.ModelC into t1
							   from b in t1.DefaultIfEmpty()
							   ////join d in _driverRepository.GetAllQueryable() on a.DriverC equals d.DriverC into t2
							   ////from d in t2.DefaultIfEmpty()
							   where ((param.TrailerC == "null" || param.TrailerC.Contains("," + a.TrailerC + ",")) &&
										(a.ModelC == null || a.ModelC == "" || b.ObjectI == "1")
							   )
							   orderby a.TrailerNo
							   select new MaintenanceReportDevice()
							   {
								   ObjectI = "1",
								   Code = a.TrailerC,
								   RegisterNo = a.TrailerNo,
								   ////DriverN = d.FirstN,
								   ModelN = b.ModelN
							   }).ToList();
			var deviceList = truckList.Concat(trailerList);

			result.Devices = deviceList.ToList();

			#region +Create plan data
			//if (param.Plan)
			//{
			//	List<MaintenanceReportInspection> Inspections = new List<MaintenanceReportInspection>();
			//	List<MaintenanceReportMaintenance> Maintenances = new List<MaintenanceReportMaintenance>();

			//	//get truck inspection
			//	Inspections.AddRange((from a in _truckRepository.GetAllQueryable()
			//						  join b in _inspectionPlanDetailRepository.GetAllQueryable() on a.TruckC equals b.Code
			//						  join c in _inspectionRepository.GetAllQueryable() on new { b.ObjectI, b.InspectionC } equals new { c.ObjectI, c.InspectionC } into t1
			//						  from c in t1.DefaultIfEmpty()
			//						  where ((a.PartnerI == "0") &&
			//						  (param.DepC == "null" || param.DepC.Contains("," + a.DepC + ",")) &&
			//						  b.ObjectI == "0" &&
			//						  (param.MaintenanceDFrom <= b.InspectionPlanD && b.InspectionPlanD <= param.MaintenanceDTo) &&
			//						  (param.TruckC == "null" || param.TruckC.Contains("," + a.TruckC + ",")))
			//						  select new MaintenanceReportInspection()
			//						  {
			//							  IsFinished = "",
			//							  Code = a.TruckC,
			//							  ObjectI = "0",
			//							  InspectionN = c.InspectionN,
			//							  InspectionC = b.InspectionC,
			//							  InspectionPlanD = b.InspectionPlanD,
			//							  ImplementOdometer = a.Odometer
			//						  }).ToList());
			//	// get truck maintenance
			//	Maintenances.AddRange((from a in _truckRepository.GetAllQueryable()
			//						   join b in _maintenancePlanDetailRepository.GetAllQueryable() on a.TruckC equals b.Code
			//						   join c in _maintenanceItemRepository.GetAllQueryable() on b.MaintenanceItemC equals c.MaintenanceItemC into t1
			//						   from c in t1.DefaultIfEmpty()
			//						   where ((a.PartnerI == "0") &&
			//						   (param.DepC == "null" || param.DepC.Contains("," + a.DepC + ",")) &&
			//						   b.ObjectI == "0" &&
			//						   (c.NoticeI == "2" || param.MaintenanceDFrom <= b.PlanMaintenanceD && b.PlanMaintenanceD <= param.MaintenanceDTo) &&
			//						   (param.TruckC == "null" || param.TruckC.Contains("," + a.TruckC + ",")))
			//						   select new MaintenanceReportMaintenance()
			//						   {
			//							   IsFinished = "",
			//							   Code = a.TruckC,
			//							   ObjectI = "0",
			//							   MaintenanceItemN = c.MaintenanceItemN,
			//							   PlanMaintenanceDate = b.PlanMaintenanceD,
			//							   PlanMaintenanceKm = b.PlanMaintenanceKm,
			//							   NoticeI = c.NoticeI,
			//							   CurrentOdometer = a.Odometer
			//						   }).ToList());
			//	// get trailer inspection
			//	Inspections.AddRange((from a in _trailerRepository.GetAllQueryable()
			//						  join b in _inspectionPlanDetailRepository.GetAllQueryable() on a.TrailerC equals b.Code
			//						  join c in _inspectionRepository.GetAllQueryable() on new { b.ObjectI, b.InspectionC } equals new { c.ObjectI, c.InspectionC } into t1
			//						  from c in t1.DefaultIfEmpty()
			//						  where (b.ObjectI == "1" &&
			//						  (param.MaintenanceDFrom <= b.InspectionPlanD && b.InspectionPlanD <= param.MaintenanceDTo) &&
			//						  (param.TrailerC == "null" || param.TrailerC.Contains("," + a.TrailerC + ",")))
			//						  select new MaintenanceReportInspection()
			//						  {
			//							  IsFinished = "",
			//							  Code = a.TrailerC,
			//							  ObjectI = "1",
			//							  InspectionN = c.InspectionN,
			//							  InspectionC = b.InspectionC,
			//							  InspectionPlanD = b.InspectionPlanD
			//						  }).ToList());
			//	// get trailer maintenance
			//	Maintenances.AddRange((from a in _trailerRepository.GetAllQueryable()
			//						   join b in _maintenancePlanDetailRepository.GetAllQueryable() on a.TrailerC equals b.Code
			//						   join c in _maintenanceItemRepository.GetAllQueryable() on b.MaintenanceItemC equals c.MaintenanceItemC into t1
			//						   from c in t1.DefaultIfEmpty()
			//						   where (b.ObjectI == "1" &&
			//						   (c.NoticeI == "2" || param.MaintenanceDFrom <= b.PlanMaintenanceD && b.PlanMaintenanceD <= param.MaintenanceDTo) &&
			//						   (param.TrailerC == "null" || param.TrailerC.Contains("," + a.TrailerC + ",")))
			//						   select new MaintenanceReportMaintenance()
			//						   {
			//							   IsFinished = "",
			//							   Code = a.TrailerC,
			//							   ObjectI = "1",
			//							   MaintenanceItemN = c.MaintenanceItemN,
			//							   PlanMaintenanceDate = b.PlanMaintenanceD,
			//							   PlanMaintenanceKm = b.PlanMaintenanceKm,
			//							   NoticeI = c.NoticeI
			//						   }).ToList());

			//	result.InspectionsPlan = Inspections;
			//	result.MaintenancesPlan = Maintenances;
			//}

			#endregion

			#region +Create finished data
			if (param.Finished)
			{
				List<MaintenanceReportInspection> Inspections = new List<MaintenanceReportInspection>();
				Inspections.AddRange((from b in _inspectionDetailRepository.GetAllQueryable()
									  join c in _inspectionRepository.GetAllQueryable() on new { b.ObjectI, b.InspectionC } equals new { c.ObjectI, c.InspectionC } into t1
									  from c in t1.DefaultIfEmpty()
									  join s in _supplierRepository.GetAllQueryable() on new { b.SupplierMainC, b.SupplierSubC } equals new { s.SupplierMainC, s.SupplierSubC } into t2
									  from s in t2.DefaultIfEmpty()
									  where (param.MaintenanceDFrom <= b.InspectionD && b.InspectionD <= param.MaintenanceDTo) &&
									  ((b.ObjectI == "1" && (param.TrailerC == "null" || param.TrailerC.Contains("," + b.Code + ",")))
									  || (b.ObjectI == "0" && (param.TruckC == "null" || param.TruckC.Contains("," + b.Code + ","))))
									  select new MaintenanceReportInspection()
									  {
										  IsFinished = "OK",
										  Code = b.Code,
										  ObjectI = b.ObjectI,
										  InspectionN = c.InspectionN,
										  InspectionC = b.InspectionC,
										  InspectionPlanD = b.InspectionPlanD,
										  InspectionD = b.InspectionD,
										  ImplementOdometer = b.Odometer,
										  InspectionDescription = b.Description,
										  SupplierN = s.SupplierShortN,
										  Total = b.Total,
									  }).ToList());

				Inspections.AddRange((from b in _truckExpenseRepository.GetAllQueryable()
									  join c in _expenseRepository.GetAllQueryable() on new { b.ExpenseC } equals new { c.ExpenseC } into t1
									  from c in t1.DefaultIfEmpty()
									  join s in _supplierRepository.GetAllQueryable() on new { b.SupplierMainC, b.SupplierSubC } equals new { s.SupplierMainC, s.SupplierSubC } into t2
									  from s in t2.DefaultIfEmpty()
									  where (param.MaintenanceDFrom <= b.InvoiceD && b.InvoiceD <= param.MaintenanceDTo) &&
									  ((b.ObjectI == "1" && (param.TrailerC == "null" || param.TrailerC.Contains("," + b.Code + ",")))
									  || (b.ObjectI == "0" && (param.TruckC == "null" || param.TruckC.Contains("," + b.Code + ",")))) &&
									  c.ExpenseI.Equals("M")
									  select new MaintenanceReportInspection()
									  {
										  IsFinished = "OK",
										  Code = b.Code,
										  ObjectI = b.ObjectI,
										  InspectionN = c.ExpenseN,
										  InspectionC = 1,
										  InspectionPlanD = null,
										  InspectionD = b.InvoiceD,
										  ImplementOdometer = 0,
										  InspectionDescription = b.Description,
										  SupplierN = s.SupplierShortN,
										  Total = b.Total,
									  }).ToList());

				List<MaintenanceReportMaintenance> Maintenances = new List<MaintenanceReportMaintenance>();
				//get truck inspection
				//Inspections.AddRange((from a in _truckRepository.GetAllQueryable()
				//					  join b in _inspectionDetailRepository.GetAllQueryable() on a.TruckC equals b.Code
				//					  join c in _inspectionRepository.GetAllQueryable() on new { b.ObjectI, b.InspectionC } equals new { c.ObjectI, c.InspectionC } into t1
				//					  from c in t1.DefaultIfEmpty()
				//					  join s in _supplierRepository.GetAllQueryable() on new { b.SupplierMainC, b.SupplierSubC } equals new { s.SupplierMainC, s.SupplierSubC } into t2
				//					  from s in t2.DefaultIfEmpty()
				//					  where ((a.PartnerI == "0") &&
				//					  (param.DepC == "null" || param.DepC.Contains("," + a.DepC + ",")) &&
				//					  b.ObjectI == "0" &&
				//					  (param.MaintenanceDFrom <= b.InspectionD && b.InspectionD <= param.MaintenanceDTo) &&
				//					  (param.TruckC == "null" || param.TruckC.Contains("," + a.TruckC + ",")))
				//					  select new MaintenanceReportInspection()
				//					  {
				//						  IsFinished = "OK",
				//						  Code = b.Code,
				//						  ObjectI = "0",
				//						  InspectionN = c.InspectionN,
				//						  InspectionC = b.InspectionC,
				//						  InspectionPlanD = b.InspectionPlanD,
				//						  InspectionD = b.InspectionD,
				//						  ImplementOdometer = b.Odometer,
				//						  InspectionDescription = b.Description,
				//						  SupplierN = s.SupplierShortN,
				//						  Total = b.Total,
				//					  }).ToList());
				// get truck maintenance
				Maintenances.AddRange((from a in _truckRepository.GetAllQueryable()
									   join b in _maintenanceDetailRepository.GetAllQueryable() on a.TruckC equals b.Code
									   join c in _maintenanceItemRepository.GetAllQueryable() on b.MaintenanceItemC equals c.MaintenanceItemC into t1
									   from c in t1.DefaultIfEmpty()
									   where ((a.PartnerI == "0") &&
									   (param.DepC == "null" || param.DepC.Contains("," + a.DepC + ",")) &&
									   b.ObjectI == "0" &&
									   (param.MaintenanceDFrom <= b.MaintenanceD && b.MaintenanceD <= param.MaintenanceDTo) &&
									   (param.TruckC == "null" || param.TruckC.Contains("," + a.TruckC + ",")))
									   select new MaintenanceReportMaintenance()
									   {
										   IsFinished = "OK",
										   Code = a.TruckC,
										   ObjectI = "0",
										   InspectionC = b.InspectionC,
										   DisplayLineNo = c.DisplayLineNo,
										   MaintenanceD = b.MaintenanceD,
										   MaintenanceItemN = c.MaintenanceItemN,
										   PlanMaintenanceDate = b.PlanMaintenanceD,
										   PlanMaintenanceKm = b.PlanMaintenanceKm,
										   NoticeI = c.NoticeI,
										   MaintenanceDesc = b.PartNo + " " + b.Description,
										   CurrentOdometer = a.Odometer,
										   Remark = b.RemarksI
									   }).ToList());
				// get trailer inspection
				//Inspections.AddRange((from a in _trailerRepository.GetAllQueryable()
				//					  join b in _inspectionDetailRepository.GetAllQueryable() on a.TrailerC equals b.Code
				//					  join c in _inspectionRepository.GetAllQueryable() on new { b.ObjectI, b.InspectionC } equals new { c.ObjectI, c.InspectionC } into t1
				//					  from c in t1.DefaultIfEmpty()
				//					  join s in _supplierRepository.GetAllQueryable() on new { b.SupplierMainC, b.SupplierSubC } equals new { s.SupplierMainC, s.SupplierSubC } into t2
				//					  from s in t2.DefaultIfEmpty()
				//					  where (b.ObjectI == "1" &&
				//					  (param.MaintenanceDFrom <= b.InspectionD && b.InspectionD <= param.MaintenanceDTo) &&
				//					  (param.TrailerC == "null" || param.TrailerC.Contains("," + a.TrailerC + ",")))
				//					  select new MaintenanceReportInspection()
				//					  {
				//						  IsFinished = "OK",
				//						  Code = b.Code,
				//						  ObjectI = "1",
				//						  InspectionN = c.InspectionN,
				//						  InspectionC = b.InspectionC,
				//						  InspectionPlanD = b.InspectionPlanD,
				//						  InspectionD = b.InspectionD,
				//						  ImplementOdometer = b.Odometer,
				//						  InspectionDescription = b.Description,
				//						  SupplierN = s.SupplierShortN,
				//						  Total = b.Total,
				//					  }).ToList());
				// get trailer maintenance
				Maintenances.AddRange((from a in _trailerRepository.GetAllQueryable()
									   join b in _maintenanceDetailRepository.GetAllQueryable() on a.TrailerC equals b.Code
									   join c in _maintenanceItemRepository.GetAllQueryable() on b.MaintenanceItemC equals c.MaintenanceItemC into t1
									   from c in t1.DefaultIfEmpty()
									   where (b.ObjectI == "1" &&
									   (param.MaintenanceDFrom <= b.MaintenanceD && b.MaintenanceD <= param.MaintenanceDTo) &&
									   (param.TrailerC == "null" || param.TrailerC.Contains("," + a.TrailerC + ",")))
									   select new MaintenanceReportMaintenance()
									   {
										   IsFinished = "OK",
										   Code = b.Code,
										   ObjectI = "1",
										   InspectionC = b.InspectionC,
										   DisplayLineNo = c.DisplayLineNo,
										   MaintenanceD = b.MaintenanceD,
										   MaintenanceItemN = c.MaintenanceItemN,
										   PlanMaintenanceDate = b.PlanMaintenanceD,
										   PlanMaintenanceKm = b.PlanMaintenanceKm,
										   NoticeI = c.NoticeI,
										   CurrentOdometer = 0,
										   Remark = b.RemarksI,
										   MaintenanceDesc = b.Description
									   }).ToList());

				//get truck expenses 
				List<MaintenanceReportInspection> Expenses = new List<MaintenanceReportInspection>();
				Expenses.AddRange((from a in _truckExpenseRepository.GetAllQueryable()
								   join b in _truckRepository.GetAllQueryable() on a.Code
									  equals b.TruckC into t1
								   join f in _trailerRepository.GetAllQueryable() on a.Code
									   equals f.TrailerC into t6
								   from f in t6.DefaultIfEmpty()
								   from b in t1.DefaultIfEmpty()
								   join c in _expenseRepository.GetAllQueryable() on new { a.ExpenseC }
									   equals new { c.ExpenseC } into t2
								   from c in t2.DefaultIfEmpty()
								   join d in _driverRepository.GetAllQueryable() on new { a.DriverC }
									   equals new { d.DriverC } into t3
								   from d in t3.DefaultIfEmpty()
								   join e in _supplierRepository.GetAllQueryable() on new { a.SupplierMainC, a.SupplierSubC }
									   equals new { e.SupplierMainC, e.SupplierSubC } into t4
								   from e in t4.DefaultIfEmpty()
								   join t in _employeeRepository.GetAllQueryable() on a.EntryClerkC equals t.EmployeeC into t5
								   from t in t5.DefaultIfEmpty()
								   where (c.ExpenseI == "M") && (param.MaintenanceDFrom <= a.InvoiceD && a.InvoiceD <= param.MaintenanceDTo) &&
									  ((a.ObjectI == "1" && (param.TrailerC == "null" || param.TrailerC.Contains("," + a.Code + ",")))
									  || (a.ObjectI == "0" && (param.TruckC == "null" || param.TruckC.Contains("," + a.Code + ","))))
								   select new MaintenanceReportInspection()
								   {
									   IsFinished = "OK",
									   Code = a.Code,
									   ObjectI = a.ObjectI,
									   InspectionN = c.ExpenseN ?? "",
									   InspectionC = 0,
									   InspectionPlanD = a.InvoiceD,
									   InspectionD = a.InvoiceD,
									   ImplementOdometer = null,
									   InspectionDescription = a.Description,
									   SupplierN = e.SupplierShortN,
									   Total = a.Total + a.Tax
								   }).ToList());
				//get trailer expenses 
				Expenses.AddRange((from a in _truckExpenseRepository.GetAllQueryable()
								   join b in _truckRepository.GetAllQueryable() on a.Code
									  equals b.TruckC into t1
								   join f in _trailerRepository.GetAllQueryable() on a.Code
									   equals f.TrailerC into t6
								   from f in t6.DefaultIfEmpty()
								   from b in t1.DefaultIfEmpty()
								   join c in _expenseRepository.GetAllQueryable() on new { a.ExpenseC }
									   equals new { c.ExpenseC } into t2
								   from c in t2.DefaultIfEmpty()
								   join d in _driverRepository.GetAllQueryable() on new { a.DriverC }
									   equals new { d.DriverC } into t3
								   from d in t3.DefaultIfEmpty()
								   join e in _supplierRepository.GetAllQueryable() on new { a.SupplierMainC, a.SupplierSubC }
									   equals new { e.SupplierMainC, e.SupplierSubC } into t4
								   from e in t4.DefaultIfEmpty()
								   join t in _employeeRepository.GetAllQueryable() on a.EntryClerkC equals t.EmployeeC into t5
								   from t in t5.DefaultIfEmpty()
								   where (c.ExpenseI == "M") && (param.MaintenanceDFrom <= a.InvoiceD && a.InvoiceD <= param.MaintenanceDTo) &&
									  ((a.ObjectI == "1" && (param.TrailerC == "null" || param.TrailerC.Contains("," + a.Code + ",")))
									  || (a.ObjectI == "0" && (param.TruckC == "null" || param.TruckC.Contains("," + a.Code + ","))))
								   select new MaintenanceReportInspection()
								   {
									   IsFinished = "OK",
									   Code = a.Code,
									   ObjectI = a.ObjectI,
									   InspectionN = c.ExpenseN ?? "",
									   InspectionC = 0,
									   InspectionPlanD = a.InvoiceD,
									   InspectionD = a.InvoiceD,
									   ImplementOdometer = null,
									   InspectionDescription = a.Description,
									   SupplierN = e.SupplierShortN,
									   Total = a.Total + a.Tax
								   }).ToList());

				result.InspectionsFinished = Inspections;
				result.MaintenancesFinished = Maintenances;
				result.ExpensesFinished = Expenses;
			}
			#endregion

			return result;
		}
		#endregion
		public Stream ExportPdfUseFuelReport(FuelConsumptionDetailReportParam param, string userName)
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
			var user = GetEmployeeByUserName(userName);
			// get data
			var dt = new FuelConsumptionDetail.UseFuelDetailDataTable();
			int intLanguage;
			var data = GetUseFuelList(param);

			#region setlanguage
			// get language for report
			if (param.Languague == "vi")
			{
				intLanguage = 1;
			}
			else if (param.Languague == "jp")
			{
				intLanguage = 3;
			}
			else
			{
				intLanguage = 2;
			}
			Dictionary<string, string> dicLanguage = new Dictionary<string, string>();

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																 (con.TextKey == "FTRUSEFUELMASTER" ||
																 con.TextKey == "LBLTRUCKNODISPATCHRP" ||
																 con.TextKey == "LBLFUELOPENINGPERRIOD" ||
																 con.TextKey == "LBLUSEFUEL" ||
																 con.TextKey == "LBLESTIMATED" ||
																 con.TextKey == "LBLFUELCLOSINGPERRIOD" ||
																 con.TextKey == "RPFTPAGE" ||
																 con.TextKey == "RPFTPRINTBY" ||
																 con.TextKey == "RPFTPRINTTIME" ||
																 con.TextKey == "RPLBLTOTAL" ||
																 con.TextKey == "LBLDRIVERREPORT"
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

			int index = 1;
			if (data != null && data.Count > 0)
			{
				for (int iloop = 0; iloop < data.Count; iloop++)
				{
					DataRow row = dt.NewRow();

					// group row
					row["No"] = index;
					row["RegisteredNo"] = data[iloop].RegisteredNo;
					row["DriverN"] = data[iloop].DriverN;
					row["OpeningPeriod"] = data[iloop].OpeningPeriod ?? 0;
					row["MidPeriod"] = data[iloop].MidPeriod ?? 0;
					row["TotalFuel"] = data[iloop].TotalFuel ?? 0;
					row["ClosingPeriod"] = data[iloop].ClosingPeriod ?? 0;
					dt.Rows.Add(row);
					index++;
				}
			}

			Stream stream = CrystalReport.Service.UseFuel.ExportPdf.Exec(dt, param.Languague, param.DateFrom, param.DateTo, companyName, companyAddress, fileName, user, dicLanguage);
			return stream;
		}

		private List<FuelConsumptionDetailViewModel> GetUseFuelList(FuelConsumptionDetailReportParam param)
		{
			var usefuel = from a in _dispatchRepository.GetAllQueryable()
								  join b in _truckRepository.GetAllQueryable() on a.TruckC equals b.TruckC into t1
								  from b in t1.DefaultIfEmpty()
								  where ((a.TransportD >= param.DateFrom) &
										 (a.TransportD <= param.DateTo) &
										 (string.IsNullOrEmpty(param.TruckC) || param.TruckC == "undefined" || a.TruckC == param.TruckC) &
										 a.DispatchI == "0"
										)
								  group a by new { a.TruckC } into g
								  select new FuelConsumptionDetailViewModel()
								  {
									  TruckC = g.Key.TruckC,
									  TotalFuel = g.Sum(d => d.TotalFuel)
								  };

			usefuel = usefuel.OrderBy("TruckC asc");
			var usefuelList = usefuel.ToList();
			for (var i = 0; i < usefuelList.Count;i++ )
			{
				decimal? prevmidperiod = 0;
				decimal? prevtotalfuel = 0;
				decimal? midperiod = 0;
				string truckC = usefuelList[i].TruckC;
				var truck = _truckRepository.Query(p => p.TruckC == truckC).FirstOrDefault();
				usefuelList[i].RegisteredNo = truck != null ? truck.RegisteredNo : "";
				var driverC = truck.DriverC;
				var driver = _driverRepository.Query(p => p.DriverC == driverC).FirstOrDefault();
				usefuelList[i].DriverN = driver != null ? (driver.LastN + " " + driver.FirstN) : "";
				var truckexpense = _truckExpenseRepository.Query(p => param.DateFrom <= p.TransportD && p.TransportD <= param.DateTo && p.Code == truckC).ToList();
				if (truckexpense.Count > 0)
				{
					for (var loop = 0; loop < truckexpense.Count; loop++)
					{
						string exC = truckexpense[loop].ExpenseC;
						var expense = _expenseRepository.Query(p => p.ExpenseC == exC && p.ExpenseI == "F").FirstOrDefault();
						midperiod = midperiod + (expense != null ? truckexpense[loop].Quantity : 0);
					}
				}
				usefuelList[i].MidPeriod = midperiod;
				//calculate opening period
				prevtotalfuel = _dispatchRepository.Query(p => p.TransportD < param.DateFrom && p.TruckC == truckC).Sum(p => p.TotalFuel);
				var prevtruckexpense = _truckExpenseRepository.Query(p => param.DateFrom < p.TransportD && p.Code == truckC).ToList();
				if (prevtruckexpense.Count > 0)
				{
					for (var iloop = 0; iloop < prevtruckexpense.Count; iloop++)
					{
						string exC = prevtruckexpense[iloop].ExpenseC;
						var expense = _expenseRepository.Query(p => p.ExpenseC == exC && p.ExpenseI == "F").FirstOrDefault();
						prevmidperiod = prevmidperiod + (expense != null ? prevtruckexpense[iloop].Quantity : 0);
					}
				}
				usefuelList[i].OpeningPeriod = prevmidperiod - prevtotalfuel;
				usefuelList[i].ClosingPeriod = usefuelList[i].OpeningPeriod + usefuelList[i].MidPeriod - usefuelList[i].TotalFuel;
			}
			return usefuelList;
		}

		public Stream ExportPdfFuelConsumptionDetail(FuelConsumptionDetailReportParam param, string userName)
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
			var user = GetEmployeeByUserName(userName);
			// get data
			var dt = new FuelConsumptionDetail.FuelConsumptionDetailDataTable();
			int intLanguage;
			var data = GetFuelConsumptionDetailList(param);

			#region setlanguage
			// get language for report
			if (param.Languague == "vi")
			{
				intLanguage = 1;
			}
			else if (param.Languague == "jp")
			{
				intLanguage = 3;
			}
			else
			{
				intLanguage = 2;
			}
			Dictionary<string, string> dicLanguage = new Dictionary<string, string>();

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																 (con.TextKey == "LBLCONTAINERSTATUS1DISPATCH" ||
																 con.TextKey == "LBLCONTAINERSTATUS2DISPATCH" ||
																 con.TextKey == "LBLCONTAINERSTATUS3DISPATCH" ||
																 con.TextKey == "SLCEMPTY" ||
																 con.TextKey == "SLCFILLED" ||
																 con.TextKey == "SLCHEAVY" ||
																 con.TextKey == "SLCLIGHT" ||
																 con.TextKey == "SLCONEWAY" ||
																 con.TextKey == "SLCTWOWAY" ||
																 con.TextKey == "LBLLOAD" ||
																  con.TextKey == "MNUFUELCONSUMPTIONDETAILREPORT" ||
																  con.TextKey == "LBLINSTRUCTIONORP" ||
																  con.TextKey == "LBLTRANSPORTDATEDISPATCHRP" ||
																  con.TextKey == "LBLTRUCKNODISPATCHRP" ||
																  con.TextKey == "LBLDRIVER" ||
																  con.TextKey == "TLTROUTERP" ||
																  con.TextKey == "LBLCONTNUMBER" ||
																  con.TextKey == "LBLREPLACEMENTINTERVALRP" ||
																  con.TextKey == "HDACTUAL1" ||
																  con.TextKey == "LBLDIFFERENCE" ||
																  con.TextKey == "LBLKMS" ||
																  con.TextKey == "LBLLITS" ||
																  con.TextKey == "LBLDISPATCHTOTALRP" ||
																  con.TextKey == "LBLENTRYCLERKN" ||
																  con.TextKey == "RPFTPAGE" ||
																  con.TextKey == "RPFTPRINTBY" ||
																  con.TextKey == "RPFTPRINTTIME" ||
																  con.TextKey == "LBLDISPATCHI" ||
																  con.TextKey == "LBLLOSS"
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

			int index = 1;
			if (data != null && data.Count > 0)
			{
				for (int iloop = 0; iloop < data.Count; iloop++)
				{
					DataRow row = dt.NewRow();

					// group row
					row["No"] = index;
					//row["OrderDate"] = Utilities.GetFormatDateReportByLanguage(data[iloop].OrderD, intLanguage);
					//row["OrderNo"] = data[iloop].OrderNo + "-" + data[iloop].DetailNo;
					//row["OrderType"] = Utilities.GetOrderTypeName(data[iloop].OrderTypeI);
					//row["Customer"] = data[iloop].CustomerShortN != "" ? data[iloop].CustomerShortN : data[iloop].CustomerN;
					//row["DepartmentN"] = data[iloop].DepartmentN;
					//row["BKBL"] = data[iloop].BLBK;
					row["ContainerNo"] = data[iloop].ContainerNo;
					//row["ContainerSize"] = data[iloop].ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] : Utilities.GetContainerSizeName(data[iloop].ContainerSizeI);
					//row["ContainerType"] = data[iloop].ContainerTypeN;
					//row["GrossWeight"] = data[iloop].GrossWeight ?? 0;
					row["TransportD"] = data[iloop].TransportD != null
						? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].TransportD, intLanguage)
						: " ";
					row["RegisteredNo"] = data[iloop].RegisteredNo;
					row["Driver"] = data[iloop].DriverN;
					// set content(containerStatus)
					//if (data[iloop].ContainerStatus == Constants.NORMAL)
					//{
					//	row["Content"] = dicLanguage["LBLCONTAINERSTATUS1DISPATCH"];
					//}
					//else if (data[iloop].ContainerStatus == Constants.LOAD)
					//{
					//	row["Content"] = dicLanguage["LBLCONTAINERSTATUS2DISPATCH"];
					//}
					//else
					//{
					//	row["Content"] = dicLanguage["LBLCONTAINERSTATUS3DISPATCH"];
					//}

					var location = "";
					if (!string.IsNullOrEmpty(data[iloop].Location1N))
					{
						location += data[iloop].Location1N;
					}
					if (!string.IsNullOrEmpty(data[iloop].Location2N))
					{
						location += ", " + data[iloop].Location2N;
					}
					if (!string.IsNullOrEmpty(data[iloop].Location3N))
					{
						location += ", " + data[iloop].Location3N;
					}
					row["Location1"] = location;
					//row["Location2"] = data[iloop].Location2N;
					//row["EstimatedDistance"] = data[iloop].Location3N;
                    row["EstimatedDistance"] = (data[iloop].EstimatedDistance ?? 0) + (data[iloop].ActualDistance ?? 0);
					//row["EstimatedFuel"] = (data[iloop].EstimatedFuel ?? 0) + (data[iloop].ActualFuel ?? 0) +
					//					   ((data[iloop].EstimatedDistance ?? 0) + (data[iloop].ActualDistance ?? 0))*
					//					   (data[iloop].LossFuelRate ?? 0);
					row["EstimatedFuel"] = (data[iloop].TotalFuel ?? 0) - (data[iloop].LossFuelRate ?? 0);
					row["LossFuelRate"] = data[iloop].LossFuelRate ?? 0;
				    row["ActualDistance"] = 0;// data[iloop].ActualDistance ?? 0;
				    row["ActualFuel"] = 0; //data[iloop].ActualFuel ?? 0;
					row["InstructionNo"] = data[iloop].InstructionNo;
					string model = "";
					if (data[iloop].ModelC != null && !string.IsNullOrEmpty(data[iloop].ModelC))
					{
						string mC = data[iloop].ModelC;
						var modelN = _modelRepository.Query(p => p.ModelC == mC).FirstOrDefault();
						model = modelN.ModelN;
					}
					row["ModelN"] = model;
					//if (data[iloop].IsEmpty == "1")
					//{
					//	row["IsEmpty"] = dicLanguage["SLCEMPTY"];
					//}
					//else if (data[iloop].IsEmpty == "0")
					//{
					//	row["IsEmpty"] = dicLanguage["SLCFILLED"];
					//}
					//if (data[iloop].IsHeavy == "1")
					//{
					//	row["IsHeavy"] = dicLanguage["SLCHEAVY"];
					//}
					//else if (data[iloop].IsHeavy == "0")
					//{
					//	row["IsHeavy"] = dicLanguage["SLCLIGHT"];
					//}
					//if (data[iloop].IsSingle == "1")
					//{
					//	row["IsSingle"] = dicLanguage["SLCONEWAY"];
					//}
					//else if (data[iloop].IsSingle == "0")
					//{
					//	row["IsSingle"] = dicLanguage["SLCTWOWAY"];
					//}
					//row["FuelConsumption"] = data[iloop].FuelConsumption;
					//row["UnitPrice"] = data[iloop].UnitPrice;
					//row["Amount"] = data[iloop].Amount;
					dt.Rows.Add(row);
					index++;
				}
			}

			Stream stream = CrystalReport.Service.FuelConsumptionDetail.ExportPdf.Exec(dt, param.Languague, param.DateFrom, param.DateTo, companyName, companyAddress, fileName, user, dicLanguage);
			return stream;
		}
		private List<FuelConsumptionDetailViewModel> GetFuelConsumptionDetailList(FuelConsumptionDetailReportParam param)
		{
			var fuelConsumption = from a in _dispatchRepository.GetAllQueryable()
								  //join m in _fuelConsumptionDetailRepository.GetAllQueryable() on new {a.OrderD,a.OrderNo, a.DetailNo,a.DispatchNo }
								  //	  equals new { m.OrderD, m.OrderNo, m.DetailNo, m.DispatchNo }
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
								  //join f in _customerRepository.GetAllQueryable() on new { e.CustomerMainC, e.CustomerSubC }
								  //	  equals new { f.CustomerMainC, f.CustomerSubC } into t5
								  // from f in t5.DefaultIfEmpty()
								  //join k in _departmentRepository.GetAllQueryable() on e.OrderDepC equals k.DepC into t6
								  // from k in t6.DefaultIfEmpty()
								  //join n in _containerTypeRepository.GetAllQueryable() on d.ContainerTypeC equals n.ContainerTypeC
								  where ((a.TransportD >= param.DateFrom) &
										 (a.TransportD <= param.DateTo) &
										 (string.IsNullOrEmpty(param.TruckC) || param.TruckC == "undefined" || a.TruckC == param.TruckC) &
										 //(string.IsNullOrEmpty(param.DriverC) || param.DriverC == "undefined" || a.DriverC == param.DriverC) &
										 a.DispatchI == "0"
									  //(param.Customer == "null" || (param.Customer).Contains(e.CustomerMainC + "_" + e.CustomerSubC)) &
									  //(string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || b.DepC == param.DepC) &
									  //(string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || e.OrderTypeI == param.OrderTypeI)
										)
								  select new FuelConsumptionDetailViewModel()
								  {
									  //OrderD = a.OrderD,
									  //OrderNo = a.OrderNo,
									  //DetailNo = a.DetailNo,
									  //DispatchNo = a.DispatchNo,
									  TransportD = a.TransportD,
									  //ContainerStatus = a.ContainerStatus,
									  //TruckC = a.TruckC,
									  RegisteredNo = (a.RegisteredNo != null && a.RegisteredNo != "") ? a.RegisteredNo : (b != null ? b.RegisteredNo : ""),
									  DriverN = c != null ? c.LastN + " " + c.FirstN : "",
									  //ApproximateDistance = m.Distance,
									  EstimatedDistance = a.ApproximateDistance,
									  EstimatedFuel = a.FuelConsumption,
									  ActualDistance = a.ActualDistance,
									  ActualFuel = a.ActualFuel,
									  ContainerNo = d.ContainerNo,
									  //OrderTypeI = e.OrderTypeI,
									  //DepartmentN = k.DepN,
									  //BLBK = e.BLBK,
									  //CustomerShortN = f != null ? (f.CustomerShortN ?? "") : "",
									  //CustomerN = f != null ? f.CustomerN : "",
									  //GrossWeight = d.GrossWeight,
									  ContainerSizeI = d.ContainerSizeI,
									  //ContainerTypeN = n.ContainerTypeN,
									  Location1N = a.Location1N,
									  Location2N = a.Location2N,
									  Location3N = a.Location3N,
									  InstructionNo = a.InstructionNo,
									  LossFuelRate = a.LossFuelRate ?? null,
									  ModelC = b.ModelC ?? "",
									  TotalFuel = a.TotalFuel ?? null
									  //IsEmpty = m.IsEmpty,
									  //IsHeavy = m.IsHeavy,
									  //IsSingle = m.IsSingle,
									  //FuelConsumption = a.FuelConsumption,
									  //UnitPrice = m.UnitPrice,
									  //Amount = m.Amount,
								  };

			fuelConsumption = fuelConsumption.OrderBy("TransportD asc, RegisteredNo asc, DriverN asc");

			var fuelConsumptionList = fuelConsumption.ToList();

			return fuelConsumptionList;
		}

		public Stream ExportPdfTransportMaintenance(MaintenanceReportParam param, string userName)
		{
			Stream stream;
			DataRow row;
			MaintenanceList.TransportDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			var companyName = "";
			var companyAddress = "";
			var fileName = "";

			// get data
			dt = new MaintenanceList.TransportDataTable();
			List<TransportMaintenanceReportData> data = GetTransportMaintenanceList(param);
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
			// get language for report
			dicLanguage = new Dictionary<string, string>();
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
																 (con.TextKey == "LBLMAINTENANCEPLANREPORT" ||
																  con.TextKey == "LBLMAINTENANCEFINISHEDREPORT" ||
																  con.TextKey == "LBLREMODELNO" ||
																  con.TextKey == "LBLREMODELYES" ||
																  con.TextKey == "LBLMAINTENANCEPLANREPORT" ||
																  con.TextKey == "LBLMAINTENANCEFINISHEDREPORT" ||
																  con.TextKey == "LBLCHECKREPORT" ||
																  con.TextKey == "LBLREPLACEREPORT" ||
																  con.TextKey == "LBLALLREPORT" ||
																  con.TextKey == "LBLDATE" ||
																  con.TextKey == "LBLMAINTENANCE" ||
																  con.TextKey == "LBLREPAIR"
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
					row["Code"] = data[iloop].RegisteredNo;
					row["Month"] = data[iloop].Month;
					row["TransportCount"] = data[iloop].TransportCount;
					row["TotalKm"] = data[iloop].TotalKm;
					row["MaintenanceAmount"] = data[iloop].MaintenanceAmount;
					dt.Rows.Add(row);
				}
			}

			stream = CrystalReport.Service.TransportMaintenance.ExportPdf.Exec(dt, intLanguage, param.Year, companyName, companyAddress, fileName, user);
			return stream;
		}
		private List<TransportMaintenanceReportData> GetTransportMaintenanceList(MaintenanceReportParam param)
		{
			var transportList = from d in _dispatchRepository.GetAllQueryable()
								where (param.TruckC.Equals("null") || param.TruckC.Contains(d.TruckC)) &&
									d.DispatchI == "0" &&
									d.TransportD.Value.Year == param.Year
								group d by new { d.TruckC, d.TransportD.Value.Month } into g
								select new TransportMaintenanceReportData()
								{
									TruckC = g.Key.TruckC,
									RegisteredNo = "",
									Month = g.Key.Month,
									TransportCount = g.Count(),
									TotalKm = g.Sum(i => i.ActualDistance ?? 0),
									MaintenanceAmount = 0
								};
			var inspectionList = from i in _inspectionDetailRepository.GetAllQueryable()
								 where (param.TruckC.Equals("null") || param.TruckC.Contains(i.Code)) &&
									i.ObjectI == "0" &&
									i.InspectionD.Year == param.Year
								 group i by new { i.Code, i.InspectionD.Month } into g
								 select new TransportMaintenanceReportData()
								 {
									 TruckC = g.Key.Code,
									 RegisteredNo = "",
									 Month = g.Key.Month,
									 TransportCount = 0,
									 TotalKm = 0,
									 MaintenanceAmount = g.Sum(d => d.Total)
								 };
			var truckExpenseList = from e in _truckExpenseRepository.GetAllQueryable()
								   join x in _expenseRepository.GetAllQueryable() on e.ExpenseC equals x.ExpenseC
								   where (param.TruckC.Equals("null") || param.TruckC.Contains(e.Code)) &&
									 e.ObjectI == "0" &&
									 e.InvoiceD.Year == param.Year &&
									 x.ExpenseI.Equals("M")
								   group e by new { e.Code, e.InvoiceD.Month } into g
								   select new TransportMaintenanceReportData()
								   {
									   TruckC = g.Key.Code,
									   RegisteredNo = "",
									   Month = g.Key.Month,
									   TransportCount = 0,
									   TotalKm = 0,
									   MaintenanceAmount = g.Sum(d => d.Total)
								   };
			var union = from d in transportList.Concat(inspectionList).Concat(truckExpenseList)
						group d by new { d.TruckC, d.Month } into g
						select new
						{
							TruckC = g.Key.TruckC,
							RegisteredNo = "",
							Month = g.Key.Month,
							TransportCount = g.Sum(d => d.TransportCount),
							TotalKm = g.Sum(d => d.TotalKm),
							MaintenanceAmount = g.Sum(d => d.MaintenanceAmount)
						};
			var result = from t in _truckRepository.GetAllQueryable()
						 join u in union
						 on t.TruckC equals u.TruckC
						 select new TransportMaintenanceReportData
						 {
							 TruckC = u.TruckC,
							 RegisteredNo = t.RegisteredNo,
							 Month = u.Month,
							 TransportCount = u.TransportCount,
							 TotalKm = u.TotalKm,
							 MaintenanceAmount = u.MaintenanceAmount
						 };

			var dt = result.ToList();
			if (dt.Count > 0)
			{
				var truckNo = dt[0].TruckC;
				var registeredNo = dt[0].RegisteredNo;
				var checkTruck = dt.Where(t => t.TruckC == truckNo);
				for (int i = 1; i <= 12; i++)
				{
					var check = checkTruck.Where(t => t.Month == i).FirstOrDefault();
					if (check == null)
					{
						dt.Add(new TransportMaintenanceReportData
						{
							TruckC = truckNo,
							RegisteredNo = registeredNo,
							Month = i,
							TransportCount = 0,
							TotalKm = 0,
							MaintenanceAmount = 0
						});
					}
				}
			}
			return dt;
		}

		public Stream ExportPdfTransportHandover(DispatchViewModel param)
		{
			Stream stream;
			var companyName = "";
			var companyAddress = "";
			var companyTaxCode = "";
			var userContactName = "";
			var phoneNumber1 = "";
			var phoneNumber2 = "";
			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				companyTaxCode = basicSetting.TaxCode;
				userContactName = basicSetting.ContactPerson ?? string.Empty;
				phoneNumber1 = basicSetting.PhoneNumber1 ?? string.Empty;
				phoneNumber2 = basicSetting.PhoneNumber2 ?? string.Empty;
				//fileName = basicSetting.Logo;
			}

			//get data
			//var customerCode = "";
			//var orderDate = "";
			//var location1 = "";
			//var location2 = "";
			//var location3 = "";
			//var containerNo = "";
			//var sealNo = "";
			//var commodity = "";
			//var pickupReturnDate = "";
			//var contactAddress = "";
			var data = (from d in _dispatchRepository.GetAllQueryable()
						join o in _orderDRepository.GetAllQueryable()
						on new { d.OrderD, d.OrderNo, d.DetailNo } equals new { o.OrderD, o.OrderNo, o.DetailNo }
						join h in _orderHRepository.GetAllQueryable()
						on new { d.OrderD, d.OrderNo } equals new { h.OrderD, h.OrderNo }
						join c in _customerRepository.GetAllQueryable()
						on new { h.CustomerMainC, h.CustomerSubC } equals new { c.CustomerMainC, c.CustomerSubC }
						//join l in _locationRepository.GetAllQueryable() on h.StopoverPlaceC equals  l.LocationC
						where (d.OrderD == param.OrderD &&
								  d.OrderNo == param.OrderNo &&
								  d.DispatchNo == param.DispatchNo &&
								  d.DetailNo == param.DetailNo)
						select new TransportConfirmViewModel()
						{
							TransportConfirmOrder = new TransportConfirmOrderViewModel()
							{
								CustomerAddress = c.Address1,
								CustomerTaxCode = c.TaxCode,
								IsCollected = h.IsCollected,
								CustomerN = c.CustomerN,
								OrderD = h.OrderD,
								ShipperN = h.ShipperN,
								//DeliveryContact = l.Description,
								//LoadingPlaceN = h.OrderTypeI == "1" ? h.LoadingPlaceN: h.StopoverPlaceN,
								//StopoverPlaceN = h.OrderTypeI == "1" ? h.StopoverPlaceN : h.DischargePlaceN,
								//DischargePlaceN = h.OrderTypeI == "1" ? h.DischargePlaceN: h.LoadingPlaceN,
								//StopoverDT = h.OrderTypeI == "1" ? h.StopoverDT : h.DischargeDT,
								LoadingPlaceN = h.LoadingPlaceN,//1
								StopoverPlaceN = h.StopoverPlaceN,//2
								DischargePlaceN = h.DischargePlaceN,//3
								StopoverDT = h.StopoverDT,
							},
							TransportConfirmContainer = new TransportConfirmContainerViewModel()
							{
								ContainerNo = o.ContainerNo,
								SealNo = o.SealNo,
								CommodityN = o.CommodityN,
							},
						}).ToList();
			var dt = new TransportConfirmViewModel();
			if (data != null && data.Count > 0)
			{
				dt = data.FirstOrDefault();

				//var customerCode = dt.TransportConfirmOrder.CustomerMainC;
				//var orderDate = "";
				//var location1 = "";
				//var location2 = "";
				//var location3 = "";
				//var containerNo = "";
				//var sealNo = "";
				//var commodity = "";
				//var pickupReturnDate = "";
				//var contactAddress = "";
			}
			stream = CrystalReport.Service.Dispatch.ExportPdf.ExecTransportHandover(dt, companyName, companyAddress,
				companyTaxCode, userContactName, phoneNumber1, phoneNumber2);
			return stream;
		}

		public Stream ExportPdfTransportInstruction(DispatchViewModel param)
		{
			Stream stream;
			var companyName = "";
			var companyAddress = "";
			var companyTel = "";

			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				companyTel = basicSetting.PhoneNumber1;
				//fileName = basicSetting.Logo;
			}

			//get data
			var customerCode = "";
			var driverName = "";
			var licenseNo = "";
			var driverTel = "";
			var route = "";
			var commodity = "";
			var trailerNo = "";
			var truckNo = "";
			var location2export = "";
			var location2import = "";
			var instructionNo = "";
			//var currentDate = "";
			var data = (from d in _dispatchRepository.GetAllQueryable()
						join o in _orderDRepository.GetAllQueryable()
						on new { d.OrderD, d.OrderNo, d.DetailNo } equals new { o.OrderD, o.OrderNo, o.DetailNo }
						join h in _orderHRepository.GetAllQueryable()
						on new { d.OrderD, d.OrderNo } equals new { h.OrderD, h.OrderNo }
						join v in _driverRepository.GetAllQueryable()
						on new { d.DriverC } equals new { v.DriverC }
						join t in _truckRepository.GetAllQueryable()
						on new { d.TruckC } equals new { t.TruckC } into t1
						from t in t1.DefaultIfEmpty()
						join r in _trailerRepository.GetAllQueryable()
						on new { o.TrailerC } equals new { r.TrailerC } into t2
						from r in t2.DefaultIfEmpty()
						join c in _customerRepository.GetAllQueryable()
						on new { h.CustomerMainC, h.CustomerSubC } equals new { c.CustomerMainC, c.CustomerSubC }
						where (d.OrderD == param.OrderD &&
								 d.OrderNo == param.OrderNo &&
								 d.DispatchNo == param.DispatchNo &&
								 d.DetailNo == param.DetailNo)
						select new
						{
							CustomerN = c.CustomerN,
							CommodityN = o.CommodityN,
							TrailerNo = r != null ? r.TrailerNo : "",
							DriverC = d.DriverC,
							DriverN = v.LastN + " " + v.FirstN,
							DriverTel = v.PhoneNumber,
							Location1N = d.Location1N,
							Location2N = d.Location2N,
							Location3N = d.Location3N,
							TruckNo = t != null ? t.RegisteredNo : "",
							OrderTypeI = h.OrderTypeI,
							InstructionNo = d.InstructionNo,
						}).ToList();
			if (data != null && data.Count > 0)
			{
				var dt = data.FirstOrDefault();
				customerCode = dt.CustomerN ?? "";
				driverName = dt.DriverN ?? "";

				var driverC = dt.DriverC;
				var licenseInfo = (from l in _driverLicenseRepository.GetAllQueryable()
								   where l.DriverC == driverC
								   select new DriverLicenseViewModel()
								   {
									   DriverLicenseNo = l.DriverLicenseNo,
									   ExpiryD = l.ExpiryD,
								   }).OrderBy("ExpiryD desc").ToList().FirstOrDefault();

				if (licenseInfo != null)
					licenseNo = licenseInfo.DriverLicenseNo;

				driverTel = dt.DriverTel ?? "";

				if (dt.Location1N != null && dt.Location1N.Length > 0)
					route += dt.Location1N + ", ";
				if (dt.Location2N != null && dt.Location2N.Length > 0)
					route += dt.Location2N + ", ";
				if (dt.Location3N != null && dt.Location3N.Length > 0)
					route += dt.Location3N + ", ";

				if (route.Length > 1)
					route = route.Remove(route.Length - 2);

				commodity = dt.CommodityN ?? "";
				trailerNo = dt.TrailerNo ?? "";
				truckNo = dt.TruckNo ?? "";
				location2export = dt.OrderTypeI != "1" ? dt.Location2N : "";
				location2import = dt.OrderTypeI == "1" ? dt.Location2N : (dt.OrderTypeI == "2" ? dt.Location3N : "");
				instructionNo = dt.InstructionNo ?? "";
			}
			stream = CrystalReport.Service.Dispatch.ExportPdf.ExecTransportInstruction(companyName, companyAddress, companyTel, customerCode,
				driverName, licenseNo, driverTel, route, commodity, trailerNo, truckNo, location2export, location2import, instructionNo);
			return stream;
		}
	}
}
