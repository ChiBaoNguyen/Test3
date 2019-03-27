using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	internal class Partner_MMap : EntityTypeConfiguration<Partner_M>
	{
		public Partner_MMap()
		{
			// Primary Key
			this.HasKey(t => new { t.PartnerMainC, t.PartnerSubC });

			// Properties
			this.Property(t => t.PartnerMainC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.PartnerSubC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.PartnerN)
				.HasMaxLength(200);

			this.Property(t => t.PartnerShortN)
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

			this.Property(t => t.IsActive)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			// Table & Column Mappings
			this.ToTable("Partner_M");
			this.Property(t => t.PartnerMainC).HasColumnName("PartnerMainC");
			this.Property(t => t.PartnerSubC).HasColumnName("PartnerSubC");
			this.Property(t => t.PartnerN).HasColumnName("PartnerN");
			this.Property(t => t.PartnerShortN).HasColumnName("PartnerShortN");
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
			this.Property(t => t.IsActive).HasColumnName("IsActive");
		}
	}
}
