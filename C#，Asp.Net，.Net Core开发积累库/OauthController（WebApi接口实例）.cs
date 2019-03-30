using JJHL.Models.adlx_DB;
using JJHL.Utility;
using JJHL.Web;
using JJHL.Web.Controllers;
using JJHL.WEB.Api;
using JJHL.WEB.Areas.Mobile_User.Controllers;
using JJHL.WEB.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace JJHL.WEB.Controllers
{
    [ClientUserFilter]
    public class OauthController : BaseApiController
    {
        private ApiTools tool = new ApiTools();

        [AcceptVerbs("Get", "Post")]
        public HttpResponseMessage authorize(string client_id, string client_secret, string response_type, string scope, string redirect_uri)
        {
            if (string.IsNullOrEmpty(client_id))
            {
                return tool.MsgFormat(ResponseCode.操作失败, "client_id值为空", "查询失败");
            }
            if (string.IsNullOrEmpty(client_secret))
            {
                return tool.MsgFormat(ResponseCode.操作失败, "client_secret值为空", "查询失败");
            }
            if (string.IsNullOrEmpty(redirect_uri))
            {
                return tool.MsgFormat(ResponseCode.操作失败, "redirect_uri值为空", "查询失败");
            }

            string token = Encrypt(client_id + client_secret, "cda2f05e");

            string returnData = "access_token=" + token + "&state=xyz&token_type=example&expires_in=3600";
            string errorInfo = UserInfoSave.loginAPI(redirect_uri, returnData, method.GET, dataEncoding.UTF8);
            return tool.MsgFormat(ResponseCode.成功, "查询成功", errorInfo);
        }

        [HttpGet]
        public HttpResponseMessage customerInfo(string client_id, string client_secret, string access_token)
        {
            if (string.IsNullOrEmpty(client_id))
            {
                return tool.MsgFormat(ResponseCode.操作失败, "client_id值为空", "查询失败");
            }
            if (string.IsNullOrEmpty(client_secret))
            {
                return tool.MsgFormat(ResponseCode.操作失败, "client_secret值为空", "查询失败");
            }
            if (string.IsNullOrEmpty(access_token))
            {
                return tool.MsgFormat(ResponseCode.操作失败, "access_token值为空", "查询失败");
            }
            string suijisu = Guid.NewGuid().ToString().Substring(0, 8);
            int usersID = UserInfoSave.userId;
            string token = Encrypt(client_id + client_secret, "cda2f05e");
            if (token == access_token)
            {
                string jiamiUserId = Encrypt(usersID.ToString(), "cda2f05e");
                List<UserInfoForSuyin> userList = new List<UserInfoForSuyin>();
                userList.Add(new UserInfoForSuyin() { uuid = jiamiUserId });
                var JsonData = JsonConvert.SerializeObject(userList);
                return tool.MsgFormat(ResponseCode.成功, "查询成功", JsonData);
            }
            else
            {
                return tool.MsgFormat(ResponseCode.操作失败, "access_token错误", "查询失败");
            }
        }

        public string Get8Digits()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
            return String.Format("{0:D8}", random);
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="str">需要加密的</param>
        /// <param name="sKey">密匙</param>
        /// <returns></returns>
        private string Encrypt(string str, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(str);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);// 密匙
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);// 初始化向量
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var retB = Convert.ToBase64String(ms.ToArray());
            return retB;
        }

    }
}
