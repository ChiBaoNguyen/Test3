using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class CustomerSettlement_MMap: EntityTypeConfiguration<CustomerSettlement_M>
	{
		public CustomerSettlement_MMap()
		{
			// Primary Key
			this.HasKey(t => new { t.CustomerMainC, t.CustomerSubC, t.ApplyD});

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

			this.Property(t => t.TaxMethodI)
				.IsFixedLength()
				.IsUnicode(false)
				.HasMaxLength(1);

			this.Property(t => t.TaxRate)
				.HasPrecision(3,1);

			this.Property(t => t.TaxRoundingI)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.RevenueRoundingI)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.FormOfSettlement)
				.IsFixedLength()
				.IsUnicode(false)
				.HasMaxLength(1);

			// Table & Column Mappings
			this.ToTable("CustomerSettlement_M");
			this.Property(t => t.CustomerMainC).HasColumnName("CustomerMainC");
			this.Property(t => t.CustomerSubC).HasColumnName("CustomerSubC");
			this.Property(t => t.ApplyD).HasColumnName("ApplyD");
			this.Property(t => t.SettlementD).HasColumnName("SettlementD");
			this.Property(t => t.TaxMethodI).HasColumnName("TaxMethodI");
			this.Property(t => t.TaxRate).HasColumnName("TaxRate");
			this.Property(t => t.TaxRoundingI).HasColumnName("TaxRoundingI");
			this.Property(t => t.RevenueRoundingI).HasColumnName("RevenueRoundingI");
			this.Property(t => t.FormOfSettlement).HasColumnName("FormOfSettlement");
		}
	}
}
