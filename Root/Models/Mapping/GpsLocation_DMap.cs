using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class GpsLocation_DMap : EntityTypeConfiguration<GpsLocation_D>
	{
        public GpsLocation_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.DriverC });

			// Properties
            this.Property(t => t.DriverC)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(5); 
            
            this.Property(t => t.UpdateD)
				.HasColumnType("datetime")
				.IsRequired();

			// Table & Column Mappings
            this.ToTable("GpsLocation_D");
            this.Property(t => t.DriverC).HasColumnName("DriverC");
            this.Property(t => t.Latitude).HasColumnName("Latitude");
            this.Property(t => t.Longitude).HasColumnName("Longitude");
            this.Property(t => t.UpdateD).HasColumnName("UpdateD");
		}
	}
}
