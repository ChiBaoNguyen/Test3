using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class Commodity_MMap : EntityTypeConfiguration<Commodity_M>
	{
		public Commodity_MMap()
		{
			// Primary Key
			this.HasKey(t => t.CommodityC);

			// Properties
			this.Property(t => t.CommodityC)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.CommodityN)
				.IsRequired()
				.HasMaxLength(50);

			this.Property(t => t.PermittedWeight)
				.HasPrecision(7, 1);

			this.Property(t => t.Description)
				.HasMaxLength(255);

			this.Property(t => t.IsActive)
				.HasMaxLength(1)
				.IsFixedLength()
				.IsUnicode(false);

			// Table & Column Mappings
			this.ToTable("Commodity_M");
			this.Property(t => t.CommodityC).HasColumnName("CommodityC");
			this.Property(t => t.CommodityN).HasColumnName("CommodityN");
			this.Property(t => t.PermittedWeight).HasColumnName("PermittedWeight");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.IsActive).HasColumnName("IsActive");
		}
	}
}
