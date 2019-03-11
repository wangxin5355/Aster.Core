using Aster.Common.Data.Core.Mapper;
using Aster.Common.Data.Core.Sql;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Aster.Common.Data.Core.Configuration
{
    public class DapperConfiguration : IDapperConfiguration
    {
        private readonly ConcurrentDictionary<Type, IClassMapper> _classMaps = new ConcurrentDictionary<Type, IClassMapper>();
        private readonly ILoggerFactory _loggerFactory;

        public List<Assembly> Assemblies { get; private set; }
        public Type DefaultMapper { get; private set; }
        public string DefaultConnectionStringName { get; private set; }
        public ISqlDialect Dialect { get; private set; }

        private DapperConfiguration(IDictionary<string, string> allConnectionStrings, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            AllConnectionStrings = allConnectionStrings;
            Assemblies = new List<Assembly>();
            DefaultConnectionStringName = "__Default";
        }

        public IDictionary<string, string> AllConnectionStrings { get; private set; }

        /// <summary>
        /// Creates a Dapper configuration with default static <see cref="IConnectionStringProvider"/> and <see cref="IDapperSessionContext"/> per thread.
        /// </summary>
        /// <returns></returns>
        public static IDapperConfiguration Use(IDictionary<string, string> allConnectionStrings, ILoggerFactory loggerFactory)
        {
            return new DapperConfiguration(allConnectionStrings, loggerFactory);
        }

        public IDapperConfiguration WithDefaultConnectionStringNamed(string name)
        {
            DefaultConnectionStringName = name;
            return this;
        }

        /// <summary>
        /// Assign a class mapper during configuration
        /// 
        /// <see cref="IClassMapper"/>  implemention is required
        /// </summary>
        /// <param name="typeOfMapper"></param>
        /// <returns></returns>
        public IDapperConfiguration UseClassMapper(Type typeOfMapper)
        {
            if (typeof(IClassMapper).IsAssignableFrom(typeOfMapper) == false)
                throw new NullReferenceException("Mapping is not type of IClassMapper");

            DefaultMapper = typeOfMapper;
            return this;
        }
        /// <summary>
        /// Changes the <see cref="ISqlDialect"/>.
        /// </summary>
        /// <param name="dialect"></param>
        /// <returns></returns>
        public IDapperConfiguration UseSqlDialect(ISqlDialect dialect)
        {
            Dialect = dialect;
            return this;
        }

        public IDapperConfiguration FromAssemblies(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            Assemblies.AddRange(assemblies);

            return this;
        }

        public IDapperConfiguration FromAssembly(string name)
        {
            string path = Directory
                .GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.AllDirectories)
                .SingleOrDefault(s =>
                {
                    var fileName = Path.GetFileNameWithoutExtension(s);
                    return fileName != null && fileName.Equals(name, StringComparison.OrdinalIgnoreCase);
                });

            if (string.IsNullOrEmpty(path))
                throw new NullReferenceException(string.Format("Assembly '{0}' could not be found.", name));

            string assemblyName = Path.GetFileNameWithoutExtension(path);
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                throw new NullReferenceException(string.Format("Assembly '{0}' could not be loaded.", name));

            Assemblies.Add(assembly);

            return this;
        }

        public IDapperConfiguration FromAssemblyContaining(Type type)
        {
            if (type == null)
                throw new NullReferenceException("Type cannot be null");

            Assemblies.Add(type.Assembly);

            return this;
        }

        public IDapperConfiguration Build()
        {
            if (Dialect == null)
                throw new NullReferenceException("SqlDialect has not been set. Call UseSqlDialect().");

            return this;
        }

        public IClassMapper GetMap(Type entityType)
        {
            if (_classMaps.TryGetValue(entityType, out IClassMapper map)) return map;
            Type mapType = GetMapType(entityType) ?? DefaultMapper.MakeGenericType(entityType);

            map = Activator.CreateInstance(mapType, new[] { _loggerFactory.CreateLogger(entityType) }) as IClassMapper;
            _classMaps[entityType] = map;

            return map;
        }

        public IClassMapper GetMap<T>() where T : class
        {
            return GetMap(typeof(T));
        }

        public Guid GetNextGuid()
        {
            byte[] b = Guid.NewGuid().ToByteArray();
            DateTime dateTime = new DateTime(1900, 1, 1);
            DateTime now = DateTime.Now;
            TimeSpan timeSpan = new TimeSpan(now.Ticks - dateTime.Ticks);
            TimeSpan timeOfDay = now.TimeOfDay;
            byte[] bytes1 = BitConverter.GetBytes(timeSpan.Days);
            byte[] bytes2 = BitConverter.GetBytes((long)(timeOfDay.TotalMilliseconds / 3.333333));
            Array.Reverse(bytes1);
            Array.Reverse(bytes2);
            Array.Copy(bytes1, bytes1.Length - 2, b, b.Length - 6, 2);
            Array.Copy(bytes2, bytes2.Length - 4, b, b.Length - 4, 4);
            return new Guid(b);
        }

        protected virtual Type GetMapType(Type entityType)
        {
            Type getType(Assembly a)
            {
                Type[] types = a.GetTypes();
                return (from type in types
                        let interfaceType = type.GetInterface(typeof(IClassMapper<>).FullName)
                        where
                            interfaceType != null &&
                            interfaceType.GetGenericArguments()[0] == entityType
                        select type).SingleOrDefault();
            }

            Type result = getType(entityType.Assembly);
            if (result != null)
            {
                return result;
            }

            return null;

            //foreach (var mappingAssembly in Assemblies)
            //{
            //    result = getType(mappingAssembly);
            //    if (result != null)
            //    {
            //        return result;
            //    }
            //}

            //return getType(entityType.Assembly);
        }
    }
}
