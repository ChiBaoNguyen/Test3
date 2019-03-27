using Root.Data.Infrastructure;
using Root.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Data.Repository
{
	public class ChatMessageRepository : RepositoryBase<ChatMessage>, IChatMessageRepository
	{
		public ChatMessageRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IChatMessageRepository : IRepository<ChatMessage>
	{
	}
}
