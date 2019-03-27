using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class Employee_MMap : EntityTypeConfiguration<Employee_M>
	{
		public Employee_MMap()
		{
			// Primary Key
			this.HasKey(t => t.EmployeeC);

			// Properties
			this.Property(t => t.EmployeeC)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.EmployeeFirstN)
				.IsRequired()
				.HasMaxLength(50);

			this.Property(t => t.EmployeeLastN)
				.IsRequired()
				.HasMaxLength(50);

			this.Property(t => t.DepC)
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.Address)
				.HasMaxLength(255);

			this.Property(t => t.PhoneNumber)
				.HasMaxLength(15)
				.IsUnicode(false);

			this.Property(t => t.BirthD)
				.HasColumnType("date");

			this.Property(t => t.JoinedD)
				.HasColumnType("date");

			this.Property(t => t.RetiredD)
				.HasColumnType("date");

			this.Property(t => t.IsActive)
				.HasMaxLength(1)
				.IsFixedLength()
				.IsUnicode(false);

			// Table & Column Mappings
			this.ToTable("Employee_M");
			this.Property(t => t.EmployeeC).HasColumnName("EmployeeC");
			this.Property(t => t.EmployeeFirstN).HasColumnName("EmployeeFirstN");
			this.Property(t => t.EmployeeLastN).HasColumnName("EmployeeLastN");
			this.Property(t => t.DepC).HasColumnName("DepC");
			this.Property(t => t.BirthD).HasColumnName("BirthD");
			this.Property(t => t.Address).HasColumnName("Address");
			this.Property(t => t.PhoneNumber).HasColumnName("PhoneNumber");
			this.Property(t => t.JoinedD).HasColumnName("JoinedD");
			this.Property(t => t.RetiredD).HasColumnName("RetiredD");
			this.Property(t => t.IsActive).HasColumnName("IsActive");
		}
	}
}
