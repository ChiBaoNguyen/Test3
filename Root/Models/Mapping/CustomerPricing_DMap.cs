using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class CustomerPricing_DMap : EntityTypeConfiguration<CustomerPricing_D>
	{
		public CustomerPricing_DMap()
		{
			// Primary Key
			this.HasKey(t => t.CustomerPricingExpenseId);

			// Properties
			this.Property(t => t.CustomerPricingExpenseId)
				.IsUnicode(false)
				.IsRequired()
				.HasMaxLength(64);

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

			this.Property(t => t.CategoryI)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(1);

			this.Property(t => t.ExpenseC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.ExpenseN)
				.IsRequired()
				.IsUnicode(true)
				.HasMaxLength(50);

			this.Property(t => t.Unit)
				.HasMaxLength(50);

			this.Property(t => t.UnitPrice)
				.HasPrecision(10, 0);

			this.Property(t => t.Quantity)
				.HasPrecision(4, 0);

			this.Property(t => t.Amount)
				.HasPrecision(10, 0);

			this.Property(t => t.ExpenseRoot)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.Description)
				.IsUnicode(true)
				.HasMaxLength(100);

			// Table & Column Mappings
			this.ToTable("CustomerPricing_D");
			this.Property(t => t.CustomerPricingExpenseId).HasColumnName("CustomerPricingExpenseId");
			this.Property(t => t.CustomerPricingId).HasColumnName("CustomerPricingId");
			this.Property(t => t.RouteId).HasColumnName("RouteId");
			this.Property(t => t.OrderD).HasColumnName("OrderD");
			this.Property(t => t.OrderNo).HasColumnName("OrderNo");
			this.Property(t => t.DetailNo).HasColumnName("DetailNo");
			this.Property(t => t.DispatchNo).HasColumnName("DispatchNo");
			this.Property(t => t.CategoryI).HasColumnName("CategoryI");
			this.Property(t => t.ExpenseC).HasColumnName("ExpenseC");
			this.Property(t => t.ExpenseN).HasColumnName("ExpenseN");
			this.Property(t => t.Unit).HasColumnName("Unit");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			this.Property(t => t.Quantity).HasColumnName("Quantity");
			this.Property(t => t.Amount).HasColumnName("Amount");
			this.Property(t => t.ExpenseRoot).HasColumnName("ExpenseRoot");
			this.Property(t => t.Description).HasColumnName("Description");
		}
	}
}
