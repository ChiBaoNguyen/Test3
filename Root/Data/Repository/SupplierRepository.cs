using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class SupplierRepository : RepositoryBase<Supplier_M>, ISupplierRepository
	{
		public SupplierRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ISupplierRepository : IRepository<Supplier_M>
	{
	}
}
