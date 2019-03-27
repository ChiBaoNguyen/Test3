using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class Expense_DMap : EntityTypeConfiguration<Expense_D>
	{
		public Expense_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.OrderD, t.OrderNo, t.DetailNo, t.DispatchNo, t.ExpenseNo, t.ExpenseC });

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

			this.Property(t => t.ExpenseNo)
				.IsRequired();

			this.Property(t => t.ExpenseC)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(5);

			this.Property(t => t.PaymentMethodI)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.InvoiceD)
				.HasColumnName("date");

			this.Property(t => t.SupplierMainC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.SupplierSubC)
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.Unit)
				.HasMaxLength(50);

			this.Property(t => t.UnitPrice)
				.HasPrecision(10, 0);

			this.Property(t => t.Quantity)
				.HasPrecision(4, 0);

			this.Property(t => t.Amount)
				.HasPrecision(10, 0);

			this.Property(t => t.TaxAmount)
				.HasPrecision(10, 0);

			this.Property(t => t.TaxRate)
				.HasPrecision(3, 1);

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

			this.Property(t => t.EntryClerkC)
				.HasMaxLength(30);

			this.Property(t => t.Description)
				.HasMaxLength(255);

			// Table & Column Mappings
			this.ToTable("Expense_D");
			this.Property(t => t.OrderD).HasColumnName("OrderD");
			this.Property(t => t.OrderNo).HasColumnName("OrderNo");
			this.Property(t => t.DetailNo).HasColumnName("DetailNo");
			this.Property(t => t.DispatchNo).HasColumnName("DispatchNo");
			this.Property(t => t.ExpenseNo).HasColumnName("ExpenseNo");
			this.Property(t => t.ExpenseC).HasColumnName("ExpenseC");
			this.Property(t => t.PaymentMethodI).HasColumnName("PaymentMethodI");
			this.Property(t => t.InvoiceD).HasColumnName("InvoiceD");
			this.Property(t => t.SupplierMainC).HasColumnName("SupplierMainC");
			this.Property(t => t.SupplierSubC).HasColumnName("SupplierSubC");
			this.Property(t => t.Unit).HasColumnName("Unit");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			this.Property(t => t.Quantity).HasColumnName("Quantity");
			this.Property(t => t.Amount).HasColumnName("Amount");
			this.Property(t => t.TaxAmount).HasColumnName("TaxAmount");
			this.Property(t => t.TaxRate).HasColumnName("TaxRate");
			this.Property(t => t.IsIncluded).HasColumnName("IsIncluded");
			this.Property(t => t.IsRequested).HasColumnName("IsRequested");
			this.Property(t => t.IsPayable).HasColumnName("IsPayable");
			this.Property(t => t.EntryClerkC).HasColumnName("EntryClerkC");
			this.Property(t => t.Description).HasColumnName("Description");
		}
	}
}
