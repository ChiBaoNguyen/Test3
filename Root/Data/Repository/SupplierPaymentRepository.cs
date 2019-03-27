using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class SupplierPaymentRepository : RepositoryBase<SupplierPayment_D>, ISupplierPaymentRepository
	{
		public SupplierPaymentRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ISupplierPaymentRepository : IRepository<SupplierPayment_D>
	{
	}
}
