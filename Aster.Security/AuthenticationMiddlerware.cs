using Aster.Security.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Aster.Security
{
    public class AuthenticationMiddlerware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenService _tokenService;

        public AuthenticationMiddlerware(ITokenService tokenService, RequestDelegate next)
        {
            _tokenService = tokenService;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var header = context.Request.Headers["Authorization"];

            var (ok, tokenInfo) = await TokenIsValid(header);

            if (ok)
            {
                var claims = new List<Claim>()
                {
                    new Claim("token",header),
                    new Claim(ClaimTypes.NameIdentifier, tokenInfo.Id.ToString()),
                    new Claim(ClaimTypes.Name, tokenInfo.UserName ?? string.Empty),
                    new Claim("RegistType", tokenInfo.RegistType??string.Empty),
                    new Claim("UserCode", tokenInfo.Code??string.Empty)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "TokenAuth");
                var principal = new ClaimsPrincipal(new[] { claimsIdentity });

                context.User = principal;
                Thread.CurrentPrincipal = principal;
            }

            await _next(context);
        }

        /// <summary>
        /// user.232.web
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<(bool ok, TokenInfo tokenInfo)> TokenIsValid(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return (false, null);

            string[] segs = token.Split('.');

            bool formatOk = segs.Length == 3 && segs[0] == "user";

            if (!formatOk)
                return (false, null);

            var tokenInfo = await _tokenService.GetTokenInfo(token);

            if (tokenInfo != null)
            {
                return (true, tokenInfo);
            }
            return (false, null);
        }
    }

    public static class AuthenticationMiddlerwareConfigureExtensions
    {
        public static void UseSecurity(this IApplicationBuilder app)
        {
            app.UseMiddleware<AuthenticationMiddlerware>();
        }
    }
}
