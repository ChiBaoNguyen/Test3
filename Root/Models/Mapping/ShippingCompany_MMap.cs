using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class ShippingCompany_MMap : EntityTypeConfiguration<ShippingCompany_M>
	{
		public ShippingCompany_MMap()
		{
			// Primary Key
			this.HasKey(t => t.ShippingCompanyC);

			// Properties
			this.Property(t => t.ShippingCompanyC)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.ShippingCompanyN)
				.IsRequired()
				.HasMaxLength(200);

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

			this.Property(t => t.ContainerCode1)
				.HasMaxLength(4)
				.IsUnicode(false);

			this.Property(t => t.ContainerCode2)
				.HasMaxLength(4)
				.IsUnicode(false);

			this.Property(t => t.ContainerCode3)
				.HasMaxLength(4)
				.IsUnicode(false);

			this.Property(t => t.ContainerCode4)
				.HasMaxLength(4)
				.IsUnicode(false);

			this.Property(t => t.ContainerCode5)
				.HasMaxLength(4)
				.IsUnicode(false);

			this.Property(t => t.ContainerCode6)
				.HasMaxLength(4)
				.IsUnicode(false);

			this.Property(t => t.ContainerCode7)
				.HasMaxLength(4)
				.IsUnicode(false);

			this.Property(t => t.ContainerCode8)
				.HasMaxLength(4)
				.IsUnicode(false);

			this.Property(t => t.ContainerCode9)
				.HasMaxLength(4)
				.IsUnicode(false);

			this.Property(t => t.ContainerCode10)
				.HasMaxLength(4)
				.IsUnicode(false);

			this.Property(t => t.IsActive)
				.HasMaxLength(1)
				.IsFixedLength()
				.IsUnicode(false);

			// Table & Column Mappings
			this.ToTable("ShippingCompany_M");
			this.Property(t => t.ShippingCompanyC).HasColumnName("ShippingCompanyC");
			this.Property(t => t.ShippingCompanyN).HasColumnName("ShippingCompanyN");
			this.Property(t => t.Address).HasColumnName("Address");
			this.Property(t => t.ContactPerson).HasColumnName("ContactPerson");
			this.Property(t => t.Email).HasColumnName("Email");
			this.Property(t => t.Fax).HasColumnName("Fax");
			this.Property(t => t.PhoneNumber).HasColumnName("PhoneNumber");
			this.Property(t => t.ContainerCode1).HasColumnName("ContainerCode1");
			this.Property(t => t.ContainerCode2).HasColumnName("ContainerCode2");
			this.Property(t => t.ContainerCode3).HasColumnName("ContainerCode3");
			this.Property(t => t.ContainerCode4).HasColumnName("ContainerCode4");
			this.Property(t => t.ContainerCode5).HasColumnName("ContainerCode5");
			this.Property(t => t.ContainerCode6).HasColumnName("ContainerCode6");
			this.Property(t => t.ContainerCode7).HasColumnName("ContainerCode7");
			this.Property(t => t.ContainerCode8).HasColumnName("ContainerCode8");
			this.Property(t => t.ContainerCode9).HasColumnName("ContainerCode9");
			this.Property(t => t.ContainerCode10).HasColumnName("ContainerCode10");
			this.Property(t => t.IsActive).HasColumnName("IsActive");
		}
	}
}
