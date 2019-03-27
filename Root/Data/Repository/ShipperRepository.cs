using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class ShipperRepository : RepositoryBase<Shipper_M>, IShipperRepository
	{
		public ShipperRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IShipperRepository : IRepository<Shipper_M>
	{
	}
}
