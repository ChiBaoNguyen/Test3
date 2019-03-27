using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class CustomerGrossProfitRepository : RepositoryBase<CustomerGrossProfit_M>, ICustomerGrossProfitRepository
	{
		public CustomerGrossProfitRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ICustomerGrossProfitRepository : IRepository<CustomerGrossProfit_M>
	{
	}
}
