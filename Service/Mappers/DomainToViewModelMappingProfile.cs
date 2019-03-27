using AutoMapper;
using Root.Models;
using Website.ViewModels.Allowance;
using Website.ViewModels.Basic;
using Website.ViewModels.CompanyExpense;
using Website.ViewModels.Container;
using Website.ViewModels.Customer;
using Website.ViewModels.CustomerPayment;
using Website.ViewModels.CustomerPricing;
using Website.ViewModels.Department;
using Website.ViewModels.Dispatch;
using Website.ViewModels.Driver;
using Website.ViewModels.DriverLicense;
using Website.ViewModels.DriverLicenseUpdate;
using Website.ViewModels.Employee;
using Website.ViewModels.Expense;
using Website.ViewModels.Feature;
using Website.ViewModels.FixedExpense;
using Website.ViewModels.FuelConsumption;
using Website.ViewModels.Inspection;
using Website.ViewModels.InspectionDetail;
using Website.ViewModels.InspectionPlanDetail;
using Website.ViewModels.Liabilities;
using Website.ViewModels.License;
using Website.ViewModels.Location;
using Website.ViewModels.MaintenanceDetail;
using Website.ViewModels.MaintenanceItem;
using Website.ViewModels.MaintenanceItemDetail;
using Website.ViewModels.MaintenancePlanDetail;
using Website.ViewModels.Model;
using Website.ViewModels.Operation;
using Website.ViewModels.Order;
using Website.ViewModels.OrderPattern;
using Website.ViewModels.Partner;
using Website.ViewModels.PartnerPayment;
using Website.ViewModels.Route;
using Website.ViewModels.Ship;
using Website.ViewModels.Supplier;
using Website.ViewModels.Surcharge;
using Website.ViewModels.TextResource;
using Website.ViewModels.Trailer;
using Website.ViewModels.Truck;
using Website.ViewModels.TruckExpense;

namespace Service.Mappers
{
	public class DomainToViewModelMappingProfile : Profile
	{
		public override string ProfileName
		{
			get
			{
				return "DomainToViewModelMappingProfile";
			}
		}


