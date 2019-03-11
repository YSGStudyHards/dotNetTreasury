using System.Web;
using System.Web.Mvc;

namespace Asp.NetMVC之Npoi导入导出实例
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
