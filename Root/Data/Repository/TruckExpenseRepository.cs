using System.Data.Entity;
using System.Linq;
using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class TruckExpenseRepository : RepositoryBase<TruckExpense_D>, ITruckExpenseRepository
	{
		private readonly IDbSet<TruckExpense_D> _dbset;
		public TruckExpenseRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{
			_dbset = DataContext.Set<TruckExpense_D>();
		}
		public TruckExpense_D FindMax()
		{
			return _dbset.Max();
		}
	}
	public interface ITruckExpenseRepository : IRepository<TruckExpense_D>
	{
		TruckExpense_D FindMax();
	}
}
