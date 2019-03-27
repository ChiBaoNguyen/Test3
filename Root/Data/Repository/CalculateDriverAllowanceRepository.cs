using System;
using System.Collections.Generic;
using System.Linq;
using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class CalculateDriverAllowanceRepository : RepositoryBase<CalculateDriverAllowance_M>, ICalculateDriverAllowanceRepository
	{
		public CalculateDriverAllowanceRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface ICalculateDriverAllowanceRepository : IRepository<CalculateDriverAllowance_M>
	{
	}
}
