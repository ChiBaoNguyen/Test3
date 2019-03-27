using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class MaintenancePlan_DMap : EntityTypeConfiguration<MaintenancePlan_D>
	{
		public MaintenancePlan_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.ObjectI, t.Code, t.MaintenanceItemC });

			// Properties
			this.Property(t => t.ObjectI)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.Code)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.MaintenanceItemC)
				.IsRequired();

			this.Property(t => t.PlanMaintenanceD)
				.HasColumnType("date");

			this.Property(t => t.PlanMaintenanceKm)
				.HasPrecision(7, 1);

			// Table & Column Mappings
			this.ToTable("MaintenancePlan_D");
			this.Property(t => t.ObjectI).HasColumnName("ObjectI");
			this.Property(t => t.Code).HasColumnName("Code");
			this.Property(t => t.MaintenanceItemC).HasColumnName("MaintenanceItemC");
			this.Property(t => t.PlanMaintenanceD).HasColumnName("PlanMaintenanceD");
			this.Property(t => t.PlanMaintenanceKm).HasColumnName("PlanMaintenanceKm");
		}
	}
}