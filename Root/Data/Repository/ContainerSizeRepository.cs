using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class ContainerSizeRepository : RepositoryBase<ContainerSize_M>, IContainerSizeRepository
	{
		public ContainerSizeRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IContainerSizeRepository : IRepository<ContainerSize_M>
	{
	}
}
