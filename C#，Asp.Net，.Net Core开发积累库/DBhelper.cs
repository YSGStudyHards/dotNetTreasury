using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Recruit_DAL
{

    /// <summary>
    /// 网上招聘数据库帮助类
    /// </summary>
    public class DBhelper
    {

        //创建数据库连接字符串
        public static string connstring = "Data Source=47.94.90.55;Initial Catalog=RecruitsDB;Persist Security Info=True;User ID=sa;Password=Lcp0210520";

        /// <summary>
        /// 数据分页所用的查询Total的数量
        /// </summary>
        /// <param name="sql">T-sql语句</param>
        /// <returns></returns>
        public static int UserExist(string sql)
        {

            int row=0;
            try
            {
                //数据集
                using (SqlConnection conn = new SqlConnection(connstring))
                {
                    //打开数据库连接
                    conn.Open();
                    //适配器填充数据
                    SqlCommand mand = new SqlCommand(sql, conn);
                    //判断是否有数据，exectuescalar是返回第一行影响的行数
                    row = (int)mand.ExecuteScalar();
                    conn.Close();

                }
            }
            catch { }
            return row;


        }
		/// <summary>
		/// 验证登录是否成功（并且判断是否存在该用户）
		/// </summary>
		/// <param name="sql"></param>
		/// <returns></returns>
        public static bool UserExistt(string sql)
        {
            bool flag = false;
            try
            {

            //数据集
                using (SqlConnection conn=new SqlConnection (connstring))
                {
                    //打开数据库连接对象
                    conn.Open();
                    DataSet ds = new DataSet();
                    SqlDataAdapter adp = new SqlDataAdapter(sql,conn);
                    adp.Fill(ds);
                    if (ds.Tables.Count==1&ds.Tables[0].Rows.Count==0)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                    
                }
            }
            catch
            {

            }
            return flag;


        }

        /// <summary>
        /// 判断是否有数据返回
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static bool Checkeof(string sql)
        {
            bool falg = false;
            try
            {
                DataSet ds = new DataSet();
                //数据库连接对象
                SqlConnection conn = new SqlConnection(connstring);

                //打开
                conn.Open();

                //创建适配器
                SqlDataAdapter adp = new SqlDataAdapter(sql, conn);

                //填充数据
                adp.Fill(ds);
                //关闭
                conn.Close();
            }
            catch (Exception)
            {


            }

            return falg;
        }
        /// <summary>
        /// 定义数据查询的方法，数据信息查询
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet Query(string sql)
        {
            //创建数据集存储数据
            DataSet ds = new DataSet();
            try
            {
                //创建数据库连接字符串
                SqlConnection conn = new SqlConnection(connstring);

                //打开
                conn.Open();

                //创建适配器
                SqlDataAdapter adp = new SqlDataAdapter(sql, conn);

                //填充数据
                adp.Fill(ds);
                //关闭
                conn.Close();


            }
            catch (Exception)
            {

                throw;
            }
            return ds;


        }
    
        /// <summary>
        /// 定义数据库数据增删改
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static bool Executesql(string sql)
        {
            bool falg = false;
            try
            {
                //创建数据库连接对象
                SqlConnection conn = new SqlConnection(connstring);
                //打开
                conn.Open();
                //执行t—sql语句
                SqlCommand mand = new SqlCommand(sql, conn);
                falg = mand.ExecuteNonQuery() > 0;

                //关闭
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return falg;

        }
        #region
        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// 用来读取数据，sqldatareder进行数据读取实现登录效果
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string strSQL)
        {
            SqlConnection connection = new SqlConnection(connstring);
            SqlCommand cmd = new SqlCommand(strSQL, connection);
            try
            {
                connection.Open();
                SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                throw e;
            }

        }
        #endregion
        //在DbHelper中添加RunProcedureDataTable方法： 
        //在DbHelper中添加RunProcedureDataTable方法： 
        public static DataTable RunProcedureDataTable(string stroreProcName, IDataParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(connstring))
            {
                DataTable dt = new DataTable();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, stroreProcName, parameters);
                sqlDA.Fill(dt);
                connection.Close();
                return dt;
            }
        }


        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)用于分页使用
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }



    }
}
