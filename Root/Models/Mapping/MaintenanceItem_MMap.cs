using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class MaintenanceItem_MMap : EntityTypeConfiguration<MaintenanceItem_M>
	{
		public MaintenanceItem_MMap()
		{
			// Primary Key
			this.HasKey(t => t.MaintenanceItemC);

			// Properties
			this.Property(t => t.MaintenanceItemC)
				.IsRequired();

			this.Property(t => t.DisplayLineNo);

			this.Property(t => t.MaintenanceItemN)
				.IsUnicode(true)
				.HasMaxLength(50);

			this.Property(t => t.NoticeI)
				.IsUnicode(false)
				.HasMaxLength(1)
				.IsFixedLength();

			this.Property(t => t.ReplacementInterval);

			this.Property(t => t.NoticeNo);

			// Table & Column Mappings
			this.ToTable("MaintenanceItem_M");
			this.Property(t => t.MaintenanceItemC).HasColumnName("MaintenanceItemC");
			this.Property(t => t.DisplayLineNo).HasColumnName("DisplayLineNo");
			this.Property(t => t.MaintenanceItemN).HasColumnName("MaintenanceItemN");
			this.Property(t => t.NoticeI).HasColumnName("NoticeI");
			this.Property(t => t.ReplacementInterval).HasColumnName("ReplacementInterval");
			this.Property(t => t.NoticeNo).HasColumnName("NoticeNo");
		}
	}
}