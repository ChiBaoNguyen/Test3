using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class TrailerRepository : RepositoryBase<Trailer_M>, ITrailerRepository
	{
		public TrailerRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ITrailerRepository : IRepository<Trailer_M>
	{
	}
}
