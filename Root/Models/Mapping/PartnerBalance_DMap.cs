using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization.Formatters;

namespace Root.Models.Mapping
{
	public class PartnerBalance_DMap : EntityTypeConfiguration<PartnerBalance_D>
	{
		public PartnerBalance_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.PartnerMainC, t.PartnerSubC, t.PartnerBalanceD });

			this.Property(t => t.PartnerMainC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.PartnerSubC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.PartnerBalanceD)
				.HasColumnType("date");

            this.Property(t => t.PartnerFee)
				.HasPrecision(12, 0);

            this.Property(t => t.PartnerExpense)
				.HasPrecision(12, 0);

			this.Property(t => t.PartnerSurcharge)
				.HasPrecision(12, 0);

			this.Property(t => t.PartnerDiscount)
				.HasPrecision(12, 0);

			this.Property(t => t.TotalAmount)
				.HasPrecision(12, 0);

			this.Property(t => t.TaxAmount)
				.HasPrecision(12, 0);

			this.Property(t => t.PaymentAmount)
				.HasPrecision(12, 0);


			// Table & Column Mappings
			this.ToTable("PartnerBalance_D");
			this.Property(t => t.PartnerMainC).HasColumnName("PartnerMainC");
			this.Property(t => t.PartnerSubC).HasColumnName("PartnerSubC");
			this.Property(t => t.PartnerBalanceD).HasColumnName("PartnerBalanceD");
            this.Property(t => t.PartnerFee).HasColumnName("PartnerFee");
            this.Property(t => t.PartnerExpense).HasColumnName("PartnerExpense");
			this.Property(t => t.PartnerSurcharge).HasColumnName("PartnerSurcharge");
			this.Property(t => t.PartnerDiscount).HasColumnName("PartnerDiscount");
			this.Property(t => t.TotalAmount).HasColumnName("TotalAmount");
			this.Property(t => t.TaxAmount).HasColumnName("TaxAmount");
			this.Property(t => t.PaymentAmount).HasColumnName("PaymentAmount");


		}
	}
}
