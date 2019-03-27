using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class Model_MMap : EntityTypeConfiguration<Model_M>
	{
		public Model_MMap()
		{
			// Primary Key
			this.HasKey(t => new { t.ObjectI, t.ModelC });

			// Properties
			this.Property(t => t.ObjectI)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(1)
				.IsFixedLength();

			this.Property(t => t.ModelC)
				.IsUnicode(true)
				.HasMaxLength(5); ;

			this.Property(t => t.ModelN)
				.IsUnicode(true)
				.HasMaxLength(50);

			// Table & Column Mappings
			this.ToTable("Model_M");
			this.Property(t => t.ObjectI).HasColumnName("ObjectI");
			this.Property(t => t.ModelC).HasColumnName("ModelC");
			this.Property(t => t.ModelN).HasColumnName("ModelN");
		}
	}
}