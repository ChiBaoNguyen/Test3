using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class DriverAllowanceRepository : RepositoryBase<DriverAllowance_M>, IDriverAllowanceRepository
	{
		public DriverAllowanceRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IDriverAllowanceRepository : IRepository<DriverAllowance_M>
	{
	}
}
