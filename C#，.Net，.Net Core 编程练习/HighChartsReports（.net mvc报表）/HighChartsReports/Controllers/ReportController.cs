using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HighChartsReports.Controllers
{

    /// <summary>
    /// 自定义数据类型
    /// </summary>
    public class MyReportDatas
    {
        public string time { get; set; }
        public int Count { get; set; }
    }


    public class ReportController : Controller
    {
        //
        // GET: /Report/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 曲线图
        /// </summary>
        /// <returns></returns>
        public ActionResult Diagram()
        {
            return View();
        }

        /// <summary>
        /// 柱状图
        /// </summary>
        /// <returns></returns>
        public ActionResult BarGraph()
        {
            return View();
        }

        /// <summary>
        /// 饼图
        /// </summary>
        /// <returns></returns>
        public ActionResult Piechart()
        {
            return View();
        }


        /// <summary>
        /// 获取数据接口
        /// </summary>
        /// <param name="BeformDays">前多少天</param>
        /// <param name="Type">请求类型</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetDataList(int BeformDays,int Type)
        {
            //时间当然大家可以根据自己需要统计的数据进行整合我这里是用来演示就没有用数据库了
            var Time = new List<String>();
            //数量
            var Count = new List<int>();
            var PieData=new List<MyReportDatas>();
            //Type为1表示曲线和柱状数据
            if (Type==1)
            {
                for (int i = 0; i < BeformDays; i++)
                {
                    Time.Add(DateTime.Now.AddDays(-BeformDays).ToShortDateString());
                    Count.Add(i + 1);
                }    
            }
            else//饼状图
            {
                for (int i = 0; i < BeformDays; i++)
                {
                    var my = new MyReportDatas();
                    my.Count = i + 1;
                    my.time = DateTime.Now.AddDays(-BeformDays).ToShortDateString();
                    PieData.Add(my);
                } 
            }
           

            var Obj = new 
            { 
                Times=Time,
                Counts=Count,
                PieDatas = PieData
            };

            return Json(Obj,JsonRequestBehavior.AllowGet);
        }

    }
}
