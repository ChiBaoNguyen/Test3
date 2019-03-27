using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class EmployeeRepository : RepositoryBase<Employee_M>, IEmployeeRepository
    {
        public EmployeeRepository(IDatabaseFactory databaseFactory) 
            : base(databaseFactory)
        {
            
        }
    }
    public interface IEmployeeRepository : IRepository<Employee_M>
    {
    }
}
