using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class CustomerPricingRepository : RepositoryBase<CustomerPricing_H>, ICustomerPricingRepository
	{
		public CustomerPricingRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ICustomerPricingRepository : IRepository<CustomerPricing_H>
	{
	}
}
