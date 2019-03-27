using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization.Formatters;

namespace Root.Models.Mapping
{
	public class CustomerBalance_DMap : EntityTypeConfiguration<CustomerBalance_D>
	{
		public CustomerBalance_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.CustomerMainC, t.CustomerSubC, t.CustomerBalanceD });

			this.Property(t => t.CustomerMainC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.CustomerSubC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.CustomerBalanceD)
				.HasColumnType("date");

			this.Property(t => t.Amount)
				.HasPrecision(12, 0);

			this.Property(t => t.TotalExpense)
				.HasPrecision(12, 0);

			this.Property(t => t.CustomerSurcharge)
				.HasPrecision(12, 0);

			this.Property(t => t.CustomerDiscount)
				.HasPrecision(12, 0);

			this.Property(t => t.DetainAmount)
				.HasPrecision(12, 0);

			this.Property(t => t.TotalAmount)
				.HasPrecision(12, 0);

			this.Property(t => t.TaxAmount)
				.HasPrecision(12, 0);

			this.Property(t => t.PaymentAmount)
				.HasPrecision(12, 0);


			// Table & Column Mappings
			this.ToTable("CustomerBalance_D");
			this.Property(t => t.CustomerMainC).HasColumnName("CustomerMainC");
			this.Property(t => t.CustomerSubC).HasColumnName("CustomerSubC");
			this.Property(t => t.CustomerBalanceD).HasColumnName("CustomerBalanceD");
			this.Property(t => t.Amount).HasColumnName("Amount");
			this.Property(t => t.TotalExpense).HasColumnName("TotalExpense");
			this.Property(t => t.CustomerSurcharge).HasColumnName("CustomerSurcharge");
			this.Property(t => t.CustomerDiscount).HasColumnName("CustomerDiscount");
			this.Property(t => t.DetainAmount).HasColumnName("DetainAmount");
			this.Property(t => t.TotalAmount).HasColumnName("TotalAmount");
			this.Property(t => t.TaxAmount).HasColumnName("TaxAmount");
			this.Property(t => t.PaymentAmount).HasColumnName("PaymentAmount");


		}
	}
}
