using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aster.Localizations.DbStringLocalizer
{
    public class LocalizationModelContext
    {
        private readonly ILogger _logger;
        private readonly SqlLocalizationOptions _sqlLocalizationOptions;
        private Dictionary<string, List<Localization>> _ldic;

        public LocalizationModelContext(
            ILogger<LocalizationModelContext> logger,
           IOptions<SqlLocalizationOptions> localizationOptions)
        {
            _logger = logger;
            _sqlLocalizationOptions = localizationOptions.Value;
        }

        public void EnsureLoadAllLocations()
        {
            if (_ldic == null)
            {
                try
                {
                    _logger.LogInformation("初始化多语言词典");
                    using (var conn = new MySqlConnection(_sqlLocalizationOptions.DbConnectionString))
                    {
                        conn.Open();
                        var ls = conn.Query<Localization>("select * from t_localization", commandTimeout: 3);
                        _ldic = ls.GroupBy(x => x.ResourceKey)
                            .ToDictionary(x => x.Key, x => x.ToList());

                        conn.Close();
                    }
                    _logger.LogInformation("化多语言词典初始化完毕");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "初始化多语言失败");
                }
            }

        }

        public IList<Localization> GetLocalizations(string resourceKey)
        {
            if (_ldic != null && _ldic.TryGetValue(resourceKey, out List<Localization> ls))
            {
                return ls;
            }
            return new List<Localization>();
        }
    }
}