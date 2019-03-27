using Microsoft.AspNet.Identity.EntityFramework;
using Root.Data.Infrastructure;

namespace Root.Data.Repository
{
	public class UserRolesRepository : RepositoryBase<IdentityUserRole>, IUserRolesRepository
	{
		public UserRolesRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{
		}
	}
	public interface IUserRolesRepository : IRepository<IdentityUserRole>
	{
	}
}