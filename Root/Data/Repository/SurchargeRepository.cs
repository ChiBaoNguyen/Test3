using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class SurchargeRepository : RepositoryBase<Surcharge_D>, ISurchargeRepository
	{
		public SurchargeRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ISurchargeRepository : IRepository<Surcharge_D>
	{
	}
}
