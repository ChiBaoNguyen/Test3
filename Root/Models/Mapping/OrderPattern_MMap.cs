using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
    public class OrderPattern_MMap : EntityTypeConfiguration<OrderPattern_M>
    {
        public OrderPattern_MMap()
        {
            // Primary Key
            this.HasKey(t => new {t.CustomerMainC, t.CustomerSubC, t.OrderPatternC});

            // Properties
            this.Property(t => t.CustomerMainC)
				.IsRequired()
                .IsUnicode(false)
				.HasMaxLength(5);


            this.Property(t => t.CustomerSubC)
				.IsRequired()
				.IsUnicode(false)
                .HasMaxLength(3);

			this.Property(t => t.OrderPatternC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.OrderPatternN)
				.HasMaxLength(50);

			this.Property(t => t.OrderTypeI)
				.IsFixedLength()
				.IsUnicode(false)
				.HasMaxLength(1);

            this.Property(t => t.ShippingCompanyC)
				.IsUnicode(false)
                .HasMaxLength(5);

			this.Property(t => t.VesselC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.VoyageN)
				.HasMaxLength(20);

			this.Property(t => t.ShipperC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.LoadingPlaceC)
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.StopoverPlaceC)
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.DischargePlaceC)
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.CalculateByTon)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.CommodityC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.CommodityN)
				.HasMaxLength(50);

			this.Property(t => t.UnitPrice)
				.HasPrecision(10, 0);

			this.Property(t => t.ContainerTypeC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.ContainerSizeI)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("OrderPattern_M");
			this.Property(t => t.CustomerMainC).HasColumnName("CustomerMainC");
			this.Property(t => t.CustomerSubC).HasColumnName("CustomerSubC");
			this.Property(t => t.OrderPatternC).HasColumnName("OrderPatternC");
			this.Property(t => t.OrderPatternN).HasColumnName("OrderPatternN");
			this.Property(t => t.OrderTypeI).HasColumnName("OrderTypeI");
			this.Property(t => t.ShippingCompanyC).HasColumnName("ShippingCompanyC");
			this.Property(t => t.VesselC).HasColumnName("VesselC");
			this.Property(t => t.VoyageN).HasColumnName("VoyageN");
			this.Property(t => t.ShipperC).HasColumnName("ShipperC");
			this.Property(t => t.LoadingPlaceC).HasColumnName("LoadingPlaceC");
			this.Property(t => t.StopoverPlaceC).HasColumnName("StopoverPlaceC");
			this.Property(t => t.DischargePlaceC).HasColumnName("DischargePlaceC");
			this.Property(t => t.CommodityC).HasColumnName("CommodityC");
			this.Property(t => t.CommodityN).HasColumnName("CommodityN");
			this.Property(t => t.CalculateByTon).HasColumnName("CalculateByTon");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			this.Property(t => t.ContainerTypeC).HasColumnName("ContainerTypeC");
			this.Property(t => t.ContainerSizeI).HasColumnName("ContainerSizeI");
        }
    }
}
