using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Aster.Common.Utils
{
    public static class ConverterUtil
    {
        /// <summary>
        /// 将JObject List转换成DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <param name="tableName"></param>
        /// <param name="lstColumn"></param>
        /// <param name="lstTitle"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable(List<JObject> entitys, string tableName, List<string> lstColumn, List<string> lstTitle)
        {
            if (lstColumn.Count != lstTitle.Count) throw new ArgumentException("列的编码和名称集合数量不一致");

            DataTable dt = new DataTable() { TableName = tableName };

            if (entitys == null || entitys.Count == 0) return dt;

            var pDic = entitys[0].Properties().ToDictionary(x => x.Name.ToUpperInvariant(), x => x);
            Dictionary<string, string> titleDic = new Dictionary<string, string>();

            string header;
            for (int i = 0; i < lstColumn.Count; i++)
            {
                var col = lstColumn[i];
                var title = lstTitle[i];
                header = col.ToUpperInvariant();
                if (pDic.ContainsKey(header))
                {
                    dt.Columns.Add(header);

                    titleDic.Add(header, title);
                }
            }

            if (entitys != null)
            {
                foreach (var entity in entitys)
                {
                    DataRow dr = dt.NewRow();
                    foreach (DataColumn col in dt.Columns)
                    {
                        var property = entity.Property(pDic[col.ColumnName].Name);
                        if (property.Value?.Type == JTokenType.Date)
                        {
                            dr[col] = property.HasValues ? ((JValue)property.Value).Value<DateTime>().ToString("yyyy-MM-dd HH:mm:ss") : null;
                        }
                        else
                        {
                            dr[col] = property.HasValues ? property.Value : default(JToken);
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }
            foreach (DataColumn col in dt.Columns)
            {
                col.ColumnName = titleDic[col.ColumnName];
            }
            dt.TableName = tableName;
            return dt;
        }
    }
}
