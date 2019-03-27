using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class TransportDistanceRepository : RepositoryBase<TransportDistance_M>, ITransportDistanceRepository
	{
		public TransportDistanceRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ITransportDistanceRepository : IRepository<TransportDistance_M>
	{
	}
}
