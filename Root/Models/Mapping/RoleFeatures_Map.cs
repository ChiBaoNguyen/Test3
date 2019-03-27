using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class RoleFeatures_Map : EntityTypeConfiguration<RoleFeatures>
	{
		public RoleFeatures_Map()
		{
			// Primary Key
			this.HasKey(t => new { t.RoleId, t.UnusedFeatureC });

			// Properties
			this.Property(t => t.RoleId)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(128);

			this.Property(t => t.UnusedFeatureC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(128);

			// Table & Column Mappings
			this.ToTable("RoleFeatures");
			this.Property(t => t.RoleId).HasColumnName("RoleId");
			this.Property(t => t.UnusedFeatureC).HasColumnName("UnusedFeatureC");
		}
	}
}
