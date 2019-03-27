using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class FuelConsumptionDetailRepository : RepositoryBase<FuelConsumption_D>, IFuelConsumptionDetailRepository
	{
		public FuelConsumptionDetailRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IFuelConsumptionDetailRepository : IRepository<FuelConsumption_D>
	{
	}
}
