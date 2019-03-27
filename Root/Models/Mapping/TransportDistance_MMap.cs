using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class TransportDistance_MMap : EntityTypeConfiguration<TransportDistance_M>
	{
		public TransportDistance_MMap()
		{
			// Primary Key
			this.HasKey(t => new { t.TransportDistanceC });

			// Properties
			this.Property(t => t.TransportDistanceC)
				.IsRequired()
				.HasMaxLength(5)
				.IsUnicode(false);

			this.Property(t => t.FromAreaC)
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.ToAreaC)
				.IsUnicode(false)
				.HasMaxLength(7);

			this.Property(t => t.WayType)
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.Km)
				.HasPrecision(10, 0);

			// Table & Column Mappings
			this.ToTable("TransportDistance_M");

			this.Property(t => t.TransportDistanceC).HasColumnName("TransportDistanceC");
			this.Property(t => t.FromAreaC).HasColumnName("FromAreaC");
			this.Property(t => t.ToAreaC).HasColumnName("ToAreaC");
			this.Property(t => t.WayType).HasColumnName("WayType");
			this.Property(t => t.Km).HasColumnName("Km");
		}
	}
}
