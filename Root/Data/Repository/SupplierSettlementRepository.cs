using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class SupplierSettlementRepository : RepositoryBase<SupplierSettlement_M>, ISupplierSettlementRepository
	{
        public SupplierSettlementRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
    public interface ISupplierSettlementRepository : IRepository<SupplierSettlement_M>
	{
	}
}
