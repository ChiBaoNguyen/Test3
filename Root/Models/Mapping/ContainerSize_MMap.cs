using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	class ContainerSize_MMap : EntityTypeConfiguration<ContainerSize_M>
	{
		public ContainerSize_MMap()
		{
			// Primary Key
			this.HasKey(t => t.ContainerSizeC);

			// Properties
			this.Property(t => t.ContainerSizeC)
				.IsRequired()
				.HasMaxLength(2);

			this.Property(t => t.ContainerSizeN)
				.IsRequired()
				.HasMaxLength(40);

			this.Property(t => t.Description)
				.HasMaxLength(255);

			// Table & Column Mappings
			this.ToTable("ContainerSize_M");
			this.Property(t => t.ContainerSizeC).HasColumnName("ContainerSizeC");
			this.Property(t => t.ContainerSizeN).HasColumnName("ContainerSizeN");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.IsActive).HasColumnName("IsActive");
		}
	}
}
