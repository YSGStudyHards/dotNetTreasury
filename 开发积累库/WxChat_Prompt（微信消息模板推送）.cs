using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using WxExpirationNotice.WxExpirationModel.Bo;

namespace WxExpirationNotice
{

    public class ReturnMsg
    {
        public string errcode{get;set;}
        public string errmsg{get;set;}
        public string msgid{get;set;}
    }
    /// <summary>
    /// 微信提醒
    /// </summary>
    public class WxChat_Prompt
    {
        /// <summary>
        /// 消息推送
        /// </summary>
        /// <param name="Access_token"></param>
        /// <param name="Openid"></param>
        /// <returns></returns>
        public string MsgPush(string Access_token,string Openid)
        {

            var Tuisong = new fx_weixintuisong { 
             WxtuisongID=1
            }.SelectObject();
            var userfollow=new fx_userfollow{WxOpendID=Openid}.SelectObject();
            //touser用户oppenid template_id消息模板id
            string Contentmsg = "{\"touser\":\"" + Openid + "\",\"template_id\":\"" + Tuisong.Template_id + "\",\"topcolor\":\"#FF0000\",\"data\":{\"first\":{\"value\":\"" + Tuisong.first + "\",\"color\":\"#173177\"},\"keyword1\":{\"value\":\"" + Tuisong.Name + "\",\"color\":\"#173177\"},\"keyword2\":{\"value\":\"" + Tuisong.Type + "\",\"color\":\"#173177\"},\"keyword3\":{\"value\":\"" + DateTime.Now + "\",\"color\":\"#173177\"},\"remark\":{\"value\":\""+Tuisong.remark+"\",\"color\":\"#173177\"}}}";
            string Result = Msg(Access_token,Contentmsg);
            return Result;
        }
        public string Msg(string access_token, string Contentmsg)
        {
            string promat = "";
            //需要提交的数据
            byte[] bs = Encoding.UTF8.GetBytes(Contentmsg);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token="+access_token+"");
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }
            HttpWebResponse respon = (HttpWebResponse)req.GetResponse();
            Stream stream = respon.GetResponseStream();
            using (StreamReader reader=new StreamReader (stream,Encoding.UTF8))
            {
                promat = reader.ReadToEnd();
            }
            ReturnMsg y = JsonConvert.DeserializeObject<ReturnMsg>(promat);
            promat = y.errmsg;
            string starttime = "微信消息推送结果" +y.errmsg +"错误提示"+y.errcode;
            WriteLog.AddLgoToTXT(starttime);
            string Resule_fenhan = "------------------------------------------------";
            WriteLog.AddLgoToTXT(Resule_fenhan);
            return promat;
 
        }

    }
}
