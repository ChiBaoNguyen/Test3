using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
    public class Screen_MMap : EntityTypeConfiguration<Screen_M>
    {
        public Screen_MMap()
        {
            // Primary Key
            this.HasKey(t => t.ScreenID);

            // Properties
            this.Property(t => t.ScreenID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ScreenName)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.Description)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("Screen_M");
            this.Property(t => t.ScreenID).HasColumnName("ScreenID");
            this.Property(t => t.ScreenName).HasColumnName("ScreenName");
            this.Property(t => t.Description).HasColumnName("Description");
        }
    }
}
