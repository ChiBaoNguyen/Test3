using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Root.Migrations;
using Root.Models;
using Root.Models.Mapping;

namespace Root.Data
{
	public partial class SGTSVNDBContext : IdentityDbContext<User>
	{
		//static SGTSVNDBContext()
		//{
		//	//Database.SetInitializer<SGTSVNDBContext>(null);
		//}

		public SGTSVNDBContext()
			: base("Name=SGTSVNDBContext", false) // false: giam 2 giay
		{
		}

		public DbSet<Commodity_M> Commodity_M { get; set; }
		public DbSet<ContainerType_M> ContainerType_M { get; set; }
		public DbSet<ContainerSize_M> ContainerSize_M { get; set; }
		public DbSet<Customer_M> Customer_M { get; set; }
		public DbSet<CustomerSettlement_M> CustomerSettlement_M { get; set; }
		public DbSet<Department_M> Department_M { get; set; }
		public DbSet<Employee_M> Employee_M { get; set; }
		public DbSet<Language_M> Language_M { get; set; }
		public DbSet<Location_M> Location_M { get; set; }
		public DbSet<Order_D> Order_D { get; set; }
		public DbSet<Order_H> Order_H { get; set; }
		public DbSet<OrderPattern_M> OrderPattern_M { get; set; }
		public DbSet<ContractTariffPattern_M> ContractTariffPattern_M { get; set; }
		public DbSet<Screen_M> Screen_M { get; set; }
		public DbSet<ShippingCompany_M> ShippingCompany_M { get; set; }
		public DbSet<TextResource_D> TextResource_D { get; set; }
		public DbSet<Vessel_M> Vessel_M { get; set; }
		public DbSet<Shipper_M> Shipper_M { get; set; }
		public DbSet<Driver_M> Driver_M { get; set; }
		public DbSet<Supplier_M> Supplier_M { get; set; }
        public DbSet<SupplierSettlement_M> SupplierSettlement_M { get; set; }
		public DbSet<Partner_M> Partner_M { get; set; }
		public DbSet<PartnerSettlement_M> PartnerSettlement_M { get; set; }
        public DbSet<PartnerContractTariffPattern_M> PartnerContractTariffPattern_M { get; set; }
		public DbSet<Dispatch_D> Dispatch_D { get; set; }
		public DbSet<Trailer_M> Trailer_M { get; set; }
		public DbSet<Truck_M> Truck_M { get; set; }
		public DbSet<Expense_D> Expense_D { get; set; }
		public DbSet<Expense_M> Expense_M { get; set; }
		public DbSet<Surcharge_D> Surcharge_D { get; set; }
		public DbSet<Allowance_D> Allowance_D { get; set; }
		public DbSet<DriverAllowance_M> DriverAllowance_M { get; set; }
        public DbSet<ExpenseCategory_M> ExpenseCategory_M { get; set; }
		public DbSet<TruckExpense_D> TruckExpense_D { get; set; }
		public DbSet<Basic_S> Basic_S { get; set; }
		public DbSet<FixedExpense_D> FixedExpense_D { get; set; }
		public DbSet<Client> Clients { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }
		public DbSet<Inspection_M> Inspection_M { get; set; }
		public DbSet<Model_M> Model_M { get; set; }
		public DbSet<MaintenanceItem_M> MaintenanceItem_M { get; set; }
		public DbSet<DriverLicense_M> DriverLicense_M { get; set; }
		public DbSet<DriverLicenseUpdate_D> DriverLicenseUpdate_D { get; set; }
		public DbSet<License_M> License_M { get; set; }
		public DbSet<RoleFeatures> RoleFeatures { get; set; }
		public DbSet<Feature_M> Feature_M { get; set; }
		public DbSet<MaintenanceItem_D> MaintenanceItem_D { get; set; }
		public DbSet<InspectionPlan_D> InspectionPlan_D { get; set; }
		public DbSet<Inspection_D> Inspection_D { get; set; }
		public DbSet<MaintenancePlan_D> MaintenancePlan_D { get; set; }
		public DbSet<Maintenance_D> Maintenance_D { get; set; }
		public DbSet<Liabilities_D> Liabilities_D { get; set; }
		public DbSet<FuelConsumption_M> FuelConsumption_M { get; set; }
		public DbSet<FuelConsumption_D> FuelConsumption_D { get; set; }
		public DbSet<CustomerPayment_D> CustomerPayment_D { get; set; }
		public DbSet<CustomerBalance_D> CustomerBalance_D { get; set; }
		public DbSet<CustomerGrossProfit_M> CustomerGrossProfit_M { get; set; }
		public DbSet<Route_H> Route_H { get; set; }
		public DbSet<Route_D> Route_D { get; set; }
		public DbSet<CustomerPricing_H> CustomerPricing_H { get; set; }
		public DbSet<CustomerPricing_D> CustomerPricing_D { get; set; }
		public DbSet<CustomerPricingLocation_D> CustomerPricingLocation_D { get; set; }
		public DbSet<Operation_M> Operation_M { get; set; }
        public DbSet<GpsLocation_D> GpsLocation_D { get; set; }
		public DbSet<ChatMessage> ChatMessage { get; set; }
        public DbSet<PartnerPayment_D> PartnerPayment_D { get; set; }
        public DbSet<PartnerBalance_D> PartnerBalance_D { get; set; }
		public DbSet<LiabilitiesItem_D> LiabilitiesItem_D { get; set; }
		public DbSet<SupplierPayment_D> SupplierPayment_D { get; set; }
		public DbSet<SupplierBalance_D> SupplierBalance_D { get; set; }
		public DbSet<CompanyExpense_D> CompanyExpense_D { get; set; }
		public DbSet<LocationDetail_M> LocationDetail_M { get; set; }
		public DbSet<TransportDistance_M> TransportDistance_M { get; set; }
		public DbSet<CalculateDriverAllowance_M> CalculateDriverAllowance_M { get; set; }
        public DbSet<UploadImageMobile> UploadImages { get; set; }
		public DbSet<ContractPartnerPattern_M> ContractPartnerPattern_M { get; set; }
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Configurations.Add(new Commodity_MMap());
			modelBuilder.Configurations.Add(new ContainerType_MMap());
			modelBuilder.Configurations.Add(new ContainerSize_MMap());
			modelBuilder.Configurations.Add(new Customer_MMap());
			modelBuilder.Configurations.Add(new CustomerSettlement_MMap());
			modelBuilder.Configurations.Add(new Department_MMap());
			modelBuilder.Configurations.Add(new Employee_MMap());
			modelBuilder.Configurations.Add(new Language_MMap());
			modelBuilder.Configurations.Add(new Location_MMap());
			modelBuilder.Configurations.Add(new Order_DMap());
			modelBuilder.Configurations.Add(new Order_HMap());
			modelBuilder.Configurations.Add(new OrderPattern_MMap());
			modelBuilder.Configurations.Add(new ContractTariffPattern_MMap());
			modelBuilder.Configurations.Add(new Screen_MMap());
			modelBuilder.Configurations.Add(new ShippingCompany_MMap());
			modelBuilder.Configurations.Add(new TextResource_DMap());
			modelBuilder.Configurations.Add(new Vessel_MMap());
			modelBuilder.Configurations.Add(new Shipper_MMap());
			modelBuilder.Configurations.Add(new Driver_MMap());
			modelBuilder.Configurations.Add(new Supplier_MMap());
            modelBuilder.Configurations.Add(new SupplierSettlement_MMap());
			modelBuilder.Configurations.Add(new Partner_MMap());
			modelBuilder.Configurations.Add(new PartnerSettlement_MMap());
            modelBuilder.Configurations.Add(new PartnerContractTariffPattern_MMap());
			modelBuilder.Configurations.Add(new Dispatch_DMap());
			modelBuilder.Configurations.Add(new Trailer_MMap());
			modelBuilder.Configurations.Add(new Truck_MMap());
			modelBuilder.Configurations.Add(new Expense_DMap());
			modelBuilder.Configurations.Add(new Expense_MMap());
			modelBuilder.Configurations.Add(new Surcharge_DMap());
			modelBuilder.Configurations.Add(new Allowance_DMap());
			modelBuilder.Configurations.Add(new DriverAllowance_MMap());
            modelBuilder.Configurations.Add(new ExpenseCategory_MMap());
			modelBuilder.Configurations.Add(new TruckExpense_DMap());
			modelBuilder.Configurations.Add(new Basic_SMap());
			modelBuilder.Configurations.Add(new FixedExpense_DMap());
			modelBuilder.Configurations.Add(new Client_Map());
			modelBuilder.Configurations.Add(new RefreshToken_Map());
			modelBuilder.Configurations.Add(new Inspection_MMap());
			modelBuilder.Configurations.Add(new Model_MMap());
			modelBuilder.Configurations.Add(new MaintenanceItem_MMap());
			modelBuilder.Configurations.Add(new DriverLicense_MMap());
			modelBuilder.Configurations.Add(new DriverLicenseUpdate_DMap());
			modelBuilder.Configurations.Add(new MaintenanceItem_DMap());
			modelBuilder.Configurations.Add(new InspectionPlan_DMap());
			modelBuilder.Configurations.Add(new Inspection_DMap());
			modelBuilder.Configurations.Add(new MaintenancePlan_DMap());
			modelBuilder.Configurations.Add(new Maintenance_DMap());
			modelBuilder.Configurations.Add(new License_MMap());
			modelBuilder.Configurations.Add(new Feature_MMap());
			modelBuilder.Configurations.Add(new RoleFeatures_Map());
			modelBuilder.Configurations.Add(new Liabilities_DMap());
			modelBuilder.Configurations.Add(new FuelConsumption_MMap());
			modelBuilder.Configurations.Add(new FuelConsumption_DMap());
			modelBuilder.Configurations.Add(new CustomerPayment_DMap());
			modelBuilder.Configurations.Add(new CustomerBalance_DMap());
			modelBuilder.Configurations.Add(new CustomerGrossProfit_MMap());
			modelBuilder.Configurations.Add(new Route_HMap());
			modelBuilder.Configurations.Add(new Route_DMap());
			modelBuilder.Configurations.Add(new CustomerPricing_HMap());
			modelBuilder.Configurations.Add(new CustomerPricing_DMap());
			modelBuilder.Configurations.Add(new CustomerPricingLocation_DMap());
			modelBuilder.Configurations.Add(new Operation_MMap());
            modelBuilder.Configurations.Add(new GpsLocation_DMap());
			modelBuilder.Configurations.Add(new ChatMessageMap());
            modelBuilder.Configurations.Add(new PartnerPayment_DMap());
            modelBuilder.Configurations.Add(new PartnerBalance_DMap());
			modelBuilder.Configurations.Add(new LiabilitiesItem_DMap());
			modelBuilder.Configurations.Add(new SupplierPayment_DMap());
			modelBuilder.Configurations.Add(new SupplierBalance_DMap());
			modelBuilder.Configurations.Add(new CompanyExpense_DMap());
			modelBuilder.Configurations.Add(new LocationDetail_MMap());
			modelBuilder.Configurations.Add(new TransportDistance_MMap());
			modelBuilder.Configurations.Add(new CalculateDriverAllowance_MMap());
            modelBuilder.Configurations.Add(new UploadImageMobile_Map());
			modelBuilder.Configurations.Add(new ContractPartnerPattern_MMap());
			base.OnModelCreating(modelBuilder); // This needs to go before the other rules!
			//authentication db
			modelBuilder.Entity<User>().ToTable("Users");
			modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
			modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
			modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
			modelBuilder.Entity<IdentityRole>().ToTable("Roles");
		}

		public virtual void Commit()
		{
			base.SaveChanges();
		}
	}
}
