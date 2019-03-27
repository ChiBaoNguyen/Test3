using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class LiabilitiesItem_DMap : EntityTypeConfiguration<LiabilitiesItem_D>
	{
		public LiabilitiesItem_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.OrderD, t.OrderNo, t.DetailNo, t.DispatchNo, t.ExpenseNo });

			// Properties
			this.Property(t => t.LiabilitiesD)
				.HasColumnType("date");

			this.Property(t => t.OrderD)
				.HasColumnType("date")
				.IsRequired();

			this.Property(t => t.OrderNo)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(7);

			this.Property(t => t.DetailNo)
				.IsRequired();

			this.Property(t => t.DispatchNo)
				.IsRequired();

			this.Property(t => t.ExpenseNo)
				.IsRequired();

			this.Property(t => t.LiabilitiesStatusI)
				.IsFixedLength()
				.IsUnicode(false)
				.HasMaxLength(1);

			// Table & Column Mappings
			this.ToTable("LiabilitiesItem_D");
		}
	}
}
