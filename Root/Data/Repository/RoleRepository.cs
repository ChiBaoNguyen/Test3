using Microsoft.AspNet.Identity.EntityFramework;
using Root.Data.Infrastructure;

namespace Root.Data.Repository
{
	public class RoleRepository : RepositoryBase<IdentityRole>, IRoleRepository
	{
		public RoleRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{
		}
	}
	public interface IRoleRepository : IRepository<IdentityRole>
	{
	}
}