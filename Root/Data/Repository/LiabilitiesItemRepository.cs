using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class LiabilitiesItemRepository : RepositoryBase<LiabilitiesItem_D>, ILiabilitiesItemRepository
	{
		public LiabilitiesItemRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ILiabilitiesItemRepository : IRepository<LiabilitiesItem_D>
	{
	}
}
