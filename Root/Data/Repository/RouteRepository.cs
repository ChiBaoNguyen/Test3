using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class RouteRepository : RepositoryBase<Route_H>, IRouteRepository
	{
		public RouteRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IRouteRepository : IRepository<Route_H>
	{
	}
}
