using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelGenerator.Models;
using System.Configuration;
using DBUtil;
using System.Data;

namespace ModelGenerator.DAL
{
    /// <summary>
    /// PostgreSQL数据库Dal
    /// </summary>
    public class PostgreSQLDal : IDal
    {
        #region 获取所有表信息
        /// <summary>
        /// 获取所有表信息
        /// </summary>
        public List<DBTable> GetAllTables()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["PostgreSQLConnection"].ToString().ToLower();
            int start = connectionString.IndexOf("database=") + 9;
            int end = connectionString.IndexOf("user id=");
            string owner = connectionString.Substring(start, end - start).Replace(";", "").ToLower();
            PostgreSQLHelper dbHelper = new PostgreSQLHelper();
            DataTable dt = dbHelper.Query(string.Format(@"
                select a.schemaname, a.tablename, d.description
                from pg_tables a
                inner join pg_namespace b on b.nspname = a.schemaname
                inner join pg_class c on c.relnamespace = b.oid and c.relname = a.tablename
                left join pg_description d on d.objoid = c.oid and objsubid = 0
                where a.schemaname not in ('pg_catalog', 'information_schema', 'topology')
                and (b.nspname || '.' || a.tablename) not in ('public.spatial_ref_sys')
                and tableowner= '{0}'", owner));

            List<DBTable> result = new List<DBTable>();
            foreach (DataRow dr in dt.Rows)
            {
                DBTable dbTable = new DBTable();
                dbTable.TableName = dr["tablename"].ToString();
                dbTable.Comments = dr["description"].ToString();
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
            string connectionString = ConfigurationManager.ConnectionStrings["PostgreSQLConnection"].ToString().ToLower();
            int start = connectionString.IndexOf("database=") + 9;
            int end = connectionString.IndexOf("user id=");
            string owner = connectionString.Substring(start, end - start).Replace(";", "").ToUpper();
            string schema = "public";
            PostgreSQLHelper dbHelper = new PostgreSQLHelper();
            DataTable dt = dbHelper.Query(string.Format(@"
                select 
                pcolumn.column_name as COLUMN_NAME, 
                pcolumn.udt_name as COLUMN_TYPE,
                col_description(pclass.oid, pcolumn.ordinal_position) as COLUMN_COMMENT,
                CASE WHEN pcolumn.numeric_scale > 0 THEN pcolumn.numeric_precision ELSE pcolumn.character_maximum_length END as LENGTH,
                pcolumn.column_default as DefaultValue,
                pcolumn.numeric_scale as NUMERIC_SCALE,
                case when pkey.colname = pcolumn.column_name then true else false end as IsPrimaryKey,
                case when pcolumn.column_default like 'nextval%' then true else false end as IsIdentity,
                pcolumn.is_nullable as IS_NULLABLE
                
                from (
                    select * 
                    from pg_tables 
                    where upper(tablename) = upper('{0}') and schemaname='{1}'
                ) ptables 
                inner join pg_class pclass on ptables.tablename = pclass.relname 
                inner join (
                    SELECT *
                    FROM information_schema.columns
                ) pcolumn on pcolumn.table_name = ptables.tablename
                left join (
	                select  pg_class.relname, pg_attribute.attname as colname 
                    from pg_constraint  
                    inner join pg_class on pg_constraint.conrelid = pg_class.oid 
	                inner join pg_attribute on pg_attribute.attrelid = pg_class.oid and  pg_attribute.attnum = pg_constraint.conkey[1]
	                inner join pg_type on pg_type.oid = pg_attribute.atttypid
	                where pg_constraint.contype='p'
                ) pkey on pcolumn.table_name = pkey.relname ", tableName, schema));

            List<DBColumn> result = new List<DBColumn>();
            foreach (DataRow dr in dt.Rows)
            {
                DBColumn column = new DBColumn();
                column.ColumnName = dr["COLUMN_NAME"].ToString();
                column.NotNull = dr["IS_NULLABLE"].ToString() == "NO" ? true : false;
                column.Comments = dr["COLUMN_COMMENT"].ToString();
                column.DataType = dr["COLUMN_TYPE"].ToString();
                column.DataScale = dr["LENGTH"].ToString();
                column.DataPrecision = dr["NUMERIC_SCALE"].ToString();
                if (dr["IsPrimaryKey"].ToString().ToLower() == "true")
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
                case "001":
                case "int4":
                case "integer":
                    if (column.NotNull)
                    {
                        data_type = "int";
                    }
                    else
                    {
                        data_type = "int?";
                    }
                    break;
                case "int8":
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
                case "float4":
                case "float8":
                case "003":
                case "004":
                case "money":
                case "numeric":
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
                case "005":
                    data_type = "string";
                    break;
                case "006":
                case "007":
                    data_type = "byte[]";
                    break;
                case "008":
                case "date":
                case "009":
                case "daterange":
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
