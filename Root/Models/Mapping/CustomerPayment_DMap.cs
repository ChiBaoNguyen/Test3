using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class CustomerPayment_DMap : EntityTypeConfiguration<CustomerPayment_D>
	{
		public CustomerPayment_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.CustomerMainC, t.CustomerSubC, t.PaymentId, t.CustomerPaymentD });

			// Properties
			this.Property(t => t.CustomerMainC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.CustomerSubC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.PaymentId)
				.IsRequired()
				.HasMaxLength(50);

			this.Property(t => t.CustomerPaymentD)
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

			this.ToTable("CustomerPayment_D");
			this.Property(t => t.CustomerMainC).HasColumnName("CustomerMainC");
			this.Property(t => t.CustomerSubC).HasColumnName("CustomerSubC");
			this.Property(t => t.PaymentId).HasColumnName("PaymentId");
			this.Property(t => t.CustomerPaymentD).HasColumnName("CustomerPaymentD");
			this.Property(t => t.Amount).HasColumnName("Amount");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.EntryClerkC).HasColumnName("EntryCleckC");
			this.Property(t => t.PaymentMethodI).HasColumnName("PaymentMethodI");
		}
	}
}
