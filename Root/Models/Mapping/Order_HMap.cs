using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class Order_HMap : EntityTypeConfiguration<Order_H>
	{
		public Order_HMap()
		{
			// Primary Key
			this.HasKey(t => new { t.OrderD, t.OrderNo });

			// Properties
			this.Property(t => t.OrderD)
				.HasColumnType("date")
				.IsRequired();

			this.Property(t => t.OrderNo)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(7);

			this.Property(t => t.OrderDepC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.EntryClerkC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.CustomerMainC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.CustomerSubC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.OrderPatternC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.OrderTypeI)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.BLBK)
				//.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(20);

			this.Property(t => t.ShippingCompanyC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.ShippingCompanyN)
				.HasMaxLength(200);

			this.Property(t => t.VesselC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.VesselN)
				.HasMaxLength(50);

			this.Property(t => t.VoyageN)
				.HasMaxLength(20);

			this.Property(t => t.ShipperC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.ShipperN)
				.HasMaxLength(50);

			this.Property(t => t.LoadingPlaceC)
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.LoadingPlaceN)
				.HasMaxLength(200);

			this.Property(t => t.LoadingDT)
				.HasColumnType("datetime2");

			this.Property(t => t.StopoverPlaceC)
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.StopoverPlaceN)
				.HasMaxLength(200);

			this.Property(t => t.StopoverDT)
				.HasColumnType("datetime2");

			this.Property(t => t.DischargePlaceC)
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.DischargePlaceN)
				.HasMaxLength(200);

			this.Property(t => t.DischargeDT)
				.HasColumnType("datetime2");

			this.Property(t => t.JobNo)
				.IsUnicode(false)
				.HasMaxLength(20);

			this.Property(t => t.Description)
				  .HasMaxLength(255);

			this.Property(t => t.TotalPrice)
				.HasPrecision(12,0);

			this.Property(t => t.TotalLoads)
				.HasPrecision(7, 1);

			this.Property(t => t.ETD)
				.HasColumnType("datetime2");

			this.Property(t => t.DischargePortC)
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.DischargePortN)
				.HasMaxLength(200);

			this.Property(t => t.IsCollected)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.ClosingDT)
				.HasColumnType("datetime2");

			this.Property(t => t.ContractNo)
				.HasMaxLength(200);

			this.Property(t => t.TermContReturnNo)
				.HasPrecision(10, 0);

			this.Property(t => t.TermContReturnDT)
				.HasColumnType("datetime2");

			this.Property(t => t.CustomerPayLiftLoweredMainC)
				//.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.CustomerPayLiftLoweredSubC)
				//.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(3);
			// Table & Column Mappings
			this.ToTable("Order_H");
			this.Property(t => t.OrderD).HasColumnName("OrderD");
			this.Property(t => t.OrderNo).HasColumnName("OrderNo");
			this.Property(t => t.OrderDepC).HasColumnName("OrderDepC");
			this.Property(t => t.EntryClerkC).HasColumnName("EntryCleckC");
			this.Property(t => t.CustomerMainC).HasColumnName("CustomerMainC");
			this.Property(t => t.CustomerSubC).HasColumnName("CustomerSubC");
			this.Property(t => t.OrderPatternC).HasColumnName("OrderPatternC");
			this.Property(t => t.OrderTypeI).HasColumnName("OrderTypeI");
			this.Property(t => t.BLBK).HasColumnName("BLBK");
			this.Property(t => t.ShippingCompanyC).HasColumnName("ShippingCompanyC");
			this.Property(t => t.ShippingCompanyN).HasColumnName("ShippingCompanyN");
			this.Property(t => t.VesselC).HasColumnName("VesselC");
			this.Property(t => t.VesselN).HasColumnName("VesselN");
			this.Property(t => t.VoyageN).HasColumnName("VoyageN");
			this.Property(t => t.ShipperC).HasColumnName("ShipperC");
			this.Property(t => t.ShipperN).HasColumnName("ShipperN");
			this.Property(t => t.LoadingPlaceC).HasColumnName("LoadingPlaceC");
			this.Property(t => t.LoadingPlaceN).HasColumnName("LoadingPlaceN");
			this.Property(t => t.LoadingDT).HasColumnName("LoadingDT");
			this.Property(t => t.StopoverPlaceC).HasColumnName("StopoverPlaceC");
			this.Property(t => t.StopoverPlaceN).HasColumnName("StopoverPlaceN");
			this.Property(t => t.StopoverDT).HasColumnName("StopoverDT");
			this.Property(t => t.DischargePlaceC).HasColumnName("DischargePlaceC");
			this.Property(t => t.DischargePlaceN).HasColumnName("DischargePlaceN");
			this.Property(t => t.DischargeDT).HasColumnName("DischargeDT");
			this.Property(t => t.JobNo).HasColumnName("JobNo");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.TotalPrice).HasColumnName("TotalPrice");
			this.Property(t => t.Quantity20HC).HasColumnName("Quantity20HC");
			this.Property(t => t.Quantity40HC).HasColumnName("Quantity40HC");
			this.Property(t => t.Quantity45HC).HasColumnName("Quantity45HC");
			this.Property(t => t.TotalLoads).HasColumnName("TotalLoads");
			this.Property(t => t.ETD).HasColumnName("ETD");
			this.Property(t => t.DischargePortC).HasColumnName("DischargePortC");
			this.Property(t => t.DischargePortN).HasColumnName("DischargePortN");
			this.Property(t => t.IsCollected).HasColumnName("IsCollected");
			this.Property(t => t.ClosingDT).HasColumnName("ClosingDT");
			this.Property(t => t.ContractNo).HasColumnName("ContractNo");
			this.Property(t => t.CustomerPayLiftLoweredMainC).HasColumnName("CustomerPayLiftLoweredMainC");
			this.Property(t => t.CustomerPayLiftLoweredSubC).HasColumnName("CustomerPayLiftLoweredSubC");
			this.Property(t => t.TermContReturnNo).HasColumnName("TermContReturnNo");
			this.Property(t => t.TermContReturnDT).HasColumnName("TermContReturnDT");
		}
	}
}
