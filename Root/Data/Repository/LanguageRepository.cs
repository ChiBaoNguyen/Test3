using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class LanguageRepository : RepositoryBase<Language_M>, ILanguageRepository
    {
        public LanguageRepository(IDatabaseFactory databaseFactory) 
            : base(databaseFactory)
        {
            
        }
    }

    public interface ILanguageRepository : IRepository<Language_M>
    {
    }
}
