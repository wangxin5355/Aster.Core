using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Aster.Common.Utils
{
    public static class SecurityUtil
    {
        //默认密钥
        private static string AESKey = "[45/*YUIdse..e;]";
        private static string DESKey = "[&HdN72]";

        public static async Task<string> Encrypt(string plainText, byte[] key, byte[] iv)
        {
            using (SymmetricAlgorithm aes = Rijndael.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (ICryptoTransform crypto = aes.CreateEncryptor())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(ms, crypto, CryptoStreamMode.Write))
                        {
                            var bytes = Encoding.UTF8.GetBytes(plainText);
                            cryptoStream.Write(bytes, 0, bytes.Length);
                            await cryptoStream.FlushAsync();
                        }
                        var encryptedBytes = ms.ToArray();
                        return Convert.ToBase64String(encryptedBytes);
                    }
                }
            }
        }

        public static async Task<string> Encrypt(string plainText, string key, string iv)
        {
            return await Encrypt(plainText, Convert.FromBase64String(key), Convert.FromBase64String(iv));
        }

        public static string GenerateIV()
        {
            using (SymmetricAlgorithm aes = Rijndael.Create())
            {
                aes.GenerateIV();

                return Convert.ToBase64String(aes.IV);
            }
        }

        public static async Task<(string iv, string cipherText)> Encrypt(string plainText, string key)
        {
            string iv = GenerateIV();
            return (iv, await Encrypt(plainText, key, iv));
        }

        public static async Task<(string iv, string cipherText)> Encrypt(string plainText, byte[] key)
        {
            return await Encrypt(plainText, Convert.ToBase64String(key));
        }

        public static async Task<string> Decrypt(string cipherText, byte[] key, byte[] iv)
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (SymmetricAlgorithm aes = Rijndael.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    using (MemoryStream ms = new MemoryStream(cipherBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cryptoStream))
                            {
                                return await sr.ReadToEndAsync();
                            }
                        }
                    }
                }
            }
        }

        public static async Task<string> Decrypt(string cipherText, string key, string iv)
        {
            return await Decrypt(cipherText, Convert.FromBase64String(key), Convert.FromBase64String(iv));
        }

        public static string ToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length);

            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public static byte[] HexToBytes(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0) hexString += " ";

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

            return returnBytes;
        }

        public static async Task<string> DESEncrypt(string plaintext, string key, string iv)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                using (TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider())
                {
                    tdsp.Mode = CipherMode.CBC;             //默认值 
                    tdsp.Padding = PaddingMode.PKCS7;       //默认值 

                    byte[] data = Encoding.UTF8.GetBytes(plaintext);
                    byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                    byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

                    using (var cryptor = tdsp.CreateEncryptor(keyBytes, ivBytes))
                    {
                        using (CryptoStream cStream = new CryptoStream(mStream, cryptor, CryptoStreamMode.Write))
                        {
                            await cStream.WriteAsync(data, 0, data.Length);
                            cStream.FlushFinalBlock();

                            byte[] ret = mStream.ToArray();

                            cStream.Close();
                            mStream.Close();

                            return ToHexString(ret);
                        }
                    }
                }
            }
        }

        public static async Task<string> DESDecrypt(string ciphertext, string key, string iv)
        {
            byte[] data = HexToBytes(ciphertext);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

            using (MemoryStream msDecrypt = new MemoryStream(data))
            {
                using (TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider())
                {
                    tdsp.Mode = CipherMode.CBC;
                    tdsp.Padding = PaddingMode.PKCS7;

                    using (var decryptor = tdsp.CreateDecryptor(keyBytes, ivBytes))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sReader = new StreamReader(csDecrypt, Encoding.UTF8))
                            {
                                return await sReader.ReadLineAsync();
                            }
                        }
                    }
                }
            }
        }

      

        /// <summary> 
        /// AES加密 
        /// </summary>
        public static string AESEncrypt(string value, string _aeskey = null)
        {
            if (string.IsNullOrEmpty(_aeskey))
            {
                _aeskey = AESKey;
            }

            byte[] keyArray = Encoding.UTF8.GetBytes(_aeskey);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(value);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary> 
        /// AES解密 
        /// </summary>
        public static string AESDecrypt(string value, string _aeskey = null)
        {
            try
            {
                if (string.IsNullOrEmpty(_aeskey))
                {
                    _aeskey = AESKey;
                }
                byte[] keyArray = Encoding.UTF8.GetBytes(_aeskey);
                byte[] toEncryptArray = Convert.FromBase64String(value);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary> 
        /// DES加密 
        /// </summary>
        public static string DESEncrypt(string value, string _deskey = null)
        {
            if (string.IsNullOrEmpty(_deskey))
            {
                _deskey = DESKey;
            }

            byte[] keyArray = Encoding.UTF8.GetBytes(_deskey);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(value);

            DESCryptoServiceProvider rDel = new DESCryptoServiceProvider();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary> 
        /// DES解密 
        /// </summary>
        public static string DESDecrypt(string value, string _deskey = null)
        {
            try
            {
                if (string.IsNullOrEmpty(_deskey))
                {
                    _deskey = DESKey;
                }
                byte[] keyArray = Encoding.UTF8.GetBytes(_deskey);
                byte[] toEncryptArray = Convert.FromBase64String(value);

                DESCryptoServiceProvider rDel = new DESCryptoServiceProvider();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
