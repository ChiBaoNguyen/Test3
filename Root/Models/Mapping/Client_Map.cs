using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Mapping
{
	public class Client_Map : EntityTypeConfiguration<Client>
	{
		public Client_Map()
		{
			// Primary Key
			this.HasKey(t => t.Id);

			// Properties
			this.Property(t => t.Secret)
				.IsRequired();

			this.Property(t => t.Name)
				.HasMaxLength(100);

			this.Property(t => t.AllowedOrigin)
				.HasMaxLength(100);

			// Table & Column Mappings
			this.ToTable("Clients");
			this.Property(t => t.Id).HasColumnName("Id");
			this.Property(t => t.Secret).HasColumnName("Secret");
			this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.ApplicationType).HasColumnName("ApplicationType");
			this.Property(t => t.Active).HasColumnName("Active");
			this.Property(t => t.RefreshTokenLifeTime).HasColumnName("RefreshTokenLifeTime");
			this.Property(t => t.AllowedOrigin).HasColumnName("AllowedOrigin");
		}
	}
}
