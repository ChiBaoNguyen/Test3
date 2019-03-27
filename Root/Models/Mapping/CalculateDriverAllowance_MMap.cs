
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class CalculateDriverAllowance_MMap : EntityTypeConfiguration<CalculateDriverAllowance_M>
	{
		public CalculateDriverAllowance_MMap()
		{
			// Primary Key
			this.HasKey(t => new { t.CalculateC });

			// Properties
			this.Property(t => t.CalculateC)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.DriverC)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.ApplyD)
				.HasColumnType("datetime");

			this.Property(t => t.TakeABreak)
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.Description)
				.HasMaxLength(255);

			this.Property(t => t.Content)
				.HasMaxLength(255);

			this.Property(t => t.AmountMoney)
				.HasPrecision(10, 0);

			this.Property(t => t.AmountMoneySubtract)
				.HasPrecision(10, 0);

			this.Property(t => t.CalculateSalary)
				.IsRequired()
				.HasMaxLength(1)
				.IsUnicode(false);

			//	Table & Column Mappings
			this.ToTable("CalculateDriverAllowance_M");

			this.Property(t => t.CalculateC).HasColumnName("CalculateC");

			this.Property(t => t.DriverC).HasColumnName("DriverC");

			this.Property(t => t.ApplyD).HasColumnName("ApplyD");

			this.Property(t => t.TakeABreak).HasColumnName("TakeABreak");

			this.Property(t => t.Description).HasColumnName("Description");

			this.Property(t => t.Content).HasColumnName("Content");

			this.Property(t => t.AmountMoney).HasColumnName("AmountMoney");

			this.Property(t => t.AmountMoneySubtract).HasColumnName("AmountMoneySubtract");

			this.Property(t => t.CalculateSalary).HasColumnName("CalculateSalary");
		}
	}
}
