using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace Root.Models.Mapping
{
	public class ChatMessageMap : EntityTypeConfiguration<ChatMessage>
	{
		public ChatMessageMap()
		{
			// Primary Key
			this.HasKey(t => t.Id);

			// Properties
			this.Property(t => t.Id)
				.IsRequired()
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

			this.Property(t => t.UserId)
				.IsUnicode(false)
				.HasMaxLength(100);

			this.Property(t => t.DriverUserId)
				.IsUnicode(false)
				.HasMaxLength(100);

			// Table & Column Mappings
			this.ToTable("ChatMessage");
		}
	}
}
