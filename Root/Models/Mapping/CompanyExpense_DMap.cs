using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class CompanyExpense_DMap : EntityTypeConfiguration<CompanyExpense_D>
	{
		public CompanyExpense_DMap()
		{
			// Primary Key
			this.HasKey(t => t.Id);

			this.Property(t => t.Id)
				.IsRequired()
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

			// Properties
			this.Property(t => t.InvoiceD)
				.IsRequired()
				.HasColumnType("date");

			this.Property(t => t.ExpenseC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.EmployeeC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.PaymentMethodI)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.SupplierMainC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.SupplierSubC)
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.Quantity)
				.HasPrecision(4, 0);

			this.Property(t => t.UnitPrice)
				.HasPrecision(10, 0);

			this.Property(t => t.Total)
				.HasPrecision(10, 0);

			this.Property(t => t.Tax)
				.HasPrecision(10, 0);

			this.Property(t => t.Description)
				.IsUnicode(true)
				.HasMaxLength(255);

			this.Property(t => t.EntryClerkC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.IsAllocated)
				.IsUnicode(false)
				.HasMaxLength(1);

			// Table & Column Mappings
			this.ToTable("CompanyExpense_D");
			this.Property(t => t.Id).HasColumnName("Id");
			this.Property(t => t.InvoiceD).HasColumnName("InvoiceD");
			this.Property(t => t.ExpenseC).HasColumnName("ExpenseC");
			this.Property(t => t.EmployeeC).HasColumnName("EmployeeC");
			this.Property(t => t.PaymentMethodI).HasColumnName("PaymentMethodI");
			this.Property(t => t.SupplierMainC).HasColumnName("SupplierMainC");
			this.Property(t => t.SupplierSubC).HasColumnName("SupplierSubC");
			this.Property(t => t.Quantity).HasColumnName("Quantity");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			this.Property(t => t.Total).HasColumnName("Total");
			this.Property(t => t.Tax).HasColumnName("Tax");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.EntryClerkC).HasColumnName("EntryClerkC");
			this.Property(t => t.IsAllocated).HasColumnName("IsAllocated");
		}
	}
}
