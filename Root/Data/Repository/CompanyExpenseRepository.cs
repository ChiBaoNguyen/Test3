using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{

	public class CompanyExpenseRepository : RepositoryBase<CompanyExpense_D>, ICompanyExpenseRepository
	{
		public CompanyExpenseRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{
		}
	}
	public interface ICompanyExpenseRepository : IRepository<CompanyExpense_D>
	{
	}
}
