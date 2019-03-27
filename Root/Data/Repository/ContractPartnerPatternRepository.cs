using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class ContractPartnerPatternRepository : RepositoryBase<ContractPartnerPattern_M>, IContractPartnerPatternRepository
	{
		public ContractPartnerPatternRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IContractPartnerPatternRepository : IRepository<ContractPartnerPattern_M>
	{
	}
}
