using System;

namespace Aster.Common.Utils
{
    public static class DateTimeUtil
    {
        public static long GetUnixTimestamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000 / 1000;
        }

        /// <summary>
        /// 获取时间戳（毫秒级别)
        /// </summary>
        /// <returns></returns>
        public static long GetTimestamps(DateTime? dt = null)
        {
            return ((dt ?? DateTime.Now).ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// 获取时间戳（微秒级别)
        /// </summary>
        /// <returns></returns>
        public static long GetTimestamp_microsecond()
        {
            return GetTimestamp_microsecond(DateTime.Now);
        }

        /// <summary>
        /// 获取时间戳（微秒级别)
        /// </summary>
        /// <returns></returns>
        public static long GetTimestamp_microsecond(DateTime dt)
        {
            return (dt.ToUniversalTime().Ticks - 621355968000000000) / 10;
        }

        /// <summary>
        /// 时间戳换算成本地时间（毫秒级别）
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime FromTimestamps(long timestamp)
        {
            return (new DateTime(timestamp * 10000 + 621355968000000000, DateTimeKind.Utc)).ToLocalTime();
        }

        /// <summary>
        /// 时间戳换算成本地时间（微秒级别）
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime FromTimeStamps_microsecond(long timestamp)
        {
            //1 tickets = 千万分之一秒，1us = 百万分之一
            //1 us= 10 tickets

            return (new DateTime(timestamp * 10 + 621355968000000000, DateTimeKind.Utc)).ToLocalTime();
        }

        /// <summary>
        /// 时间戳换算成本地时间（微秒级别）eg:xxx.123456
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime FromTimeStamps_microsecond(double timestamp)
        {
            return FromTimeStamps_microsecond((long)(timestamp * 1000000));
        }
    }
}
