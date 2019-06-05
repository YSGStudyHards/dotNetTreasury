using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AggregatePayment.SwiftpassComon
{
    public class SHA256Util
    {
        private static SHA256Util _objUtil;

        public static SHA256Util _
        {
            get=>_objUtil??(new SHA256Util());
            set => _objUtil = value;
        }

        /*
         * 知识点：
         * ToString("x2");字符串格式控制码转化为16进制，每个字符两位数
         *
         */

        /// <summary>
        /// 获取Sha256加密数据，美团sign加密方式使用的就是sah256
        /// </summary>
        /// <param name="plaintext">加密字符串</param>
        /// <returns></returns>
        public  string GetHashSha256(string plaintext)
        {
            byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(plaintext);
            SHA256 sha256 = new SHA256CryptoServiceProvider();

            byte[] retVal = sha256.ComputeHash(bytValue);
            StringBuilder sb = new StringBuilder();
            foreach (var t in retVal)
            {
                sb.Append(t.ToString("x2"));
            }
            return sb.ToString();
        }



        /// <summary>
        /// SHA256加密【HMAC-SHA256】微信提供的，与java统一推荐使用该方法
        /// </summary>
        /// <param name="plaintext">以key1=value1&key2=value2格式拼接，并且最后待&key=美团签名密钥的不为空的字符串</param>
        /// <param name="salt">美团开放平台中的签名密钥</param>
        /// <returns></returns>
        public string Sha256Encryption(string plaintext, string salt)
        {
            string result = "";
            var enc = Encoding.UTF8;//修改Default为UTF8，否则提示签名错误
            byte[]
                baText2BeHashed = enc.GetBytes(plaintext),
                baSalt = enc.GetBytes(salt);
            System.Security.Cryptography.HMACSHA256 hasher = new HMACSHA256(baSalt);//使用指定的密钥数据初始化 <see cref="T:System.Security.Cryptography.HMACSHA256" /> 类的新实例。
            byte[] baHashedText = hasher.ComputeHash(baText2BeHashed);
            result = string.Join("", baHashedText.ToList().Select(b => b.ToString("x2")).ToArray());
            return result;
        }


        /// <summary>
        /// HmacSha256加密方式，保持与java加密风格统一，转化为16进制每个字母两位字符码的格式
        /// </summary>
        /// <param name="plaintext"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public string HmacSha256(string plaintext, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(plaintext);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return string.Join("", hashmessage.ToList().Select(b => b.ToString("x2")).ToArray());
            }
        }


        /// <summary>
        ///  HmacSha256加密方式，保持与java加密风格统一【不对加密字符进行字符格式转化】
        /// </summary>
        /// <param name="data"></param>
        /// <param name="secretAccessKey"></param>
        /// <returns></returns>
        public string CreateSignature(string data, string secretAccessKey)
        {
            byte[] secretKey = Encoding.UTF8.GetBytes(secretAccessKey);
            HMACSHA256 hmac = new HMACSHA256(secretKey);//使用指定密钥初始化HmacSha256类的实例
            hmac.Initialize();//初始化
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] rawHmac = hmac.ComputeHash(bytes);
           
            return Convert.ToBase64String(rawHmac);
        }

        #region javaHmacSha256加密

        //import java.util.*;
        //import javax.crypto.*;
        //import javax.crypto.spec.*;

        //public class Test
        //{
        //    public static void main(String[] args) throws Exception
        //    {
        //    String secretAccessKey = "mykey";
        //    String data = "my data";
        //    byte[] secretKey = secretAccessKey.getBytes();
        //    SecretKeySpec signingKey = new SecretKeySpec(secretKey, "HmacSHA256");
        //    Mac mac = Mac.getInstance("HmacSHA256");
        //    mac.init(signingKey);
        //    byte[] bytes = data.getBytes();
        //    byte[] rawHmac = mac.doFinal(bytes);
        //    System.out.println(Base64.encodeBytes(rawHmac));
        //     }
        //}

        #endregion


    }
}
