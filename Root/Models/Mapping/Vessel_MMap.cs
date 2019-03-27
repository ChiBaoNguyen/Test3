using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class Vessel_MMap : EntityTypeConfiguration<Vessel_M>
	{
		public Vessel_MMap()
		{
			// Primary Key
			this.HasKey(t => t.VesselC);

			// Properties
			this.Property(t => t.VesselC)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.VesselN)
				.IsRequired()
				.HasMaxLength(50);

			this.Property(t => t.ShippingCompanyC)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.IsActive)
				.HasMaxLength(1)
				.IsFixedLength()
				.IsUnicode(false);

			// Table & Column Mappings
			this.ToTable("Vessel_M");
			this.Property(t => t.VesselC).HasColumnName("VesselC");
			this.Property(t => t.VesselN).HasColumnName("VesselN");
			this.Property(t => t.ShippingCompanyC).HasColumnName("ShippingCompanyC");
			this.Property(t => t.IsActive).HasColumnName("IsActive");
		}
	}
}
