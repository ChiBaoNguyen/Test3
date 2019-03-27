using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	internal class Truck_MMap : EntityTypeConfiguration<Truck_M>
	{
		public Truck_MMap()
		{
			// Primary Key
			this.HasKey(t => t.TruckC);

			this.Property(t => t.TruckC)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.RegisteredNo)
				.HasMaxLength(20)
				.IsUnicode(false);

			this.Property(t => t.RegisteredD)
				.HasColumnType("date");

			this.Property(t => t.VIN)
				.HasMaxLength(50)
				.IsUnicode(false);

			this.Property(t => t.MakeN)
				.HasMaxLength(50)
				.IsUnicode(true);

			this.Property(t => t.DepC)
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.DriverC)
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.AcquisitionD)
				.HasColumnType("date");

			this.Property(t => t.PartnerI)
				.HasMaxLength(1)
				.IsFixedLength()
				.IsUnicode(false);

			this.Property(t => t.GrossWeight)
				.HasPrecision(6, 1);

			this.Property(t => t.PartnerMainC)
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.PartnerSubC)
				.HasMaxLength(3)
				.IsUnicode(false);

			this.Property(t => t.Odometer)
				.HasPrecision(7, 1);

			this.Property(t => t.RemodelI)
				.HasMaxLength(1)
				.IsFixedLength()
				.IsUnicode(false);

			this.Property(t => t.IsActive)
				.HasMaxLength(1)
				.IsFixedLength()
				.IsUnicode(false);

			this.Property(t => t.ModelC)
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.DisusedD)
				.HasColumnType("date");

            this.Property(t => t.ModelYear)
                .HasMaxLength(4)
                .IsUnicode(false);

			this.Property(t => t.Status)
				.HasMaxLength(50);

			this.Property(t => t.StatusFromD)
				.HasColumnType("date");

			this.Property(t => t.StatusToD)
				.HasColumnType("date");

			this.Property(t => t.LossFuelRate)
				.HasPrecision(4, 2);

			this.Property(t => t.AssistantC)
				.HasMaxLength(5)
				.IsUnicode(false);

			// Table & Column Mappings
			this.ToTable("Truck_M");
			this.Property(t => t.TruckC).HasColumnName("TruckC");
			this.Property(t => t.RegisteredNo).HasColumnName("RegisteredNo");
			this.Property(t => t.RegisteredD).HasColumnName("RegisteredD");
			this.Property(t => t.VIN).HasColumnName("VIN");
			this.Property(t => t.MakeN).HasColumnName("MakeN");
			this.Property(t => t.DepC).HasColumnName("DepC");
			this.Property(t => t.DriverC).HasColumnName("DriverC");
			this.Property(t => t.AcquisitionD).HasColumnName("AcquisitionD");
			this.Property(t => t.PartnerI).HasColumnName("PartnerI");
			this.Property(t => t.GrossWeight).HasColumnName("GrossWeight");
			this.Property(t => t.PartnerMainC).HasColumnName("PartnerMainC");
			this.Property(t => t.PartnerSubC).HasColumnName("PartnerSubC");
			this.Property(t => t.Odometer).HasColumnName("Odometer");
			this.Property(t => t.ModelC).HasColumnName("ModelC");
			this.Property(t => t.RemodelI).HasColumnName("RemodelI");
			this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.ModelYear).HasColumnName("ModelYear");
			this.Property(t => t.LossFuelRate).HasColumnName("LossFuelRate");
			this.Property(t => t.AssistantC).HasColumnName("AssistantC");

		}
	}
}
