using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	internal class Shipper_MMap : EntityTypeConfiguration<Shipper_M>
	{
		public Shipper_MMap()
		{
			// Primary Key
			this.HasKey(t => t.ShipperC);

			this.Property(t => t.ShipperC)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.ShipperN)
				.IsRequired()
				.HasMaxLength(50);

			this.Property(t => t.Address)
				.HasMaxLength(255);

			this.Property(t => t.ContactPerson)
				.HasMaxLength(50);

			this.Property(t => t.PhoneNumber)
				.HasMaxLength(15)
				.IsUnicode(false);

			this.Property(t => t.Fax)
				.HasMaxLength(15)
				.IsUnicode(false);

			this.Property(t => t.Email)
				.HasMaxLength(50)
				.IsUnicode(false);

			this.Property(t => t.IsActive)
				.HasMaxLength(1)
				.IsFixedLength()
				.IsUnicode(false);

			// Table & Column Mappings
			this.ToTable("Shipper_M");
			this.Property(t => t.ShipperC).HasColumnName("ShipperC");
			this.Property(t => t.ShipperN).HasColumnName("ShipperN");
			this.Property(t => t.Address).HasColumnName("Address");
			this.Property(t => t.ContactPerson).HasColumnName("ContactPerson");
			this.Property(t => t.PhoneNumber).HasColumnName("PhoneNumber");
			this.Property(t => t.Fax).HasColumnName("Fax");
			this.Property(t => t.Email).HasColumnName("Email");
			this.Property(t => t.IsActive).HasColumnName("IsActive");
		}
	}
}
