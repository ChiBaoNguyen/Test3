using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class ContainerRepository : RepositoryBase<Order_D>, IContainerRepository
    {
        public ContainerRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
    public interface IContainerRepository : IRepository<Order_D>
    {
    }
}
