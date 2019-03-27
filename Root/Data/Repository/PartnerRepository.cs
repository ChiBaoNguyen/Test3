using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class PartnerRepository : RepositoryBase<Partner_M>, IPartnerRepository
	{
		public PartnerRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IPartnerRepository : IRepository<Partner_M>
	{
	}
}
