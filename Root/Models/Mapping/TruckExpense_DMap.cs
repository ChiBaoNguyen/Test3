using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class TruckExpense_DMap : EntityTypeConfiguration<TruckExpense_D>
	{
		public TruckExpense_DMap()
		{
			// Primary Key
			this.HasKey(t => t.Id);

			this.Property(t => t.Id)
				.IsRequired()
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

			// Properties
			this.Property(t => t.Code)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.InvoiceD)
				.IsRequired()
				.HasColumnType("date");

			this.Property(t => t.TransportD)
				.IsRequired()
				.HasColumnType("date");

			this.Property(t => t.ExpenseC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.EntryClerkC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.DriverC)
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
				.HasPrecision(5, 1);

			this.Property(t => t.UnitPrice)
				.HasPrecision(10, 0);

			this.Property(t => t.Total)
				.HasPrecision(10, 0);

			this.Property(t => t.Tax)
				.HasPrecision(10, 0);

			this.Property(t => t.Description)
				.IsUnicode(true)
				.HasMaxLength(255);

            this.Property(t => t.ObjectI)
                .IsUnicode(false)
                .IsFixedLength()
                .HasMaxLength(1);

			this.Property(t => t.IsAllocated)
				.IsUnicode(false)
				.HasMaxLength(1);

			// Table & Column Mappings
			this.ToTable("TruckExpense_D");
			this.Property(t => t.Id).HasColumnName("Id");
			this.Property(t => t.Code).HasColumnName("Code");
			this.Property(t => t.InvoiceD).HasColumnName("InvoiceD");
			this.Property(t => t.TransportD).HasColumnName("TransportD");
			this.Property(t => t.EntryClerkC).HasColumnName("EntryClerkC");
			this.Property(t => t.ExpenseC).HasColumnName("ExpenseC");
			this.Property(t => t.DriverC).HasColumnName("DriverC");
			this.Property(t => t.PaymentMethodI).HasColumnName("PaymentMethodI");
			this.Property(t => t.SupplierMainC).HasColumnName("SupplierMainC");
			this.Property(t => t.SupplierSubC).HasColumnName("SupplierSubC");
			this.Property(t => t.Quantity).HasColumnName("Quantity");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			this.Property(t => t.Total).HasColumnName("Total");
			this.Property(t => t.Tax).HasColumnName("Tax");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.ObjectI).HasColumnName("ObjectI");
			this.Property(t => t.IsAllocated).HasColumnName("IsAllocated");
		}
	}
}