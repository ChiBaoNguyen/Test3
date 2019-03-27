using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class AllowanceDetailRepository : RepositoryBase<Allowance_D>, IAllowanceDetailRepository
	{
		public AllowanceDetailRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IAllowanceDetailRepository : IRepository<Allowance_D>
	{
	}
}
