using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class DriverLicenseUpdateRepository : RepositoryBase<DriverLicenseUpdate_D>, IDriverLicenseUpdateRepository
	{
		public DriverLicenseUpdateRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IDriverLicenseUpdateRepository : IRepository<DriverLicenseUpdate_D>
	{
	}
}