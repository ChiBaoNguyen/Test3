using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Root.Models.Mapping
{
	public class Operation_MMap : EntityTypeConfiguration<Operation_M>
	{
		public Operation_MMap()
		{
			// Primary Key
			this.HasKey(t => t.OperationC);

			// Properties
			this.Property(t => t.OperationC)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(5);

			this.Property(t => t.OrderTypeI)
				.IsRequired()
				.IsUnicode(false)
				.IsFixedLength()
				.HasMaxLength(1);

			this.Property(t => t.DisplayLineNo);

			this.Property(t => t.OperationN)
				.IsUnicode(true)
				.HasMaxLength(3);

			this.Property(t => t.Description)
				.HasMaxLength(100);

			// Table & Column Mappings
			this.ToTable("Operation_M");
			this.Property(t => t.OperationC).HasColumnName("OperationC");
			this.Property(t => t.OrderTypeI).HasColumnName("OrderTypeI");
			this.Property(t => t.DisplayLineNo).HasColumnName("DisplayLineNo");
			this.Property(t => t.OperationN).HasColumnName("OperationN");
			this.Property(t => t.Description).HasColumnName("Description");
		}
	}
}