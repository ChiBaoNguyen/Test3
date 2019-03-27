using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class Inspection_MMap : EntityTypeConfiguration<Inspection_M>
	{
		public Inspection_MMap()
		{
			// Primary Key
			this.HasKey(t => t.InspectionC);

			// Properties
			this.Property(t => t.InspectionC)
				.IsRequired();

			this.Property(t => t.DisplayLineNo);

			this.Property(t => t.InspectionN)
				.IsUnicode(true)
				.HasMaxLength(50);

			this.Property(t => t.NoticeNo)
				.IsRequired();

			this.Property(t => t.ObjectI)
				.IsRequired();

			// Table & Column Mappings
			this.ToTable("Inspection_M");
			this.Property(t => t.InspectionC).HasColumnName("InspectionC");
			this.Property(t => t.DisplayLineNo).HasColumnName("DisplayLineNo");
			this.Property(t => t.InspectionN).HasColumnName("InspectionN");
			this.Property(t => t.NoticeNo).HasColumnName("NoticeNo");
			this.Property(t => t.ObjectI).HasColumnName("ObjectI");
		}
	}
}