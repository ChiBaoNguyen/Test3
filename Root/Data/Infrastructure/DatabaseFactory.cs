using Root.Models;

namespace Root.Data.Infrastructure
{
    public class DatabaseFactory : Disposable, IDatabaseFactory
    {
        private SGTSVNDBContext _dataContext;

        public SGTSVNDBContext Get()
        {
            return _dataContext ?? (_dataContext = new SGTSVNDBContext());
        }

        protected override void DisposeCore()
        {
            if (_dataContext != null)
            {
                _dataContext.Dispose();
            }
        }
    }
}
