using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class InspectionPlanDetailRepository : RepositoryBase<InspectionPlan_D>, IInspectionPlanDetailRepository
    {
		public InspectionPlanDetailRepository(IDatabaseFactory databaseFactory) 
            : base(databaseFactory)
        {
            
        }
    }
	public interface IInspectionPlanDetailRepository : IRepository<InspectionPlan_D>
    {
    }
}
