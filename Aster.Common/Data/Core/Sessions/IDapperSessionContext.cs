using System;
using System.Data;

namespace Aster.Common.Data.Core.Sessions
{
    public interface IDapperSessionContext : IDisposable
    {
        IDapperSession GetSession(Type entityType);
        IDapperSession GetSession<T>() where T : class, IEntity;
        IDapperSession GetSession(string connectionStringName);
        void RequireNew();
        void RequireNew(IsolationLevel level);
        void Cancel();
    }
}
