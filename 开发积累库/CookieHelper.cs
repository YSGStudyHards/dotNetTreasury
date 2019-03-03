using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace SNH.Utility
{
    public class CookieHelper
    {
        /// <summary>
        /// 清除指定Cookie
        /// </summary>
        /// <param name="cookiename">cookiename</param>
        public static void ClearCookie(string cookiename)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookiename];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddYears(-3);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        /// <summary>
        /// 获取指定Cookie值
        /// </summary>
        /// <param name="cookiename">cookiename</param>
        /// <returns></returns>
        public static string GetCookieValue(string cookiename)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookiename];
            string str = string.Empty;
            if (cookie != null)
            {

                str = CryptoHelper.DecryptDes(cookie.Value, CryptoHelper.DesKey, CryptoHelper.DesIv);
            }
            return str;
        }
        /// <summary>
        /// 添加一个Cookie（24小时过期）
        /// </summary>
        /// <param name="cookiename"></param>
        /// <param name="cookievalue"></param>
        public static void SetCookie(string cookiename, string cookievalue)
        {
            SetCookie(cookiename, CryptoHelper.EncryptDes(cookievalue, CryptoHelper.DesKey, CryptoHelper.DesIv), DateTime.Now.AddDays(7.0));
        }
        /// <summary>
        /// 添加一个Cookie
        /// </summary>
        /// <param name="cookiename">cookie名</param>
        /// <param name="cookievalue">cookie值</param>
        /// <param name="expires">过期时间 DateTime</param>
        public static void SetCookie(string cookiename, string cookievalue, DateTime expires)
        {
            HttpCookie cookie = new HttpCookie(cookiename)
            {
                Value = cookievalue,
                Expires = expires
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }


        #region 额外
        public static string ListToStr(List<string> list)
        {
            string str = string.Empty;
            foreach (var item in list)
            {
                str += item + ",";
            }
            return str.Trim(',');
        }

        public static List<string> StrToList(string str)
        {
            string[] strArr = str.Split(',');
            List<string> list = new List<string>();
            foreach (string item in strArr)
            {
                list.Add(item);
            }
            return list;
        }

        #endregion
    }
}
