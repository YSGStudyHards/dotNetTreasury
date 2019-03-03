using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SNH.Utility
{
    public class CryptoHelper
    {
        /// <summary>  
        ///AES加密（加密步骤）  
        ///1，加密字符串得到2进制数组；  
        ///2，将2禁止数组转为16进制；  
        ///3，进行base64编码  
        /// </summary>  
        /// <param name="toEncrypt">要加密的字符串</param>  
        /// <param name="key">密钥</param>  
        public static String AES_Encrypt(String toEncrypt, String key="12345678")
        {
            Byte[] _Key = Encoding.ASCII.GetBytes(key);
            Byte[] _Source = Encoding.UTF8.GetBytes(toEncrypt);

            Aes aes = Aes.Create("AES");
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _Key;
            ICryptoTransform cTransform = aes.CreateEncryptor();
            Byte[] cryptData = cTransform.TransformFinalBlock(_Source, 0, _Source.Length);
            String HexCryptString = Hex_2To16(cryptData);
            Byte[] HexCryptData = Encoding.UTF8.GetBytes(HexCryptString);
            String CryptString = Convert.ToBase64String(HexCryptData);
            return CryptString;
        }

        /// <summary>  
        /// AES解密（解密步骤）  
        /// 1，将BASE64字符串转为16进制数组  
        /// 2，将16进制数组转为字符串  
        /// 3，将字符串转为2进制数据  
        /// 4，用AES解密数据  
        /// </summary>  
        /// <param name="encryptedSource">已加密的内容</param>  
        /// <param name="key">密钥</param>  
        public static String AES_Decrypt(string encryptedSource, string key= "12345678")
        {
            Byte[] _Key = Encoding.ASCII.GetBytes(key);
            Aes aes = Aes.Create("AES");
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _Key;
            ICryptoTransform cTransform = aes.CreateDecryptor();

            Byte[] encryptedData = Convert.FromBase64String(encryptedSource);
            String encryptedString = Encoding.UTF8.GetString(encryptedData);
            Byte[] _Source = Hex_16To2(encryptedString);
            Byte[] originalSrouceData = cTransform.TransformFinalBlock(_Source, 0, _Source.Length);
            String originalString = Encoding.UTF8.GetString(originalSrouceData);
            return originalString;
        }

        private static String Hex_2To16(Byte[] bytes)
        {
            String hexString = String.Empty;
            Int32 iLength = 65535;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                if (bytes.Length < iLength)
                {
                    iLength = bytes.Length;
                }

                for (int i = 0; i < iLength; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }

        private static Byte[] Hex_16To2(String hexString)
        {
            if ((hexString.Length % 2) != 0)
            {
                hexString += " ";
            }
            Byte[] returnBytes = new Byte[hexString.Length / 2];
            for (Int32 i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

        public static string DES_Encrypt(string original, string key)
        {
            byte[] buff = System.Text.Encoding.Default.GetBytes(original);
            byte[] kb = System.Text.Encoding.Default.GetBytes(key);
            return Convert.ToBase64String(DES_Encrypt(buff, kb));
        }

        /// <summary>
        /// 使用给定密钥解密
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="encoding">字符编码方案</param>
        /// <returns>明文</returns>
        public static string DES_Decrypt(string encrypted, string key, Encoding encoding)
        {
            try
            {
                byte[] buff = Convert.FromBase64String(encrypted);
                byte[] kb = System.Text.Encoding.Default.GetBytes(key);
                return encoding.GetString(DES_Decrypt(buff, kb));
            }
            catch (Exception ex)
            {
                //LogHelper.WriteException("DES_Decrypt", ex);
            }
            return encrypted;
        }

        private static byte[] DES_Encrypt(byte[] original, byte[] key)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = MakeMD(key);
            des.Mode = CipherMode.ECB;

            return des.CreateEncryptor().TransformFinalBlock(original, 0, original.Length);
        }


        /// <summary>
        /// 使用给定密钥解密数据
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        private static byte[] DES_Decrypt(byte[] encrypted, byte[] key)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = MakeMD(key);
            des.Mode = CipherMode.ECB;

            return des.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
        }

        /// <summary>
        /// 生成MD摘要
        /// </summary>
        /// <param name="original">数据源</param>
        /// <returns>摘要</returns>
        public static byte[] MakeMD(byte[] original)
        {
            using (MD5CryptoServiceProvider hashmd = new MD5CryptoServiceProvider())
            {
                return hashmd.ComputeHash(original);
            }
        }

        #region des实现

        /// <summary>
        /// Des默认密钥向量
        /// </summary>
        public static byte[] DesIv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// Des加解密钥必须8位
        /// </summary>
        public const string DesKey = "deskey8w";
        /// <summary>
        /// 获取Des8位密钥
        /// </summary>
        /// <param name="key">Des密钥字符串</param>
        /// <returns>Des8位密钥</returns>
        static byte[] GetDesKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "Des密钥不能为空");
            }
            if (key.Length > 8)
            {
                key = key.Substring(0, 8);
            }
            if (key.Length < 8)
            {
                // 不足8补全
                key = key.PadRight(8, '0');
            }
            return Encoding.UTF8.GetBytes(key);
        }
        /// <summary>
        /// Des加密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="key">des密钥，长度必须8位</param>
        /// <param name="iv">密钥向量</param>
        /// <returns>加密后的字符串</returns>
        public static string EncryptDes(string source, string key, byte[] iv)
        {
            using (DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider())
            {
                byte[] rgbKeys = GetDesKey(key),
                    rgbIvs = iv,
                    inputByteArray = Encoding.UTF8.GetBytes(source);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, desProvider.CreateEncryptor(rgbKeys, rgbIvs), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cryptoStream.FlushFinalBlock();
                        // 1.第一种
                        return Convert.ToBase64String(memoryStream.ToArray());

                        // 2.第二种
                        //StringBuilder result = new StringBuilder();
                        //foreach (byte b in memoryStream.ToArray())
                        //{
                        //    result.AppendFormat("{0:X2}", b);
                        //}
                        //return result.ToString();
                    }
                }
            }
        }
        /// <summary>
        /// Des解密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="key">des密钥，长度必须8位</param>
        /// <param name="iv">密钥向量</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptDes(string source, string key, byte[] iv)
        {
            using (DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider())
            {
                byte[] rgbKeys = GetDesKey(key),
                    rgbIvs = iv,
                    inputByteArray = Convert.FromBase64String(source);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, desProvider.CreateDecryptor(rgbKeys, rgbIvs), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cryptoStream.FlushFinalBlock();
                        return Encoding.UTF8.GetString(memoryStream.ToArray());
                    }
                }
            }
        }

        #endregion

        #region aes实现

        /// <summary>
        /// Aes加解密钥必须32位
        /// </summary>
        public static string AesKey = "asekey32w";
        /// <summary>
        /// 获取Aes32位密钥
        /// </summary>
        /// <param name="key">Aes密钥字符串</param>
        /// <returns>Aes32位密钥</returns>
        static byte[] GetAesKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "Aes密钥不能为空");
            }
            if (key.Length < 32)
            {
                // 不足32补全
                key = key.PadRight(32, '0');
            }
            if (key.Length > 32)
            {
                key = key.Substring(0, 32);
            }
            return Encoding.UTF8.GetBytes(key);
        }
        /// <summary>
        /// Aes加密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="key">aes密钥，长度必须32位</param>
        /// <returns>加密后的字符串</returns>
        public static string EncryptAes(string source, string key)
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = GetAesKey(key);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor())
                {
                    byte[] inputBuffers = Encoding.UTF8.GetBytes(source);
                    byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                    aesProvider.Clear();
                    aesProvider.Dispose();
                    return Convert.ToBase64String(results, 0, results.Length);
                }
            }
        }
        /// <summary>
        /// Aes解密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="key">aes密钥，长度必须32位</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptAes(string source, string key)
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = GetAesKey(key);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor())
                {
                    byte[] inputBuffers = Convert.FromBase64String(source);
                    byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                    aesProvider.Clear();
                    return Encoding.UTF8.GetString(results);
                }
            }
        }

        #endregion
    }
}
