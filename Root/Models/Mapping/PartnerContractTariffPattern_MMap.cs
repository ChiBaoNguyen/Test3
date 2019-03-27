using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class PartnerContractTariffPattern_MMap : EntityTypeConfiguration<PartnerContractTariffPattern_M>
	{
        public PartnerContractTariffPattern_MMap()
		{
			// Primary Key
            this.HasKey(t => new { t.PartnerMainC, t.PartnerSubC, t.ApplyD, t.DepartureC, t.DestinationC, t.ContainerSizeI, t.ContainerTypeC });

			// Properties
            this.Property(t => t.PartnerMainC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

            this.Property(t => t.PartnerSubC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.ApplyD)
				.IsRequired()
				.HasColumnType("date");

			this.Property(t => t.DepartureC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.DestinationC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.ContainerTypeC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.ContainerSizeI)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.UnitPrice)
				.HasPrecision(10, 0);

			// Table & Column Mappings
            this.ToTable("PartnerContractTariffPattern_M");
            this.Property(t => t.PartnerMainC).HasColumnName("PartnerMainC");
            this.Property(t => t.PartnerSubC).HasColumnName("PartnerSubC");
			this.Property(t => t.ApplyD).HasColumnName("ApplyD");
			this.Property(t => t.DepartureC).HasColumnName("DepartureC");
			this.Property(t => t.DestinationC).HasColumnName("DestinationC");
			this.Property(t => t.ContainerTypeC).HasColumnName("ContainerTypeC");
			this.Property(t => t.ContainerSizeI).HasColumnName("ContainerSizeI");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
		}
	}
}
