using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class CustomerPaymentRepository : RepositoryBase<CustomerPayment_D>, ICustomerPaymentRepository
	{
		public CustomerPaymentRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ICustomerPaymentRepository : IRepository<CustomerPayment_D>
	{
	}
}
