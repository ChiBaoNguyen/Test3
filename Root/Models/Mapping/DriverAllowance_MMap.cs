using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class DriverAllowance_MMap: EntityTypeConfiguration<DriverAllowance_M>
	{
		public DriverAllowance_MMap()
		{
			// Primary Key
			this.HasKey(t => new { t.CustomerMainC, t.CustomerSubC, t.ApplyD, t.UnitPriceMethodI, t.DepartureC, t.DestinationC, t.ContainerSizeI, t.DriverAllowanceId, t.DisplayLineNo });

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
				.HasColumnName("date")
				.IsRequired();

			this.Property(t => t.UnitPriceMethodI)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.DepartureC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.DestinationC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(7);

			//this.Property(t => t.ContainerTypeC)
			//	.IsRequired()
			//	.IsUnicode(false)
			//	.HasMaxLength(5);
			this.Property(t => t.EmptyGoods)
				.HasPrecision(10, 0);

			this.Property(t => t.ContainerSizeI)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(1);

			this.Property(t => t.UnitPrice)
				.HasPrecision(10,0);

			this.Property(t => t.UnitPriceRate)
				.HasPrecision(3, 1);

			this.Property(t => t.ContainerSize)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(1);

			this.Property(t => t.Empty)
				.HasPrecision(10, 0);

			this.Property(t => t.ShortRoad)
				.HasPrecision(10, 0);

			this.Property(t => t.LongRoad)
				.HasPrecision(10, 0);

			this.Property(t => t.GradientRoad)
				.HasPrecision(10, 0);

			this.Property(t => t.DriverAllowanceId)
				.IsRequired();
			// Table & Column Mappings
			this.ToTable("DriverAllowance_M");
			this.Property(t => t.CustomerMainC).HasColumnName("CustomerMainC");
			this.Property(t => t.CustomerSubC).HasColumnName("CustomerSubC");
			this.Property(t => t.ApplyD).HasColumnName("ApplyD");
			this.Property(t => t.UnitPriceMethodI).HasColumnName("UnitPriceMethodI");
			this.Property(t => t.DepartureC).HasColumnName("DepartureC");
			this.Property(t => t.DestinationC).HasColumnName("DestinationC");
			//this.Property(t => t.ContainerTypeC).HasColumnName("ContainerTypeC");
			this.Property(t => t.ContainerSizeI).HasColumnName("ContainerSizeI");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			this.Property(t => t.UnitPriceRate).HasColumnName("UnitPriceRate");
			this.Property(t => t.DisplayLineNo).HasColumnName("DisplayLineNo");

			this.Property(t => t.ContainerSize).HasColumnName("ContainerSize");
			this.Property(t => t.Empty).HasColumnName("Empty");
			this.Property(t => t.ShortRoad).HasColumnName("ShortRoad");
			this.Property(t => t.LongRoad).HasColumnName("LongRoad");
			this.Property(t => t.GradientRoad).HasColumnName("GradientRoad");
			this.Property(t => t.DriverAllowanceId).HasColumnName("DriverAllowanceId");
			this.Property(t => t.EmptyGoods).HasColumnName("EmptyGoods");
		}
	}
}
