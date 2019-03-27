using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class SupplierBalanceRepository : RepositoryBase<SupplierBalance_D>, ISupplierBalanceRepository
	{
		public SupplierBalanceRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ISupplierBalanceRepository : IRepository<SupplierBalance_D>
	{
	}
}
