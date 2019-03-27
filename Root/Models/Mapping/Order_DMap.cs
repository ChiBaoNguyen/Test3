using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization.Formatters;

namespace Root.Models.Mapping
{
	public class Order_DMap : EntityTypeConfiguration<Order_D>
	{
		public Order_DMap()
		{
			// Primary Key
			this.HasKey(t => new { t.OrderD, t.OrderNo, t.DetailNo });

			// Properties
			this.Property(t => t.OrderD)
				.IsRequired()
				.HasColumnType("date"); ;

			this.Property(t => t.OrderNo)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(7);

			this.Property(t => t.DetailNo)
				.IsRequired();

			this.Property(t => t.ContainerNo)
				.IsUnicode(false)
				.HasMaxLength(15);

			this.Property(t => t.ContainerSizeI)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.ContainerTypeC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.CommodityC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.CommodityN)
				.HasMaxLength(50);

			this.Property(t => t.EstimatedWeight)
				.HasPrecision(7, 1);

			this.Property(t => t.NetWeight)
				.HasPrecision(7, 1);

			this.Property(t => t.GrossWeight)
				.HasPrecision(7, 1);

			this.Property(t => t.UnitPrice)
				.IsRequired()
				.HasPrecision(10, 0);

			this.Property(t => t.TotalPrice)
				.HasPrecision(11, 0);

			this.Property(t => t.SealNo)
				.HasMaxLength(20);

			this.Property(t => t.Description)
				.HasMaxLength(255);

			this.Property(t => t.TrailerC)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.ActualLoadingD)
				.HasColumnType("date");

			this.Property(t => t.ActualDischargeD)
				.HasColumnType("date");

			this.Property(t => t.ActualPickupReturnD)
				.HasColumnType("date");

			this.Property(t => t.RevenueD)
				.HasColumnType("date");

			this.Property(t => t.Amount)
				.HasPrecision(12, 0);

			this.Property(t => t.TotalExpense)
				.HasPrecision(12, 0);

			this.Property(t => t.CustomerSurcharge)
				.HasPrecision(12, 0);

			this.Property(t => t.CustomerDiscount)
				.HasPrecision(12, 0);

			this.Property(t => t.PartnerAmount)
				.HasPrecision(12, 0);

			this.Property(t => t.PartnerExpense)
				.HasPrecision(12, 0);

			this.Property(t => t.PartnerSurcharge)
				.HasPrecision(12, 0);

			this.Property(t => t.PartnerDiscount)
				.HasPrecision(12, 0);

			this.Property(t => t.DetainAmount)
				.HasPrecision(12, 0);

			this.Property(t => t.TotalAmount)
				.HasPrecision(12, 0);

			this.Property(t => t.TaxAmount)
				.HasPrecision(12, 0);

			this.Property(t => t.TotalPartnerAmount)
				.HasPrecision(12, 0);

			this.Property(t => t.PartnerTaxAmount)
				.HasPrecision(12, 0);

			this.Property(t => t.TotalCost)
				.HasPrecision(12, 0);

			this.Property(t => t.TotalDriverAllowance)
				.HasPrecision(12, 0);

			this.Property(t => t.DetainDay);

			this.Property(t => t.TruckCLastDispatch)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.LocationDispatch1)
				.IsUnicode(true)
				.HasMaxLength(200);

			this.Property(t => t.LocationDispatch2)
				.IsUnicode(true)
				.HasMaxLength(200);

			this.Property(t => t.LocationDispatch3)
				.IsUnicode(true)
				.HasMaxLength(200);

			this.Property(t => t.CalculateByTon)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.TruckCReturn)
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.LastDDispatch)
				.HasColumnType("date");

			this.Property(t => t.TaxRate)
				.HasPrecision(3, 1);

			this.Property(t => t.TransportConfirmDescription)
				.HasMaxLength(255);
			// Table & Column Mappings
			this.ToTable("Order_D");
			this.Property(t => t.OrderD).HasColumnName("OrderD");
			this.Property(t => t.OrderNo).HasColumnName("OrderNo");
			this.Property(t => t.DetailNo).HasColumnName("DetailNo");
			this.Property(t => t.ContainerNo).HasColumnName("ContainerNo");
			this.Property(t => t.ContainerSizeI).HasColumnName("ContainerSizeI");
			this.Property(t => t.ContainerTypeC).HasColumnName("ContainerTypeC");
			this.Property(t => t.CommodityC).HasColumnName("CommodityC");
			this.Property(t => t.CommodityN).HasColumnName("CommodityN");
			this.Property(t => t.EstimatedWeight).HasColumnName("EstimatedWeight");
			this.Property(t => t.NetWeight).HasColumnName("NetWeight");
			this.Property(t => t.GrossWeight).HasColumnName("GrossWeight");
			this.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
			this.Property(t => t.SealNo).HasColumnName("SealNo");
			this.Property(t => t.Description).HasColumnName("Description");
			this.Property(t => t.TrailerC).HasColumnName("TrailerC");
			this.Property(t => t.ActualLoadingD).HasColumnName("ActualLoadingD");
			this.Property(t => t.ActualDischargeD).HasColumnName("ActualDischargeD");
			this.Property(t => t.ActualPickupReturnD).HasColumnName("ActualPickupReturnD");
			this.Property(t => t.RevenueD).HasColumnName("RevenueD");
			this.Property(t => t.Amount).HasColumnName("Amount");
			this.Property(t => t.TotalExpense).HasColumnName("TotalExpense");
			this.Property(t => t.CustomerSurcharge).HasColumnName("CustomerSurcharge");
			this.Property(t => t.CustomerDiscount).HasColumnName("CustomerDiscount");
			this.Property(t => t.PartnerAmount).HasColumnName("PartnerAmount");
			this.Property(t => t.PartnerExpense).HasColumnName("PartnerExpense");
			this.Property(t => t.PartnerSurcharge).HasColumnName("PartnerSurcharge");
			this.Property(t => t.PartnerDiscount).HasColumnName("PartnerDiscount");
			this.Property(t => t.DetainAmount).HasColumnName("DetainAmount");
			this.Property(t => t.TotalAmount).HasColumnName("TotalAmount");
			this.Property(t => t.TaxAmount).HasColumnName("TaxAmount");
			this.Property(t => t.TotalPartnerAmount).HasColumnName("TotalPartnerAmount");
			this.Property(t => t.PartnerTaxAmount).HasColumnName("PartnerTaxAmount");
			this.Property(t => t.TotalCost).HasColumnName("TotalCost");
			this.Property(t => t.TotalDriverAllowance).HasColumnName("TotalDriverAllowance");
			this.Property(t => t.DetainDay).HasColumnName("DetainDay");
			this.Property(t => t.CalculateByTon).HasColumnName("CalculateByTon");
			this.Property(t => t.TruckCLastDispatch).HasColumnName("TruckCLastDispatch");
			this.Property(t => t.LocationDispatch1).HasColumnName("LocationDispatch1");
			this.Property(t => t.LocationDispatch2).HasColumnName("LocationDispatch2");
			this.Property(t => t.LocationDispatch3).HasColumnName("LocationDispatch3");
			this.Property(t => t.TruckCReturn).HasColumnName("TruckCReturn");
			this.Property(t => t.LastDDispatch).HasColumnName("LastDDispatch");
			this.Property(t => t.TaxRate).HasColumnName("TaxRate");
			this.Property(t => t.TransportConfirmDescription).HasColumnName("TransportConfirmDescription");
		}
	}
}
