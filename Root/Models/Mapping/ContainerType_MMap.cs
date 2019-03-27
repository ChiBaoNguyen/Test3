using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class ContainerType_MMap : EntityTypeConfiguration<ContainerType_M>
	{
		public ContainerType_MMap()
		{
			// Primary Key
			this.HasKey(t => t.ContainerTypeC);

			// Properties
			this.Property(t => t.ContainerTypeC)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false); ;

			this.Property(t => t.ContainerTypeN)
				.IsRequired()
				.HasMaxLength(50);

			this.Property(t => t.Description)
				.HasMaxLength(255);

			this.Property(t => t.IsActive)
				.HasMaxLength(1)
				.IsFixedLength()
				.IsUnicode(false);

			// Table & Column Mappings
			this.ToTable("ContainerType_M");
			this.Property(t => t.ContainerTypeC).HasColumnName("ContainerTypeC");
			this.Property(t => t.ContainerTypeN).HasColumnName("ContainerTypeN");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.IsActive).HasColumnName("IsActive");
		}
	}
}
