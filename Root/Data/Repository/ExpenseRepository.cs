using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class ExpenseRepository : RepositoryBase<Expense_M>, IExpenseRepository
	{
		public ExpenseRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IExpenseRepository : IRepository<Expense_M>
	{
	}
}
