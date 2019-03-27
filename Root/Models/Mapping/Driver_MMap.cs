using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class Driver_MMap : EntityTypeConfiguration<Driver_M>
	{
		public Driver_MMap()
		{
			// Primary Key
			this.HasKey(t => t.DriverC);

			// Properties
			this.Property(t => t.DriverC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.FirstN)
				.IsRequired()
				.HasMaxLength(40);

			this.Property(t => t.LastN)
				.IsRequired()
				.HasMaxLength(200);

			this.Property(t => t.DepC)
				.HasMaxLength(5);

			this.Property(t => t.Address)
				.HasMaxLength(255);

			this.Property(t => t.PhoneNumber)
				.HasMaxLength(15);

			this.Property(t => t.AdvancePaymentLimit)
				.HasPrecision(10, 0);

			this.Property(t => t.BasicSalary)
				.HasPrecision(10, 0);

			this.Property(t => t.BirthD)
				.HasColumnType("date");

			this.Property(t => t.JoinedD)
				.HasColumnType("date");

			this.Property(t => t.RetiredD)
				.HasColumnType("date");

			this.Property(t => t.IsActive)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			// Table & Column Mappings
			this.ToTable("Driver_M");
			this.Property(t => t.DriverC).HasColumnName("DriverC");
			this.Property(t => t.FirstN).HasColumnName("FirstN");
			this.Property(t => t.LastN).HasColumnName("LastN");
			this.Property(t => t.DepC).HasColumnName("DepC");
			this.Property(t => t.BirthD).HasColumnName("BirthD");
			this.Property(t => t.Address).HasColumnName("Address");
			this.Property(t => t.PhoneNumber).HasColumnName("PhoneNumber");
			this.Property(t => t.JoinedD).HasColumnName("JoinedD");
			this.Property(t => t.RetiredD).HasColumnName("RetiredD");
			this.Property(t => t.AdvancePaymentLimit).HasColumnName("AdvancePaymentLimit");
			this.Property(t => t.IsActive).HasColumnName("IsActive");
			this.Property(t => t.BasicSalary).HasColumnName("BasicSalary");
		}
	}
}