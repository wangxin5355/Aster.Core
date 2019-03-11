using System;

namespace Aster.Common.Data.Core.Attributes
{
    public class DataContextAttribute : Attribute
    {
        public string ConnectionStringName { get; private set; }

        public DataContextAttribute(string connectionStringName) { ConnectionStringName = connectionStringName; }
    }
}
