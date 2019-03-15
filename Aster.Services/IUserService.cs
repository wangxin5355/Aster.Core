using Aster.Services.Models;
using System;
using System.Threading.Tasks;

namespace Aster.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="packType">平台类型ios androd,pcweb</param>
        /// <returns></returns>
        Task<LoginRpsModel> Login(string userName, string password, string packType);
    }
}
