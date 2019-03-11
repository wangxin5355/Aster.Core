using System;
using System.Linq;
using System.Security.Claims;

namespace Aster.Common.Extensions
{
    public static class ClaimPrincipalExtensions
    {
        public static string GetToken(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.Claims.FirstOrDefault(x => x.Type == "token")?.Value ?? string.Empty;
        }

        public static string GetUserName(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        }

        public static int GetId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var identity = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Convert.ToInt32(identity?.Value);
        }

        /// <summary>
        /// 获取用户是否允许交易的状态
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static bool GetUserAllowTradeStatus(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return Convert.ToBoolean(principal.Claims.FirstOrDefault(x => x.Type == "allowTrade")?.Value ?? "False");
        }

        /// <summary>
        /// 获取用户的禁用状态
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static bool GetUserIsValidStatus(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return Convert.ToBoolean(principal.Claims.FirstOrDefault(x => x.Type == "isValid")?.Value ?? "False");
        }

        /// <summary>
        /// 获取用户的持仓上限模板ID
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static int GetUserPositionLimitTemplateId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return Convert.ToInt32(principal.Claims.FirstOrDefault(x => x.Type == "positionLimitTemplateId")?.Value);
        }
    }
}
