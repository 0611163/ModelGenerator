using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtil;
using ModelGenerator.Models;

namespace ModelGenerator.DAL
{
    /// <summary>
    /// MySql数据库DAL
    /// </summary>
    public class MySqlDal : IDal
    {
        #region 获取所有表信息
        /// <summary>
        /// 获取所有表信息
        /// </summary>
        public List<DBTable> GetAllTables()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ToString();
            int start = connectionString.IndexOf("database=") + 9;
            int end = connectionString.IndexOf("user id=");
            string owner = connectionString.Substring(start, end - start).Replace(";", "").ToUpper();
            MySqlHelper dbHelper = new MySqlHelper();
            DataTable dt = dbHelper.Query(string.Format(@"
                SELECT TABLE_NAME as TABLE_NAME,TABLE_COMMENT as COMMENTS 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = '{0}'", owner));

            List<DBTable> result = new List<DBTable>();
            foreach (DataRow dr in dt.Rows)
            {
                DBTable dbTable = new DBTable();
                dbTable.TableName = dr["TABLE_NAME"].ToString();
                dbTable.Comments = dr["COMMENTS"].ToString();
                result.Add(dbTable);
            }

            return result;
        }
        #endregion

        #region 获取表的所有字段名及字段类型
        /// <summary>
        /// 获取表的所有字段名及字段类型
        /// </summary>
        public List<DBColumn> GetAllColumns(string tableName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ToString();
            int start = connectionString.IndexOf("database=") + 9;
            int end = connectionString.IndexOf("user id=");
            string owner = connectionString.Substring(start, end - start).Replace(";", "").ToUpper();
            MySqlHelper dbHelper = new MySqlHelper();
            DataTable dt = dbHelper.Query(string.Format(@"
                select * 
                from INFORMATION_SCHEMA.Columns 
                where table_name='{0}' 
                and table_schema='{1}'", tableName, owner));

            List<DBColumn> result = new List<DBColumn>();
            foreach (DataRow dr in dt.Rows)
            {
                DBColumn column = new DBColumn();
                column.ColumnName = dr["COLUMN_NAME"].ToString();
                column.NotNull = dr["IS_NULLABLE"].ToString() == "NO" ? true : false;
                column.Comments = dr["COLUMN_COMMENT"].ToString();
                string dataType = dr["COLUMN_TYPE"].ToString();
                int pos = dataType.IndexOf("(");
                if (pos != -1) dataType = dataType.Substring(0, pos);
                column.DataType = dataType;
                column.DataScale = dr["CHARACTER_MAXIMUM_LENGTH"].ToString();
                column.DataPrecision = dr["NUMERIC_SCALE"].ToString();
                if (dr["COLUMN_KEY"].ToString() == "PRI")
                {
                    column.PrimaryKey = true;
                }
                else
                {
                    column.PrimaryKey = false;
                }

                result.Add(column);
            }

            return result;
        }
        #endregion

        #region 类型转换
        /// <summary>
        /// 类型转换
        /// </summary>
        public string ConvertDataType(DBColumn column)
        {
            string data_type = "string";
            switch (column.DataType)
            {
                case "tinyint":
                case "smallint":
                case "int":
                    if (column.NotNull)
                    {
                        data_type = "int";
                    }
                    else
                    {
                        data_type = "int?";
                    }
                    break;
                case "bigint":
                    if (column.NotNull)
                    {
                        data_type = "long";
                    }
                    else
                    {
                        data_type = "long?";
                    }
                    break;
                case "float":
                case "double":
                case "decimal":
                    if (column.NotNull)
                    {
                        data_type = "decimal";
                    }
                    else
                    {
                        data_type = "decimal?";
                    }
                    break;
                case "char":
                    data_type = "string";
                    break;
                case "varchar":
                    data_type = "string";
                    break;
                case "text":
                    data_type = "string";
                    break;
                case "longtext":
                    data_type = "string";
                    break;
                case "longblob":
                case "blob":
                    data_type = "byte[]";
                    break;
                case "time":
                case "date":
                case "datetime":
                    if (column.NotNull)
                    {
                        data_type = "DateTime";
                    }
                    else
                    {
                        data_type = "DateTime?";
                    }
                    break;
                default:
                    throw new Exception("Model生成器未实现数据库字段类型" + column.DataType + "的转换");
            }
            return data_type;
        }
        #endregion

    }
}
