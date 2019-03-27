using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class InspectionPlan_DMap : EntityTypeConfiguration<InspectionPlan_D>
	{
		public InspectionPlan_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.ObjectI, t.Code, t.InspectionC });

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

			this.Property(t => t.InspectionPlanD)
				.IsRequired()
				.HasColumnType("date");

			// Table & Column Mappings
			this.ToTable("InspectionPlan_D");
			this.Property(t => t.ObjectI).HasColumnName("ObjectI");
			this.Property(t => t.Code).HasColumnName("Code");
			this.Property(t => t.InspectionC).HasColumnName("InspectionC");
			this.Property(t => t.InspectionPlanD).HasColumnName("InspectionPlanD");
		}
	}
}