using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class GpsLocationRepository : RepositoryBase<GpsLocation_D>, IGpsLocationRepository
    {
        public GpsLocationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IGpsLocationRepository : IRepository<GpsLocation_D>
    {
    }
}
