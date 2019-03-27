using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class ExpenseDetailRepository : RepositoryBase<Expense_D>, IExpenseDetailRepository
	{
		public ExpenseDetailRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IExpenseDetailRepository : IRepository<Expense_D>
	{
	}
}
