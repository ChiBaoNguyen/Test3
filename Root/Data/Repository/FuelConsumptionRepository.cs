using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class FuelConsumptionRepository : RepositoryBase<FuelConsumption_M>, IFuelConsumptionRepository
	{
		public FuelConsumptionRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IFuelConsumptionRepository : IRepository<FuelConsumption_M>
	{
	}
}
