using System;
using System.Collections.Generic;

namespace Aster.Security.Models
{
    public class TokenOptions
    {
        /// <summary>
        /// 过期时间HH:MM:SS
        /// </summary>
        public TimeSpan DefaultExpireTime { get; set; } = TimeSpan.Parse("00:30:00");

        public Expires[] Expires { get; set; } = new[] {
             new Expires() { LoginType="ios", ExpireTime= null },
             new Expires(){ LoginType="android", ExpireTime= null }
        };

        /// <summary>
        /// ip白名单 (配置WhiteListFilterAttribute)
        /// </summary>
        public List<string> IpWhiteList { get; set; }
    }

    public class Expires
    {
        /// <summary>
        /// 登录类型
        /// </summary>
        public string LoginType { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan? ExpireTime { get; set; }
    }
}
