using System;
using Root.Models;

namespace Root.Data.Infrastructure
{
    public interface IDatabaseFactory: IDisposable
    {
        SGTSVNDBContext Get();
    }
}