﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class Surcharge_DMap : EntityTypeConfiguration<Surcharge_D>
	{
		public Surcharge_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.OrderD, t.OrderNo, t.DetailNo, t.DispatchNo, t.DispatchI, t.SurchargeNo, t.SurchargeC });

			// Properties
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

			this.Property(t => t.DispatchI)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.SurchargeNo)
				.IsRequired();

			this.Property(t => t.SurchargeC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.Unit)
				.HasMaxLength(50);

			this.Property(t => t.UnitPrice)
				.HasPrecision(10, 0);

			this.Property(t => t.Quantity)
				.HasPrecision(4, 0);

			this.Property(t => t.Amount)
				.HasPrecision(10, 0);

			this.Property(t => t.Description)
				.HasMaxLength(255);

			// Table & Column Mappings
			this.ToTable("Surcharge_D");
			this.Property(t => t.OrderD).HasColumnName("OrderD");
			this.Property(t => t.OrderNo).HasColumnName("OrderNo");
			this.Property(t => t.DetailNo).HasColumnName("DetailNo");
			this.Property(t => t.DispatchNo).HasColumnName("DispatchNo");
			this.Property(t => t.DispatchI).HasColumnName("DispatchI");
			this.Property(t => t.SurchargeNo).HasColumnName("SurchargeNo");
			this.Property(t => t.SurchargeC).HasColumnName("SurchargeC");
			this.Property(t => t.Unit).HasColumnName("Unit");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			this.Property(t => t.Quantity).HasColumnName("Quantity");
			this.Property(t => t.Amount).HasColumnName("Amount");
			this.Property(t => t.Description).HasColumnName("Description");

		}
	}
}
