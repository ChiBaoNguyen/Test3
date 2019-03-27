using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class Route_HMap : EntityTypeConfiguration<Route_H>
	{
		public Route_HMap()
		{
			// Primary Key
			this.HasKey(t => t.RouteId);

			// Properties
			this.Property(t => t.RouteId)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(64);

			this.Property(t => t.Location1C)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(7);

			this.Property(t => t.Location2C)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(7);

			this.Property(t => t.ContainerTypeC)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(5);

			this.Property(t => t.ContainerSizeI)
				.IsUnicode(false)
				.IsRequired()
				.IsFixedLength()
				.HasMaxLength(1);

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

			this.Property(t => t.RouteN)
				.HasMaxLength(50);

			this.Property(t => t.TotalExpense)
				.HasPrecision(12, 0);

			// Table & Column Mappings
			this.ToTable("Route_H");
			this.Property(t => t.RouteId).HasColumnName("RouteId");
			this.Property(t => t.Location1C).HasColumnName("Location1C");
			this.Property(t => t.Location2C).HasColumnName("Location2C");
			this.Property(t => t.ContainerTypeC).HasColumnName("ContainerTypeC");
			this.Property(t => t.ContainerSizeI).HasColumnName("ContainerSizeI");
			this.Property(t => t.IsEmpty).HasColumnName("IsEmpty");
			this.Property(t => t.IsHeavy).HasColumnName("IsHeavy");
			this.Property(t => t.IsSingle).HasColumnName("IsSingle");
			this.Property(t => t.RouteN).HasColumnName("RouteN");
			this.Property(t => t.TotalExpense).HasColumnName("TotalExpense");
		}
	}
}
