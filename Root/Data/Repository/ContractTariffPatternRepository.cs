using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class ContractTariffPatternRepository : RepositoryBase<ContractTariffPattern_M>, IContractTariffPatternRepository
	{
		public ContractTariffPatternRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IContractTariffPatternRepository : IRepository<ContractTariffPattern_M>
	{
	}
}
