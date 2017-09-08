using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DbLogger
{
    public class ConnectionStrings
    {
        private static readonly string EncryptKey;
        static ConnectionStrings()
        {
            EncryptKey = ConfigurationManager.AppSettings["EncryptKey"];
        }

        private static string _logDb;
        public static string LogDb
        {
            get
            {
                return _logDb ?? (_logDb = Decrypt(ConfigurationManager.ConnectionStrings["LogDb"].ConnectionString,
                           EncryptKey));
            }
        }


        /// <summary>
        /// 使用指定密钥解密
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        private static string Decrypt(string encrypted, string key)
        {
            var encoding = Encoding.Default;
            var encryptedBytes = Convert.FromBase64String(encrypted);
            var keyBytes = Encoding.Default.GetBytes(key);

            var des = new TripleDESCryptoServiceProvider();
            var hashmd5 = new MD5CryptoServiceProvider();
            des.Key = hashmd5.ComputeHash(keyBytes);
            hashmd5 = null;
            des.Mode = CipherMode.ECB;
            var bytes = des.CreateDecryptor().TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return encoding.GetString(bytes);
        }
    }
}
