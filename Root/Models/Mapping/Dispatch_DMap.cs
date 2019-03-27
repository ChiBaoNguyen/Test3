using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class Dispatch_DMap : EntityTypeConfiguration<Dispatch_D>
	{
		public Dispatch_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.OrderD, t.OrderNo, t.DetailNo, t.DispatchNo });

			// Properties
			this.Property(t => t.OrderD)
				.IsRequired()
				.HasColumnType("date");

			this.Property(t => t.OrderNo)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(7);

			this.Property(t => t.DetailNo)
				.IsRequired();

			this.Property(t => t.DispatchNo)
				.IsRequired();

			this.Property(t => t.TransportD)
				.HasColumnType("date");

			this.Property(t => t.DispatchI)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.TruckC)
				.IsUnicode(true)
				.HasMaxLength(5);

			this.Property(t => t.RegisteredNo)
				.HasMaxLength(20)
				.IsUnicode(false);

			this.Property(t => t.DriverC)
				.IsUnicode(true)
				.HasMaxLength(5);

			this.Property(t => t.PartnerMainC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.PartnerSubC)
				.IsUnicode(false)
				.HasMaxLength(3);

			this.Property(t => t.OrderTypeI)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.ContainerStatus)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.DispatchStatus)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.Location1C)
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.Location1N)
				.IsUnicode(true)
				.HasMaxLength(200);

			this.Property(t => t.Location2C)
				.HasMaxLength(7);

			this.Property(t => t.Location2N)
				.IsUnicode(true)
				.HasMaxLength(200);

			this.Property(t => t.Location3C)
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.Location3N)
				.IsUnicode(true)
				.HasMaxLength(200);

			this.Property(t => t.Operation1C)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.Operation2C)
				.IsUnicode(false)
				.HasMaxLength(5); 

			this.Property(t => t.Operation3C)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.TransportFee)
				.HasPrecision(10, 0);

			this.Property(t => t.AllowanceOfDriver)
				.HasPrecision(10, 0);

			this.Property(t => t.PartnerFee)
				.HasPrecision(10, 0);

			this.Property(t => t.IncludedExpense)
				.HasPrecision(10, 0);

			this.Property(t => t.DriverAllowance)
				.HasPrecision(10, 0);

			this.Property(t => t.Expense)
				.HasPrecision(10, 0);

			this.Property(t => t.PartnerExpense)
				.HasPrecision(10, 0);

			this.Property(t => t.PartnerSurcharge)
				.HasPrecision(10, 0);

			this.Property(t => t.PartnerDiscount)
				.HasPrecision(10, 0);

			this.Property(t => t.PartnerTaxAmount)
				.HasPrecision(10, 0);

			this.Property(t => t.Location1DT)
				.HasColumnType("datetime2");

			this.Property(t => t.Location2DT)
				.HasColumnType("datetime2");

			this.Property(t => t.Location3DT)
				.HasColumnType("datetime2");

			this.Property(t => t.InvoiceD)
				.HasColumnType("date");

			this.Property(t => t.TransportDepC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.ApproximateDistance)
				.HasPrecision(10, 0);

			this.Property(t => t.ActualDistance)
				.HasPrecision(10, 0);

			this.Property(t => t.FuelConsumption)
				.HasPrecision(10, 2);

			this.Property(t => t.ActualFuel)
				.HasPrecision(10, 2);

			this.Property(t => t.DetainDay);

			this.Property(t => t.InvoiceStatus)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.Description)
				.HasMaxLength(255);

			this.Property(t => t.InstructionNo)
				.HasMaxLength(50);

			this.Property(t => t.TotalKm)
				.HasPrecision(10, 0);

			this.Property(t => t.TotalFuel)
				.HasPrecision(10, 2);

			this.Property(t => t.LossFuelRate)
				.HasPrecision(4, 2);

			this.Property(t => t.VirtualDataNoGoods)
				.HasPrecision(10, 0);

			this.Property(t => t.VirtualDataHaveGoods)
				.HasPrecision(10, 0);

			this.Property(t => t.WayType)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);
            this.Property(t => t.OrderImageKey);
            this.Property(t => t.ImageCount);

			this.Property(t => t.AssistantC)
				.IsUnicode(true)
				.HasMaxLength(5);
			// Table & Column Mappings
			this.ToTable("Dispatch_D");
			this.Property(t => t.OrderD).HasColumnName("OrderD");
			this.Property(t => t.OrderNo).HasColumnName("OrderNo");
			this.Property(t => t.DetailNo).HasColumnName("DetailNo");
			this.Property(t => t.DispatchNo).HasColumnName("DispatchNo");
			this.Property(t => t.TransportD).HasColumnName("TransportD");
			this.Property(t => t.DispatchI).HasColumnName("DispatchI");
			this.Property(t => t.TruckC).HasColumnName("TruckC");
			this.Property(t => t.RegisteredNo).HasColumnName("RegisteredNo");
			this.Property(t => t.DriverC).HasColumnName("DriverC");
			this.Property(t => t.PartnerMainC).HasColumnName("PartnerMainC");
			this.Property(t => t.PartnerSubC).HasColumnName("PartnerSubC");
			this.Property(t => t.OrderTypeI).HasColumnName("OrderTypeI");
			this.Property(t => t.DispatchOrder).HasColumnName("DispatchOrder");
			this.Property(t => t.ContainerStatus).HasColumnName("ContainerStatus");
			this.Property(t => t.DispatchStatus).HasColumnName("DispatchStatus");
			this.Property(t => t.Location1C).HasColumnName("Location1C");
			this.Property(t => t.Location1N).HasColumnName("Location1N");
			this.Property(t => t.Location1DT).HasColumnName("Location1DT");
			this.Property(t => t.Location2C).HasColumnName("Location2C");
			this.Property(t => t.Location2N).HasColumnName("Location2N");
			this.Property(t => t.Location2DT).HasColumnName("Location2DT");
			this.Property(t => t.Location3C).HasColumnName("Location3C");
			this.Property(t => t.Location3N).HasColumnName("Location3N");
			this.Property(t => t.Location3DT).HasColumnName("Location3DT");
			this.Property(t => t.Operation1C).HasColumnName("Operation1C");
			this.Property(t => t.IsTransported1).HasColumnName("IsTransported1");
			this.Property(t => t.Operation2C).HasColumnName("Operation2C");
			this.Property(t => t.IsTransported2).HasColumnName("IsTransported2");
			this.Property(t => t.Operation3C).HasColumnName("Operation3C");
			this.Property(t => t.IsTransported3).HasColumnName("IsTransported3");
			this.Property(t => t.CountTransport).HasColumnName("CountTransport");
			this.Property(t => t.TransportFee).HasColumnName("TransportFee");
			this.Property(t => t.PartnerFee).HasColumnName("PartnerFee");
			this.Property(t => t.IncludedExpense).HasColumnName("IncludedExpense");
			this.Property(t => t.DriverAllowance).HasColumnName("DriverAllowance");
			this.Property(t => t.Expense).HasColumnName("Expense");
			this.Property(t => t.PartnerExpense).HasColumnName("PartnerExpense");
			this.Property(t => t.PartnerSurcharge).HasColumnName("PartnerSurcharge");
			this.Property(t => t.PartnerDiscount).HasColumnName("PartnerDiscount");
            this.Property(t => t.PartnerTaxAmount).HasColumnName("PartnerTaxAmount");
			this.Property(t => t.InvoiceD).HasColumnName("InvoiceD");
			this.Property(t => t.TransportDepC).HasColumnName("TransportDepC");
			this.Property(t => t.ApproximateDistance).HasColumnName("ApproximateDistance");
			this.Property(t => t.ActualDistance).HasColumnName("ActualDistance");
			this.Property(t => t.FuelConsumption).HasColumnName("FuelConsumption");
			this.Property(t => t.ActualFuel).HasColumnName("ActualFuel");
			this.Property(t => t.DetainDay).HasColumnName("DetainDay");
			this.Property(t => t.InvoiceStatus).HasColumnName("InvoiceStatus");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.InstructionNo).HasColumnName("InstructionNo");
			this.Property(t => t.AllowanceOfDriver).HasColumnName("AllowanceOfDriver");
			this.Property(t => t.TotalKm).HasColumnName("TotalKm");
			this.Property(t => t.TotalFuel).HasColumnName("TotalFuel");
			this.Property(t => t.LossFuelRate).HasColumnName("LossFuelRate");
			this.Property(t => t.WayType).HasColumnName("WayType");
            this.Property(t => t.OrderImageKey).HasColumnName("OrderImageKey");
            this.Property(t => t.ImageCount).HasColumnName("ImageCount");
			this.Property(t => t.AssistantC).HasColumnName("AssistantC");

			this.Property(t => t.VirtualDataNoGoods).HasColumnName("VirtualDataNoGoods");
			this.Property(t => t.VirtualDataHaveGoods).HasColumnName("VirtualDataHaveGoods");
		}
	}
}
