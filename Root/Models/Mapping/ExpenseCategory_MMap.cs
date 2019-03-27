using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
    public class ExpenseCategory_MMap : EntityTypeConfiguration<ExpenseCategory_M>
    {
        public ExpenseCategory_MMap()
        {
            // Primary Key
            this.HasKey(t => t.CategoryC);

            // Properties
            this.Property(t => t.CategoryC)
                .IsRequired()
                .HasMaxLength(5)
				.IsUnicode(false);

            this.Property(t => t.CategoryN)
                .HasMaxLength(50);

            this.Property(t => t.CategoryShortN)
                .HasMaxLength(30);

			this.Property(t => t.IsActive)
				.HasMaxLength(1)
				.IsFixedLength()
				.IsUnicode(false);

            // Table & Column Mappings
            this.ToTable("ExpenseCategory_M");
            this.Property(t => t.CategoryC).HasColumnName("CategoryC");
            this.Property(t => t.CategoryN).HasColumnName("CategoryN");
            this.Property(t => t.CategoryShortN).HasColumnName("CategoryShortN");
			this.Property(t => t.IsActive).HasColumnName("IsActive");
        }
    }
}
