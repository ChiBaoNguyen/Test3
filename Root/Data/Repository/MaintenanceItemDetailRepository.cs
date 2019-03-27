using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class MaintenanceItemDetailRepository : RepositoryBase<MaintenanceItem_D>, IMaintenanceItemDetailRepository
    {
		public MaintenanceItemDetailRepository(IDatabaseFactory databaseFactory) 
            : base(databaseFactory)
        {
            
        }
    }
	public interface IMaintenanceItemDetailRepository : IRepository<MaintenanceItem_D>
    {
    }
}