		protected override void Configure()
		{
			//Mapper.CreateMap<Resource, ResourceViewModel>();
			//Mapper.CreateMap<ResourceActivity,ResourceActivityViewModel>()
			//    .ForMember(vm => vm.ActivityDateString, dm=> dm.MapFrom(dModel => dModel.ActivityDate.ToLongDateString()));

			Mapper.CreateMap<Customer_M, CustomerViewModel>();
			Mapper.CreateMap<Customer_M, InvoiceViewModel>();
            Mapper.CreateMap<CustomerSettlement_M, CustomerSettlementViewModel>();
			Mapper.CreateMap<TextResource_D, TextResourceViewModel>();
			Mapper.CreateMap<ContainerType_M, ContainerTypeViewModel>();
			Mapper.CreateMap<ContainerSize_M, ContainerSizeViewModel>();
			Mapper.CreateMap<Department_M, DepartmentViewModel>();
			Mapper.CreateMap<Employee_M, EmployeeViewModel>();
			Mapper.CreateMap<Location_M, LocationViewModel>();
			Mapper.CreateMap<OrderPattern_M, OrderPatternViewModel>();
			Mapper.CreateMap<ShippingCompany_M, ShippingCompanyViewModel>();
			Mapper.CreateMap<Vessel_M, VesselViewModel>();
			Mapper.CreateMap<Commodity_M, CommodityViewModel>();
			Mapper.CreateMap<Order_H, OrderViewModel>();
			Mapper.CreateMap<Order_D, ContainerViewModel>();
			Mapper.CreateMap<Shipper_M, ShipperViewModel>();
			Mapper.CreateMap<Driver_M, DriverViewModel>();
			Mapper.CreateMap<Supplier_M, SupplierViewModel>();
			Mapper.CreateMap<Dispatch_D, DispatchViewModel>();
			Mapper.CreateMap<ContractTariffPattern_M, ContractTariffPatternViewModel>();
			//Mapper.CreateMap<Department_M, EmployeeViewModel>()
			//    .ForMember(src => src.DepC, dest => dest.MapFrom(s => s.DepC));
			Mapper.CreateMap<Trailer_M, TrailerViewModel>();
			Mapper.CreateMap<Truck_M, TruckViewModel>();
			Mapper.CreateMap<Order_H, TransportConfirmOrderViewModel>();
			Mapper.CreateMap<Order_D, TransportConfirmContainerViewModel>();
			Mapper.CreateMap<Dispatch_D, TransportConfirmDispatchViewModel>();
			Mapper.CreateMap<Partner_M, PartnerViewModel>();
            Mapper.CreateMap<Partner_M, PartnerInvoiceViewModel>();
            Mapper.CreateMap<PartnerSettlement_M, PartnerSettlementViewModel>();
            Mapper.CreateMap<PartnerContractTariffPattern_M, PartnerContractTariffPatternViewModel>();
			Mapper.CreateMap<Expense_M, ExpenseViewModel>();
			Mapper.CreateMap<Expense_D, ExpenseDetailViewModel>();
			Mapper.CreateMap<ExpenseViewModel, ExpenseDetailViewModel>();
            Mapper.CreateMap<Supplier_M, SupplierViewModel>();
            Mapper.CreateMap<Supplier_M, SupplierInvoiceViewModel>();
            Mapper.CreateMap<SupplierSettlement_M, SupplierSettlementViewModel>();
			Mapper.CreateMap<Surcharge_D, SurchargeDetailViewModel>();
			Mapper.CreateMap<Allowance_D, AllowanceDetailViewModel>();
			Mapper.CreateMap<ExpenseViewModel, SurchargeDetailViewModel>();
			Mapper.CreateMap<ExpenseViewModel, AllowanceDetailViewModel>();
			Mapper.CreateMap<ExpenseBasicViewModel, SurchargeDetailViewModel>();
			Mapper.CreateMap<ExpenseBasicViewModel, AllowanceDetailViewModel>();
            Mapper.CreateMap<ExpenseCategory_M, ExpenseCategoryViewModel>();
			Mapper.CreateMap<TruckExpense_D, TruckExpenseViewModel>();
			Mapper.CreateMap<Basic_S, BasicViewModel>();
			Mapper.CreateMap<FixedExpense_D, FixedExpenseViewModel>();
			Mapper.CreateMap<Inspection_M, InspectionViewModel>();
			Mapper.CreateMap<Model_M, ModelViewModel>();
			Mapper.CreateMap<MaintenanceItem_M, MaintenanceItemViewModel>();
			Mapper.CreateMap<DriverLicense_M, DriverLicenseViewModel>();
			Mapper.CreateMap<DriverLicenseUpdate_D, DriverLicenseUpdateViewModel>();
			Mapper.CreateMap<MaintenanceItem_D, MaintenanceItemDetailViewModel>();
			Mapper.CreateMap<InspectionPlan_D, InspectionPlanDetailViewModel>();
			Mapper.CreateMap<Inspection_D, InspectionDetailViewModel>();
			Mapper.CreateMap<MaintenancePlan_D, MaintenancePlanDetailViewModel>();
			Mapper.CreateMap<Maintenance_D, MaintenanceDetailViewModel>();
			Mapper.CreateMap<License_M, LicenseViewModel>();
			Mapper.CreateMap<Liabilities_D, LiabilitiesViewModel>();
			Mapper.CreateMap<Feature_M, FeatureViewModel>();
			Mapper.CreateMap<FuelConsumption_M, FuelConsumptionViewModel>();
			Mapper.CreateMap<FuelConsumption_D, FuelConsumptionDetailParams>();
			Mapper.CreateMap<CustomerPayment_D, CustomerPaymentViewModel>();
			Mapper.CreateMap<CustomerGrossProfit_M, CustomerGrossProfitViewModel>();
			Mapper.CreateMap<Route_H, RouteViewModel>();
			Mapper.CreateMap<Route_D, RouteExpenseViewModel>();
			Mapper.CreateMap<CustomerPricing_H, CustomerPricingViewModel>();
			Mapper.CreateMap<CustomerPricing_D, CustomerPricingDetailViewModel>();
			Mapper.CreateMap<Route_H, SuggestedRoute>();
			Mapper.CreateMap<Route_D, CustomerPricingDetailViewModel>();
            Mapper.CreateMap<Trailer_M, SuggestedWarningTrailer>();
            Mapper.CreateMap<Trailer_M, TrailerViewModel>();
			Mapper.CreateMap<Operation_M, OperationViewModel>();
            Mapper.CreateMap<PartnerPayment_D, PartnerPaymentViewModel>();
			Mapper.CreateMap<LiabilitiesItem_D, LiabilitiesExpenseViewModel>();
			Mapper.CreateMap<SupplierPayment_D, PartnerPaymentViewModel>();
			Mapper.CreateMap<CompanyExpense_D, CompanyExpenseViewModel>();
		}
	}
}