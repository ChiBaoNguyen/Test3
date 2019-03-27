using AutoMapper;
using Root.Models;
using Root.Models.Mapping;
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
using Website.ViewModels.Route;
using Website.ViewModels.Ship;
using Website.ViewModels.Surcharge;
using Website.ViewModels.Supplier;
using Website.ViewModels.Trailer;
using Website.ViewModels.Truck;
using Website.ViewModels.TruckExpense;
using Website.ViewModels.FixedExpense;
using Website.ViewModels.Feature;
using Website.ViewModels.PartnerPayment;

namespace Service.Mappers
{
	public class ViewModelToDomainMappingProfile : Profile
	{
		public override string ProfileName
		{
			get
			{
				return "ViewModelToDomainMappingProfile";
			}
		}

		protected override void Configure()
		{
			//Mapper.CreateMap<ResourceActivityViewModel, ResourceActivity>();
			//Mapper.CreateMap<RegisterViewModel, ApplicationUser>().ForMember(user => user.UserName, vm => vm.MapFrom(rm => rm.Email));
			Mapper.CreateMap<CustomerViewModel, Customer_M>();
			Mapper.CreateMap<InvoiceViewModel, Customer_M>();
			Mapper.CreateMap<ContainerTypeViewModel, ContainerType_M>();
			Mapper.CreateMap<ContainerSizeViewModel, ContainerSize_M>();
			Mapper.CreateMap<DepartmentViewModel, Department_M>();
			Mapper.CreateMap<EmployeeViewModel, Employee_M>();
			Mapper.CreateMap<OrderViewModel, Order_H>();
			Mapper.CreateMap<ContainerViewModel, Order_D>();
			Mapper.CreateMap<CommodityViewModel, Commodity_M>();
			Mapper.CreateMap<LocationViewModel, Location_M>();
			Mapper.CreateMap<ShipperViewModel, Shipper_M>();
			Mapper.CreateMap<ShippingCompanyViewModel, ShippingCompany_M>();
			Mapper.CreateMap<VesselViewModel, Vessel_M>();
			Mapper.CreateMap<DriverViewModel, Driver_M>();
			Mapper.CreateMap<SupplierViewModel, Supplier_M>();
			Mapper.CreateMap<SupplierSettlementViewModel, SupplierSettlement_M>();
			Mapper.CreateMap<ContractTariffPatternViewModel, ContractTariffPattern_M>();
			//Mapper.CreateMap<EmployeeViewModel, Department_M>();
			Mapper.CreateMap<DispatchViewModel, Dispatch_D>();
			Mapper.CreateMap<TrailerViewModel, Trailer_M>();
			Mapper.CreateMap<TruckViewModel, Truck_M>();
			Mapper.CreateMap<OrderPatternViewModel, OrderPattern_M>();
			Mapper.CreateMap<CustomerSettlementViewModel, CustomerSettlement_M>();
			Mapper.CreateMap<PartnerViewModel, Partner_M>();
			Mapper.CreateMap<PartnerSettlementViewModel, PartnerSettlement_M>();
			Mapper.CreateMap<PartnerContractTariffPatternViewModel, PartnerContractTariffPattern_M>();
			Mapper.CreateMap<ExpenseViewModel, Expense_M>();
			Mapper.CreateMap<TransportConfirmOrderViewModel, Order_H>();
			Mapper.CreateMap<TransportConfirmContainerViewModel, Order_D>();
			Mapper.CreateMap<TransportConfirmDispatchViewModel, Dispatch_D>();
			Mapper.CreateMap<ExpenseDetailViewModel, Expense_D>();
			Mapper.CreateMap<SurchargeDetailViewModel, Surcharge_D>();
			Mapper.CreateMap<AllowanceDetailViewModel, Allowance_D>();
			Mapper.CreateMap<ExpenseCategoryViewModel, ExpenseCategory_M>();
			Mapper.CreateMap<TruckExpenseViewModel, TruckExpense_D>();
			Mapper.CreateMap<BasicViewModel, Basic_S>();
			Mapper.CreateMap<FixedExpenseViewModel, FixedExpense_D>();
			Mapper.CreateMap<LicenseViewModel, License_M>();
			Mapper.CreateMap<InspectionViewModel, Inspection_M>();
			Mapper.CreateMap<ModelViewModel, Model_M>();
			Mapper.CreateMap<MaintenanceItemViewModel, MaintenanceItem_M>();
			Mapper.CreateMap<DriverLicenseViewModel, DriverLicense_M>();
			Mapper.CreateMap<DriverLicenseUpdateViewModel, DriverLicenseUpdate_D>();
			Mapper.CreateMap<MaintenanceItemDetailViewModel, MaintenanceItem_D>();
			Mapper.CreateMap<InspectionPlanDetailViewModel, InspectionPlan_D>();
			Mapper.CreateMap<InspectionDetailViewModel, Inspection_D>();
			Mapper.CreateMap<MaintenancePlanDetailViewModel, MaintenancePlan_D>();
			Mapper.CreateMap<MaintenanceDetailViewModel, Maintenance_D>();
			Mapper.CreateMap<LiabilitiesViewModel, Liabilities_D>();
			Mapper.CreateMap<FeatureViewModel, Feature_M>();
			Mapper.CreateMap<FuelConsumptionViewModel, FuelConsumption_M>();
			Mapper.CreateMap<FuelConsumptionDetailParams, FuelConsumption_D>();
			Mapper.CreateMap<CustomerPaymentViewModel, CustomerPayment_D>();
			Mapper.CreateMap<CustomerGrossProfitViewModel, CustomerGrossProfit_M>();
			Mapper.CreateMap<RouteViewModel, Route_H>();
			Mapper.CreateMap<RouteExpenseViewModel, Route_D>();
			Mapper.CreateMap<CustomerPricingViewModel, CustomerPricing_H>();
			Mapper.CreateMap<CustomerPricingDetailViewModel, CustomerPricing_D>();
			Mapper.CreateMap<OperationViewModel, Operation_M>();
            Mapper.CreateMap<PartnerPaymentViewModel, PartnerPayment_D>();
			Mapper.CreateMap<LiabilitiesExpenseViewModel, LiabilitiesItem_D>();
			Mapper.CreateMap<PartnerPaymentViewModel, SupplierPayment_D>();
			Mapper.CreateMap<CompanyExpenseViewModel, CompanyExpense_D>();
		}
	}
}