using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class SurchargeDetailRepository : RepositoryBase<Surcharge_D>, ISurchargeDetailRepository
	{
		public SurchargeDetailRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ISurchargeDetailRepository : IRepository<Surcharge_D>
	{
	}
}
