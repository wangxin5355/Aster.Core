using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Aster.Common.Utils
{
    public static class HashUtil
    {
        public static (string salt, string passwordHash) PasswordHash(string password)
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Hash(salt, password);

            return (Convert.ToBase64String(salt), hashed);
        }

        private static string Hash(byte[] saltBytes, string str)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: str,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        public static bool PasswordHashCheck(string salt, string passworldHash, string passworld)
        {
            var saltBytes = Convert.FromBase64String(salt);
            string hashed = Hash(saltBytes, passworld);

            return hashed == passworldHash;
        }

        public static string GetMD5(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                sb.Append(s[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string GetSHA256(string str)
        {
            using (var algorithm = SHA256.Create())
            {
                var s = algorithm.ComputeHash(Encoding.UTF8.GetBytes(str));

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < s.Length; i++)
                {
                    sb.Append(s[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
