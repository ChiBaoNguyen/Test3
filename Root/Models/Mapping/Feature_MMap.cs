using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class Feature_MMap : EntityTypeConfiguration<Feature_M>
	{
		public Feature_MMap()
		{
			// Primary Key
			this.HasKey(t => t.FeatureC);
			

			// Properties
			this.Property(t => t.FeatureC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(128);

			this.Property(t => t.FeatureN)
				.HasMaxLength(256);

			this.Property(t => t.FeatureGroupN)
				.HasMaxLength(256);

			this.Property(t => t.FeatureParentGroupN)
				.HasMaxLength(256);

			// Table & Column Mappings
			this.ToTable("Feature_M");
			this.Property(t => t.FeatureC).HasColumnName("FeatureC");
			this.Property(t => t.FeatureN).HasColumnName("FeatureN");
			this.Property(t => t.FeatureGroupN).HasColumnName("FeatureGroupN");
			this.Property(t => t.FeatureParentGroupN).HasColumnName("FeatureParentGroupN");
			this.Property(t => t.SortOrder).HasColumnName("SortOrder");
		}
	}
}
