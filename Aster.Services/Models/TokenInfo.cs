using Aster.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aster.Services.Models
{
    public class TokenInfo
    {
        public TokenInfo(User user)
        {
            UserId = user.Id;
            UserName = user.UserName;
        }

        public int Id { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public long ExpireTime { get; set; }

        public string Token { get; set; }

        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 平台类型
        /// </summary>
        public string PackType { get; set; }

        /// <summary>
        /// 激活时间
        /// </summary>
        public DateTime? ActivedTime { get; set; }

        /// <summary>
        /// 账号是否有效
        /// </summary>
        public bool IsValid { get; set; }
    }
}
