using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Aster.Localizations.DbStringLocalizer
{
    public class SqlStringLocalizer : IStringLocalizer
    {
        private readonly Dictionary<string, Dictionary<string, string>> _localizations;
        private readonly LocalizationModelContext _modelContext;
        private readonly string _resourceKey;
        private readonly SqlLocalizationOptions _options;

        public SqlStringLocalizer(Dictionary<string, Dictionary<string, string>> localizations,
            LocalizationModelContext modelContext,
            string resourceKey,
            SqlLocalizationOptions options)
        {
            _modelContext = modelContext;
            _localizations = localizations;
            _resourceKey = resourceKey;
            _options = options;
        }
        public virtual LocalizedString this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var text = GetText(name, null);

                return new LocalizedString(name, text.text ?? name, text.notSucceed);
            }
        }

        public virtual LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var text = GetText(name, null);
                var str = string.Format(text.text ?? name, arguments);

                return new LocalizedString(name, str, text.notSucceed);
            }
        }

        public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return GetAllStrings(includeParentCultures, null);
        }

        protected IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures, CultureInfo culture)
        {
            string cultureName = (culture ?? CultureInfo.CurrentCulture).Name;

            foreach (var (key, vals) in _localizations)
            {
                if (vals.TryGetValue(cultureName, out string val) && !string.IsNullOrWhiteSpace(val))
                    yield return new LocalizedString(key, val, false);
                else
                    yield return new LocalizedString(key, key, true);
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return culture == null ?
                new SqlStringLocalizer(_localizations,
                _modelContext,
                _resourceKey,
                _options) :
                new ResourceManagerWithCultureSqlStringLocalizer(_localizations,
                _modelContext,
                _resourceKey,
                _options,
                culture);
        }

        protected (string text, bool notSucceed) GetText(string key, CultureInfo culture)
        {
            string cultureName = (culture ?? CultureInfo.CurrentCulture).Name;

            if (cultureName == _options.DefaultCulture) return (key, false);

            if (_localizations.TryGetValue(key, out Dictionary<string, string> dic))
            {
                if (dic.TryGetValue(cultureName, out string result) && !string.IsNullOrWhiteSpace(result))
                {
                    return (result, false);
                }
            }

            return (key, true);
        }
    }
}
