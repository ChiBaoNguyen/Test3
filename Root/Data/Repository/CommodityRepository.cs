using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class CommodityRepository : RepositoryBase<Commodity_M>, ICommodityRepository
    {
        public CommodityRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
    public interface ICommodityRepository : IRepository<Commodity_M>
    {
    }
}
