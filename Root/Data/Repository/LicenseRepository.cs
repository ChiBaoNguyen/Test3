using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class LicenseRepository : RepositoryBase<License_M>, ILicenseRepository
	{
		public LicenseRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{
			
		}
	}
	public interface ILicenseRepository : IRepository<License_M>
	{
	}
}
