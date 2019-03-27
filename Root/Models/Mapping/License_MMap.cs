using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class License_MMap : EntityTypeConfiguration<License_M>
	{
		public License_MMap()
		{
			// Primary Key
			this.HasKey(t => t.LicenseC);

			// Properties
			this.Property(t => t.LicenseC)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.LicenseN)
				.IsRequired()
				.HasMaxLength(50);

			// Table & Column Mappings
			this.ToTable("License_M");
			this.Property(t => t.LicenseC).HasColumnName("LicenseC");
			this.Property(t => t.LicenseN).HasColumnName("LicenseN");
			this.Property(t => t.DisplayLineNo).HasColumnName("DisplayLineNo");
		}
	}
}
