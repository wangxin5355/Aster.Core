using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aster.UserService.ViewModel
{
    public class LoginViewModel
    {
        /// <summary>
        /// 用户名（邮箱/手机号)
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
