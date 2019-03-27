using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class CustomerPricing_HMap : EntityTypeConfiguration<CustomerPricing_H>
	{
		public CustomerPricing_HMap()
		{
			// Primary Key
			this.HasKey(t => t.CustomerPricingId);

			// Properties
			this.Property(t => t.CustomerPricingId)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(64);

			this.Property(t => t.Location1C)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(7);

			this.Property(t => t.Location2C)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(7);

			this.Property(t => t.ContainerTypeC)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(5);

			this.Property(t => t.ContainerSizeI)
				.IsUnicode(false)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.CustomerMainC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.CustomerSubC)
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.GrossProfitRatio)
				.HasPrecision(4, 2);

			this.Property(t => t.TotalExpense)
				.HasPrecision(12, 0);

			this.Property(t => t.EstimatedPrice)
				.HasPrecision(12, 0);

			this.Property(t => t.EstimatedD)
				.IsRequired()
				.HasColumnType("date");

			// Table & Column Mappings
			this.ToTable("CustomerPricing_H");
			this.Property(t => t.CustomerPricingId).HasColumnName("CustomerPricingId");
			this.Property(t => t.Location1C).HasColumnName("Location1C");
			this.Property(t => t.Location2C).HasColumnName("Location2C");
			this.Property(t => t.ContainerTypeC).HasColumnName("ContainerTypeC");
			this.Property(t => t.ContainerSizeI).HasColumnName("ContainerSizeI");
			this.Property(t => t.CustomerMainC).HasColumnName("CustomerMainC");
			this.Property(t => t.CustomerSubC).HasColumnName("CustomerSubC");
			this.Property(t => t.GrossProfitRatio).HasColumnName("GrossProfitRatio");
			this.Property(t => t.TotalExpense).HasColumnName("TotalExpense");
			this.Property(t => t.EstimatedPrice).HasColumnName("EstimatedPrice");
			this.Property(t => t.EstimatedD).HasColumnName("EstimatedD");
		}
	}
}
