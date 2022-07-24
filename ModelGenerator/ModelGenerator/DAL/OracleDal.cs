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
    /// Oracle数据库DAL
    /// </summary>
    public class OracleDal : IDal
    {
        #region 获取所有表信息
        /// <summary>
        /// 获取所有表信息
        /// </summary>
        public List<DBTable> GetAllTables()
        {
            OracleHelper dbHelper = new OracleHelper();
            DataTable dt = dbHelper.Query(@"
                select a.TABLE_NAME,b.COMMENTS 
                from user_tables a,user_tab_comments b 
                WHERE a.TABLE_NAME=b.TABLE_NAME");

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
            string connectionString = ConfigurationManager.ConnectionStrings["OracleConnection"].ToString();
            int start = connectionString.IndexOf("User Id=") + 8;
            int end = connectionString.IndexOf("Password=");
            string owner = connectionString.Substring(start, end - start).Replace(";", "").ToUpper();
            OracleHelper dbHelper = new OracleHelper();
            DataTable dt = dbHelper.Query(string.Format(@"
                select a.*,b.COMMENTS
                from user_tab_columns a, user_col_comments b
                where a.TABLE_NAME=b.TABLE_NAME and a.COLUMN_NAME=b.COLUMN_NAME 
                and a.TABLE_NAME='{0}'
                order by column_id", tableName));

            List<DBColumn> result = new List<DBColumn>();
            foreach (DataRow dr in dt.Rows)
            {
                DBColumn column = new DBColumn();
                column.ColumnName = dr["COLUMN_NAME"].ToString();
                column.NotNull = dr["NULLABLE"].ToString() == "N" ? true : false;
                column.Comments = dr["COMMENTS"].ToString();
                column.DataType = dr["DATA_TYPE"].ToString();
                column.DataScale = dr["DATA_SCALE"].ToString();
                column.DataPrecision = dr["DATA_PRECISION"].ToString();

                DataTable dt2 = dbHelper.Query(string.Format(@"
                    select *
                    from user_cons_columns c,user_constraints d
                    where c.owner='{2}' and c.constraint_name=d.constraint_name
                    and c.TABLE_NAME='{0}' and c.COLUMN_NAME='{1}'", tableName, dr["COLUMN_NAME"].ToString(), owner));
                if (dt2.Rows.Count > 0)
                {
                    foreach (DataRow dr2 in dt2.Rows)
                    {
                        if (dr2["CONSTRAINT_TYPE"].ToString() == "P")
                        {
                            column.PrimaryKey = true;
                            break;
                        }
                    }
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
                case "NUMBER":
                    if (column.DataScale.Trim() == "0")
                    {
                        if (column.DataScale.Trim() != "" && int.Parse(column.DataScale.Trim()) > 9)
                        {
                            if (column.NotNull)
                            {
                                data_type = "long";
                            }
                            else
                            {
                                data_type = "long?";
                            }
                        }
                        else
                        {
                            if (column.NotNull)
                            {
                                data_type = "int";
                            }
                            else
                            {
                                data_type = "int?";
                            }
                        }
                    }
                    else
                    {
                        if (column.NotNull)
                        {
                            data_type = "decimal";
                        }
                        else
                        {
                            data_type = "decimal?";
                        }
                    }
                    break;
                case "LONG":
                    if (column.NotNull)
                    {
                        data_type = "long";
                    }
                    else
                    {
                        data_type = "long?";
                    }
                    break;
                case "VARCHAR2":
                    data_type = "string";
                    break;
                case "NVARCHAR2":
                    data_type = "string";
                    break;
                case "CHAR":
                    data_type = "string";
                    break;
                case "DATE":
                    if (column.NotNull)
                    {
                        data_type = "DateTime";
                    }
                    else
                    {
                        data_type = "DateTime?";
                    }
                    break;
                case "CLOB":
                    data_type = "string";
                    break;
                case "BLOB":
                    data_type = "string";
                    break;
                case "TIMESTAMP(1)":
                case "TIMESTAMP(2)":
                case "TIMESTAMP(3)":
                case "TIMESTAMP(4)":
                case "TIMESTAMP(5)":
                case "TIMESTAMP(6)":
                case "TIMESTAMP(7)":
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
