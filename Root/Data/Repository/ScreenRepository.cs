using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class ScreenRepository : RepositoryBase<Screen_M>, IScreenRepository
    {
        public ScreenRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }

    public interface IScreenRepository : IRepository<Screen_M>
    {
    }
}
