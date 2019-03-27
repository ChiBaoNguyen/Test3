using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class Allowance_DMap : EntityTypeConfiguration<Allowance_D>
	{
		public Allowance_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.OrderD, t.OrderNo, t.DetailNo, t.DispatchNo, t.AllowanceNo, t.AllowanceC });

			// Properties
			this.Property(t => t.OrderD)
				.HasColumnType("date")
				.IsRequired();

			this.Property(t => t.OrderNo)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(7);

			this.Property(t => t.DetailNo)
				.IsRequired();

			this.Property(t => t.DispatchNo)
				.IsRequired();

			this.Property(t => t.AllowanceNo)
				.IsRequired();

			this.Property(t => t.AllowanceC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.Unit)
				.HasMaxLength(50);

			this.Property(t => t.UnitPrice)
				.HasPrecision(10, 0);

			this.Property(t => t.Quantity)
				.HasPrecision(4, 0);

			this.Property(t => t.Amount)
				.HasPrecision(10, 0);

			this.Property(t => t.Description)
				.HasMaxLength(255);

			// Table & Column Mappings
			this.ToTable("Allowance_D");
			this.Property(t => t.OrderD).HasColumnName("OrderD");
			this.Property(t => t.OrderNo).HasColumnName("OrderNo");
			this.Property(t => t.DetailNo).HasColumnName("DetailNo");
			this.Property(t => t.DispatchNo).HasColumnName("DispatchNo");
			this.Property(t => t.AllowanceNo).HasColumnName("AllowanceNo");
			this.Property(t => t.AllowanceC).HasColumnName("AllowanceC");
			this.Property(t => t.Unit).HasColumnName("Unit");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			this.Property(t => t.Quantity).HasColumnName("Quantity");
			this.Property(t => t.Amount).HasColumnName("Amount");
			this.Property(t => t.Description).HasColumnName("Description");
		}
	}
}
