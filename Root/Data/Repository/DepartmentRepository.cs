using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class DepartmentRepository : RepositoryBase<Department_M>, IDepartmentRepository
    {
        public DepartmentRepository(IDatabaseFactory databaseFactory) 
            : base(databaseFactory)
        {
            
        }
    }
    public interface IDepartmentRepository : IRepository<Department_M>
    {
    }
}
