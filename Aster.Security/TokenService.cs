using Aster.Cache;
using Aster.Security.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aster.Security
{
    public class TokenService : ITokenService
    {
        private readonly TokenOptions _tokenOptions;
        private readonly IDistributedCache _distributedCache;
        private readonly RedisOptions _redisOptions;
        private const string _user_token_cache = "user.{0}.{1}";

        public TokenService(IOptions<TokenOptions> tokenOptions,
            IOptions<RedisOptions> redisOptions,
            IDistributedCache distributedCache)
        {
            _tokenOptions = tokenOptions.Value;
            _redisOptions = redisOptions.Value;
            _distributedCache = distributedCache;
        }

        public async Task DelToken(string userCode, string loginType)
        {
            if (string.IsNullOrWhiteSpace(userCode))
                throw new ArgumentNullException(nameof(userCode));

            if (string.IsNullOrWhiteSpace(loginType))
                throw new ArgumentNullException(loginType);

            loginType = loginType.ToLowerInvariant();
            string key = string.Format(_user_token_cache, userCode, loginType);

            await _distributedCache.RemoveAsync(key);
        }

        public async Task DelToken(string tokenKey)
        {
            if (string.IsNullOrWhiteSpace(tokenKey))
                throw new ArgumentNullException(tokenKey);

            await _distributedCache.RemoveAsync(tokenKey);
        }

        public async Task<string> BuildToken(TokenInfo token, string loginType)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            if (string.IsNullOrWhiteSpace(loginType))
                throw new ArgumentNullException(loginType);

            loginType = loginType.ToLowerInvariant();

            TimeSpan? expireIn = GetExpireIn(loginType);

            //过期时间写入token
            token.ExpireIn = expireIn.HasValue ? (int)expireIn.Value.TotalSeconds : -1;

            string key = string.Format(_user_token_cache, token.Code, loginType);

            await _distributedCache.Set(key, token, expireIn);

            return key;
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

        public async Task RefreshToken(string tokenKey, Func<int, Task<TokenInfo>> getTokenInfo)
        {
            if (tokenKey == null)
                throw new ArgumentNullException(nameof(tokenKey));
            if (getTokenInfo == null)
                throw new ArgumentNullException(nameof(getTokenInfo));

            string json = await _distributedCache.GetStringAsync(tokenKey);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var old = JsonConvert.DeserializeObject<TokenInfo>(json);
                if (old != null)
                {
                    var tokenInfo = await getTokenInfo(old.Id);
                    if (tokenInfo != null)
                    {
                        string loginType = tokenKey.Substring(tokenKey.LastIndexOf('.') + 1);
                        await BuildToken(tokenInfo, loginType);
                    }
                }
            }
        }

        public async Task<TokenInfo> GetTokenInfo(string userCode, string loginType)
        {
            if (string.IsNullOrWhiteSpace(userCode))
                throw new ArgumentNullException(nameof(userCode));

            if (string.IsNullOrWhiteSpace(loginType))
                throw new ArgumentNullException(loginType);

            loginType = loginType.ToLowerInvariant();
            return await GetTokenInfo(string.Format(_user_token_cache, userCode, loginType));
        }

        public async Task<TokenInfo> GetTokenInfo(string tokenKey)
        {
            if (string.IsNullOrWhiteSpace(tokenKey))
                throw new ArgumentNullException(nameof(tokenKey));

            string json = await _distributedCache.GetStringAsync(tokenKey);

            if (string.IsNullOrWhiteSpace(json)) return null;

            return JsonConvert.DeserializeObject<TokenInfo>(json);
        }

        public async Task<List<string>> GetUserAllTokens(string userCode)
        {
            //Aster:usersystem:user.7DO9772PYH3SPH2EAV5C.*
            var keys = await RedisHelper.KeysAsync(pattern: $"{_redisOptions.KeyPreffix}{string.Format(_user_token_cache, userCode, "*")}");

            return keys.Select(x => x.ToString().Replace(_redisOptions.KeyPreffix, string.Empty)).ToList();
        }

        public async Task OfflineAllClients(string userCode)
        {
            if (string.IsNullOrWhiteSpace(userCode)) throw new ArgumentNullException(nameof(userCode));

            var tokens = await GetUserAllTokens(userCode);
            if (tokens.Count > 0)
            {
                var ts = tokens.Select(x => DelToken(x)).ToList();
                foreach (var t in ts) await t;
            }
        }
    }
}
