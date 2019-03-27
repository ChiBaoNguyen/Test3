using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class PartnerPaymentRepository : RepositoryBase<PartnerPayment_D>, IPartnerPaymentRepository
	{
		public PartnerPaymentRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IPartnerPaymentRepository : IRepository<PartnerPayment_D>
	{
	}
}
