using Aster.Common.Web.Models;
using System.Text;

namespace Aster.Common.Extensions
{
    public static class StringExtensions
    {
        public static byte[] ToBytes(this string s)
        {
            if (s == null)
                return null;

            return Encoding.UTF8.GetBytes(s);
        }

        public static PackTypeEnum? ToPackType(this string type)
        {
            switch (type)
            {
                case "1": return PackTypeEnum.PcWeb;
                case "2": return PackTypeEnum.H5;
                case "3": return PackTypeEnum.IOS;
                case "4": return PackTypeEnum.Android;
                default:
                    return default(PackTypeEnum?);
            }
        }
    }
}
