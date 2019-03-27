using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	internal class Trailer_MMap : EntityTypeConfiguration<Trailer_M>
	{
		public Trailer_MMap()
		{
			// Primary Key
			this.HasKey(t => t.TrailerC);

			this.Property(t => t.TrailerC)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.TrailerNo)
				.IsRequired()
				.HasMaxLength(20)
				.IsUnicode(false);

			this.Property(t => t.RegisteredD)
				.HasColumnType("date");

			this.Property(t => t.VIN)
				.HasMaxLength(50)
				.IsUnicode(false);

			this.Property(t => t.DriverC)
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.ModelC)
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.UsingDriverC)
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.IsUsing)
				.HasMaxLength(1)
				.IsFixedLength()
				.IsUnicode(false);

			this.Property(t => t.IsActive)
				.HasMaxLength(1)
				.IsFixedLength()
				.IsUnicode(false);

			this.Property(t => t.GrossWeight)
				.HasPrecision(6, 1);

			this.Property(t => t.Situation)
				  .HasMaxLength(255);

			this.Property(t => t.FromDate)
				.HasColumnType("date");

			this.Property(t => t.ToDate)
				.HasColumnType("date");
			// Table & Column Mappings
			this.ToTable("Trailer_M");
			this.Property(t => t.TrailerC).HasColumnName("TrailerC");
			this.Property(t => t.TrailerNo).HasColumnName("TrailerNo");
			this.Property(t => t.RegisteredD).HasColumnName("RegisteredD");
			this.Property(t => t.VIN).HasColumnName("VIN");
			this.Property(t => t.DriverC).HasColumnName("DriverC");
			this.Property(t => t.ModelC).HasColumnName("ModelC");
			this.Property(t => t.UsingDriverC).HasColumnName("UsingDriverC");
			this.Property(t => t.IsUsing).HasColumnName("IsUsing");
			this.Property(t => t.IsActive).HasColumnName("IsActive");
			this.Property(t => t.GrossWeight).HasColumnName("GrossWeight");
			this.Property(t => t.Situation).HasColumnName("Situation");
			this.Property(t => t.FromDate).HasColumnName("FromDate");
			this.Property(t => t.ToDate).HasColumnName("ToDate");
		}
	}
}
