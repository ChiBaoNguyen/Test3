using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class LocationDetail_MMap : EntityTypeConfiguration<LocationDetail_M>
	{
		public LocationDetail_MMap()
		{
			// Primary Key
			this.HasKey(t => new { t.ExpenseC, t.ContainerSizeI, t.LocationC, t.LocationDetailId });

			this.Property(t => t.ExpenseC)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(5);

			this.Property(t => t.ExpenseN)
				.IsRequired()
				.HasMaxLength(50);

			this.Property(t => t.ContainerSizeI)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.AmountMoney)
				.HasPrecision(10, 1);

			this.Property(t => t.Category)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.Display)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.CategoryExpense)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.Description)
				.HasMaxLength(255);

			this.Property(t => t.LocationDetailId)
				.IsRequired();

			// Table & Column Mappings
			this.ToTable("LocationDetail_M");

			this.Property(t => t.ExpenseC).HasColumnName("ExpenseC");
			this.Property(t => t.ExpenseN).HasColumnName("ExpenseN");
			this.Property(t => t.ContainerSizeI).HasColumnName("ContainerSizeI");
			this.Property(t => t.AmountMoney).HasColumnName("AmountMoney");
			this.Property(t => t.Category).HasColumnName("Category");
			this.Property(t => t.Display).HasColumnName("Display");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.CategoryExpense).HasColumnName("CategoryExpense");
			this.Property(t => t.LocationDetailId).HasColumnName("LocationDetailId");
		}
	}
}
