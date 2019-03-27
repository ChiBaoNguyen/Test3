using System.Collections.Generic;
using Root.Data.Infrastructure;
using Root.Models;
using Root.Models.Calendar;

namespace Root.Data.Repository
{
	public class InspectionDetailRepository : RepositoryBase<Inspection_D>, IInspectionDetailRepository
    {
		public InspectionDetailRepository(IDatabaseFactory databaseFactory) 
            : base(databaseFactory)
        {
            
        }
		public IEnumerable<CalendarPlanItemForCounting> ExecSpToGetPlanForCounting(string query, params object[] parameters)
		{
			return DataContext.Database.SqlQuery<CalendarPlanItemForCounting>(query, parameters);
		}

		public IEnumerable<CalendarPlanItem> ExecSpToGetPlan(string query, params object[] parameters)
		{
			return DataContext.Database.SqlQuery<CalendarPlanItem>(query, parameters);
		}
    }
	public interface IInspectionDetailRepository : IRepository<Inspection_D>
    {
		IEnumerable<CalendarPlanItemForCounting> ExecSpToGetPlanForCounting(string query, params object[] parameters);
		IEnumerable<CalendarPlanItem> ExecSpToGetPlan(string query, params object[] parameters);
    }
}
