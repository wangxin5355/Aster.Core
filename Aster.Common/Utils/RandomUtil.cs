using System;
using System.Security.Cryptography;

namespace Aster.Common.Utils
{
    public static class RandomUtil
    {
        /// <summary>
        /// 生成唯一的字符串
        /// eg: 66bHktcr206Zl9h9nZ28Lg
        /// </summary>
        /// <param name="len">串长度，默认20</param>
        /// <returns></returns>
        public static string GetUniqueStr(int len = 20)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[len];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[Random() % chars.Length];
            }

            var finalString = new String(stringChars);
            return finalString;
        }

        /// <summary>
		/// 返回介于min和max之间的一个随机数。包括min，不包括max
		/// </summary>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		public static int Random(int min, int max)
        {
            return RandomGen3.Next(min, max);
        }

        /// <summary>
        /// 返回非负随机数（多线程下也严格随机）
        /// </summary>
        /// <returns></returns>
        public static int Random()
        {
            return RandomGen3.Next();
        }

        public static string Random(string chars, int len)
        {
            var stringChars = new char[len];
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[Random() % chars.Length];
            }
            var finalString = new String(stringChars);
            return finalString;
        }

        /// <summary>
        /// 获取6位数字组成的随机串
        /// </summary>
        /// <returns></returns>
        public static string GetRandomDigitStr6()
        {
            return Random(0, 999999).ToString("D6");
        }

        #region RandomGen3

        /// <summary>
        /// 多线程环境下严格随机的Random实现
        /// </summary>
        private static class RandomGen3
        {
            private static RNGCryptoServiceProvider _global = new RNGCryptoServiceProvider();

            [ThreadStatic]
            private static Random _local;

            public static int Next()
            {
                Random inst = _local;
                if (inst == null)
                {
                    byte[] buffer = new byte[4];
                    _global.GetBytes(buffer);
                    _local = inst = new Random(BitConverter.ToInt32(buffer, 0));
                }
                return inst.Next();
            }

            public static int Next(int min, int max)
            {
                Random inst = _local;
                if (inst == null)
                {
                    byte[] buffer = new byte[4];
                    _global.GetBytes(buffer);
                    _local = inst = new Random(BitConverter.ToInt32(buffer, 0));
                }
                return inst.Next(min, max);
            }
        }

        #endregion

        public static string GetPhoneNo(string preffix = "138")
        {
            return preffix + BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0).ToString().Substring(0, 8);
        }
    }
}
