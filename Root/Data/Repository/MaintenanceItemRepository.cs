using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class MaintenanceItemRepository : RepositoryBase<MaintenanceItem_M>, IMaintenanceItemRepository
	{
		public MaintenanceItemRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IMaintenanceItemRepository : IRepository<MaintenanceItem_M>
	{
	}
}