using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Aster.Localizations.DbStringLocalizer
{
    public class ResourceManagerWithCultureSqlStringLocalizer : SqlStringLocalizer
    {
        private readonly CultureInfo _culture;

        public ResourceManagerWithCultureSqlStringLocalizer(
            Dictionary<string, Dictionary<string, string>> localizations,
            LocalizationModelContext modelContext,
            string resourceKey, 
            SqlLocalizationOptions options,
            CultureInfo culture) : base(localizations, modelContext, resourceKey, options)
        {
            _culture = culture;
        }

        public override LocalizedString this[string name]
        {
            get
            {
                var text = GetText(name, _culture);

                return new LocalizedString(name, text.text ?? name, text.notSucceed);
            }
        }

        /// <inheritdoc />
        public override LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var text = GetText(name, _culture);
                var str = string.Format(text.text ?? name, arguments);

                return new LocalizedString(name, str, text.notSucceed);
            }
        }

        /// <inheritdoc />
        public override IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            GetAllStrings(includeParentCultures, _culture);
    }
}
