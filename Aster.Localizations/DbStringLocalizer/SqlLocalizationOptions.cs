namespace Aster.Localizations.DbStringLocalizer
{
    public class SqlLocalizationOptions : LocationOption
    {
        /// <summary>
        /// 数据库连接
        /// </summary>
        public string DbConnectionString { get; set; }

        /// <summary>
        /// Creates a new item in the SQL database if the resource is not found
        /// </summary>
        public bool CreateNewRecordWhenLocalisedStringDoesNotExist { get; set; }
    }
}
