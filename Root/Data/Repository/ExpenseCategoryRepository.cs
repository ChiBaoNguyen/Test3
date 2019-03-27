using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class ExpenseCategoryRepository : RepositoryBase<ExpenseCategory_M>, IExpenseCategoryRepository
    {
        public ExpenseCategoryRepository(IDatabaseFactory databaseFactory) 
            : base(databaseFactory)
        {
            
        }
    }
    public interface IExpenseCategoryRepository : IRepository<ExpenseCategory_M>
    {
    }
}
