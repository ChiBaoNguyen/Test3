using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class FixedExpenseRepository : RepositoryBase<FixedExpense_D>, IFixedExpenseRepository
	{
		public FixedExpenseRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IFixedExpenseRepository : IRepository<FixedExpense_D>
	{
	}
}
