using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class BasicRepository : RepositoryBase<Basic_S>, IBasicRepository
    {
		public BasicRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
    public interface IBasicRepository : IRepository<Basic_S>
    {
    }
}
