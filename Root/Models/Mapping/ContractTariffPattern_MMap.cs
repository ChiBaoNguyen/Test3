using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class ContractTariffPattern_MMap : EntityTypeConfiguration<ContractTariffPattern_M>
	{
		public ContractTariffPattern_MMap()
		{
			// Primary Key
			this.HasKey(t => new { t.CustomerMainC, t.CustomerSubC, t.ApplyD, t.DepartureC, t.DestinationC, t.ContainerSizeI, t.UnitPrice, t.CalculateByTon });

			// Properties
			this.Property(t => t.CustomerMainC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.CustomerSubC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.ApplyD)
				.IsRequired()
				.HasColumnType("date");

			this.Property(t => t.DepartureC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.DestinationC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.ContainerSizeI)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.UnitPrice)
				.IsRequired()
				.HasPrecision(10, 0);

			this.Property(t => t.CommodityC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.CommodityN)
				.HasMaxLength(50);

			this.Property(t => t.CalculateByTon)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			// Table & Column Mappings
			this.ToTable("ContractTariffPattern_M");
			this.Property(t => t.CustomerMainC).HasColumnName("CustomerMainC");
			this.Property(t => t.CustomerSubC).HasColumnName("CustomerSubC");
			this.Property(t => t.ApplyD).HasColumnName("ApplyD");
			this.Property(t => t.DepartureC).HasColumnName("DepartureC");
			this.Property(t => t.DestinationC).HasColumnName("DestinationC");
			this.Property(t => t.ContainerSizeI).HasColumnName("ContainerSizeI");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			this.Property(t => t.DisplayLineNo).HasColumnName("DisplayLineNo");
			this.Property(t => t.CommodityC).HasColumnName("CommodityC");
			this.Property(t => t.CommodityN).HasColumnName("CommodityN");
			this.Property(t => t.CalculateByTon).HasColumnName("CalculateByTon");
		}
	}
}
