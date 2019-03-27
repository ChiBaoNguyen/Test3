using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class PartnerContractTariffPatternRepository : RepositoryBase<PartnerContractTariffPattern_M>, IPartnerContractTariffPatternRepository
	{
        public PartnerContractTariffPatternRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
    public interface IPartnerContractTariffPatternRepository : IRepository<PartnerContractTariffPattern_M>
	{
	}
}
