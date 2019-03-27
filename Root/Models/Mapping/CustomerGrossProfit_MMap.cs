using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class CustomerGrossProfit_MMap : EntityTypeConfiguration<CustomerGrossProfit_M>
	{
		public CustomerGrossProfit_MMap()
		{
			// Primary Key
			this.HasKey(t => new { t.CustomerMainC, t.CustomerSubC, t.ApplyD });

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


			this.Property(t => t.GrossProfitRatio)
				.HasPrecision(4, 2);

			this.Property(t => t.Description)
				.HasMaxLength(255);

			// Table & Column Mappings
			this.ToTable("CustomerGrossProfit_M");
			this.Property(t => t.CustomerMainC).HasColumnName("CustomerMainC");
			this.Property(t => t.CustomerSubC).HasColumnName("CustomerSubC");
			this.Property(t => t.ApplyD).HasColumnName("ApplyD");
			this.Property(t => t.GrossProfitRatio).HasColumnName("GrossProfitRatio");
			this.Property(t => t.Description).HasColumnName("Description");
		}
	}
}
