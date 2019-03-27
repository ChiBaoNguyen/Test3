using System.Data.Entity;
using System.Linq;
using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class DispatchRepository : RepositoryBase<Dispatch_D>, IDispatchRepository
	{
		private readonly IDbSet<Dispatch_D> _dbset;
		public DispatchRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{
			_dbset = DataContext.Set<Dispatch_D>();
		}
		public Dispatch_D FindMax()
		{
			return _dbset.Max();
		}
	}
	public interface IDispatchRepository : IRepository<Dispatch_D>
	{
		Dispatch_D FindMax();
	}
}
