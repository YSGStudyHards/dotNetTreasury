using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// 数据库操作
    /// </summary>
    public class DatabaseOpreas
    {
        private static DatabaseOpreas obj;

        public DatabaseOpreas()
        {

        }

        public static DatabaseOpreas _
        {
            get
            {
                if (obj == null)
                {
                    obj = new DatabaseOpreas();
                }
                return obj;
            }
            set
            {
                value = obj;
            }
        }

        /// <summary>
        /// 将list集合转换成datatable
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public  System.Data.DataTable ListToDataTable(IList list)
        {
            System.Data.DataTable result = new System.Data.DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    //获取类型
                    Type colType = pi.PropertyType;
                    //当类型为Nullable<>时
                    if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }
                    result.Columns.Add(pi.Name, colType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
    }
}
