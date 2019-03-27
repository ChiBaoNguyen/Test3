using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class DriverLicense_MMap : EntityTypeConfiguration<DriverLicense_M>
	{
		public DriverLicense_MMap()
		{
			// Primary Key
			this.HasKey(t => new {t.DriverC, t.LicenseC });

			// Properties
			this.Property(t => t.DriverC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.LicenseC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.DriverLicenseNo)
				.IsUnicode(false)
				.HasMaxLength(50);

			this.Property(t => t.DriverLicenseD)
				.HasColumnType("date");

			this.Property(t => t.ExpiryD)
				.HasColumnType("date");

			// Table & Column Mappings
			this.ToTable("DriverLicense_M");
			this.Property(t => t.DriverC).HasColumnName("DriverC");
			this.Property(t => t.LicenseC).HasColumnName("LicenseC");
			this.Property(t => t.DriverLicenseNo).HasColumnName("DriverLicenseNo");
			this.Property(t => t.DriverLicenseD).HasColumnName("DriverLicenseD");
			this.Property(t => t.ExpiryD).HasColumnName("ExpiryD");
		}
	}
}