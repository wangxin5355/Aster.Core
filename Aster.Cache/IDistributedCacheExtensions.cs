using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aster.Cache
{
    public static class IDistributedCacheExtensions
    {
        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> getData, TimeSpan? expire = null, CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            if (getData == null) throw new ArgumentNullException(nameof(getData));

            token.ThrowIfCancellationRequested();

            var bytes = await cache.GetAsync(key, token).ConfigureAwait(false);

            if (bytes == null)
            {
                var m = await getData();
                if (m != null)
                {
                    var options = new DistributedCacheEntryOptions();
                    if (expire != null)
                        options.AbsoluteExpirationRelativeToNow = expire;

                    await cache.SetStringAsync(key, JsonConvert.SerializeObject(m), options, token).ConfigureAwait(false);
                }
                return m;
            }

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }

        //[Obsolete]
        //public static T Get<T>(this IDistributedCache cache, string key, Func<T> getData, TimeSpan? expire = null, CancellationToken token = default(CancellationToken))
        //{
        //    if (string.IsNullOrWhiteSpace(key))
        //        throw new ArgumentNullException(nameof(key));

        //    if (getData == null)
        //        throw new ArgumentNullException(nameof(getData));

        //    token.ThrowIfCancellationRequested();

        //    var bytes = cache.Get(key);

        //    if (bytes == null)
        //    {
        //        var m = getData();
        //        if (m != null)
        //        {
        //            var options = new DistributedCacheEntryOptions();
        //            if (expire != null)
        //                options.AbsoluteExpirationRelativeToNow = expire;
        //            cache.SetString(key, JsonConvert.SerializeObject(m), options);
        //        }

        //        return m;
        //    }

        //    return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        //}

        public static async Task Set<T>(this IDistributedCache cache, string key, T data, TimeSpan? expireIn = null, CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            token.ThrowIfCancellationRequested();

            string jsonData = JsonConvert.SerializeObject(data);
            var options = new DistributedCacheEntryOptions();
            if (expireIn != null)
            {
                options.AbsoluteExpirationRelativeToNow = expireIn.Value;
            };
            await cache.SetStringAsync(key, jsonData, options, token);
        }

        public static async Task SetSlidingExpiration<T>(this IDistributedCache cache, string key, T data, TimeSpan? expireIn = null, CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            token.ThrowIfCancellationRequested();

            string jsonData = JsonConvert.SerializeObject(data);
            var options = new DistributedCacheEntryOptions();
            if (expireIn != null)
            {
                options.SlidingExpiration = expireIn;
            };
            await cache.SetStringAsync(key, jsonData, options, token);
        }
    }
}