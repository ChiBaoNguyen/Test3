using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class MaintenancePlanDetailRepository : RepositoryBase<MaintenancePlan_D>, IMaintenancePlanDetailRepository
    {
		public MaintenancePlanDetailRepository(IDatabaseFactory databaseFactory) 
            : base(databaseFactory)
        {
            
        }
    }
	public interface IMaintenancePlanDetailRepository : IRepository<MaintenancePlan_D>
    {
    }
}
