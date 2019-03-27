using System.Data.Entity;
using System.Linq;
using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class OrderRepository : RepositoryBase<Order_H>, IOrderRepository
	{
		private readonly IDbSet<Order_H> _dbset;
		public OrderRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{
			_dbset = DataContext.Set<Order_H>();
		}
		public Order_H FindMax()
		{
			return _dbset.Max();
		}
	}
	public interface IOrderRepository : IRepository<Order_H>
	{
		Order_H FindMax();
	}
}
