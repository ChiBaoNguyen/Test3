using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class FuelConsumption_DMap : EntityTypeConfiguration<FuelConsumption_D>
	{
		public FuelConsumption_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.OrderD, t.OrderNo, t.DetailNo, t.DispatchNo });

			// Properties
			this.Property(t => t.OrderD)
				.IsRequired()
				.HasColumnType("date");

			this.Property(t => t.OrderNo)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(7);

			this.Property(t => t.DetailNo)
				.IsRequired();

			this.Property(t => t.DispatchNo)
				.IsRequired();

			this.Property(t => t.IsEmpty)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.IsHeavy)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.IsSingle)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.Distance)
				.HasPrecision(5, 0);

			this.Property(t => t.FuelConsumption)
				.HasPrecision(6, 1);

			this.Property(t => t.UnitPrice)
				.HasPrecision(5, 0);

			this.Property(t => t.Amount)
				.HasPrecision(10, 0);

			// Table & Column Mappings
			this.ToTable("FuelConsumption_D");
			this.Property(t => t.OrderD).HasColumnName("OrderD");
			this.Property(t => t.OrderNo).HasColumnName("OrderNo");
			this.Property(t => t.DetailNo).HasColumnName("DetailNo");
			this.Property(t => t.DispatchNo).HasColumnName("DispatchNo");
			this.Property(t => t.IsEmpty).HasColumnName("IsEmpty");
			this.Property(t => t.IsHeavy).HasColumnName("IsHeavy");
			this.Property(t => t.IsSingle).HasColumnName("IsSingle");
			this.Property(t => t.Distance).HasColumnName("Distance");
			this.Property(t => t.FuelConsumption).HasColumnName("FuelConsumption");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			this.Property(t => t.Amount).HasColumnName("Amount");
		}
	}
}
