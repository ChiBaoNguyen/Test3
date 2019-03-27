using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class CustomerBalanceRepository : RepositoryBase<CustomerBalance_D>, ICustomerBalanceRepository
	{
		public CustomerBalanceRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ICustomerBalanceRepository : IRepository<CustomerBalance_D>
	{
	}
}
