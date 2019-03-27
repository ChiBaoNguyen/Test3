using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class FixedExpense_DMap : EntityTypeConfiguration<FixedExpense_D>
	{
		public FixedExpense_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.DepC, t.Year, t.ExpenseC, t.TruckC });

			// Properties
			this.Property(t => t.DepC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.Year)
				.IsRequired();

			this.Property(t => t.ExpenseC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.TruckC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.EntryClerkC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.Total)
				.HasPrecision(10, 0);

			this.Property(t => t.Month1)
				.HasPrecision(10, 0);

			this.Property(t => t.Month2)
				.HasPrecision(10, 0);

			this.Property(t => t.Month3)
				.HasPrecision(10, 0);

			this.Property(t => t.Month4)
				.HasPrecision(10, 0);

			this.Property(t => t.Month5)
				.HasPrecision(10, 0);

			this.Property(t => t.Month6)
				.HasPrecision(10, 0);

			this.Property(t => t.Month7)
				.HasPrecision(10, 0);

			this.Property(t => t.Month8)
				.HasPrecision(10, 0);

			this.Property(t => t.Month9)
				.HasPrecision(10, 0);

			this.Property(t => t.Month10)
				.HasPrecision(10, 0);

			this.Property(t => t.Month11)
				.HasPrecision(10, 0);

			this.Property(t => t.Month12)
				.HasPrecision(10, 0);

			// Table & Column Mappings
			this.ToTable("FixedExpense_D");
			this.Property(t => t.DepC).HasColumnName("DepC");
			this.Property(t => t.Year).HasColumnName("Year");
			this.Property(t => t.ExpenseC).HasColumnName("ExpenseC");
			this.Property(t => t.TruckC).HasColumnName("TruckC");
			this.Property(t => t.EntryClerkC).HasColumnName("EntryClerkC");
			this.Property(t => t.ExpenseC).HasColumnName("ExpenseC");
			this.Property(t => t.Total).HasColumnName("Total");
			this.Property(t => t.Month1).HasColumnName("Month1");
			this.Property(t => t.Month2).HasColumnName("Month2");
			this.Property(t => t.Month3).HasColumnName("Month3");
			this.Property(t => t.Month4).HasColumnName("Month4");
			this.Property(t => t.Month5).HasColumnName("Month5");
			this.Property(t => t.Month6).HasColumnName("Month6");
			this.Property(t => t.Month7).HasColumnName("Month7");
			this.Property(t => t.Month8).HasColumnName("Month8");
			this.Property(t => t.Month9).HasColumnName("Month9");
			this.Property(t => t.Month10).HasColumnName("Month10");
			this.Property(t => t.Month11).HasColumnName("Month11");
			this.Property(t => t.Month12).HasColumnName("Month12");
		}
	}
}