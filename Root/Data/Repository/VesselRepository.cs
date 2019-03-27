using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class VesselRepository : RepositoryBase<Vessel_M>, IVesselRepository
    {
        public VesselRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
    public interface IVesselRepository : IRepository<Vessel_M>
    {
    }
}
