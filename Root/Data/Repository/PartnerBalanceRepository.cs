using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class PartnerBalanceRepository : RepositoryBase<PartnerBalance_D>, IPartnerBalanceRepository
	{
		public PartnerBalanceRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IPartnerBalanceRepository : IRepository<PartnerBalance_D>
	{
	}
}
