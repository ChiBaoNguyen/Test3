using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class Maintenance_DMap : EntityTypeConfiguration<Maintenance_D>
	{
		public Maintenance_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.ObjectI, t.Code, t.InspectionC, t.MaintenanceD, t.MaintenanceItemC });

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

			this.Property(t => t.InspectionC)
				.IsRequired();

			this.Property(t => t.MaintenanceD)
				.IsRequired()
				.HasColumnType("date");

			this.Property(t => t.MaintenanceItemC)
				.IsRequired();

			this.Property(t => t.PlanMaintenanceD)
				.HasColumnType("date");

			this.Property(t => t.PlanMaintenanceKm)
				.HasPrecision(7, 1);

			this.Property(t => t.RemarksI)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.NextMaintenanceD)
				.HasColumnType("date");

			this.Property(t => t.NextMaintenanceKm)
				.HasPrecision(7, 1);

			this.Property(t => t.PartNo)
				.IsUnicode(true)
				.HasMaxLength(255);

			this.Property(t => t.Quantity)
				.HasPrecision(4, 0);

			this.Property(t => t.Unit)
				 .HasMaxLength(50);

			this.Property(t => t.UnitPrice)
				.HasPrecision(10, 0);

			this.Property(t => t.Amount)
				.HasPrecision(10, 0);

			this.Property(t => t.Description)
				.IsUnicode(true)
				.HasMaxLength(255);

			// Table & Column Mappings
			this.ToTable("Maintenance_D");
			this.Property(t => t.ObjectI).HasColumnName("ObjectI");
			this.Property(t => t.Code).HasColumnName("Code");
			this.Property(t => t.InspectionC).HasColumnName("InspectionC");
			this.Property(t => t.MaintenanceD).HasColumnName("MaintenanceD");
			this.Property(t => t.MaintenanceItemC).HasColumnName("MaintenanceItemC");
			this.Property(t => t.PlanMaintenanceD).HasColumnName("PlanMaintenanceD");
			this.Property(t => t.PlanMaintenanceKm).HasColumnName("PlanMaintenanceKm");
			this.Property(t => t.RemarksI).HasColumnName("RemarksI");
			this.Property(t => t.NextMaintenanceD).HasColumnName("NextMaintenanceD");
			this.Property(t => t.NextMaintenanceKm).HasColumnName("NextMaintenanceKm");
			this.Property(t => t.Description).HasColumnName("PartNo");
			this.Property(t => t.Quantity).HasColumnName("Quantity");
			this.Property(t => t.Unit).HasColumnName("Unit");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			this.Property(t => t.Amount).HasColumnName("Amount");
			this.Property(t => t.Description).HasColumnName("Description");
		}
	}
}