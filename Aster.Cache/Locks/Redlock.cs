using System;
using System.Threading.Tasks;

namespace Aster.Cache
{
    public class Redlock
    {
        const int _defaultRetryCount = 3;
        readonly TimeSpan _defaultRetryDelay = new TimeSpan(0, 0, 0, 0, 200);
        const double _clockDriveFactor = 0.01;

        /// <summary>
        /// String containing the Lua unlock script.
        /// </summary>
        const string UnlockScript = @"
            if redis.call(""get"",KEYS[1]) == ARGV[1] then
                return redis.call(""del"",KEYS[1])
            else
                return 0
            end";


        protected static byte[] CreateUniqueLockId()
        {
            return Guid.NewGuid().ToByteArray();
        }

        protected async Task<bool> LockInstance(string resource, byte[] val, TimeSpan ttl)
        {
            bool succeeded;
            try
            {
                succeeded = await RedisHelper.SetAsync(resource, val, (int)ttl.TotalSeconds, CSRedis.RedisExistence.Nx);
            }
            catch
            {
                succeeded = false;
            }
            return succeeded;
        }

        protected async Task UnlockInstance(string resource, byte[] val)
        {
            await RedisHelper.EvalAsync(UnlockScript, resource, new[] { val });
        }

        public async Task<(bool ok, Lock locker)> Lock(string resource, TimeSpan ttl)
        {
            var val = CreateUniqueLockId();

            (bool successfull, Lock locker) = await Retry(_defaultRetryCount, _defaultRetryDelay, async () =>
              {
                  try
                  {
                      int n = 0;
                      var startTime = DateTime.Now;

                      // Use keys
                      if (await LockInstance(resource, val, ttl)) n += 1;

                      /*
                       * Add 2 milliseconds to the drift to account for Redis expires
                       * precision, which is 1 millisecond, plus 1 millisecond min drift 
                       * for small TTLs.        
                       */
                      var drift = Convert.ToInt32((ttl.TotalMilliseconds * _clockDriveFactor) + 2);
                      var validity_time = ttl - (DateTime.Now - startTime) - new TimeSpan(0, 0, 0, 0, drift);

                      if (n >= 1 && validity_time.TotalMilliseconds > 0)
                      {
                          return (true, new Lock(resource, val, validity_time));
                      }
                      else
                      {
                          await UnlockInstance(resource, val);
                          return (false, null);
                      }
                  }
                  catch { return (false, null); }
              });

            return (successfull, locker);
        }

        /// <summary>
        /// 加锁运行
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="ttl"></param>
        /// <param name="action"></param>
        /// <exception cref="AcquireLockFailException">如果获取锁失败，抛出异常</exception>
        /// <returns></returns>
        public async Task RunWithLock(string resource, TimeSpan ttl, Func<Task> action)
        {
            Lock locker = null;

            try
            {
                var (ok, l) = await Lock(resource, ttl);
                locker = l;

                if (!ok)
                    throw new AcquireLockFailException(resource, ttl);

                await action();
            }
            finally
            {
                if (locker != null) await Unlock(locker);
            }
        }

        /// <summary>
        /// 加锁运行
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="ttl"></param>
        /// <param name="action"></param>
        /// <exception cref="AcquireLockFailException">如果获取锁失败，抛出异常</exception>
        /// <returns></returns>
        public async Task RunWithLock(string resource, TimeSpan ttl, Action action)
        {
            Lock locker = null;

            try
            {
                var (ok, l) = await Lock(resource, ttl);
                locker = l;

                if (!ok)
                    throw new AcquireLockFailException(resource, ttl);

                action();
            }
            finally
            {
                if (locker != null) await Unlock(locker);
            }
        }

        /// <summary>
        /// 加锁运行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="ttl"></param>
        /// <param name="action"></param>
        /// <exception cref="AcquireLockFailException">如果获取锁失败，抛出异常</exception>
        /// <returns></returns>
        public async Task<T> RunWithLock<T>(string resource, TimeSpan ttl, Func<Task<T>> action)
        {
            Lock locker = null;

            try
            {
                var (ok, l) = await Lock(resource, ttl);
                locker = l;

                if (!ok)
                    throw new AcquireLockFailException(resource, ttl);

                return await action();
            }
            finally
            {
                if (locker != null) await Unlock(locker);
            }
        }

        protected async Task<(bool ok, Lock locker)> Retry(int retryCount, TimeSpan retryDelay, Func<Task<(bool ok, Lock locker)>> action)
        {
            int maxRetryDelay = (int)retryDelay.TotalMilliseconds;
            Random rnd = new Random();
            int currentRetry = 0;

            while (currentRetry++ < retryCount)
            {
                var r = await action();
                if (r.ok) return r;

                await Task.Delay(rnd.Next(maxRetryDelay));
            }

            return (false, null);
        }

        public async Task Unlock(Lock lockObject)
        {
            await UnlockInstance(lockObject.Resource, lockObject.Value);
        }
    }
}