using System;
using System.Collections.Generic;
using System.Reflection;
using Aster.Common.Data.Core.Mapper;
using Aster.Common.Data.Core.Sessions;
using Aster.Common.Data.Core.Sql;

namespace Aster.Common.Data.Core.Configuration
{
    public interface IDapperConfiguration
    {
        IDapperConfiguration WithDefaultConnectionStringNamed(string name);
        IDapperConfiguration FromAssembly(string name);
        IDapperConfiguration FromAssemblies(IEnumerable<Assembly> assemblies);
        IDapperConfiguration FromAssemblyContaining(Type assemblyType);
        IDapperConfiguration UseClassMapper(Type typeOfMapper);
        IDapperConfiguration UseSqlDialect(ISqlDialect dialect);
        IDapperConfiguration Build();

        List<Assembly> Assemblies { get; }

        Type DefaultMapper { get; }

        IDictionary<string, string> AllConnectionStrings { get; }

        string DefaultConnectionStringName { get; }
        ISqlDialect Dialect { get; }
        IClassMapper GetMap(Type entityType);
        IClassMapper GetMap<T>() where T : class;
        Guid GetNextGuid();
    }
}