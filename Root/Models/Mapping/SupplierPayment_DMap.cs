using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class SupplierPayment_DMap : EntityTypeConfiguration<SupplierPayment_D>
	{
		public SupplierPayment_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.SupplierMainC, t.SupplierSubC, t.PaymentId, t.SupplierPaymentD });

			// Properties
			this.Property(t => t.SupplierMainC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.SupplierSubC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.PaymentId)
				.IsRequired()
				.HasMaxLength(50);

			this.Property(t => t.SupplierPaymentD)
				.IsRequired()
				.HasColumnType("date");

			this.Property(t => t.Amount)
				.HasPrecision(10, 0);

			this.Property(t => t.Description)
				.HasMaxLength(255);

			this.Property(t => t.EntryClerkC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.PaymentMethodI)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.ToTable("SupplierPayment_D");
			this.Property(t => t.SupplierMainC).HasColumnName("SupplierMainC");
			this.Property(t => t.SupplierSubC).HasColumnName("SupplierSubC");
			this.Property(t => t.PaymentId).HasColumnName("PaymentId");
			this.Property(t => t.SupplierPaymentD).HasColumnName("SupplierPaymentD");
			this.Property(t => t.Amount).HasColumnName("Amount");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.EntryClerkC).HasColumnName("EntryCleckC");
			this.Property(t => t.PaymentMethodI).HasColumnName("PaymentMethodI");
		}
	}
}
