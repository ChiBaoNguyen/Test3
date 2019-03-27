using System.Security.Cryptography;
using AutoMapper;
using CrystalReport.Dataset.CustomerExpense;
using CrystalReport.Dataset.CustomerRevenue;
using CrystalReport.Dataset.CustomersExpense;
using CrystalReport.Dataset.Dispatch;
using CrystalReport.Dataset.DriverAllowance;
using CrystalReport.Dataset.DriverDispatch;
using CrystalReport.Dataset.DriverRevenue;
using CrystalReport.Dataset.Expense;
using CrystalReport.Dataset.PartnerCustomer;
using CrystalReport.Dataset.PartnerExpense;
using CrystalReport.Dataset.SupplierExpense;
using CrystalReport.Dataset.TransportExpense;
using CrystalReport.Dataset.TruckBalance;
using CrystalReport.Dataset.TruckExpense;
using CrystalReport.Dataset.TruckRevenue;
using CrystalReport.Service.Dispatch;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading;
using Website.Enum;
using Website.Utilities;
using Website.ViewModels.InvoiceInfo;

namespace Service.Services
{
	public interface IInvoiceInfoService
	{
		InvoiceInfotViewModel GetLimitStartAndEndInvoiceD(string invoiceMainC, string invoiceSubC, int invoiceMonth, int invoiceYear);
		InvoiceInfotViewModel GetLimitStartAndEndInvoiceDPartner(string invoiceMainC, string invoiceSubC, int invoiceMonth, int invoiceYear);
		InvoiceInfotViewModel GetLimitStartAndEndInvoiceDSelf(int invoiceMonth, int invoiceYear);
	}

	public class InvoiceInfoService : IInvoiceInfoService
	{
		private readonly ICustomerSettlementRepository _customerSettlementRepository;
		private readonly IPartnerSettlementRepository _partnerSettlementRepository;
		private readonly IBasicRepository _basicRepository;

		private readonly IUnitOfWork _unitOfWork;

		public InvoiceInfoService(ICustomerSettlementRepository customerSettlementRepository,
								  IPartnerSettlementRepository partnerSettlementRepository,
								  IBasicRepository basicRepository,
								  IUnitOfWork unitOfWork
								 )
		{
			this._customerSettlementRepository = customerSettlementRepository;
			this._partnerSettlementRepository = partnerSettlementRepository;
			this._basicRepository = basicRepository;
			this._unitOfWork = unitOfWork;
		}

		public InvoiceInfotViewModel GetLimitStartAndEndInvoiceD(string invoiceMainC, string invoiceSubC, int invoiceMonth, int invoiceYear)
		{
			DateTime? startDate = null;
			DateTime? endDate = null;
			var settlement = new CustomerSettlement_M();
			var result = new InvoiceInfotViewModel();

			var customerSettlement = _customerSettlementRepository.Query(cus => cus.CustomerMainC == invoiceMainC &&
																				cus.CustomerSubC == invoiceSubC &&
																				cus.ApplyD.Year <= invoiceYear)
																  .OrderBy("ApplyD desc");

			if (customerSettlement.Any())
			{
				var customerSettlementList = customerSettlement.ToList();

				if (customerSettlementList[0].ApplyD.Year < invoiceYear)
				{
					settlement = customerSettlementList[0];
				}
				else
				{
					for (var iloop = 0; iloop < customerSettlementList.Count; iloop++)
					{
						if (customerSettlementList[iloop].ApplyD.Month <= invoiceMonth)
						{
							settlement = customerSettlementList[iloop];
							break;
						}

						settlement = null;
					}
				}

				// check settlement
				if (settlement != null)
				{
					// set start date
					if (settlement.ApplyD.Month == invoiceMonth && settlement.ApplyD.Year == invoiceYear)
					{
						startDate = settlement.ApplyD;
					}
					else
					{
						if (settlement.SettlementD == 31)
						{
							startDate = new DateTime(invoiceYear, invoiceMonth, 1);
						}
						else
						{
							startDate = new DateTime(invoiceYear, invoiceMonth, 1);
							//startDate = new DateTime(invoiceYear, invoiceMonth, settlement.SettlementD + 1);
						}
					}
					// set end date
					if (settlement.SettlementD == 31)
					{
						endDate = new DateTime(invoiceYear, invoiceMonth, DateTime.DaysInMonth(invoiceYear, invoiceMonth));
					}
					else
					{
						if (invoiceMonth == 12)
						{
							endDate = new DateTime(invoiceYear + 1, 1, settlement.SettlementD);
						}
						else
						{
							endDate = new DateTime(invoiceYear, invoiceMonth, DateTime.DaysInMonth(invoiceYear, invoiceMonth));
							//endDate = new DateTime(invoiceYear, invoiceMonth + 1, settlement.SettlementD);
						}
					}
				}
			}

			if (startDate == null)
			{
				startDate = new DateTime(invoiceYear, invoiceMonth, 1);
				endDate = new DateTime(invoiceYear, invoiceMonth, DateTime.DaysInMonth(invoiceYear, invoiceMonth));
			}

			result.CustomerMainC = invoiceMainC;
			result.CustomerSubC = invoiceSubC;
			result.StartDate = (DateTime)startDate;
			result.EndDate = (DateTime)endDate;
			result.SettlementD = 31;
			result.TaxMethodI = "0";
			result.TaxRate = 0;
			result.TaxRoundingI = "0";
			result.RevenueRoundingI = "0";
			if (settlement != null)
			{
				result.ApplyD = settlement.ApplyD;
				result.SettlementD = settlement.SettlementD;
				result.TaxMethodI = settlement.TaxMethodI;
				result.TaxRate = (settlement.TaxRate != null ? (decimal)settlement.TaxRate : 0);
				result.TaxRoundingI = settlement.TaxRoundingI;
				result.RevenueRoundingI = settlement.RevenueRoundingI;
			}

			return result;
		}

