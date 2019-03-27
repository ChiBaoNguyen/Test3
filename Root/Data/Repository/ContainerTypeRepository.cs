using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class ContainerTypeRepository : RepositoryBase<ContainerType_M>, IContainerTypeRepository
    {
        public ContainerTypeRepository(IDatabaseFactory databaseFactory) 
            : base(databaseFactory)
        {
            
        }
    }
    public interface IContainerTypeRepository : IRepository<ContainerType_M>
    {
    }
}
