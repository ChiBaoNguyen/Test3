using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class OperationRepository : RepositoryBase<Operation_M>, IOperationRepository
	{
		public OperationRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IOperationRepository : IRepository<Operation_M>
	{
	}
}