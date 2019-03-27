using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class TruckRepository : RepositoryBase<Truck_M>, ITruckRepository
	{
		public TruckRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ITruckRepository : IRepository<Truck_M>
	{
	}
}
