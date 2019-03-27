using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class CustomerPricingLocationRepository : RepositoryBase<CustomerPricingLocation_D>, ICustomerPricingLocationRepository
	{
		public CustomerPricingLocationRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ICustomerPricingLocationRepository : IRepository<CustomerPricingLocation_D>
	{
	}
}
