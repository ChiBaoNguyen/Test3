using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class RouteDetailRepository : RepositoryBase<Route_D>, IRouteDetailRepository
	{
		public RouteDetailRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IRouteDetailRepository : IRepository<Route_D>
	{
	}
}
