using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class CustomerSettlementRepository : RepositoryBase<CustomerSettlement_M>, ICustomerSettlementRepository
	{
		public CustomerSettlementRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ICustomerSettlementRepository : IRepository<CustomerSettlement_M>
	{
	}
}
