using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class PartnerSettlementRepository : RepositoryBase<PartnerSettlement_M>, IPartnerSettlementRepository
	{
		public PartnerSettlementRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IPartnerSettlementRepository : IRepository<PartnerSettlement_M>
	{
	}
}
