using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class DriverRepository : RepositoryBase<Driver_M>, IDriverRepository
    {
		public DriverRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IDriverRepository : IRepository<Driver_M>
    {
    }
}
