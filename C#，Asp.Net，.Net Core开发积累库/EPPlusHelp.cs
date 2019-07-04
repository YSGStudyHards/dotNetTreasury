﻿using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ThirdPartyHelp
{
    public class EPPlusHelp
    {
        #region 需要用到的实体
        private class XlsEntity
        {
            /// <summary>
            /// 实体属性名
            /// </summary>
            public string EntityName { get; set; }

            /// <summary>
            /// Excel列名称
            /// </summary>
            public string ColumnName { get; set; }

            /// <summary>
            /// 列下标
            /// </summary>
            public int ColumnIndex { get; set; }
        }
        #endregion

        #region 从Excel加载列表List
        /// <summary>
        /// 从Excel加载列表List
        /// </summary>
        /// <typeparam name="T">加载的类型</typeparam>
        /// <param name="worksheet">Excel的sheet</param>
        /// <returns></returns>
        public static List<T> LoadListFromExcel<T>(ExcelWorksheet worksheet) where T : new()
        {
            List<XlsEntity> xlsHeader = new List<XlsEntity>();
            List<T> resultList = new List<T>();
            Type nullableType = typeof(Nullable<>);

            int colStart = worksheet.Dimension.Start.Column;
            int colEnd = worksheet.Dimension.End.Column;
            int rowStart = worksheet.Dimension.Start.Row;
            int rowEnd = worksheet.Dimension.End.Row;

            PropertyInfo[] propertyInfoList = typeof(T).GetProperties();
            XlsEntity xlsEntity;

            #region 将实体和excel列标题进行对应绑定,添加到集合中

            for (int i = colStart; i <= colEnd; i++)
            {
                string columnName = worksheet.Cells[rowStart, i].Value.ToString();

                xlsEntity = xlsHeader.FirstOrDefault(e => e.ColumnName == columnName);

                for (int j = 0; j < propertyInfoList.Length; j++)
                {

                    if (xlsEntity != null && xlsEntity.ColumnName == columnName)
                    {
                        xlsEntity.ColumnIndex = i;
                        xlsHeader.Add(xlsEntity);
                    }
                    else if (propertyInfoList[j].Name == columnName)
                    {
                        xlsEntity = new XlsEntity
                        {
                            ColumnName = columnName,
                            EntityName = propertyInfoList[j].Name,
                            ColumnIndex = i
                        };
                        xlsHeader.Add(xlsEntity);
                        break;
                    }
                }
            }

            #endregion

            #region 根据对应的实体名列名的对应绑定就行值的绑定

            for (int row = rowStart + 1; row <= rowEnd; row++)
            {
                T result = new T();
                foreach (PropertyInfo p in propertyInfoList)
                {
                    var xlsRow = xlsHeader.FirstOrDefault(e => e.EntityName == p.Name);
                    if (xlsRow == null) continue;

                    ExcelRange cell = worksheet.Cells[row, xlsRow.ColumnIndex];
                    if (cell.Value == null) continue;

                    try
                    {
                        var value = GetValueByFormat(cell);
                        if (!p.PropertyType.IsGenericType)
                        {
                            p.SetValue(result, Convert.ChangeType(value, p.PropertyType), null);
                        }
                        else if (p.PropertyType.GetGenericTypeDefinition() == nullableType)
                        {
                            p.SetValue(result, Convert.ChangeType(value, Nullable.GetUnderlyingType(p.PropertyType)), null);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                resultList.Add(result);
            }

            #endregion
            return resultList;
        }
        #endregion

        /// <summary>
        /// Excel的sheet转换为DataTable
        /// </summary>
        /// <param name="worksheet">Excel的sheet</param>
        /// <returns></returns>
        public static DataTable WorksheetToDataTable(ExcelWorksheet worksheet)
        {
            //初始化DataTable
            DataTable dt = new DataTable(worksheet.Name);

            //获取Excel行列总数
            int colStart = worksheet.Dimension.Start.Column;
            int colEnd = worksheet.Dimension.End.Column;
            int rowStart = worksheet.Dimension.Start.Row;
            int rowEnd = worksheet.Dimension.End.Row;

            //设置DataTable的列名
            for (int i = colStart; i <= colEnd; i++)
            {
                ExcelRange cell = worksheet.Cells[rowStart, i];
                DataColumn dc = new DataColumn(cell.Value.ToString());
                dt.Columns.Add(dc);
            }

            //设置DataTable的值
            for (int row = rowStart + 1, i = 0; row <= rowEnd; row++, i++)
            {
                for (int col = colStart, j = 0; col <= colEnd; col++, j++)
                {
                    dt.Rows.Add();
                    dt.Rows[i][j] = GetValueByFormat(worksheet.Cells[row, col]);
                }
            }

            return dt;
        }


        /// <summary>
        /// Excel的sheet转换为DataSet
        /// </summary>
        /// <param name="package">Excel package</param>
        /// <returns></returns>
        public static DataSet ExcelPackageToDataSet(ExcelPackage package)
        {
            DataSet dataSet = new DataSet();
            foreach (var item in package.Workbook.Worksheets)
            {
                DataTable dt = WorksheetToDataTable(item);
                dataSet.Tables.Add(dt);
            }
            return dataSet;
        }

        /// <summary>
        /// 根据格式获取值
        /// </summary>
        /// <returns></returns>
        private static object GetValueByFormat(ExcelRange excelRange)
        {
            string format = excelRange.Style.Numberformat.Format;
            if (string.IsNullOrEmpty(format)) return excelRange.Value;

            if (format.Contains("@"))
            {
                return excelRange.GetValue<string>();
            }
            else if (format.IndexOf("yyyy", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return excelRange.GetValue<DateTime?>();
            }
            else if (format.Contains("0") || format.Contains("#"))
            {
                if (format.Contains("."))
                {
                    return excelRange.GetValue<decimal?>();
                }
                else
                {
                    return excelRange.GetValue<long?>();
                }
            }
            return excelRange.Value;
        }

        /// <summary>
        /// 获取类型对应的格式
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetTypeFormat(Type type)
        {
            if (
                type == typeof(string) ||
                type == typeof(char)
                )
            {
                return "@";
            }
            else if (
                type == typeof(DateTime) ||
                type == typeof(DateTime?) ||
                type == typeof(DateTimeOffset) ||
                type == typeof(DateTimeOffset?)
                )
            {
                return "yyyy/m/d h:mm";
            }
            else if (
                type == typeof(int) ||
                type == typeof(int?) ||
                type == typeof(long) ||
                type == typeof(long?)
                )
            {
                return "0";
            }
            else if (
                type == typeof(double) ||
                type == typeof(double?) ||
                type == typeof(decimal) ||
                type == typeof(decimal?) ||
                type == typeof(float) ||
                type == typeof(float?)
                )
            {
                return "0.00";
            }
            return string.Empty;
        }

        /// <summary>
        /// 按照类型设置所有列的格式
        /// </summary>
        /// <param name="package"></param>
        public static void SetAllColumnFormat<T>(ExcelWorksheet worksheet)
        {
            if (typeof(T) == typeof(DataTable)) return;
            PropertyInfo[] props = typeof(T).GetProperties();
            int i = 1;
            foreach (PropertyInfo prop in props)
            {
                worksheet.Column(i).Style.Numberformat.Format = GetTypeFormat(prop.PropertyType);
                ++i;
            }
        }

        /// <summary>
        /// 根据DataTable按照类型设置所有列的格式
        /// </summary>
        public static void SetAllColumnFormatByDataTable(ExcelWorksheet worksheet, DataTable dt)
        {
            IEnumerable<DataColumn> columns = dt.Columns.Cast<DataColumn>();
            int i = 1;
            foreach (DataColumn col in columns)
            {
                worksheet.Column(i).Style.Numberformat.Format = GetTypeFormat(col.DataType);
                ++i;
            }
        }

    }
}
