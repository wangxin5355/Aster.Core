using System;

namespace Aster.Common.Data.Core.Sessions
{
    public interface IDapperSessionFactory
    {
        /// <summary>
        /// 获取type对应数据库连接字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetTypeConnectionStringName(Type type);

        /// <summary>
        /// connectionStringName对应数据库的连接（每次调用都会重新开启连接，调用方需要做会话缓存）
        /// </summary>
        /// <param name="connectionStringName">数据库连接字符串名称</param>
        /// <returns></returns>
        IDapperSession GetSession(string connectionStringName);
    }
}