		public InvoiceInfotViewModel GetLimitStartAndEndInvoiceDPartner(string invoiceMainC, string invoiceSubC, int invoiceMonth, int invoiceYear)
		{
			DateTime? startDate = null;
			DateTime? endDate = null;
			var settlement = new PartnerSettlement_M();
			var result = new InvoiceInfotViewModel();

			var partnerSettlement = _partnerSettlementRepository.Query(par => par.PartnerMainC == invoiceMainC &&
																			  par.PartnerSubC == invoiceSubC &&
																			  par.ApplyD.Year <= invoiceYear)
																  .OrderBy("ApplyD desc");

			if (partnerSettlement.Any())
			{
				var partnerSettlementList = partnerSettlement.ToList();

				if (partnerSettlementList[0].ApplyD.Year < invoiceYear)
				{
					settlement = partnerSettlementList[0];
				}
				else
				{
					for (var iloop = 0; iloop < partnerSettlementList.Count; iloop++)
					{
						if (partnerSettlementList[iloop].ApplyD.Month <= invoiceMonth)
						{
							settlement = partnerSettlementList[iloop];
							break;
						}

						settlement = null;
					}
				}

				// check settlement
				if (settlement != null)
				{
					// set start date
					if (settlement.ApplyD.Month == invoiceMonth && settlement.ApplyD.Year == invoiceYear)
					{
						startDate = settlement.ApplyD;
					}
					else
					{
						if (settlement.SettlementD == 31)
						{
							startDate = new DateTime(invoiceYear, invoiceMonth, 1);
						}
						else
						{
							startDate = new DateTime(invoiceYear, invoiceMonth, settlement.SettlementD + 1);
						}
					}
					// set end date
					if (settlement.SettlementD == 31)
					{
						endDate = new DateTime(invoiceYear, invoiceMonth, DateTime.DaysInMonth(invoiceYear, invoiceMonth));
					}
					else
					{
						if (invoiceMonth == 12)
						{
							endDate = new DateTime(invoiceYear + 1, 1, settlement.SettlementD);
						}
						else
						{
							endDate = new DateTime(invoiceYear, invoiceMonth + 1, settlement.SettlementD);
						}
					}
				}
			}

			if (startDate == null)
			{
				startDate = new DateTime(invoiceYear, invoiceMonth, 1);
				endDate = new DateTime(invoiceYear, invoiceMonth, DateTime.DaysInMonth(invoiceYear, invoiceMonth));
			}

			result.PartnerMainC = invoiceMainC;
			result.PartnerSubC = invoiceSubC;
			result.StartDate = (DateTime)startDate;
			result.EndDate = (DateTime)endDate;
			result.SettlementD = 31;
			result.TaxMethodI = "0";
			result.TaxRate = 0;
			result.TaxRoundingI = "0";
			result.RevenueRoundingI = "0";
			if (settlement != null)
			{
				result.ApplyD = settlement.ApplyD;
				result.SettlementD = settlement.SettlementD;
				result.TaxMethodI = settlement.TaxMethodI;
				result.TaxRate = (settlement.TaxRate != null ? (decimal)settlement.TaxRate : 0);
				result.TaxRoundingI = settlement.TaxRoundingI;
				result.RevenueRoundingI = settlement.RevenueRoundingI;
			}

			return result;
		}

		public InvoiceInfotViewModel GetLimitStartAndEndInvoiceDSelf(int invoiceMonth, int invoiceYear)
		{
			DateTime? startDate = null;
			DateTime? endDate = null;
			var result = new InvoiceInfotViewModel();

			var basic = _basicRepository.GetAllQueryable().FirstOrDefault();

			if (basic != null)
			{
				// set startDate
				if (basic.SettlementDay == 31)
				{
					startDate = new DateTime(invoiceYear, invoiceMonth, 1);
				}
				else
				{
					startDate = new DateTime(invoiceYear, invoiceMonth, basic.SettlementDay + 1);
				}

				// set end date
				if (basic.SettlementDay == 31)
				{
					endDate = new DateTime(invoiceYear, invoiceMonth, DateTime.DaysInMonth(invoiceYear, invoiceMonth));
				}
				else
				{
					if (invoiceMonth == 12)
					{
						endDate = new DateTime(invoiceYear + 1, 1, basic.SettlementDay);
					}
					else
					{
						endDate = new DateTime(invoiceYear, invoiceMonth + 1, basic.SettlementDay);
					}
				}
			}

			if (startDate == null)
			{
				startDate = new DateTime(invoiceYear, invoiceMonth, 1);
				endDate = new DateTime(invoiceYear, invoiceMonth, DateTime.DaysInMonth(invoiceYear, invoiceMonth));
			}

			result.StartDate = (DateTime)startDate;
			result.EndDate = (DateTime)endDate;
			result.SettlementD = 31;
			result.TaxMethodI = "0";
			result.TaxRate = 0;
			result.TaxRoundingI = "0";
			result.RevenueRoundingI = "0";
			if (basic != null)
			{
				result.SettlementD = basic.SettlementDay;
				result.TaxMethodI = basic.TaxMethodI;
				result.TaxRate = (decimal)basic.TaxRate;
				result.TaxRoundingI = basic.TaxRoundingI;
			}

			return result;
		}
	}
}