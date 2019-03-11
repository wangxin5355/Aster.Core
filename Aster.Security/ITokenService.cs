using Aster.Security.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aster.Security
{
    public interface ITokenService
    {
        /// <summary>
        /// 获取用户token信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="loginType"></param>
        /// <returns></returns>
        Task<TokenInfo> GetTokenInfo(string userCode, string loginType);

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="tokenKey"></param>
        /// <param name="getTokenInfo"></param>
        /// <returns></returns>
        Task RefreshToken(string tokenKey, Func<int, Task<TokenInfo>> getTokenInfo);

        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="loginType"></param>
        /// <returns></returns>
        Task<string> BuildToken(TokenInfo token, string loginType);

        /// <summary>
        /// 获取用户的token信息
        /// </summary>
        /// <param name="tokenKey"></param>
        /// <returns></returns>
        Task<TokenInfo> GetTokenInfo(string tokenKey);

        /// <summary>
        /// 删除用户的token
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="loginType"></param>
        /// <returns></returns>
        Task DelToken(string userCode, string loginType);

        /// <summary>
        /// 删除用户的token
        /// </summary>
        /// <param name="tokenKey"></param>
        /// <returns></returns>
        Task DelToken(string tokenKey);

        /// <summary>
        /// 获取用户当前有效的所有的token
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        Task<List<string>> GetUserAllTokens(string userCode);

        /// <summary>
        /// 下线用户所有的客户端
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        Task OfflineAllClients(string userCode);
    }
}
