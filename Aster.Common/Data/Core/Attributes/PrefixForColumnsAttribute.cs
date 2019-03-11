using System;

namespace Aster.Common.Data.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PrefixForColumnsAttribute : Attribute
    {
        public string Prefix { get; private set; }
        public PrefixForColumnsAttribute(string prefix)
        {
            Prefix = prefix;
        }
    }
}