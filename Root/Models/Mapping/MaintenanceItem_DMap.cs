using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class MaintenanceItem_DMap : EntityTypeConfiguration<MaintenanceItem_D>
	{
		public MaintenanceItem_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.ObjectI, t.ModelC, t.MaintenanceItemC });

			// Properties
			this.Property(t => t.ObjectI)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.ModelC).IsRequired();

			this.Property(t => t.MaintenanceItemC)
				.IsRequired();

			this.Property(t => t.DisplayLineNo);

			// Table & Column Mappings
			this.ToTable("MaintenanceItem_D");
			this.Property(t => t.ObjectI).HasColumnName("ObjectI");
			this.Property(t => t.ModelC).HasColumnName("ModelC");
			this.Property(t => t.MaintenanceItemC).HasColumnName("MaintenanceItemC");
			this.Property(t => t.DisplayLineNo).HasColumnName("DisplayLineNo");
		}
	}
}