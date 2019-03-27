using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class CustomerProfitMarkupRepository : RepositoryBase<CustomerGrossProfit_M>, ICustomerGrossProfitRepository
	{
		public CustomerProfitMarkupRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ICustomerProfitMarkupRepository : IRepository<CustomerGrossProfit_M>
	{
	}
}
