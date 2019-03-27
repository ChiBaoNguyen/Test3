using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class ModelRepository : RepositoryBase<Model_M>, IModelRepository
	{
		public ModelRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IModelRepository : IRepository<Model_M>
	{
	}
}