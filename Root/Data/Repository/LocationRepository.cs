using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class LocationRepository : RepositoryBase<Location_M>, ILocationRepository
    {
        public LocationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
    public interface ILocationRepository : IRepository<Location_M>
    {
    }
}
