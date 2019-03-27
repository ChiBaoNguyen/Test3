using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class CustomerRepository : RepositoryBase<Customer_M>, ICustomerRepository
    {
        public CustomerRepository(IDatabaseFactory databaseFactory) 
            : base(databaseFactory)
        {
            
        }
    }
    public interface ICustomerRepository : IRepository<Customer_M>
    {
    }
}
