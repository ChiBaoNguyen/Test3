using System.Net.Sockets;
using AutoMapper.Internal;
using CrystalReport.Dataset.CustomerExpense;
using CrystalReport.Dataset.CustomerRevenue;
using CrystalReport.Dataset.Dispatch;
using CrystalReport.Dataset.DriverAllowance;
using CrystalReport.Dataset.DriverDispatch;
using CrystalReport.Dataset.DriverRevenue;
using CrystalReport.Dataset.Expense;
using CrystalReport.Dataset.Maintenance;
using CrystalReport.Dataset.PartnerCustomer;
using CrystalReport.Dataset.PartnerExpense;
using CrystalReport.Dataset.SupplierExpense;
using CrystalReport.Dataset.TransportExpense;
using CrystalReport.Dataset.TruckBalance;
using CrystalReport.Dataset.TruckExpense;
using CrystalReport.Dataset.TruckRevenue;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.InteropServices.ComTypes;
using AutoMapper;
using Root.Models.Mapping;
using Website.Enum;
using Website.Utilities;
using Website.ViewModels.Container;
using Website.ViewModels.Dispatch;
using Website.ViewModels.DriverAllowance;
using Website.ViewModels.Expense;
using Website.ViewModels.Order;
using Website.ViewModels.Report.CustomerExpense;
using Website.ViewModels.Report.CustomerRevenue;
using Website.ViewModels.Report.DriverRevenue;
using Website.ViewModels.Report.Maintenance;
using Website.ViewModels.Report.PartnerCustomer;
using Website.ViewModels.Report.PartnerExpense;
using Website.ViewModels.Report.SupplierExpense;
using Website.ViewModels.Report.TruckBalance;
using Website.ViewModels.Report.TruckExpense;
using Website.ViewModels.Report.TruckRevenue;
using Website.ViewModels.Surcharge;
using Website.ViewModels.TruckExpense;
using CrystalReport.Dataset.CustomerBalance;
using CrystalReport.Dataset.Order;
using Website.ViewModels.Customer;
using Website.ViewModels.Report.CustomerBalance;
using CALLCONV = System.Runtime.InteropServices.CALLCONV;
using Website.ViewModels.Liabilities;
using Website.ViewModels.CalculateDriverAllowance;
using Newtonsoft.Json;

namespace Service.Services
{
	public partial interface IReportService
	{
		Stream ExportExcelTransportationPlan(DispatchReportParam param, string userName);
		Stream ExportExcel(DispatchReportParam param, string userName);
		Stream ExportPdf(DispatchReportParam param, string userName);
		Stream ExportPdf(DriverDispatchReportParam param, string userName);
		Stream ExportExcelCustomerExpense(CustomerExpenseReportParam param);
		Stream ExportExcelCustomerExpenseLoad(CustomerExpenseReportParam param, string userName);
		Stream ExportExcelCustomerExpenseGeneral(CustomerExpenseReportParam param);
		Stream ExportExcelCustomerExpenseTransportFeeVertical(CustomerExpenseReportParam param);
		Stream ExportExcelCustomerExpenseTransportFeeHorizontal(CustomerExpenseReportParam param);
		Stream ExportExcelCustomerExpenseLOLOHorizontal(CustomerExpenseReportParam param);
		Stream ExportExcelCustomerExpenseLOLOVertical(CustomerExpenseReportParam param);
		int UpdateInvoiceStatus(CustomerExpenseReportParam param);
		Stream ExportPdfCustomerExpense(CustomerExpenseReportParam param);
		Stream ExportPdfCustomerExpenseLoad(CustomerExpenseReportParam param, string userName);
		Stream ExportPdfDriverAllowance(DriverAllowanceReportParam param, string userName);
		Stream ExportPdfDriverSalary(DriverAllowanceReportParam param, string userName);
		List<DriverAllowanceListReport> GetDriverAllowanceList(DriverAllowanceReportParam param);
		//Stream ExportPdfOrder(OrderReportParam param);
		Stream ExportExcelOrderGeneral(OrderReportParam param, string username);
		Stream ExportPdfOrderGeneral(OrderReportParam param, string username);
		Stream ExportExcelExpense(ExpenseReportParam param, string userName);
		Stream ExportPdfExpense(ExpenseReportParam param, string userName);
		Stream ExportPdfCustomersExpense(CustomerExpenseReportParam param);
		Stream ExportPdfCustomerRevenue(CustomerExpenseReportParam param);
		Stream ExportPdfTruckRevenueDetail(TruckRevenueReportParam param);
		Stream ExportPdfTruckRevenueGeneral(TruckRevenueReportParam param);
		Stream ExportPdfTruckBalance(TruckBalanceReportParam param, string userName);
		Stream ExportPdfDriverRevenueDetail(DriverRevenueReportParam param);
		Stream ExportPdfDriverRevenueGeneral(DriverRevenueReportParam param);
		Stream ExportPdfPartnerCustomerExpense(PartnerCustomerExpenseReportParam param);
		Stream ExportPdfPartnerExpense(PartnerExpenseReportParam param);
		Stream ExportPdfSupplierExpense(SupplierExpenseReportParam param);
		Stream ExportPdfTruckExpense(TruckExpenseReportParam param);
		Stream ExportPdfCustomerBalance(CustomerExpenseReportParam param, string userName);
	}
	public partial class ReportService : IReportService
	{
		#region init
		private readonly IDispatchRepository _dispatchRepository;
		private readonly IContainerRepository _orderDRepository;
		private readonly IOrderRepository _orderHRepository;
		private readonly ICustomerRepository _customerRepository;
		private readonly IDepartmentRepository _departmentRepository;
		private readonly IContainerTypeRepository _containerTypeRepository;
		private readonly ITrailerRepository _trailerRepository;
		private readonly ITruckRepository _truckRepository;
		private readonly IDriverRepository _driverRepository;
		private readonly IPartnerRepository _partnerRepository;
		private readonly ITextResourceRepository _textResourceRepository;
		private readonly IExpenseRepository _expenseRepository;
		private readonly IExpenseDetailRepository _expenseDetailRepository;
		private readonly ISurchargeDetailRepository _surchargeDetailRepository;
		private readonly IAllowanceDetailRepository _allowanceDetailRepository;
		private readonly IShippingCompanyRepository _shippingCompanyRepository;
		private readonly ICustomerSettlementRepository _customerSettlementRepository;
		private readonly IPartnerSettlementRepository _partnerSettlementRepository;
		private readonly IBasicRepository _basicRepository;
		private readonly IFixedExpenseRepository _fixedExpenseRepository;
		private readonly ISupplierRepository _supplierRepository;
		private readonly ITruckExpenseRepository _truckExpenseRepository;
		private readonly IExpenseCategoryRepository _expenseCategoryRepository;
		private readonly IInspectionRepository _inspectionRepository;
		private readonly IInspectionPlanDetailRepository _inspectionPlanDetailRepository;
		private readonly IModelRepository _modelRepository;
		private readonly IMaintenancePlanDetailRepository _maintenancePlanDetailRepository;
		private readonly IMaintenanceItemRepository _maintenanceItemRepository;
		private readonly IInspectionDetailRepository _inspectionDetailRepository;
		private readonly IMaintenanceDetailRepository _maintenanceDetailRepository;
		private readonly ILiabilitiesRepository _liabilitiesRepository;
		private readonly ICalculateDriverAllowanceRepository _calculateDriverAllowanceRepository;
		private readonly ILiabilitiesItemRepository _liabilitiesItemRepository;
		private readonly IFuelConsumptionDetailRepository _fuelConsumptionDetailRepository;
		private readonly ICustomerPaymentRepository _customerPaymentRepository;
		private readonly ICustomerBalanceRepository _customerBalanceRepository;
		private readonly ICustomerPricingRepository _customerPricingRepository;
		private readonly ICustomerPricingDetailRepository _customerPricingDetailRepository;
		private readonly ILocationRepository _locationRepository;
		private readonly IOperationRepository _operationRepository;
		private readonly IPartnerBalanceRepository _partnerBalanceRepository;
		private readonly IPartnerPaymentRepository _partnerPaymentRepository;
		private readonly IDriverAllowanceRepository _driverAllowanceRepository;
		private readonly IUnitOfWork _unitOfWork;
		private IInvoiceInfoService _invoiceInfoService;
		private ICustomerService _customerService;
		private IPartnerService _partnerService;
		private ILocationService _locationService;
		private readonly IUserRepository _userRepository;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly ICompanyExpenseRepository _companyExpenseRepository;
		private readonly IDriverLicenseRepository _driverLicenseRepository;

		public ReportService(IDispatchRepository dispatchRepository,
							 IContainerRepository orderDRepository,
							 IOrderRepository orderHRepository,
							 ICustomerRepository customerRepository,
							 IDepartmentRepository departmentRepository,
							 IContainerTypeRepository containerTypeRepository,
							 ITrailerRepository trailerRepository,
							 ITruckRepository truckRepository,
							 IDriverRepository driverRepository,
							 IPartnerRepository partnerRepository,
							 IShippingCompanyRepository shippingCompanyRepository,
							 ITextResourceRepository textResourceRepository,
							 IExpenseRepository expenseRepository,
							 IExpenseDetailRepository expenseDetailRepository,
							 ISurchargeDetailRepository surchargeDetailRepository,
							 IAllowanceDetailRepository allowanceDetailRepository,
							 ICustomerSettlementRepository customerSettlementRepository,
							 IPartnerSettlementRepository partnerSettlementRepository,
							 IBasicRepository basicRepository,
							 IFixedExpenseRepository fixedExpenseRepository,
							 ITruckExpenseRepository truckExpenseRepository,
							 ISupplierRepository supplierRepository,
							 IExpenseCategoryRepository expenseCategoryRepository,
							 IInvoiceInfoService invoiceInfoService,
							 ICustomerService customerService,
							 IPartnerService partnerService,
							 ILocationService locationService,
							 IUnitOfWork unitOfWork,
							 IInspectionRepository inspectionRepository,
							 IInspectionPlanDetailRepository inspectionPlanDetailRepository,
							IModelRepository modelRepository,
							IMaintenancePlanDetailRepository maintenancePlanDetailRepository,
							IMaintenanceItemRepository maintenanceItemRepository,
							IInspectionDetailRepository inspectionDetailRepository,
							IMaintenanceDetailRepository maintenanceDetailRepository,
							ILiabilitiesRepository liabilitiesRepository,
							ICalculateDriverAllowanceRepository calculateDriverAllowanceRepository,
							IFuelConsumptionDetailRepository fuelConsumptionDetailRepository,
							ICustomerPaymentRepository customerPaymentRepository,
							ICustomerBalanceRepository customerBalanceRepository,
							ICustomerPricingRepository customerPricingRepository,
							ICustomerPricingDetailRepository customerPricingDetailRepository,
							ILocationRepository locationRepository,
							IOperationRepository operationRepository,
							IPartnerBalanceRepository partnerBalanceRepository,
							IPartnerPaymentRepository partnerPaymentRepository,
							ILiabilitiesItemRepository liabilitiesItemRepository,
							IUserRepository userRepository,
							IEmployeeRepository employeeRepository,
							ICompanyExpenseRepository companyExpenseRepository,
							IDriverLicenseRepository driverLicenseRepository,
							IDriverAllowanceRepository driverAllowanceRepository
			)
		{
			this._dispatchRepository = dispatchRepository;
			this._orderDRepository = orderDRepository;
			this._orderHRepository = orderHRepository;
			this._customerRepository = customerRepository;
			this._departmentRepository = departmentRepository;
			this._containerTypeRepository = containerTypeRepository;
			this._trailerRepository = trailerRepository;
			this._truckRepository = truckRepository;
			this._driverRepository = driverRepository;
			this._partnerRepository = partnerRepository;
			this._shippingCompanyRepository = shippingCompanyRepository;
			this._textResourceRepository = textResourceRepository;
			this._expenseRepository = expenseRepository;
			this._expenseDetailRepository = expenseDetailRepository;
			this._surchargeDetailRepository = surchargeDetailRepository;
			this._allowanceDetailRepository = allowanceDetailRepository;
			this._customerSettlementRepository = customerSettlementRepository;
			this._partnerSettlementRepository = partnerSettlementRepository;
			this._basicRepository = basicRepository;
			this._fixedExpenseRepository = fixedExpenseRepository;
			this._truckExpenseRepository = truckExpenseRepository;
			this._supplierRepository = supplierRepository;
			this._expenseCategoryRepository = expenseCategoryRepository;
			this._invoiceInfoService = invoiceInfoService;
			this._customerService = customerService;
			this._partnerService = partnerService;
			this._locationService = locationService;
			this._unitOfWork = unitOfWork;
			this._inspectionRepository = inspectionRepository;
			this._inspectionPlanDetailRepository = inspectionPlanDetailRepository;
			this._modelRepository = modelRepository;
			this._maintenancePlanDetailRepository = maintenancePlanDetailRepository;
			this._maintenanceItemRepository = maintenanceItemRepository;
			this._inspectionDetailRepository = inspectionDetailRepository;
			this._maintenanceDetailRepository = maintenanceDetailRepository;
			this._liabilitiesRepository = liabilitiesRepository;
			this._fuelConsumptionDetailRepository = fuelConsumptionDetailRepository;
			this._customerPaymentRepository = customerPaymentRepository;
			this._customerBalanceRepository = customerBalanceRepository;
			this._customerPricingRepository = customerPricingRepository;
			this._customerPricingDetailRepository = customerPricingDetailRepository;
			this._locationRepository = locationRepository;
			this._operationRepository = operationRepository;
			this._partnerBalanceRepository = partnerBalanceRepository;
			this._partnerPaymentRepository = partnerPaymentRepository;
			this._liabilitiesItemRepository = liabilitiesItemRepository;
			this._userRepository = userRepository;
			this._employeeRepository = employeeRepository;
			this._companyExpenseRepository = companyExpenseRepository;
			this._driverLicenseRepository = driverLicenseRepository;
			this._calculateDriverAllowanceRepository = calculateDriverAllowanceRepository;
			this._driverAllowanceRepository = driverAllowanceRepository;
		}
		#endregion

		#region IReportService members
		public Stream ExportExcel(DispatchReportParam param, string userName)
		{
			List<DispatchListReport> data = GetDispatchList(param);
			Dictionary<string, string> dicLanguage;
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
			//ger employeeName
			var user = GetEmployeeByUserName(userName);
			// get language for report
			dicLanguage = new Dictionary<string, string>();

			if (param.Laguague == "vi")
			{
				intLanguage = 1;
			}
			else if (param.Laguague == "jp")
			{
				intLanguage = 3;
			}
			else
			{
				intLanguage = 2;
			}

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
											   (con.TextKey == "LBLORDERDATEDISPATCH" ||
												con.TextKey == "LBLCUSTOMER" ||
												con.TextKey == "LBLORDERTYPEDISPATCH" ||
												con.TextKey == "LBLORDERDEPARTMENTDISPATCH" ||
												con.TextKey == "LBLBLBKDISPATCH" ||
												con.TextKey == "LBLSHIPPINGAGENCYDISPATCH" ||
												con.TextKey == "LBLCONTAINERNODISPATCH" ||
												con.TextKey == "LBLTRAILERNODISPATCH" ||
												con.TextKey == "LBLLOADINGPLACEDISPATCH" ||
												con.TextKey == "LBLLOADINGCUTOFFDISPATCH" ||
												con.TextKey == "LBLLOADINGDATEDISPATCH" ||
												con.TextKey == "LBLDISCHARGEPLACEDISPATCH" ||
												con.TextKey == "LBLDISCHARGECUTOFFDISPATCH" ||
												con.TextKey == "LBLDISCHARGEDATEDISPATCH" ||
												con.TextKey == "LBLDISPATCHORDER" ||
												con.TextKey == "LBLTRANSPORTDATEDISPATCH" ||
												con.TextKey == "LBLTRUCKNODISPATCH" ||
												con.TextKey == "LBLDRIVERDISPATCH" ||
												con.TextKey == "LBLCONTAINERSTATUSDISPATCH" ||
												con.TextKey == "LBLLOCATION1DISPATCH" ||
												con.TextKey == "LBLLOCATION2DISPATCH" ||
												con.TextKey == "LBLLOCATION3DISPATCH" ||
												con.TextKey == "LBLTRANSPORT" ||
												con.TextKey == "LBLCONTAINERSTATUS1DISPATCH" ||
												con.TextKey == "LBLCONTAINERSTATUS2DISPATCH" ||
												con.TextKey == "LBLCONTAINERSTATUS3DISPATCH" ||
												con.TextKey == "LBLDISPATCHSTATUS1" ||
												con.TextKey == "LBLDISPATCHSTATUS2" ||
												con.TextKey == "LBLDISPATCHSTATUS3" ||
												con.TextKey == "LBLDISPATCHSTATUS4" ||
												con.TextKey == "TLTDISPATCHREPORT" ||
												con.TextKey == "LBLDISPATCHTOTAL" ||
												con.TextKey == "LBLREPORTI" ||
												con.TextKey == "LBLREPORTIORDER" ||
												con.TextKey == "LBLREPORTIDISPATCH"
										)).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
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

			return CrystalReport.Service.Dispatch.ExportExcel.ExportDispatchListToExcel(data, dicLanguage, intLanguage, category, companyName, companyAddress, fileName, user);
		}

		public Stream ExportExcelTransportationPlan(DispatchReportParam param, string userName)
		{
			List<TransportationPlanReport> data = GetTransportationPlanReport(param);
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			var companyName = "";
			var companyAddress = "";
			var companyTaxCode = "";
			var fileName = "";
			var phoneNumber = "";
			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				companyTaxCode = basicSetting.TaxCode;
				fileName = basicSetting.Logo;
				phoneNumber = basicSetting.PhoneNumber1;
			}
			//ger employeeName
			var user = GetEmployeeByUserName(userName);
			// get language for report
			dicLanguage = new Dictionary<string, string>();

			if (param.Laguague == "vi")
			{
				intLanguage = 1;
			}
			else if (param.Laguague == "jp")
			{
				intLanguage = 3;
			}
			else
			{
				intLanguage = 2;
			}

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
											   ((con.TextKey == "LBLDATE" && con.ScreenID == 99) ||
												con.TextKey == "LBLORDERTYPESHORT" ||
												con.TextKey == "MNUSHIPPINGCOMPANY" ||
												con.TextKey == "LBLBLBKREPORT" ||
												con.TextKey == "RPHDCOMMODITY" ||
												con.TextKey == "LBLSTOPOVERPLACESHORT" ||
												con.TextKey == "LBLLOADINGPLACESHORT" ||
												con.TextKey == "LBLDISCHARGEPLACESHORT" ||
												con.TextKey == "LBLNUMBERCONT" ||
												con.TextKey == "LBLJOBNO" ||
												con.TextKey == "LBLEXPIRATIONSHORT" ||
												con.TextKey == "LBLTRUCKRUNREPORT" ||
												con.TextKey == "LBLRETURNDATEREPORT" ||
												con.TextKey == "LBLMOOCNO" ||
												con.TextKey == "LBLWEIGHT" ||
												(con.TextKey == "LBLDESCRIPTION" && con.ScreenID == 1) ||
												con.TextKey == "LBLORDERTITLEREPORT" ||
												con.TextKey == "LBLTRUCKRETURNREPORT" ||
												con.TextKey == "LBLEXPORTSHORT" ||
												con.TextKey == "LBLIMPORTSHORT" ||
												con.TextKey == "LBLOTHER" ||
												con.TextKey == "RPFTPRINTBY" ||
												con.TextKey == "RPFTPRINTTIME" ||
												con.TextKey == "LBLCONTSIZE" ||
												con.TextKey == "RPHDCONTNO" ||
												con.TextKey == "LBLLOADINGLOCATIONREPORT" ||
												con.TextKey == "LBLDISCHARGELOCATIONREPORT" ||
												con.TextKey == "LBLSTEVEDORAGELOCATIONREPORT" ||
												con.TextKey == "LBLLIFTINGINFO"
										)).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue.ToUpper());
				}
			}
			//get department
			var department = param.DepC != "null"
				? (from d in _departmentRepository.GetAllQueryable() where d.DepC == param.DepC select d.DepN).FirstOrDefault()
				: null;

			// set category
			var title = dicLanguage["LBLORDERTITLEREPORT"] + String.Format(": {0}{1}{2}",
				Utilities.GetFormatDateReportByLanguage(param.TransportDFrom, intLanguage),
				param.TransportDFrom != param.TransportDTo ? (" - " + Utilities.GetFormatDateReportByLanguage(param.TransportDTo, intLanguage)) : "",
				department != null ? (" - " + department) : "");

			return CrystalReport.Service.Transport.ExportExcel.ExportTransportationPlanToExcel(data, dicLanguage, intLanguage, title, companyName, companyAddress, fileName, user, phoneNumber);
		}

		public List<TransportationPlanReport> GetTransportationPlanReport(DispatchReportParam param)
		{
			var transportationPlan = from a in _orderHRepository.GetAllQueryable()
									 join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
										 equals new { b.OrderD, b.OrderNo } into t1
									 from b in t1.DefaultIfEmpty()
									 join dis in _dispatchRepository.GetAllQueryable() on new { b.OrderD, b.OrderNo, b.DetailNo }
										 equals new { dis.OrderD, dis.OrderNo, dis.DetailNo } into t6
									 from dis in t6.DefaultIfEmpty()
									 join d in _customerRepository.GetAllQueryable() on new { a.CustomerMainC, a.CustomerSubC }
										 equals new { d.CustomerMainC, d.CustomerSubC } into t3
									 from d in t3.DefaultIfEmpty()
									 //join d1 in _customerRepository.GetAllQueryable() on new { a.CustomerPayLiftLoweredMainC, a.CustomerPayLiftLoweredSubC }
									 //	equals new { d1.CustomerMainC, d1.CustomerSubC } into tt3
									 //from d1 in tt3.DefaultIfEmpty()
									 join c in _trailerRepository.GetAllQueryable() on b.TrailerC equals c.TrailerC into t2
									 from c in t2.DefaultIfEmpty()
									 join e in _truckRepository.GetAllQueryable() on b.TruckCReturn equals e.TruckC into t4
									 from e in t4.DefaultIfEmpty()
									 join f in _truckRepository.GetAllQueryable() on b.TruckCLastDispatch equals f.TruckC into t5
									 from f in t5.DefaultIfEmpty()
									 //from d1 in _customerRepository.Query(cus => cus.CustomerMainC == a.CustomerPayLiftLoweredMainC && cus.CustomerSubC == a.CustomerPayLiftLoweredSubC).FirstOrDefault()
									 //where (a.CustomerPayLiftLoweredMainC == d1.CustomerMainC && a.CustomerPayLiftLoweredSubC == d1.CustomerSubC) &
									 where (param.DepC == "0" || a.OrderDepC == param.DepC) &
										   (a.OrderD >= param.TransportDFrom && a.OrderD <= param.TransportDTo) &
										   (param.Customer == "null" ||
											("," + param.Customer + ",").Contains("," + a.CustomerMainC + "_" + a.CustomerSubC + ",")) &
										   (param.TruckC == "null" || ("," + param.TruckC + ",").Contains("," + b.TruckCLastDispatch + ",") ||
											("," + param.TruckC + ",").Contains("," + b.TruckCReturn + ",")) &
											(param.DriverC=="null" || param.DriverC == dis.DriverC) &
											(param.DispatchI == "2" || param.DispatchI == dis.DispatchI) &
								((param.DispatchStatus0 && param.DispatchStatus1 && param.DispatchStatus2) ||
								(!param.DispatchStatus0 && !param.DispatchStatus1 && !param.DispatchStatus2 && dis == null) ||
								(param.DispatchStatus0 && (dis == null || dis.DispatchStatus == Constants.NOTDISPATCH)) ||
								(param.DispatchStatus1 && dis.DispatchStatus == Constants.DISPATCH) ||
								(param.DispatchStatus2 && (dis.DispatchStatus == Constants.TRANSPORTED || dis.DispatchStatus == Constants.CONFIRMED)))
									 select new TransportationPlanReport()
									 {
										 OrderD = a.OrderD,
										 OrderNo = a.OrderNo,
										 DetailNo = b.DetailNo,
										 OrderTypeI = a.OrderTypeI,
										 ContainerSize = b.ContainerSizeI,
										 ShippingCompanyN = a.ShippingCompanyN,
										 Booking = a.BLBK,
										 CommodityN = b.CommodityN,
										 Locaion1N = b.LocationDispatch1,
										 Locaion2N = b.LocationDispatch2,
										 Locaion3N = b.LocationDispatch3,
										 ContainerNo = b.ContainerNo,
										 JobNo = a.JobNo,
										 ClosingDT = a.ClosingDT,
										 TruckNoRun = f.RegisteredNo,
										 TruckNoReturn = e.RegisteredNo,
										 ReturnDate = null,
										 TrailerNo = c.TrailerNo,
										 EstimatedWeight = b.EstimatedWeight,
										 Description = b.Description,
										 LocaionRoot1N = a.LoadingPlaceN,
										 LocaionRoot2N = a.StopoverPlaceN,
										 LocaionRoot3N = a.DischargePlaceN,
										 IsCollected = a.IsCollected,
										 CustomerPayLiftSubC = a.CustomerPayLiftLoweredSubC,
										 CustomerPayLiftMainC = a.CustomerPayLiftLoweredMainC,
										 ActualDischargeD = b.ActualDischargeD,
										 ActualPickupReturnD = b.ActualPickupReturnD
									 };
			var data = transportationPlan.OrderBy("OrderD asc, OrderNo asc").ToList();
			//var data = transportationPlan.OrderBy("OrderD desc, OrderNo desc, TrailerNo asc").ToList();
			if (data.Count > 0)
			{
				DateTime oDFlag = data[0].OrderD;
				string oNoFlag = data[0].OrderNo;
				for (int i = 0; i < data.Count; i++)
				{
					DateTime orderD = data[i].OrderD;
					string orderNo = data[i].OrderNo;
					int detailNo = data[i].DetailNo;
					var dis = _dispatchRepository.Query(p => p.OrderD == orderD && p.OrderNo == orderNo && p.DetailNo == detailNo).ToList();
					if (dis.Count > 0)
					{
						int maxDispatch = dis.Max(p => p.DispatchNo);
						for (var loop = 0; loop < dis.Count; loop++)
						{
							if (dis[loop].DispatchI == "1")
							{
								string registeredNo = "";
								string truckC = dis[loop].TruckC;
								if (!string.IsNullOrEmpty(truckC))
								{
									var truck = _truckRepository.Query(p => p.TruckC == truckC).FirstOrDefault();
									registeredNo = truck != null ? truck.RegisteredNo : "";
								}
								else
								{
									registeredNo = dis[loop].RegisteredNo;
								}
								string partMainC = dis[loop].PartnerMainC;
								string partSuc = dis[loop].PartnerSubC;
								var part = _partnerRepository.Query(p => p.PartnerMainC == partMainC && p.PartnerSubC == partSuc).FirstOrDefault();
								if (dis[loop].DispatchNo == 1)
								{
									data[i].TruckNoRun = (part != null ? (!string.IsNullOrEmpty(part.PartnerShortN) ? (part.PartnerShortN) : "") : "") + (!string.IsNullOrEmpty(registeredNo) ? (" (" + registeredNo + ")") : "");
								}
								if (dis[loop].DispatchNo == maxDispatch)
								{
									data[i].TruckNoReturn = (part != null ? (!string.IsNullOrEmpty(part.PartnerShortN) ? (part.PartnerShortN) : "") : "") + (!string.IsNullOrEmpty(registeredNo) ? (" (" + registeredNo + ")") : "");
								}
							}
						}
					}
					if (data[i].OrderTypeI == "0")
					{
						data[i].ReturnDate = data[i].ActualDischargeD;
					}
					else
					{
						data[i].ReturnDate = data[i].ActualPickupReturnD;
					}
					if ((i + 1 < data.Count &&
						 ((data[i + 1].OrderD != oDFlag || data[i + 1].OrderD == oDFlag) && data[i + 1].OrderNo != oNoFlag)) ||
						(i + 1 == data.Count))
					{
						string custMainC = data[i].CustomerPayLiftMainC;
						string custSuc = data[i].CustomerPayLiftSubC;
						var basicSet = _basicRepository.GetAllQueryable().FirstOrDefault();
						var cust =_customerRepository.Query(p => p.CustomerMainC == custMainC && p.CustomerSubC == custSuc).FirstOrDefault();
						if (data[i].IsCollected == "1")
						{
							data[i].IsCollected = basicSet != null? (basicSet.CompanyFullN + (!string.IsNullOrEmpty(basicSet.Address1) ? (", " + basicSet.Address1) : "") + (!string.IsNullOrEmpty(basicSet.TaxCode) ? (", " + basicSet.TaxCode) : "")) : "";
						}
						else
						{
							data[i].IsCollected = cust != null ? (cust.CustomerN + (!string.IsNullOrEmpty(cust.Address1) ? (", " + cust.Address1) : "") + (!string.IsNullOrEmpty(cust.TaxCode) ? (", " + cust.TaxCode) : "")) : "";
						}
						if (i + 1 < data.Count)
						{
							oDFlag = data[i + 1].OrderD;
							oNoFlag = data[i + 1].OrderNo;
						}
					}
					else data[i].IsCollected = "";
					//cannot clean code because CustomerPayLiftMainC, CustomerPayLiftSubC cannot compare
					var tmp = _customerRepository.GetAllQueryable().ToList().Where(cus => cus.CustomerMainC == data[i].CustomerPayLiftMainC && cus.CustomerSubC == data[i].CustomerPayLiftSubC)
							.FirstOrDefault();
					data[i].CustomerPayLift = tmp != null ? tmp.CustomerShortN : "";
				}
			}
			return data;
		}

		public Stream ExportPdf(DispatchReportParam param, string userName)
		{
			Stream stream;
			DataRow row;
			DispatchList.DispatchListDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			int index;
			int total = 0;
			var companyName = "";
			var companyAddress = "";
			var companyTaxCode = "";
			var fileName = "";

			// get data
			dt = new DispatchList.DispatchListDataTable();
			List<DispatchListReport> data = GetDispatchList(param);
			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				companyTaxCode = basicSetting.TaxCode;
				fileName = basicSetting.Logo;
			}
			//ger employeeName
			var user = GetEmployeeByUserName(userName);
			// get language for report
			dicLanguage = new Dictionary<string, string>();

			if (param.Laguague == "vi")
			{
				intLanguage = 1;
			}
			else if (param.Laguague == "jp")
			{
				intLanguage = 3;
			}
			else
			{
				intLanguage = 2;
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
																  con.TextKey == "LBLLOAD" ||
																  con.TextKey == "LBLLOLO" ||
																  con.TextKey == "LBLDISPATCHI1" ||
																  con.TextKey == "RPHDCUSTOMERN"
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
				index = 1;
				for (int iloop = 0; iloop < data.Count; iloop++)
				{
					// detail row
					if (data[iloop].DispatchDList != null && data[iloop].DispatchDList.Count > 0)
					{
						total = total + data[iloop].DispatchDList.Count;
						for (int jloop = 0; jloop < data[iloop].DispatchDList.Count; jloop++)
						{
							row = dt.NewRow();

							// group row
							row["No"] = index;
							row["OrderDate"] = Utilities.GetFormatShortDateReportByLanguage(data[iloop].OrderD, intLanguage);
							row["Customer"] = data[iloop].CustomerShortN != "" ? data[iloop].CustomerShortN : data[iloop].CustomerN;
							row["OrderType"] = Utilities.GetOrderTypeName(data[iloop].OrderTypeI);
							row["Department"] = data[iloop].DepN;
							row["OrderNo"] = data[iloop].OrderNo;
							row["BL/BK"] = data[iloop].BLBK;
							row["ShippingAgency"] = data[iloop].ShippingCompanyN;
							if (data[iloop].VesselN != null && data[iloop].VesselN.Length > 0)
								row["ShippingAgency"] += " / " + data[iloop].VesselN;
							row["ContainerNo"] = data[iloop].ContainerNo;
							row["ContainerSize"] = (data[iloop].ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] + (" (" + (data[iloop].NetWeight).ToString("#,###.#") + ")")
								: Utilities.GetContainerSizeName(data[iloop].ContainerSizeI));
							row["TrailerNo"] = data[iloop].TrailerNo;
							//row["LoadingPlace"] = data[iloop].LoadingPlaceN != null ? data[iloop].LoadingPlaceN : "";
							//row["Cut-off1"] = data[iloop].LoadingDT != null ? Utilities.GetFormatDateAndHourReportByLanguage((DateTime)data[iloop].LoadingDT, intLanguage) : "";
							//row["LoadingDate"] = data[iloop].ActualLoadingD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].ActualLoadingD, intLanguage) : "";
							//row["DischargePlace"] = data[iloop].DischargePlaceN != null ? data[iloop].DischargePlaceN : "";
							//row["Cut-off2"] = data[iloop].DischargeDT != null ? Utilities.GetFormatDateAndHourReportByLanguage((DateTime)data[iloop].DischargeDT, intLanguage) : "";
							//row["DischargeDate"] = data[iloop].ActualDischargeD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].ActualDischargeD, intLanguage) : "";
							row["CommodityN"] = data[iloop].CommodityN;
							row["JobNo"] = data[iloop].JobNo;
							row["SealNo"] = data[iloop].SealNo;
							row["DetailNo"] = data[iloop].DetailNo;
							row["key"] = row["OrderDate"] + row["OrderNo"].ToString() + row["DetailNo"];

							//row["Order"] = data[iloop].DispatchDList[jloop].DispatchNo;
							row["Order"] = jloop + 1;
							row["TransportDate"] = data[iloop].DispatchDList[jloop].TransportD != null ? Utilities.GetFormatShortDateReportByLanguage((DateTime)data[iloop].DispatchDList[jloop].TransportD, intLanguage) : "";
							row["TruckNo"] = data[iloop].DispatchDList[jloop].RegisteredNo;
							row["Driver"] = data[iloop].DispatchDList[jloop].DriverN;
							//if (data[iloop].DispatchDList[jloop].ContainerStatus == Constants.NORMAL)
							//{
							//	row["Status"] = dicLanguage["LBLCONTAINERSTATUS1DISPATCH"];
							//}
							//else if (data[iloop].DispatchDList[jloop].ContainerStatus == Constants.LOAD)
							//{
							//	row["Status"] = dicLanguage["LBLCONTAINERSTATUS2DISPATCH"];
							//}
							//else
							//{
							//	row["Status"] = dicLanguage["LBLCONTAINERSTATUS3DISPATCH"];
							//}
							row["Location1"] = data[iloop].DispatchDList[jloop].Location1N + "<br>" +
								(data[iloop].DispatchDList[jloop].Location1DT != null ? Utilities.GetFormatDateAndHourReportByLanguage((DateTime)data[iloop].DispatchDList[jloop].Location1DT, intLanguage) : "");
							row["Location2"] = data[iloop].DispatchDList[jloop].Location2N + "<br>" +
								(data[iloop].DispatchDList[jloop].Location2DT != null ? Utilities.GetFormatDateAndHourReportByLanguage((DateTime)data[iloop].DispatchDList[jloop].Location2DT, intLanguage) : "");
							row["Location3"] = data[iloop].DispatchDList[jloop].Location3N + "<br>" +
								(data[iloop].DispatchDList[jloop].Location3DT != null ? Utilities.GetFormatDateAndHourReportByLanguage((DateTime)data[iloop].DispatchDList[jloop].Location3DT, intLanguage) : "");
							row["Transport"] = data[iloop].DispatchDList[jloop].DispatchStatus;
							if (data[iloop].DispatchDList[jloop].DispatchStatus == Constants.NOTDISPATCH)
							{
								row["Transport"] = dicLanguage["LBLDISPATCHSTATUS1"];
							}
							else if (data[iloop].DispatchDList[jloop].DispatchStatus == Constants.DISPATCH)
							{
								row["Transport"] = dicLanguage["LBLDISPATCHSTATUS2"];
							}
							else if (data[iloop].DispatchDList[jloop].DispatchStatus == Constants.TRANSPORTED)
							{
								row["Transport"] = dicLanguage["LBLDISPATCHSTATUS3"];
							}
							else
							{
								row["Transport"] = dicLanguage["LBLDISPATCHSTATUS4"];
							}

							row["InstructionNo"] = data[iloop].DispatchDList[jloop].InstructionNo;
							row["Description"] = String.IsNullOrEmpty(data[iloop].IsCollected) ? data[iloop].DispatchDList[jloop].Description
									: data[iloop].DispatchDList[jloop].Description + "<br>" + dicLanguage["LBLLOLO"] + ": " +
									(data[iloop].IsCollected.Equals("1") ? dicLanguage["LBLDISPATCHI1"] : dicLanguage["RPHDCUSTOMERN"]);
							dt.Rows.Add(row);
						}
					}
					else
					{
						row = dt.NewRow();

						// group row
						row["No"] = index;
						row["OrderDate"] = Utilities.GetFormatDateReportByLanguage(data[iloop].OrderD, intLanguage);
						row["Customer"] = data[iloop].CustomerShortN;
						row["OrderType"] = Utilities.GetOrderTypeName(data[iloop].OrderTypeI);
						row["Department"] = data[iloop].DepN;
						row["OrderNo"] = data[iloop].OrderNo;
						row["BL/BK"] = data[iloop].BLBK;
						row["ShippingAgency"] = data[iloop].ShippingCompanyN;
						if (data[iloop].VesselN != null && data[iloop].VesselN.Length > 0)
							row["ShippingAgency"] += " / " + data[iloop].VesselN;
						row["ContainerNo"] = data[iloop].ContainerNo;
						row["ContainerSize"] = (data[iloop].ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] + (" (" + (data[iloop].NetWeight).ToString("#,###.#") + ")")
							: Utilities.GetContainerSizeName(data[iloop].ContainerSizeI));
						row["TrailerNo"] = data[iloop].TrailerNo;
						//row["LoadingPlace"] = data[iloop].LoadingPlaceN != null ? data[iloop].LoadingPlaceN : "";
						//row["Cut-off1"] = data[iloop].LoadingDT != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].LoadingDT, intLanguage) : "";
						//row["LoadingDate"] = data[iloop].ActualLoadingD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].ActualLoadingD, intLanguage) : "";
						//row["DischargePlace"] = data[iloop].DischargePlaceN != null ? data[iloop].DischargePlaceN : "";
						//row["Cut-off2"] = data[iloop].DischargeDT != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].DischargeDT, intLanguage) : "";
						//row["DischargeDate"] = data[iloop].ActualDischargeD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].ActualDischargeD, intLanguage) : "";
						row["CommodityN"] = data[iloop].CommodityN;
						row["JobNo"] = data[iloop].JobNo;
						row["SealNo"] = data[iloop].SealNo;
						row["DetailNo"] = data[iloop].DetailNo;
						row["key"] = row["OrderDate"] + row["OrderNo"].ToString() + row["DetailNo"];

						dt.Rows.Add(row);
					}
					index++;
				}
			}

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

			stream = CrystalReport.Service.Dispatch.ExportPdf.Exec(dt, intLanguage, total.ToString(), fromDate, toDate, category, companyName, companyAddress, fileName, user);
			return stream;
		}

		public List<DispatchListReport> GetDispatchList(DispatchReportParam param)
		{
			List<DispatchListReport> result = new List<DispatchListReport>();
			// get data from Order_D and Order_H
			var order = from a in _orderDRepository.GetAllQueryable()
						join b in _orderHRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
								equals new { b.OrderD, b.OrderNo } into t1
						from b in t1.DefaultIfEmpty()
						join c in _customerRepository.GetAllQueryable() on new { b.CustomerMainC, b.CustomerSubC }
							 equals new { c.CustomerMainC, c.CustomerSubC } into t2
						from c in t2.DefaultIfEmpty()
						join d in _departmentRepository.GetAllQueryable() on b.OrderDepC
							 equals d.DepC into t3
						from d in t3.DefaultIfEmpty()
						//join e in _shippingCompanyRepository.GetAllQueryable() on b.ShippingCompanyC
						//	 equals e.ShippingCompanyC into t4
						//from e in t4.DefaultIfEmpty()
						join f in _trailerRepository.GetAllQueryable() on a.TrailerC
							 equals f.TrailerC into t5
						from f in t5.DefaultIfEmpty()
						where (((param.ReportI == "O" && a.OrderD >= param.TransportDFrom) || (param.ReportI == "D")) &
							   ((param.ReportI == "O" && a.OrderD <= param.TransportDTo) || (param.ReportI == "D")) &
							   (param.Customer == "null" || (param.Customer).Contains(b.CustomerMainC + "_" + b.CustomerSubC)) &
							   (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || (param.ReportI == "O" && b.OrderDepC == param.DepC) || (param.ReportI == "D")) &
							   (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || b.OrderTypeI == param.OrderTypeI)
							  )

						select new DispatchListReport()
						{
							OrderD = a.OrderD,
							OrderNo = a.OrderNo,
							DetailNo = a.DetailNo,
							CustomerShortN = c != null ? c.CustomerShortN : "",
							CustomerN = c != null ? c.CustomerN : "",
							OrderTypeI = b.OrderTypeI,
							DepN = d != null ? d.DepN : "",
							BLBK = b.BLBK,
							ShippingCompanyN = b.ShippingCompanyN,
							VesselN = b.VesselN,
							ContainerNo = a.ContainerNo,
							ContainerSizeI = a.ContainerSizeI,
							TrailerNo = f != null ? f.TrailerNo : "",
							CommodityN = a.CommodityN,
							JobNo = b.JobNo,
							SealNo = a.SealNo,
							IsCollected = b.IsCollected,
							NetWeight = a.ContainerSizeI == "3" ? (a.NetWeight ?? 0) : 0,
							//LoadingPlaceN = b.LoadingPlaceN,
							//LoadingDT = b.LoadingDT,
							//ActualLoadingD = a.ActualLoadingD,
							//DischargeDT = b.DischargeDT,
							//DischargePlaceN = b.DischargePlaceN,
							//ActualDischargeD = a.ActualDischargeD
						};

			order = order.OrderBy("OrderD asc, OrderNo asc, DetailNo asc");
			var orderList = order.ToList();

			for (int iloop = 0; iloop < orderList.Count; iloop++)
			{
				var orderD = orderList[iloop].OrderD;
				var orderNo = orderList[iloop].OrderNo;
				var detailNo = orderList[iloop].DetailNo;
				var dispatch = from a in _dispatchRepository.GetAllQueryable()
							   join b in _driverRepository.GetAllQueryable() on a.DriverC
								   equals b.DriverC into t1
							   from b in t1.DefaultIfEmpty()
							   // bổ sung thêm Partner
							   join p in _partnerRepository.GetAllQueryable() on new { a.PartnerMainC, a.PartnerSubC }
							   equals new { p.PartnerMainC, p.PartnerSubC } into t6
							   from p in t6.DefaultIfEmpty()
							   // end
							   join c in _truckRepository.GetAllQueryable() on a.TruckC
								   equals c.TruckC into t2
							   from c in t2.DefaultIfEmpty()
							   join o1 in _operationRepository.GetAllQueryable() on a.Operation1C
									equals o1.OperationC into t3
							   from o1 in t3.DefaultIfEmpty()
							   join o2 in _operationRepository.GetAllQueryable() on a.Operation2C
								equals o2.OperationC into t4
							   from o2 in t4.DefaultIfEmpty()
							   join o3 in _operationRepository.GetAllQueryable() on a.Operation3C
									equals o3.OperationC into t5
							   from o3 in t5.DefaultIfEmpty()
							   where (a.OrderD == orderD &
								   a.OrderNo == orderNo &
								   a.DetailNo == detailNo &
								   (param.TransportDFrom == null || (a.TransportD >= param.TransportDFrom && param.ReportI == "D") || param.ReportI == "O") &
								   (param.TransportDTo == null || (a.TransportD <= param.TransportDTo && param.ReportI == "D") || param.ReportI == "O") &
								   (param.TruckC == "null" || param.TruckC.Contains(c.TruckC)) &
								   ((param.DispatchI == "0" && param.DispatchI == a.DispatchI && (param.DriverC == "null" || param.DriverC.Contains(a.DriverC))) ||
									(param.DispatchI == "1" && param.DispatchI == a.DispatchI && (param.Partner == "null" || (param.Partner).Contains(p.PartnerMainC + "_" + p.PartnerSubC))) ||
									(param.DispatchI == "2" && (param.DriverC == "null" || param.DriverC.Contains(a.DriverC)) && (param.Partner == "null" || (param.Partner).Contains(p.PartnerMainC + "_" + p.PartnerSubC)))) &
								   (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || (param.ReportI == "O") || (param.ReportI == "D" && c.DepC == param.DepC)) &
								   ((param.DispatchStatus0 && a.DispatchStatus == "0") ||
									(param.DispatchStatus1 && a.DispatchStatus == "1") ||
									(param.DispatchStatus2 && (a.DispatchStatus == "2" || a.DispatchStatus == "3"))
								   )
								 )
							   select new DispatchViewModel()
							   {
								   DispatchNo = a.DispatchNo,
								   TransportD = a.TransportD,
								   RegisteredNo = (a.RegisteredNo != null && a.RegisteredNo != "") ? a.RegisteredNo : (c != null ? c.RegisteredNo : ""),
								   DriverN = b != null ? b.LastN + " " + b.FirstN : (p != null ? (p.PartnerShortN != "" ? p.PartnerShortN : p.PartnerN) : ""),
								   ContainerStatus = a.ContainerStatus,
								   Location1N = (a.Operation1C != null && a.Operation1C != "0") ? a.Location1N + " (" + o1.OperationN + ")" : a.Location1N,
								   Location2N = (a.Operation2C != null && a.Operation2C != "0") ? a.Location2N + " (" + o2.OperationN + ")" : a.Location2N,
								   Location3N = (a.Operation3C != null && a.Operation3C != "0") ? a.Location3N + " (" + o3.OperationN + ")" : a.Location3N,
								   Location1DT = a.Location1DT,
								   Location2DT = a.Location2DT,
								   Location3DT = a.Location3DT,
								   DispatchStatus = a.DispatchStatus,
								   InstructionNo = a.InstructionNo,
								   Description = a.Description,
							   };

				dispatch = dispatch.OrderBy("DispatchNo asc");


				//if (param.DispatchStatus0 && param.DispatchStatus1 && param.DispatchStatus2)
				//{
				//	if (dispatch.Any())
				//	{
				//		orderList[iloop].DispatchDList = dispatch.ToList();
				//	}
				//	result.Add(orderList[iloop]);
				//}
				//else
				//{
				//	if (dispatch.Any())
				//	{
				//		orderList[iloop].DispatchDList = dispatch.ToList();
				//		result.Add(orderList[iloop]);
				//	}
				//}
				if (dispatch.Any())
				{
					orderList[iloop].DispatchDList = dispatch.ToList();
					result.Add(orderList[iloop]);
				}
				else if (param.ReportI == "O")
				{
					orderList[iloop].DispatchDList = dispatch.ToList();
					result.Add(orderList[iloop]);
				}
			}

			return result;
		}

		public Stream ExportPdf(DriverDispatchReportParam param, string userName)
		{
			Stream stream;
			DataRow row;
			DriverDispatch.DriverDispatchDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			int index;
			var companyName = "";
			var companyAddress = "";
			var companyTaxCode = "";
			var fileName = "";
			// get data
			dt = new DriverDispatch.DriverDispatchDataTable();
			List<DispatchDetailViewModel> data = GetDriverDispatchReportList(param);
			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				companyTaxCode = basicSetting.TaxCode;
				fileName = basicSetting.Logo;
			}
			//ger employeeName
			var user = GetEmployeeByUserName(userName);
			// get language for report
			dicLanguage = new Dictionary<string, string>();

			if (param.Laguague == "vi")
			{
				intLanguage = 1;
			}
			else if (param.Laguague == "jp")
			{
				intLanguage = 3;
			}
			else
			{
				intLanguage = 2;
			}

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																 (con.TextKey == "LBLCONTAINERSTATUS1DISPATCH" ||
																  con.TextKey == "LBLCONTAINERSTATUS2DISPATCH" ||
																  con.TextKey == "LBLCONTAINERSTATUS3DISPATCH" ||
																  con.TextKey == "LBLREPORTI" ||
																  con.TextKey == "LBLREPORTIORDER" ||
																  con.TextKey == "LBLREPORTIDISPATCH" ||
																  con.TextKey == "LBLTON" ||
																  con.TextKey == "LBLLOAD" ||
																  con.TextKey == "LBLLOLO" ||
																  con.TextKey == "LBLDISPATCHI1" ||
																  con.TextKey == "RPHDCUSTOMERN" ||
																  con.TextKey == "LBLEXPORTSHORT" ||
																  con.TextKey == "LBLIMPORTSHORT" ||
																  con.TextKey == "LBLWAREHOUSETRANSFER"
																  )).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			index = 1;
			if (data != null && data.Count > 0)
			{
				for (int iloop = 0; iloop < data.Count; iloop++)
				{
					row = dt.NewRow();

					// group row
					row["key"] = data[iloop].Dispatch.DriverN + data[iloop].Dispatch.RegisteredNo;
					row["TransportD"] = data[iloop].Dispatch.TransportD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].Dispatch.TransportD, intLanguage) : "";
					row["Customer"] = data[iloop].OrderH.CustomerShortN != "" ? data[iloop].OrderH.CustomerShortN : data[iloop].OrderH.CustomerN;
					row["ContainerNo"] = data[iloop].OrderD.ContainerNo;
					row["Type"] = (data[iloop].OrderD.ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] + " (" + (data[iloop].OrderD.NetWeight ?? 0).ToString("#,###.#") + ")" : Utilities.GetContainerSizeName(data[iloop].OrderD.ContainerSizeI));
					row["TrailerNo"] = (String.IsNullOrEmpty(data[iloop].OrderD.TrailerNo) ? "" : data[iloop].OrderD.TrailerNo);
					//row["OrderType"] = Utilities.GetOrderTypeName(data[iloop].OrderH.OrderTypeI);
					row["Location1"] = data[iloop].Dispatch.Location1N + " " +
					                   (data[iloop].Dispatch.Location1A != null && data[iloop].Dispatch.Location1A != ""
						                   ? "(" + data[iloop].Dispatch.Location1A + ")"
						                   : "") +
					                   (data[iloop].Dispatch.Location1DT != null
						                   ? "<br>(" +
						                     Utilities.GetFormatDateAndHourReportByLanguage((DateTime) data[iloop].Dispatch.Location1DT,
							                     intLanguage) + ")"
						                   : "");
					row["Location2"] = data[iloop].Dispatch.Location2N + " " +
					                   (data[iloop].Dispatch.Location2A != null && data[iloop].Dispatch.Location2A != ""
						                   ? "(" + data[iloop].Dispatch.Location2A + ")"
						                   : "") +
					                   (data[iloop].Dispatch.Location2DT != null
						                   ? "<br>(" +
						                     Utilities.GetFormatDateAndHourReportByLanguage((DateTime) data[iloop].Dispatch.Location2DT,
							                     intLanguage) + ")"
						                   : "");
					row["Location3"] = data[iloop].Dispatch.Location3N + " " +
					                   (data[iloop].Dispatch.Location3A != null && data[iloop].Dispatch.Location3A != ""
						                   ? "(" + data[iloop].Dispatch.Location3A + ")"
						                   : "") +
					                   (data[iloop].Dispatch.Location3DT != null
						                   ? "<br>(" +
						                     Utilities.GetFormatDateAndHourReportByLanguage((DateTime) data[iloop].Dispatch.Location3DT,
							                     intLanguage) + ")"
						                   : "");
					row["RegisteredNo"] = data[iloop].Dispatch.RegisteredNo;
					row["DriverN"] = data[iloop].Dispatch.DriverN;
					row["BLBK"] = (!String.IsNullOrEmpty(data[iloop].OrderH.BLBK) ? (data[iloop].OrderH.BLBK + "<br>") : "") + data[iloop].OrderH.ShippingCompanyN;
					row["Operation1N"] = data[iloop].Dispatch.Operation1N;
					row["Operation2N"] = data[iloop].Dispatch.Operation2N;
					row["Operation3N"] = data[iloop].Dispatch.Operation3N;
					row["IsCollected"] = data[iloop].OrderH.IsCollected;
					row["ShippingCompanyN"] = data[iloop].OrderH.OrderTypeI == "0" ? dicLanguage["LBLEXPORTSHORT"] : (data[iloop].OrderH.OrderTypeI == "1" ? dicLanguage["LBLIMPORTSHORT"] : dicLanguage["LBLWAREHOUSETRANSFER"]);
					//if (data[iloop].OrderH.DischargeDT != null)
					//{
					//	row["Cut-off"] = Utilities.GetFormatDateAndHourReportByLanguage((DateTime)data[iloop].OrderH.DischargeDT, intLanguage);
					//}
					//else if (data[iloop].OrderH.StopoverDT != null)
					//{
					//	row["Cut-off"] = Utilities.GetFormatDateAndHourReportByLanguage((DateTime)data[iloop].OrderH.StopoverDT, intLanguage);
					//}
					//else if (data[iloop].OrderH.LoadingDT != null)
					//{
					//	row["Cut-off"] = Utilities.GetFormatDateAndHourReportByLanguage((DateTime)data[iloop].OrderH.LoadingDT, intLanguage);
					//}
					//else
					//{
					//	row["Cut-off"] = "";
					//}
					//if (data[iloop].Dispatch.ContainerStatus == Constants.NORMAL)
					//{
					//	row["Content"] = dicLanguage["LBLCONTAINERSTATUS1DISPATCH"];
					//}
					//else if (data[iloop].Dispatch.ContainerStatus == Constants.LOAD)
					//{
					//	row["Content"] = dicLanguage["LBLCONTAINERSTATUS2DISPATCH"];
					//}
					//else
					//{
					//	row["Content"] = dicLanguage["LBLCONTAINERSTATUS3DISPATCH"];
					//}
					row["NetWeight"] = data[iloop].OrderD.NetWeight ?? 0;
					row["SealNo"] = data[iloop].OrderD.SealNo;
					row["CommodityN"] = data[iloop].OrderD.CommodityN;
					row["Description"] = String.IsNullOrEmpty(data[iloop].OrderH.IsCollected) ? data[iloop].Dispatch.Description
									: data[iloop].Dispatch.Description;
					var basicSet = _basicRepository.GetAllQueryable().FirstOrDefault();
					row["Contact"] = basicSet.ContactPerson != null
						? (basicSet.ContactPerson.ToUpper() + "-" +
						   (basicSet.PhoneNumber1 ?? (basicSet.PhoneNumber2 ?? "")))
						: "";
					dt.Rows.Add(row);

					index++;
				}
			}
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

			stream = CrystalReport.Service.DriverDispatch.ExportPdf.Exec(dt,
																		 intLanguage,
																		 (index - 1).ToString(),
																		 fromDate,
																		 toDate,
																		 category,
																		 companyName,
																		 companyAddress,
																		 fileName,
																		 user
																		 );
			return stream;
		}

		public List<DispatchDetailViewModel> GetDriverDispatchReportList(DriverDispatchReportParam param)
		{
			var driverCList = ConvertStringToList(param.DriverC);
			var truckCList = ConvertStringToList(param.TruckC);
			var driver = from a in _dispatchRepository.GetAllQueryable()
						 join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo }
								 equals new { b.OrderD, b.OrderNo, b.DetailNo } into t1
						 from b in t1.DefaultIfEmpty()
						 join c in _orderHRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
							  equals new { c.OrderD, c.OrderNo } into t2
						 from c in t2.DefaultIfEmpty()
						 join d in _customerRepository.GetAllQueryable() on new { c.CustomerMainC, c.CustomerSubC }
							  equals new { d.CustomerMainC, d.CustomerSubC } into t3
						 from d in t3.DefaultIfEmpty()
						 join e in _driverRepository.GetAllQueryable() on new { a.DriverC }
							  equals new { e.DriverC }
						 join f in _truckRepository.GetAllQueryable() on new { a.TruckC }
							   equals new { f.TruckC }
						 join t in _trailerRepository.GetAllQueryable() on new { b.TrailerC }
							   equals new { t.TrailerC } into t7
						 from t in t7.DefaultIfEmpty()
						 join o1 in _operationRepository.GetAllQueryable() on a.Operation1C
							  equals o1.OperationC into t4
						 from o1 in t4.DefaultIfEmpty()
						 join o2 in _operationRepository.GetAllQueryable() on a.Operation2C
							  equals o2.OperationC into t5
						 from o2 in t5.DefaultIfEmpty()
						 join o3 in _operationRepository.GetAllQueryable() on a.Operation3C
							  equals o3.OperationC into t6
						 from o3 in t6.DefaultIfEmpty()
						 join l1 in _locationRepository.GetAllQueryable() on a.Location1C
							  equals l1.LocationC into t10
						 from l1 in t10.DefaultIfEmpty()
						 join l2 in _locationRepository.GetAllQueryable() on a.Location2C
							  equals l2.LocationC into t8
						 from l2 in t8.DefaultIfEmpty()
						 join l3 in _locationRepository.GetAllQueryable() on a.Location3C
							  equals l3.LocationC into t9
						 from l3 in t9.DefaultIfEmpty()
						 where ((param.TransportDFrom == null || (param.ReportI == "O" && c.OrderD >= param.TransportDFrom) || (param.ReportI == "D" && a.TransportD >= param.TransportDFrom)) &
								(param.TransportDTo == null || (param.ReportI == "O" && c.OrderD <= param.TransportDTo) || (param.ReportI == "D" && a.TransportD <= param.TransportDTo)) &
								(param.TruckC == "null" || truckCList.Contains(a.TruckC)) &
								((param.DriverC == "null") || driverCList.Contains(a.DriverC)) &
								(string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || (param.ReportI == "O" && c.OrderDepC == param.DepC) || (param.ReportI == "D" && f.DepC == param.DepC)) &
								(string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || c.OrderTypeI == param.OrderTypeI)
							   )
						 select new DispatchDetailViewModel()
						 {
							 Dispatch = new DispatchViewModel
							 {
								 TransportD = a.TransportD,
								 DriverC = a.DriverC,
								 DriverN = e != null ? e.LastN + " " + e.FirstN : "",
								 TruckC = a.TruckC,
								 RegisteredNo = f != null ? f.RegisteredNo : "",
								 DispatchOrder = a.DispatchOrder,
								 ContainerStatus = a.ContainerStatus,
								 Location1N = a.Location1N,
								 Location2N = a.Location2N,
								 Location3N = a.Location3N,
								 Location1DT = a.Location1DT,
								 Location2DT = a.Location2DT,
								 Location3DT = a.Location3DT,
								 Location1A = l1 != null ? l1.Address : "",
								 Location2A = l2 != null ? l2.Address : "",
								 Location3A = l3 != null ? l3.Address : "",
								 Operation1N = (a.Operation1C != null && a.Operation1C != "" && a.Operation1C != "0") ? o1.OperationN : "",
								 Operation2N = (a.Operation2C != null && a.Operation2C != "" && a.Operation2C != "0") ? o2.OperationN : "",
								 Operation3N = (a.Operation3C != null && a.Operation3C != "" && a.Operation3C != "0") ? o3.OperationN : "",
								 Description = a.Description,
							 },
							 OrderD = new ContainerViewModel()
							 {
								 ContainerNo = b.ContainerNo,
								 SealNo = b.SealNo,
								 ContainerSizeI = b.ContainerSizeI,
								 NetWeight = b.ContainerSizeI == "3" ? b.NetWeight : 0,
								 CommodityN = b.CommodityN,
								 TrailerNo = t.TrailerNo,
							 },
							 OrderH = new OrderViewModel()
							 {
								 CustomerPayLiftSubC = c.CustomerPayLiftLoweredSubC,
								 CustomerPayLiftMainC = c.CustomerPayLiftLoweredMainC,
								 OrderTypeI = c.OrderTypeI,
								 CustomerShortN = d != null ? d.CustomerShortN : "",
								 CustomerN = d != null ? d.CustomerN : "",
								 IsCollected = c.IsCollected,
								 BLBK = c.BLBK,
								 ShippingCompanyN = c.ShippingCompanyN
								 //LoadingDT = c.LoadingDT,
								 //StopoverDT = c.StopoverDT,
								 //DischargeDT = c.DischargeDT
							 }
						 };

			driver = driver.OrderBy("Dispatch.DriverN asc, Dispatch.RegisteredNo asc, Dispatch.TransportD asc, Dispatch.DispatchOrder asc");
			var driverList = driver.ToList();
            var basicSet = _basicRepository.GetAllQueryable().FirstOrDefault();
			for (int i = 0; i < driverList.Count; i++)
			{
				string custMainC = driverList[i].OrderH.CustomerPayLiftMainC;
				string custSuc = driverList[i].OrderH.CustomerPayLiftSubC;
				var cust =
					_customerRepository.Query(p => p.CustomerMainC == custMainC && p.CustomerSubC == custSuc).FirstOrDefault();
				if (driverList[i].OrderH.IsCollected == "1")
				{
					driverList[i].OrderH.IsCollected = basicSet != null ? basicSet.CompanyFullN +", "+ basicSet.Address1+", " + basicSet.TaxCode : "";
				}
                else if (driverList[i].OrderH.IsCollected == "0")
				{
					driverList[i].OrderH.IsCollected = cust != null
						? (cust.CustomerN + ", " + cust.Address1 + ", " + cust.TaxCode)
						: "";
				}
				else driverList[i].OrderH.IsCollected = "";

			}
			return driverList;
		}

		public Stream ExportExcelCustomerExpense(CustomerExpenseReportParam param)
		{
			List<CustomerExpenseReportData> data = GetCustomerExpenseReportList(param);
			Dictionary<string, string> dicLanguage;
			int intLanguage;

			// get language for report
			dicLanguage = new Dictionary<string, string>();

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

			// get basic infomation
			var company = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (company != null)
			{
				dicLanguage.Add("COMPANYNAME", company.CompanyFullN);
				dicLanguage.Add("COMPANYADDRESS", company.Address1);
				dicLanguage.Add("COMPANYTAXCODE", company.TaxCode);
			}

			// get customer name
			var customer = _customerRepository.Query(cus => cus.CustomerMainC == param.CustomerMainC &&
															cus.CustomerSubC == param.CustomerSubC
													).FirstOrDefault();
			if (customer != null)
			{
				dicLanguage.Add("CUSTOMERNAME", customer.CustomerN);
			}
			else
			{
				dicLanguage.Add("CUSTOMERNAME", "");
			}
			dicLanguage.Add("CUSTOMERMAINCODE", param.CustomerMainC);

			var customerSettlement = _customerSettlementRepository.Query(cus => cus.CustomerMainC == param.CustomerMainC &&
																				cus.CustomerSubC == param.CustomerSubC &&
																				cus.ApplyD < DateTime.Now
																		);
			var customerSettlementOrder = customerSettlement.OrderBy("ApplyD desc").FirstOrDefault();
			if (customerSettlementOrder != null)
			{
				if (customerSettlementOrder.TaxRate == null)
				{
					dicLanguage.Add("COMPANYTAXRATE", "0");
				}
				else
				{
					dicLanguage.Add("COMPANYTAXRATE", customerSettlementOrder.TaxRate.ToString());
				}
			}

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
											   (con.TextKey == "LBLCUSTCODE" ||
												con.TextKey == "LBLTRANSPORTFEEREPORT" ||
												con.TextKey == "LBLTAXVAT" ||
												con.TextKey == "LBLTRANSPORTFEEAFTERTAX" ||
												con.TextKey == "LBLEXPENSEREPORT" ||
												con.TextKey == "LBLTOTALAMOUNTREPORT" ||
												con.TextKey == "LBLLOADINGDATEDISPATCH" ||
												con.TextKey == "LBLDISCHARGEDATEDISPATCH" ||
												con.TextKey == "LBLDETAINDAYREPORT" ||
												con.TextKey == "LBLLOCATIONREPORT" ||
												con.TextKey == "LBLCONTAINERNODISPATCH" ||
												con.TextKey == "LBLDETAINFEE" ||
												con.TextKey == "LBLSURCHARGEREPORT" ||
												con.TextKey == "LBLCONTAINERTYPEREPORT" ||
												con.TextKey == "LBLEXPENSETYPEREPORT" ||
												con.TextKey == "LBLINVOICENOREPORT" ||
												con.TextKey == "LBLAMOUNTREPORT" ||
												con.TextKey == "LBLTOTALREPORT" ||
												con.TextKey == "TLTTRANSPORTFEEREPORT" ||
												con.TextKey == "LBLDEARREPORT" ||
												con.TextKey == "LBLTAXCODECOMPANYREPORT" ||
												con.TextKey == "LBLTRUCKNO" ||
												con.TextKey == "LBLTAXAMOUNT" ||
												con.TextKey == "LBLORDERTYPEREPORT" ||
												con.TextKey == "LBLADDRESS" ||
												con.TextKey == "LBLCUSTOMER" ||
												con.TextKey == "LBLORDERTYPEDISPATCH" ||
												con.TextKey == "LBLBLBKREPORT" ||
												con.TextKey == "LBLDESCRIPTION" ||
												con.TextKey == "LBLLOAD" ||
												con.TextKey == "LBLCHARGETRANSPORT" ||
												con.TextKey == "LBLCONTNUMBER" ||
												con.TextKey == "LBLDESCRIPTIONREPORT"
										)).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			// add month/year title
			var monthYear = "";
			if (param.ReportI.Equals("A"))
			{
				if (param.Languague == "vi")
				{
					monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
				else if (param.Languague == "jp")
				{
					monthYear = param.TransportM.Value.Year + "年" + param.TransportM.Value.Month + "月";
				}
				else
				{
					monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
			}
			else
			{
				var fromDate = (DateTime)param.OrderDFrom;
				var toDate = (DateTime)param.OrderDTo;

				monthYear = "From " + fromDate.ToString("MM/dd/yyyy") + " to " + toDate.ToString("MM/dd/yyyy");
				if (intLanguage == 1)
				{
					monthYear = "Từ ngày " + fromDate.ToString("dd/MM/yyyy") + " đến ngày " + toDate.ToString("dd/MM/yyyy");
				}
				if (intLanguage == 3)
				{
					monthYear = fromDate.Year + "年" + fromDate.Month + "月" + fromDate.Day + "日" + "から" +
								toDate.Year + "年" + toDate.Month + "月" + toDate.Day + "日" + "まで";
				}
			}
			dicLanguage.Add("LBLMONTHYEAR", monthYear);
			// get file name logo
			var fileName = "";
			//int beginDetetionDay = 1;
			//decimal detentionAmount = 0;
			var basic = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basic != null && !string.IsNullOrEmpty(basic.Logo))
			{
				fileName = basic.Logo;
				//detentionAmount = basic.DetentionAmount != null ? (decimal)basic.DetentionAmount : 0;
				//beginDetetionDay = basic.BeginDetentionDay;
			}

			return CrystalReport.Service.CustomerExpense.ExportExcel.ExportCustomerExpenseListToExcel(data, dicLanguage, intLanguage, fileName);
		}

		public Stream ExportExcelCustomerExpenseTransportFeeVertical(CustomerExpenseReportParam param)
		{
			List<CustomerExpenseReportData> data = GetCustomerExpenseReportList(param);
			Dictionary<string, string> dicLanguage;
			int intLanguage;

			// get language for report
			dicLanguage = new Dictionary<string, string>();

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

			// get basic infomation
			var company = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (company != null)
			{
				dicLanguage.Add("COMPANYNAME", company.CompanyFullN);
				dicLanguage.Add("COMPANYADDRESS", company.Address1);
				dicLanguage.Add("COMPANYTAXCODE", company.TaxCode);
			}

			// get customer name
			var customer = _customerRepository.Query(cus => cus.CustomerMainC == param.CustomerMainC &&
															cus.CustomerSubC == param.CustomerSubC
													).FirstOrDefault();
			if (customer != null)
			{
				dicLanguage.Add("CUSTOMERNAME", customer.CustomerN);
			}
			else
			{
				dicLanguage.Add("CUSTOMERNAME", "");
			}
			dicLanguage.Add("CUSTOMERMAINCODE", param.CustomerMainC);

			var customerSettlement = _customerSettlementRepository.Query(cus => cus.CustomerMainC == param.CustomerMainC &&
																				cus.CustomerSubC == param.CustomerSubC &&
																				cus.ApplyD < DateTime.Now
																		);
			var customerSettlementOrder = customerSettlement.OrderBy("ApplyD desc").FirstOrDefault();
			if (customerSettlementOrder != null)
			{
				if (customerSettlementOrder.TaxRate == null)
				{
					dicLanguage.Add("COMPANYTAXRATE", "0");
				}
				else
				{
					dicLanguage.Add("COMPANYTAXRATE", customerSettlementOrder.TaxRate.ToString());
				}
			}

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
											   (con.TextKey == "LBLCUSTCODE" ||
												con.TextKey == "LBLTAXEXCLUSIONAMOUNT" ||
												con.TextKey == "LBLTAXAMOUNTREPORT" ||
												con.TextKey == "LBLTRANSPORTFEEAFTERTAX" ||
												con.TextKey == "LBLISREQUESTEDRP" ||
												con.TextKey == "LBLTOTALAMOUNTREPORT" ||
												con.TextKey == "LBLLOADINGDATEDISPATCH" ||
												con.TextKey == "LBLDISCHARGEDATEDISPATCH" ||
												con.TextKey == "LBLDETAINDAYREPORT" ||
												con.TextKey == "LBLLOCATIONREPORT" ||
												con.TextKey == "LBLCONTAINERNODISPATCH" ||
												con.TextKey == "LBLDETAINFEE" ||
												con.TextKey == "LBLSURCHARGEFEE" ||
												con.TextKey == "LBLCATEGORY" ||
												con.TextKey == "LBLEXPENSETYPEREPORT" ||
												con.TextKey == "LBLINVOICENOREPORT" ||
												con.TextKey == "LBLAMOUNTREPORT" ||
												con.TextKey == "LBLTOTALREPORT" ||
												con.TextKey == "TLTTRANSPORTFEEREPORT" ||
												con.TextKey == "LBLDEARREPORT" ||
												con.TextKey == "LBLTAXCODECOMPANYREPORT" ||
												con.TextKey == "LBLTRUCKNO" ||
												con.TextKey == "LBLTAXAMOUNT" ||
												con.TextKey == "LBLORDERTYPEREPORT" ||
												con.TextKey == "LBLADDRESS" ||
												con.TextKey == "LBLCUSTOMER" ||
												con.TextKey == "LBLORDERTYPEDISPATCH" ||
												con.TextKey == "LBLBLBKREPORT" ||
												con.TextKey == "LBLDESCRIPTION" ||
												con.TextKey == "LBLLOAD" ||
												con.TextKey == "LBLAMOUNTSHORTRP" ||
												con.TextKey == "LBLCONTNUMBER" ||
												con.TextKey == "LBLDESCRIPTIONREPORT" ||
												con.TextKey == "LBLDATEREPORT" ||
												con.TextKey == "LBLSTOPOVERPLACESHORT" ||
												con.TextKey == "LBLLOADINGPLACESHORT" ||
												con.TextKey == "LBLDISCHARGEPLACESHORT" ||
												con.TextKey == "LBLEXPLAIN" ||
												con.TextKey == "LBLDEARREPORT" ||
												con.TextKey == "LBLPAYONBEHALFRP"
										)).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			// add month/year title
			var monthYear = "";
			if (param.ReportI.Equals("A"))
			{
				if (param.Languague == "vi")
				{
					monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
				else if (param.Languague == "jp")
				{
					monthYear = param.TransportM.Value.Year + "年" + param.TransportM.Value.Month + "月";
				}
				else
				{
					monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
			}
			else
			{
				var fromDate = (DateTime)param.OrderDFrom;
				var toDate = (DateTime)param.OrderDTo;

				monthYear = "From " + fromDate.ToString("MM/dd/yyyy") + " to " + toDate.ToString("MM/dd/yyyy");
				if (intLanguage == 1)
				{
					monthYear = "Từ ngày " + fromDate.ToString("dd/MM/yyyy") + " đến ngày " + toDate.ToString("dd/MM/yyyy");
				}
				if (intLanguage == 3)
				{
					monthYear = fromDate.Year + "年" + fromDate.Month + "月" + fromDate.Day + "日" + "から" +
								toDate.Year + "年" + toDate.Month + "月" + toDate.Day + "日" + "まで";
				}
			}
			dicLanguage.Add("LBLMONTHYEAR", monthYear);
			// get file name logo
			var fileName = "";
			//int beginDetetionDay = 1;
			//decimal detentionAmount = 0;
			var basic = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basic != null && !string.IsNullOrEmpty(basic.Logo))
			{
				fileName = basic.Logo;
				//detentionAmount = basic.DetentionAmount != null ? (decimal)basic.DetentionAmount : 0;
				//beginDetetionDay = basic.BeginDetentionDay;
			}
			var lolo = param.LoLo;
			return CrystalReport.Service.CustomerExpense.ExportExcel.ExportCustomerExpenseTransportFeeVertical(data, dicLanguage, intLanguage, fileName, lolo);
		}

		public Stream ExportExcelCustomerExpenseTransportFeeHorizontal(CustomerExpenseReportParam param)
		{
			List<CustomerExpenseReportData> data = GetCustomerExpenseReportList(param);
			int countdataList = data.Count;
			if (countdataList > 0)
			{
				for (int i = 0; i < countdataList; i++)
				{
					int countdata = data[i].CustomerExpenseList.Count;
					if (countdata > 0)
					{
						var keyNew = "";
						var keyOld = "";
						int checkaddlist = 0;
						keyOld = data[i].CustomerExpenseList[0].OrderD.OrderD + data[i].CustomerExpenseList[0].OrderD.OrderNo + data[i].CustomerExpenseList[0].OrderD.DetailNo;
						for (int j = 0; j < countdata; j++)
						{
							if (j + 1 < countdata)
							{
								keyNew = data[i].CustomerExpenseList[j + 1].OrderD.OrderD + data[i].CustomerExpenseList[j + 1].OrderD.OrderNo +
								         data[i].CustomerExpenseList[j + 1].OrderD.DetailNo;
								if (keyNew == keyOld)
								{
									List<LiftOnList> listLiftOn = new List<LiftOnList>();
									List<LiftOffList> listLiftOff = new List<LiftOffList>();
									List<OtherListLoLo> listOther = new List<OtherListLoLo>();
									string expCOld = data[i].CustomerExpenseList[j].ExpenseD.ExpenseC;
									var expenseOld = _expenseRepository.Query(p => p.ExpenseC == expCOld).FirstOrDefault();
									if (j + 2 < countdata)
									{
										var turnnew = data[i].CustomerExpenseList[j + 2].OrderD.OrderD +
													  data[i].CustomerExpenseList[j + 2].OrderD.OrderNo +
													  data[i].CustomerExpenseList[j + 2].OrderD.DetailNo;
										if (keyOld != turnnew)
										{
											if (expenseOld != null)
											{
												if (expenseOld.ExpenseI == "N")
												{
													LiftOnList item = new LiftOnList();
													item.AmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Amount;
													item.DescriptionLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
													listLiftOn.Add(item);
												}
												else if (expenseOld.ExpenseI == "H")
												{
													LiftOffList item = new LiftOffList();
													item.AmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Amount;
													item.DescriptionLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
													listLiftOff.Add(item);
												}
												else
												{
													OtherListLoLo item = new OtherListLoLo();
													item.AmountOther = data[i].CustomerExpenseList[j].ExpenseD.Amount;
													item.DescriptionOther = data[i].CustomerExpenseList[j].ExpenseD.ExpenseN ?? "";
													listOther.Add(item);
												}
											}
										}
									}
									else
									{
										if (expenseOld != null)
										{
											if (expenseOld.ExpenseI == "N")
											{
												LiftOnList item = new LiftOnList();
												item.AmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
												listLiftOn.Add(item);
											}
											else if (expenseOld.ExpenseI == "H")
											{
												LiftOffList item = new LiftOffList();
												item.AmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
												listLiftOff.Add(item);
											}
											else
											{
												OtherListLoLo item = new OtherListLoLo();
												item.AmountOther = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionOther = data[i].CustomerExpenseList[j].ExpenseD.ExpenseN ?? "";
												listOther.Add(item);
											}
										}
									}

									string expCNew = data[i].CustomerExpenseList[j + 1].ExpenseD.ExpenseC;
									var expenseNew = _expenseRepository.Query(p => p.ExpenseC == expCNew).FirstOrDefault();
									if (expenseNew != null)
									{
										if (expenseNew.ExpenseI == "N")
										{
											LiftOnList item = new LiftOnList();
											item.AmountLiftOn = data[i].CustomerExpenseList[j + 1].ExpenseD.Amount;
											item.DescriptionLiftOn = data[i].CustomerExpenseList[j + 1].ExpenseD.Description ?? "";
											listLiftOn.Add(item);
										}
										else if (expenseNew.ExpenseI == "H")
										{
											LiftOffList item = new LiftOffList();
											item.AmountLiftOff = data[i].CustomerExpenseList[j + 1].ExpenseD.Amount;
											item.DescriptionLiftOff = data[i].CustomerExpenseList[j + 1].ExpenseD.Description ?? "";
											listLiftOff.Add(item);
										}
										else
										{
											OtherListLoLo item = new OtherListLoLo();
											item.AmountOther = data[i].CustomerExpenseList[j + 1].ExpenseD.Amount;
											item.DescriptionOther = data[i].CustomerExpenseList[j + 1].ExpenseD.ExpenseN ?? "";
											listOther.Add(item);
										}
									}
									
									List<LiftOnList> listLiftOnBackup = data[i].CustomerExpenseList[j].ExpenseD.LiftOnList ??
									                                    new List<LiftOnList>();
									List<LiftOffList> listLiftOffBackup = data[i].CustomerExpenseList[j].ExpenseD.LiftOffList ??
									                                      new List<LiftOffList>();
									List<OtherListLoLo> listOtherBackup = data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo ??
									                                      new List<OtherListLoLo>();

									data[i].CustomerExpenseList[j].ExpenseD.LiftOnList = new List<LiftOnList>();
									data[i].CustomerExpenseList[j].ExpenseD.LiftOnList.AddRange(listLiftOnBackup);
									data[i].CustomerExpenseList[j].ExpenseD.LiftOnList.AddRange(listLiftOn);

									data[i].CustomerExpenseList[j].ExpenseD.LiftOffList = new List<LiftOffList>();
									data[i].CustomerExpenseList[j].ExpenseD.LiftOffList.AddRange(listLiftOffBackup);
									data[i].CustomerExpenseList[j].ExpenseD.LiftOffList.AddRange(listLiftOff);

									data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo = new List<OtherListLoLo>();
									data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo.AddRange(listOtherBackup);
									data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo.AddRange(listOther);
									data[i].CustomerExpenseList.RemoveAt(j + 1);
									keyOld = data[i].CustomerExpenseList[j].OrderD.OrderD + data[i].CustomerExpenseList[j].OrderD.OrderNo +
									         data[i].CustomerExpenseList[j].OrderD.DetailNo;
									countdata--;
									j--;
									checkaddlist++;
								}
								else
								{
									if (checkaddlist < 1)
									{
										string expC = data[i].CustomerExpenseList[j].ExpenseD.ExpenseC;
										var expense = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
										if (expense != null)
										{
											if (expense.ExpenseI == "N")
											{
												LiftOnList item = new LiftOnList();
												item.AmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
												item.IsIncludedLiftOn = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
												item.IsRequestedLiftOn = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
												item.TaxAmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
												data[i].CustomerExpenseList[j].ExpenseD.LiftOnList = new List<LiftOnList>();
												data[i].CustomerExpenseList[j].ExpenseD.LiftOnList.Add(item);
											}
											else if (expense.ExpenseI == "H")
											{
												LiftOffList item = new LiftOffList();
												item.AmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
												item.IsIncludedLiftOff = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
												item.IsRequestedLiftOff = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
												item.TaxAmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
												data[i].CustomerExpenseList[j].ExpenseD.LiftOffList = new List<LiftOffList>();
												data[i].CustomerExpenseList[j].ExpenseD.LiftOffList.Add(item);
											}
											else
											{
												OtherListLoLo item = new OtherListLoLo();
												item.AmountOther = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionOther = data[i].CustomerExpenseList[j].ExpenseD.ExpenseN ?? "";
												item.IsIncludedOther = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
												item.IsRequestedOther = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
												item.TaxAmountOther = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
												data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo = new List<OtherListLoLo>();
												data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo.Add(item);
											}
										}
									}
									keyOld = keyNew;
								}
							}
							else
							{
								if (j == 0 && checkaddlist < 1)
								{
									string expC = data[i].CustomerExpenseList[j].ExpenseD.ExpenseC;
									var expense = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									if (expense != null)
									{
										if (expense.ExpenseI == "N")
										{
											LiftOnList item = new LiftOnList();
											item.AmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Amount;
											item.DescriptionLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
											data[i].CustomerExpenseList[j].ExpenseD.LiftOnList = new List<LiftOnList>();
											data[i].CustomerExpenseList[j].ExpenseD.LiftOnList.Add(item);
										}
										else if (expense.ExpenseI == "H")
										{
											LiftOffList item = new LiftOffList();
											item.AmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Amount;
											item.DescriptionLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
											data[i].CustomerExpenseList[j].ExpenseD.LiftOffList = new List<LiftOffList>();
											data[i].CustomerExpenseList[j].ExpenseD.LiftOffList.Add(item);
										}
										else
										{
											OtherListLoLo item = new OtherListLoLo();
											item.AmountOther = data[i].CustomerExpenseList[j].ExpenseD.Amount;
											item.DescriptionOther = data[i].CustomerExpenseList[j].ExpenseD.ExpenseN ?? "";
											data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo = new List<OtherListLoLo>();
											data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo.Add(item);
										}
									}
								}
							}
						}
					}
				}

			}
			Dictionary<string, string> dicLanguage;
			int intLanguage;

			// get language for report
			dicLanguage = new Dictionary<string, string>();

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

			// get basic infomation
			var company = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (company != null)
			{
				dicLanguage.Add("COMPANYNAME", company.CompanyFullN);
				dicLanguage.Add("COMPANYADDRESS", company.Address1);
				dicLanguage.Add("COMPANYTAXCODE", company.TaxCode);
			}

			// get customer name
			var customer = _customerRepository.Query(cus => cus.CustomerMainC == param.CustomerMainC &&
															cus.CustomerSubC == param.CustomerSubC
													).FirstOrDefault();
			if (customer != null)
			{
				dicLanguage.Add("CUSTOMERNAME", customer.CustomerN);
			}
			else
			{
				dicLanguage.Add("CUSTOMERNAME", "");
			}
			dicLanguage.Add("CUSTOMERMAINCODE", param.CustomerMainC);

			var customerSettlement = _customerSettlementRepository.Query(cus => cus.CustomerMainC == param.CustomerMainC &&
																				cus.CustomerSubC == param.CustomerSubC &&
																				cus.ApplyD < DateTime.Now
																		);
			var customerSettlementOrder = customerSettlement.OrderBy("ApplyD desc").FirstOrDefault();
			if (customerSettlementOrder != null)
			{
				if (customerSettlementOrder.TaxRate == null)
				{
					dicLanguage.Add("COMPANYTAXRATE", "0");
				}
				else
				{
					dicLanguage.Add("COMPANYTAXRATE", customerSettlementOrder.TaxRate.ToString());
				}
			}

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
											   (con.TextKey == "LBLCUSTCODE" ||
												con.TextKey == "RPHDAMOUNT" ||
												con.TextKey == "LBLTAXAMOUNTREPORT" ||
												con.TextKey == "LBLTRANSPORTFEEAFTERTAX" ||
												con.TextKey == "LBLPAYONBEHALFRP" ||
												con.TextKey == "LBLTOTALAMOUNTREPORT" ||
												con.TextKey == "LBLLOADINGDATEDISPATCH" ||
												con.TextKey == "LBLDISCHARGEDATEDISPATCH" ||
												con.TextKey == "LBLDETAINDAYREPORT" ||
												con.TextKey == "LBLLOCATIONREPORT" ||
												con.TextKey == "LBLCONTAINERNODISPATCH" ||
												con.TextKey == "LBLDETAINFEE" ||
												con.TextKey == "LBLSURCHARGEFEE" ||
												con.TextKey == "LBLCATEGORY" ||
												con.TextKey == "LBLEXPENSETYPEREPORT" ||
												con.TextKey == "LBLINVOICENOREPORT" ||
												con.TextKey == "LBLAMOUNTREPORT" ||
												con.TextKey == "LBLTOTALREPORT" ||
												con.TextKey == "TLTTRANSPORTFEEREPORT" ||
												con.TextKey == "LBLDEARREPORT" ||
												con.TextKey == "LBLTAXCODECOMPANYREPORT" ||
												con.TextKey == "LBLTRUCKNO" ||
												con.TextKey == "LBLTAXAMOUNT" ||
												con.TextKey == "LBLORDERTYPEREPORT" ||
												con.TextKey == "LBLADDRESS" ||
												con.TextKey == "LBLCUSTOMER" ||
												con.TextKey == "LBLORDERTYPEDISPATCH" ||
												con.TextKey == "LBLBLBKREPORT" ||
												con.TextKey == "LBLDESCRIPTION" ||
												con.TextKey == "LBLLOAD" ||
												con.TextKey == "LBLAMOUNTSHORTRP" ||
												con.TextKey == "LBLCONTNUMBER" ||
												con.TextKey == "LBLDESCRIPTIONREPORT" ||
												con.TextKey == "LBLDATEREPORT" ||
												con.TextKey == "LBLSTOPOVERPLACESHORT" ||
												con.TextKey == "LBLLOADINGPLACESHORT" ||
												con.TextKey == "LBLDISCHARGEPLACESHORT" ||
												con.TextKey == "LBLEXPLAIN" ||
												con.TextKey == "LBLDEARREPORT" ||
												con.TextKey == "LBLLIFT" ||
												con.TextKey == "LBLLOWERED" ||
												con.TextKey == "LBLOTHER" ||
												con.TextKey == "LBLBILLLIFTON" ||
												con.TextKey == "LBLBILLLIFTOFF" ||
												con.TextKey == "LBLISREQUESTEDRP"
										)).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			// add month/year title
			var monthYear = "";
			if (param.ReportI.Equals("A"))
			{
				if (param.Languague == "vi")
				{
					monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
				else if (param.Languague == "jp")
				{
					monthYear = param.TransportM.Value.Year + "年" + param.TransportM.Value.Month + "月";
				}
				else
				{
					monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
			}
			else
			{
				var fromDate = (DateTime)param.OrderDFrom;
				var toDate = (DateTime)param.OrderDTo;

				monthYear = "From " + fromDate.ToString("MM/dd/yyyy") + " to " + toDate.ToString("MM/dd/yyyy");
				if (intLanguage == 1)
				{
					monthYear = "Từ ngày " + fromDate.ToString("dd/MM/yyyy") + " đến ngày " + toDate.ToString("dd/MM/yyyy");
				}
				if (intLanguage == 3)
				{
					monthYear = fromDate.Year + "年" + fromDate.Month + "月" + fromDate.Day + "日" + "から" +
								toDate.Year + "年" + toDate.Month + "月" + toDate.Day + "日" + "まで";
				}
			}
			dicLanguage.Add("LBLMONTHYEAR", monthYear);
			// get file name logo
			var fileName = "";
			//int beginDetetionDay = 1;
			//decimal detentionAmount = 0;
			var basic = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basic != null && !string.IsNullOrEmpty(basic.Logo))
			{
				fileName = basic.Logo;
				//detentionAmount = basic.DetentionAmount != null ? (decimal)basic.DetentionAmount : 0;
				//beginDetetionDay = basic.BeginDetentionDay;
			}
			var lolo = param.LoLo;
			return CrystalReport.Service.CustomerExpense.ExportExcel.ExportExcelCustomerExpenseTransportFeeHorizontal(data, dicLanguage, intLanguage, fileName, lolo);
		}

		public Stream ExportExcelCustomerExpenseLOLOHorizontal(CustomerExpenseReportParam param)
		{
			List<CustomerExpenseReportData> data = GetCustomerExpenseReportList(param);
			int countdataList = data.Count;
			if (countdataList > 0)
			{
				for (int i = 0; i < countdataList; i++)
				{
					int countdata = data[i].CustomerExpenseList.Count;
					if (countdata > 0)
					{
						var keyNew = "";
						var keyOld = "";
						int checkaddlist = 0;
						keyOld = data[i].CustomerExpenseList[0].OrderD.OrderD + data[i].CustomerExpenseList[0].OrderD.OrderNo + data[i].CustomerExpenseList[0].OrderD.DetailNo;
						for (int j = 0; j < countdata; j++)
						{
							if (j + 1 < countdata)
							{
								keyNew = data[i].CustomerExpenseList[j + 1].OrderD.OrderD + data[i].CustomerExpenseList[j + 1].OrderD.OrderNo +
										 data[i].CustomerExpenseList[j + 1].OrderD.DetailNo;
								if (keyNew == keyOld)
								{
									List<LiftOnList> listLiftOn = new List<LiftOnList>();
									List<LiftOffList> listLiftOff = new List<LiftOffList>();
									List<OtherListLoLo> listOther = new List<OtherListLoLo>();
									string expCOld = data[i].CustomerExpenseList[j].ExpenseD.ExpenseC;
									var expenseOld = _expenseRepository.Query(p => p.ExpenseC == expCOld).FirstOrDefault();
									if (j + 2 < countdata)
									{
										var turnnew = data[i].CustomerExpenseList[j + 2].OrderD.OrderD +
													  data[i].CustomerExpenseList[j + 2].OrderD.OrderNo +
													  data[i].CustomerExpenseList[j + 2].OrderD.DetailNo;
										if (keyOld != turnnew)
										{
											if (expenseOld != null)
											{
												if (expenseOld.ExpenseI == "N")
												{
													LiftOnList item = new LiftOnList();
													item.AmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Amount;
													item.DescriptionLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
													listLiftOn.Add(item);
												}
												else if (expenseOld.ExpenseI == "H")
												{
													LiftOffList item = new LiftOffList();
													item.AmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Amount;
													item.DescriptionLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
													listLiftOff.Add(item);
												}
												else
												{
													OtherListLoLo item = new OtherListLoLo();
													item.AmountOther = data[i].CustomerExpenseList[j].ExpenseD.Amount;
													item.DescriptionOther = data[i].CustomerExpenseList[j].ExpenseD.ExpenseN ?? "";
													listOther.Add(item);
												}
											}
										}
									}
									else
									{
										if (expenseOld != null)
										{
											if (expenseOld.ExpenseI == "N")
											{
												LiftOnList item = new LiftOnList();
												item.AmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
												listLiftOn.Add(item);
											}
											else if (expenseOld.ExpenseI == "H")
											{
												LiftOffList item = new LiftOffList();
												item.AmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
												listLiftOff.Add(item);
											}
											else
											{
												OtherListLoLo item = new OtherListLoLo();
												item.AmountOther = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionOther = data[i].CustomerExpenseList[j].ExpenseD.ExpenseN ?? "";
												listOther.Add(item);
											}
										}
										
									}

									string expCNew = data[i].CustomerExpenseList[j + 1].ExpenseD.ExpenseC;
									var expenseNew = _expenseRepository.Query(p => p.ExpenseC == expCNew).FirstOrDefault();
									if (expenseNew != null)
									{
										if (expenseNew.ExpenseI == "N")
										{
											LiftOnList item = new LiftOnList();
											item.AmountLiftOn = data[i].CustomerExpenseList[j + 1].ExpenseD.Amount;
											item.DescriptionLiftOn = data[i].CustomerExpenseList[j + 1].ExpenseD.Description ?? "";
											listLiftOn.Add(item);
										}
										else if (expenseNew.ExpenseI == "H")
										{
											LiftOffList item = new LiftOffList();
											item.AmountLiftOff = data[i].CustomerExpenseList[j + 1].ExpenseD.Amount;
											item.DescriptionLiftOff = data[i].CustomerExpenseList[j + 1].ExpenseD.Description ?? "";
											listLiftOff.Add(item);
										}
										else
										{
											OtherListLoLo item = new OtherListLoLo();
											item.AmountOther = data[i].CustomerExpenseList[j + 1].ExpenseD.Amount;
											item.DescriptionOther = data[i].CustomerExpenseList[j + 1].ExpenseD.ExpenseN ?? "";
											listOther.Add(item);
										}
									}
									
									List<LiftOnList> listLiftOnBackup = data[i].CustomerExpenseList[j].ExpenseD.LiftOnList ??
																		new List<LiftOnList>();
									List<LiftOffList> listLiftOffBackup = data[i].CustomerExpenseList[j].ExpenseD.LiftOffList ??
																		  new List<LiftOffList>();
									List<OtherListLoLo> listOtherBackup = data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo ??
																		  new List<OtherListLoLo>();

									data[i].CustomerExpenseList[j].ExpenseD.LiftOnList = new List<LiftOnList>();
									data[i].CustomerExpenseList[j].ExpenseD.LiftOnList.AddRange(listLiftOnBackup);
									data[i].CustomerExpenseList[j].ExpenseD.LiftOnList.AddRange(listLiftOn);

									data[i].CustomerExpenseList[j].ExpenseD.LiftOffList = new List<LiftOffList>();
									data[i].CustomerExpenseList[j].ExpenseD.LiftOffList.AddRange(listLiftOffBackup);
									data[i].CustomerExpenseList[j].ExpenseD.LiftOffList.AddRange(listLiftOff);

									data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo = new List<OtherListLoLo>();
									data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo.AddRange(listOtherBackup);
									data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo.AddRange(listOther);
									data[i].CustomerExpenseList.RemoveAt(j + 1);
									keyOld = data[i].CustomerExpenseList[j].OrderD.OrderD + data[i].CustomerExpenseList[j].OrderD.OrderNo +
											 data[i].CustomerExpenseList[j].OrderD.DetailNo;
									countdata--;
									j--;
									checkaddlist++;
								}
								else
								{
									if (checkaddlist < 1)
									{
										string expC = data[i].CustomerExpenseList[j].ExpenseD.ExpenseC;
										var expense = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
										if (expense != null)
										{
											if (expense.ExpenseI == "N")
											{
												LiftOnList item = new LiftOnList();
												item.AmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
												item.IsIncludedLiftOn = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
												item.IsRequestedLiftOn = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
												item.TaxAmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
												data[i].CustomerExpenseList[j].ExpenseD.LiftOnList = new List<LiftOnList>();
												data[i].CustomerExpenseList[j].ExpenseD.LiftOnList.Add(item);
											}
											else if (expense.ExpenseI == "H")
											{
												LiftOffList item = new LiftOffList();
												item.AmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
												item.IsIncludedLiftOff = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
												item.IsRequestedLiftOff = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
												item.TaxAmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
												data[i].CustomerExpenseList[j].ExpenseD.LiftOffList = new List<LiftOffList>();
												data[i].CustomerExpenseList[j].ExpenseD.LiftOffList.Add(item);
											}
											else
											{
												OtherListLoLo item = new OtherListLoLo();
												item.AmountOther = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionOther = data[i].CustomerExpenseList[j].ExpenseD.ExpenseN ?? "";
												item.IsIncludedOther = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
												item.IsRequestedOther = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
												item.TaxAmountOther = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
												data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo = new List<OtherListLoLo>();
												data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo.Add(item);
											}
										}
									}
									keyOld = keyNew;
								}
							}
							else
							{
								if (j == 0 && checkaddlist < 1)
								{
									string expC = data[i].CustomerExpenseList[j].ExpenseD.ExpenseC;
									var expense = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									if (expense != null)
									{
										if (expense.ExpenseI == "N")
										{
											LiftOnList item = new LiftOnList();
											item.AmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Amount;
											item.DescriptionLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
											data[i].CustomerExpenseList[j].ExpenseD.LiftOnList = new List<LiftOnList>();
											data[i].CustomerExpenseList[j].ExpenseD.LiftOnList.Add(item);
										}
										else if (expense.ExpenseI == "H")
										{
											LiftOffList item = new LiftOffList();
											item.AmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Amount;
											item.DescriptionLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
											data[i].CustomerExpenseList[j].ExpenseD.LiftOffList = new List<LiftOffList>();
											data[i].CustomerExpenseList[j].ExpenseD.LiftOffList.Add(item);
										}
										else
										{
											OtherListLoLo item = new OtherListLoLo();
											item.AmountOther = data[i].CustomerExpenseList[j].ExpenseD.Amount;
											item.DescriptionOther = data[i].CustomerExpenseList[j].ExpenseD.ExpenseN ?? "";
											data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo = new List<OtherListLoLo>();
											data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo.Add(item);
										}
									}
								}
							}
						}
					}
				}

			}
			Dictionary<string, string> dicLanguage;
			int intLanguage;

			// get language for report
			dicLanguage = new Dictionary<string, string>();

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

			// get basic infomation
			var company = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (company != null)
			{
				dicLanguage.Add("COMPANYNAME", company.CompanyFullN);
				dicLanguage.Add("COMPANYADDRESS", company.Address1);
				dicLanguage.Add("COMPANYTAXCODE", company.TaxCode);
			}

			// get customer name
			var customer = _customerRepository.Query(cus => cus.CustomerMainC == param.CustomerMainC &&
															cus.CustomerSubC == param.CustomerSubC
													).FirstOrDefault();
			if (customer != null)
			{
				dicLanguage.Add("CUSTOMERNAME", customer.CustomerN);
			}
			else
			{
				dicLanguage.Add("CUSTOMERNAME", "");
			}
			dicLanguage.Add("CUSTOMERMAINCODE", param.CustomerMainC);

			var customerSettlement = _customerSettlementRepository.Query(cus => cus.CustomerMainC == param.CustomerMainC &&
																				cus.CustomerSubC == param.CustomerSubC &&
																				cus.ApplyD < DateTime.Now
																		);
			var customerSettlementOrder = customerSettlement.OrderBy("ApplyD desc").FirstOrDefault();
			if (customerSettlementOrder != null)
			{
				if (customerSettlementOrder.TaxRate == null)
				{
					dicLanguage.Add("COMPANYTAXRATE", "0");
				}
				else
				{
					dicLanguage.Add("COMPANYTAXRATE", customerSettlementOrder.TaxRate.ToString());
				}
			}

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
											   (con.TextKey == "LBLCUSTCODE" ||
												con.TextKey == "LBLTRANSPORTFEEREPORT" ||
												con.TextKey == "LBLTAXVAT" ||
												con.TextKey == "LBLTRANSPORTFEEAFTERTAX" ||
												con.TextKey == "LBLISREQUESTEDRP" ||
												con.TextKey == "LBLTOTALAMOUNTREPORT" ||
												con.TextKey == "LBLLOADINGDATEDISPATCH" ||
												con.TextKey == "LBLDISCHARGEDATEDISPATCH" ||
												con.TextKey == "LBLDETAINDAYREPORT" ||
												con.TextKey == "LBLLOCATIONREPORT" ||
												con.TextKey == "LBLCONTAINERNODISPATCH" ||
												con.TextKey == "LBLDETAINFEE" ||
												con.TextKey == "LBLSURCHARGEREPORT" ||
												con.TextKey == "LBLCATEGORY" ||
												con.TextKey == "LBLEXPENSETYPEREPORT" ||
												con.TextKey == "LBLINVOICENOREPORT" ||
												con.TextKey == "LBLAMOUNTREPORT" ||
												con.TextKey == "LBLTOTALREPORT" ||
												con.TextKey == "TLTLOLOFEEREPORT" ||
												con.TextKey == "LBLDEARREPORT" ||
												con.TextKey == "LBLTAXCODECOMPANYREPORT" ||
												con.TextKey == "LBLTRUCKNO" ||
												con.TextKey == "LBLTAXAMOUNT" ||
												con.TextKey == "LBLORDERTYPEREPORT" ||
												con.TextKey == "LBLADDRESS" ||
												con.TextKey == "LBLCUSTOMER" ||
												con.TextKey == "LBLORDERTYPEDISPATCH" ||
												con.TextKey == "LBLBLBKREPORT" ||
												con.TextKey == "LBLDESCRIPTION" ||
												con.TextKey == "LBLLOAD" ||
												con.TextKey == "LBLCHARGETRANSPORT" ||
												con.TextKey == "LBLCONTNUMBER" ||
												con.TextKey == "LBLDESCRIPTIONREPORT" ||
												con.TextKey == "LBLDATEREPORT" ||
												con.TextKey == "LBLSTOPOVERPLACESHORT" ||
												con.TextKey == "LBLLOADINGPLACESHORT" ||
												con.TextKey == "LBLDISCHARGEPLACESHORT" ||
												con.TextKey == "LBLEXPLAIN" ||
												con.TextKey == "LBLDEARREPORT" ||
												con.TextKey == "LBLLIFT" ||
												con.TextKey == "LBLLOWERED" ||
												con.TextKey == "LBLOTHER" ||
												con.TextKey == "LBLBILLLIFTON" ||
												con.TextKey == "LBLBILLLIFTOFF" ||
												con.TextKey == "LBLCUSTOMERPAYLIFTORLOWERED"
										)).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			// add month/year title
			var monthYear = "";
			if (param.ReportI.Equals("A"))
			{
				if (param.Languague == "vi")
				{
					monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
				else if (param.Languague == "jp")
				{
					monthYear = param.TransportM.Value.Year + "年" + param.TransportM.Value.Month + "月";
				}
				else
				{
					monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
			}
			else
			{
				var fromDate = (DateTime)param.OrderDFrom;
				var toDate = (DateTime)param.OrderDTo;

				monthYear = "From " + fromDate.ToString("MM/dd/yyyy") + " to " + toDate.ToString("MM/dd/yyyy");
				if (intLanguage == 1)
				{
					monthYear = "Từ ngày " + fromDate.ToString("dd/MM/yyyy") + " đến ngày " + toDate.ToString("dd/MM/yyyy");
				}
				if (intLanguage == 3)
				{
					monthYear = fromDate.Year + "年" + fromDate.Month + "月" + fromDate.Day + "日" + "から" +
								toDate.Year + "年" + toDate.Month + "月" + toDate.Day + "日" + "まで";
				}
			}
			dicLanguage.Add("LBLMONTHYEAR", monthYear);
			// get file name logo
			var fileName = "";
			//int beginDetetionDay = 1;
			//decimal detentionAmount = 0;
			var basic = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basic != null && !string.IsNullOrEmpty(basic.Logo))
			{
				fileName = basic.Logo;
				//detentionAmount = basic.DetentionAmount != null ? (decimal)basic.DetentionAmount : 0;
				//beginDetetionDay = basic.BeginDetentionDay;
			}

			return CrystalReport.Service.CustomerExpense.ExportExcel.ExportExcelCustomerExpenseLOLOHorizontal(data, dicLanguage, intLanguage, fileName);
		}

		public Stream ExportExcelCustomerExpenseLOLOVertical(CustomerExpenseReportParam param)
		{
			List<CustomerExpenseReportData> data = GetCustomerExpenseReportList(param);
			Dictionary<string, string> dicLanguage;
			int intLanguage;

			// get language for report
			dicLanguage = new Dictionary<string, string>();

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

			// get basic infomation
			var company = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (company != null)
			{
				dicLanguage.Add("COMPANYNAME", company.CompanyFullN);
				dicLanguage.Add("COMPANYADDRESS", company.Address1);
				dicLanguage.Add("COMPANYTAXCODE", company.TaxCode);
			}

			// get customer name
			var customer = _customerRepository.Query(cus => cus.CustomerMainC == param.CustomerMainC &&
															cus.CustomerSubC == param.CustomerSubC
													).FirstOrDefault();
			if (customer != null)
			{
				dicLanguage.Add("CUSTOMERNAME", customer.CustomerN);
			}
			else
			{
				dicLanguage.Add("CUSTOMERNAME", "");
			}
			dicLanguage.Add("CUSTOMERMAINCODE", param.CustomerMainC);

			var customerSettlement = _customerSettlementRepository.Query(cus => cus.CustomerMainC == param.CustomerMainC &&
																				cus.CustomerSubC == param.CustomerSubC &&
																				cus.ApplyD < DateTime.Now
																		);
			var customerSettlementOrder = customerSettlement.OrderBy("ApplyD desc").FirstOrDefault();
			if (customerSettlementOrder != null)
			{
				if (customerSettlementOrder.TaxRate == null)
				{
					dicLanguage.Add("COMPANYTAXRATE", "0");
				}
				else
				{
					dicLanguage.Add("COMPANYTAXRATE", customerSettlementOrder.TaxRate.ToString());
				}
			}

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
											   (con.TextKey == "LBLCUSTCODE" ||
												con.TextKey == "LBLTRANSPORTFEEREPORT" ||
												con.TextKey == "LBLTOTALAMOUNTREPORT" ||
												con.TextKey == "LBLLOADINGDATEDISPATCH" ||
												con.TextKey == "LBLDISCHARGEDATEDISPATCH" ||
												con.TextKey == "LBLDETAINDAYREPORT" ||
												con.TextKey == "LBLLOCATIONREPORT" ||
												con.TextKey == "LBLCONTAINERNODISPATCH" ||
												con.TextKey == "LBLDETAINFEE" ||
												con.TextKey == "LBLSURCHARGEREPORT" ||
												con.TextKey == "LBLCATEGORY" ||
												con.TextKey == "LBLEXPENSETYPEREPORT" ||
												con.TextKey == "LBLINVOICENOREPORT" ||
												con.TextKey == "LBLAMOUNTREPORT" ||
												con.TextKey == "LBLTOTALREPORT" ||
												con.TextKey == "TLTLOLOFEEREPORT" ||
												con.TextKey == "LBLDEARREPORT" ||
												con.TextKey == "LBLTAXCODECOMPANYREPORT" ||
												con.TextKey == "LBLTRUCKNO" ||
												con.TextKey == "LBLTAXAMOUNT" ||
												con.TextKey == "LBLORDERTYPEREPORT" ||
												con.TextKey == "LBLADDRESS" ||
												con.TextKey == "LBLCUSTOMER" ||
												con.TextKey == "LBLORDERTYPEDISPATCH" ||
												con.TextKey == "LBLBLBKREPORT" ||
												con.TextKey == "LBLDESCRIPTION" ||
												con.TextKey == "LBLLOAD" ||
												con.TextKey == "LBLCHARGETRANSPORT" ||
												con.TextKey == "LBLCONTNUMBER" ||
												con.TextKey == "LBLDESCRIPTIONREPORT" ||
												con.TextKey == "LBLDATEREPORT" ||
												con.TextKey == "LBLSTOPOVERPLACESHORT" ||
												con.TextKey == "LBLLOADINGPLACESHORT" ||
												con.TextKey == "LBLDISCHARGEPLACESHORT" ||
												con.TextKey == "LBLEXPLAIN" ||
												con.TextKey == "LBLDEARREPORT" ||
												con.TextKey == "LBLLIFT" ||
												con.TextKey == "LBLLOWERED" ||
												con.TextKey == "LBLOTHER" ||
												con.TextKey == "LBLBILLLIFTON" ||
												con.TextKey == "LBLBILLLIFTOFF" ||
												con.TextKey == "LBLCUSTOMERPAYLIFTORLOWERED"
										)).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			// add month/year title
			var monthYear = "";
			if (param.ReportI.Equals("A"))
			{
				if (param.Languague == "vi")
				{
					monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
				else if (param.Languague == "jp")
				{
					monthYear = param.TransportM.Value.Year + "年" + param.TransportM.Value.Month + "月";
				}
				else
				{
					monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
			}
			else
			{
				var fromDate = (DateTime)param.OrderDFrom;
				var toDate = (DateTime)param.OrderDTo;

				monthYear = "From " + fromDate.ToString("MM/dd/yyyy") + " to " + toDate.ToString("MM/dd/yyyy");
				if (intLanguage == 1)
				{
					monthYear = "Từ ngày " + fromDate.ToString("dd/MM/yyyy") + " đến ngày " + toDate.ToString("dd/MM/yyyy");
				}
				if (intLanguage == 3)
				{
					monthYear = fromDate.Year + "年" + fromDate.Month + "月" + fromDate.Day + "日" + "から" +
								toDate.Year + "年" + toDate.Month + "月" + toDate.Day + "日" + "まで";
				}
			}
			dicLanguage.Add("LBLMONTHYEAR", monthYear);
			// get file name logo
			var fileName = "";
			//int beginDetetionDay = 1;
			//decimal detentionAmount = 0;
			var basic = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basic != null && !string.IsNullOrEmpty(basic.Logo))
			{
				fileName = basic.Logo;
				//detentionAmount = basic.DetentionAmount != null ? (decimal)basic.DetentionAmount : 0;
				//beginDetetionDay = basic.BeginDetentionDay;
			}

			return CrystalReport.Service.CustomerExpense.ExportExcel.ExportExcelCustomerExpenseLOLOVertical(data, dicLanguage, intLanguage, fileName);
		}

		public Stream ExportExcelCustomerExpenseGeneral(CustomerExpenseReportParam param)
		{
			List<CustomerExpenseReportData> data = GetCustomerExpenseGeneralReportList(param);
			int countdataList = data.Count;
			if (countdataList > 0)
			{
				for (int i = 0; i < countdataList; i++)
				{
					int countdata = data[i].CustomerExpenseList.Count;
					if (countdata > 0)
					{
						var keyNew = "";
						var keyOld = "";
						int checkaddlist = 0;
						keyOld = data[i].CustomerExpenseList[0].OrderD.OrderD + data[i].CustomerExpenseList[0].OrderD.OrderNo + data[i].CustomerExpenseList[0].OrderD.DetailNo;
						for (int j = 0; j < countdata; j++)
						{
							if (j + 1 < countdata)
							{
								keyNew = data[i].CustomerExpenseList[j + 1].OrderD.OrderD + data[i].CustomerExpenseList[j + 1].OrderD.OrderNo +
										 data[i].CustomerExpenseList[j + 1].OrderD.DetailNo;
								if (keyNew == keyOld)
								{
									List<LiftOnList> listLiftOn = new List<LiftOnList>();
									List<LiftOffList> listLiftOff = new List<LiftOffList>();
									List<OtherListLoLo> listOther = new List<OtherListLoLo>();
									string expCOld = data[i].CustomerExpenseList[j].ExpenseD.ExpenseC;
									var expenseOld = _expenseRepository.Query(p => p.ExpenseC == expCOld).FirstOrDefault();
									if (j + 2 < countdata)
									{
										var turnnew = data[i].CustomerExpenseList[j + 2].OrderD.OrderD +
													  data[i].CustomerExpenseList[j + 2].OrderD.OrderNo +
													  data[i].CustomerExpenseList[j + 2].OrderD.DetailNo;
										if (keyOld != turnnew)
										{
											if (expenseOld != null)
											{
												if (expenseOld.ExpenseI == "N")
												{
													LiftOnList item = new LiftOnList();
													item.AmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Amount;
													item.DescriptionLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
													item.IsIncludedLiftOn = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
													item.IsRequestedLiftOn = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
													item.TaxAmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
													listLiftOn.Add(item);
												}
												else if (expenseOld.ExpenseI == "H")
												{
													LiftOffList item = new LiftOffList();
													item.AmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Amount;
													item.DescriptionLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
													item.IsIncludedLiftOff = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
													item.IsRequestedLiftOff = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
													item.TaxAmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
													listLiftOff.Add(item);
												}
												else
												{
													OtherListLoLo item = new OtherListLoLo();
													item.AmountOther = data[i].CustomerExpenseList[j].ExpenseD.Amount;
													item.DescriptionOther = data[i].CustomerExpenseList[j].ExpenseD.ExpenseN ?? "";
													item.IsIncludedOther = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
													item.IsRequestedOther = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
													item.TaxAmountOther = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
													listOther.Add(item);
												}
											}
										}
									}
									else
									{
										if (expenseOld != null)
										{
											if (expenseOld.ExpenseI == "N")
											{
												LiftOnList item = new LiftOnList();
												item.AmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
												item.IsIncludedLiftOn = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
												item.IsRequestedLiftOn = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
												item.TaxAmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
												listLiftOn.Add(item);
											}
											else if (expenseOld.ExpenseI == "H")
											{
												LiftOffList item = new LiftOffList();
												item.AmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
												item.IsIncludedLiftOff = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
												item.IsRequestedLiftOff = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
												item.TaxAmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
												listLiftOff.Add(item);
											}
											else
											{
												OtherListLoLo item = new OtherListLoLo();
												item.AmountOther = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionOther = data[i].CustomerExpenseList[j].ExpenseD.ExpenseN ?? "";
												item.IsIncludedOther = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
												item.IsRequestedOther = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
												item.TaxAmountOther = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
												listOther.Add(item);
											}
										}
									}

									string expCNew = data[i].CustomerExpenseList[j + 1].ExpenseD.ExpenseC;
									var expenseNew = _expenseRepository.Query(p => p.ExpenseC == expCNew).FirstOrDefault();
									if (expenseNew != null)
									{
										if (expenseNew.ExpenseI == "N")
										{
											LiftOnList item = new LiftOnList();
											item.AmountLiftOn = data[i].CustomerExpenseList[j + 1].ExpenseD.Amount;
											item.DescriptionLiftOn = data[i].CustomerExpenseList[j + 1].ExpenseD.Description ?? "";
											item.IsIncludedLiftOn = data[i].CustomerExpenseList[j + 1].ExpenseD.IsIncluded;
											item.IsRequestedLiftOn = data[i].CustomerExpenseList[j + 1].ExpenseD.IsRequested;
											item.TaxAmountLiftOn = data[i].CustomerExpenseList[j + 1].ExpenseD.TaxAmount;
											listLiftOn.Add(item);
										}
										else if (expenseNew.ExpenseI == "H")
										{
											LiftOffList item = new LiftOffList();
											item.AmountLiftOff = data[i].CustomerExpenseList[j + 1].ExpenseD.Amount;
											item.DescriptionLiftOff = data[i].CustomerExpenseList[j + 1].ExpenseD.Description ?? "";
											item.IsIncludedLiftOff = data[i].CustomerExpenseList[j + 1].ExpenseD.IsIncluded;
											item.IsRequestedLiftOff = data[i].CustomerExpenseList[j + 1].ExpenseD.IsRequested;
											item.TaxAmountLiftOff = data[i].CustomerExpenseList[j + 1].ExpenseD.TaxAmount;
											listLiftOff.Add(item);
										}
										else
										{
											OtherListLoLo item = new OtherListLoLo();
											item.AmountOther = data[i].CustomerExpenseList[j + 1].ExpenseD.Amount;
											item.DescriptionOther = data[i].CustomerExpenseList[j + 1].ExpenseD.ExpenseN ?? "";
											item.IsIncludedOther = data[i].CustomerExpenseList[j + 1].ExpenseD.IsIncluded;
											item.IsRequestedOther = data[i].CustomerExpenseList[j + 1].ExpenseD.IsRequested;
											item.TaxAmountOther = data[i].CustomerExpenseList[j + 1].ExpenseD.TaxAmount;
											listOther.Add(item);
										}
									}

									List<LiftOnList> listLiftOnBackup = data[i].CustomerExpenseList[j].ExpenseD.LiftOnList ??
																		new List<LiftOnList>();
									List<LiftOffList> listLiftOffBackup = data[i].CustomerExpenseList[j].ExpenseD.LiftOffList ??
																		  new List<LiftOffList>();
									List<OtherListLoLo> listOtherBackup = data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo ??
																		  new List<OtherListLoLo>();

									data[i].CustomerExpenseList[j].ExpenseD.LiftOnList = new List<LiftOnList>();
									data[i].CustomerExpenseList[j].ExpenseD.LiftOnList.AddRange(listLiftOnBackup);
									data[i].CustomerExpenseList[j].ExpenseD.LiftOnList.AddRange(listLiftOn);

									data[i].CustomerExpenseList[j].ExpenseD.LiftOffList = new List<LiftOffList>();
									data[i].CustomerExpenseList[j].ExpenseD.LiftOffList.AddRange(listLiftOffBackup);
									data[i].CustomerExpenseList[j].ExpenseD.LiftOffList.AddRange(listLiftOff);

									data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo = new List<OtherListLoLo>();
									data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo.AddRange(listOtherBackup);
									data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo.AddRange(listOther);
									data[i].CustomerExpenseList.RemoveAt(j + 1);
									keyOld = data[i].CustomerExpenseList[j].OrderD.OrderD + data[i].CustomerExpenseList[j].OrderD.OrderNo +
											 data[i].CustomerExpenseList[j].OrderD.DetailNo;
									countdata--;
									j--;
									checkaddlist++;
								}
								else
								{
									if (checkaddlist < 1)
									{
										string expC = data[i].CustomerExpenseList[j].ExpenseD.ExpenseC;
										var expense = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
										if (expense != null)
										{
											if (expense.ExpenseI == "N")
											{
												LiftOnList item = new LiftOnList();
												item.AmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
												item.IsIncludedLiftOn = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
												item.IsRequestedLiftOn = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
												item.TaxAmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
												data[i].CustomerExpenseList[j].ExpenseD.LiftOnList = new List<LiftOnList>();
												data[i].CustomerExpenseList[j].ExpenseD.LiftOnList.Add(item);
											}
											else if (expense.ExpenseI == "H")
											{
												LiftOffList item = new LiftOffList();
												item.AmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
												item.IsIncludedLiftOff = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
												item.IsRequestedLiftOff = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
												item.TaxAmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
												data[i].CustomerExpenseList[j].ExpenseD.LiftOffList = new List<LiftOffList>();
												data[i].CustomerExpenseList[j].ExpenseD.LiftOffList.Add(item);
											}
											else
											{
												OtherListLoLo item = new OtherListLoLo();
												item.AmountOther = data[i].CustomerExpenseList[j].ExpenseD.Amount;
												item.DescriptionOther = data[i].CustomerExpenseList[j].ExpenseD.ExpenseN ?? "";
												item.IsIncludedOther = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
												item.IsRequestedOther = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
												item.TaxAmountOther = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
												data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo = new List<OtherListLoLo>();
												data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo.Add(item);
											}
										}
									}
									keyOld = keyNew;
								}
							}
							else
							{
								if (j == 0 && checkaddlist < 1)
								{
									string expC = data[i].CustomerExpenseList[j].ExpenseD.ExpenseC;
									var expense = _expenseRepository.Query(p => p.ExpenseC == expC).FirstOrDefault();
									if (expense != null)
									{
										if (expense.ExpenseI == "N")
										{
											LiftOnList item = new LiftOnList();
											item.AmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Amount;
											item.DescriptionLiftOn = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
											item.IsIncludedLiftOn = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
											item.IsRequestedLiftOn = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
											item.TaxAmountLiftOn = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
											data[i].CustomerExpenseList[j].ExpenseD.LiftOnList = new List<LiftOnList>();
											data[i].CustomerExpenseList[j].ExpenseD.LiftOnList.Add(item);
										}
										else if (expense.ExpenseI == "H")
										{
											LiftOffList item = new LiftOffList();
											item.AmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Amount;
											item.DescriptionLiftOff = data[i].CustomerExpenseList[j].ExpenseD.Description ?? "";
											item.IsIncludedLiftOff = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
											item.IsRequestedLiftOff = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
											item.TaxAmountLiftOff = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
											data[i].CustomerExpenseList[j].ExpenseD.LiftOffList = new List<LiftOffList>();
											data[i].CustomerExpenseList[j].ExpenseD.LiftOffList.Add(item);
										}
										else
										{
											OtherListLoLo item = new OtherListLoLo();
											item.AmountOther = data[i].CustomerExpenseList[j].ExpenseD.Amount;
											item.DescriptionOther = data[i].CustomerExpenseList[j].ExpenseD.ExpenseN ?? "";
											item.IsIncludedOther = data[i].CustomerExpenseList[j].ExpenseD.IsIncluded;
											item.IsRequestedOther = data[i].CustomerExpenseList[j].ExpenseD.IsRequested;
											item.TaxAmountOther = data[i].CustomerExpenseList[j].ExpenseD.TaxAmount;
											data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo = new List<OtherListLoLo>();
											data[i].CustomerExpenseList[j].ExpenseD.OtherListLoLo.Add(item);
										}
									}
								}
							}
						}
					}
				}

			}
			Dictionary<string, string> dicLanguage;
			int intLanguage;

			// get language for report
			dicLanguage = new Dictionary<string, string>();

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

			// get basic infomation
			var company = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (company != null)
			{
				dicLanguage.Add("COMPANYNAME", company.CompanyFullN);
				dicLanguage.Add("COMPANYADDRESS", company.Address1);
				dicLanguage.Add("COMPANYTAXCODE", company.TaxCode);
			}

			// get customer name
			var customer = _customerRepository.Query(cus => cus.CustomerMainC == param.CustomerMainC &&
															cus.CustomerSubC == param.CustomerSubC
													).FirstOrDefault();
			if (customer != null)
			{
				dicLanguage.Add("CUSTOMERNAME", customer.CustomerN);
			}
			else
			{
				dicLanguage.Add("CUSTOMERNAME", "");
			}
			dicLanguage.Add("CUSTOMERMAINCODE", param.CustomerMainC);

			var customerSettlement = _customerSettlementRepository.Query(cus => cus.CustomerMainC == param.CustomerMainC &&
																				cus.CustomerSubC == param.CustomerSubC &&
																				cus.ApplyD < DateTime.Now
																		);
			var customerSettlementOrder = customerSettlement.OrderBy("ApplyD desc").FirstOrDefault();
			if (customerSettlementOrder != null)
			{
				if (customerSettlementOrder.TaxRate == null)
				{
					dicLanguage.Add("COMPANYTAXRATE", "0");
				}
				else
				{
					dicLanguage.Add("COMPANYTAXRATE", customerSettlementOrder.TaxRate.ToString());
				}
			}

			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
											   (con.TextKey == "LBLCUSTCODE" ||
												con.TextKey == "LBLBILLCOST" ||
												con.TextKey == "LBLTAXAMOUNTREPORT" ||
												con.TextKey == "LBLAFTERTAXAMOUNTRP" ||
												con.TextKey == "LBLPAYONBEHALFRP" ||
												con.TextKey == "LBLTOTALAMOUNTREPORT" ||
												con.TextKey == "LBLLOADINGDATEDISPATCH" ||
												con.TextKey == "LBLDISCHARGEDATEDISPATCH" ||
												con.TextKey == "LBLDETAINDAYREPORT" ||
												con.TextKey == "LBLLOCATIONREPORT" ||
												con.TextKey == "LBLCONTAINERNODISPATCH" ||
												con.TextKey == "LBLDETAINFEE" ||
												con.TextKey == "LBLSURCHARGEFEE" ||
												con.TextKey == "LBLCATEGORY" ||
												con.TextKey == "LBLLOAD" ||
												con.TextKey == "LBLINVOICENOREPORT" ||
												con.TextKey == "LBLAMOUNTREPORT" ||
												con.TextKey == "LBLTOTALREPORT" ||
												con.TextKey == "TLTTRANSPORTFEEREPORT" ||
												con.TextKey == "LBLDEARREPORT" ||
												con.TextKey == "LBLTAXCODECOMPANYREPORT" ||
												con.TextKey == "LBLTRUCKNO" ||
												con.TextKey == "LBLTAXAMOUNT" ||
												con.TextKey == "LBLORDERTYPEREPORT" ||
												con.TextKey == "LBLADDRESS" ||
												con.TextKey == "LBLCUSTOMER" ||
												con.TextKey == "LBLORDERTYPEDISPATCH" ||
												con.TextKey == "LBLBLBKREPORT" ||
												con.TextKey == "LBLDESCRIPTION" ||
												con.TextKey == "LBLLOLO" ||
												con.TextKey == "LBLAMOUNTSHORTRP" ||
												con.TextKey == "LBLCONTNUMBER" ||
												con.TextKey == "LBLDESCRIPTIONREPORT" ||
												con.TextKey == "LBLDATEREPORT" ||
												con.TextKey == "LBLSTOPOVERPLACESHORT" ||
												con.TextKey == "LBLLOADINGPLACESHORT" ||
												con.TextKey == "LBLDISCHARGEPLACESHORT" ||
												con.TextKey == "LBLEXPLAIN" ||
												con.TextKey == "LBLDEARREPORT" ||
												con.TextKey == "LBLLIFT" ||
												con.TextKey == "LBLLOWERED" ||
												con.TextKey == "LBLOTHER" ||
												con.TextKey == "LBLBILLLIFTON" ||
												con.TextKey == "LBLBILLLIFTOFF" ||
												con.TextKey == "LBLISREQUESTEDRP" ||
												con.TextKey == "LBLPOSTAGE" ||
												con.TextKey == "LBLLOLOFEEINCLUDEVAT" ||
												con.TextKey == "LBLBEFORETAX"
										)).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			// add month/year title
			var monthYear = "";
			if (param.ReportI.Equals("A"))
			{
				if (param.Languague == "vi")
				{
					monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
				else if (param.Languague == "jp")
				{
					monthYear = param.TransportM.Value.Year + "年" + param.TransportM.Value.Month + "月";
				}
				else
				{
					monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
			}
			else
			{
				var fromDate = (DateTime)param.OrderDFrom;
				var toDate = (DateTime)param.OrderDTo;

				monthYear = "From " + fromDate.ToString("MM/dd/yyyy") + " to " + toDate.ToString("MM/dd/yyyy");
				if (intLanguage == 1)
				{
					monthYear = "Từ ngày " + fromDate.ToString("dd/MM/yyyy") + " đến ngày " + toDate.ToString("dd/MM/yyyy");
				}
				if (intLanguage == 3)
				{
					monthYear = fromDate.Year + "年" + fromDate.Month + "月" + fromDate.Day + "日" + "から" +
								toDate.Year + "年" + toDate.Month + "月" + toDate.Day + "日" + "まで";
				}
			}
			dicLanguage.Add("LBLMONTHYEAR", monthYear);
			// get file name logo
			var fileName = "";
			//int beginDetetionDay = 1;
			//decimal detentionAmount = 0;
			var basic = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basic != null && !string.IsNullOrEmpty(basic.Logo))
			{
				fileName = basic.Logo;
				//detentionAmount = basic.DetentionAmount != null ? (decimal)basic.DetentionAmount : 0;
				//beginDetetionDay = basic.BeginDetentionDay;
			}
			var lolo = param.LoLo;
			return CrystalReport.Service.CustomerExpense.ExportExcel.ExportCustomerExpenseListToExcelGeneral(data, dicLanguage, intLanguage, fileName, lolo);
		}

		public Stream ExportPdfCustomerExpense(CustomerExpenseReportParam param)
		{
			Stream stream;
			DataRow row;
			CustomerExpense.CustomerExpenseDataTable dt;
			Dictionary<string, string> dicLanguage;
			var companyName = "";
			var companyAddress = "";
			var companyTaxCode = "";
			var fileName = "";
			int intLanguage;
			int beginDetetionDay = 1;
			//decimal detentionAmount = 0;
			decimal totalColumn15 = 0;
			decimal totalTransport = 0;
			decimal totalDetain = 0;
			int startRecord = 0;

			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				companyTaxCode = basicSetting.TaxCode;
				fileName = basicSetting.Logo;
				//detentionAmount = basicSetting.DetentionAmount != null ? (decimal)basicSetting.DetentionAmount : 0;
				beginDetetionDay = basicSetting.BeginDetentionDay;
			}

			// get data
			dt = new CustomerExpense.CustomerExpenseDataTable();
			List<CustomerExpenseReportData> data = GetCustomerExpenseReportList(param);

			// get language for report
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
			dicLanguage = new Dictionary<string, string>();
			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																  (con.TextKey == "LBLLOAD" ||
																   con.TextKey == "LBLTON" ||
																   con.TextKey == "MNUCUSTOMEREXPENSEREPORT" ||
																   con.TextKey == "LBLTRANSPORTFEEREPORT" ||
																   con.TextKey == "LBLTAXAMOUNTREPORT" ||
																   con.TextKey == "LBLTRANSPORTFEEAFTERTAXRP" ||
																   con.TextKey == "LBLPAYONBEHALFRP" ||
																   con.TextKey == "LBLAFTERTAXAMOUNTRP" ||
																   con.TextKey == "LBLINVOICEATTACHED" ||
																   con.TextKey == "LBLCUSTOMER" ||
																   con.TextKey == "LBLADDRESSRP" ||
																   con.TextKey == "LBLORDERTYPEDISPATCH" ||
																   con.TextKey == "LBLLOADINGDATEDISPATCHRP" ||
																   con.TextKey == "LBLDISCHARGEDATEDISPATCHRP" ||
																   con.TextKey == "LBLTRUCKNODISPATCHRP" ||
																   con.TextKey == "LBLCONTNUMBER" ||
																   con.TextKey == "LBLCONTSIZERP" ||
																   con.TextKey == "MNULOCATION" ||
																   con.TextKey == "LBLAMOUNTRP" ||
																   con.TextKey == "LBLAMOUNTSHORTRP" ||
																   con.TextKey == "LBLSURCHARGEFEE" ||
																   con.TextKey == "LBLTETOTAL" ||
																   con.TextKey == "LBLISREQUESTEDRP" ||
																   con.TextKey == "LBLCATEGORY" ||
																   con.TextKey == "LBLAMOUNTMONEY" ||
																   con.TextKey == "LBLVOUCHER" ||
																   con.TextKey == "LBLEXPLAIN" ||
																   con.TextKey == "LBLDISPATCHTOTALRP" ||
																   con.TextKey == "LBLTRANSPORTDATEDISPATCHRP"
																  )).ToList();

			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			// create 
			if (data != null && data.Count > 0)
			{
				for (var iloop = 0; iloop < data.Count; iloop++)
				{
					decimal totalTransportFee = 0;
					decimal totalExpense = 0;
					decimal totalTax = 0;

					if (iloop != 0)
					{
						startRecord = dt.Rows.Count;
					}

					if (data[iloop].CustomerExpenseList != null && data[iloop].CustomerExpenseList.Count > 0)
					{
						var newKey = "";
						var oldKey = "";
						var rowOfGroup = 0;
						var index = 1;
						totalTransportFee = 0;
						totalExpense = 0;
						totalTax = 0;
						for (var jloop = 0; jloop < data[iloop].CustomerExpenseList.Count; jloop++)
						{
							row = dt.NewRow();
							decimal amount = 0;
							decimal detainAmount = 0;
							decimal surchargeAmount = 0;
							decimal taxAmount = 0;
							decimal taxRate = 0;
							newKey = data[iloop].CustomerExpenseList[jloop].OrderD.OrderD + data[iloop].CustomerExpenseList[jloop].OrderD.OrderNo + data[iloop].CustomerExpenseList[jloop].OrderD.DetailNo;
							if (newKey != oldKey)
							{
								oldKey = newKey;
								rowOfGroup = 1;
								var detetionDay = (data[iloop].CustomerExpenseList[jloop].DetainDay - beginDetetionDay + 1) > 0
													? (data[iloop].CustomerExpenseList[jloop].DetainDay - beginDetetionDay + 1)
													: 0;
								detainAmount = data[iloop].CustomerExpenseList[jloop].DetainAmount;
								// set info row
								row["No"] = index++;
								row["keyRow"] = row["No"];
								row["Department"] = data[iloop].CustomerExpenseList[jloop].OrderH.OrderDepN;
								row["OrderType"] = Utilities.GetOrderTypeName(data[iloop].CustomerExpenseList[jloop].OrderH.OrderTypeI);
								//row["LoadingDate"] = (data[iloop].CustomerExpenseList[jloop].OrderD.ActualLoadingD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].CustomerExpenseList[jloop].OrderD.ActualLoadingD, intLanguage) : "")
								//				+ "<br>" + (data[iloop].CustomerExpenseList[jloop].OrderD.ActualDischargeD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].CustomerExpenseList[jloop].OrderD.ActualDischargeD, intLanguage) : "");
								row["TransportD"] = (data[iloop].CustomerExpenseList[jloop].TransportD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].CustomerExpenseList[jloop].TransportD, intLanguage) : "");
								if (data[iloop].CustomerExpenseList[jloop].DetainDay != 0)
								{
									row["DetainDay"] = data[iloop].CustomerExpenseList[jloop].DetainDay;
									totalDetain = totalDetain + detainAmount;//detetionDay * detentionAmount;
								}
								//string location = "";
								//if (!string.IsNullOrEmpty(data[iloop].CustomerExpenseList[jloop].OrderD.LocationDispatch1))
								//{
								//	location += data[iloop].CustomerExpenseList[jloop].OrderD.LocationDispatch1 + ", ";
								//}
								//if (!string.IsNullOrEmpty(data[iloop].CustomerExpenseList[jloop].OrderD.LocationDispatch2))
								//{
								//	location += data[iloop].CustomerExpenseList[jloop].OrderD.LocationDispatch2 + ", ";
								//}
								//if (!string.IsNullOrEmpty(data[iloop].CustomerExpenseList[jloop].OrderD.LocationDispatch3))
								//{
								//	location += data[iloop].CustomerExpenseList[jloop].OrderD.LocationDispatch3 + ", ";
								//}
								//if (location.Length > 1)
								//	row["Location"] = location.Remove(location.Length - 2);
								row["Location1"] = data[iloop].CustomerExpenseList[jloop].OrderD.LocationDispatch1 ?? "" ;

								row["Location2"] = data[iloop].CustomerExpenseList[jloop].OrderD.LocationDispatch2 ?? "";

								row["Location3"] = data[iloop].CustomerExpenseList[jloop].OrderD.LocationDispatch3 ?? "";

								row["BKBL"] = data[iloop].CustomerExpenseList[jloop].OrderH.BLBK;
								row["RegisteredNoLastDispatch"] = data[iloop].CustomerExpenseList[jloop].OrderD.RegisteredNo;

								row["ContainerNo"] = data[iloop].CustomerExpenseList[jloop].OrderD.ContainerNo
									+ "<br>" + (data[iloop].CustomerExpenseList[jloop].OrderD.ContainerSizeI == "3"
												? (data[iloop].CustomerExpenseList[jloop].OrderD.NetWeight > 0
													? dicLanguage["LBLLOAD"] + " (" + data[iloop].CustomerExpenseList[jloop].OrderD.NetWeight + ")" // " " + dicLanguage["LBLTON"] +
													: dicLanguage["LBLLOAD"])
												: Utilities.GetContainerSizeName(data[iloop].CustomerExpenseList[jloop].OrderD.ContainerSizeI));

								if (data[iloop].CustomerExpenseList[jloop].OrderD.Amount != null)
								{
									amount = (decimal)data[iloop].CustomerExpenseList[jloop].OrderD.Amount;
									row["TransportFee"] = amount;
									totalTransportFee += amount;
								}
								// set detain amount
								//if (data[iloop].CustomerExpenseList[jloop].DetainDay > 0)
								//{
								surchargeAmount = data[iloop].CustomerExpenseList[jloop].SurchargeAmount;
								row["DetainFee"] = detainAmount + surchargeAmount;
								row["Sum"] = amount + detainAmount + surchargeAmount;
								totalTransportFee += detainAmount + surchargeAmount;
								//}
								//else
								//{
								//	if (data[iloop].CustomerExpenseList[jloop].SurchargeAmount > 0)
								//	{
								//		row["DetainFee"] = (data[iloop].CustomerExpenseList[jloop].SurchargeAmount).ToString("#,###", cul.NumberFormat);
								//		totalTransportFee = totalTransportFee + data[iloop].CustomerExpenseList[jloop].SurchargeAmount;
								//	}
								//}
								taxAmount = data[iloop].CustomerExpenseList[jloop].TaxAmount;
								row["TaxAmount"] = taxAmount;
								row["Description"] = data[iloop].CustomerExpenseList[jloop].Description;
								// cal totalColumn15
								totalColumn15 = 0;
								if (data[iloop].CustomerExpenseList[jloop].OrderD.Amount != null)
								{
									totalTransport = totalTransport + amount;

									//if (data[iloop].TaxMethodI == "0")
									//{
									//	totalColumn15 = totalColumn15 + ((decimal)data[iloop].CustomerExpenseList[jloop].OrderD.Amount + detainAmount + data[iloop].CustomerExpenseList[jloop].SurchargeAmount) * (100 + data[iloop].TaxRate) / 100;
									//}
									//else
									//{
									//	totalColumn15 = totalColumn15 + Utilities.CalByMethodRounding(((decimal)data[iloop].CustomerExpenseList[jloop].OrderD.Amount + detainAmount + data[iloop].CustomerExpenseList[jloop].SurchargeAmount) * (100 + data[iloop].TaxRate) / 100, data[iloop].TaxRoundingI);
									//	totalTax = totalTax + Utilities.CalByMethodRounding(((decimal)data[iloop].CustomerExpenseList[jloop].OrderD.Amount + detainAmount + data[iloop].CustomerExpenseList[jloop].SurchargeAmount) * (data[iloop].TaxRate) / 100, data[iloop].TaxRoundingI);
									//}
									totalColumn15 += (decimal)(amount + detainAmount + surchargeAmount + taxAmount);
									totalTax += taxAmount;
								}
								if (data[iloop].CustomerExpenseList[jloop].ExpenseD.Amount != null)
								{
									totalColumn15 = totalColumn15 + (decimal)data[iloop].CustomerExpenseList[jloop].ExpenseD.Amount;
								}

								taxRate = (amount + detainAmount + surchargeAmount == 0) ? 0 : Math.Round(taxAmount * 100 / (amount + detainAmount + surchargeAmount));
								row["KeyCustomer"] = data[iloop].CustomerMainC + "_" + data[iloop].CustomerSubC + "_" + taxRate;
								row["TaxRate"] = taxRate;
							}
							else
							{
								row["DetainFee"] = 0;
								row["keyRow"] = dt.Rows[dt.Rows.Count - 1]["keyRow"];
								row["KeyCustomer"] = dt.Rows[dt.Rows.Count - 1]["KeyCustomer"];
								row["TaxRate"] = dt.Rows[dt.Rows.Count - 1]["TaxRate"];
								if (data[iloop].CustomerExpenseList[jloop].ExpenseD.Amount != null)
								{
									totalColumn15 = totalColumn15 + (decimal)data[iloop].CustomerExpenseList[jloop].ExpenseD.Amount;
								}

								rowOfGroup = rowOfGroup + 1;
							}

							row["ExpenseType"] = data[iloop].CustomerExpenseList[jloop].ExpenseD.ExpenseN;
							row["InvoiceNo"] = data[iloop].CustomerExpenseList[jloop].ExpenseD.Description;
							if (data[iloop].CustomerExpenseList[jloop].ExpenseD.Amount != null)
							{
								row["ExpenseFee"] = (decimal)data[iloop].CustomerExpenseList[jloop].ExpenseD.Amount;
								totalExpense = totalExpense + (decimal)data[iloop].CustomerExpenseList[jloop].ExpenseD.Amount;
							}
							else
							{
								row["ExpenseFee"] = 0;
							}

							// set info customer

							row["CustomerMainC"] = data[iloop].CustomerMainC;
							row["CustomerSubC"] = data[iloop].CustomerSubC;
							row["CustomerAddress"] = data[iloop].CustomerAddress;
							row["CustomerTaxCode"] = data[iloop].CustomerTaxCode;
							row["CustomerN"] = data[iloop].CustomerN;
							row["TaxMethodI"] = data[iloop].TaxMethodI;
							row["TaxRoundingI"] = data[iloop].TaxRoundingI;
							row["RevenueRoundingI"] = data[iloop].RevenueRoundingI;

							dt.Rows.Add(row);

							// write total
							if (jloop + 1 < data[iloop].CustomerExpenseList.Count)
							{
								var keyCheck = data[iloop].CustomerExpenseList[jloop + 1].OrderD.OrderD + data[iloop].CustomerExpenseList[jloop + 1].OrderD.OrderNo + data[iloop].CustomerExpenseList[jloop + 1].OrderD.DetailNo;
								if (newKey != keyCheck)
								{
									if (totalColumn15 > 0)
									{
										if (data[iloop].TaxMethodI == "0")
										{
											dt.Rows[dt.Rows.Count - rowOfGroup]["Total"] = totalColumn15.ToString("#,###.###", cul.NumberFormat);
										}
										else
										{
											dt.Rows[dt.Rows.Count - rowOfGroup]["Total"] = totalColumn15.ToString("#,###", cul.NumberFormat);
										}
									}
								}
							}
							else
							{
								if (totalColumn15 > 0)
								{
									if (data[iloop].TaxMethodI == "0")
									{
										dt.Rows[dt.Rows.Count - rowOfGroup]["Total"] = totalColumn15.ToString("#,###.###", cul.NumberFormat);
									}
									else
									{
										dt.Rows[dt.Rows.Count - rowOfGroup]["Total"] = totalColumn15.ToString("#,###", cul.NumberFormat);
									}
								}
							}
						}
					}
				}
			}

			// set month and year
			var monthYear = "";
			if (param.ReportI.Equals("A"))
			{
				monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				if (intLanguage == 1)
				{
					monthYear = "Tháng " + param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
				if (intLanguage == 3)
				{
					monthYear = param.TransportM.Value.Year + "年" + param.TransportM.Value.Month + "月";
				}
			}
			else
			{
				var fromDate = (DateTime)param.OrderDFrom;
				var toDate = (DateTime)param.OrderDTo;

				monthYear = "From " + fromDate.ToString("MM/dd/yyyy") + " to " + toDate.ToString("MM/dd/yyyy");
				if (intLanguage == 1)
				{
					monthYear = "Từ ngày " + fromDate.ToString("dd/MM/yyyy") + " đến ngày " + toDate.ToString("dd/MM/yyyy");
				}
				if (intLanguage == 3)
				{
					monthYear = fromDate.Year + "年" + fromDate.Month + "月" + fromDate.Day + "日" + "から" +
								toDate.Year + "年" + toDate.Month + "月" + toDate.Day + "日" + "まで";
				}
			}
			stream = CrystalReport.Service.CustomerExpense.ExportPdf.Exec(dt,
																		  intLanguage,
																		  companyName,
																		  companyAddress,
																		  companyTaxCode,
																		  monthYear,
																		  fileName,
																		  dicLanguage
																		 );
			return stream;
		}

		public List<CustomerExpenseReportData> GetCustomerExpenseGeneralReportList(CustomerExpenseReportParam param)
		{
			var dataList = new List<CustomerExpenseReportData>();
			var result = new List<CustomerExpenseReportData>();
			List<CustomerExpenseItem> listorder = new List<CustomerExpenseItem>();
			string sub = string.Empty;
			if (param.Customer == "null")
			{
				var customerList = _customerService.GetInvoices();

				if (customerList != null && customerList.Count > 0)
				{
					param.Customer = "";
					for (var iloop = 0; iloop < customerList.Count; iloop++)
					{
						string custMainC = customerList[iloop].CustomerMainC;
						string custSubC = customerList[iloop].CustomerSubC;
						var settlement0 = _customerSettlementRepository.Query(p => p.CustomerMainC == custMainC && p.CustomerSubC == custSubC && p.FormOfSettlement == Constants.NORMAL).OrderByDescending(p => p.ApplyD).FirstOrDefault();
						var settlement1 = _customerSettlementRepository.Query(p => p.CustomerMainC == custMainC && p.CustomerSubC == custSubC && p.FormOfSettlement == Constants.SETTLEMENT).OrderByDescending(p => p.ApplyD).FirstOrDefault();
						if (param.ReportI == "A")
						{
							if (settlement0 == null)
							{
								param.Customer = param.Customer + "," + customerList[iloop].CustomerMainC + "_" +
								                 customerList[iloop].CustomerSubC;
							}
						}
						if (param.ReportI == "N")
						{
							if (settlement1 == null)
							{
								param.Customer = param.Customer + "," + customerList[iloop].CustomerMainC + "_" +
								                 customerList[iloop].CustomerSubC;
							}
						}
					}
					sub = param.Customer.Substring(1);
					param.Customer = sub;
				}
			}
			
			// get data
			if (param.Customer != "null")
			{
				var customerArr = (param.Customer).Split(new string[] { "," }, StringSplitOptions.None);
				for (var iloop = 0; iloop < customerArr.Length; iloop++)
				{
					var item = new CustomerExpenseReportData();

					var arr = (customerArr[iloop]).Split(new string[] { "_" }, StringSplitOptions.None);
					var customerMainC = arr[0];
					var customerSubC = arr[1];
					var settlement0 = _customerSettlementRepository.Query(p => p.CustomerMainC == customerMainC && p.CustomerSubC == customerSubC && p.FormOfSettlement == "0").OrderByDescending(p => p.ApplyD).FirstOrDefault();
					var settlement1 = _customerSettlementRepository.Query(p => p.CustomerMainC == customerMainC && p.CustomerSubC == customerSubC && p.FormOfSettlement == "1").OrderByDescending(p => p.ApplyD).FirstOrDefault();

					if ((param.ReportI == "A" && settlement1 != null && settlement0 == null) ||
						(param.ReportI == "N" && settlement0 != null && settlement1 == null) ||
						((param.ReportI == "A" || param.ReportI == "N") &&
						 ((settlement1 == null && settlement0 == null) || (settlement1 != null && settlement0 != null))))
					{
						DateTime startDate;
						DateTime endDate;
						if (param.ReportI.Equals("A"))
						{
							// get month and year transport
							var month = param.TransportM.Value.Month;
							var year = param.TransportM.Value.Year;
							var invoiceInfo = _invoiceInfoService.GetLimitStartAndEndInvoiceD(customerMainC, customerSubC, month, year);
							startDate = invoiceInfo.StartDate.Date;
							endDate = invoiceInfo.EndDate.Date;
						}
						else
						{
							startDate = (DateTime) param.OrderDFrom;
							endDate = (DateTime) param.OrderDTo;
						}
						// get customers who shared a invoice company
						var customerStr = "";
						var customerList = _customerService.GetCustomersByInvoice(customerMainC, customerSubC);
						for (var aloop = 0; aloop < customerList.Count; aloop++)
						{
							customerStr = customerStr + "," + customerList[aloop].CustomerMainC + "_" + customerList[aloop].CustomerSubC;
						}

						// get and calculate detain date
						var dispatch = from a in _dispatchRepository.GetAllQueryable()
							join b in _orderDRepository.GetAllQueryable() on new {a.OrderD, a.OrderNo, a.DetailNo}
								equals new {b.OrderD, b.OrderNo, b.DetailNo} into t1
							from b in t1.DefaultIfEmpty()
							where ((param.InvoiceStatus == "-1" ||
							        (param.InvoiceStatus == "0" && a.InvoiceStatus != "1") ||
							        (param.InvoiceStatus == "1" && a.InvoiceStatus == "1")
								) &
							       (b.RevenueD >= startDate & b.RevenueD <= endDate))
							select new DispatchViewModel()
							{
								OrderD = a.OrderD,
								OrderNo = a.OrderNo,
								DetailNo = a.DetailNo,
								DispatchNo = a.DispatchNo,
								DetainDay = a.DetainDay,
								TruckC = a.TruckC,
								TransportD = a.TransportD
							};
						dispatch = dispatch.OrderBy("OrderD asc, OrderNo asc, DetailNo asc, DispatchNo asc");

						var groupDispatch = from b in dispatch
							group b by new {b.OrderD, b.OrderNo, b.DetailNo, b.TransportD}
							into c
							select new
							{
								c.Key.OrderD,
								c.Key.OrderNo,
								c.Key.DetailNo,
								c.Key.TransportD,
								DetainDay = c.Sum(b => b.DetainDay)
							};

						//get Truck No
						var truck = from b in dispatch
							join t in _truckRepository.GetAllQueryable()
								on b.TruckC equals t.TruckC
							where (b.DispatchNo == 1)
							select new
							{
								OrderD = b.OrderD,
								OrderNo = b.OrderNo,
								DetailNo = b.DetailNo,
								TruckNo = t.RegisteredNo
							};

						// get surcharge
						var surcharge = from a in _surchargeDetailRepository.GetAllQueryable()
							join b in groupDispatch on new {a.OrderD, a.OrderNo, a.DetailNo}
								equals new {b.OrderD, b.OrderNo, b.DetailNo} into t1
							where a.DispatchNo == 0
							select new SurchargeDetailViewModel()
							{
								OrderD = a.OrderD,
								OrderNo = a.OrderNo,
								DetailNo = a.DetailNo,
								Amount = a.Amount,
								Description = a.Description
							};
						var groupSurcharge = (from b in surcharge.AsEnumerable()
							group b by new {b.OrderD, b.OrderNo, b.DetailNo}
							into c
							select new
							{
								OrderD = c.Key.OrderD,
								OrderNo = c.Key.OrderNo,
								DetailNo = c.Key.DetailNo,
								Amount = c.Sum(b => b.Amount),
								Description = String.Join(",", c.Select(b => b.Description))
							}).ToList();
						//var temp = groupSurcharge.ToList();
						// get data
						var data = from d in _orderDRepository.GetAllQueryable()
							join e in _orderHRepository.GetAllQueryable() on new {d.OrderD, d.OrderNo}
								equals new {e.OrderD, e.OrderNo} into t1
							from e in t1.DefaultIfEmpty()
							join m in _truckRepository.GetAllQueryable() on d.TruckCLastDispatch
								equals m.TruckC into t9
							from m in t9.DefaultIfEmpty()
							//join c in _customerRepository.GetAllQueryable() on new { e.CustomerMainC, e.CustomerSubC }
							//	equals new { c.CustomerMainC, c.CustomerSubC } into t6
							//from c in t6.DefaultIfEmpty()
							join f in _departmentRepository.GetAllQueryable() on e.OrderDepC
								equals f.DepC into t2
							from f in t2.DefaultIfEmpty()
							join g in groupDispatch on new {d.OrderD, d.OrderNo, d.DetailNo}
								equals new {g.OrderD, g.OrderNo, g.DetailNo}
							join h in _expenseDetailRepository.Query(h => h.Amount > 0) on
								new {d.OrderD, d.OrderNo, d.DetailNo}
								equals new {h.OrderD, h.OrderNo, h.DetailNo} into t4
							from h in t4.DefaultIfEmpty()
							join k in _expenseRepository.GetAllQueryable() on h.ExpenseC
								equals k.ExpenseC into t5
							from k in t5.DefaultIfEmpty()
							//join s in groupSurcharge on new { d.OrderD, d.OrderNo, d.DetailNo }
							// equals new { s.OrderD, s.OrderNo, s.DetailNo } into t7
							//from s in t7.DefaultIfEmpty()
							join t in truck on new {d.OrderD, d.OrderNo, d.DetailNo}
								equals new {t.OrderD, t.OrderNo, t.DetailNo} into t8
							from t in t8.DefaultIfEmpty()
							where ((customerStr.Contains("," + e.CustomerMainC + "_" + e.CustomerSubC)) &
							       (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || e.OrderDepC == param.DepC) &
							       (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || e.OrderTypeI == param.OrderTypeI) &
							       (param.BLBK.Equals("null") || e.BLBK.Equals(param.BLBK)) &
							       (param.JobNo.Equals("null") || e.JobNo.Contains(param.JobNo))
								)
							select new CustomerExpenseItem()
							{
								OrderD = new ContainerViewModel()
								{
									OrderD = d.OrderD,
									OrderNo = d.OrderNo,
									DetailNo = d.DetailNo,
									ContainerNo = d.ContainerNo,
									ContainerSizeI = d.ContainerSizeI,
									Amount = d.Amount,
									ActualLoadingD = d.ActualLoadingD,
									ActualDischargeD = d.ActualDischargeD,
									NetWeight = d.NetWeight,
									RegisteredNo = m.RegisteredNo ?? "",
									LocationDispatch1 = d.LocationDispatch1 ?? "",
									LocationDispatch2 = d.LocationDispatch2 ?? "",
									LocationDispatch3 = d.LocationDispatch3 ?? "",
									CustomerSurcharge = d.CustomerSurcharge,
									TotalAmount = d.TotalAmount,
									TotalExpense = d.TotalExpense
								},
								OrderH = new OrderViewModel()
								{
									OrderD = e.OrderD,
									OrderNo = e.OrderNo,
									//CustomerMainC = e.CustomerMainC,
									//CustomerSubC = e.CustomerSubC,
									//CustomerN = c != null ? c.CustomerN : "",
									OrderTypeI = e.OrderTypeI,
									OrderDepN = f != null ? f.DepN : "",
									LoadingPlaceN = e.LoadingPlaceN,
									StopoverPlaceN = e.StopoverPlaceN,
									DischargePlaceN = e.DischargePlaceN,
									BLBK = e.BLBK,
									IsCollected = e.IsCollected,
									JobNo = e.JobNo,
									CustomerMainC = e.CustomerMainC,
									CustomerSubC = e.CustomerSubC,
									CustomerPayLiftLoweredMainC = e.CustomerPayLiftLoweredMainC,
									CustomerPayLiftLoweredSubC = e.CustomerPayLiftLoweredSubC
								},
								DetainDay = g.DetainDay,
								TransportD = g.TransportD,
								//SurchargeAmount = s.Amount != null ? (decimal)s.Amount : 0,
								//Description = s.Description,
								DetainAmount = d.DetainAmount ?? 0,
								TaxAmount = d.TaxAmount ?? 0,
								ExpenseD = new ExpenseDetailViewModel()
								{
									ExpenseC = h != null ? h.ExpenseC : "",
									ExpenseN = k != null ? k.ExpenseN : "",
									Description = h != null ? h.Description : "",
									Amount = h != null ? h.Amount : null,
									TaxAmount = h != null ? (h.TaxAmount ?? 0) : 0,
									TaxRate = h != null ? h.TaxRate : null,
									ExpenseI = k.ExpenseI,
									IsIncluded = h.IsIncluded,
									IsRequested = h.IsRequested
								},
								TruckNo = t.TruckNo,
								//CustomerAddress = c.Address1,
								//CustomerTaxCode = c.TaxCode
							};
						data = data.OrderBy("OrderD.OrderD asc, OrderD.OrderNo asc, OrderD.DetailNo asc");
						var listmultiple = data
							.GroupBy(u => u.OrderH.CustomerPayLiftLoweredMainC)
							.Select(grp => grp.ToList())
							.ToList();
						var list = data.ToList();
						if (param.ReportType == 0 || param.ReportType == 2 || param.ReportType == 3 ||
						    ((param.ReportType == 4 || param.ReportType == 5) && listmultiple.Count < 2))
						{
							string custMainCOrderH = "";
							string custSubCOrderH = "";
							string custPayMainCOrderH = "";
							string custPaySubCOrderH = "";
							//add surcharge
							if (list != null && list.Count > 0)
							{
								for (int i = 0; i < list.Count; i++)
								{
									custPayMainCOrderH = list[i].OrderH.CustomerPayLiftLoweredMainC;
									custPaySubCOrderH = list[i].OrderH.CustomerPayLiftLoweredSubC;

									custMainCOrderH = list[i].OrderH.CustomerMainC;
									custSubCOrderH = list[i].OrderH.CustomerSubC;
									var orderD = list[i].OrderH.OrderD;
									var orderNo = list[i].OrderH.OrderNo;
									var detailNo = list[i].OrderD.DetailNo;
									var surchargeData = (from s in groupSurcharge
										where s.OrderD == orderD &&
										      s.OrderNo == orderNo &&
										      s.DetailNo == detailNo
										select new CustomerExpenseItem()
										{
											SurchargeAmount = s.Amount ?? 0,
											Description = s.Description
										}).ToList();
									if (surchargeData != null && surchargeData.Count > 0)
									{
										list[i].SurchargeAmount = surchargeData[0].SurchargeAmount;
										list[i].Description = surchargeData[0].Description.Length > 1 ? surchargeData[0].Description : "";
									}
								}
								item.CustomerPayLiftLoweredMainC = custPayMainCOrderH;
								item.CustomerPayLiftLoweredSubC = custPaySubCOrderH;
								item.CustomerMainC = custMainCOrderH;
								item.CustomerSubC = custSubCOrderH;
								var customer =
									_customerRepository.Query(p => p.CustomerMainC == custMainCOrderH && p.CustomerSubC == custSubCOrderH)
										.FirstOrDefault();
								if (customer != null)
								{
									item.CustomerN = customer.CustomerN;
									item.CustomerAddress = customer.Address1;
									item.CustomerTaxCode = customer.TaxCode;
								}

								var customerPay =
									_customerRepository.Query(p => p.CustomerMainC == custPayMainCOrderH && p.CustomerSubC == custPaySubCOrderH)
										.FirstOrDefault();
								if (customerPay != null)
								{
									item.CustomerPayLiftLoweredN = customerPay.CustomerN;
									item.CustomerPayLiftLoweredAddress = customerPay.Address1;
								}

								for (var loop = 0; loop < list.Count; loop++)
								{
									var finalList = new List<CustomerExpenseItem>();
									var compareEmpty = 0;
									var compareAdd = 0;
									decimal taxRate = (100*(list[loop].TaxAmount))/
									                  ((list[loop].OrderD.TotalAmount != 0 && list[loop].OrderD.TotalAmount != null
										                  ? ((decimal) list[loop].OrderD.TotalAmount -
										                     (list[loop].OrderD.TotalExpense != 0 && list[loop].OrderD.TotalExpense != null
											                     ? (decimal) list[loop].OrderD.TotalExpense
											                     : 0))
										                  : 0) > 0
										                  ? ((decimal) list[loop].OrderD.TotalAmount - (decimal) list[loop].OrderD.TotalExpense)
										                  : 1);
									for (var subloop = 0; subloop < list.Count; subloop++)
									{
										decimal subTaxRate = (100*(list[subloop].TaxAmount))/
										                     ((list[subloop].OrderD.TotalAmount != 0 && list[subloop].OrderD.TotalAmount != null
											                     ? ((decimal) list[subloop].OrderD.TotalAmount -
											                        (list[subloop].OrderD.TotalExpense != 0 && list[subloop].OrderD.TotalExpense != null
												                        ? (decimal) list[subloop].OrderD.TotalExpense
												                        : 0))
											                     : 0) > 0
											                     ? ((decimal) list[subloop].OrderD.TotalAmount -
											                        (decimal) list[subloop].OrderD.TotalExpense)
											                     : 1);
										if (taxRate == subTaxRate)
										{
											finalList.Add(list[subloop]);
											list.RemoveAt(subloop);
											subloop--;
											compareAdd++;
										}
										else
										{
											compareEmpty++;
										}
									}
									if (compareEmpty > 0 && compareAdd == 0)
									{
										finalList.Add(list[loop]);
									}
									loop--;
									//item.CustomerExpenseList = finalList;
									var listAddGeneral = new CustomerExpenseReportData();
									listAddGeneral.CustomerMainC = custMainCOrderH;
									listAddGeneral.CustomerSubC = custSubCOrderH;
									var customer2 =
										_customerRepository.Query(p => p.CustomerMainC == custMainCOrderH && p.CustomerSubC == custSubCOrderH)
											.FirstOrDefault();
									if (customer2 != null)
									{
										listAddGeneral.CustomerN = customer2.CustomerN;
										listAddGeneral.CustomerAddress = customer2.Address1;
										listAddGeneral.CustomerTaxCode = customer2.TaxCode;
									}

									listAddGeneral.CustomerPayLiftLoweredMainC = custPayMainCOrderH;
									listAddGeneral.CustomerPayLiftLoweredSubC = custPaySubCOrderH;
									var customer3 =
										_customerRepository.Query(p => p.CustomerMainC == custPayMainCOrderH && p.CustomerSubC == custPaySubCOrderH)
											.FirstOrDefault();
									if (customer3 != null)
									{
										listAddGeneral.CustomerPayLiftLoweredN = customer3.CustomerN;
										listAddGeneral.CustomerPayLiftLoweredAddress = customer3.Address1;
									}

									listAddGeneral.CustomerExpenseList = finalList;
									listAddGeneral.TaxRate = Math.Round((100*(decimal) listAddGeneral.CustomerExpenseList[0].TaxAmount)/
									                                    ((listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount != null &&
									                                      listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount != 0
										                                    ? ((decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount -
										                                       (listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense != 0 &&
										                                        listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense != null
											                                       ? (decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense
											                                       : 0))
										                                    : 0) > 0
										                                    ? ((decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount -
										                                       (decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense)
										                                    : 1));
									listAddGeneral.TaxRate = listAddGeneral.TaxRate > 100 ? 0 : listAddGeneral.TaxRate;
									dataList.Add(listAddGeneral);
								}
								//dataList.Add(item);
							}
						}
						if ((param.ReportType == 4 || param.ReportType == 5) && listmultiple.Count >= 2)
						{
							for (int lm = 0; lm < listmultiple.Count; lm++)
							{
								string custMainCOrderH = "";
								string custSubCOrderH = "";
								string custPayMainCOrderH = "";
								string custPaySubCOrderH = "";
								//add surcharge
								if (listmultiple[lm] != null && listmultiple[lm].Count > 0)
								{
									for (int i = 0; i < listmultiple[lm].Count; i++)
									{
										custPayMainCOrderH = listmultiple[lm][i].OrderH.CustomerPayLiftLoweredMainC;
										custPaySubCOrderH = listmultiple[lm][i].OrderH.CustomerPayLiftLoweredSubC;

										custMainCOrderH = listmultiple[lm][i].OrderH.CustomerMainC;
										custSubCOrderH = listmultiple[lm][i].OrderH.CustomerSubC;
										var orderD = listmultiple[lm][i].OrderH.OrderD;
										var orderNo = listmultiple[lm][i].OrderH.OrderNo;
										var detailNo = listmultiple[lm][i].OrderD.DetailNo;
										var surchargeData = (from s in groupSurcharge
											where s.OrderD == orderD &&
											      s.OrderNo == orderNo &&
											      s.DetailNo == detailNo
											select new CustomerExpenseItem()
											{
												SurchargeAmount = s.Amount ?? 0,
												Description = s.Description
											}).ToList();
										if (surchargeData != null && surchargeData.Count > 0)
										{
											listmultiple[lm][i].SurchargeAmount = surchargeData[0].SurchargeAmount;
											listmultiple[lm][i].Description = surchargeData[0].Description.Length > 1 ? surchargeData[0].Description : "";
										}
									}
									item.CustomerPayLiftLoweredMainC = custPayMainCOrderH;
									item.CustomerPayLiftLoweredSubC = custPaySubCOrderH;
									item.CustomerMainC = custMainCOrderH;
									item.CustomerSubC = custSubCOrderH;
									var customer =
										_customerRepository.Query(p => p.CustomerMainC == custMainCOrderH && p.CustomerSubC == custSubCOrderH)
											.FirstOrDefault();
									if (customer != null)
									{
										item.CustomerN = customer.CustomerN;
										item.CustomerAddress = customer.Address1;
										item.CustomerTaxCode = customer.TaxCode;
									}

									var customerPay =
										_customerRepository.Query(p => p.CustomerMainC == custPayMainCOrderH && p.CustomerSubC == custPaySubCOrderH)
											.FirstOrDefault();
									if (customerPay != null)
									{
										item.CustomerPayLiftLoweredN = customerPay.CustomerN;
										item.CustomerPayLiftLoweredAddress = customerPay.Address1;
									}

									for (var loop = 0; loop < listmultiple[lm].Count; loop++)
									{
										var finalList = new List<CustomerExpenseItem>();
										var compareEmpty = 0;
										var compareAdd = 0;
										decimal taxRate = (100*(listmultiple[lm][loop].TaxAmount))/
										                  ((listmultiple[lm][loop].OrderD.TotalAmount != 0 &&
										                    listmultiple[lm][loop].OrderD.TotalAmount != null
											                  ? ((decimal) listmultiple[lm][loop].OrderD.TotalAmount -
											                     (listmultiple[lm][loop].OrderD.TotalExpense != 0 &&
											                      listmultiple[lm][loop].OrderD.TotalExpense != null
												                     ? (decimal) listmultiple[lm][loop].OrderD.TotalExpense
												                     : 0))
											                  : 0) > 0
											                  ? ((decimal) listmultiple[lm][loop].OrderD.TotalAmount -
											                     (decimal) listmultiple[lm][loop].OrderD.TotalExpense)
											                  : 1);
										for (var subloop = 0; subloop < listmultiple[lm].Count; subloop++)
										{
											decimal subTaxRate = (100*(listmultiple[lm][subloop].TaxAmount))/
											                     ((listmultiple[lm][subloop].OrderD.TotalAmount != 0 &&
											                       listmultiple[lm][subloop].OrderD.TotalAmount != null
												                     ? ((decimal) listmultiple[lm][subloop].OrderD.TotalAmount -
												                        (listmultiple[lm][subloop].OrderD.TotalExpense != 0 &&
												                         listmultiple[lm][subloop].OrderD.TotalExpense != null
													                        ? (decimal) listmultiple[lm][subloop].OrderD.TotalExpense
													                        : 0))
												                     : 0) > 0
												                     ? ((decimal) listmultiple[lm][subloop].OrderD.TotalAmount -
												                        (decimal) listmultiple[lm][subloop].OrderD.TotalExpense)
												                     : 1);
											if (taxRate == subTaxRate)
											{
												finalList.Add(listmultiple[lm][subloop]);
												listmultiple[lm].RemoveAt(subloop);
												subloop--;
												compareAdd++;
											}
											else
											{
												compareEmpty++;
											}
										}
										if (compareEmpty > 0 && compareAdd == 0)
										{
											finalList.Add(listmultiple[lm][loop]);
										}
										loop--;
										//item.CustomerExpenseList = finalList;
										var listAddGeneral = new CustomerExpenseReportData();
										listAddGeneral.CustomerMainC = custMainCOrderH;
										listAddGeneral.CustomerSubC = custSubCOrderH;
										var customer2 =
											_customerRepository.Query(p => p.CustomerMainC == custMainCOrderH && p.CustomerSubC == custSubCOrderH)
												.FirstOrDefault();
										if (customer2 != null)
										{
											listAddGeneral.CustomerN = customer2.CustomerN;
											listAddGeneral.CustomerAddress = customer2.Address1;
											listAddGeneral.CustomerTaxCode = customer2.TaxCode;
										}

										listAddGeneral.CustomerPayLiftLoweredMainC = custPayMainCOrderH;
										listAddGeneral.CustomerPayLiftLoweredSubC = custPaySubCOrderH;
										var customer3 =
											_customerRepository.Query(p => p.CustomerMainC == custPayMainCOrderH && p.CustomerSubC == custPaySubCOrderH)
												.FirstOrDefault();
										if (customer3 != null)
										{
											listAddGeneral.CustomerPayLiftLoweredN = customer3.CustomerN;
											listAddGeneral.CustomerPayLiftLoweredAddress = customer3.Address1;
										}

										listAddGeneral.CustomerExpenseList = finalList;
										listAddGeneral.TaxRate = Math.Round((100*(decimal) listAddGeneral.CustomerExpenseList[0].TaxAmount)/
										                                    ((listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount != null &&
										                                      listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount != 0
											                                    ? ((decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount -
											                                       (listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense != 0 &&
											                                        listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense != null
												                                       ? (decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense
												                                       : 0))
											                                    : 0) > 0
											                                    ? ((decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount -
											                                       (decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense)
											                                    : 1));
										listAddGeneral.TaxRate = listAddGeneral.TaxRate > 100 ? 0 : listAddGeneral.TaxRate;
										dataList.Add(listAddGeneral);
									}
									//dataList.Add(item);
								}
							}
						}
					}
				}
				if (param.ReportType == 4 || param.ReportType == 5)
				{
					for (var kloop = 0; kloop < dataList.Count; kloop++)
					{
						var custMC = dataList[kloop].CustomerMainC;
						var custSC = dataList[kloop].CustomerSubC;
						var custpayMC = dataList[kloop].CustomerPayLiftLoweredMainC;
						var custpaySC = dataList[kloop].CustomerPayLiftLoweredSubC;
						decimal taxR = dataList[kloop].TaxRate;
						for (var ploop = 0; ploop < dataList.Count; ploop++)
						{
							if (kloop != ploop)
							{
								if (dataList[ploop].CustomerMainC == custMC && dataList[ploop].CustomerSubC == custSC &&
									dataList[ploop].CustomerPayLiftLoweredMainC == custpayMC && dataList[ploop].CustomerPayLiftLoweredSubC == custpaySC &&
									dataList[ploop].TaxRate == taxR)
								{
									dataList[kloop].CustomerExpenseList.AddRange(dataList[ploop].CustomerExpenseList);
									dataList.RemoveAt(ploop);
									kloop--;
									ploop--;
								}
							}
						}
					}
				}
				if (param.ReportType == 0 || param.ReportType == 2 || param.ReportType == 3)
				{
					for (var kloop = 0; kloop < dataList.Count; kloop++)
					{
						var custMC = dataList[kloop].CustomerMainC;
						var custSC = dataList[kloop].CustomerSubC;
						decimal taxR = dataList[kloop].TaxRate;
						for (var ploop = 0; ploop < dataList.Count; ploop++)
						{
							if (kloop != ploop)
							{
								if (dataList[ploop].CustomerMainC == custMC && dataList[ploop].CustomerSubC == custSC &&
									dataList[ploop].TaxRate == taxR)
								{
									dataList[kloop].CustomerExpenseList.AddRange(dataList[ploop].CustomerExpenseList);
									dataList.RemoveAt(ploop);
									kloop--;
									ploop--;
								}
							}
						}
					}
				}

				// update invoice status
				if (dataList.Count > 0)
				{
					for (var iloop = 0; iloop < dataList.Count; iloop++)
					{
						if (dataList[iloop].CustomerExpenseList != null && dataList[iloop].CustomerExpenseList.Count > 0)
						{
							var itemResult = new CustomerExpenseReportData();

							itemResult.CustomerMainC = dataList[iloop].CustomerMainC;
							itemResult.CustomerSubC = dataList[iloop].CustomerSubC;
							itemResult.CustomerN = dataList[iloop].CustomerN;
							itemResult.CustomerAddress = dataList[iloop].CustomerAddress;

							itemResult.CustomerPayLiftLoweredMainC = dataList[iloop].CustomerPayLiftLoweredMainC;
							itemResult.CustomerPayLiftLoweredSubC = dataList[iloop].CustomerPayLiftLoweredSubC;
							itemResult.CustomerPayLiftLoweredN = dataList[iloop].CustomerPayLiftLoweredN;
							itemResult.CustomerPayLiftLoweredAddress = dataList[iloop].CustomerPayLiftLoweredAddress;

							itemResult.CustomerTaxCode = dataList[iloop].CustomerTaxCode;
							itemResult.TaxRate = dataList[iloop].TaxRate;
							//itemResult.ApplyD = dataList[iloop].ApplyD;
							//itemResult.SettlementD = dataList[iloop].SettlementD;
							//itemResult.TaxMethodI = dataList[iloop].TaxMethodI;
							//itemResult.TaxRate = dataList[iloop].TaxRate;
							//itemResult.TaxRoundingI = dataList[iloop].TaxRoundingI;
							//itemResult.RevenueRoundingI = dataList[iloop].RevenueRoundingI;
							itemResult.CustomerExpenseList = new List<CustomerExpenseItem>();

							for (var jloop = 0; jloop < dataList[iloop].CustomerExpenseList.Count; jloop++)
							{
								var orderD = dataList[iloop].CustomerExpenseList[jloop].OrderD.OrderD;
								var orderNo = dataList[iloop].CustomerExpenseList[jloop].OrderD.OrderNo;
								var detailNo = dataList[iloop].CustomerExpenseList[jloop].OrderD.DetailNo;

								var dispatchCount = _dispatchRepository.Query(dis => dis.OrderD == orderD &
																					 dis.OrderNo == orderNo &
																					 dis.DetailNo == detailNo
									).ToList();

								if (dispatchCount.Count > 0)
								{
									int countflag = 0;
									string truckC = "";
									if (dataList[iloop].CustomerExpenseList[jloop].OrderH.OrderTypeI == "0")
									{
										for (int i = 0; i < dispatchCount.Count; i++)
										{
											if (dispatchCount[i].ContainerStatus == "3")
											{

												truckC = dispatchCount[i].TruckC ?? "";
												countflag++;
												break;
											}
										}
										if (countflag < 1)
										{
											var selectLiftLowered = dispatchCount.Where(p => p.ContainerStatus == "4").FirstOrDefault();
											if (selectLiftLowered != null)
											{
												truckC = selectLiftLowered.TruckC ?? "";
											}
										}
									}
									else
									{
										for (int i = 0; i < dispatchCount.Count; i++)
										{
											if (dispatchCount[i].ContainerStatus == "2")
											{
												truckC = dispatchCount[i].TruckC ?? "";
												countflag++;
												break;
											}
										}
										if (countflag < 1)
										{
											var selectLiftLowered = dispatchCount.Where(p => p.ContainerStatus == "4").FirstOrDefault();
											if (selectLiftLowered != null)
											{
												truckC = selectLiftLowered.TruckC ?? "";
											}
										}
									}
									var truck = _truckRepository.Query(p => p.TruckC == truckC).FirstOrDefault();
									dataList[iloop].CustomerExpenseList[jloop].OrderD.RegisteredNo = truck != null ? truck.RegisteredNo : "";
									//itemResult.CustomerExpenseList.Add(dataList[iloop].CustomerExpenseList[jloop]);

									// transfer invoice status
									//for (var kloop = 0; kloop < dispatchCount.Count; kloop++)
									//{
									//	var dispatchUpdate = dispatchCount[kloop];
									//	dispatchUpdate.InvoiceStatus = "1";
									//	_dispatchRepository.Update(dispatchUpdate);
									//}
								}
								if (dataList[iloop].CustomerExpenseList[jloop].ExpenseD.ExpenseC != null &&
									dataList[iloop].CustomerExpenseList[jloop].ExpenseD.Description != null &&
									dataList[iloop].CustomerExpenseList[jloop].ExpenseD.Amount != null)
								{
									string exC = dataList[iloop].CustomerExpenseList[jloop].ExpenseD.ExpenseC;
									string desc = dataList[iloop].CustomerExpenseList[jloop].ExpenseD.Description;
									decimal amount = (decimal)dataList[iloop].CustomerExpenseList[jloop].ExpenseD.Amount;
									for (var k = jloop + 1; k < dataList[iloop].CustomerExpenseList.Count; k++)
									{
										if (exC == dataList[iloop].CustomerExpenseList[k].ExpenseD.ExpenseC &&
											desc == dataList[iloop].CustomerExpenseList[k].ExpenseD.Description &&
											amount == dataList[iloop].CustomerExpenseList[k].ExpenseD.Amount)
										{
											dataList[iloop].CustomerExpenseList.RemoveAt(k);
											k--;
											jloop--;
										}
									}
								}
							}
							for (var jloop = 0; jloop < dataList[iloop].CustomerExpenseList.Count; jloop++)
							{
								itemResult.CustomerExpenseList.Add(dataList[iloop].CustomerExpenseList[jloop]);
							}
							result.Add(itemResult);
						}
					}
					SaveReport();
				}
				if (result.Count > 0)
				{
					for (int r = 0; r < result.Count; r++)
					{
						listorder = result[r].CustomerExpenseList.OrderBy(p => p.TransportD).ToList();
						result[r].CustomerExpenseList = listorder;
						//get name for invoice customer
						string custM = result[r].CustomerMainC;
						string custS = result[r].CustomerSubC;
						var getCustomerShortN =
							_customerRepository.Query(p => p.CustomerMainC == custM && p.CustomerSubC == custS).FirstOrDefault();
						if (getCustomerShortN != null)
						{
							result[r].CustomerShortN = getCustomerShortN.CustomerShortN;
							result[r].InvoiceMainC = getCustomerShortN.InvoiceMainC ?? "";
							result[r].InvoiceSubC = getCustomerShortN.InvoiceSubC ?? "";
							string invoiceM = result[r].InvoiceMainC;
							string invoiceS = result[r].InvoiceSubC;
							var getInvoiceShortN =
								_customerRepository.Query(p => p.CustomerMainC == invoiceM && p.CustomerSubC == invoiceS).FirstOrDefault();
							if (getInvoiceShortN != null)
							{
								result[r].InvoiceN = getInvoiceShortN.CustomerN;
							}
						}
					}
				}
			}
			return result;
		}

		public List<CustomerExpenseReportData> GetCustomerExpenseReportList(CustomerExpenseReportParam param)
		{
			var dataList = new List<CustomerExpenseReportData>();
			var result = new List<CustomerExpenseReportData>();
			List<CustomerExpenseItem> listorder = new List<CustomerExpenseItem>();
			string sub = string.Empty;
			if (param.Customer == "null")
			{
				var customerList = _customerService.GetInvoices();

				if (customerList != null && customerList.Count > 0)
				{
					param.Customer = "";
					for (var iloop = 0; iloop < customerList.Count; iloop++)
					{
						string custMainC = customerList[iloop].CustomerMainC;
						string custSubC = customerList[iloop].CustomerSubC;
						var settlement0 = _customerSettlementRepository.Query(p => p.CustomerMainC == custMainC && p.CustomerSubC == custSubC && p.FormOfSettlement == Constants.NORMAL).OrderByDescending(p => p.ApplyD).FirstOrDefault();
						var settlement1 = _customerSettlementRepository.Query(p => p.CustomerMainC == custMainC && p.CustomerSubC == custSubC && p.FormOfSettlement == Constants.SETTLEMENT).OrderByDescending(p => p.ApplyD).FirstOrDefault();
						if (param.ReportI == "A")
						{
							if (settlement0 == null)
							{
								param.Customer = param.Customer + "," + customerList[iloop].CustomerMainC + "_" +
								                 customerList[iloop].CustomerSubC;
							}
						}
						if (param.ReportI == "N")
						{
							if (settlement1 == null)
							{
								param.Customer = param.Customer + "," + customerList[iloop].CustomerMainC + "_" +
								                 customerList[iloop].CustomerSubC;
							}
						}
					}
					sub = param.Customer.Substring(1);
					param.Customer = sub;
				}
			}
			// get data
			if (param.Customer != "null")
			{
				var customerArr = (param.Customer).Split(new string[] { "," }, StringSplitOptions.None);
				string customertemp = "";
				for (var loop = 0; loop < customerArr.Length; loop++)
				{
					var arr1 = (customerArr[loop]).Split(new string[] { "_" }, StringSplitOptions.None);
					var invoiceMainC = arr1[0];
					var invoiceSubC = arr1[1];
					var cus = _customerRepository.Query(p => p.InvoiceMainC == invoiceMainC && p.InvoiceSubC == invoiceSubC).ToList();
					if (cus.Count > 0)
					{
						for (var l = 0; l < cus.Count; l++)
						{
							customertemp = customertemp + "," + cus[l].CustomerMainC + "_" + cus[l].CustomerSubC;
						}
					}
				}
				var customertempArr = (customertemp.Substring(1)).Split(new string[] { "," }, StringSplitOptions.None);
				customertempArr.Distinct();
				customerArr = customertempArr;
				for (var iloop = 0; iloop < customerArr.Length; iloop++)
				{
					var item = new CustomerExpenseReportData();
					var arr = (customerArr[iloop]).Split(new string[] { "_" }, StringSplitOptions.None);
					var customerMainC = arr[0];
					var customerSubC = arr[1];
					var settlement0 = _customerSettlementRepository.Query(p => p.CustomerMainC == customerMainC && p.CustomerSubC == customerSubC && p.FormOfSettlement == "0").OrderByDescending(p => p.ApplyD).FirstOrDefault();
					var settlement1 = _customerSettlementRepository.Query(p => p.CustomerMainC == customerMainC && p.CustomerSubC == customerSubC && p.FormOfSettlement == "1").OrderByDescending(p => p.ApplyD).FirstOrDefault();
					if ((param.ReportI == "A" && settlement1 != null && settlement0 == null) ||
					    (param.ReportI == "N" && settlement0 != null && settlement1 == null) ||
					    ((param.ReportI == "A" || param.ReportI == "N") &&
					     ((settlement1 == null && settlement0 == null) || (settlement1 != null && settlement0 != null))))
					{
						DateTime startDate;
						DateTime endDate;
						if (param.ReportI.Equals("A"))
						{
							// get month and year transport
							var month = param.TransportM.Value.Month;
							var year = param.TransportM.Value.Year;
							var invoiceInfo = _invoiceInfoService.GetLimitStartAndEndInvoiceD(customerMainC, customerSubC, month, year);
							startDate = invoiceInfo.StartDate.Date;
							endDate = invoiceInfo.EndDate.Date;
						}
						else
						{
							startDate = (DateTime) param.OrderDFrom;
							endDate = (DateTime) param.OrderDTo;
						}
						// get customers who shared a invoice company
						var customerStr = "";
						var customerList = _customerService.GetCustomersByInvoice(customerMainC, customerSubC);
						for (var aloop = 0; aloop < customerList.Count; aloop++)
						{
							customerStr = customerStr + "," + customerList[aloop].CustomerMainC + "_" + customerList[aloop].CustomerSubC;
						}

						// get and calculate detain date
						var dispatch = from a in _dispatchRepository.GetAllQueryable()
							join b in _orderDRepository.GetAllQueryable() on new {a.OrderD, a.OrderNo, a.DetailNo}
								equals new {b.OrderD, b.OrderNo, b.DetailNo} into t1
							from b in t1.DefaultIfEmpty()
							where ((param.InvoiceStatus == "-1" ||
							        (param.InvoiceStatus == "0" && a.InvoiceStatus != "1") ||
							        (param.InvoiceStatus == "1" && a.InvoiceStatus == "1")
								) &
							       (b.RevenueD >= startDate & b.RevenueD <= endDate))
							select new DispatchViewModel()
							{
								OrderD = a.OrderD,
								OrderNo = a.OrderNo,
								DetailNo = a.DetailNo,
								DispatchNo = a.DispatchNo,
								DetainDay = a.DetainDay,
								TruckC = a.TruckC,
								TransportD = a.TransportD
							};
						dispatch = dispatch.OrderBy("OrderD asc, OrderNo asc, DetailNo asc, DispatchNo asc");

						var groupDispatch = from b in dispatch
							group b by new {b.OrderD, b.OrderNo, b.DetailNo, b.TransportD}
							into c
							select new
							{
								c.Key.OrderD,
								c.Key.OrderNo,
								c.Key.DetailNo,
								c.Key.TransportD,
								DetainDay = c.Sum(b => b.DetainDay)
							};

						//get Truck No
						var truck = from b in dispatch
							join t in _truckRepository.GetAllQueryable()
								on b.TruckC equals t.TruckC
							where (b.DispatchNo == 1)
							select new
							{
								OrderD = b.OrderD,
								OrderNo = b.OrderNo,
								DetailNo = b.DetailNo,
								TruckNo = t.RegisteredNo
							};

						// get surcharge
						var surcharge = from a in _surchargeDetailRepository.GetAllQueryable()
							join b in groupDispatch on new {a.OrderD, a.OrderNo, a.DetailNo}
								equals new {b.OrderD, b.OrderNo, b.DetailNo} into t1
							where a.DispatchNo == 0
							select new SurchargeDetailViewModel()
							{
								OrderD = a.OrderD,
								OrderNo = a.OrderNo,
								DetailNo = a.DetailNo,
								Amount = a.Amount,
								Description = a.Description
							};
						var groupSurcharge = (from b in surcharge.AsEnumerable()
							group b by new {b.OrderD, b.OrderNo, b.DetailNo}
							into c
							select new
							{
								OrderD = c.Key.OrderD,
								OrderNo = c.Key.OrderNo,
								DetailNo = c.Key.DetailNo,
								Amount = c.Sum(b => b.Amount),
								Description = String.Join(",", c.Select(b => b.Description))
							}).ToList();
						//var temp = groupSurcharge.ToList();
						// get data
						var data = from d in _orderDRepository.GetAllQueryable()
							join e in _orderHRepository.GetAllQueryable() on new {d.OrderD, d.OrderNo}
								equals new {e.OrderD, e.OrderNo} into t1
							from e in t1.DefaultIfEmpty()
							join m in _truckRepository.GetAllQueryable() on d.TruckCLastDispatch
								equals m.TruckC into t9
							from m in t9.DefaultIfEmpty()
							//join c in _customerRepository.GetAllQueryable() on new { e.CustomerMainC, e.CustomerSubC }
							//	equals new { c.CustomerMainC, c.CustomerSubC } into t6
							//from c in t6.DefaultIfEmpty()
							join f in _departmentRepository.GetAllQueryable() on e.OrderDepC
								equals f.DepC into t2
							from f in t2.DefaultIfEmpty()
							join g in groupDispatch on new {d.OrderD, d.OrderNo, d.DetailNo}
								equals new {g.OrderD, g.OrderNo, g.DetailNo}
							join h in _expenseDetailRepository.Query(h => h.IsRequested == "1" && h.Amount > 0) on
								new {d.OrderD, d.OrderNo, d.DetailNo}
								equals new {h.OrderD, h.OrderNo, h.DetailNo} into t4
							from h in t4.DefaultIfEmpty()
							join k in _expenseRepository.GetAllQueryable() on h.ExpenseC
								equals k.ExpenseC into t5
							from k in t5.DefaultIfEmpty()
							//join s in groupSurcharge on new { d.OrderD, d.OrderNo, d.DetailNo }
							// equals new { s.OrderD, s.OrderNo, s.DetailNo } into t7
							//from s in t7.DefaultIfEmpty()
							join t in truck on new {d.OrderD, d.OrderNo, d.DetailNo}
								equals new {t.OrderD, t.OrderNo, t.DetailNo} into t8
							from t in t8.DefaultIfEmpty()
							where ((customerStr.Contains("," + e.CustomerMainC + "_" + e.CustomerSubC)) &
							       (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || e.OrderDepC == param.DepC) &
							       (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || e.OrderTypeI == param.OrderTypeI) &
							       (param.BLBK.Equals("null") || e.BLBK.Equals(param.BLBK)) &
							       (param.JobNo.Equals("null") || e.JobNo.Contains(param.JobNo))
								)
							select new CustomerExpenseItem()
							{
								OrderD = new ContainerViewModel()
								{
									OrderD = d.OrderD,
									OrderNo = d.OrderNo,
									DetailNo = d.DetailNo,
									ContainerNo = d.ContainerNo,
									ContainerSizeI = d.ContainerSizeI,
									Amount = d.Amount,
									ActualLoadingD = d.ActualLoadingD,
									ActualDischargeD = d.ActualDischargeD,
									NetWeight = d.NetWeight,
									RegisteredNo = m.RegisteredNo ?? "",
									LocationDispatch1 = d.LocationDispatch1 ?? "",
									LocationDispatch2 = d.LocationDispatch2 ?? "",
									LocationDispatch3 = d.LocationDispatch3 ?? "",
									CustomerSurcharge = d.CustomerSurcharge,
									TotalAmount = d.TotalAmount,
									TotalExpense = d.TotalExpense
								},
								OrderH = new OrderViewModel()
								{
									OrderD = e.OrderD,
									OrderNo = e.OrderNo,
									//CustomerMainC = e.CustomerMainC,
									//CustomerSubC = e.CustomerSubC,
									//CustomerN = c != null ? c.CustomerN : "",
									OrderTypeI = e.OrderTypeI,
									OrderDepN = f != null ? f.DepN : "",
									LoadingPlaceN = e.LoadingPlaceN,
									StopoverPlaceN = e.StopoverPlaceN,
									DischargePlaceN = e.DischargePlaceN,
									BLBK = e.BLBK,
									IsCollected = e.IsCollected,
									JobNo = e.JobNo,
									CustomerMainC = e.CustomerMainC,
									CustomerSubC = e.CustomerSubC,
									CustomerPayLiftLoweredMainC = e.CustomerPayLiftLoweredMainC,
									CustomerPayLiftLoweredSubC = e.CustomerPayLiftLoweredSubC
								},
								DetainDay = g.DetainDay,
								TransportD = g.TransportD,
								//SurchargeAmount = s.Amount != null ? (decimal)s.Amount : 0,
								//Description = s.Description,
								DetainAmount = d.DetainAmount ?? 0,
								TaxAmount = d.TaxAmount ?? 0,
								ExpenseD = new ExpenseDetailViewModel()
								{
									ExpenseC = h != null ? h.ExpenseC : "",
									ExpenseN = k != null ? k.ExpenseN : "",
									Description = h != null ? h.Description : "",
									Amount = h != null ? h.Amount : null,
									TaxAmount = h != null ? h.TaxAmount : null,
									TaxRate = h != null ? h.TaxRate : null,
									ExpenseI = k.ExpenseI,
									IsIncluded = h.IsIncluded,
									IsRequested = h.IsRequested
								},
								TruckNo = t.TruckNo,
								//CustomerAddress = c.Address1,
								//CustomerTaxCode = c.TaxCode
							};
						data = data.OrderBy("OrderD.OrderD asc, OrderD.OrderNo asc, OrderD.DetailNo asc");
						var listmultiple = data
							.GroupBy(u => u.OrderH.CustomerPayLiftLoweredMainC)
							.Select(grp => grp.ToList())
							.ToList();
						var list = data.ToList();
						if (param.ReportType == 0 || param.ReportType == 2 || param.ReportType == 3 ||
						    ((param.ReportType == 4 || param.ReportType == 5) && listmultiple.Count < 2))
						{
							string custMainCOrderH = "";
							string custSubCOrderH = "";
							string custPayMainCOrderH = "";
							string custPaySubCOrderH = "";
							//add surcharge
							if (list != null && list.Count > 0)
							{
								for (int i = 0; i < list.Count; i++)
								{
									custPayMainCOrderH = list[i].OrderH.CustomerPayLiftLoweredMainC;
									custPaySubCOrderH = list[i].OrderH.CustomerPayLiftLoweredSubC;

									custMainCOrderH = list[i].OrderH.CustomerMainC;
									custSubCOrderH = list[i].OrderH.CustomerSubC;
									var orderD = list[i].OrderH.OrderD;
									var orderNo = list[i].OrderH.OrderNo;
									var detailNo = list[i].OrderD.DetailNo;
									var surchargeData = (from s in groupSurcharge
										where s.OrderD == orderD &&
										      s.OrderNo == orderNo &&
										      s.DetailNo == detailNo
										select new CustomerExpenseItem()
										{
											SurchargeAmount = s.Amount ?? 0,
											Description = s.Description
										}).ToList();
									if (surchargeData != null && surchargeData.Count > 0)
									{
										list[i].SurchargeAmount = surchargeData[0].SurchargeAmount;
										list[i].Description = surchargeData[0].Description.Length > 1 ? surchargeData[0].Description : "";
									}
								}
								item.CustomerPayLiftLoweredMainC = custPayMainCOrderH;
								item.CustomerPayLiftLoweredSubC = custPaySubCOrderH;
								item.CustomerMainC = custMainCOrderH;
								item.CustomerSubC = custSubCOrderH;
								var customer =
									_customerRepository.Query(p => p.CustomerMainC == custMainCOrderH && p.CustomerSubC == custSubCOrderH)
										.FirstOrDefault();
								if (customer != null)
								{
									item.CustomerN = customer.CustomerN;
									item.CustomerAddress = customer.Address1;
									item.CustomerTaxCode = customer.TaxCode;
								}

								var customerPay =
									_customerRepository.Query(p => p.CustomerMainC == custPayMainCOrderH && p.CustomerSubC == custPaySubCOrderH)
										.FirstOrDefault();
								if (customerPay != null)
								{
									item.CustomerPayLiftLoweredN = customerPay.CustomerN;
									item.CustomerPayLiftLoweredAddress = customerPay.Address1;
								}

								for (var loop = 0; loop < list.Count; loop++)
								{
									var finalList = new List<CustomerExpenseItem>();
									var compareEmpty = 0;
									var compareAdd = 0;
									decimal taxRate = (100*(list[loop].TaxAmount))/
									                  ((list[loop].OrderD.TotalAmount != 0 && list[loop].OrderD.TotalAmount != null
										                  ? ((decimal) list[loop].OrderD.TotalAmount -
										                     (list[loop].OrderD.TotalExpense != 0 && list[loop].OrderD.TotalExpense != null
											                     ? (decimal) list[loop].OrderD.TotalExpense
											                     : 0))
										                  : 0) > 0
										                  ? ((decimal) list[loop].OrderD.TotalAmount - (decimal) list[loop].OrderD.TotalExpense)
										                  : 1);
									for (var subloop = 0; subloop < list.Count; subloop++)
									{
										decimal subTaxRate = (100*(list[subloop].TaxAmount))/
										                     ((list[subloop].OrderD.TotalAmount != 0 && list[subloop].OrderD.TotalAmount != null
											                     ? ((decimal) list[subloop].OrderD.TotalAmount -
											                        (list[subloop].OrderD.TotalExpense != 0 && list[subloop].OrderD.TotalExpense != null
												                        ? (decimal) list[subloop].OrderD.TotalExpense
												                        : 0))
											                     : 0) > 0
											                     ? ((decimal) list[subloop].OrderD.TotalAmount -
											                        (decimal) list[subloop].OrderD.TotalExpense)
											                     : 1);
										if (taxRate == subTaxRate)
										{
											finalList.Add(list[subloop]);
											list.RemoveAt(subloop);
											subloop--;
											compareAdd++;
										}
										else
										{
											compareEmpty++;
										}
									}
									if (compareEmpty > 0 && compareAdd == 0)
									{
										finalList.Add(list[loop]);
									}
									loop--;
									//item.CustomerExpenseList = finalList;
									var listAddGeneral = new CustomerExpenseReportData();
									listAddGeneral.CustomerMainC = custMainCOrderH;
									listAddGeneral.CustomerSubC = custSubCOrderH;
									var customer2 =
										_customerRepository.Query(p => p.CustomerMainC == custMainCOrderH && p.CustomerSubC == custSubCOrderH)
											.FirstOrDefault();
									if (customer2 != null)
									{
										listAddGeneral.CustomerN = customer2.CustomerN;
										listAddGeneral.CustomerAddress = customer2.Address1;
										listAddGeneral.CustomerTaxCode = customer2.TaxCode;
									}

									listAddGeneral.CustomerPayLiftLoweredMainC = custPayMainCOrderH;
									listAddGeneral.CustomerPayLiftLoweredSubC = custPaySubCOrderH;
									var customer3 =
										_customerRepository.Query(p => p.CustomerMainC == custPayMainCOrderH && p.CustomerSubC == custPaySubCOrderH)
											.FirstOrDefault();
									if (customer3 != null)
									{
										listAddGeneral.CustomerPayLiftLoweredN = customer3.CustomerN;
										listAddGeneral.CustomerPayLiftLoweredAddress = customer3.Address1;
									}

									listAddGeneral.CustomerExpenseList = finalList;
									listAddGeneral.TaxRate = Math.Round((100*(decimal) listAddGeneral.CustomerExpenseList[0].TaxAmount)/
									                                    ((listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount != null &&
									                                      listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount != 0
										                                    ? ((decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount -
										                                       (listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense != 0 &&
										                                        listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense != null
											                                       ? (decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense
											                                       : 0))
										                                    : 0) > 0
										                                    ? ((decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount -
										                                       (decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense)
										                                    : 1));
									listAddGeneral.TaxRate = listAddGeneral.TaxRate > 100 ? 0 : listAddGeneral.TaxRate;
									dataList.Add(listAddGeneral);
								}
								//dataList.Add(item);
							}
						}
						if ((param.ReportType == 4 || param.ReportType == 5) && listmultiple.Count >= 2)
						{
							for (int lm = 0; lm < listmultiple.Count; lm++)
							{
								string custMainCOrderH = "";
								string custSubCOrderH = "";
								string custPayMainCOrderH = "";
								string custPaySubCOrderH = "";
								//add surcharge
								if (listmultiple[lm] != null && listmultiple[lm].Count > 0)
								{
									for (int i = 0; i < listmultiple[lm].Count; i++)
									{
										custPayMainCOrderH = listmultiple[lm][i].OrderH.CustomerPayLiftLoweredMainC;
										custPaySubCOrderH = listmultiple[lm][i].OrderH.CustomerPayLiftLoweredSubC;

										custMainCOrderH = listmultiple[lm][i].OrderH.CustomerMainC;
										custSubCOrderH = listmultiple[lm][i].OrderH.CustomerSubC;
										var orderD = listmultiple[lm][i].OrderH.OrderD;
										var orderNo = listmultiple[lm][i].OrderH.OrderNo;
										var detailNo = listmultiple[lm][i].OrderD.DetailNo;
										var surchargeData = (from s in groupSurcharge
											where s.OrderD == orderD &&
											      s.OrderNo == orderNo &&
											      s.DetailNo == detailNo
											select new CustomerExpenseItem()
											{
												SurchargeAmount = s.Amount ?? 0,
												Description = s.Description
											}).ToList();
										if (surchargeData != null && surchargeData.Count > 0)
										{
											listmultiple[lm][i].SurchargeAmount = surchargeData[0].SurchargeAmount;
											listmultiple[lm][i].Description = surchargeData[0].Description.Length > 1 ? surchargeData[0].Description : "";
										}
									}
									item.CustomerPayLiftLoweredMainC = custPayMainCOrderH;
									item.CustomerPayLiftLoweredSubC = custPaySubCOrderH;
									item.CustomerMainC = custMainCOrderH;
									item.CustomerSubC = custSubCOrderH;
									var customer =
										_customerRepository.Query(p => p.CustomerMainC == custMainCOrderH && p.CustomerSubC == custSubCOrderH)
											.FirstOrDefault();
									if (customer != null)
									{
										item.CustomerN = customer.CustomerN;
										item.CustomerAddress = customer.Address1;
										item.CustomerTaxCode = customer.TaxCode;
									}

									var customerPay =
										_customerRepository.Query(p => p.CustomerMainC == custPayMainCOrderH && p.CustomerSubC == custPaySubCOrderH)
											.FirstOrDefault();
									if (customerPay != null)
									{
										item.CustomerPayLiftLoweredN = customerPay.CustomerN;
										item.CustomerPayLiftLoweredAddress = customerPay.Address1;
									}

									for (var loop = 0; loop < listmultiple[lm].Count; loop++)
									{
										var finalList = new List<CustomerExpenseItem>();
										var compareEmpty = 0;
										var compareAdd = 0;
										decimal taxRate = (100*(listmultiple[lm][loop].TaxAmount))/
										                  ((listmultiple[lm][loop].OrderD.TotalAmount != 0 &&
										                    listmultiple[lm][loop].OrderD.TotalAmount != null
											                  ? ((decimal) listmultiple[lm][loop].OrderD.TotalAmount -
											                     (listmultiple[lm][loop].OrderD.TotalExpense != 0 &&
											                      listmultiple[lm][loop].OrderD.TotalExpense != null
												                     ? (decimal) listmultiple[lm][loop].OrderD.TotalExpense
												                     : 0))
											                  : 0) > 0
											                  ? ((decimal) listmultiple[lm][loop].OrderD.TotalAmount -
											                     (decimal) listmultiple[lm][loop].OrderD.TotalExpense)
											                  : 1);
										for (var subloop = 0; subloop < listmultiple[lm].Count; subloop++)
										{
											decimal subTaxRate = (100*(listmultiple[lm][subloop].TaxAmount))/
											                     ((listmultiple[lm][subloop].OrderD.TotalAmount != 0 &&
											                       listmultiple[lm][subloop].OrderD.TotalAmount != null
												                     ? ((decimal) listmultiple[lm][subloop].OrderD.TotalAmount -
												                        (listmultiple[lm][subloop].OrderD.TotalExpense != 0 &&
												                         listmultiple[lm][subloop].OrderD.TotalExpense != null
													                        ? (decimal) listmultiple[lm][subloop].OrderD.TotalExpense
													                        : 0))
												                     : 0) > 0
												                     ? ((decimal) listmultiple[lm][subloop].OrderD.TotalAmount -
												                        (decimal) listmultiple[lm][subloop].OrderD.TotalExpense)
												                     : 1);
											if (taxRate == subTaxRate)
											{
												finalList.Add(listmultiple[lm][subloop]);
												listmultiple[lm].RemoveAt(subloop);
												subloop--;
												compareAdd++;
											}
											else
											{
												compareEmpty++;
											}
										}
										if (compareEmpty > 0 && compareAdd == 0)
										{
											finalList.Add(listmultiple[lm][loop]);
										}
										loop--;
										//item.CustomerExpenseList = finalList;
										var listAddGeneral = new CustomerExpenseReportData();
										listAddGeneral.CustomerMainC = custMainCOrderH;
										listAddGeneral.CustomerSubC = custSubCOrderH;
										var customer2 =
											_customerRepository.Query(p => p.CustomerMainC == custMainCOrderH && p.CustomerSubC == custSubCOrderH)
												.FirstOrDefault();
										if (customer2 != null)
										{
											listAddGeneral.CustomerN = customer2.CustomerN;
											listAddGeneral.CustomerAddress = customer2.Address1;
											listAddGeneral.CustomerTaxCode = customer2.TaxCode;
										}

										listAddGeneral.CustomerPayLiftLoweredMainC = custPayMainCOrderH;
										listAddGeneral.CustomerPayLiftLoweredSubC = custPaySubCOrderH;
										var customer3 =
											_customerRepository.Query(p => p.CustomerMainC == custPayMainCOrderH && p.CustomerSubC == custPaySubCOrderH)
												.FirstOrDefault();
										if (customer3 != null)
										{
											listAddGeneral.CustomerPayLiftLoweredN = customer3.CustomerN;
											listAddGeneral.CustomerPayLiftLoweredAddress = customer3.Address1;
										}

										listAddGeneral.CustomerExpenseList = finalList;
										listAddGeneral.TaxRate = Math.Round((100*(decimal) listAddGeneral.CustomerExpenseList[0].TaxAmount)/
										                                    ((listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount != null &&
										                                      listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount != 0
											                                    ? ((decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount -
											                                       (listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense != 0 &&
											                                        listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense != null
												                                       ? (decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense
												                                       : 0))
											                                    : 0) > 0
											                                    ? ((decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount -
											                                       (decimal) listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense)
											                                    : 1));
										listAddGeneral.TaxRate = listAddGeneral.TaxRate > 100 ? 0 : listAddGeneral.TaxRate;
										dataList.Add(listAddGeneral);
									}
									//dataList.Add(item);
								}
							}
						}
					}
				}
				if (param.ReportType == 4 || param.ReportType == 5)
				{
					for (var kloop = 0; kloop < dataList.Count; kloop++)
					{
						var custMC = dataList[kloop].CustomerMainC;
						var custSC = dataList[kloop].CustomerSubC;
						var custpayMC = dataList[kloop].CustomerPayLiftLoweredMainC;
						var custpaySC = dataList[kloop].CustomerPayLiftLoweredSubC;
						decimal taxR = dataList[kloop].TaxRate;
						for (var ploop = 0; ploop < dataList.Count; ploop++)
						{
							if (kloop != ploop)
							{
								if (dataList[ploop].CustomerMainC == custMC && dataList[ploop].CustomerSubC == custSC &&
									dataList[ploop].CustomerPayLiftLoweredMainC == custpayMC && dataList[ploop].CustomerPayLiftLoweredSubC == custpaySC &&
									dataList[ploop].TaxRate == taxR)
								{
									dataList[kloop].CustomerExpenseList.AddRange(dataList[ploop].CustomerExpenseList);
									dataList.RemoveAt(ploop);
									kloop--;
									ploop--;
								}
							}
						}
					}
				}
				if (param.ReportType == 0 || param.ReportType == 2 || param.ReportType == 3)
				{
					for (var kloop = 0; kloop < dataList.Count; kloop++)
					{
						var custMC = dataList[kloop].CustomerMainC;
						var custSC = dataList[kloop].CustomerSubC;
						decimal taxR = dataList[kloop].TaxRate;
						for (var ploop = 0; ploop < dataList.Count; ploop++)
						{
							if (kloop != ploop)
							{
								if (dataList[ploop].CustomerMainC == custMC && dataList[ploop].CustomerSubC == custSC &&
									dataList[ploop].TaxRate == taxR)
								{
									dataList[kloop].CustomerExpenseList.AddRange(dataList[ploop].CustomerExpenseList);
									dataList.RemoveAt(ploop);
									kloop--;
									ploop--;
								}
							}
						}
					}
				}
				
				// update invoice status
				if (dataList.Count > 0)
				{
					for (var iloop = 0; iloop < dataList.Count; iloop++)
					{
						if (dataList[iloop].CustomerExpenseList != null && dataList[iloop].CustomerExpenseList.Count > 0)
						{
							var itemResult = new CustomerExpenseReportData();

							itemResult.CustomerMainC = dataList[iloop].CustomerMainC;
							itemResult.CustomerSubC = dataList[iloop].CustomerSubC;
							itemResult.CustomerN = dataList[iloop].CustomerN;
							itemResult.CustomerAddress = dataList[iloop].CustomerAddress;

							itemResult.CustomerPayLiftLoweredMainC = dataList[iloop].CustomerPayLiftLoweredMainC;
							itemResult.CustomerPayLiftLoweredSubC = dataList[iloop].CustomerPayLiftLoweredSubC;
							itemResult.CustomerPayLiftLoweredN = dataList[iloop].CustomerPayLiftLoweredN;
							itemResult.CustomerPayLiftLoweredAddress = dataList[iloop].CustomerPayLiftLoweredAddress;

							itemResult.CustomerTaxCode = dataList[iloop].CustomerTaxCode;
							itemResult.TaxRate = dataList[iloop].TaxRate;
							//itemResult.ApplyD = dataList[iloop].ApplyD;
							//itemResult.SettlementD = dataList[iloop].SettlementD;
							//itemResult.TaxMethodI = dataList[iloop].TaxMethodI;
							//itemResult.TaxRate = dataList[iloop].TaxRate;
							//itemResult.TaxRoundingI = dataList[iloop].TaxRoundingI;
							//itemResult.RevenueRoundingI = dataList[iloop].RevenueRoundingI;
							itemResult.CustomerExpenseList = new List<CustomerExpenseItem>();

							for (var jloop = 0; jloop < dataList[iloop].CustomerExpenseList.Count; jloop++)
							{
								var orderD = dataList[iloop].CustomerExpenseList[jloop].OrderD.OrderD;
								var orderNo = dataList[iloop].CustomerExpenseList[jloop].OrderD.OrderNo;
								var detailNo = dataList[iloop].CustomerExpenseList[jloop].OrderD.DetailNo;

								var dispatchCount = _dispatchRepository.Query(dis => dis.OrderD == orderD &
																					 dis.OrderNo == orderNo &
																					 dis.DetailNo == detailNo
									).ToList();

								if (dispatchCount.Count > 0)
								{
									int countflag = 0;
									string truckC = "";
									if (dataList[iloop].CustomerExpenseList[jloop].OrderH.OrderTypeI == "0")
									{
										for (int i = 0; i < dispatchCount.Count; i++)
										{
											if (dispatchCount[i].ContainerStatus == "3")
											{

												truckC = dispatchCount[i].TruckC ?? "";
												countflag++;
												break;
											}
										}
										if (countflag < 1)
										{
											var selectLiftLowered = dispatchCount.Where(p => p.ContainerStatus == "4").FirstOrDefault();
											if (selectLiftLowered != null)
											{
												truckC = selectLiftLowered.TruckC ?? "";
											}
										}
									}
									else
									{
										for (int i = 0; i < dispatchCount.Count; i++)
										{
											if (dispatchCount[i].ContainerStatus == "2")
											{
												truckC = dispatchCount[i].TruckC ?? "";
												countflag++;
												break;
											}
										}
										if (countflag < 1)
										{
											var selectLiftLowered = dispatchCount.Where(p => p.ContainerStatus == "4").FirstOrDefault();
											if (selectLiftLowered != null)
											{
												truckC = selectLiftLowered.TruckC ?? "";
											}
										}
									}
									var truck = _truckRepository.Query(p => p.TruckC == truckC).FirstOrDefault();
									dataList[iloop].CustomerExpenseList[jloop].OrderD.RegisteredNo = truck != null ? truck.RegisteredNo : "";
									//itemResult.CustomerExpenseList.Add(dataList[iloop].CustomerExpenseList[jloop]);

									// transfer invoice status
									//for (var kloop = 0; kloop < dispatchCount.Count; kloop++)
									//{
									//	var dispatchUpdate = dispatchCount[kloop];
									//	dispatchUpdate.InvoiceStatus = "1";
									//	_dispatchRepository.Update(dispatchUpdate);
									//}
								}
								if (dataList[iloop].CustomerExpenseList[jloop].ExpenseD.ExpenseC != null &&
									dataList[iloop].CustomerExpenseList[jloop].ExpenseD.Description != null &&
									dataList[iloop].CustomerExpenseList[jloop].ExpenseD.Amount != null)
								{
									string exC = dataList[iloop].CustomerExpenseList[jloop].ExpenseD.ExpenseC;
									string desc = dataList[iloop].CustomerExpenseList[jloop].ExpenseD.Description;
									decimal amount = (decimal)dataList[iloop].CustomerExpenseList[jloop].ExpenseD.Amount;
									for (var k = jloop + 1; k < dataList[iloop].CustomerExpenseList.Count; k++)
									{
										if (exC == dataList[iloop].CustomerExpenseList[k].ExpenseD.ExpenseC &&
											desc == dataList[iloop].CustomerExpenseList[k].ExpenseD.Description &&
											amount == dataList[iloop].CustomerExpenseList[k].ExpenseD.Amount)
										{
											dataList[iloop].CustomerExpenseList.RemoveAt(k);
											k--;
											jloop--;
										}
									}
								}
							}
							for (var jloop = 0; jloop < dataList[iloop].CustomerExpenseList.Count; jloop++)
							{
								itemResult.CustomerExpenseList.Add(dataList[iloop].CustomerExpenseList[jloop]);
							}
							result.Add(itemResult);
						}
					}
					SaveReport();
				}
				if (result.Count > 0)
				{
					for (int r = 0; r < result.Count; r++)
					{
						listorder = result[r].CustomerExpenseList.OrderBy(p => p.TransportD).ToList();
						result[r].CustomerExpenseList = listorder;
						//get name for invoice customer
						string custM = result[r].CustomerMainC;
						string custS = result[r].CustomerSubC;
						var getCustomerShortN =
							_customerRepository.Query(p => p.CustomerMainC == custM && p.CustomerSubC == custS).FirstOrDefault();
						if (getCustomerShortN != null)
						{
							result[r].CustomerShortN = getCustomerShortN.CustomerShortN;
							result[r].InvoiceMainC = getCustomerShortN.InvoiceMainC ?? "";
							result[r].InvoiceSubC = getCustomerShortN.InvoiceSubC ?? "";
							string invoiceM = result[r].InvoiceMainC;
							string invoiceS = result[r].InvoiceSubC;
							var getInvoiceShortN =
								_customerRepository.Query(p => p.CustomerMainC == invoiceM && p.CustomerSubC == invoiceS).FirstOrDefault();
							if (getInvoiceShortN != null)
							{
								result[r].InvoiceN = getInvoiceShortN.CustomerN;
							}
						}
					}
				}
			}
			return result;
		}
		public int UpdateInvoiceStatus(CustomerExpenseReportParam param)
		{
			var dataList = new List<CustomerExpenseReportData>();
			string sub = string.Empty;
			int numberofupdate = 0;
			if (param.Customer == "null")
			{
				var customerList = _customerService.GetInvoices();

				if (customerList != null && customerList.Count > 0)
				{
					param.Customer = "";
					for (var iloop = 0; iloop < customerList.Count; iloop++)
					{
						string custMainC = customerList[iloop].CustomerMainC;
						string custSubC = customerList[iloop].CustomerSubC;
						var settlement0 = _customerSettlementRepository.Query(p => p.CustomerMainC == custMainC && p.CustomerSubC == custSubC && p.FormOfSettlement == Constants.NORMAL).OrderByDescending(p => p.ApplyD).FirstOrDefault();
						var settlement1 = _customerSettlementRepository.Query(p => p.CustomerMainC == custMainC && p.CustomerSubC == custSubC && p.FormOfSettlement == Constants.SETTLEMENT).OrderByDescending(p => p.ApplyD).FirstOrDefault();
						if (param.ReportI == "A")
						{
							if (settlement0 == null)
							{
								param.Customer = param.Customer + "," + customerList[iloop].CustomerMainC + "_" +
												 customerList[iloop].CustomerSubC;
							}
						}
						if (param.ReportI == "N")
						{
							if (settlement1 == null)
							{
								param.Customer = param.Customer + "," + customerList[iloop].CustomerMainC + "_" +
												 customerList[iloop].CustomerSubC;
							}
						}
					}
					sub = param.Customer.Substring(1);
					param.Customer = sub;
				}
			}
			// get data
			if (param.Customer != "null")
			{
				var customerArr = (param.Customer).Split(new string[] { "," }, StringSplitOptions.None);

				for (var iloop = 0; iloop < customerArr.Length; iloop++)
				{
					var item = new CustomerExpenseReportData();
					var arr = (customerArr[iloop]).Split(new string[] { "_" }, StringSplitOptions.None);
					var customerMainC = arr[0];
					var customerSubC = arr[1];
					var settlement0 = _customerSettlementRepository.Query(p => p.CustomerMainC == customerMainC && p.CustomerSubC == customerSubC && p.FormOfSettlement == "0").OrderByDescending(p => p.ApplyD).FirstOrDefault();
					var settlement1 = _customerSettlementRepository.Query(p => p.CustomerMainC == customerMainC && p.CustomerSubC == customerSubC && p.FormOfSettlement == "1").OrderByDescending(p => p.ApplyD).FirstOrDefault();
					if ((param.ReportI == "A" && settlement1 != null && settlement0 == null) ||
						(param.ReportI == "N" && settlement0 != null && settlement1 == null) ||
						((param.ReportI == "A" || param.ReportI == "N") &&
						 ((settlement1 == null && settlement0 == null) || (settlement1 != null && settlement0 != null))))
					{
						DateTime startDate;
						DateTime endDate;
						if (param.ReportI.Equals("A"))
						{
							// get month and year transport
							var month = param.TransportM.Value.Month;
							var year = param.TransportM.Value.Year;
							var invoiceInfo = _invoiceInfoService.GetLimitStartAndEndInvoiceD(customerMainC, customerSubC, month, year);
							startDate = invoiceInfo.StartDate.Date;
							endDate = invoiceInfo.EndDate.Date;
						}
						else
						{
							startDate = (DateTime)param.OrderDFrom;
							endDate = (DateTime)param.OrderDTo;
						}
						// get customers who shared a invoice company
						var customerStr = "";
						var customerList = _customerService.GetCustomersByInvoice(customerMainC, customerSubC);
						for (var aloop = 0; aloop < customerList.Count; aloop++)
						{
							customerStr = customerStr + "," + customerList[aloop].CustomerMainC + "_" + customerList[aloop].CustomerSubC;
						}

						// get and calculate detain date
						var dispatch = from a in _dispatchRepository.GetAllQueryable()
									   join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo }
										   equals new { b.OrderD, b.OrderNo, b.DetailNo } into t1
									   from b in t1.DefaultIfEmpty()
									   where (b.RevenueD >= startDate & b.RevenueD <= endDate)
									   select new DispatchViewModel()
									   {
										   OrderD = a.OrderD,
										   OrderNo = a.OrderNo,
										   DetailNo = a.DetailNo,
										   DispatchNo = a.DispatchNo,
										   DetainDay = a.DetainDay,
										   TruckC = a.TruckC,
										   TransportD = a.TransportD
									   };
						dispatch = dispatch.OrderBy("OrderD asc, OrderNo asc, DetailNo asc, DispatchNo asc");

						var groupDispatch = from b in dispatch
											group b by new { b.OrderD, b.OrderNo, b.DetailNo, b.TransportD }
												into c
												select new
												{
													c.Key.OrderD,
													c.Key.OrderNo,
													c.Key.DetailNo,
													c.Key.TransportD,
													DetainDay = c.Sum(b => b.DetainDay)
												};

						//get Truck No
						var truck = from b in dispatch
									join t in _truckRepository.GetAllQueryable()
										on b.TruckC equals t.TruckC
									where (b.DispatchNo == 1)
									select new
									{
										OrderD = b.OrderD,
										OrderNo = b.OrderNo,
										DetailNo = b.DetailNo,
										TruckNo = t.RegisteredNo
									};

						// get surcharge
						var surcharge = from a in _surchargeDetailRepository.GetAllQueryable()
										join b in groupDispatch on new { a.OrderD, a.OrderNo, a.DetailNo }
											equals new { b.OrderD, b.OrderNo, b.DetailNo } into t1
										where a.DispatchNo == 0
										select new SurchargeDetailViewModel()
										{
											OrderD = a.OrderD,
											OrderNo = a.OrderNo,
											DetailNo = a.DetailNo,
											Amount = a.Amount,
											Description = a.Description
										};
						var groupSurcharge = (from b in surcharge.AsEnumerable()
											  group b by new { b.OrderD, b.OrderNo, b.DetailNo }
												  into c
												  select new
												  {
													  OrderD = c.Key.OrderD,
													  OrderNo = c.Key.OrderNo,
													  DetailNo = c.Key.DetailNo,
													  Amount = c.Sum(b => b.Amount),
													  Description = String.Join(",", c.Select(b => b.Description))
												  }).ToList();
						//var temp = groupSurcharge.ToList();
						// get data
						var data = from d in _orderDRepository.GetAllQueryable()
								   join e in _orderHRepository.GetAllQueryable() on new { d.OrderD, d.OrderNo }
									   equals new { e.OrderD, e.OrderNo } into t1
								   from e in t1.DefaultIfEmpty()
								   join m in _truckRepository.GetAllQueryable() on d.TruckCLastDispatch
									   equals m.TruckC into t9
								   from m in t9.DefaultIfEmpty()
								   //join c in _customerRepository.GetAllQueryable() on new { e.CustomerMainC, e.CustomerSubC }
								   //	equals new { c.CustomerMainC, c.CustomerSubC } into t6
								   //from c in t6.DefaultIfEmpty()
								   join f in _departmentRepository.GetAllQueryable() on e.OrderDepC
									   equals f.DepC into t2
								   from f in t2.DefaultIfEmpty()
								   join g in groupDispatch on new { d.OrderD, d.OrderNo, d.DetailNo }
									   equals new { g.OrderD, g.OrderNo, g.DetailNo }
								   join h in _expenseDetailRepository.Query(h => h.IsRequested == "1" && h.Amount > 0) on
									   new { d.OrderD, d.OrderNo, d.DetailNo }
									   equals new { h.OrderD, h.OrderNo, h.DetailNo } into t4
								   from h in t4.DefaultIfEmpty()
								   join k in _expenseRepository.GetAllQueryable() on h.ExpenseC
									   equals k.ExpenseC into t5
								   from k in t5.DefaultIfEmpty()
								   //join s in groupSurcharge on new { d.OrderD, d.OrderNo, d.DetailNo }
								   // equals new { s.OrderD, s.OrderNo, s.DetailNo } into t7
								   //from s in t7.DefaultIfEmpty()
								   join t in truck on new { d.OrderD, d.OrderNo, d.DetailNo }
									   equals new { t.OrderD, t.OrderNo, t.DetailNo } into t8
								   from t in t8.DefaultIfEmpty()
								   where ((customerStr.Contains("," + e.CustomerMainC + "_" + e.CustomerSubC)) &
										  (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || e.OrderDepC == param.DepC) &
										  (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || e.OrderTypeI == param.OrderTypeI) &
										  (param.BLBK.Equals("null") || e.BLBK.Equals(param.BLBK)) &
										  (param.JobNo.Equals("null") || e.JobNo.Contains(param.JobNo))
									   )
								   select new CustomerExpenseItem()
								   {
									   OrderD = new ContainerViewModel()
									   {
										   OrderD = d.OrderD,
										   OrderNo = d.OrderNo,
										   DetailNo = d.DetailNo,
										   ContainerNo = d.ContainerNo,
										   ContainerSizeI = d.ContainerSizeI,
										   Amount = d.Amount,
										   ActualLoadingD = d.ActualLoadingD,
										   ActualDischargeD = d.ActualDischargeD,
										   NetWeight = d.NetWeight,
										   RegisteredNo = m.RegisteredNo ?? "",
										   LocationDispatch1 = d.LocationDispatch1 ?? "",
										   LocationDispatch2 = d.LocationDispatch2 ?? "",
										   LocationDispatch3 = d.LocationDispatch3 ?? "",
										   CustomerSurcharge = d.CustomerSurcharge,
										   TotalAmount = d.TotalAmount,
										   TotalExpense = d.TotalExpense
									   },
									   OrderH = new OrderViewModel()
									   {
										   OrderD = e.OrderD,
										   OrderNo = e.OrderNo,
										   //CustomerMainC = e.CustomerMainC,
										   //CustomerSubC = e.CustomerSubC,
										   //CustomerN = c != null ? c.CustomerN : "",
										   OrderTypeI = e.OrderTypeI,
										   OrderDepN = f != null ? f.DepN : "",
										   LoadingPlaceN = e.LoadingPlaceN,
										   StopoverPlaceN = e.StopoverPlaceN,
										   DischargePlaceN = e.DischargePlaceN,
										   BLBK = e.BLBK,
										   IsCollected = e.IsCollected,
										   JobNo = e.JobNo,
										   CustomerMainC = e.CustomerMainC,
										   CustomerSubC = e.CustomerSubC,
										   CustomerPayLiftLoweredMainC = e.CustomerPayLiftLoweredMainC,
										   CustomerPayLiftLoweredSubC = e.CustomerPayLiftLoweredSubC
									   },
									   DetainDay = g.DetainDay,
									   TransportD = g.TransportD,
									   //SurchargeAmount = s.Amount != null ? (decimal)s.Amount : 0,
									   //Description = s.Description,
									   DetainAmount = d.DetainAmount ?? 0,
									   TaxAmount = d.TaxAmount ?? 0,
									   ExpenseD = new ExpenseDetailViewModel()
									   {
										   ExpenseC = h != null ? h.ExpenseC : "",
										   ExpenseN = k != null ? k.ExpenseN : "",
										   Description = h != null ? h.Description : "",
										   Amount = h != null ? h.Amount : null,
										   TaxAmount = h != null ? h.TaxAmount : null,
										   TaxRate = h != null ? h.TaxRate : null,
										   ExpenseI = k.ExpenseI,
										   IsIncluded = h.IsIncluded,
										   IsRequested = h.IsRequested
									   },
									   TruckNo = t.TruckNo,
									   //CustomerAddress = c.Address1,
									   //CustomerTaxCode = c.TaxCode
								   };
						data = data.OrderBy("OrderD.OrderD asc, OrderD.OrderNo asc, OrderD.DetailNo asc");
						var list = data.ToList();
						string custMainCOrderH = "";
						string custSubCOrderH = "";
						//add surcharge
						if (list != null && list.Count > 0)
						{
							for (int i = 0; i < list.Count; i++)
							{
								custMainCOrderH = list[i].OrderH.CustomerMainC;
								custSubCOrderH = list[i].OrderH.CustomerSubC;
								var orderD = list[i].OrderH.OrderD;
								var orderNo = list[i].OrderH.OrderNo;
								var detailNo = list[i].OrderD.DetailNo;
								var surchargeData = (from s in groupSurcharge
														where s.OrderD == orderD &&
															s.OrderNo == orderNo &&
															s.DetailNo == detailNo
														select new CustomerExpenseItem()
														{
															SurchargeAmount = s.Amount ?? 0,
															Description = s.Description
														}).ToList();
								if (surchargeData != null && surchargeData.Count > 0)
								{
									list[i].SurchargeAmount = surchargeData[0].SurchargeAmount;
									list[i].Description = surchargeData[0].Description.Length > 1 ? surchargeData[0].Description : "";
								}
							}
							item.CustomerMainC = custMainCOrderH;
							item.CustomerSubC = custSubCOrderH;
							var customer =
								_customerRepository.Query(p => p.CustomerMainC == custMainCOrderH && p.CustomerSubC == custSubCOrderH)
									.FirstOrDefault();
							if (customer != null)
							{
								item.CustomerN = customer.CustomerN;
								item.CustomerAddress = customer.Address1;
								item.CustomerTaxCode = customer.TaxCode;
							}

							for (var loop = 0; loop < list.Count; loop++)
							{
								var finalList = new List<CustomerExpenseItem>();
								var compareEmpty = 0;
								var compareAdd = 0;
								decimal taxRate = (100 * (list[loop].TaxAmount)) /
													((list[loop].OrderD.TotalAmount != 0 && list[loop].OrderD.TotalAmount != null
														? ((decimal)list[loop].OrderD.TotalAmount -
															(list[loop].OrderD.TotalExpense != 0 && list[loop].OrderD.TotalExpense != null
																? (decimal)list[loop].OrderD.TotalExpense
																: 0))
														: 0) > 0
														? ((decimal)list[loop].OrderD.TotalAmount - (decimal)list[loop].OrderD.TotalExpense)
														: 1);
								for (var subloop = 0; subloop < list.Count; subloop++)
								{
									decimal subTaxRate = (100 * (list[subloop].TaxAmount)) /
															((list[subloop].OrderD.TotalAmount != 0 && list[subloop].OrderD.TotalAmount != null
																? ((decimal)list[subloop].OrderD.TotalAmount -
																(list[subloop].OrderD.TotalExpense != 0 && list[subloop].OrderD.TotalExpense != null
																	? (decimal)list[subloop].OrderD.TotalExpense
																	: 0))
																: 0) > 0
																? ((decimal)list[subloop].OrderD.TotalAmount -
																(decimal)list[subloop].OrderD.TotalExpense)
																: 1);
									if (taxRate == subTaxRate)
									{
										finalList.Add(list[subloop]);
										list.RemoveAt(subloop);
										subloop--;
										compareAdd++;
									}
									else
									{
										compareEmpty++;
									}
								}
								if (compareEmpty > 0 && compareAdd == 0)
								{
									finalList.Add(list[loop]);
								}
								loop--;
								//item.CustomerExpenseList = finalList;
								var listAddGeneral = new CustomerExpenseReportData();
								listAddGeneral.CustomerMainC = custMainCOrderH;
								listAddGeneral.CustomerSubC = custSubCOrderH;
								var customer2 =
									_customerRepository.Query(p => p.CustomerMainC == custMainCOrderH && p.CustomerSubC == custSubCOrderH)
										.FirstOrDefault();
								if (customer2 != null)
								{
									listAddGeneral.CustomerN = customer2.CustomerN;
									listAddGeneral.CustomerAddress = customer2.Address1;
									listAddGeneral.CustomerTaxCode = customer2.TaxCode;
								}

								listAddGeneral.CustomerExpenseList = finalList;
								listAddGeneral.TaxRate = Math.Round((100 * (decimal)listAddGeneral.CustomerExpenseList[0].TaxAmount) /
																	((listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount != null &&
																		listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount != 0
																		? ((decimal)listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount -
																			(listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense != 0 &&
																			listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense != null
																				? (decimal)listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense
																				: 0))
																		: 0) > 0
																		? ((decimal)listAddGeneral.CustomerExpenseList[0].OrderD.TotalAmount -
																			(decimal)listAddGeneral.CustomerExpenseList[0].OrderD.TotalExpense)
																		: 1));
								listAddGeneral.TaxRate = listAddGeneral.TaxRate > 100 ? 0 : listAddGeneral.TaxRate;
								dataList.Add(listAddGeneral);
							}
							//dataList.Add(item);
						}
					}
				}
				for (var kloop = 0; kloop < dataList.Count; kloop++)
				{
					var custMC = dataList[kloop].CustomerMainC;
					var custSC = dataList[kloop].CustomerSubC;
					decimal taxR = dataList[kloop].TaxRate;
					for (var ploop = 0; ploop < dataList.Count; ploop++)
					{
						if (kloop != ploop)
						{
							if (dataList[ploop].CustomerMainC == custMC && dataList[ploop].CustomerSubC == custSC &&
								dataList[ploop].TaxRate == taxR)
							{
								dataList[kloop].CustomerExpenseList.AddRange(dataList[ploop].CustomerExpenseList);
								dataList.RemoveAt(ploop);
								kloop--;
								ploop--;
							}
						}
					}
				}

				// update invoice status
				if (dataList.Count > 0)
				{
					for (var iloop = 0; iloop < dataList.Count; iloop++)
					{
						if (dataList[iloop].CustomerExpenseList != null && dataList[iloop].CustomerExpenseList.Count > 0)
						{

							for (var jloop = 0; jloop < dataList[iloop].CustomerExpenseList.Count; jloop++)
							{
								var orderD = dataList[iloop].CustomerExpenseList[jloop].OrderD.OrderD;
								var orderNo = dataList[iloop].CustomerExpenseList[jloop].OrderD.OrderNo;
								var detailNo = dataList[iloop].CustomerExpenseList[jloop].OrderD.DetailNo;

								var dispatchCount = _dispatchRepository.Query(dis => dis.OrderD == orderD &
																					 dis.OrderNo == orderNo &
																					 dis.DetailNo == detailNo
									).ToList();

								if (dispatchCount.Count > 0)
								{
									// transfer invoice status
									for (var kloop = 0; kloop < dispatchCount.Count; kloop++)
									{
										var dispatchUpdate = dispatchCount[kloop];
										dispatchUpdate.InvoiceStatus = "1";
										dispatchUpdate.DispatchStatus = "3";
										_dispatchRepository.Update(dispatchUpdate);
										numberofupdate++;
									}
								}
							}
						}
					}
					SaveReport();
				}
			}
			if (numberofupdate > 0)
			{
				return 1;
			}
			return 0;
		}
		public Stream ExportPdfCustomerExpenseLoad(CustomerExpenseReportParam param, string userName)
		{
			Stream stream;
			var dt = new CustomerExpense.CustomerExpenseDataTable();
			DataRow row;
			Dictionary<string, string> dicLanguage;
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
			//ger employeeName
			var user = GetEmployeeByUserName(userName);
			// get data
			List<CustomerExpenseReportData> data = GetCustomerExpenseLoadReport(param);

			// get language for report
			int intLanguage = Utilities.ConvertLanguageToInt(param.Languague);

			dicLanguage = new Dictionary<string, string>();
			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																  (con.TextKey == "LBLLOAD" ||
																   con.TextKey == "LBLTON" ||
																   con.TextKey == "LBLCUSTOMEREXPENSELOADTITTLE" ||
																   con.TextKey == "LBLINVOICEATTACHED" ||
																   con.TextKey == "LBLCUSTOMER" ||
																   con.TextKey == "LBLADDRESSRP" ||
																   con.TextKey == "LBLTRANSPORTDATEDISPATCHRP" ||
																   con.TextKey == "LBLTRUCKNODISPATCHRP" ||
																   con.TextKey == "LBLLOADINGLOC" ||
																   con.TextKey == "LBLDISCHARGELOC" ||
																   con.TextKey == "RPHDQUANTITYNETWEIGHT" ||
																   con.TextKey == "LBLUNITPRICERP" ||
																   con.TextKey == "LBLAMOUNTSHORTRP" ||
																   con.TextKey == "LBLSURCHARGEFEE" ||
																   con.TextKey == "LBLDESCRIPTIONREPORT" ||
																   con.TextKey == "LBLAMOUNTRP" ||
																   con.TextKey == "LBLTAXAMOUNTREPORT" ||
																   con.TextKey == "LBLTRANSPORTFEEAFTERTAXRP" ||
																   con.TextKey == "LBLENTRYCLERKN" ||
																   con.TextKey == "RPFTPAGE" ||
																   con.TextKey == "RPFTPRINTBY" ||
																   con.TextKey == "RPFTPRINTTIME"
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
				for (var iloop = 0; iloop < data.Count; iloop++)
				{
					decimal totalTransportFee = 0;
					decimal totalDetainFee = 0;
					//decimal totalExpense = 0;
					decimal totalTax = 0;

					if (data[iloop].CustomerExpenseList != null && data[iloop].CustomerExpenseList.Count > 0)
					{
						var newKey = "";
						var oldKey = "";
						//var rowOfGroup = 0;
						var index = 1;
						totalTransportFee = 0;
						totalDetainFee = 0;
						//totalExpense = 0;
						totalTax = 0;
						for (var jloop = 0; jloop < data[iloop].CustomerExpenseList.Count; jloop++)
						{
							row = dt.NewRow();
							decimal amount = 0;
							decimal detainamount = 0;
							decimal taxAmount = 0;
							decimal taxRate = 0;
							decimal totalTransport = 0;

							//c1
							if (data[iloop].CustomerExpenseList[jloop].OrderD.ActualLoadingD != null)
								row["LoadingDate"] = Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].CustomerExpenseList[jloop].OrderD.ActualLoadingD, intLanguage);
							//c2
							row["Department"] = data[iloop].CustomerExpenseList[jloop].TruckNo;
							//c3
							row["OrderType"] = data[iloop].CustomerExpenseList[jloop].OrderH.OrderNo;
							//c4
							row["Location"] = data[iloop].CustomerExpenseList[jloop].OrderH.LoadingPlaceN;
							//c5
							row["BKBL"] = data[iloop].CustomerExpenseList[jloop].OrderH.StopoverPlaceN;
							//c6
							row["Description"] = data[iloop].CustomerExpenseList[jloop].OrderH.DischargePlaceN;
							//c7
							row["ContainerNo"] = data[iloop].CustomerExpenseList[jloop].OrderD.NetWeight > 0
								? data[iloop].CustomerExpenseList[jloop].OrderD.NetWeight : 0;
							//c8
							row["ExpenseFee"] = data[iloop].CustomerExpenseList[jloop].OrderD.UnitPrice;
							//c9
							if (data[iloop].CustomerExpenseList[jloop].OrderD.Amount != null)
							{
								amount = (decimal)data[iloop].CustomerExpenseList[jloop].OrderD.Amount;
								row["TransportFee"] = amount;
								totalTransportFee += amount;
							}
							if (data[iloop].CustomerExpenseList[jloop].OrderD.DetainAmount != null)
							{
								detainamount = (decimal)data[iloop].CustomerExpenseList[jloop].OrderD.DetainAmount;
								row["DetainFee"] = detainamount;
								totalDetainFee += detainamount;
							}
							//var surchargeDescription = data[iloop].CustomerExpenseList[jloop].Description;
							//if (surchargeDescription == null)
							//{
							//	row["TaxMethodI"] = data[iloop].CustomerExpenseList[jloop].OrderD.Description;
							//}
							//else
							//{
							//	row["TaxMethodI"] = surchargeDescription + ", " + data[iloop].CustomerExpenseList[jloop].OrderD.Description;
							//}
							row["TaxMethodI"] = data[iloop].CustomerExpenseList[jloop].OrderD.Description;
							row["ExpenseType"] = "";
							row["InvoiceNo"] = "";


							row["Sum"] = amount + detainamount;
							taxAmount = data[iloop].CustomerExpenseList[jloop].TaxAmount;
							row["TaxAmount"] = taxAmount;

							// cal totalColumn15
							//totalColumn15 = 0;
							if (data[iloop].CustomerExpenseList[jloop].OrderD.Amount != null)
							{
								totalTransport = totalTransport + amount;
								//totalColumn15 += (decimal)(amount + taxAmount);
								totalTax += taxAmount;
							}
							//taxRate = (amount == 0) ? 0 : Math.Round(taxAmount*100/(amount + detainamount));
							if (amount + detainamount == 0)
							{
								taxRate = (int)(data[iloop].CustomerExpenseList[jloop].CustomerSettlement.TaxRate ?? 0);
							}
							else
							{
								taxRate = Math.Round(taxAmount * 100 / (amount + detainamount));
							}
							newKey = data[iloop].CustomerMainC + "_" + data[iloop].CustomerSubC + "_" + taxRate;
							row["KeyCustomer"] = newKey;
							row["TaxRate"] = taxRate;

							if (newKey != oldKey)
							{
								oldKey = newKey;
								index = 1;
								//rowOfGroup = 1;
							}
							// set info row
							row["No"] = index++;
							row["keyRow"] = row["No"];

							// set info customer
							row["CustomerMainC"] = data[iloop].CustomerMainC;
							row["CustomerSubC"] = data[iloop].CustomerSubC;
							row["CustomerAddress"] = data[iloop].CustomerAddress;
							row["CustomerTaxCode"] = data[iloop].CustomerTaxCode;
							row["CustomerN"] = data[iloop].CustomerN;

							row["TaxRoundingI"] = data[iloop].TaxRoundingI;
							row["RevenueRoundingI"] = data[iloop].RevenueRoundingI;

							dt.Rows.Add(row);
						}
					}
				}
			}

			// set month and year
			var monthYear = "";
			if (param.ReportI.Equals("A"))
			{
				monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				if (intLanguage == 1)
				{
					monthYear = "Tháng " + param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
				if (intLanguage == 3)
				{
					monthYear = param.TransportM.Value.Year + "年" + param.TransportM.Value.Month + "月";
				}
			}
			else
			{
				var fromDate = (DateTime)param.OrderDFrom;
				var toDate = (DateTime)param.OrderDTo;

				monthYear = "From " + fromDate.ToString("MM/dd/yyyy") + " to " + toDate.ToString("MM/dd/yyyy");
				if (intLanguage == 1)
				{
					monthYear = "Từ ngày " + fromDate.ToString("dd/MM/yyyy") + " đến ngày " + toDate.ToString("dd/MM/yyyy");
				}
				if (intLanguage == 3)
				{
					monthYear = fromDate.Year + "年" + fromDate.Month + "月" + fromDate.Day + "日" + "から" +
								toDate.Year + "年" + toDate.Month + "月" + toDate.Day + "日" + "まで";
				}
			}
			stream = CrystalReport.Service.CustomerExpense.ExportPdf.ExecLoad(dt, intLanguage, companyName, companyAddress, companyTaxCode,
																				monthYear, fileName, user, dicLanguage);
			return stream;
		}
		public Stream ExportExcelCustomerExpenseLoad(CustomerExpenseReportParam param, string userName)
		{
			Dictionary<string, string> dicLanguage;
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
			//ger employeeName
			var user = GetEmployeeByUserName(userName);
			// get data
			List<CustomerExpenseReportData> data = GetCustomerExpenseLoadReport(param);

			// get language for report
			int intLanguage = Utilities.ConvertLanguageToInt(param.Languague);

			dicLanguage = new Dictionary<string, string>();
			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																  (con.TextKey == "LBLLOAD" ||
																	con.TextKey == "LBLTON" ||
																	con.TextKey == "LBLLOADINGLOC" ||
																	con.TextKey == "LBLDISCHARGELOC" ||
																	con.TextKey == "RPHDAMOUNT" ||
																	con.TextKey == "RPHDTOTALSURCHARGE" ||
																	con.TextKey == "LBLDESCRIPTION" ||
																	con.TextKey == "LBLTAXCODECOMPANYREPORT" ||
																	con.TextKey == "LBLCUSTCODE" ||
																	con.TextKey == "LBLCUSTOMER" ||
																	con.TextKey == "LBLADDRESS" ||

																	con.TextKey == "LBLTRANSPORTFEEREPORT" ||
																	con.TextKey == "COMPANYTAXRATE" ||
																	con.TextKey == "LBLTAXVAT" ||
																	con.TextKey == "LBLTRANSPORTFEEAFTERTAX" ||

																	con.TextKey == "LBLTRANSPORTDREPORT" ||
																	con.TextKey == "LBLTRUCKNO" ||
				//con.TextKey == "Nơi lấy hàng " ||
				//con.TextKey == "Nơi giao hàng " ||
																	con.TextKey == "RPHDQUANTITYNETWEIGHT" ||
																	con.TextKey == "RPHDUNITPRICE" ||
																	con.TextKey == "LBLTOL" ||
																	con.TextKey == "LBLCUSTOMEREXPENSELOADTITTLE"
																  )).ToList();

			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			// set month and year
			var monthYear = "";
			if (param.ReportI.Equals("A"))
			{
				monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				if (intLanguage == 1)
				{
					monthYear = "Tháng " + param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
				}
				if (intLanguage == 3)
				{
					monthYear = param.TransportM.Value.Year + "年" + param.TransportM.Value.Month + "月";
				}
			}
			else
			{
				var fromDate = (DateTime)param.OrderDFrom;
				var toDate = (DateTime)param.OrderDTo;

				monthYear = "From " + fromDate.ToString("MM/dd/yyyy") + " to " + toDate.ToString("MM/dd/yyyy");
				if (intLanguage == 1)
				{
					monthYear = "Từ ngày " + fromDate.ToString("dd/MM/yyyy") + " đến ngày " + toDate.ToString("dd/MM/yyyy");
				}
				if (intLanguage == 3)
				{
					monthYear = fromDate.Year + "年" + fromDate.Month + "月" + fromDate.Day + "日" + "から" +
								toDate.Year + "年" + toDate.Month + "月" + toDate.Day + "日" + "まで";
				}
			}
			return CrystalReport.Service.CustomerExpense.ExportExcel.ExecLoadToExcel(data, dicLanguage, intLanguage, companyName, companyAddress, companyTaxCode,
																				monthYear, fileName, user);
		}
		public List<CustomerExpenseReportData> GetCustomerExpenseLoadReport(CustomerExpenseReportParam param)
		{
			var dataList = new List<CustomerExpenseReportData>();
			var result = new List<CustomerExpenseReportData>();
			string sub = string.Empty;
			if (param.Customer == "null")
			{
				var customerList = _customerService.GetInvoices();

				if (customerList != null && customerList.Count > 0)
				{
					param.Customer = "";
					for (var iloop = 0; iloop < customerList.Count; iloop++)
					{
						string custMainC = customerList[iloop].CustomerMainC;
						string custSubC = customerList[iloop].CustomerSubC;
						var settlement0 = _customerSettlementRepository.Query(p => p.CustomerMainC == custMainC && p.CustomerSubC == custSubC && p.FormOfSettlement == Constants.NORMAL).OrderByDescending(p => p.ApplyD).FirstOrDefault();
						var settlement1 = _customerSettlementRepository.Query(p => p.CustomerMainC == custMainC && p.CustomerSubC == custSubC && p.FormOfSettlement == Constants.SETTLEMENT).OrderByDescending(p => p.ApplyD).FirstOrDefault();
						if (param.ReportI == "A")
						{
							if (settlement0 == null)
							{
								param.Customer = param.Customer + "," + customerList[iloop].CustomerMainC + "_" +
								                 customerList[iloop].CustomerSubC;
							}
						}
						if (param.ReportI == "N")
						{
							if (settlement1 == null)
							{
								param.Customer = param.Customer + "," + customerList[iloop].CustomerMainC + "_" +
								                 customerList[iloop].CustomerSubC;
							}
						}
					}
					sub = param.Customer.Substring(1);
					param.Customer = sub;
				}
			}
			// get data
			if (param.Customer != "null")
			{
				var customerArr = (param.Customer).Split(new string[] { "," }, StringSplitOptions.None);
				for (var iloop = 0; iloop < customerArr.Length - 1; iloop++)
				{
					var item = new CustomerExpenseReportData();

					var arr = (customerArr[iloop]).Split(new string[] {"_"}, StringSplitOptions.None);
					var customerMainC = arr[0];
					var customerSubC = arr[1];
					var settlement0 =
						_customerSettlementRepository.Query(
							p => p.CustomerMainC == customerMainC && p.CustomerSubC == customerSubC && p.FormOfSettlement == "0")
							.OrderByDescending(p => p.ApplyD)
							.FirstOrDefault();
					var settlement1 =
						_customerSettlementRepository.Query(
							p => p.CustomerMainC == customerMainC && p.CustomerSubC == customerSubC && p.FormOfSettlement == "1")
							.OrderByDescending(p => p.ApplyD)
							.FirstOrDefault();
					if ((param.ReportI == "A" && settlement1 != null && settlement0 == null) ||
						(param.ReportI == "N" && settlement0 != null && settlement1 == null) ||
						((param.ReportI == "A" || param.ReportI == "N") &&
						 ((settlement1 == null && settlement0 == null) || (settlement1 != null && settlement0 != null))))
					{
						DateTime startDate;
						DateTime endDate;
						if (param.ReportI.Equals("A"))
						{
							// get month and year transport
							var month = param.TransportM.Value.Month;
							var year = param.TransportM.Value.Year;
							var invoiceInfo = _invoiceInfoService.GetLimitStartAndEndInvoiceD(customerMainC, customerSubC, month, year);
							startDate = invoiceInfo.StartDate.Date;
							endDate = invoiceInfo.EndDate.Date;
						}
						else
						{
							startDate = (DateTime) param.OrderDFrom;
							endDate = (DateTime) param.OrderDTo;
						}
						// get customers who shared a invoice company

						var customerSet = _customerService.GetCustomerSettlementByMainCodeSubCode(customerMainC, customerSubC);

						var customerStr = "";
						var customerList = _customerService.GetCustomersByInvoice(customerMainC, customerSubC);
						for (var aloop = 0; aloop < customerList.Count; aloop++)
						{
							customerStr = customerStr + "," + customerList[aloop].CustomerMainC + "_" + customerList[aloop].CustomerSubC;
						}

						//get Truck No
						var dispatch = from a in _dispatchRepository.GetAllQueryable()
							join b in _orderDRepository.GetAllQueryable() on new {a.OrderD, a.OrderNo, a.DetailNo}
								equals new {b.OrderD, b.OrderNo, b.DetailNo} into t1
							from b in t1.DefaultIfEmpty()
							join t in _truckRepository.GetAllQueryable()
								on a.TruckC equals t.TruckC into bt
							from t in bt.DefaultIfEmpty()
							where ((param.InvoiceStatus == "-1" ||
							        (param.InvoiceStatus == "0" && a.InvoiceStatus != "1") ||
							        (param.InvoiceStatus == "1" && a.InvoiceStatus == "1")
								) &
							       (b.RevenueD >= startDate & b.RevenueD <= endDate) &
							       a.DispatchNo == 1)
							select new DispatchViewModel()
							{
								OrderD = a.OrderD,
								OrderNo = a.OrderNo,
								DetailNo = a.DetailNo,
								DispatchNo = a.DispatchNo,
								TruckC = a.TruckC,
								TransportD = a.TransportD,
								Location1C =
									a.Operation1C.Equals("LH") || a.Operation1C.Equals("LC")
										? a.Location1C
										: (a.Operation2C.Equals("LH") || a.Operation2C.Equals("LC")
											? a.Location2C
											: (a.Operation3C.Equals("LH") || a.Operation3C.Equals("LC") ? a.Location3C : "")),
								Location2C =
									a.Operation1C.Equals("XH") || a.Operation1C.Equals("HC")
										? a.Location1C
										: (a.Operation2C.Equals("XH") || a.Operation2C.Equals("HC")
											? a.Location2C
											: (a.Operation3C.Equals("XH") || a.Operation3C.Equals("HC") ? a.Location3C : "")),
								Location1N =
									a.Operation1C.Equals("LH") || a.Operation1C.Equals("LC")
										? a.Location1N
										: (a.Operation2C.Equals("LH") || a.Operation2C.Equals("LC")
											? a.Location2N
											: (a.Operation3C.Equals("LH") || a.Operation3C.Equals("LC") ? a.Location3N : "")),
								Location2N =
									a.Operation1C.Equals("XH") || a.Operation1C.Equals("HC")
										? a.Location1N
										: (a.Operation2C.Equals("XH") || a.Operation2C.Equals("HC")
											? a.Location2N
											: (a.Operation3C.Equals("XH") || a.Operation3C.Equals("HC") ? a.Location3N : "")),
								RegisteredNo = t != null ? t.RegisteredNo : a.RegisteredNo
							};
						dispatch = dispatch.OrderBy("OrderD asc, OrderNo asc, DetailNo asc, DispatchNo asc");

						//var truck = from b in dispatch
						//join t in _truckRepository.GetAllQueryable()
						//on b.TruckC equals t.TruckC into bt
						//from t in bt.DefaultIfEmpty()
						//				where (b.DispatchNo == 1)
						//				select new
						//				{
						//					OrderD = b.OrderD,
						//					OrderNo = b.OrderNo,
						//					DetailNo = b.DetailNo,
						//					TruckNo = t != null? t.RegisteredNo : "",
						//					TruckNo = b.RegisteredNo,
						//					TransportD = b.TransportD,
						//					Location1C = b.Location1C,
						//					Location2C= b.Location2C,
						//					Location1N = b.Location1N,
						//					Location2N = b.Location2N,
						//				};
						//var truckList = truck.ToList();
						// get surcharge
						var surcharge =
							from d in _orderDRepository.GetAllQueryable()
							join e in _orderHRepository.GetAllQueryable() on new {d.OrderD, d.OrderNo}
								equals new {e.OrderD, e.OrderNo} into t1
							from e in t1.DefaultIfEmpty()
							join s in _surchargeDetailRepository.GetAllQueryable() on new {d.OrderD, d.OrderNo, d.DetailNo}
								equals new {s.OrderD, s.OrderNo, s.DetailNo}
							select new SurchargeDetailViewModel()
							{
								OrderD = s.OrderD,
								OrderNo = s.OrderNo,
								DetailNo = s.DetailNo,
								Amount = s.Amount,
								Description = s.Description
							};
						var groupSurcharge = (from b in surcharge.AsEnumerable()
							group b by new {b.OrderD, b.OrderNo, b.DetailNo}
							into c
							select new
							{
								OrderD = c.Key.OrderD,
								OrderNo = c.Key.OrderNo,
								DetailNo = c.Key.DetailNo,
								Amount = c.Sum(b => b.Amount),
								Description = String.Join(", ", c.Select(b => b.Description))
							}).ToList();
						// get data
						var data = from d in _orderDRepository.GetAllQueryable()
							join e in _orderHRepository.GetAllQueryable() on new {d.OrderD, d.OrderNo}
								equals new {e.OrderD, e.OrderNo} into t4
							from e in t4.DefaultIfEmpty()
							//join f in _departmentRepository.GetAllQueryable() on e.OrderDepC
							//	equals f.DepC into t2
							//from f in t2.DefaultIfEmpty()
							//join t in truck on new { d.OrderD, d.OrderNo, d.DetailNo }
							//equals new { t.OrderD, t.OrderNo, t.DetailNo } into t8
							//from t in t8.DefaultIfEmpty()
							join t in dispatch on new {d.OrderD, d.OrderNo, d.DetailNo}
								equals new {t.OrderD, t.OrderNo, t.DetailNo}
							//join l1 in _locationRepository.GetAllQueryable() on t.Location1C equals l1.LocationC into t9
							//from l1 in t9.DefaultIfEmpty()
							join l2 in _locationRepository.GetAllQueryable() on t.Location2C equals l2.LocationC into t10
							from l2 in t10.DefaultIfEmpty()
							where ((customerStr.Contains("," + e.CustomerMainC + "_" + e.CustomerSubC)) &
							       (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || e.OrderDepC == param.DepC) &
							       (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || e.OrderTypeI == param.OrderTypeI) &
							       (param.BLBK.Equals("null") || e.BLBK.Equals(param.BLBK)) &
							       d.ContainerSizeI == "3" &
							       (d.RevenueD >= startDate & d.RevenueD <= endDate)
								)
							select new CustomerExpenseItem()
							{
								CustomerSettlement = new CustomerSettlementViewModel()
								{
									TaxRate = customerSet.TaxRate,
								},
								OrderD = new ContainerViewModel()
								{
									Description = d.Description,
									OrderD = d.OrderD,
									OrderNo = d.OrderNo,
									DetailNo = d.DetailNo,
									ContainerNo = d.ContainerNo,
									ContainerSizeI = d.ContainerSizeI,
									ContainerTypeC = d.ContainerTypeC,
									Amount = d.Amount,
									DetainAmount = d.CustomerSurcharge + d.DetainAmount,
									ActualLoadingD = t.TransportD,
									NetWeight = d.NetWeight,
									UnitPrice = d.UnitPrice,
								},
								OrderH = new OrderViewModel()
								{
									OrderD = e.OrderD,
									OrderNo = e.OrderNo,
									OrderTypeI = e.OrderTypeI,
									//OrderDepN = f != null ? f.DepN : "",
									LoadingPlaceN = t.Location1N,
									StopoverPlaceN = t.Location2N,
									DischargePlaceN = l2 != null ? l2.Address : "",
								},
								DetainDay = 0,
								DetainAmount = 0,
								TaxAmount = d.TaxAmount ?? 0,
								TruckNo = t.RegisteredNo,
							};
						data = data.OrderBy("OrderD.ActualLoadingD asc, TruckNo asc");
						var list = data.ToList();
						//add surcharge
						if (list != null && list.Count > 0)
						{
							for (int i = 0; i < list.Count; i++)
							{
								var orderD = list[i].OrderH.OrderD;
								var orderNo = list[i].OrderH.OrderNo;
								var detailNo = list[i].OrderD.DetailNo;
								var surchargeData = (from s in groupSurcharge
									where s.OrderD == orderD &&
									      s.OrderNo == orderNo &&
									      s.DetailNo == detailNo
									select new CustomerExpenseItem()
									{
										SurchargeAmount = s.Amount ?? 0,
										Description = s.Description
									}).ToList();
								if (surchargeData != null && surchargeData.Count > 0)
								{
									list[i].SurchargeAmount = surchargeData[0].SurchargeAmount;
									//list[i].Description = surchargeData[0].Description;
									string[] temp = surchargeData[0].Description.Split(',');

									if (temp.Where(t => !t.Contains(" ") && !t.Contains("")).ToList().Count > 0)
									{
										list[i].Description = surchargeData[0].Description;
									}
									list[i].Description = "";
								}
							}
						}
						item.CustomerMainC = customerMainC;
						item.CustomerSubC = customerSubC;
						var invoiceCustomerInfo = _customerService.GetCustomersByMainCodeSubCode(customerMainC, customerSubC);
						item.CustomerN = invoiceCustomerInfo.Customer.CustomerN;
						item.CustomerAddress = invoiceCustomerInfo.Customer.Address1;
						item.CustomerTaxCode = invoiceCustomerInfo.Customer.TaxCode;

						item.CustomerExpenseList = list;

						dataList.Add(item);
					}
				}
			}

			// update invoice status
			if (dataList.Count > 0)
			{
				for (var iloop = 0; iloop < dataList.Count; iloop++)
				{
					if (dataList[iloop].CustomerExpenseList != null && dataList[iloop].CustomerExpenseList.Count > 0)
					{
						var itemResult = new CustomerExpenseReportData();
						itemResult.CustomerMainC = dataList[iloop].CustomerMainC;
						itemResult.CustomerSubC = dataList[iloop].CustomerSubC;
						itemResult.CustomerN = dataList[iloop].CustomerN;
						itemResult.CustomerAddress = dataList[iloop].CustomerAddress;
						itemResult.CustomerTaxCode = dataList[iloop].CustomerTaxCode;
						//itemResult.ApplyD = dataList[iloop].ApplyD;
						//itemResult.SettlementD = dataList[iloop].SettlementD;
						//itemResult.TaxMethodI = dataList[iloop].TaxMethodI;
						//itemResult.TaxRate = dataList[iloop].TaxRate;
						//itemResult.TaxRoundingI = dataList[iloop].TaxRoundingI;
						//itemResult.RevenueRoundingI = dataList[iloop].RevenueRoundingI;
						itemResult.CustomerExpenseList = new List<CustomerExpenseItem>();

						for (var jloop = 0; jloop < dataList[iloop].CustomerExpenseList.Count; jloop++)
						{
							var orderD = dataList[iloop].CustomerExpenseList[jloop].OrderD.OrderD;
							var orderNo = dataList[iloop].CustomerExpenseList[jloop].OrderD.OrderNo;
							var detailNo = dataList[iloop].CustomerExpenseList[jloop].OrderD.DetailNo;

							var dispatchCount = _dispatchRepository.Query(dis => dis.OrderD == orderD &
																				  dis.OrderNo == orderNo &
																				  dis.DetailNo == detailNo
																			).ToList();

							if (dispatchCount.Count > 0)
							{
								itemResult.CustomerExpenseList.Add(dataList[iloop].CustomerExpenseList[jloop]);

								// transfer invoice status
								//for (var kloop = 0; kloop < dispatchCount.Count; kloop++)
								//{
								//	var dispatchUpdate = dispatchCount[kloop];
								//	dispatchUpdate.InvoiceStatus = "1";
								//	_dispatchRepository.Update(dispatchUpdate);
								//}
							}
						}

						result.Add(itemResult);
					}
				}
				SaveReport();
			}

			return result;
		}
		public Stream ExportPdfDriverAllowance(DriverAllowanceReportParam param, string userName)
		{
			Stream stream;
			DataRow row;
			DriverAllowanceList.DriverAllowanceDetailDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			var companyName = "";
			var companyAddress = "";
			var fileName = "";
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
			// get data
			dt = new DriverAllowanceList.DriverAllowanceDetailDataTable();
			List<DriverAllowanceListReport> data = GetDriverAllowanceList(param);
			data = data.OrderByDescending(x => x.DriverC).ThenByDescending(x => x.TransportD).ToList();

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
																 (con.TextKey == "TLTDRIVERALLOWANCE" ||
																 con.TextKey == "LBLDRIVERREPORT" ||
																 con.TextKey == "LBLTRANSPORTDREPORT" ||
																 con.TextKey == "LBLORDERENTRYNO" ||
																 con.TextKey == "LBLCUSTOMERREPORT" ||
																 con.TextKey == "LBLCONTAINERNO" ||
																 con.TextKey == "LBLLOCATIONREPORT" ||
																 con.TextKey == "LBLDRIVINGALLOWANCE" ||
																 con.TextKey == "LBLOTHALLOWANCE" ||
																 con.TextKey == "RPLBLTOTAL" ||
																 con.TextKey == "LBLTOTALREPORT" ||
																 con.TextKey == "LBLNETWEIGHTSHORT" ||

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

			if (data != null && data.Count > 0)
			{
				for (int i = 0; i < data.Count; i++)
				{
					int index = 1;
					row = dt.NewRow();
					row["No"] = index++;
					row["TransportD"] = Utilities.GetFormatDateReportByLanguage(data[i].TransportD, intLanguage);
					row["OrderNo"] = data[i].OrderNo;
					row["CustomerN"] = data[i].CustomerN;
					row["ContainerNo"] = data[i].ContainerNo;
					row["NetWeight"] = data[i].NetWeight > 0 ? data[i].NetWeight.ToString("#,###.#") : "";
					var location = "";
					if (data[i].Location1 != null && data[i].Location1 != "")
						location += data[i].Location1 + ", ";
					if (data[i].Location2 != null && data[i].Location2 != "")
						location += data[i].Location2 + ", ";
					if (data[i].Location3 != null && data[i].Location3 != "")
						location += data[i].Location3 + ", ";
					if (!String.IsNullOrWhiteSpace(location) && location.Length > 2)
						row["Location"] = location.Remove(location.Length - 2);

					row["DriverAllowance"] = data[i].DriverAllowance - data[i].Amount;
					row["OtherDriverAllowance"] = data[i].Amount;
					row["DriverC"] = data[i].DriverC;
					row["DriverN"] = data[i].DriverN;
					dt.Rows.Add(row);
				}

				//total = data.Sum(m => m.Amount);
				//var listDate = data.GroupBy(item => item.OrderD)
				//	.Select(grp => grp.First())
				//	.OrderBy(or=>or.OrderD)
				//	.ToList();

				//int index = 1;
				//	foreach (var currentDay in listDate)
				//	{
				//		var allowanceInDay = from item in data
				//							 where item.OrderD == currentDay.OrderD
				//							 select item;

				//		bool isFirst = true;
				//		foreach (var item in allowanceInDay)
				//		{
				//			row = dt.NewRow();

				//			if (isFirst)
				//			{
				//				row["No"] = index++;
				//				row["Day"] = Utilities.GetFormatDateReportByLanguage(item.OrderD, intLanguage);
				//				row["OrderD"] = Utilities.GetFormatDateReportByLanguage(item.OrderD, intLanguage);
				//				row["ExpenseN"] = item.ExpenseN == "TLTDRIVERALLOWANCE" ? dicLanguage["TLTDRIVERALLOWANCE"] : item.ExpenseN;
				//				row["Total"] = item.Amount.ToString("#,###", cul.NumberFormat);

				//				isFirst = false;
				//			}
				//			else
				//			{
				//				row["No"] = string.Empty;
				//				row["Day"] = string.Empty;
				//				row["OrderD"] = Utilities.GetFormatDateReportByLanguage(item.OrderD, intLanguage);
				//				row["ExpenseN"] = item.ExpenseN == "TLTDRIVERALLOWANCE" ? dicLanguage["TLTDRIVERALLOWANCE"] : item.ExpenseN;
				//				row["Total"] = item.Amount.ToString("#,###", cul.NumberFormat);
				//			}


				//			dt.Rows.Add(row);
				//		}
				//	}

			}
			stream = CrystalReport.Service.DriverAllowance.ExportPdf.Exec(dt, intLanguage, dicLanguage, param.AllowanceDFrom, param.AllowanceDTo,
																			companyName, companyAddress, fileName, user);
			return stream;
		}
		public Stream ExportPdfDriverSalary(DriverAllowanceReportParam param, string userName)
		{
			Stream stream;
			DataRow row;
			DriverAllowanceList.DriverAllowanceDetailDataTable dt;
			DriverAllowanceList.SettlementMoneyDataTable dt4;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			var companyName = "";
			var companyAddress = "";
			var fileName = "";
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
			// get data
			dt = new DriverAllowanceList.DriverAllowanceDetailDataTable();
			dt4 = new DriverAllowanceList.SettlementMoneyDataTable();

			DriverAllowanceListReport data = GetDriverSalaryList(param);
			//data = data.DriverAllowanceList.OrderByDescending(x => x.DriverC).ThenByDescending(x => x.TransportD).ToList();

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
																 (con.TextKey == "LBLSALARYDRIVER" ||
																 con.TextKey == "LBLDRIVERREPORT" ||
																 con.TextKey == "LBLTRANSPORTDREPORT" ||
																 con.TextKey == "LBLORDERENTRYNO" ||
																 con.TextKey == "LBLCUSTOMERREPORT" ||
																 con.TextKey == "LBLCONTAINERNO" ||
																 con.TextKey == "LBLLOCATIONREPORT" ||
																 con.TextKey == "LBLDRIVINGALLOWANCE" ||
																 con.TextKey == "LBLOTHALLOWANCE" ||
																 con.TextKey == "RPLBLTOTAL" ||
																 con.TextKey == "LBLTOTALREPORT" ||
																 con.TextKey == "LBLNETWEIGHTSHORT" ||

																  con.TextKey == "RPFTPRINTTIME" ||
																  con.TextKey == "RPFTPRINTBY" ||
																  con.TextKey == "RPFTPAGE" ||
																  con.TextKey == "RPFTCREATOR" ||
																  con.TextKey == "LBLTURNOVERCALCULATEDALLOWANCE" ||
																  con.TextKey == "LBLTURNOVERRATE" ||
																  con.TextKey == "LBLADVANCEMONEYREMAINING" ||
																  con.TextKey == "LBLOTHERMONEY" ||
																  con.TextKey == "LBLREALMONEY" ||
																  con.TextKey == "LBLSETTLEMENTMONEY" ||
																  con.TextKey == "LBLDATEFULL" ||
																  con.TextKey == "LBLADVANCELIABILITIES" ||
																  con.TextKey == "LBLEXPENSELIABILITIES" ||
																  con.TextKey == "LBLCONTENTCALCULATE" ||
																  con.TextKey == "LBLBASICSALARY"
																  )).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			var checkHaveFreight = 0;
			if (data != null && data.DriverAllowanceList.Count > 0)
			{
				for (int i = 0; i < data.DriverAllowanceList.Count; i++)
				{
					int index = 1;
					row = dt.NewRow();
					row["No"] = index++;
					row["TransportD"] = Utilities.GetFormatDateReportByLanguage(data.DriverAllowanceList[i].TransportD, intLanguage);
					row["OrderNo"] = data.DriverAllowanceList[i].OrderNo;
					row["CustomerN"] = data.DriverAllowanceList[i].CustomerN;
					row["ContainerNo"] = data.DriverAllowanceList[i].ContainerNo;
					row["NetWeight"] = data.DriverAllowanceList[i].NetWeight > 0 ? data.DriverAllowanceList[i].NetWeight.ToString("#,###.#") : "";
					var location = "";
					if (data.DriverAllowanceList[i].Location1 != null && data.DriverAllowanceList[i].Location1 != "")
						location += data.DriverAllowanceList[i].Location1 + ", ";
					if (data.DriverAllowanceList[i].Location2 != null && data.DriverAllowanceList[i].Location2 != "")
						location += data.DriverAllowanceList[i].Location2 + ", ";
					if (data.DriverAllowanceList[i].Location3 != null && data.DriverAllowanceList[i].Location3 != "")
						location += data.DriverAllowanceList[i].Location3 + ", ";
					if (!String.IsNullOrWhiteSpace(location) && location.Length > 2)
						row["Location"] = location.Remove(location.Length - 2);

					row["DriverAllowance"] = data.DriverAllowanceList[i].DriverAllowance - data.DriverAllowanceList[i].Amount;
					row["OtherDriverAllowance"] = data.DriverAllowanceList[i].Amount;
					row["DriverC"] = data.DriverAllowanceList[i].DriverC;
					row["DriverN"] = data.DriverAllowanceList[i].DriverN;
					row["AllowanceOfDriver"] = data.DriverAllowanceList[i].AllowanceOfDriver;
					row["BasicSalary"] = data.DriverAllowanceList[i].BasicSalary;
					decimal totalAdvance = 0;
					decimal totalOther = 0;
					string dC = data.DriverAllowanceList[i].DriverC;
					var listLia = data.LiabilitiesDetailList.Where(p => p.DriverC == dC).ToList();
					for (int x = 0; x < listLia.Count; x++)
					{
						if (listLia[x].LiabilitiesI == "")
						{
							totalOther = totalOther + listLia[x].Amount ?? 0;
						}
						else if (listLia[x].LiabilitiesI == "0")
						{
							totalAdvance = totalAdvance + listLia[x].Amount ?? 0;
						}
						else if (listLia[x].LiabilitiesI == "1")
						{
							totalAdvance = totalAdvance - listLia[x].Amount ?? 0;
						}
					}
					decimal totalSalary = 0;
					var listDriverAllowance = data.DriverAllowanceList.Where(p => p.DriverC == dC).ToList();
					for (int x = 0; x < listDriverAllowance.Count; x++)
					{
						totalSalary = totalSalary + listDriverAllowance[x].Amount +
									  (listDriverAllowance[x].DriverAllowance - listDriverAllowance[x].Amount);
					}
					row["TotalAdvanceMoneyRemaining"] = -totalAdvance;
					row["TotalOtherMoney"] = totalOther;
					row["TotalSalary"] = totalSalary;
					//set UnitPriceRate
					DateTime transD = data.DriverAllowanceList[i].TransportD;
					var checkMethod = _driverAllowanceRepository.Query(p => DateTime.Compare(transD, p.ApplyD) >= 0).ToList();
					if (checkMethod.Count > 0)
					{
						var maxD = checkMethod.Max(p => p.ApplyD);
						var selectR = _driverAllowanceRepository.Query(p => p.ApplyD == maxD).FirstOrDefault();
						if (selectR != null)
						{
							if (selectR.UnitPriceMethodI == "1")
							{
								row["UnitPriceRate"] = selectR.UnitPriceRate ?? 0;
								checkHaveFreight = 1;
							}
							else
							{
								row["UnitPriceRate"] = 0;
							}
						}
					}
					dt.Rows.Add(row);
				}
				for (int x = 0; x < data.LiabilitiesDetailList.Count; x++)
				{
					int index = 1;
					row = dt4.NewRow();
					row["No"] = index++;
					row["DriverC"] = data.LiabilitiesDetailList[x].DriverC;
					row["SettlementD"] = Utilities.GetFormatDateReportByLanguage(data.LiabilitiesDetailList[x].LiabilitiesD, intLanguage);
					row["Description"] = data.LiabilitiesDetailList[x].Description;
					if (data.LiabilitiesDetailList[x].LiabilitiesI == "")
					{
						row["Amount"] = data.LiabilitiesDetailList[x].Amount;
						row["LiabilitiesAmount"] = 0;
						row["PaymentAmount"] = 0;
					}
					else if(data.LiabilitiesDetailList[x].LiabilitiesI == "0")
					{
						row["LiabilitiesAmount"] = data.LiabilitiesDetailList[x].Amount;
						row["PaymentAmount"] = 0;
						row["Amount"] = 0;
					}
					else
					{
						row["PaymentAmount"] = data.LiabilitiesDetailList[x].Amount;
						row["Amount"] = 0;
						row["LiabilitiesAmount"] = 0;
					}
					dt4.Rows.Add(row);
				}
			}

			if (checkHaveFreight != 1)
			{
				stream = CrystalReport.Service.DriverAllowance.ExportPdf.ExecSalaryNoFreight(dt, dt4, intLanguage, dicLanguage,
					param.AllowanceDFrom, param.AllowanceDTo,
					companyName, companyAddress, fileName, user);
			}
			else
			{
				stream = CrystalReport.Service.DriverAllowance.ExportPdf.ExecSalary(dt, dt4, intLanguage, dicLanguage,
					param.AllowanceDFrom, param.AllowanceDTo,
					companyName, companyAddress, fileName, user);
			}
			return stream;
		}
		public List<DriverAllowanceListReport> GetDriverAllowanceList(DriverAllowanceReportParam param)
		{
			List<DriverAllowanceListReport> result = new List<DriverAllowanceListReport>();
			// get data from Order_D and Order_H
			var list = from d in _dispatchRepository.GetAllQueryable()
					   join b in _allowanceDetailRepository.GetAllQueryable() on new { d.OrderD, d.OrderNo, d.DetailNo, d.DispatchNo }
							   equals new { b.OrderD, b.OrderNo, b.DetailNo, b.DispatchNo } into t1
					   from b in t1.DefaultIfEmpty()
					   //join c in _expenseRepository.GetAllQueryable() on b.AllowanceC
					   //		equals c.ExpenseC into t2
					   //		from c in t2.DefaultIfEmpty()
					   join dr in _driverRepository.GetAllQueryable() on d.DriverC equals dr.DriverC into t3
					   from dr in t3.DefaultIfEmpty()
					   where ((param.AllowanceDFrom == null || d.TransportD >= param.AllowanceDFrom) &&
							  (param.AllowanceDTo == null || d.TransportD <= param.AllowanceDTo) &&
							  (d.DispatchI == "0") &&
						   //(param.AllowanceType == "null" || (param.AllowanceType).Contains(c.ExpenseC)) &
							  (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || dr.DepC == param.DepC)
						   //(c.CategoryI == "3")
							 )
					   select new DriverAllowanceListReport()
					   {
						   OrderD = d.OrderD,
						   OrderNo = d.OrderNo,
						   DetailNo = d.DetailNo,
						   DispatchNo = d.DispatchNo,
						   DriverAllowance = d.DriverAllowance ?? 0,
						   Amount = b.Amount ?? 0
					   };

			var groupList = (from a in list
							 //where o.Amount > 0
							 group a by new { a.OrderD, a.OrderNo, a.DetailNo, a.DispatchNo }
								 into g
								 select new DriverAllowanceListReport()
								 {
									 OrderD = g.Key.OrderD,
									 OrderNo = g.Key.OrderNo,
									 DetailNo = g.Key.DetailNo,
									 DispatchNo = g.Key.DispatchNo,
									 DriverAllowance = g.Sum(x => x.DriverAllowance),
									 Amount = g.Sum(x => x.Amount)
								 }).ToList();

			if (groupList.Count > 0)
			{
				for (int i = 0; i < groupList.Count; i++)
				{
					var orderD = groupList[i].OrderD;
					var orderNo = groupList[i].OrderNo;
					var detailNo = groupList[i].DetailNo;
					var dispatchNo = groupList[i].DispatchNo;
					var driverAllowance = groupList[i].DriverAllowance;
					var amount = groupList[i].Amount;
					//if (driverAllowance > 0 && amount > 0 )
					var allowance = (from h in _orderHRepository.GetAllQueryable()
									 join o in _orderDRepository.GetAllQueryable() on new { h.OrderD, h.OrderNo } equals new { o.OrderD, o.OrderNo }
									 join d in _dispatchRepository.GetAllQueryable() on new { o.OrderD, o.OrderNo, o.DetailNo } equals new { d.OrderD, d.OrderNo, d.DetailNo }
									 join c in _customerRepository.GetAllQueryable() on new { h.CustomerMainC, h.CustomerSubC } equals new { c.CustomerMainC, c.CustomerSubC }
									 join dr in _driverRepository.GetAllQueryable() on d.DriverC equals dr.DriverC
									 where (d.OrderD == orderD && d.OrderNo == orderNo &&
										 d.DetailNo == detailNo && d.DispatchNo == dispatchNo
										 )
									 select new DriverAllowanceListReport()
									 {
										 OrderD = d.OrderD,
										 OrderNo = d.OrderNo,
										 DetailNo = d.DetailNo,
										 DispatchNo = d.DispatchNo,
										 TransportD = d.TransportD ?? d.OrderD,
										 CustomerN = c.CustomerShortN != "" ? c.CustomerShortN : c.CustomerN,
										 ContainerNo = o.ContainerNo,
										 Location1 = d.Location1N,
										 Location2 = d.Location2N,
										 Location3 = d.Location3N,
										 DriverAllowance = driverAllowance,
										 Amount = amount,
										 DriverC = dr.DriverC,
										 DriverN = dr.LastN + " " + dr.FirstN,
										 NetWeight = o.NetWeight ?? 0
									 }).FirstOrDefault();
					if (allowance != null)
						result.Add(allowance);
				}
			}

			//result = list.ToList();
			//var driverAllowance = from d in _dispatchRepository.GetAllQueryable()
			//					  where ((param.AllowanceDFrom == null || d.TransportD >= param.AllowanceDFrom) &
			//							 (param.AllowanceDTo == null || d.TransportD <= param.AllowanceDTo) &
			//							 d.TransportD != null &
			//							 d.DriverAllowance != null
			//							 )
			//					  group d by new { d.TransportD }
			//						  into g
			//						  select new
			//						  {
			//							  TransportD = g.Key.TransportD,
			//							  Amount = g.Sum(x => x.DriverAllowance)
			//						  };
			//foreach (var d in driverAllowance)
			//{
			//	var amount = d.Amount ?? 0;

			//	if (order.Any(x => x.OrderD == d.TransportD))
			//	{
			//		amount = amount - (order.Where(x => x.OrderD == d.TransportD).Sum(x => x.Amount));
			//	}
			//	if (amount > 0)
			//	{
			//		result.Insert(0, new DriverAllowanceListReport()
			//		{
			//			OrderD = d.TransportD ?? DateTime.Now,
			//			ExpenseN = "TLTDRIVERALLOWANCE",
			//			Amount = amount
			//		});
			//	}
			//}
			return result;
		}

		public DriverAllowanceListReport GetDriverSalaryList(DriverAllowanceReportParam param)
		{
			List<DriverAllowanceReport> result = new List<DriverAllowanceReport>();
			List<LiabilitiesViewModel> liabilitiesDetail = new List<LiabilitiesViewModel>();

			// get data from Order_D and Order_H
			var list = from d in _dispatchRepository.GetAllQueryable()
					   join b in _allowanceDetailRepository.GetAllQueryable() on new { d.OrderD, d.OrderNo, d.DetailNo, d.DispatchNo }
							   equals new { b.OrderD, b.OrderNo, b.DetailNo, b.DispatchNo } into t1
					   from b in t1.DefaultIfEmpty()
					   //join c in _expenseRepository.GetAllQueryable() on b.AllowanceC
					   //		equals c.ExpenseC into t2
					   //		from c in t2.DefaultIfEmpty()
					   join dr in _driverRepository.GetAllQueryable() on d.DriverC equals dr.DriverC into t3
					   from dr in t3.DefaultIfEmpty()
					   where ((param.AllowanceDFrom == null || d.TransportD >= param.AllowanceDFrom) &&
							  (param.AllowanceDTo == null || d.TransportD <= param.AllowanceDTo) &&
							  (d.DispatchI == "0") &&
						   //(param.AllowanceType == "null" || (param.AllowanceType).Contains(c.ExpenseC)) &
							  (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || dr.DepC == param.DepC)
						   //(c.CategoryI == "3")
							 )
					   select new DriverAllowanceReport()
					   {
						   OrderD = d.OrderD,
						   OrderNo = d.OrderNo,
						   DetailNo = d.DetailNo,
						   DispatchNo = d.DispatchNo,
						   DriverAllowance = d.DriverAllowance ?? 0,
						   Amount = b.Amount ?? 0
					   };

			var groupList = (from a in list
							 //where o.Amount > 0
							 group a by new { a.OrderD, a.OrderNo, a.DetailNo, a.DispatchNo }
								 into g
								 select new DriverAllowanceReport()
								 {
									 OrderD = g.Key.OrderD,
									 OrderNo = g.Key.OrderNo,
									 DetailNo = g.Key.DetailNo,
									 DispatchNo = g.Key.DispatchNo,
									 DriverAllowance = g.Sum(x => x.DriverAllowance),
									 Amount = g.Sum(x => x.Amount)
								 }).ToList();

			if (groupList.Count > 0)
			{
				for (int i = 0; i < groupList.Count; i++)
				{
					var orderD = groupList[i].OrderD;
					var orderNo = groupList[i].OrderNo;
					var detailNo = groupList[i].DetailNo;
					var dispatchNo = groupList[i].DispatchNo;
					var driverAllowance = groupList[i].DriverAllowance;
					var amount = groupList[i].Amount;
					//if (driverAllowance > 0 && amount > 0 )
					var allowance = (from h in _orderHRepository.GetAllQueryable()
									 join o in _orderDRepository.GetAllQueryable() on new { h.OrderD, h.OrderNo } equals new { o.OrderD, o.OrderNo }
									 join d in _dispatchRepository.GetAllQueryable() on new { o.OrderD, o.OrderNo, o.DetailNo } equals new { d.OrderD, d.OrderNo, d.DetailNo }
									 join c in _customerRepository.GetAllQueryable() on new { h.CustomerMainC, h.CustomerSubC } equals new { c.CustomerMainC, c.CustomerSubC }
									 join dr in _driverRepository.GetAllQueryable() on d.DriverC equals dr.DriverC
									 where (d.OrderD == orderD && d.OrderNo == orderNo &&
										 d.DetailNo == detailNo && d.DispatchNo == dispatchNo
										 )
									 select new DriverAllowanceReport()
									 {
										 OrderD = d.OrderD,
										 OrderNo = d.OrderNo,
										 DetailNo = d.DetailNo,
										 DispatchNo = d.DispatchNo,
										 TransportD = d.TransportD ?? d.OrderD,
										 CustomerN = c.CustomerShortN != "" ? c.CustomerShortN : c.CustomerN,
										 ContainerNo = o.ContainerNo,
										 Location1 = d.Location1N,
										 Location2 = d.Location2N,
										 Location3 = d.Location3N,
										 DriverAllowance = driverAllowance,
										 Amount = amount,
										 DriverC = dr.DriverC,
										 DriverN = dr.LastN + " " + dr.FirstN,
										 NetWeight = o.NetWeight ?? 0,
										 AllowanceOfDriver = d.AllowanceOfDriver ?? 0,
										 BasicSalary = dr.BasicSalary ?? 0
									 }).FirstOrDefault();
					if (allowance != null)
						result.Add(allowance);
				}
			}
			var groupResult = (from a in result
							 //where o.Amount > 0
							   group a by new { a.DriverC }
								 into g
								 select new DriverAllowanceReport()
								 {
									 DriverC = g.Key.DriverC,
								 }).ToList();
			result = result.OrderByDescending(x => x.DriverC).ThenByDescending(x => x.TransportD).ToList();

			if (groupResult.Count > 0)
			{
				for (var iloop = 0; iloop < groupResult.Count; iloop++)
				{
					string driC = groupResult[iloop].DriverC;
					var liabilites = (from l in _liabilitiesRepository.GetAllQueryable()
						where
							(l.DriverC == driC &&
							 (param.AllowanceDFrom == null || l.LiabilitiesD >= param.AllowanceDFrom) &&
							 (param.AllowanceDTo == null || l.LiabilitiesD <= param.AllowanceDTo))
						select new LiabilitiesViewModel()
						{
							DriverC = l.DriverC,
							LiabilitiesD = l.LiabilitiesD,
							Amount = l.Amount ?? 0,
							Description = l.Description ?? "",
							LiabilitiesI = l.LiabilitiesI
						});
					var liabilitesgroup = (from l in liabilites.AsEnumerable()
										  group l by new {
												l.Description,
												l.LiabilitiesI
											}
											into g
											  select new LiabilitiesViewModel()
											  {
												  DriverC = g.First().DriverC,
												  LiabilitiesD = g.First().LiabilitiesD,
												  Amount = g.Sum(s => s.Amount),
												  Description = g.First().Description,
												  LiabilitiesI = g.First().LiabilitiesI
											  }).ToList();
					for (int c = 0; c < liabilitesgroup.Count; c++)
					{
						liabilitiesDetail.Add(new LiabilitiesViewModel()
						{
							DriverC = liabilitesgroup[c].DriverC,
							LiabilitiesD = liabilitesgroup[c].LiabilitiesD,
							Amount = liabilitesgroup[c].Amount ?? 0,
							Description = liabilitesgroup[c].Description ?? "",
							LiabilitiesI = liabilitesgroup[c].LiabilitiesI
						});
					}
					var calculate = (from l in _calculateDriverAllowanceRepository.GetAllQueryable()
									 where (l.DriverC == driC &&
									 (param.AllowanceDFrom == null || l.ApplyD >= param.AllowanceDFrom) &&
									 (param.AllowanceDTo == null || l.ApplyD <= param.AllowanceDTo))
									 select new CalculateDriverAllowanceViewModel()
									 {
										 DriverC = l.DriverC,
										 ApplyD = l.ApplyD,
										 AmountMoney = l.AmountMoney ?? 0,
										 AmountMoneySubtract = l.AmountMoneySubtract ?? 0,
										 Description = l.Description ?? "",
										 Content = l.Content ?? ""
									 }).ToList();
					for (int c = 0; c < calculate.Count; c++)
					{
						liabilitiesDetail.Add(new LiabilitiesViewModel()
						{
							DriverC = calculate[c].DriverC,
							LiabilitiesD = (DateTime) calculate[c].ApplyD,
							Amount =
								(calculate[c].AmountMoney != 0 && calculate[c].AmountMoneySubtract == 0)
									? calculate[c].AmountMoney
									: ((calculate[c].AmountMoney == 0 && calculate[c].AmountMoneySubtract != 0)
										? -calculate[c].AmountMoneySubtract
										: 0),
							Description = calculate[c].Content ?? "",
							LiabilitiesI = ""
						});
					}
				}
			}
			return new DriverAllowanceListReport()
			{
				DriverAllowanceList = result,
				LiabilitiesDetailList = liabilitiesDetail.ToList()
			};
		}

		public Stream ExportPdfOrderGeneral(OrderReportParam param, string userName)
		{
			Stream stream;
			DataRow row;
			CrystalReport.Dataset.Order.OrderList.OrderGeneralDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;

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
			var user = (from a in _userRepository.GetAllQueryable()
						join c in _employeeRepository.GetAllQueryable() on a.EmployeeC equals c.EmployeeC into t2
						from c in t2.DefaultIfEmpty()
						where (a.UserName == userName)
						select new
						{
							EmployeeN = c != null ? c.EmployeeLastN + " " + c.EmployeeFirstN : ""
						}).ToList().FirstOrDefault().EmployeeN;

			// get data
			dt = new CrystalReport.Dataset.Order.OrderList.OrderGeneralDataTable();
			List<OrderListReport> data = GetOrderGeneralList(param);
			CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
			if (param.Language == "vi")
			{
				intLanguage = 1;
			}
			else if (param.Language == "jp")
			{
				intLanguage = 3;
				cul = CultureInfo.GetCultureInfo("ja-JP");
			}
			else
			{
				intLanguage = 2;
				cul = CultureInfo.GetCultureInfo("en-US");
			}

			dicLanguage = new Dictionary<string, string>();
			var languages = _textResourceRepository.Query(con => (con.LanguageID == intLanguage) &&
																  (con.TextKey == "TLTORDERREPORT" ||
																   con.TextKey == "LBLORDERNODISPATCH" ||
																   con.TextKey == "LBLORDERTYPEREPORT" ||
																   con.TextKey == "LBLORDERDATEDISPATCH" ||
																   con.TextKey == "LBLCUSTOMERREPORT" ||
																   con.TextKey == "LBLBLBKREPORT" ||
																   con.TextKey == "LBLJOBNO" ||
																   con.TextKey == "LBLSHIPPINGAGENCY" ||
																   con.TextKey == "MNUVESSEL" ||
																   con.TextKey == "TLTROUTE" ||
																   con.TextKey == "RPHDCONTSIZE" ||
																   con.TextKey == "LBLNETWEIGHT" ||
																   con.TextKey == "LBLTON" ||
																   con.TextKey == "LBLTEUNITPRICE" ||
																   con.TextKey == "RPLBLTOTAL" ||
																   con.TextKey == "LBLTOL" ||
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

			if (data != null && data.Count > 0)
			{
				for (int iloop = 0; iloop < data.Count; iloop++)
				{
					row = dt.NewRow();
					row["OrderD"] = data[iloop].OrderD.ToString("dd/MM/yyyy");
					row["OrderNo"] = data[iloop].OrderNo + "<br>" +
						Utilities.GetOrderTypeName(data[iloop].OrderTypeI);
					row["CustomerShortN"] = data[iloop].CustomerShortN;
					row["BLBK"] = data[iloop].BLBK + "<br>" +
						data[iloop].JobNo;
					row["ShippingCompanyN"] = data[iloop].ShippingCompanyN + "<br>" +
						data[iloop].VesselN;

					DateTime oD = data[iloop].OrderD;
					string oNo = data[iloop].OrderNo;
					decimal? tolP = data[iloop].TotalPrice;
					decimal? uP = data[iloop].UnitPrice;
                    var route = "";
                    if (!String.IsNullOrWhiteSpace(data[iloop].LoadingPlaceN))
                        route += data[iloop].LoadingPlaceN + ", ";
                    if (!String.IsNullOrWhiteSpace(data[iloop].StopoverPlaceN))
                        route += data[iloop].StopoverPlaceN + ", ";
                    if (!String.IsNullOrWhiteSpace(data[iloop].DischargePlaceN))
                        route += data[iloop].DischargePlaceN;
                    if (string.IsNullOrEmpty(route))
                    {
                        var lord =_orderDRepository.Query(
							p =>
								p.OrderD == oD && p.OrderNo == oNo && p.TotalPrice == tolP &&
								p.UnitPrice == uP).FirstOrDefault();
                        if (lord != null)
                        {
                            if (!String.IsNullOrWhiteSpace(lord.LocationDispatch1))
                                route += lord.LocationDispatch1 + ", ";
                            if (!String.IsNullOrWhiteSpace(lord.LocationDispatch2))
                                route += lord.LocationDispatch2 + ", ";
                            if (!String.IsNullOrWhiteSpace(lord.LocationDispatch3))
                                route += lord.LocationDispatch3;
                        }
                    }

				    row["Route"] = route;

					row["Quantity20"] = data[iloop].Quantity20HC;
					row["Quantity40"] = data[iloop].Quantity40HC;
					row["Quantity45"] = data[iloop].Quantity45HC;
					row["NetWeight"] = data[iloop].TotalLoads;
					row["UnitPrice"] = data[iloop].UnitPrice;
					row["TotalPrice"] = data[iloop].TotalPrice;
					dt.Rows.Add(row);
				}
			}

			// set fromDate and toDate
			var fromDate = "";
			if (param.OrderDFrom != null)
			{
				if (intLanguage == 1)
				{
					fromDate = ((DateTime)param.OrderDFrom).ToString("dd/MM/yyyy");
				}
				else if (intLanguage == 2)
				{
					fromDate = ((DateTime)param.OrderDFrom).ToString("MM/dd/yyyy");
				}
				else if (intLanguage == 3)
				{
					var date = (DateTime)param.OrderDFrom;
					fromDate = date.Year + "年" + date.Month + "月" + date.Day + "日";
				}
			}
			var toDate = "";
			if (param.OrderDTo != null)
			{
				if (intLanguage == 1)
				{
					toDate = ((DateTime)param.OrderDTo).ToString("dd/MM/yyyy");
				}
				else if (intLanguage == 2)
				{
					toDate = ((DateTime)param.OrderDTo).ToString("MM/dd/yyyy");
				}
				else if (intLanguage == 3)
				{
					var date = (DateTime)param.OrderDTo;
					toDate = date.Year + "年" + ("0" + date.Month).Substring(("0" + date.Month).Length - 2) + "月" + date.Day + "日";
				}
			}

			stream = CrystalReport.Service.Order.ExportPdf.Exec(dt,
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

		public Stream ExportExcelOrderGeneral(OrderReportParam param, string userName)
		{
			Stream stream;
			DataRow row;
			OrderList.OrderGeneralDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			var companyName = "";
			var companyAddress = "";
			var companyTaxCode = "";
			var fileName = "";

			// get data
			dt = new OrderList.OrderGeneralDataTable();
			List<OrderDetailListReport> data = GetOrderGeneralDetailList(param);
			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				companyTaxCode = basicSetting.TaxCode;
				fileName = basicSetting.Logo;
			}
			//ger employeeName
			var user = GetEmployeeByUserName(userName);


			// get language for report
			dicLanguage = new Dictionary<string, string>();
			CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
			if (param.Language == "vi")
			{
				intLanguage = 1;
			}
			else if (param.Language == "jp")
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
																 (
																  con.TextKey == "TLTORDERREPORT" ||
																  con.TextKey == "LBLINVOICEDATE" ||
																  con.TextKey == "LBLTEQUANTITY" ||
																  con.TextKey == "LBLUNITPRICE" ||
																  con.TextKey == "RPLBLTOTAL" ||
																  con.TextKey == "RPFTCREATOR" ||
																  con.TextKey == "RPFTPRINTBY" ||
																  con.TextKey == "RPFTPRINTTIME" ||
																  con.TextKey == "RPHDORDERNO" ||
																  con.TextKey == "RPHDORDERTYPE" ||
																  con.TextKey == "LBLORDERDATEDISPATCH" ||
																  con.TextKey == "LBLCUSTOMERREPORT" ||
																  con.TextKey == "LBLBLBKREPORT" ||
																  con.TextKey == "LBLJOBNO" ||
																  con.TextKey == "MNUSHIPPINGCOMPANY" ||
																  con.TextKey == "MNUVESSEL" ||
																  con.TextKey == "TLTROUTE" ||
																  con.TextKey == "LBLCONTSIZE" ||
																  con.TextKey == "LBLNETWEIGHT" ||
																  con.TextKey == "RPHDUNITPRICE" ||
																  con.TextKey == "LBLTOL"
																  )).ToList();
			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}
			var conV = (from list in _orderDRepository.GetAllQueryable()
						select new ContainerViewModel()
						{
							OrderD = list.OrderD,
							OrderNo = list.OrderNo,
							TotalPrice = list.TotalPrice,
							UnitPrice = list.UnitPrice,
							LocationDispatch1 = list.LocationDispatch1,
							LocationDispatch2 = list.LocationDispatch2,
							LocationDispatch3 = list.LocationDispatch3
						}).ToList();
			return CrystalReport.Service.Order.ExportExcel.ExportOrderListToExcel(conV, data, dicLanguage, intLanguage, companyName, companyAddress, fileName, user, param.OrderDFrom, param.OrderDTo);
		}

		public List<OrderListReport> GetOrderGeneralList(OrderReportParam param)
		{
			List<OrderListReport> result = new List<OrderListReport>();

			var order = from a in _orderHRepository.GetAllQueryable()
						join b in _customerRepository.GetAllQueryable() on new { a.CustomerMainC, a.CustomerSubC }
							equals new { b.CustomerMainC, b.CustomerSubC } into t2
						from b in t2.DefaultIfEmpty()
						join o in _orderDRepository.GetAllQueryable()
						   on new { a.OrderD, a.OrderNo } equals new { o.OrderD, o.OrderNo }
						where ((param.OrderDFrom == null || a.OrderD >= param.OrderDFrom) &
							   (param.OrderDTo == null || a.OrderD <= param.OrderDTo) &
							   (param.Customer == "null" || (param.Customer).Contains(a.CustomerMainC + "_" + a.CustomerSubC)) &
							//(string.IsNullOrEmpty(param.EntryClerkC) || param.EntryClerkC == "null" || a.EntryClerkC == param.EntryClerkC) &
							//(string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || a.OrderDepC == param.DepC) &
							   (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || a.OrderTypeI == param.OrderTypeI) &
							   (param.JobNo == "null" || a.JobNo == param.JobNo)
							  )
						select new OrderListReport()
						{
							OrderD = a.OrderD,
							OrderNo = a.OrderNo,
							CustomerShortN = b != null ? (b.CustomerShortN != "" ? b.CustomerShortN : "") : "",
							OrderTypeI = a.OrderTypeI,
							BLBK = a.BLBK,
							ShippingCompanyN = a.ShippingCompanyN,
							VesselN = a.VesselN,
							LoadingPlaceN = a.LoadingPlaceN,
							StopoverPlaceN = a.StopoverPlaceN,
							DischargePlaceN = a.DischargePlaceN,
							JobNo = a.JobNo,
							ContainerSizeI = o.ContainerSizeI,
							NetWeight = o.NetWeight,
							UnitPrice = o.UnitPrice,
							TotalPrice = o.TotalPrice,
						};

			var group = from o in order.AsEnumerable()
						group o by new
						{
							o.OrderD,
							o.OrderNo,
							o.ContainerSizeI,
							o.UnitPrice,
							o.CustomerShortN,
							o.OrderTypeI,
							o.BLBK,
							o.ShippingCompanyN,
							o.VesselN,
							o.LoadingPlaceN,
							o.StopoverPlaceN,
							o.DischargePlaceN,
							o.JobNo
						} into g
						select new OrderListReport()
						{
							OrderD = g.Key.OrderD,
							OrderNo = g.Key.OrderNo,
							ContainerSizeI = g.Key.ContainerSizeI,
							UnitPrice = g.Key.UnitPrice,
							CustomerShortN = g.Key.CustomerShortN,
							OrderTypeI = g.Key.OrderTypeI,
							BLBK = g.Key.BLBK,
							ShippingCompanyN = g.Key.ShippingCompanyN,
							VesselN = g.Key.VesselN,
							LoadingPlaceN = g.Key.LoadingPlaceN,
							StopoverPlaceN = g.Key.StopoverPlaceN,
							DischargePlaceN = g.Key.DischargePlaceN,
							JobNo = g.Key.JobNo,
							Quantity20HC = g.Key.ContainerSizeI == "0" ? g.Count() : 0,
							Quantity40HC = g.Key.ContainerSizeI == "1" ? g.Count() : 0,
							Quantity45HC = g.Key.ContainerSizeI == "2" ? g.Count() : 0,
							TotalLoads = g.Key.ContainerSizeI == "3" ? g.Sum(i => i.NetWeight) : 0,
							TotalPrice = g.Sum(i => i.TotalPrice)
						};

			if (param.SortBy.Equals("C"))
			{
				group = group.OrderBy("CustomerShortN asc, OrderD asc, OrderNo asc");
			}
			else // default sort by Order
			{
				group = group.OrderBy("OrderD asc, OrderNo asc");
			}
			result = group.ToList();

			return result;
		}

		public List<OrderDetailListReport> GetOrderGeneralDetailList(OrderReportParam param)
		{
			List<OrderDetailListReport> result = new List<OrderDetailListReport>();

			var order = from a in _orderHRepository.GetAllQueryable()
						join b in _customerRepository.GetAllQueryable() on new { a.CustomerMainC, a.CustomerSubC }
							equals new { b.CustomerMainC, b.CustomerSubC } into t2
						from b in t2.DefaultIfEmpty()
						join o in _orderDRepository.GetAllQueryable()
						   on new { a.OrderD, a.OrderNo } equals new { o.OrderD, o.OrderNo }
						where ((param.OrderDFrom == null || a.OrderD >= param.OrderDFrom) &
							   (param.OrderDTo == null || a.OrderD <= param.OrderDTo) &
							   (param.Customer == "null" || (param.Customer).Contains(a.CustomerMainC + "_" + a.CustomerSubC)) &
							//(string.IsNullOrEmpty(param.EntryClerkC) || param.EntryClerkC == "null" || a.EntryClerkC == param.EntryClerkC) &
							//(string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || a.OrderDepC == param.DepC) &
							   (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || a.OrderTypeI == param.OrderTypeI) &
							   (param.JobNo == "null" || a.JobNo == param.JobNo)
							  )
						select new OrderDetailListReport()
						{
							OrderD = a.OrderD,
							OrderNo = a.OrderNo,
							CustomerShortN = b != null ? (b.CustomerShortN != "" ? b.CustomerShortN : "") : "",
							OrderTypeI = a.OrderTypeI,
							BLBK = a.BLBK,
							ShippingCompanyN = a.ShippingCompanyN,
							VesselN = a.VesselN,
							LoadingPlaceN = a.LoadingPlaceN,
							StopoverPlaceN = a.StopoverPlaceN,
							DischargePlaceN = a.DischargePlaceN,
							JobNo = a.JobNo,
							ContainerSizeI = o.ContainerSizeI,
							NetWeight = o.NetWeight,
							UnitPrice = o.UnitPrice,
							TotalPrice = o.TotalPrice,
						};

			var group = from o in order.AsEnumerable()
						group o by new
						{
							o.OrderD,
							o.OrderNo,
							o.ContainerSizeI,
							o.UnitPrice,
							o.CustomerShortN,
							o.OrderTypeI,
							o.BLBK,
							o.ShippingCompanyN,
							o.VesselN,
							o.LoadingPlaceN,
							o.StopoverPlaceN,
							o.DischargePlaceN,
							o.JobNo
						} into g
						select new OrderDetailListReport()
						{
							OrderD = g.Key.OrderD,
							OrderNo = g.Key.OrderNo,
							ContainerSizeI = g.Key.ContainerSizeI,
							UnitPrice = g.Key.UnitPrice,
							CustomerShortN = g.Key.CustomerShortN,
							OrderTypeI = g.Key.OrderTypeI,
							BLBK = g.Key.BLBK,
							ShippingCompanyN = g.Key.ShippingCompanyN,
							VesselN = g.Key.VesselN,
							LoadingPlaceN = g.Key.LoadingPlaceN,
							StopoverPlaceN = g.Key.StopoverPlaceN,
							DischargePlaceN = g.Key.DischargePlaceN,
							JobNo = g.Key.JobNo,
							Quantity20HC = g.Key.ContainerSizeI == "0" ? g.Count() : 0,
							Quantity40HC = g.Key.ContainerSizeI == "1" ? g.Count() : 0,
							Quantity45HC = g.Key.ContainerSizeI == "2" ? g.Count() : 0,
							TotalLoads = g.Key.ContainerSizeI == "3" ? g.Sum(i => i.NetWeight) : 0,
							TotalPrice = g.Sum(i => i.TotalPrice)
						};

			if (param.SortBy.Equals("C"))
			{
				group = group.OrderBy("CustomerShortN asc, OrderD asc, OrderNo asc");
			}
			else // default sort by Order
			{
				group = group.OrderBy("OrderD asc, OrderNo asc");
			}
			result = group.ToList();

			return result;
		}

		public Stream ExportExcelExpense(ExpenseReportParam param, string userName)
		{
			Stream stream;
			DataRow row;
			ExpenseList.ExpenseListDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			var companyName = "";
			var companyAddress = "";
			var companyTaxCode = "";
			var fileName = "";

			// get data
			dt = new ExpenseList.ExpenseListDataTable();
			List<ExpenseDetailListReport> data = GetExpenseListDetail(param);
			// get basic information
			var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (basicSetting != null)
			{
				companyName = basicSetting.CompanyFullN;
				companyAddress = basicSetting.Address1;
				companyTaxCode = basicSetting.TaxCode;
				fileName = basicSetting.Logo;
			}
			//ger employeeName
			var user = GetEmployeeByUserName(userName);


			// get language for report
			dicLanguage = new Dictionary<string, string>();
			CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
			if (param.Language == "vi")
			{
				intLanguage = 1;
			}
			else if (param.Language == "jp")
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
																 (con.TextKey == "TLTEXPENSEREPORT" ||
																 con.TextKey == "TLTEXPENSE" ||
																  con.TextKey == "LBLINVOICEDATE" ||
																  con.TextKey == "LBLREGISTEREDNOFIXEDEXPENSE" ||
																  con.TextKey == "LBLTRAILER" ||
																  con.TextKey == "LBLEMPLOYEE" ||
																  con.TextKey == "LBLTEQUANTITY" ||
																  con.TextKey == "LBLUNITPRICE" ||
																  con.TextKey == "RPLBLTOTAL" ||
																  con.TextKey == "LBLTAXRATE" ||
																  con.TextKey == "LBLAMOUNTREPORT" ||
																  con.TextKey == "RPHDCONTENT" ||
																  con.TextKey == "LBLCATEGORYC" ||
																  con.TextKey == "LBLOTHER" ||
																  con.TextKey == "RPFTCREATOR" ||
																  con.TextKey == "RPFTPRINTBY" ||
																  con.TextKey == "RPFTPRINTTIME" ||
																  con.TextKey == "TLTROUTE" ||
																  con.TextKey == "LBLTOL" ||
																  con.TextKey == "LBLTAXAMOUNT" ||
																  con.TextKey == "LBLTETOTAL" ||
																  con.TextKey == "LBLTRANSPORTDATEDISPATCH" ||
																  con.TextKey == "LBLSUPPIERREPORT" ||
																  con.TextKey == "LBLTYPE" ||
																  con.TextKey == "LBLCONTSIZERP" ||
																  con.TextKey == "LBLCONTNUMBER" ||
																  con.TextKey == "LBLLOAD" ||
																  con.TextKey == "MNUTRUCK"
																  )).ToList();
			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			if (param.ObjectI == "0")
			{
				return CrystalReport.Service.Expense.ExportExcel.ExportExpenseTruckListToExcel(data, dicLanguage, intLanguage, companyName, companyAddress, fileName, user, param.InvoiceDFrom,
				param.InvoiceDTo);
			}
			else if (param.ObjectI == "1")
			{
				return CrystalReport.Service.Expense.ExportExcel.ExportExpenseTrailerListToExcel(data, dicLanguage, intLanguage, companyName, companyAddress, fileName, user, param.InvoiceDFrom,
				param.InvoiceDTo);
			}
			else if (param.ObjectI == "3")
			{
				return CrystalReport.Service.Expense.ExportExcel.ExportExpenseDriverListToExcel(data, dicLanguage, intLanguage, companyName, companyAddress, fileName, user, param.InvoiceDFrom,
				param.InvoiceDTo);
			}
			else if (param.ObjectI == "2")
			{
				return CrystalReport.Service.Expense.ExportExcel.ExportExpenseEmployeeListToExcel(data, dicLanguage, intLanguage, companyName, companyAddress, fileName, user, param.InvoiceDFrom,
				param.InvoiceDTo);
			}
			else
			{
				return CrystalReport.Service.Expense.ExportExcel.ExportExpenseListToExcel(data, dicLanguage, intLanguage, companyName, companyAddress, fileName, user, param.InvoiceDFrom,
				param.InvoiceDTo);
			}
		}

		public Stream ExportPdfExpense(ExpenseReportParam param, string userName)
		{
			if (param.ReportType == "0")
			{
				return ExportPdfExpenseGeneral(param);
			}

			return ExportPdfExpenseDetail(param, userName);
		}

		public Stream ExportPdfExpenseGeneral(ExpenseReportParam param)
		{
			Stream stream;
			DataRow row;
			ExpenseList.ExpenseListDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			decimal total = 0;
			decimal taxAmountTotal = 0;
			decimal requestedTotal = 0;
			decimal includedTotal = 0;
			decimal payableTotal = 0;

			// get data
			dt = new ExpenseList.ExpenseListDataTable();
			List<ExpenseListReport> data = GetExpenseListGeneral(param);

			// get language for report
			dicLanguage = new Dictionary<string, string>();
			CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
			if (param.Language == "vi")
			{
				intLanguage = 1;
			}
			else if (param.Language == "jp")
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
																 (con.TextKey == "PAYMENTMETHODI0" ||
																  con.TextKey == "PAYMENTMETHODI1" ||
																  con.TextKey == "LBLREPORTI" ||
																  con.TextKey == "LBLREPORTITRANSPORT" ||
																  con.TextKey == "LBLREPORTIACCOUNTING" ||
																  con.TextKey == "LBLTRANSPORTDREPORT" ||
																  con.TextKey == "LBLINVOICEDREPORT"
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
				total = data.Sum(m => m.Amount);
				taxAmountTotal = data.Sum(m => m.TaxAmount);
				var listDate = new List<ExpenseListReport>();
				if (param.ReportI == "0")
				{
					listDate = data.GroupBy(item => item.TransportD)
					.Select(grp => grp.First())
					.ToList();
				}
				else
				{
					listDate = data.GroupBy(item => item.InvoiceD)
					.Select(grp => grp.First())
					.ToList();
				}

				int index = 1;
				foreach (var currentDay in listDate)
				{
					IEnumerable<ExpenseListReport> allowanceInDay;
					if (param.ReportI == "0")
					{
						allowanceInDay = from item in data
										 where item.TransportD == currentDay.TransportD
										 select item;
					}
					else
					{
						allowanceInDay = from item in data
										 where item.InvoiceD == currentDay.InvoiceD
										 select item;
					}

					bool isFirst = true;
					foreach (var item in allowanceInDay)
					{
						row = dt.NewRow();

						if (isFirst)
						{
							row["No"] = index++;
							if (param.ReportI == "1")
							{
								row["Day"] = item.InvoiceD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)item.InvoiceD, intLanguage) : null;
							}
							else
							{
								row["Day"] = item.TransportD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)item.TransportD, intLanguage) : null;
							}

							isFirst = false;
						}
						else
						{
							row["No"] = string.Empty;
							row["Day"] = string.Empty;
						}

						if (param.ReportI == "1")
						{
							row["OrderD"] = item.InvoiceD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)item.InvoiceD, intLanguage) : null;
						}
						else
						{
							row["OrderD"] = item.TransportD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)item.TransportD, intLanguage) : null;
						}
						row["ExpenseC"] = item.ExpenseC;
						row["ExpenseN"] = item.ExpenseN;
						row["Amount"] = item.Amount.ToString("#,###", cul.NumberFormat);
						row["TaxAmount"] = item.TaxAmount.ToString("#,###", cul.NumberFormat);
						row["IsIncluded"] = item.IsIncluded.ToString("#,###", cul.NumberFormat);
						row["IsRequested"] = item.IsRequested.ToString("#,###", cul.NumberFormat);
						row["IsPayable"] = item.IsPayable.ToString("#,###", cul.NumberFormat);
						row["PaymentMethod"] = item.PaymentMethod == "1" ? dicLanguage["PAYMENTMETHODI1"] : item.PaymentMethod == "0" ? dicLanguage["PAYMENTMETHODI0"] : string.Empty;
						row["Description"] = item.Description;
						includedTotal += item.IsIncluded;
						requestedTotal += item.IsRequested;
						payableTotal += item.IsPayable;

						dt.Rows.Add(row);
					}
				}

			}

			// set category
			var category = dicLanguage["LBLREPORTI"] + ": ";
			var columnDate = "";
			if (param.ReportI == "0")
			{
				category = category + dicLanguage["LBLREPORTITRANSPORT"];
				columnDate = dicLanguage["LBLTRANSPORTDREPORT"];
			}
			else
			{
				category = category + dicLanguage["LBLREPORTIACCOUNTING"];
				columnDate = dicLanguage["LBLINVOICEDREPORT"];
			}

			stream = CrystalReport.Service.Expense.ExportPdf.Exec(dt, intLanguage, total.ToString("#,###", cul.NumberFormat),
				taxAmountTotal.ToString("#,###", cul.NumberFormat),
				includedTotal.ToString("#,###", cul.NumberFormat),
				requestedTotal.ToString("#,###", cul.NumberFormat),
				payableTotal.ToString("#,###", cul.NumberFormat),
				param.InvoiceDFrom,
				param.InvoiceDTo,
				category,
				columnDate);

			return stream;
		}

		public List<ExpenseListReport> GetExpenseListGeneral(ExpenseReportParam param)
		{
			List<ExpenseListReport> result = new List<ExpenseListReport>();
			// get data from Expense_D
			result = (from e in _expenseDetailRepository.GetAllQueryable()
					  join ex in _expenseRepository.GetAllQueryable() on e.ExpenseC
						  equals ex.ExpenseC into t1
					  from ex in t1.DefaultIfEmpty()
					  join o in _orderHRepository.GetAllQueryable() on new { e.OrderD, e.OrderNo }
						  equals new { o.OrderD, o.OrderNo } into t2
					  from o in t2.DefaultIfEmpty()
					  join d in _dispatchRepository.GetAllQueryable() on new { e.OrderD, e.OrderNo, e.DetailNo, e.DispatchNo }
						  equals new { d.OrderD, d.OrderNo, d.DetailNo, d.DispatchNo } into t3
					  from d in t3.DefaultIfEmpty()
					  join t in _truckRepository.GetAllQueryable() on d.TruckC
					  equals t.TruckC into t4
					  from t in t4.DefaultIfEmpty()
					  where (((param.ReportI == "1" && e.InvoiceD >= param.InvoiceDFrom & e.InvoiceD <= param.InvoiceDTo) ||
							  (param.ReportI == "0" && d.TransportD >= param.InvoiceDFrom & d.TransportD <= param.InvoiceDTo)
							 ) &
							 (param.ExpenseCategories == "null" || (param.ExpenseCategories).Contains(e.ExpenseC)) &
							 (param.Suppliers == "null" || (param.Suppliers).Contains(e.SupplierSubC + "_" + e.SupplierSubC)) &
							 (param.PaymentMethod == "-1" || e.PaymentMethodI == param.PaymentMethod) &
							 t.PartnerI == "0"
						  )
					  select new ExpenseListReport()
					  {
						  InvoiceD = e.InvoiceD,
						  TransportD = d != null ? d.TransportD : null,
						  OrderNo = e.OrderNo,
						  ExpenseN = ex.ExpenseN,
						  ExpenseC = ex.ExpenseC,
						  Amount = e.Amount ?? 0,
						  TaxAmount = e.TaxAmount ?? 0,
						  IsIncluded = e.IsIncluded == null ? 0 : e.IsIncluded == "1" ? e.Amount ?? 0 : 0,
						  IsRequested = e.IsRequested == null ? 0 : e.IsRequested == "1" ? e.Amount ?? 0 : 0,
						  IsPayable = e.IsPayable == null ? 0 : e.IsPayable == "1" ? e.Amount ?? 0 : 0,
						  PaymentMethod = e.PaymentMethodI,
						  Description = e.Description
					  }).OrderBy(param.ReportI == "1" ? "InvoiceD asc" : "TransportD asc").ToList();

			var resulFromTruckExpense = (from t in _truckExpenseRepository.GetAllQueryable()
										 join e in _expenseRepository.GetAllQueryable() on t.ExpenseC equals e.ExpenseC
										 join f in _truckRepository.GetAllQueryable() on t.Code equals f.TruckC into t1
										 from f in t1.DefaultIfEmpty()
										 where (((param.ReportI == "1" && t.InvoiceD >= param.InvoiceDFrom & t.InvoiceD <= param.InvoiceDTo) ||
												 (param.ReportI == "0" && t.TransportD >= param.InvoiceDFrom & t.TransportD <= param.InvoiceDTo)
												 ) &
												(t.ObjectI == "0") &
												(param.ExpenseCategories == "null" || (param.ExpenseCategories).Contains(e.ExpenseC)) &
												(e.CategoryI == "1") &
												(param.Suppliers == "null" || (param.Suppliers).Contains(t.SupplierMainC + "_" + t.SupplierSubC)) &
												(param.PaymentMethod == "-1" || t.PaymentMethodI == param.PaymentMethod) &
												f.PartnerI == "0"
										 )
										 select new ExpenseListReport()
										 {
											 InvoiceD = t.InvoiceD,
											 TransportD = t.TransportD,
											 ExpenseN = e.ExpenseN,
											 ExpenseC = e.ExpenseC,
											 Amount = t.Total ?? 0,
											 TaxAmount = t.Tax ?? 0,
											 IsIncluded = t.Total ?? 0,
											 IsRequested = 0,
											 IsPayable = 0,
											 PaymentMethod = t.PaymentMethodI,
											 Description = t.Description
										 }).OrderBy(param.ReportI == "1" ? "InvoiceD asc" : "TransportD asc").ToList();

			result = result.Concat(resulFromTruckExpense).ToList();

			result = result.OrderBy(param.ReportI == "1" ? "InvoiceD asc" : "TransportD asc").ToList();

			if (param.ReportI == "1")
			{
				result = (from o in result
						  where o.Amount > 0
						  group o by new { o.InvoiceD, o.ExpenseC, o.ExpenseN, o.PaymentMethod }
							  into g
							  select new ExpenseListReport()
							  {
								  InvoiceD = g.Key.InvoiceD,
								  ExpenseC = g.Key.ExpenseC,
								  ExpenseN = g.Key.ExpenseN,
								  Amount = g.Sum(x => x.Amount),
								  TaxAmount = g.Sum(x => x.TaxAmount),
								  IsIncluded = g.Sum(x => x.IsIncluded),
								  IsRequested = g.Sum(x => x.IsRequested),
								  IsPayable = g.Sum(x => x.IsPayable),
								  PaymentMethod = g.Key.PaymentMethod,
								  Description = g.Aggregate(string.Empty, (x, i) => x + " " + i.Description)
							  }).ToList();
			}
			else
			{
				result = (from o in result
						  where o.Amount > 0
						  group o by new { o.TransportD, o.ExpenseC, o.ExpenseN, o.PaymentMethod }
							  into g
							  select new ExpenseListReport()
							  {
								  TransportD = g.Key.TransportD,
								  ExpenseC = g.Key.ExpenseC,
								  ExpenseN = g.Key.ExpenseN,
								  Amount = g.Sum(x => x.Amount),
								  TaxAmount = g.Sum(x => x.TaxAmount),
								  IsIncluded = g.Sum(x => x.IsIncluded),
								  IsRequested = g.Sum(x => x.IsRequested),
								  IsPayable = g.Sum(x => x.IsPayable),
								  PaymentMethod = g.Key.PaymentMethod,
								  Description = g.Aggregate(string.Empty, (x, i) => x + " " + i.Description)
							  }).ToList();
			}

			return result;
		}

		public Stream ExportPdfExpenseDetail(ExpenseReportParam param, string userName)
		{
			Stream stream;
			DataRow row;
			ExpenseList.ExpenseListDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			var companyName = "";
			var companyAddress = "";
			var companyTaxCode = "";
			var fileName = "";

			// get data
			dt = new ExpenseList.ExpenseListDataTable();
			if (param.ObjectI == "9")
			{
				List<ExpenseDetailListReport> data = GetExpenseListDetail(param);
				// get basic information
				var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
				if (basicSetting != null)
				{
					companyName = basicSetting.CompanyFullN;
					companyAddress = basicSetting.Address1;
					companyTaxCode = basicSetting.TaxCode;
					fileName = basicSetting.Logo;
				}
				//ger employeeName
				var user = GetEmployeeByUserName(userName);


				// get language for report
				dicLanguage = new Dictionary<string, string>();
				CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
				if (param.Language == "vi")
				{
					intLanguage = 1;
				}
				else if (param.Language == "jp")
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
																	 (con.TextKey == "PAYMENTMETHODI0" ||
																	  con.TextKey == "PAYMENTMETHODI1" ||
																	  con.TextKey == "LBLREPORTI" ||
																	  con.TextKey == "LBLREPORTITRANSPORT" ||
																	  con.TextKey == "LBLREPORTIACCOUNTING" ||
																	  con.TextKey == "LBLCATEGORYC" ||
																	  con.TextKey == "LBLOTHER" ||
																	  con.TextKey == "RPLBLTOTAL" ||
																	  con.TextKey == "RPFTPRINTTIME" ||
																	  con.TextKey == "RPFTPRINTBY" ||
																	  con.TextKey == "RPFTPAGE" ||
																	  con.TextKey == "RPFTCREATOR" ||

																	  con.TextKey == "MNUEXPENSELISTREPORT" ||
																	  con.TextKey == "MNUSTATISTICSEXPENSE" ||
																	  con.TextKey == "LBLINVOICEDATERP" ||
																	  con.TextKey == "LBLTRANSPORTDATEDISPATCHRP" ||
																	  con.TextKey == "MNUTRUCK" ||
																	  con.TextKey == "MNUTRAILER" ||
																	  con.TextKey == "MNUEMPLOYEE" ||
																	  con.TextKey == "TLTROUTERP" ||
																	  con.TextKey == "LBLQUANTITYSHORTRP" ||
																	  con.TextKey == "LBLUNITPRICERP" ||
																	  con.TextKey == "LBLTOL" ||
																	  con.TextKey == "LBLTAXAMOUNTREPORT" ||
																	  con.TextKey == "LBLTETOTAL" ||
																	  con.TextKey == "LBLCONTENTCALCULATE" ||
																	  con.TextKey == "LBLSUPPLIERSHORTRP" ||
																	  con.TextKey == "LBLTYPE" ||
																	  con.TextKey == "LBLCONTSIZERP" ||
																	  con.TextKey == "LBLCONTNUMBER" ||
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
					for (var iloop = 0; iloop < data.Count; iloop++)
					{
						row = dt.NewRow();
						row["InvoiceD"] = data[iloop].InvoiceD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].InvoiceD, intLanguage) : null;
						row["InvoiceDate"] = (DateTime)data[iloop].InvoiceD.Value;
						//row["OrderNo"] = data[iloop].OrderNo;
						//row["CategoryN"] = dicLanguage["LBLCATEGORYC"] + ": " + (data[iloop].CategoryN == "" ? dicLanguage["LBLOTHER"] : data[iloop].CategoryN);
						row["CategoryN"] = ((data[iloop].CategoryN == "" || data[iloop].CategoryN == null) ? dicLanguage["LBLOTHER"] : data[iloop].CategoryN);
						row["ExpenseN"] = data[iloop].ExpenseN;
						row["PaymentMethodI"] = data[iloop].PaymentMethodI == "1" ? dicLanguage["PAYMENTMETHODI1"] : data[iloop].PaymentMethodI == "0" ? dicLanguage["PAYMENTMETHODI0"] : string.Empty; ;
						row["TransportD"] = data[iloop].TransportD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].TransportD, intLanguage) : null;
						//row["RegisteredNo"] = (!string.IsNullOrEmpty(data[iloop].RegisteredNo) ? (data[iloop].RegisteredNo + "<br>") : "") + data[iloop].TrailerNo;
						row["RegisteredNo"] = data[iloop].RegisteredNo;
						row["DriverN"] = data[iloop].DriverN;
						row["SupplierN"] = !string.IsNullOrEmpty(data[iloop].SupplierShortN) ? data[iloop].SupplierShortN : data[iloop].SupplierN;
						row["TrailerNo"] = data[iloop].TrailerNo;
						row["EntryClerkN"] = data[iloop].EntryClerkN;
						row["UnitPrice"] = data[iloop].UnitPrice ?? 0;
						row["Quantity"] = data[iloop].Quantity ?? 0;
						row["TotalAmount"] = data[iloop].TotalAmount ?? 0;
						row["TaxAmount"] = data[iloop].TaxAmount ?? 0;
						row["TotalTaxAndAmount"] = (decimal?)((data[iloop].TotalAmount ?? 0) + (data[iloop].TaxAmount ?? 0));
						row["Description"] = data[iloop].Description;
						row["OrderTypeI"] = Utilities.GetOrderTypeName(data[iloop].OrderTypeI);
						row["ContNo"] = data[iloop].ContainerNo;
						row["ContSize"] = (data[iloop].ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] + " (" + (data[iloop].NetWeight ?? 0).ToString("#,###.#") + ")" : Utilities.GetContainerSizeName(data[iloop].ContainerSizeI));

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
						row["Location1N"] = temp;
						//row["Location2N"] = data[iloop].Location2N;
						//row["Location3N"] = data[iloop].Location3N;

						dt.Rows.Add(row);
					}
				}

				// set category
				//var category = dicLanguage["LBLREPORTI"] + ": ";
				//if (param.ReportI == "0")
				//{
				//	category = category + dicLanguage["LBLREPORTITRANSPORT"];
				//}
				//else
				//{
				//	category = category + dicLanguage["LBLREPORTIACCOUNTING"];
				//}

				stream = CrystalReport.Service.Expense.ExportPdf.ExecCategory(dt,
																	   intLanguage,
																	   param.InvoiceDFrom,
																	   param.InvoiceDTo,
																	   dicLanguage,
																	   companyName, companyAddress, fileName, user);
			}
			else if (param.ObjectI == "0")
			{
				List<ExpenseDetailListReport> data = GetExpenseListDetail(param);
				// get basic information
				var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
				if (basicSetting != null)
				{
					companyName = basicSetting.CompanyFullN;
					companyAddress = basicSetting.Address1;
					companyTaxCode = basicSetting.TaxCode;
					fileName = basicSetting.Logo;
				}
				//ger employeeName
				var user = GetEmployeeByUserName(userName);


				// get language for report
				dicLanguage = new Dictionary<string, string>();
				CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
				if (param.Language == "vi")
				{
					intLanguage = 1;
				}
				else if (param.Language == "jp")
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
																	 (con.TextKey == "PAYMENTMETHODI0" ||
																	  con.TextKey == "PAYMENTMETHODI1" ||
																	  con.TextKey == "LBLREPORTI" ||
																	  con.TextKey == "LBLREPORTITRANSPORT" ||
																	  con.TextKey == "LBLREPORTIACCOUNTING" ||
																	  con.TextKey == "LBLCATEGORYC" ||
																	  con.TextKey == "LBLOTHER" ||

																	  con.TextKey == "RPLBLTOTAL" ||

																	  con.TextKey == "RPFTPRINTTIME" ||
																	  con.TextKey == "RPFTPRINTBY" ||
																	  con.TextKey == "RPFTPAGE" ||
																	  con.TextKey == "RPFTCREATOR" ||

																	  con.TextKey == "MNUEXPENSELISTREPORT" ||
																	  con.TextKey == "MNUSTATISTICSEXPENSE" ||
																	  con.TextKey == "LBLINVOICEDATERP" ||
																	  con.TextKey == "LBLTRANSPORTDATEDISPATCHRP" ||
																	  con.TextKey == "MNUTRUCK" ||
																	  con.TextKey == "MNUTRAILER" ||
																	  con.TextKey == "MNUEMPLOYEE" ||
																	  con.TextKey == "TLTROUTERP" ||
																	  con.TextKey == "LBLQUANTITYSHORTRP" ||
																	  con.TextKey == "LBLUNITPRICERP" ||
																	  con.TextKey == "LBLTOL" ||
																	  con.TextKey == "LBLTAXAMOUNTREPORT" ||
																	  con.TextKey == "LBLTETOTAL" ||
																	  con.TextKey == "LBLCONTENTCALCULATE" ||
																	  con.TextKey == "LBLSUPPLIERSHORTRP" ||
																	  con.TextKey == "LBLTYPE" ||
																	  con.TextKey == "LBLCONTSIZERP" ||
																	  con.TextKey == "LBLCONTNUMBER" ||
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
					for (var iloop = 0; iloop < data.Count; iloop++)
					{
						if (data[iloop].RegisteredNo != "")
						{
							row = dt.NewRow();
							row["InvoiceD"] = data[iloop].InvoiceD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].InvoiceD, intLanguage) : null;
							row["InvoiceDate"] = (DateTime)data[iloop].InvoiceD.Value;
							//row["OrderNo"] = data[iloop].OrderNo;
							//row["CategoryN"] = dicLanguage["LBLCATEGORYC"] + ": " + (data[iloop].CategoryN == "" ? dicLanguage["LBLOTHER"] : data[iloop].CategoryN);
							row["CategoryN"] = (data[iloop].CategoryN == "" ? dicLanguage["LBLOTHER"] : data[iloop].CategoryN);
							row["ExpenseN"] = data[iloop].ExpenseN;
							row["PaymentMethodI"] = data[iloop].PaymentMethodI == "1" ? dicLanguage["PAYMENTMETHODI1"] : data[iloop].PaymentMethodI == "0" ? dicLanguage["PAYMENTMETHODI0"] : string.Empty; ;
							row["TransportD"] = data[iloop].TransportD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].TransportD, intLanguage) : null;
							row["RegisteredNo"] = data[iloop].RegisteredNo;
							row["DriverN"] = data[iloop].DriverN;
							row["SupplierN"] = !string.IsNullOrEmpty(data[iloop].SupplierShortN) ? data[iloop].SupplierShortN : data[iloop].SupplierN;
							row["TrailerNo"] = data[iloop].TrailerNo;
							row["EntryClerkN"] = data[iloop].EntryClerkN;
							row["UnitPrice"] = data[iloop].UnitPrice ?? 0;
							row["Quantity"] = data[iloop].Quantity ?? 0;
							row["TotalAmount"] = data[iloop].TotalAmount ?? 0;
							row["TaxAmount"] = data[iloop].TaxAmount ?? 0;
							row["TotalTaxAndAmount"] = (decimal?)((data[iloop].TotalAmount ?? 0) + (data[iloop].TaxAmount ?? 0));
							row["Description"] = data[iloop].Description;
							row["OrderTypeI"] = Utilities.GetOrderTypeName(data[iloop].OrderTypeI);
							row["ContNo"] = data[iloop].ContainerNo;
							row["ContSize"] = (data[iloop].ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] + " (" + (data[iloop].NetWeight ?? 0).ToString("#,###.#") + ")" : Utilities.GetContainerSizeName(data[iloop].ContainerSizeI));

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
							row["Location1N"] = temp;
							//row["Location2N"] = data[iloop].Location2N;
							//row["Location3N"] = data[iloop].Location3N;

							dt.Rows.Add(row);
						}

					}
				}

				// set category
				//var category = dicLanguage["LBLREPORTI"] + ": ";
				//if (param.ReportI == "0")
				//{
				//	category = category + dicLanguage["LBLREPORTITRANSPORT"];
				//}
				//else
				//{
				//	category = category + dicLanguage["LBLREPORTIACCOUNTING"];
				//}

				stream = CrystalReport.Service.Expense.ExportPdf.ExecCategoryTruck(dt,
																	   intLanguage,
																	   param.InvoiceDFrom,
																	   param.InvoiceDTo,
																	   dicLanguage,
																	   companyName, companyAddress, fileName, user);
			}
			else if (param.ObjectI == "1")
			{
				List<ExpenseDetailListReport> data = GetExpenseListDetail(param);
				// get basic information
				var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
				if (basicSetting != null)
				{
					companyName = basicSetting.CompanyFullN;
					companyAddress = basicSetting.Address1;
					companyTaxCode = basicSetting.TaxCode;
					fileName = basicSetting.Logo;
				}
				//ger employeeName
				var user = GetEmployeeByUserName(userName);


				// get language for report
				dicLanguage = new Dictionary<string, string>();
				CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
				if (param.Language == "vi")
				{
					intLanguage = 1;
				}
				else if (param.Language == "jp")
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
																	 (con.TextKey == "PAYMENTMETHODI0" ||
																	  con.TextKey == "PAYMENTMETHODI1" ||
																	  con.TextKey == "LBLREPORTI" ||
																	  con.TextKey == "LBLREPORTITRANSPORT" ||
																	  con.TextKey == "LBLREPORTIACCOUNTING" ||
																	  con.TextKey == "LBLCATEGORYC" ||
																	  con.TextKey == "LBLOTHER" ||

																	  con.TextKey == "RPLBLTOTAL" ||

																	  con.TextKey == "RPFTPRINTTIME" ||
																	  con.TextKey == "RPFTPRINTBY" ||
																	  con.TextKey == "RPFTPAGE" ||
																	  con.TextKey == "RPFTCREATOR" ||
																	  con.TextKey == "MNUEXPENSELISTREPORT" ||
																	  con.TextKey == "MNUSTATISTICSEXPENSE" ||
																	  con.TextKey == "LBLINVOICEDATERP" ||
																	  con.TextKey == "LBLTRANSPORTDATEDISPATCHRP" ||
																	  con.TextKey == "MNUTRUCK" ||
																	  con.TextKey == "MNUTRAILER" ||
																	  con.TextKey == "MNUEMPLOYEE" ||
																	  con.TextKey == "TLTROUTERP" ||
																	  con.TextKey == "LBLQUANTITYSHORTRP" ||
																	  con.TextKey == "LBLUNITPRICERP" ||
																	  con.TextKey == "LBLTOL" ||
																	  con.TextKey == "LBLTAXAMOUNTREPORT" ||
																	  con.TextKey == "LBLTETOTAL" ||
																	  con.TextKey == "LBLCONTENTCALCULATE" ||
																	  con.TextKey == "LBLSUPPLIERSHORTRP" ||
																	  con.TextKey == "LBLTYPE" ||
																	  con.TextKey == "LBLCONTSIZERP" ||
																	  con.TextKey == "LBLCONTNUMBER" ||
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
					for (var iloop = 0; iloop < data.Count; iloop++)
					{
						if (data[iloop].TrailerNo != "")
						{
							row = dt.NewRow();
							row["InvoiceD"] = data[iloop].InvoiceD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].InvoiceD, intLanguage) : null;
							row["InvoiceDate"] = (DateTime)data[iloop].InvoiceD.Value;
							//row["OrderNo"] = data[iloop].OrderNo;
							//row["CategoryN"] = dicLanguage["LBLCATEGORYC"] + ": " + (data[iloop].CategoryN == "" ? dicLanguage["LBLOTHER"] : data[iloop].CategoryN);
							row["CategoryN"] = (data[iloop].CategoryN == "" ? dicLanguage["LBLOTHER"] : data[iloop].CategoryN);
							row["ExpenseN"] = data[iloop].ExpenseN;
							row["PaymentMethodI"] = data[iloop].PaymentMethodI == "1" ? dicLanguage["PAYMENTMETHODI1"] : data[iloop].PaymentMethodI == "0" ? dicLanguage["PAYMENTMETHODI0"] : string.Empty; ;
							row["TransportD"] = data[iloop].TransportD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].TransportD, intLanguage) : null;
							row["RegisteredNo"] = data[iloop].RegisteredNo;
							row["DriverN"] = data[iloop].DriverN;
							row["SupplierN"] = !string.IsNullOrEmpty(data[iloop].SupplierShortN) ? data[iloop].SupplierShortN : data[iloop].SupplierN;
							row["TrailerNo"] = data[iloop].TrailerNo;
							row["EntryClerkN"] = data[iloop].EntryClerkN;
							row["UnitPrice"] = data[iloop].UnitPrice ?? 0;
							row["Quantity"] = data[iloop].Quantity ?? 0;
							row["TotalAmount"] = data[iloop].TotalAmount ?? 0;
							row["TaxAmount"] = data[iloop].TaxAmount ?? 0;
							row["TotalTaxAndAmount"] = (decimal?)((data[iloop].TotalAmount ?? 0) + (data[iloop].TaxAmount ?? 0));
							row["Description"] = data[iloop].Description;
							row["OrderTypeI"] = Utilities.GetOrderTypeName(data[iloop].OrderTypeI);
							row["ContNo"] = data[iloop].ContainerNo;
							row["ContSize"] = (data[iloop].ContainerSizeI == "3" ? dicLanguage["LBLLOAD"] + " (" + (data[iloop].NetWeight ?? 0).ToString("#,###.#") + ")" : Utilities.GetContainerSizeName(data[iloop].ContainerSizeI));

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
							row["Location1N"] = temp;
							//row["Location2N"] = data[iloop].Location2N;
							//row["Location3N"] = data[iloop].Location3N;

							dt.Rows.Add(row);
						}

					}
				}

				// set category
				//var category = dicLanguage["LBLREPORTI"] + ": ";
				//if (param.ReportI == "0")
				//{
				//	category = category + dicLanguage["LBLREPORTITRANSPORT"];
				//}
				//else
				//{
				//	category = category + dicLanguage["LBLREPORTIACCOUNTING"];
				//}

				stream = CrystalReport.Service.Expense.ExportPdf.ExecCategoryTrailer(dt,
																	   intLanguage,
																	   param.InvoiceDFrom,
																	   param.InvoiceDTo,
																	   dicLanguage,
																	   companyName, companyAddress, fileName, user);
			}
			else if (param.ObjectI == "2")
			{
				List<ExpenseDetailListReport> data = GetExpenseListDetail(param);
				// get basic information
				var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
				if (basicSetting != null)
				{
					companyName = basicSetting.CompanyFullN;
					companyAddress = basicSetting.Address1;
					companyTaxCode = basicSetting.TaxCode;
					fileName = basicSetting.Logo;
				}
				//ger employeeName
				var user = GetEmployeeByUserName(userName);


				// get language for report
				dicLanguage = new Dictionary<string, string>();
				CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
				if (param.Language == "vi")
				{
					intLanguage = 1;
				}
				else if (param.Language == "jp")
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
																	 (con.TextKey == "PAYMENTMETHODI0" ||
																	  con.TextKey == "PAYMENTMETHODI1" ||
																	  con.TextKey == "LBLREPORTI" ||
																	  con.TextKey == "LBLREPORTITRANSPORT" ||
																	  con.TextKey == "LBLREPORTIACCOUNTING" ||
																	  con.TextKey == "LBLCATEGORYC" ||
																	  con.TextKey == "LBLOTHER" ||

																	  con.TextKey == "RPLBLTOTAL" ||

																	  con.TextKey == "RPFTPRINTTIME" ||
																	  con.TextKey == "RPFTPRINTBY" ||
																	  con.TextKey == "RPFTPAGE" ||
																	  con.TextKey == "RPFTCREATOR" ||

																	  con.TextKey == "MNUEXPENSELISTREPORT" ||
																	  con.TextKey == "MNUSTATISTICSEXPENSE" ||
																	  con.TextKey == "LBLINVOICEDATERP" ||
																	  con.TextKey == "LBLTRANSPORTDATEDISPATCHRP" ||
																	  con.TextKey == "MNUTRUCK" ||
																	  con.TextKey == "MNUTRAILER" ||
																	  con.TextKey == "MNUEMPLOYEE" ||
																	  con.TextKey == "TLTROUTERP" ||
																	  con.TextKey == "LBLQUANTITYSHORTRP" ||
																	  con.TextKey == "LBLUNITPRICERP" ||
																	  con.TextKey == "LBLTOL" ||
																	  con.TextKey == "LBLTAXAMOUNTREPORT" ||
																	  con.TextKey == "LBLTETOTAL" ||
																	  con.TextKey == "LBLCONTENTCALCULATE" ||
																	  con.TextKey == "LBLSUPPLIERSHORTRP"
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
					for (var iloop = 0; iloop < data.Count; iloop++)
					{
						if (data[iloop].EntryClerkN != "")
						{
							row = dt.NewRow();
							row["InvoiceD"] = data[iloop].InvoiceD != null
								? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].InvoiceD, intLanguage)
								: null;
							row["InvoiceDate"] = (DateTime)data[iloop].InvoiceD.Value;
							//row["OrderNo"] = data[iloop].OrderNo;
							//row["CategoryN"] = dicLanguage["LBLCATEGORYC"] + ": " + (data[iloop].CategoryN == "" ? dicLanguage["LBLOTHER"] : data[iloop].CategoryN);
							row["CategoryN"] = (data[iloop].CategoryN == "" ? dicLanguage["LBLOTHER"] : data[iloop].CategoryN);
							row["ExpenseN"] = data[iloop].ExpenseN;
							row["PaymentMethodI"] = data[iloop].PaymentMethodI == "1"
								? dicLanguage["PAYMENTMETHODI1"]
								: data[iloop].PaymentMethodI == "0" ? dicLanguage["PAYMENTMETHODI0"] : string.Empty;
							;
							row["TransportD"] = data[iloop].TransportD != null
								? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].TransportD, intLanguage)
								: null;
							row["RegisteredNo"] = data[iloop].RegisteredNo;
							row["DriverN"] = data[iloop].DriverN;
							row["SupplierN"] = !string.IsNullOrEmpty(data[iloop].SupplierShortN)
								? data[iloop].SupplierShortN
								: data[iloop].SupplierN;
							row["TrailerNo"] = data[iloop].TrailerNo;
							row["EntryClerkN"] = data[iloop].EntryClerkN;
							row["UnitPrice"] = data[iloop].UnitPrice ?? 0;
							row["Quantity"] = data[iloop].Quantity ?? 0;
							row["TotalAmount"] = data[iloop].TotalAmount ?? 0;
							row["TaxAmount"] = data[iloop].TaxAmount ?? 0;
							row["TotalTaxAndAmount"] = (decimal?)((data[iloop].TotalAmount ?? 0) + (data[iloop].TaxAmount ?? 0));
							row["Description"] = data[iloop].Description;
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
							row["Location1N"] = temp;
							//row["Location2N"] = data[iloop].Location2N;
							//row["Location3N"] = data[iloop].Location3N;

							dt.Rows.Add(row);
						}
					}
				}

				// set category
				//var category = dicLanguage["LBLREPORTI"] + ": ";
				//if (param.ReportI == "0")
				//{
				//	category = category + dicLanguage["LBLREPORTITRANSPORT"];
				//}
				//else
				//{
				//	category = category + dicLanguage["LBLREPORTIACCOUNTING"];
				//}

				stream = CrystalReport.Service.Expense.ExportPdf.ExecCategoryEmployee(dt,
					intLanguage,
					param.InvoiceDFrom,
					param.InvoiceDTo,
					dicLanguage,
					companyName, companyAddress, fileName, user);
			}
			else
			{
				List<ExpenseDetailListReport> data = GetExpenseListDetail(param);
				// get basic information
				var basicSetting = _basicRepository.GetAllQueryable().FirstOrDefault();
				if (basicSetting != null)
				{
					companyName = basicSetting.CompanyFullN;
					companyAddress = basicSetting.Address1;
					companyTaxCode = basicSetting.TaxCode;
					fileName = basicSetting.Logo;
				}
				//ger employeeName
				var user = GetEmployeeByUserName(userName);


				// get language for report
				dicLanguage = new Dictionary<string, string>();
				CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
				if (param.Language == "vi")
				{
					intLanguage = 1;
				}
				else if (param.Language == "jp")
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
																	 (con.TextKey == "PAYMENTMETHODI0" ||
																	  con.TextKey == "PAYMENTMETHODI1" ||
																	  con.TextKey == "LBLREPORTI" ||
																	  con.TextKey == "LBLREPORTITRANSPORT" ||
																	  con.TextKey == "LBLREPORTIACCOUNTING" ||
																	  con.TextKey == "LBLCATEGORYC" ||
																	  con.TextKey == "LBLOTHER" ||

																	  con.TextKey == "RPLBLTOTAL" ||

																	  con.TextKey == "RPFTPRINTTIME" ||
																	  con.TextKey == "RPFTPRINTBY" ||
																	  con.TextKey == "RPFTPAGE" ||
																	  con.TextKey == "RPFTCREATOR" ||
																	  con.TextKey == "MNUEXPENSELISTREPORT" ||
																	  con.TextKey == "MNUSTATISTICSEXPENSE" ||
																	  con.TextKey == "LBLINVOICEDATERP" ||
																	  con.TextKey == "LBLTRANSPORTDATEDISPATCHRP" ||
																	  con.TextKey == "MNUTRUCK" ||
																	  con.TextKey == "MNUTRAILER" ||
																	  con.TextKey == "MNUEMPLOYEE" ||
																	  con.TextKey == "TLTROUTERP" ||
																	  con.TextKey == "LBLQUANTITYSHORTRP" ||
																	  con.TextKey == "LBLUNITPRICERP" ||
																	  con.TextKey == "LBLTOL" ||
																	  con.TextKey == "LBLTAXAMOUNTREPORT" ||
																	  con.TextKey == "LBLTETOTAL" ||
																	  con.TextKey == "LBLCONTENTCALCULATE" ||
																	  con.TextKey == "LBLSUPPLIERSHORTRP"
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
					for (var iloop = 0; iloop < data.Count; iloop++)
					{
						if (data[iloop].DriverN != "")
						{
							row = dt.NewRow();
							row["InvoiceD"] = data[iloop].InvoiceD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].InvoiceD, intLanguage) : null;
							row["InvoiceDate"] = (DateTime)data[iloop].InvoiceD.Value;
							//row["OrderNo"] = data[iloop].OrderNo;
							//row["CategoryN"] = dicLanguage["LBLCATEGORYC"] + ": " + (data[iloop].CategoryN == "" ? dicLanguage["LBLOTHER"] : data[iloop].CategoryN);
							row["CategoryN"] = (data[iloop].CategoryN == "" ? dicLanguage["LBLOTHER"] : data[iloop].CategoryN);
							row["ExpenseN"] = data[iloop].ExpenseN;
							row["PaymentMethodI"] = data[iloop].PaymentMethodI == "1" ? dicLanguage["PAYMENTMETHODI1"] : data[iloop].PaymentMethodI == "0" ? dicLanguage["PAYMENTMETHODI0"] : string.Empty; ;
							row["TransportD"] = data[iloop].TransportD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].TransportD, intLanguage) : null;
							row["RegisteredNo"] = (!string.IsNullOrEmpty(data[iloop].RegisteredNo) ? (data[iloop].RegisteredNo + (!string.IsNullOrEmpty(data[iloop].TrailerNo) ? "<br>" : "")) : "") + data[iloop].TrailerNo;
							row["DriverN"] = data[iloop].DriverN;
							row["SupplierN"] = !string.IsNullOrEmpty(data[iloop].SupplierShortN) ? data[iloop].SupplierShortN : data[iloop].SupplierN;
							row["TrailerNo"] = data[iloop].TrailerNo;
							row["EntryClerkN"] = data[iloop].EntryClerkN;
							row["UnitPrice"] = data[iloop].UnitPrice ?? 0;
							row["Quantity"] = data[iloop].Quantity ?? 0;
							row["TotalAmount"] = data[iloop].TotalAmount ?? 0;
							row["TaxAmount"] = data[iloop].TaxAmount ?? 0;
							row["TotalTaxAndAmount"] = (decimal?)((data[iloop].TotalAmount ?? 0) + (data[iloop].TaxAmount ?? 0));
							row["Description"] = data[iloop].Description;
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
							row["Location1N"] = temp;
							//row["Location2N"] = data[iloop].Location2N;
							//row["Location3N"] = data[iloop].Location3N;

							dt.Rows.Add(row);
						}
					}
				}

				// set category
				//var category = dicLanguage["LBLREPORTI"] + ": ";
				//if (param.ReportI == "0")
				//{
				//	category = category + dicLanguage["LBLREPORTITRANSPORT"];
				//}
				//else
				//{
				//	category = category + dicLanguage["LBLREPORTIACCOUNTING"];
				//}

				stream = CrystalReport.Service.Expense.ExportPdf.ExecCategoryDriver(dt,
																	   intLanguage,
																	   param.InvoiceDFrom,
																	   param.InvoiceDTo,
																	   dicLanguage,
																	   companyName, companyAddress, fileName, user);
			}
			return stream;
		}

		public List<ExpenseDetailListReport> GetExpenseListDetail(ExpenseReportParam param)
		{
			DateTime nextDate = new DateTime();
			if (param.InvoiceDTo != null)
			{
				nextDate = param.InvoiceDTo.AddDays(1);
			}
			// get data from Expense_D
			var expenseDList = (from a in _expenseDetailRepository.GetAllQueryable()
								join b in _dispatchRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo, a.DispatchNo }
									equals new { b.OrderD, b.OrderNo, b.DetailNo, b.DispatchNo } into t1
								from b in t1.DefaultIfEmpty()
								join oh in _orderHRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
									equals new { oh.OrderD, oh.OrderNo } into tt
								from oh in tt.DefaultIfEmpty()
								join od in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo }
									equals new { od.OrderD, od.OrderNo, od.DetailNo } into ttt
								from od in ttt.DefaultIfEmpty()
								join trai in _trailerRepository.GetAllQueryable() on od.TrailerC
								   equals trai.TrailerC into tttt
								from trai in tttt.DefaultIfEmpty()
								join f in _truckRepository.GetAllQueryable() on b.TruckC
								   equals f.TruckC into t5
								from f in t5.DefaultIfEmpty()
								join k in _driverRepository.GetAllQueryable() on b.DriverC
								   equals k.DriverC into t6
								from k in t6.DefaultIfEmpty()
								//join c in _orderHRepository.GetAllQueryable() on new {a.OrderD, a.OrderNo}
								//	equals new {c.OrderD, c.OrderNo} into t2
								//from c in t2.DefaultIfEmpty()
								join d in _expenseRepository.GetAllQueryable() on a.ExpenseC
									equals d.ExpenseC into t3
								from d in t3.DefaultIfEmpty()
								join ec in _expenseCategoryRepository.GetAllQueryable()
									on d.CategoryC equals ec.CategoryC into t7
								from ec in t7.DefaultIfEmpty()
								//join em in _employeeRepository.GetAllQueryable()
								//	on a.EntryClerkC equals em.EmployeeC into t8
								//from em in t8.DefaultIfEmpty()
								join s in _supplierRepository.GetAllQueryable() on new { a.SupplierMainC, a.SupplierSubC }
								   equals new { s.SupplierMainC, s.SupplierSubC } into t4
								from s in t4.DefaultIfEmpty()
								//where (((param.ReportI == "1" && a.InvoiceD >= param.InvoiceDFrom & a.InvoiceD <= param.InvoiceDTo) ||
								where (((param.ReportI == "1" && a.InvoiceD >= param.InvoiceDFrom & a.InvoiceD < nextDate) ||
										(param.ReportI == "0" && b.TransportD >= param.InvoiceDFrom & b.TransportD <= param.InvoiceDTo)
										) &
								  (param.ExpenseCategories == "null" || ("," + param.ExpenseCategories + ",").Contains("," + ec.CategoryC + ",")) &
								  (param.Suppliers == "null" || ("," + param.Suppliers + ",").Contains("," + a.SupplierMainC + "_" + a.SupplierSubC + ",")) &
								  (((param.ObjectI == "3") && (param.Drivers == "null" || ("," + param.Drivers + ",").Contains("," + b.DriverC + ","))) ||
								   ((param.ObjectI == "0") && (param.Trucks == "null" || ("," + param.Trucks + ",").Contains("," + b.TruckC + ","))) ||
								   ((param.ObjectI == "2") && (param.Employees == "null" || ("," + param.Employees + ",").Contains("," + a.EntryClerkC + ","))) ||
								   ((param.ObjectI == "1") && (param.Trailers == "null" || false)) ||
								   ((param.ObjectI == "9") && (param.Drivers == "null" || ("," + param.Drivers + ",").Contains("," + b.DriverC + ","))
								   && (param.Trucks == "null" || ("," + param.Trucks + ",").Contains("," + b.TruckC + ","))
								   && (param.Employees == "null" || ("," + param.Employees + ",").Contains("," + a.EntryClerkC + ","))
								   && (param.Trailers == "null" || false))) &
								  (param.PaymentMethod == "-1" || a.PaymentMethodI == param.PaymentMethod) &
								  f.PartnerI == "0" & a.IsIncluded == "1"
								 )
								select new ExpenseDetailListReport()
								{
									NetWeight = od.ContainerSizeI == "3" ? od.NetWeight : 0,
									ContainerSizeI = od.ContainerSizeI,
									ContainerNo = od.ContainerNo,
									OrderTypeI = oh.OrderTypeI,
									InvoiceD = a.InvoiceD,
									//OrderNo = a.OrderNo,
									CategoryN = ec != null ? ec.CategoryN : "",
									ExpenseN = d != null ? d.ExpenseN : "",
									PaymentMethodI = a.PaymentMethodI,
									TransportD = b != null ? b.TransportD : null,
									RegisteredNo = f != null ? f.RegisteredNo : "",
									TrailerNo = (trai != null) ? trai.TrailerNo : "",
									DriverN = k != null ? k.LastN + " " + k.FirstN : "",
									SupplierShortN = s != null ? s.SupplierShortN : "",
									SupplierN = s != null ? s.SupplierN : "",
									//EntryClerkN = em != null ? em.EmployeeLastN + " " + em.EmployeeFirstN : "",
									EntryClerkN = "",
									UnitPrice = 0,//a.UnitPrice,
									Quantity = a.Quantity,
									TotalAmount = a.Amount,
									TaxAmount = a.TaxAmount,
									Description = a.Description,
									Location1N = b.Location1N,
									Location2N = b.Location2N,
									Location3N = b.Location3N
								}).OrderBy(param.ReportI == "1" ? "InvoiceD asc" : "TransportD asc").ToList();
			// get data from TruckExpense_D
			var truckExpenseDList = (from a in _truckExpenseRepository.GetAllQueryable()
									 join b in _truckRepository.GetAllQueryable() on a.Code
										equals b.TruckC into t1
									 from b in t1.DefaultIfEmpty()
									 join tr in _trailerRepository.GetAllQueryable()
										on a.Code equals tr.TrailerC into t5
									 from tr in t5.DefaultIfEmpty()
									 join c in _expenseRepository.GetAllQueryable() on a.ExpenseC
										equals c.ExpenseC into t2
									 from c in t2.DefaultIfEmpty()
									 join d in _driverRepository.GetAllQueryable() on a.DriverC
										equals d.DriverC into t3
									 from d in t3.DefaultIfEmpty()
									 join ec in _expenseCategoryRepository.GetAllQueryable()
										on c.CategoryC equals ec.CategoryC into t7
									 from ec in t7.DefaultIfEmpty()
									 join em in _employeeRepository.GetAllQueryable()
									   on a.EntryClerkC equals em.EmployeeC into t8
									 from em in t8.DefaultIfEmpty()
									 join s in _supplierRepository.GetAllQueryable() on new { a.SupplierMainC, a.SupplierSubC }
										equals new { s.SupplierMainC, s.SupplierSubC } into t4
									 from s in t4.DefaultIfEmpty()
									 //where (((param.ReportI == "1" && a.InvoiceD >= param.InvoiceDFrom & a.InvoiceD <= param.InvoiceDTo) ||
									 where (((param.ReportI == "1" && a.InvoiceD >= param.InvoiceDFrom & a.InvoiceD < nextDate) ||
											 (param.ReportI == "0" && a.TransportD >= param.InvoiceDFrom & a.TransportD <= param.InvoiceDTo)) &
											(param.Suppliers == "null" || (a.SupplierMainC != null && a.SupplierMainC != "" && ("," + param.Suppliers + ",").Contains("," + a.SupplierMainC + "_" + a.SupplierSubC + ","))) &
											(param.ExpenseCategories == "null" || ("," + param.ExpenseCategories + ",").Contains("," + ec.CategoryC + ",")) &
											(((param.ObjectI == "3") && (param.Drivers == "null" || ("," + param.Drivers + ",").Contains("," + a.DriverC + ","))) ||
											((param.ObjectI == "0") && a.ObjectI.Equals("0") && a.Code.Equals(b.TruckC) && b.PartnerI == "0" && (param.Trucks == "null" || ("," + param.Trucks + ",").Contains("," + a.Code + ","))) ||
											((param.ObjectI == "1") && a.ObjectI.Equals("1") && a.Code.Equals(tr.TrailerC) && (param.Trailers == "null" || ("," + param.Trailers + ",").Contains("," + a.Code + ","))) ||
											((param.ObjectI == "2") && (param.Employees == "null" || ("," + param.Employees + ",").Contains("," + a.EntryClerkC + ","))) ||
											((param.ObjectI == "9") && (param.Drivers == "null" || ("," + param.Drivers + ",").Contains("," + a.DriverC + ","))
											&& (param.Trucks == "null" || (a.ObjectI.Equals("0") && a.Code.Equals(b.TruckC) && b.PartnerI == "0" && ("," + param.Trucks + ",").Contains("," + a.Code + ",")))
											&& (param.Trailers == "null" || (a.ObjectI.Equals("1") && a.Code.Equals(tr.TrailerC) && ("," + param.Trailers + ",").Contains("," + a.Code + ",")))
											&& (param.Employees == "null" || ("," + param.Employees + ",").Contains("," + a.EntryClerkC + ",")))) &
											(param.PaymentMethod == "-1" || a.PaymentMethodI == param.PaymentMethod)
										  )
									 select new ExpenseDetailListReport()
									 {
										 NetWeight = 0,
										 ContainerSizeI = "",
										 ContainerNo = "",
										 OrderTypeI = "",
										 InvoiceD = a.InvoiceD,
										 CategoryN = ec != null ? ec.CategoryN : "",
										 ExpenseN = c != null ? c.ExpenseN : "",
										 PaymentMethodI = a.PaymentMethodI,
										 TransportD = a.TransportD,
										 RegisteredNo = (b != null && a.ObjectI == "0") ? b.RegisteredNo : "",
										 TrailerNo = (tr != null && a.ObjectI == "1") ? tr.TrailerNo : "",
										 DriverN = d != null ? d.LastN + " " + d.FirstN : "",
										 SupplierShortN = s != null ? s.SupplierShortN : "",
										 SupplierN = s != null ? s.SupplierN : "",
										 EntryClerkN = em != null ? em.EmployeeLastN + " " + em.EmployeeFirstN : "",
										 //EntryClerkN = "",
										 UnitPrice = a.UnitPrice,
										 Quantity = a.Quantity,
										 TotalAmount = a.Total,
										 TaxAmount = a.Tax,
										 Description = a.Description
									 }).OrderBy(param.ReportI == "1" ? "InvoiceD asc" : "TransportD asc").ToList();
			// get data from CompanyExpense_D
			var companyExpenseDList = (from a in _companyExpenseRepository.GetAllQueryable()

									   join c in _expenseRepository.GetAllQueryable() on a.ExpenseC
										  equals c.ExpenseC into t2
									   from c in t2.DefaultIfEmpty()
									   join ec in _expenseCategoryRepository.GetAllQueryable()
										  on c.CategoryC equals ec.CategoryC into t7
									   from ec in t7.DefaultIfEmpty()
									   join em in _employeeRepository.GetAllQueryable()
										  on a.EntryClerkC equals em.EmployeeC into t8
									   from em in t8.DefaultIfEmpty()
									   join s in _supplierRepository.GetAllQueryable() on new { a.SupplierMainC, a.SupplierSubC }
										  equals new { s.SupplierMainC, s.SupplierSubC } into t4
									   from s in t4.DefaultIfEmpty()
									   //where ((a.InvoiceD >= param.InvoiceDFrom & a.InvoiceD <= param.InvoiceDTo) &
									   where ((a.InvoiceD >= param.InvoiceDFrom & a.InvoiceD < nextDate) &
											  (param.Suppliers == "null" || (a.SupplierMainC != null && a.SupplierMainC != "" && ("," + param.Suppliers + ",").Contains("," + a.SupplierMainC + "_" + a.SupplierSubC + ","))) &
											  (((param.ObjectI == "2") && (param.Employees == "null" || ("," + param.Employees + ",").Contains("," + a.EntryClerkC + ","))) ||
											  ((param.ObjectI == "3" || param.ObjectI == "1" || param.ObjectI == "0") && (false)) ||
											  ((param.ObjectI == "9") && (param.Employees == "null" || ("," + param.Employees + ",").Contains("," + a.EntryClerkC + ","))
											  && (param.Trucks == "null" || false)
											  && (param.Trailers == "null" || false)
											  && (param.Drivers == "null" || false))) &
											  (param.ExpenseCategories == "null" || ("," + param.ExpenseCategories + ",").Contains("," + ec.CategoryC + ",")) &
											  (param.PaymentMethod == "-1" || a.PaymentMethodI == param.PaymentMethod)
											)
									   select new ExpenseDetailListReport()
									   {
										   NetWeight = 0,
										   ContainerSizeI = "",
										   ContainerNo = "",
										   OrderTypeI = "",
										   InvoiceD = a.InvoiceD,
										   CategoryN = ec != null ? ec.CategoryN : "",
										   ExpenseN = c != null ? c.ExpenseN : "",
										   PaymentMethodI = a.PaymentMethodI,
										   TransportD = a.InvoiceD,
										   RegisteredNo = "",
										   TrailerNo = "",
										   DriverN = "",
										   SupplierShortN = s != null ? s.SupplierShortN : "",
										   SupplierN = s != null ? s.SupplierN : "",
										   EntryClerkN = em != null ? em.EmployeeLastN + " " + em.EmployeeFirstN : "",
										   UnitPrice = a.UnitPrice,
										   Quantity = a.Quantity,
										   TotalAmount = a.Total,
										   TaxAmount = a.Tax,
										   Description = a.Description
									   }).OrderBy(param.ReportI == "1" ? "InvoiceD asc" : "TransportD asc").ToList();
			//get data from Inspection_D
			var inspectionExpenseDList = (from a in _inspectionDetailRepository.GetAllQueryable()
										  join b in _expenseRepository.GetAllQueryable() on a.ExpenseC
												equals b.ExpenseC into t
										  join c in _truckRepository.GetAllQueryable() on a.Code
										  equals c.TruckC into t1
										  from c in t1.DefaultIfEmpty()
										  from b in t.DefaultIfEmpty()
										  join d in _trailerRepository.GetAllQueryable()
										  on a.Code equals d.TrailerC into t2
										  from d in t2.DefaultIfEmpty()
										  join dr in _driverRepository.GetAllQueryable() on a.EntryClerkC
										  equals dr.DriverC into t3
										  from dr in t3.DefaultIfEmpty()
										  join ec in _expenseCategoryRepository.GetAllQueryable()
										  on b.CategoryC equals ec.CategoryC into t4
										  from ec in t4.DefaultIfEmpty()
										  where ((a.InspectionD >= param.InvoiceDFrom & a.InspectionD <= param.InvoiceDTo)) &
										  (a.Total >= 1 && a.ExpenseC != null) &
										  (param.ExpenseCategories == "null" || ("," + param.ExpenseCategories + ",").Contains("," + b.CategoryC + ",")) &
										  (param.Suppliers == "null" || ("," + param.Suppliers + ",").Contains("," + a.SupplierMainC + "_" + a.SupplierSubC + ",")) &
										  (((param.ObjectI == "3") && (param.Drivers == "null" || ("," + param.Drivers + ",").Contains("," + a.EntryClerkC + ","))) ||
										  ((param.ObjectI == "0") && a.ObjectI.Equals("0") && a.Code.Equals(c.TruckC) && c.PartnerI == "0" && (param.Trucks == "null" || ("," + param.Trucks + ",").Contains("," + a.Code + ","))) ||
										  ((param.ObjectI == "1") && a.ObjectI.Equals("1") && a.Code.Equals(d.TrailerC) && (param.Trailers == "null" || ("," + param.Trailers + ",").Contains("," + a.Code + ","))) ||
										  ((param.ObjectI == "2") && (param.Employees == "null" || false)) ||
										  ((param.ObjectI == "9") && (param.Drivers == "null" || ("," + param.Drivers + ",").Contains("," + a.EntryClerkC + ","))
										  && (param.Trucks == "null" || (a.ObjectI.Equals("0") && a.Code.Equals(c.TruckC) && c.PartnerI == "0" && ("," + param.Trucks + ",").Contains("," + a.Code + ",")))
										  && (param.Trailers == "null" || (a.ObjectI.Equals("1") && a.Code.Equals(d.TrailerC) && ("," + param.Trailers + ",").Contains("," + a.Code + ",")))
										  && (param.Employees == "null" || false))) &
										  (param.PaymentMethod == "-1" || a.PaymentMethodI == param.PaymentMethod)
										  select new ExpenseDetailListReport()
										  {
											  NetWeight = 0,
											  ContainerSizeI = "",
											  ContainerNo = "",
											  OrderTypeI = "",
											  InvoiceD = a.InspectionD,
											  //CategoryN = "",
											  CategoryN = ec.CategoryN,
											  ExpenseN = b != null ? b.ExpenseN : "",
											  PaymentMethodI = "",
											  TransportD = a.InspectionD,
											  RegisteredNo = (c != null && a.ObjectI == "0") ? c.RegisteredNo : "",
											  TrailerNo = (d != null && a.ObjectI == "1") ? d.TrailerNo : "",
											  DriverN = dr != null ? dr.LastN + " " + dr.FirstN : "",
											  EntryClerkN = "",
											  SupplierShortN = "",
											  SupplierN = "",
											  //EntryClerkN = em != null ? em.EmployeeLastN + " " + em.EmployeeFirstN : "",
											  UnitPrice = 0,
											  Quantity = 1,
											  TotalAmount = a.Total,
											  TaxAmount = 0,
											  Description = ""

										  }).ToList();
			var returnexpenseDList = expenseDList.Concat(inspectionExpenseDList).Concat(truckExpenseDList).Concat(companyExpenseDList).ToList();
			//var result = returnexpenseDList.Where(p => p.TotalAmount > 0).ToList();
			var result = returnexpenseDList.ToList();
			result = result.OrderBy((param.ReportI == "1" ? "InvoiceD asc" : "TransportD asc"), "OrderTypeI asc", "ContainerSizeI asc").ToList();

			return result;
		}

		public Stream ExportPdfCustomersExpense(CustomerExpenseReportParam param)
		{
			Stream stream;
			DataRow row;
			CustomerRevenueGeneral.CustomerRevenueGeneralDataTable dt;
			int intLanguage;

			// get data
			dt = new CustomerRevenueGeneral.CustomerRevenueGeneralDataTable();
			List<CustomerRevenueGeneralReportData> data = GetCustomersExpenseReportList(param);

			// get language for report
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

			if (data != null && data.Count > 0)
			{
				for (int iloop = 0; iloop < data.Count; iloop++)
				{
					row = dt.NewRow();

					var customerMainC = data[iloop].CustomerMainC;
					var customerSubC = data[iloop].CustomerSubC;
					var quantity20 = 0;
					var quantity40 = 0;
					var quantity45 = 0;
					if (data[iloop].Quantity20HC != null)
					{
						quantity20 = (int)data[iloop].Quantity20HC;
					}
					if (data[iloop].Quantity40HC != null)
					{
						quantity40 = (int)data[iloop].Quantity40HC;
					}
					if (data[iloop].Quantity45HC != null)
					{
						quantity45 = (int)data[iloop].Quantity45HC;
					}
					row["QuantityContainer"] = quantity20 + quantity40 + quantity45;
					row["Quantity20"] = quantity20;
					row["Quantity40"] = quantity40;
					row["Quantity45"] = quantity45;
					row["TotalLoads"] = data[iloop].TotalLoads;
					row["Amount"] = data[iloop].Amount;
					row["TotalExpense"] = data[iloop].TotalExpense;
					row["CustomerSurcharge"] = data[iloop].CustomerSurcharge + data[iloop].DetainAmount;
					row["CustomerDiscount"] = data[iloop].CustomerDiscount;
					row["TotalAmount"] = data[iloop].TotalAmount;
					//if (data[iloop].TaxMethodI == "1")
					//{
					row["TaxAmount"] = data[iloop].TaxAmount;
					//}
					//else
					//{
					//	var amount = data[iloop].Amount ?? 0;
					//	var customerSurcharge = data[iloop].CustomerSurcharge ?? 0;
					//	row["TaxAmount"] = Utilities.CalByMethodRounding((amount + customerSurcharge) * data[iloop].TaxRate / 100, data[iloop].TaxRoundingI);
					//}

					// get customerN
					var customer = _customerRepository.Query(cus => cus.CustomerMainC == customerMainC &
																	cus.CustomerSubC == customerSubC).FirstOrDefault();
					row["CustomerN"] = customer != null ? customer.CustomerN : "";

					dt.Rows.Add(row);
				}
			}

			// set fromDate and toDate
			var fromDate = "";
			if (param.OrderDFrom != null)
			{
				if (intLanguage == 1)
				{
					fromDate = ((DateTime)param.OrderDFrom).ToString("dd/MM/yyyy");
				}
				else if (intLanguage == 2)
				{
					fromDate = ((DateTime)param.OrderDFrom).ToString("MM/dd/yyyy");
				}
				else if (intLanguage == 3)
				{
					var date = (DateTime)param.OrderDFrom;
					fromDate = date.Year + "年" + date.Month + "月" + date.Day + "日";
				}
			}
			var toDate = "";
			if (param.OrderDTo != null)
			{
				if (intLanguage == 1)
				{
					toDate = ((DateTime)param.OrderDTo).ToString("dd/MM/yyyy");
				}
				else if (intLanguage == 2)
				{
					toDate = ((DateTime)param.OrderDTo).ToString("MM/dd/yyyy");
				}
				else if (intLanguage == 3)
				{
					var date = (DateTime)param.OrderDTo;
					toDate = date.Year + "年" + ("0" + date.Month).Substring(("0" + date.Month).Length - 2) + "月" + date.Day + "日";
				}
			}

			// set month and year
			var monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
			if (intLanguage == 3)
			{
				monthYear = param.TransportM.Value.Year + "年" + param.TransportM.Value.Month + "月";
			}

			stream = CrystalReport.Service.CustomersExpense.ExportPdf.Exec(dt, intLanguage, monthYear);
			return stream;
		}

		public List<CustomerRevenueGeneralReportData> GetCustomersExpenseReportList(CustomerExpenseReportParam param)
		{
			List<CustomerRevenueGeneralReportData> result = new List<CustomerRevenueGeneralReportData>();

			// get month and year transport
			var month = param.TransportM.Value.Month;
			var year = param.TransportM.Value.Year;

			// get Detention amount
			//int beginDetentionDay = 0;
			//decimal detentionAmount = 0;
			//var basic = _basicRepository.GetAllQueryable().FirstOrDefault();
			//if (basic != null && basic.DetentionAmount != null)
			//{
			//	detentionAmount = (decimal)basic.DetentionAmount;
			//	beginDetentionDay = basic.BeginDetentionDay;
			//}

			if (param.Customer == "null")
			{
				var customerList = _customerService.GetInvoices();

				if (customerList != null && customerList.Count > 0)
				{
					param.Customer = "";
					for (var iloop = 0; iloop < customerList.Count; iloop++)
					{
						if (iloop == 0)
						{
							param.Customer = customerList[iloop].CustomerMainC + "_" + customerList[iloop].CustomerSubC;
						}
						else
						{
							param.Customer = param.Customer + "," + customerList[iloop].CustomerMainC + "_" + customerList[iloop].CustomerSubC;
						}
					}
				}
			}

			if (param.Customer != "null")
			{
				var customerArr = (param.Customer).Split(new string[] { "," }, StringSplitOptions.None);
				for (var iloop = 0; iloop < customerArr.Length; iloop++)
				{
					var arr = (customerArr[iloop]).Split(new string[] { "_" }, StringSplitOptions.None);
					var invoiceMainC = arr[0];
					var invoiceSubC = arr[1];
					var invoiceInfo = _invoiceInfoService.GetLimitStartAndEndInvoiceD(invoiceMainC, invoiceSubC, month, year);
					var startDate = invoiceInfo.StartDate.Date;
					var endDate = invoiceInfo.EndDate.Date;

					// get customers who shared a invoice company
					var customerStr = "";
					var customerList = _customerService.GetCustomersByInvoice(invoiceMainC, invoiceSubC);
					for (var aloop = 0; aloop < customerList.Count; aloop++)
					{
						customerStr = customerStr + "," + customerList[aloop].CustomerMainC + "_" + customerList[aloop].CustomerSubC;
					}

					// get orderH
					var data = from a in _orderHRepository.GetAllQueryable()
							   join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
								equals new { b.OrderD, b.OrderNo } into t1
							   from b in t1.DefaultIfEmpty()
							   where (customerStr.Contains("," + a.CustomerMainC + "_" + a.CustomerSubC) &
									  (b.RevenueD >= startDate & b.RevenueD <= endDate) &
									  (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || a.OrderDepC == param.DepC) &
									  (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || a.OrderTypeI == param.OrderTypeI)
									)
							   select new CustomerRevenueGeneralReportData()
							   {
								   InvoiceMainC = invoiceMainC,
								   InvoiceSubC = invoiceSubC,
								   ContainerSize20 = b.ContainerSizeI == "0" ? 1 : 0,
								   ContainerSize40 = b.ContainerSizeI == "1" ? 1 : 0,
								   ContainerSize45 = b.ContainerSizeI == "2" ? 1 : 0,
								   NetWeight = b.ContainerSizeI == "3" ? b.NetWeight : 0,
								   Amount = b != null ? b.Amount : 0,
								   TotalExpense = b != null ? b.TotalExpense : 0,
								   CustomerSurcharge = b != null ? b.CustomerSurcharge : 0,
								   DetainAmount = b != null ? b.DetainAmount : 0,
								   CustomerDiscount = b != null ? b.CustomerDiscount : 0,
								   TotalAmount = b != null ? b.TotalAmount : 0,
								   TaxAmount = b != null ? b.TaxAmount : 0,
							   };

					data = data.OrderBy("InvoiceMainC asc, InvoiceSubC asc");

					var groupData = from b in data
									group b by new { b.InvoiceMainC, b.InvoiceSubC }
										into c
										select new
										{
											c.Key.InvoiceMainC,
											c.Key.InvoiceSubC,
											Quantity20HC = c.Sum(b => b.ContainerSize20),
											Quantity40HC = c.Sum(b => b.ContainerSize40),
											Quantity45HC = c.Sum(b => b.ContainerSize45),
											TotalLoads = c.Sum(b => b.NetWeight),
											Amount = c.Sum(b => b.Amount),
											TotalExpense = c.Sum(b => b.TotalExpense),
											CustomerSurcharge = c.Sum(b => b.CustomerSurcharge),
											DetainAmount = c.Sum(b => b.DetainAmount),
											CustomerDiscount = c.Sum(b => b.CustomerDiscount),
											TotalAmount = c.Sum(b => b.TotalAmount),
											TaxAmount = c.Sum(b => b.TaxAmount),
										};

					var groupDataList = groupData.ToList();

					if (groupDataList.Count > 0)
					{
						var item = new CustomerRevenueGeneralReportData();
						var customerMainCTemp = groupDataList[0].InvoiceMainC;
						var customerSubCTemp = groupDataList[0].InvoiceSubC;
						item.CustomerMainC = customerMainCTemp;
						item.CustomerSubC = customerSubCTemp;
						item.Amount = groupDataList[0].Amount;
						item.TotalExpense = groupDataList[0].TotalExpense;
						item.CustomerSurcharge = groupDataList[0].CustomerSurcharge;
						item.DetainAmount = groupDataList[0].DetainAmount;
						item.CustomerDiscount = groupDataList[0].CustomerDiscount;
						item.TotalAmount = groupDataList[0].TotalAmount;
						item.TaxAmount = groupDataList[0].TaxAmount;
						item.Quantity20HC = groupDataList[0].Quantity20HC;
						item.Quantity40HC = groupDataList[0].Quantity40HC;
						item.Quantity45HC = groupDataList[0].Quantity45HC;
						item.TotalLoads = groupDataList[0].TotalLoads;
						item.TaxMethodI = invoiceInfo.TaxMethodI;
						item.TaxRate = invoiceInfo.TaxRate;
						item.TaxRoundingI = invoiceInfo.TaxRoundingI;

						// get detention amount
						//var dispatch = from a in _dispatchRepository.GetAllQueryable()
						//			   join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo }
						//				equals new { b.OrderD, b.OrderNo, b.DetailNo }
						//			   join c in _orderHRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
						//				equals new { c.OrderD, c.OrderNo }
						//			   where (customerStr.Contains("," + c.CustomerMainC + "_" + c.CustomerSubC) &
						//					  (b.RevenueD >= startDate & b.RevenueD <= endDate) &
						//					  (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || c.OrderDepC == param.DepC) &
						//					  (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || a.OrderTypeI == param.OrderTypeI) &
						//					  a.DispatchStatus == Constants.CONFIRMED
						//					)
						//			   select new DispatchViewModel()
						//			   {
						//				   OrderD = a.OrderD,
						//				   OrderNo = a.OrderNo,
						//				   DetailNo = a.DetailNo,
						//				   DetainDay = a.DetainDay
						//			   };

						//var dispatchList = (from b in dispatch
						//					group b by new { b.OrderD, b.OrderNo, b.DetailNo }
						//					into c
						//					select new
						//					{
						//						DetainDay = c.Sum(b => b.DetainDay),
						//					}).ToList();

						//if (dispatchList.Count > 0)
						//{
						//	for (var kloop = 0; kloop < dispatchList.Count; kloop++)
						//	{
						//		item.CustomerSurcharge += ((dispatchList[kloop].DetainDay - beginDetentionDay + 1) > 0 ? (dispatchList[kloop].DetainDay - beginDetentionDay + 1) : 0) * detentionAmount;
						//	}
						//}

						result.Add(item);
					}
				}
			}

			return result;
		}

		public Stream ExportPdfCustomerRevenue(CustomerExpenseReportParam param)
		{
			if (param.ReportType == 0)
			{
				return ExportPdfCustomerRevenueGeneral(param);
			}

			return ExportPdfCustomerRevenueDetail(param);
		}

		public Stream ExportPdfCustomerRevenueDetail(CustomerExpenseReportParam param)
		{
			Stream stream;
			DataRow row;
			CustomerRevenue.CustomerRevenueDataTable dt;
			int intLanguage;
			//decimal totalExpenseInRow = 0;
			//decimal totalInRow = 0;
			//decimal total = 0;
			//decimal totalTransport = 0;
			//decimal totalDetain = 0;
			//decimal totalExpense = 0;
			//decimal taxRate = 0;

			// get data
			dt = new CustomerRevenue.CustomerRevenueDataTable();
			List<CustomerRevenueReportData> data; data = GetCustomerRevenueDetailList(param);

			// get language for report
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

			if (data != null && data.Count > 0)
			{
				for (int iloop = 0; iloop < data.Count; iloop++)
				{
					row = dt.NewRow();

					row["Key"] = data[iloop].OrderH.CustomerMainC + "_" + data[iloop].OrderH.CustomerSubC;
					row["OrderD"] = Utilities.GetFormatDateReportByLanguage(data[iloop].OrderH.OrderD, intLanguage);
					row["OrderNo"] = data[iloop].OrderH.OrderNo;
					row["OrderTypeN"] = Utilities.GetOrderTypeName(data[iloop].OrderH.OrderTypeI);
					if (!string.IsNullOrEmpty(data[iloop].OrderH.LoadingPlaceN))
					{
						row["Location"] += data[iloop].OrderH.LoadingPlaceN;
					}
					if (!string.IsNullOrEmpty(data[iloop].OrderH.StopoverPlaceN))
					{
						row["Location"] += ", " + data[iloop].OrderH.StopoverPlaceN;
					}
					if (!string.IsNullOrEmpty(data[iloop].OrderH.DischargePlaceN))
					{
						row["Location"] = ", " + data[iloop].OrderH.DischargePlaceN;
					}
					row["Quantity20"] = data[iloop].OrderH.Quantity20HC != null ? data[iloop].OrderH.Quantity20HC : 0;
					row["Quantity40"] = data[iloop].OrderH.Quantity40HC != null ? data[iloop].OrderH.Quantity40HC : 0;
					row["Quantity45"] = data[iloop].OrderH.Quantity45HC != null ? data[iloop].OrderH.Quantity45HC : 0;
					row["TotalLoads"] = data[iloop].OrderH.TotalLoads != null ? data[iloop].OrderH.TotalLoads : 0;
					row["Amount"] = data[iloop].Amount;
					row["TotalExpense"] = data[iloop].TotalExpense;
					row["CustomerSurcharge"] = data[iloop].CustomerSurcharge + data[iloop].DetainAmount;
					row["CustomerDiscount"] = data[iloop].CustomerDiscount;
					row["TotalAmount"] = data[iloop].TotalAmount + data[iloop].TaxAmount - data[iloop].CustomerDiscount;
					row["DetainFee"] = data[iloop].TaxAmount;
					row["CustomerN"] = data[iloop].OrderH.CustomerN;

					dt.Rows.Add(row);
				}
			}

			// set month and year
			var monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
			if (intLanguage == 3)
			{
				monthYear = param.TransportM.Value.Year + "年" + param.TransportM.Value.Month + "月";
			}

			stream = CrystalReport.Service.CustomerRevenue.ExportPdf.Exec(dt, intLanguage, monthYear);
			return stream;
		}

		public List<CustomerRevenueReportData> GetCustomerRevenueDetailList(CustomerExpenseReportParam param)
		{
			var result = new List<CustomerRevenueReportData>();

			// get Detention amount
			//int beginDetentionDay = 0;
			//decimal detentionAmount = 0;
			//var basic = _basicRepository.GetAllQueryable().FirstOrDefault();
			//if (basic != null && basic.DetentionAmount != null)
			//{
			//	detentionAmount = (decimal)basic.DetentionAmount;
			//	beginDetentionDay = basic.BeginDetentionDay;
			//}

			var month = param.TransportM.Value.Month;
			var year = param.TransportM.Value.Year;
			// get settlement info
			var invoiceInfo = _invoiceInfoService.GetLimitStartAndEndInvoiceDSelf(month, year);
			var startDate = invoiceInfo.StartDate.Date;
			var endDate = invoiceInfo.EndDate.Date;

			if (param.Customer == "null")
			{
				var customerList = _customerService.GetInvoices();

				if (customerList != null && customerList.Count > 0)
				{
					param.Customer = "";
					for (var iloop = 0; iloop < customerList.Count; iloop++)
					{
						if (iloop == 0)
						{
							param.Customer = customerList[iloop].CustomerMainC + "_" + customerList[iloop].CustomerSubC;
						}
						else
						{
							param.Customer = param.Customer + "," + customerList[iloop].CustomerMainC + "_" + customerList[iloop].CustomerSubC;
						}
					}
				}
			}

			// get data
			if (param.Customer != "null")
			{
				var customerArr = (param.Customer).Split(new string[] { "," }, StringSplitOptions.None);
				for (var iloop = 0; iloop < customerArr.Length; iloop++)
				{
					var arr = (customerArr[iloop]).Split(new string[] { "_" }, StringSplitOptions.None);
					var customerMainC = arr[0];
					var customerSubC = arr[1];

					// get invoice name
					var nameInvoice = _customerService.GetCustomerShortName(customerMainC, customerSubC);
					if (string.IsNullOrEmpty(nameInvoice))
					{
						nameInvoice = _customerService.GetCustomerName(customerMainC, customerSubC);
					}

					// get customers who shared a invoice company
					var customerStr = "";
					var customerList = _customerService.GetCustomersByInvoice(customerMainC, customerSubC);
					for (var aloop = 0; aloop < customerList.Count; aloop++)
					{
						customerStr = customerStr + "," + customerList[aloop].CustomerMainC + "_" + customerList[aloop].CustomerSubC;
					}

					// get data
					var data = (from a in _orderHRepository.GetAllQueryable()
								join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
								 equals new { b.OrderD, b.OrderNo } into t1
								from b in t1.DefaultIfEmpty()
								where (customerStr.Contains("," + a.CustomerMainC + "_" + a.CustomerSubC) &
									   (b.RevenueD >= startDate & b.RevenueD <= endDate) &
									   (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || a.OrderDepC == param.DepC) &
									   (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || a.OrderTypeI == param.OrderTypeI)
									)
								select new CustomerRevenueReportData()
								{
									OrderH = new OrderViewModel()
									{
										OrderD = a.OrderD,
										OrderNo = a.OrderNo,
										OrderTypeI = a.OrderTypeI,
										LoadingPlaceN = a.LoadingPlaceN,
										StopoverPlaceN = a.StopoverPlaceN,
										DischargePlaceN = a.DischargePlaceN,
										CustomerMainC = customerMainC,
										CustomerSubC = customerSubC,
										CustomerN = nameInvoice,
										Quantity20HC = a.Quantity20HC,
										Quantity40HC = a.Quantity40HC,
										Quantity45HC = a.Quantity45HC,
										TotalLoads = a.TotalLoads,
									},
									Amount = 0,
									TotalExpense = 0,
									CustomerSurcharge = 0,
									CustomerDiscount = 0,
									TotalAmount = 0,
									DetainAmount = 0,
									TaxAmount = 0
								}).Distinct().AsQueryable();

					data = data.OrderBy("OrderH.CustomerMainC asc, OrderH.CustomerSubC asc, OrderH.OrderD asc, OrderH.OrderNo asc");
					var dataList = data.ToList();

					// get money
					for (var jloop = 0; jloop < dataList.Count(); jloop++)
					{
						var orderD = dataList[jloop].OrderH.OrderD;
						var orderNo = dataList[jloop].OrderH.OrderNo;
						var orderDList = _orderDRepository.Query(p => p.OrderD == orderD &&
																	  p.OrderNo == orderNo);
						if (orderDList.Any())
						{
							var groupOrderDList = from b in orderDList
												  group b by new { b.OrderD, b.OrderNo }
													  into c
													  select new
													  {
														  c.Key.OrderD,
														  c.Key.OrderNo,
														  Amount = c.Sum(b => b.Amount),
														  TotalExpense = c.Sum(b => b.TotalExpense),
														  CustomerSurcharge = c.Sum(b => b.CustomerSurcharge),
														  CustomerDiscount = c.Sum(b => b.CustomerDiscount),
														  TotalAmount = c.Sum(b => b.TotalAmount),
														  DetainAmount = c.Sum(b => b.DetainAmount),
														  TaxAmount = c.Sum(b => b.TaxAmount)
													  };
							var costList = groupOrderDList.ToList();
							if (costList.Count > 0)
							{
								dataList[jloop].Amount = costList[0].Amount != null ? (decimal)(costList[0].Amount) : 0;
								dataList[jloop].TotalExpense = costList[0].TotalExpense != null ? (decimal)(costList[0].TotalExpense) : 0;
								dataList[jloop].CustomerSurcharge = costList[0].CustomerSurcharge != null ? (decimal)(costList[0].CustomerSurcharge) : 0;
								dataList[jloop].CustomerDiscount = costList[0].CustomerDiscount != null ? (decimal)(costList[0].CustomerDiscount) : 0;
								dataList[jloop].TotalAmount = costList[0].TotalAmount != null ? (decimal)(costList[0].TotalAmount) : 0;
								dataList[jloop].DetainAmount = costList[0].DetainAmount != null ? (decimal)(costList[0].DetainAmount) : 0;
								dataList[jloop].TaxAmount = costList[0].TaxAmount != null ? (decimal)(costList[0].TaxAmount) : 0;
							}

							// get detain day
							//var dispatch = from a in _dispatchRepository.GetAllQueryable()
							//			   where (a.DispatchStatus == Constants.CONFIRMED &
							//					  a.OrderD == orderD &
							//					  a.OrderNo == orderNo)
							//			   select new DispatchViewModel()
							//			   {
							//				   OrderD = a.OrderD,
							//				   OrderNo = a.OrderNo,
							//				   DetailNo = a.DetailNo,
							//				   DetainDay = a.DetainDay,
							//			   };

							//if (dispatch.Any())
							//{
							//	var groupDispatch = from b in dispatch
							//						group b by new { b.OrderD, b.OrderNo, b.DetailNo } into c
							//						select new
							//						{
							//							DetainDay = c.Sum(b => b.DetainDay)
							//						};
							//	var detainList = groupDispatch.ToList();
							//	if (detainList.Count > 0)
							//	{
							//		for (var kloop = 0; kloop < detainList.Count; kloop++)
							//		{
							//			dataList[jloop].TotalDetain += detentionAmount * ((detainList[kloop].DetainDay - beginDetentionDay + 1) > 0 ? (detainList[kloop].DetainDay - beginDetentionDay + 1) : 0);
							//		}
							//	}
							//}

							result.Add(dataList[jloop]);
						}
					}
				}
			}

			return result;
		}

		public Stream ExportPdfCustomerRevenueGeneral(CustomerExpenseReportParam param)
		{
			Stream stream;
			DataRow row;
			CustomerRevenueGeneral.CustomerRevenueGeneralDataTable dt;
			int intLanguage;

			// get data
			dt = new CustomerRevenueGeneral.CustomerRevenueGeneralDataTable();
			List<CustomerRevenueGeneralReportData> data = GetCustomerRevenueGeneralList(param);

			// get language for report
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

			if (data != null && data.Count > 0)
			{
				for (int iloop = 0; iloop < data.Count; iloop++)
				{
					row = dt.NewRow();

					var customerMainC = data[iloop].CustomerMainC;
					var customerSubC = data[iloop].CustomerSubC;
					var quantity20 = 0;
					var quantity40 = 0;
					var quantity45 = 0;
					if (data[iloop].Quantity20HC != null)
					{
						quantity20 = (int)data[iloop].Quantity20HC;
					}
					if (data[iloop].Quantity40HC != null)
					{
						quantity40 = (int)data[iloop].Quantity40HC;
					}
					if (data[iloop].Quantity45HC != null)
					{
						quantity45 = (int)data[iloop].Quantity45HC;
					}
					row["QuantityContainer"] = quantity20 + quantity40 + quantity45;
					row["Quantity20"] = quantity20;
					row["Quantity40"] = quantity40;
					row["Quantity45"] = quantity45;
					row["TotalLoads"] = data[iloop].TotalLoads;

					row["Amount"] = data[iloop].Amount;
					row["TotalExpense"] = data[iloop].TotalExpense;
					row["CustomerSurcharge"] = (data[iloop].CustomerSurcharge ?? 0) + (data[iloop].DetainAmount ?? 0);
					row["CustomerDiscount"] = data[iloop].CustomerDiscount;
					row["TotalAmount"] = data[iloop].TotalAmount + data[iloop].TaxAmount;
					//if (data[iloop].TaxMethodI == "1")
					//{
					row["TaxAmount"] = data[iloop].TaxAmount;
					//}
					//else
					//{
					//	var amount = data[iloop].Amount ?? 0;
					//	var customerSurcharge = data[iloop].CustomerSurcharge ?? 0;
					//	row["TaxAmount"] = Utilities.CalByMethodRounding((amount + customerSurcharge) * data[iloop].TaxRate / 100, data[iloop].TaxRoundingI);
					//}

					// get customerN
					var customer = _customerRepository.Query(cus => cus.CustomerMainC == customerMainC &
																	cus.CustomerSubC == customerSubC).FirstOrDefault();
					if (customer != null)
					{
						if (!string.IsNullOrEmpty(customer.CustomerShortN))
						{
							row["CustomerN"] = customer.CustomerShortN;
						}
						else
						{
							row["CustomerN"] = customer.CustomerN;
						}
					}
					else
					{
						row["CustomerN"] = "";
					}

					dt.Rows.Add(row);
				}
			}

			// set month and year
			var monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
			if (intLanguage == 3)
			{
				monthYear = param.TransportM.Value.Year + "年" + param.TransportM.Value.Month + "月";
			}

			stream = CrystalReport.Service.CustomerRevenue.ExportPdf.Exec2(dt, intLanguage, monthYear);
			return stream;
		}

		public List<CustomerRevenueGeneralReportData> GetCustomerRevenueGeneralList(CustomerExpenseReportParam param)
		{
			var result = new List<CustomerRevenueGeneralReportData>();

			// get detetion amount from basic
			//int beginDetentionDay = 0;
			//decimal detentionAmount = 0;
			//var basic = _basicRepository.GetAllQueryable().FirstOrDefault();
			//if (basic != null && basic.DetentionAmount != null)
			//{
			//	detentionAmount = (decimal) basic.DetentionAmount;
			//	beginDetentionDay = basic.BeginDetentionDay;
			//}

			var month = param.TransportM.Value.Month;
			var year = param.TransportM.Value.Year;
			// get settlement info
			var invoiceInfo = _invoiceInfoService.GetLimitStartAndEndInvoiceDSelf(month, year);
			var startDate = invoiceInfo.StartDate.Date;
			var endDate = invoiceInfo.EndDate.Date;

			if (param.Customer == "null")
			{
				var customerList = _customerService.GetInvoices();

				if (customerList != null && customerList.Count > 0)
				{
					param.Customer = "";
					for (var iloop = 0; iloop < customerList.Count; iloop++)
					{
						if (iloop == 0)
						{
							param.Customer = customerList[iloop].CustomerMainC + "_" + customerList[iloop].CustomerSubC;
						}
						else
						{
							param.Customer = param.Customer + "," + customerList[iloop].CustomerMainC + "_" + customerList[iloop].CustomerSubC;
						}
					}
				}
			}

			// get data
			if (param.Customer != "null")
			{
				var customerArr = (param.Customer).Split(new string[] { "," }, StringSplitOptions.None);
				for (var iloop = 0; iloop < customerArr.Length; iloop++)
				{
					var arr = (customerArr[iloop]).Split(new string[] { "_" }, StringSplitOptions.None);
					var customerMainC = arr[0];
					var customerSubC = arr[1];
					var invoiceCustomer = _invoiceInfoService.GetLimitStartAndEndInvoiceD(customerMainC, customerSubC, month, year);

					// get customers who shared a invoice company
					var customerStr = "";
					var customerList = _customerService.GetCustomersByInvoice(customerMainC, customerSubC);
					for (var aloop = 0; aloop < customerList.Count; aloop++)
					{
						customerStr = customerStr + "," + customerList[aloop].CustomerMainC + "_" + customerList[aloop].CustomerSubC;
					}

					var data = from a in _orderHRepository.GetAllQueryable()
							   join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
								equals new { b.OrderD, b.OrderNo } into t1
							   from b in t1.DefaultIfEmpty()
							   where (customerStr.Contains("," + a.CustomerMainC + "_" + a.CustomerSubC) &
									 (b.RevenueD >= startDate & b.RevenueD <= endDate) &
									 (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || a.OrderDepC == param.DepC) &
									 (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || a.OrderTypeI == param.OrderTypeI)
								)
							   select new CustomerRevenueGeneralReportData()
							   {
								   InvoiceMainC = customerMainC,
								   InvoiceSubC = customerSubC,
								   ContainerSize20 = b.ContainerSizeI == "0" ? 1 : 0,
								   ContainerSize40 = b.ContainerSizeI == "1" ? 1 : 0,
								   ContainerSize45 = b.ContainerSizeI == "2" ? 1 : 0,
								   TotalLoads = b.ContainerSizeI == "3" ? b.NetWeight : 0,
								   Amount = b != null ? b.Amount : 0,
								   TotalExpense = b != null ? b.TotalExpense : 0,
								   CustomerSurcharge = b != null ? b.CustomerSurcharge : 0,
								   DetainAmount = b != null ? b.DetainAmount : 0,
								   CustomerDiscount = b != null ? b.CustomerDiscount : 0,
								   TotalAmount = b != null ? b.TotalAmount : 0,
								   TaxAmount = b != null ? b.TaxAmount : 0,
							   };

					data = data.OrderBy("InvoiceMainC asc, InvoiceSubC asc");

					var groupData = from b in data
									group b by new { b.InvoiceMainC, b.InvoiceSubC }
										into c
										select new
										{
											c.Key.InvoiceMainC,
											c.Key.InvoiceSubC,
											Quantity20HC = c.Sum(b => b.ContainerSize20),
											Quantity40HC = c.Sum(b => b.ContainerSize40),
											Quantity45HC = c.Sum(b => b.ContainerSize45),
											TotalLoads = c.Sum(b => b.TotalLoads),
											Amount = c.Sum(b => b.Amount),
											TotalExpense = c.Sum(b => b.TotalExpense),
											CustomerSurcharge = c.Sum(b => b.CustomerSurcharge),
											DetainAmount = c.Sum(b => b.DetainAmount),
											CustomerDiscount = c.Sum(b => b.CustomerDiscount),
											TotalAmount = c.Sum(b => b.TotalAmount),
											TaxAmount = c.Sum(b => b.TaxAmount)
										};

					var groupDataList = groupData.ToList();
					if (groupDataList.Count > 0)
					{
						var item = new CustomerRevenueGeneralReportData();
						var customerMainCTemp = customerMainC;
						var customerSubCTemp = customerSubC;
						item.CustomerMainC = customerMainCTemp;
						item.CustomerSubC = customerSubCTemp;
						item.Amount = groupDataList[0].Amount;
						item.TotalExpense = groupDataList[0].TotalExpense;
						item.CustomerSurcharge = groupDataList[0].CustomerSurcharge;
						item.DetainAmount = groupDataList[0].DetainAmount;
						item.CustomerDiscount = groupDataList[0].CustomerDiscount;
						item.TotalAmount = groupDataList[0].TotalAmount;
						item.TaxAmount = groupDataList[0].TaxAmount;
						item.Quantity20HC = groupDataList[0].Quantity20HC;
						item.Quantity40HC = groupDataList[0].Quantity40HC;
						item.Quantity45HC = groupDataList[0].Quantity45HC;
						item.TotalLoads = groupDataList[0].TotalLoads;
						item.TaxMethodI = invoiceCustomer.TaxMethodI;
						item.TaxRate = invoiceCustomer.TaxRate;
						item.TaxRoundingI = invoiceCustomer.TaxRoundingI;

						// get detain total
						//var detain = (from a in _orderHRepository.GetAllQueryable()
						//		   join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
						//			equals new { b.OrderD, b.OrderNo } into t1
						//		   from b in t1.DefaultIfEmpty()
						//		   join c in _dispatchRepository.GetAllQueryable() on new { b.OrderD, b.OrderNo, b.DetailNo }
						//				equals new { c.OrderD, c.OrderNo, c.DetailNo } into t2
						//			   from c in t2.DefaultIfEmpty()
						//		   where (customerStr.Contains("," + a.CustomerMainC + "_" + a.CustomerSubC) &
						//				 (b.RevenueD >= startDate & b.RevenueD <= endDate) &
						//				 (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || a.OrderDepC == param.DepC) &
						//				 (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || a.OrderTypeI == param.OrderTypeI)
						//			)
						//			group c by new { c.OrderD, c.OrderNo, c.DetailNo } into g
						//		   select new DispatchViewModel()
						//		   {
						//			   OrderD = g.Key.OrderD,
						//			   OrderNo = g.Key.OrderNo,
						//			   DetailNo = g.Key.DetailNo,
						//			   DetainDay = g.Sum(b => b.DetainDay)
						//		   });

						//List<DispatchViewModel> detainList = detain.ToList();
						//if (detainList.Count > 0)
						//{
						//	for (var kloop = 0; kloop < detainList.Count; kloop++)
						//	{
						//		item.TotalDetain += detentionAmount * ((detainList[kloop].DetainDay - beginDetentionDay + 1) > 0 ? (detainList[kloop].DetainDay - beginDetentionDay + 1) : 0);
						//	}
						//}

						result.Add(item);
					}
				}
			}

			return result;
		}

		public Stream ExportPdfCustomerBalance(CustomerExpenseReportParam param, string userName)
		{
			Stream stream;
			DataRow row;
			CustomerBalance.CustomerBalanceDataTable dt = new CustomerBalance.CustomerBalanceDataTable();
			Dictionary<string, string> dicLanguage = new Dictionary<string, string>();
			int intLanguage;
			var companyName = "";
			var companyAddress = "";
			var fileName = "";
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
			// get data
			List<CustomerBalanceReportData> data = GetCustomerBalanceList(param);

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
				//(con.ScreenID == 99) &&
																 (con.TextKey == "LBLEXPENSE" ||
																  con.TextKey == "LBLPROFIT" ||
																  con.TextKey == "LBLTRANSPORTFEE" ||
																  con.TextKey == "LBLPROFIT/REVENUE" ||
																  con.TextKey == "LBLINCLUDEDEXPENSE" ||
																  con.TextKey == "LBLDETAINFEE" ||
																  con.TextKey == "LBLSURCHARGEREPORT" ||
																  con.TextKey == "LBLPARTNERFEE" ||
																  con.TextKey == "LBLTOLCUSTOMERDISCOUNT" ||
																  con.TextKey == "LBLALLOWANCE" ||
																  con.TextKey == "LBLPAYONBEHALF" ||
																  con.TextKey == "RPHDREVENUE" ||
																  con.TextKey == "LBLOTHER" ||
																  con.TextKey == "LBLALLOCATIONI" ||
																  con.TextKey == "LBLDIRECTEXPENSE" ||

																  con.TextKey == "MNUCUSTOMERBALANCEREPORT" ||
																  con.TextKey == "LBLCUSTOMER" ||
																  con.TextKey == "LBLINCOME" ||
																  con.TextKey == "LBLEXPENSERP" ||
																  con.TextKey == "LBLPROFIT" ||
																  con.TextKey == "LBLITEMRP" ||
																  con.TextKey == "LBLAMOUNTMONEY" ||
																  con.TextKey == "LBLENTRYCLERKN" ||
																  con.TextKey == "RPFTPAGE" ||
																  con.TextKey == "RPFTPRINTBY" ||
																  con.TextKey == "RPFTPRINTTIME" ||
																  con.TextKey == "LBLDISPATCHTOTALRP"
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
				var invoiceInfo = _invoiceInfoService.GetLimitStartAndEndInvoiceDSelf(param.TransportM.Value.Month, param.TransportM.Value.Year);
				var startDate = invoiceInfo.StartDate.Date;
				var endDate = invoiceInfo.EndDate.Date;
				decimal totalCompanyExpense = GetTotalCompanyExpenseForCompanyBalance(startDate, endDate);
				decimal totalCustomerRevenue = GetTotalCustomerRevenueForCompanyBalance(startDate, endDate);//data.Sum(x => x.Amount ?? 0);
				for (int i = 0; i < data.Count; i++)
				{
					var customerMainC = data[i].CustomerMainC;
					var customerSubC = data[i].CustomerSubC;
					var customerSurchargeList = data[0].CustomerSurchargeList.Where(x => x.CustomerMainC == customerMainC &&
																						 x.CustomerSubC == customerSubC &&
																						 x.Value > 0)
																			 .OrderBy("Name").ToList();
					var totalExpenseList = data[0].TotalExpenseList.Where(x => x.CustomerMainC == customerMainC &&
																			   x.CustomerSubC == customerSubC &&
																			   x.Value > 0)
																	.OrderBy("Name").ToList();
					var includedExpenseList = data[0].IncludedExpenseList.Where(x => x.CustomerMainC == customerMainC &&
																					 x.CustomerSubC == customerSubC &&
																					 x.Value > 0)
																		 .OrderBy("Name").ToList();


					int totalExpenseCount = totalExpenseList.Count();
					int totalSurchargeCount = customerSurchargeList.Count();
					int totalIncludedCount = includedExpenseList.Count();

					int totalRow = Math.Max(totalIncludedCount + 4, totalExpenseCount + totalSurchargeCount + 1);

					for (int rowindex = 0; rowindex < totalRow; rowindex++)
					{
						//decimal transportFee = 0;
						decimal detainAmount = 0;
						decimal totalExpense = 0;
						decimal customerSurcharge = 0;
						decimal amount = 0;
						decimal customerDiscount = 0;
						decimal includedExpense = 0;
						decimal profit;
						decimal allocatedCost = 0;

						row = dt.NewRow();
						row["Key"] = customerMainC + "_" + customerSubC;
						row["CustomerN"] = data[i].CustomerN;

						// set money
						decimal driverAllowance = data[i].DriverAllowance != null ? (decimal)data[i].DriverAllowance : 0;
						//transportFee = data[i].TransportFee != null ? (decimal) data[i].TransportFee : 0;
						//detainAmount = data[i].DetainAmount != null ? (decimal)data[i].DetainAmount : 0;
						//totalExpense = data[i].TotalExpense != null ? (decimal)data[i].TotalExpense : 0;
						//customerSurcharge = data[i].CustomerSurcharge != null ? (decimal)data[i].CustomerSurcharge : 0;
						amount = data[i].Amount != null ? (decimal)data[i].Amount : 0;
						allocatedCost = (totalCustomerRevenue == 0) ? 0 : Utilities.CalByMethodRounding(totalCompanyExpense * amount / totalCustomerRevenue, "0");

						customerDiscount = data[i].CustomerDiscount != null ? (decimal)data[i].CustomerDiscount : 0;
						includedExpense = data[i].IncludedExpense != null ? (decimal)data[i].IncludedExpense : 0;
						decimal partnerAmount = data[i].PartnerAmount != null ? (decimal)data[i].PartnerAmount : 0;

						row["TotalIncome"] = (amount + detainAmount + totalExpense + customerSurcharge).ToString("#,###", cul.NumberFormat);
						row["TotalExpense"] = (driverAllowance + customerDiscount + partnerAmount + includedExpense).ToString("#,###", cul.NumberFormat);
						profit = amount + detainAmount + totalExpense + customerSurcharge -
								driverAllowance - customerDiscount - partnerAmount - includedExpense;

						if (rowindex == 0)
						{
							row["IncomeN"] = dicLanguage["RPHDREVENUE"];
							row["IncomeAmount"] = amount.ToString("#,###", cul.NumberFormat);

							row["ExpenseN"] = "<b>" + dicLanguage["LBLALLOWANCE"] + "</b>";
							row["ExpenseAmount"] = driverAllowance.ToString("#,###", cul.NumberFormat);

							row["ProfitN"] = dicLanguage["LBLPROFIT"];
							row["ProfitAmount"] = profit.ToString("#,###", cul.NumberFormat); ;
						}
						else if (rowindex == 1)
						{
							//row["IncomeN"] = "<b>" + dicLanguage["LBLDETAINFEE"] + "</b>";
							//row["IncomeAmount"] = detainAmount.ToString("#,###", cul.NumberFormat);

							//row["ExpenseN"] = "<b>" + dicLanguage["LBLTOLCUSTOMERDISCOUNT"] + "</b>";
							//row["ExpenseAmount"] = customerDiscount.ToString("#,###", cul.NumberFormat);
							row["ExpenseN"] = "<b>" + dicLanguage["LBLPARTNERFEE"] + "</b>";
							row["ExpenseAmount"] = partnerAmount.ToString("#,###", cul.NumberFormat);

							row["ProfitN"] = dicLanguage["LBLPROFIT/REVENUE"];
							if (amount != 0) // + detainAmount + totalExpense + customerSurcharge 
							{
								row["ProfitAmount"] = Utilities.CalByMethodRounding(100 * profit / (amount + detainAmount + totalExpense + customerSurcharge), "0") + "%";
							}
						}
						else if (rowindex == 2)
						{
							//row["IncomeN"] = "<b>" + dicLanguage["LBLPAYONBEHALF"] + "</b>";
							//row["IncomeAmount"] = totalExpense.ToString("#,###", cul.NumberFormat);

							//row["ExpenseN"] = "<b>" + dicLanguage["LBLINCLUDEDEXPENSE"] + "</b>";
							//row["ExpenseAmount"] = includedExpense.ToString("#,###", cul.NumberFormat);
							//row["ExpenseN"] = "<b>" + dicLanguage["LBLPARTNERFEE"] + "</b>";
							//row["ExpenseAmount"] = partnerAmount.ToString("#,###", cul.NumberFormat);
							row["ExpenseN"] = "<b>" + dicLanguage["LBLALLOCATIONI"] + "</b>";
							row["ExpenseAmount"] = allocatedCost.ToString("#,###", cul.NumberFormat);
						}
						else
						{
							int index = rowindex - 3;
							//if (index < totalExpenseCount)
							//{
							//row["IncomeN"] = totalExpenseList[index].Name;
							//row["IncomeAmount"] = totalExpenseList[index].Value != null ? ((decimal)totalExpenseList[index].Value).ToString("#,###", cul.NumberFormat) : null;
							//}
							//else if (index == totalExpenseCount)
							//{
							//row["IncomeN"] = "<b>" + dicLanguage["LBLSURCHARGEREPORT"] + "</b>";
							//row["IncomeAmount"] = data[i].CustomerSurcharge != null ? ((decimal)data[i].CustomerSurcharge).ToString("#,###", cul.NumberFormat) : null;
							//}
							//else if ((index - totalExpenseCount - 1 ) < totalSurchargeCount)
							//{
							//	int index2 = index - totalExpenseCount - 1; 
							//row["IncomeN"] = customerSurchargeList[index2].Name;
							//row["IncomeAmount"] = customerSurchargeList[index2].Value != null ? ((decimal)customerSurchargeList[index2].Value).ToString("#,###", cul.NumberFormat) : null;
							//}
							if (rowindex == 3)
							{
								row["ExpenseN"] = "<b>" + dicLanguage["LBLDIRECTEXPENSE"] + "</b>";
								row["ExpenseAmount"] = includedExpense.ToString("#,###", cul.NumberFormat);
							}
							else if ((index - 1) < totalIncludedCount)
							{
								int index3 = index - 1;
								row["ExpenseN"] = includedExpenseList[index3].Name == "" ? dicLanguage["LBLOTHER"] : includedExpenseList[index3].Name;
								row["ExpenseAmount"] = includedExpenseList[index3].Value != null ? ((decimal)includedExpenseList[index3].Value).ToString("#,###", cul.NumberFormat) : null;
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

			stream = CrystalReport.Service.CustomerBalance.ExportPdf.Exec(dt, intLanguage, monthYear, companyName, companyAddress, fileName, user, dicLanguage);
			return stream;
		}

		private List<CustomerBalanceReportData> GetCustomerBalanceList(CustomerExpenseReportParam param)
		{
			var result = new List<CustomerBalanceReportData>();

			// Lấy danh sách customer
			var month = param.TransportM.Value.Month;
			var year = param.TransportM.Value.Year;
			// get settlement info
			var invoiceInfo = _invoiceInfoService.GetLimitStartAndEndInvoiceDSelf(month, year);
			var startDate = invoiceInfo.StartDate.Date;
			var endDate = invoiceInfo.EndDate.Date;

			// get basic info
			//int beginDetentionDay = 1;
			//decimal detentionAmount = 0;
			//var basic = _basicRepository.GetAllQueryable().FirstOrDefault();
			//if (basic != null && !string.IsNullOrEmpty(basic.Logo))
			//{
			//	detentionAmount = basic.DetentionAmount != null ? (decimal)basic.DetentionAmount : 0;
			//	beginDetentionDay = basic.BeginDetentionDay;
			//}

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
				var customerArr = (param.Customer).Split(new string[] { "," }, StringSplitOptions.None);

				var totalExpenseList = new List<CustomerBalanceReportExpense>();
				var customerSurchargeList = new List<CustomerBalanceReportExpense>();
				var includedExpenseList = new List<CustomerBalanceReportExpense>();

				// xu ly tung khach hang
				for (var iloop = 0; iloop < customerArr.Length; iloop++)
				{
					var arr = (customerArr[iloop]).Split(new string[] { "_" }, StringSplitOptions.None);
					var invoiceMainC = arr[0];
					var invoiceSubC = arr[1];

					// get invoice name
					var invoiceName = _customerService.GetCustomerShortName(invoiceMainC, invoiceSubC);
					if (string.IsNullOrEmpty(invoiceName))
					{
						invoiceName = _customerService.GetCustomerName(invoiceMainC, invoiceSubC);
					}

					// get customers who shared a invoice company
					var customerStr = "";
					var customerList = _customerService.GetCustomersByInvoice(invoiceMainC, invoiceSubC);
					for (var aloop = 0; aloop < customerList.Count; aloop++)
					{
						customerStr = customerStr + "," + customerList[aloop].CustomerMainC + "_" + customerList[aloop].CustomerSubC;
					}

					// get data for 1 invoice
					#region getdata
					var data = (from a in _orderHRepository.GetAllQueryable()
								join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo }
									equals new { b.OrderD, b.OrderNo } into t1
								from b in t1.DefaultIfEmpty()
								where (customerStr.Contains("," + a.CustomerMainC + "_" + a.CustomerSubC) &
									   (b.RevenueD >= startDate & b.RevenueD <= endDate) &
									   (string.IsNullOrEmpty(param.DepC) || param.DepC == "0" || a.OrderDepC == param.DepC) &
									   (string.IsNullOrEmpty(param.OrderTypeI) || param.OrderTypeI == "-1" || a.OrderTypeI == param.OrderTypeI)
									)
								select new CustomerBalanceReportData()
								{
									OrderD = a.OrderD,
									OrderNo = a.OrderNo,
									DetailNo = b.DetailNo,
									CustomerMainC = invoiceMainC,
									CustomerSubC = invoiceSubC,
									CustomerN = invoiceName,
									Amount = b.Amount + b.DetainAmount + b.CustomerSurcharge,// + b.TaxAmount,
									//TransportFee = 0,
									PartnerAmount = b.PartnerAmount,// - b.PartnerDiscount + b.PartnerTaxAmount,
									TotalExpense = b.TotalExpense,
									//CustomerSurcharge = b.CustomerSurcharge,
									//CustomerDiscount = b.CustomerDiscount,
									//DetainAmount = b.DetainAmount,
									IncludedExpense = b.TotalCost + b.PartnerExpense,
									DriverAllowance = b.TotalDriverAllowance,
								}).AsQueryable();

					var dataList = data.ToList();

					// lay danh sach chi tiet
					for (var jloop = 0; jloop < dataList.Count(); jloop++)
					{
						var orderD = dataList[jloop].OrderD;
						var orderNo = dataList[jloop].OrderNo;
						var detailNo = dataList[jloop].DetailNo;


						//var dispatch = from a in _dispatchRepository.GetAllQueryable()
						//			   where (a.OrderD == orderD &&
						//					  a.OrderNo == orderNo &&
						//					  a.DetailNo == detailNo
						//			   )
						//			   select new DispatchViewModel()
						//			   {
						//				   OrderD = a.OrderD,
						//				   OrderNo = a.OrderNo,
						//				   DetailNo = a.DetailNo,
						//				   DetainDay = a.DetainDay,
						//				   TransportFee = a.TransportFee
						//			   };

						//var dispatchList = (from b in dispatch
						//					group b by new { b.OrderD, b.OrderNo, b.DetailNo }
						//						into c
						//						select new
						//						{
						//							DetainDay = c.Sum(b => b.DetainDay),
						//							TransportFee = c.Sum(b => b.TransportFee)
						//						}).ToList();

						//if (dispatchList.Count > 0)
						//{
						//	dataList[jloop].DetainAmount = ((dispatchList[0].DetainDay - beginDetentionDay + 1) > 0 ? (dispatchList[0].DetainDay - beginDetentionDay + 1) : 0) * detentionAmount;
						//	dataList[jloop].TransportFee = dispatchList[0].TransportFee;
						//}

						#region CÁC KHOẢN THU
						//totalExpenseList.AddRange(
						//	(from a in _expenseRepository.GetAllQueryable()
						//		join b in _expenseDetailRepository.GetAllQueryable() on a.ExpenseC equals b.ExpenseC into t1
						//		from b in t1.DefaultIfEmpty()
						//		where (b.OrderD == orderD &&
						//				 b.OrderNo == orderNo &&
						//				 b.DetailNo == detailNo &&
						//				 (b.IsRequested == "1")
						//			)
						//		select
						//			new CustomerBalanceReportExpense()
						//			{
						//				CustomerMainC = invoiceMainC,
						//				CustomerSubC = invoiceSubC,
						//				Name = a.ExpenseN,
						//				Value = b.Amount
						//			}).ToList()
						//	);

						//customerSurchargeList.AddRange(
						//	(from a in _expenseRepository.GetAllQueryable()
						//		join b in _surchargeDetailRepository.GetAllQueryable() on a.ExpenseC equals b.SurchargeC into t1
						//		from b in t1.DefaultIfEmpty()
						//		where (b.OrderD == orderD &&
						//				 b.OrderNo == orderNo &&
						//				 b.DetailNo == detailNo
						//			)
						//		select
						//			new CustomerBalanceReportExpense()
						//			{
						//				CustomerMainC = invoiceMainC,
						//				CustomerSubC = invoiceSubC,
						//				Name = a.ExpenseN,
						//				Value = b.Amount
						//			}).ToList()
						//);

						#endregion

						#region CAC KHOAN CHI
						// Lấy chi phí chi
						var includedExpenseData = (from a in _expenseRepository.GetAllQueryable()
												   join b in _expenseDetailRepository.GetAllQueryable() on a.ExpenseC equals b.ExpenseC into t1
												   from b in t1.DefaultIfEmpty()
												   join c in _expenseCategoryRepository.GetAllQueryable() on a.CategoryC equals c.CategoryC into t2
												   from c in t2.DefaultIfEmpty()
												   where (b.OrderD == orderD &&
															 b.OrderNo == orderNo &&
															 b.DetailNo == detailNo &&
															 b.IsIncluded == "1"
													   )
												   select new CustomerBalanceReportExpense()
												   {
													   CustomerMainC = invoiceMainC,
													   CustomerSubC = invoiceSubC,
													   Name = c != null ? c.CategoryN : "",
													   Value = b.Amount
												   });

						includedExpenseList.AddRange(
												(from a in includedExpenseData
												 group a by new { a.CustomerMainC, a.CustomerSubC, a.Name } into g
												 select new CustomerBalanceReportExpense()
												 {
													 CustomerMainC = g.Key.CustomerMainC,
													 CustomerSubC = g.Key.CustomerSubC,
													 Name = g.Key.Name,
													 Value = g.Sum(x => x.Value)
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
							  group b by new { b.CustomerMainC, b.CustomerSubC, b.CustomerN }
								  into c
								  select new CustomerBalanceReportData()
								  {
									  CustomerMainC = c.Key.CustomerMainC,
									  CustomerSubC = c.Key.CustomerSubC,
									  CustomerN = c.Key.CustomerN,
									  Amount = c.Sum(b => b.Amount),
									  //TransportFee = c.Sum(b => b.TransportFee),
									  PartnerAmount = c.Sum(b => b.PartnerAmount),
									  TotalExpense = c.Sum(b => b.TotalExpense),
									  CustomerSurcharge = c.Sum(b => b.CustomerSurcharge),
									  CustomerDiscount = c.Sum(b => b.CustomerDiscount),
									  DetainAmount = c.Sum(b => b.DetainAmount),
									  IncludedExpense = c.Sum(b => b.IncludedExpense),
									  DriverAllowance = c.Sum(b => b.DriverAllowance),
								  }).ToList();

					result[0].TotalExpenseList = (from b in totalExpenseList
												  group b by new { b.CustomerMainC, b.CustomerSubC, b.Name }
													  into c
													  select new CustomerBalanceReportExpense()
													  {
														  CustomerMainC = c.Key.CustomerMainC,
														  CustomerSubC = c.Key.CustomerSubC,
														  Name = c.Key.Name,
														  Value = c.Sum(b => b.Value),
													  }).ToList();
					result[0].CustomerSurchargeList = (from b in customerSurchargeList
													   group b by new { b.CustomerMainC, b.CustomerSubC, b.Name }
														   into c
														   select new CustomerBalanceReportExpense()
														   {
															   CustomerMainC = c.Key.CustomerMainC,
															   CustomerSubC = c.Key.CustomerSubC,
															   Name = c.Key.Name,
															   Value = c.Sum(b => b.Value),
														   }).ToList();
					result[0].IncludedExpenseList = (from b in includedExpenseList
													 group b by new { b.CustomerMainC, b.CustomerSubC, b.Name }
														 into c
														 select new CustomerBalanceReportExpense()
														 {
															 CustomerMainC = c.Key.CustomerMainC,
															 CustomerSubC = c.Key.CustomerSubC,
															 Name = c.Key.Name,
															 Value = c.Sum(b => b.Value),
														 }).ToList();
				}

				#endregion
			}
			return result;
		}

		private decimal GetTotalCustomerRevenueForCompanyBalance(DateTime startDate, DateTime endDate)
		{
			var data = (from b in _orderDRepository.GetAllQueryable()
						where b.RevenueD >= startDate & b.RevenueD <= endDate
						select new CustomerBalanceReportData()
						{
							Amount = b.Amount + b.DetainAmount + b.CustomerSurcharge,// + b.TaxAmount,
						}).ToList();
			return data.Sum(x => x.Amount ?? 0);
		}
		private decimal GetTotalCompanyExpenseForCompanyBalance(DateTime startDate, DateTime endDate)
		{
			var expenseD = (from a in _expenseRepository.GetAllQueryable()
							join e in _truckExpenseRepository.GetAllQueryable() on a.ExpenseC equals e.ExpenseC into t3
							from e in t3.DefaultIfEmpty()
							join c in _expenseCategoryRepository.GetAllQueryable() on a.CategoryC equals c.CategoryC into t2
							from c in t2.DefaultIfEmpty()
							where (e.InvoiceD >= startDate) &
									(e.InvoiceD <= endDate) &
									e.IsAllocated.Equals("0")
							select new CustomerBalanceReportExpense()
							{
								Name = c != null ? c.CategoryN : "",
								Value = e.Total,
							}).Concat(from a in _expenseRepository.GetAllQueryable()
									  join e in _companyExpenseRepository.GetAllQueryable() on a.ExpenseC equals e.ExpenseC into t3
									  from e in t3.DefaultIfEmpty()
									  join c in _expenseCategoryRepository.GetAllQueryable() on a.CategoryC equals c.CategoryC into t2
									  from c in t2.DefaultIfEmpty()
									  where (e.InvoiceD >= startDate) &
											  (e.InvoiceD <= endDate) &
											  e.IsAllocated.Equals("0")
									  select new CustomerBalanceReportExpense()
									  {
										  Name = c != null ? c.CategoryN : "",
										  Value = e.Total,
									  });
			var expenseData = (from a in expenseD
							   group a by new { a.Name } into g
							   select new CustomerBalanceReportExpense()
							   {
								   Name = g.Key.Name,
								   Value = g.Sum(x => x.Value),
							   }).ToList();

			// Lấy ds chi phí sữa chữa (dang update table, tam thoi = 0)
			var maintenance = new List<CustomerBalanceReportExpense>();
			// Lấy ds chi phí cố định
			var month = startDate.Month;
			var fixedExpense = (from a in _fixedExpenseRepository.GetAllQueryable()
								join b in _expenseRepository.GetAllQueryable() on a.ExpenseC equals b.ExpenseC into t1
								from b in t1
								where (a.Year == endDate.Year)
								select new CustomerBalanceReportExpense()
								{
									Name = b.ExpenseN,
									Value = (month == 1 ? a.Month1 : 0) +
												(month == 2 ? a.Month2 : 0) +
												(month == 3 ? a.Month3 : 0) +
												(month == 4 ? a.Month4 : 0) +
												(month == 5 ? a.Month5 : 0) +
												(month == 6 ? a.Month6 : 0) +
												(month == 7 ? a.Month7 : 0) +
												(month == 8 ? a.Month8 : 0) +
												(month == 9 ? a.Month9 : 0) +
												(month == 10 ? a.Month10 : 0) +
												(month == 11 ? a.Month11 : 0) +
												(month == 12 ? a.Month12 : 0)
								}).ToList();

			return expenseData.Sum(x => x.Value ?? 0) + fixedExpense.Sum(x => x.Value ?? 0);
		}

		private decimal GetTotalTruckRevenueForTruckBalance(DateTime startDate, DateTime endDate)
		{
			var incomData = (from b in _dispatchRepository.GetAllQueryable()
							 where ((startDate == null || b.TransportD >= startDate) &
									(endDate == null || b.TransportD <= endDate)
							   )
							 select new TruckBalanceReportTransportFee
							 {
								 TransportFee = b.TransportFee
							 }).ToList();
			return incomData.Sum(x => x.TransportFee ?? 0);
		}

		private decimal GetTotalCompanyExpenseForTruckBalance(DateTime startDate, DateTime endDate)
		{
			var expenseD = (from a in _expenseRepository.GetAllQueryable()
							join e in _companyExpenseRepository.GetAllQueryable() on a.ExpenseC equals e.ExpenseC into t3
							from e in t3.DefaultIfEmpty()
							join c in _expenseCategoryRepository.GetAllQueryable() on a.CategoryC equals c.CategoryC into t2
							from c in t2.DefaultIfEmpty()
							where (e.InvoiceD >= startDate) &
									(e.InvoiceD <= endDate) &
									e.IsAllocated.Equals("0")
							select new CustomerBalanceReportExpense()
							{
								Name = c != null ? c.CategoryN : "",
								Value = e.Total,
							});
			var expenseData = (from a in expenseD
							   group a by new { a.Name } into g
							   select new CustomerBalanceReportExpense()
							   {
								   Name = g.Key.Name,
								   Value = g.Sum(x => x.Value),
							   }).ToList();

			return expenseData.Sum(x => x.Value ?? 0);
		}

		#region Truck
		public Stream ExportPdfTruckRevenueDetail(TruckRevenueReportParam param)
		{
			Stream stream;
			DataRow row;
			TruckRevenue.TruckRevenueDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			decimal total20 = 0;
			decimal total40 = 0;
			decimal total45 = 0;
			decimal totalAmount = 0;

			// get data
			dt = new TruckRevenue.TruckRevenueDataTable();
			List<TruckRevenueReportData> data;
			data = GetTruckRevenueListDetail(param);

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
																 (con.TextKey == "LBLIMPORT" ||
																  con.TextKey == "LBLEXPORT" ||
																  con.TextKey == "LBLOTHER"
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
				totalAmount = data.Sum(m => m.Amount);
				total20 = data.Sum(m => m.ContainerSize20);
				total40 = data.Sum(m => m.ContainerSize40);
				total45 = data.Sum(m => m.ContainerSize45);
				int index = 1;
				var listDate = data.GroupBy(item => item.TransportD)
					.Select(grp => grp.First())
					.OrderBy(item => item.TransportD)
					.ToList();

				foreach (var currentDay in listDate)
				{
					var allowanceInDay = from item in data
										 where item.TransportD == currentDay.TransportD
										 select item;
					bool isFirst = true;
					foreach (var item in allowanceInDay)
					{
						row = dt.NewRow();

						if (isFirst)
						{
							row["No"] = index++;


							isFirst = false;
						}
						else
						{
							row["No"] = string.Empty;
						}
						row["TransportD"] = item.TransportD == null
								? ""
								: Utilities.GetFormatDateReportByLanguage(item.TransportD ?? DateTime.Now, intLanguage);
						row["Day"] = item.TransportD == null
							? ""
							: Utilities.GetFormatDateReportByLanguage(item.TransportD ?? DateTime.Now, intLanguage);
						row["OrderTypeI"] = item.OrderTypeId == "0"
																	? dicLanguage["LBLEXPORT"]
																	: item.OrderTypeId == "1"
																		? dicLanguage["LBLIMPORT"]
																		: dicLanguage["LBLOTHER"];
						row["LocationN"] = item.Location;
						row["20HC"] = item.ContainerSize20.ToString("####", cul.NumberFormat);
						row["40HC"] = item.ContainerSize40.ToString("####", cul.NumberFormat);
						row["45HC"] = item.ContainerSize45.ToString("####", cul.NumberFormat);
						row["Load"] = item.Load;
						row["Amount"] = item.Amount;
						row["RegisteredNo"] = item.RegisteredNo;

						dt.Rows.Add(row);
					}

				}
			}

			stream = CrystalReport.Service.TruckRevenue.ExportPdf.Exec(dt, intLanguage, param.DateFrom, param.DateTo, total20, total40, total45, totalAmount);
			return stream;
		}
		public Stream ExportPdfTruckRevenueGeneral(TruckRevenueReportParam param)
		{
			Stream stream;
			DataRow row;
			TruckRevenue.TruckRevenueDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			decimal total20 = 0;
			decimal total40 = 0;
			decimal total45 = 0;
			decimal totalAmount = 0;

			// get data
			dt = new TruckRevenue.TruckRevenueDataTable();
			List<TruckRevenueReportData> data;
			data = GetTruckRevenueListGeneral(param);

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

			if (data != null && data.Count > 0)
			{
				totalAmount = data.Sum(m => m.Amount);
				total20 = data.Sum(m => m.ContainerSize20);
				total40 = data.Sum(m => m.ContainerSize40);
				total45 = data.Sum(m => m.ContainerSize45);

				foreach (var item in data)
				{
					if (item.Amount > 0 || item.ContainerSize20 > 0 || item.ContainerSize40 > 0 ||
						item.ContainerSize45 > 0 || item.Load > 0)
					{
						row = dt.NewRow();
						row["20HC"] = item.ContainerSize20.ToString("####", cul.NumberFormat);
						row["40HC"] = item.ContainerSize40.ToString("####", cul.NumberFormat);
						row["45HC"] = item.ContainerSize45.ToString("####", cul.NumberFormat);
						row["Load"] = item.Load;
						row["Amount"] = item.Amount;
						row["RegisteredNo"] = item.RegisteredNo;
						dt.Rows.Add(row);
					}
				}
			}

			stream = CrystalReport.Service.TruckRevenue.ExportPdf.ExecGeneralReport(dt, intLanguage, param.DateFrom, param.DateTo, total20, total40, total45, totalAmount);
			return stream;
		}

		public List<TruckRevenueReportData> GetTruckRevenueListDetail(TruckRevenueReportParam param)
		{
			var data = from a in _dispatchRepository.GetAllQueryable()
					   join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo } equals new { b.OrderD, b.OrderNo, b.DetailNo } into t1
					   from b in t1.DefaultIfEmpty()
					   join c in _truckRepository.GetAllQueryable() on a.TruckC equals c.TruckC into t2
					   from c in t2.DefaultIfEmpty()
					   where ((param.TruckList == "null" || (param.TruckList).Contains(a.TruckC)) &
							  (param.DateFrom == null || a.TransportD >= param.DateFrom) &
							  (param.DateTo == null || a.TransportD <= param.DateTo) &
							  a.TruckC != null &
							  a.TruckC != "" &
							  (c.DisusedD == null || c.DisusedD >= param.DateFrom) &
							  c.PartnerI == "0"
						   )
					   select new TruckRevenueReportData()
					   {
						   TransportD = a.TransportD,
						   OrderTypeId = a.OrderTypeI,
						   ContainerSize20 = b.ContainerSizeI == "0" ? 1 : 0,
						   ContainerSize40 = b.ContainerSizeI == "1" ? 1 : 0,
						   ContainerSize45 = b.ContainerSizeI == "2" ? 1 : 0,
						   Load = b.ContainerSizeI == "3" ? (b.NetWeight ?? 0) : 0,
						   Location = (string.IsNullOrEmpty(b.ContainerNo) == false)
							   ? b.ContainerNo : "",
						   Amount = a.TransportFee ?? 0,
						   TruckC = a.TruckC,
						   RegisteredNo = c.RegisteredNo
					   };

			List<TruckRevenueReportData> temp = data.ToList();

			return (from o in temp
					group o by new { o.RegisteredNo, o.TransportD, o.OrderTypeId, o.Location }
						into g
						select new TruckRevenueReportData()
						{
							RegisteredNo = g.Key.RegisteredNo,
							TransportD = g.Key.TransportD,
							OrderTypeId = g.Key.OrderTypeId,
							Location = g.Key.Location,
							ContainerSize20 = g.Sum(x => x.ContainerSize20),
							ContainerSize40 = g.Sum(x => x.ContainerSize40),
							ContainerSize45 = g.Sum(x => x.ContainerSize45),
							Load = g.Sum(x => x.Load),
							Amount = g.Sum(x => x.Amount)
						}).ToList();

		}
		public List<TruckRevenueReportData> GetTruckRevenueListGeneral(TruckRevenueReportParam param)
		{
			var data = from t in _truckRepository.GetAllQueryable()
					   join o in
						   (
							   from o in _orderDRepository.GetAllQueryable()
							   join d in _dispatchRepository.GetAllQueryable() on new { o.OrderD, o.OrderNo, o.DetailNo } equals new { d.OrderD, d.OrderNo, d.DetailNo }
							   where ((param.DateFrom == null || d.TransportD >= param.DateFrom) &
								 (param.DateTo == null || d.TransportD <= param.DateTo) &
								 d.TruckC != null &
								 d.TruckC != "")
							   select new
							   {
								   TruckC = d.TruckC,
								   ContainerSizeI = o.ContainerSizeI,
								   NetWeight = o.NetWeight,
								   Amount = d.TransportFee
							   }
							   ) on t.TruckC equals o.TruckC into t1
					   from o in t1.DefaultIfEmpty()
					   where (
					   (param.TruckList == "null" || (param.TruckList).Contains(t.TruckC)) &
					   (t.DisusedD == null || t.DisusedD >= param.DateFrom) &
					   t.PartnerI == "0")
					   select new TruckRevenueReportData()
					   {
						   ContainerSize20 = o.ContainerSizeI == "0" ? 1 : 0,
						   ContainerSize40 = o.ContainerSizeI == "1" ? 1 : 0,
						   ContainerSize45 = o.ContainerSizeI == "2" ? 1 : 0,
						   Load = o.ContainerSizeI == "3" ? (o.NetWeight ?? 0) : 0,
						   Amount = o.Amount ?? 0,
						   TruckC = t.TruckC,
						   RegisteredNo = t.RegisteredNo
					   };

			List<TruckRevenueReportData> temp = data.ToList();

			return (from o in temp
					group o by new { o.RegisteredNo }
						into g
						select new TruckRevenueReportData()
						{
							RegisteredNo = g.Key.RegisteredNo,
							ContainerSize20 = g.Sum(x => x.ContainerSize20),
							ContainerSize40 = g.Sum(x => x.ContainerSize40),
							ContainerSize45 = g.Sum(x => x.ContainerSize45),
							Load = g.Sum(x => x.Load),
							Amount = g.Sum(x => x.Amount)
						}).ToList();

		}

		public Stream ExportPdfTruckBalance(TruckBalanceReportParam param, string userName)
		{
			Stream stream;
			DataRow row;
			TruckBalance.TruckBalanceDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;
			var companyName = "";
			var companyAddress = "";
			var fileName = "";
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

			// get data
			dt = new TruckBalance.TruckBalanceDataTable();
			TruckBalanceReportData data;
			data = GetTruckBalanceList(param);

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
																 (con.TextKey == "TLTTRUCKEXPENSEENTRY" ||
																  con.TextKey == "LBLPROFIT" ||
																  con.TextKey == "RPHDREVENUE" ||
																  con.TextKey == "LBLPROFIT/REVENUE" ||
																  con.TextKey == "LBLFIXEDEXPENSE" ||
																  con.TextKey == "LBLMAINTENANCEEXPENSE" ||
																  con.TextKey == "LBLSURCHARGE" ||
																  con.TextKey == "LBLPAYONBEHALF" ||
																  con.TextKey == "LBLOTHER" ||
																  con.TextKey == "RPLBLCOMPANYEXPENSEALLOCATION" ||
																  con.TextKey == "LBLCOMMONCOSTALLOCATION" ||
																  con.TextKey == "LBLDEPCRECIATIONCHARGE" ||
																  con.TextKey == "LBLDIRECTCHARGE"
																  )).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			string methodRounding = "0";
			decimal detentionAmount = 0;
			//Get basic setting
			var company = _basicRepository.GetAllQueryable().FirstOrDefault();
			if (company != null)
			{
				methodRounding = company.RevenueRoundingI;
				detentionAmount = company.DetentionAmount ?? 0;
			}

			if (data != null && data.TruckList.Count > 0)
			{
				decimal totalTrailerExpense = 0;
				var trailerExpense = (from a in _truckExpenseRepository.GetAllQueryable()
									  where a.ObjectI == "1"
									  select new TruckExpenseViewModel()
									  {
										  Total = a.Total
									  }).ToList();
				if (trailerExpense != null)
				{
					for (int i = 0; i < trailerExpense.Count; i++)
					{
						totalTrailerExpense += (decimal)trailerExpense[i].Total;
					}
				}
				decimal totalRevenue = GetTotalTruckRevenueForTruckBalance(param.DateFrom, param.DateTo);//data.TransportFees.Sum(x => x.TransportFee ?? 0);
				decimal totalCompanyExpense = totalTrailerExpense + GetTotalCompanyExpenseForTruckBalance(param.DateFrom, param.DateTo);
				foreach (var truck in data.TruckList)
				{
					var incomData = data.TransportFees.Where(x => x.TruckC == truck.TruckC);
					var surchargeFee = data.SurchargeFees.Where(x => x.TruckC == truck.TruckC);
					var expenseInclude = data.ExpensesInclude.Where(x => x.TruckC == truck.TruckC);
					var expenseData = data.ExpensesRequest.Where(x => x.TruckC == truck.TruckC && x.Value != 0);
					var maintenanceData = data.MaintenanceExpenses.Where(x => x.TruckC == truck.TruckC && x.Value != 0);
					var fixedData = data.FixedExpenses.Where(x => x.TruckC == truck.TruckC && x.Value != 0);
					var beforeFixedData = data.BeforeFixedExpenses.Where(x => x.TruckC == truck.TruckC && x.Value != 0);
					var afterFixedData = data.AfterFixedExpenses.Where(x => x.TruckC == truck.TruckC && x.Value != 0);

					decimal incom = (incomData.Sum(x => x.TransportFee) ?? 0);// +
					//(surchargeFee.Sum(x => x.TransportFee) ?? 0)*detentionAmount +
					//(expenseInclude.Sum(x => x.Value) ?? 0);
					var companyExpense = totalRevenue != 0 ? Utilities.CalByMethodRounding((totalCompanyExpense * incom / totalRevenue), methodRounding) : 0;
					var expenseDataSum = expenseData.Sum(x => x.Value) ?? 0;
					var maintenanceDataSum = maintenanceData.Sum(x => x.Value) ?? 0;
					var fixedDataSum = (fixedData.Sum(x => x.Value) ?? 0) - (beforeFixedData.Sum(x => x.Value) ?? 0) - (afterFixedData.Sum(x => x.Value) ?? 0);
					decimal totalBenefit = incom - (expenseDataSum + maintenanceDataSum + fixedDataSum + companyExpense);

					decimal totalBenefitPercent = incom != 0 ? (totalBenefit / incom) * 100 : 0;
					if (incom == 0 && totalBenefit == 0) continue;

					int total_row = expenseData.Count() + maintenanceData.Count() + fixedData.Count();
					int expenseIndex = 0;
					int maintenanceIndex = 0;
					int fixedIndex = 0;
					bool expenseHeader = false;
					bool maintenanceHeader = false;
					bool fixedHeader = false;
					for (int i = 0; i < Math.Max(1, total_row + 3); i++) // bo maintenanceData
					{
						row = dt.NewRow();
						row["RegisteredNo"] = truck.RegisteredNo;
						row["IncomN"] = "";
						row["IncomAmount"] = "";
						row["IsHeader"] = "";
						if (i == 0)
						{
							row["IncomN"] = dicLanguage["RPHDREVENUE"];
							row["IncomAmount"] = Utilities.CalByMethodRounding(incom, methodRounding);
						}
						//else if (i == 1)
						//{
						//	row["IncomN"] = dicLanguage["LBLSURCHARGE"];
						//	row["IncomAmount"] = Utilities.CalByMethodRounding((surchargeFee.Sum(x => x.TransportFee) ?? 0) * detentionAmount, methodRounding);
						//}
						//else if (i == 2)
						//{
						//	row["IncomN"] = dicLanguage["LBLPAYONBEHALF"];
						//	row["IncomAmount"] = Utilities.CalByMethodRounding(expenseInclude.Sum(x => x.Value) ?? 0, methodRounding);
						//}

						if (expenseIndex < expenseData.Count() + 1)
						{
							if (expenseHeader == false)
							{
								expenseHeader = true;
								row["RegisteredNo"] = truck.RegisteredNo;
								row["ExpenseN"] = dicLanguage["LBLDIRECTCHARGE"];
								row["ExpenseAmount"] = Utilities.CalByMethodRounding(expenseDataSum, methodRounding);
								row["IsHeader"] = "1";
							}
							else
							{
								row["ExpenseN"] = (expenseData.ElementAt(expenseIndex - 1).Name == "" ? dicLanguage["LBLOTHER"] : expenseData.ElementAt(expenseIndex - 1).Name);
								row["ExpenseAmount"] = Utilities.CalByMethodRounding(expenseData.ElementAt(expenseIndex - 1).Value ?? 0, methodRounding);
							}
							expenseIndex++;
						}
						//else if (maintenanceIndex < maintenanceData.Count() + 1)
						//{
						//	if (maintenanceHeader == false)
						//	{
						//		maintenanceHeader = true;
						//		row["ExpenseN"] = dicLanguage["LBLMAINTENANCEEXPENSE"];
						//		row["ExpenseAmount"] = Utilities.CalByMethodRounding(maintenanceData.Sum(x => x.Value) ?? 0,methodRounding);
						//		row["IsHeader"] = "1";
						//	}
						//	else
						//	{
						//		row["ExpenseN"] = maintenanceData.ElementAt(maintenanceIndex - 1).Name;
						//		row["ExpenseAmount"] = Utilities.CalByMethodRounding(maintenanceData.ElementAt(maintenanceIndex - 1).Value??0,methodRounding);
						//	}
						//	maintenanceIndex++;
						//}
						else if (fixedIndex < fixedData.Count() + 1)
						{
							if (fixedHeader == false)
							{
								fixedHeader = true;
								row["ExpenseN"] = dicLanguage["LBLDEPCRECIATIONCHARGE"];
								row["ExpenseAmount"] = Utilities.CalByMethodRounding(fixedDataSum, methodRounding);
								row["IsHeader"] = "1";
							}
							else
							{
								var fixedExpenseName = fixedData.ElementAt(fixedIndex - 1).Name;
								row["ExpenseN"] = fixedExpenseName;
								row["ExpenseAmount"] = Utilities.CalByMethodRounding((fixedData.ElementAt(fixedIndex - 1).Value ?? 0)
									- (beforeFixedData.Where(x => x.Name.Equals(fixedExpenseName)).Sum(x => x.Value) ?? 0)
									- (afterFixedData.Where(x => x.Name.Equals(fixedExpenseName)).Sum(x => x.Value) ?? 0), methodRounding);
							}
							fixedIndex++;
						}
						if (i == total_row + 2)
						{
							row["ExpenseN"] = dicLanguage["LBLCOMMONCOSTALLOCATION"];
							row["ExpenseAmount"] = companyExpense;
							row["IsHeader"] = "1";
						}
						if (i == 0)
						{
							row["BenefitN"] = dicLanguage["LBLPROFIT"];
							row["BenefitAmount"] = totalBenefit.ToString("#,###", cul.NumberFormat);
						}
						else if (i == 1)
						{
							row["BenefitN"] = dicLanguage["LBLPROFIT/REVENUE"];
							row["BenefitAmount"] = Math.Round(totalBenefitPercent, 2) != 0
								? Math.Round(totalBenefitPercent, 2).ToString("#,###.##", cul.NumberFormat) + "%"
								: "0" + "%";
						}
						else
						{
							row["BenefitN"] = "";
							row["BenefitAmount"] = "";
						}
						dt.Rows.Add(row);
					}
				}
			}

			stream = CrystalReport.Service.TruckBalance.ExportPdf.Exec(dt, intLanguage, param.DateFrom, param.DateTo, companyName, companyAddress, fileName, user);
			return stream;
		}
		private TruckBalanceReportData GetTruckBalanceList(TruckBalanceReportParam param)
		{
			#region CÁC KHOẢN THU
			// Lấy danh sách tien van chuyen thu duoc
			var incomData = from a in _truckRepository.GetAllQueryable()
							join b in _dispatchRepository.GetAllQueryable() on a.TruckC equals b.TruckC into t1
							from b in t1.DefaultIfEmpty()
							where ((param.TruckList == "null" || param.TruckList.Contains(b.TruckC)) &
								//(param.DepC == "null" || param.DepC.Contains(b.TransportDepC)) &
									 (param.DateFrom == null || b.TransportD >= param.DateFrom) &
									(param.DateTo == null || b.TransportD <= param.DateTo)
							  )
							select new TruckBalanceReportTransportFee
							{
								TruckC = b.TruckC,
								TransportFee = b.TransportFee
							};
			var ngayluucong = from a in _truckRepository.GetAllQueryable()
							  join b in _dispatchRepository.GetAllQueryable() on a.TruckC equals b.TruckC into t1
							  from b in t1.DefaultIfEmpty()
							  where ((param.TruckList == "null" || param.TruckList.Contains(b.TruckC)) &
								  //(param.DepC == "null" || param.DepC.Contains(b.TransportDepC)) &
									   (param.DateFrom == null || b.TransportD >= param.DateFrom) &
									  (param.DateTo == null || b.TransportD <= param.DateTo)
								)
							  select new TruckBalanceReportTransportFee
							  {
								  TruckC = b.TruckC,
								  TransportFee = b.DetainDay
							  };

			var chiphithu = from a in _expenseRepository.GetAllQueryable()
							join b in _expenseDetailRepository.GetAllQueryable() on a.ExpenseC equals b.ExpenseC into t1
							from b in t1.DefaultIfEmpty()
							join c in _dispatchRepository.GetAllQueryable() on new { b.OrderD, b.OrderNo, b.DetailNo, b.DispatchNo } equals new { c.OrderD, c.OrderNo, c.DetailNo, c.DispatchNo } into t2
							from c in t2.DefaultIfEmpty()
							where ((param.TruckList == "null" || param.TruckList.Contains(c.TruckC)) &
								//(param.DepC == "null" || param.DepC.Contains(c.TransportDepC)) &
								   (param.DateFrom == null || c.TransportD >= param.DateFrom) &
								  (param.DateTo == null || c.TransportD <= param.DateTo)
						  )
							select
							new TruckBalanceReportExpense()
							{
								Name = a.ExpenseN,
								Value = b.Amount,
								TruckC = c.TruckC
							};
			#endregion

			// Lấy danh sách xe
			var truckList = from a in _truckRepository.GetAllQueryable()
							where ((param.TruckList == "null" || param.TruckList.Contains(a.TruckC)) &&
									(param.DepC == "null" || a.DepC == param.DepC) &&
									a.PartnerI == "0" &&
									(a.DisusedD == null || a.DisusedD >= param.DateFrom))
							select new TruckBalanceReportTruckList()
							{
								TruckC = a.TruckC,
								RegisteredNo = a.RegisteredNo
							};

			// Lấy chi phí chi
			var expenseD = (from a in _expenseRepository.GetAllQueryable()
							join b in _expenseDetailRepository.GetAllQueryable() on a.ExpenseC equals b.ExpenseC into t1
							from b in t1.DefaultIfEmpty()
							join c in _dispatchRepository.GetAllQueryable() on new { b.OrderD, b.OrderNo, b.DetailNo, b.DispatchNo } equals new { c.OrderD, c.OrderNo, c.DetailNo, c.DispatchNo } into t2
							from c in t2.DefaultIfEmpty()
							join e in _expenseCategoryRepository.GetAllQueryable() on a.CategoryC equals e.CategoryC into t3
							from e in t3.DefaultIfEmpty()
							where ((param.TruckList == "null" || param.TruckList.Contains(c.TruckC)) &
								//(param.DepC == "null" || param.DepC.Contains(c.TransportDepC)) &
								   (param.DateFrom == null || c.TransportD >= param.DateFrom) &
								 (param.DateTo == null || c.TransportD <= param.DateTo) &
								 b.IsIncluded == "1"
							)
							select new TruckBalanceReportExpense()
							{
								Name = e != null ? e.CategoryN : "",
								Value = b.Amount,
								TruckC = c.TruckC
							}).Concat(from a in _expenseRepository.GetAllQueryable()
									  join e in _truckExpenseRepository.GetAllQueryable() on a.ExpenseC equals e.ExpenseC into t3
									  from e in t3.DefaultIfEmpty()
									  join c in _expenseCategoryRepository.GetAllQueryable() on a.CategoryC equals c.CategoryC into t2
									  from c in t2.DefaultIfEmpty()
									  where (e.ObjectI == "0" & (param.TruckList == "null" || param.TruckList.Contains(e.Code)) &
											  (param.DateFrom == null || e.TransportD >= param.DateFrom) &
											  (param.DateTo == null || e.TransportD <= param.DateTo) &
											  e.IsAllocated == "0"
										  //Lan: mac dinh tat ca cac chi phi trong Truck Expense deu phai quan ly noi bo
										  //Week21#190320151423: Truck Expense: Bỏ [Yêu cầu thanh toán]
										  //a.IsIncluded == "1"
									  )
									  select new TruckBalanceReportExpense()
									  {
										  Name = c != null ? c.CategoryN : "",
										  Value = e.Total,
										  TruckC = e.Code
									  });
			var expenseData = from a in expenseD
							  group a by new { a.TruckC, a.Name } into g
							  select new TruckBalanceReportExpense()
							  {
								  Name = g.Key.Name,
								  Value = g.Sum(x => x.Value),
								  TruckC = g.Key.TruckC
							  };

			// Lấy ds chi phí sữa chữa (dang update table, tam thoi = 0)
			var maintenance = new List<TruckBalanceReportExpense>();
			// Lấy ds chi phí cố định
			//int daysOfMonthFrom = System.DateTime.DaysInMonth(param.DateFrom.Year, param.DateFrom.Month);
			//int daysOfMonthTo = System.DateTime.DaysInMonth(param.DateTo.Year, param.DateTo.Month);
			var fixedExpense = from a in _fixedExpenseRepository.GetAllQueryable()
							   join b in _expenseRepository.GetAllQueryable() on a.ExpenseC equals b.ExpenseC into t1
							   from b in t1
							   where ((param.TruckList == "null" || param.TruckList.Contains(a.TruckC)) &
								   //(param.DepC == "null" || param.DepC.Contains(a.DepC)) &
											 (a.Year >= param.DateFrom.Year) &
											 (a.Year <= param.DateTo.Year)
										  )
							   group new { a, b } by new { b.ExpenseN, a.TruckC } into g
							   select new TruckBalanceReportExpense()
							   {
								   Name = g.Key.ExpenseN,
								   Value = g.Sum(x => x.a.Month1 + x.a.Month2 + x.a.Month3 + x.a.Month4 + x.a.Month5 + x.a.Month6
												+ x.a.Month7 + x.a.Month8 + x.a.Month9 + x.a.Month10 + x.a.Month11 + x.a.Month12),
								   TruckC = g.Key.TruckC
							   };
			var beforeFixedExpense = from a in _fixedExpenseRepository.GetAllQueryable()
									 join b in _expenseRepository.GetAllQueryable() on a.ExpenseC equals b.ExpenseC into t1
									 from b in t1
									 where ((param.TruckList == "null" || param.TruckList.Contains(a.TruckC)) &
										 //(param.DepC == "null" || param.DepC.Contains(a.DepC)) &
												  (a.Year == param.DateFrom.Year)
											   )
									 select new TruckBalanceReportExpense()
									 {
										 Name = b.ExpenseN,
										 Value =
											 (param.DateFrom.Month > 1 ? a.Month1 : 0) +
											 (param.DateFrom.Month > 2 ? a.Month2 : 0) +
											 (param.DateFrom.Month > 3 ? a.Month3 : 0) +
											 (param.DateFrom.Month > 4 ? a.Month4 : 0) +
											 (param.DateFrom.Month > 5 ? a.Month5 : 0) +
											 (param.DateFrom.Month > 6 ? a.Month6 : 0) +
											 (param.DateFrom.Month > 7 ? a.Month7 : 0) +
											 (param.DateFrom.Month > 8 ? a.Month8 : 0) +
											 (param.DateFrom.Month > 9 ? a.Month9 : 0) +
											 (param.DateFrom.Month > 10 ? a.Month10 : 0) +
											 (param.DateFrom.Month > 11 ? a.Month11 : 0),
										 TruckC = a.TruckC
									 };
			var afterFixedExpense = from a in _fixedExpenseRepository.GetAllQueryable()
									join b in _expenseRepository.GetAllQueryable() on a.ExpenseC equals b.ExpenseC into t1
									from b in t1
									where ((param.TruckList == "null" || param.TruckList.Contains(a.TruckC)) &
										//(param.DepC == "null" || param.DepC.Contains(a.DepC)) &
												 (a.Year == param.DateTo.Year)
											  )
									select new TruckBalanceReportExpense()
									{
										Name = b.ExpenseN,
										Value =
											(param.DateTo.Month < 2 ? a.Month2 : 0) +
											(param.DateTo.Month < 3 ? a.Month3 : 0) +
											(param.DateTo.Month < 4 ? a.Month4 : 0) +
											(param.DateTo.Month < 5 ? a.Month5 : 0) +
											(param.DateTo.Month < 6 ? a.Month6 : 0) +
											(param.DateTo.Month < 7 ? a.Month7 : 0) +
											(param.DateTo.Month < 8 ? a.Month8 : 0) +
											(param.DateTo.Month < 9 ? a.Month9 : 0) +
											(param.DateTo.Month < 10 ? a.Month10 : 0) +
											(param.DateTo.Month < 11 ? a.Month11 : 0) +
											(param.DateTo.Month < 12 ? a.Month12 : 0),
										TruckC = a.TruckC
									};
			return new TruckBalanceReportData()
			{
				TruckList = truckList.ToList(),
				TransportFees = incomData.ToList(),
				SurchargeFees = ngayluucong.ToList(),
				ExpensesInclude = chiphithu.ToList(),
				ExpensesRequest = expenseData.ToList(),
				FixedExpenses = fixedExpense.ToList(),
				BeforeFixedExpenses = beforeFixedExpense.ToList(),
				AfterFixedExpenses = afterFixedExpense.ToList(),
				MaintenanceExpenses = maintenance
			};
		}
		#endregion
		public Stream ExportPdfPartnerCustomerExpense(PartnerCustomerExpenseReportParam param)
		{
			Stream stream;
			DataRow row;
			PartnerCustomerExpenseList.PartnerCustomerExpenseListDataTable dt;
			int intLanguage;

			// get data
			dt = new PartnerCustomerExpenseList.PartnerCustomerExpenseListDataTable();
			List<PartnerCustomerExpenseReportData> data = GetPartnerCustomerExpenseList(param);

			// get language for report
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


			if (data != null && data.Count > 0)
			{
				for (int iloop = 0; iloop < data.Count; iloop++)
				{
					row = dt.NewRow();

					row["OrderD"] = data[iloop].Dispatch.OrderD;
					row["OrderNo"] = data[iloop].Dispatch.OrderNo;
					row["DetailNo"] = data[iloop].Dispatch.DetailNo;
					row["DispatchNo"] = data[iloop].Dispatch.DispatchNo;
					row["TransportD"] = data[iloop].Dispatch.TransportD != null ? Utilities.GetFormatDateReportByLanguage((DateTime)data[iloop].Dispatch.TransportD, intLanguage) : ""; ;
					// set customer name
					row["CustomerN"] = data[iloop].OrderH.CustomerN;
					if (!string.IsNullOrEmpty(data[iloop].OrderH.CustomerShortN))
					{
						row["CustomerN"] = data[iloop].OrderH.CustomerShortN;
					}
					// set location
					if (!string.IsNullOrEmpty(data[iloop].OrderH.LoadingPlaceN))
					{
						row["Location"] = data[iloop].OrderH.LoadingPlaceN;
					}
					if (!string.IsNullOrEmpty(data[iloop].OrderH.StopoverPlaceN))
					{
						row["Location"] += ", " + data[iloop].OrderH.StopoverPlaceN;
					}
					if (!string.IsNullOrEmpty(data[iloop].OrderH.DischargePlaceN))
					{
						row["Location"] += ", " + data[iloop].OrderH.DischargePlaceN;
					}
					row["ContainerNo"] = data[iloop].OrderD.ContainerNo;
					if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE1)
					{
						row["Container20"] = "1";
					}
					else if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE2)
					{
						row["Container40"] = "1";
					}
					else if (data[iloop].OrderD.ContainerSizeI == Constants.CONTAINERSIZE3)
					{
						row["Container45"] = "1";
					}
					// set money
					var partnerFee = data[iloop].Dispatch.PartnerFee == null ? 0 : (decimal)data[iloop].Dispatch.PartnerFee;
					var partnerExpense = data[iloop].Dispatch.PartnerExpense == null ? 0 : (decimal)data[iloop].Dispatch.PartnerExpense;
					var partnerSurcharge = data[iloop].Dispatch.PartnerSurcharge == null ? 0 : (decimal)data[iloop].Dispatch.PartnerSurcharge;
					var partnerDiscount = data[iloop].Dispatch.PartnerDiscount == null ? 0 : (decimal)data[iloop].Dispatch.PartnerDiscount;
					var partnerTaxAmount = data[iloop].Dispatch.PartnerTaxAmount == null ? 0 : (decimal)data[iloop].Dispatch.PartnerTaxAmount;
					row["PartnerFee"] = partnerFee;
					row["PartnerExpense"] = partnerExpense;
					row["PartnerSurcharge"] = partnerSurcharge;
					row["PartnerDiscount"] = partnerDiscount;
					// set total
					row["Total"] = (partnerFee + partnerSurcharge).ToString("#,###", cul.NumberFormat);
					// set total tax
					//if (data[iloop].TaxMethodI == "1")
					//{
					//	row["TotalTax"] = Utilities.CalByMethodRounding((partnerFee + partnerSurcharge) * data[iloop].TaxRate / 100, data[iloop].TaxRoundingI).ToString("#,###", cul.NumberFormat);
					//}
					//else
					//{
					//	row["TotalTax"] = ((partnerFee + partnerSurcharge) * data[iloop].TaxRate / 100).ToString("#,###.###", cul.NumberFormat);
					//}
					partnerTaxAmount += Utilities.CalByMethodRounding(partnerSurcharge * data[iloop].TaxRate / 100, data[iloop].TaxRoundingI);
					row["TotalTax"] = partnerTaxAmount.ToString("#,###.###", cul.NumberFormat);
					// set total After Tax
					//if (data[iloop].TaxMethodI == "1")
					//{
					//	row["TotalAfterTax"] = (partnerFee + partnerSurcharge - partnerDiscount +
					//							Utilities.CalByMethodRounding((partnerFee + partnerSurcharge) * data[iloop].TaxRate / 100, data[iloop].TaxRoundingI)).ToString("#,###", cul.NumberFormat);
					//}
					//else
					//{
					//	row["TotalAfterTax"] =
					//		(partnerFee + partnerSurcharge - partnerDiscount +
					//		 (partnerFee + partnerSurcharge) * data[iloop].TaxRate / 100).ToString("#,###.###", cul.NumberFormat);
					//}
					row["TotalAfterTax"] = (partnerFee + partnerSurcharge - partnerDiscount + partnerTaxAmount + partnerExpense).ToString("#,###", cul.NumberFormat);

					// set key
					var partnerMainC = data[iloop].PartnerMainC;
					var partnerSubC = data[iloop].PartnerSubC;
					row["Key"] = partnerMainC + "_" + partnerSubC;
					// set partner name
					row["PartnerN"] = "";
					var partnerN = _partnerRepository.Query(par => par.PartnerMainC == partnerMainC && par.PartnerSubC == partnerSubC).FirstOrDefault();
					if (partnerN != null)
					{
						row["PartnerN"] = partnerN.PartnerN;
					}

					// set tax for partner
					row["TaxMethodI"] = data[iloop].TaxMethodI;
					row["TaxRate"] = data[iloop].TaxRate;
					row["TaxRoundingI"] = data[iloop].TaxRoundingI;

					dt.Rows.Add(row);
				}
			}

			// set month and year
			var monthYear = param.TransportM.Month + "/" + param.TransportM.Year;
			if (intLanguage == 3)
			{
				monthYear = param.TransportM.Year + "年" + param.TransportM.Month + "月";
			}

			stream = CrystalReport.Service.PartnerCustomerExpense.ExportPdf.Exec(dt, intLanguage, monthYear);
			return stream;
		}

		public List<PartnerCustomerExpenseReportData> GetPartnerCustomerExpenseList(PartnerCustomerExpenseReportParam param)
		{
			var result = new List<PartnerCustomerExpenseReportData>();

			// get month and year transport
			var month = param.TransportM.Month;
			var year = param.TransportM.Year;

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
					var item = new CustomerExpenseReportData();

					var arr = (partrnerArr[iloop]).Split(new string[] { "_" }, StringSplitOptions.None);
					var partnerMainC = arr[0];
					var partnerSubC = arr[1];
					var invoiceInfo = _invoiceInfoService.GetLimitStartAndEndInvoiceDPartner(partnerMainC, partnerSubC, month, year);
					var startDate = invoiceInfo.StartDate.Date;
					var endDate = invoiceInfo.EndDate.Date;

					// get partner who shared a invoice company
					var partnerStr = "";
					var partnerList = _partnerService.GetPartnersByInvoice(partnerMainC, partnerSubC);
					for (var aloop = 0; aloop < partnerList.Count; aloop++)
					{
						partnerStr = partnerStr + "," + partnerList[aloop].PartnerMainC + "_" + partnerList[aloop].PartnerSubC;
					}

					if (partnerStr != "")
					{
						var data = from a in _dispatchRepository.GetAllQueryable()
								   join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo } equals new { b.OrderD, b.OrderNo, b.DetailNo } into t1
								   from b in t1.DefaultIfEmpty()
								   join c in _orderHRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo } equals new { c.OrderD, c.OrderNo } into t2
								   from c in t2.DefaultIfEmpty()
								   join d in _customerRepository.GetAllQueryable() on new { c.CustomerMainC, c.CustomerSubC } equals new { d.CustomerMainC, d.CustomerSubC } into t3
								   from d in t3.DefaultIfEmpty()
								   join e in _partnerRepository.GetAllQueryable() on new { a.PartnerMainC, a.PartnerSubC } equals new { e.PartnerMainC, e.PartnerSubC } into t4
								   from e in t4.DefaultIfEmpty()
								   where (a.DispatchStatus == Constants.CONFIRMED &
										  (a.TransportD >= startDate & a.TransportD <= endDate) &
										  (a.PartnerMainC != null & a.PartnerMainC != "" & a.PartnerSubC != null & partnerStr.Contains(a.PartnerMainC + "_" + a.PartnerSubC)) &
										  (param.DepC == "0" || a.TransportDepC == param.DepC) &
										  (param.Customer == "null" || (param.Customer).Contains(c.CustomerMainC + "_" + c.CustomerSubC))
									   )
								   select new PartnerCustomerExpenseReportData()
								   {
									   PartnerMainC = invoiceInfo.PartnerMainC,
									   PartnerSubC = invoiceInfo.PartnerSubC,
									   TaxMethodI = invoiceInfo.TaxMethodI,
									   TaxRate = invoiceInfo.TaxRate,
									   TaxRoundingI = invoiceInfo.TaxRoundingI,
									   Dispatch = new DispatchViewModel()
									   {
										   OrderD = a.OrderD,
										   OrderNo = a.OrderNo,
										   DetailNo = a.DetailNo,
										   DispatchNo = a.DispatchNo,
										   TransportD = a.TransportD,
										   PartnerFee = a.PartnerFee,
										   PartnerExpense = a.PartnerExpense,
										   PartnerSurcharge = a.PartnerSurcharge,
										   PartnerDiscount = a.PartnerDiscount,
										   PartnerMainC = a.PartnerMainC,
										   PartnerSubC = a.PartnerSubC,
										   PartnerN = e != null ? e.PartnerN : "",
										   PartnerTaxAmount = a.PartnerTaxAmount
									   },
									   OrderD = new ContainerViewModel()
									   {
										   ContainerNo = b.ContainerNo,
										   ContainerSizeI = b.ContainerSizeI
									   },
									   OrderH = new OrderViewModel()
									   {
										   CustomerMainC = c.CustomerMainC,
										   CustomerSubC = c.CustomerSubC,
										   CustomerN = d != null ? d.CustomerN : "",
										   CustomerShortN = d != null ? d.CustomerShortN : "",
										   LoadingPlaceN = c.LoadingPlaceN,
										   StopoverPlaceN = c.StopoverPlaceN,
										   DischargePlaceN = c.DischargePlaceN
									   }
								   };

						data = data.OrderBy("Dispatch.PartnerN, Dispatch.TransportD, Dispatch.OrderD, Dispatch.OrderNo, Dispatch.DetailNo, Dispatch.DispatchNo");
						var dataList = data.ToList();
						if (dataList.Count > 0)
						{
							result.AddRange(dataList);
						}
					}
				}
			}

			return result;
		}
		#region Driver
		public Stream ExportPdfDriverRevenueDetail(DriverRevenueReportParam param)
		{
			Stream stream;
			DataRow row;
			DriverRevenue.DriverRevenueDataTable dt;
			Dictionary<string, string> dicLanguage;
			int intLanguage;

			// get data
			dt = new DriverRevenue.DriverRevenueDataTable();
			List<DriverRevenueReportData> data;
			data = GetDriverRevenueList(param);

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
																 (con.TextKey == "LBLIMPORT" ||
																  con.TextKey == "LBLEXPORT" ||
																  con.TextKey == "LBLOTHER"
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
				//int index = 1;
				//var listDate = data.GroupBy(item => item.TransportD)
				//    .Select(grp => grp.First())
				//    .OrderBy(item => item.TransportD)
				//    .ToList();

				//foreach (var currentDay in listDate)
				//{
				//var allowanceInDay = from item in data
				//                     where item.TransportD == currentDay.TransportD
				//                     select item;
				//bool isFirst = true;
				foreach (var item in data)
				{
					row = dt.NewRow();
					row["TransportD"] = item.TransportD == null
						? ""
						: Utilities.GetFormatDateReportByLanguage(item.TransportD ?? DateTime.Now, intLanguage);
					//row["Day"] = item.TransportD == null ? "" : Convert.ToDateTime(item.TransportD).ToString("dd/MM/yy");
					row["OrderTypeI"] = item.OrderTypeId == "0"
																? dicLanguage["LBLEXPORT"]
																: item.OrderTypeId == "1"
																	? dicLanguage["LBLIMPORT"]
																	: dicLanguage["LBLOTHER"];
					row["LocationN"] = item.Location;
					//row["20HC"] = item.ContainerSize20.ToString("####", cul.NumberFormat);
					//row["40HC"] = item.ContainerSize40.ToString("####", cul.NumberFormat);
					//row["45HC"] = item.ContainerSize45.ToString("####", cul.NumberFormat);
					row["CustomerN"] = item.CustomerN;
					row["ContainerNo"] = item.ContainerNo;
					row["ContainerSizeI"] = item.ContainerSizeI;
					row["DriverAllowance"] = item.DriverAllowance;
					row["Amount"] = item.Amount;
					row["DriverC"] = item.DriverC;
					row["DriverName"] = intLanguage == 1 ? string.Format("{0} {1}", item.LastN, item.FirstN) : string.Format("{0} {1}", item.FirstN, item.LastN);

					dt.Rows.Add(row);
				}

				//}
			}

			stream = CrystalReport.Service.DriverRevenue.ExportPdf.Exec(dt, intLanguage, param.DateFrom, param.DateTo);
			return stream;
		}
		public Stream ExportPdfDriverRevenueGeneral(DriverRevenueReportParam param)
		{
			Stream stream;
			DataRow row;
			DriverRevenue.DriverRevenueDataTable dt;
			int intLanguage;
			int Total20HC = 0;
			int Total40HC = 0;
			int Total45HC = 0;
			decimal TotalAmount = 0;

			// get data
			dt = new DriverRevenue.DriverRevenueDataTable();
			List<DriverRevenueReportData> data;
			data = GetDriverRevenueGeneralList(param);

			// get language for report
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

			if (data != null && data.Count > 0)
			{

				foreach (var item in data)
				{
					row = dt.NewRow();
					Total20HC += item.ContainerSize20;
					Total40HC += item.ContainerSize40;
					Total45HC += item.ContainerSize45;
					TotalAmount += item.Amount ?? 0;

					row["20HC"] = item.ContainerSize20.ToString("####", cul.NumberFormat);
					row["40HC"] = item.ContainerSize40.ToString("####", cul.NumberFormat);
					row["45HC"] = item.ContainerSize45.ToString("####", cul.NumberFormat);
					row["Load"] = item.Load;
					row["Amount"] = item.Amount;
					row["DriverName"] = intLanguage == 1
						? string.Format("{0} {1}", item.LastN, item.FirstN)
						: string.Format("{0} {1}", item.FirstN, item.LastN);

					dt.Rows.Add(row);
				}
			}

			stream = CrystalReport.Service.DriverRevenue.ExportPdf.ExecGeneralReport(dt, intLanguage, param.DateFrom, param.DateTo, Total20HC, Total40HC, Total45HC, TotalAmount);
			return stream;
		}
		public List<DriverRevenueReportData> GetDriverRevenueList(DriverRevenueReportParam param)
		{
			var data = from a in _dispatchRepository.GetAllQueryable()
					   join b in _orderDRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo, a.DetailNo } equals new { b.OrderD, b.OrderNo, b.DetailNo } into t1
					   from b in t1.DefaultIfEmpty()
					   join c in _driverRepository.GetAllQueryable() on a.DriverC equals c.DriverC into t2
					   from c in t2.DefaultIfEmpty()
					   join o in _orderHRepository.GetAllQueryable() on new { a.OrderD, a.OrderNo } equals new { o.OrderD, o.OrderNo } into t3
					   from o in t3.DefaultIfEmpty()
					   join t in _customerRepository.GetAllQueryable() on new { o.CustomerMainC, o.CustomerSubC } equals new { t.CustomerMainC, t.CustomerSubC } into t4
					   from t in t4.DefaultIfEmpty()
					   where ((param.DriverList == "null" || (param.DriverList).Contains(a.DriverC)) &
							  (param.DateFrom == null || a.TransportD >= param.DateFrom) &
							  (param.DateTo == null || a.TransportD <= param.DateTo) &
							  a.DriverC != null &
							  a.DriverC != "" &
							  a.TransportD != null
						   )
					   select new DriverRevenueReportData()
					   {
						   TransportD = a.TransportD,
						   OrderTypeId = a.OrderTypeI,
						   ContainerNo = b.ContainerNo,
						   CustomerN = t.CustomerN,
						   DriverAllowance = b.TotalDriverAllowance ?? 0,
						   ContainerSizeI = b.ContainerSizeI,
						   //ContainerSize20 = b.ContainerSizeI == "0" ? 1 : 0,
						   //ContainerSize40 = b.ContainerSizeI == "1" ? 1 : 0,
						   //ContainerSize45 = b.ContainerSizeI == "2" ? 1 : 0,
						   Location = (string.IsNullOrEmpty(a.Location3N) == false)
							   ? a.Location3N
							   : string.IsNullOrEmpty(a.Location2N) == false ? a.Location2N : a.Location1N,
						   Amount = a.TransportFee ?? 0,
						   DriverC = a.DriverC,
						   FirstN = c.FirstN,
						   LastN = c.LastN

					   };
			return data.ToList();
			//List<DriverRevenueReportData> temp = data.ToList();

			//return (from o in temp
			//        group o by new { o.DriverC, o.TransportD, o.OrderTypeId, o.Location, o.FirstN, o.LastN }
			//            into g
			//            select new DriverRevenueReportData()
			//            {
			//                DriverC = g.Key.DriverC,
			//                TransportD = g.Key.TransportD,
			//                OrderTypeId = g.Key.OrderTypeId,
			//                Location = g.Key.Location,
			//                ContainerSize20 = g.Sum(x => x.ContainerSize20),
			//                ContainerSize40 = g.Sum(x => x.ContainerSize40),
			//                ContainerSize45 = g.Sum(x => x.ContainerSize45),
			//                Amount = g.Sum(x => x.Amount),
			//                FirstN = g.Key.FirstN,
			//                LastN = g.Key.LastN
			//            }).ToList();

		}
		public List<DriverRevenueReportData> GetDriverRevenueGeneralList(DriverRevenueReportParam param)
		{
			var data = from d in _driverRepository.GetAllQueryable()
					   join o in
						   (
						   from o in _orderDRepository.GetAllQueryable()
						   join d in _dispatchRepository.GetAllQueryable()
						   on new { o.OrderD, o.OrderNo, o.DetailNo } equals new { d.OrderD, d.OrderNo, d.DetailNo }
						   where (
							 d.TransportD >= param.DateFrom &
							 d.TransportD <= param.DateTo &
							 d.DriverC != null &
							 d.DriverC != "" &
							 d.TransportD != null
						   )
						   select new
						   {
							   DriverC = d.DriverC,
							   ContainerSizeI = o.ContainerSizeI,
							   Amount = d.TransportFee,
							   NetWeight = o.NetWeight
						   }
						   ) on d.DriverC equals o.DriverC into t1
					   from o in t1.DefaultIfEmpty()
					   where (param.DriverList == "null" || param.DriverList.Contains(d.DriverC))
					   select new DriverRevenueReportData()
					   {
						   ContainerSize20 = o.ContainerSizeI == "0" ? 1 : 0,
						   ContainerSize40 = o.ContainerSizeI == "1" ? 1 : 0,
						   ContainerSize45 = o.ContainerSizeI == "2" ? 1 : 0,
						   Load = o.ContainerSizeI == "3" ? (o.NetWeight ?? 0) : 0,
						   Amount = o.Amount,
						   DriverC = d.DriverC,
						   FirstN = d.FirstN,
						   LastN = d.LastN
					   };

			List<DriverRevenueReportData> temp = data.ToList();

			return (from o in temp
					group o by new { o.DriverC, o.FirstN, o.LastN }
						into g
						select new DriverRevenueReportData()
						{
							DriverC = g.Key.DriverC,
							ContainerSize20 = g.Sum(x => x.ContainerSize20),
							ContainerSize40 = g.Sum(x => x.ContainerSize40),
							ContainerSize45 = g.Sum(x => x.ContainerSize45),
							Load = g.Sum(x => x.Load),
							Amount = g.Sum(x => x.Amount),
							FirstN = g.Key.FirstN,
							LastN = g.Key.LastN
						}).ToList();

		}
		#endregion
		public Stream ExportPdfPartnerExpense(PartnerExpenseReportParam param)
		{
			Stream stream;
			DataRow row;
			PartnerExpenseList.PartnerExpenseListDataTable dt;
			int intLanguage;
			//decimal taxRate = 0;

			// get data
			dt = new PartnerExpenseList.PartnerExpenseListDataTable();
			List<PartnerExpenseReportData> data = GetPartnerExpenseList(param);

			// get language for report
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


			if (data != null && data.Count > 0)
			{
				for (int iloop = 0; iloop < data.Count; iloop++)
				{
					row = dt.NewRow();

					row["PartnerMainC"] = data[iloop].PartnerMainC;
					row["PartnerSubC"] = data[iloop].PartnerSubC;
					row["PartnerN"] = data[iloop].PartnerN;
					//if (!string.IsNullOrEmpty(data[iloop].PartnerShortN))
					//{
					//	row["PartnerN"] = data[iloop].PartnerShortN;
					//}
					row["ContainerAmount"] = data[iloop].ContainerAmount;
					// set money
					var partnerFee = data[iloop].PartnerFee == null ? 0 : (decimal)data[iloop].PartnerFee;
					var partnerExpense = data[iloop].PartnerExpense == null ? 0 : (decimal)data[iloop].PartnerExpense;
					var partnerSurcharge = data[iloop].PartnerSurcharge == null ? 0 : (decimal)data[iloop].PartnerSurcharge;
					var partnerDiscount = data[iloop].PartnerDiscount == null ? 0 : (decimal)data[iloop].PartnerDiscount;
					var partnerToTalTax = data[iloop].PartnerToTalTax + Utilities.CalByMethodRounding(partnerSurcharge * data[iloop].TaxRate / 100, data[iloop].TaxRoundingI);
					row["PartnerFee"] = partnerFee;
					row["PartnerExpense"] = partnerExpense;
					row["PartnerSurcharge"] = partnerSurcharge;
					row["PartnerDiscount"] = partnerDiscount;

					// set total
					row["Total"] = (partnerFee + partnerSurcharge).ToString("#,###", cul.NumberFormat);
					// set total tax
					//if (data[iloop].TaxMethodI == "1")
					//{
					row["TotalTax"] = partnerToTalTax.ToString("#,###", cul.NumberFormat);
					//}
					//else
					//{
					//	row["TotalTax"] = Utilities.CalByMethodRounding((partnerFee + partnerSurcharge) * data[iloop].TaxRate / 100, data[iloop].TaxRoundingI).ToString("#,###.###", cul.NumberFormat);
					//}
					// set total After Tax
					//if (data[iloop].TaxMethodI == "1")
					//{
					row["TotalAfterTax"] = (partnerFee + partnerSurcharge - partnerDiscount + partnerToTalTax + partnerExpense).ToString("#,###", cul.NumberFormat);
					//}
					//else
					//{
					//	row["TotalAfterTax"] = Utilities.CalByMethodRounding(
					//		(partnerFee + partnerSurcharge - partnerDiscount +
					//		 (partnerFee + partnerSurcharge) * data[iloop].TaxRate / 100), data[iloop].TaxRoundingI).ToString("#,###.###", cul.NumberFormat);
					//}
					row["TaxMethodI"] = data[iloop].TaxMethodI;
					row["TaxRate"] = data[iloop].TaxRate;
					row["TaxRoundingI"] = data[iloop].TaxRoundingI;

					dt.Rows.Add(row);
				}
			}

			// set month and year
			var monthYear = param.TransportM.Value.Month + "/" + param.TransportM.Value.Year;
			if (intLanguage == 3)
			{
				monthYear = param.TransportM.Value.Year + "年" + param.TransportM.Value.Month + "月";
			}

			stream = CrystalReport.Service.PartnerExpense.ExportPdf.Exec(dt, intLanguage, monthYear);
			return stream;
		}

		public List<PartnerExpenseReportData> GetPartnerExpenseList(PartnerExpenseReportParam param)
		{
			var result = new List<PartnerExpenseReportData>();

			// get month and year transport
			var month = param.TransportM.Value.Month;
			var year = param.TransportM.Value.Year;

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
					var invoiceInfo = _invoiceInfoService.GetLimitStartAndEndInvoiceDPartner(partnerMainC, partnerSubC, month, year);
					var startDate = invoiceInfo.StartDate.Date;
					var endDate = invoiceInfo.EndDate.Date;

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
									   where (a.DispatchStatus == Constants.CONFIRMED &
											  (a.TransportD >= startDate & a.TransportD <= endDate) &
											  (param.DepC == "0" || a.TransportDepC == param.DepC) &
											  (a.PartnerMainC != null & a.PartnerMainC != "" & a.PartnerSubC != null & a.PartnerSubC != "" &
											   partnerStr.Contains(a.PartnerMainC + "_" + a.PartnerSubC)
											  )
											 )
									   select new DispatchViewModel()
									   {
										   OrderD = a.OrderD,
										   OrderNo = a.OrderNo,
										   DetailNo = a.DetailNo,
										   DispatchNo = a.DispatchNo,
										   PartnerMainC = partnerMainC,
										   PartnerSubC = partnerSubC,
										   PartnerFee = a.PartnerFee,
										   PartnerExpense = a.PartnerExpense,
										   PartnerSurcharge = a.PartnerSurcharge,
										   PartnerDiscount = a.PartnerDiscount,
										   PartnerToTalTax = a.PartnerTaxAmount ?? 0
										   //PartnerToTalTax = invoiceInfo.TaxRoundingI == "1" ? Math.Ceiling(((decimal)(a.PartnerFee ?? 0) + (decimal)(a.PartnerSurcharge ?? 0)) * invoiceInfo.TaxRate / 100) : (invoiceInfo.TaxMethodI == "2" ? Math.Floor(((decimal)(a.PartnerFee ?? 0) + (decimal)(a.PartnerSurcharge ?? 0)) * invoiceInfo.TaxRate / 100) : Math.Round(((decimal)(a.PartnerFee ?? 0) + (decimal)(a.PartnerSurcharge ?? 0)) * invoiceInfo.TaxRate / 100))
									   };
						//dispatch = dispatch.OrderBy("PartnerMainC asc, PartnerSubC asc");

						var groupDispatch = from b in dispatch
											group b by new { b.PartnerMainC, b.PartnerSubC } into c
											select new
											{
												c.Key.PartnerMainC,
												c.Key.PartnerSubC,
												ContainerAmount = c.Count(),
												PartnerFee = c.Sum(b => b.PartnerFee),
												PartnerExpense = c.Sum(b => b.PartnerExpense),
												PartnerSurcharge = c.Sum(b => b.PartnerSurcharge),
												PartnerDiscount = c.Sum(b => b.PartnerDiscount),
												PartnerToTalTax = c.Sum(b => b.PartnerToTalTax)
											};
						var temp2 = groupDispatch.ToList();
						var data = from a in groupDispatch
								   join b in _partnerRepository.GetAllQueryable() on new { a.PartnerMainC, a.PartnerSubC } equals new { b.PartnerMainC, b.PartnerSubC } into t1
								   from b in t1.DefaultIfEmpty()
								   select new PartnerExpenseReportData()
								   {
									   PartnerMainC = a.PartnerMainC,
									   PartnerSubC = a.PartnerSubC,
									   ContainerAmount = a.ContainerAmount,
									   PartnerFee = a.PartnerFee,
									   PartnerExpense = a.PartnerExpense,
									   PartnerSurcharge = a.PartnerSurcharge,
									   PartnerDiscount = a.PartnerDiscount,
									   PartnerToTalTax = a.PartnerToTalTax,
									   PartnerN = b != null ? b.PartnerN : "",
									   PartnerShortN = b != null ? b.PartnerShortN : "",
									   TaxMethodI = invoiceInfo.TaxMethodI,
									   TaxRate = invoiceInfo.TaxRate,
									   TaxRoundingI = invoiceInfo.TaxRoundingI
								   };

						//data = data.OrderBy("PartnerN asc, PartnerShortN asc");
						var dataList = data.ToList();
						if (dataList.Count > 0)
						{
							result.AddRange(dataList);
						}
					}
				}
			}

			return result;
		}

		public Stream ExportPdfSupplierExpense(SupplierExpenseReportParam param)
		{
			Stream stream;
			DataRow row;
			SupplierExpenseList.SupplierExpenseListDataTable dt;
			int intLanguage;
			//decimal taxRate = 0;
			int totalItem = 0;

			// get data
			dt = new SupplierExpenseList.SupplierExpenseListDataTable();
			List<SupplierExpenseReportData> data = GetSupplierExpenseList(param);

			// get language for report
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


			if (data != null && data.Count > 0)
			{
				string oldKey = "";
				string newKey = "";
				for (int iloop = 0; iloop < data.Count; iloop++)
				{
					row = dt.NewRow();
					newKey = data[iloop].SupplierMainC + "_" + data[iloop].SupplierSubC;
					if (newKey != oldKey)
					{
						oldKey = newKey;
						totalItem++;
					}

					row["SupplierMainC"] = data[iloop].SupplierMainC;
					row["SupplierSubC"] = data[iloop].SupplierSubC;

					row["SupplierN"] = data[iloop].SupplierN;
					if (!string.IsNullOrEmpty(data[iloop].SupplierShortN))
					{
						row["SupplierN"] = data[iloop].SupplierShortN;
					}
					row["ExpenseC"] = data[iloop].ExpenseC;
					row["ExpenseN"] = data[iloop].ExpenseN;
					row["UnitPrice"] = data[iloop].UnitPrice;
					if (data[iloop].UnitPrice == null)
					{
						row["UnitPrice"] = 0;
					}
					row["Quantity"] = data[iloop].Quantity;
					if (data[iloop].Quantity == null)
					{
						row["Quantity"] = 0;
					}
					row["Total"] = data[iloop].Total;
					if (data[iloop].Total == null)
					{
						row["Total"] = 0;
					}
					row["TaxAmount"] = data[iloop].TaxAmount;
					if (data[iloop].TaxAmount == null)
					{
						row["TaxAmount"] = 0;
					}

					dt.Rows.Add(row);
				}
			}

			// set fromDate and toDate
			var fromDate = "";
			if (param.ExpenseDFrom != null)
			{
				if (intLanguage == 1)
				{
					fromDate = ((DateTime)param.ExpenseDFrom).ToString("dd/MM/yyyy");
				}
				else if (intLanguage == 2)
				{
					fromDate = ((DateTime)param.ExpenseDFrom).ToString("MM/dd/yyyy");
				}
				else if (intLanguage == 3)
				{
					var date = (DateTime)param.ExpenseDFrom;
					fromDate = date.Year + "年" + date.Month + "月" + date.Day + "日";
				}
			}
			var toDate = "";
			if (param.ExpenseDTo != null)
			{
				if (intLanguage == 1)
				{
					toDate = ((DateTime)param.ExpenseDTo).ToString("dd/MM/yyyy");
				}
				else if (intLanguage == 2)
				{
					toDate = ((DateTime)param.ExpenseDTo).ToString("MM/dd/yyyy");
				}
				else if (intLanguage == 3)
				{
					var date = (DateTime)param.ExpenseDTo;
					toDate = date.Year + "年" + ("0" + date.Month).Substring(("0" + date.Month).Length - 2) + "月" + date.Day + "日";
				}
			}

			stream = CrystalReport.Service.SupplierExpense.ExportPdf.Exec(dt, intLanguage, fromDate, toDate, totalItem);
			return stream;
		}

		public List<SupplierExpenseReportData> GetSupplierExpenseList(SupplierExpenseReportParam param)
		{
			var truckExpense = from a in _truckExpenseRepository.GetAllQueryable()
							   join b in _supplierRepository.GetAllQueryable() on new { a.SupplierMainC, a.SupplierSubC } equals new { b.SupplierMainC, b.SupplierSubC } into t1
							   from b in t1.DefaultIfEmpty()
							   join c in _expenseRepository.GetAllQueryable() on a.ExpenseC equals c.ExpenseC into t2
							   from c in t2.DefaultIfEmpty()
							   where ((param.ExpenseDFrom == null || a.InvoiceD >= param.ExpenseDFrom) &
									  (param.ExpenseDTo == null || a.InvoiceD <= param.ExpenseDTo) &
									  ((param.Supplier == "null" && a.SupplierMainC != null && a.SupplierSubC != "") || (param.Supplier != "null" && param.Supplier.Contains(a.SupplierMainC + "_" + a.SupplierSubC))) &
									  (param.Expense == "null" || param.Expense.Contains(a.ExpenseC))
								 )
							   select new TruckExpenseViewModel()
							   {
								   SupplierMainC = a.SupplierMainC,
								   SupplierSubC = a.SupplierSubC,
								   ExpenseC = a.ExpenseC,
								   UnitPrice = a.UnitPrice,
								   Quantity = a.Quantity,
								   Total = a.Total,
								   Tax = a.Tax
							   };
			truckExpense = truckExpense.OrderBy("SupplierMainC asc, SupplierSubC asc, ExpenseC asc, UnitPrice asc");

			var groupTruckExpense = from b in truckExpense
									group b by new { b.SupplierMainC, b.SupplierSubC, b.ExpenseC, b.UnitPrice } into c
									select new
									{
										c.Key.SupplierMainC,
										c.Key.SupplierSubC,
										c.Key.ExpenseC,
										c.Key.UnitPrice,
										Quantity = c.Sum(b => b.Quantity),
										Total = c.Sum(b => b.Total),
										TaxAmount = c.Sum(b => b.Tax)
									};

			var expenseD = from a in _expenseDetailRepository.GetAllQueryable()
						   where ((param.ExpenseDFrom == null || a.InvoiceD >= param.ExpenseDFrom) &
								  (param.ExpenseDTo == null || a.InvoiceD <= param.ExpenseDTo) &
								  ((param.Supplier == "null" && a.SupplierMainC != null && a.SupplierSubC != "") || (param.Supplier != "null" && param.Supplier.Contains(a.SupplierMainC + "_" + a.SupplierSubC))) &
								  (param.Expense == "null" || param.Expense.Contains(a.ExpenseC))
								 )
						   select new ExpenseDetailViewModel()
						   {
							   SupplierMainC = a.SupplierMainC,
							   SupplierSubC = a.SupplierSubC,
							   ExpenseC = a.ExpenseC,
							   UnitPrice = a.UnitPrice,
							   Quantity = a.Quantity,
							   Amount = a.Amount,
							   TaxAmount = a.TaxAmount
						   };
			expenseD = expenseD.OrderBy("SupplierMainC asc, SupplierSubC asc, ExpenseC asc, UnitPrice asc");

			var groupExpenseD = from b in expenseD
								group b by new { b.SupplierMainC, b.SupplierSubC, b.ExpenseC, b.UnitPrice } into c
								select new
								{
									c.Key.SupplierMainC,
									c.Key.SupplierSubC,
									c.Key.ExpenseC,
									c.Key.UnitPrice,
									Quantity = c.Sum(b => b.Quantity),
									Total = c.Sum(b => b.Amount),
									TaxAmount = c.Sum(b => b.TaxAmount)
								};

			var union = from a in groupTruckExpense.Concat(groupExpenseD)
						group a by new { a.SupplierMainC, a.SupplierSubC, a.ExpenseC, a.UnitPrice }
							into b
							select new
							{
								SupplierMainC = b.Key.SupplierMainC,
								SupplierSubC = b.Key.SupplierSubC,
								ExpenseC = b.Key.ExpenseC,
								UnitPrice = b.Key.UnitPrice,
								Quantity = b.Sum((a => a.Quantity)),
								Total = b.Sum((a => a.Total)),
								TaxAmount = b.Sum((a => a.TaxAmount)),
							};

			union = union.OrderBy("SupplierMainC asc, SupplierSubC asc, ExpenseC asc, UnitPrice asc");

			// get data
			var data = from a in union
					   join b in _supplierRepository.GetAllQueryable() on new { a.SupplierMainC, a.SupplierSubC } equals new { b.SupplierMainC, b.SupplierSubC } into t1
					   from b in t1.DefaultIfEmpty()
					   join c in _expenseRepository.GetAllQueryable() on a.ExpenseC equals c.ExpenseC into t2
					   from c in t2.DefaultIfEmpty()
					   select new SupplierExpenseReportData()
					   {
						   SupplierMainC = a.SupplierMainC,
						   SupplierSubC = a.SupplierSubC,
						   SupplierN = b != null ? b.SupplierN : "",
						   SupplierShortN = b != null ? b.SupplierShortN : "",
						   ExpenseC = a.ExpenseC,
						   ExpenseN = c != null ? c.ExpenseN : "",
						   UnitPrice = a.UnitPrice,
						   Quantity = a.Quantity,
						   Total = a.Total,
						   TaxAmount = a.TaxAmount,
					   };

			data = data.OrderBy("SupplierMainC asc, SupplierSubC asc, ExpenseC asc, UnitPrice asc");

			var result = data.ToList();

			return result;
		}

		#endregion

		public Stream ExportPdfTruckExpense(TruckExpenseReportParam param)
		{
			Stream stream;
			DataRow row;
			TruckExpense.TruckExpenseDataTable dt;
			TruckExpense.ExpenseDetailDataTable dtDetail;
			Dictionary<string, string> dicLanguage;
			int intLanguage;

			// get data
			dt = new TruckExpense.TruckExpenseDataTable();
			dtDetail = new TruckExpense.ExpenseDetailDataTable();
			TruckExpenseReportData data;
			data = GetTruckExpenseList(param);

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
																 (con.TextKey == "LBLIMPORT" ||
																  con.TextKey == "LBLEXPORT" ||
																  con.TextKey == "LBLOTHER"
																  )).ToList();


			foreach (TextResource_D item in languages)
			{
				if (!dicLanguage.ContainsKey(item.TextKey))
				{
					dicLanguage.Add(item.TextKey, item.TextValue);
				}
			}

			if (data != null)
			{
				foreach (var truck in data.TruckList)
				{
					row = dt.NewRow();
					row["RegisteredNo"] = truck.RegisteredNo;
					var containerCount = data.CountainerCount.Where(x => x.TruckC == truck.TruckC).FirstOrDefault();
					int count = containerCount != null ? containerCount.ContainerCount : 0;
					row["ContainerCount"] = count;
					decimal? transportFee = data.TransportFees.Where(x => x.TruckC == truck.TruckC).Sum(x => x.TransportFee);
					row["TransportFee"] = transportFee;
					decimal? expense = data.Expenses.Where(x => x.TruckC == truck.TruckC).Sum(x => x.Value);
					row["Expense"] = expense;
					decimal? maintenanceExpense = data.MaintenanceExpenses.Where(x => x.TruckC == truck.TruckC).Sum(x => x.Value);
					row["MaintenanceExpense"] = maintenanceExpense;
					decimal? fixedExpense = data.FixedExpenses.Where(x => x.TruckC == truck.TruckC).Sum(x => x.Value);
					row["FixedExpense"] = fixedExpense;
					row["AverageExpense"] = count == 0 ? "" : ((expense + maintenanceExpense + fixedExpense) / count).ToString();
					dt.Rows.Add(row);
				}

				foreach (var item in data.ExpenseDetail.OrderBy(x => x.TruckC))
				{
					row = dtDetail.NewRow();

					row["TruckC"] = item.TruckC;
					row["RegisteredNo"] = item.RegisteredNo;
					row["CategoryC"] = item.CategoryC;
					row["CategoryN"] = item.CategoryN;
					row["Amount"] = item.Amount == null || item.Amount.ToString() == "" ? 0 : item.Amount;

					dtDetail.Rows.Add(row);
				}
			}

			stream = CrystalReport.Service.TruckExpense.ExportPdf.Exec(dt, dtDetail, intLanguage, param.DateFrom, param.DateTo);
			return stream;
		}

		private TruckExpenseReportData GetTruckExpenseList(TruckExpenseReportParam param)
		{
			// Lấy chi phí vận chuyển
			var incomData = from a in _truckRepository.GetAllQueryable()
							join b in _dispatchRepository.GetAllQueryable() on a.TruckC equals b.TruckC into t1
							from b in t1.DefaultIfEmpty()
							join t in _truckRepository.GetAllQueryable() on b.TruckC equals t.TruckC into t2
							from t in t2.DefaultIfEmpty()
							where ((param.DepC == "0" || param.DepC == t.DepC) &
									 (param.DateFrom == null || b.TransportD >= param.DateFrom) &
									(param.DateTo == null || b.TransportD <= param.DateTo) &
									(t.DisusedD == null || t.DisusedD >= param.DateFrom) &
									t.PartnerI == "0"
							  )
							select new TruckExpenseReportTransportFee
							{
								TruckC = b.TruckC,
								TransportFee = b.TransportFee
							};
			// Đếm số container cho mỗi xe
			var containerCount = from o in _orderDRepository.GetAllQueryable()
								 join d in _dispatchRepository.GetAllQueryable() on new { o.OrderD, o.OrderNo, o.DetailNo } equals
									 new { d.OrderD, d.OrderNo, d.DetailNo } into t1
								 from d in t1.DefaultIfEmpty()
								 join t in _truckRepository.GetAllQueryable() on d.TruckC equals t.TruckC into t2
								 from t in t2.DefaultIfEmpty()
								 where
									 ((param.DepC == "0" || param.DepC == t.DepC) &
									  (param.DateFrom == null || d.TransportD >= param.DateFrom) &
									  (param.DateTo == null || d.TransportD <= param.DateTo) &
									  (t.DisusedD == null || t.DisusedD >= param.DateFrom) &
									  t.PartnerI == "0"
										 )
								 group d by d.TruckC into g
								 from d in g.DefaultIfEmpty()
								 select new TruckExpenseReportContainerCount()
								 {
									 TruckC = d.TruckC,
									 ContainerCount = g.Count()
								 };

			// Lấy danh sách xe
			var truckList = from a in _truckRepository.GetAllQueryable()
							where (
								(a.DisusedD == null || a.DisusedD >= param.DateFrom) &
								a.PartnerI == "0"
							)
							select new TruckExpenseReportTruckList()
							{
								TruckC = a.TruckC,
								RegisteredNo = a.RegisteredNo
							};
			// Lấy chi phí quan ly noi bo
			var expenseByContainerExpense = from a in _expenseRepository.GetAllQueryable()
											join b in _expenseDetailRepository.GetAllQueryable() on a.ExpenseC equals b.ExpenseC
											join c in _dispatchRepository.GetAllQueryable() on new { b.OrderD, b.OrderNo, b.DetailNo, b.DispatchNo } equals new { c.OrderD, c.OrderNo, c.DetailNo, c.DispatchNo }
											join t in _truckRepository.GetAllQueryable() on c.TruckC equals t.TruckC
											where ((param.DepC == "0" || param.DepC == t.DepC) &
												   (param.DateFrom == null || c.TransportD >= param.DateFrom) &
												  (param.DateTo == null || c.TransportD <= param.DateTo) &
												  (t.DisusedD == null || t.DisusedD >= param.DateFrom) &
												  b.IsIncluded == "1" &
												  t.PartnerI == "0"
											)
											select new
											{
												Value = b.Amount,
												TruckC = c.TruckC
											};

			var expenseByTruckExpense = from te in _truckExpenseRepository.GetAllQueryable()
										join t in _truckRepository.GetAllQueryable() on te.Code equals t.TruckC
										where ((param.DepC == "0" || param.DepC == t.DepC) &
											   (param.DateFrom == null || te.TransportD >= param.DateFrom) &
											   (param.DateTo == null || te.TransportD <= param.DateTo) &
											   (t.DisusedD == null || t.DisusedD >= param.DateFrom) &
											   (te.ObjectI == "0") &
												t.PartnerI == "0"
											)
										select new
										{
											Value = te.Total,
											TruckC = te.Code
										};
			var expenseData = expenseByContainerExpense.Concat(expenseByTruckExpense);

			var expenseDataTotal = from a in expenseData
								   group a by new { a.TruckC }
									   into g
									   select new TruckExpenseReportExpense()
									   {
										   Value = g.Sum(x => x.Value),
										   TruckC = g.Key.TruckC
									   };

			// Lấy ds chi phí sữa chữa (dang update table, tam thoi = 0)
			var maintenance = new List<TruckExpenseReportExpense>();
			// Lấy ds chi phí cố định
			int daysOfMonthFrom = DateTime.DaysInMonth(param.DateFrom.Year, param.DateFrom.Month);
			int daysOfMonthTo = DateTime.DaysInMonth(param.DateTo.Year, param.DateTo.Month);
			var fixedExpense = from a in _fixedExpenseRepository.GetAllQueryable()
							   join b in _expenseRepository.GetAllQueryable() on a.ExpenseC equals b.ExpenseC into t1
							   from b in t1
							   join t in _truckRepository.GetAllQueryable() on a.TruckC equals t.TruckC into t2
							   from t in t2.DefaultIfEmpty()
							   where ((param.DepC == "0" || param.DepC == t.DepC) &
											 (a.Year >= param.DateFrom.Year) &
											 (a.Year <= param.DateTo.Year) &
											 (t.DisusedD == null || t.DisusedD >= param.DateFrom) &
											 t.PartnerI == "0"
										  )
							   select new TruckExpenseReportExpense()
							   {
								   Name = b.ExpenseN,
								   Value = param.DateFrom.Month == param.DateTo.Month ?
								   (((param.DateFrom.Month == 1 ? a.Month1 ?? 0 :
								   param.DateFrom.Month == 2 ? a.Month2 :
								   param.DateFrom.Month == 3 ? a.Month3 :
								   param.DateFrom.Month == 4 ? a.Month4 :
								   param.DateFrom.Month == 5 ? a.Month5 :
								   param.DateFrom.Month == 6 ? a.Month6 :
								   param.DateFrom.Month == 7 ? a.Month7 :
								   param.DateFrom.Month == 8 ? a.Month8 :
								   param.DateFrom.Month == 9 ? a.Month9 :
								   param.DateFrom.Month == 10 ? a.Month10 :
								   param.DateFrom.Month == 11 ? a.Month11 : a.Month12) / daysOfMonthFrom) * (param.DateTo.Day - param.DateFrom.Day + 1))
								   :
								   (((param.DateFrom.Month == 1 ? a.Month1 ?? 0 :
								   param.DateFrom.Month == 2 ? a.Month2 :
								   param.DateFrom.Month == 3 ? a.Month3 :
								   param.DateFrom.Month == 4 ? a.Month4 :
								   param.DateFrom.Month == 5 ? a.Month5 :
								   param.DateFrom.Month == 6 ? a.Month6 :
								   param.DateFrom.Month == 7 ? a.Month7 :
								   param.DateFrom.Month == 8 ? a.Month8 :
								   param.DateFrom.Month == 9 ? a.Month9 :
								   param.DateFrom.Month == 10 ? a.Month10 :
								   param.DateFrom.Month == 11 ? a.Month11 : a.Month12) / daysOfMonthFrom) * (daysOfMonthFrom - param.DateFrom.Day + 1))
								   +
								   (((param.DateTo.Month == 1 ? a.Month1 ?? 0 :
								   param.DateTo.Month == 2 ? a.Month2 :
								   param.DateTo.Month == 3 ? a.Month3 :
								   param.DateTo.Month == 4 ? a.Month4 :
								   param.DateTo.Month == 5 ? a.Month5 :
								   param.DateTo.Month == 6 ? a.Month6 :
								   param.DateTo.Month == 7 ? a.Month7 :
								   param.DateTo.Month == 8 ? a.Month8 :
								   param.DateTo.Month == 9 ? a.Month9 :
								   param.DateTo.Month == 10 ? a.Month10 :
								   param.DateTo.Month == 11 ? a.Month11 : a.Month12) / daysOfMonthTo) * (param.DateTo.Day)),
								   TruckC = a.TruckC
							   };

			// Lay Expense Detail cua cac xe

			var expenseDetailByExpense = from d in _dispatchRepository.GetAllQueryable()
										 join exd in _expenseDetailRepository.GetAllQueryable() on new { d.OrderD, d.OrderNo, d.DetailNo, d.DispatchNo }
											 equals new { exd.OrderD, exd.OrderNo, exd.DetailNo, exd.DispatchNo }
										 join exm in _expenseRepository.GetAllQueryable() on exd.ExpenseC equals exm.ExpenseC
										 where ((d.TransportD >= param.DateFrom) & (d.TransportD <= param.DateTo))
										 select new
										 {
											 Amount = exd.Amount,
											 TruckC = d.TruckC,
											 CategoryC = exm.CategoryC
										 };
			var expenseDetailByTruckExpense = from a in _truckExpenseRepository.GetAllQueryable()
											  join b in _expenseRepository.GetAllQueryable() on a.ExpenseC equals b.ExpenseC
											  where ((a.TransportD >= param.DateFrom) & (a.TransportD <= param.DateTo) &
													 (a.ObjectI == "0"))
											  select new
											  {
												  Amount = a.Total,
												  TruckC = a.Code,
												  CategoryC = b.CategoryC
											  };
			var ExpenseDetailUnion = expenseDetailByExpense.Concat(expenseDetailByTruckExpense);
			var expenseDetailUnionGroup = from a in ExpenseDetailUnion
										  group a by new { a.TruckC, a.CategoryC }
											  into g
											  select new
											  {
												  Amount = g.Sum(x => x.Amount),
												  TruckC = g.Key.TruckC,
												  CategoryC = g.Key.CategoryC
											  };

			var expenseByTruckDetail =
				from t in _truckRepository.GetAllQueryable()
				join a in expenseDetailUnionGroup on t.TruckC equals a.TruckC into t2
				from a in t2.DefaultIfEmpty()
				where (
					(param.DepC == "0" || param.DepC == t.DepC) &
					(t.DisusedD == null || t.DisusedD >= param.DateFrom) &
					t.PartnerI == "0"

					)
				group a by new { t.RegisteredNo, t.TruckC, a.CategoryC }
					into g
					select new TruckExpenseReportExpenseDetail()
					{
						RegisteredNo = g.Key.RegisteredNo,
						CategoryC = g.Key.CategoryC,
						Amount = g.Sum(x => x.Amount),
						TruckC = g.Key.TruckC
					};


			var expenseCategoryList = from exc in _expenseCategoryRepository.GetAllQueryable()
									  select exc;

			List<TruckExpenseReportExpenseDetail> expenseDetail = new List<TruckExpenseReportExpenseDetail>();
			foreach (var truck in expenseByTruckDetail)
			{
				foreach (var exc in expenseCategoryList)
				{
					expenseDetail.Add(new TruckExpenseReportExpenseDetail()
					{
						RegisteredNo = truck.RegisteredNo,
						CategoryN = exc.CategoryN,
						Amount = truck.CategoryC == exc.CategoryC ? truck.Amount : 0,
						TruckC = truck.TruckC
					});
				}
			}

			return new TruckExpenseReportData()
			{
				TruckList = truckList.ToList(),
				TransportFees = incomData.ToList(),
				Expenses = expenseDataTotal.ToList(),
				FixedExpenses = fixedExpense.ToList(),
				MaintenanceExpenses = maintenance.ToList(),
				CountainerCount = containerCount.ToList(),
				ExpenseDetail = expenseDetail.ToList()
			};
		}
		public void SaveReport()
		{
			_unitOfWork.Commit();
		}
		private List<string> ConvertStringToList(string value)
		{
			var result = new List<string>();
			if (string.IsNullOrEmpty(value) || value == "null") return result;
			var list = value.Split(',');
			if (!list.Any()) return result;
			return list.ToList();
		}
	}
}