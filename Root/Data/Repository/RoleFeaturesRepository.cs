using System.Collections.Generic;
using Root.Data.Infrastructure;
using Root.Models;
using Root.Models.Authorization;

namespace Root.Data.Repository
{
	public class RoleFeaturesRepository : RepositoryBase<RoleFeatures>, IRoleFeaturesRepository
	{
		public RoleFeaturesRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
		public IEnumerable<AuthorizedRoleFeature> ExecSpToGetRoleFeatures(string query, params object[] parameters)
		{
			return DataContext.Database.SqlQuery<AuthorizedRoleFeature>(query, parameters);
		}
	}

	public interface IRoleFeaturesRepository : IRepository<RoleFeatures>
	{
		IEnumerable<AuthorizedRoleFeature> ExecSpToGetRoleFeatures(string query, params object[] parameters);
	}
}
