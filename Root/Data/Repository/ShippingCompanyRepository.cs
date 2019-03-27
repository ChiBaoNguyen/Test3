using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
    public class ShippingCompanyRepository : RepositoryBase<ShippingCompany_M>, IShippingCompanyRepository
    {
        public ShippingCompanyRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {

        }
    }
    public interface IShippingCompanyRepository : IRepository<ShippingCompany_M>
    {
    }
}
