using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class TextResourceRepository : RepositoryBase<TextResource_D>, ITextResourceRepository
    {
        public TextResourceRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }

    public interface ITextResourceRepository : IRepository<TextResource_D>
    {
    }
}
