using System;
using System.Collections.Generic;
using System.Text;

namespace Aster.Services.Models
{
    public class LoginRpsModel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Token 
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// token到期时间。根据业务需要是否做token过期。如果有token过期，业务方需要定时刷新token来保持登陆状态
        /// </summary>
        public DateTime TokenExpireTime { get; set; }


    }
}
