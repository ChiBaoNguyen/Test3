using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
    public class UploadImageMobile_Map : EntityTypeConfiguration<UploadImageMobile>
    {
        public UploadImageMobile_Map()
        {
            this.HasKey(t => t.ImageId);

            // Properties
            this.Property(t => t.ImageLink);

            this.Property(t => t.OrderImageKey);

            this.Property(t => t.CreatedDate);

            // Table & Column Mappings
            this.ToTable("UploadImageMobile");
            this.Property(t => t.ImageId).HasColumnName("ImageId");
            this.Property(t => t.ImageLink).HasColumnName("ImageLink");
            this.Property(t => t.OrderImageKey).HasColumnName("OrderImageKey");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
        }
    }
}
