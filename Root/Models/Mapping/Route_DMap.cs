using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class Route_DMap : EntityTypeConfiguration<Route_D>
	{
		public Route_DMap()
		{
			// Primary Key
			this.HasKey(t => t.RouteExpenseId);

			// Properties
			this.Property(t => t.RouteExpenseId)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(64);

			this.Property(t => t.RouteId)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(64);

			this.Property(t => t.CategoryI)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(1);

			this.Property(t => t.ExpenseC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.ExpenseN)
				.IsRequired()
				.IsUnicode(true)
				.HasMaxLength(50);

			this.Property(t => t.UsedExpenseD)
				.HasColumnType("date");

			this.Property(t => t.Unit)
				.HasMaxLength(50);

			this.Property(t => t.UnitPrice)
				.HasPrecision(10, 0);

			this.Property(t => t.Quantity)
				.HasPrecision(4, 0);

			this.Property(t => t.Amount)
				.HasPrecision(10, 0);

			// Table & Column Mappings
			this.ToTable("Route_D");
			this.Property(t => t.RouteExpenseId).HasColumnName("RouteExpenseId");
			this.Property(t => t.RouteId).HasColumnName("RouteId");
			this.Property(t => t.DisplayLineNo).HasColumnName("DisplayLineNo");
			this.Property(t => t.CategoryI).HasColumnName("CategoryI");
			this.Property(t => t.ExpenseC).HasColumnName("ExpenseC");
			this.Property(t => t.ExpenseN).HasColumnName("ExpenseN");
			this.Property(t => t.UsedExpenseD).HasColumnName("UsedExpenseD");
			this.Property(t => t.Unit).HasColumnName("Unit");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			this.Property(t => t.Quantity).HasColumnName("Quantity");
			this.Property(t => t.Amount).HasColumnName("Amount");
			this.Property(t => t.IsUsed).HasColumnName("IsUsed");
		}
	}
}
