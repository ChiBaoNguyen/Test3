using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class Customer_MMap : EntityTypeConfiguration<Customer_M>
	{
		public Customer_MMap()
		{
			// Primary Key
			this.HasKey(t => new { t.CustomerMainC, t.CustomerSubC });

			// Properties
			this.Property(t => t.CustomerMainC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.CustomerSubC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.CustomerN)
				.HasMaxLength(200);

			this.Property(t => t.CustomerShortN)
				.HasMaxLength(30);

			this.Property(t => t.Address1)
				.HasMaxLength(255);

			this.Property(t => t.Address2)
				.HasMaxLength(255);

			this.Property(t => t.ContactPerson)
				.HasMaxLength(50);

			this.Property(t => t.PhoneNumber1)
				.IsUnicode(false)
				.HasMaxLength(15);

			this.Property(t => t.PhoneNumber2)
				.IsUnicode(false)
				.HasMaxLength(15);

			this.Property(t => t.Fax)
				.IsUnicode(false)
				.HasMaxLength(15);

			this.Property(t => t.Email)
				.IsUnicode(false)
				.HasMaxLength(50);

			this.Property(t => t.InvoiceMainC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.InvoiceSubC)
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.TaxCode)
				.IsUnicode(false)
				.HasMaxLength(15);

			this.Property(t => t.BankAccNumber)
				.IsUnicode(false)
				.HasMaxLength(15);

			this.Property(t => t.BankAccN)
				.HasMaxLength(50);

			this.Property(t => t.BankN)
				.HasMaxLength(100);



			this.Property(t => t.IsCollected)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.IsActive)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			// Table & Column Mappings
			this.ToTable("Customer_M");
			this.Property(t => t.CustomerMainC).HasColumnName("CustomerMainC");
			this.Property(t => t.CustomerSubC).HasColumnName("CustomerSubC");
			this.Property(t => t.CustomerN).HasColumnName("CustomerN");
			this.Property(t => t.CustomerShortN).HasColumnName("CustomerShortN");
			this.Property(t => t.Address1).HasColumnName("Address1");
			this.Property(t => t.Address2).HasColumnName("Address2");
			this.Property(t => t.ContactPerson).HasColumnName("ContactPerson");
			this.Property(t => t.PhoneNumber1).HasColumnName("PhoneNumber1");
			this.Property(t => t.PhoneNumber2).HasColumnName("PhoneNumber2");
			this.Property(t => t.Fax).HasColumnName("Fax");
			this.Property(t => t.Email).HasColumnName("Email");
			this.Property(t => t.InvoiceMainC).HasColumnName("InvoiceMainC");
			this.Property(t => t.InvoiceSubC).HasColumnName("InvoiceSubC");
			this.Property(t => t.TaxCode).HasColumnName("TaxCode");
			this.Property(t => t.BankAccNumber).HasColumnName("BankAccNumber");
			this.Property(t => t.BankAccN).HasColumnName("BankAccN");
			this.Property(t => t.BankN).HasColumnName("BankN");
			this.Property(t => t.IsCollected).HasColumnName("IsCollected");
			this.Property(t => t.IsActive).HasColumnName("IsActive");
		}
	}
}
