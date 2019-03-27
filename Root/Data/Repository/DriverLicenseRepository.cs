using System.Collections.Generic;
using Root.Data.Infrastructure;
using Root.Models;
using Root.Models.Calendar;

namespace Root.Data.Repository
{
	public class DriverLicenseRepository : RepositoryBase<DriverLicense_M>, IDriverLicenseRepository
	{
		public DriverLicenseRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
		public IEnumerable<CalendarPlanItemForCounting> ExecSpToGetPlanForCounting(string query, params object[] parameters)
		{
			return DataContext.Database.SqlQuery<CalendarPlanItemForCounting>(query, parameters);
		}
	}
	public interface IDriverLicenseRepository : IRepository<DriverLicense_M>
	{
		IEnumerable<CalendarPlanItemForCounting> ExecSpToGetPlanForCounting(string query, params object[] parameters);
	}
}