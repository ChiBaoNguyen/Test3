using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	class CustomerPricingLocation_DMap : EntityTypeConfiguration<CustomerPricingLocation_D>
	{
		public CustomerPricingLocation_DMap()
		{
			// Primary Key
			this.HasKey(t => t.CustomerPricingLocationId);

			// Properties
			this.Property(t => t.CustomerPricingLocationId)
				.IsRequired()
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

			this.Property(t => t.CustomerPricingId)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(64);

			this.Property(t => t.RouteId)
				.IsUnicode(false)
				.HasMaxLength(64);

			this.Property(t => t.OrderD)
				.HasColumnType("date");

			this.Property(t => t.OrderNo)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(7);

			this.Property(t => t.ExpenseRoot)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			// Table & Column Mappings
			this.ToTable("CustomerPricingLocation_D");
			this.Property(t => t.CustomerPricingLocationId).HasColumnName("CustomerPricingLocationId");
			this.Property(t => t.CustomerPricingId).HasColumnName("CustomerPricingId");
			this.Property(t => t.RouteId).HasColumnName("RouteId");
			this.Property(t => t.OrderD).HasColumnName("OrderD");
			this.Property(t => t.OrderNo).HasColumnName("OrderNo");
			this.Property(t => t.DetailNo).HasColumnName("DetailNo");
			this.Property(t => t.DispatchNo).HasColumnName("DispatchNo");
			this.Property(t => t.ExpenseRoot).HasColumnName("ExpenseRoot");
		}
	}
}
