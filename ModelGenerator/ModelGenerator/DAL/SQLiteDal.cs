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
    /// SQLite数据库DAL
    /// </summary>
    public class SQLiteDal : IDal
    {
        #region 获取所有表名
        /// <summary>
        /// 获取数据库名
        /// </summary>
        public List<DBTable> GetAllTables()
        {
            SQLiteHelper sqliteHelper = new SQLiteHelper();
            DataTable dt = sqliteHelper.Query("select tbl_name from sqlite_master where type='table'");

            List<DBTable> result = new List<DBTable>();
            foreach (DataRow dr in dt.Rows)
            {
                DBTable dbTable = new DBTable();
                dbTable.TableName = dr["tbl_name"].ToString();
                dbTable.Comments = string.Empty;
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
            SQLiteHelper sqliteHelper = new SQLiteHelper();
            DataTable dt = sqliteHelper.Query("PRAGMA table_info('" + tableName + "')");

            List<DBColumn> result = new List<DBColumn>();
            foreach (DataRow dr in dt.Rows)
            {
                DBColumn column = new DBColumn();
                column.ColumnName = dr["name"].ToString();
                column.NotNull = dr["notnull"].ToString() == "1" ? true : false;
                column.Comments = string.Empty;
                column.DataType = "string";
                column.DataScale = string.Empty;
                column.DataPrecision = string.Empty;
                if (dr["pk"].ToString() == "1")
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
            return "string";
        }
        #endregion

    }
}
