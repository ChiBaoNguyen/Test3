using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
    public class Department_MMap : EntityTypeConfiguration<Department_M>
    {
        public Department_MMap()
        {
            // Primary Key
            this.HasKey(t => t.DepC);

            // Properties
            this.Property(t => t.DepC)
                .IsRequired()
                .HasMaxLength(5)
				.IsUnicode(false);

            this.Property(t => t.DepN)
                .HasMaxLength(50);

			this.Property(t => t.IsActive)
				.HasMaxLength(1)
				.IsFixedLength()
				.IsUnicode(false);

            // Table & Column Mappings
            this.ToTable("Department_M");
            this.Property(t => t.DepC).HasColumnName("DepC");
            this.Property(t => t.DepN).HasColumnName("DepN");
			this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
