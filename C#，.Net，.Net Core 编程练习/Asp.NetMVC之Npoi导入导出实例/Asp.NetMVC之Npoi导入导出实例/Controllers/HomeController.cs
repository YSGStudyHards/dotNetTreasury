using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;

namespace Asp.NetMVC之Npoi导入导出实例.Controllers
{
    public class HomeController : Controller
    {
        //实例化上下文对象
        UserDBEntities UserEntites = new UserDBEntities();

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetUserInfo(int page = 1, int limit = 15)
        {
            try
            {
                //使用ef--skip().take()进行数据分页前面必须增加orderby，否则报错
                var List = UserEntites.UserInfo.OrderBy(p => p.Id).Skip((page - 1) * limit).Take(limit).ToList();

                return Json(new { code = 0, count = UserEntites.UserInfo.Count(), data = List }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {code=1,msg=ex.Message });
            }
        }

        /// <summary>
        /// 数据导入
        /// </summary>
        /// <returns></returns>
        public ActionResult UserImport()
        {
            return View();
        }

        /// <summary>
        /// 数据导入
        /// </summary>
        /// <param name="FileStram"></param>
        /// <returns></returns>
        public ActionResult DataImport(HttpPostedFileBase  file)
        {
            var message="";
            int Columns = 0;
            //判断是否提交excel文件
            var FileName = file.FileName.Split('.');
            if (file!=null&&file.ContentLength>0)
            {
                if (FileName[1]=="xls"||FileName[1]== "xlsx")
                {
                    //首先我们需要导入数据的话第一步其实就是先把excel数据保存到本地中，然后通过Npoi封装的方法去读取已保存的Excel数据
                    
                    string DictorysPath=Server.MapPath("~/Content/ExcelFiles/"+ DateTime.Now.ToString("yyyyMMdd"));
                    if (!System.IO.Directory.Exists(DictorysPath))
                    {
                        System.IO.Directory.CreateDirectory(DictorysPath);
                    }

                    file.SaveAs(System.IO.Path.Combine(DictorysPath,file.FileName));

                    //将Excel数据转化为DataTable数据源
                    DataTable  Dt=NpoiHelper.Import(System.IO.Path.Combine(DictorysPath, file.FileName), FileName[1]);
                    List<UserInfo> list = new List<UserInfo>();

                    for (int i = 0; i < Dt.Rows.Count; i++)
                    {
                        UserInfo U = new UserInfo();
                        //从行索引从1开始，标题除外
                        U.UserName = Dt.Rows[i][0].ToString();
                        U.Sex = Dt.Rows[i][1].ToString();
                        U.Phone = Dt.Rows[i][2].ToString();
                        U.Hobby = Dt.Rows[i][3].ToString();
                        list.Add(U);
                    }

                    //数据全部添加
                    UserEntites.Set<UserInfo>().AddRange(list);
                    Columns=UserEntites.SaveChanges();
                    if (Columns>0)
                    {
                        message = "导入成功";
                    }
                    else
                    {
                        message = "导入失败";
                    }

                }
                else
                {
                    message = "格式错误";
                }
            }
            else
            {
                message = "未找到需要导入的数据";
            }
            ViewBag.Columns = Columns;
            ViewBag.Message = message;
            return View();
        }

        /// <summary>
        /// 数据导出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Export()
        {
            try
            {
                //导出所有数据
                var All_ListData = UserEntites.UserInfo.ToList();

                //将list 转化为datatable类型
                var Dt = DatabaseOpreas._.ListToDataTable(All_ListData);

                NpoiHelper.Export(Dt, "用户信息", Server.MapPath("~/Content/Export.xls"));//这里的路径是需要写入你需要保存的文件格式的，不需要创建自动检测创建

                return Json(new {code=1,msg= "/Content/Export.xls" });
            }
            catch (Exception ex)
            {
                return Json(new {code=0,msg=ex.Message });    
            }

        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}