using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
    public class TextResource_DMap : EntityTypeConfiguration<TextResource_D>
    {
        public TextResource_DMap()
        {
            // Primary Key
            this.HasKey(t => t.TextID);

            // Properties
            this.Property(t => t.TextKey)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TextValue)
                .HasMaxLength(255);

            this.Property(t => t.Description)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("TextResource_D");
            this.Property(t => t.TextID).HasColumnName("TextID");
            this.Property(t => t.TextKey).HasColumnName("TextKey");
            this.Property(t => t.TextValue).HasColumnName("TextValue");
            this.Property(t => t.LanguageID).HasColumnName("LanguageID");
            this.Property(t => t.ScreenID).HasColumnName("ScreenID");
            this.Property(t => t.Description).HasColumnName("Description");
        }
    }
}
