using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class FeaturesRepository : RepositoryBase<Feature_M>, IFeatureRepository
	{
		public FeaturesRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IFeatureRepository : IRepository<Feature_M>
	{
	}
}
