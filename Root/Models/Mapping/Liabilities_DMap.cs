using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class Liabilities_DMap : EntityTypeConfiguration<Liabilities_D>
	{
		public Liabilities_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.DriverC, t.LiabilitiesD, t.LiabilitiesI, t.LiabilitiesNo });

			// Properties
			this.Property(t => t.LiabilitiesD)
				.HasColumnType("date")
				.IsRequired();

			this.Property(t => t.DriverC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.LiabilitiesI)
				.IsRequired()
				.HasMaxLength(1);

			this.Property(t => t.LiabilitiesNo)
				.IsRequired();

			this.Property(t => t.ReceiptNo)
				.HasMaxLength(50);

			this.Property(t => t.Amount)
				.HasPrecision(10, 0);

			this.Property(t => t.Description)
				.HasMaxLength(255);

			// Table & Column Mappings
			this.ToTable("Liabilities_D");
			this.Property(t => t.DriverC).HasColumnName("DriverC");
			this.Property(t => t.LiabilitiesD).HasColumnName("LiabilitiesD");
			this.Property(t => t.LiabilitiesI).HasColumnName("LiabilitiesI");
			this.Property(t => t.Amount).HasColumnName("Amount");
			this.Property(t => t.Description).HasColumnName("Description");
		}
	}
}
