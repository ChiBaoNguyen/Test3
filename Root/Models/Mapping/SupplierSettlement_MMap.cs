using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class SupplierSettlement_MMap : EntityTypeConfiguration<SupplierSettlement_M>
	{
        public SupplierSettlement_MMap()
		{
			// Primary Key
            this.HasKey(t => new { t.SupplierMainC, t.SupplierSubC, t.ApplyD });

			// Properties
            this.Property(t => t.SupplierMainC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

            this.Property(t => t.SupplierSubC)
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

			// Table & Column Mappings
            this.ToTable("SupplierSettlement_M");
            this.Property(t => t.SupplierMainC).HasColumnName("SupplierMainC");
            this.Property(t => t.SupplierSubC).HasColumnName("SupplierSubC");
			this.Property(t => t.ApplyD).HasColumnName("ApplyD");
			this.Property(t => t.SettlementD).HasColumnName("SettlementD");
			this.Property(t => t.TaxMethodI).HasColumnName("TaxMethodI");
			this.Property(t => t.TaxRate).HasColumnName("TaxRate");
			this.Property(t => t.TaxRoundingI).HasColumnName("TaxRoundingI");
			this.Property(t => t.RevenueRoundingI).HasColumnName("RevenueRoundingI");
		}
	}
}
