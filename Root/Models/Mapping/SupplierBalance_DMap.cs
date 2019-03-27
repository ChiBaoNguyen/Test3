using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class SupplierBalance_DMap : EntityTypeConfiguration<SupplierBalance_D>
	{
		public SupplierBalance_DMap()
		{
			// Primary Key
			this.HasKey(t => new {t.SupplierMainC, t.SupplierSubC, t.SupplierBalanceD});

			this.Property(t => t.SupplierMainC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.SupplierSubC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.SupplierBalanceD)
				.HasColumnType("date");

			this.Property(t => t.TotalAmount)
				.HasPrecision(12, 0);

			this.Property(t => t.TaxAmount)
				.HasPrecision(12, 0);

			this.Property(t => t.PaymentAmount)
				.HasPrecision(12, 0);


			// Table & Column Mappings
			this.ToTable("SupplierBalance_D");
		}
	}
}
