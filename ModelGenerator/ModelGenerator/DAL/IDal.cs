using ModelGenerator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGenerator.DAL
{
    /// <summary>
    /// 数据库操作接口
    /// </summary>
    public interface IDal
    {
        /// <summary>
        /// 获取数据库名
        /// </summary>
        List<DBTable> GetAllTables();
        /// <summary>
        /// 获取表的所有字段名及字段类型
        /// </summary>
        List<DBColumn> GetAllColumns(string tableName);
        /// <summary>
        /// 类型转换
        /// </summary>
        string ConvertDataType(DBColumn column);
    }
}
