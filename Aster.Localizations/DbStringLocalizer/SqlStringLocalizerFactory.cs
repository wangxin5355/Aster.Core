using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Aster.Localizations.DbStringLocalizer
{
    public class SqlStringLocalizerFactory : IStringLocalizerFactory, IStringExtendedLocalizerFactory
    {
        private readonly LocalizationModelContext _modelContext;
        private readonly ConcurrentDictionary<string, IStringLocalizer> _resourceLocalizations;
        private readonly IOptions<SqlLocalizationOptions> _options;

        public SqlStringLocalizerFactory(
           LocalizationModelContext context,
           LocalizationModelContext modelContext,
           IOptions<SqlLocalizationOptions> localizationOptions)
        {
            _options = localizationOptions;
            _modelContext = modelContext;
            _resourceLocalizations = new ConcurrentDictionary<string, IStringLocalizer>();
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return Create(resourceSource.FullName);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            string resourceKey = baseName + location;

            return Create(resourceKey);
        }

        private IStringLocalizer Create(string resourceKey)
        {
            if (string.IsNullOrWhiteSpace(resourceKey))
                throw new ArgumentNullException(nameof(resourceKey));

            if (_resourceLocalizations.TryGetValue(resourceKey, out IStringLocalizer localizer))
            {
                return localizer;
            }

            var resourceAll = GetAllFromDatabaseForResource(resourceKey);

            localizer = new SqlStringLocalizer(resourceAll, _modelContext, resourceKey, _options.Value);

            return _resourceLocalizations.GetOrAdd(resourceKey, localizer);
        }

        private Dictionary<string, Dictionary<string, string>> GetAllFromDatabaseForResource(string resourceKey)
        {
            var data = _modelContext.GetLocalizations(resourceKey);

            return data.GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, y => y.ToDictionary(z => z.Culture, z => z.Text));
        }
    }
}
