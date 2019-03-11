using System;

namespace Aster.Cache
{
    public class AcquireLockFailException : Exception
    {
        public string LockResource { get; set; }

        public TimeSpan TTL { get; set; }

        public AcquireLockFailException(string lockResource, TimeSpan ttl)
            : base($"获取锁失败，resource: {lockResource}, ttl: {ttl}")
        {
            LockResource = lockResource;
            TTL = ttl;
        }
    }
}
