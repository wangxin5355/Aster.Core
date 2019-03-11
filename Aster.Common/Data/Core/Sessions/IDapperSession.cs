using System.Data;

namespace Aster.Common.Data.Core.Sessions
{
    public interface IDapperSession : IDbConnection
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; set; }
    }
}
