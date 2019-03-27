using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class LiabilitiesRepository : RepositoryBase<Liabilities_D>, ILiabilitiesRepository
	{
		public LiabilitiesRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ILiabilitiesRepository : IRepository<Liabilities_D>
	{
	}
}
