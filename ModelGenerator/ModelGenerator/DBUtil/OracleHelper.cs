using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;

namespace DBUtil
{
    /// <summary>
    /// Oracle操作类
    /// </summary>
    public class OracleHelper
    {
        #region 变量
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"].ToString();
        #endregion

        #region Exists
        public bool Exists(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        #endregion

        #region 执行SQL语句，返回影响的记录数
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        connection.Close();
                        throw new Exception(e.Message);
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        #endregion

        #region 执行查询语句，返回DataTable
        /// <summary>
        /// 执行查询语句，返回DataTable
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataTable</returns>
        public DataTable Query(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
                    DataSet ds = new DataSet();
                    command.Fill(ds, "ds");
                    return ds.Tables[0];
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        #endregion

        #region 获取用户名
        /// <summary>
        /// 获取用户名
        /// </summary>
        public string GetUser()
        {
            int start = connectionString.IndexOf("User Id=") + 8;
            string str = connectionString.Substring(start);
            int length = str.IndexOf("Password=");
            str = str.Substring(0, length);
            return str.Replace(";", "").ToUpper();
        }
        #endregion

    }
}
