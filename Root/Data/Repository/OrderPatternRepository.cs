using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class OrderPatternRepository : RepositoryBase<OrderPattern_M>, IOrderPatternRepository
    {
        public OrderPatternRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
    public interface IOrderPatternRepository : IRepository<OrderPattern_M>
    {
    }
}
