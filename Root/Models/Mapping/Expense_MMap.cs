using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class Expense_MMap : EntityTypeConfiguration<Expense_M>
	{
		public Expense_MMap()
		{
			// Primary Key
			this.HasKey(t =>  t.ExpenseC);

			// Properties
			this.Property(t => t.ExpenseC)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(5);

			this.Property(t => t.ExpenseN)
				.IsRequired()
				.HasMaxLength(50);

			this.Property(t => t.CategoryI)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.CategoryC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.ExpenseI)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.Unit)
				.HasMaxLength(50);

			this.Property(t => t.UnitPrice)
				.HasPrecision(10, 0);

			this.Property(t => t.TaxRate)
				.HasPrecision(3, 1);

			this.Property(t => t.PaymentMethodI)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.IsIncluded)
				.IsFixedLength()
				.IsUnicode(false)
				.HasMaxLength(1);

			this.Property(t => t.IsRequested)
				.IsFixedLength()
				.IsUnicode(false)
				.HasMaxLength(1);

			this.Property(t => t.IsPayable)
				.IsFixedLength()
				.IsUnicode(false)
				.HasMaxLength(1);

			this.Property(t => t.Description)
				.HasMaxLength(255);

			this.Property(t => t.TaxRoundingI)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.ViewReport)
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.ColumnName)
				.HasMaxLength(255);

			// Table & Column Mappings
			this.ToTable("Expense_M");
			this.Property(t => t.ExpenseC).HasColumnName("ExpenseC");
			this.Property(t => t.ExpenseN).HasColumnName("ExpenseN");
			this.Property(t => t.CategoryI).HasColumnName("CategoryI");
			this.Property(t => t.CategoryC).HasColumnName("CategoryC");
			this.Property(t => t.ExpenseI).HasColumnName("ExpenseI");
			this.Property(t => t.Unit).HasColumnName("Unit");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			this.Property(t => t.TaxRate).HasColumnName("TaxRate");
			this.Property(t => t.PaymentMethodI).HasColumnName("PaymentMethodI");
			this.Property(t => t.IsIncluded).HasColumnName("IsIncluded");
			this.Property(t => t.IsRequested).HasColumnName("IsRequested");
			this.Property(t => t.IsPayable).HasColumnName("IsPayable");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.TaxRoundingI).HasColumnName("TaxRoundingI");
			this.Property(t => t.ViewReport).HasColumnName("ViewReport");
			this.Property(t => t.ColumnName).HasColumnName("ColumnName");
		}
	}
}
