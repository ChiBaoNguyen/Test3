using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
    public class Language_MMap : EntityTypeConfiguration<Language_M>
    {
        public Language_MMap()
        {
            // Primary Key
            this.HasKey(t => t.LanguageID);

            // Properties
            this.Property(t => t.LanguageID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.LanguageCode)
                .HasMaxLength(3);

            this.Property(t => t.Country)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Language_M");
            this.Property(t => t.LanguageID).HasColumnName("LanguageID");
            this.Property(t => t.LanguageCode).HasColumnName("LanguageCode");
            this.Property(t => t.Country).HasColumnName("Country");
        }
    }
}
