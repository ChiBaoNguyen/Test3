using Root.Data.Infrastructure;
using Root.Models;

namespace Root.Data.Repository
{
	public class InspectionRepository : RepositoryBase<Inspection_M>, IInspectionRepository
	{
		public InspectionRepository(IDatabaseFactory databaseFactory)
			: base(databaseFactory)
		{

		}
	}
	public interface IInspectionRepository : IRepository<Inspection_M>
	{
	}
}