using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class FuelConsumption_MMap : EntityTypeConfiguration<FuelConsumption_M>
	{
		public FuelConsumption_MMap()
		{
			// Primary Key
			this.HasKey(t => new { t.FuelConsumptionC });

			// Properties
			this.Property(t => t.FuelConsumptionC)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.FuelConsumptionId)
				.IsRequired()
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

			this.Property(t => t.ContainerSizeI)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			//this.Property(t => t.IsEmpty)
			//	.IsUnicode(false)
			//	.IsFixedLength()
			//	.HasMaxLength(1);

			//this.Property(t => t.IsHeavy)
			//	.IsUnicode(false)
			//	.IsFixedLength()
			//	.HasMaxLength(1);

			//this.Property(t => t.IsSingle)
			//	.IsUnicode(false)
			//	.IsFixedLength()
			//	.HasMaxLength(1);

			this.Property(t => t.ShortRoad)
				.HasPrecision(4, 2);

			this.Property(t => t.LongRoad)
				.HasPrecision(4, 2);

			this.Property(t => t.Gradient)
				.HasPrecision(4, 2);

			this.Property(t => t.Empty)
				.HasPrecision(4, 2);

			this.Property(t => t.ModelC)
				.HasMaxLength(5)
				.IsUnicode(false);
			//this.Property(t => t.UnitPrice)
			//	.HasPrecision(5, 0);

			//this.Property(t => t.Amount)
			//	.HasPrecision(10, 0);

			// Table & Column Mappings
			this.ToTable("FuelConsumption_M");
			this.Property(t => t.FuelConsumptionC).HasColumnName("FuelConsumptionC");
			this.Property(t => t.FuelConsumptionId).HasColumnName("FuelConsumptionId");
			this.Property(t => t.ContainerSizeI).HasColumnName("ContainerSizeI");
			//this.Property(t => t.IsEmpty).HasColumnName("IsEmpty");
			//this.Property(t => t.IsHeavy).HasColumnName("IsHeavy");
			//this.Property(t => t.IsSingle).HasColumnName("IsSingle");
			//this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			//this.Property(t => t.Amount).HasColumnName("Amount");
			this.Property(t => t.ShortRoad).HasColumnName("ShortRoad");
			this.Property(t => t.LongRoad).HasColumnName("LongRoad");
			this.Property(t => t.Gradient).HasColumnName("Gradient");
			this.Property(t => t.Empty).HasColumnName("Empty");
			this.Property(t => t.ModelC).HasColumnName("ModelC");
		}
	}
}
