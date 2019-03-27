using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class PartnerPayment_DMap : EntityTypeConfiguration<PartnerPayment_D>
	{
		public PartnerPayment_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.PartnerMainC, t.PartnerSubC, t.PaymentId, t.PartnerPaymentD });

			// Properties
			this.Property(t => t.PartnerMainC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.PartnerSubC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.PaymentId)
				.IsRequired()
				.HasMaxLength(50);

			this.Property(t => t.PartnerPaymentD)
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

			this.ToTable("PartnerPayment_D");
			this.Property(t => t.PartnerMainC).HasColumnName("PartnerMainC");
			this.Property(t => t.PartnerSubC).HasColumnName("PartnerSubC");
			this.Property(t => t.PaymentId).HasColumnName("PaymentId");
			this.Property(t => t.PartnerPaymentD).HasColumnName("PartnerPaymentD");
			this.Property(t => t.Amount).HasColumnName("Amount");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.EntryClerkC).HasColumnName("EntryCleckC");
			this.Property(t => t.PaymentMethodI).HasColumnName("PaymentMethodI");
		}
	}
}
