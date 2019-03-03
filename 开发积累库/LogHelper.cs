using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Web;

namespace Framework.Core.Log
{
    public class LogHelp
    {
        private string Id = Guid.NewGuid().ToString();
        private string _LogDirectory = "D:\\Log\\";

        public string RequestId
        {
            get
            {
                if (!HttpContext.Current.Items.Contains((object)this.Id))
                    HttpContext.Current.Items.Add((object)this.Id, (object)Guid.NewGuid().ToString());
                return HttpContext.Current.Items[(object)this.Id].ToString();
            }
        }

        public string LogDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(this._LogDirectory))
                    this._LogDirectory = HttpContext.Current.Server.MapPath("\\Log\\");
                return this._LogDirectory;
            }
            set
            {
                this._LogDirectory = value;
            }
        }

        public LogHelp(string LogDirectory = "")
        {
            this.LogDirectory = LogDirectory;
        }

        public void Debug(params object[] os)
        {
            this.Write(LogLevel.Debug, os);
        }

        public void Info(params object[] os)
        {
            this.Write(LogLevel.Info, os);
        }

        public void Warn(params object[] os)
        {
            this.Write(LogLevel.Warn, os);
        }

        public void Error(params object[] os)
        {
            this.Write(LogLevel.Error, os);
        }

        public void Log(params object[] os)
        {
            this.Write(LogLevel.Log, os);
        }

        public void Write(LogLevel log, params object[] os)
        {
            string str1 = DateTimeExtension.ToUinxTime(DateTime.Now).ToString() + (object)" " + (string)(object)log;
            string str2 = "null";
            if (HttpContext.Current != null)
                str2 = this.RequestId;
            string str3 = str1 + " " + this.Id + " " + str2 + " " + JArray.FromObject((object)os).ToString().Replace("\n", "").Replace("\r", "").Replace(" ", "");
            try
            {
                StreamWriter streamWriter = new StreamWriter(this.LogDirectory + DateTime.Now.Date.ToString("yyyyMMdd") + ".logdata", true, Encoding.UTF8);
                streamWriter.WriteLine(str3);
                streamWriter.Flush();
                streamWriter.Close();
            }
            catch
            {
            }
        }
    }
}
