using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class LocationDetailRepository : RepositoryBase<LocationDetail_M>,
		ILocationDetailRepository
	{
		public LocationDetailRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}

	public interface ILocationDetailRepository : IRepository<LocationDetail_M>
	{
	}
}