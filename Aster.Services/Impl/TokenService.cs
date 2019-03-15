using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aster.Cache;
using Aster.Common.Utils;
using Aster.Security.Models;
using Aster.Services.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Aster.Services.Impl
{
    public class TokenService : ITokenService
    {
        private const string _user_token_cache = "user.{0}.{1}";
        private readonly TokenOptions _tokenOptions;
        private readonly IDistributedCache _distributedCache;

        public TokenService(IOptions<TokenOptions> tokenOptions,
            IDistributedCache distributedCache)
        {
            _tokenOptions = tokenOptions.Value;
            _distributedCache = distributedCache;
        }

        public async Task<string> BuildToken(Models.TokenInfo token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            if (string.IsNullOrWhiteSpace(token.PackType))
                throw new ArgumentNullException(token.PackType);

            token.PackType = token.PackType.ToLowerInvariant();

            TimeSpan? expireIn = GetExpireIn(token.PackType);

            //过期时间写入token
            long expireTimeSpan = DateTimeUtil.GetTimestamp_microsecond(DateTime.Now);
            token.ExpireTime = expireIn.HasValue ? (int)expireIn.Value.TotalSeconds+ expireTimeSpan : -1;

            string key = string.Format(_user_token_cache, token.UserId, token.PackType);
            var tokenstr = SecurityUtil.AESEncrypt(key);
            token.Token = tokenstr;
            await _distributedCache.Set(key, token, expireIn);
            return tokenstr;
        }

        private TimeSpan? GetExpireIn(string loginType)
        {
            if (_tokenOptions.Expires != null)
            {
                var expire = _tokenOptions.Expires
                    .FirstOrDefault(x => x.LoginType.Equals(loginType, StringComparison.InvariantCultureIgnoreCase));

                if (expire != null)
                    return expire.ExpireTime;
            }

            return _tokenOptions.DefaultExpireTime;
        }


        public async Task DelToken(int userId, string packType)
        {
            if (userId <= 0)
                return;

            if (string.IsNullOrWhiteSpace(packType))
                throw new ArgumentNullException(packType);

            packType = packType.ToLowerInvariant();
            string key = string.Format(_user_token_cache, userId, packType);

            await _distributedCache.RemoveAsync(key);
        }

        public async Task<Models.TokenInfo> GetTokenInfo(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            var tokenKey= SecurityUtil.AESDecrypt(token);
            string json = await _distributedCache.GetStringAsync(tokenKey);

            if (string.IsNullOrWhiteSpace(json)) return null;

            return JsonConvert.DeserializeObject<Models.TokenInfo>(json);
        }
    }
}
