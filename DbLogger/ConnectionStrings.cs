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
        private static readonly string _encryptKey;
        static ConnectionStrings()
        {
            _encryptKey = ConfigurationManager.AppSettings["EncryptKey"];
        }

        static string _logDb;
        public static string LogDb
        {
            get
            {
                if (_logDb == null)
                {
                    _logDb = Decrypt(ConfigurationManager.ConnectionStrings["LogDb"].ConnectionString, _encryptKey);
                }
                return _logDb;
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
            Encoding encoding = Encoding.Default;
            byte[] encryptedBytes = Convert.FromBase64String(encrypted);
            byte[] keyBytes = Encoding.Default.GetBytes(key);

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            des.Key = hashmd5.ComputeHash(keyBytes);
            hashmd5 = null;
            des.Mode = CipherMode.ECB;
            byte[] bytes = des.CreateDecryptor().TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return encoding.GetString(bytes);
        }
    }
}
