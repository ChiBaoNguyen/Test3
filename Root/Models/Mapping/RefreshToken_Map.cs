using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class RefreshToken_Map : EntityTypeConfiguration<RefreshToken>
	{
		public RefreshToken_Map()
		{
			// Primary Key
			this.HasKey(t => t.Id);

			// Properties
			this.Property(t => t.Subject)
				.HasMaxLength(50)
				.IsRequired();

			this.Property(t => t.ProtectedTicket)
				.IsRequired();

			// Table & Column Mappings
			this.ToTable("RefreshTokens");
			this.Property(t => t.Id).HasColumnName("Id");
			this.Property(t => t.Subject).HasColumnName("Subject");
			this.Property(t => t.ClientId).HasColumnName("ClientId");
			this.Property(t => t.IssuedUtc).HasColumnName("IssuedUtc");
			this.Property(t => t.ExpiresUtc).HasColumnName("ExpiresUtc");
			this.Property(t => t.ProtectedTicket).HasColumnName("ProtectedTicket");
		}
	}
}
