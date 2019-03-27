using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class Inspection_DMap : EntityTypeConfiguration<Inspection_D>
	{
		public Inspection_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.ObjectI, t.Code, t.InspectionC, t.InspectionD });

			// Properties
			this.Property(t => t.ObjectI)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.Code)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.InspectionC)
				.IsRequired();

			this.Property(t => t.InspectionD)
				.IsRequired()
				.HasColumnType("date");

			this.Property(t => t.InspectionPlanD)
				.HasColumnType("date");

			this.Property(t => t.Odometer)
				.HasPrecision(7, 1);

			this.Property(t => t.Description)
				.IsUnicode(true)
				.HasMaxLength(255);

			this.Property(t => t.ExpenseC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.EntryClerkC)
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

			this.Property(t => t.Total)
				.HasPrecision(10, 0);

			this.Property(t => t.TaxAmount)
				.HasPrecision(10, 0);

			this.Property(t => t.IsAllocated)
				.IsUnicode(false)
				.HasMaxLength(1);

			// Table & Column Mappings
			this.ToTable("Inspection_D");
			this.Property(t => t.ObjectI).HasColumnName("ObjectI");
			this.Property(t => t.Code).HasColumnName("Code");
			this.Property(t => t.InspectionC).HasColumnName("InspectionC");
			this.Property(t => t.InspectionD).HasColumnName("InspectionD");
			this.Property(t => t.InspectionPlanD).HasColumnName("InspectionPlanD");
			this.Property(t => t.Odometer).HasColumnName("Odometer");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.ExpenseC).HasColumnName("ExpenseC");
			this.Property(t => t.EntryClerkC).HasColumnName("EntryClerkC");
			this.Property(t => t.PaymentMethodI).HasColumnName("PaymentMethodI");
			this.Property(t => t.SupplierMainC).HasColumnName("SupplierMainC");
			this.Property(t => t.SupplierSubC).HasColumnName("SupplierSubC");
			this.Property(t => t.Total).HasColumnName("Total");
			this.Property(t => t.TaxAmount).HasColumnName("TaxAmount");
			this.Property(t => t.IsAllocated).HasColumnName("IsAllocated");
		}
	}
}