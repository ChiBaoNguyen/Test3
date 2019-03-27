using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class CustomerPricingDetailRepository : RepositoryBase<CustomerPricing_D>, ICustomerPricingDetailRepository
	{
		public CustomerPricingDetailRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ICustomerPricingDetailRepository : IRepository<CustomerPricing_D>
	{
	}
}
