using System;

namespace Aster.Cache
{
    public class Lock
    {
        public Lock(string resource, byte[] val, TimeSpan validity)
        {
            Resource = resource;
            Value = val;
            Validity = validity;
        }

        public string Resource { get; private set; }

        public byte[] Value { get; private set; }

        public TimeSpan Validity { get; private set; }
    }
}
