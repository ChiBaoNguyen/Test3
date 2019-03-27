using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class DriverLicenseUpdate_DMap : EntityTypeConfiguration<DriverLicenseUpdate_D>
	{
		public DriverLicenseUpdate_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.DriverC, t.LicenseC, t.ExpiryD });

			// Properties
			this.Property(t => t.DriverC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.LicenseC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.ExpiryD)
				.IsRequired()
				.HasColumnType("date");

			this.Property(t => t.UpdateD)
				.HasColumnType("date");

			this.Property(t => t.NextExpiryD)
				.HasColumnType("date");

			// Table & Column Mappings
			this.ToTable("DriverLicenseUpdate_D");
			this.Property(t => t.DriverC).HasColumnName("DriverC");
			this.Property(t => t.LicenseC).HasColumnName("LicenseC");
			this.Property(t => t.ExpiryD).HasColumnName("ExpiryD");
			this.Property(t => t.UpdateD).HasColumnName("UpdateD");
			this.Property(t => t.NextExpiryD).HasColumnName("NextExpiryD");
		}
	}
}