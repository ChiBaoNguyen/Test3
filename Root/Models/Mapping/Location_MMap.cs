using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
    public class Location_MMap : EntityTypeConfiguration<Location_M>
    {
        public Location_MMap()
        {
            // Primary Key
            this.HasKey(t => t.LocationC);

            // Properties
            this.Property(t => t.LocationC)
                .IsRequired()
				.IsUnicode(false)
                .HasMaxLength(7);

			this.Property(t => t.LocationN)
				.HasMaxLength(200);

            this.Property(t => t.Address)
                .HasMaxLength(255);

			this.Property(t => t.LocationI)
				.IsFixedLength()
				.IsUnicode(false)
				.HasMaxLength(1);

			this.Property(t => t.Description)
				.HasMaxLength(255);
			
			this.Property(t => t.AreaC)
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.IsActive)
				.IsFixedLength()
				.IsUnicode(false)
				.HasMaxLength(1);

			this.Property(t => t.SupplierMainC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.SupplierSubC)
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.SupplierN)
				.HasMaxLength(200);
            // Table & Column Mappings
            this.ToTable("Location_M");
            this.Property(t => t.LocationC).HasColumnName("LocationC");
            this.Property(t => t.LocationN).HasColumnName("LocationN");
			this.Property(t => t.Address).HasColumnName("Address");
			this.Property(t => t.LocationI).HasColumnName("LocationI");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.AreaC).HasColumnName("AreaC");
			this.Property(t => t.IsActive).HasColumnName("IsActive");
			this.Property(t => t.SupplierMainC).HasColumnName("SupplierMainC");
			this.Property(t => t.SupplierSubC).HasColumnName("SupplierSubC");
			this.Property(t => t.SupplierN).HasColumnName("SupplierN");
        }
    }
}
