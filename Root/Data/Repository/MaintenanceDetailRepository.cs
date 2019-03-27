using System.Collections.Generic;
using Root.Data.Infrastructure;
using Root.Models;
using Root.Models.Calendar;

namespace Root.Data.Repository
{
    public class MaintenanceDetailRepository : RepositoryBase<Maintenance_D>, IMaintenanceDetailRepository
    {
		public MaintenanceDetailRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }

		public IEnumerable<CalendarPlanItemForCounting> ExecSpToGetPlanForCounting(string query, params object[] parameters)
		{
			return DataContext.Database.SqlQuery<CalendarPlanItemForCounting>(query, parameters);
		}

		public IEnumerable<CalendarPlanItem> ExecExecSpToGetPlan(string query, params object[] parameters)
		{
			return DataContext.Database.SqlQuery<CalendarPlanItem>(query, parameters);
		}
    }

    public interface IMaintenanceDetailRepository : IRepository<Maintenance_D>
    {
	    IEnumerable<CalendarPlanItemForCounting> ExecSpToGetPlanForCounting(string query, params object[] parameters);
	    IEnumerable<CalendarPlanItem> ExecExecSpToGetPlan(string query, params object[] parameters);
    }
}
